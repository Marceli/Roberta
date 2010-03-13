//**************************************************************//
// Paul Wilson -- www.WilsonDotNet.com -- Paul@WilsonDotNet.com //
// Feel free to use and modify -- just leave these credit lines //
// I also always appreciate any other public credit you provide //
//**************************************************************//
// Ken Muse (http://www.MomentsFromImpact.com) gave significant //
// assistance with Recursive PersistChanges and Cascade Deletes //
//**************************************************************//
// The ILoadOnDemand Interface is similar to the latest ObjectSpaces spec:
//   Thanks to both Allan Ritchie (A.Ritchie@ACRWebSolutions.com)
//   and Jerry Shea (http://www.RenewTek.com) for related advice
// SelectSP advice and code from Allan Ritchie (A.Ritchie@ACRWebSolutions.com)
// XML comments on remianing public members from Jeff Lanning (jefflanning@gmail.com).
using System;
using System.Collections;

namespace Wilson.ORMapper
{
	/// <summary>
	///		ObjectList is used for Lazy Loading Child ObjectSets
	///	</summary>
	public class ObjectList : ILoadOnDemand, IObjectSet
	{
		private Internals.Context context;
		private Type type;
		private ObjectQuery query = null;
		private SelectProcedure selectSP;
		
		private ObjectSet list = null;

		internal ObjectList(Internals.Context context, ObjectQuery objectQuery) {
			this.context = context;
			this.type = objectQuery.ObjectType;
			this.query = objectQuery;
		}

		internal ObjectList(Internals.Context context, SelectProcedure selectSP) {
			this.context = context;
			this.type = selectSP.ObjectType;
			this.selectSP = selectSP;
		}

		internal ObjectList(Internals.Context context, ObjectSet objectSet) {
			this.context = context;
			this.type = objectSet.ObjectType;
			this.list = objectSet;
		}

		internal ObjectList(Internals.Context context, Type objectType) {
			this.context = context;
			this.type = objectType;
		}

		#region ILoadOnDemand Members

		/// <summary>The Object has been Loaded from the Database</summary>
		public bool IsLoaded {
			get { return (this.list != null); }
		}

		/// <summary>Get the Latest Version of Object from Database</summary>
		public void Resync() {
			this.list = null;
			int count = this.Count;
		}

		#endregion

		internal ArrayList Removed {
			get { return this.List.Removed; }
		}
	
		/// <summary>
		/// Returns a string representing the type of list and the type of objects held.
		/// </summary>
		public override string ToString() {
			return this.List.ToString();
		}

		private ObjectSet List {
			get {
				if (!this.IsLoaded) {
					if (this.query != null) {
						this.list = this.context.GetObjectSet(this.query, false);
					}
					else if (this.selectSP != null) {
						this.list = this.context.GetObjectSet(this.selectSP, false);
					}
					else {
						this.list = new ObjectSet(this.type, 1, 0, 0);
					}
				}
				return this.list;
			}
		}

		#region IObjectSet Members

		/// <summary>Gets an object instance with the specified key</summary>
		/// <param name="objectKey">The key for the object</param>
		/// <returns>An object instance</returns>
		public object GetObject(object objectKey) {
			return this.List.GetObject(objectKey);
		}

		/// <summary>Adds an object to the collection</summary>
		/// <param name="objectKey">The key for the object</param>
		/// <param name="entityObject">The entity object to add to the collection</param>
		public void Add(object objectKey, object entityObject) {
			this.List.Add(objectKey, entityObject);
		}

		/// <summary>Removes an object from the collection with the specified key</summary>
		/// <param name="objectKey">The key for the object</param>
		public void RemoveByKey(object objectKey) {
			this.List.RemoveByKey(objectKey);
		}

		#endregion

		#region IObjectList Members

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

		private IList IList {
			get { return this.List.IList; }
		}

		#region IList Members

		/// <summary>
		/// Gets or sets the element at the specified index.
		/// </summary>
		public object this[int index] {
			get {
				int safeIndex = (index < 0 ? 0 : index);
				return this.IList[safeIndex];
			}
			set { this.IList[index] = value; }
		}

		/// <summary>
		/// Gets a value indicating if this list is read-only.
		/// </summary>
		public bool IsReadOnly {
			get { return this.List.IsReadOnly; }
		}

		/// <summary>
		/// Gets a value indicating if the size of this list is fixed.
		/// </summary>
		public bool IsFixedSize {
			get { return this.List.IsFixedSize; }
		}

		/// <summary>
		/// Adds an object to the list.
		/// </summary>
		/// <param name="entityObject">Object to be added.</param>
		/// <returns>The index of the object in the list.</returns>
		public int Add(object entityObject) {
			object objectKey = this.context.GetObjectKey(entityObject);
			this.List.Add(objectKey, entityObject);
			return this.List.Count - 1;
		}

		/// <summary>
		/// Inserts an element into the list at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which value should be inserted.</param>
		/// <param name="entityObject">The Object to insert.</param>
		public void Insert(int index, object entityObject) {
			this.List.Insert(index, entityObject);
		}

		/// <summary>
		/// Removes all objects from this list.
		/// </summary>
		public void Clear() {
			this.List.Clear();
		}

		/// <summary>
		/// Removes a specific item from the list.
		/// </summary>
		/// <param name="entityObject">Object to be removed from the list.</param>
		public void Remove(object entityObject) {
			object objectKey = this.context.GetObjectKey(entityObject);
			this.List.RemoveByKey(objectKey);
		}

		/// <summary>
		/// Removes the item at the specified location in the list.
		/// </summary>
		/// <param name="index"></param>
		public void RemoveAt(int index) {
			this.List.RemoveAt(index);
		}

		/// <summary>
		/// Determines whether the list contains a specific item.
		/// </summary>
		/// <param name="entityObject">Object to locate in the list.</param>
		/// <returns>True if the item is in the list; otherwise, false.</returns>
		public bool Contains(object entityObject) {
			return this.IList.Contains(entityObject);
		}

		/// <summary>
		/// Determines the index of a specific item in the list.
		/// </summary>
		/// <param name="entityObject">Object to locate in the list</param>
		/// <returns>Index of the item in the list.</returns>
		public int IndexOf(object entityObject) {
			return this.IList.IndexOf(entityObject);
		}

		#endregion

		#region ICollection Members

		/// <summary>
		/// Gets a value indicating whether access to the list is thread-safe.
		/// </summary>
		public bool IsSynchronized {
			get { return this.IList.IsSynchronized; }
		}

		/// <summary>
		/// Gets the number of items in the list.
		/// </summary>
		public int Count {
			get { return this.IList.Count; }
		}

		/// <summary>
		/// Copies the elements of the list to an array, starting at a particular point in the array.
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		public void CopyTo(Array array, int index) {
			this.IList.CopyTo(array, index);
		}

		/// <summary>
		/// Gets an object that can be used to synchronize access to the list.
		/// </summary>
		public object SyncRoot {
			get { return this.IList.SyncRoot; }
		}

		#endregion

		#region IEnumerable Members

		/// <summary>
		/// Returns an enumerator that can iterate through the list.
		/// </summary>
		/// <returns>An IEnumerator for the entire list.</returns>
		public IEnumerator GetEnumerator() {
			return this.IList.GetEnumerator();
		}

		#endregion
	}
}
