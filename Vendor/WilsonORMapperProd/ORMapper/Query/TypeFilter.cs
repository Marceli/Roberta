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
	internal class TypeFilter : Expression
	{
		private Expression _source;
		private Type _type;
		private bool _baseTypeOnly;

		public TypeFilter(Expression source, Type type) : this(source, type, false)
		{
		}

		public TypeFilter(Expression source, Type type, bool baseTypeOnly)
		{
			this.Source = source;
			this.Type = type;
			this.BaseTypeOnly = baseTypeOnly;
		}

		public override NodeType NodeType
		{
			get { return NodeType.TypeFilter; }
		}

		public bool BaseTypeOnly
		{
			get { return _baseTypeOnly; }
			set { _baseTypeOnly = value; }
		}

		public Expression Source
		{
			get { return _source; }
			set
			{
				if( value == null ) throw new ArgumentNullException("value");
				
				// expression better be a filtering type
				if( !value.IsFilter() )
				{
					throw new Exception("Source expression must be filtering.");
				}

				_source = value;
				_source.Parent = this;
			}
		}

		public Type Type
		{
			get { return _type; }
			set
			{
				if( value == null ) throw new ArgumentNullException("value");
				_type = value;
			}
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
			get { return _type; }
		}


		public override object Clone()
		{
			return new TypeFilter((Expression)_source.Clone(), _type);
		}

		public override void WriteXml(XmlWriter xmlw)
		{
			xmlw.WriteStartElement(this.GetType().Name);
			xmlw.WriteAttributeString("Type", _type.Name);
			xmlw.WriteAttributeString("BaseTypeOnly", _baseTypeOnly.ToString());

			xmlw.WriteStartElement("Source");
			_source.WriteXml(xmlw);
			xmlw.WriteEndElement();

			xmlw.WriteEndElement();
		}

		public override string ToString()
		{
			return _type.ToString();
		}
	}
}
