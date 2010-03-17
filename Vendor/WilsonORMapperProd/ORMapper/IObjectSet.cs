//**************************************************************//
// Paul Wilson -- www.WilsonDotNet.com -- Paul@WilsonDotNet.com //
// Feel free to use and modify -- just leave these credit lines //
// I also always appreciate any other public credit you provide //
//**************************************************************//
using System;
using System.Collections;

namespace Wilson.ORMapper
{
	/// <summary>
	///     Common Interface used for ObjectSet and ObjectList
	/// </summary>
	public interface IObjectSet : IObjectPage, IList
	{
		/// <summary>Gets an object instance with the specified key</summary>
		/// <param name="objectKey">The key for the object</param>
		/// <returns>An object instance</returns>
		object GetObject(object objectKey);

		/// <summary>Adds an object to the collection</summary>
		/// <param name="objectKey">The key for the object</param>
		/// <param name="entityObject">The entity object to add to the collection</param>
		void Add(object objectKey, object entityObject);

		/// <summary>Removes an object from the collection with the specified key</summary>
		/// <param name="objectKey">The key for the object</param>
		void RemoveByKey(object objectKey);
	}

	
#if DOTNETV2
	/// <summary>
	/// <![CDATA[
	///     Common Interface used for ObjectSet<T> and ObjectList<T>
	/// ]]>
	/// </summary>
	public interface IObjectSet<T> : IObjectPage, System.Collections.Generic.IList<T>
	{
		/// <summary>Gets an object instance with the specified key</summary>
		/// <param name="objectKey">The key for the object</param>
		/// <returns>An object instance</returns>
		T GetObject(object objectKey);

		/// <summary>Adds an object to the collection</summary>
		/// <param name="objectKey">The key for the object</param>
		/// <param name="entityObject">The entity object to add to the collection</param>
		void Add(object objectKey, T entityObject);

		/// <summary>Removes an object from the collection with the specified key</summary>
		/// <param name="objectKey">The key for the object</param>
		void RemoveByKey(object objectKey);
	}
#endif
	
	/// <summary>
	///     Common Interface used for ObjectSet, ObjectList, and ObjectReader
	/// </summary>
	public interface IObjectPage
	{
		/// <summary>The object type for this collection</summary>
		Type ObjectType { get; }

		/// <summary>The current page number</summary>
		int PageIndex { get; }

		/// <summary>The total number of pages</summary>
		int PageCount { get; }

		/// <summary>The total number of objects</summary>
		int TotalCount { get; }
	}

	/// <summary>
	///     Common Interface used for ObjectHolder and ObjectList
	/// </summary>
	public interface ILoadOnDemand
	{
		/// <summary>The Object has been Loaded from the Database</summary>
		bool IsLoaded { get; }

		/// <summary>Get the Latest Version of Object from Database</summary>
		void Resync();
	}
}
