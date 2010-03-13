//-------------------------------------------------
// OPath Query Engine
// Written by Jeff Lanning (jefflanning@gmail.com)
// Modeled after SDK for Longhorn CTP Build 4074
// Version 1: Dec 2004 - May 2005
//-------------------------------------------------
using System;
using System.IO;
using System.Xml;

using Wilson.ORMapper.Internals;

namespace Wilson.ORMapper.Query
{
	internal class Property : Expression
	{
		private string _name;
		private Expression _source;
		private Type _ownerClass;
		private Type _propertyType;
		private bool _isRelational;
		private RelationMap _relationMap;

		public Property(string name, Expression source)
		{
			this.Name = name;
			this.Source = source;
		}

		public override NodeType NodeType
		{
			get { return NodeType.Property; }
		}

		public string Name
		{
			get { return _name; }
			set
			{
				if( value == null ) throw new ArgumentNullException("value");
				if( value.Length == 0 ) throw new ArgumentException("Property must be at least one character in length.", "value");
				_name = value;
			}
		}

		public Expression Source
		{
			get { return _source; }
			set
			{
				if( value == null ) throw new ArgumentNullException("value");
				_source = value;
				_source.Parent = this;
			}
		}
        
		internal Type OwnerClass
		{
			get	{ return _ownerClass; }
			set { _ownerClass = value; }
		}

		public Type PropertyType
		{
			get { return _propertyType; }
			set { _propertyType = value; }
		}

		public bool IsRelational
		{
			get { return _isRelational; }
			set { _isRelational = value; }
		}

		public RelationMap RelationMap
		{
			get { return _relationMap; }
			set { _relationMap = value; }
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
			return (_propertyType == typeof(bool));
		}

		public override bool IsFilter()
		{
			return true;
		}

		public override Type ValueType
		{
			get { return _propertyType; }
		}

		public override object Clone()
		{
			return new Property(_name, (Expression)_source.Clone());
		}

		public override void WriteXml(XmlWriter xmlw)
		{
			xmlw.WriteStartElement(this.GetType().Name);
			xmlw.WriteAttributeString("Name", _name);
			xmlw.WriteAttributeString("IsRelational", _isRelational.ToString());
			xmlw.WriteAttributeString("PropertyType", (_propertyType != null) ? _propertyType.Name : "null");
			xmlw.WriteAttributeString("OwnerClass", (_ownerClass != null) ? _ownerClass.Name : "null");
			
			xmlw.WriteStartElement("Source");
			_source.WriteXml(xmlw);
			xmlw.WriteEndElement();

			xmlw.WriteEndElement();
		}

		public override string ToString()
		{
			return string.Format("{0}", _name);
		}
	}
}
