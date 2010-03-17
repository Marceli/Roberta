//**************************************************************//
// Paul Wilson -- www.WilsonDotNet.com -- Paul@WilsonDotNet.com //
// Feel free to use and modify -- just leave these credit lines //
// I also always appreciate any other public credit you provide //
//**************************************************************//
#if DOTNETV2
using System;
using System.Collections;
using System.Data;
using Wilson.ORMapper.Query;

namespace Wilson.ORMapper
{
	/// <summary>
	///		ObjectReader uses a Forward-Only Read-Only Live Database Cursor
	///	</summary>
	///	<example>The following example shows how to use the ObjectReader to
	///	retrieve all Contacts.
	///	<code>
	/// <![CDATA[
	///	public static ObjectSpace Manager; // See Initialization Example
	///
	///	// Retrieve ObjectReader Cursor of All Contact Object -- Custom Processing
	///	ObjectReader<Contact> cursor = Manager.GetObjectReader<Contact>(String.Empty);
	///	while (cursor.Read()) {
	///		Contact contact = cursor.Current();
	///	}
	///	cursor.Close();
	/// ]]>
	///	</code>
	///	</example>
	public class ObjectReader<T> : ObjectReader
	{
		internal ObjectReader(Internals.Context context, ObjectQuery<T> objectQuery, bool firstLevel)
			: base(context, objectQuery, firstLevel) {}

		// Jeff Lanning (jefflanning@gmail.com): Added for OPath support.
		internal ObjectReader(Internals.Context context, CompiledQuery<T> query, bool firstLevel, object[] parameters)
			: base(context, query, firstLevel, parameters) {}

		internal ObjectReader(Internals.Context context, SelectProcedure<T> selectProcedure, bool firstLevel)
			: base(context, selectProcedure, firstLevel) {}

		/// <summary>The current object of the ObjectReader</summary>
		/// <returns>The current object instance</returns>
		public new T Current() {
			return (T) base.Current();
		}
	}
}
#endif