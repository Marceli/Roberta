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
	internal class Parameter : Expression
	{
		private int _ordinal;

		internal Type inferredType;

		public Parameter(int ordinal)
		{
			this._ordinal = ordinal;
			this.inferredType = null;
		}

		public override NodeType NodeType
		{
			get { return NodeType.Parameter; }
		}

		public int Ordinal
		{
			get { return _ordinal; }
		}

		public override bool IsConst
		{
			get { return true; }
		}

		public override bool IsArithmetic()
		{
			return true;
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
			get { return this.inferredType; }
		}


		public override object Clone()
		{
			return new Parameter(_ordinal);
		}

		public override void WriteXml(XmlWriter xmlw)
		{
			xmlw.WriteStartElement(this.GetType().Name);
			xmlw.WriteAttributeString("Ordinal", _ordinal.ToString());
			xmlw.WriteEndElement();
		}

		public override string ToString()
		{
			return "@P" + _ordinal;
		}
	}
}
