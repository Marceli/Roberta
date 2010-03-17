//**************************************************************//
// Paul Wilson -- www.WilsonDotNet.com -- Paul@WilsonDotNet.com //
// Feel free to use and modify -- just leave these credit lines //
// I also always appreciate any other public credit you provide //
//**************************************************************//
// EndTracking (widely requested) is now in ObjectSpaces spec   //
// Thanks to both Allan Ritchie (A.Ritchie@ACRWebSolutions.com) //
// and Gerrod Thomas (http://www.Gerrod.com) for advice & code  //
// and for especially helping me to solve the Resync problems   //
//**************************************************************//
// Includes support for GetCollection for Strongly Typed IList Collections and
// Embedded Resource Mappings from Allan Ritchie (A.Ritchie@ACRWebSolutions.com)
// Includes support for typed datasets from Ben Priebe (http://stickfly.com)
// Includes CompiledQuery overloads for OPath support from Jeff Lanning (jefflanning@gmail.com)
// Includes EntityKey optimizations from Marc Brooks (http://musingmarc.blogspot.com)
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Timers;
using System.Xml;

namespace Wilson.ORMapper.Internals {
	[LicenseProvider(typeof(DemoProvider))]
	internal class Context : IDisposable
	{
		private Hashtable instances = Hashtable.Synchronized(new Hashtable()); // Jeff Lanning (jefflanning@gmail.com): Made syncronized for better thread safety.
		private TimeSpan session;
		private Timer timer = null;
		
#if DEMO
		private License license = null;
#endif
		private Mappings mappings;
		private Connection connection;
		private CustomProvider provider;

		public Instance this[object entityObject] {
			get {
				// Jeff Lanning (jefflanning@gmail.com): Made thread-safe (and improved performance a bit).
				EntityKey entityKey = new EntityKey(this, entityObject, true);
				Instance instance = (Instance)this.instances[entityKey];
				if (instance == null) {
					throw new ORMapperException("ObjectSpace: Entity Object not being Tracked - " + entityKey);
				}
				instance.EntityObject = entityObject; // Refresh WeakReference
				return instance;
			}
		}

		internal bool IsTracked(object entityObject) {
			EntityKey entityKey = new EntityKey(this, entityObject, true);
			return this.instances.Contains(entityKey);
		}

		public TimeSpan Session {
			get { return this.session; }
		}

		public Mappings Mappings {
			get { return this.mappings; }
		}

		public Connection Connection {
			get { return this.connection; }
		}

		public CustomProvider Provider {
			get { return this.provider; }
		}

		internal Context(string mappingFile, string connectString, CustomProvider customProvider,
			int sessionMinutes, int cleanupMinutes) {
#if DEMO
			this.license = LicenseManager.Validate(this.GetType(), this);
#endif
			Mappings.LoadAssemblies();
			XmlDocument mappings = new XmlDocument();
			if (mappingFile.IndexOf("/") > -1 || mappingFile.IndexOf("\\") > -1 || mappingFile.Split('.').Length <= 2) {
				// Try to Automatically Resolve Path of Mapping File
				string fullPath = Mappings.GetFullPath(mappingFile);
				mappings.Load(fullPath);
			}
			else {
				// Try to Automatically Load Embedded Mapping File
				using (Stream stream = Mappings.GetResourceStream(mappingFile)) {
					mappings.Load(stream);
				}
			}
			this.Init(mappings, connectString, customProvider, sessionMinutes, cleanupMinutes);
		}

		internal Context(Stream mappingStream, string connectString, CustomProvider customProvider,
			int sessionMinutes, int cleanupMinutes) {
#if DEMO
			this.license = LicenseManager.Validate(this.GetType(), this);
#endif
			Mappings.LoadAssemblies();
			XmlDocument mappings = new XmlDocument();
			try {
				mappings.Load(mappingStream);
			}
			finally {
				if (mappingStream != null) mappingStream.Close();
			}
			this.Init(mappings, connectString, customProvider, sessionMinutes, cleanupMinutes);
		}

		internal Context(Context context) {
			this.mappings = context.mappings;
			this.connection = context.connection;
			this.provider = context.provider;
			GC.SuppressFinalize(this); // No timer to cleanup
		}

		private void Init(XmlDocument xmlMappings, string connectString, CustomProvider customProvider,
			int sessionMinutes, int cleanupMinutes) {
			if (connectString == null || connectString.Length == 0) {
				throw new ORMapperException("ObjectSpace: ConnectionString was Empty");
			}
			this.mappings = new Mappings(xmlMappings, customProvider);
			this.connection = new Connection(connectString, customProvider);
			this.provider = customProvider;
			this.session = new TimeSpan(0, (sessionMinutes < 0 ? 0 : sessionMinutes), 0);
			this.timer = new Timer((cleanupMinutes < 1 ? 1 : cleanupMinutes) * 60000);
			this.timer.Elapsed += new ElapsedEventHandler(this.CleanupInstances);
			this.timer.Start();
		}

		// Timer must periodically Removes Invalid Instances
		private void CleanupInstances(Object sender, System.Timers.ElapsedEventArgs e) {
			if (this.timer == null) return;
			this.timer.Enabled = false;

			// Jeff Lanning (jefflanning@gmail.com): Made fetching the keys thread-safe.
			EntityKey[] keys;
			lock (this.instances.SyncRoot) {
				keys = new EntityKey[this.instances.Keys.Count];
				this.instances.Keys.CopyTo(keys, 0);
			}

			// Thanks to David D'Amico (ORMapper@datasolinc.com) -- Avoid Blow Up on Invalid Objects
			for (int i = 0; i < keys.Length; i++) {
				EntityKey entityKey = keys[i];
				Instance instance = (Instance)this.instances[entityKey];
				if (instance != null && !instance.IsValid) {
					this.instances.Remove(entityKey);
				}
			}

			this.timer.Enabled = true;
		}

		public object GetObject(Type objectType) {
			object entityObject = Activator.CreateInstance(objectType, true);
			EntityMap map = this.Mappings[objectType];
			if (map.KeyType != KeyType.User
				&& map.KeyType != KeyType.Composite) {
				if (map.AutoTrack) {
					this.StartTracking(entityObject, InitialState.Inserted);
				}
			}
			return entityObject;
		}

		public object GetObject(Type objectType, object objectKey, bool firstLevel) {
			if (objectKey == null) return null;
			object entityObject = null;
			EntityKey entityKey = new EntityKey(objectType, objectKey);
			if (this.instances.Contains(entityKey)) {
				EntityMap map = this.Mappings[objectType];
				if (map.AutoTrack) {
					Instance instance = (Instance) this.instances[entityKey];
					entityObject = instance.EntityObject;
				}
			}
			if (entityObject == null) {
				Commands commands = this.Mappings.Commands(objectType);
				using (ObjectReader reader = this.GetObjectReader(commands.KeyQuery(objectType, objectKey), firstLevel)) {
					if (reader.Read()) entityObject = reader.Current();
				}
			}
			return entityObject;
		}

		public object GetObjectKey(object entityObject) {
			EntityKey entityKey = new EntityKey(this, entityObject, true);
			return entityKey.Value;
		}

		// Includes GetObjectCount by Ken Muse (http://www.MomentsFromImpact.com)
		public int GetObjectCount(ObjectQuery objectQuery) {
			Commands commands = this.Mappings.Commands(objectQuery.ObjectType);
			// Oracle returns decimals so cast to int failed
			return Convert.ToInt32(this.Connection.GetScalarValue(objectQuery.ObjectType, CommandInfo.GetCount, commands.RecordCount(objectQuery.WhereClause)));
		}

		// Jeff Lanning (jefflanning@gmail.com): Added for OPath support.
		public object GetObject(CompiledQuery query, bool firstLevel, object[] parameters) {
			//NOTE: Longhorn 4074 throws an exception if more than one record is returned.
			//		Decided it was better (and faster) not to do the same.
			using (ObjectReader reader = this.GetObjectReader(query, true, parameters)) {
				return (reader.Read()) ? reader.Current() : null;
			}
		}

		// Jeff Lanning (jefflanning@gmail.com): Added for OPath support.
		public ObjectReader GetObjectReader(CompiledQuery query, bool firstLevel, object[] parameters) {
			return new ObjectReader(this, query, firstLevel, parameters);
		}

		public ObjectReader GetObjectReader(ObjectQuery objectQuery, bool firstLevel) {
			return new ObjectReader(this, objectQuery, firstLevel);
		}

		public ObjectReader GetObjectReader(SelectProcedure selectProcedure, bool firstLevel) {
			return new ObjectReader(this, selectProcedure, firstLevel);
		}

#if DOTNETV2
		// Jeff Lanning (jefflanning@gmail.com): Added for OPath support.
		public ObjectReader<T> GetObjectReader<T>(CompiledQuery<T> query, bool firstLevel, object[] parameters) {
			if (query.ObjectType != typeof(T)) {
				throw new ArgumentException("Type of query does not match type of method specified.", "query");
			}
			return new ObjectReader<T>(this, query, firstLevel, parameters);
		}

		public ObjectReader<T> GetObjectReader<T>(ObjectQuery<T> objectQuery, bool firstLevel) {
			return new ObjectReader<T>(this, objectQuery, firstLevel);
		}

		public ObjectReader<T> GetObjectReader<T>(SelectProcedure<T> selectProcedure, bool firstLevel) {
			return new ObjectReader<T>(this, selectProcedure, firstLevel);
		}
#endif

		// Jeff Lanning (jefflanning@gmail.com): Added for OPath support.
		public ObjectSet GetObjectSet(CompiledQuery query, bool firstLevel, object[] parameters) {
			using (ObjectReader reader = this.GetObjectReader(query, firstLevel, parameters)) {
				return this.GetObjectSet(reader);
			}
		}

		public ObjectSet GetObjectSet(ObjectQuery objectQuery, bool firstLevel) {
			using (ObjectReader reader = this.GetObjectReader(objectQuery, firstLevel)) {
				return this.GetObjectSet(reader);
			}
		}

		public ObjectSet GetObjectSet(SelectProcedure selectProcedure, bool firstLevel) {
			using (ObjectReader reader = this.GetObjectReader(selectProcedure, firstLevel)) {
				return this.GetObjectSet(reader);
			}
		}

		private ObjectSet GetObjectSet(ObjectReader reader) {
			ObjectSet objectSet = null;
			try {
				objectSet = new ObjectSet(reader.ObjectType, reader.PageIndex, reader.PageCount, reader.TotalCount);
				while (reader.Read()) {
					object entityObject = reader.Current();
					object keyValue = reader.ObjectKey();
					objectSet.Add(keyValue, entityObject);
				}
			}
			catch (ORMapperException) {
				throw;
			}
			catch (Exception exception) {
				throw new ORMapperException("GetObjectSet failed for " + reader.ObjectType.ToString(), exception);
			}
			finally {
				if (reader != null) reader.Close();
			}
			return objectSet;
		}

#if DOTNETV2
		// Jeff Lanning (jefflanning@gmail.com): Added for OPath support.
		public ObjectSet<T> GetObjectSet<T>(CompiledQuery query, bool firstLevel, object[] parameters) {
			if (query.ObjectType != typeof(T)) {
				throw new ArgumentException("Type of query does not match type of method specified.", "query");
			}
			using (ObjectReader reader = this.GetObjectReader(query, firstLevel, parameters)) {
				return this.GetObjectSet<T>(reader);
			}
		}

		public ObjectSet<T> GetObjectSet<T>(ObjectQuery<T> objectQuery, bool firstLevel) {
			using (ObjectReader reader = this.GetObjectReader(objectQuery, firstLevel)) {
				return this.GetObjectSet<T>(reader);
			}
		}

		public ObjectSet<T> GetObjectSet<T>(SelectProcedure<T> selectProcedure, bool firstLevel) {
			using (ObjectReader reader = this.GetObjectReader(selectProcedure, firstLevel)) {
				return this.GetObjectSet<T>(reader);
			}
		}

		private ObjectSet<T> GetObjectSet<T>(ObjectReader reader) {
			ObjectSet<T> objectSet = null;
			try {
				objectSet = new ObjectSet<T>(reader.PageIndex, reader.PageCount, reader.TotalCount);
				while (reader.Read()) {
					T entityObject = (T) reader.Current();
					object keyValue = reader.ObjectKey();
					objectSet.Add(keyValue, entityObject);
				}
			}
			catch (ORMapperException) {
				throw;
			}
			catch (Exception exception) {
				throw new ORMapperException("GetObjectSet<T> failed for " + reader.ObjectType.ToString(), exception);
			}
			finally {
				if (reader != null) reader.Close();
			}
			return objectSet;
		}

		//Paul Welter - Internal ObjectList<T> Calls.  
		public ObjectSet<T> GetObjectSet<T>(ObjectQuery objectQuery, bool firstLevel)
		{
			using (ObjectReader reader = this.GetObjectReader(objectQuery, firstLevel))
			{
				return this.GetObjectSet<T>(reader);
			}
		}

		public ObjectSet<T> GetObjectSet<T>(SelectProcedure selectProcedure, bool firstLevel)
		{
			using (ObjectReader reader = this.GetObjectReader(selectProcedure, firstLevel))
			{
				return this.GetObjectSet<T>(reader);
			}
		}
#endif

		public IList GetCollection(Type collectionType, ObjectQuery objectQuery, bool firstLevel) {
			using (ObjectReader reader = this.GetObjectReader(objectQuery, firstLevel)) {
				return this.GetCollection(collectionType, reader);
			}
		}

		public IList GetCollection(Type collectionType, SelectProcedure selectProcedure, bool firstLevel) {
			using (ObjectReader reader = this.GetObjectReader(selectProcedure, firstLevel)) {
				return this.GetCollection(collectionType, reader);
			}
		}

		public IList GetCollection(Type collectionType, CompiledQuery compiledQuery, bool firstLevel, params object[] parameters) {
			using (ObjectReader reader = this.GetObjectReader(compiledQuery, firstLevel, parameters)) {
				return this.GetCollection(collectionType, reader);
			}
		}

		private IList GetCollection(Type collectionType, ObjectReader reader) {
			IList collection = null;
			if (collectionType == typeof(ObjectSet)) {
				collection = new ObjectSet(reader.ObjectType, reader.PageIndex, reader.PageCount, reader.TotalCount);
			}
			else if (collectionType == typeof(ObjectList)) {
				ObjectSet objectSet = new ObjectSet(reader.ObjectType, reader.PageIndex, reader.PageCount, reader.TotalCount);
				collection = new ObjectList(this, objectSet);
			}
			else {
				collection = (IList) Activator.CreateInstance(collectionType, true);
			}

			try {
				while (reader.Read()) {
					if (collectionType == typeof(IObjectSet)) {
						((IObjectSet) collection).Add(reader.ObjectKey(), reader.Current());
					}
					else {
						collection.Add(reader.Current());
					}
				}
			}
			finally {
				if (reader != null) reader.Close();
			}
			return collection;
		}

#if DOTNETV2
		public System.Collections.ObjectModel.Collection<T> GetCollection<T>(ObjectQuery<T> objectQuery, bool firstLevel) {
			using (ObjectReader reader = this.GetObjectReader(objectQuery, firstLevel)) {
				return this.GetCollection<T>(reader);
			}
		}

		public System.Collections.ObjectModel.Collection<T> GetCollection<T>(SelectProcedure<T> selectProcedure, bool firstLevel) {
			using (ObjectReader reader = this.GetObjectReader(selectProcedure, firstLevel)) {
				return this.GetCollection<T>(reader);
			}
		}

		public System.Collections.ObjectModel.Collection<T> GetCollection<T>(CompiledQuery compiledQuery, bool firstLevel, params object[] parameters) {
			using (ObjectReader reader = this.GetObjectReader(compiledQuery, firstLevel, parameters)) {
				return this.GetCollection<T>(reader);
			}
		}

		private System.Collections.ObjectModel.Collection<T> GetCollection<T>(ObjectReader reader) {
			System.Collections.ObjectModel.Collection<T> collection = new System.Collections.ObjectModel.Collection<T>();
			try {
				while (reader.Read()) {
					collection.Add((T) reader.Current());
				}
			}
			finally {
				if (reader != null) reader.Close();
			}
			return collection;
		}
#endif

		// Jeff Lanning (jefflanning@gmail.com): Added for OPath support.
		public DataSet GetDataSet(CompiledQuery query, object[] parameters)	{
			return this.connection.GetDataSet(query.ObjectType, CommandInfo.DataSet, query, parameters);
		}

		public DataSet GetDataSet(DataSet dataSet, ObjectQuery objectQuery) {
			Commands commands = this.mappings.Commands(objectQuery.ObjectType);
			return this.connection.GetDataSet(objectQuery.ObjectType, CommandInfo.DataSet, dataSet, commands.Select(objectQuery));
		}

		public DataSet GetDataSet(DataSet dataSet, ObjectQuery objectQuery, string[] selectFields) {
			Commands commands = this.mappings.Commands(objectQuery.ObjectType);
			return this.connection.GetDataSet(objectQuery.ObjectType, CommandInfo.DataSet, dataSet, commands.Select(objectQuery, selectFields));
		}

		public DataSet GetDataSet(DataSet dataSet, SelectProcedure selectProcedure) {
			return this.connection.GetDataSet(selectProcedure.ObjectType, CommandInfo.DataSet, dataSet, selectProcedure.ProcedureName, selectProcedure.parameters);
		}

		public void StartTracking(object entityObject, InitialState initialState) {
			Instance instance = new Instance(this, entityObject, initialState);
			EntityKey entityKey = new EntityKey(this, instance.EntityObject, instance.IsPersisted);
			this.StartTracking(entityKey, instance);
		}

		internal void StartTracking(Instance instance) {
			object entityObject = instance.EntityObject;
			EntityKey entityKey = new EntityKey(this, entityObject, instance.IsPersisted);
			this.StartTracking(entityKey, instance);
		}

		internal void StartTracking(EntityKey entityKey, Instance instance) {
			this.instances[entityKey] = instance;
		}

		public void EndTracking(object entityObject) {
			EntityKey entityKey = new EntityKey(this, entityObject, true);
			if (this.instances.Contains(entityKey)) {
				this.instances.Remove(entityKey);
			}
		}

		public void ClearTracking() {
			this.instances.Clear();
		}

		// Thanks to both Allan Ritchie (A.Ritchie@ACRWebSolutions.com)
		// and Gerrod Thomas (http://www.Gerrod.com) for advice and code
		public object Resync(object entityObject) {
			EntityMap entity = this.mappings[entityObject.GetType()];
			for (int index = 0; index < entity.RelationCount; index++) {
				RelationMap relation = entity.Relation(index);
				object member = this[entityObject].GetField(relation.Member);
				ILoadOnDemand lazy = member as ILoadOnDemand;

				if (lazy != null) {
					if (lazy.IsLoaded) {
						lazy.Resync();
					}
				}
			}
			this.EndTracking(entityObject);

			EntityKey entityKey = new EntityKey(this, entityObject, true);
			return this.GetObject(entityObject.GetType(), entityKey.Value, true);
		}

		#region IDisposable Members

		public void Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing) {
			if (this.timer != null) {
				this.timer.Dispose();
				this.timer = null;
			}
			if (this.instances != null) {
				this.instances.Clear();
			}
		}

		~Context() {
			try {
				this.Dispose(false);
			}
			catch {
				// Do nothing. This happens when the underlying handle has already been released in a GC cycle.
			}
		}

		#endregion
	}
}