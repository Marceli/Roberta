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
	internal class OrderByItemCollection : CollectionBase
	{
		internal OrderByItemCollection()
		{
		}

		internal OrderByItemCollection(OrderByItem[] items)
		{
			this.InnerList.AddRange(items);
		}

		public OrderByItem this[int index]
		{
			get { return (OrderByItem)this.InnerList[index]; }
			set
			{
				if( value == null ) throw new ArgumentNullException("value");
				this.InnerList[index] = value;
			}
		}

		public int Add(OrderByItem value)
		{
			return this.InnerList.Add(value);
		}

		internal OrderByItemCollection Clone()
		{
			OrderByItemCollection result = new OrderByItemCollection();
			foreach( OrderByItem item in this.InnerList )
			{
				result.Add(item); // clone item?
			}
			return result;
		}

		public bool Contains(OrderByItem value)
		{
			return this.InnerList.Contains(value);
		}

		public void CopyTo(OrderByItem[] array, int index)
		{
			this.CopyTo(array, index);
		}
 
		public int IndexOf(OrderByItem value)
		{
			return this.InnerList.IndexOf(value);
		}

		public void Insert(int index, OrderByItem value)
		{
			this.InnerList.Insert(index, value);
		}

		public void Remove(OrderByItem value)
		{
			this.InnerList.Remove(value);
		}
	}
}
