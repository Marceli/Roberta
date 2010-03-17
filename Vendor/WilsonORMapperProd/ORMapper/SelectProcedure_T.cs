//**************************************************************//
// Paul Wilson -- www.WilsonDotNet.com -- Paul@WilsonDotNet.com //
// Feel free to use and modify -- just leave these credit lines //
// I also always appreciate any other public credit you provide //
//**************************************************************//
#if DOTNETV2
using System;
using System.Data;
using Wilson.ORMapper.Internals;

namespace Wilson.ORMapper
{
	/// <summary>
	///     The SelectProcedure class is used to load an entity
	///     object collection with a stored procedure.
	/// </summary>
	/// <example>The following example shows how to use the
	/// SelectProcedure to get all Contacts with names that start with A.
	///		<code>
	/// <![CDATA[
	///	public static ObjectSpace Manager; // See Initialization Example
	///	
	///	// Get All Contacts with Names that start with A
	///	SelectProcedure<Contact> selectProc = new SelectProcedure<Contact>("RetrieveContacts");
	///	selectProc.AddParameter("@ContactName", "A");
	///	ObjectSet<Contact> contacts = Manager.GetObjectSet<Contact>(selectProc);
	/// ]]>
	///		</code>
	/// </example>

	[Serializable()]
	public class SelectProcedure<T> : SelectProcedure
	{
		/// <summary>Creates a new SelectProcedure instance</summary>
		/// <param name="procedureName">The name of the procedure to be executed</param>
		public SelectProcedure(string procedureName)
			: base(typeof(T), procedureName) {}
	}
}
#endif