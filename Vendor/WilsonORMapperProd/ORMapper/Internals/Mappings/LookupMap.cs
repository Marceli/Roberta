//**************************************************************//
// Paul Wilson -- www.WilsonDotNet.com -- Paul@WilsonDotNet.com //
// Feel free to use and modify -- just leave these credit lines //
// I also always appreciate any other public credit you provide //
//**************************************************************//
using System;
using System.Collections;

namespace Wilson.ORMapper.Internals
{
	// <lookup member="memberName" field="fieldName" [nullValue="nullValue"] [alias="propertyName"]
	//   table="lookupTable" foreignKey="foreignKey" lookupKey="lookupKey" [parameter="parameterName"] />
	internal class LookupMap : FieldMap
	{
		private string table;
		private string[] source;
		private string[] dest;
		internal string tableAlias;
		
		public string Table {
			get { return this.table; }
		}

		public string[] Source {
			get { return this.source; }
		}

		public string[] Dest {
			get { return this.dest; }
		}

		public override bool IsLookup {
			get { return true; }
		}

		public string TableAlias {
			get { return this.tableAlias; }
		}

		public override string FieldAlias {
			get { return this.TableAlias + "_" + this.Field; }
		}

		internal LookupMap(string member, string field, string nullValue, string parameter, string table, string source, string dest, Type memberType, CustomProvider provider)
			: base(member, field, nullValue, parameter, PersistType.ReadOnly, memberType, provider)
		{
			if (table == null || table.Length == 0) {
				throw new MappingException("Mapping: Lookup table was Missing - " + member);
			}
			if (source == null || source.Length == 0) {
				throw new MappingException("Mapping: Lookup foreignKey was Missing - " + member);
			}
			if (dest == null || dest.Length == 0) {
				throw new MappingException("Mapping: Lookup lookupKey was Missing - " + member);
			}
			this.table = table;
			this.source = source.Replace(", ", ",").Split(',');
			this.dest = dest.Replace(", ", ",").Split(',');
		}
	}
}
