//-------------------------------------------------
// OPath Query Engine
// Written by Jeff Lanning (jefflanning@gmail.com)
// Modeled after SDK for Longhorn CTP Build 4074
// Version 1: Dec 2004 - May 2005
//-------------------------------------------------
using System;

namespace Wilson.ORMapper.Query
{
	/// <summary>
	/// Exception that is thrown when an OPath expression is invalid.
	/// </summary>
	public class OPathException : System.ApplicationException
	{
		/// <summary>
		/// Initializes a new instance of this class with the specified error message.
		/// </summary>
		/// <param name="message">A message that describes the current exception.</param>
		public OPathException(string message)
			: base(message) { }

		/// <summary>
		/// Initializes a new instance of this class with a specified error message
		/// and a reference to the inner exception that is the cause of this exception.
		/// </summary>
		/// <param name="message">A message that describes the current exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception.</param>
		public OPathException(string message, Exception innerException)
			: base(message, innerException) { }
	}
}

