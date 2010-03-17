//-------------------------------------------------
// OPath Query Engine
// Written by Jeff Lanning (jefflanning@gmail.com)
// Modeled after SDK for Longhorn CTP Build 4074
// Version 1: Dec 2004 - May 2005
//-------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;

namespace Wilson.ORMapper.Query
{
	/// <summary>
	/// OPath Expression Parser Engine
	/// </summary>
	/// <remarks>
	/// Parsing technique inspired by the class notes from:
	/// Willamette University, Prof. Fritz Ruehr, CS 384 - Programming Languages
	/// http://www.willamette.edu/~fruehr/348/lab3.html
	/// </remarks>
	internal class OPathParser
	{
		private Stack _args = new Stack();
		private Stack _ops = new Stack();
		private int _parameterCount = 0;

		public OPathParser()
		{
		}

		public Expression ParseObjectQuery(Type type, string opath, bool baseTypeOnly, out int parameterCount)
		{
			Expression expr = Parse(opath);
			parameterCount = _parameterCount;
			return new Filter(new TypeFilter(new Empty(), type, baseTypeOnly), expr);
		}

		public Expression Parse(string expression)
		{
			// reset our member vars
			_args.Clear();
			_ops.Clear();
			_parameterCount = 0;

			if( expression == null )
			{
				expression = string.Empty;
			}

			using( StringReader reader = new StringReader(expression) )
			{
				OPathLexer lexer = new OPathLexer(reader);

				Yytoken token;
				while( (token = lexer.Lex()) != null )
				{
#if DEBUG_PARSER
					Debug.WriteLine("----------------------------------------");
					DumpStacks("Starting Token: " + token.ToString());
#endif
					ProcessToken(token, lexer);
				
#if DEBUG_PARSER
					Debug.WriteLine("----------------------------------------\n");
					Debug.WriteLine("");
#endif
				}
			}

#if DEBUG_PARSER
			Debug.WriteLine("----------------------------------------");
			Debug.WriteLine("Starting Final Reduction");
#endif
			// see if we have a empty expression
			if( _args.Count == 0 )
			{
				return new Empty();
			}

			// reduce remaining operators on the stack
			while( _ops.Count > 0 )
			{
				Reduce(); // reduce the stacks
			}

#if DEBUG_PARSER
			Debug.WriteLine("----------------------------------------");
			Debug.WriteLine("");
#endif
			// expression better be completely reduced
			if( _args.Count > 1 )
			{
				throw new OPathException("Expression is not valid.");
			}

			// return root of the tree
			return (Expression)_args.Pop();
		}

		private void ProcessToken(Yytoken token, OPathLexer lexer)
		{
			switch( token.Type )
			{
				case TokenType.IDENT:
				{
					Property node = new Property(token.Text, new Context());
					_args.Push(node);
					break;
				}
				case TokenType.PARENT:
				{
					if( lexer.NextToken == null || lexer.NextToken.Type != TokenType.PERIOD )
					{
						throw new OPathException("Position " + token.Position + ": Parent relationship operator must be followed by a dot operator.");
					}
					Parent node = new Parent(new Context());
					_args.Push(node);
					break;
				}
				case TokenType.PARAM:
				{
					Parameter node = new Parameter(_parameterCount);
					_parameterCount += 1;

					_args.Push(node);
					break;
				}
				case TokenType.CONST:
				{
					Literal node = new Literal(token.Value);
					_args.Push(node);
					break;
				}
				case TokenType.PERIOD:
				{
					Yytoken last = lexer.LastToken;
					Yytoken next = lexer.NextToken;
					if( last == null || next == null )
					{
						throw new OPathException("Position " + token.Position + ": The dot operator (.) cannot be at the very beginning or end of an expression.");
					}
					// dot operator not valid unless it follows a property/relationship identifier, parent operator, or filter
					if( last.Type != TokenType.IDENT && last.Type != TokenType.PARENT && last.Type != TokenType.RBRACE )
					{
						throw new OPathException("Position " + token.Position + ": Dot operators cannot be used in this way.");
					}
					// parent operator cannot be after a property/relationship identifer
					if( next.Type == TokenType.PARENT && last.Type != TokenType.PARENT )
					{
						throw new OPathException("Position " + token.Position + ": Parent relationship operators (^) cannot be used in this way.");
					}
					goto case TokenType.MODULO;
				}
				case TokenType.MINUS:
				{
					// change the token to a negation unless followed by a token that supports subtraction
					Yytoken last = lexer.LastToken;
					if( last == null || (last.Type != TokenType.CONST && last.Type != TokenType.IDENT && last.Type != TokenType.PARAM && last.Type != TokenType.RPAREN) )
					{
						token.Type = TokenType.NEGATE;
					}
					goto case TokenType.COMMA;
				}
				case TokenType.COMMA:
				case TokenType.OP_EQ:
				case TokenType.OP_NE:
				case TokenType.OP_GT:
				case TokenType.OP_GE:
				case TokenType.OP_LT:
				case TokenType.OP_LE:
				case TokenType.OP_LIKE:
				case TokenType.AND:
				case TokenType.OR:
				case TokenType.NOT:
				case TokenType.PLUS:
				case TokenType.MULTIPLY:
				case TokenType.DIVIDE:
				case TokenType.MODULO:
				{
					if( _ops.Count == 0 || ComparePrecedence(token, (Yytoken)_ops.Peek()) > 0 ) // new token has higher precedence
					{
						_ops.Push(token);
					}
					else // new token has lower or same precedence
					{
						Reduce();
						// process this token again
						ProcessToken(token, lexer); // note: recursive call
					}
					break;
				}
				case TokenType.OP_IN:
				case TokenType.FN_ISNULL:
				case TokenType.FN_LEN:
				case TokenType.FN_TRIM:
				case TokenType.FN_LEFT:
				case TokenType.FN_RIGHT:
				case TokenType.FN_SUBSTR:
				case TokenType.FN_UPPER:
				case TokenType.FN_LOWER:
				case TokenType.FN_EXISTS:
				{
					Yytoken nextToken = lexer.Lex();
					if( nextToken.Type != TokenType.LPAREN )
					{
						throw new OPathException("Function '" + token.Text + "' must be followed by an opening parenthesis.");
					}
					_ops.Push(nextToken);
					_ops.Push(token);
					break;
				}
				case TokenType.LPAREN:
				case TokenType.LBRACE:
				{
					_ops.Push(token);
					break;
				}
				case TokenType.RPAREN:
				{
					if( _ops.Count == 0 ) throw new OPathException("Closing parenthesis encountered without matching opening parenthesis.");

					if( ((Yytoken)_ops.Peek()).Type == TokenType.LPAREN )
					{
						_ops.Pop(); // "cancel" the operator pair
					}
					else
					{
						Reduce();
						// process this token again
						ProcessToken(token, lexer); // note: recursive call
					}
					break;
				}
				case TokenType.RBRACE:
				{
					if( _ops.Count == 0 ) throw new OPathException("Closing brace encountered without matching opening brace.");

					if( ((Yytoken)_ops.Peek()).Type == TokenType.LBRACE )
					{
						Reduce(); // a final reduce to "cancel" the pair
					}
					else
					{
						Reduce();
						// process this token again
						ProcessToken(token, lexer); // note: recursive call
					}
					break;
				}
				default:
				{
					throw new NotSupportedException("Token type " + token.Type + " at position " + token.Position + " in expression is not supported.");
				}
			}

#if DEBUG_PARSER
			DumpStacks("\nAfter Token: " + token.ToString());
#endif
		}


		private void Reduce()
		{
#if DEBUG_PARSER
			DumpStacks("\nReducing...");
#endif
			if( _ops.Count == 0 )
			{
				throw new OPathException("Oparators stack is empty. Cannot reduce.");
			}

			Yytoken op = (Yytoken)_ops.Pop();
			switch( op.Type )
			{
				case TokenType.OP_EQ:
				case TokenType.OP_NE:
				case TokenType.OP_GT:
				case TokenType.OP_GE:
				case TokenType.OP_LT:
				case TokenType.OP_LE:
				case TokenType.AND:
				case TokenType.OR:
				case TokenType.PLUS:
				case TokenType.MINUS:
				case TokenType.MULTIPLY:
				case TokenType.DIVIDE:
				case TokenType.MODULO:
				{
					if( _args.Count < 2 ) throw new OPathException("Not enough arguments on stack for operator " + op.Type + ".");

					BinaryOperator bop = GetBinaryOperator(op);
					Expression right = (Expression)_args.Pop();
					Expression left = (Expression)_args.Pop();

					_args.Push( new Binary(bop, left, right) );
					break;
				}
				case TokenType.FN_TRIM:
				case TokenType.FN_LEN:
				case TokenType.FN_LEFT:
				case TokenType.FN_RIGHT:
				case TokenType.FN_SUBSTR:
				case TokenType.FN_UPPER:
				case TokenType.FN_LOWER:
				{
					if( _args.Count < 1 ) throw new OPathException("No argument on stack for operator " + op.Type + ".");

					FunctionOperator fop = GetFunctionOperator(op);
					Expression[] argList;
					if( _args.Peek() is Expression )
					{
						argList = new Expression[] { (Expression)_args.Pop() };
					}
					else
					{
						argList = (Expression[])_args.Pop();
					}

					_args.Push( new Function(fop, argList) );
					break;
				}
				case TokenType.OP_LIKE:
				case TokenType.OP_IN:
				{
					if( _args.Count < 1 ) throw new OPathException("No argument on stack for operator " + op.Type + ".");

					FunctionOperator fop = GetFunctionOperator(op);
					Expression[] argList;
					if( _args.Peek() is Expression )
					{
						argList = new Expression[2];
						argList[1] = (Expression)_args.Pop();
						argList[0] = (Expression)_args.Pop();
					}
					else
					{
						Expression[] oldList = (Expression[])_args.Pop();
						argList = new Expression[oldList.Length + 1];
						oldList.CopyTo(argList, 1);
						argList[0] = (Expression)_args.Pop();
					}

					_args.Push( new Function(fop, argList) );
					break;
				}
				case TokenType.NEGATE:
				{
					if( _args.Count < 1 ) throw new OPathException("No argument on stack for operator " + op.Type + ".");
					
					// try to negate the operand to avoid wrapping it in a unary negation node
					Expression operand = (Expression)_args.Peek();
					if( operand.NodeType == NodeType.Literal )
					{
						Literal literal = (Literal)operand;

						object newValue = null;
						if( literal.Value is int ) newValue = -(int)literal.Value;
						else if( literal.Value is decimal ) newValue = -(decimal)literal.Value;
						else if( literal.Value is long ) newValue = -(long)literal.Value;
						else if( literal.Value is double ) newValue = -(double)literal.Value;
						
						if( newValue != null )
						{
							literal.Value = newValue;
							break;
						}
					}

					// if we get here, we could not negate the operand
					goto case TokenType.NOT;
				}
				case TokenType.NOT:
				case TokenType.FN_ISNULL:
				case TokenType.FN_EXISTS:
				{
					if( _args.Count < 1 ) throw new OPathException("No argument on stack for operator " + op.Type + ".");

					UnaryOperator uop = GetUnaryOperator(op);
					Expression operand = (Expression)_args.Pop();

					_args.Push( new Unary(uop, operand) );
					break;
				}
				case TokenType.PERIOD:
				{
					if( _args.Count < 2 ) throw new OPathException("Not enough arguments on stack for operator " + op.Type + ".");

					Expression child = (Expression)_args.Pop();
					Expression parent = (Expression)_args.Pop();

					if( child.NodeType != NodeType.Property && child.NodeType != NodeType.Parent && child.NodeType != NodeType.Axis )
					{
						throw new OPathException("Invalid expression syntax near character position " + op.Position + ".");
					}

					if( child.NodeType == NodeType.Parent && parent.NodeType != NodeType.Parent )
					{
						throw new OPathException("Parent relationship operator (^) cannot be applied to a relationship or filter.");
					}

					if( parent.NodeType == NodeType.Property )
					{
						(parent as Property).IsRelational = true;
						_args.Push( new Axis(parent, child, true) );
					}
					else if( parent.NodeType == NodeType.Parent )
					{
						if( child.NodeType == NodeType.Axis )
						{
							throw new OPathException("Parent relationship operator (^) cannot be applied to a relationship or filter.");
						}
						else if( child.NodeType == NodeType.Parent )
						{
							(child as Parent).Source = (Parent)parent;
							_args.Push(child);
						}
						else if( child.NodeType == NodeType.Property )
						{
							(child as Property).Source = (Parent)parent;
							_args.Push(child);
						}
					}
					else if( parent.NodeType == NodeType.Axis )
					{
						Axis axis = (Axis)parent;
						if( !axis.IsDot )
						{
							_args.Push( new Axis(parent, child, true) );
						}
						else // is dot
						{
							// wrap the property at the end of the axis chain into an axis with the child as the new constraint
							Axis leafAxis = axis;
							while( leafAxis.Constraint.NodeType == NodeType.Axis )
							{
								leafAxis = (Axis)leafAxis.Constraint;
							}

							Expression newSource = leafAxis.Constraint;
							if( newSource.NodeType != NodeType.Property )
							{
								throw new OPathException("Constraint on leaf axis is not a Property node.");
							}

							(newSource as Property).IsRelational = true;
							leafAxis.Constraint = new Axis(newSource, child);

							_args.Push(axis);
						}
					}
					else
					{
						throw new OPathException("Parent is not a Property or Axis node.");
					}
					break;
				}
				case TokenType.LBRACE:
				{
					if( _args.Count < 2 ) throw new OPathException("Not enough arguments on stack for operator " + op.Type + ".");

					Expression constraint = (Expression)_args.Pop();
					Expression source = (Expression)_args.Pop();

					if( source.NodeType != NodeType.Property )
					{
						throw new OPathException("Filter source is not a Property node.");
					}

					(source as Property).IsRelational = true;

					_args.Push( new Axis(source, constraint, false) );
					break;
				}
				case TokenType.COMMA:
				{
					if( _args.Count < 2 ) throw new OPathException("Not enough arguments on stack for operator " + op.Type + ".");

					Expression argument = (Expression)_args.Pop();

					Expression[] list;
					if( _args.Peek() is Expression )
					{
						list = new Expression[2];
						list[1] = argument;
						list[0] = (Expression)_args.Pop();
					}
					else
					{
						Expression[] oldList = (Expression[])_args.Pop();
						list = new Expression[oldList.Length + 1];
						oldList.CopyTo(list, 0);
						list[list.Length - 1] = argument;
					}

					_args.Push(list);
					break;
				}
				default:
				{
					throw new NotSupportedException("Operator " + op.Type + " is not supported.");
				}
			}

#if DEBUG_PARSER
			DumpStacks("To...");
			Debug.WriteLine("");
#endif
		}


		private int ComparePrecedence(Yytoken left, Yytoken right)
		{
			// compare the group values (not the full group-index value)
			int leftValue = ((int)left.Type & 0xF00);
			int rightValue = ((int)right.Type & 0xF00);

			int result = -1 * leftValue.CompareTo(rightValue);

#if DEBUG_PARSER
			Debug.WriteLine(string.Format("ComparePrecedence({0}, {1}) = {2}", left.Type, right.Type, result));
#endif
			return result;
		}

		private BinaryOperator GetBinaryOperator(Yytoken op)
		{
			switch( op.Type )
			{
				case TokenType.OP_EQ: return BinaryOperator.Equality;
				case TokenType.OP_NE: return BinaryOperator.Inequality;
				case TokenType.OP_GT: return BinaryOperator.GreaterThan;
				case TokenType.OP_GE: return BinaryOperator.GreaterEqual;
				case TokenType.OP_LT: return BinaryOperator.LessThan;
				case TokenType.OP_LE: return BinaryOperator.LessEqual;
				case TokenType.AND: return BinaryOperator.LogicalAnd;
				case TokenType.OR: return BinaryOperator.LogicalOr;
				case TokenType.PLUS: return BinaryOperator.Addition;
				case TokenType.MINUS: return BinaryOperator.Subtraction;
				case TokenType.MULTIPLY: return BinaryOperator.Multiplication;
				case TokenType.DIVIDE: return BinaryOperator.Division;
				case TokenType.MODULO: return BinaryOperator.Modulus;
				default:
				{
					throw new NotSupportedException("Token Type " + op.Type + " is not a valid binary operator.");
				}
			}
		}

		private UnaryOperator GetUnaryOperator(Yytoken op)
		{
			switch( op.Type)
			{
				case TokenType.NEGATE: return UnaryOperator.Negation;
				case TokenType.NOT: return UnaryOperator.LogicalNot;
				case TokenType.FN_ISNULL: return UnaryOperator.IsNull;
				case TokenType.FN_EXISTS: return UnaryOperator.Exists;
				default:
				{
					throw new NotSupportedException("Token Type " + op.Type + " is not a valid unary operator.");
				}
			}
		}

		private FunctionOperator GetFunctionOperator(Yytoken op)
		{
			switch( op.Type )
			{
				case TokenType.OP_LIKE: return FunctionOperator.Like;
				case TokenType.OP_IN: return FunctionOperator.In;
				case TokenType.FN_TRIM: return FunctionOperator.Trim;
				case TokenType.FN_LEN: return FunctionOperator.Len;
				case TokenType.FN_LEFT: return FunctionOperator.Left;
				case TokenType.FN_RIGHT: return FunctionOperator.Right;
				case TokenType.FN_SUBSTR: return FunctionOperator.Substring;
				case TokenType.FN_UPPER: return FunctionOperator.UpperCase;
				case TokenType.FN_LOWER: return FunctionOperator.LowerCase;
				default:
				{
					throw new NotSupportedException("Token Type " + op.Type + " is not a valid function operator.");
				}
			}
		}


#if DEBUG_PARSER
		private void DumpStacks(string header)
		{
			Debug.WriteLine(header);
			DumpStack(_args, "Arguments");
			DumpStack(_ops, "Operators");
		}

		private void DumpStack(Stack stack, string name)
		{
			Stack temp = (Stack)stack.Clone();

			string list = null;
			if( temp.Count == 0 )
			{
				list = "[Empty]";
			}
			else
			{
				while( temp.Count > 0 )
				{
					object item = temp.Pop();
					string text = null;
					if( item is Array )
					{
						foreach( object obj in (Array)item )
						{
							if( text != null ) text += ", ";
							text += "{" + obj.ToString() + "}";
						}
						text = "[" + text + "]";
					}
					else
					{
						text = "{" + item.ToString() + "}";
					}
					list = text + "  " + list;
				}
			}

			Debug.WriteLine(name + ": " + list);
		}
#endif
	}
}
