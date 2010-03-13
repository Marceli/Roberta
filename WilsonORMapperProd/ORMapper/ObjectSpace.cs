//**************************************************************//
// Paul Wilson -- www.WilsonDotNet.com -- Paul@WilsonDotNet.com //
// Feel free to use and modify -- just leave these credit lines //
// I also always appreciate any other public credit you provide //
//**************************************************************//
// Thanks to Paul Welter (http://www.LoreSoft.com)
//   for XmlComments and NDoc and Improved Exception Handling
// Thanks to Jeff Lanning (jefflanning@gmail.com)
//   for the OPath Query Engine included as of v4.0.
using System;
using System.Collections;
using System.Data;
using System.IO;
using Wilson.ORMapper.Query;

namespace Wilson.ORMapper
{
	/// <summary>
	///     The ObjectSpace class is the persistence engine that all work occurs through.
	/// </summary>
	/// <example>The following example shows how to use ObjectSpace to do some basic CRUD operations.
	///	<code>
	///	public static ObjectSpace Manager; // See Initialization Example
	///	
	///	// Create Object using ObjectSpace's GetObject method
	///	Contact contact = (Contact) Manager.GetObject(typeof(Contact));
	///	contact.Name = "Wilson, Paul";
	///	contact.Company = "WilsonDotNet.com";
	///	Manager.PersistChanges(contact); // Insert into Database
	///	int id = contact.Id; // Auto Identity assigned by Database
	///	
	///	// Retrieve Object by Primary Key using GetObject method
	///	contact = (Contact) Manager.GetObject(typeof(Contact), id);
	///	
	///	// Update Object changes using the PersistChanges method
	///	contact.Name = "Paul Wilson";
	///	contact.Company = ""; // Configured to be the NullValue
	///	Manager.PersistChanges(contact); // Update the Database
    ///	
	///	// Delete Object using MarkForDeletion and PersistChanges
	///	Manager.MarkForDeletion(contact); // Mark for Deletion
	///	Manager.PersistChanges(contact); // Delete from Database
	///	</code>
	/// </example>
	public class ObjectSpace : IDisposable
	{
		//Performance Timer if Necessary
		//public static long Ticks = 0;
		//long ticks = DateTime.Now.Ticks;
		//ObjectSpace.Ticks += DateTime.Now.Ticks - ticks;

		internal Internals.Context context;

		/// <summary>
		///     Use for In-Process Desktop Applications
		/// </summary>
		/// <param name="mappingFile" type="string">
		///     A path to the mapping file
		/// </param>
		/// <param name="connectString" type="string">
		///     A valid connection string for the database
		/// </param>
		/// <param name="providerType" type="Wilson.ORMapper.Provider">
		///     The database provider type
		/// </param>
		/// <example> Create an instance of ObjectSpace for a Windows app using MS SQL
		///	<code>
		///	public static ObjectSpace Manager;
		///	
		///	string mappingFile = @"C:\Data\Examples\Contacts\Mappings.config";
		///	string connectMsSql = "Server=(local);Database=Contacts;Trusted_Connection=True;";
		///	
		///	Manager = new ObjectSpace(mappingFile, connectMsSql, Provider.MsSql);
		///	</code>
		/// </example>
		public ObjectSpace(string mappingFile, string connectString, Provider providerType)
			: this(mappingFile, connectString, providerType, 0, 5) {}

		/// <summary>
		///     Use for In-Process Desktop Applications
		/// </summary>
		/// <param name="mappingFile" type="string">
		///     A path to the mapping file
		/// </param>
		/// <param name="connectString" type="string">
		///     A valid connection string for the database
		/// </param>
		/// <param name="customProvider" type="Wilson.ORMapper.CustomProvider">
		///     The database custom provider definition
		/// </param>
		public ObjectSpace(string mappingFile, string connectString, CustomProvider customProvider)
			: this(mappingFile, connectString, customProvider, 0, 5) {}

		/// <summary>
		///     Use for In-Process Desktop Applications
		/// </summary>
		/// <param name="mappingStream" type="Stream">
		///     A stream of the mapping file
		/// </param>
		/// <param name="connectString" type="string">
		///     A valid connection string for the database
		/// </param>
		/// <param name="providerType" type="Wilson.ORMapper.Provider">
		///     The database provider type
		/// </param>
		/// <example> Create an instance of ObjectSpace for a Windows app using MS SQL.
		/// Note that the mapping file is an embedded resource.
		///	<code>
		///	public static ObjectSpace Manager;
		///	
		///	Stream mappingStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Mappings.config");
		///	string connectMsSql = "Server=(local);Database=Contacts;Trusted_Connection=True;";
		///	
		///	Manager = new ObjectSpace(mappingStream, connectMsSql, Provider.MsSql);
		///	</code>
		/// </example>
		public ObjectSpace(Stream mappingStream, string connectString, Provider providerType)
			: this(mappingStream, connectString, providerType, 0, 5) {}

		/// <summary>
		///     Use for In-Process Desktop Applications
		/// </summary>
		/// <param name="mappingStream" type="Stream">
		///     A stream of the mapping file
		/// </param>
		/// <param name="connectString" type="string">
		///     A valid connection string for the database
		/// </param>
		/// <param name="customProvider" type="Wilson.ORMapper.CustomProvider">
		///     The database custom provider definition
		/// </param>
		public ObjectSpace(Stream mappingStream, string connectString, CustomProvider customProvider)
			: this(mappingStream, connectString, customProvider, 0, 5) {}

		/// <summary>
		///     Use for Web and Remote Server Applications
		/// </summary>
		/// <param name="mappingFile" type="string">
		///     A path to the mapping file
		/// </param>
		/// <param name="connectString" type="string">
		///     A valid connection string for the database
		/// </param>
		/// <param name="providerType" type="Wilson.ORMapper.Provider">
		///     The database provider type
		/// </param>
		/// <param name="sessionMinutes" type="int">
		///     The number of minutes to keep data in a session
		/// </param>
		/// <param name="cleanupMinutes" type="int">
		///     The number of minutes before the clean up timer runs
		/// </param>
		/// <example> Create an instance of ObjectSpace for a web or distributed application.
		/// Note that entity values are tracked for 20 minutes, like web sessions.
		/// Also note a timer is activated every 5 minutes to cleanup old values.
		///	<code>
		///	public static ObjectSpace Manager;
		///	
		///	string mappingFile = @"C:\Data\Examples\Contacts\Mappings.config";
		///	string connectMsSql = "Server=(local);Database=Contacts;Trusted_Connection=True;";
		///	
		///	Manager = new ObjectSpace(mappingFile, connectMsSql, Provider.MsSql, 20, 5);
		///	</code>
		/// </example>
		public ObjectSpace(string mappingFile, string connectString, Provider providerType,
			int sessionMinutes, int cleanupMinutes)
		{
			this.context = new Internals.Context(mappingFile, connectString,
				new CustomProvider(providerType), sessionMinutes, cleanupMinutes);
		}

		/// <summary>
		///     Use for Web and Remote Server Applications
		/// </summary>
		/// <param name="mappingFile" type="string">
		///     A path to the mapping file
		/// </param>
		/// <param name="connectString" type="string">
		///     A valid connection string for the database
		/// </param>
		/// <param name="customProvider" type="Wilson.ORMapper.CustomProvider">
		///     The database custom provider definition
		/// </param>
		/// <param name="sessionMinutes" type="int">
		///     The number of minutes to keep data in a session
		/// </param>
		/// <param name="cleanupMinutes" type="int">
		///     The number of minutes before the clean up timer runs
		/// </param>
		public ObjectSpace(string mappingFile, string connectString, CustomProvider customProvider,
			int sessionMinutes, int cleanupMinutes)
		{
			this.context = new Internals.Context(mappingFile, connectString,
				customProvider, sessionMinutes, cleanupMinutes);
		}

		/// <summary>
		///     Use for Web and Remote Server Applications
		/// </summary>
		/// <param name="mappingStream" type="Stream">
		///     A stream of the mapping file
		/// </param>
		/// <param name="connectString" type="string">
		///     A valid connection string for the database
		/// </param>
		/// <param name="providerType" type="Wilson.ORMapper.Provider">
		///     The database provider type
		/// </param>
		/// <param name="sessionMinutes" type="int">
		///     The number of minutes to keep data in a session
		/// </param>
		/// <param name="cleanupMinutes" type="int">
		///     The number of minutes before the clean up timer runs
		/// </param>
		public ObjectSpace(Stream mappingStream, string connectString, Provider providerType,
			int sessionMinutes, int cleanupMinutes)
		{
			this.context = new Internals.Context(mappingStream, connectString,
				new CustomProvider(providerType),	sessionMinutes, cleanupMinutes);
		}

		/// <summary>
		///     Use for Web and Remote Server Applications
		/// </summary>
		/// <param name="mappingStream" type="Stream">
		///     A stream of the mapping file
		/// </param>
		/// <param name="connectString" type="string">
		///     A valid connection string for the database
		/// </param>
		/// <param name="customProvider" type="Wilson.ORMapper.CustomProvider">
		///     The database custom provider definition
		/// </param>
		/// <param name="sessionMinutes" type="int">
		///     The number of minutes to keep data in a session
		/// </param>
		/// <param name="cleanupMinutes" type="int">
		///     The number of minutes before the clean up timer runs
		/// </param>
		public ObjectSpace(Stream mappingStream, string connectString, CustomProvider customProvider,
			int sessionMinutes, int cleanupMinutes)
		{
			this.context = new Internals.Context(mappingStream, connectString,
				customProvider, sessionMinutes, cleanupMinutes);
		}

		private ObjectSpace(Internals.Context context) {
			this.context = new Internals.Context(context);
		}

		/// <summary>
		///     Create New ObjectSpace Context for Isolation
		/// </summary>
		public ObjectSpace IsolatedContext {
			get { return new ObjectSpace(this.context); }
		}

		/// <summary>
		///     Get QueryHelper to Help Build Expressions
		/// </summary>
		public QueryHelper QueryHelper {
			get { return new QueryHelper(this.context.Mappings); }
		}

		/// <summary>
		///     Set an Interceptor to Handle all Database Commands
		///     for Logging or modifying Sql Commands as needed.
		///     Set to null to stop interception for performance.
		/// </summary>
		/// <param name="interceptor">An implementation of IInterceptCommand.</param>
		public void SetInterceptor(IInterceptCommand interceptor) {
			this.context.Connection.SetInterceptor(interceptor);
		}

		/// <summary>
		///     Create New Object and Start Tracking
		/// </summary>
		/// <param name="objectType" type="System.Type">
		///     The type of object to create
		/// </param>
		/// <returns>
		///     A new instance of the object
		/// </returns>
		public object GetObject(Type objectType) {
			return this.context.GetObject(objectType);
		}

		/// <summary>
		///     Get Existing Object and Start Tracking
		/// </summary>
		/// <param name="objectType" type="System.Type">
		///    The type of object to create
		/// </param>
		/// <param name="objectKey" type="object">
		///    The key of the object to get
		/// </param>
		/// <returns>
		///     A new instance of the object
		/// </returns>
		public object GetObject(Type objectType, object objectKey) {
			if (this.context.Mappings[objectType.ToString()].KeyType == KeyType.None) {
				throw new ORMapperException("ObjectSpace: Cannot Get None KeyTypes with Key - " + objectType.ToString());
			}
			return this.context.GetObject(objectType, objectKey, true);
		}

#if DOTNETV2
		/// <summary>
		///     Create New Object and Start Tracking
		/// </summary>
		/// <returns>
		///     A new instance of the object
		/// </returns>
		public T GetObject<T>() {
			return (T) this.context.GetObject(typeof(T));
		}

		/// <summary>
		///     Get Existing Object and Start Tracking
		/// </summary>
		/// <param name="objectKey" type="object">
		///    The key of the object to get
		/// </param>
		/// <returns>
		///     A new instance of the object
		/// </returns>
		public T GetObject<T>(object objectKey) {
			Type objectType = typeof(T);
			if (this.context.Mappings[objectType.ToString()].KeyType == KeyType.None) {
				throw new ORMapperException("ObjectSpace: Cannot Get None KeyTypes with Key - " + objectType.ToString());
			}
			return (T) this.context.GetObject(objectType, objectKey, true);
		}
#endif

		/// <summary>
		///     Get Primary Key of an Entity Object
		/// </summary>
		/// <param name="entityObject" type="object">
		///    The object to look up the key from
		/// </param>
		/// <returns>
		///     The key for the object
		/// </returns>
		public object GetObjectKey(object entityObject) {
			if (this.context.Mappings[entityObject.GetType().ToString()].KeyType == KeyType.None) {
				throw new ORMapperException("ObjectSpace: Cannot Get Key for None KeyTypes - " + entityObject.GetType().ToString());
			}
			try { return this.context.GetObjectKey(entityObject);	}
			catch {	return null; }
		}

		/// <summary>
		///     Get Current State of an Entity Object
		/// </summary>
		/// <param name="entityObject" type="object">
		///    The object to get the state from
		/// </param>
		/// <returns>
		///     A Wilson.ORMapper.ObjectState value
		/// </returns>
		public ObjectState GetObjectState(object entityObject) {
			try { return this.context[entityObject].State; }
			catch { return ObjectState.Unknown; }
		}

		/// <summary>
		///     Get Record Count given Where Clause
		/// </summary>
		/// <param name="objectType" type="System.Type">
		///    The type of object to retrieve
		/// </param>
		/// <param name="whereClause" type="string">
		///    The SQL where clause to use when retrieving data
		/// </param>
		/// <returns>
		///     The number of rows found
		/// </returns>
		public int GetObjectCount(Type objectType, string whereClause) {
			ObjectQuery query = new ObjectQuery(objectType, whereClause, String.Empty);
			return this.GetObjectCount(query);
		}
		
		/// <summary>
		///     Get Record Count given Where Clause
		/// </summary>
		/// <param name="objectQuery" type="Wilson.ORMapper.ObjectQuery">
		///    The ObjectQuery used to generate the SQL where clause
		/// </param>
		/// <returns>
		///     The number of rows found
		/// </returns>
		public int GetObjectCount(ObjectQuery objectQuery) {
			return this.context.GetObjectCount(objectQuery);
		}

#if DOTNETV2
		/// <summary>
		///     Get Record Count given Where Clause
		/// </summary>
		/// <param name="whereClause" type="string">
		///    The SQL where clause to use when retrieving data
		/// </param>
		/// <returns>
		///     The number of rows found
		/// </returns>
		public int GetObjectCount<T>(string whereClause) {
			ObjectQuery<T> query = new ObjectQuery<T>(whereClause, String.Empty);
			return this.GetObjectCount<T>(query);
		}

		/// <summary>
		///     Get Record Count given Where Clause
		/// </summary>
		/// <param name="objectQuery" type="Wilson.ORMapper.ObjectQuery">
		///    The ObjectQuery used to generate the SQL where clause
		/// </param>
		/// <returns>
		///     The number of rows found
		/// </returns>
		public int GetObjectCount<T>(ObjectQuery<T> objectQuery) {
			return this.context.GetObjectCount(objectQuery);
		}
#endif
		
		/// <summary>
		///     Get a Forward-Only Read-Only Cursor
		/// </summary>
		/// <param name="objectType" type="System.Type">
		///     The type of object to retrieve
		/// </param>
		/// <param name="whereClause" type="string">
		///     The SQL where clause to use when retrieving data
		/// </param>
		/// <returns>
		///     An ObjectReader instance
		/// </returns>
		public ObjectReader GetObjectReader(Type objectType, string whereClause) {
			ObjectQuery query = new ObjectQuery(objectType, whereClause, String.Empty);
			return this.GetObjectReader(query);
		}

		/// <summary>
		///     Get a Forward-Only Read-Only Cursor
		/// </summary>
		/// <param name="objectQuery" type="Wilson.ORMapper.ObjectQuery">
		///     The ObjectQuery used to generate the SQL where clause
		/// </param>
		/// <returns>
		///     An ObjectReader instance
		/// </returns>
		public ObjectReader GetObjectReader(ObjectQuery objectQuery) {
			string sortClause = (objectQuery.SortClause.Length > 0 ? objectQuery.SortClause
				: this.context.Mappings[objectQuery.ObjectType.ToString()].SortOrder);
			ObjectQuery query = new ObjectQuery(objectQuery.ObjectType,	objectQuery.WhereClause,
				sortClause, objectQuery.PageSize, objectQuery.PageIndex);
			return this.context.GetObjectReader(query, true);
		}
		
		/// <summary>
		///     Get a Forward-Only Read-Only Cursor using a stored procedure
		/// </summary>
		/// <param name="selectProcedure" type="Wilson.ORMapper.SelectProcedure">
		///     A SelectProcdure instance used to define the stored procedure call
		/// </param>
		/// <returns>
		///     An ObjectReader instance
		/// </returns>
		public ObjectReader GetObjectReader(SelectProcedure selectProcedure) {
			return this.context.GetObjectReader(selectProcedure, true);
		}

#if DOTNETV2
		/// <summary>
		///     Get a Forward-Only Read-Only Cursor
		/// </summary>
		/// <param name="whereClause" type="string">
		///     The SQL where clause to use when retrieving data
		/// </param>
		/// <returns>
		///     An ObjectReader instance
		/// </returns>
		public ObjectReader<T> GetObjectReader<T>(string whereClause) {
			ObjectQuery<T> query = new ObjectQuery<T>(whereClause, String.Empty);
			return this.GetObjectReader<T>(query);
		}

		/// <summary>
		///     Get a Forward-Only Read-Only Cursor
		/// </summary>
		/// <param name="objectQuery" type="Wilson.ORMapper.ObjectQuery">
		///     The ObjectQuery used to generate the SQL where clause
		/// </param>
		/// <returns>
		///     An ObjectReader instance
		/// </returns>
		public ObjectReader<T> GetObjectReader<T>(ObjectQuery<T> objectQuery) {
			string sortClause = (objectQuery.SortClause.Length > 0 ? objectQuery.SortClause
				: this.context.Mappings[objectQuery.ObjectType.ToString()].SortOrder);
			ObjectQuery<T> query = new ObjectQuery<T>(objectQuery.WhereClause,
				sortClause, objectQuery.PageSize, objectQuery.PageIndex);
			return this.context.GetObjectReader<T>(query, true);
		}

		/// <summary>
		///     Get a Forward-Only Read-Only Cursor using a stored procedure
		/// </summary>
		/// <param name="selectProcedure" type="Wilson.ORMapper.SelectProcedure">
		///     A SelectProcdure instance used to define the stored procedure call
		/// </param>
		/// <returns>
		///     An ObjectReader instance
		/// </returns>
		public ObjectReader<T> GetObjectReader<T>(SelectProcedure<T> selectProcedure) {
			return this.context.GetObjectReader<T>(selectProcedure, true);
		}
#endif

		/// <summary>
		///     Get a One-Way(Read) Bindable Object Collection
		/// </summary>
		/// <param name="objectType" type="System.Type">
		///     The type of object to retrieve
		/// </param>
		/// <param name="whereClause" type="string">
		///     The SQL where clause to use when retrieving data
		/// </param>
		/// <returns>
		///     An ObjectSet collection
		/// </returns>
		public ObjectSet GetObjectSet(Type objectType, string whereClause) {
			ObjectQuery query = new ObjectQuery(objectType, whereClause, String.Empty);
			return this.GetObjectSet(query);
		}

		/// <summary>
		///     Get a One-Way(Read) Bindable Object Collection
		/// </summary>
		/// <param name="objectQuery" type="Wilson.ORMapper.ObjectQuery">
		///     The ObjectQuery used to generate the SQL where clause
		/// </param>
		/// <returns>
		///      An ObjectSet collection
		/// </returns>
		public ObjectSet GetObjectSet(ObjectQuery objectQuery) {
			string sortClause = (objectQuery.SortClause.Length > 0 ? objectQuery.SortClause
				: this.context.Mappings[objectQuery.ObjectType.ToString()].SortOrder);
			ObjectQuery query = new ObjectQuery(objectQuery.ObjectType,	objectQuery.WhereClause,
				sortClause, objectQuery.PageSize, objectQuery.PageIndex);
			return this.context.GetObjectSet(query, true);
		}
		
		/// <summary>
		///     Get a One-Way Bindable Object Collection
		/// </summary>
		/// <param name="selectProcedure" type="Wilson.ORMapper.SelectProcedure">
		///     A SelectProcdure instance used to define the stored procedure call
		/// </param>
		/// <returns>
		///     A One-Way Bindable Object Collection
		/// </returns>
		public ObjectSet GetObjectSet(SelectProcedure selectProcedure) {
			return this.context.GetObjectSet(selectProcedure, true);
		}

#if DOTNETV2
		/// <summary>
		///     Get a One-Way(Read) Bindable Object Collection
		/// </summary>
		/// <param name="whereClause" type="string">
		///     The SQL where clause to use when retrieving data
		/// </param>
		/// <returns>
		///     An ObjectSet collection
		/// </returns>
		public ObjectSet<T> GetObjectSet<T>(string whereClause) {
			ObjectQuery<T> query = new ObjectQuery<T>(whereClause, String.Empty);
			return this.GetObjectSet<T>(query);
		}

		/// <summary>
		///     Get a One-Way(Read) Bindable Object Collection
		/// </summary>
		/// <param name="objectQuery" type="Wilson.ORMapper.ObjectQuery">
		///     The ObjectQuery used to generate the SQL where clause
		/// </param>
		/// <returns>
		///      An ObjectSet collection
		/// </returns>
		public ObjectSet<T> GetObjectSet<T>(ObjectQuery<T> objectQuery) {
			string sortClause = (objectQuery.SortClause.Length > 0 ? objectQuery.SortClause
				: this.context.Mappings[objectQuery.ObjectType.ToString()].SortOrder);
			ObjectQuery<T> query = new ObjectQuery<T>(objectQuery.WhereClause,
				sortClause, objectQuery.PageSize, objectQuery.PageIndex);
			return this.context.GetObjectSet<T>(query, true);
		}

		/// <summary>
		///     Get a One-Way Bindable Object Collection
		/// </summary>
		/// <param name="selectProcedure" type="Wilson.ORMapper.SelectProcedure">
		///     A SelectProcdure instance used to define the stored procedure call
		/// </param>
		/// <returns>
		///     A One-Way Bindable Object Collection
		/// </returns>
		public ObjectSet<T> GetObjectSet<T>(SelectProcedure<T> selectProcedure) {
			return this.context.GetObjectSet<T>(selectProcedure, true);
		}
#endif

		/// <summary>
		///     Get a Strongly Typed Object Collection
		/// </summary>
		/// <param name="collectionType" type="System.Type">
		///     The data type of the collection to create
		/// </param>
		/// <param name="objectType" type="System.Type">
		///     The type of object to retrieve
		/// </param>
		/// <param name="whereClause" type="string">
		///     The SQL where clause to use when retrieving data
		/// </param>
		/// <returns>
		///     A Strongly Typed Object Collection
		/// </returns>
		public IList GetCollection(Type collectionType, Type objectType, string whereClause) {
			ObjectQuery query = new ObjectQuery(objectType, whereClause, String.Empty);
			return this.GetCollection(collectionType, query);
		}

		/// <summary>
		///     Get a Strongly Typed Object Collection
		/// </summary>
		/// <param name="collectionType" type="System.Type">
		///     The data type of the collection to create
		/// </param>
		/// <param name="objectQuery" type="Wilson.ORMapper.ObjectQuery">
		///     The ObjectQuery instance used to generate the SQL where clause
		/// </param>
		/// <returns>
		///     A Strongly Typed Object Collection
		/// </returns>
		public IList GetCollection(Type collectionType, ObjectQuery objectQuery) {
			string sortClause = (objectQuery.SortClause.Length > 0 ? objectQuery.SortClause
				: this.context.Mappings[objectQuery.ObjectType.ToString()].SortOrder);
			ObjectQuery query = new ObjectQuery(objectQuery.ObjectType,	objectQuery.WhereClause,
				sortClause, objectQuery.PageSize, objectQuery.PageIndex);
			return this.context.GetCollection(collectionType, query, true);
		}
		
		/// <summary>
		///     Get a Strongly Typed Object Collection
		/// </summary>
		/// <param name="collectionType" type="System.Type">
		///     The data type of the collection to create
		/// </param>
		/// <param name="selectProcedure" type="Wilson.ORMapper.SelectProcedure">
		///     A SelectProcdure instance used to define the stored procedure call
		/// </param>
		/// <returns>
		///     Strongly Typed Object Collection
		/// </returns>
		public IList GetCollection(Type collectionType, SelectProcedure selectProcedure) {
			return this.context.GetCollection(collectionType, selectProcedure, true);
		}

#if DOTNETV2
		/// <summary>
		///     Get a Strongly Typed Object Collection
		/// </summary>
		/// <param name="whereClause" type="string">
		///     The SQL where clause to use when retrieving data
		/// </param>
		/// <returns>
		///     A Strongly Typed Object Collection
		/// </returns>
		public System.Collections.ObjectModel.Collection<T> GetCollection<T>(string whereClause) {
			ObjectQuery<T> query = new ObjectQuery<T>(whereClause, String.Empty);
			return this.GetCollection<T>(query);
		}

		/// <summary>
		///     Get a Strongly Typed Object Collection
		/// </summary>
		/// <param name="objectQuery" type="Wilson.ORMapper.ObjectQuery">
		///     The ObjectQuery instance used to generate the SQL where clause
		/// </param>
		/// <returns>
		///     A Strongly Typed Object Collection
		/// </returns>
		public System.Collections.ObjectModel.Collection<T> GetCollection<T>(ObjectQuery<T> objectQuery) {
			string sortClause = (objectQuery.SortClause.Length > 0 ? objectQuery.SortClause
				: this.context.Mappings[objectQuery.ObjectType.ToString()].SortOrder);
			ObjectQuery<T> query = new ObjectQuery<T>(objectQuery.WhereClause,
				sortClause, objectQuery.PageSize, objectQuery.PageIndex);
			return this.context.GetCollection<T>(query, true);
		}
		
		/// <summary>
		///     Get a Strongly Typed Object Collection
		/// </summary>
		/// <param name="selectProcedure" type="Wilson.ORMapper.SelectProcedure">
		///     A SelectProcdure instance used to define the stored procedure call
		/// </param>
		/// <returns>
		///     Strongly Typed Object Collection
		/// </returns>
		public System.Collections.ObjectModel.Collection<T> GetCollection<T>(SelectProcedure<T> selectProcedure) {
			return this.context.GetCollection<T>(selectProcedure, true);
		}
#endif

		/// <summary>
		///     Get a Raw DataSet where may be Needed
		/// </summary>
		/// <param name="objectType" type="System.Type">
		///     The type of object to retrieve
		/// </param>
		/// <param name="whereClause" type="string">
		///     The SQL where clause to use when retrieving data
		/// </param>
		/// <returns>
		///     A System.Data.DataSet object instance
		/// </returns>
		public DataSet GetDataSet(Type objectType, string whereClause) {
			ObjectQuery query = new ObjectQuery(objectType, whereClause, String.Empty);
			return this.GetDataSet(query);
		}

		/// <summary>
		///     Get a Raw DataSet where may be Needed
		/// </summary>
		/// <param name="objectQuery" type="Wilson.ORMapper.ObjectQuery">
		///     The ObjectQuery instance used to generate the SQL where clause
		/// </param>
		/// <returns>
		///     A System.Data.DataSet object instance
		/// </returns>
		public DataSet GetDataSet(ObjectQuery objectQuery) {
			string sortClause = (objectQuery.SortClause.Length > 0 ? objectQuery.SortClause
				: this.context.Mappings[objectQuery.ObjectType.ToString()].SortOrder);
			ObjectQuery query = new ObjectQuery(objectQuery.ObjectType,	objectQuery.WhereClause,
				sortClause, objectQuery.PageSize, objectQuery.PageIndex);
			return this.context.GetDataSet(null, query);
		}

		/// <summary>
		///     Get a Raw DataSet with custom Fields
		/// </summary>
		/// <param name="objectType" type="System.Type">
		///     The type of object to retrieve
		/// </param>
		/// <param name="whereClause" type="string">
		///     The SQL where clause to use when retrieving data
		/// </param>
		/// <param name="selectFields" type="string[]">
		///     An array of fields to select
		/// </param>
		/// <returns>
		///     A System.Data.DataSet object instance
		/// </returns>
		public DataSet GetDataSet(Type objectType, string whereClause, string[] selectFields) {
			ObjectQuery query = new ObjectQuery(objectType, whereClause, String.Empty);
			return this.GetDataSet(query, selectFields);
		}

		/// <summary>
		///     Get a Raw DataSet with custom Fields
		/// </summary>
		/// <param name="objectQuery" type="Wilson.ORMapper.ObjectQuery">
		///     The ObjectQuery instance used to generate the SQL where clause
		/// </param>
		/// <param name="selectFields" type="string[]">
		///     An array of fields to select
		/// </param>
		/// <returns>
		///     A System.Data.DataSet object instance
		/// </returns>
		public DataSet GetDataSet(ObjectQuery objectQuery, string[] selectFields) {
			string sortClause = (objectQuery.SortClause.Length > 0 ? objectQuery.SortClause
				: this.context.Mappings[objectQuery.ObjectType.ToString()].SortOrder);
			ObjectQuery query = new ObjectQuery(objectQuery.ObjectType,	objectQuery.WhereClause,
				sortClause, objectQuery.PageSize, objectQuery.PageIndex);
			return this.context.GetDataSet(null, query, selectFields);
		}
		
		/// <summary>
		///     Get a Raw DataSet where may be Needed
		/// </summary>
		/// <param name="selectProcedure" type="Wilson.ORMapper.SelectProcedure">
		///     A SelectProcdure instance used to define the stored procedure call
		/// </param>
		/// <returns>
		///      A System.Data.DataSet object instance
		/// </returns>
		public DataSet GetDataSet(SelectProcedure selectProcedure) {
			return this.context.GetDataSet(null, selectProcedure);
		}
		
		/// <summary>
		///     Get a Raw DataSet where may be Needed
		/// </summary>
		/// <param name="sqlStatement" type="string">
		///     The SQL statement to execute
		/// </param>
		/// <returns>
		///     A System.Data.DataSet object instance
		/// </returns>
		public DataSet GetDataSet(string sqlStatement) {
			return this.context.Connection.GetDataSet(null, CommandInfo.DataSet, sqlStatement);
		}

		/// <summary>
		///     Get a Raw DataSet where may be Needed
		/// </summary>
		/// <param name="dataSet">
		///     The typed dataset to populate with the resultset
		/// </param>
		/// <param name="objectType" type="System.Type">
		///     The type of object to retrieve
		/// </param>
		/// <param name="whereClause" type="string">
		///     The SQL where clause to use when retrieving data
		/// </param>
		/// <returns>
		///     A System.Data.DataSet object instance
		/// </returns>
		public DataSet GetDataSet(DataSet dataSet, Type objectType, string whereClause) {
			ObjectQuery query = new ObjectQuery(objectType, whereClause, String.Empty);
			return this.GetDataSet(dataSet, query);
		}

		/// <summary>
		///     Get a Raw DataSet where may be Needed
		/// </summary>
		/// <param name="dataSet">
		///     The typed dataset to populate with the resultset
		/// </param>
		/// <param name="objectQuery" type="Wilson.ORMapper.ObjectQuery">
		///     The ObjectQuery instance used to generate the SQL where clause
		/// </param>
		/// <returns>
		///     A System.Data.DataSet object instance
		/// </returns>
		public DataSet GetDataSet(DataSet dataSet, ObjectQuery objectQuery) {
			string sortClause = (objectQuery.SortClause.Length > 0 ? objectQuery.SortClause
				: this.context.Mappings[objectQuery.ObjectType.ToString()].SortOrder);
			ObjectQuery query = new ObjectQuery(objectQuery.ObjectType,	objectQuery.WhereClause,
				sortClause, objectQuery.PageSize, objectQuery.PageIndex);
			return this.context.GetDataSet(dataSet, query);
		}

		/// <summary>
		///     Get a Raw DataSet with custom Fields
		/// </summary>
		/// <param name="dataSet">
		///     The typed dataset to populate with the resultset
		/// </param>
		/// <param name="objectType" type="System.Type">
		///     The type of object to retrieve
		/// </param>
		/// <param name="whereClause" type="string">
		///     The SQL where clause to use when retrieving data
		/// </param>
		/// <param name="selectFields" type="string[]">
		///     An array of fields to select
		/// </param>
		/// <returns>
		///     A System.Data.DataSet object instance
		/// </returns>
		public DataSet GetDataSet(DataSet dataSet, Type objectType, string whereClause, string[] selectFields) {
			ObjectQuery query = new ObjectQuery(objectType, whereClause, String.Empty);
			return this.GetDataSet(dataSet, query, selectFields);
		}

		/// <summary>
		///     Get a Raw DataSet with custom Fields
		/// </summary>
		/// <param name="dataSet">
		///     The typed dataset to populate with the resultset
		/// </param>
		/// <param name="objectQuery" type="Wilson.ORMapper.ObjectQuery">
		///     The ObjectQuery instance used to generate the SQL where clause
		/// </param>
		/// <param name="selectFields" type="string[]">
		///     An array of fields to select
		/// </param>
		/// <returns>
		///     A System.Data.DataSet object instance
		/// </returns>
		public DataSet GetDataSet(DataSet dataSet, ObjectQuery objectQuery, string[] selectFields) {
			string sortClause = (objectQuery.SortClause.Length > 0 ? objectQuery.SortClause
				: this.context.Mappings[objectQuery.ObjectType.ToString()].SortOrder);
			ObjectQuery query = new ObjectQuery(objectQuery.ObjectType,	objectQuery.WhereClause,
				sortClause, objectQuery.PageSize, objectQuery.PageIndex);
			return this.context.GetDataSet(dataSet, query, selectFields);
		}
		
		/// <summary>
		///     Get a Raw DataSet where may be Needed
		/// </summary>
		/// <param name="dataSet">
		///     The typed dataset to populate with the resultset
		/// </param>
		/// <param name="selectProcedure" type="Wilson.ORMapper.SelectProcedure">
		///     A SelectProcdure instance used to define the stored procedure call
		/// </param>
		/// <returns>
		///      A System.Data.DataSet object instance
		/// </returns>
		public DataSet GetDataSet(DataSet dataSet, SelectProcedure selectProcedure) {
			return this.context.GetDataSet(dataSet, selectProcedure);
		}
		
		/// <summary>
		///     Get a Raw DataSet where may be Needed
		/// </summary>
		/// <param name="dataSet">
		///     The typed dataset to populate with the resultset
		/// </param>
		/// <param name="sqlStatement" type="string">
		///     The SQL statement to execute
		/// </param>
		/// <returns>
		///     A System.Data.DataSet object instance
		/// </returns>
		public DataSet GetDataSet(DataSet dataSet, string sqlStatement) {
			return this.context.Connection.GetDataSet(null, CommandInfo.DataSet, dataSet, sqlStatement);
		}

#if DOTNETV2
		/// <summary>
		///     Get a Raw DataSet where may be Needed
		/// </summary>
		/// <param name="whereClause" type="string">
		///     The SQL where clause to use when retrieving data
		/// </param>
		/// <returns>
		///     A System.Data.DataSet object instance
		/// </returns>
		public DataSet GetDataSet<T>(string whereClause) {
			ObjectQuery<T> query = new ObjectQuery<T>(whereClause, String.Empty);
			return this.GetDataSet<T>(query);
		}

		/// <summary>
		///     Get a Raw DataSet where may be Needed
		/// </summary>
		/// <param name="objectQuery" type="Wilson.ORMapper.ObjectQuery">
		///     The ObjectQuery instance used to generate the SQL where clause
		/// </param>
		/// <returns>
		///     A System.Data.DataSet object instance
		/// </returns>
		public DataSet GetDataSet<T>(ObjectQuery<T> objectQuery) {
			string sortClause = (objectQuery.SortClause.Length > 0 ? objectQuery.SortClause
				: this.context.Mappings[objectQuery.ObjectType.ToString()].SortOrder);
			ObjectQuery<T> query = new ObjectQuery<T>(objectQuery.WhereClause,
				sortClause, objectQuery.PageSize, objectQuery.PageIndex);
			return this.context.GetDataSet(null, query);
		}

		/// <summary>
		///     Get a Raw DataSet with custom Fields
		/// </summary>
		/// <param name="whereClause" type="string">
		///     The SQL where clause to use when retrieving data
		/// </param>
		/// <param name="selectFields" type="string[]">
		///     An array of fields to select
		/// </param>
		/// <returns>
		///     A System.Data.DataSet object instance
		/// </returns>
		public DataSet GetDataSet<T>(string whereClause, string[] selectFields) {
			ObjectQuery<T> query = new ObjectQuery<T>(whereClause, String.Empty);
			return this.GetDataSet<T>(query, selectFields);
		}

		/// <summary>
		///     Get a Raw DataSet with custom Fields
		/// </summary>
		/// <param name="objectQuery" type="Wilson.ORMapper.ObjectQuery">
		///     The ObjectQuery instance used to generate the SQL where clause
		/// </param>
		/// <param name="selectFields" type="string[]">
		///     An array of fields to select
		/// </param>
		/// <returns>
		///     A System.Data.DataSet object instance
		/// </returns>
		public DataSet GetDataSet<T>(ObjectQuery objectQuery, string[] selectFields) {
			string sortClause = (objectQuery.SortClause.Length > 0 ? objectQuery.SortClause
				: this.context.Mappings[objectQuery.ObjectType.ToString()].SortOrder);
			ObjectQuery<T> query = new ObjectQuery<T>(objectQuery.WhereClause,
				sortClause, objectQuery.PageSize, objectQuery.PageIndex);
			return this.context.GetDataSet(null, query, selectFields);
		}

		/// <summary>
		///     Get a Raw DataSet where may be Needed
		/// </summary>
		/// <param name="selectProcedure" type="Wilson.ORMapper.SelectProcedure">
		///     A SelectProcdure instance used to define the stored procedure call
		/// </param>
		/// <returns>
		///      A System.Data.DataSet object instance
		/// </returns>
		public DataSet GetDataSet<T>(SelectProcedure<T> selectProcedure) {
			return this.context.GetDataSet(null, selectProcedure);
		}

		/// <summary>
		///     Get a Raw DataSet where may be Needed
		/// </summary>
		/// <param name="dataSet">
		///     The typed dataset to populate with the resultset
		/// </param>
		/// <param name="whereClause" type="string">
		///     The SQL where clause to use when retrieving data
		/// </param>
		/// <returns>
		///     A System.Data.DataSet object instance
		/// </returns>
		public DataSet GetDataSet<T>(DataSet dataSet, string whereClause) {
			ObjectQuery<T> query = new ObjectQuery<T>(whereClause, String.Empty);
			return this.GetDataSet<T>(dataSet, query);
		}

		/// <summary>
		///     Get a Raw DataSet where may be Needed
		/// </summary>
		/// <param name="dataSet">
		///     The typed dataset to populate with the resultset
		/// </param>
		/// <param name="objectQuery" type="Wilson.ORMapper.ObjectQuery">
		///     The ObjectQuery instance used to generate the SQL where clause
		/// </param>
		/// <returns>
		///     A System.Data.DataSet object instance
		/// </returns>
		public DataSet GetDataSet<T>(DataSet dataSet, ObjectQuery<T> objectQuery) {
			string sortClause = (objectQuery.SortClause.Length > 0 ? objectQuery.SortClause
				: this.context.Mappings[objectQuery.ObjectType.ToString()].SortOrder);
			ObjectQuery<T> query = new ObjectQuery<T>(objectQuery.WhereClause,
				sortClause, objectQuery.PageSize, objectQuery.PageIndex);
			return this.context.GetDataSet(dataSet, query);
		}

		/// <summary>
		///     Get a Raw DataSet with custom Fields
		/// </summary>
		/// <param name="dataSet">
		///     The typed dataset to populate with the resultset
		/// </param>
		/// <param name="whereClause" type="string">
		///     The SQL where clause to use when retrieving data
		/// </param>
		/// <param name="selectFields" type="string[]">
		///     An array of fields to select
		/// </param>
		/// <returns>
		///     A System.Data.DataSet object instance
		/// </returns>
		public DataSet GetDataSet<T>(DataSet dataSet, string whereClause, string[] selectFields) {
			ObjectQuery<T> query = new ObjectQuery<T>(whereClause, String.Empty);
			return this.GetDataSet<T>(dataSet, query, selectFields);
		}

		/// <summary>
		///     Get a Raw DataSet with custom Fields
		/// </summary>
		/// <param name="dataSet">
		///     The typed dataset to populate with the resultset
		/// </param>
		/// <param name="objectQuery" type="Wilson.ORMapper.ObjectQuery">
		///     The ObjectQuery instance used to generate the SQL where clause
		/// </param>
		/// <param name="selectFields" type="string[]">
		///     An array of fields to select
		/// </param>
		/// <returns>
		///     A System.Data.DataSet object instance
		/// </returns>
		public DataSet GetDataSet<T>(DataSet dataSet, ObjectQuery objectQuery, string[] selectFields) {
			string sortClause = (objectQuery.SortClause.Length > 0 ? objectQuery.SortClause
				: this.context.Mappings[objectQuery.ObjectType.ToString()].SortOrder);
			ObjectQuery<T> query = new ObjectQuery<T>(objectQuery.WhereClause,
				sortClause, objectQuery.PageSize, objectQuery.PageIndex);
			return this.context.GetDataSet(dataSet, query, selectFields);
		}

		/// <summary>
		///     Get a Raw DataSet where may be Needed
		/// </summary>
		/// <param name="dataSet">
		///     The typed dataset to populate with the resultset
		/// </param>
		/// <param name="selectProcedure" type="Wilson.ORMapper.SelectProcedure">
		///     A SelectProcdure instance used to define the stored procedure call
		/// </param>
		/// <returns>
		///      A System.Data.DataSet object instance
		/// </returns>
		public DataSet GetDataSet<T>(DataSet dataSet, SelectProcedure<T> selectProcedure) {
			return this.context.GetDataSet(dataSet, selectProcedure);
		}
#endif

		/// <summary>
		///     Get a Transaction for greater control
		/// </summary>
		/// <returns>
		///     A Transaction object for Persistence
		/// </returns>
		public Transaction BeginTransaction() {
			return this.BeginTransaction(IsolationLevel.ReadCommitted);
		}

		/// <summary>
		///     Get a Transaction for greater control
		/// </summary>
		/// <param name="isolationLevel">
		///     The IsolationLevel for the Transaction
		/// </param>
		/// <returns>
		///     A Transaction object for Persistence
		/// </returns>
		public Transaction BeginTransaction(IsolationLevel isolationLevel) {
			return new Transaction(this.context, isolationLevel);
		}

		/// <summary>
		///     Directly Execute a Command where Needed
		/// </summary>
		/// <param name="selectProcedure" type="string">
		///     The SelectProcedure to execute
		/// </param>
		/// <returns>
		///     An int value indicating rows affected
		/// </returns>
		public int ExecuteCommand(SelectProcedure selectProcedure) {
			return this.context.Connection.ExecuteCommand(selectProcedure.ObjectType, CommandInfo.Command,
				selectProcedure.ProcedureName, selectProcedure.parameters);
		}

		/// <summary>
		///     Directly Execute a Command where Needed
		/// </summary>
		/// <param name="sqlStatement" type="string">
		///     The SQL statement to execute
		/// </param>
		/// <returns>
		///     An int value indicating rows affected
		/// </returns>
		public int ExecuteCommand(string sqlStatement) {
			return this.context.Connection.ExecuteCommand(null, CommandInfo.Command, sqlStatement);
		}

		/// <summary>
		///     Efficiently Retrieve a Single Value
		/// </summary>
		/// <param name="selectProcedure" type="string">
		///     The SelectProcedure to execute
		/// </param>
		/// <returns>
		///     The first column of the first row
		/// </returns>
		public object ExecuteScalar(SelectProcedure selectProcedure) {
			return this.context.Connection.GetScalarValue(selectProcedure.ObjectType, CommandInfo.GetScalar,
				selectProcedure.ProcedureName, selectProcedure.parameters);
		}

		/// <summary>
		///     Efficiently Retrieve a Single Value
		/// </summary>
		/// <param name="sqlStatement" type="string">
		///     The SQL statement to execute
		/// </param>
		/// <returns>
		///     The first column of the first row
		/// </returns>
		public object ExecuteScalar(string sqlStatement) {
			return this.context.Connection.GetScalarValue(null, CommandInfo.GetScalar, sqlStatement);
		}

		/// <summary>
		///     Directly Execute an Update where Needed
		/// </summary>
		/// <param name="objectType" type="System.Type">
		///     The type of object to retrieve
		/// </param>
		/// <param name="whereClause" type="string">
		///     The SQL where clause to use when retrieving data
		/// </param>
		/// <param name="updateClause" type="string">
		///     The SQL update clause to use when updating data
		/// </param>
		/// <returns>
		///     An int value indicating rows affected
		/// </returns>
		public int ExecuteUpdate(Type objectType, string whereClause, string updateClause) {
			Internals.Commands commands = this.context.Mappings.Commands(objectType.ToString());
			return this.context.Connection.ExecuteCommand(objectType, CommandInfo.BatchUpdate, commands.CreateUpdate(whereClause, updateClause));
		}

		/// <summary>
		///     Directly Execute a Deletion where Needed
		/// </summary>
		/// <param name="objectType" type="System.Type">
		///     The type of object to retrieve
		/// </param>
		/// <param name="whereClause" type="string">
		///     The SQL where clause to use when retrieving data
		/// </param>
		/// <returns>
		///     An int value indicating rows affected
		/// </returns>
		public int ExecuteDelete(Type objectType, string whereClause) {
			Internals.Commands commands = this.context.Mappings.Commands(objectType.ToString());
			return this.context.Connection.ExecuteCommand(objectType, CommandInfo.BatchDelete, commands.CreateDelete(whereClause));
		}

#if DOTNETV2
		/// <summary>
		///     Directly Execute an Update where Needed
		/// </summary>
		/// <param name="whereClause" type="string">
		///     The SQL where clause to use when retrieving data
		/// </param>
		/// <param name="updateClause" type="string">
		///     The SQL update clause to use when updating data
		/// </param>
		/// <returns>
		///     An int value indicating rows affected
		/// </returns>
		public int ExecuteUpdate<T>(string whereClause, string updateClause) {
			Internals.Commands commands = this.context.Mappings.Commands(typeof(T).ToString());
			return this.context.Connection.ExecuteCommand(typeof(T), CommandInfo.BatchUpdate, commands.CreateUpdate(whereClause, updateClause));
		}

		/// <summary>
		///     Directly Execute a Deletion where Needed
		/// </summary>
		/// <param name="whereClause" type="string">
		///     The SQL where clause to use when retrieving data
		/// </param>
		/// <returns>
		///     An int value indicating rows affected
		/// </returns>
		public int ExecuteDelete<T>(string whereClause) {
			Internals.Commands commands = this.context.Mappings.Commands(typeof(T).ToString());
			return this.context.Connection.ExecuteCommand(typeof(T), CommandInfo.BatchDelete, commands.CreateDelete(whereClause));
		}
#endif

		/// <summary>
		///     Start Tracking all Changes to an Object
		/// </summary>
		/// <param name="entityObject" type="object">
		///     Object instance to perform action on
		/// </param>
		/// <param name="initialState" type="Wilson.ORMapper.InitialState">
		///     The initial state of the object(s)
		/// </param>
		public void StartTracking(object entityObject, InitialState initialState) {
			this.context.StartTracking(entityObject, initialState);
		}

		/// <summary>
		///     Start Tracking all Changes to a Collection
		/// </summary>
		/// <param name="entityObjects" type="System.Collections.ICollection">
		///     Collection of object instances to perform action on
		/// </param>
		/// <param name="initialState" type="Wilson.ORMapper.InitialState">
		///     The initial state of the object(s)
		/// </param>
		public void StartTracking(ICollection entityObjects, InitialState initialState) {
			foreach (object entityObject in entityObjects) {
				this.StartTracking(entityObject, initialState);
			}
		}

		/// <summary>
		///     End Tracking (Clear Cache) all Changes to an Object
		/// </summary>
		/// <param name="entityObject" type="object">
		///     Object instance to end tracking and remove from cache
		/// </param>
		public void EndTracking(object entityObject) {
			this.context.EndTracking(entityObject);
		}

		/// <summary>
		///     End Tracking (Clear Cache) all Changes to a Collection
		/// </summary>
		/// <param name="entityObjects" type="System.Collections.ICollection">
		///     Collection of objects to end tracking and remove from cache
		/// </param>
		public void EndTracking(ICollection entityObjects) {
			foreach (object entityObject in entityObjects) {
				this.EndTracking(entityObject);
			}
		}

		/// <summary>
		///     Clear Tracking (Clear Cache) all Object Instances
		/// </summary>
		public void ClearTracking() {
			this.context.ClearTracking();
		}
		
		/// <summary>
		///     Mark Object for Deletion with PersistChanges
		/// </summary>
		/// <param name="entityObject" type="object">
		///     Object instance to perform action on
		/// </param>
		public void MarkForDeletion(object entityObject) {
			this.context[entityObject].MarkForDeletion();
		}
		
		/// <summary>
		///     Mark Collection for Deletion with PersistChanges
		/// </summary>
		/// <param name="entityObjects" type="System.Collections.ICollection">
		///     Collection of object instances to perform action on
		/// </param>
		public void MarkForDeletion(ICollection entityObjects) {
			foreach (object entityObject in entityObjects) {
				this.MarkForDeletion(entityObject);
			}
		}

		/// <summary>
		///     Cancel all Changes to an Object since Tracking
		/// </summary>
		/// <param name="entityObject" type="object">
		///     Object instance to perform action on
		/// </param>
		public void CancelChanges(object entityObject) {
			this.context[entityObject].CancelChanges();
		}

		/// <summary>
		///     Cancel all Changes to a Collection since Tracking
		/// </summary>
		/// <param name="entityObjects" type="System.Collections.ICollection">
		///     Collection of object instances to perform action on
		/// </param>
		public void CancelChanges(ICollection entityObjects) {
			foreach (object entityObject in entityObjects) {
				this.CancelChanges(entityObject);
			}
		}

		/// <summary>
		///     Save all Changes -- Insert, Update, or Delete
		/// </summary>
		/// <param name="entityObject" type="object">
		///     Object instance to perform action on
		/// </param>
		public void PersistChanges(object entityObject) {
			object[] entityObjects = new object[] {entityObject};
			this.PersistChanges(entityObjects, PersistDepth.SingleObject);
		}

		/// <summary>
		///     Save all Changes -- Insert, Update, or Delete
		/// </summary>
		/// <param name="entityObjects" type="System.Collections.ICollection">
		///     Collection of object instances to perform action on
		/// </param>
		public void PersistChanges(ICollection entityObjects) {
			this.PersistChanges(entityObjects, PersistDepth.SingleObject);
		}

		/// <summary>
		///     Save all Changes -- Insert, Update, or Delete
		/// </summary>
		/// <param name="entityObject" type="object">
		///     Object instance to perform action on
		/// </param>
		/// <param name="persistDepth" type="Wilson.ORMapper.PersistDepth">
		///    The depth at which to persist child objects
		/// </param>
		public void PersistChanges(object entityObject, PersistDepth persistDepth) {
			object[] entityObjects = new object[] {entityObject};
			this.PersistChanges(entityObjects, persistDepth);
		}

		/// <summary>
		///     Save all Changes -- Insert, Update, or Delete
		/// </summary>
		/// <param name="entityObjects" type="System.Collections.ICollection">
		///     Collection of object instances to perform action on
		/// </param>
		/// <param name="persistDepth" type="Wilson.ORMapper.PersistDepth">
		///     The depth at which to persist child objects
		/// </param>
		public void PersistChanges(ICollection entityObjects, PersistDepth persistDepth) {
			Transaction transaction = null;
			try {
				transaction = this.BeginTransaction();
				transaction.PersistChanges(entityObjects, persistDepth);
				transaction.Commit();
			}
			catch {
				if (transaction != null) transaction.Rollback();
				throw;
			}
			finally {
				if (transaction != null) transaction.Dispose();
			}
		}
		
		/// <summary>
		///     Get the Latest Version of Object from Database
		/// </summary>
		/// <param name="entityObject" type="object">
		///     Object instance to perform action on
		/// </param>
		/// <returns>
		///     The latest version of the object
		/// </returns>
		public object Resync(object entityObject) {
			return this.context.Resync(entityObject);
		}
		
		/// <summary>
		///     Get the Latest Version of Collection from Database
		/// </summary>
		/// <param name="entityObjects" type="System.Collections.ICollection">
		///     Collection of object instances to perform action on
		/// </param>
		public void Resync(ICollection entityObjects) {
			foreach (object entityObject in entityObjects) {
				this.Resync(entityObject);
			}
		}

		#region --- OPath Support by Jeff Lanning (jefflanning@gmail.com) ---

		/// <summary>
		/// Compiles an OPathQuery instance using this ObjectSpace as the mapping reference.
		/// </summary>
		/// <param name="query">OPathQuery to be compiled.</param>
		/// <returns>A CompiledQuery object.</returns>
		public CompiledQuery Compile(OPathQuery query) {
			return query.Compile(this);
		}

#if DOTNETV2
		/// <summary>
		/// Compiles an OPathQuery instance using this ObjectSpace as the mapping reference.
		/// </summary>
		/// <param name="query">OPathQuery to be compiled.</param>
		/// <returns>A CompiledQuery object.</returns>
		public CompiledQuery<T> Compile<T>(OPathQuery<T> query) {
			return query.Compile(this);
		}
#endif

		/// <summary>
		/// Executes an OPathQuery against the data store and returns the first matching object.
		/// Null is returned if no object is found.
		/// </summary>
		/// <param name="query">OPathQuery to execute.</param>
		/// <returns>The first object matching the query; or null if no match was found.</returns>
		public object GetObject(OPathQuery query) {
			return this.GetObject(query, (object[])null);
		}

		/// <summary>
		/// Executes an OPathQuery against the data store and returns the first matching object.
		/// Null is returned if no object is found.
		/// </summary>
		/// <param name="query">OPathQuery to execute.</param>
		/// <param name="parameters">Parameter values to use when executing the query.</param>
		/// <returns>The first object matching the query; or null if no match was found.</returns>
		public object GetObject(OPathQuery query, params object[] parameters) {
			return this.GetObject(query.Compile(this), parameters);
		}

		/// <summary>
		/// Executes a CompiledQuery against the data store and returns the first matching object.
		/// Null is returned if no object is found.
		/// </summary>
		/// <param name="compiledQuery">CompiledQuery to execute.</param>
		/// <returns>The first object matching the query; or null if no match was found.</returns>
		public object GetObject(CompiledQuery compiledQuery) {
			return this.GetObject(compiledQuery, (object[])null);
		}

		/// <summary>
		/// Executes a CompiledQuery against the data store and returns the first matching object.
		/// Null is returned if no object is found.
		/// </summary>
		/// <param name="compiledQuery">CompiledQuery to execute.</param>
		/// <param name="parameters">Parameter values to use when executing the query.</param>
		/// <returns>The first object matching the query; or null if no match was found.</returns>
		public object GetObject(CompiledQuery compiledQuery, params object[] parameters) {
			return this.context.GetObject(compiledQuery, true, parameters);
		}

#if DOTNETV2
		/// <summary>
		/// Executes an OPathQuery against the data store and returns the first matching object.
		/// Null is returned if no object is found.
		/// </summary>
		/// <param name="query">OPathQuery to execute.</param>
		/// <returns>The first object matching the query; or null if no match was found.</returns>
		public T GetObject<T>(OPathQuery<T> query) {
			return this.GetObject<T>(query, (object[])null);
		}

		/// <summary>
		/// Executes an OPathQuery against the data store and returns the first matching object.
		/// Null is returned if no object is found.
		/// </summary>
		/// <param name="query">OPathQuery to execute.</param>
		/// <param name="parameters">Parameter values to use when executing the query.</param>
		/// <returns>The first object matching the query; or null if no match was found.</returns>
		public T GetObject<T>(OPathQuery<T> query, params object[] parameters) {
			return this.GetObject<T>(query.Compile(this), parameters);
		}

		/// <summary>
		/// Executes a CompiledQuery against the data store and returns the first matching object.
		/// Null is returned if no object is found.
		/// </summary>
		/// <param name="compiledQuery">CompiledQuery to execute.</param>
		/// <returns>The first object matching the query; or null if no match was found.</returns>
		public T GetObject<T>(CompiledQuery<T> compiledQuery) {
			return this.GetObject<T>(compiledQuery, (object[])null);
		}

		/// <summary>
		/// Executes a CompiledQuery against the data store and returns the first matching object.
		/// Null is returned if no object is found.
		/// </summary>
		/// <param name="compiledQuery">CompiledQuery to execute.</param>
		/// <param name="parameters">Parameter values to use when executing the query.</param>
		/// <returns>The first object matching the query; or null if no match was found.</returns>
		public T GetObject<T>(CompiledQuery<T> compiledQuery, params object[] parameters) {
			return (T)this.context.GetObject(compiledQuery, true, parameters);
		}
#endif

		/// <summary>
		/// Executes an OPathQuery against the data store and returns an ObjectReader holding the results.
		/// </summary>
		/// <param name="query">OPathQuery to execute.</param>
		/// <returns>An ObjectReader positioned at the beginning of the object data stream.</returns>
		public ObjectReader GetObjectReader(OPathQuery query) {
			return this.GetObjectReader(query, (object[])null);
		}

		/// <summary>
		/// Executes an OPathQuery against the data store and returns an ObjectReader holding the results.
		/// </summary>
		/// <param name="query">OPathQuery to execute.</param>
		/// <param name="parameters">Parameter values to use when executing the query.</param>
		/// <returns>An ObjectReader positioned at the beginning of the object data stream.</returns>
		public ObjectReader GetObjectReader(OPathQuery query, params object[] parameters) {
			return this.GetObjectReader(query.Compile(this), parameters);
		}

		/// <summary>
		/// Executes a CompiledQuery against the data store and returns an ObjectReader holding the results.
		/// </summary>
		/// <param name="compiledQuery">CompiledQuery to execute.</param>
		/// <returns>An ObjectReader positioned at the beginning of the object data stream.</returns>
		public ObjectReader GetObjectReader(CompiledQuery compiledQuery) {
			return this.GetObjectReader(compiledQuery, (object[])null);
		}

		/// <summary>
		/// Executes a CompiledQuery against the data store and returns an ObjectReader holding the results.
		/// </summary>
		/// <param name="compiledQuery">CompiledQuery to execute.</param>
		/// <param name="parameters">Parameter values to use when executing the query.</param>
		/// <returns>An ObjectReader positioned at the beginning of the object data stream.</returns>
		public ObjectReader GetObjectReader(CompiledQuery compiledQuery, params object[] parameters) {
			return this.context.GetObjectReader(compiledQuery, true, parameters);
		}

#if DOTNETV2
		/// <summary>
		/// Executes an OPathQuery against the data store and returns an ObjectReader holding the results.
		/// </summary>
		/// <param name="query">OPathQuery to execute.</param>
		/// <returns>An ObjectReader positioned at the beginning of the object data stream.</returns>
		public ObjectReader<T> GetObjectReader<T>(OPathQuery<T> query) {
			return this.GetObjectReader<T>(query, (object[])null);
		}

		/// <summary>
		/// Executes an OPathQuery against the data store and returns an ObjectReader holding the results.
		/// </summary>
		/// <param name="query">OPathQuery to execute.</param>
		/// <param name="parameters">Parameter values to use when executing the query.</param>
		/// <returns>An ObjectReader positioned at the beginning of the object data stream.</returns>
		public ObjectReader<T> GetObjectReader<T>(OPathQuery<T> query, params object[] parameters) {
			return this.GetObjectReader<T>(query.Compile(this), parameters);
		}

		/// <summary>
		/// Executes a CompiledQuery against the data store and returns an ObjectReader holding the results.
		/// </summary>
		/// <param name="compiledQuery">CompiledQuery to execute.</param>
		/// <returns>An ObjectReader positioned at the beginning of the object data stream.</returns>
		public ObjectReader<T> GetObjectReader<T>(CompiledQuery<T> compiledQuery) {
			return this.GetObjectReader<T>(compiledQuery, (object[])null);
		}

		/// <summary>
		/// Executes a CompiledQuery against the data store and returns an ObjectReader holding the results.
		/// </summary>
		/// <param name="compiledQuery">CompiledQuery to execute.</param>
		/// <param name="parameters">Parameter values to use when executing the query.</param>
		/// <returns>An ObjectReader positioned at the beginning of the object data stream.</returns>
		public ObjectReader<T> GetObjectReader<T>(CompiledQuery<T> compiledQuery, params object[] parameters) {
			return this.context.GetObjectReader<T>(compiledQuery, true, parameters);
		}
#endif

		/// <summary>
		/// Executes an OPathQuery against the data store and returns an ObjectSet filled with the results.
		/// </summary>
		/// <param name="query">OPathQuery to execute.</param>
		/// <returns>An ObjectSet filled with objects retrieved from the data store.</returns>
		public ObjectSet GetObjectSet(OPathQuery query) {
			return this.GetObjectSet(query, (object[])null);
		}

		/// <summary>
		/// Executes an OPathQuery against the database and returns an ObjectSet filled with the results.
		/// </summary>
		/// <param name="query">OPathQuery to execute.</param>
		/// <param name="parameters">Parameter values to use when executing the query.</param>
		/// <returns>An ObjectSet filled with objects retrieved from the data store.</returns>
		public ObjectSet GetObjectSet(OPathQuery query, params object[] parameters) {
			return this.GetObjectSet(query.Compile(this), parameters);
		}

		/// <summary>
		/// Executes a CompiledQuery against the database and returns an ObjectSet filled with the results.
		/// </summary>
		/// <param name="compiledQuery">CompiledQuery to execute.</param>
		/// <returns>An ObjectSet filled with objects retrieved from the data store.</returns>
		public ObjectSet GetObjectSet(CompiledQuery compiledQuery) {
			return this.GetObjectSet(compiledQuery, (object[])null);
		}

		/// <summary>
		/// Executes a CompiledQuery against the database and returns an ObjectSet filled with the results.
		/// </summary>
		/// <param name="compiledQuery">CompiledQuery to execute.</param>
		/// <param name="parameters">Parameter values to use when executing the query.</param>
		/// <returns>An ObjectSet filled with objects retrieved from the data store.</returns>
		public ObjectSet GetObjectSet(CompiledQuery compiledQuery, params object[] parameters) {
			return this.context.GetObjectSet(compiledQuery, true, parameters);
		}

#if DOTNETV2
		/// <summary>
		/// Executes an OPathQuery against the data store and returns an ObjectSet filled with the results.
		/// </summary>
		/// <param name="query">OPathQuery to execute.</param>
		/// <returns>An ObjectSet filled with objects retrieved from the data store.</returns>
		public ObjectSet<T> GetObjectSet<T>(OPathQuery<T> query) {
			return this.GetObjectSet<T>(query, (object[])null);
		}

		/// <summary>
		/// Executes an OPathQuery against the data store and returns the first matching object.
		/// Null is returned if no object is found.
		/// </summary>
		/// <param name="query">OPathQuery to execute.</param>
		/// <param name="parameters">Parameter values to use when executing the query.</param>
		/// <returns>The first object matching the query; or null if no match was found.</returns>
		public ObjectSet<T> GetObjectSet<T>(OPathQuery<T> query, params object[] parameters) {
			return this.GetObjectSet<T>(query.Compile(this), parameters);
		}

		/// <summary>
		/// Executes a CompiledQuery against the data store and returns the first matching object.
		/// Null is returned if no object is found.
		/// </summary>
		/// <param name="compiledQuery">CompiledQuery to execute.</param>
		/// <returns>The first object matching the query; or null if no match was found.</returns>
		public ObjectSet<T> GetObjectSet<T>(CompiledQuery<T> compiledQuery) {
			return this.GetObjectSet<T>(compiledQuery, (object[])null);
		}

		/// <summary>
		/// Executes a CompiledQuery against the data store and returns the first matching object.
		/// Null is returned if no object is found.
		/// </summary>
		/// <param name="compiledQuery">CompiledQuery to execute.</param>
		/// <param name="parameters">Parameter values to use when executing the query.</param>
		/// <returns>The first object matching the query; or null if no match was found.</returns>
		public ObjectSet<T> GetObjectSet<T>(CompiledQuery<T> compiledQuery, params object[] parameters) {
			return this.context.GetObjectSet<T>((CompiledQuery)compiledQuery, true, parameters);
		}
#endif	

		/// <summary>
		/// Executes an OPathQuery against the data store and returns an IList filled with the results.
		/// </summary>
		/// <param name="collectionType">Type of collection to return. Type specified must implement IList.</param>
		/// <param name="query">OPathQuery to execute.</param>
		/// <returns>An IList of the specified type, filled with objects retrieved from the data store.</returns>
		public IList GetCollection(Type collectionType, OPathQuery query) {
			return this.GetCollection(collectionType, query, (object[])null);
		}

		/// <summary>
		/// Executes an OPathQuery against the data store and returns an IList filled with the results.
		/// </summary>
		/// <param name="collectionType">Type of collection to return. Type specified must implement IList.</param>
		/// <param name="query">OPathQuery to execute.</param>
		/// <param name="parameters">Parameter values to use when executing the query.</param>
		/// <returns>An IList of the specified type, filled with objects retrieved from the data store.</returns>
		public IList GetCollection(Type collectionType, OPathQuery query, params object[] parameters) {
			return this.GetCollection(collectionType, query.Compile(this), parameters);
		}
		
		/// <summary>
		/// Executes an CompiledQuery against the data store and returns an IList filled with the results.
		/// </summary>
		/// <param name="collectionType">Type of collection to return. Type specified must implement IList.</param>
		/// <param name="compiledQuery">CompiledQuery to execute.</param>
		/// <returns>An IList of the specified type, filled with objects retrieved from the data store.</returns>
		public IList GetCollection(Type collectionType, CompiledQuery compiledQuery) {
			return this.GetCollection(collectionType, compiledQuery, (object[])null);
		}

		/// <summary>
		/// Executes an CompiledQuery against the data store and returns an IList filled with the results.
		/// </summary>
		/// <param name="collectionType">Type of collection to return. Type specified must implement IList.</param>
		/// <param name="compiledQuery">CompiledQuery to execute.</param>
		/// <param name="parameters">Parameter values to use when executing the query.</param>
		/// <returns>An IList of the specified type, filled with objects retrieved from the data store.</returns>
		public IList GetCollection(Type collectionType, CompiledQuery compiledQuery, params object[] parameters) {
			return this.context.GetCollection(collectionType, compiledQuery, true, parameters);
		}

#if DOTNETV2
		/// <summary>
		/// Executes an OPathQuery against the data store and returns an strongly-typed collection filled with the results.
		/// </summary>
		/// <param name="query">OPathQuery to execute.</param>
		/// <returns>An strongly-typed collection filled with objects retrieved from the data store.</returns>
		public System.Collections.ObjectModel.Collection<T> GetCollection<T>(OPathQuery<T> query) {
			return this.GetCollection<T>(query, (object[])null);
		}

		/// <summary>
		/// Executes an OPathQuery against the data store and returns an strongly-typed collection filled with the results.
		/// </summary>
		/// <param name="query">OPathQuery to execute.</param>
		/// <param name="parameters">Parameter values to use when executing the query.</param>
		/// <returns>An strongly-typed collection filled with objects retrieved from the data store.</returns>
		public System.Collections.ObjectModel.Collection<T> GetCollection<T>(OPathQuery<T> query, params object[] parameters) {
			return this.GetCollection<T>(query.Compile(this), parameters);
		}
		
		/// <summary>
		/// Executes a CompiledQuery against the data store and returns an strongly-typed collection filled with the results.
		/// </summary>
		/// <param name="compiledQuery">CompiledQuery to execute.</param>
		/// <returns>An strongly-typed collection filled with objects retrieved from the data store.</returns>
		public System.Collections.ObjectModel.Collection<T> GetCollection<T>(CompiledQuery<T> compiledQuery) {
			return this.GetCollection<T>(compiledQuery, (object[])null);
		}
		
		/// <summary>
		/// Executes a CompiledQuery against the data store and returns an strongly-typed collection filled with the results.
		/// </summary>
		/// <param name="compiledQuery">CompiledQuery to execute.</param>
		/// <param name="parameters">Parameter values to use when executing the query.</param>
		/// <returns>An strongly-typed collection filled with objects retrieved from the data store.</returns>
		public System.Collections.ObjectModel.Collection<T> GetCollection<T>(CompiledQuery<T> compiledQuery, params object[] parameters) {
			return this.context.GetCollection<T>(compiledQuery, true, parameters);
		}
#endif

		/// <summary>
		/// Executes an OPathQuery against the data store and returns a DataSet filled with the results.
		/// </summary>
		/// <param name="query">OPathQuery to execute.</param>
		/// <returns>A DataSet filled with record retrieved from the data store.</returns>
		public DataSet GetDataSet(OPathQuery query) {
			return GetDataSet(query, (object[])null);
		}

		/// <summary>
		/// Executes an OPathQuery against the data store and returns a DataSet filled with the results.
		/// </summary>
		/// <param name="query">OPathQuery to execute.</param>
		/// <param name="parameters">Parameter values to use when executing the query.</param>
		/// <returns>A DataSet filled with record retrieved from the data store.</returns>
		public DataSet GetDataSet(OPathQuery query, params object[] parameters)	{
			return GetDataSet(query.Compile(this), parameters);
		}

		/// <summary>
		/// Executes a CompiledQuery against the database and returns a DataSet filled with the results.
		/// </summary>
		/// <param name="compiledQuery">CompiledQuery to execute.</param>
		/// <returns>A DataSet filled with records retrieved from the data store.</returns>
		public DataSet GetDataSet(CompiledQuery compiledQuery) {
			return GetDataSet(compiledQuery, (object[])null);
		}

		/// <summary>
		/// Executes a CompiledQuery against the database and returns a DataSet filled with the results.
		/// </summary>
		/// <param name="compiledQuery">CompiledQuery to execute.</param>
		/// <param name="parameters">Parameter values to use when executing the query.</param>
		/// <returns>A DataSet filled with records retrieved from the data store.</returns>
		public DataSet GetDataSet(CompiledQuery compiledQuery, params object[] parameters) {
			return this.context.GetDataSet(compiledQuery, parameters);
		}

#if DOTNETV2
		/// <summary>
		/// Executes an OPathQuery against the data store and returns a DataSet filled with the results.
		/// </summary>
		/// <param name="query">OPathQuery to execute.</param>
		/// <returns>A DataSet filled with record retrieved from the data store.</returns>
		public DataSet GetDataSet<T>(OPathQuery<T> query) {
			return GetDataSet<T>(query, (object[])null);
		}

		/// <summary>
		/// Executes an OPathQuery against the data store and returns a DataSet filled with the results.
		/// </summary>
		/// <param name="query">OPathQuery to execute.</param>
		/// <param name="parameters">Parameter values to use when executing the query.</param>
		/// <returns>A DataSet filled with record retrieved from the data store.</returns>
		public DataSet GetDataSet<T>(OPathQuery<T> query, params object[] parameters)	{
			return GetDataSet<T>(query.Compile(this), parameters);
		}

		/// <summary>
		/// Executes a CompiledQuery against the database and returns a DataSet filled with the results.
		/// </summary>
		/// <param name="compiledQuery">CompiledQuery to execute.</param>
		/// <returns>A DataSet filled with records retrieved from the data store.</returns>
		public DataSet GetDataSet<T>(CompiledQuery<T> compiledQuery) {
			return GetDataSet<T>(compiledQuery, (object[])null);
		}

		/// <summary>
		/// Executes a CompiledQuery against the database and returns a DataSet filled with the results.
		/// </summary>
		/// <param name="compiledQuery">CompiledQuery to execute.</param>
		/// <param name="parameters">Parameter values to use when executing the query.</param>
		/// <returns>A DataSet filled with records retrieved from the data store.</returns>
		public DataSet GetDataSet<T>(CompiledQuery<T> compiledQuery, params object[] parameters) {
			return this.context.GetDataSet(compiledQuery, parameters);
		}
#endif

		#endregion
		
		/// <summary>
		///     Debug String -- Includes List of all Values
		/// </summary>
		/// <param name="entityObject" type="object">
		///      Object instance to perform action on
		/// </param>
		/// <returns>
		///     A string value...
		/// </returns>
		public string ToString(object entityObject) {
			try { return this.context[entityObject].ToString(); }
			catch (Exception error) { return error.Message; }
		}

		#region IDisposable Members
		/// <summary>
		/// Dispose is NOT needed for any IsolatedContext ObjectSpaces.
		/// Dispose is ONLY needed if the AppDomain is not also ending.
		/// </summary>
		public void Dispose() {
			this.context.Dispose();
		}

		#endregion
	}
}