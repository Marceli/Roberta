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
	internal class Axis : Filter
	{
		private bool _isDot;

		public Axis(Expression source, Expression constraint) : base(source, constraint)
		{
			this.IsDot = true;
		}

		public Axis(Expression source, Expression constraint, bool isDot) : this(source, constraint)
		{
			this.IsDot = isDot;
		}

		public override NodeType NodeType
		{
			get { return NodeType.Axis; }
		}

		public bool IsDot
		{
			get { return _isDot; }
			set { _isDot = value; }
		}

		public override bool IsConst
		{
			get { return false; }
		}

		public override bool IsArithmetic()
		{
			return true;
		}

		public override bool IsBoolean()
		{
			return true;
		}

		public override bool IsFilter()
		{
			return true;
		}


		public override object Clone()
		{
			return new Axis((Expression)base.Source.Clone(), (Expression)base.Constraint.Clone(), IsDot);
		}

		public override void WriteXml(XmlWriter xmlw)
		{
			xmlw.WriteStartElement(this.GetType().Name);
			xmlw.WriteAttributeString("IsDot", _isDot.ToString());

			xmlw.WriteStartElement("Source");
			base.Source.WriteXml(xmlw);
			xmlw.WriteEndElement();

			xmlw.WriteStartElement("Constraint");
			base.Constraint.WriteXml(xmlw);
			xmlw.WriteEndElement();

			xmlw.WriteEndElement();
		}

		public override string ToString()
		{
			return "AXIS(" + base.Constraint.ToString() + ")";
		}
	}
}

