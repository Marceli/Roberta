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

using Wilson.ORMapper.Internals;

namespace Wilson.ORMapper.Query
{
	internal class OPathSortParser
	{
		private Mappings _maps;
		private string _expression;
		private Stack _args = new Stack();
		private Stack _ops = new Stack();

		public OPathSortParser(Mappings maps)
		{
			_maps = maps;
		}

		public OrderBy Parse(Type type, string expression)
		{
			// Example expressions for an Employee query:
			// "FirstName, LastName"
			// "FirstName ASC, LastName DESC"
			// "Company.Name"
			// "Company.Name ASC"
			// "Company.Address.State ASC"
			// "Company.Name ASC, Company.Address.State ASC, FirstName ASC"
			// "Company.Name ASC, HomeAddress.ZipCode, Company.Address.State DESC, ZipCode"

			_expression = expression;

			EntityMap entity = _maps[type];
			if( entity == null )
			{
				throw new Exception("Type " + type + " does not have an entity mapping defined to the database.");
			}
		
			OrderByItemCollection items = new OrderByItemCollection();
			OrderByJoinCollection joins = new OrderByJoinCollection();

			using( StringReader reader = new StringReader(expression) )
			{
				OPathLexer lexer = new OPathLexer(reader);

				lexer.Lex();
				while( lexer.CurrentToken != null )
				{
					OrderByItem item = ParseItem(lexer, entity, joins, null);
					items.Add(item);
				}

				if( lexer.LastToken != null && lexer.LastToken.Type == TokenType.COMMA )
				{
					throw new OPathException("A trailing comma was detected in sort expression '" + expression + "'.");
				}
			}

			return new OrderBy(null, items, joins);
		}

		private OrderByItem ParseItem(OPathLexer lexer, EntityMap entity, OrderByJoinCollection joins, OrderByJoin parentJoin)
		{
			Yytoken token = lexer.CurrentToken;
			if( token.Type != TokenType.IDENT )
			{
				throw new OPathException("'" + token.Text + "' encountered where a property or relationship was expected in sort expression '" + _expression + "'.");
			}

			string propertyName = token.Text;

			token = lexer.Lex();
			if( token == null )
			{
				FieldMap field;
				try
				{
					field = entity.GetFieldMap(propertyName);
				}
				catch
				{
					throw new OPathException(string.Format("The specified property '{0}' in the sort '{1}' is not defined in the entity map for type '{2}'.", propertyName, _expression, entity.Type));
				}

				return new OrderByItem(propertyName, field, true, parentJoin);
			}
			if( token.Type != TokenType.PERIOD )
			{
				if( token.Type != TokenType.ASCEND && token.Type != TokenType.DESCEND && token.Type != TokenType.COMMA )
				{
					throw new OPathException("'" + token.Text + "' is not valid in sort expression '" + _expression + "'.");
				}

				FieldMap field;
				try
				{
					field = entity.GetFieldMap(propertyName);
				}
				catch
				{
					throw new OPathException(string.Format("The specified property '{0}' in the sort '{1}' is not defined in the entity map for type '{2}'.", propertyName, _expression, entity.Type));
				}

				bool ascending = (token.Type != TokenType.DESCEND);

				if( token.Type != TokenType.COMMA )
				{
					lexer.MoveToNext();
				}
				lexer.MoveToNext();

				return new OrderByItem(propertyName, field, ascending, parentJoin);
			}
			else // dot operator (.)
			{
				token = lexer.Lex();
				if( token == null )
				{
					throw new OPathException("End of expression encountered where a property or relationship was expected in sort expression '" + _expression + "'.");
				}
				if( token.Type != TokenType.IDENT )
				{
					throw new OPathException("'" + token.Text + "' encountered where a property or relationship was expected in sort expression '" + _expression + "'.");
				}

				RelationMap relation = entity.Relation(propertyName);
				if( relation == null )
				{
					throw new OPathException(string.Format("The specified relationship '{0}' in the sort expression '{1}' is not defined in the entity map for type '{2}'.", propertyName, _expression, entity.Type));
				}
				if( relation.Relationship != Relationship.Parent )
				{
					throw new Exception("Relationship '" + relation.Alias + "' is not a ManyToOne relation.  Only ManyToOne relations are allowed in sort expressions.");
				}

				EntityMap relEntity = _maps[relation.Type];

				OrderByJoin join = joins[propertyName];
				if( join == null )
				{
					join = new OrderByJoin(relation);
					joins.Add(join);
				}

				// recursive call
				return ParseItem(lexer, relEntity, join.NestedJoins, join);
			}
		}				 

		private Yytoken[] Lex(string value)
		{
			ArrayList tokens = new ArrayList();
			using( StringReader reader = new StringReader(value) )
			{
				OPathLexer lexer = new OPathLexer(reader);

				Yytoken token;
				while( (token = lexer.Lex()) != null )
				{
					tokens.Add(token);
				}
			}

			return (Yytoken[])tokens.ToArray(typeof(Yytoken));
		}
	}
}
