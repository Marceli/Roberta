//**************************************************************//
// Paul Wilson -- www.WilsonDotNet.com -- Paul@WilsonDotNet.com //
// Feel free to use and modify -- just leave these credit lines //
// I also always appreciate any other public credit you provide //
//**************************************************************//
// Ken Muse (http://www.MomentsFromImpact.com) gave significant //
// assistance with Recursive PersistChanges and Cascade Deletes //
//**************************************************************//
using System;
using System.Collections;

namespace Wilson.ORMapper
{
	/// <summary>
	///     ObjectSet is a One-Way(Read) Bindable Collection of Objects
	/// </summary>
	///	<example>The following example shows how to use the ObjectSet to
	///	retrieve all Contacts and bind them to a data grid.
	///	<code>
	///	public static ObjectSpace Manager; // See Initialization Example
	///
	///	ObjectSet contacts = Manager.GetObjectSet(typeof(Contact), string.Empty);
	///
	///	dataGrid.DataSource = contacts;
	/// dataGrid.DataBind();
	///	</code>
	///	</example>
	[Serializable()]
	public class ObjectSet : CollectionBase, IObjectSet
	{
		private Type objectType;
		private int pageIndex = 1;
		private int pageCount = 0;
		private int totalCount = 0;


		static private ArrayKeyComparer arrayKeyComparer = new ArrayKeyComparer ();			// added by RTS/GOP
		private Hashtable keyValues = new Hashtable(arrayKeyComparer, arrayKeyComparer);	// added by RTS/GOP
//		private Hashtable keyValues = new Hashtable();										// removed by RTS/GOP
		private ArrayList removed = new ArrayList();

		internal ObjectSet(Type objectType, int pageIndex, int pageCount, int totalCount) {
			this.objectType = objectType;
			this.pageIndex = pageIndex;
			this.pageCount = pageCount;
			this.totalCount = totalCount;
		}

		internal ArrayList Removed {
			get { return this.removed; }
		}

		/// <summary>
		/// Returns a string representing the type of this ObjectSet and the type of objects held.
		/// </summary>
		/// <returns>A string value.</returns>
		public override string ToString() {
			return this.GetType().ToString() + ": " + this.ObjectType.ToString();
		}

		#region IObjectSet Members

		/// <summary>
		///     Gets an object instance with the specified key
		/// </summary>
		/// <param name="objectKey" type="object">
		///		The key for the object
		/// </param>
		/// <returns>
		///     An object instance
		/// </returns>
		public object GetObject(object objectKey) {
			if (!this.keyValues.ContainsKey(objectKey)) return null;
			return this.List[(int) this.keyValues[objectKey]];
		}

		/// <summary>
		///     Adds an object to the collection
		/// </summary>
		/// <param name="objectKey" type="object">
		///		The key for the object
		/// </param>
		/// <param name="entityObject" type="object">
		///		The entity object to add to the collection
		/// </param>
		public void Add(object objectKey, object entityObject) {
			// Exception added by Allan Ritchie (A.Ritchie@ACRWebSolutions.com)
			// Better Inheritance Support with the IsSubClassOf Check of Type
			Type type = entityObject.GetType();
			if (type != this.objectType && !type.IsSubclassOf(this.objectType)) {
				throw new ORMapperException("ObjectSet: Must Add Objects of Type - " + this.objectType.ToString());
			}

			int index = this.InnerList.Add(entityObject);
			this.keyValues.Add(objectKey, index);
		}

		/// <summary>
		///     Removes an object from the collection with the specified key
		/// </summary>
		/// <param name="objectKey" type="object">
		///		The key for the object
		/// </param>
		public void RemoveByKey(object objectKey) {
			int index = (int) this.keyValues[objectKey];
			this.removed.Add(this.List[index]);
			this.InnerList.RemoveAt(index);
			this.keyValues.Remove(objectKey);

			// Includes Bug-Fix by Jerry Shea (http://www.RenewTek.com)
			object[] keys = new object[this.keyValues.Count];
			this.keyValues.Keys.CopyTo(keys, 0);
			for (int counter = 0; counter < keys.Length; counter++) {
				int position = (int) this.keyValues[keys[counter]];
				if (position > index) {
					this.keyValues[keys[counter]] = position - 1;
				}
			}
		}

		#endregion

		#region IObjectList Members

		/// <summary>The object type for this collection</summary>
		public Type ObjectType {
			get { return this.objectType; }
		}

		/// <summary>The current page number</summary>
		public int PageIndex {
			get { return this.pageIndex; }
		}

		/// <summary>The total number of pages for the query</summary>
		public int PageCount {
			get { return this.pageCount; }
		}

		/// <summary>The total number of objects for the query</summary>
		public int TotalCount {
			get { return (this.pageCount == 0 && this.totalCount == 0 ? this.Count : this.totalCount); }
		}

		#endregion

		internal IList IList {
			get { return this.List; }
		}

		#region IList Members

		/// <summary>Gets or sets the element at the specified index.</summary>
		public object this[int index] {
			get {
				int safeIndex = (index < 0 ? 0 : index);
				return this.InnerList[safeIndex];
			}
			set { this.InnerList[index] = value; }
		}

		/// <summary>True if collection is read-only</summary>
		public bool IsReadOnly {
			get { return false; }
		}

		/// <summary>True if collection is fixed sized</summary>
		public bool IsFixedSize {
			get { return false; }
		}

		/// <summary>Not Supported</summary>
		/// <remarks>Use other Add Signature</remarks>
		public int Add(object entityObject) {
			throw new ORMapperException("ObjectSet: Use other Add Signature");
		}

		/// <summary>Not Supported</summary>
		/// <remarks>Use Add instead of Insert</remarks>
		public void Insert(int index, object entityObject) {
			throw new ORMapperException("ObjectSet: Use Add instead of Insert");
		}

		/// <summary>Removes all object from the collection</summary>
		public new void Clear() {
			this.removed.AddRange(this.List);
			this.InnerList.Clear();
			this.keyValues.Clear();
		}

		/// <summary>Not Supported</summary>
		/// <remarks>Use RemoveKey instead of Remove</remarks>
		public void Remove(object entityObject) {
			throw new ORMapperException("ObjectSet: Use RemoveKey instead of Remove");
		}

		/// <summary>Not Supported</summary>
		/// <remarks>Use RemoveKey instead of RemoveAt</remarks>
		public new void RemoveAt(int index) {
			throw new ORMapperException("ObjectSet: Use RemoveKey instead of RemoveAt");
		}

		#endregion
	}

	/// <summary>Represents special comparer and hashcode provider for System.Array subtypes.</summary>
	/// <remarks>(RTS/GOP) Peter Goetzl (gop@rts.co.at) added this helper class. It allows to use composite
	/// key types.</remarks>
	[Serializable()]
	internal class ArrayKeyComparer : System.Collections.IComparer, System.Collections.IHashCodeProvider
	{
		#region IComparer Members
		/// <summary>Returns the sort oder of two System.Object instances.</summary>
		/// <remarks>Null values are treated as smaller than any not null value.</remarks>
		/// <param name="x">Object X.</param>
		/// <param name="y">Object Y</param>
		/// <returns>Returns 0 if X == Y; 1 if X smaller than Y; -1 if X greater than Y.</returns>
		public int Compare (object x, object y)
		{
			if (!x.GetType ().IsSubclassOf (typeof (System.Array)) || !y.GetType ().IsSubclassOf (typeof (System.Array))) 
				return object.Equals (x, y)? 0 : 1;

			object [] ax = (object []) x;
			object [] ay = (object []) y;
			if (ax.Length != ay.Length) return ax.Length - ay.Length;
			for (int i=0; i<ax.Length; ++i) {
				if (ax[i] != null && ay[i] != null)	{	
					int cmp = ((IComparable)ax[i]).CompareTo (ay[i]);
					if (cmp != 0) return cmp;
				}
				else {
					if (ax[i] != null) return 1;
					if (ay[i] != null) return -1;
				}
			}
			return 0;
		}
		#endregion

		#region IHashCodeProvider Members
		/// <summary>Computes special hashcode for arrays.</summary>
		/// <remarks>If the object type is not derived from <see cref="System.Array" /> the computation is 
		/// delegated to <see cref="object.GetHashCode" /> method.</remarks>
		/// <param name="obj">Object from where the hashcode is computed.</param>
		/// <returns>Hashcode.</returns>
		public int GetHashCode(object obj)
		{
			if (obj == null) throw new ArgumentNullException ("obj");
			if (!obj.GetType ().IsSubclassOf (typeof (System.Array)))
				return obj.GetHashCode ();

			object [] aobj = (object []) obj;
			int hash = (aobj [0] != null)? aobj [0].GetHashCode () : 0;
			for (int i=1; i<aobj.Length; ++i) {
				//hash += (aobj [i] != null)? aobj [i].GetHashCode () : 0; 
				hash <<= 5;
				hash ^= (aobj [i] != null)? aobj [i].GetHashCode () : 0; 
			}
			return hash;
		}
		#endregion
	}

}
