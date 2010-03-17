//**************************************************************//
// Paul Wilson -- www.WilsonDotNet.com -- Paul@WilsonDotNet.com //
// Feel free to use and modify -- just leave these credit lines //
// I also always appreciate any other public credit you provide //
//**************************************************************//
using System;
using System.Text.RegularExpressions;
using Wilson.ORMapper.Internals;

namespace Wilson.ORMapper
{
	/// <summary>
	///     The QueryHelper class helps build provider-specific expressions
	///     for query where-clauses and sort-order field-names.
	/// </summary>
	/// <example>The following example shows how to use the QueryHelper to generate
	/// the where clause for getting all contacts who's company is WilsonDotNet.com.
	///	<code>
	///	public static ObjectSpace Manager; // See Initialization Example
	///	
	/// QueryHelper helper = Manager.QueryHelper;
	/// string where = helper.GetExpression("Contact.Company", "WilsonDotNet.com");
	/// string sort = helper.GetFieldName("Contact.Name") + " ASC";
	/// 
	///	ObjectQuery query = new ObjectQuery(typeof(Contact), where, sort);
	///	ObjectSet pageContacts = Manager.GetObjectSet(query);
	///	</code>
	/// </example>
	[Serializable()]
	public class QueryHelper
	{
		private Mappings mappings;

		/// <summary>
		///		Returns the database provider-specific field-name, like [TableName].[FieldName],
		///		given an entity-alias object description, like ClassName.PropertyName.
		///	</summary>
		/// <param name="entityAlias">The entity alias to lookup</param>
		/// <returns>The database provider-specific field-name</returns>
		public string GetFieldName(string entityAlias) {
			try {
				int lastDot = entityAlias.LastIndexOf(".");
				string entityName = entityAlias.Substring(0, lastDot);
				string aliasName = entityAlias.Substring(lastDot + 1);
				return this.mappings.GetCommands(entityName).GetFieldName(aliasName);
			}
			catch (Exception exception) {
				throw new MappingException("QueryHelper: EntityAlias was not Found - " + entityAlias, exception);
			}
		}

		/// <summary>
		///		Returns the database provider-specific table-name, like [TableName],
		///		given an entity-type description, like ClassName or Namespace.ClassName.
		///	</summary>
		/// <param name="entityType">The entity type to lookup</param>
		/// <returns>The database provider-specific table-name</returns>
		public string GetTableName(string entityType) {
			return this.mappings.GetCommands(entityType).GetTableName();
		}

		/// <summary>
		///		Returns the database provider-specific expression, like
		///		[TableName].[FieldName] = 'Value', given an entity-alias
		///		object description, like ClassName.PropertyName, and value
		///	</summary>
		/// <param name="entityAlias">The entity alias to use in the expression</param>
		/// <param name="fieldValue">The value to use in the expression</param>
		/// <returns>The database provider-specific expression</returns>
		public string GetExpression(string entityAlias, object fieldValue) {
			return GetExpression(entityAlias, fieldValue, ComparisonOperators.Equals);
		}
		
		// Added by Paul Welter (http://www.LoreSoft.com)
		/// <summary>
		///		Returns the database provider-specific expression, like
		///		[TableName].[FieldName] = 'Value', given an entity-alias
		///		object description, like ClassName.PropertyName, and value
		///	</summary>
		/// <param name="entityAlias">The entity alias to use in the expression</param>
		/// <param name="fieldValue">The value to use in the expression</param>
		/// <param name="comparison">The comparison operator to use with this expression</param>
		/// <returns>The database provider-specific expression</returns>
		public string GetExpression(string entityAlias, object fieldValue, ComparisonOperators comparison) {
			string fieldName = this.GetFieldName(entityAlias);
			if (fieldValue == null || fieldValue == DBNull.Value) {
				switch (comparison) {
					case ComparisonOperators.IsNotNull: return fieldName + " IS NOT NULL";
					default : return fieldName + " IS NULL";
				}
			}
			else {
				string value = this.CleanValue(fieldValue);
				switch (comparison) {
					case ComparisonOperators.GreaterThan: return fieldName + " > " + value;
					case ComparisonOperators.GreaterThanEqual: return fieldName + " >= " + value;
					case ComparisonOperators.IsNotNull: return fieldName + " IS NOT NULL";
					case ComparisonOperators.IsNull: return fieldName + " IS NULL";
					case ComparisonOperators.LessThan: return fieldName + " < " + value;
					case ComparisonOperators.LessThanEqual: return fieldName + " <= " + value;
					case ComparisonOperators.Like: return fieldName + " LIKE " + value;
					case ComparisonOperators.NotEqual: return fieldName + " <> " + value;
					default: return fieldName + " = " + value;
				}
			}
		}

		/// <summary>
		///		Returns the database provider-specific expression from an OPath syntax
		/// </summary>
		/// <param name="opathClause">The OPath expression</param>
		/// <remarks>
		///		OPath Syntax Converter provided by Oakleaf Enterprises (Mere Mortals .NET Framework)
		///		Kevin McNeish, Rick Strahl, John Miller, and Jason Mesches (http://www.OakleafSD.com)
		/// </remarks>
		/// <returns>The database provider-specific expression</returns>
		[Obsolete("Use the newer full OPath Engine instead -- see the OPathQuery and OPathQuery<T> classes.")]
		public string GetExpression(string opathClause) {
			string CleanExpression = opathClause;
			
			//Substitute OPath-specific operators for those recognized
			//	by SQL engine.
			CleanExpression = CleanExpression.Replace("&&", "and");
			CleanExpression = CleanExpression.Replace("||", "or");
			CleanExpression = CleanExpression.Replace("!=", "<>");
			CleanExpression = CleanExpression.Replace("!", "not");
			CleanExpression = CleanExpression.Replace("==", "=");

			//If there's a compound expression
			//	(e.g. Orders[Freight > 5].Details.Quantity > 50) need to
			//	parse it now.  (The example translates to Order.Freight > 5
			//	AND Details.Quantity > 50)
			CleanExpression = CleanExpression.Replace("].", "] and ");
			
			//For now, we'll use the pattern that all left brackets "["
			//	will be replaced by "." and all right brackets "]" will
			//	be removed.  Relationship hierarchies are addressed below
			CleanExpression = CleanExpression.Replace("[", ".");
			CleanExpression = CleanExpression.Replace("]", "");

			//VB style date replacement
			CleanExpression = Regex.Replace(CleanExpression,
				@"#(?<month>\d{1,2})/(?<day>\d{1,2})/(?<year>\d{2,4})#",
				"'${year}-${month}-${day}'");
			
			//Change "isnull(field)" to "field is null"
			//	Added "(" to the test just to be sure we didn't catch
			//	a field that for some unfortunate reason has "isnull"
			//	embedded in its name.
			//  Also decided to run Regex replace instead of string replace
			//	because string.Replace has no functionality for case
			//	insensitivity (ISNULL vs. isnull vs IsNull).
			//	Regex does, with the (?i) inline character.
			CleanExpression = Regex.Replace(CleanExpression,
				@"\b(?i)isnull\(\s*(?<field>\w*)\s*\)", "${field} is null");

			//Change "trim(field)" to "ltrim(rtrim(field))"
			CleanExpression = Regex.Replace(CleanExpression,
				@"\b(?i)trim\((?<alias>\w*\)\s*)", "ltrim(rtrim(${alias})");

			//IIF statements need to transformed into CASE statements.
			//	This pattern will help with any embedded parentheses pairs
			//	(e.g. IIF(TRIM(field), 'true', 'false')
			//	Explained http://www.oreilly.com/catalog/regex2/
			Regex RegExpr =
				new Regex(@"(?i)iif\((?>[^()]+|\((?<open>)|\)(?<-open>))*(?(open)(?!))\)");
			Match Matches;
			for (Matches = RegExpr.Match(CleanExpression); Matches.Success;
				Matches = RegExpr.Match(CleanExpression)) {
				CleanExpression = CleanExpression.Replace(Matches.Value,
					Regex.Replace(Matches.Value,
					@"(?i)iif\((?<eval>[^\,]*)\,(?<pos>[^\,]*)\,(?<neg>[^\)]*)\)",
					"CASE WHEN ${eval} THEN ${pos} ELSE ${neg} END"));
			}

			//Translate Parent-relationship operators
			//	(e.g. Directors.Managers.Employees[^.^.DirectorID = 12] refers
			//	to the DirectorID property of Directors and should resolve to
			//	Directors.DirectorID = 12)
			RegExpr = new Regex(@"\w+\.\^\.");
			for (Matches = RegExpr.Match(CleanExpression); Matches.Success;
				Matches = RegExpr.Match(CleanExpression)) {
				CleanExpression =
					CleanExpression.Replace(Matches.Value, System.String.Empty);
			}

			//Remove all parent relationship qualifiers from the expression
			//	(Grandparent.Parent.Child.Alias becomes Child.Alias)
			//	Since the "final" entity.alias clause is at the end, use the
			//	RightToLeft option.  Have to create the Regex object for that option,
			//	since there's no inline capability.
			RegExpr = new Regex(@"(\w+\.+)(?<entityalias>\w+\.\w+)", RegexOptions.RightToLeft);

			for (Matches = RegExpr.Match(CleanExpression); Matches.Success;
				Matches = Matches.NextMatch()) {
				//Here, the expression we're looking for is the 3rd match...
				//  The first is the entire expression, the second is
				//	the first grouped expression match (\w+\.+), which
				//	leaves the 3rd as the last word.word combination.
				CleanExpression = CleanExpression.Replace(Matches.Value,
					Matches.Groups[2].ToString());
			}

			//At this point, we should have a simple, clean expression from which
			//	we can gather all Entity/Alias combos and pass them to QueryHelper's
			//	GetFieldName() to derive the table/field names for the query.
			RegExpr = new Regex(@"\w+\.\w+");

			string FieldName;

			for (Matches = RegExpr.Match(CleanExpression); Matches.Success;
				Matches = Matches.NextMatch()) {
				try {
					//Look up the Entity.Alias in the schema
					//	and replace it in the string for the
					//	SQL'able Table.FieldName
					FieldName = this.GetFieldName(Matches.Value);
					CleanExpression = CleanExpression.Replace(Matches.Value, FieldName);
				}
				catch(MappingException e) {

					//QueryHelper throws a "mapping exception" if
					//	the entity/alias combination can't be found
					//	in the schema.  We'll just eat the exception
					//	and continue.  Remember, we've cleaned up
					//	quite a bit of the potential ugliness that was
					//	passed in to this method.  The part(s) of the
					//	expression that couldn't be found in the schema
					//	will remain unchanged in the returned string.
					string message = e.Message;
				}
			}
			return CleanExpression;
		}

		/// <summary>
		/// Format a raw value as a string for SQL
		///   and to protect against SQL injection
		/// </summary>
		/// <param name="value">User entered raw value</param>
		/// <returns>String formatted value for SQL</returns>
		public string CleanValue(object value) {
			if (value is string) {
				return "'" + value.ToString().Replace("'", "''") + "'";
			}
			else if (value is DateTime) {
				string DD = this.mappings.provider.DateDelimiter;
				string format = this.mappings.provider.DateFormat;
				if (format != null && format.Length > 0) {
					return DD + ((DateTime)value).ToString(format) + DD;
				}
				else {
					return DD + value.ToString() + DD;
				}
			}
			else if (value is Guid) {
				string GD = this.mappings.provider.GuidDelimiter;
				return GD + "{" + value.ToString() + "}" + GD;
			}
			else if (value is bool) {
				return (((bool)value) ? this.mappings.provider.TrueLiteral : this.mappings.provider.FalseLiteral);
			}
			else {
				return value.ToString();
			}
		}

		/// <summary>
		/// Convert a value to another type -- supports Nullable types, unlike Convert.ChangeType.
		/// </summary>
		/// <param name="value">Value to be converted -- must be a value type.</param>
		/// <param name="type">Type to convert value to -- must be a value type.</param>
		/// <returns></returns>
		static public object ChangeType(object value, Type type) {
#if DOTNETV2
			if (value == null && type.IsGenericType) return Activator.CreateInstance(type);
#endif
			if (value == null) return null;
			if (type == value.GetType()) return value;
			// Includes Support for Enumerated Member Types from Jerry Shea (http://www.RenewTek.com)
			if (type.IsEnum) {
				if (value is string) 
					return Enum.Parse (type, value as string); // RTS/GOP
				else
					return Enum.ToObject(type, value);
			}
#if DOTNETV2
			if (!type.IsInterface && type.IsGenericType) {
				Type innerType = type.GetGenericArguments()[0];
				object innerValue = QueryHelper.ChangeType(value, innerType);
				return Activator.CreateInstance(type, new object[] { innerValue });
			}
#endif
			if (value is string && type == typeof(Guid)) return new Guid(value as string);
			if (value is string && type == typeof(Version)) return new Version(value as string);
			if (!(value is IConvertible)) return value;
			return Convert.ChangeType(value, type);
		}

		internal QueryHelper(Mappings mappings) {
			this.mappings = mappings;
		}
	}
}
