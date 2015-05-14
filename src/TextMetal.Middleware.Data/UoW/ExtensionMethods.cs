/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using TextMetal.Middleware.Common.Utilities;

namespace TextMetal.Middleware.Data.UoW
{
	/// <summary>
	/// Provides extension methods for unit of work instances.
	/// </summary>
	public static class ExtensionMethods
	{
		#region Methods/Operators

		/// <summary>
		/// An extension method to create a new data parameter from the data source.
		/// </summary>
		/// <param name="unitOfWork"> The target unit of work. </param>
		/// <param name="parameterDirection"> Specifies the parameter direction. </param>
		/// <param name="dbType"> Specifies the parameter provider-(in)dependent type. </param>
		/// <param name="parameterSize"> Specifies the parameter size. </param>
		/// <param name="parameterPrecision"> Specifies the parameter precision. </param>
		/// <param name="parameterScale"> Specifies the parameter scale. </param>
		/// <param name="parameterNullable"> Specifies the parameter nullable-ness. </param>
		/// <param name="parameterName"> Specifies the parameter name. </param>
		/// <param name="parameterValue"> Specifies the parameter value. </param>
		/// <returns> The data parameter with the specified properties set. </returns>
		public static IDbDataParameter CreateParameter(this IUnitOfWork unitOfWork, ParameterDirection parameterDirection, DbType dbType, int parameterSize, byte parameterPrecision, byte parameterScale, bool parameterNullable, string parameterName, object parameterValue)
		{
			IDbDataParameter dbDataParameter;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			dbDataParameter = AdoNetFascade.Instance.CreateParameter(unitOfWork.Connection, unitOfWork.Transaction, parameterDirection, dbType, parameterSize, parameterPrecision, parameterScale, parameterNullable, parameterName, parameterValue);

			return dbDataParameter;
		}

		/// <summary>
		/// An extension method to execute a dictionary query operation against a target unit of work.
		/// This overload is for backwards compatability.
		/// DO NOT DISPOSE OF UNIT OF WORK CONTEXT - UP TO THE CALLER.
		/// </summary>
		/// <param name="unitOfWork"> The target unit of work. </param>
		/// <param name="commandType"> The type of the command. </param>
		/// <param name="commandText"> The SQL text or stored procedure name. </param>
		/// <param name="commandParameters"> The parameters to use during the operation. </param>
		/// <param name="recordsAffected"> The output count of records affected. </param>
		/// <returns> A list of dictionary instances, containing key/value pairs of data. </returns>
		public static IList<IDictionary<string, object>> ExecuteDictionary(this IUnitOfWork unitOfWork, CommandType commandType, string commandText, IEnumerable<IDbDataParameter> commandParameters, out int recordsAffected)
		{
			int _recordsAffected;
			List<IDictionary<string, object>> list;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			_recordsAffected = -1;

			// FORCE EAGER LOADING HERE
			list = AdoNetFascade.Instance.ExecuteDictionary(unitOfWork.Connection, unitOfWork.Transaction, commandType, commandText, commandParameters, (ra) => _recordsAffected = ra).ToList();

			recordsAffected = _recordsAffected;

			return list;
		}

		/// <summary>
		/// An extension method to execute a dictionary query operation against a target unit of work.
		/// DO NOT DISPOSE OF UNIT OF WORK CONTEXT - UP TO THE CALLER.
		/// </summary>
		/// <param name="unitOfWork"> The target unit of work. </param>
		/// <param name="commandType"> The type of the command. </param>
		/// <param name="commandText"> The SQL text or stored procedure name. </param>
		/// <param name="commandParameters"> The parameters to use during the operation. </param>
		/// <param name="recordsAffectedCallback"> Executed when the output count of records affected is available to return (post enumeration). </param>
		/// <returns> An enumerable of dictionary instances, containing key/value pairs of data. </returns>
		public static IEnumerable<IDictionary<string, object>> ExecuteDictionary(this IUnitOfWork unitOfWork, CommandType commandType, string commandText, IEnumerable<IDbDataParameter> commandParameters, Action<int> recordsAffectedCallback)
		{
			IEnumerable<IDictionary<string, object>> retval;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			// DO NOT DISPOSE OF DATA READER HERE - THE YIELD STATE MACHINE BELOW WILL DO THIS
			retval = AdoNetFascade.Instance.ExecuteDictionary(unitOfWork.Connection, unitOfWork.Transaction, commandType, commandText, commandParameters, recordsAffectedCallback);

			return retval;
		}

		public static TValue ExecuteScalar<TValue>(this IUnitOfWork unitOfWork, CommandType commandType, string commandText, IEnumerable<IDbDataParameter> commandParameters)
		{
			int recordsAffected;
			IEnumerable<IDictionary<string, object>> results;
			IDictionary<string, object> result;
			object dbValue;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			results = AdoNetFascade.Instance.ExecuteDictionary(unitOfWork.Connection, unitOfWork.Transaction, commandType, commandText, commandParameters, out recordsAffected);

			if ((object)results == null)
				return default(TValue);

			result = results.SingleOrDefault();

			if ((object)result == null)
				return default(TValue);

			if (result.Count != 1)
				return default(TValue);

			if (result.Keys.Count != 1)
				return default(TValue);

			dbValue = result[result.Keys.First()];

			return dbValue.ChangeType<TValue>();
		}

		/// <summary>
		/// An extension method to execute a schema query operation against a target unit of work.
		/// DO NOT DISPOSE OF UNIT OF WORK CONTEXT - UP TO THE CALLER.
		/// </summary>
		/// <param name="unitOfWork"> The target unit of work. </param>
		/// <param name="commandType"> The type of the command. </param>
		/// <param name="commandText"> The SQL text or stored procedure name. </param>
		/// <param name="commandParameters"> The parameters to use during the operation. </param>
		/// <param name="recordsAffected"> The output count of records affected. </param>
		/// <returns> A list of dictionary instances, containing key/value pairs of data. </returns>
		public static IList<IDictionary<string, object>> ExecuteSchema(this IUnitOfWork unitOfWork, CommandType commandType, string commandText, IEnumerable<IDbDataParameter> commandParameters, out int recordsAffected)
		{
			int _recordsAffected = -1;
			var list = AdoNetFascade.Instance.ExecuteSchema(unitOfWork.Connection, unitOfWork.Transaction, commandType, commandText, commandParameters, (ra) => _recordsAffected = ra).ToList(); // FORCE EAGER LOADING HERE
			recordsAffected = _recordsAffected;
			return list;
		}

		/// <summary>
		/// An extension method to execute a schema query operation against a target unit of work.
		/// DO NOT DISPOSE OF UNIT OF WORK CONTEXT - UP TO THE CALLER.
		/// </summary>
		/// <param name="unitOfWork"> The target unit of work. </param>
		/// <param name="commandType"> The type of the command. </param>
		/// <param name="commandText"> The SQL text or stored procedure name. </param>
		/// <param name="commandParameters"> The parameters to use during the operation. </param>
		/// <param name="recordsAffectedCallback"> Executed when the output count of records affected is available to return (post enumeration). </param>
		/// <returns> An enumerable of dictionary instances, containing key/value pairs of schema data. </returns>
		public static IEnumerable<IDictionary<string, object>> ExecuteSchema(this IUnitOfWork unitOfWork, CommandType commandType, string commandText, IEnumerable<IDbDataParameter> commandParameters, Action<int> recordsAffectedCallback)
		{
			IEnumerable<IDictionary<string, object>> retval;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			retval = AdoNetFascade.Instance.ExecuteSchema(unitOfWork.Connection, unitOfWork.Transaction, commandType, commandText, commandParameters, recordsAffectedCallback);

			return retval;
		}

		public static IDictionary<string, object> GetOutputAsRecord(this IEnumerable<IDbDataParameter> dbDataParameters)
		{
			IDictionary<string, object> output;

			if ((object)dbDataParameters == null)
				throw new ArgumentNullException("dbDataParameters");

			output = new Dictionary<string, object>();

			foreach (IDbDataParameter dbDataParameter in dbDataParameters)
			{
				if (dbDataParameter.Direction != ParameterDirection.InputOutput &&
					dbDataParameter.Direction != ParameterDirection.Output &&
					dbDataParameter.Direction != ParameterDirection.ReturnValue)
					continue;

				output.Add(dbDataParameter.ParameterName, dbDataParameter.Value);
			}

			return output;
		}

		#endregion
	}
}