//-------------------------------------------------
// OPath Query Engine
// Written by Jeff Lanning (jefflanning@gmail.com)
// Modeled after SDK for Longhorn CTP Build 4074
// Version 1: Dec 2004 - May 2005
//-------------------------------------------------
using System;
using System.IO;
using System.Xml;

namespace Wilson.ORMapper.Query
{
	internal class OrderBy : Expression
	{
		private Expression _source;
		private OrderByItemCollection _items;
		private OrderByJoinCollection _joins;

		public OrderBy(Expression source, OrderByItemCollection orderByItems, OrderByJoinCollection orderByJoins)
		{
			this.Source = source;
			_items = orderByItems;
			_joins = orderByJoins;
		}

		public override NodeType NodeType
		{
			get { return NodeType.OrderBy; }
		}

		public Expression Source
		{
			get { return _source; }
			set { _source = value; }
		}

		public OrderByItemCollection OrderByItems
		{
			get { return _items; }
		}
		
		public OrderByJoinCollection OrderByJoins
		{
			get { return _joins; }
		}

		public override bool IsArithmetic()
		{
			return false;
		}

		public override bool IsBoolean()
		{
			return false;
		}

		public override bool IsFilter()
		{
			return false;
		}

		public override Type ValueType
		{
			get { return _source.ValueType; }
		}



		public override object Clone()
		{
			return new OrderBy((Expression)_source.Clone(), (OrderByItemCollection)_items.Clone(), (OrderByJoinCollection)_joins.Clone());
		}

		public override void WriteXml(XmlWriter xmlw)
		{
			xmlw.WriteStartElement(this.GetType().Name);

			xmlw.WriteStartElement("Source");
			_source.WriteXml(xmlw);
			xmlw.WriteEndElement();

			xmlw.WriteStartElement("OrderByItems");
			foreach( OrderByItem item in _items )
			{
				xmlw.WriteStartElement("Item");
				xmlw.WriteAttributeString("Property", item.Item);
				xmlw.WriteAttributeString("Ascending", item.Ascending.ToString());
				xmlw.WriteEndElement();
			}
			xmlw.WriteEndElement();
			
			xmlw.WriteEndElement();
		}

		public override string ToString()
		{
			string sortList = null;
			for( int i = 0; i < _items.Count; i++ )
			{
				if( i > 0 ) sortList += ", ";
				sortList += _items[i].Item + " " + (_items[i].Ascending ? "ASC" : "DESC");
			}
			return string.Format("ORDER BY ({1})", sortList);
		}
	}

	internal enum OrderByDirection
	{
		Ascending = 0,
		Descending = 1
	}
}