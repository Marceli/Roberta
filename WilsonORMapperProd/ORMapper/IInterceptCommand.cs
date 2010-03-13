//**************************************************************//
// Paul Wilson -- www.WilsonDotNet.com -- Paul@WilsonDotNet.com //
// Feel free to use and modify -- just leave these credit lines //
// I also always appreciate any other public credit you provide //
//**************************************************************//
using System;
using System.Data;

namespace Wilson.ORMapper
{
	/// <summary>
	///     Optional Interface used for Intercepting all Database Commands so
	///     that Logging can be implemented or Sql Commands modified if needed.
	/// </summary>
	public interface IInterceptCommand
	{
		/// <summary>
		///     Method to implement to handle Interception of all Database Commands.
		/// </summary>
		void InterceptCommand(Guid transactionId, Type entityType, CommandInfo commandInfo, IDbCommand dbCommand);
	}

	/// <summary>
	/// Information about the type of database command being intercepted.
	/// </summary>
	public enum CommandInfo {
		/// <summary>A New Transaction was Started.</summary>
		BeginTran,
		/// <summary>The Transaction was Committed.</summary>
		CommitTran,
		/// <summary>The Transaction was Rolled Back.</summary>
		RollbackTran,
		/// <summary>ExecuteScalar method was called.</summary>
		GetScalar,
		/// <summary>A Count of Objects was returned.</summary>
		GetCount,
		/// <summary>GetObject/Set/Reader/Collection.</summary>
		Select,
		/// <summary>PersistChanges for New Objects.</summary>
		Insert,
		/// <summary>PersistChanges for Updated Objects.</summary>
		Update,
		/// <summary>PersistChanges for Marked Deletes.</summary>
		Delete,
		/// <summary>The GetDataSet method was called.</summary>
		DataSet,
		/// <summary>ExecuteCommand method was called.</summary>
		Command,
		/// <summary>ExecuteUpdate method was called.</summary>
		BatchUpdate,
		/// <summary>ExecuteDelete method was called.</summary>
		BatchDelete,
		/// <summary>Insert for Many-to-Many Relations.</summary>
		ManyInsert,
		/// <summary>Delete for Many-to-Many Relations.</summary>
		ManyDelete,
		/// <summary>Cascade Delete of Child Relations.</summary>
		CascadeDelete
	}
}