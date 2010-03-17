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
	internal enum Relationship {
		Child,
		Parent,
		Many
	}

	// <relation relationship="OneToMany|ManyToOne|ManyToMany" member="memberName" field="fieldName"
	//		type="typeName" [alias="aliasName"] [queryOnly="bool"] [lazyLoad="bool"] [cascadeDelete="bool"]
	//		[table="tableName" sourceField="sourceField" destField="destField"] [filter="whereClause"]
	//    [selectSP="selectSPName"] [insertSP="insertSPName" deleteSP="deleteSPName"] />
	internal abstract class RelationMap : AttributeMap
	{
		private string[] fields;
		private Relationship relationship;
		private string type;
		private string alias;
		private bool queryOnly = false;
		private bool lazy = true;
		private bool cascade = false;
		private string filter;
		private string selectSP;

		public string[] Fields {
			get { return this.fields; }
		}

		public Relationship Relationship {
			get { return this.relationship; }
		}

		public string Type {
			get { return this.type; }
		}

		public string Alias {
			get { return this.alias; }
		}

		public bool QueryOnly {
			get { return this.queryOnly; }
		}

		public bool Lazy {
			get { return this.lazy; }
		}

		public bool Cascade {
			get { return this.cascade; }
		}

		public string Filter {
			get { return this.filter; }
		}

		public string SelectSP {
			get { return this.selectSP; }
		}

		internal RelationMap(Relationship relationship, string member, string field, string type, string alias, bool queryOnly,
			bool lazy, bool cascade, string filter, string selectSP, CustomProvider provider) : base(member, field)
		{
			if (type == null || type.Length == 0) {
				throw new MappingException("Mapping: Relation type was Missing");
			}
			if (alias == null || alias.Length == 0) { // Should never happend since alias defaults to member
				throw new MappingException("Mapping: Relation alias was Missing");
			}

			this.fields = field.Replace(", ", ",").Split(',');
			this.relationship = relationship;
			this.type = type;
			this.alias = alias;
			this.queryOnly = queryOnly;
			this.lazy = lazy;
			this.cascade = cascade;
			this.filter = filter;
			this.selectSP = selectSP;
			if (selectSP != null && selectSP.Length > 0) {
				for (int index = 0; index < this.fields.Length; index++) {
					this.fields[index] = provider.GetParameterDefault(this.fields[index]);
				}
			}
		}
	}
}
