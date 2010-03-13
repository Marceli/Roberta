//**************************************************************//
// Paul Wilson -- www.WilsonDotNet.com -- Paul@WilsonDotNet.com //
// Feel free to use and modify -- just leave these credit lines //
// I also always appreciate any other public credit you provide //
//**************************************************************//
// Includes Support for Composite Primary Keys from Jerry Shea (http://www.RenewTek.com)
using System;
using System.Text;

namespace Wilson.ORMapper.Internals
{
	internal class OracleCommands : Commands
	{
		internal OracleCommands(EntityMap entity, CustomProvider provider) : base(entity, provider)	{}

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
				return selectClause + where + sort;
			}
			else if (objectQuery.PageIndex <= 1) {
				string format = "SELECT * FROM (SELECT {0} FROM {1}{2}{3}) {4} WHERE RowNum <= {5}"; //NPF modified for oracle
				return String.Format(format, this.selectFields, this.fromTables,
					where, sort, this.GetDelimited(this.entity.Table), objectQuery.PageSize);
			}
			else {
				int totalCount = objectQuery.PageSize * objectQuery.PageIndex;
				int prevCount = objectQuery.PageSize * (objectQuery.PageIndex - 1);
				string format = "SELECT * FROM (SELECT *, RowNum AS {0} FROM ("	//NPF modified for oracle
					+ "SELECT {1} FROM {2}{3}{4}) {5} WHERE RowNum <= {6}) {5} WHERE {0} > {7}";
				return String.Format(format, "RowIndex", this.selectFields, this.fromTables,
					where, sort, this.GetDelimited(this.entity.Table), totalCount, prevCount);
			}
		}

		override protected string SelectIdentity() {
			return " RETURNING " + this.GetDelimited(this.entity.AutoKeyMember().Field) + " INTO :KeyField"; // only called for auto
		}
	}
}
