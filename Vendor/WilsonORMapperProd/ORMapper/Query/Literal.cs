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
	internal class Literal : Expression
	{
		private object _value;
		private LiteralType _literalType;

		public Literal(object value)
		{
			this.Value = value;
		}

		public override NodeType NodeType
		{
			get { return NodeType.Literal; }
		}

		public object Value
		{
			get { return _value; }
			set
			{
				if( value == null ) throw new ArgumentException("");
				_value = value;
				_literalType = GetLiteralType(_value.GetType());
			}
		}

		public LiteralType LiteralType
		{
			get { return _literalType; }
		}

		public override bool IsArithmetic()
		{
			return true; //V2: should strings really be "Arithmetic"?
		}

		public override bool IsBoolean()
		{
			return (_value is bool);
		}

		public override bool IsFilter()
		{
			return false;
		}

		public override Type ValueType
		{
			get { return _value.GetType(); }
		}


		public override object Clone()
		{
			return new Literal(_value);
		}

		public override void WriteXml(XmlWriter xmlw)
		{
			xmlw.WriteStartElement(this.GetType().Name);
			xmlw.WriteAttributeString("Value", _value.ToString());
			xmlw.WriteAttributeString("ValueType", this.ValueType.ToString());
			xmlw.WriteAttributeString("LiteralType", this.LiteralType.ToString());
			xmlw.WriteEndElement();
		}

		public override string ToString()
		{
			return _value.ToString();
		}

		private LiteralType GetLiteralType(Type type)
		{
			if( type == typeof(string) ) return LiteralType.String;
			if( type == typeof(sbyte) ) return LiteralType.SByte;
			if( type == typeof(byte) ) return LiteralType.Byte;
			if( type == typeof(short) ) return LiteralType.Int16;
			if( type == typeof(int) ) return LiteralType.Int32;
			if( type == typeof(long) ) return LiteralType.Int64;
			if( type == typeof(ushort) ) return LiteralType.UInt16;
			if( type == typeof(uint) ) return LiteralType.UInt32;
			if( type == typeof(ulong) ) return LiteralType.UInt64;
			if( type == typeof(char) ) return LiteralType.Char;
			if( type == typeof(float) ) return LiteralType.Single;
			if( type == typeof(double) ) return LiteralType.Double;
			if( type == typeof(bool) ) return LiteralType.Boolean;
			if( type == typeof(decimal) ) return LiteralType.Decimal;
			if( type == typeof(Guid) ) return LiteralType.Guid;
			if( type == typeof(DateTime) ) return LiteralType.DateTime;
			if( type == typeof(TimeSpan) ) return LiteralType.TimeSpan;

			throw new ArgumentException("Could not convert type '" + type + "' a LiteralType enum value.", "type");
		}
	}

	internal enum LiteralType
	{
		String,
		SByte,
		Byte,
		Int16,
		Int32,
		Int64,
		UInt16,
		UInt32,
		UInt64,
		Char,
		Single,
		Double,
		Boolean,
		Decimal,
		Guid,
		DateTime,
		TimeSpan,
	}
}
