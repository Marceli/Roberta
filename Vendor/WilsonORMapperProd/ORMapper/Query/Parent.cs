//-------------------------------------------------
// OPath Query Engine
// Written by Jeff Lanning (jefflanning@gmail.com)
// Modeled after SDK for Longhorn CTP Build 4074
// Version 1: Dec 2004 - May 2005
//-------------------------------------------------
using System;
using System.Xml;

namespace Wilson.ORMapper.Query
{
	internal class Parent : Context
	{
		private Context _source;

		public Parent(Context source)
		{
			this.Source = source;
		}

		public override NodeType NodeType
		{
			get { return NodeType.Parent; }
		}

		public int Level
		{
			get
			{
				if( _source.NodeType != NodeType.Parent )
				{
					return 1;
				}
				else // parent source
				{
					return ((_source as Parent).Level + 1);
				}
			}
		}

		public Context Source
		{
			get { return _source; }
			set
			{
				if( value == null ) throw new ArgumentNullException("value");
				_source = value;
				_source.Parent = this;
			}
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
			return true;
		}

		public override object Clone()
		{
			return new Parent((Context)_source.Clone());
		}

		public override void WriteXml(XmlWriter xmlw)
		{
			xmlw.WriteStartElement(this.GetType().Name);
			xmlw.WriteAttributeString("Level", this.Level.ToString());

			xmlw.WriteStartElement("Source");
			_source.WriteXml(xmlw);
			xmlw.WriteEndElement();

			xmlw.WriteEndElement();
		}

		public override string ToString()
		{
			return "^.";
		}
	}
}