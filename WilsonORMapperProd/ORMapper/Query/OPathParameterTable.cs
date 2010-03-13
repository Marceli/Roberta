//-------------------------------------------------
// OPath Query Engine
// Written by Jeff Lanning (jefflanning@gmail.com)
// Modeled after SDK for Longhorn CTP Build 4074
// Version 1: Dec 2004 - May 2005
//-------------------------------------------------
using System;
using System.Collections;

namespace Wilson.ORMapper.Query
{
	/// <summary>
	/// Represents a collection of parameters for use with an OPathQuery.
	/// </summary>
	internal class OPathParameterTable : ICollection, IEnumerable //V2: Consider making public again later; like Longhorn 4074 API
	{
		private ArrayList _list = new ArrayList();

		/// <summary>
		/// Gets the number of parameters contained in this instance.
		/// </summary>
		public int Count
		{
			get { return _list.Count; }
		}

		/// <summary>
		/// Gets a value indicating whether access to this collection is synchronized (thread-safe).
		/// </summary>
		public bool IsSynchronized
		{
			get { return _list.IsSynchronized; }
		}

		/// <summary>
		/// Gets the parameter at the specified index.
		/// </summary>
		public OPathParameter this[int index]
		{
			get { return (OPathParameter)_list[index]; }
		}

		/// <summary>
		/// Gets the parameter with the specified name.
		/// </summary>
		public OPathParameter this[string parameterName]
		{
			get
			{
				foreach( OPathParameter param in _list )
				{
					if( param.Name == parameterName )
					{
						return param;
					}
				}
				return null;
			}
		}

		/// <summary>
		/// Gets an object that can be used to synchronize access to the instance.
		/// </summary>
		public object SyncRoot
		{
			get { return _list.SyncRoot; }
		}

		/// <summary>
		/// Adds a parameter to this instance.
		/// </summary>
		/// <param name="parameter">Parameter to add.</param>
		internal void Add(OPathParameter parameter)
		{
			if( this.Contains(parameter.Name) )
			{
				throw new ArgumentException("A parameter named '" + parameter.Name + "' already exists.", "parameter");
			}
			_list.Add(parameter);
		}

		/// <summary>
		/// Determines whether a parameter with the specified name is in the collection.
		/// </summary>
		/// <param name="name">Name of the parameter to find.</param>
		/// <returns>True if parameter is found; otherwise, false.</returns>
		internal bool Contains(string name)
		{
			foreach( OPathParameter parameter in _list )
			{
				if( parameter.Name == name )
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Returns an enumerator that can iterate through the collection.
		/// </summary>
		/// <returns>An IEnumerator for the entire collection.</returns>
		public IEnumerator GetEnumerator()
		{
			return _list.GetEnumerator();
		}

		/// <summary>
		/// Copies the entire list to a compatible one-dimensional Array, starting at the specified index of the target array.
		/// </summary>
		/// <param name="array">The one-dimensional Array that is the destination of the elements to be copied.</param>
		/// <param name="index">The zero-based index in the Array at which copying begins.</param>
		public void CopyTo(Array array, int index)
		{
			_list.CopyTo(array, index);
		}
	}
}