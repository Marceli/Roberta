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
	/// Represents a collection of parameters for use with an OPathQuery.
	/// </summary>
	internal class OPathParameterCollection : OPathParameterTable //V2: Consider making public again later; like Longhorn 4074 API
	{
		/// <summary>
		/// Creates a new instance of this class.
		/// </summary>
		public OPathParameterCollection()
		{
		}

		/// <summary>
		/// Adds a new parameter to the collection.
		/// </summary>
		/// <param name="parameterName">Name of the parameter.</param>
		/// <param name="parameterType">Type of the parameter.</param>
		public void Add(string parameterName, Type parameterType)
		{
			base.Add( new OPathParameter(parameterName, parameterType) );
		}

		/// <summary>
		/// Adds a new parameter to the collection.
		/// </summary>
		/// <param name="parameterName">Name of the parameter.</param>
		/// <param name="parameterValue">Value of the parameter.</param>
		public void Add(string parameterName, object parameterValue) // NOTE: not in longhorn spec
		{
			base.Add( new OPathParameter(parameterName, parameterValue.GetType(), parameterValue) );
		}

		/// <summary>
		/// Adds a new parameter to the collection.
		/// </summary>
		/// <param name="parameterName">Name of the parameter.</param>
		/// <param name="parameterType">Type of the parameter.</param>
		/// <param name="parameterValue">Value of the parameter.</param>
		public void Add(string parameterName, Type parameterType, object parameterValue)
		{
			base.Add( new OPathParameter(parameterName, parameterType, parameterValue) );
		}
	}
}
