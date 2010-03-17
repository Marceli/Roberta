//**************************************************************//
// Paul Wilson -- www.WilsonDotNet.com -- Paul@WilsonDotNet.com //
// Feel free to use and modify -- just leave these credit lines //
// I also always appreciate any other public credit you provide //
//**************************************************************//
// Commands now inherit the connection timeout for long-running queries
// Fix for Providers without Timeouts by Stephan Wagner (http://www.calac.net)
using System;
using System.Data;
using System.Diagnostics;
using Wilson.ORMapper.Query;

namespace Wilson.ORMapper.Internals
{
	internal class Connection
	{
		public static readonly string SplitToken = "<$>";
		private string connection;
		private CustomProvider provider;
		private int commandTimeout;
		private bool supportsTimeout = true;
		private IInterceptCommand interceptor = null;

		public void SetInterceptor(IInterceptCommand interceptor) {
			this.interceptor = interceptor;
		}

		internal Connection(string connectString, CustomProvider customProvider) {
			if (connectString == null || connectString.Length == 0) {
				throw new ORMapperException("Internals: ConnectionString was Empty");
			}
			if (customProvider.Provider == Provider.Access && connectString.ToUpper().IndexOf("PROVIDER") < 0) {
				this.connection = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + connectString;
			}
			else {
				this.connection = connectString;
			}
			this.provider = customProvider;

			// Check Timeout Support and Valid Connection String
			IDbConnection connection = null;
			try {
				connection = ProviderFactory.GetConnection(this.connection, this.provider);
				this.commandTimeout = connection.ConnectionTimeout;
				try {
					IDbCommand command = connection.CreateCommand();
					command.CommandTimeout = this.commandTimeout;
				}
				catch {
					this.supportsTimeout = false;
				}
				connection.Open();
			}
			catch (Exception exception) {
				throw new ORMapperException("ObjectSpace: Connection String is Invalid - " + exception.Message, exception);
			}
			finally {
				if (connection != null) { connection.Close(); }
			}
		}

		// Jeff Lanning (jefflanning@gmail.com): Added overload for OPath support
		public IDataReader GetDataReader(Type entityType, CommandInfo commandInfo, CompiledQuery query, object[] parameterValues) {
			IDbConnection conn = null;
			try {
				conn = ProviderFactory.GetConnection(this.connection, this.provider);
				IDbCommand cmd = CreateDbCommand(Guid.NewGuid(), entityType, commandInfo, conn, query, parameterValues);
				cmd.Connection.Open();
				return cmd.ExecuteReader(CommandBehavior.CloseConnection);
			}
			catch {
				if( conn != null ) conn.Close();
				throw;
			}
		}

		public IDataReader GetDataReader(Type entityType, CommandInfo commandInfo, string sqlStatement, params Parameter[] parameters) {
			IDbCommand command = null;
			return this.GetDataReader(entityType, commandInfo, out command, sqlStatement, parameters);
		}

		public IDataReader GetDataReader(Type entityType, CommandInfo commandInfo, out IDbCommand dbCommand, string sqlStatement, params Parameter[] parameters) {
			IDbConnection connection = null;
			try {
				connection = ProviderFactory.GetConnection(this.connection, this.provider);
				dbCommand = CreateDbCommand(Guid.NewGuid(), entityType, commandInfo, connection, sqlStatement, parameters);
				connection.Open(); // Must Close DataReader to Close Connection
				return dbCommand.ExecuteReader(CommandBehavior.CloseConnection);
			}
			catch {
				if (connection != null) { connection.Close(); }
				throw;
			}
		}

		public object GetScalarValue(Type entityType, CommandInfo commandInfo, string sqlStatement, params Parameter[] parameters) {
			string sqlStatement1 = sqlStatement;
			string sqlStatement2 = null;
			int splitIndex = sqlStatement.IndexOf(Connection.SplitToken);
			if (splitIndex >= 0) {
				sqlStatement1 = sqlStatement.Substring(0, splitIndex);
				sqlStatement2 = sqlStatement.Substring(splitIndex + Connection.SplitToken.Length);
			}

			object value = null;

			using (IDbConnection connection = ProviderFactory.GetConnection(this.connection, this.provider))
			{
				IDbCommand command = CreateDbCommand(Guid.NewGuid(), entityType, commandInfo, connection, sqlStatement1, parameters);
				if (this.provider.UseInsertReturn && sqlStatement.ToUpper().StartsWith("INSERT")) {
					IDbDataParameter dbParameter = command.CreateParameter();
					dbParameter.ParameterName = "KeyField";
					dbParameter.DbType = DbType.Decimal;
					dbParameter.Direction = ParameterDirection.ReturnValue;
					command.Parameters.Add(dbParameter);
				}
				connection.Open(); // Connection will be Closed in all Cases
				if (sqlStatement2 != null) {
					bool success = (command.ExecuteNonQuery() > 0);
					if (!success) return null;
					command = CreateDbCommand(Guid.NewGuid(), entityType, CommandInfo.GetScalar, connection, sqlStatement2, null);
				}
				if (this.provider.UseInsertReturn && sqlStatement.ToUpper().StartsWith("INSERT")) {
					if (command.ExecuteNonQuery() > 0) {
						value = ((IDbDataParameter)command.Parameters["KeyField"]).Value;
					}
				}
				else {
					value = command.ExecuteScalar();
				}
				this.SetOutputParameters(command, parameters);
			}
			return value;
		}

		public int ExecuteCommand(Type entityType, CommandInfo commandInfo, string sqlStatement, params Parameter[] parameters) {
			int output = 0; // Success if any Rows are Affected

			using (IDbConnection connection = ProviderFactory.GetConnection(this.connection, this.provider))
			{
				IDbCommand command = CreateDbCommand(Guid.NewGuid(), entityType, commandInfo, connection, sqlStatement, parameters);
				connection.Open(); // Connection will be Closed in all Cases
				output = command.ExecuteNonQuery();
				this.SetOutputParameters(command, parameters);
			}

			return output;
		}

		// Jeff Lanning (jefflanning@gmail.com): Added overload for OPath support
		public DataSet GetDataSet(Type entityType, CommandInfo commandInfo, CompiledQuery query, object[] parameterValues) {
			using (IDbConnection conn = ProviderFactory.GetConnection(this.connection, this.provider))
			{
				IDbCommand cmd = CreateDbCommand(Guid.NewGuid(), entityType, commandInfo, conn, query, parameterValues);
				IDbDataAdapter adapter = ProviderFactory.GetAdapter(cmd, this.provider);
				DataSet dataSet = new DataSet("WilsonORMapper");
				adapter.Fill(dataSet);				
				return dataSet;
			}
		}

		public DataSet GetDataSet(Type entityType, CommandInfo commandInfo, string sqlStatement, params Parameter[] parameters) {
			return this.GetDataSet(entityType, commandInfo, null, sqlStatement, parameters);
		}

		// Includes support for typed datasets from Ben Priebe (http://stickfly.com)
		public DataSet GetDataSet(Type entityType, CommandInfo commandInfo, DataSet dataSet, string sqlStatement, params Parameter[] parameters) {
			if (dataSet == null) dataSet = new DataSet("WilsonORMapper");
			using (IDbConnection connection = ProviderFactory.GetConnection(this.connection, this.provider))
			{
				IDbCommand command = CreateDbCommand(Guid.NewGuid(), entityType, commandInfo, connection, sqlStatement, parameters);
				IDbDataAdapter adapter = ProviderFactory.GetAdapter(command, this.provider);
				if (dataSet.Tables.Count > 0) {
					string tableName = dataSet.Tables[0].TableName;
					(adapter as System.Data.Common.DbDataAdapter).Fill(dataSet, tableName);
				}
				else {
					adapter.Fill(dataSet);
				}
				this.SetOutputParameters(command, parameters);
			}
			return dataSet;
		}


		public IDbTransaction GetTransaction(IsolationLevel isolationLevel) {
			IDbConnection connection = ProviderFactory.GetConnection(this.connection, this.provider);
			connection.Open();
			try {
				if (isolationLevel == IsolationLevel.ReadCommitted) {
					return connection.BeginTransaction();
				}
				else {
					return connection.BeginTransaction(isolationLevel);
				}
			}
			catch (Exception ex) {
				System.Diagnostics.Debug.WriteLine(ex.Message);
				throw;
			}
		}

		public object TransactionScalar(Guid transactionId, Type entityType, CommandInfo commandInfo, IDbTransaction transaction, string sqlStatement, params Parameter[] parameters) {
			object value = null;

			string sqlStatement1 = sqlStatement;
			string sqlStatement2 = null;
			int splitIndex = sqlStatement.IndexOf(Connection.SplitToken);
			if (splitIndex >= 0) {
				sqlStatement1 = sqlStatement.Substring(0, splitIndex);
				sqlStatement2 = sqlStatement.Substring(splitIndex + Connection.SplitToken.Length);
			}

			IDbCommand command = CreateDbCommand(transactionId, entityType, commandInfo, transaction.Connection, sqlStatement1, parameters);
			command.Transaction = transaction;
			if (this.provider.UseInsertReturn && sqlStatement.ToUpper().StartsWith("INSERT")) {
				IDbDataParameter dbParameter = command.CreateParameter();
				dbParameter.ParameterName = "KeyField";
				dbParameter.DbType = DbType.Decimal;
				dbParameter.Direction = ParameterDirection.ReturnValue;
				command.Parameters.Add(dbParameter);
			}
			if (sqlStatement2 != null) {
				command.ExecuteNonQuery();
				command = CreateDbCommand(transactionId, entityType, CommandInfo.GetScalar, transaction.Connection, sqlStatement2, null);
				command.Transaction = transaction;
			}
			if (this.provider.UseInsertReturn && sqlStatement.ToUpper().StartsWith("INSERT")) {
				if (command.ExecuteNonQuery() > 0) {
					value = ((IDbDataParameter)command.Parameters["KeyField"]).Value;
				}
			}
			else {
				value = command.ExecuteScalar();
			}
			this.SetOutputParameters(command, parameters);

			return value;
		}

		public int TransactionCommand(Guid transactionId, Type entityType, CommandInfo commandInfo, IDbTransaction transaction, string sqlStatement, params Parameter[] parameters) {
			IDbCommand command = CreateDbCommand(transactionId, entityType, commandInfo, transaction.Connection, sqlStatement, parameters);
			command.Transaction = transaction;
			int output = command.ExecuteNonQuery();
			this.SetOutputParameters(command, parameters);
			return output;
		}

		public DataSet TransactionDataSet(Guid transactionId, Type entityType, CommandInfo commandInfo, IDbTransaction transaction, string sqlStatement, params Parameter[] parameters) {
			return this.TransactionDataSet(transactionId, entityType, commandInfo, transaction, null, sqlStatement, parameters);
		}

		// Includes support for typed datasets from Ben Priebe (http://stickfly.com)
		public DataSet TransactionDataSet(Guid transactionId, Type entityType, CommandInfo commandInfo, IDbTransaction transaction, DataSet dataSet, string sqlStatement, params Parameter[] parameters) {
			IDbCommand command = CreateDbCommand(transactionId, entityType, commandInfo, transaction.Connection, sqlStatement, parameters);
			command.Transaction = transaction;
			IDbDataAdapter adapter = ProviderFactory.GetAdapter(command, this.provider);

			if (dataSet == null) dataSet = new DataSet("WilsonORMapper");
			if (dataSet.Tables.Count > 0) 
			{
				string tableName = dataSet.Tables[0].TableName;
				(adapter as System.Data.Common.DbDataAdapter).Fill(dataSet, tableName);
			}
			else {
				adapter.Fill(dataSet);
			}
			this.SetOutputParameters(command, parameters);
			return dataSet;
		}

		private IDbCommand CreateDbCommand(Guid transactionId, Type entityType, CommandInfo commandInfo, IDbConnection conn, string sqlStatement, Parameter[] parameters) {
			IDbCommand cmd = conn.CreateCommand();
			cmd.CommandText = sqlStatement;
			cmd.CommandType = this.GetCommandType(sqlStatement);
			if (this.supportsTimeout) cmd.CommandTimeout = this.commandTimeout;

			if (parameters != null) {
				for (int i = 0; i < parameters.Length; i++) {
					cmd.Parameters.Add(this.GetParameter(cmd, parameters[i]));
				}
			}

			this.InterceptCommand(transactionId, entityType, commandInfo, cmd);
			return cmd;
		}

		private IDbCommand CreateDbCommand(Guid transactionId, Type entityType, CommandInfo commandInfo, IDbConnection conn, CompiledQuery query, object[] parameterValues) {
			IDbCommand cmd = conn.CreateCommand();
			cmd.CommandText = query.SqlQuery;
			cmd.CommandType = CommandType.Text;
			if (this.supportsTimeout) {
				cmd.CommandTimeout = (query.baseQuery != null) ? query.baseQuery.CommandTimeout : this.commandTimeout;
			}

			// build the parameter array (use the ordinals to get them in the correct order)
			Parameter[] parameters = new Parameter[query.parameterCount];
			if (parameterValues != null) {
				if (parameterValues.Length != parameters.Length) {
					throw new Exception("Number of parameters in the expression does not match number of values provided.");
				}
				
				for (int i = 0; i < parameters.Length; i++) {
					OPathParameter p = query.parameterTable[i];
					parameters[p.Ordinal] = new Parameter(p.Name, parameterValues[i], null);
				}
			}
			else // use the parameters in the base query (if any)
			{
				//note: the compiler will only allow an exact number of parameters in the base query... or none at all.
				for (int i = 0; i < parameters.Length; i++) {
					OPathParameter p = query.parameterTable[i];
					parameters[p.Ordinal] = new Parameter(p.Name, p.Value, null);
				}
			}

			// add the parameters to the command
			for (int i = 0; i < parameters.Length; i++) {
				cmd.Parameters.Add(this.GetParameter(cmd, parameters[i]));
			}
	
			this.InterceptCommand(transactionId, entityType, commandInfo, cmd);
			return cmd;
		}

		private CommandType GetCommandType(string sqlStatement) {
			if (sqlStatement.IndexOf(' ') < 0) {
				return CommandType.StoredProcedure;
			}
			else {
				return CommandType.Text;
			}
		}

		internal bool HasOutputParameters(IDbCommand command, params Parameter[] parameters) {
			for( int i = 0; i < parameters.Length; i++ ) {
				Parameter parameter = parameters[i];
				if ((parameter.Direction == ParameterDirection.Output || parameter.Direction == ParameterDirection.InputOutput)
						&& command.Parameters.Contains(parameter.Name)) {
					return true;
				}
			}
			return false;
		}

		// Output Parameter Support by Alister McIntyre (http://www.aruspex.com.au)
		internal void SetOutputParameters(IDbCommand command, params Parameter[] parameters) {
			for( int i = 0; i < parameters.Length; i++ ) {
				Parameter parameter = parameters[i];
				if ((parameter.Direction == ParameterDirection.Output || parameter.Direction == ParameterDirection.InputOutput)
						&& command.Parameters.Contains(parameter.Name)) {
					parameter.value = ((IDataParameter)command.Parameters[parameter.Name]).Value;
				}
			}
		}

		private IDbDataParameter GetParameter(IDbCommand command, Parameter parameter) {
			IDbDataParameter dbParameter = command.CreateParameter();
			dbParameter.ParameterName = this.provider.GetParameterName(parameter.Name);
			dbParameter.Direction = parameter.Direction;

#if DOTNETV2 && V2BETA2
			if (parameter.Value is INullableValue) {
				INullableValue nullable = (INullableValue) parameter.Value;
				if (nullable.HasValue) {
					if (this.provider.UseDateTimeString && nullable.Value is DateTime) {
						dbParameter.Value = nullable.Value.ToString();
					}
					else {
						dbParameter.Value = nullable.Value;
					}
				}
				else {
					dbParameter.Value = DBNull.Value;
				}

				// Improved Parameter Typing necessary for some Providers that Do Not Check
				if (parameter.Type != null) {
					if (parameter.Type == typeof(byte?[])) dbParameter.DbType = DbType.Binary;
					else if (parameter.Type == typeof(bool?)) dbParameter.DbType = DbType.Boolean;
					else if (parameter.Type == typeof(byte?)) dbParameter.DbType = DbType.Byte;
					else if (parameter.Type == typeof(DateTime?)) dbParameter.DbType = DbType.DateTime;
					else if (parameter.Type == typeof(decimal?)) dbParameter.DbType = DbType.Decimal;
					else if (parameter.Type == typeof(double?)) dbParameter.DbType = DbType.Double;
					else if (parameter.Type == typeof(Guid?)) dbParameter.DbType = DbType.Guid;
					else if (parameter.Type == typeof(short?)) dbParameter.DbType = DbType.Int16;
					else if (parameter.Type == typeof(int?)) dbParameter.DbType = DbType.Int32;
					else if (parameter.Type == typeof(long?)) dbParameter.DbType = DbType.Int64;
					else if (parameter.Type == typeof(sbyte?)) dbParameter.DbType = DbType.SByte;
					else if (parameter.Type == typeof(float?)) dbParameter.DbType = DbType.Single;
					else if (parameter.Type == typeof(TimeSpan?)) dbParameter.DbType = DbType.Time;
					else if (parameter.Type == typeof(ushort?)) dbParameter.DbType = DbType.UInt16;
					else if (parameter.Type == typeof(uint?)) dbParameter.DbType = DbType.UInt32;
					else if (parameter.Type == typeof(ulong?)) dbParameter.DbType = DbType.UInt64;
					else if (parameter.Type.IsEnum) dbParameter.DbType = DbType.Int32; // Jason Shigley
					// Allow Provider Default -- Oracle uses AnsiString
					// else dbParameter.DbType = DbType.String;
				}
			}
			else {
#endif
			if (parameter.Value == null) {
					dbParameter.Value = DBNull.Value;
				}
				else {
					// DateTime provider-specific min/max from Marc Brooks (http://musingmarc.blogspot.com)
					if (parameter.Value is DateTime) {
						DateTime dateValue = (DateTime)parameter.Value;
						if (dateValue == DateTime.MinValue) {
							dateValue = this.provider.MinimumDate;
						}
						else if (dateValue == DateTime.MaxValue) {
							dateValue = this.provider.MaximumDate;
						}
						if (this.provider.UseDateTimeString) {
							dbParameter.Value = dateValue.ToString();
						}
						else {
							dbParameter.Value = dateValue;
						}
					}
					else {
						dbParameter.Value = parameter.Value;
					}
				}

				// Improved Parameter Typing necessary for some Providers that Do Not Check
				if (parameter.Type != null) {
					if (parameter.Type == typeof(byte[])) dbParameter.DbType = DbType.Binary;
					else if (parameter.Type == typeof(bool)) dbParameter.DbType = DbType.Boolean;
					else if (parameter.Type == typeof(byte)) dbParameter.DbType = DbType.Byte;
					else if (parameter.Type == typeof(DateTime)) dbParameter.DbType = DbType.DateTime;
					else if (parameter.Type == typeof(decimal)) dbParameter.DbType = DbType.Decimal;
					else if (parameter.Type == typeof(double)) dbParameter.DbType = DbType.Double;
					else if (parameter.Type == typeof(Guid)) dbParameter.DbType = DbType.Guid;
					else if (parameter.Type == typeof(short)) dbParameter.DbType = DbType.Int16;
					else if (parameter.Type == typeof(int)) dbParameter.DbType = DbType.Int32;
					else if (parameter.Type == typeof(long)) dbParameter.DbType = DbType.Int64;
					else if (parameter.Type == typeof(sbyte)) dbParameter.DbType = DbType.SByte;
					else if (parameter.Type == typeof(float)) dbParameter.DbType = DbType.Single;
					else if (parameter.Type == typeof(TimeSpan)) dbParameter.DbType = DbType.Time;
					else if (parameter.Type == typeof(ushort)) dbParameter.DbType = DbType.UInt16;
					else if (parameter.Type == typeof(uint)) dbParameter.DbType = DbType.UInt32;
					else if (parameter.Type == typeof(ulong)) dbParameter.DbType = DbType.UInt64;
					else if (parameter.Type.IsEnum) dbParameter.DbType = DbType.Int32; // Jason Shigley
					// Allow Provider Default -- Oracle uses AnsiString
					// else dbParameter.DbType = DbType.String;
				}
#if DOTNETV2 && V2BETA2
			}
#endif
			return dbParameter;
		}

		internal void InterceptCommand(Guid transactionId, Type entityType, CommandInfo commandInfo, IDbCommand dbCommand) {
			if (this.interceptor != null) {
				try {
					this.interceptor.InterceptCommand(transactionId, entityType, commandInfo, dbCommand);
				}
				catch (Exception error) {
					Debug.WriteLine("Interceptor Error: " + error.Message);
				}
			}
			this.DebugWrite(transactionId, entityType, commandInfo, dbCommand);
		}

		private void DebugWrite(Guid transactionId, Type entityType, CommandInfo commandInfo, IDbCommand dbCommand) {
// Jeff Lanning (jefflanning@gmail.com): Changed from DEBUG to DEBUG_MAPPER to control output spew when developing with debug builds.
#if DEBUG_MAPPER
			string debug = transactionId.ToString() + " - " + commandInfo.ToString();
			if (dbCommand != null) {
				debug += ": " + dbCommand.CommandText;
				for (int index = 0; index < dbCommand.Parameters.Count; index++) {
					if (index == 0) debug += "\r\n  ";
					IDbDataParameter parameter = dbCommand.Parameters[index] as IDbDataParameter;
					debug += parameter.ParameterName + " = " + parameter.Value + ", ";
				}
			}
			Debug.WriteLine(debug);
#endif
		}
	}
}
