//**************************************************************//
// Paul Wilson -- www.WilsonDotNet.com -- Paul@WilsonDotNet.com //
// Feel free to use and modify -- just leave these credit lines //
// I also always appreciate any other public credit you provide //
//**************************************************************//
using System;

namespace Wilson.ORMapper
{
	/// <summary>
	///     The ObjectQuery class is used to create more detailed
	///     queries, including paging.
	/// </summary>
	/// <example>The following example gets a paged collection of Contacts.
	///		<code>
	///	public static ObjectSpace Manager; // See Initialization Example
	///
	/// QueryHelper helper = Manager.QueryHelper;
	/// string where = helper.GetExpression("Contact.Company", "WilsonDotNet.com");
	/// string sort = helper.GetFieldName("Contact.Name") + " ASC";
	/// int pageSize = 25;
	/// int currentPage = 3;
    ///
	///	ObjectQuery pageQuery = new ObjectQuery(typeof(Contact), where, sort, pageSize, currentPage);
	///	ObjectSet pageContacts = Manager.GetObjectSet(pageQuery);
	///		</code>
	/// </example>
	[Serializable()]
	public class ObjectQuery
	{
		private Type objectType;
		private string whereClause;
		private string sortClause;
		internal string manyTable = null;
		private int pageSize = 0;
		private int pageIndex = 1;
		private bool skipCounts = false;

		/// <summary>The object type used for this query</summary>
		public Type ObjectType {
			get { return this.objectType; }
		}

		/// <summary>The where clause used for this query</summary>
		public string WhereClause {
			get { return this.whereClause; }
		}

		/// <summary>The sort clause used for this query</summary>
		public string SortClause {
			get { return this.sortClause; }
		}

		/// <summary>The number of records in each page</summary>
		public int PageSize {
			get { return this.pageSize; }
		}

		/// <summary>The current page number</summary>
		public int PageIndex {
			get { return this.pageIndex; }
		}

		/// <summary>Skip PageCount and TotalCount</summary>
		public bool SkipCounts {
			get { return this.skipCounts; }
		}

		/// <summary>
		///     Creates a ObjectQuery to return a full collection
		/// </summary>
		/// <param name="objectType" type="System.Type">
		///     The type of object to retrieve
		/// </param>
		/// <param name="whereClause" type="string">
		///		The SQL where clause to use when retrieving data
		/// </param>
		/// <param name="sortClause" type="string">
		///     The SQL sort clause to use when retrieving data
		/// </param>
		public ObjectQuery(Type objectType, string whereClause, string sortClause) {
			this.objectType = objectType;
			this.whereClause = (whereClause == null ? String.Empty : whereClause);
			this.sortClause = (sortClause == null ? String.Empty : sortClause);
			this.skipCounts = false;
		}

		internal ObjectQuery(Type objectType, string whereClause, string sortClause, string manyTable) {
			this.objectType = objectType;
			this.whereClause = (whereClause == null ? String.Empty : whereClause);
			this.sortClause = (sortClause == null ? String.Empty : sortClause);
			this.manyTable = manyTable;
			this.skipCounts = false;
		}

		/// <summary>
		///     Creates a ObjectQuery to return a paged collection
		/// </summary>
		/// <param name="objectType" type="System.Type">
		///     The type of object to retrieve
		/// </param>
		/// <param name="whereClause" type="string">
		///		The SQL where clause to use when retrieving data
		/// </param>
		/// <param name="sortClause" type="string">
		///     The SQL sort clause to use when retrieving data
		/// </param>
		/// <param name="pageSize" type="int">
		///     The number of records in each page
		/// </param>
		/// <param name="pageIndex" type="int">
		///     The page number to return
		/// </param>
		public ObjectQuery(Type objectType, string whereClause, string sortClause, int pageSize, int pageIndex)
			: this(objectType, whereClause, sortClause)
		{
			this.pageSize = (pageSize < 0 ? 0 : pageSize);
			this.pageIndex = (pageIndex < 1 ? 1 : pageIndex);
			this.skipCounts = false;
		}

		/// <summary>
		///     Creates a ObjectQuery to return a paged collection
		/// </summary>
		/// <param name="objectType" type="System.Type">
		///     The type of object to retrieve
		/// </param>
		/// <param name="whereClause" type="string">
		///		The SQL where clause to use when retrieving data
		/// </param>
		/// <param name="sortClause" type="string">
		///     The SQL sort clause to use when retrieving data
		/// </param>
		/// <param name="pageSize" type="int">
		///     The number of records in each page
		/// </param>
		/// <param name="pageIndex" type="int">
		///     The page number to return
		/// </param>
		/// <param name="skipCounts" type="bool">
		///     Skip PageCount and TotalCount
		/// </param>
		public ObjectQuery(Type objectType, string whereClause, string sortClause, int pageSize, int pageIndex, bool skipCounts)
			: this(objectType, whereClause, sortClause) {
			this.pageSize = (pageSize < 0 ? 0 : pageSize);
			this.pageIndex = (pageIndex < 1 ? 1 : pageIndex);
			this.skipCounts = skipCounts;
		}
	}
}
