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
	internal class Context : Expression
	{
		private Expression _link;

		public Context()
		{
		}

		public override NodeType NodeType
		{
			get { return NodeType.Context; }
		}

		public Expression Link
		{
			get { return _link; }
			set
			{
				if( value == null ) throw new ArgumentNullException("value");
				_link = value;
			}
		}

		public Expression SourceLink
		{
			get
			{
				//V2: What was MS doing with this property?
				throw new NotImplementedException("This property has not been implemented.");
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
			return new Context();
		}

		public override void WriteXml(XmlWriter xmlw)
		{
			xmlw.WriteStartElement(this.GetType().Name);

			xmlw.WriteStartElement("Link");
			if( _link != null ) _link.WriteXml(xmlw);
			xmlw.WriteEndElement();

			xmlw.WriteEndElement();
		}
	}
}
