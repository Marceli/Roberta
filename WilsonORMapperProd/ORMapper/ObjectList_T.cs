#if DOTNETV2
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Wilson.ORMapper
{
	/// <summary>
	///		ObjectList is used for Lazy Loading Child ObjectSets
	///	</summary>
	public class ObjectList<T> : ILoadOnDemand, IObjectSet<T>, IList
	{
		private Internals.Context context;
		private Type type;
		private ObjectQuery query = null;
		private SelectProcedure selectSP;
		private ObjectSet<T> list = null;
		private object syncRoot;

		internal ObjectList(Internals.Context context, ObjectQuery objectQuery)
		{
			this.context = context;
			this.type = typeof(T);
			this.query = objectQuery;
		}

		internal ObjectList(Internals.Context context, SelectProcedure selectSP)
		{
			this.context = context;
			this.type = typeof(T);
			this.selectSP = selectSP;
		}

		internal ObjectList(Internals.Context context, ObjectSet<T> objectSet)
		{
			this.context = context;
			this.type = typeof(T);
			this.list = objectSet;
		}

		internal ObjectList(Internals.Context context)
		{
			this.context = context;
			this.type = typeof(T);
		}

		internal ArrayList Removed
		{
			get { return this.List.Removed; }
		}

		private ObjectSet<T> List
		{
			get
			{
				if (!this.IsLoaded)
				{
					if (this.query != null)
					{
						this.list = this.context.GetObjectSet<T>(this.query, false);
					}
					else if (this.selectSP != null)
					{
						this.list = this.context.GetObjectSet<T>(this.selectSP, false);
					}
					else
					{
						this.list = new ObjectSet<T>(1, 0, 0);
					}
				}
				return this.list;
			}
		}

		/// <summary>
		/// Returns a string representing the type of list and the type of objects held.
		/// </summary>
		public override string ToString()
		{
			return this.List.ToString();
		}

		#region ILoadOnDemand Members

		/// <summary>The Object has been Loaded from the Database</summary>
		public bool IsLoaded
		{
			get { return (this.list != null); }
		}

		/// <summary>Get the Latest Version of Object from Database</summary>
		public void Resync()
		{
			this.list = null;
			int count = this.Count;
		}

		#endregion

		#region IObjectSet<T> Members

		/// <summary>Gets an object instance with the specified key</summary>
		/// <param name="objectKey">The key for the object</param>
		/// <returns>An object instance</returns>
		public T GetObject(object objectKey)
		{
			return (T)this.List.GetObject(objectKey);
		}

		/// <summary>Adds an object to the collection</summary>
		/// <param name="objectKey">The key for the object</param>
		/// <param name="entityObject">The entity object to add to the collection</param>
		public void Add(object objectKey, T entityObject)
		{
			this.List.Add(objectKey, entityObject);
		}

		/// <summary>Removes an object from the collection with the specified key</summary>
		/// <param name="objectKey">The key for the object</param>
		public void RemoveByKey(object objectKey)
		{
			this.List.RemoveByKey(objectKey);
		}

		#endregion

		#region IObjectPage Members

		/// <summary>The object type for this collection</summary>
		public Type ObjectType {
			get { return this.type; }
		}

		/// <summary>The current page number</summary>
		public int PageIndex {
			get { return this.List.PageIndex; }
		}

		/// <summary>The total number of pages</summary>
		public int PageCount {
			get { return this.List.PageCount; }
		}

		/// <summary>The total number of objects</summary>
		public int TotalCount {
			get { return this.List.TotalCount; }
		}

		#endregion

		#region IList<T> Members

		/// <summary>
		/// Determines the index of a specific item in the list.
		/// </summary>
		/// <param name="entityObject">Object to locate in the list</param>
		/// <returns>Index of the item in the list.</returns>
		public int IndexOf(T entityObject)
		{
			return this.List.IndexOf(entityObject);
		}

		/// <summary>
		/// Inserts an element into the list at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which value should be inserted.</param>
		/// <param name="entityObject">The Object to insert.</param>
		public void Insert(int index, T entityObject)
		{
			this.List.Insert(index, entityObject);
		}

		/// <summary>
		/// Removes the item at the specified location in the list.
		/// </summary>
		/// <param name="index"></param>
		public void RemoveAt(int index)
		{
			this.List.RemoveAt(index);
		}

		/// <summary>
		/// Gets or sets the element at the specified index.
		/// </summary>
		public T this[int index]
		{
			get
			{
				int safeIndex = (index < 0 ? 0 : index);
				return (T)this.List[safeIndex];
			}
			set 
			{
				this.List[index] = value; 
			}
		}

		#endregion

		#region ICollection<T> Members

		/// <summary>
		/// Adds an object to the list.
		/// </summary>
		/// <param name="entityObject">Object to be added.</param>
		/// <returns>The index of the object in the list.</returns>
		public void Add(T entityObject)
		{
			object objectKey = this.context.GetObjectKey(entityObject);
			this.List.Add(objectKey, entityObject);
			//return this.List.Count - 1;
		}

		/// <summary>
		/// Removes all objects from this list.
		/// </summary>
		public void Clear()
		{
			this.List.Clear();
		}

		/// <summary>
		/// Determines whether the list contains a specific item.
		/// </summary>
		/// <param name="entityObject">Object to locate in the list.</param>
		/// <returns>True if the item is in the list; otherwise, false.</returns>
		public bool Contains(T entityObject)
		{
			return this.List.Contains(entityObject);
		}

		/// <summary>
		/// Copies the elements of the list to an array, starting at a particular point in the array.
		/// </summary>
		/// <param name="array"></param>
		/// <param name="arrayIndex"></param>
		public void CopyTo(T[] array, int arrayIndex)
		{
			this.List.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Gets the number of items in the list.
		/// </summary>
		public int Count
		{
			get { return this.List.Count; }
		}

		/// <summary>
		/// Gets a value indicating if this list is read-only.
		/// </summary>
		public bool IsReadOnly
		{
			get { return this.List.IsReadOnly; }
		}

		/// <summary>
		/// Removes a specific item from the list.
		/// </summary>
		/// <param name="entityObject">Object to be removed from the list.</param>
		public bool Remove(T entityObject)
		{
			object objectKey = this.context.GetObjectKey(entityObject);
			this.List.RemoveByKey(objectKey);
			return true;
		}

		#endregion

		#region IEnumerable<T> Members
		/// <summary>
		/// Returns an enumerator that can iterate through the list.
		/// </summary>
		/// <returns>An IEnumerator for the entire list.</returns>
		public IEnumerator<T> GetEnumerator()
		{
			return this.List.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.List.GetEnumerator();
		}

		#endregion

		#region IList Members

		int IList.Add(object entityObject)
		{
			object objectKey = this.context.GetObjectKey(entityObject);
			this.List.Add(objectKey, (T)entityObject);
			return this.List.Count - 1;
		}

		void IList.Clear()
		{
			this.List.Clear();
		}

		bool IList.Contains(object entityObject)
		{
			return this.List.Contains((T)entityObject);
		}

		int IList.IndexOf(object entityObject)
		{
			return this.List.IndexOf((T)entityObject);
		}

		void IList.Insert(int index, object entityObject)
		{
			this.List.Insert(index, entityObject);
		}

		bool IList.IsFixedSize
		{
			get { return this.List.IsFixedSize; }
		}

		bool IList.IsReadOnly
		{
			get { return this.List.IsReadOnly; }
		}

		void IList.Remove(object entityObject)
		{
			object objectKey = this.context.GetObjectKey(entityObject);
			this.List.RemoveByKey(objectKey);
		}

		void IList.RemoveAt(int index)
		{
			this.List.RemoveAt(index);
		}

		object IList.this[int index]
		{
			get
			{
				int safeIndex = (index < 0 ? 0 : index);
				return (T)this.List[safeIndex];
			}
			set
			{
				this.List[index] = (T)value;
			}
		}

		#endregion

		#region ICollection Members

		void ICollection.CopyTo(Array array, int arrayIndex)
		{
			((IList)this.List).CopyTo(array, arrayIndex);
		}

		int ICollection.Count
		{
			get { return this.List.Count; }
		}

		bool ICollection.IsSynchronized
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		object ICollection.SyncRoot
		{
			get
			{
				if (this.syncRoot == null)
				{
					System.Threading.Interlocked.CompareExchange(ref this.syncRoot, new object(), (object)null);
				}
				return this.syncRoot;
			}
		}

		#endregion
}
}
#endif
