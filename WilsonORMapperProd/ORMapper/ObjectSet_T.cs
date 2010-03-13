//**************************************************************//
// Paul Wilson -- www.WilsonDotNet.com -- Paul@WilsonDotNet.com //
// Feel free to use and modify -- just leave these credit lines //
// I also always appreciate any other public credit you provide //
//**************************************************************//
// Ken Muse (http://www.MomentsFromImpact.com) gave significant //
// assistance with Recursive PersistChanges and Cascade Deletes //
//**************************************************************//
#if DOTNETV2
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
	/// <![CDATA[
	///	public static ObjectSpace Manager; // See Initialization Example
	///	
	///	ObjectSet<Contact> contacts = Manager.GetObjectSet<Contact>(string.Empty);
	///	
	///	dataGrid.DataSource = contacts;
	/// dataGrid.DataBind();
	/// ]]>
	///	</code>
	///	</example>
	[Serializable()]
	public class ObjectSet<T> : System.Collections.ObjectModel.Collection<T>, IObjectSet<T>
	{
		private Type objectType;
		private int pageIndex = 1;
		private int pageCount = 0;
		private int totalCount = 0;

		private Hashtable keyValues = new Hashtable();
		private ArrayList removed = new ArrayList();

		internal ObjectSet(int pageIndex, int pageCount, int totalCount) {
			this.objectType = typeof(T);
			this.pageIndex = pageIndex;
			this.pageCount = pageCount;
			this.totalCount = totalCount;
		}

		internal ObjectSet(ObjectReader reader)
			: this (reader.PageIndex, reader.PageCount, reader.TotalCount)
		{
			try {
				while (reader.Read()) {
					this.Add(reader.ObjectKey(), (T) reader.Current());
				}
			}
			finally {
				if (reader != null) reader.Close();
			}
		}

		internal ArrayList Removed {
			get { return this.removed; }
		}

		/// <summary>
		/// Returns a string representing the type of this ObjectSet and the type of objects held.
		/// </summary>
		/// <returns>A string value.</returns>
		public override string ToString() {
			return this.GetType().ToString() + ": " + this.objectType.ToString();
		}

		#region IObjectSet<T> Members

		/// <summary>
		///     Gets an object instance with the specified key
		/// </summary>
		/// <param name="objectKey" type="object">
		///		The key for the object
		/// </param>
		/// <returns>
		///     An object instance
		/// </returns>
		public T GetObject(object objectKey) {
			if (!this.keyValues.ContainsKey(objectKey)) return default(T);
			return (T) this[(int) this.keyValues[objectKey]];
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
		public void Add(object objectKey, T entityObject) {
			this.Items.Add(entityObject);
			int index = this.Items.IndexOf(entityObject);
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
			this.removed.Add(this[index]);
			this.Items.RemoveAt(index);
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

		#region IList Members

		/// <summary>Gets or sets the element at the specified index.</summary>
		new public T this[int index]	{
			get	{
				int safeIndex = (index < 0 ? 0 : index);
				return this.Items[safeIndex];
			}
			set { this.Items[index] = value; }
		}

		/// <summary>True if collection is readonly</summary>
		public bool IsReadOnly {
			get { return false; }
		}

		/// <summary>True if collection is fixed sized</summary>
		public bool IsFixedSize	{
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
			this.removed.AddRange(this);
			this.Items.Clear();
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
}
#endif