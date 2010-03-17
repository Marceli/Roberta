//-------------------------------------------------
// OPath Query Engine
// Written by Jeff Lanning (jefflanning@gmail.com)
// Modeled after SDK for Longhorn CTP Build 4074
// Version 1: Dec 2004 - May 2005
//-------------------------------------------------
using System;
using System.Data;
using System.IO;
using System.Xml;

namespace Wilson.ORMapper.Query
{
	internal class Binary : Expression
	{
		private BinaryOperator _op;
		private Expression _left;
		private Expression _right;
		private Type _resultType;

		public Binary(BinaryOperator op, Expression left, Expression right)
		{
			this.Operator = op;
			this.Left = left;
			this.Right = right;
		}

		
		public override NodeType NodeType
		{
			get { return NodeType.Binary; }
		}

		public BinaryOperator Operator
		{
			get { return _op; }
			set { _op = value; }
		}

		public Expression Left
		{
			get { return _left; }
			set
			{
				if( value == null ) throw new ArgumentNullException("value");

				_left = value;
				_left.Parent = this;
			}
		}

		public Expression Right
		{
			get { return _right; }
			set
			{
				if( value == null ) throw new ArgumentNullException("value");

				_right = value;
				_right.Parent = this;
			}
		}

		
		public override bool IsConst
		{
			get { return (_left.IsConst && _right.IsConst); }
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
			return false;
		}

		public override Type ValueType
		{
			get
			{
				if( _resultType == null ) //V2: this caching technique does not help when null is the actual result type
				{
					_resultType = GetResultType(_left, _right, _op);
				}
				return _resultType;
			}
		}


		public override object Clone()
		{
			return new Binary(_op, (Expression)_left.Clone(), (Expression)_right.Clone());
		}

		public override void WriteXml(XmlWriter xmlw)
		{
			xmlw.WriteStartElement(this.GetType().Name);
			xmlw.WriteAttributeString("Operator", _op.ToString());

			xmlw.WriteStartElement("Left");
			_left.WriteXml(xmlw);
			xmlw.WriteEndElement();

			xmlw.WriteStartElement("Right");
			_right.WriteXml(xmlw);
			xmlw.WriteEndElement();
			
			xmlw.WriteEndElement();
		}

		public override string ToString()
		{
			return string.Format("({0} {1} {2})", _left, _op, _right);
		}


		public override void Validate()
		{
			if( IsOperatorLogical(_op) )
			{
				if( !this.Left.IsBoolean() || !this.Right.IsBoolean() )
				{
					throw new OPathException("Logical operators can only be used between two boolean expressions.");
				}
			}
			if( IsOperatorArithmetic(_op) )
			{
				if( !this.Left.IsArithmetic() || !this.Right.IsArithmetic() )
				{
					throw new OPathException("The " + _op + " operator must be placed between two arithmetic expressions.");
				}
			}

			//V2: Add more validation here
		}

		internal static Type GetResultType(Expression leftExpr, Expression rightExpr, BinaryOperator op)
		{
			if( IsOperatorBoolean(op) )
			{
				return typeof(bool);
			}

			if( op == BinaryOperator.Concatenation )
			{
				return typeof(string);
			}

			// if we make it here, we are deal with an Arithmetic operator

			//V2: the code below is a gross simplification or the actual logic that should be here truly deal with all the scenarios
			//    there is MUCH MUCH more to do here to fully implement this method.

			Type leftType = leftExpr.ValueType;
			Type rightType = rightExpr.ValueType;

			if( leftExpr.NodeType == NodeType.Parameter )
			{
				leftType = rightType; //V2: use inferredType?
			}
			else if( rightExpr.NodeType == NodeType.Parameter )
			{
				rightType = leftType; //V2: use inferredType?
			}

			// to simplify the logic convert char types to strings
			if( leftType == typeof(char) )
			{
				leftType = typeof(string);
			}
			if( rightType == typeof(char) )
			{
				rightType = typeof(string);
			}

			// a string combined with any other type equals a string
			if( leftType == typeof(string) || rightType == typeof(string) )
			{
				return typeof(string);
			}

			// a datetime combined with any other type equals a datetime
			if( leftType == typeof(DateTime) || rightType == typeof(DateTime) )
			{
				return typeof(DateTime);
			}

            // determine the precedence of the two types
			int leftIndex = GetPrecedenceIndex(leftType);
			int rightIndex = GetPrecedenceIndex(rightType);

			if( leftIndex == 0 || rightIndex == 0 )
			{
				//note: we should probably be throwing an error here
				return null;
			}

			// return the type with the higher precedence
			return (leftIndex >= rightIndex) ? leftType : rightType;
		}


		internal static bool IsNumeric(Type type)
		{
			return (Binary.IsInteger(type) || Binary.IsFloat(type));
		}

		internal static bool IsInteger(Type type)
		{
			return (type == typeof(short) 
				|| type == typeof(int) 
				|| type == typeof(long) 
				|| type == typeof(ushort) 
				|| type == typeof(uint) 
				|| type == typeof(ulong) 
				|| type == typeof(sbyte) 
				|| type == typeof(byte));
		}

		internal static bool IsFloat(Type type)
		{
			return (type == typeof(float) 
				|| type == typeof(double) 
				|| type == typeof(decimal));
		}

		internal static bool IsSigned(Type type)
		{
			return (type == typeof(short) 
				|| type == typeof(int) 
				|| type == typeof(long) 
				|| type == typeof(sbyte));
		}

		internal static bool IsUnsigned(Type type)
		{
			return (type == typeof(ushort) 
				|| type == typeof(uint) 
				|| type == typeof(ulong)				
				|| type == typeof(byte));
		}

		internal static bool IsMixedSigns(Type left, Type right)
		{
			return (Binary.IsSigned(left) && Binary.IsUnsigned(right)
				|| Binary.IsUnsigned(left) && Binary.IsSigned(right));
		}

		
		internal static bool IsOperatorArithmetic(BinaryOperator op)
		{
			switch( op )
			{
				case BinaryOperator.Addition:
				case BinaryOperator.Subtraction:
				case BinaryOperator.Multiplication:
				case BinaryOperator.Division:
				case BinaryOperator.Modulus:
				{
					return true;
				}
				default:
				{
					return false;
				}
			}
		}

		internal static bool IsOperatorBoolean(BinaryOperator op)
		{
			return (IsOperatorLogical(op) || IsOperatorRelational(op));
		}

		internal static bool IsOperatorLogical(BinaryOperator op)
		{
			switch( op )
			{
				case BinaryOperator.LogicalAnd:
				case BinaryOperator.LogicalOr:
				{
					return true;
				}
				default:
				{
					return false;
				}
			}
		}

		internal static bool IsOperatorRelational(BinaryOperator op)
		{
			switch( op )
			{
				case BinaryOperator.Equality:
				case BinaryOperator.Inequality:
				case BinaryOperator.LessThan:
				case BinaryOperator.LessEqual:
				case BinaryOperator.GreaterThan:
				case BinaryOperator.GreaterEqual:
				{
					return true;
				}
				default:
				{
					return false;
				}
			}
		}

		
		private static int GetPrecedenceIndex(Type type)
		{
			switch( Type.GetTypeCode(type) )
			{
				//case TypeCode.Char: return -4;
				//case TypeCode.String: return -3;
				//case TypeCode.Boolean: return -2;
				//case TypeCode.DateTime: return -1;
				case TypeCode.SByte: return 1;
				case TypeCode.Byte: return 2;
				case TypeCode.Int16: return 3;
				case TypeCode.UInt16: return 4;
				case TypeCode.Int32: return 5;
				case TypeCode.UInt32: return 6;
				case TypeCode.Int64: return 7;
				case TypeCode.UInt64: return 8;
				case TypeCode.Decimal: return 10;
				case TypeCode.Single: return 11;
				case TypeCode.Double: return 12;
				default:
				{
					//note: handle TimeSpan when supported
					return 0;
				}
			}
		}
	}

	internal enum BinaryOperator
	{
		LogicalAnd,
		LogicalOr,
		Equality,
		Inequality,
		LessThan,
		LessEqual,
		GreaterThan,
		GreaterEqual,
		Addition,
		Subtraction,
		Multiplication,
		Division,
		Modulus,
		Concatenation,
		//InInterval
	}
}

