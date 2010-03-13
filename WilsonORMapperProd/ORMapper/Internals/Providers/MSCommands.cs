//**************************************************************//
// Paul Wilson -- www.WilsonDotNet.com -- Paul@WilsonDotNet.com //
// Feel free to use and modify -- just leave these credit lines //
// I also always appreciate any other public credit you provide //
//**************************************************************//
using System;
using System.Text;

namespace Wilson.ORMapper.Internals
{
	internal class MSCommands : Commands
	{
		internal MSCommands(EntityMap entity, CustomProvider provider) : base(entity, provider) {}

		override protected string Select(string selectClause, ObjectQuery objectQuery) {
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
				return selectClause + where + sort + ";";
			}
			else if (objectQuery.PageIndex <= 1) {
				string top = selectClause.Insert(6, " TOP " + objectQuery.PageSize.ToString());
				return top + where + sort + ";";
			}
			else {
				int prevCount = objectQuery.PageSize * (objectQuery.PageIndex - 1);
				string keyField = this.entity.KeyFields[0].Field;
				if (this.entity.KeyFields.Length > 1)
					throw new ORMapperException("ObjectSpace: Composite Key Paging is Not Supported");
				string query = (objectQuery.WhereClause.Length == 0
					? String.Empty : objectQuery.WhereClause + " AND ");
				string format = "{0} WHERE {1} IN (SELECT TOP {2} {1} FROM {3} WHERE {4}{1}"
					+ " NOT IN (SELECT TOP {5} {1} FROM {3}{6}{7}){7}){7};";

                string fullKeyField = string.Format("[{0}].{1}", this.entity.Table, this.GetDelimited(keyField));

				return String.Format(format, selectClause, fullKeyField, objectQuery.PageSize.ToString(),
                    this.fromTables, query, prevCount.ToString(), where, sort);
			}
		}

		override protected string SelectIdentity() {
			if (this.provider.Provider == Provider.MsSql) {
				return "; SELECT KeyField = SCOPE_IDENTITY();";
			}
			else {
				return ";" + Connection.SplitToken + "SELECT @@IDENTITY AS KeyField;";
			}
		}
	}
}
