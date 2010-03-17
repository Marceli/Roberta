//**************************************************************//
// Paul Wilson -- www.WilsonDotNet.com -- Paul@WilsonDotNet.com //
// Feel free to use and modify -- just leave these credit lines //
// I also always appreciate any other public credit you provide //
//**************************************************************//
using System;
using System.Data;
using System.Reflection;

namespace Wilson.ORMapper.Internals
{
	internal sealed class ProviderFactory
	{
		public static IDbConnection GetConnection(string connection, CustomProvider customProvider) {
			if (!customProvider.IsCustom) {
				// These were split out into mini-methods to support medium trust
				switch (customProvider.Provider) {
					case Provider.MsSql: return GetMsSqlConnection(connection);
					case Provider.Sql2005: return GetMsSqlConnection(connection);
					case Provider.Access: return GetOleDbConnection(connection);
					case Provider.Oracle: return GetOracleConnection(connection);
					case Provider.OleDb: return GetOleDbConnection(connection);
					case Provider.Odbc: return GetOdbcConnection(connection);
					default:
						throw new ORMapperException("Internals: Data Provider was Invalid");
				}
			}
			else {
				IDbConnection connect = (IDbConnection) Activator.CreateInstance(customProvider.Connection);
				connect.ConnectionString = connection;
				return connect;
			}
		}

		private static IDbConnection GetMsSqlConnection(string connection) {
			return new System.Data.SqlClient.SqlConnection(connection);
		}

		private static IDbConnection GetOracleConnection(string connection) {
			return new System.Data.OracleClient.OracleConnection(connection);
		}

		private static IDbConnection GetOleDbConnection(string connection) {
			return new System.Data.OleDb.OleDbConnection(connection);
		}

		private static IDbConnection GetOdbcConnection(string connection) {
			return new System.Data.Odbc.OdbcConnection(connection);
		}

		public static IDbDataAdapter GetAdapter(IDbCommand command, CustomProvider customProvider) {
			if (!customProvider.IsCustom) {
				// These were split out into mini-methods to support medium trust
				switch (customProvider.Provider) {
					case Provider.MsSql: return GetMsSqlAdapter(command);
					case Provider.Sql2005: return GetMsSqlAdapter(command);
					case Provider.Access: return GetOleDbAdapter(command);
					case Provider.Oracle: return GetOracleAdapter(command);
					case Provider.OleDb: return GetOleDbAdapter(command);
					case Provider.Odbc: return GetOdbcAdapter(command);
					default:
						throw new ORMapperException("Internals: Data Provider was Invalid");
				}
			}
			else {
				IDbDataAdapter adapter = (IDbDataAdapter) Activator.CreateInstance(customProvider.DataAdapter);
				adapter.SelectCommand = command;
				return adapter;
			}
		}

		private static IDbDataAdapter GetMsSqlAdapter(IDbCommand command) {
			return new System.Data.SqlClient.SqlDataAdapter((System.Data.SqlClient.SqlCommand)command);
		}

		private static IDbDataAdapter GetOracleAdapter(IDbCommand command) {
			return new System.Data.OracleClient.OracleDataAdapter((System.Data.OracleClient.OracleCommand)command);
		}

		private static IDbDataAdapter GetOleDbAdapter(IDbCommand command) {
			return new System.Data.OleDb.OleDbDataAdapter((System.Data.OleDb.OleDbCommand)command);
		}

		private static IDbDataAdapter GetOdbcAdapter(IDbCommand command) {
			return new System.Data.Odbc.OdbcDataAdapter((System.Data.Odbc.OdbcCommand)command);
		}

		public static Commands GetCommands(EntityMap entity, CustomProvider customProvider) {
			if (!customProvider.IsCustom) {
				switch (customProvider.Provider) {
					case Provider.MsSql : return new MSCommands(entity, customProvider);
					case Provider.Sql2005 : return new Sql2005Commands(entity, customProvider);
					case Provider.Access : return new MSCommands(entity, customProvider);
					case Provider.Oracle : return new OracleCommands(entity, customProvider);
					default : return new Commands(entity, customProvider);
				}
			}
			else {
				return new Commands(entity, customProvider);
			}
		}

		private ProviderFactory() {}
	}
}
