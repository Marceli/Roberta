//-------------------------------------------------
// OPath Query Engine
// Written by Jeff Lanning (jefflanning@gmail.com)
// Modeled after SDK for Longhorn CTP Build 4074
// Version 1: Dec 2004 - May 2005
//-------------------------------------------------
using System;
using System.Collections;

namespace Wilson.ORMapper.Query
{
	internal class OrderByJoinCollection : CollectionBase
	{
		internal OrderByJoinCollection()
		{
		}

		internal OrderByJoinCollection(OrderByJoin[] items)
		{
			this.InnerList.AddRange(items);
		}


		public OrderByJoin this[int index]
		{
			get { return (OrderByJoin)this.InnerList[index]; }
		}

		public OrderByJoin this[string name]
		{
			get
			{
				int index = this.IndexOf(name);
				return (index >= 0) ? (OrderByJoin)this.InnerList[index] : null;
			}
		}


		public int Add(OrderByJoin value)
		{
			if( this.Contains(value.Name) )
			{
				throw new ArgumentException("Object with same name already exists in collection.", "value");
			}
			return this.InnerList.Add(value);
		}

		internal OrderByJoinCollection Clone()
		{
			OrderByJoinCollection clone = new OrderByJoinCollection();
			foreach( OrderByJoin item in this.InnerList )
			{
				clone.Add(item.Clone());
			}
			return clone;
		}

		public bool Contains(string name)
		{
			return (this.IndexOf(name) >= 0);
		}

		public void CopyTo(OrderByJoin[] array, int index)
		{
			this.CopyTo(array, index);
		}
 
		public int IndexOf(string name)
		{
			for( int i = this.InnerList.Count - 1; i >= 0; i-- )
			{
				if( (this.InnerList[i] as OrderByJoin).Name == name )
				{
					return i;
				}
			}
			return -1;
		}

		public void Insert(int index, OrderByJoin value)
		{
			this.InnerList.Insert(index, value);
		}

		public void Remove(OrderByJoin value)
		{
			this.InnerList.Remove(value);
		}
	}
}
