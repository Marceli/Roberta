//**************************************************************//
// Paul Wilson -- www.WilsonDotNet.com -- Paul@WilsonDotNet.com //
// Feel free to use and modify -- just leave these credit lines //
// I also always appreciate any other public credit you provide //
//**************************************************************//
using System;

namespace Wilson.ORMapper
{
	/// <summary>
	///     Optional Interface used to catch Events
	/// </summary>
	public interface IObjectNotification
	{
		/// <summary>Triggered after an entity object is created in the database</summary>
		/// <param name="transaction">The current transaction object for custom cases</param>
		void OnCreated(Transaction transaction);

		/// <summary>Triggered before an entity object is created in the database</summary>
		/// <param name="transaction">The current transaction object for custom cases</param>
		void OnCreating(Transaction transaction);
		
		/// <summary>Triggered after an entity object is deleted in the database</summary>
		/// <param name="transaction">The current transaction object for custom cases</param>
		void OnDeleted(Transaction transaction);
		
		/// <summary>Triggered before an entity object is deleted in the database</summary>
		/// <param name="transaction">The current transaction object for custom cases</param>
		void OnDeleting(Transaction transaction);
		
		/// <summary>Triggered when an entity object is materialized from the database</summary>
		/// <param name="dataRecord">The current dataRecord used to materialize object</param>
		void OnMaterialized(System.Data.IDataRecord dataRecord);
		
		/// <summary>Triggered when an error has occurred persisting an entity object</summary>
		/// <param name="transaction">The current transaction object for custom cases</param>
		/// <param name="exception">The current exception encountered in persistence</param>
		void OnPersistError(Transaction transaction, Exception exception);
		
		/// <summary>Triggered after an entity object is updated in the database</summary>
		/// <param name="transaction">The current transaction object for custom cases</param>
		void OnUpdated(Transaction transaction);
		
		/// <summary>Triggered before an entity object is updated in the database</summary>
		/// <param name="transaction">The current transaction object for custom cases</param>
		void OnUpdating(Transaction transaction);
	}
}
