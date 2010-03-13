//------------------------------------------------------------------------------
// <autogenerated>
//		This code was generated by a CodeSmith Template.
// </autogenerated>
//------------------------------------------------------------------------------

using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Reflection;
using Wilson.ORMapper;

namespace Bll
{
	/// <summary>
	/// The DataManager class is the singleton instance of the ObjectSpace class
	/// </summary>
	public sealed class Dm
	{
		private const string MAPPING_FILE = "Mapping.config";
        private const string CONNECTION_NAME = "Roberta.Bll.DataManager";
        public const string EMPTY_STR = "";
		
		/// <summary>The application connection string read from app.config</summary>		
		/// <example>
		/// Add the following key to the "connectionStrings" section of your config:
		/// <code><![CDATA[
		/// 	<configuration>
		/// 		<connectionStrings>
		/// 			<add name="Vulcan.Repozytorium.DAL.DataManager" 
		/// 				connectionString="Data Source=(local);Initial Catalog=DATABASE;Integrated Security=True"
		/// 				providerName="System.Data.SqlClient" />
		/// 		</connectionStrings>
		/// 	</configuration>
		/// ]]></code>
		/// </example>
		public static readonly string ConnectionString = GetDefaultConnectionString();
		/// <summary>The singletion instance of an ObjectSpace Class</summary>
		public static readonly ObjectSpace ObjectSpace = GetDefaultInstance();

        #region DAL Methods

        ///<summary>Retrieve an instance of this class using its ID from the persistence store</summary>
        public static T Retrieve<T>(int id)
        {
            return ObjectSpace.GetObject<T>(id);
        }
        ///<summary>Retrieve all instances of this class from the persistence store</summary>
       
        public static Collection<T> RetrieveAll<T>()
        {
            return RetrieveAll<T>(string.Empty);
        }

        ///<summary>Retrieve all instances of this class from the persistence store</summary>
        ///<param name="sortClause">The SQL sort statement</param>
        public static Collection<T> RetrieveAll<T>(string sortClause)
        {
            ObjectQuery<T> query = new ObjectQuery<T>(string.Empty, sortClause);
            return ObjectSpace.GetCollection(query);
        }
        ///<summary>Retrieve instances of this class from the persistence store based on the ObjectQuery</summary>
        ///<param name="query">The object query to filter the records</param>
       
        public static Collection<T> RetrieveOPath<T>(OPathQuery<T> query)
        {
            return ObjectSpace.GetCollection<T>(query);
        }
        public static Collection<T> RetrieveOPath<T>(OPathQuery<T> query,object[] parameters)
        {
            return ObjectSpace.GetCollection<T>(query, parameters);
        }
        public static T GetObject<T>(string whereOPathExpression, object[] parameters)
        {
            return ObjectSpace.GetObject(new OPathQuery<T>(whereOPathExpression), parameters);
        }
        public static T GetObject<T>(int id)
        {
            return ObjectSpace.GetObject<T>(id);
        }
        public static ObjectSet<T> RetrieveQuery<T>(ObjectQuery<T> query)
        {
            return ObjectSpace.GetObjectSet<T>(query);
        }


       

       


/*
        ///<summary>Retrieve the first instance of this class using the where clause</summary>
        ///<param name="whereClause">The SQL where clause to filter the records</param>
        public static T RetrieveScalarSql<T>(string whereClause)
        {
            return RetrieveScalarSql<T>(whereClause, string.Empty);
        }

        ///<summary>Retrieve the first instance of this class using the where clause</summary>
        ///<param name="whereClause">The SQL where clause to filter the records</param>
        ///<param name="sortClause">The SQL sort statement</param>
        public static T RetrieveScalarSql<T>(string whereClause, string sortClause)
        {
            int pageCount;
            Collection<T> pageSet = RetrievePageSql<T>(whereClause, sortClause, 1, 1, out pageCount);
            if (pageSet != null && pageSet.Count > 0)
                return pageSet[0];
            else
                return default(T);
        }



        ///<summary>Retrieve a paged collection of instances of this class from the persistence store</summary>
        ///<param name="sortOPathExpr">The OPath sort expression</param>
        ///<param name="maximumRows">The number of records in each page</param>
        ///<param name="startRowIndex">The row index to return</param>
        public static Collection<T> RetrievePage<T>(string sortOPathExpr, int maximumRows, int startRowIndex)
        {
            return RetrievePage<T>(string.Empty, sortOPathExpr, maximumRows, startRowIndex);
        }

        ///<summary>Retrieve a paged collection of instances of this class from the persistence store</summary>
        ///<param name="whereOPathExpr">The OPath where expression to filter the records</param>
        ///<param name="sortOPathExpr">The OPath sort expression</param>
        ///<param name="maximumRows">The number of records in each page</param>
        ///<param name="startRowIndex">The row index to return</param>
        public static Collection<T> RetrievePage<T>(string whereOPathExpr, string sortOPathExpr, int maximumRows,
                                                 int startRowIndex)
        {
            return RetrievePage<T>(whereOPathExpr, string.Empty, sortOPathExpr, maximumRows, startRowIndex);
        }

        ///<summary>Retrieve a paged collection of instances of this class from the persistence store</summary>
        ///<param name="whereOPathExpr">The OPath where expression to filter the records</param>
        ///<param name="defSortOPathExpr">Default OPath sort expression, used when sort expression is null or empty</param>
        ///<param name="sortOPathExpr">The OPath sort expression</param>
        ///<param name="maximumRows">The number of records in each page</param>
        ///<param name="startRowIndex">The row index to return</param>
        public static Collection<T> RetrievePage<T>(string whereOPathExpr, string defSortOPathExpr, string sortOPathExpr,
                                                 int maximumRows, int startRowIndex)
        {
            QueryHelper helper = ObjectSpace.QueryHelper;

            string sort = string.Empty;
            if (sortOPathExpr != null && sortOPathExpr.Length > 0)
                sort = helper.GetExpression(sortOPathExpr);
            else if (defSortOPathExpr != null && defSortOPathExpr.Length > 0)
                sort = helper.GetExpression(defSortOPathExpr);

            string where = (whereOPathExpr != null && whereOPathExpr.Length > 0)
                               ? helper.GetExpression(whereOPathExpr)
                               : string.Empty;
            int pageIndex = DalTools.GetPageIndex(maximumRows, startRowIndex);

            int pageCount;
            return RetrievePageSql<T>(where, sort, maximumRows, pageIndex, out pageCount);
        }

        ///<summary>Retrieve a paged collection of instances of this class from the persistence store</summary>
        ///<param name="whereClause">The SQL where clause to filter the records</param>
        ///<param name="sortClause">The SQL sort statement</param>
        ///<param name="maximumRows">The number of records in each page</param>
        ///<param name="startRowIndex">The row index to return</param>
        public static Collection<T> RetrievePageSql<T>(string whereClause, string sortClause, int maximumRows,
                                                    int startRowIndex)
        {
            int pageIndex = DalTools.GetPageIndex(maximumRows, startRowIndex);

            int pageCount;
            return RetrievePageSql<T>(whereClause, sortClause, maximumRows, pageIndex, out pageCount);
        }

        ///<summary>Retrieve a paged collection of instances of this class from the persistence store</summary>
        ///<param name="whereClause">The SQL where clause to filter the records</param>
        ///<param name="sortClause">The SQL sort statement</param>
        ///<param name="pageSize">The number of records in each page</param>
        ///<param name="pageIndex">The page index to return</param>
        ///<param name="pageCount">The total number of pages</param>
        private static Collection<T> RetrievePageSql<T>(string whereClause, string sortClause, int pageSize, int pageIndex,
                                                     out int pageCount)
        {
            ObjectQuery<T> query = new ObjectQuery<T>(whereClause, sortClause, pageSize, pageIndex);
            ObjectSet<T> pageSet = RetrieveQuery(query);
            pageCount = pageSet.PageCount;
            return pageSet;
        }

        ///<summary>Retrieve a paged collection of instances of this class from the persistence store</summary>
        ///<param name="whereClause">The SQL where clause to filter the records</param>
        /// <param name="defSortClause">Default SQL sort expression, used when sort expression is null or empty</param>
        ///<param name="sortClause">The SQL sort statement</param>
        ///<param name="maximumRows">The number of records in each page</param>
        ///<param name="startRowIndex">The row index to return</param>
        public static Collection<T> RetrievePageSql<T>(string whereClause, string defSortClause, string sortClause,
                                                    int maximumRows, int startRowIndex)
        {
            string sort = sortClause;
            if (string.IsNullOrEmpty(sort))
                sort = defSortClause;

            int pageIndex = DalTools.GetPageIndex(maximumRows, startRowIndex);

            int pageCount;
            return RetrievePageSql<T>(whereClause, sort, maximumRows, pageIndex, out pageCount);
        }

        ///<summary>Returns the number of all records in table.</summary>
        public static int TotalRecordsCount<T>()
        {
            return RecordsCount<T>(string.Empty);
        }

        ///<summary>Retrieve a paged collection of instances of this class from the persistence store</summary>
        ///<param name="whereOPathExpr">The OPath where expression to filter the records</param>
        public static int RecordsCount<T>(string whereOPathExpr)
        {
            return RecordsCount<T>(whereOPathExpr, string.Empty);
        }

        ///<summary>Retrieve a paged collection of instances of this class from the persistence store</summary>
        ///<param name="whereOPathExpr">The OPath where expression to filter the records</param>
        ///<param name="defSortOPathExpr">Default OPath sort expression</param>		
        public static int RecordsCount<T>(string whereOPathExpr, string defSortOPathExpr)
        {
            string where = string.Empty;

            QueryHelper helper = ObjectSpace.QueryHelper;

            if (!string.IsNullOrEmpty(whereOPathExpr))
                where = string.Format(" where {0}", helper.GetExpression(whereOPathExpr));

            string tableName = helper.GetTableName(typeof(T).Name);

            string cmd = string.Format("select count(*) from {1}{0}", where, tableName);
            return (int)DataManager.ObjectSpace.ExecuteScalar(cmd);
        }

        ///<summary>Retrieve a paged collection of instances of this class from the persistence store</summary>
        ///<param name="whereClause">The SQL where clause to filter the records</param>
        public static int RecordsCountSql<T>(string whereClause)
        {
            return RecordsCountSql<T>(whereClause, string.Empty);
        }

        ///<summary>Retrieve a paged collection of instances of this class from the persistence store</summary>
        ///<param name="whereClause">The SQL where clause to filter the records</param>
        ///<param name="sortClause">The SQL sort statement</param>
        public static int RecordsCountSql<T>(string whereClause, string defSortClause)
        {
            string where = string.Empty;

            if (whereClause != null && whereClause.Length > 0)
                where = string.Format(" where {0}", whereClause);

            QueryHelper helper = ObjectSpace.QueryHelper;
            string tableName = helper.GetTableName(typeof(T).Name);

            string cmd = string.Format("select count(*) from {1}{0}", where, tableName);
            return (int)ObjectSpace.ExecuteScalar(cmd);
        }

        ///<summary>Retrieve instances of this class from the persistence store based on the where clause</summary>
        ///<param name="whereClause">The SQL where clause to filter the records</param>
        public static Collection<T> RetrieveQuerySql<T>(string whereClause)
        {
            return RetrieveQuerySql<T>(whereClause, string.Empty);
        }

        ///<summary>Retrieve instances of this class from the persistence store based on the where clause</summary>
        ///<param name="whereClause">The SQL where clause to filter the records</param>
        ///<param name="sortClause">The SQL sort statement</param>
        public static Collection<T> RetrieveQuerySql<T>(string whereClause, string sortClause)
        {
            ObjectQuery<T> query = new ObjectQuery<T>(whereClause, sortClause);
            return RetrieveQuery(query);
        }

*/
        ///<summary>Delete instances from the persistence store based on the where clause</summary>
        ///<param name="whereClause">The SQL where clause of rows to delete</param>
        public static int DeleteSql<T>(string whereClause)
        {
            return DeleteSql<T>(null, whereClause);
        }

        ///<summary>Delete instances from the persistence store based on the where clause</summary>
        ///<param name="transaction">An instance of a Wilson.ORMapper.Transaction to perform operation with.</param>
        ///<param name="whereClause">The SQL where clause of rows to delete</param>
        public static int DeleteSql<T>(Transaction transaction, string whereClause)
        {
            if (transaction == null)
                return ObjectSpace.ExecuteDelete(typeof(T), whereClause);
            else
                return transaction.ExecuteDelete(typeof(T), whereClause);
        }

        ///<summary>Delete instance from the persistence store based on primary key(s)</summary>
        public static int DeleteByKey<T>(int id)
        {
            return DeleteByKey<T>(null, id);
        }

        ///<summary>Delete instance from the persistence store based on primary key(s) using a transaction</summary>
        ///<param name="transaction">An instance of a Wilson.ORMapper.Transaction to perform operation with.</param>
        public static int DeleteByKey<T>(Transaction transaction, int id)
        {
            return DeleteSql<T>(transaction, string.Format("id={0}", id));
        }
/*
        ///<summary>Updates instances from the persistence store based on the where clause and uses the Update clause to set the values</summary>
        ///<param name="whereClause">The SQL where clause of rows to update</param>
        ///<param name="updateClause">The SQL update clause for values to set</param>
        public static int UpdateSql<T>(string whereClause, string updateClause)
        {
            return DataManager.ObjectSpace.ExecuteUpdate(typeof(T), whereClause, updateClause);
        }

        ///<summary>Updates instances from the persistence store based on the where clause and uses the Update clause to set the values</summary>
        ///<param name="transaction">An instance of a Wilson.ORMapper.Transaction to perform operation with.</param>
        ///<param name="whereClause">The SQL where clause of rows to update</param>
        ///<param name="updateClause">The SQL update clause for values to set</param>
        public static int UpdateWithTransactionSql<T>(Transaction transaction, string whereClause, string updateClause)
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");

            return transaction.ExecuteUpdate(typeof(T), whereClause, updateClause);
        }
*/
        #endregion

        private Dm()
		{ }

		private static ObjectSpace GetDefaultInstance()
		{
			Assembly assembly = Assembly.GetAssembly(typeof(Dm));
			
            foreach ( string name in assembly.GetManifestResourceNames() )
                if ( name.EndsWith(MAPPING_FILE) )
			        using (Stream mappingStream = assembly.GetManifestResourceStream(name))
			        {
        				return new ObjectSpace(
                        mappingStream, ConnectionString, Provider.MsSql, 20, 8);
        			}

            return null;
		}

        private static string GetDefaultConnectionString()
		{
			ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[CONNECTION_NAME];
			return settings.ConnectionString;
		}

	   
	}
}

