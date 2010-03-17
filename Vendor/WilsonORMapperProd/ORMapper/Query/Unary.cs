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
	internal class Unary : Expression
	{
		private UnaryOperator _op;
		private Expression _operand;

		public Unary(UnaryOperator op, Expression operand)
		{
			this.Operator = op;
			this.Operand = operand;
		}

		public override NodeType NodeType
		{
			get { return NodeType.Unary; }
		}

		public UnaryOperator Operator
		{
			get { return _op; }
			set { _op = value; }
		}

		public Expression Operand
		{
			get { return _operand; }
			set
			{
				if( value == null ) throw new ArgumentNullException("value");
				
				_operand = value;
				_operand.Parent = this;
			}
		}

		public override bool IsConst
		{
			get { return _operand.IsConst; }
		}

		public override bool IsArithmetic()
		{
			return IsOperatorArithmetic(_op);
		}

		public override bool IsBoolean()
		{
			return IsOperatorBoolean(_op);
		}

		public override bool IsFilter()
		{
			return true;
		}

		public override Type ValueType
		{
			get
			{
				switch( _op )
				{
					case UnaryOperator.Negation:
					{
						if( _operand.NodeType == NodeType.Parameter )
						{
							//V2: use inferredType var??? have to be set somewhere first?
							return typeof(decimal);
						}
						else
						{
							return _operand.ValueType;
						}
					}
					case UnaryOperator.LogicalNot:
					{
						if( _operand.ValueType != typeof(bool) )
						{
							throw new OPathException("Logical Not operator can only be applied to boolean expressions.");
						}
						return typeof(bool);
					}
					case UnaryOperator.IsNull:
					{
						return typeof(bool);
					}
					case UnaryOperator.Exists:
					{
						return typeof(bool);
					}
					default:
					{
						throw new NotSupportedException("Unary operator '" + _op + "' was not expected.");
					}
				}
			}
		}


		public override object Clone()
		{
			return new Unary(_op, (Expression)_operand.Clone());
		}

		public override void WriteXml(XmlWriter xmlw)
		{
			xmlw.WriteStartElement(this.GetType().Name);
			xmlw.WriteAttributeString("Operator", _op.ToString());

			xmlw.WriteStartElement("Operand");
			_operand.WriteXml(xmlw);
			xmlw.WriteEndElement();
			
			xmlw.WriteEndElement();
		}

		public override string ToString()
		{
			return string.Format("{0}({1})", _op, _operand);
		}

		internal static Type GetResultType(Expression leftExpr, Expression rightExpr, BinaryOperator op)
		{
			throw new NotImplementedException();
		}

		internal static bool IsOperatorArithmetic(UnaryOperator op)
		{
			return (op == UnaryOperator.Negation);
		}

		internal static bool IsOperatorBoolean(UnaryOperator op)
		{
			return (op != UnaryOperator.Negation);
		}

		public override void Validate()
		{
			// verify the operand is of the correct type
			switch( _op )
			{
				case UnaryOperator.Negation:
				{
					if( !_operand.IsArithmetic() )
					{
						throw new OPathException("Negation operator cannot be applied to operand.  Only arithmetic expressions can be applied.");
					}
					break;
				}
				case UnaryOperator.LogicalNot:
				{
					if( !_operand.IsBoolean() )
					{
						throw new OPathException("Logical Not operator cannot be applied to operand.  Only boolean expressions can be applied.");
					}
					break;
				}
				case UnaryOperator.IsNull:
				{
					// nothing to check - valid for most any operand
					break;
				}
				case UnaryOperator.Exists:
				{
					if( !_operand.IsFilter() )
					{
						throw new OPathException("Exists function can only be applied to filter expressions.");
					}
					break;
				}
				default:
				{
					throw new NotSupportedException("Unary operator '" + _op + "' was not expected.");
				}
			}
		}
	}

	internal enum UnaryOperator
	{
		Negation,
		LogicalNot,
		IsNull,
		Exists,
	}
}

