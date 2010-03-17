//**************************************************************//
// Paul Wilson -- www.WilsonDotNet.com -- Paul@WilsonDotNet.com //
// Feel free to use and modify -- just leave these credit lines //
// I also always appreciate any other public credit you provide //
//**************************************************************//
#if DOTNETV2
using System;
using System.Collections;

namespace Wilson.ORMapper
{
	/// <summary>
	///     ObjectHolder is used for Lazy Loading Parent Objects
	/// </summary>ne
	public class ObjectHolder<T> : ObjectHolder
	{
		internal ObjectHolder(Internals.Context context, object key)
			: base(context, typeof(T), key) {}

		internal ObjectHolder(Internals.Context context, SelectProcedure<T> selectSP)
			: base(context, selectSP) {}

		/// <summary>The inner object to load when needed</summary>
		public new T InnerObject {
			get { return (T) base.InnerObject; }
			set { base.InnerObject = value; }
		}
	}
}
#endif