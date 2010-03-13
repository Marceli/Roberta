//**************************************************************//
// Paul Wilson -- www.WilsonDotNet.com -- Paul@WilsonDotNet.com //
// Feel free to use and modify -- just leave these credit lines //
// I also always appreciate any other public credit you provide //
//**************************************************************//
// Thanks to Allan Ritchie (A.Ritchie@ACRWebSolutions.com) for  //
// advice and code and significant assistance with transactions //
//**************************************************************//
using System;
using System.Collections;
using System.Data;
using Wilson.ORMapper.Internals;

namespace Wilson.ORMapper
{
	/// <summary>
	///     The Transaction class is used to allow greater
	///     control over the persistence of multiple objects.
	/// </summary>
	/// <example>The following example shows how to use Transaction:
	///		<code>
	///	public static ObjectSpace Manager; // See Initialization Example
	///	
	///	// Persist entityObject1 and entityObject2 in Transaction
	///	Transaction transaction = null;
	///	try {
	///	  transaction = Manager.BeginTransaction();
	///	  transaction.PersistChanges(entityObject1);
	///	  transaction.PersistChanges(entityObject2);
	///	  transaction.Commit();
	///	}
	///	catch {
	///	  transaction.Rollback();
	///	}
	///	finally {
	///	  transaction.Dispose();
	///	}
	///		</code>
	/// </example>
	
	public class Transaction : IDisposable
	{
		private Context context;
		internal IDbTransaction transaction;
		internal IDbConnection connection;
		private ArrayList instances = new ArrayList();
		internal readonly Guid id = Guid.NewGuid();

		internal Transaction(Context context, IsolationLevel isolationLevel) {
			this.context = context;
			this.transaction = this.context.Connection.GetTransaction(isolationLevel);
			this.connection = this.transaction.Connection;
			this.context.Connection.InterceptCommand(this.id, null, CommandInfo.BeginTran, null);
		}

		/// <summary>
		///     Commit the current transaction to the database.
		/// </summary>
		public void Commit() {
			try {
				this.transaction.Commit();
				foreach (PersistOptions option in instances) {
					option.Instance.CommitChanges(option.Depth);
				}
			}
			finally {
				try {
					this.context.Connection.InterceptCommand(this.id, null, CommandInfo.CommitTran, null);
				}
				finally {
					this.Dispose();
				}
			}
		}

		/// <summary>
		///     Rollback the current transaction in the database.
		/// </summary>
		public void Rollback() {
			try {
				this.transaction.Rollback();
				foreach (PersistOptions option in instances) {
					option.Instance.RollbackChanges(option.Depth);
				}
			}
			catch (InvalidOperationException exception) {
				// Handle sporadic false rollback exception
				if (exception.Message.EndsWith("Transaction has completed; it is no longer usable.")) {
					System.Diagnostics.Debug.WriteLine("Rollback: " + exception.Message);
				}
				else {
					throw;
				}
			}
			finally {
				try {
					this.context.Connection.InterceptCommand(this.id, null, CommandInfo.RollbackTran, null);
				}
				finally {
					this.Dispose();
				}
			}
		}

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
			return this.GetDataSet(null, query);
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
			return this.GetDataSet(null, query, selectFields);
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
			return this.GetDataSet(null, selectProcedure);
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
			return this.context.Connection.TransactionDataSet(this.id, null, CommandInfo.DataSet, this.transaction, sqlStatement);
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
			Commands commands = this.context.Mappings.Commands(query.ObjectType.ToString());
			return this.context.Connection.TransactionDataSet(this.id, objectQuery.ObjectType, CommandInfo.DataSet, this.transaction, dataSet, commands.Select(query));
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
			Commands commands = this.context.Mappings.Commands(query.ObjectType.ToString());
			return this.context.Connection.TransactionDataSet(this.id, objectQuery.ObjectType, CommandInfo.DataSet, this.transaction, dataSet, commands.Select(query, selectFields));
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
			return this.context.Connection.TransactionDataSet(this.id, selectProcedure.ObjectType, CommandInfo.DataSet, this.transaction, dataSet, selectProcedure.ProcedureName, selectProcedure.parameters);
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
			return this.context.Connection.TransactionDataSet(this.id, null, CommandInfo.DataSet, this.transaction, dataSet, sqlStatement);
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
			return this.context.Connection.TransactionCommand(this.id, selectProcedure.ObjectType, CommandInfo.Command, this.transaction,
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
			return this.context.Connection.TransactionCommand(this.id, null, CommandInfo.Command, this.transaction, sqlStatement);
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
			return this.context.Connection.TransactionScalar(this.id, selectProcedure.ObjectType, CommandInfo.GetScalar, this.transaction,
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
			return this.context.Connection.TransactionScalar(this.id, null, CommandInfo.GetScalar, this.transaction, sqlStatement);
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
			Commands commands = this.context.Mappings.Commands(objectType.ToString());
			return this.context.Connection.TransactionCommand(this.id, objectType, CommandInfo.BatchUpdate, this.transaction,
				commands.CreateUpdate(whereClause, updateClause));
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
			Commands commands = this.context.Mappings.Commands(objectType.ToString());
			return this.context.Connection.TransactionCommand(this.id, objectType, CommandInfo.BatchDelete, this.transaction,
				commands.CreateDelete(whereClause));
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
			foreach (object entityObject in entityObjects) {
				Internals.Instance instance = this.context[entityObject];
				// Jeff Lanning (jefflanning@gmail.com): Added null check for better error message.
				if (instance == null) {
					throw new ORMapperException("Entity object '" + entityObject.GetType().ToString() + "' is not being tracked.");
				}
				instances.Add(new PersistOptions(instance, persistDepth));
				instance.PersistChanges(this, persistDepth);
			}
		}

		#region IDisposable Members

		/// <summary>Close the Connection of the Transaction</summary>
		public void Dispose() {
			if (this.connection != null) {
				this.connection.Close();
			}
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Frees resources and perform other cleanup operations before this instance is reclaimed by garbage collection.
		/// </summary>
		~Transaction() {
			try {
				this.Dispose();
			}
			catch {
				// Do nothing. This happens when the underlying handle has already been released in a GC cycle.
			}
		}

		#endregion
	}

	// Jeff Lanning (jefflanning@gmail.com): Keep weak reference alive during transaction.
	internal class PersistOptions
	{
		private Instance instance;
		private PersistDepth depth;
		private object entityObject;

		public Instance Instance {
			get { return this.instance; }
		}

		public PersistDepth Depth {
			get { return this.depth; }
		}

		internal PersistOptions(Instance instance, PersistDepth depth) {
			this.instance = instance;
			this.depth = depth;
			this.entityObject = instance.EntityObject;
		}
	}
}
