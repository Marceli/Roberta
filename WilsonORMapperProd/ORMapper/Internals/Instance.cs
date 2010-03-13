//**************************************************************//
// Paul Wilson -- www.WilsonDotNet.com -- Paul@WilsonDotNet.com //
// Feel free to use and modify -- just leave these credit lines //
// I also always appreciate any other public credit you provide //
//**************************************************************//
// Ken Muse (http://www.MomentsFromImpact.com) gave significant //
// assistance with Recursive PersistChanges and Cascade Deletes //
//**************************************************************//
// Includes Support for Composite Primary Keys from Jerry Shea  //
// and Extensive Help Adding Support and Testing for Composite  //
// Relations from Nick Franceschina (http://www.simpulse.net)   //
//**************************************************************//
// Includes Support for Relations with Stored Procedures with   //
// assistance of Allan Ritchie (A.Ritchie@ACRWebSolutions.com)  //
//**************************************************************//
// Includes Null-Value Assistance from Tim Byng (http://www.missioninc.com)
// Includes Support for Member Properties from Chris Schletter (http://www.thzero.com)
// Includes EntityKey optimizations from Marc Brooks (http://musingmarc.blogspot.com)
// Includes DOTNETV2 consolidation from Marc Brooks (http://musingmarc.blogspot.com)
// Includes Composite Auto key support from Marc Brooks (http://musingmarc.blogspot.com)
using System;
using System.Collections;
using System.Data;
using System.Reflection;

namespace Wilson.ORMapper.Internals {
	internal class Instance {
		private Context context;
		private EntityMap entity;
		private Commands commands;

		private WeakReference instance; // GC requires Weak Reference
		private DateTime lastAccess;		// Server Apps require Session
		private InitialState initial;
		private bool isDeleted = false;
		private object[] values;
#if DOTNETV2
		private BindingFlags internalFlags = BindingFlags.CreateInstance | (BindingFlags.NonPublic | BindingFlags.Instance);
#endif

		internal EntityMap EntityMap {
			get {
				return this.entity;
			}
		}

		internal object EntityObject {
			get {
				if (!this.IsValid) return null;
				this.lastAccess = DateTime.Now;
				return this.instance.Target;
			}
			set {
				if (this.instance != null) this.instance.Target = value;
			}
		}

		public bool IsValid {
			get {
				return (this.instance != null && (this.instance.IsAlive
					|| DateTime.Now.Subtract(this.lastAccess) < this.context.Session));
			}
		}

		public bool IsPersisted {
			get { return this.initial != InitialState.Inserted; }
		}

		public ObjectState State {
			get {
				this.lastAccess = DateTime.Now;
				if (this.instance == null) return ObjectState.Unknown;
				if (this.isDeleted) return ObjectState.Deleted;
				if (this.initial == InitialState.Inserted) return ObjectState.Inserted;
				if (this.initial == InitialState.Updated) return ObjectState.Updated;
				return (this.IsDirty() ? ObjectState.Updated : ObjectState.Unchanged);
			}
		}

		private Instance(Context context, object entityObject) {
			this.context = context;
			string type = entityObject.GetType().ToString();
			this.entity = this.context.Mappings[type];
			this.commands = this.context.Mappings.Commands(type);
			this.instance = new WeakReference(entityObject);
			this.values = new object[this.entity.FieldCount];
		}

		internal Instance(Context context, object entityObject, InitialState initialState)
			: this(context, entityObject) {
			for (int index = 0; index < this.entity.RelationCount; index++) {
				RelationMap relation = this.entity.Relation(index);
				if (relation.QueryOnly) continue;

				// Do NOT initialize relations for entities with existing relations
				if (this.GetField(relation.Member) == null) {
					string typeName = relation.Type;
					Type type = EntityMap.GetType(typeName);

					object relations = null;

					if (!relation.Lazy && relation.Relationship == Relationship.Parent) {
						relations = this.context.GetObject(type);
					}
					else {
#if DOTNETV2
						Type genericType = null;
						object[] args = null;

						if (relation.Lazy) {
							if (relation.Relationship == Relationship.Parent) {
								genericType = typeof(ObjectHolder<>).MakeGenericType(type);
								args = new object[] { this.context, DBNull.Value };
							}
							else {
								genericType = typeof(ObjectList<>).MakeGenericType(type);
								args = new object[] { this.context };
							}
						}
						else {
							genericType = typeof(ObjectSet<>).MakeGenericType(type);
							args = new object[] { 1, 0, 0 };
						}

						relations = Activator.CreateInstance(genericType, internalFlags, null, args, null, null);
#else
						if (relation.Lazy) {
							if (relation.Relationship == Relationship.Parent) {
								relations = new ObjectHolder(this.context, type, null);
							}
							else {
								relations = new ObjectList(this.context, type);
							}
						}
						else {
							relations = new ObjectSet(type, 1, 0, 0);
						}
#endif
					}

					this.SetField(relation.Member, relations);
				}
			}
			this.StartTracking(initialState);
		}

		internal Instance(Context context, object entityObject, IDataReader data)
			: this(context, entityObject) {
			this.SetObject(data);
		}

		private void SetObject(IDataRecord data) {
			this.lastAccess = DateTime.Now;
			int level = LocalStore.Level;
			LocalStore.Level++;
			for (int index = 0; index < this.entity.FieldCount; index++) {
				FieldMap member = this.entity[index];
				object nullValue = member.NullValue;
				string fieldAlias = member.FieldAlias;

				// Handle Database DateTimes not Supported by .NET from Nick Franceschina
				object memberValue = null;
				try {
					memberValue = data[fieldAlias];
				}
				catch (ArgumentOutOfRangeException) {
					memberValue = nullValue;
				}

				memberValue = this.MemberValue(memberValue, nullValue);

				// Bug-Fix for Changes to Type by Jerry Shea (http://www.RenewTek.com)
				this.values[index] = this.SetField(member, memberValue);
			}
			for (int index = 0; index < this.entity.RelationCount; index++) {
				RelationMap relation = this.entity.Relation(index);
				if (relation.QueryOnly) continue;

				string typeName = relation.Type;
				Type type = EntityMap.GetType(typeName);
				Commands commands = this.context.Mappings.Commands(type);

				string[] dataFields = new string[this.entity.KeyFields.Length];
				for (int index0 = 0; index0 < this.entity.KeyFields.Length; index0++) {
					dataFields[index0] = this.entity.KeyFields[index0].Field;
				}
				string[] relatedFields = relation.Fields;

				object[] dataValues = new object[dataFields.Length];
				for (int index1 = 0; index1 < dataFields.Length; index1++) {
					dataValues[index1] = (data[dataFields[index1]] == DBNull.Value ? null : data[dataFields[index1]]);
				}
				string whereClause = relation.Filter;
				string sortClause = this.context.Mappings[typeName].SortOrder;

				ObjectQuery query = null;
				SelectProcedure selectSP = null;
				if (relation.SelectSP == null || relation.SelectSP.Length == 0) {
					if (relation.Relationship == Relationship.Many) {
						string manyTable = ((ManyMap)relation).Table;
						string[] sourceFields = ((ManyMap)relation).Source;
						string[] destFields = ((ManyMap)relation).Dest;
						query = commands.ManyQuery(type, relatedFields, manyTable, sourceFields, destFields,
							dataValues, whereClause, sortClause);
					}
					else if (relation.Relationship == Relationship.Child) {
						query = commands.FieldQuery(type, relatedFields, dataValues, whereClause, sortClause);
					}
				}
				else {
					selectSP = new SelectProcedure(type, relation.SelectSP);
					if (relation.Relationship == Relationship.Parent) {
						for (int index2 = 0; index2 < relatedFields.Length; index2++) {
							selectSP.AddParameter(relatedFields[index2], data[relatedFields[index2]]);
						}
					}
					else {
						for (int index3 = 0; index3 < relatedFields.Length; index3++) {
							selectSP.AddParameter(relatedFields[index3], data[dataFields[index3]]);
						}
					}
				}
				object[] relatedValues = null;
				if (relation.Relationship == Relationship.Parent && selectSP == null) {
					relatedValues = new object[relatedFields.Length];
					for (int index4 = 0; index4 < relatedFields.Length; index4++) {
						relatedValues[index4] = (data[relatedFields[index4]] == DBNull.Value ? null : data[relatedFields[index4]]);
					}
				}

				bool usedExisting = false;
				object relations = null;
				if (relation.Lazy) {
#if DOTNETV2
					Type genericType = null;
					object[] args = null;

					if (relation.Relationship == Relationship.Parent) {
						if (selectSP == null) {
							if (relatedValues.Length == 1) {
								genericType = typeof(ObjectHolder<>).MakeGenericType(type);
								// Force proper ObjectHolder constructor to be called when relatedValues[0] is null - Steve Eichert (http:steve.emxsoftware.com)
								args = new object[] { this.context, relatedValues[0] == null ? DBNull.Value : relatedValues[0] };
							}
							else {
								genericType = typeof(ObjectHolder<>).MakeGenericType(type);
								args = new object[] { this.context, relatedValues };
							}
						}
						else {
							genericType = typeof(ObjectHolder<>).MakeGenericType(type);
							args = new object[] { this.context, selectSP };
						}
					}
					else {
						if (selectSP == null) {
							genericType = typeof(ObjectList<>).MakeGenericType(type);
							args = new object[] { this.context, query };
						}
						else {
							genericType = typeof(ObjectList<>).MakeGenericType(type);
							args = new object[] { this.context, selectSP };
						}
					}

					relations = Activator.CreateInstance(genericType, internalFlags, null, args, null, null);
#else
					if (relation.Relationship == Relationship.Parent) {
						if (selectSP == null) {
							if (relatedValues.Length == 1) {
								relations = new ObjectHolder(this.context, type, relatedValues[0]);
							}
							else {
								relations = new ObjectHolder(this.context, type, relatedValues);
							}
						}
						else {
							relations = new ObjectHolder(this.context, selectSP);
						}
					}
					else {
						if (selectSP == null) {
							relations = new ObjectList(this.context, query);
						}
						else {
							relations = new ObjectList(this.context, selectSP);
						}
					}
#endif
				}
				else {
					// Avoid infinite relationship loop for non-lazy-loaded collections
					if (LocalStore.IsLoaded(type) && level > 1)
						continue;
					if (relation.Relationship == Relationship.Parent) {
						if (selectSP == null) {
							if (relatedValues.Length == 1) {
								relations = this.context.GetObject(type, relatedValues[0], false);
							}
							else {
								relations = this.context.GetObject(type, relatedValues, false);
							}
						}
						else {
							relations = this.context.GetObjectSet(selectSP, false)[0];
						}
					}
					else {
#if DOTNETV2
						if (selectSP == null) {
							using (ObjectReader reader = this.context.GetObjectReader(query, false)) {
								Type genericType = typeof(ObjectSet<>).MakeGenericType(type);
								object[] args = new object[] { reader };
								relations = Activator.CreateInstance(genericType, internalFlags, null, args, null, null);
							}
						}
						else {
							using (ObjectReader reader = this.context.GetObjectReader(selectSP, false)) {
								Type genericType = typeof(ObjectSet<>).MakeGenericType(type);
								object[] args = new object[] { reader };
								relations = Activator.CreateInstance(genericType, internalFlags, null, args, null, null);
							}
						}
#else
						if (selectSP == null) {
							relations = this.context.GetObjectSet(query, false);
						}
						else {
							relations = this.context.GetObjectSet(selectSP, false);
						}
#endif

						// Do NOT initialize list relations for non-lazy entities with existing relations
						IList relationList = this.GetField(relation.Member) as IList;
						if (relationList != null) {
							relationList.Clear();
							foreach (object entity in relations as IEnumerable) {
								relationList.Add(entity);
							}
							usedExisting = true;
						}
					}
				}

				if (!usedExisting) this.SetField(relation.Member, relations);
			}
			LocalStore.Level = level;

			this.StartTracking(InitialState.Unchanged, false);
		}

		public void StartTracking(InitialState initialState) {
			this.StartTracking(initialState, true);
		}

		private void StartTracking(InitialState initialState, bool setValues) {
			this.lastAccess = DateTime.Now;
			this.initial = initialState;
			this.isDeleted = false;
			if (setValues) {
				for (int index = 0; index < this.entity.FieldCount; index++) {
					this.values[index] = this.GetField(this.entity[index]);
				}
			}
		}

		public void MarkForDeletion() {
			this.lastAccess = DateTime.Now;
			this.isDeleted = true;
		}

		public void CancelChanges() {
			this.lastAccess = DateTime.Now;
			this.isDeleted = false;
			for (int index = 0; index < this.entity.FieldCount; index++) {
				this.SetField(this.entity[index], this.values[index]);
			}
		}

		public void PersistChanges(Transaction transaction, PersistDepth persistDepth) {
			this.PersistChanges(transaction, persistDepth, false);
		}

		// Make sure cascade deletes bubble through all related child collection
		private void PersistChanges(Transaction transaction, PersistDepth persistDepth, bool parentDeleted) {
			this.lastAccess = DateTime.Now;
			try {
				if (this.entity.ReadOnly) {
					throw new PersistenceException("ObjectSpace: Entity is ReadOnly - " + this.entity.Type);
				}
				else if (this.State == ObjectState.Deleted || parentDeleted) {
					if (this.entity.HasEvents) {
						((IObjectNotification)this.EntityObject).OnDeleting(transaction);
					}
					this.PersistChildren(transaction, persistDepth, parentDeleted);
					this.CascadeDeletes(transaction);
					if (this.State == ObjectState.Deleted) this.DeleteObject(transaction);
					if (this.entity.HasEvents) {
						((IObjectNotification)this.EntityObject).OnDeleted(transaction);
					}
				}
				else if (this.initial == InitialState.Inserted) {
					if (this.entity.HasEvents) {
						((IObjectNotification)this.EntityObject).OnCreating(transaction);
					}
					this.InsertObject(transaction);
					this.PersistChildren(transaction, persistDepth, parentDeleted);
					if (this.entity.HasEvents) {
						((IObjectNotification)this.EntityObject).OnCreated(transaction);
					}
				}
				else {
					if (this.entity.HasEvents) {
						((IObjectNotification)this.EntityObject).OnUpdating(transaction);
					}
					this.PersistChildren(transaction, persistDepth, parentDeleted);
					if (this.State == ObjectState.Updated) this.UpdateObject(transaction);
					if (this.entity.HasEvents) {
						((IObjectNotification)this.EntityObject).OnUpdated(transaction);
					}
				}
			}
			catch (Exception exception) {
				if (this.entity.HasEvents) {
					((IObjectNotification)this.EntityObject).OnPersistError(transaction, exception);
				}
				throw;
			}
		}

		// Make sure cascade deletes bubble through all related child collection
		private void PersistChildren(Transaction transaction, PersistDepth persistDepth, bool parentDeleted) {
			if (persistDepth == PersistDepth.ObjectGraph) {
				object[] keyValues = new object[this.entity.KeyFields.Length];
				for (int index1 = 0; index1 < this.entity.KeyFields.Length; index1++) {
					keyValues[index1] = this.GetField(this.entity.KeyFields[index1].Member);
				}
				for (int index = 0; index < this.entity.RelationCount; index++) {
					bool cascadeDelete = (this.State == ObjectState.Deleted) && this.entity.Relation(index).Cascade;
					if (this.entity.Relation(index).Relationship == Relationship.Child) {
						EntityMap childMap = this.context.Mappings[this.entity.Relation(index).Type];
						string[] childMembers = new string[this.entity.Relation(index).Fields.Length];
						for (int index2 = 0; index2 < childMembers.Length; index2++) {
							for (int field = 0; field < childMap.FieldCount; field++) {
								if (childMap[field].Field == this.entity.Relation(index).Fields[index2]
									&& childMap[field].PersistType == PersistType.Persist) {
									childMembers[index2] = childMap[field].Member;
									break;
								}
							}
						}
						IList children = (IList)this.GetField(this.entity.Relation(index).Member);
						// Do not lazy-load lists to persist them -- Jerry Shea (http://www.RenewTek.com)
						if (!(children is ILoadOnDemand) || (children as ILoadOnDemand).IsLoaded || cascadeDelete || parentDeleted) {
							// Force recursive persistence if there are more child relations in this graph
							bool subChildren = (this.context.Mappings[this.entity.Relation(index).Type].ChildRelations > 0);
							foreach (object entityChild in children) {
								if (subChildren || cascadeDelete || parentDeleted || this.context[entityChild].State != ObjectState.Unchanged) {
									if (this.context[entityChild].State == ObjectState.Inserted) {
										for (int index3 = 0; index3 < childMembers.Length; index3++) {
											this.context[entityChild].SetField(childMembers[index3], keyValues[index3]);
										}
									}
									if (this.context[entityChild].State != ObjectState.Unknown) {
										this.context[entityChild].PersistChanges(transaction, persistDepth, cascadeDelete || parentDeleted);
									}
								}
							}
							BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
							PropertyInfo relation = children.GetType().GetProperty("Removed", flags);
							if (relation != null) {
								ArrayList removed = (ArrayList)relation.GetValue(children, null);
								foreach (object removedChild in removed) {
									if (this.context[removedChild].State != ObjectState.Unknown) {
										this.context[removedChild].MarkForDeletion();
										this.context[removedChild].PersistChanges(transaction, persistDepth, true);
									}
								}
								removed.Clear();
							}
						}
					}
					else if (this.entity.Relation(index).Relationship == Relationship.Many) {
						ManyMap manyMap = (ManyMap)this.entity.Relation(index);
						IList children = (IList)this.GetField(manyMap.Member);
						// Do not lazy-load lists to persist them
						if (!(children is ILoadOnDemand) || (children as ILoadOnDemand).IsLoaded || cascadeDelete || parentDeleted) {
							if (manyMap.DeleteSP == null || manyMap.DeleteSP.Length == 0) {
								string whereClause = this.commands.GetExpression(manyMap.Table, manyMap.Source, keyValues);
								string sqlDelete = this.commands.CreateDelete(manyMap.Table, whereClause);
								this.context.Connection.TransactionCommand(transaction.id, this.EntityObject.GetType(), CommandInfo.ManyDelete, transaction.transaction, sqlDelete);
							}
							else {
								Parameter[] parameters = new Parameter[keyValues.Length];
								for (int index4 = 0; index4 < keyValues.Length; index4++) {
									parameters[index4] = new Parameter(manyMap.Source[index4], keyValues[index4], null);
								}
								this.context.Connection.TransactionCommand(transaction.id, this.EntityObject.GetType(), CommandInfo.ManyDelete, transaction.transaction, manyMap.DeleteSP, parameters);
							}
							if (!(cascadeDelete || parentDeleted)) { // Bug-Fix for Multiple Many-to-Many to Avoid Re-Inserts on Deletes
								foreach (object entityChild in children) {
									Instance childInstance = this.context[entityChild];
									object childKeys = EntityKey.GetObjectKey(this.context, childInstance);
									if (manyMap.InsertSP == null || manyMap.InsertSP.Length == 0) {
										object[] childValues = null;
										if (childKeys.GetType().IsArray) {
											childValues = childKeys as object[];
										}
										else {
											childValues = new object[] { childKeys };
										}
										string sqlInsert = this.commands.InsertMany(manyMap.Table,
											manyMap.Source, manyMap.Dest, keyValues, childValues);
										this.context.Connection.TransactionCommand(transaction.id, this.EntityObject.GetType(), CommandInfo.ManyInsert, transaction.transaction, sqlInsert);
									}
									else {
										Parameter[] parameters = new Parameter[2 * keyValues.Length];
										for (int index5 = 0; index5 < keyValues.Length; index5++) {
											parameters[index5] = new Parameter(manyMap.Source[index5], keyValues[index5], null);
										}
										if (childKeys.GetType().IsArray) {
											for (int index6 = keyValues.Length; index6 < 2 * keyValues.Length; index6++) {
												parameters[index6] = new Parameter(manyMap.Dest[index6], (childKeys as object[])[index6], null);
											}
										}
										else {
											parameters[keyValues.Length] = new Parameter(manyMap.Dest[0], childKeys, null);
										}
										this.context.Connection.TransactionCommand(transaction.id, this.EntityObject.GetType(), CommandInfo.ManyInsert, transaction.transaction, manyMap.InsertSP, parameters);
									}
								}
							}
						}
					}
				}
			}
		}

		private void CascadeDeletes(Transaction transaction) {
			object[] keyValues = new object[this.entity.KeyFields.Length];
			for (int index = 0; index < this.entity.KeyFields.Length; index++) {
				keyValues[index] = this.GetField(this.entity.KeyFields[index].Member);
			}
			for (int index = 0; index < this.entity.RelationCount; index++) {
				if (this.entity.Relation(index).Cascade) { // Not supported by stored procs
					if (this.entity.Relation(index).Relationship == Relationship.Child) {
						string childType = this.entity.Relation(index).Type;
						string[] childFields = this.entity.Relation(index).Fields;
						Commands childCommands = this.context.Mappings.Commands(childType);
						string whereClause = childCommands.GetExpression(childFields, keyValues);
						string sqlDelete = childCommands.CreateDelete(whereClause);
						this.context.Connection.TransactionCommand(transaction.id, this.EntityObject.GetType(), CommandInfo.CascadeDelete, transaction.transaction, sqlDelete);
					}
					else if (this.entity.Relation(index).Relationship == Relationship.Many) {
						ManyMap manyMap = (ManyMap)this.entity.Relation(index);
						string whereClause = this.commands.GetExpression(manyMap.Table, manyMap.Source, keyValues);
						string sqlDelete = this.commands.CreateDelete(manyMap.Table, whereClause);
						this.context.Connection.TransactionCommand(transaction.id, this.EntityObject.GetType(), CommandInfo.CascadeDelete, transaction.transaction, sqlDelete);
					}
				}
			}
		}

		public void CommitChanges(PersistDepth persistDepth) {
			this.CommitChildren(persistDepth);
			if (this.State == ObjectState.Deleted) {
				this.instance = null;
			}
			else {
				if (this.initial == InitialState.Inserted && this.entity.KeyType == KeyType.Auto) {
					// InitialState Bug-Fix by Gerrod Thomas (http://www.Gerrod.com)
					this.initial = InitialState.Unchanged;
					EntityKey entityKey = new EntityKey(this.context, this.EntityObject, this.IsPersisted);
					this.context.StartTracking(entityKey, this);
				}
				this.StartTracking(InitialState.Unchanged);
			}
		}

		private void CommitChildren(PersistDepth persistDepth) {
			if (persistDepth == PersistDepth.ObjectGraph) {
				for (int index = 0; index < this.entity.RelationCount; index++) {
					if (this.entity.Relation(index).Relationship == Relationship.Child) {
						IList children = (IList)this.GetField(this.entity.Relation(index).Member);
						// Do not lazy-load lists to persist them -- Jerry Shea (http://www.RenewTek.com)
						if (!(children is ILoadOnDemand) || (children as ILoadOnDemand).IsLoaded) {
							foreach (object entityChild in children) {
								try {
									if (this.context.IsTracked(entityChild)) {
										this.context[entityChild].CommitChanges(persistDepth);
									}
									else {
										this.context.StartTracking(entityChild, InitialState.Unchanged);
									}
								}
								catch {
									// Do Nothing
								}
							}
						}
					}
				}
			}
		}

		public void RollbackChanges(PersistDepth persistDepth) {
			this.RollbackChildren(persistDepth);
			if (this.initial == InitialState.Inserted && this.entity.KeyType == KeyType.Auto) {
				// Tracking Bug-Fix by Gerrod Thomas (http://www.Gerrod.com)
				this.context.EndTracking(this.EntityObject);
				FieldMap autoKeyMember = this.entity.AutoKeyMember();
				this.SetField(autoKeyMember, this.GetField(autoKeyMember));
				this.context.StartTracking(this);
			}
		}

		private void RollbackChildren(PersistDepth persistDepth) {
			if (persistDepth == PersistDepth.ObjectGraph) {
				for (int index = 0; index < this.entity.RelationCount; index++) {
					if (this.entity.Relation(index).Relationship == Relationship.Child) {
						IList children = (IList)this.GetField(this.entity.Relation(index).Member);
						foreach (object entityChild in children) {
							try {
								this.context[entityChild].RollbackChanges(persistDepth);
							}
							catch {
								// Do Nothing
							}
						}
					}
				}
			}
		}

		private void InsertObject(Transaction transaction) {
			EntityMap map = this.entity;

			FieldMap autoKeyMember = null;
			int paramCount = map.FieldCount - map.ReadOnlyCount - map.ConcurrentCount;

			if (map.KeyType == KeyType.Auto) {
				// autos have one key segment at the end that is an out value...
				paramCount--;
			}

			Parameter[] parameters = new Parameter[paramCount];
			int paramIndex = 0;
			for (int index = 0; index < map.FieldCount; index++) {
				FieldMap field = map[index];
				if (field.PersistType == PersistType.Persist) {
					if (map.KeyType == KeyType.Auto && map.IsAutoKeyMember(field.Member)) {
						// skip the final key segment that are auto-assigned but note it
						autoKeyMember = field;
						continue;
					}
					string name = field.Parameter;
					object nullValue = field.NullValue;
					object value = this.GetField(field.Member);
					parameters[paramIndex] = new Parameter(name, value, nullValue);
					paramIndex++;
				}
			}

			if (map.KeyType == KeyType.Auto) {
				object keyValue = this.context.Connection.TransactionScalar(transaction.id, this.EntityObject.GetType(), CommandInfo.Insert,
					transaction.transaction, this.commands.Insert, parameters);
				if (keyValue == null) {
					throw new PersistenceException("ObjectSpace: Entity Object was not Inserted - " + this.entity.Type);
				}
				// Tracking Bug-Fix by Gerrod Thomas (http://www.Gerrod.com)
				this.context.EndTracking(this.EntityObject);
				this.SetField(autoKeyMember, keyValue);
				this.context.StartTracking(this);
			}
			else {
				int output = this.context.Connection.TransactionCommand(transaction.id, this.EntityObject.GetType(), CommandInfo.Insert,
					transaction.transaction, this.commands.Insert, parameters);
				if (output <= 0) {
				throw new PersistenceException("ObjectSpace: Entity Object was not Inserted - " + this.entity.Type);
				}
			}
		}

		private void UpdateObject(Transaction transaction) {
			int changedCount = 0;
			int paramCount = this.entity.FieldCount - this.entity.ReadOnlyCount;
			if (this.initial == InitialState.Unchanged && this.entity.ChangesOnly && this.entity.UpdateSP.Length == 0) {
				for (int index = 0; index < this.entity.FieldCount; index++) {
					if (this.entity[index].PersistType == PersistType.Persist
						&& !this.entity.IsKeyMember(this.entity[index].Member)) {
						if (this.IsDirty(index)) {
							changedCount++;
						}
						else {
							paramCount--;
						}
					}
				}
				if (changedCount == 0) return;
			}

			Parameter[] parameters = new Parameter[paramCount];
			FieldMap[] changedFields = new FieldMap[changedCount];
			int paramIndex = 0;
			int changedIndex = 0;
			int concurrentIndex = paramCount - 1;
			for (int index = 0; index < this.entity.FieldCount; index++) {
				if (this.entity[index].PersistType != PersistType.ReadOnly
					&& !this.entity.IsKeyMember(this.entity[index].Member)) {
					string name = this.entity[index].Parameter;
					object nullValue = this.entity[index].NullValue;
					object value = this.GetField(this.entity[index].Member);
					if (this.entity[index].PersistType == PersistType.Concurrent) {
						parameters[concurrentIndex] = new Parameter(name, value, nullValue);
						concurrentIndex--;
					}
					else { // PersistType.Persist
						if (changedCount == 0 || this.IsDirty(index)) {
							parameters[paramIndex] = new Parameter(name, value, nullValue);
							paramIndex++;
							if (changedCount > 0) {
								changedFields[changedIndex] = this.entity[index];
								changedIndex++;
							}
						}
					}
				}
			}

			FieldMap[] keyFields = this.entity.KeyFields;
			for (int index = 0; index < keyFields.Length; index++) {
				string keyName = keyFields[index].Parameter;
				object keyNullValue = keyFields[index].NullValue;
				object keyValue = this.GetField(keyFields[index].Member);
				parameters[paramIndex] = new Parameter(keyName, keyValue, keyNullValue);
				paramIndex++;
			}
			string update;
			if (changedCount == 0) {
				update = this.commands.Update;
			}
			else {
				update = this.commands.CreateUpdate(changedFields);
			}
			int output = this.context.Connection.TransactionCommand(transaction.id, this.EntityObject.GetType(), CommandInfo.Update,
				transaction.transaction, update, parameters);
			if (output <= 0) {
				throw new PersistenceException("ObjectSpace: Entity Object was not Updated - " + this.entity.Type);
			}
		}

		private void DeleteObject(Transaction transaction) {
			if (this.initial != InitialState.Inserted) {
				FieldMap[] keyFields = this.entity.KeyFields;
				Parameter[] parameters = new Parameter[this.entity.ConcurrentCount + keyFields.Length];
				int paramIndex = 0;
				for (int index = 0; index < keyFields.Length; index++) {
					string keyName = keyFields[index].Parameter;
					object keyNullValue = keyFields[index].NullValue;
					object keyValue = this.GetField(keyFields[index].Member);
					parameters[paramIndex] = new Parameter(keyName, keyValue, keyNullValue);
					paramIndex++;
				}

				for (int index = 0; index < this.entity.FieldCount; index++) {
					if (this.entity[index].PersistType == PersistType.Concurrent
						&& !this.entity.IsKeyMember(this.entity[index].Member)) {
						string name = this.entity[index].Parameter;
						object nullValue = this.entity[index].NullValue;
						object value = this.GetField(this.entity[index].Member);
						if (this.entity[index].PersistType == PersistType.Concurrent) {
							parameters[paramIndex] = new Parameter(name, value, nullValue);
							paramIndex++;
						}
					}
				}
				int output = this.context.Connection.TransactionCommand(transaction.id, this.EntityObject.GetType(), CommandInfo.Delete,
					transaction.transaction, this.commands.Delete, parameters);
				if (output <= 0) {
					throw new PersistenceException("ObjectSpace: Entity Object was not Deleted - " + this.entity.Type);
				}
			}
		}

		public override string ToString() {
			string contents = this.entity.Type + ": ";
			for (int index = 0; index < this.entity.FieldCount; index++) {
				object value = this.GetField(this.entity[index].Member);
				contents += this.entity[index].Member + " = " + (value == null ? "Null" : value.ToString()) + ", ";
			}
			return contents.Substring(0, contents.Length - 2);
		}

		private bool IsDirty() {
			for (int index = 0; index < this.entity.FieldCount; index++) {
				if (this.IsDirty(index)) return true;
			}
			return false;
		}

		private bool IsDirty(int fieldIndex) {
			object initial = this.values[fieldIndex];
			object current = this.GetField(this.entity[fieldIndex].Member);
			if (initial == null && current == null) return false;
			if (initial == null && current != null) return true;
			if (initial != null && current == null) return true;
			return !current.Equals(initial);
		}

		internal object GetField(FieldMap member) {
			return this.GetField(member.Member);
		}

		internal object GetField(string member) {
			return this.GetField(this.entity, member);
		}

		internal object GetField(EntityMap entityMap, string member) {
			return Instance.GetField(this.entity, this.EntityObject, member);
		}

		internal static object GetField(EntityMap entityMap, object entityObject, string member) {
			if (entityMap.HasHelper) {
				try {
					return ((IObjectHelper)entityObject)[member];
				}
				catch (Exception exception) {
					throw new MappingException("Mapping: IObjectHelper is missing " + member + " : " + entityMap.Type, exception);
				}
			}
			else {
				try {
					// Improved Support for Embedded Objects from Chris Schletter (http://www.thzero.com)
					object memberValue = entityObject;
					string[] memberParts = member.Split('.');
					for (int index = 0; index < memberParts.Length; index++) {
						string typeName = memberValue.GetType().ToString();
						MemberInfo memberField = EntityMap.FindField(typeName, memberParts[index]);
						if (memberField is FieldInfo) {
							memberValue = (memberField as FieldInfo).GetValue(memberValue);
						}
						else {
							memberValue = (memberField as PropertyInfo).GetValue(memberValue, null);
						}
					}
					return memberValue;
				}
				catch (Exception exception) {
					throw new ORMapperException("GetField failed for " + member + " : " + entityMap.Type, exception);
				}
			}
		}

		internal object SetField(FieldMap member, object value) {
			return this.SetField(member.Member, value);
		}

		internal object SetField(string member, object value) {
			return this.SetField(this.entity, member, value);
		}

		internal object SetField(EntityMap entityMap, string member, object value) {
			return Instance.SetField(this.entity, this.EntityObject, member, value, this.context.Provider);
		}

		// Includes Support for Enumerated Member Types from Jerry Shea (http://www.RenewTek.com)
		internal static object SetField(EntityMap entityMap, object entityObject, string member, object value, CustomProvider provider) {
			Type type = EntityMap.GetType(entityMap.Member(member));
			object typedValue = QueryHelper.ChangeType(value, type);
			if (entityMap.HasHelper) {
				try {
					((IObjectHelper)entityObject)[member] = typedValue;
				}
				catch (Exception exception) {
					if (typedValue == null) {
						throw new ORMapperException("ObjectSpace: SetField failed for NULL " + member + " : " + entityMap.Type, exception);
					}
					else {
						throw new MappingException("Mapping: IObjectHelper is missing " + member + " : " + entityMap.Type, exception);
					}
				}
			}
			else {
				try {
					// Improved Support for Embedded Objects from Chris Schletter (http://www.thzero.com)
					object memberValue = entityObject;
					string[] memberParts = member.Split('.');
					for (int index = 0; index < memberParts.Length; index++) {
						string typeName = memberValue.GetType().ToString();
						MemberInfo memberField = EntityMap.FindField(typeName, memberParts[index]);
						if (index == memberParts.Length - 1) {
							if (memberField is FieldInfo) {
								(memberField as FieldInfo).SetValue(memberValue, typedValue);
							}
							else {
								(memberField as PropertyInfo).SetValue(memberValue, typedValue, null);
							}
						}
						else {
							if (memberField is FieldInfo) {
								memberValue = (memberField as FieldInfo).GetValue(memberValue);
							}
							else {
								memberValue = (memberField as PropertyInfo).GetValue(memberValue, null);
							}
						}
					}
				}
				catch (Exception exception) {
					if (typedValue == null) {
						throw new ORMapperException("ObjectSpace: SetField failed for NULL " + member + " : " + entityMap.Type, exception);
					}
					else {
						throw new ORMapperException("ObjectSpace: SetField failed for " + member + " : " + entityMap.Type, exception);
					}
				}
			}
			return typedValue;
		}

		private object MemberValue(object value, object nullValue) {
			if (value == System.DBNull.Value) value = nullValue;
			// DateTime provider-specific min/max from Marc Brooks (http://musingmarc.blogspot.com)
			if (value is DateTime) {
				CustomProvider provider = this.context.Provider;
				DateTime dateValue = (DateTime)value;
				if (dateValue <= provider.MinimumDate) {
					value = DateTime.MinValue;
				}
				else if (dateValue >= provider.MaximumDate) {
					value = DateTime.MaxValue;
				}
			}
			return value;
		}
	}
}