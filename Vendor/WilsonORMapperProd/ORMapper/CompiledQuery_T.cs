//-------------------------------------------------
// OPath Query Engine
// Written by Jeff Lanning (jefflanning@gmail.com)
// Modeled after SDK for Longhorn CTP Build 4074
// Version 1: Dec 2004 - May 2005
//-------------------------------------------------
#if DOTNETV2
using System;
using System.Collections.Generic;

using Wilson.ORMapper.Query;
using Wilson.ORMapper.Internals;

namespace Wilson.ORMapper
{
	/// <summary>
	/// A pre-compiled query that can be used by the ObjectSpace engine to return an ObjectReader or ObjectSet.
	/// </summary>
	public class CompiledQuery<T> : CompiledQuery
	{
		internal CompiledQuery(ObjectExpression oe)
			: base(oe)
		{
			if( base.ObjectType != typeof(T) )
			{
				throw new ArgumentException("Type of expression compiled does not match this type of CompiledQuery.");
			}
		}
	}
}
#endif