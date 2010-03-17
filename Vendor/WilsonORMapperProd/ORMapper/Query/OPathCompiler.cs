//-------------------------------------------------
// OPath Query Engine
// Written by Jeff Lanning (jefflanning@gmail.com)
// Modeled after SDK for Longhorn CTP Build 4074
// Version 1: Dec 2004 - May 2005
//-------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using Wilson.ORMapper.Internals;

namespace Wilson.ORMapper.Query
{
	internal class OPathCompiler
	{
		private Mappings _maps;
		private CustomProvider _provider;
		private string _sqlQuery;
		private OPathParameterTable _parameterTable;

		// used during SQL query generation
		private static readonly char[] ALIAS_LIST = new char[] {'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z'};
		private int _nextAliasIndex;
		private OrderBy _orderByNode = null;  // holds the OrderBy node (which we run into first thing) until we get a chance to generate the clause
		private Hashtable _lookupFieldToAliasMap = new Hashtable(); // maps LookupMap fields to table alias (FieldAlias is used as the key)

		public OPathCompiler()
		{
		}

		public string SqlQuery
		{
			get { return _sqlQuery; }
		}

		public OPathParameterTable ParameterTable
		{
			get { return _parameterTable; }
		}


		public void Compile(ObjectExpression oe)
		{
			_maps = oe.Mappings;
			_provider = oe.Mappings.provider;

			_nextAliasIndex = 0;
			_parameterTable = new OPathParameterTable();

			// generate the database-specific query statement (and the parameter table at the same time)
			using( StringWriter writer = new StringWriter(new StringBuilder(500)) )
			{
				WriteSqlQuery(writer, oe.Expression);
				writer.Write(_provider.LineTerminator);
				_sqlQuery = writer.ToString();
			}

			SetParameterOrder(oe);

			// all or none of the parameters have to be provided
			if( _parameterTable != null )
			{
				if( _parameterTable.Count != oe.ParameterCount )
				{
					throw new Exception("Number of parameters in the expression does not match number of parameters in the OPathQuery.");
				}
			}

#if DEBUG
			//Debug.WriteLine(_sqlQuery);
			//Debug.WriteLine("");
#endif
		}


		private void SetParameterOrder(ObjectExpression oe)
		{
			if( oe.ParameterCount == 0 ) return;
			
			if( _provider.Provider == Provider.Access || _provider.Provider == Provider.OleDb )
			{
				// the OleDb driver expects the order of parameters to be specified in "subquery major" order (which I consider a *major* design flaw).
				// in other words, the order is driven by a post traversal of the select statements (filters), with standard left-to-right order within a given select.
				Expression.EnumNodesCallBack postCallback = new Expression.EnumNodesCallBack(this.OleDbSetParameterOrder);
				Expression.EnumNodes(oe.Expression, null, postCallback, 0);
			}
			else // not an OleDb provider
			{
				// use standard right-to-left order (which the parameters are already in)
				for( int i = _parameterTable.Count - 1; i >= 0; i-- )
				{
					_parameterTable[i].Ordinal = i;
				}
			}
		}

		private bool OleDbSetParameterOrder(Expression node, object[] args)
		{
			if( node.NodeType == NodeType.Filter )
			{
				// the parameters in this filter are the next in sequence
				Filter filter = (Filter)node;
				Expression.EnumNodesCallBack callback = new Expression.EnumNodesCallBack(this.OleDbSetParameterOrderInFilter);
				Expression.EnumNodes(filter.Source, callback, null, args);
				Expression.EnumNodes(filter.Constraint, callback, null, args);
			}
			return true;
		}

		private bool OleDbSetParameterOrderInFilter(Expression node, object[] args)
		{
			if( node.NodeType == NodeType.Filter )
			{
				// prevent enumerator from traversing child filters
				return false;
			}
			else if( node.NodeType == NodeType.Parameter )
			{
				int index = (node as Query.Parameter).Ordinal;
				int nextOrdinal = (int)args[0];
				_parameterTable[index].Ordinal = nextOrdinal;
				args[0] = nextOrdinal + 1;
			}
			return true;
		}


		private void WriteSqlQuery(TextWriter w, Expression expr)
		{
			switch( expr.NodeType )
			{
				case NodeType.Property:
				{
					Property property = (Property)expr;
					
					EntityMap map = _maps[property.OwnerClass];
					FieldMap field = map.GetFieldMap(property.Name);
					if( field == null )
					{
						throw new Exception("Property '" + property.Name + "' could not be found for entity type '" + property.OwnerClass + "'.");
					}

					string alias;
					if( !field.IsLookup )
					{
						alias = GetAlias(property);
					}
					else // lookup field
					{
						LookupMap lookup = (LookupMap)field;
						alias = (string)_lookupFieldToAliasMap[lookup.FieldAlias];
					}
					if( alias == null )
					{
						throw new Exception("Could not find table alias for property '" + property.Name + "' in entity '" + property.OwnerClass + "'.");
					}

					WriteSqlColumn(w, alias, field.Field);
					return;
				}
				case NodeType.Parameter:
				{
					Query.Parameter node = (Query.Parameter)expr;

					string name;
					if( !_provider.NoNamedParameters )
					{
						name = _provider.ParameterPrefix + "P" + node.Ordinal;
					}
					else
					{
						name = "?";
					}

					_parameterTable.Add( new OPathParameter(name, node.ValueType) );
	
					w.Write(name);
					return;
				}
				case NodeType.Literal:
				{
					Literal node = (Literal)expr;
					WriteLiteral(w, node.Value);
					return;
				}
				case NodeType.Binary:
				{
					Binary node = (Binary)expr;

					bool isFormat;
					string keyword = GetSqlKeyword(node.Operator, out isFormat);

					if( isFormat )
					{
						WriteFormat(w, keyword, node.Left, node.Right);
					}
					else
					{
						w.Write('(');
						WriteSqlQuery(w, node.Left);
						w.Write(' ');
						w.Write(keyword);
						w.Write(' ');
						WriteSqlQuery(w, node.Right);
						w.Write(')');
					}
					return;
				}
				case NodeType.Unary:
				{
					Unary node = (Unary)expr;

					if( node.Operator == UnaryOperator.Exists )
					{
						w.Write(GetSqlKeyword(node.Operator));
						w.Write("\n(\n");
						WriteSqlQuery(w, node.Operand);
						w.Write("\n)");
					}
					else if( node.Operator == UnaryOperator.IsNull )
					{
						w.Write('(');
						WriteSqlQuery(w, node.Operand);
						w.Write(' ');
						w.Write(GetSqlKeyword(node.Operator));
						w.Write(')');
					}
					else
					{
						w.Write(GetSqlKeyword(node.Operator));
						w.Write('(');
						WriteSqlQuery(w, node.Operand);
						w.Write(')');
					}
					return;
				}
				case NodeType.Filter:
				{
					Filter filter = (Filter)expr;

					// the only supported filters are the root and ones directly below an exists node
					if( filter.Owner != null && (filter.Owner.NodeType != NodeType.Unary || (filter.Owner as Unary).Operator != UnaryOperator.Exists) )
					{
						throw new NotSupportedException("Filter with source type '" + filter.Source.NodeType + "' was not expected.");
					}

					EntityMap entity = _maps[filter.ValueType];
					string entityAlias = GetNextAlias();
					filter.Alias = entityAlias;

					bool whereStarted = false;
					if( filter.Source.NodeType == NodeType.TypeFilter ) // root filter/outer select
					{
						Join[] lookupJoins;
						WriteSelectClause(w, entity, entityAlias, out lookupJoins);

						Join[] joins;
						if( _orderByNode != null )
						{
							Join[] orderByJoins = GetJoinsFromOrderBy(entity, entityAlias, _orderByNode);
							joins = new Join[lookupJoins.Length + orderByJoins.Length];
							lookupJoins.CopyTo(joins, 0);
							orderByJoins.CopyTo(joins, lookupJoins.Length);
						}
						else
						{
							joins = lookupJoins;
						}

						WriteFromClause(w, entity.Table, entityAlias, joins);
						if( entity.BaseEntity != null )
						{
							WriteSubEntityConstraint(w, entityAlias, entity.TypeField, entity.TypeValue, ref whereStarted);
						}
						WriteFilterConstraints(w, filter, whereStarted);
						
						// add the default sort order if the entity has one defined but the query is not ordered
						if( _orderByNode == null && entity.SortOrder != null && entity.SortOrder.Length > 0 )
						{
							w.Write("\nORDER BY ");
							w.Write(entity.SortOrder);
						}
					}
					else if( filter.Source.NodeType == NodeType.Property || filter.Source.NodeType == NodeType.Filter ) // nested filter/subquery
					{
						Expression source = filter.Source;
						while( source.NodeType == NodeType.Filter )
						{
							source = (source as Filter).Source;
						}
						if( source.NodeType != NodeType.Property ) throw new Exception("Could not find source property for filter.");

						Property relProperty = (Property)source;
						if( !relProperty.IsRelational ) throw new Exception("Expected source property for Filter node to be relational.");

						RelationMap relation = relProperty.RelationMap;
						EntityMap sourceEntity = _maps[relProperty.OwnerClass];
						string sourceAlias = GetAlias(relProperty);

						string relationConstraint = ReplaceTableNameWithAlias(relation.Filter, sourceEntity.Table, sourceAlias);
						relationConstraint = ReplaceTableNameWithAlias(relationConstraint, entity.Table, entityAlias);
						
						switch( relation.Relationship )
						{
							case Relationship.Parent:
							{
								w.Write("SELECT *");
								WriteFromClause(w, entity.Table, entityAlias, null);
								if( entity.BaseEntity != null )
								{
									WriteSubEntityConstraint(w, entityAlias, entity.TypeField, entity.TypeValue, ref whereStarted);
								}
								WriteJoinCondition(w, sourceAlias, relation.Fields, entityAlias, GetFieldNames(entity.KeyFields), relationConstraint, true, ref whereStarted);
								WriteFilterConstraints(w, filter, whereStarted);
								break;
							}
							case Relationship.Child:
							{
								w.Write("SELECT *");
								WriteFromClause(w, entity.Table, entityAlias, null);
								if( entity.BaseEntity != null )
								{
									WriteSubEntityConstraint(w, entityAlias, entity.TypeField, entity.TypeValue, ref whereStarted);
								}
								WriteJoinCondition(w, sourceAlias, GetFieldNames(sourceEntity.KeyFields), entityAlias, relation.Fields, relationConstraint, true, ref whereStarted);
								WriteFilterConstraints(w, filter, whereStarted);
								break;
							}
							case Relationship.Many:
							{
								ManyMap junctionMap = (ManyMap)relation;
								string junctionAlias = GetNextAlias();

								relationConstraint = ReplaceTableNameWithAlias(relationConstraint, junctionMap.Table, junctionAlias);								

								// write the junction and child table, inner joined together in one select
								w.Write("SELECT *");
								WriteFromClause(w, junctionMap.Table, junctionAlias, null);
								w.Write(", ");
								WriteSqlTable(w, entity.Table, entityAlias);
								if( entity.BaseEntity != null )
								{
									WriteSubEntityConstraint(w, entityAlias, entity.TypeField, entity.TypeValue, ref whereStarted);
								}
								WriteJoinCondition(w, sourceAlias, GetFieldNames(sourceEntity.KeyFields), junctionAlias, junctionMap.Source, null, true, ref whereStarted);
								WriteJoinCondition(w, junctionAlias, junctionMap.Dest, entityAlias, GetFieldNames(entity.KeyFields), relationConstraint, true, ref whereStarted);
								WriteFilterConstraints(w, filter, whereStarted);

								break;
							}
							default:
							{
								throw new NotSupportedException("Relationship type '" + relation.Relationship + "' is not supported.");
							}
						}
					}
					else
					{
						throw new NotImplementedException("Filter with source type '" + filter.Source.NodeType + "' was not expected.");
					}
					return;
				}
				case NodeType.Function:
				{
					Function node = (Function)expr;

					bool isFormat;
					string keyword = GetSqlKeyword(node.Operator, out isFormat);
					if( isFormat )
					{
						WriteFormat(w, keyword, node.Params);
					}
					else
					{
						if( node.Operator != FunctionOperator.In )
						{
							throw new NotSupportedException("Function operator '" + node.Operator + "' was not expected.");
						}
						w.Write('(');
						WriteSqlQuery(w, node.Params[0]);
						w.Write(' ');
						w.Write(keyword);
						w.Write(" (");
						for( int i = 1; i < node.Params.Length; i++ )
						{
							if( i > 1 ) w.Write(", ");
							WriteSqlQuery(w, node.Params[i]);
						}
						w.Write("))");
					}
					return;
				}
				case NodeType.OrderBy:
				{
					OrderBy node = (OrderBy)expr;
					_orderByNode = node;

					Filter filter = (node.Source as Filter);
					if( filter == null )
					{
						throw new Exception("Expected source of OrderBy node to be Filter.");
					}

					WriteSqlQuery(w, filter);

					w.Write("\nORDER BY ");
					for( int i = 0; i < node.OrderByItems.Count; i++ )
					{
						OrderByItem item = node.OrderByItems[i];

						FieldMap field = item.FieldInfo;
						if( field == null )
						{
							throw new Exception("Field info does not exists for OrderByItem '" + item.Item + "'.");
						}

						string alias = (item.Join == null) ? filter.Alias : item.Join.Alias;

						if( i > 0 )	w.Write(", ");
						WriteSqlColumn(w, alias, field.Field);
						w.Write( (item.Ascending) ? " ASC" : " DESC");
					}
					return;
				}
				case NodeType.Empty:
				{
					return;
				}
				default:
				{
					throw new NotSupportedException("Expression type '" + expr.GetType() + "' is not currently supported.");
				}
			}
		}


		private void WriteSelectClause(TextWriter w, EntityMap entity, string entityAlias, out Join[] lookupJoins)
		{
			w.Write("SELECT ");

			int columnCount = 0;
			ArrayList joins = new ArrayList();
			ArrayList joinMaps = new ArrayList();

			// add all core entity fields and lookup columns
			for( int i = 0; i < entity.Fields.Length; i++ )
			{
				if( columnCount > 0 ) w.Write(", ");

				FieldMap field = entity.Fields[i];
				if( !field.IsLookup )
				{
					WriteSqlColumn(w, entityAlias, field.Field);
				}
				else // lookup column
				{
					LookupMap lookup = (LookupMap)field;

					// see if we already have a join setup for the table associated to this lookup field
					string tableAlias = null;
					for( int j = 0; j < joinMaps.Count; j++ )
					{
						if( CanLookupMapsUseSameJoin((LookupMap)joinMaps[j], lookup) )
						{
							tableAlias = (joins[j] as Join).TableAlias;
							break;
						}
					}

					// create a new join if an existing one was not found above
					if( tableAlias == null )
					{
						tableAlias = GetNextAlias();
						
						StringWriter joinCondition = new StringWriter();
						bool whereStarted = false;
						WriteJoinCondition(joinCondition, entityAlias, lookup.Source, tableAlias, lookup.Dest, null, false, ref whereStarted);

						joins.Add( new Join(lookup.Table, tableAlias, joinCondition.ToString(), 1) );
						joinMaps.Add(lookup);
					}

					WriteSqlColumn(w, tableAlias, lookup.Field, lookup.FieldAlias);

					// add lookup to the map - in case we need the alias later during query generation
					_lookupFieldToAliasMap.Add(lookup.FieldAlias, tableAlias);
				}
				
				columnCount += 1;
			}

			// add any sub fields
			if( entity.SubFields.Keys.Count > 0 )
			{
				foreach( string subField in entity.SubFields.Keys )
				{
					if( columnCount > 0 ) w.Write(", ");
					WriteSqlColumn(w, entityAlias, subField);
					columnCount += 1;
				}
			}

			// add type field if specified
			if( entity.TypeField != null && entity.TypeField.Length > 0 )
			{
				if( columnCount > 0 ) w.Write(", ");
				WriteSqlColumn(w, entityAlias, entity.TypeField);
				columnCount += 1;
			}

			// return our array of joins
			lookupJoins = (Join[])joins.ToArray(typeof(Join));
		}

		private void WriteFromClause(TextWriter w, string tableName, string tableAlias, Join[] joins)
		{
			// write a SQL-92 From clause with the specified joins... unless the provider is Access
			if( _provider.Provider != Provider.Access )
			{
				// SQL-92 Example Output:
				// 
				// FROM Node n1
				//   INNER OUTER JOIN Node n2 ON n1.NodeID = n2.ParentNodeID
				//     INNER OUTER JOIN Node n4 ON n2.NodeID = n4.ParentNodeID
				//     INNER OUTER JOIN Node n5 ON n2.NodeID = n5.ParentNodeID
				//   INNER OUTER JOIN Node n3 ON n1.NodeID = n3.ParentNodeID
				//     INNER OUTER JOIN Node n6 ON n3.NodeID = n6.ParentNodeID
				//     INNER OUTER JOIN Node n7 ON n3.NodeID = n7.ParentNodeID

				w.Write("\nFROM ");
				WriteSqlTable(w, tableName, tableAlias);

				if( joins != null )
				{
					for( int i = 0; i < joins.Length; i++ )
					{
						Join join = joins[i];

						w.Write('\n');
						w.Write(new string(' ', join.IndentDepth * 2));
						w.Write("LEFT OUTER JOIN ");
						WriteSqlTable(w, join.TableName, join.TableAlias);
						w.Write(" ON ");
						w.Write(join.JoinCondition);
					}
				}
			}
			else // MS Access
			{
				// MS Access Example Output:
				// 
				// FROM ((((((Node AS n1
				//   INNER JOIN Node AS n2 ON n1.NodeID = n2.ParentNodeID) 
				//     INNER JOIN Node AS n4 ON n2.NodeID = n4.ParentNodeID) 
				//     INNER JOIN Node AS n5 ON n2.NodeID = n5.ParentNodeID) 
				//   INNER JOIN Node AS n3 ON n1.NodeID = n3.ParentNodeID) 
				//     INNER JOIN Node AS n6 ON n3.NodeID = n6.ParentNodeID) 
				//     INNER JOIN Node AS n7 ON n3.NodeID = n7.ParentNodeID)

				w.Write("\nFROM ");

				if( joins != null && joins.Length > 0 )
				{
					w.Write(new string('(', joins.Length));
				}

				WriteSqlTable(w, tableName, tableAlias);

				if( joins != null )
				{
					for( int i = 0; i < joins.Length; i++ )
					{
						Join join = joins[i];

						w.Write('\n');
						w.Write(new string(' ', join.IndentDepth * 2));
						w.Write(" LEFT OUTER JOIN ");
						WriteSqlTable(w, join.TableName, join.TableAlias);
						w.Write(" ON ");
						w.Write(join.JoinCondition);
						w.Write(")");
					}
				}
			}
		}

		private void WriteSubEntityConstraint(TextWriter w, string tableAlias, string typeField, object typeValue, ref bool whereStarted)
		{
			if( !whereStarted )
			{
				w.Write("\nWHERE ");
				whereStarted = true;
			}
			else
			{
				w.Write(' ');
				w.Write(GetSqlKeyword(BinaryOperator.LogicalAnd));
				w.Write(' ');
			}

			w.Write('(');
			WriteSqlColumn(w, tableAlias, typeField);
			w.Write(' ');
			w.Write(GetSqlKeyword(BinaryOperator.Equality));
			w.Write(' ');
			WriteLiteral(w, typeValue);
			w.Write(')');
		}

		private void WriteJoinCondition(TextWriter w, string primaryAlias, string[] primaryKeyColumns, string foreignAlias, string[] foreignKeyColumns, string relationFilter, bool addingToWhere, ref bool whereStarted)
		{
			if( primaryKeyColumns.Length != foreignKeyColumns.Length )
			{
				throw new Exception("Primary and foreign key column counts do not match.");
			}
			if( primaryKeyColumns.Length == 0 )
			{
				throw new Exception("No join columns specified.");
			}

			for( int i = 0; i < primaryKeyColumns.Length; i++ )
			{
				if( !whereStarted )
				{
					if( addingToWhere )
					{
						w.Write("\nWHERE ");
					}
					whereStarted = true;
				}
				else
				{
					w.Write(' ');
					w.Write(GetSqlKeyword(BinaryOperator.LogicalAnd));
					w.Write(' ');
				}

				w.Write('(');
				WriteSqlColumn(w, primaryAlias, primaryKeyColumns[i]);
				w.Write(' ');
				w.Write(GetSqlKeyword(BinaryOperator.Equality));
				w.Write(' ');
				WriteSqlColumn(w, foreignAlias, foreignKeyColumns[i]);
				w.Write(')');
			}

			if( relationFilter != null && relationFilter.Length > 0 )
			{
				w.Write(' ');
				w.Write(GetSqlKeyword(BinaryOperator.LogicalAnd));
				w.Write(" (");
				w.Write(relationFilter);
				w.Write(')');
			}
		}

		private void WriteFilterConstraints(TextWriter w, Filter filter, bool whereStarted)
		{
			// add any source constraints in a nested filter
			if( filter.Source.NodeType == NodeType.Filter )
			{
				Filter sourceFilter = (Filter)filter.Source;
				sourceFilter.Alias = filter.Alias;

				if( !whereStarted )
				{
					w.Write("\nWHERE ");
					whereStarted = true;
				}
				else
				{
					w.Write(' ');
					w.Write(GetSqlKeyword(BinaryOperator.LogicalAnd));
					w.Write(' ');
				}

				WriteSqlQuery(w, sourceFilter.Constraint);
			}

			// add the filter constraints
			if( filter.Constraint.NodeType != NodeType.Empty )
			{
				if( !whereStarted )
				{
					w.Write("\nWHERE ");
					whereStarted = true;
				}
				else
				{
					w.Write(' ');
					w.Write(GetSqlKeyword(BinaryOperator.LogicalAnd));
					w.Write(' ');
				}

				WriteSqlQuery(w, filter.Constraint);
			}
		}

		
		private void WriteSqlTable(TextWriter w, string tableName, string alias)
		{
			// note: this handles fully-qualified table names (e.g., "Database.Owner.Table" and "Database..Table")
			string[] parts = tableName.Split('.');
			for( int i = 0; i < parts.Length; i++ )
			{
				if( i > 0 ) w.Write('.');
				if( parts[i].Length > 0 )
				{
					w.Write(_provider.StartDelimiter);
					w.Write(parts[i]);
					w.Write(_provider.EndDelimiter);
				}
			}
			
			w.Write(' '); //NOTE: Removed "AS" keyword since it's not supported by Oracle (and perhaps others)
			w.Write(alias);
		}

		private void WriteSqlColumn(TextWriter w, string alias, string columnName)
		{
			WriteSqlColumn(w, alias, columnName, null);
		}

		private void WriteSqlColumn(TextWriter w, string alias, string columnName, string columnAlias)
		{
			w.Write(alias);
			w.Write('.');
			w.Write(_provider.StartDelimiter);
			w.Write(columnName);
			w.Write(_provider.EndDelimiter);
			if( columnAlias != null )
			{
				if( _provider.ColumnAliasKeyword != null && _provider.ColumnAliasKeyword.Length > 0 )
				{
					w.Write(' ');
					w.Write(_provider.ColumnAliasKeyword);
					w.Write(' ');
				}
				w.Write(columnAlias);
			}
		}

		private void WriteLiteral(TextWriter w, object value)
		{
			if( value is string )
			{
				w.Write('\'');
				w.Write(value.ToString().Replace("'", "''"));
				w.Write('\'');
			}
			else if( value is bool )
			{
				w.Write( ((bool)value ? _provider.TrueLiteral : _provider.FalseLiteral) );
			}
			else if( value is DateTime )
			{
				w.Write(_provider.DateDelimiter);
				if( _provider.DateFormat != null && _provider.DateFormat.Length > 0 )
				{
					w.Write( ((DateTime)value).ToString(_provider.DateFormat) );
				}
				else
				{
					w.Write(value);
				}
				w.Write(_provider.DateDelimiter);
			}
			else if( value is Guid )
			{
				w.Write(_provider.GuidDelimiter);
				w.Write('{');
				w.Write(value);
				w.Write('}');
				w.Write(_provider.GuidDelimiter);
			}
			else
			{
				w.Write(value);
			}
		}

		private void WriteFormat(TextWriter w, string format, params Expression[] args)
		{
			//V2: for better perf, consider writing custom formatter which streams the args into the format; rather than caching strings

			string[] values = new string[args.Length];
			for( int i = 0; i < values.Length; i++ )
			{
				using( StringWriter sw = new StringWriter() )
				{
					WriteSqlQuery(sw, args[i]);
					values[i] = sw.ToString();
				}
			}

			w.Write(format, values);
		}


		private string GetNextAlias()
		{
			char alias = ALIAS_LIST[_nextAliasIndex % ALIAS_LIST.Length];
			_nextAliasIndex += 1;

			return new string(alias, (_nextAliasIndex / ALIAS_LIST.Length) + 1);
		}

		private string GetAlias(Property property)
		{
			Expression node = (property.Source as Query.Context).Link;
			while( node != null && node.NodeType != NodeType.Filter )
			{
				node = node.Owner;
			}
			if( node == null )
			{
				throw new Exception("Expected to find Filter in owner chain of property.");
			}
			return (node as Filter).Alias;
		}


		private string GetSqlKeyword(BinaryOperator op)
		{
			bool isFormat;
			string symbol = GetSqlKeyword(op, out isFormat);
			if( isFormat )
			{
				throw new ArgumentException("Binary operator " + op + " cannot be used with this overload beacuse a string format is returned.");
			}
			return symbol;
		}

		private string GetSqlKeyword(BinaryOperator op, out bool isFormat)
		{
			isFormat = false;
			switch( op )
			{
				case BinaryOperator.LogicalAnd: return "AND";
				case BinaryOperator.LogicalOr: return "OR";
				case BinaryOperator.Equality: return "=";
				case BinaryOperator.Inequality: return _provider.InequalityOperator;
				case BinaryOperator.LessThan: return "<";
				case BinaryOperator.LessEqual: return "<=";
				case BinaryOperator.GreaterThan: return ">";
				case BinaryOperator.GreaterEqual: return ">=";
				case BinaryOperator.Addition: return "+";
				case BinaryOperator.Subtraction: return "-";
				case BinaryOperator.Multiplication: return "*";
				case BinaryOperator.Division: return "/";
				case BinaryOperator.Modulus: isFormat = true; return _provider.ModulusFunction;
				case BinaryOperator.Concatenation: return _provider.ConcatenationOperator;
					//case BinaryOperator.InInterval: return "";
				default:
				{
					throw new NotSupportedException("Binary operator " + op + " is not supported.");
				}
			}
		}

		private string GetSqlKeyword(UnaryOperator op)
		{
			switch( op )
			{
				case UnaryOperator.Negation: return "-";
				case UnaryOperator.LogicalNot: return "NOT";
				case UnaryOperator.IsNull: return "IS NULL";
				case UnaryOperator.Exists: return "EXISTS";
				default:
				{
					throw new NotSupportedException("Unary operator " + op + " is not supported.");
				}
			}
		}

		private string GetSqlKeyword(FunctionOperator op, out bool isFormat)
		{
			isFormat = true;

			switch( op )
			{
				case FunctionOperator.Trim: return _provider.TrimFunction;
				case FunctionOperator.Len: return _provider.LengthFunction;
				case FunctionOperator.Left: return _provider.LeftFunction;
				case FunctionOperator.Right: return _provider.RightFunction;
				case FunctionOperator.Substring: return _provider.SubstringFunction;
				case FunctionOperator.UpperCase: return _provider.UpperCaseFunction;
				case FunctionOperator.LowerCase: return _provider.LowerCaseFunction;
				case FunctionOperator.Like: return _provider.LikeFunction;
				case FunctionOperator.In: isFormat = false; return "IN";
				default:
				{
					throw new NotSupportedException("Function operator " + op + " is not supported.");
				}
			}
		}


		private string[] GetFieldNames(FieldMap[] fields)
		{
			string[] fieldNames = new string[fields.Length];
			for( int i = 0; i < fields.Length; i++ )
			{
				fieldNames[i] = fields[i].Field;
			}
			return fieldNames;
		}


		private bool CanLookupMapsUseSameJoin(LookupMap a, LookupMap b)
		{
			if( string.Compare(a.Table, b.Table, true) != 0 )
			{
				return false;
			}

			if( a.Source.Length != b.Source.Length )
			{
				return false;
			}

			if( a.Dest.Length != b.Dest.Length )
			{
				return false;
			}

			for( int i = 0; i < a.Source.Length; i++)
			{
				if( string.Compare(a.Source[i], b.Source[i], true) != 0 )
				{
					return false;
				}
			}

			for( int i = 0; i < a.Dest.Length; i++)
			{
				if( string.Compare(a.Dest[i], b.Dest[i], true) != 0 )
				{
					return false;
				}
			}

			return true;
		}

		private Join[] GetJoinsFromOrderBy(EntityMap sourceEntity, string sourceAlias, OrderBy node)
		{
			// traverse the join tree, building a flat array of Join objects
			ArrayList joinList = new ArrayList();
			AddOrderByJoinsToList(joinList, sourceEntity, sourceAlias, node.OrderByJoins, 1);
			return (Join[])joinList.ToArray(typeof(Join));
		}

		private void AddOrderByJoinsToList(ArrayList joinList, EntityMap sourceEntity, string sourceAlias, OrderByJoinCollection joins, int indentDepth)
		{
			foreach( OrderByJoin join in joins )
			{
				RelationMap relation = join.RelationMap;
				if( relation.Relationship != Relationship.Parent )
				{
					throw new Exception("Relationship '" + relation.Alias + "' is not a ManyToOne relation.  Only ManyToOne relations are allowed in sort expressions.");
				}

				EntityMap joinEntity = _maps[relation.Type];
				join.Alias = GetNextAlias();
				
				string relationConstraint = ReplaceTableNameWithAlias(relation.Filter, sourceEntity.Table, sourceAlias);
				relationConstraint = ReplaceTableNameWithAlias(relationConstraint, joinEntity.Table, join.Alias);

				StringWriter joinCondition = new StringWriter();
				bool whereStarted = false;
				WriteJoinCondition(joinCondition, sourceAlias, relation.Fields, join.Alias, GetFieldNames(joinEntity.KeyFields), relationConstraint, false, ref whereStarted);

				joinList.Add( new Join(joinEntity.Table, join.Alias, joinCondition.ToString(), indentDepth));

				// traverse inner joins using a recursive call
				if( join.NestedJoins.Count > 0 )
				{
					AddOrderByJoinsToList(joinList, joinEntity, join.Alias, join.NestedJoins, indentDepth + 1);
				}
			}
		}


		private string ReplaceTableNameWithAlias(string value, string tableName, string tableAlias)
		{
			// NOTE:
			// SQL Server, Access, and Oracle all freak out when a table name is used to qualify a field 
			// but the table has been assigned an alias (which is 100% of the time for OPath).  We have to 
			// replace table names with aliases in order for the relationship filter expresions to be usable 
			// within the queries we generate.

			// don't bother processing the filter if there is no expression or table qualifer in the expression
			if( value == null || value.Length == 0 || value.IndexOf('.') < 0 )
			{
				return value;
			}

			// deal with fully-qualified table names being passed (e.g., "Database.Owner.Table" and "Database..Table")
			string[] parts = tableName.Split('.');
			if( parts.Length > 1 )
			{
				tableName = parts[parts.Length - 1];
			}

			// remove any qualifiers on the table name and get the start and end patterns
			string startPattern = null;
			if( _provider.StartDelimiter != null && _provider.StartDelimiter.Length > 0 )
			{
				tableName = tableName.Replace(_provider.StartDelimiter, null);
				startPattern = "(" + Regex.Escape(_provider.StartDelimiter) + ")?";
			}

			string endPattern = null;
			if( _provider.EndDelimiter != null && _provider.EndDelimiter.Length > 0 )
			{
				tableName = tableName.Replace(_provider.EndDelimiter, null);
				endPattern = "(" + Regex.Escape(_provider.EndDelimiter) + ")?";
			}

			// build table replacement regex pattern
			// example: (?<a>^|\W)(\[)?(?i:Table)(\])?\.
			// example: (?<a>^|\W)(?i:Table)\.
			string pattern = @"(?<a>^|\W)" + startPattern + "(?i:" + Regex.Escape(tableName) + ")" + endPattern + @"\.";
			
			// make the replacements and return the result
			return Regex.Replace(value, pattern, @"$1" + tableAlias + ".", RegexOptions.ExplicitCapture | RegexOptions.Compiled);
		}


		private class Join
		{
			public string TableName;
			public string TableAlias;
			public string JoinCondition;
			public int IndentDepth;

			public Join(string tableName, string tableAlias, string condition, int indentDepth)
			{
				this.TableName = tableName;
				this.TableAlias = tableAlias;
				this.JoinCondition = condition;
				this.IndentDepth = indentDepth;
			}
		}
	}
}

