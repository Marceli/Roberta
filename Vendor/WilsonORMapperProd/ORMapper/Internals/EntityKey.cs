//**************************************************************//
// Paul Wilson -- www.WilsonDotNet.com -- Paul@WilsonDotNet.com //
// Feel free to use and modify -- just leave these credit lines //
// I also always appreciate any other public credit you provide //
//**************************************************************//
// Includes Support for Composite Primary Keys from Jerry Shea (http://www.RenewTek.com)
// Includes Support for Member Properties from Chris Schletter (http://www.thzero.com)
// Includes EntityKey optimizations from Marc Brooks (http://musingmarc.blogspot.com)
using System;
using System.Collections;
using System.Reflection;
using System.Text;

namespace Wilson.ORMapper.Internals {
	internal class EntityKey {
		private Type type;
		private object value;

		private static Hashtable keys = new Hashtable();

		public Type Type {
			get { return this.type; }
		}

		public object Value {
			get { return this.value; }
		}

		internal EntityKey(Type objectType, object objectKey) {
			this.type = objectType;
			this.value = objectKey;
		}

		internal EntityKey(Context context, object entityObject, bool isPersisted) {
			this.type = entityObject.GetType();
			this.value = GetObjectKey(context.Mappings[this.type], entityObject, isPersisted);
		}

		internal static object GetObjectKey(Context context, Instance instance) {
			return GetObjectKey(instance.EntityMap, instance.EntityObject, instance.IsPersisted);
		}

		private static object GetMember(EntityMap entityMap, object entityObject, string keyMember) {
			if (entityMap.HasHelper) {
				return ((IObjectHelper) entityObject)[keyMember];
			}
			else {
				try {
					// Improved Support for Embedded Objects from Chris Schletter (http://www.thzero.com)
					object memberValue = entityObject;
					string[] memberParts = keyMember.Split('.');
					for (int index = 0; index < memberParts.Length; index++) {
						string typeName = memberValue.GetType().ToString();
						MemberInfo memberField = EntityMap.FindField(typeName, memberParts[index]);
						if (memberField is FieldInfo) {
							memberValue = (memberField as FieldInfo).GetValue(memberValue);
						}
						else {
							memberValue = (memberField as PropertyInfo).GetValue(memberValue, null);
						}
					}
					return memberValue;
				}
				catch (Exception exception) {
					throw new ORMapperException("GetField failed for " + keyMember, exception);
				}
			}
		}

		private static object GetObjectKey(EntityMap entityMap, object entityObject, bool isPersisted) {
			FieldMap[] keyFields = entityMap.KeyFields;
			// Paul Welter (http://www.LoreSoft.com) -- allow inserts by using all columns as key
			if (keyFields.Length == 0 || entityMap.KeyType == KeyType.None) {
				keyFields = entityMap.Fields;
			}

			object[] keyArray = new object[keyFields.Length];

			for (int index = 0; index < keyFields.Length; index++) {
				string keyMember = keyFields[index].Member;
				object keyValue = GetMember(entityMap, entityObject, keyMember);
				if (entityMap.KeyType == KeyType.None && keyValue == null) keyValue = string.Empty;
				if (EntityKey.IsNull(keyValue, isPersisted)) {
					switch (entityMap.KeyType) {
						case KeyType.Auto: keyValue = EntityKey.GetAutoKey(keyValue, entityMap.Type); break;
						case KeyType.Guid: keyValue = EntityKey.GetGuidKey(keyValue is Guid); break;
						default: throw new ORMapperException("ObjectSpace: Entity Object is missing Key - " + entityMap.Type);
					}
					if (entityMap.HasHelper) {
						((IObjectHelper) entityObject)[keyMember] = keyValue;
					}
					else {
						if (entityMap.Member(keyMember) is FieldInfo) {
							(entityMap.Member(keyMember) as FieldInfo).SetValue(entityObject, keyValue);
						}
						else {
							(entityMap.Member(keyMember) as PropertyInfo).SetValue(entityObject, keyValue, null);
						}
					}
				}

				keyArray[index] = keyValue;
			}

			if (keyArray.Length == 1)
				return keyArray[0];
			else
				return keyArray;
		}

		// Includes Additional Null Tests by Gerrod Thomas (http://www.Gerrod.com)
		private static bool IsNull(object keyValue, bool isPersisted) {
			// Bug-Fix to properly handle Primary Keys that are Zero-Valued
			if (isPersisted) {
				return (keyValue == null);
			}
			else {
				return (keyValue == null
					|| (keyValue is long && (long) keyValue == 0L)
					|| (keyValue is int && (int) keyValue == 0)
					|| (keyValue is short && (short) keyValue == 0)
					|| (keyValue is Guid && (Guid) keyValue == Guid.Empty)
					|| (keyValue is string && ((string) keyValue).Length == 0)
					|| (keyValue is DateTime && (DateTime) keyValue == DateTime.MinValue));
			}
		}

		private static object GetAutoKey(object keyValue, string typeName) {
			if (keyValue is Guid) {
				return Guid.NewGuid();
			}

			AutoKey keyAssigner = EntityKey.keys[typeName] as AutoKey;

			if (keyAssigner == null) {
				lock(EntityKey.keys) {
					keyAssigner = EntityKey.keys[typeName] as AutoKey;
					if (keyAssigner == null) {
						// could be type specific here...
						keyAssigner = new AutoKey();
						EntityKey.keys[typeName] = keyAssigner;
					}
				}
			}

			object nextKey =  keyAssigner.Next();	// locked during the Next call only...

			// Includes Additional Auto Types by Gerrod Thomas (http://www.Gerrod.com)
			if (!(keyValue is int)) {
				nextKey = QueryHelper.ChangeType(nextKey, keyValue.GetType());
			}

			return nextKey;
		}

		private static object GetGuidKey(bool isGuid) {
			Guid newGuid = Guid.NewGuid();
			if (isGuid) return newGuid;
			return newGuid.ToString();
		}

		// must NOT be a struct or value type, we want this guy to not be boxed when in the HashTable.
		private class AutoKey {
			private int m_Next;

			internal int Next() {
				lock (this) {
					return --m_Next;
				}
			}
		}

		public override string ToString() {
			StringBuilder retval = new StringBuilder(this.type.ToString()).Append(':');
			if (this.value is object[]) {
				object[] values = this.value as object[];
				for (int index = 0; index < values.Length; index++) {
					retval.Append(values[index].ToString()).Append(',');
				}
				if (values.Length > 0) retval.Length--;  // trim trailing comma
			}
			else {
				retval.Append(this.value.ToString());
			}
			return retval.ToString();
		}

		/// <summary>
		/// Determines if this EntityKey is equal to another object. The EntityKey is equal to another 
		/// object if that object is an EntityKey, and the all the elements compare equal using 
		/// <see cref="EntityKey.Equals(EntityKey)"/>.
		/// </summary>
		/// <param name="obj">Object to compare for equality.</param>
		/// <returns>True if the objects are equal. False if the objects are not equal.</returns>
		public override bool Equals(object obj) {
			if (this == obj) {
				return true;
			}

			if (obj != null && obj is EntityKey) {
				return this.Equals(obj as EntityKey);
			}

			return false;
		}

		/// <summary>
		/// Determines if this EntityKey is equal to another object. The EntityKey is equal to another 
		/// EntityKey if all the elements compare equal using using IComparable&lt;T&gt;.Equals or object.Equals.
		/// </summary>
		/// <param name="other">EntityKey to compare with for equality.</param>
		/// <returns>True if the EntityKey are equal. False if the EntityKey are not equal.</returns>
		public bool Equals(EntityKey other) {
			if (this.type != other.type)
				return false;

			object[] thisValue = this.value as object[];

			if (thisValue != null) {
				object[] otherValue = other.value as object[];

				// this has an array of key elements, the other must as well and of the same length
				if (otherValue == null || thisValue.Length != otherValue.Length)
					return false;

				for (int i = 0; i < thisValue.Length; i++) {
					if (!EqualOneValue(thisValue[i], otherValue[i]))
						return false;
				}

				return true;
			}

			return EqualOneValue(this.value, other.value);
		}

		/// <summary>
		/// Returns a hash code for the EntityKey, suitable for use in a hash-table or other hashed collection.
		/// Two EntityKeys that compare equal (using Equals) will have the same hash code. The hash code for
		/// the EntityKey is derived by combining the hash codes for each of the elements of the EntityKey.
		/// </summary>
		/// <returns>The hash code.</returns>
		public override int GetHashCode() {
			// Build the hash code from the hash codes of Type and Value. 
			int hashValue = (this.type == null) ? 0x61E04917 : this.type.GetHashCode();

			object[] thisValues = this.value as object[];

			if (thisValues != null) {
				for (int i = 0; i < thisValues.Length; i++) {
					hashValue = (hashValue << 5) ^ thisValues[i].GetHashCode();
				}
			}
			else if (this.value != null) {
				hashValue = (hashValue << 5) ^ this.value.GetHashCode();
			}

			return hashValue;
		}

		static private bool EqualOneValue(object left, object right) {
			if (left == null || right == null) {
				return (left == null && right == null);
			}

			return left.Equals(right);
		}
	}
}