//**************************************************************//
// Paul Wilson -- www.WilsonDotNet.com -- Paul@WilsonDotNet.com //
// Feel free to use and modify -- just leave these credit lines //
// I also always appreciate any other public credit you provide //
//**************************************************************//
// Allan Ritchie (A.Ritchie@ACRWebSolutions.com) provided lots  //
// of advice and code and significant assistance with providers //
//**************************************************************//
// Date Format concept from Ken Muse (http://www.MomentsFromImpact.com) and Others
// Many new settings for OPath support from Jeff Lanning (jefflanning@gmail.com)
// DateTime provider-specific min/max from Marc Brooks (http://musingmarc.blogspot.com)
using System;
using System.Collections;
using System.Data;
using System.Reflection;
using Wilson.ORMapper.Internals;

namespace Wilson.ORMapper
{
	/// <summary>
	///     The CustomProvider class is the means to plug in a custom database provider,
	///     including setting proper delimiters, identity query, and select page query.
	/// </summary>
	public class CustomProvider
	{
		private bool isCustomProvider;
		private Provider providerType;
		private Type connectionType;
		private Type dataAdapterType;

		// ANSI Default Delimiters
		private string startDelimiter = "\"";
		private string endDelimiter = "\"";
		private string guidDelimiter = "'";
		private string dateDelimiter = "'";
		private string lineTerminator = ";";

		// Other Default Settings
		private string dateFormat = "yyyy-MM-dd HH:mm:ss";
		private string parameterPrefix = "@";
		private string identityQuery = "SELECT MAX({0}) FROM {1}";
		private string selectPageQuery = null;
		private string columnAliasKeyword = "AS";
		private DateTime minimumDate = new DateTime(0001, 01, 01, 00, 00, 00);
		private DateTime maximumDate = new DateTime(9999, 12, 31, 23, 59, 59);

		// OPath settings for database-specific query generation (the defaults below are setup for SQL Server)
		private string trueLiteral = "1";
		private string falseLiteral = "0";
		private string inequalityOperator = "!=";
		private string concatenationOperator = "+";
		private string modulusFunction = "({0} % {1})";
		private string lengthFunction = "LEN({0})";
		private string leftFunction = "LEFT({0}, {1})";
		private string rightFunction = "RIGHT({0}, {1})";
		private string substringFunction = "SUBSTRING({0}, {1}, {2})";
		private string trimFunction = "LTRIM(RTRIM({0})";
		private string likeFunction = "({0} LIKE {1})";
		private string upperCaseFunction = "UPPER({0})";
		private string lowerCaseFunction = "LOWER({0})";

		/// <summary>
		///     Start and End Delimiters are placed around all table and field names.
		///     The double quote is the default -- most databases support in ANSI mode.
		/// </summary>
		/// <example>
		/// <code>
		/// Default: "
		/// MySql: `
		/// PostgreSql: [
		/// Sqlite: [
		/// Firebird: "
		/// DB2: `
		/// VistaDB: [
		/// </code>
		/// </example>
		public string StartDelimiter {
			get { return this.startDelimiter; }
			set { this.startDelimiter = value; }
		}

		/// <summary>
		///     Start and End Delimiters are placed around all table and field names.
		///     The double quote is the default -- most databases support in ANSI mode.
		/// </summary>
		/// <example>
		/// <code>
		/// Default: "
		/// MySql: `
		/// PostgreSql: ]
		/// Sqlite: ]
		/// Firebird: "
		/// DB2: `
		/// VistaDB: ]
		/// </code>
		/// </example>
		public string EndDelimiter {
			get { return this.endDelimiter; }
			set { this.endDelimiter = value; }
		}

		/// <summary>
		///     Delimiters placed around Guid values -- if not the default single quote.
		/// </summary>
		/// <example>
		/// <code>
		/// Default: '
		/// </code>
		/// </example>
		public string GuidDelimiter {
			get { return this.guidDelimiter; }
			set { this.guidDelimiter = value; }
		}

		/// <summary>
		///     Delimiters placed around Date values -- if not the default single quote.
		/// </summary>
		/// <example>
		/// <code>
		/// Default: '
		/// </code>
		/// </example>
		public string DateDelimiter {
			get { return this.dateDelimiter; }
			set { this.dateDelimiter = value; }
		}

		/// <summary>
		///     Terminator at the end of each statement -- if not the default semi-colon.
		/// </summary>
		/// <example>
		/// <code>
		/// Default: ;
		/// </code>
		/// </example>
		public string LineTerminator {
			get { return this.lineTerminator; }
			set { this.lineTerminator = value; }
		}

		// Changed the default as recommended by Humfrey Goffin and Jeff Lanning
		/// <summary>
		///     Format used for date values in SQL -- if not the default ToString().
		/// </summary>
		/// <example>
		/// <code>
		/// Default: yyyy-MM-dd HH:mm:ss
		/// Oracle: dd-MMM-yyyy HH:mm:ss
		/// </code>
		/// </example>
		public string DateFormat {
			get { return this.dateFormat; }
			set { this.dateFormat = value; }
		}

		/// <summary>
		///     Prefix used to indicate parameters in SQL -- if not the default @ symbol.
		/// </summary>
		/// <example>
		/// <code>
		/// Default: @
		/// PostgreSql: :
		/// </code>
		/// </example>
		public string ParameterPrefix {
			get { return this.parameterPrefix; }
			set { this.parameterPrefix = value; }
		}

		/// <summary>
		///     This should be the SQL statement to retrieve the last "identity" value.
		///     Reference the name of the key field as {0} and the name of the table as {1}.
		/// </summary>
		/// <example>
		/// <code>
		/// Default: "SELECT MAX({0}) FROM {1}"
		/// MySql: "SELECT LAST_INSERT_ID()"
		/// PostgreSql: "SELECT currval('{1}_{0}_seq')"
		/// Sqlite: "SELECT last_insert_rowid()"
		/// Firebird: "SELECT gen_id(gen_{1}_id, 0) FROM RDB$DATABASE"
		/// DB2: "VALUES IDENTITY_VAL_LOCAL()"
		/// VistaDB: "SELECT LastIdentity({0}) FROM {1}"
		/// </code>
		/// </example>
		public string IdentityQuery {
			get { return this.identityQuery; }
			set { this.identityQuery = value; }
		}

		/// <summary>
		///     This should be the SQL statement to retrieve a specific "page" of data.
		///     Reference the page size as {0}, skip rows as {1}, and start row as {2}.
		/// </summary>
		/// <example>
		/// <code>
		/// Default: null -- Not Supported
		/// MySql: "SELECT * LIMIT {0} OFFSET {1}"
		/// PostgreSql: "SELECT * LIMIT {0} OFFSET {1}"
		/// Sqlite: "SELECT * LIMIT {0} OFFSET {1}"
		/// Firebird: "SELECT FIRST {0} SKIP {1} *"
		/// DB2: null -- Not Supported
		/// VistaDB: "SELECT TOP {2}, {0} *"
		/// </code>
		/// </example>
		public string SelectPageQuery {
			get { return this.selectPageQuery; }
			set { this.selectPageQuery = value; }
		}

		/// <summary>
		///     Keyword used to alias field-names in SQL -- if not the default empty string.
		/// </summary>
		/// <example>
		/// <code>
		/// Default: AS
		/// Oracle: String.Empty
		/// </code>
		/// </example>
		public string ColumnAliasKeyword {
			get { return this.columnAliasKeyword; }
			set { this.columnAliasKeyword = value; }
		}

		/// <summary>
		///     Minimum DateTime allowed in the database -- becomes value of DateTime.MinValue.
		/// </summary>
		/// <example>
		/// <code>
		/// Default: 0001-01-01
		/// MS Sql: 1753-01-01
		/// Oracle: 0001-01-01 (Oracle supports -4712-01-01, but .NET does not)
		/// </code>
		/// </example>
		public DateTime MinimumDate {
			get { return this.minimumDate; }
			set { this.minimumDate = value; }
		}

		/// <summary>
		///     Maximum DateTime allowed in the database -- becomes value of DateTime.MaxValue.
		/// </summary>
		/// <example>
		/// <code>
		/// Default: 9999-12-31
		/// Oracle: 4712-01-01
		/// </code>
		/// </example>
		public DateTime MaximumDate {
			get { return this.maximumDate; }
			set { this.maximumDate = value; }
		}


		#region --- Settings for OPath Support by Jeff Lanning (jefflanning@gmail.com) ---

		/// <summary>
		/// Gets or sets the SQL literal used to represent 'true'.
		/// </summary>
		/// <remarks>
		/// This setting it required by the OPath Engine and used to generate database-specific queries.
		/// </remarks>
		public string TrueLiteral
		{
			get { return this.trueLiteral; }
			set { this.trueLiteral = value; }
		}

		/// <summary>
		/// Gets or sets the SQL literal used to represent 'false'.
		/// </summary>
		/// <remarks>
		/// This setting it required by the OPath Engine and used to generate database-specific queries.
		/// </remarks>
		public string FalseLiteral
		{
			get { return this.falseLiteral; }
			set { this.falseLiteral = value; }
		}

		/// <summary>
		/// Gets or sets the SQL operator used to test for inequalities.
		/// </summary>
		/// <remarks>
		/// This setting it required by the OPath Engine and used to generate database-specific queries.
		/// </remarks>
		public string InequalityOperator
		{
			get { return this.inequalityOperator; }
			set { this.inequalityOperator = value; }
		}

		/// <summary>
		/// Gets or sets the SQL operator used to concatenate two strings.
		/// </summary>
		/// <remarks>
		/// This setting it required by the OPath Engine and used to generate database-specific queries.
		/// </remarks>
		public string ConcatenationOperator
		{
			get { return this.concatenationOperator; }
			set { this.concatenationOperator = value; }
		}

		/// <summary>
		/// Gets or sets a SQL expression which can compute the modulus between two operands.
		/// Value must be a formatted string with two parameters: {0} for left operand, and {1} for right operand.
		/// </summary>
		/// <remarks>
		/// This setting it required by the OPath Engine and used to generate database-specific queries.
		/// </remarks>
		public string ModulusFunction
		{
			get { return this.modulusFunction; }
			set { this.modulusFunction = value; }
		}

		/// <summary>
		/// Gets or sets a SQL expression which can compute the number of characters in a string.
		/// Value must be a formatted string with one parameter: {0} for the operand.
		/// </summary>
		/// <remarks>
		/// This setting it required by the OPath Engine and used to generate database-specific queries.
		/// </remarks>
		public string LengthFunction
		{
			get { return this.lengthFunction; }
			set { this.lengthFunction = value; }
		}

		/// <summary>
		/// Gets or sets a SQL expression which can extract a specified number of characters from the left of a string.
		/// Value must be a formatted string with two parameters: {0} for the value, {1} for the length.
		/// </summary>
		/// <remarks>
		/// This setting it required by the OPath Engine and used to generate database-specific queries.
		/// </remarks>
		public string LeftFunction
		{
			get { return this.leftFunction; }
			set { this.leftFunction = value; }
		}

		/// <summary>
		/// Gets or sets a SQL expression which can extract a specified number of characters from the right of a string.
		/// Value must be a formatted string with two parameters: {0} for the value, {1} for the length.
		/// </summary>
		/// <remarks>
		/// This setting it required by the OPath Engine and used to generate database-specific queries.
		/// </remarks>
		public string RightFunction
		{
			get { return this.rightFunction; }
			set { this.rightFunction = value; }
		}

		/// <summary>
		/// Gets or sets a SQL expression which can extract a substring from a string.
		/// Value must be a formatted string with three parameters: {0} for the value, {1} for the start index, and {2} for the length.
		/// Note: The start index is assumed to be one-based.
		/// </summary>
		/// <remarks>
		/// This setting it required by the OPath Engine and used to generate database-specific queries.
		/// </remarks>
		public string SubstringFunction
		{
			get { return this.substringFunction; }
			set { this.substringFunction = value; }
		}

		/// <summary>
		/// Gets or sets a SQL expression which can trim the whitespace from both sides of a string.
		/// Value must be a formatted string with one parameter: {0} for the operand.
		/// </summary>
		/// <remarks>
		/// This setting it required by the OPath Engine and used to generate database-specific queries.
		/// </remarks>
		public string TrimFunction
		{
			get { return this.trimFunction; }
			set { this.trimFunction = value; }
		}

		/// <summary>
		/// Gets or sets a SQL expression which can perform a LIKE comparison on a string.
		/// Value must be a formatted string with two parameters: {0} for the value and {1} for the pattern.
		/// </summary>
		/// <remarks>
		/// This setting it required by the OPath Engine and used to generate database-specific queries.
		/// </remarks>
		public string LikeFunction {
			get { return this.likeFunction; }
			set { this.likeFunction = value; }
		}

		/// <summary>
		/// Gets or sets a SQL expression which can convert all lowercase characters in a string to uppercase.
		/// Value must be a formatted string with one parameter: {0} for the value.
		/// </summary>
		/// <remarks>
		/// This setting it required by the OPath Engine and used to generate database-specific queries.
		/// </remarks>
		public string UpperCaseFunction {
			get { return this.upperCaseFunction; }
			set { this.upperCaseFunction = value; }
		}

		/// <summary>
		/// Gets or sets a SQL expression which can convert all uppercase characters in a string to lowercase.
		/// Value must be a formatted string with one parameter: {0} for the value.
		/// </summary>
		/// <remarks>
		/// This setting it required by the OPath Engine and used to generate database-specific queries.
		/// </remarks>
		public string LowerCaseFunction {
			get { return this.lowerCaseFunction; }
			set { this.lowerCaseFunction = value; }
		}

		#endregion

		internal bool IsCustom {
			get { return this.isCustomProvider; }
		}

		internal Provider Provider {
			get { return this.providerType; }
		}

		internal Type Connection {
			get { return this.connectionType; }
		}

		internal Type DataAdapter {
			get { return this.dataAdapterType; }
		}

		internal bool NoNamedParameters  {
			get { return (!this.isCustomProvider && (this.providerType == Provider.OleDb || this.providerType == Provider.Odbc)); }
		}

		internal bool UseParameterPrefix {
			get { return (this.providerType != Provider.Oracle); }
		}

		internal bool UseDateTimeString {
			get { return (this.providerType == Provider.Access); }
		}

		internal bool UseInsertReturn {
			get { return (this.providerType == Provider.Oracle); }
		}

		// Default Parameter Name Fix by Stephen Roughley (http://www.RedBlackSoftware.co.uk)
		internal string GetParameterDefault(string fieldName) {
			if (this.NoNamedParameters) return "?";
			string cleanName = fieldName.TrimStart('_').Replace(" ","_").Replace("/","_").Replace("-","_");
			return (cleanName.StartsWith(this.parameterPrefix) ? cleanName : this.parameterPrefix + cleanName);
		}

		internal string GetParameterName(string parameterName) {
			if (this.UseParameterPrefix) return parameterName;
			return parameterName.Remove(0, this.parameterPrefix.Length);
		}
		
		internal CustomProvider(Provider providerType) {
			this.isCustomProvider = false;
			this.providerType = providerType;
			switch (this.providerType) {
				case Provider.MsSql :
				case Provider.Sql2005 :
					this.startDelimiter = "[";
					this.endDelimiter = "]";
					this.minimumDate = new DateTime(1753, 01, 01, 00, 00, 00);
					// OPath Settings:
					this.trueLiteral = "1";
					this.falseLiteral = "0";
					this.inequalityOperator = "!=";
					this.concatenationOperator = "+";
					this.modulusFunction = "({0} % {1})";
					this.lengthFunction = "LEN({0})";
					this.leftFunction = "LEFT({0}, {1})";
					this.rightFunction = "RIGHT({0}, {1})";
					this.substringFunction = "SUBSTRING({0}, {1}, {2})";
					this.trimFunction = "LTRIM(RTRIM({0}))";
					this.likeFunction = "({0} LIKE {1})";
					this.upperCaseFunction = "UPPER({0})";
					this.lowerCaseFunction = "LOWER({0})";
					break;
				case Provider.Access :
					this.startDelimiter = "[";
					this.endDelimiter = "]";
					this.guidDelimiter = "";
					this.dateDelimiter = "#";
					// OPath Settings:
					this.trueLiteral = "TRUE";
					this.falseLiteral = "FALSE";
					this.inequalityOperator = "<>";
					this.concatenationOperator = "&";
					this.modulusFunction = "({0} MOD {1})";
					this.lengthFunction = "LEN({0})";
					this.leftFunction = "LEFT({0}, {1})";
					this.rightFunction = "RIGHT({0}, {1})";
					this.substringFunction = "MID({0}, {1}, {2})";
					this.trimFunction = "TRIM({0})";
					this.likeFunction = "({0} LIKE {1})";
					this.upperCaseFunction = "UCASE({0})";
					this.lowerCaseFunction = "LCASE({0})";
					break;
				case Provider.Oracle :
					this.lineTerminator = "";
					this.dateFormat = "dd-MMM-yyyy HH:mm:ss"; // Jeff Lanning (jefflanning@gmail.com): added time to format
					this.parameterPrefix = ":";
					this.columnAliasKeyword = "";
					this.minimumDate = new DateTime(0001, 01, 01, 00, 00, 00);
					this.maximumDate = new DateTime(4712, 12, 31, 23, 59, 59);
					// OPath Settings:
					this.trueLiteral = "TRUE";
					this.falseLiteral = "FALSE";
					this.inequalityOperator = "!=";
					this.concatenationOperator = "||";
					this.modulusFunction = "MOD({0}, {1})";
					this.lengthFunction = "LENGTH({0})";
					this.leftFunction = "SUBSTR({0}, 1, {1})";
					this.rightFunction = "SUBSTR({0}, -{1}, {1})";
					this.substringFunction = "SUBSTR({0}, {1}, {2})";
					this.trimFunction = "LTRIM(RTRIM({0}))";
					this.likeFunction = "({0} LIKE {1})";
					this.upperCaseFunction = "UPPER({0})";
					this.lowerCaseFunction = "LOWER({0})";
					break;
			}
		}

		/// <summary>
		///     The CustomProvider class is the means to plug in a custom database provider,
		///     including setting proper delimiters, identity query, and select page query.
		/// </summary>
		/// <param name="adoAssemblyName">Assembly name of the ADO.NET custom provider</param>
		/// <param name="connectionType">Type name of the ADO.NET IDbConnection class</param>
		/// <param name="dataAdapterType">Type name of the ADO.NET IDbDataAdapter class</param>
		/// <example>
		/// <code>
		/// MySql: "ByteFX.MySqlClient", "ByteFX.Data.MySqlClient.MySqlConnection", "ByteFX.Data.MySqlClient.MySqlDataAdapter"
		/// PostgreSql: "Npgsql", "Npgsql.NpgsqlConnection", "Npgsql.NpgsqlDataAdapter"
		/// Sqlite: "Finisar.SQLite", "Finisar.SQLite.SQLiteConnection", "Finisar.SQLite.SQLiteDataAdapter"
		/// Firebird: "FirebirdSql.Data.Firebird", "FirebirdSql.Data.Firebird.FbConnection", "FirebirdSql.Data.Firebird.FbDataAdapter"
		/// DB2: "IBM.Data.DB2", "IBM.Data.DB2.DB2Connection", "IBM.Data.DB2.DB2DataAdapter"
		/// VistaDB: "VistaDB.Provider", "VistaDB.VistaDBConnection", "VistaDB.VistaDBDataAdapter"
		/// </code>
		/// </example>
		public CustomProvider(string adoAssemblyName, string connectionType, string dataAdapterType) {
			this.isCustomProvider = true;
			this.providerType = Provider.OleDb;

			Assembly assembly = Assembly.LoadWithPartialName(adoAssemblyName);
			if (assembly == null) {
				throw new ORMapperException("ObjectSpace: AdoAssemblyName was Invalid - " + adoAssemblyName);
			}

			this.connectionType = assembly.GetType(connectionType);
			if (this.connectionType.GetInterface(typeof(IDbConnection).ToString()) == null) {
				throw new ORMapperException("ObjectSpace: ConnectionType was Invalid - " + connectionType);
			}
			
			this.dataAdapterType = assembly.GetType(dataAdapterType);
			if (this.dataAdapterType.GetInterface(typeof(IDbDataAdapter).ToString()) == null) {
				throw new ORMapperException("ObjectSpace: DataAdapterType was Invalid - " + dataAdapterType);
			}
		}
	}
}
