//**************************************************************//
// Paul Wilson -- www.WilsonDotNet.com -- Paul@WilsonDotNet.com //
// Feel free to use and modify -- just leave these credit lines //
// I also always appreciate any other public credit you provide //
//**************************************************************//
// Includes Null-Value Assistance from Tim Byng (http://www.missioninc.com)
// Output Parameter Support by Alister McIntyre (http://www.aruspex.com.au)
using System;
using System.Data;

namespace Wilson.ORMapper.Internals
{
	[Serializable()]
	internal class Parameter
	{
		private string name;
		internal object value;
		private Type type;
		private ParameterDirection direction = ParameterDirection.Input;

		public string Name {
			get { return this.name; }
		}

		public object Value {
			get { return this.value; }
		}

		public Type Type {
			get { return this.type; }
		}

		public ParameterDirection Direction {
			get { return this.direction; }
		}

		internal Parameter(string name, object value, object nullValue)
			: this(name, value, nullValue, ParameterDirection.Input) {}

		internal Parameter(string name, object value, object nullValue, ParameterDirection direction) {
			if (name == null || name.Length == 0) {
				throw new MappingException("Internals: Parameter Name was Missing");
			}
			this.name = name;
			this.value = this.DBValue(value, nullValue);
			if (value == null && nullValue == null) {
				this.type = null;
			}
			else {
				this.type = (value != null ? value.GetType() : nullValue.GetType());
			}
			this.direction = direction;
		}

		private object DBValue(object value, object nullValue) {
			if (value == null) {
				return DBNull.Value;
			}
			else if (nullValue == null) {
				return value;
			}
			else if (nullValue is object[] && (nullValue as object[]).Length == 0) {
				return (value is object[] && (value as object[]).Length == 0 ? DBNull.Value : value);
			}
			else {
				return (value.Equals(nullValue) ? DBNull.Value : value);
			}
		}
	}
}
