//**************************************************************//
// Paul Wilson -- www.WilsonDotNet.com -- Paul@WilsonDotNet.com //
// Feel free to use and modify -- just leave these credit lines //
// I also always appreciate any other public credit you provide //
//**************************************************************//
using System;
using System.Data;
using Wilson.ORMapper.Internals;

namespace Wilson.ORMapper
{
	/// <summary>
	///     The SelectProcedure class is used to load an entity
	///     object collection with a stored procedure.
	/// </summary>
	/// <example>The following example shows how to use the
	/// SelectProcedure to get all Contacts with names that start with A.
	///		<code>
	///	public static ObjectSpace Manager; // See Initialization Example
	/// 
	///	// Get All Contacts with Names that start with A
	///	SelectProcedure selectProc = new SelectProcedure(typeof(Contact), "RetrieveContacts");
	///	selectProc.AddParameter("@ContactName", "A");
	///	ObjectSet contacts = Manager.GetObjectSet(selectProc);
	///		</code>
	/// </example>

	[Serializable()]
	public class SelectProcedure
	{
		private Type objectType;
		private string procedureName;
		internal Parameter[] parameters = new Parameter[0];

		/// <summary>The type of object to retrieve with this procedure</summary>
		public Type ObjectType {
			get { return this.objectType; }
		}

		/// <summary>The name of the procedure to be executed</summary>
		public string ProcedureName {
			get { return this.procedureName; }
		}

		/// <summary>Collection of procedure parameter names</summary>
		/// <param name="index">The index of the parameter name to retrieve</param>
		/// <returns>The parameter name</returns>
		public string ParameterName(int index) {
			return this.parameters[index].Name;
		}

		/// <summary>Collection of procedure parameter values</summary>
		/// <param name="index">The index of the parameter value to retrieve</param>
		/// <returns>The parameter value</returns>
		public object ParameterValue(int index) {
			return this.parameters[index].Value;
		}

		/// <summary>The number of parameters in the parameters collection</summary>
		public int ParameterCount {
			get { return this.parameters.Length; }
		}

		
		/// <summary>Creates a new SelectProcedure instance</summary>
		/// <param name="objectType">The type of object to retrieve with this procedure</param>
		/// <param name="procedureName">The name of the procedure to be executed</param>
		public SelectProcedure(Type objectType, string procedureName) {
			if (procedureName == null || procedureName.Length == 0) {
				throw new MappingException("SelectProcedure: ProcedureName was Missing");
			}
			this.objectType = objectType;
			this.procedureName = procedureName;
		}

		/// Modified by Ben Priebe (http://stickfly.com) - 05-Aug-2004
		/// Includes Null-Value Assistance from Tim Byng (http://www.missioninc.com)
		/// <summary>The number of parameters in the parameters collection</summary>
		/// <param name="parameterName">The name of the parameter to add</param>
		/// <param name="parameterValue">The value of the parameter</param>
		public int AddParameter(string parameterName, object parameterValue) {
			return AddParameter(parameterName, parameterValue, null);
		}

		/// Added by Ben Priebe (http://stickfly.com) - 05-Aug-2004
		/// Includes Null-Value Assistance from Tim Byng (http://www.missioninc.com)
		/// <summary>The number of parameters in the parameters collection</summary>
		/// <param name="parameterName">The name of the parameter to add</param>
		/// <param name="parameterValue">The value of the parameter</param>
		/// <param name="nullValue">The value that should be interpreted as DBNull for this parameter type </param>
		public int AddParameter(string parameterName, object parameterValue, object nullValue) {
			int index = this.parameters.Length;
			Parameter[] tempParameters = new Parameter[index + 1];
			this.parameters.CopyTo(tempParameters, 0);
			tempParameters[index] = new Parameter(parameterName, parameterValue, nullValue);
			this.parameters = tempParameters;
			return index;
		}

		/// Output Parameter Support by Alister McIntyre (http://www.aruspex.com.au)
		/// <summary>The number of parameters in the parameters collection</summary>
		/// <param name="parameterName">The name of the parameter to add</param>
		/// <param name="parameterValue">The value of the parameter</param>
		/// <param name="direction">The direction of the parameter</param>
		public int AddParameter(string parameterName, object parameterValue, ParameterDirection direction) {
			int index = this.parameters.Length;
			Parameter[] tempParameters = new Parameter[index + 1];
			this.parameters.CopyTo(tempParameters, 0);
			tempParameters[index] = new Parameter(parameterName, parameterValue, null, direction);
			this.parameters = tempParameters;
			return index;
		}
	}
}
