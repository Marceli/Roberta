//**************************************************************//
// Paul Wilson -- www.WilsonDotNet.com -- Paul@WilsonDotNet.com //
// Feel free to use and modify -- just leave these credit lines //
// I also always appreciate any other public credit you provide //
//**************************************************************//
using System;
using System.Runtime.Serialization;

namespace Wilson.ORMapper
{
	/// <summary>
	/// ORMapper General Exception
	/// </summary>
	[Serializable()]
	public class ORMapperException : System.Exception
	{
		internal ORMapperException(string message) : base(message) {}
		internal ORMapperException(string message, Exception inner) : base(message, inner) {}
		internal ORMapperException(SerializationInfo info, StreamingContext context) : base(info, context) {} 
	}

	/// <summary>
	/// ORMapper Mapping Exception
	/// </summary>
	[Serializable()]
	public class MappingException : ORMapperException
	{
		internal MappingException(string message) : base(message) {}
		internal MappingException(string message, Exception inner) : base(message, inner) {}
		internal MappingException(SerializationInfo info, StreamingContext context) : base(info, context) {} 
	}

	/// <summary>
	/// ORMapper Persistence Exception
	/// </summary>
	[Serializable()]
	public class PersistenceException : ORMapperException
	{
		internal PersistenceException(string message) : base(message) {}
		internal PersistenceException(string message, Exception inner) : base(message, inner) {}
		internal PersistenceException(SerializationInfo info, StreamingContext context) : base(info, context) {} 
	}
}
