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
	internal class Empty : Expression
	{
		public Empty()
		{
		}

		public override NodeType NodeType
		{
			get { return NodeType.Empty; }
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
			return new Empty();
		}

		public override void WriteXml(XmlWriter xmlw)
		{
			xmlw.WriteStartElement(this.GetType().Name);
			xmlw.WriteEndElement();
		}

		public override string ToString()
		{
			return null;
		}
	}
}
