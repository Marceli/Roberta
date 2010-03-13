//**************************************************************//
// Paul Wilson -- www.WilsonDotNet.com -- Paul@WilsonDotNet.com //
// Feel free to use and modify -- just leave these credit lines //
// I also always appreciate any other public credit you provide //
//**************************************************************//
// Includes Even Better InnerObject Support from Jerry Shea (http://www.RenewTek.com)
// The ILoadOnDemand Interface is similar to the latest ObjectSpaces spec:
//   Thanks to Allan Ritchie (A.Ritchie@ACRWebSolutions.com) for related advice
using System;
using System.Collections;

namespace Wilson.ORMapper
{
	/// <summary>
	///     ObjectHolder is used for Lazy Loading Parent Objects
	/// </summary>
	public class ObjectHolder : ILoadOnDemand
	{
		private Internals.Context context;
		private Type type;
		private SelectProcedure selectSP;
		private object key;
		
		private object entity = null;

		internal ObjectHolder(Internals.Context context, Type type, object key) {
			this.context = context;
			this.type = type;
			this.selectSP = null;
			// DBNull Bug-Fix by Gerrod Thomas (http://www.Gerrod.com)
			this.key = (key is System.DBNull ? null : key);
		}

		internal ObjectHolder(Internals.Context context, SelectProcedure selectSP) {
			this.context = context;
			this.type = selectSP.ObjectType;
			this.selectSP = selectSP;
			// DBNull Bug-Fix by Gerrod Thomas (http://www.Gerrod.com)
			this.key = (selectSP.ParameterValue(0) is System.DBNull ? null : selectSP.ParameterValue(0));
		}

		#region ILoadOnDemand Members

		/// <summary>The Object has been Loaded from the Database</summary>
		public bool IsLoaded {
			get { return (this.entity != null); }
		}

		/// <summary>Get the Latest Version of Object from Database</summary>
		public void Resync() {
			this.entity = null;
			object load = this.InnerObject;
		}

		#endregion

		// Lazy-Key Property included from Paul Hatcher (http://www.grassoc.co.uk)
		/// <summary>The key for the object</summary>
		public object Key {
			get { return this.key; }
			set {
				// Null Bug-Fix by Gerrod Thomas (http://www.Gerrod.com)
				if ((key == null && value != null) || !this.key.Equals(value)) {
					// DBNull Bug-Fix by Jerry Shea (http://www.RenewTek.com)
					this.key = (value is System.DBNull ? null : value);
					if (this.selectSP != null) {
						SelectProcedure newSP = new SelectProcedure(this.type, this.selectSP.ProcedureName);
						newSP.AddParameter(this.selectSP.ParameterName(0), value);
						this.selectSP = newSP;
					}
					this.entity = null;
				}
			}
		}

		/// <summary>The inner object to load when needed</summary>
		public object InnerObject {
			get {
				if (!this.IsLoaded && this.key != null) {
					if (this.selectSP == null) {
						this.entity = this.context.GetObject(this.type, this.key, false);
					}
					else {
						this.entity = this.context.GetObjectSet(this.selectSP, false)[0];
					}
				}
				return this.entity;
			}
			set {
				this.entity = value;
				// Null Bug-Fix by Gerrod Thomas (http://www.Gerrod.com)
				this.key = (value != null ? this.context.GetObjectKey(this.entity) : null);
			}
		}
	}
}
