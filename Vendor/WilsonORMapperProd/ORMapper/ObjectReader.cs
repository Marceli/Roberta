//**************************************************************//
// Paul Wilson -- www.WilsonDotNet.com -- Paul@WilsonDotNet.com //
// Feel free to use and modify -- just leave these credit lines //
// I also always appreciate any other public credit you provide //
//**************************************************************//
using System;
using System.Collections;
using System.Data;
using Wilson.ORMapper.Query;

namespace Wilson.ORMapper
{
	/// <summary>
	///		ObjectReader uses a Forward-Only Read-Only Live Database Cursor
	///	</summary>
	///	<example>The following example shows how to use the ObjectReader to
	///	retrieve all Contacts.
	///	<code>
	///	public static ObjectSpace Manager; // See Initialization Example
	///
	///	// Retrieve ObjectReader Cursor of All Contact Object -- Custom Processing
	///	ObjectReader cursor = Manager.GetObjectReader(typeof(Contact), String.Empty);
	///	while (cursor.Read()) {
	///		Contact contact = (Contact) cursor.Current();
	///	}
	///	cursor.Close();
	///	</code>
	///	</example>
	public class ObjectReader : IObjectPage, IEnumerable, IEnumerator, IDisposable
	{
		private Internals.Context context;
		private bool firstLevel;
		private Type objectType;
		private int pageIndex;
		
		private int pageCount = 1;
		private int totalCount = 0;
		private object current;
		private bool hasEvents;
		private bool hasObjects;
		private bool firstRead;
		private IDataReader data = null;

		private SelectProcedure selectProcedure = null;
		private IDbCommand command = null;

		internal ObjectReader(Internals.Context context, ObjectQuery objectQuery, bool firstLevel) {
			this.context = context;
			this.firstLevel = firstLevel;
			this.objectType = objectQuery.ObjectType;
			this.pageIndex = objectQuery.PageIndex;
			this.hasEvents = context.Mappings[this.objectType.ToString()].HasEvents;

			Internals.Commands commands = context.Mappings.Commands(this.objectType.ToString());
			if (objectQuery.PageSize > 0 && !objectQuery.SkipCounts) {
				string selectCount = commands.RecordCount(objectQuery.WhereClause);
				this.totalCount = int.Parse(context.Connection.GetScalarValue(this.objectType, CommandInfo.GetCount, selectCount).ToString());
				double pages = (double) (1 + (this.totalCount - 1) / objectQuery.PageSize);
				this.pageCount = int.Parse(Math.Floor(pages).ToString());
			}
			this.data = context.Connection.GetDataReader(this.objectType, CommandInfo.Select, commands.Select(objectQuery));
			this.firstRead = true;
			if (this.data != null) this.hasObjects = this.data.Read();
			if (!this.hasObjects) this.Close();
		}

		// Jeff Lanning (jefflanning@gmail.com): Added for OPath support.
		internal ObjectReader(Internals.Context context, CompiledQuery query, bool firstLevel, object[] parameters) {
			this.context = context;
			this.firstLevel = firstLevel;
			this.objectType = query.ObjectType;
			this.pageIndex = 1;
			this.hasEvents = context.Mappings[this.objectType].HasEvents;

			this.data = context.Connection.GetDataReader(this.objectType, CommandInfo.Select, query, parameters);

			this.firstRead = true;
			if (this.data != null) this.hasObjects = this.data.Read();
			if (!this.hasObjects) this.Close();
		}

		internal ObjectReader(Internals.Context context, SelectProcedure selectProcedure, bool firstLevel) {
			this.context = context;
			this.firstLevel = firstLevel;
			this.objectType = selectProcedure.ObjectType;
			this.pageIndex = 1;
			this.hasEvents = context.Mappings[this.objectType.ToString()].HasEvents;
			
			this.selectProcedure = selectProcedure;
			this.data = context.Connection.GetDataReader(this.objectType, CommandInfo.Select, out this.command, selectProcedure.ProcedureName, selectProcedure.parameters);
			this.firstRead = true;
			if (this.data != null) this.hasObjects = this.data.Read();
			if (!this.hasObjects) this.Close();
		}
	
		/// <summary>
		/// Returns a string representing the type of this ObjectSet and the type of objects held.
		/// </summary>
		/// <returns>A string value.</returns>
		public override string ToString() {
			return this.GetType().ToString() + ": " + this.ObjectType.ToString();
		}

		#region IObjectPage Members

		/// <summary>The object type for this collection</summary>
		public Type ObjectType {
			get { return this.objectType; }
		}

		/// <summary>The current page number</summary>
		public int PageIndex {
			get { return this.pageIndex; }
		}

		/// <summary>The total number of pages</summary>
		public int PageCount {
			get { return this.pageCount; }
		}

		/// <summary>The total number of objects.</summary>
		public int TotalCount {
			get { return this.totalCount; }
		}

		#endregion

		/// <summary>The current object of the ObjectReader</summary>
		/// <returns>The current object instance</returns>
		public object Current() {
			return this.current;
		}
		
		/// <summary>Get the key for the current object</summary>
		/// <returns>The key for the current object</returns>
		public object ObjectKey() {
			return context.GetObjectKey(this.current);
		}

		/// <summary>Gets a value indicating whether the ObjectReader contains one or more objects.</summary>
		/// <value>True if the ObjectReader contains one or more objects; otherwise false.</value>
		public bool HasObjects {
			get { return this.hasObjects; }
		}

		/// <summary>Gets a value indicating whether the reader is closed</summary>
		/// <value>True if the ObjectReader is closed; otherwise, false.</value>
		public bool IsClosed {
			get { return (this.data == null ? true : this.data.IsClosed); }
		}

		/// <summary>Advances the ObjectReader to the next record.</summary>
		/// <returns>True if there are more objects; otherwise, false.</returns>
		public bool Read() {
			bool read;
			if (this.firstRead) {
				this.firstRead = false;
				read = this.hasObjects;
			}
			else {
				read = this.data.Read();
			}

			if (read) {
				if (this.firstLevel) Internals.LocalStore.Reset(this.objectType);
				Internals.EntityMap entity = this.context.Mappings[this.ObjectType];
				Type instanceType = this.ObjectType;
				if (entity.SubTypes.Count > 0) {
					string typeValue = this.data[entity.TypeField].ToString();
					if (entity.SubTypes.ContainsKey(typeValue)) {
						instanceType = Internals.EntityMap.GetType((string)entity.SubTypes[typeValue]);
					}
				}
				this.current = Activator.CreateInstance(instanceType, true);
				Internals.Instance instance = new Internals.Instance(this.context, this.current, this.data);
				if (this.hasEvents) {
					((IObjectNotification) this.current).OnMaterialized(this.data);
				}
				if (this.context.Mappings[objectType.ToString()].AutoTrack) {
					this.context.StartTracking(instance);
				}
			}
			else {
				this.current = null;
				this.Close();
			}
			return read;
		}

		/// <summary>Close the ObjectReader</summary>
		public void Close() {
			if (this.data != null && !this.data.IsClosed) {
				if (this.command != null && this.context.Connection.HasOutputParameters(this.command, this.selectProcedure.parameters)) {
					while (this.data.Read());
					this.data.Close();
					this.context.Connection.SetOutputParameters(this.command, this.selectProcedure.parameters);
				}
				else {
					this.data.Close();
				}
			}
			GC.SuppressFinalize(this);
		}

		#region IEnumerable Members

		/// <summary>
		/// Returns an enumerator that can iterate through the reader.
		/// </summary>
		/// <returns>An IEnumerator for the entire reader.</returns>
		public IEnumerator GetEnumerator() {
			return (IEnumerator) this;
		}

		#endregion

		#region IEnumerator Members

		
		/// <summary>Not Supported. ObjectReader Enumerator is Forward-Only</summary>
		public void Reset() {
			throw new ORMapperException("ObjectReader: Enumerator is Forward-Only");
		}

		object IEnumerator.Current {
			get {	return this.current; }
		}

		/// <summary>Advances the ObjectReader to the next record.</summary>
		/// <returns>True if there are more objects; otherwise, false.</returns>
		public bool MoveNext() {
			return this.Read();
		}

		#endregion

		#region IDisposable Members

		/// <summary>Close the ObjectReader</summary>
		public void Dispose() {
			this.Close();
		}

		/// <summary>
		/// Frees resources and perform other cleanup operations before this instance is reclaimed by garbage collection.
		/// </summary>
		~ObjectReader() {
			// Error Handling added by Ken Muse (http://www.MomentsFromImpact.com)
			try {
				if (this.data != null && !this.data.IsClosed) this.data.Close();
			}
			catch {
				// Do nothing. This happens when the underlying handle has already been released in a GC cycle.
			}
		}

		#endregion
	}
}
