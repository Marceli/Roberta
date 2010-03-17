//-------------------------------------------------
// OPath Query Engine
// Written by Jeff Lanning (jefflanning@gmail.com)
// Modeled after SDK for Longhorn CTP Build 4074
// Version 1: Dec 2004 - May 2005
//-------------------------------------------------
using System;
using Wilson.ORMapper.Internals;

namespace Wilson.ORMapper.Query
{
	/// <summary>
	/// The ObjectExpression is used to compile an object query for use by the ObjectSpace engine when returning objects from
	/// the data source. A CompiledQuery can be used to return a "read-only" ObjectReader, and can also be cached for a
	/// performance gain if your application is making multiple calls for the same query results.
	/// </summary>
	internal class ObjectExpression  //V2: Public class in Longhorn Spec
	{
		private Type _objectType;
		private Expression _expression;
		private Mappings _maps;
		private int _parameterCount;
		internal OPathQuery baseQuery;

		internal ObjectExpression(Type objectType, Expression expression, Mappings maps, int parameterCount)
		{
			_objectType = objectType;
			_expression = expression;
			_maps = maps;
			_parameterCount = parameterCount;
		}

		/// <summary>
		/// Gets the class type of this expression.
		/// </summary>
		public Type ObjectType
		{
			get { return _objectType; }
		}

		internal Expression Expression
		{
			get { return _expression; }
		}

		internal Mappings Mappings
		{
			get { return _maps; }
		}

		internal int ParameterCount
		{
			get { return _parameterCount; }
		}


		internal CompiledQuery Compile()
		{
			if( _maps == null ) throw new ArgumentNullException("_maps");
			return new CompiledQuery(this);
		}

		/// <summary>
		/// Compiles this ObjectExpression using the mappings found the specified ObjectSpace instance.
		/// </summary>
		/// <param name="os">ObjectSpace instance to use.</param>
		/// <returns>A CompiledQuery that is the result of this instance being compiled.</returns>
		public CompiledQuery Compile(ObjectSpace os)
		{
			if( os == null ) throw new ArgumentNullException("os");
			_maps = os.context.Mappings;
			return new CompiledQuery(this);
		}
	}
}
