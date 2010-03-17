//**************************************************************//
// Paul Wilson -- www.WilsonDotNet.com -- Paul@WilsonDotNet.com //
// Feel free to use and modify -- just leave these credit lines //
// I also always appreciate any other public credit you provide //
//**************************************************************//
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

namespace Wilson.ORMapper
{
	internal class DemoProvider : LicenseProvider
	{
		public override License GetLicense(LicenseContext context,
			Type type, object instance, bool allowExceptions) {
#if (DEMO)
			if (context.UsageMode == LicenseUsageMode.Designtime || Debugger.IsAttached) {
				return new DemoLicense(true);
			}
			else {
				bool runSpecial = false;
				try {
					LocalDataStoreSlot slot = Thread.GetNamedDataSlot("Wilson.ORMapper.AllowDemo");
					string allowDemoKey = Thread.GetData(slot) as string;
					if (allowDemoKey != null) {
						if (allowDemoKey.Equals("WilsonWebPortal")) runSpecial = true;
						else if (allowDemoKey.Equals("WilsonORHelper")) runSpecial = true;
					}
				}
				catch {} // Do Nothing

				bool runMatrix = false;
				if (!runSpecial) {
					try {
						if (Process.GetProcessesByName("WebMatrix").Length > 0) runMatrix = true;
						else if (Process.GetProcessesByName("WebServer").Length > 0) runMatrix = true;
					}
					catch {} // Do Nothing
				}
				if (runSpecial || runMatrix) {
					return new DemoLicense(true);
				}
				else {
					return null;
				}
			}
#else
			return new DemoLicense(false);
#endif
		}

		private class DemoLicense : License
		{
			private bool development;

			public DemoLicense(bool Development) {
				this.development = Development;
			}

			public override string LicenseKey {
				get {	return (this.development ? "Development" : "Production"); }
			}

			public override void Dispose() { }
		}
	}
}
