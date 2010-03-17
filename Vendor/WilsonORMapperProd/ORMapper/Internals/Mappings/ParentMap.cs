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
	// <relation relationship="ManyToOne" member="memberName" field="fieldName"
	//		type="typeName" [alias="aliasName"] [queryOnly="bool"] [lazyLoad="bool"]
	//    [cascadeDelete="bool"] [filter="whereClause"] [selectSP="selectSPName"] />
	// Note: selectSP Parameters - field
	internal class ParentMap : RelationMap
	{
		internal ParentMap(string member, string field, string type, string alias, bool queryOnly, bool lazy, bool cascade, string filter, string selectSP, CustomProvider provider)
			: base(Relationship.Parent, member, field, type, alias, queryOnly, lazy, cascade, filter, selectSP, provider) {}
	}
}
