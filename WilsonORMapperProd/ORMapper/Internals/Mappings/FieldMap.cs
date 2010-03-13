//**************************************************************//
// Paul Wilson -- www.WilsonDotNet.com -- Paul@WilsonDotNet.com //
// Feel free to use and modify -- just leave these credit lines //
// I also always appreciate any other public credit you provide //
//**************************************************************//
// Includes Null-Value Assistance from Tim Byng (http://www.missioninc.com)
// Includes Null-Value Expression support from Jeff Lanning (jefflanning@gmail.com)
using System;
using System.Collections;
using System.Reflection;

namespace Wilson.ORMapper.Internals
{
	// <attribute member="memberName" field="fieldName" [nullValue="nullValue"] [alias="propertyName"]
	//   [parameter="parameterName"] [persistType="Persist|ReadOnly|Concurrent"] />
	internal class FieldMap : AttributeMap
	{
		private static Hashtable nullValues = new Hashtable(100); // maps null values -> real values

		// Alias is tracked in EntityMap's Helper
		private object nullValue;
		private string parameter;
		private PersistType persistType = PersistType.Persist;
		private Type memberType;

		public object NullValue {
			get { return this.nullValue; }
		}

		public string Parameter {
			get { return this.parameter; }
		}

		public PersistType PersistType {
			get { return this.persistType; }
		}

		// Jeff Lanning (jefflanning@gmail.com): Added to expose member for OPath support.
		public Type MemberType {
			get { return this.memberType; }
		}

		public virtual bool IsLookup {
			get { return false; }
		}

		public virtual string FieldAlias {
			get { return this.Field; }
		}

		internal FieldMap(string member, string field, string nullValue, string parameter, PersistType persistType, Type memberType, CustomProvider provider)
			: base(member, field)
		{
			if (memberType == null) {
				throw new MappingException("Mapping: Field memberType was Missing - " + member);
			}
			this.persistType = persistType;
			this.memberType = memberType;

			if (nullValue == null) {
				this.nullValue = null;
			}
			else {
				// Jeff Lanning (jefflanning@gmail.com): Update to support null value expressions and make error message less ambiguous.
				try {
					this.nullValue = ConvertNullValue(nullValue, memberType);
				}
				catch (Exception ex) {
					throw new MappingException("Mapping: Failed to convert nullValue '" + nullValue + "' to type " + memberType.FullName + " for member '" + member + "'.", ex);
				}
			}
			if (parameter == null || parameter.Length == 0) {
				if (persistType != PersistType.Concurrent) {
					this.parameter = provider.GetParameterDefault(field);
				}
				else { // Concurrent Parameter Name Fix by Stephan Wagner (http://www.calac.net)
					this.parameter = provider.GetParameterDefault(member);
				}
			}
			else {
				this.parameter = parameter;
			}
		}


		/// <summary>
		/// Converts a string value or expression to the specified data type.
		/// </summary>
		/// <param name="expression">Value or expression to be converted.</param>
		/// <param name="targetType">Data type to which the value or expression will be converted.</param>
		/// <returns>Value converted to the specified type.</returns>
		/// <remarks>
		/// Expression Examples:
		/// - null
		/// - MinValue
		/// - Int32.MinValue
		/// - System.Int32.MinValue
		/// - String.Empty
		/// - Guid.Empty
		/// - CustomNamespace.CustomType.MemberOrProperty
		/// </remarks>
		private static object ConvertNullValue(string expression, Type targetType) // Jeff Lanning (jefflanning@gmail.com): Added method to convert expressions to values.
		{
			if (expression == null) throw new ArgumentNullException("expression");

			// return the value if the type+expression combo is already in our cache
			string key = targetType.FullName + "\t" + expression;
			object value = nullValues[key];
			if (value != null || nullValues.ContainsKey(key)) { //NOTE: have to check cache a second time since nulls are valid values
				return value;
			}

			Type type;
			string memberName;

			// try to get the type from the expression
			// default to the targetType if no type name is in the expression or could not convertable to a type
			int index = expression.LastIndexOf(Type.Delimiter);
			if (index > 0) { // type delimiter found
				string typeName = expression.Substring(0, index);
				type = Type.GetType(typeName);
				if (type == null) {
					type = Type.GetType("System." + typeName);
				}

				if (type != null) {
					memberName = expression.Substring(index + 1);
				}
				else { // could not find the type
					type = targetType;
					memberName = expression;
				}
			}
			else { // no type delimiter
				type = targetType;
				memberName = expression;
			}

			// see if the member name is a field
			FieldInfo field = type.GetField(memberName);
			if (field != null) {
				value = field.GetValue(type);
			}
			else { // not a field
				// see if the member name is a property
				PropertyInfo property = type.GetProperty(memberName);
				if (property != null) {
					value = property.GetValue(null, null);
				}
				else { // not a property
					// "null" and "nothing" converts to null value
					switch (expression.ToLower()) {
						case "null":
						case "nothing":	{
							if (targetType.IsValueType)	{
								throw new Exception("Null object cannot be converted to a value type.");									
							}
							return null;
						}
						default: {
							// use the original expression as the value to be converted
							value = expression;
							break;
						}
					}
				}
			}

			// convert the value to the target type; special-case Guid and byte-array types
			if (targetType == typeof(Guid) && value is string) {
				value = new Guid((string)value);
			}
			else if (targetType == typeof(byte[]) && value is string) {
				value = new byte[0]; // Paul Welter (http://www.LoreSoft.com) -- support nullable byte array
			}
			else {
				value = Convert.ChangeType(value, targetType);
			}

			// add the final value to the cache
			nullValues.Add(key, value);

			return value;
		}
	}
}
