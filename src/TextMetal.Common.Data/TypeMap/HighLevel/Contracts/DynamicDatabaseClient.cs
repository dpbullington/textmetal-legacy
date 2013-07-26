/*
	Copyright ©2002-2010 D. P. Bullington
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

using TextMetal.Common.Data.TypeMap.LowLevel;

namespace TextMetal.Common.Data.TypeMap.Contracts
{
	/// <summary>
	/// Represents a dynamic database client.
	/// </summary>
	public class DynamicDatabaseClient : DynamicInvoker
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the DynamicDatabaseClient class.
		/// </summary>
		/// <param name="database">The database instance used to perform operations.</param>
		public DynamicDatabaseClient(IDatabase database)
		{
			if ((object)database == null)
				throw new ArgumentNullException("database");

			this.database = database;
		}

		#endregion

		#region Fields/Constants

		private readonly IDatabase database;

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Disposes of the inner database, if present.
		/// Once disposed, the instance cannot be reused.
		/// </summary>
		public override void Dispose()
		{
			if (base.Disposed)
				return;

			try
			{
				// do nothing
			}
			finally
			{
				base.Dispose();
			}
		}

		/// <summary>
		/// Represnts a dynamic invocation of a proxied type member.
		/// </summary>
		/// <param name="proxiedType">The run-time type of the proxied type (may differ from MemberInfo.DeclaringType).</param>
		/// <param name="invokedMethodInfo">The MemberInfo of the invoked member.</param>
		/// <param name="proxyInstance">The proxy object instance.</param>
		/// <param name="invocationParameters">The parameters passed to the invoked member, if appliable.</param>
		/// <returns>The return value from the invoked member, if appliable.</returns>
		public override object Invoke(Type proxiedType, MethodInfo invokedMethodInfo, object proxyInstance, object[] invocationParameters)
		{
			Type contractType;
			Type returnParameterType;
			ParameterInfo returnParameterInfo;
			object returnValue;
			ParameterInfo[] parameterInfos;

			DatabaseContractAttribute databaseContractAttribute;
			CommandContractAttribute commandContractAttribute;
			ReturnContractAttribute returnContractAttribute;

			List<IDataParameter> dataParameters;
			IDataParameter dataParameter;

			Dictionary<int, IDataParameter> outputDataParameters;
			bool isClrOutputParameter;

			if (!((object)invokedMethodInfo != null &&
			      invokedMethodInfo.DeclaringType == typeof(IDisposable)) &&
			    base.Disposed) // always forward dispose invocations
				throw new ObjectDisposedException(typeof(DynamicDatabaseClient).FullName);

			// sanity checks
			if ((object)proxiedType == null)
				throw new ArgumentNullException("proxiedType");

			if ((object)invokedMethodInfo == null)
				throw new ArgumentNullException("invokedMethodInfo");

			if ((object)proxyInstance == null)
				throw new ArgumentNullException("proxyInstance");

			if ((object)invocationParameters == null)
				throw new ArgumentNullException("invocationParameters");

			if (invokedMethodInfo.DeclaringType == typeof(object))
				return InvokeOnObject(proxiedType, invokedMethodInfo, proxyInstance, invocationParameters);
			else if (invokedMethodInfo.DeclaringType == typeof(IDisposable))
			{
				if (invokedMethodInfo.Name == "Dispose")
					this.Dispose();
				else
					throw new InvalidOperationException(string.Format("Method '{0}' not supported on '{1}'.", invokedMethodInfo.Name, typeof(object).FullName));

				return null;
			}

			// obtain the contract type
			contractType = invokedMethodInfo.DeclaringType;

			// are we executing a database contract?
			if (!contractType.IsInterface)
				throw new InvalidOperationException(string.Format("The type '{0}' is not an interface.", contractType.FullName));

			Reflexion.GetZeroAttributes<CommandContractAttribute>(contractType);
			//Reflexion.GetZeroAttributes<DatabaseContractAttribute>(contractType);
			Reflexion.GetZeroAttributes<ParameterContractAttribute>(contractType);
			Reflexion.GetZeroAttributes<ReturnContractAttribute>(contractType);

			databaseContractAttribute = Reflexion.GetOneAttribute<DatabaseContractAttribute>(contractType);

			if ((object)databaseContractAttribute == null)
				throw new InvalidOperationException(string.Format("The type '{0}' does not specify the '{1}'.", contractType.FullName, typeof(DatabaseContractAttribute).FullName));

			//Reflexion.GetZeroAttributes<CommandContractAttribute>(invokedMethodInfo);
			Reflexion.GetZeroAttributes<DatabaseContractAttribute>(invokedMethodInfo);
			Reflexion.GetZeroAttributes<ParameterContractAttribute>(invokedMethodInfo);
			Reflexion.GetZeroAttributes<ReturnContractAttribute>(invokedMethodInfo);

			// are we executing a database command?
			commandContractAttribute = Reflexion.GetOneAttribute<CommandContractAttribute>(invokedMethodInfo);

			if ((object)commandContractAttribute == null)
				throw new InvalidOperationException(string.Format("The type::member '{0}::{1}' does not specify the '{2}'.", contractType.FullName, invokedMethodInfo.Name, typeof(CommandContractAttribute).FullName));

			// examine parameters
			dataParameters = new List<IDataParameter>();
			outputDataParameters = new Dictionary<int, IDataParameter>();
			parameterInfos = invokedMethodInfo.GetParameters();

			if ((object)parameterInfos != null)
			{
				int parameterIndex = 0;

				foreach (ParameterInfo parameterInfo in parameterInfos)
				{
					Type parameterType;
					ParameterDirection direction;
					DbType type;
					int size;
					byte precision;
					byte scale;
					bool nullable;
					string name;
					object value;
					ParameterContractAttribute parameterContractAttribute;

					parameterType = parameterInfo.ParameterType;

					Reflexion.GetZeroAttributes<CommandContractAttribute>(parameterType);
					Reflexion.GetZeroAttributes<DatabaseContractAttribute>(parameterType);
					//Reflexion.GetZeroAttributes<ParameterContractAttribute>(parameterType);
					Reflexion.GetZeroAttributes<ReturnContractAttribute>(parameterType);

					// ensure parameter is marked with attribute
					parameterContractAttribute = Reflexion.GetOneAttribute<ParameterContractAttribute>(parameterInfo);

					if ((object)parameterContractAttribute == null)
						throw new InvalidOperationException(string.Format("The type::method::parameter '{0}::{1}::{2}' does not specify the '{3}'.", contractType.FullName, invokedMethodInfo.Name, parameterInfo.Name, typeof(ParameterContractAttribute).FullName));

					// get parameter name (specified)
					name = parameterContractAttribute.Name;

					// get parameter size (specified)
					size = parameterContractAttribute.Size;

					// get parameter precision (specified)
					precision = parameterContractAttribute.Precision;

					// get parameter scale (specified)
					scale = parameterContractAttribute.Scale;

					// get parameter nullable-ness (specified)
					nullable = parameterContractAttribute.Nullable;

					// get parameter value (infered)
					value = invocationParameters[parameterIndex];

					isClrOutputParameter = (!parameterInfo.IsOut && parameterType.IsByRef) ||
					                       (parameterInfo.IsOut && parameterType.IsByRef);

					// get parameter direction...
					if (parameterContractAttribute.UseInferredDirection)
					{
						// get parameter direction (infered)

						if (!parameterInfo.IsOut && parameterType.IsByRef) // byref
							direction = ParameterDirection.InputOutput;
						else if (parameterInfo.IsOut && parameterType.IsByRef) // 'out'
							direction = ParameterDirection.Output;
						else if (!parameterType.IsByRef) // byval
							direction = ParameterDirection.Input;
						else
							throw new InvalidOperationException(string.Format("The type::method::parameter '{0}::{1}::{2}' does not specify a valid parameter passing modifier.", contractType.FullName, invokedMethodInfo.Name, parameterInfo.Name));
					}
					else
					{
						// get parameter direction (specified)
						direction = (ParameterDirection)parameterContractAttribute.Direction;
					}

					// coerce conversion of char to string
					if (parameterType == typeof(char))
						throw new InvalidOperationException(string.Format("The type::method::parameter '{0}::{1}::{2}' specifies a parameter of type '{3}'; please use '{4}' instead.", contractType.FullName, invokedMethodInfo.Name, parameterInfo.Name, typeof(char).FullName, typeof(string).FullName));

					// coerce conversion of char[] to string
					if (parameterType == typeof(char[]))
						throw new InvalidOperationException(string.Format("The type::method::parameter '{0}::{1}::{2}' specifies a parameter of type '{3}'; please use '{4}' instead.", contractType.FullName, invokedMethodInfo.Name, parameterInfo.Name, typeof(char[]).FullName, typeof(string).FullName));

					// get parameter type...
					if (parameterContractAttribute.UseInferredType)
					{
						// get parameter type (infered)
						type = MappingUtils.InferDbTypeForClrType(parameterType);
					}
					else
					{
						// get parameter type (specified)
						type = (DbType)parameterContractAttribute.Type;
					}

					// add parameter to collection
					dataParameter = this.database.CreateParameter(direction, type, size, precision, scale, nullable, name, value);
					dataParameters.Add(dataParameter);

					if (isClrOutputParameter)
						outputDataParameters.Add(parameterIndex, dataParameter);

					// increment parameter index
					parameterIndex++;
				}
			}

			// get the command return type
			returnParameterInfo = invokedMethodInfo.ReturnParameter;
			returnParameterType = returnParameterInfo.ParameterType;

			Reflexion.GetZeroAttributes<CommandContractAttribute>(returnParameterInfo);
			Reflexion.GetZeroAttributes<DatabaseContractAttribute>(returnParameterInfo);
			Reflexion.GetZeroAttributes<ParameterContractAttribute>(returnParameterInfo);
			//Reflexion.GetZeroAttributes<ReturnContractAttribute>(returnParameterInfo);

			// ensure return is marked with attribute
			returnContractAttribute = Reflexion.GetOneAttribute<ReturnContractAttribute>(returnParameterInfo);

			if ((object)returnContractAttribute == null)
				throw new InvalidOperationException(string.Format("The type::method::return '{0}::{1}' does not specify the '{2}'.", contractType.FullName, invokedMethodInfo.Name, typeof(ReturnContractAttribute).FullName));

			// execute database method and capture return value;
			// make the call based on the command method return type
			if (returnParameterType == typeof(int) ||
			    returnParameterType == typeof(void))
			{
				int recordsAffected;

				recordsAffected = this.database.ExecuteNonQuery(commandContractAttribute.Type, commandContractAttribute.Text, dataParameters.ToArray(), commandContractAttribute.UseDefaultTimeout ? (int?)null : commandContractAttribute.Timeout, commandContractAttribute.Prepare);

				returnValue = recordsAffected;
			}
			else if (returnParameterType == typeof(object))
			{
				object scalarValue;

				scalarValue = this.database.ExecuteScalar(commandContractAttribute.Type, commandContractAttribute.Text, dataParameters.ToArray(), commandContractAttribute.UseDefaultTimeout ? (int?)null : commandContractAttribute.Timeout, commandContractAttribute.Prepare);

				returnValue = scalarValue;
			}
			else if (returnParameterType == typeof(DataSet))
			{
				IDataReader dataReader;
				DataSet dataSet;

				using (dataReader = this.database.ExecuteReader(commandContractAttribute.Type, commandContractAttribute.Text, dataParameters.ToArray(), commandContractAttribute.Behavior, commandContractAttribute.UseDefaultTimeout ? (int?)null : commandContractAttribute.Timeout, commandContractAttribute.Prepare))
					dataSet = AdoNetHelper.FillDSFromDR(dataReader, returnContractAttribute.DataSetName, returnContractAttribute.SourceTableName);

				returnValue = dataSet;
			}
			else
				throw new InvalidOperationException(string.Format("The type::method '{0}::{1}' specifies an unsupported return type '{2}'.", contractType.FullName, invokedMethodInfo.Name, returnParameterType.FullName));

			// extract output parameter values
			foreach (KeyValuePair<int, IDataParameter> outputMappingParameter in outputDataParameters)
				invocationParameters[outputMappingParameter.Key] = outputMappingParameter.Value.Value;

			if ((object)returnParameterType == null ||
			    returnParameterType == typeof(void))
				returnValue = null;

			return returnValue;
		}

		#endregion
	}
}