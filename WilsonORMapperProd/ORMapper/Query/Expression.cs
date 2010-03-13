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
	internal abstract class Expression : ICloneable
	{
		internal delegate bool EnumNodesCallBack(Expression node, object[] args);

		private Expression _parent;
		
		protected Expression()
		{
		}

		public abstract NodeType NodeType { get; } //NOTE: longhorn spec has this as a virtual property

		internal Expression Parent
		{
			set { _parent = value; }
		}

		public Expression Owner
		{
			get { return _parent; }
		}

		public virtual bool IsConst
		{
			get { return true; }
		}


		public virtual bool IsArithmetic() //V2: consider making abstract
		{
			return false;
		}

		public virtual bool IsBoolean() //V2: consider making abstract
		{
			return false;
		}

		public virtual bool IsFilter() //V2: consider making abstract
		{
			return false;
		}

		public virtual Type ValueType
		{
			get { return null; }
		}

		//public object GetAnnotation(AnnotationType annotationType) //V2: In Longhorn spec
		//public void SetAnnotation(AnnotationType annotationType, object o) //V2: In Longhorn spec

		public abstract object Clone();
		
		public virtual void Validate()
		{
			// no validation if derived type does not implement this method
		}

		public override string ToString()
		{
			throw new NotImplementedException();
		}

		public string ToXmlString()
		{
			using( StringWriter buffer = new StringWriter() )
			{
				XmlTextWriter writer = new XmlTextWriter(buffer);
				writer.Formatting = Formatting.Indented;
				writer.Indentation = 2;

				this.WriteXml(writer);
			
				return buffer.ToString();
			}
		}

		public abstract void WriteXml(XmlWriter w);


		public static void EnumNodes(Expression root, EnumNodesCallBack callback)
		{
			EnumNodes(root, callback, null, null);
		}

		public static void EnumNodes(Expression root, EnumNodesCallBack callback, EnumNodesCallBack postCallback, params object[] args)
		{
			switch( root.NodeType )
			{
				case NodeType.Literal:
				case NodeType.Parameter:
				case NodeType.Context:
				{
					if( callback != null )
					{
						callback(root, args);
					}
					if( postCallback != null )
					{
						postCallback(root, args);
					}
					return;
				}
				case NodeType.Binary:
				{
					if( callback == null || callback(root, args) )
					{
						Binary node = (Binary)root;
						Expression.EnumNodes(node.Left, callback, postCallback, args);
						Expression.EnumNodes(node.Right, callback, postCallback, args);

						if( postCallback != null )
						{
							postCallback(root, args);
						}
					}
					return;
				}
				case NodeType.Unary:
				{
					if( callback == null || callback(root, args) )
					{
						Unary node = (Unary)root;
						Expression.EnumNodes(node.Operand, callback, postCallback, args);

						if( postCallback != null )
						{
							postCallback(root, args);
						}
					}
					return;
				}
				case NodeType.Axis:
				case NodeType.Filter:
				{
					if( callback == null || callback(root, args) )
					{
						Filter node = (Filter)root;
						Expression.EnumNodes(node.Source, callback, postCallback, args);
						Expression.EnumNodes(node.Constraint, callback, postCallback, args);

						if( postCallback != null )
						{
							postCallback(root, args);
						}
					}
					return;
				}
				case NodeType.Property:
				{
					if( callback == null || callback(root, args) )
					{
						Property node = (Property)root;
						Expression.EnumNodes(node.Source, callback, postCallback, args);

						if( postCallback != null )
						{
							postCallback(root, args);
						}
					}
					return;
				}
				case NodeType.Parent:
				{
					if( callback == null || callback(root, args) )
					{
						Parent node = (Parent)root;
						Expression.EnumNodes(node.Source, callback, postCallback, args);

						if( postCallback != null )
						{
							postCallback(root, args);
						}
					}
					return;
				}
				case NodeType.Function:
				{
					if( callback == null || callback(root, args) )
					{
						Function node = (Function)root;
						for( int i = 0; i < node.Params.Length; i++ )
						{
							Expression.EnumNodes(node.Params[i], callback, postCallback, args);
						}

						if( postCallback != null )
						{
							postCallback(root, args);
						}
					}
					return;
				}
				case NodeType.TypeFilter:
				{
					if( callback == null || callback(root, args) )
					{
						TypeFilter node = (TypeFilter)root;
						Expression.EnumNodes(node.Source, callback, postCallback, args);

						if( postCallback != null )
						{
							postCallback(root, args);
						}
					}
					return;
				}
				case NodeType.Empty:
				{
					return;
				}
				case NodeType.OrderBy:
				{
					if( callback == null || callback(root, args) )
					{
						OrderBy node = (OrderBy)root;
						Expression.EnumNodes(node.Source, callback, postCallback, args);

						if( postCallback != null )
						{
							postCallback(root, args);
						}
					}
					return;
				}
				default:
				{
					throw new NotSupportedException("Expression type '" + root.GetType() + "' is not currently supported.");
				}
			}
		}


		public static void Replace(Expression oldNode, Expression newNode)
		{
			if( oldNode == null ) throw new ArgumentNullException("oldNode");
			if( newNode == null ) throw new ArgumentNullException("newNode");

			Expression parent = oldNode.Owner;

			switch( parent.NodeType )
			{
				case NodeType.Axis:
				{
					Axis axis = (Axis)parent;
					if( axis.Source == oldNode )
					{
						axis.Source = newNode;
					}
					else if( axis.Constraint == oldNode )
					{
						axis.Constraint = newNode;
					}
					break;
				}
				case NodeType.Binary:
				{
					Binary binary = (Binary)parent;
					if( binary.Left == oldNode )
					{
						binary.Left = newNode;
					}
					else if( binary.Right == oldNode )
					{
						binary.Right = newNode;
					}
					break;
				}
				case NodeType.Filter:
				{
					Filter filter = (Filter)parent;
					if( filter.Source == oldNode )
					{
						filter.Source = newNode;
					}
					else if( filter.Constraint == oldNode )
					{
						filter.Constraint = newNode;
					}
					break;
				}
				case NodeType.Function:
				{
					Function function = (Function)parent;
					for( int i = 0; i < function.Params.Length; i++ )
					{
						if( function.Params[i] == oldNode )
						{
							function.Params[i] = newNode;
						}
					}
					break;
				}
				case NodeType.Property:
				{
					Property property = (Property)parent;
					if( property.Source == oldNode )
					{
						property.Source = newNode;
					}
					break;
				}
				case NodeType.TypeFilter:
				{
					TypeFilter filter = (TypeFilter)parent;
					if( filter.Source == oldNode )
					{
						filter.Source = newNode;
					}
					break;
				}
				case NodeType.Unary:
				{
					Unary unary = (Unary)parent;
					if( unary.Operand == oldNode )
					{
						unary.Operand = newNode;
					}
					break;
				}
				default:
				{
					throw new NotSupportedException("Node type '" + oldNode.Owner.NodeType + "' was not expected.");
				}
			}

			// remove old node's parent association
			oldNode.Parent = null;
		}
	}

	internal enum NodeType
	{
		//Aggregate,
		Axis,
		Binary,
		//Conditional,
		Context,
		//Distinct,
		Empty,
		//Expression,
		Filter,
		Function,
		//Join,
		//InOperator,
		Literal,
		//ObjectSpaceNode,
		OrderBy,
		Parameter,
		Parent,
		//Projection,
		Property,
		//RelTraversal,
		//Span,
		//Reference,
		//TypeCast,
		//TypeConversion,
		TypeFilter,
		//TypeProjection,
		Unary,
	}
}