//**************************************************************//
// Paul Wilson -- www.WilsonDotNet.com -- Paul@WilsonDotNet.com //
// Feel free to use and modify -- just leave these credit lines //
// I also always appreciate any other public credit you provide //
//**************************************************************//
using System;

namespace Wilson.ORMapper.Internals
{
	// <attribute member="memberName" field="fieldName" />
	internal abstract class AttributeMap
	{
		private string member;
		private string field;

		public string Member {
			get { return this.member; }
		}

		public string Field {
			get { return this.field; }
		}

		internal AttributeMap(string member, string field) {
			if (member == null || member.Length == 0) {
				throw new MappingException("Mapping: Entity member was Missing");
			}
			if (field == null || field.Length == 0) {
				throw new MappingException("Mapping: Entity field was Missing - " + member);
			}
			
			this.member = member;
			this.field = field;
		}
	}
}
