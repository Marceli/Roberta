//-------------------------------------------------
// OPath Query Engine
// Written by Jeff Lanning (jefflanning@gmail.com)
// Modeled after SDK for Longhorn CTP Build 4074
// Version 1: Dec 2004 - May 2005
//-------------------------------------------------
using System;

using Wilson.ORMapper.Query;
using Wilson.ORMapper.Internals;

namespace Wilson.ORMapper
{
	/// <summary>
	/// Represents a pre-compiled query that can be executed by an ObjectSpace instance.
	/// </summary>
	public class CompiledQuery
	{
		private ObjectExpression _objectExpression;
		private Type _objectType;
		private string _sqlQuery;
		internal OPathQuery baseQuery;
		internal int parameterCount;
		internal OPathParameterTable parameterTable;

		internal CompiledQuery(ObjectExpression oe)
		{
			_objectExpression = oe;
			_objectType = oe.ObjectType;
			this.baseQuery = oe.baseQuery;
			this.parameterCount = oe.ParameterCount;

			OPathCompiler compiler = new OPathCompiler();
			compiler.Compile(oe);

			_sqlQuery = compiler.SqlQuery;
			this.parameterTable = compiler.ParameterTable;
		}


		/// <summary>
		/// Gets the Type of the objects returned by this query.
		/// </summary>
		public Type ObjectType
		{
			get { return _objectType; }
		}

		/// <summary>
		/// Gets the database-specific SELECT statement that is executed against the data store
		/// when this CompiledQuery is used to retrieve objects.
		/// </summary>
		public string SqlQuery
		{
			get { return _sqlQuery; }
		}
	}
}

