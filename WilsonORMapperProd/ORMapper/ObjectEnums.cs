//**************************************************************//
// Paul Wilson -- www.WilsonDotNet.com -- Paul@WilsonDotNet.com //
// Feel free to use and modify -- just leave these credit lines //
// I also always appreciate any other public credit you provide //
//**************************************************************//
// Includes Support for Composite Primary Keys from Jerry Shea
using System;

namespace Wilson.ORMapper
{
	/// <summary>
	///     The database provider type
	/// </summary>
	public enum Provider {
		/// <summary>Microsoft SQL Server Provider</summary>
		MsSql,
		/// <summary>Microsoft Access Provider</summary>
		Access,
		/// <summary>Oracle Provider</summary>
		Oracle,
		/// <summary>OleDB Provider</summary>
		OleDb,
		/// <summary>ODBC Provider</summary>
		Odbc,
		/// <summary>MS SQL Server 2005 Provider</summary>
		Sql2005
	}

	/// <summary>
	///     The primary key types
	/// </summary>
	public enum KeyType {
		/// <summary>Database Generated Key</summary>
		Auto,
		/// <summary>ORMapper Generates Guid</summary>
		Guid,
		/// <summary>User Provides the Key</summary>
		User,
		/// <summary>User Multiple Column Key</summary>
		Composite,
		/// <summary>For Read-Only Cases/Views</summary>
		None
	}

	/// <summary>
	///     The initial state of an object
	/// </summary>
	public enum InitialState {
		/// <summary>New Record</summary>
		Inserted,
		/// <summary>Existing not Modified</summary>
		Unchanged,
		/// <summary>Existing and Modified</summary>
		Updated
	}

	/// <summary>
	///     The current state of an object
	/// </summary>
	public enum ObjectState {
		/// <summary>New but not Inserted</summary>
		Inserted,
		/// <summary>Existing not Modified</summary>
		Unchanged,
		/// <summary>Existing and Modified</summary>
		Updated,
		/// <summary>Marked for Deletion</summary>
		Deleted,
		/// <summary>Actually Deleted</summary>
		Unknown
	}

	/// <summary>
	///     The depth at which to persist child objects
	/// </summary>
	public enum PersistDepth {
		/// <summary>Save Only the Immediate Entity Object</summary>
		SingleObject,
		/// <summary>Save Changes to Related Children Also</summary>
		ObjectGraph
	}

	/// <summary>
	///     The primary key types
	/// </summary>
	public enum PersistType {
		/// <summary>Save Field Changes in Insert and Update</summary>
		Persist,
		/// <summary>Do Not Save Changes in Insert or Update</summary>
		ReadOnly,
		/// <summary>Read-Only and Use to Check Concurrency</summary>
		Concurrent
	}

	// Added by Paul Welter (http://www.LoreSoft.com)
	/// <summary>
	///     Comparison operators test whether or not two expressions are the same.
	/// </summary>
	public enum ComparisonOperators {
		/// <summary>Equal to Operator</summary>
		Equals,
		/// <summary>Greater than Operator</summary>
		GreaterThan,
		/// <summary>Greater than or equal to Operator</summary>
		GreaterThanEqual,
		/// <summary>Determines whether or not a given expression is NULL</summary>
		IsNotNull,
		/// <summary>Determines whether or not a given expression is NULL</summary>
		IsNull,
		/// <summary>Less than</summary>
		LessThan,
		/// <summary>Less than or equal to Operator</summary>
		LessThanEqual,
		/// <summary>Determines whether or not a given character string matches a specified pattern</summary>
		Like,
		/// <summary>Not equal to Operator</summary>
		NotEqual
	}
}
