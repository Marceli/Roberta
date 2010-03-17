//-------------------------------------------------
// OPath Query Engine
// Written by Jeff Lanning (jefflanning@gmail.com)
// Modeled after SDK for Longhorn CTP Build 4074
// Version 1: Dec 2004 - May 2005
//-------------------------------------------------
using System;

namespace Wilson.ORMapper.Query
{
	/// <summary>
	/// Represents a parameter for an OPathQuery.
	/// </summary>
	internal class OPathParameter //V2: Consider making public again later; like Longhorn 4074 API
	{
		private string _name;
		private Type _type;
		private object _value;
		private int _ordinal;

		//V2: why is no constructor defined in Longhorn specs???
		internal OPathParameter(string name, Type type) : this(name, type, null)
		{
		}

		internal OPathParameter(string name, Type type, object value)
		{
			if( name == null ) throw new ArgumentNullException("name");
			if( name.Length == 0 ) throw new ArgumentException("Parameter name is empty.", "name");

			_name = name;
			_type = type;
			_value = value;
		}

		/// <summary>
		/// Gets or sets the name of this parameter.
		/// </summary>
		public string Name
		{
			get { return _name; }
		}

		/// <summary>
		/// Gets or sets the data type of value this parameter.
		/// </summary>
		public Type ParameterType
		{
			get { return _type; }
		}

		/// <summary>
		/// Gets of sets the value of this parameter.
		/// </summary>
		public object Value
		{
			get { return _value; }
			set { _value = value; }
		}


		internal int Ordinal
		{
			get { return _ordinal; }
			set { _ordinal = value; }
		}
	}
}
