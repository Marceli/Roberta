//**************************************************************//
// Paul Wilson -- www.WilsonDotNet.com -- Paul@WilsonDotNet.com //
// Feel free to use and modify -- just leave these credit lines //
// I also always appreciate any other public credit you provide //
//**************************************************************//
// CascadeDelete help from Ken Muse (http://www.MomentsFromImpact.com)
// SelectSP advice and code from Allan Ritchie (A.Ritchie@ACRWebSolutions.com)
using System;

namespace Wilson.ORMapper.Internals
{
	// <relation relationship="OneToMany" member="memberName" field="fieldName"
	//		type="typeName" [alias="aliasName"] [queryOnly="bool"] [lazyLoad="bool"]
	//    [cascadeDelete="bool"] [filter="whereClause"] [selectSP="selectSPName"] />
	// Note: selectSP Parameters - field
	//    cascadeDelete does not work with stored procs
	internal class ChildMap : RelationMap
	{
		internal ChildMap(string member, string field, string type, string alias, bool queryOnly, bool lazy, bool cascade, string filter, string selectSP, CustomProvider provider)
			: base(Relationship.Child, member, field, type, alias, queryOnly, lazy, cascade, filter, selectSP, provider) {}
	}
}
