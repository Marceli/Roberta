//**************************************************************//
// Paul Wilson -- www.WilsonDotNet.com -- Paul@WilsonDotNet.com //
// Feel free to use and modify -- just leave these credit lines //
// I also always appreciate any other public credit you provide //
//**************************************************************//
using System;
using System.Text;

namespace Wilson.ORMapper.Internals
{
	internal class Sql2005Commands : Commands
	{
		internal Sql2005Commands(EntityMap entity, CustomProvider provider) : base(entity, provider) {}

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
				if (sort.Length == 0)
					throw new ORMapperException("ObjectSpace: Paging requires a Sort Clause");
				int totalCount = objectQuery.PageSize * objectQuery.PageIndex;
				int prevCount = objectQuery.PageSize * (objectQuery.PageIndex - 1);
				string format = "SELECT * FROM (SELECT TOP {0} {1}, Row_Number()"
						+ " OVER ({2}) AS {3} FROM {4}{5}) AS {6} WHERE {3} > {7};";
				return string.Format(format, totalCount, this.selectFields, sort, "RowIndex",
					this.fromTables, where, this.GetDelimited(this.entity.Table), prevCount);
			}
		}

		override protected string SelectIdentity() {
			return "; SELECT KeyField = SCOPE_IDENTITY();";
		}
	}
}
