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
	internal class Filter : Expression
	{
		private Expression _source;
		private Expression _constraint;
		private string _alias;

		public Filter(Axis axis) : this(axis.Source, axis.Constraint)
		{
		}

		public Filter(Expression source, Expression constraint)
		{
			this.Source = source;
			this.Constraint = constraint;
		}

		public override NodeType NodeType
		{
			get { return NodeType.Filter; }
		}

		public Expression Source
		{
			get { return _source; }
			set
			{
				if( value == null ) throw new ArgumentNullException("value");
				
				// the source node better be filtering
				if( !value.IsFilter() )
				{
					throw new Exception("Source expression must be filtering.");
				}

				_source = value;
				_source.Parent = this;
			}
		}

		public Expression Constraint
		{
			get { return _constraint; }
			set
			{
				if( value == null ) throw new ArgumentNullException("value");

				_constraint = value;
				_constraint.Parent = this;
			}
		}

		public string Alias
		{
			get { return _alias; }
			set { _alias = value; }
		}

		public override bool IsConst
		{
			get { return false; }
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

		public override Type ValueType
		{
			get { return _source.ValueType; }
		}


		public override object Clone()
		{
			return new Filter((Expression)_source.Clone(), (Expression)_constraint.Clone());
		}

		public override void WriteXml(XmlWriter xmlw)
		{
			xmlw.WriteStartElement(this.GetType().Name);

			xmlw.WriteStartElement("Source");
			_source.WriteXml(xmlw);
			xmlw.WriteEndElement();

			xmlw.WriteStartElement("Constraint");
			_constraint.WriteXml(xmlw);
			xmlw.WriteEndElement();


			xmlw.WriteEndElement();
		}

		public override string ToString()
		{
			return "FILTER(" + _constraint.ToString() + ")";
		}
	}
}
