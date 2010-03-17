//**************************************************************//
// Paul Wilson -- www.WilsonDotNet.com -- Paul@WilsonDotNet.com //
// Feel free to use and modify -- just leave these credit lines //
// I also always appreciate any other public credit you provide //
//**************************************************************//
using System;

namespace Wilson.ORMapper
{
	/// <summary>
	///     Optional Interface used to increase Performance by using
	///     an indexer instead of reflection to get/set property values.
	/// </summary>
	public interface IObjectHelper
	{
		/// <summary>
		///     The Item indexer for the object.  This property is used
		///     to increase performance with loading and persisting the object.
		/// </summary>
		object this[string memberName] { get; set; }
	}
}
