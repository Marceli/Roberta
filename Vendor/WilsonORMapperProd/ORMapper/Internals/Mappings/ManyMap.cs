//**************************************************************//
// Paul Wilson -- www.WilsonDotNet.com -- Paul@WilsonDotNet.com //
// Feel free to use and modify -- just leave these credit lines //
// I also always appreciate any other public credit you provide //
//**************************************************************//
// CascadeDelete help from Ken Muse (http://www.MomentsFromImpact.com)
// SelectSP advice and code from Allan Ritchie (A.Ritchie@ACRWebSolutions.com)
// Help on Composite Relations from Nick Franceschina (http://www.simpulse.net)
using System;

namespace Wilson.ORMapper.Internals
{
	// <relation relationship="ManyToMany" member="memberName" field="fieldName"
	//		type="typeName" [alias="aliasName"] [queryOnly="bool"] [lazyLoad="bool"] [cascadeDelete="bool"]
	//		table="tableName" sourceField="sourceField" destField="destField" [filter="whereClause"]
	//    [selectSP="selectSPName" insertSP="insertSPName" deleteSP="deleteSPName"] />
	// Note: selectSP Parameters - field
	//    insertSP Parameters - source, dest
	//    deleteSP Parameters - source
	//    cascadeDelete does not work with stored procs
	internal class ManyMap : RelationMap
	{
		private string table;
		private string[] source;
		private string[] dest;
		private string insertSP;
		private string deleteSP;

		public string Table {
			get { return this.table; }
		}

		public string[] Source {
			get { return this.source; }
		}

		public string[] Dest {
			get { return this.dest; }
		}

		public string InsertSP {
			get { return this.insertSP; }
		}

		public string DeleteSP {
			get { return this.deleteSP; }
		}

		internal ManyMap(string member, string field, string type, string alias, string table, string source, string dest,
			bool queryOnly, bool lazy, bool cascade, string filter, string selectSP, string insertSP, string deleteSP, CustomProvider provider)
			: base(Relationship.Many, member, field, type, alias, queryOnly, lazy, cascade, filter, selectSP, provider)
		{
			if (table == null || table.Length == 0) {
				throw new MappingException("Mapping: Relation table was Missing - " + member);
			}
			if (source == null || source.Length == 0) {
				throw new MappingException("Mapping: Relation sourceField was Missing - " + member);
			}
			if (dest == null || dest.Length == 0) {
				throw new MappingException("Mapping: Relation destField was Missing - " + member);
			}
			this.table = table;
			this.source = source.Replace(", ", ",").Split(',');
			this.dest = dest.Replace(", ", ",").Split(',');
			this.insertSP = insertSP;
			this.deleteSP = deleteSP;
			if (insertSP != null && insertSP.Length > 0 && deleteSP != null && deleteSP.Length > 0) {
				for (int index = 0; index < System.Math.Min(this.source.Length, this.dest.Length); index++) {
					this.source[index] = provider.GetParameterDefault(this.source[index]);
					this.dest[index] = provider.GetParameterDefault(this.dest[index]);
				}
			}
		}
	}
}
