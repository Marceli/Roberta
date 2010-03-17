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
	internal class Function : Expression
	{
		private FunctionOperator _op;
		private Expression[] _args;

		public Function(FunctionOperator op, Expression[] args)
		{
			if( op == FunctionOperator.Substring && args.Length == 2 )
			{
				Expression[] newArgs = new Expression[3];
				newArgs[0] = args[0];
				newArgs[1] = args[1];
				newArgs[2] = new Function(FunctionOperator.Len, new Expression[] {(Expression)args[0].Clone()});
				args = newArgs;
			}

			this.Operator = op;
			this.Params = args;
		}

		public override NodeType NodeType
		{
			get { return NodeType.Function; }
		}

		public FunctionOperator Operator
		{
			get { return _op; }
			set { _op = value; }
		}

		public Expression[] Params
		{
			get { return _args; }
			set
			{
				if( value == null ) throw new ArgumentNullException("value");
	
				_args = value;
				for( int i = 0; i < _args.Length; i++ )
				{
					_args[i].Parent = this;
				}
			}
		}

		public override bool IsConst
		{
			get { return false; }
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
				switch( _op )
				{
					case FunctionOperator.Trim: return typeof(string);
					case FunctionOperator.Len: return typeof(int);
					case FunctionOperator.Left: return typeof(string);
					case FunctionOperator.Right: return typeof(string); 
					case FunctionOperator.Substring: return typeof(string);
					case FunctionOperator.UpperCase: return typeof(string);
					case FunctionOperator.LowerCase: return typeof(string);
					case FunctionOperator.Like: return null;
					case FunctionOperator.In: return null;
					default:
					{
						throw new NotSupportedException("Function operator '" + _op + "' is not supported.");
					}
				}
			}
		}


		public override object Clone()
		{
			Expression[] args = new Expression[_args.Length];
			for( int i = 0; i < _args.Length; i++ )
			{
				args[i] = (Expression)_args[i].Clone();
			}
			return new Function(_op, args);
		}

		public override void WriteXml(XmlWriter xmlw)
		{
			xmlw.WriteStartElement(this.GetType().Name);
			xmlw.WriteAttributeString("Operator", _op.ToString());

			for( int i = 0; i < _args.Length; i++ )
			{
				xmlw.WriteStartElement("Paramater");
				xmlw.WriteAttributeString("Index", i.ToString());

				_args[i].WriteXml(xmlw);

				xmlw.WriteEndElement();
			}
			
			xmlw.WriteEndElement();
		}

		public override string ToString()
		{
			string argList = null;
			for( int i = 0; i < _args.Length; i++ )
			{
				if( argList != null ) argList += ", ";
				argList += _args[i].ToString();
			}
			return string.Format("{0}({1})", _op, argList);
		}


		internal static bool IsOperatorArithmetic(FunctionOperator op)
		{
			return (op != FunctionOperator.Like && op != FunctionOperator.In);
		}

		internal static bool IsOperatorBoolean(FunctionOperator op)
		{
			return (op == FunctionOperator.Like || op == FunctionOperator.In);
		}


		public override void Validate()
		{
			// there is currently no way to support traversals within a function
			Expression.EnumNodes(this, new Expression.EnumNodesCallBack(this.ValidateNoFilters));

			// verify argument types based on function
			switch( _op )
			{
				case FunctionOperator.Trim:
				case FunctionOperator.Len:
				case FunctionOperator.UpperCase:
				case FunctionOperator.LowerCase:
				{
					if( _args.Length != 1 )
					{
						throw new OPathException("Invalid number of parameters passed to " + GetDisplayName(_op) + " function.");
					}
					if( _args[0].NodeType == NodeType.Parameter )
					{
						(_args[0] as Parameter).inferredType = typeof(string);
					}
					if( _args[0].ValueType != typeof(string) )
					{
						throw new OPathException("Parameter of " + GetDisplayName(_op) + " function must be of type string.");
					}
					break;
				}
				case FunctionOperator.Left:
				case FunctionOperator.Right:
				{
					if( _args.Length != 2 )
					{
						throw new OPathException("Invalid number of parameters passed to " + GetDisplayName(_op) + " function.");
					}
					if( _args[0].NodeType == NodeType.Parameter )
					{
						(_args[0] as Parameter).inferredType = typeof(string);
					}
					if( _args[1].NodeType == NodeType.Parameter )
					{
						(_args[1] as Parameter).inferredType = typeof(long);
					}

					if( _args[0].ValueType != typeof(string) )
					{
						throw new OPathException("First parameter of " + GetDisplayName(_op) + " function must be of type string.");
					}
					if( !Binary.IsInteger(_args[1].ValueType) )
					{
						throw new OPathException("Second parameter of " + GetDisplayName(_op) + " function must be of integer type.");
					}
					break;
				}
				case FunctionOperator.Substring:
				{
					if( _args.Length != 3 )
					{
						throw new OPathException("Invalid number of parameters passed to " + GetDisplayName(_op) + " function.");
					}
					if( _args[0].NodeType == NodeType.Parameter )
					{
						(_args[0] as Parameter).inferredType = typeof(string);
					}
					if( _args[1].NodeType == NodeType.Parameter )
					{
						(_args[1] as Parameter).inferredType = typeof(long);
					}
					if( _args[2].NodeType == NodeType.Parameter )
					{
						(_args[2] as Parameter).inferredType = typeof(long);
					}

					if( _args[0].ValueType != typeof(string) )
					{
						throw new OPathException("First parameter of " + GetDisplayName(_op) + " function must be of type string.");
					}
					if( !Binary.IsInteger(_args[1].ValueType) )
					{
						throw new OPathException("First parameter of " + GetDisplayName(_op) + " function must be of integer type.");
					}
					if( !Binary.IsInteger(_args[2].ValueType) )
					{						
						throw new OPathException("Second parameter of " + GetDisplayName(_op) + " function must be of integer type.");
					}
					break;
				}
				case FunctionOperator.Like:
				{
					if( _args.Length != 2 )
					{
						throw new OPathException("Invalid number of parameters passed to " + GetDisplayName(_op) + " function.");
					}
					if( _args[0].NodeType == NodeType.Parameter )
					{
						(_args[0] as Parameter).inferredType = typeof(string);
					}
					if( _args[1].NodeType == NodeType.Parameter )
					{
						(_args[1] as Parameter).inferredType = typeof(string);
					}

					//NOTE: Until type casting is supported, let's just send the left operand to the database and let it try casting
					//if( _args[0].ValueType != typeof(string) )
					//{
					//	throw new OPathException("Left operand of " + GetDisplayName(_op) + " comparison must be of type string.");
					//}
					if( _args[1].ValueType != typeof(string) )
					{
						throw new OPathException("Right operand of " + GetDisplayName(_op) + " comparison must be of type string.");
					}
					break;
				}
				case FunctionOperator.In:
				{
					if( _args.Length < 2 )
					{
						throw new OPathException("Invalid number of parameters passed to " + GetDisplayName(_op) + " function.");
					}
					break;
				}
				default:
				{
					throw new NotSupportedException("Binary operator '" + _op + "' was not expected.");
				}
			}
		}


		private bool ValidateNoFilters(Expression node, object[] noargs)
		{
			if( node.NodeType == NodeType.Filter || node.NodeType == NodeType.Axis )
			{
				throw new OPathException("Relationship traversal found in parameter for " + GetDisplayName(_op) + " function.  Please move the traversal outside the function.");
			}

			return true;
		}

		public static string GetDisplayName(FunctionOperator op)
		{
			switch( op )
			{
				case FunctionOperator.Trim: return "TRIM";
				case FunctionOperator.Len: return "LEN";
				case FunctionOperator.Left: return "LEFT";
				case FunctionOperator.Right: return "RIGHT"; 
				case FunctionOperator.Substring: return "SUBSTRING";
				case FunctionOperator.UpperCase: return "UPPER";
				case FunctionOperator.LowerCase: return "LOWER";
				case FunctionOperator.Like: return "LIKE";
				case FunctionOperator.In: return "IN";
				default:
				{
					throw new NotSupportedException("Function operator '" + op + "' was not expected.");
				}
			}
		}
	}

	internal enum FunctionOperator
	{
		In,  //note: it could be argued that this is a binary operator
		Trim,
		Len,
		Like,
		Left,
		Right,
		Substring,
		UpperCase,
		LowerCase,
		//FileName, //NOTE: never going to support this
		//FilePath, //NOTE: never going to support this
	}
}


