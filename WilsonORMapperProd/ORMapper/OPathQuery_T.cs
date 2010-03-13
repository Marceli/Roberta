//-------------------------------------------------
// OPath Query Engine
// Written by Jeff Lanning (jefflanning@gmail.com)
// Modeled after SDK for Longhorn CTP Build 4074
// Version 1: Dec 2004 - May 2005
//-------------------------------------------------
#if DOTNETV2
using System;
using System.Collections.Generic;
using Wilson.ORMapper.Query;
using Wilson.ORMapper.Internals;

namespace Wilson.ORMapper
{
	/// <summary>
	/// Represents criteria for searching objects in a data store using queries written in the OPath language.
	/// </summary>
	public class OPathQuery<T> : OPathQuery
	{
		/// <summary>
		/// Creates a new instances of this class.
		/// </summary>
		/// <param name="whereExpression">OPath query expression defining the criteria for objects returned.</param>
		public OPathQuery(string whereExpression)
			: base(typeof(T), whereExpression)
		{
		}

		/// <summary>
		/// Creates a new instances of this class.
		/// </summary>
		/// <param name="whereExpression">OPath query expression defining the criteria for objects returned.</param>
		/// <param name="sortExpression">OPath sort expression defining the order of the objects returned.</param>
		public OPathQuery(string whereExpression, string sortExpression)
			: base(typeof(T), whereExpression, sortExpression)
		{
		}

		/// <summary>
		/// Compiles this OPathQuery using the mappings found in the specified ObjectSpace instance.
		/// </summary>
		/// <param name="os">ObjectSpace instance to use.</param>
		/// <returns>A CompiledQuery that is the result of this instance being compiled.</returns>
		public new CompiledQuery<T> Compile(ObjectSpace os)
		{
			ObjectExpression oe = OPath.Parse(this, os.context.Mappings);
			return new CompiledQuery<T>(oe);
		}

		/// <summary>
		/// Executes this OPathQuery against an ObjectSpace data store and returns the first matching object.
		/// Null is returned if no object is found.
		/// </summary>
		/// <param name="os">ObjectSpace instance to use.</param>
		/// <returns>The first object matching the query; or null if no match was found.</returns>
		public T GetObject(ObjectSpace os)
		{
			return GetObject(os, (object[])null);
		}

		/// <summary>
		/// Executes this OPathQuery against an ObjectSpace data store and returns the first matching object.
		/// Null is returned if no object is found.
		/// </summary>
		/// <param name="os">ObjectSpace instance to use.</param>
		/// <param name="parameters">Parameter values to use when executing the query.</param>
		/// <returns>The first object matching the query; or null if no match was found.</returns>
		public T GetObject(ObjectSpace os, params object[] parameters)
		{
			CompiledQuery<T> cq = this.Compile(os);
			return (T)os.GetObject(cq, parameters);
		}

		/// <summary>
		/// Executes this OPathQuery against an ObjectSpace data store and returns an ObjectSet filled with the results.
		/// </summary>
		/// <param name="os">ObjectSpace instance to use.</param>
		/// <returns>An ObjectSet filled with objects retrieved from the data store.</returns>
		public ObjectSet<T> GetObjectSet(ObjectSpace os)
		{
			return GetObjectSet(os, (object[])null);
		}

		/// <summary>
		/// Executes this OPathQuery against an ObjectSpace data store and returns an ObjectSet filled with the results.
		/// </summary>
		/// <param name="os">ObjectSpace instance to use.</param>
		/// <param name="parameters">Parameter values to use when executing the query.</param>
		/// <returns>An ObjectSet filled with objects retrieved from the data store.</returns>
		public ObjectSet<T> GetObjectSet(ObjectSpace os, params object[] parameters)
		{
			CompiledQuery<T> cq = this.Compile(os);
			return os.GetObjectSet<T>(cq, parameters);
		}

		/// <summary>
		/// Executes this query against an ObjectSpace data store and returns an array filled with the results.
		/// </summary>
		/// <param name="os">ObjectSpace instance to use.</param>
		/// <returns>An array filled with objects retrieved from the data store.</returns>
		public T[] GetArray(ObjectSpace os)
		{
			return GetArray(os, (object[])null);
		}

		/// <summary>
		/// Executes this query against an ObjectSpace data store and returns an array filled with the results.
		/// </summary>
		/// <param name="os">ObjectSpace instance to use.</param>
		/// <param name="parameters">Parameter values to use when executing the query.</param>
		/// <returns>An array filled with objects retrieved from the data store.</returns>
		public T[] GetArray(ObjectSpace os, params object[] parameters)
		{
			return GetList(os, parameters).ToArray();
		}

		/// <summary>
		/// Executes this query against an ObjectSpace data store and returns an array filled with the results.
		/// </summary>
		/// <param name="os">ObjectSpace instance to use.</param>
		/// <returns>An array filled with objects retrieved from the data store.</returns>
		public List<T> GetList(ObjectSpace os)
		{
			return GetList(os, (object[])null);
		}

		/// <summary>
		/// Executes this query against an ObjectSpace data store and returns an array filled with the results.
		/// </summary>
		/// <param name="os">ObjectSpace instance to use.</param>
		/// <param name="parameters">Parameter values to use when executing the query.</param>
		/// <returns>An array filled with objects retrieved from the data store.</returns>
		public List<T> GetList(ObjectSpace os, params object[] parameters)
		{
			CompiledQuery<T> cq = this.Compile(os);

			List<T> list = new List<T>(32);
			using( ObjectReader reader = os.GetObjectReader(cq, parameters) )
			{
				while( reader.Read() )
				{
					list.Add((T)reader.Current());
				}
			}
			return list;
		}
	}
}
#endif