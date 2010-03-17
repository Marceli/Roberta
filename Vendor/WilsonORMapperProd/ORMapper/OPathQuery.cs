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
	/// Represents criteria for searching objects in a data store using queries written in the OPath language.
	/// </summary>
	public class OPathQuery : BaseQuery
	{
		private Type _objectType;
		private string _sort;

		/// <summary>
		/// Creates a new instances of this class.
		/// </summary>
		/// <param name="objectType">Type of object to be queried.</param>
		/// <param name="whereExpression">OPath query expression defining the criteria for objects returned.</param>
		public OPathQuery(Type objectType, string whereExpression)
		{
			_objectType = objectType;
			base.where = whereExpression;
			_sort = null;
		}

		/// <summary>
		/// Creates a new instances of this class.
		/// </summary>
		/// <param name="objectType">Type of object to be queried.</param>
		/// <param name="whereExpression">OPath query expression defining the criteria for objects returned.</param>
		/// <param name="sortExpression">OPath sort expression defining the order of the objects returned.</param>
		/// <remarks>
		/// This overload conflicts with the (type, where, span) overload in the Longhorn 4074 spec.
		/// There is currently no support for spans and no plans to add support in the near future.
		/// </remarks>
		public OPathQuery(Type objectType, string whereExpression, string sortExpression)
		{
			_objectType = objectType;
			base.where = whereExpression;
			_sort = sortExpression;
		}


		//public OPathQuery(Type objectType, OPathExpression whereExpression)
		//public OPathQuery(Type resultType, string whereExpression, string spans);
		//public OPathQuery(Type resultType, OPathExpression whereExpression, string spans);
		//public OPathQuery(QueryAlias rootAlias, OPathExpression whereExpression, string spans);

		//public Span QuerySpan { get; }
		//public QueryAlias ResultAlias { get; }
		//public OrderByList Sort { get; }


		/// <summary>
		/// Gets the Type of the objects returned by this query.
		/// </summary>
		public Type ObjectType
		{
			get { return _objectType; }
		}


		/// <summary>
		/// Gets the OPath expression that represents the sort criteria of the query.
		/// </summary>
		public string SortExpression // NOTE: not in longhorn spec (Longhorn uses a complex OrderByList API that seems like overkill)
		{
			get { return _sort; }
		}
		

		/// <summary>
		/// Compiles this OPathQuery using the mappings found in the specified ObjectSpace instance.
		/// </summary>
		/// <param name="os">ObjectSpace instance to use.</param>
		/// <returns>A CompiledQuery that is the result of this instance being compiled.</returns>
		public CompiledQuery Compile(ObjectSpace os)
		{
			ObjectExpression oe = OPath.Parse(this, os.context.Mappings);
			return new CompiledQuery(oe);
		}
	}
}
