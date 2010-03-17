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
	internal class OrderByItem
	{
		private string _item;
		private bool _ascending;
		private FieldMap _fieldInfo;
		private OrderByJoin _join;
		//private object _physicalInfo;

		public OrderByItem(string item, FieldMap fieldInfo, bool ascending, OrderByJoin join)
		{
			this.Item = item;
			this.FieldInfo = fieldInfo;
			this.Ascending = ascending;
			this.Join = join;
		}
	
		public string Item
		{
			get { return _item; }
			set { _item = value; }
		}

		public FieldMap FieldInfo
		{
			get { return _fieldInfo; }
			set { _fieldInfo = value; }
		}

		public bool Ascending
		{
			get { return _ascending; }
			set { _ascending = value; }
		}

		public OrderByJoin Join
		{
			get { return _join; }
			set { _join = value; }
		}

		//NOTE: this property is not used with our simplified sort parser
		//public object PhysicalInfo
		//{
		//	get { return _physicalInfo; }
		//	set { _physicalInfo = value; }
		//}
	}
}