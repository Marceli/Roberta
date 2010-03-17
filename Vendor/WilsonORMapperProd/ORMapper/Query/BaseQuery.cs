//-------------------------------------------------
// OPath Query Engine
// Written by Jeff Lanning (jefflanning@gmail.com)
// Modeled after SDK for Longhorn CTP Build 4074
// Version 1: Dec 2004 - May 2005
//-------------------------------------------------
using System;
using System.Collections;

namespace Wilson.ORMapper.Query
{
	/// <summary>
	/// Provides the abstract base class for object-based queries written in the OPath query language.
	/// </summary>
	public abstract class BaseQuery
	{
		internal string where;
		internal OPathParameterCollection parameters = new OPathParameterCollection();
		internal int commandTimeout = 30;

		//public QueryAliasList Aliases { get; }

		//public bool BaseTypeOnly { get; set; }

		/// <summary>
		/// Gets or sets the maximum time (in seconds) this query will be allowed to execute before it is terminated. The default is 30 seconds.
		/// </summary>
		public int CommandTimeout
		{
			get { return this.commandTimeout; }
			set
			{
				// NOTE: explicitly making 0 (infinite timeout) invalid as it should never really be used.
				if( value < 0 ) throw new ArgumentOutOfRangeException("value", value, "Value must be greater than zero.");
				if( value == 0 ) throw new ArgumentOutOfRangeException("value", "Value and cannot be zero; infinite timeout is not allowed.");
				this.commandTimeout = value;
			}
		}

		/// <summary>
		/// Gets the collection of parameters associated to this query.
		/// </summary>
		internal OPathParameterCollection Parameters //V2: Consider making public again (waiting on MS to firm up API)
		{
			get { return this.parameters; }
		}

		//public Join QueryJoin { get; }

		//public OrderByList QueryOrderBy { get; }

		/// <summary>
		/// Gets the OPath expression that represents the search criteria of the query.
		/// </summary>
		public string WhereExpression //OPathExpression
		{
			get { return this.where; }
		}

		//public override string ToString();
	}
}
