//**************************************************************//
// Paul Wilson -- www.WilsonDotNet.com -- Paul@WilsonDotNet.com //
// Feel free to use and modify -- just leave these credit lines //
// I also always appreciate any other public credit you provide //
//**************************************************************//
// Ken Muse (http://www.MomentsFromImpact.com) gave significant //
// assistance with Recursive PersistChanges and Cascade Deletes //
//**************************************************************//
// Includes Support for Composite Primary Keys and Better Guid  //
// Delimiter Handling from Jerry Shea (http://www.RenewTek.com) //
//**************************************************************//
// Help on Composite Relations from Nick Franceschina (http://www.simpulse.net)
// Includes Composite Auto key support from Marc Brooks (http://musingmarc.blogspot.com)
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace Wilson.ORMapper.Internals
{
	internal class Commands
	{
		private string QS = "\""; // Quote-Start Delimiter for Fields and Tables
		private string QE = "\""; // Quote-End Delimiter for Fields and Tables
		private string TS = ";"; // Terminate-Statement Delimiter for SQL Batch
		protected string DD = "'"; // Date Delimiter since Access is Different
		protected string GD = "'"; // Guid Delimiter since Access is Different

		protected EntityMap entity;
		protected CustomProvider provider;
		protected string select;
		protected string insert;
		protected string update;
		protected string delete;

		protected string selectFields;
		protected string fromTables;
		protected string updatePreSet;
		protected string updatePostSet;

		public string GetFieldName(string aliasName) {
			FieldMap field = this.entity.GetFieldMap(aliasName);
			string fieldName =  this.entity.GetFieldMap(aliasName).Field;
			string tableName = (field.IsLookup ? (field as LookupMap).TableAlias : this.entity.Table);
			return this.GetDelimited(tableName, fieldName);
		}

		public string GetTableName() {
			return this.GetDelimited(this.entity.Table);
		}

		public string GetExpression(string fieldName, object fieldValue) {
			return this.GetExpression(this.entity.Table, fieldName, fieldValue);
		}

		public string GetExpression(string[] fieldName, object[] fieldValue) {
			// Jeff Lanning (jefflanning@gmail.com): Added length check to avoid a confusing index-out-of-range exception below.
			if (fieldName.Length != fieldValue.Length) {
				throw new ArgumentException("The passed arrays are different lengths.  This may indicate a problem with the mapping for entity '" + entity.Type + "'.");
			}
			StringBuilder retval = new StringBuilder();
			for (int index = 0; index < fieldName.Length; index++) {
				if (index > 0) retval.Append(" AND ");
				retval.Append(this.GetExpression(this.entity.Table, fieldName[index], fieldValue[index]));
			}
			return retval.ToString();
		}

		public string GetExpression(string tableName, string fieldName, object fieldValue) {
			if (fieldValue == null || fieldValue == DBNull.Value) {
				return string.Format("{0} IS NULL", this.GetDelimited(tableName, fieldName));
			}
			else {
				return string.Format("{0} = {1}", this.GetDelimited(tableName, fieldName), this.CleanValue(fieldValue));
			}
		}

		public string GetExpression(string tableName, string[] fieldName, object[] fieldValue) {
			// Jeff Lanning (jefflanning@gmail.com): Added length check to avoid a confusing index-out-of-range exception below.
			if (fieldName.Length != fieldValue.Length) {
				throw new ArgumentException("The passed arrays are different lengths.  This may indicate a problem with the mapping for entity '" + entity.Type + "'.");
			}
			StringBuilder retval = new StringBuilder();
			for (int index = 0; index < fieldName.Length; index++) {
				if (index > 0) retval.Append(" AND ");
				retval.Append(this.GetExpression(tableName, fieldName[index], fieldValue[index]));
			}
			return retval.ToString();
		}

		private string CleanValue(object value) {
			if (value is string) {
				return "'" + value.ToString().Replace("'", "''") + "'";
			}
			else if (value is DateTime) {
				if (this.provider.DateFormat != null && this.provider.DateFormat.Length > 0) {
					return DD + ((DateTime)value).ToString(this.provider.DateFormat) + DD;
				}
				else {
					return DD + value.ToString() + DD;
				}
			}
			else if (value is Guid) {
				return GD + "{" + value.ToString() + "}" + GD;
			}
			else {
				return value.ToString();
			}
		}

		// Handle Database.Owner.Table and Database..Table Syntax
		public string GetDelimited(string objectName) {
			string[] objectParts = objectName.Split('.');
			for (int index = 0; index < objectParts.Length; index++) {
				if (objectParts[index].Length > 0) {
					objectParts[index] = QS + objectParts[index] + QE;
				}
			}
			return string.Join(".", objectParts);
		}

		// Handle Database.Owner.Table and Database..Table Syntax
		protected string GetDelimited(string tableName, string fieldName) {
			return string.Format("{0}.{2}{1}{3}", this.GetDelimited(tableName), fieldName, QS, QE);
		}

		public string Insert {
			get { return this.insert; }
		}

		public string Update {
			get { return this.update; }
		}

		public string Delete {
			get { return this.delete; }
		}

		public ObjectQuery KeyQuery(Type objectType, object objectKey) {
			object[] compositeKeyValues = objectKey as object[];
			if (compositeKeyValues != null) {
				string[] keyFields = new string[compositeKeyValues.Length];
				for (int index = 0; index < compositeKeyValues.Length; index++)
					keyFields[index] = this.entity.KeyFields[index].Field;
				return this.FieldQuery(objectType, keyFields, compositeKeyValues);
			}
			else {
				string[] keyFields = new string[] {this.entity.KeyFields[0].Field};
				object[] objectKeys = new object[] {objectKey};
				return this.FieldQuery(objectType, keyFields, objectKeys, string.Empty, string.Empty);
			}
		}

		public ObjectQuery FieldQuery(Type objectType, string[] fieldName, object[] fieldValue) {
			string selectKey = this.GetExpression(fieldName, fieldValue);
			return new ObjectQuery(objectType, selectKey, String.Empty);
		}

		public ObjectQuery FieldQuery(Type objectType, string[] fieldName, object[] fieldValue,
				string whereClause, string sortClause) {
			string selectKey = this.GetExpression(this.entity.Table, fieldName, fieldValue)
				+ (whereClause != null && whereClause.Length > 0 ? " AND " + whereClause : string.Empty);
			return new ObjectQuery(objectType, selectKey, sortClause);
		}

		public ObjectQuery ManyQuery(Type objectType, string[] manyField, string manyTable,
			string[] parentField, string[] childField, object[] dataValue, string whereClause, string sortClause)
		{
			StringBuilder retval = new StringBuilder();
			for (int index = 0; index < dataValue.Length; index++) {
				if (index > 0) retval.Append(" AND ");
				retval.Append(this.GetDelimited(this.entity.Table, manyField[index]));
				retval.Append(" = ");
				retval.Append(this.GetDelimited(manyTable, childField[index]));
				retval.Append(" AND ");
				retval.Append(this.GetExpression(manyTable, parentField[index], dataValue[index]));
			}
			retval.Append(whereClause != null && whereClause.Length > 0 ? " AND " + whereClause : string.Empty);
			return new ObjectQuery(objectType, retval.ToString(), sortClause, manyTable);
		}

		public string Select(ObjectQuery objectQuery) {
			string selectClause = this.select;
			if (objectQuery.manyTable != null && objectQuery.manyTable.Length > 0) {
				selectClause += ", " + this.GetDelimited(objectQuery.manyTable);
			}
			return this.Select(selectClause, objectQuery);
		}

		public string Select(ObjectQuery objectQuery, string[] selectFields) {
			return this.Select(this.CreateSelect(selectFields), objectQuery);
		}

		virtual protected string Select(string selectClause, ObjectQuery objectQuery) {
			string where = null;
			if (this.entity.BaseEntity == null) {
				where = (objectQuery.WhereClause.Length == 0
					? String.Empty : " WHERE " + objectQuery.WhereClause);
			}
			else {
				string filter = " WHERE " + this.GetExpression(this.entity.Table, this.entity.TypeField, this.entity.TypeValue);
				where = filter + (objectQuery.WhereClause.Length == 0
					? string.Empty : " AND " + objectQuery.WhereClause);
			}
			string sort = (objectQuery.SortClause.Length == 0
				? String.Empty : " ORDER BY " + objectQuery.SortClause);
			if (objectQuery.PageSize <= 0) {
				return selectClause + where + sort + TS;
			}
			else if (this.provider.SelectPageQuery != null && this.provider.SelectPageQuery.Length > 0) {
				int prevCount = objectQuery.PageSize * (objectQuery.PageIndex - 1);
				string query = this.provider.SelectPageQuery.Replace("*",
					selectClause.Remove(0, 6) + where + sort);
				return string.Format(query, objectQuery.PageSize, prevCount, prevCount + 1);
			}
			else {
				throw new ORMapperException("ObjectSpace: Generic Paging is Not Supported");
			}
		}

		public string RecordCount(string whereClause) {
			string query = null;
			if (this.entity.BaseEntity == null) {
				query = (whereClause.Length == 0 ? String.Empty : " WHERE " + whereClause);
			}
			else {
				string filter = " WHERE " + this.GetExpression(this.entity.Table, this.entity.TypeField, this.entity.TypeValue);
				query = filter + (whereClause.Length == 0 ? String.Empty : " AND " + whereClause);
			}
			return "SELECT COUNT(*) AS RecordCount FROM " + this.fromTables + query + TS;
		}
		
		internal Commands(EntityMap entity, CustomProvider provider) {
			this.QS = provider.StartDelimiter;
			this.QE = provider.EndDelimiter;
			this.DD = provider.DateDelimiter;
			this.GD = provider.GuidDelimiter;
			this.TS = provider.LineTerminator;
			this.entity = entity;
			this.provider = provider;

			this.selectFields = this.CreateFields();
			this.select = this.CreateSelect();
#if DEBUG_MAPPER
			Debug.WriteLine("  " + this.select);
#endif
			if (!this.entity.ReadOnly) {
				this.insert = this.CreateInsert();
				this.update = this.CreateUpdate();
				this.delete = this.CreateDelete();
#if DEBUG_MAPPER
				Debug.WriteLine("  " + this.insert);
				Debug.WriteLine("  " + this.update);
				Debug.WriteLine("  " + this.delete);
#endif
			}
		}

		private string CreateFields() {
			StringBuilder fieldList = new StringBuilder();
			StringBuilder fromList = new StringBuilder();
			Hashtable lookupTables = new Hashtable();
			fromList.Append(this.GetDelimited(this.entity.Table));
			for (int index = 0; index < this.entity.FieldCount; index++) {
				this.AddSelectField(fieldList, fromList, lookupTables, this.entity[index]);
			}
			foreach (FieldMap subField in this.entity.SubFields.Values) {
				this.AddSelectField(fieldList, fromList, lookupTables, subField);
			}
			if (this.entity.TypeField != null && this.entity.TypeField.Length > 0) {
				fieldList.Append(this.GetDelimited(this.entity.Table, this.entity.TypeField) + ", ");
			}
			this.fromTables = fromList.ToString().PadLeft(fromList.Length + lookupTables.Count, '(');
			if (fieldList.Length >= 2) fieldList.Remove(fieldList.Length - 2, 2);
			return fieldList.ToString();
		}

		private void AddSelectField(StringBuilder fieldList, StringBuilder fromList, Hashtable lookupTables, FieldMap field) {
			if (!field.IsLookup) {
				fieldList.Append(this.GetDelimited(this.entity.Table, field.Field) + ", ");
			}
			else {
				LookupMap lookup = field as LookupMap;
				string lookupKey = string.Format("{0};{1};{2}", lookup.Table.ToUpper(),
					string.Join(",", lookup.Source).ToUpper(), string.Join(",", lookup.Dest).ToUpper());
				if (!lookupTables.ContainsKey(lookupKey)) {
					lookup.tableAlias = lookup.Table + "_" + (lookupTables.Count + 1).ToString();
					lookupTables.Add(lookupKey, lookup.tableAlias);
					StringBuilder join = new StringBuilder();
					join.AppendFormat(" LEFT JOIN {0} {1} {2} ON", this.GetDelimited(lookup.Table),
							this.provider.ColumnAliasKeyword, this.GetDelimited(lookup.TableAlias));
					for (int fkey = 0; fkey < lookup.Source.Length; fkey++) {
						join.AppendFormat(" {0} = {1} AND", this.GetDelimited(this.entity.Table, lookup.Source[fkey]),
							this.GetDelimited(lookup.TableAlias, lookup.Dest[fkey]));
					}
					fromList.Append(join.Remove(join.Length - 4, 4).ToString() + ")");
				}
				else {
					lookup.tableAlias = (string)lookupTables[lookupKey];
				}
				fieldList.AppendFormat("{0} {1} {2}, ", this.GetDelimited(lookup.TableAlias, lookup.Field),
					this.provider.ColumnAliasKeyword, this.GetDelimited(lookup.FieldAlias));
			}
		}

		private string CreateSelect(string[] selectFields) {
			StringBuilder fieldList = new StringBuilder();
			for (int index = 0; index < selectFields.Length; index++) {
				fieldList.Append(selectFields[index] + ", ");
			}
			if (this.entity.TypeField != null && this.entity.TypeField.Length > 0) {
				fieldList.Append(this.GetDelimited(this.entity.TypeField) + ", ");
			}
			if (fieldList.Length >= 2) fieldList.Remove(fieldList.Length - 2, 2);
			return "SELECT " + fieldList.ToString() + " FROM " + this.fromTables;
		}

		private string CreateSelect() {
			return string.Format("SELECT {0} FROM {1}", this.selectFields, this.fromTables);
		}

		private string CreateInsert() {
			if (this.entity.InsertSP.Length == 0) {
				EntityMap map = this.entity;
				StringBuilder fieldList = new StringBuilder();
				StringBuilder parameterList = new StringBuilder();
				for (int index = 0; index < map.FieldCount; index++) {
					FieldMap field = map[index];

					if (field.PersistType == PersistType.Persist) {
						if (map.KeyType == KeyType.Auto) {
							if (map.IsAutoKeyMember(field.Member)) {
								// skip the final key segment of an auto key
								continue;
							}
						}
						fieldList.Append(this.GetDelimited(this.entity[index].Field) + ", ");
						parameterList.Append(this.entity[index].Parameter + ", ");
					}
				}
				if (map.TypeField != null && map.TypeField.Length > 0
						&& map.TypeValue != null && map.TypeValue.ToString().Length > 0) {
					fieldList.Append(this.GetDelimited(this.entity.TypeField) + ", ");
					parameterList.Append(this.CleanValue(this.entity.TypeValue) + ", ");
				}
				if (fieldList.Length >= 2) fieldList.Remove(fieldList.Length - 2, 2);
				if (parameterList.Length >= 2) parameterList.Remove(parameterList.Length - 2, 2);
				string identity = (this.entity.KeyType == KeyType.Auto ? this.SelectIdentity() : TS);
				return String.Format("INSERT INTO {0} ({1}) VALUES ({2}){3}",
					this.GetDelimited(this.entity.Table), fieldList.ToString(), parameterList.ToString(), identity);
			}
			else {
				return this.entity.InsertSP;
			}
		}

		virtual protected string SelectIdentity() {
			string keyField = this.entity.AutoKeyMember().Field;
			return string.Format(TS + Connection.SplitToken + this.provider.IdentityQuery + TS,
				keyField, this.entity.Table);
		}

		public string CreateUpdate(string whereClause, string updateClause) {
			return String.Format("UPDATE {0} SET {1} WHERE {2}{3}",
				this.GetDelimited(this.entity.Table), updateClause, whereClause, TS);
		}

		public string CreateUpdate(FieldMap[] fieldMaps) {
			StringBuilder fieldList = new StringBuilder();
			for (int index = 0; index < fieldMaps.Length; index++) {
				fieldList.Append(this.GetDelimited(fieldMaps[index].Field) + " = "
						+ fieldMaps[index].Parameter + ", ");
			}
			if (fieldList.Length >= 2) fieldList.Remove(fieldList.Length - 2, 2);
			return this.updatePreSet + fieldList.ToString() + this.updatePostSet;
		}

		private string GetKeyWhereClause() {
			StringBuilder retval = new StringBuilder();
			FieldMap[] keyFields = this.entity.KeyFields;
			for (int index = 0; index < keyFields.Length; index++) {
				if (index > 0) retval.Append(" AND ");
				retval.Append(String.Format("{0} = {1}",
					this.GetDelimited(this.entity.Table, keyFields[index].Field), keyFields[index].Parameter));
			}
			return retval.ToString();
		}

		private string CreateUpdate() {
			if (this.entity.UpdateSP.Length == 0) {
				StringBuilder fieldList = new StringBuilder();
				StringBuilder concurrentList = new StringBuilder();
				for (int index = 0; index < this.entity.FieldCount; index++) {
					if (this.entity[index].PersistType != PersistType.ReadOnly
							&& ! this.entity.IsKeyMember(this.entity[index].Member)) {
						if (this.entity[index].PersistType == PersistType.Concurrent) {
							concurrentList.Append(" AND " + this.GetDelimited(this.entity[index].Field) + " = "
									+ this.entity[index].Parameter);
						}
						else { // PersistType.Persist
							fieldList.Append(this.GetDelimited(this.entity[index].Field) + " = "
								+ this.entity[index].Parameter + ", ");
						}
					}
				}
				if (fieldList.Length >= 2) fieldList.Remove(fieldList.Length - 2, 2);

				this.updatePreSet = String.Format("UPDATE {0} SET ", this.GetDelimited(this.entity.Table));
				this.updatePostSet = String.Format(" WHERE {0}{1}{2}",
					this.GetKeyWhereClause(), concurrentList.ToString(), TS);
				return this.updatePreSet + fieldList.ToString() + this.updatePostSet;
			}
			else {
				return this.entity.UpdateSP;
			}
		}

		public string CreateDelete(string whereClause) {
			return this.CreateDelete(this.entity.Table, whereClause);
		}

		public string CreateDelete(string tableName, string whereClause) {
			return String.Format("DELETE FROM {0} WHERE {1}{2}",
				this.GetDelimited(tableName), whereClause, TS);
		}

		private string CreateDelete() {
			if (this.entity.DeleteSP.Length == 0) {
				StringBuilder concurrentList = new StringBuilder();
				for (int index = 0; index < this.entity.FieldCount; index++) {
					if (this.entity[index].PersistType == PersistType.Concurrent
							&& ! this.entity.IsKeyMember(this.entity[index].Member)) {
						concurrentList.Append(" AND " + this.GetDelimited(this.entity[index].Field) + " = "
								+ this.entity[index].Parameter);
					}
				}
				return String.Format("DELETE FROM {0} WHERE {1}{2}{3}",
					this.GetDelimited(this.entity.Table), this.GetKeyWhereClause(), concurrentList.ToString(), TS);
			}
			else {
				return this.entity.DeleteSP;
			}
		}

		public string InsertMany(string tableName, string[] parentField, string[] childField,
				object[] parentValue, object[] childValue) {
			StringBuilder parentFields = new StringBuilder();
			StringBuilder parentValues = new StringBuilder();
			StringBuilder childFields = new StringBuilder();
			StringBuilder childValues = new StringBuilder();
			for (int index = 0; index < parentField.Length; index++) {
				if (index > 0) {
					parentFields.Append(", ");
					parentValues.Append(", ");
					childFields.Append(", ");
					childValues.Append(", ");
				}
				parentFields.Append(this.GetDelimited(parentField[index]));
				parentValues.Append(this.CleanValue(parentValue[index]));
				childFields.Append(this.GetDelimited(childField[index]));
				childValues.Append(this.CleanValue(childValue[index]));
			}
			return String.Format("INSERT INTO {0} ({1}, {2}) VALUES ({3}, {4}){5}",
				this.GetDelimited(tableName), parentFields.ToString(), childFields.ToString(),
				parentValues.ToString(), childValues.ToString(), TS);
		}
	}
}
