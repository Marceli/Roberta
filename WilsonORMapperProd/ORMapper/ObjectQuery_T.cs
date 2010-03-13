//**************************************************************//
// Paul Wilson -- www.WilsonDotNet.com -- Paul@WilsonDotNet.com //
// Feel free to use and modify -- just leave these credit lines //
// I also always appreciate any other public credit you provide //
//**************************************************************//
#if DOTNETV2
using System;

namespace Wilson.ORMapper
{
	/// <summary>
	///     The ObjectQuery class is used to create more detailed
	///     queries, including paging.
	/// </summary>
	/// <example>The following example gets a paged collection of Contacts.
	///		<code>
	/// <![CDATA[
	///	public static ObjectSpace Manager; // See Initialization Example
	///
	/// QueryHelper helper = Manager.QueryHelper;
	/// string where = helper.GetExpression("Contact.Company", "WilsonDotNet.com");
	/// string sort = helper.GetFieldName("Contact.Name") + " ASC";
	/// int pageSize = 25;
	/// int currentPage = 3;
    ///
	///	ObjectQuery<Contact> pageQuery = new ObjectQuery<Contact>(where, sort, pageSize, currentPage);
	///	ObjectSet<Contact> pageContacts = Manager.GetObjectSet<Contact>(pageQuery);
	/// ]]>
	///		</code>
	/// </example>
	[Serializable()]
	public class ObjectQuery<T> : ObjectQuery
	{
		/// <summary>
		///     Creates a ObjectQuery to return a full collection
		/// </summary>
		/// <param name="whereClause" type="string">
		///		The SQL where clause to use when retrieving data
		/// </param>
		/// <param name="sortClause" type="string">
		///     The SQL sort clause to use when retrieving data
		/// </param>
		public ObjectQuery(string whereClause, string sortClause)
			: base(typeof(T), whereClause, sortClause) {}

		internal ObjectQuery(string whereClause, string sortClause, string manyTable)
			: base(typeof(T), whereClause, sortClause, manyTable) { }

		/// <summary>
		///     Creates a ObjectQuery to return a paged collection
		/// </summary>
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
		public ObjectQuery(string whereClause, string sortClause, int pageSize, int pageIndex)
			: base(typeof(T), whereClause, sortClause, pageSize, pageIndex) { }

		/// <summary>
		///     Creates a ObjectQuery to return a paged collection
		/// </summary>
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
		public ObjectQuery(string whereClause, string sortClause, int pageSize, int pageIndex, bool skipCounts)
			: base(typeof(T), whereClause, sortClause, pageSize, pageIndex, skipCounts) { }
	}
}
#endif
