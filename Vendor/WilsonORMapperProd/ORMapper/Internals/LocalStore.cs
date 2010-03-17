//**************************************************************//
// Paul Wilson -- www.WilsonDotNet.com -- Paul@WilsonDotNet.com //
// Feel free to use and modify -- just leave these credit lines //
// I also always appreciate any other public credit you provide //
//**************************************************************//
using System;
using System.Collections;
using System.Threading;

namespace Wilson.ORMapper.Internals
{
	// Avoid Infinite Relationship Loop for non-Lazy-Loaded Collections
	sealed internal class LocalStore
	{
		// See http://blogs.msdn.com/junfeng/archive/2005/12/31/508423.aspx
		[ThreadStatic()]
		static private int level;
		[ThreadStatic()]
		static private Hashtable types;

		// Each External Method Should call Reset and No Internal Method Should
		static public void Reset(Type type) {
			LocalStore.Level = 0;
			LocalStore.Types.Clear();
			LocalStore.Types.Add(type.ToString(), true);
		}

		static public int Level {
			get { return LocalStore.level; }
			set { LocalStore.level = value; }
		}

		// Only Necessary to Check IsLoaded for non-Lazy-Loaded Collections
		static public bool IsLoaded(Type type) {
			string typeName = type.ToString();
			if (LocalStore.Types.Contains(typeName)) {
				return true;
			}
			else {
				LocalStore.Types.Add(typeName, true);
				return false;
			}
		}

		// Use Thread Local Storage for Tracking Loaded Types for Each Thread
		static private Hashtable Types {
			get {
				if (LocalStore.types == null) {
					LocalStore.types = new Hashtable();
				}
				return LocalStore.types;
			}
		}

		private LocalStore() {}
	}
}
