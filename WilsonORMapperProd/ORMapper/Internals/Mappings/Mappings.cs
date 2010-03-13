//**************************************************************//
// Paul Wilson -- www.WilsonDotNet.com -- Paul@WilsonDotNet.com //
// Feel free to use and modify -- just leave these credit lines //
// I also always appreciate any other public credit you provide //
//**************************************************************//
// Includes support for Composite Primary Keys from Jerry Shea
// SelectSP advice and code from Allan Ritchie (A.Ritchie@ACRWebSolutions.com)
// Includes support for Multiple Mapping Files or Streams with <file> Element
// Includes support for Default Namespace from Jeff Lanning (jefflanning@gmail.com)
// Includes Default Namespace mods from Marc Brooks (http://musingmarc.blogspot.com)
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Wilson.ORMapper.Internals {
	internal class Mappings {
		private Hashtable entities = new Hashtable();
		private Hashtable commands = new Hashtable();
		private Hashtable helper = new Hashtable(); // Track short entityName -> full entityType
		internal CustomProvider provider;
		private string version;

		public EntityMap this[Type type] {
			get { return this[type.ToString()]; }
		}

		// Includes Mapping Inheritance from Jerry Shea (http://www.RenewTek.com)
		public EntityMap this[string type] {
			get {
				EntityMap entity = this.entities[type] as EntityMap;
				if (entity == null) {
					// if no entity map found then try to find one for ancestor type
					for (Type baseType = EntityMap.GetType(type).BaseType; baseType != null; baseType = baseType.BaseType) {
						entity = this.entities[baseType.ToString()] as EntityMap;
						if (entity != null) {
							this.entities[type] = entity; // if found, put into entity map so we don't have to do this again
							break;
						}
					}
				}
				return entity;
			}
		}

		public Commands Commands(Type type) {
			return this.Commands(type.ToString());
		}

		// Includes Mapping Inheritance by Jim Daugherty and Michael Robin
		public Commands Commands(string type) {
			Commands commands = this.commands[type] as Commands;
			if (commands == null) {
				// if no commands found then try to find one for ancestor type
				for (Type baseType = EntityMap.GetType(type).BaseType; baseType != null; baseType = baseType.BaseType) {
					commands = this.commands[baseType.ToString()] as Commands;
					if (commands != null) {
						this.commands[type] = commands; // if found, put into commands so we don't have to do this again
						break;
					}
				}
			}
			return commands;
		}

		public Commands GetCommands(string entityName) {
			string entityType;
			if (entityName.IndexOf(".") == -1) {
				entityType = (string)this.helper[entityName];
				if (entityType == null) {
					throw new MappingException("Mappings: Multiple Entities with Short Type Name - " + entityName);
				}
			}
			else {
				entityType = entityName;
			}
			return this.Commands(entityType);
		}

		public ICollection Types {
			get { return this.entities.Keys; }
		}

		// Embedded Resource Mappings from Allan Ritchie (A.Ritchie@ACRWebSolutions.com)
		internal Mappings(XmlDocument xmlMappings, CustomProvider customProvider) {
			this.provider = customProvider;

#if DEBUG_MAPPER
			Debug.WriteLine("========== Mappings Start Here ===========");
#endif

			this.ParseMappings(xmlMappings, null);

#if DEBUG_MAPPER
			Debug.WriteLine("========== Mappings End Here ===========");
#endif
		}

		// <mappings [version="4.2"] [defaultNamespace="Namespace"] />
		private void ParseMappings(XmlDocument mappings, string outerDefaultNamespace) {
			XmlNode root = mappings.SelectSingleNode("mappings");
			if (root == null) {
				throw new MappingException("Mappings: Root mappings is Missing");
			}

			this.version = this.GetValue(root, "version", "4.2");
			if (this.version != "2.0" && this.version != "2.1" && this.version != "2.2"
				&& this.version != "3.0" && this.version != "3.1" && this.version != "3.2"
				&& this.version != "4.0" && this.version != "4.1" && this.version != "4.2") {
				throw new MappingException("Mappings: Version is not compatible");
			}

			string defaultNamespace = this.GetValue(root, "defaultNamespace", null);
			if (defaultNamespace == null || defaultNamespace.Trim().Length == 0) {
				defaultNamespace = outerDefaultNamespace;
			}

			foreach (XmlNode entityNode in root.SelectNodes("entity")) {
				this.ParseEntity(entityNode, defaultNamespace);
			}
			foreach (XmlNode subEntityNode in root.SelectNodes("subEntity")) {
				this.ParseSubEntity(subEntityNode, defaultNamespace);
			}
			foreach (XmlNode subFileNode in root.SelectNodes("file")) {
				this.ParseFile(subFileNode, defaultNamespace);
			}
		}

		// <file path="file|path|resource" [embedded="bool"] />
		private void ParseFile(XmlNode fileNode, string defaultNamespace) {
			string path = this.GetValue(fileNode, "path");
			bool embedded; // Default embedded is False
			switch (this.GetValue(fileNode, "embedded", "FALSE").ToUpper()) {
				case "TRUE": embedded = true; break;
				default: embedded = false; break;
			}

			XmlDocument mappings = new XmlDocument();
			if (!embedded) {
				// Try to Automatically Resolve Path of Mapping File
				string fullPath = Mappings.GetFullPath(path);
				mappings.Load(fullPath);
			}
			else {
				// Try to Automatically Load Embedded Mapping File
				using (Stream stream = Mappings.GetResourceStream(path)) {
					mappings.Load(stream);
				}
			}
#if DEBUG_MAPPER
			Debug.WriteLine("----- File = " + path + " -----");
#endif
			this.ParseMappings(mappings, defaultNamespace);
		}

		private void ParseEntity(XmlNode entityNode, string defaultNamespace) {
			string type = this.GetNamespacedValue(entityNode, "type", defaultNamespace);
			string table = this.GetValue(entityNode, "table");
			string keyMember = this.GetValue(entityNode, "keyMember");
			KeyType keyType; // Default keyType is Auto
			switch (this.GetValue(entityNode, "keyType", "AUTO").ToUpper()) {
				case "GUID": keyType = KeyType.Guid; break;
				case "USER": keyType = KeyType.User; break;
				case "COMPOSITE": keyType = KeyType.Composite; break;
				case "NONE": keyType = KeyType.None; break;
				default: keyType = KeyType.Auto; break;
			}
			string sortOrder = this.GetValue(entityNode, "sortOrder");

			// Paul Welter (http://www.LoreSoft.com) -- Removed special case for None keyType
			bool readOnly = true; // Default readOnly is False
			switch (this.GetValue(entityNode, "readOnly", "FALSE").ToUpper()) {
				case "TRUE": readOnly = true; break;
				default: readOnly = false; break;
			}

			bool changesOnly; // Default changesOnly is False
			switch (this.GetValue(entityNode, "changesOnly", "FALSE").ToUpper()) {
				case "TRUE": changesOnly = true; break;
				default: changesOnly = false; break;
			}

			bool autoTrack; // Default autoTrack is True
			switch (this.GetValue(entityNode, "autoTrack", "TRUE").ToUpper()) {
				case "FALSE": autoTrack = false; break;
				default: autoTrack = true; break;
			}

			string typeField = this.GetValue(entityNode, "typeField", null);
			string typeValue = this.GetValue(entityNode, "typeValue", null);

			EntityMap entity = new EntityMap(
				type, table, keyMember, keyType, sortOrder, readOnly, changesOnly, autoTrack, typeField, typeValue);
			// Stored Procedures are Optional but Supported
			string insertSP = this.GetValue(entityNode, "insertSP");
			string updateSP = this.GetValue(entityNode, "updateSP");
			string deleteSP = this.GetValue(entityNode, "deleteSP");
			entity.AddSProcs(insertSP, updateSP, deleteSP);
			foreach (XmlNode attributeNode in entityNode.SelectNodes("attribute")) {
				this.ParseAttribute(entity, attributeNode);
			}
			foreach (XmlNode attributeNode in entityNode.SelectNodes("lookup")) {
				this.ParseLookup(entity, attributeNode);
			}
			foreach (XmlNode attributeNode in entityNode.SelectNodes("relation")) {
				this.ParseRelation(entity, attributeNode, defaultNamespace);
			}
#if DEBUG_MAPPER
			Debug.WriteLine("Entity = " + type + " : "
				+ entity.FieldCount.ToString() + " Fields, "
				+ entity.RelationCount.ToString() + " Relations");
#endif
			if (keyType != KeyType.None) {
				// Jeff Lanning (jefflanning@gmail.com): Added length check to prevent "Index Out Of Bounds" exception later during execution (which is very hard to debug).
				if (entity.KeyFields.Length != keyMember.Split(',').Length) {
					throw new MappingException("Number of key fields specified for entity '" + type + "' does not matched the number found.");
				}
				try {
					string keyField;
					foreach (FieldMap f in entity.KeyFields)
						keyField = f.Field;
				}
				catch (Exception exception) {
					throw new MappingException("Mapping: Entity keyMember missing from Attribute members - " + type, exception);
				}
			}
			this.entities.Add(type, entity);
			this.commands.Add(type, ProviderFactory.GetCommands(entity, this.provider));
			string typeName = type.Substring(type.LastIndexOf(".") + 1);
			if (!this.helper.ContainsKey(typeName)) {
				this.helper.Add(typeName, type);
			}
			else {
				this.helper[typeName] = null;
			}
		}

		// <subEntity type="typeName" inherits="baseTypeName" typeValue="typeDiscriminatorValue" />
		private void ParseSubEntity(XmlNode subEntityNode, string defaultNamespace) {
			string type = this.GetNamespacedValue(subEntityNode, "type", defaultNamespace);
			string inherits = this.GetNamespacedValue(subEntityNode, "inherits", defaultNamespace);
			string typeValue = this.GetValue(subEntityNode, "typeValue");
			EntityMap subEntity = new EntityMap(type, this[inherits], typeValue);

			foreach (XmlNode attributeNode in subEntityNode.SelectNodes("attribute")) {
				this.ParseAttribute(subEntity, attributeNode);
			}
			foreach (XmlNode attributeNode in subEntityNode.SelectNodes("lookup")) {
				this.ParseLookup(subEntity, attributeNode);
			}
			foreach (XmlNode attributeNode in subEntityNode.SelectNodes("relation")) {
				this.ParseRelation(subEntity, attributeNode, defaultNamespace);
			}
#if DEBUG_MAPPER
			Debug.WriteLine("SubEntity = " + type + " : "
				+ subEntity.FieldCount.ToString() + " Fields, "
				+ subEntity.RelationCount.ToString() + " Relations");
#endif
			this.entities.Add(type, subEntity);
			this.commands.Add(type, ProviderFactory.GetCommands(subEntity, this.provider));

#if DEBUG_MAPPER
			Debug.WriteLine("SubEntity Inherits = " + inherits + " : "
				+ subEntity.BaseEntity.FieldCount.ToString() + " Fields, "
				+ subEntity.BaseEntity.RelationCount.ToString() + " Relations");
#endif
			this.commands[inherits] = ProviderFactory.GetCommands(subEntity.BaseEntity, this.provider);

			string typeName = type.Substring(type.LastIndexOf(".") + 1);
			if (!this.helper.ContainsKey(typeName)) {
				this.helper.Add(typeName, type);
			}
			else {
				this.helper[typeName] = null;
			}
		}

		// Includes Null-Value Assistance from Tim Byng (http://www.missioninc.com)
		private void ParseAttribute(EntityMap entity, XmlNode attributeNode) {
			string member = this.GetValue(attributeNode, "member");
			string field = this.GetValue(attributeNode, "field");
			// Null Values use nullValue if provided
			string nullValue = this.GetValue(attributeNode, "nullValue", null);
			string alias = this.GetValue(attributeNode, "alias", member);
			// Stored Procedures use parameter if Provided
			string parameter = this.GetValue(attributeNode, "parameter");

			PersistType persistType; // Default persistType is Persist
			switch (this.GetValue(attributeNode, "persistType", "PERSIST").ToUpper()) {
				case "READONLY": persistType = PersistType.ReadOnly; break;
				case "CONCURRENT": persistType = PersistType.Concurrent; break;
				default: persistType = PersistType.Persist; break;
			}
			entity.AddField(member, field, nullValue, alias, parameter, persistType, this.provider);
		}

		private void ParseLookup(EntityMap entity, XmlNode attributeNode) {
			string member = this.GetValue(attributeNode, "member");
			string field = this.GetValue(attributeNode, "field");
			string nullValue = this.GetValue(attributeNode, "nullValue", null);
			string alias = this.GetValue(attributeNode, "alias", member);
			string table = this.GetValue(attributeNode, "table");
			string source = this.GetValue(attributeNode, "foreignKey");
			string dest = this.GetValue(attributeNode, "lookupKey");
			string parameter = this.GetValue(attributeNode, "parameter");

			entity.AddLookup(member, field, nullValue, alias, parameter, table, source, dest, this.provider);
		}

		private void ParseRelation(EntityMap entity, XmlNode attributeNode, string defaultNamespace) {
			Relationship relationship;
			switch (this.GetValue(attributeNode, "relationship").ToUpper()) {
				case "ONETOMANY": relationship = Relationship.Child; break;
				case "MANYTOONE": relationship = Relationship.Parent; break;
				case "MANYTOMANY": relationship = Relationship.Many; break;
				default: throw new MappingException("Mapping: Relation relationship is Invalid");
			}

			string member = this.GetValue(attributeNode, "member");
			string field = this.GetValue(attributeNode, "field");
			string type = this.GetNamespacedValue(attributeNode, "type", defaultNamespace);
			// Jeff Lanning (jefflanning@gmail.com): Added optional "alias" attribute for use in OPath queries (and to help code generators build entity classes)
			string alias = this.GetValue(attributeNode, "alias", member);

			// optional queryOnly is used to enable OPath on relationships without actually loading any relationships -- no member allowed, alias is required
			bool queryOnly; // Default queryOnly is False
			switch (this.GetValue(attributeNode, "queryOnly", "FALSE").ToUpper()) {
				case "TRUE": queryOnly = true; break;
				default: queryOnly = false; break;
			}
			if (!queryOnly) {
				if (member == null || member.Length == 0) {
					throw new MappingException("Mapping: Relation attribute 'member' must be specified when 'queryOnly' is set to false - " + entity.Type + " : " + member);
				}
			}
			else {
				if (member != null && member.Length > 0) {
					throw new MappingException("Mapping: Relation attribute 'member' is not allowed when 'queryOnly' is set to true - " + entity.Type + " : " + member);
				}
				if (alias == null || alias.Length == 0) {
					throw new MappingException("Mapping: Relation attribute 'alias' must be specified when queryOnly is set to true - " + entity.Type + " : " + type);
				}
				member = alias; // Required internally for hashtables, but would be inconsistent if allowed externally
			}

			bool lazyLoad; // Default lazyLoad is True
			switch (this.GetValue(attributeNode, "lazyLoad", "TRUE").ToUpper()) {
				case "FALSE": lazyLoad = false; break;
				default: lazyLoad = true; break;
			}

			// CascadeDelete help from Ken Muse (http://www.MomentsFromImpact.com)
			bool cascadeDelete; // Default cascaseDelete is False
			switch (this.GetValue(attributeNode, "cascadeDelete", "FALSE").ToUpper()) {
				case "TRUE": cascadeDelete = true; break;
				default: cascadeDelete = false; break;
			}

			string filter = this.GetValue(attributeNode, "filter");
			string selectSP = this.GetValue(attributeNode, "selectSP");

			if (relationship == Relationship.Child) {
				entity.AddChild(member, field, type, alias, queryOnly, lazyLoad, cascadeDelete, filter, selectSP, this.provider);
			}
			else if (relationship == Relationship.Parent) {
				entity.AddParent(member, field, type, alias, queryOnly, lazyLoad, cascadeDelete, filter, selectSP, this.provider);
			}
			else {
				string table = this.GetValue(attributeNode, "table");
				string source = this.GetValue(attributeNode, "sourceField");
				string dest = this.GetValue(attributeNode, "destField");
				string insertSP = this.GetValue(attributeNode, "insertSP");
				string deleteSP = this.GetValue(attributeNode, "deleteSP");
				entity.AddMany(member, field, type, alias, table, source, dest, queryOnly, lazyLoad, cascadeDelete, filter, selectSP, insertSP, deleteSP, this.provider);
			}
		}

		private string GetValue(XmlNode node, string name) {
			return this.GetValue(node, name, String.Empty);
		}

		private string GetValue(XmlNode node, string name, string defaultValue) {
			return (node.Attributes[name] == null ? defaultValue : node.Attributes[name].Value);
		}

		private string GetNamespacedValue(XmlNode node, string name, string defaultNamespace) {
			string value = this.GetValue(node, name);
			if (defaultNamespace != null && value.IndexOf('.') < 0) {
				value = defaultNamespace + "." + value;
			}

			return value;
		}

		static internal void LoadAssemblies() {
			// Allow Entity Objects to Exist in any Referenced Assembly
			Assembly entryAssembly = Assembly.GetEntryAssembly();
			if (entryAssembly == null) {
				entryAssembly = Assembly.GetCallingAssembly(); // Web Applications
			}
			if (entryAssembly != null) {
				AssemblyName[] assemblies = entryAssembly.GetReferencedAssemblies();
				foreach (AssemblyName assembly in assemblies) {
					AppDomain.CurrentDomain.Load(assembly.FullName);
				}
			}
		}

		static internal string GetFullPath(string filePath) {
			string fullPath = filePath;
			string directory = AppDomain.CurrentDomain.BaseDirectory;
			while (!File.Exists(fullPath)) {
				fullPath = Path.Combine(directory, filePath);
				if (Directory.GetParent(directory) == null) break;
				directory = Directory.GetParent(directory).FullName;
			}
			if (!File.Exists(fullPath)) {
				throw new MappingException("ObjectSpace: MappingFile was not Found - " + filePath);
			}
			return fullPath;
		}

		static internal Stream GetResourceStream(string resourceName) {
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly assembly in assemblies) {
				if (assembly is System.Reflection.Emit.AssemblyBuilder) continue;
				Stream stream = assembly.GetManifestResourceStream(resourceName);
				if (stream != null) return stream;
			}
			throw new MappingException("ObjectSpace: MappingStream was not Found - " + resourceName);
		}
	}
}