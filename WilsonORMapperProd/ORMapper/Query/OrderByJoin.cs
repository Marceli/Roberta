//-------------------------------------------------
// OPath Query Engine
// Written by Jeff Lanning (jefflanning@gmail.com)
// Modeled after SDK for Longhorn CTP Build 4074
// Version 1: Dec 2004 - May 2005
//-------------------------------------------------
using System;
using Wilson.ORMapper.Internals;

namespace Wilson.ORMapper.Query
{
	internal class OrderByJoin
	{
		private RelationMap _relationMap;
		private OrderByJoinCollection _nestedJoins = new OrderByJoinCollection();
		private string _alias = null;

		public OrderByJoin(RelationMap relationMap)
		{
			this.RelationMap = relationMap;
		}

		public RelationMap RelationMap
		{
			get { return _relationMap; }
			set
			{
				if( value == null ) throw new ArgumentNullException("value");
				_relationMap = value;
			}
		}

		public string Name
		{
			get { return _relationMap.Alias; }
		}

		public string Alias
		{
			get { return _alias; }
			set { _alias = value; }
		}

		public OrderByJoinCollection NestedJoins
		{
			get { return _nestedJoins; }
		}


		internal OrderByJoin Clone()
		{
			OrderByJoin clone = new OrderByJoin(_relationMap);
			clone._alias = _alias;
			clone._nestedJoins = _nestedJoins.Clone();
			return clone;
		}
	}
}