/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Xml;

using TextMetal.Common.Core;

namespace TextMetal.Common.Data
{
	/// <summary>
	/// Provides static helper and/or extension methods for ADO.NET.
	/// </summary>
	public static class AdoNetHelper
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

			if ((object)unitOfWork.Connection == null)
				throw new InvalidOperationException("There is not a valid connection associated with the current unit of work.");

			using (IDbCommand dbCommand = unitOfWork.Connection.CreateCommand())
				dbDataParameter = dbCommand.CreateParameter();

			dbDataParameter.ParameterName = parameterName;
			dbDataParameter.Size = parameterSize;
			dbDataParameter.Value = parameterValue;
			dbDataParameter.Direction = parameterDirection;
			dbDataParameter.DbType = dbType;
			Reflexion.SetLogicalPropertyValue(dbDataParameter, "IsNullable", parameterNullable, true, false);
			dbDataParameter.Precision = parameterPrecision;
			dbDataParameter.Scale = parameterScale;

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
			int _recordsAffected = -1;
			var list = ExecuteDictionary(unitOfWork, commandType, commandText, commandParameters, (ra) => _recordsAffected = ra).ToList(); // FORCE EAGER LOADING HERE
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
			IDictionary<string, object> obj;
			IDataReader dataReader;
			int recordsAffected;

			// force no preparation
			const bool COMMAND_PREPARE = false;

			// force provider default timeout
			const object COMMAND_TIMEOUT = null; /*int?*/

			// force command behavior to default; the unit of work will manage connection lifetime
			const CommandBehavior COMMAND_BEHAVIOR = CommandBehavior.Default;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)unitOfWork.Connection == null)
				throw new InvalidOperationException("There is not a valid connection associated with the current unit of work.");

			Trace.WriteLine("[+++ before yield: ExecuteDictionary +++]");

			using (dataReader = ExecuteReader(unitOfWork.Connection, unitOfWork.Transaction, commandType, commandText, commandParameters, COMMAND_BEHAVIOR, (int?)COMMAND_TIMEOUT, COMMAND_PREPARE))
			{
				//do
				{
					while (dataReader.Read())
					{
						obj = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

						for (int index = 0; index < dataReader.FieldCount; index++)
						{
							string key;
							object value;

							key = dataReader.GetName(index);
							value = dataReader.GetValue(index).ChangeType<object>();

							obj.Add(key, value);
						}

						yield return obj;
					}
				}
				//while (dataReader.NextResult());
			}

			Trace.WriteLine("[+++ after yield: ExecuteDictionary +++]");

			recordsAffected = dataReader.RecordsAffected;

			if ((object)recordsAffectedCallback != null)
				recordsAffectedCallback(recordsAffected);
		}

		/// <summary>
		/// Executes a reader query operation against the database.
		/// </summary>
		/// <param name="dbConnection"> The database connection. </param>
		/// <param name="dbTransaction"> An optional local database transaction. </param>
		/// <param name="commandType"> The type of the command. </param>
		/// <param name="commandText"> The SQL text or stored procedure name. </param>
		/// <param name="commandParameters"> The parameters to use during the operation. </param>
		/// <param name="commandBehavior"> The reader behavior. </param>
		/// <param name="commandTimeout"> The command timeout (use null for default). </param>
		/// <param name="commandPrepare"> Whether to prepare the command at the data source. </param>
		/// <returns> The data reader result. </returns>
		public static IDataReader ExecuteReader(IDbConnection dbConnection, IDbTransaction dbTransaction, CommandType commandType, string commandText, IEnumerable<IDbDataParameter> commandParameters, CommandBehavior commandBehavior, int? commandTimeout, bool commandPrepare)
		{
			IDataReader dataReader;

			if ((object)dbConnection == null)
				throw new ArgumentNullException("dbConnection");

			using (IDbCommand dbCommand = dbConnection.CreateCommand())
			{
				dbCommand.Transaction = dbTransaction;
				dbCommand.CommandType = commandType;
				dbCommand.CommandText = commandText;

				if ((object)commandTimeout != null)
					dbCommand.CommandTimeout = (int)commandTimeout;

				// add parameters
				if ((object)commandParameters != null)
				{
					foreach (IDbDataParameter commandParameter in commandParameters)
					{
						if ((object)commandParameter.Value == null)
							commandParameter.Value = DBNull.Value;

						dbCommand.Parameters.Add(commandParameter);
					}
				}

				if (commandPrepare)
					dbCommand.Prepare();

				// do the database work
				dataReader = dbCommand.ExecuteReader(commandBehavior);

				return new WrapperDataReader(dataReader);
			}
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
			var list = ExecuteSchema(unitOfWork, commandType, commandText, commandParameters, (ra) => _recordsAffected = ra).ToList(); // FORCE EAGER LOADING HERE
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
			IDictionary<string, object> obj;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)unitOfWork.Connection == null)
				throw new InvalidOperationException("There is not a valid connection associated with the current unit of work.");

			// 2011-09-07 (dpbullington@gmail.com / issue #12): found quirk if CommandBehavior == KeyInfo, hidden columns in views get returned; reverting to CommandBehavior == SchemaOnly
			using (IDataReader dataReader = ExecuteReader(unitOfWork.Connection, unitOfWork.Transaction, commandType, commandText, commandParameters, CommandBehavior.SchemaOnly, null, false))
			{
				using (DataTable dataTable = dataReader.GetSchemaTable())
				{
					if ((object)dataTable != null)
					{
						//dataTable.WriteXml(@"out.xml");
						foreach (DataRow dataRow in dataTable.Rows)
						{
							obj = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

							for (int index = 0; index < dataTable.Columns.Count; index++)
							{
								string key;
								object value;

								key = dataTable.Columns[index].ColumnName;
								value = dataRow[index].ChangeType<object>();

								obj.Add(key, value);
							}

							yield return obj;
						}
					}
				}
			}
		}

		public static TValue FetchScalar<TValue>(this IUnitOfWork unitOfWork, CommandType commandType, string commandText, IEnumerable<IDbDataParameter> commandParameters)
		{
			int recordsAffected;
			IEnumerable<IDictionary<string, object>> results;
			IDictionary<string, object> result;
			object dbValue;

			results = unitOfWork.ExecuteDictionary(commandType, commandText, commandParameters, out recordsAffected);

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
		/// Returns a DbType mapping for a Type.
		/// An InvalidOperationException is thrown for unmappable types.
		/// </summary>
		/// <param name="clrType"> The CLR type to map to a DbType. </param>
		/// <returns> The mapped DbType. </returns>
		public static DbType InferDbTypeForClrType(Type clrType)
		{
			if ((object)clrType == null)
				throw new ArgumentNullException("clrType");

			if (clrType.IsByRef /* || type.IsPointer || type.IsArray */)
				return InferDbTypeForClrType(clrType.GetElementType());
			else if (clrType.IsGenericType &&
					!clrType.IsGenericTypeDefinition &&
					clrType.GetGenericTypeDefinition() == typeof(Nullable<>))
				return InferDbTypeForClrType(Nullable.GetUnderlyingType(clrType));
			else if (clrType.IsEnum)
				return InferDbTypeForClrType(Enum.GetUnderlyingType(clrType));
			else if (clrType == typeof(Boolean))
				return DbType.Boolean;
			else if (clrType == typeof(Byte))
				return DbType.Byte;
			else if (clrType == typeof(DateTime))
				return DbType.DateTime;
			else if (clrType == typeof(DateTimeOffset))
				return DbType.DateTimeOffset;
			else if (clrType == typeof(Decimal))
				return DbType.Decimal;
			else if (clrType == typeof(Double))
				return DbType.Double;
			else if (clrType == typeof(Guid))
				return DbType.Guid;
			else if (clrType == typeof(Int16))
				return DbType.Int16;
			else if (clrType == typeof(Int32))
				return DbType.Int32;
			else if (clrType == typeof(Int64))
				return DbType.Int64;
			else if (clrType == typeof(SByte))
				return DbType.SByte;
			else if (clrType == typeof(Single))
				return DbType.Single;
			else if (clrType == typeof(TimeSpan))
				return DbType.Time;
			else if (clrType == typeof(UInt16))
				return DbType.UInt16;
			else if (clrType == typeof(UInt32))
				return DbType.UInt32;
			else if (clrType == typeof(UInt64))
				return DbType.UInt64;
			else if (clrType == typeof(Byte[]))
				return DbType.Binary;
			else if (clrType == typeof(Boolean[]))
				return DbType.Byte;
			else if (clrType == typeof(String))
				return DbType.String;
			else if (clrType == typeof(XmlDocument))
				return DbType.Xml;
			else if (clrType == typeof(Object))
				return DbType.Object;
			else
				throw new InvalidOperationException(string.Format("Cannot infer parameter type from unsupported CLR type '{0}'.", clrType.FullName));
		}

		#endregion
	}
}