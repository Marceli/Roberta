//**************************************************************//
// Paul Wilson -- www.WilsonDotNet.com -- Paul@WilsonDotNet.com //
// Feel free to use and modify -- just leave these credit lines //
// I also always appreciate any other public credit you provide //
//**************************************************************//
// CascadeDelete help from Ken Muse (http://www.MomentsFromImpact.com)
// Includes Support for Composite Primary Keys from Jerry Shea
// SelectSP advice and code from Allan Ritchie (A.Ritchie@ACRWebSolutions.com)
// Includes Support for Member Properties from Chris Schletter (http://www.thzero.com)
// Includes Composite Auto key support from Marc Brooks (http://musingmarc.blogspot.com)
using System;
using System.Collections;
using System.Reflection;

namespace Wilson.ORMapper.Internals
{
	// <entity type="typeName" table="tableName" keyMember="memberName[,...]"
	//   [keyType="Auto|Guid|User|Composite|None"] [sortOrder="sortClause"]
	//   [readOnly="bool"] [changesOnly="bool"] [autoTrack="bool"]
	//   [typeField="typeDiscriminatorField"] [typeValue="typeDiscriminatorValue"]
	//   [insertSP="insertSPName" updateSP="updateSPName" deleteSP="deleteSPName"] />
	internal class EntityMap
	{
		private string type;
		private string table;
		private string[] keyMembers;
		private KeyType keyType = KeyType.Auto;
		private string sortOrder;
		private bool readOnly = false;
		private bool changesOnly = false;
		private bool autoTrack = true;

		private EntityMap baseEntity = null;
		private string typeField = null;
		private object typeValue = null;
		private Hashtable subTypes = new Hashtable();
		private Hashtable subFields = new Hashtable();

		private string insertSP;
		private string updateSP;
		private string deleteSP;
		private bool hasHelper = false;
		private bool hasEvents = false;
		private int readOnlyCount = 0;
		private int concurrentCount = 0;
		private int childRelations = 0;

		private FieldMap[] fields = new FieldMap[0];
		private FieldMap[] keyFields = new FieldMap[0];
		private RelationMap[] relations = new RelationMap[0];
		private Hashtable members = new Hashtable();
		private Hashtable helper = new Hashtable(); // Track alias -> field index

		private static Hashtable typesByName = Hashtable.Synchronized(new Hashtable(50));

		public string Type {
			get { return this.type; }
		}

		public string Table {
			get { return this.table; }
		}

		internal bool IsKeyMember(string member) {
			for (int index = 0; index < this.keyFields.Length; index++) {
				if (this.keyFields[index].Member.Equals(member)) return true;
			}
			return false;
		}

		// Marc Brooks (IDisposable@gmail.com): handling multiple-segment Auto keys
		internal bool IsAutoKeyMember(string member) {
			return this.AutoKeyMember().Member.Equals(member);
		}

		internal bool IsAutoKeyMember(int memberIndex) {
			return memberIndex == this.AutoKeyMemberIndex();
		}

		internal int AutoKeyMemberIndex() {
			return this.keyFields.Length - 1;
		}

		internal FieldMap AutoKeyMember() {
			return this.keyFields[this.AutoKeyMemberIndex()];
		}

		// Jeff Lanning (jefflanning@gmail.com): Added to exposed member var for OPath support.
		public FieldMap[] Fields {
			get { return this.fields; }
		}

		public FieldMap[] KeyFields {
			get { return this.keyFields; }
		}

		public KeyType KeyType {
			get { return this.keyType; }
		}

		public string SortOrder {
			get { return this.sortOrder; }
		}

		public bool ReadOnly {
			get { return this.readOnly; }
		}

		public bool ChangesOnly {
			get { return this.changesOnly; }
		}

		public bool AutoTrack {
			get { return this.autoTrack; }
		}

		public EntityMap BaseEntity {
			get { return this.baseEntity; }
		}

		public string TypeField {
			get { return this.typeField; }
		}

		public object TypeValue {
			get { return this.typeValue; }
		}

		public Hashtable SubTypes {
			get { return this.subTypes; }
		}

		public Hashtable SubFields {
			get { return this.subFields; }
		}

		public string InsertSP {
			get { return this.insertSP; }
		}

		public string UpdateSP {
			get { return this.updateSP; }
		}

		public string DeleteSP {
			get { return this.deleteSP; }
		}

		public bool HasHelper {
			get { return this.hasHelper; }
		}

		public bool HasEvents {
			get { return this.hasEvents; }
		}

		public FieldMap GetFieldMap(string aliasName) {
			object index = this.helper[aliasName];
			// Jeff Lanning (jefflanning@gmail.com): Added null check to provide useful error messages (internal callers are not checking for null).
			if (index == null)	{
				throw new Exception("Type '" + this.type + "' does not have a property named '" + aliasName + "'.");
			}
			return this[(int)index];
		}

		public FieldMap this[int index] {
			get { return this.fields[index]; }
		}

		public RelationMap Relation(int index) {
			return this.relations[index];
		}

		// Jeff Lanning (jefflanning@gmail.com): Added for OPath support.
		public RelationMap Relation(string alias) {
			if (alias == null) throw new ArgumentNullException("alias");
			for (int i = 0; i < relations.Length; i++) {
				if (relations[i].Alias == alias) {
					return relations[i];
				}
			}
			return null;
		}

		public MemberInfo Member(string name) {
			return (MemberInfo) this.members[name];
		}

		public int FieldCount {
			get { return this.fields.Length; }
		}

		public int RelationCount {
			get { return this.relations.Length; }
		}

		public int ChildRelations {
			get { return this.childRelations; }
		}

		public int ReadOnlyCount {
			get { return this.readOnlyCount; }
		}

		public int ConcurrentCount {
			get { return this.concurrentCount; }
		}

		internal EntityMap(string type, string table, string keyMember, KeyType keyType, string sortOrder,
				bool readOnly, bool changesOnly, bool autoTrack, string typeField, string typeValue)
		{
			if (type == null || type.Length == 0) {
				throw new MappingException("Mapping: Entity type was Missing");
			}
			if (table == null || table.Length == 0) {
				throw new MappingException("Mapping: Entity table was Missing - " + type);
			}
			if (keyType != KeyType.None && (keyMember == null || keyMember.Length == 0)) {
				throw new MappingException("Mapping: Entity keyMember was Missing - " + type);
			}
			this.type = type;
			this.table = table;
			this.keyMembers = keyMember.Replace(", ", ",").Split(',');
			this.keyType = keyType;
			this.sortOrder = (sortOrder == null ? String.Empty : sortOrder);
			this.readOnly = readOnly;
			this.changesOnly = changesOnly;
			this.autoTrack = autoTrack;

			this.baseEntity = null;
			this.typeField = typeField;
			try {
				if (typeValue == null) {
					this.typeValue = null;
				}
				else {
					this.typeValue = int.Parse(typeValue);
				}
			}
			catch {
				this.typeValue = typeValue.Trim('\'');
			}
			
			if (EntityMap.GetType(this.type) == null) {
				throw new MappingException("Mapping: Entity type was Invalid - " + type);
			}
			if (EntityMap.GetType(this.type).GetInterface(typeof(IObjectHelper).ToString()) != null) {
				this.hasHelper = true;
			}
			if (EntityMap.GetType(this.type).GetInterface(typeof(IObjectNotification).ToString()) != null) {
				this.hasEvents = true;
			}
		}

		internal EntityMap(string type, EntityMap baseEntity, string typeValue) {
			if (type == null || type.Length == 0) {
				throw new MappingException("Mapping: SubEntity type was Missing");
			}
			if (baseEntity == null) {
				throw new MappingException("Mapping: SubEntity inherits was undefined Entity - " + type);
			}
			if (baseEntity.TypeField == null || baseEntity.TypeField.Length == 0) {
				throw new MappingException("Mapping: SubEntity entity had undefined typeField - " + type);
			}
			if (typeValue == null || typeValue.Length == 0) {
				throw new MappingException("Mapping: SubEntity typeValue was not defined - " + type);
			}
			
			this.type = type;
			this.table = baseEntity.table;
			this.keyMembers = baseEntity.keyMembers.Clone() as string[];
			this.keyType = baseEntity.keyType;
			this.sortOrder = baseEntity.sortOrder;
			this.readOnly = baseEntity.readOnly;
			this.changesOnly = baseEntity.changesOnly;
			this.autoTrack = baseEntity.autoTrack;
			
			this.baseEntity = baseEntity;
			this.typeField = baseEntity.typeField;
			try {
				if (typeValue == null) {
					this.typeValue = null;
				}
				else {
					this.typeValue = int.Parse(typeValue);
				}
			}
			catch {
				this.typeValue = typeValue.Trim('\'');
			}
			baseEntity.AddSubType(this.typeValue.ToString(), type);

			this.insertSP = baseEntity.insertSP;
			this.updateSP = baseEntity.updateSP;
			this.deleteSP = baseEntity.deleteSP;
			this.readOnlyCount = baseEntity.readOnlyCount;
			this.concurrentCount = baseEntity.concurrentCount;
			
			this.fields = baseEntity.fields.Clone() as FieldMap[];
			this.keyFields = baseEntity.keyFields.Clone() as FieldMap[];
			this.relations = baseEntity.relations.Clone() as RelationMap[];
			this.members = baseEntity.members.Clone() as Hashtable;
			this.helper = baseEntity.helper.Clone() as Hashtable;

			if (EntityMap.GetType(this.type) == null) {
				throw new MappingException("Mapping: Entity type was Invalid - " + type);
			}
			if (EntityMap.GetType(this.type).GetInterface(typeof(IObjectHelper).ToString()) != null) {
				this.hasHelper = true;
			}
			if (EntityMap.GetType(this.type).GetInterface(typeof(IObjectNotification).ToString()) != null) {
				this.hasEvents = true;
			}
		}

		internal void AddSubType(string typeValue, string typeName) {
			this.subTypes.Add(typeValue, typeName);
		}

		// Includes Null-Value Assistance from Tim Byng (http://www.missioninc.com)
		internal void AddField(string member, string field, string nullValue, string alias,
				string parameter, PersistType persistType, CustomProvider provider) {
			this.AddMember(member);
			FieldMap[] tempFields = new FieldMap[this.fields.Length + 1];
			this.fields.CopyTo(tempFields, 0);
			tempFields[this.fields.Length] = new FieldMap(member, field, nullValue,
				parameter, persistType, EntityMap.GetType(this.Member(member)), provider);
			int keyIndex = -1;
			for (int index = 0; index < this.keyMembers.Length; index++) {
				if (this.keyMembers[index].Equals(member)) keyIndex = index;
			}
			if (keyIndex > -1) {
				FieldMap[] tempKeyFields = new FieldMap[this.keyFields.Length + 1];
				this.keyFields.CopyTo(tempKeyFields, 0);
				tempKeyFields[this.keyFields.Length] = tempFields[this.fields.Length];
				this.keyFields = tempKeyFields;
			}
			if (persistType == PersistType.ReadOnly) this.readOnlyCount++;
			if (persistType == PersistType.Concurrent) this.concurrentCount++;
			this.fields = tempFields;
			this.helper.Add(alias, this.fields.Length - 1);

			if (this.baseEntity != null && !this.baseEntity.subFields.ContainsKey(field)) {
				this.baseEntity.subFields.Add(field, this.fields[this.fields.Length - 1]);
			}
		}

		internal void AddLookup(string member, string field, string nullValue, string alias,
				string parameter, string table, string source, string dest, CustomProvider provider) {
			this.AddMember(member);
			FieldMap[] tempFields = new FieldMap[this.fields.Length + 1];
			this.fields.CopyTo(tempFields, 0);
			tempFields[this.fields.Length] = new LookupMap(member, field, nullValue,
				parameter, table, source, dest, EntityMap.GetType(this.Member(member)), provider);
			this.readOnlyCount++;
			this.fields = tempFields;
			this.helper.Add(alias, this.fields.Length - 1);

			if (this.baseEntity != null && !this.baseEntity.subFields.ContainsKey(field)) {
				this.baseEntity.subFields.Add(field, this.fields[this.fields.Length - 1]);
			}
		}

		internal void AddChild(string member, string field, string type, string alias, bool queryOnly, bool lazy, bool cascade, string filter, string selectSP, CustomProvider provider) {
			this.AddRelation(member, new ChildMap(member, field, type, alias, queryOnly, lazy, cascade, filter, selectSP, provider));
			this.childRelations++;
		}

		internal void AddParent(string member, string field, string type, string alias, bool queryOnly, bool lazy, bool cascade, string filter, string selectSP, CustomProvider provider) {
			this.AddRelation(member, new ParentMap(member, field, type, alias, queryOnly, lazy, cascade, filter, selectSP, provider));
		}

		internal void AddMany(string member, string field, string type, string alias, string table, string parent, string child,
				bool queryOnly, bool lazy, bool cascade, string filter, string selectSP, string insertSP, string deleteSP, CustomProvider provider) {
					this.AddRelation(member, new ManyMap(member, field, type, alias, table, parent, child, queryOnly, lazy, cascade, filter, selectSP, insertSP, deleteSP, provider));
		}

		private void AddRelation(string member, RelationMap relation) {
			if (EntityMap.GetType(relation.Type) == null) {
				throw new MappingException("Mapping: Relation type was Invalid - " + relation.Type);
			}

			RelationMap[] tempRelations = new RelationMap[this.relations.Length + 1];
			this.relations.CopyTo(tempRelations, 0);
			tempRelations[this.relations.Length] = relation;
			this.relations = tempRelations;
			if (!relation.QueryOnly) this.AddMember(member);
		}

		// Includes Mapping Inheritance from Jerry Shea (http://www.RenewTek.com)
		// Includes Embedded Objects from Paul Hatcher (http://www.grassoc.co.uk)
		private void AddMember(string member) {
			MemberInfo field = null;
			if (member == null || member.Length == 0) {
				throw new MappingException("Mapping: Entity member was Missing - " + this.type);
			}

			// Improved Support for Embedded Objects from Chris Schletter (http://www.thzero.com)
			string typeName = this.type;
			string[] memberParts = member.Split('.');
			for (int index = 0; index < memberParts.Length; index++) {
				field = FindField(typeName, memberParts[index]);
				if (field != null) {
					typeName = EntityMap.GetType(field).FullName;
				}
				else {
					break;
				}
			}
			
			if (field == null) {
				throw new MappingException("Mapping: Entity member was Invalid - " + member + " : " + type);
			}
			this.members.Add(member, field);
		}

		// Includes Embedded Objects from Paul Hatcher (http://www.grassoc.co.uk)
		internal static MemberInfo FindField(string typeName, string member) {
			BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
			MemberInfo field = null;
			Type type = EntityMap.GetType(typeName);
			while (type != null && field == null) {
			  // if we can't find field in type, then try all of type's ancestors
				field = type.GetField(member, flags);
				type = type.BaseType;
			}

			// Improved Support for Embedded Objects from Chris Schletter (http://www.thzero.com)
			if (field == null) {
				type = EntityMap.GetType(typeName);
				while (type != null && field == null) {
					// if we can't find property in type, then try all of type's ancestors
					field = type.GetProperty(member, flags);
					type = type.BaseType;
				}
			}
			return field;
		}

		internal void AddSProcs(string insertSP, string updateSP, string deleteSP) {
			this.insertSP = (insertSP == null ? String.Empty : insertSP);
			this.updateSP = (updateSP == null ? String.Empty : updateSP);
			this.deleteSP = (deleteSP == null ? String.Empty : deleteSP);
		}

		internal static Type GetType(MemberInfo member) {
			if (member is FieldInfo) {
				return (member as FieldInfo).FieldType;
			}
			else {
				return (member as PropertyInfo).PropertyType;
			}
		}

		// Jeff Lanning (jefflanning@gmail.com): Performance improvement by caching resolved types.
		internal static Type GetType(string typeName) {
			Type type = (Type) typesByName[typeName];
			if (type == null) {
				foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
					type = assembly.GetType(typeName);
					if (type != null) break;
				}
				if (type == null) {
					throw new ORMapperException("ORMapper: Type could not be located in any loaded assembly - " + typeName);
				}
				typesByName[typeName] = type;
			}
			return type;
		}
	}
}
