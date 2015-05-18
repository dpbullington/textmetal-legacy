/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml;

using TextMetal.Middleware.Common.Utilities;

namespace TextMetal.Middleware.Data
{
	public class AdoNetFascade : IAdoNetFascade
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the AdoNetFascade class.
		/// </summary>
		/// <param name="reflectionFascade"> The reflection instance to use. </param>
		public AdoNetFascade(IReflectionFascade reflectionFascade)
		{
			if ((object)reflectionFascade == null)
				throw new ArgumentNullException("reflectionFascade");

			this.reflectionFascade = reflectionFascade;
		}

		/// <summary>
		/// Initializes a new instance of the AdoNetFascade class.
		/// </summary>
		private AdoNetFascade()
			: this(Common.Utilities.ReflectionFascade.Instance)
		{
		}

		#endregion

		#region Fields/Constants

		private const string RESULTSET_INDEX_RECORD_KEY = "__ResultsetIndex__";
		private static readonly AdoNetFascade instance = new AdoNetFascade();
		private readonly IReflectionFascade reflectionFascade;

		#endregion

		#region Properties/Indexers/Events

		public static AdoNetFascade Instance
		{
			get
			{
				return instance;
			}
		}

		public static string ResultsetIndexRecordKey
		{
			get
			{
				return RESULTSET_INDEX_RECORD_KEY;
			}
		}

		private IReflectionFascade ReflectionFascade
		{
			get
			{
				return this.reflectionFascade;
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Create a new data parameter from the data source.
		/// </summary>
		/// <param name="dbConnection"> The database connection. </param>
		/// <param name="dbTransaction"> An optional local database transaction. </param>
		/// <param name="parameterDirection"> Specifies the parameter direction. </param>
		/// <param name="parameterDbType"> Specifies the parameter provider-(in)dependent type. </param>
		/// <param name="parameterSize"> Specifies the parameter size. </param>
		/// <param name="parameterPrecision"> Specifies the parameter precision. </param>
		/// <param name="parameterScale"> Specifies the parameter scale. </param>
		/// <param name="parameterNullable"> Specifies the parameter nullable-ness. </param>
		/// <param name="parameterName"> Specifies the parameter name. </param>
		/// <param name="parameterValue"> Specifies the parameter value. </param>
		/// <returns> The data parameter with the specified properties set. </returns>
		public IDbDataParameter CreateParameter(IDbConnection dbConnection, IDbTransaction dbTransaction, ParameterDirection parameterDirection, DbType parameterDbType, int parameterSize, byte parameterPrecision, byte parameterScale, bool parameterNullable, string parameterName, object parameterValue)
		{
			IDbDataParameter dbDataParameter;

			if ((object)dbConnection == null)
				throw new ArgumentNullException("dbConnection");

			using (IDbCommand dbCommand = dbConnection.CreateCommand())
				dbDataParameter = dbCommand.CreateParameter();

			dbDataParameter.ParameterName = parameterName;
			dbDataParameter.Size = parameterSize;
			dbDataParameter.Value = parameterValue;
			dbDataParameter.Direction = parameterDirection;
			dbDataParameter.DbType = parameterDbType;
			this.ReflectionFascade.SetLogicalPropertyValue(dbDataParameter, "IsNullable", parameterNullable, true, false);
			dbDataParameter.Precision = parameterPrecision;
			dbDataParameter.Scale = parameterScale;

			return dbDataParameter;
		}

		/// <summary>
		/// Execute a command against a data source, mapping the data reader to a list of dictionaries.
		/// This overload is for backwards compatability; this overload perfoms EAGER LOADING.
		/// DO NOT DISPOSE OF CONNECTION/TRANSACTION - UP TO THE CALLER.
		/// </summary>
		/// <param name="dbConnection"> The database connection. </param>
		/// <param name="dbTransaction"> An optional local database transaction. </param>
		/// <param name="commandType"> The type of the command. </param>
		/// <param name="commandText"> The SQL text or stored procedure name. </param>
		/// <param name="commandParameters"> The parameters to use during the operation. </param>
		/// <param name="recordsAffected"> The output count of records affected. </param>
		/// <returns> A list of dictionary instances, containing key/value pairs of data. </returns>
		public IList<IDictionary<string, object>> ExecuteDictionary(IDbConnection dbConnection, IDbTransaction dbTransaction, CommandType commandType, string commandText, IEnumerable<IDbDataParameter> commandParameters, out int recordsAffected)
		{
			int _recordsAffected;
			List<IDictionary<string, object>> records;

			if ((object)dbConnection == null)
				throw new ArgumentNullException("dbConnection");

			_recordsAffected = -1;

			// FORCE EAGER LOADING HERE
			records = this.ExecuteDictionary(dbConnection, dbTransaction, commandType, commandText, commandParameters, (ra) => _recordsAffected = ra).ToList();

			recordsAffected = _recordsAffected;

			return records;
		}

		/// <summary>
		/// Execute a command against a data source, mapping the data reader to an enumerable of dictionaries.
		/// This overload is for backwards compatability; this overload perfoms LAZY LOADING/DEFERRED EXECUTION.
		/// DO NOT DISPOSE OF CONNECTION/TRANSACTION - UP TO THE CALLER.
		/// </summary>
		/// <param name="dbConnection"> The database connection. </param>
		/// <param name="dbTransaction"> An optional local database transaction. </param>
		/// <param name="commandType"> The type of the command. </param>
		/// <param name="commandText"> The SQL text or stored procedure name. </param>
		/// <param name="commandParameters"> The parameters to use during the operation. </param>
		/// <param name="recordsAffectedCallback"> Executed when the output count of records affected is available to return (post enumeration). </param>
		/// <returns> An enumerable of dictionary instances, containing key/value pairs of data. </returns>
		public IEnumerable<IDictionary<string, object>> ExecuteDictionary(IDbConnection dbConnection, IDbTransaction dbTransaction, CommandType commandType, string commandText, IEnumerable<IDbDataParameter> commandParameters, Action<int> recordsAffectedCallback)
		{
			IEnumerable<IDictionary<string, object>> records;
			IDataReader dataReader;

			// force no preparation
			const bool COMMAND_PREPARE = false;

			// force provider default timeout
			const object COMMAND_TIMEOUT = null; /*int?*/

			// force command behavior to default; the unit of work will manage connection lifetime
			const CommandBehavior COMMAND_BEHAVIOR = CommandBehavior.Default;

			if ((object)dbConnection == null)
				throw new ArgumentNullException("dbConnection");

			// DO NOT DISPOSE OF DATA READER HERE - THE YIELD STATE MACHINE BELOW WILL DO THIS
			dataReader = this.ExecuteReader(dbConnection, dbTransaction, commandType, commandText, commandParameters, COMMAND_BEHAVIOR, (int?)COMMAND_TIMEOUT, COMMAND_PREPARE);
			records = this.GetEnumerableDictionary(dataReader, recordsAffectedCallback);

			return records;
		}

		/// <summary>
		/// Executes a command, returning a data reader, against a data source.
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
		public IDataReader ExecuteReader(IDbConnection dbConnection, IDbTransaction dbTransaction, CommandType commandType, string commandText, IEnumerable<IDbDataParameter> commandParameters, CommandBehavior commandBehavior, int? commandTimeout, bool commandPrepare)
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

				return dataReader;
			}
		}

		/// <summary>
		/// Execute a command against a data source, mapping the data reader GetSchemaTable() result to a list of dictionaries.
		/// This overload is for backwards compatability; this overload perfoms EAGER LOADING.
		/// DO NOT DISPOSE OF CONNECTION/TRANSACTION - UP TO THE CALLER.
		/// </summary>
		/// <param name="dbConnection"> The database connection. </param>
		/// <param name="dbTransaction"> An optional local database transaction. </param>
		/// <param name="commandType"> The type of the command. </param>
		/// <param name="commandText"> The SQL text or stored procedure name. </param>
		/// <param name="commandParameters"> The parameters to use during the operation. </param>
		/// <param name="recordsAffected"> The output count of records affected. </param>
		/// <returns> A list of dictionary instances, containing key/value pairs of schema metadata. </returns>
		public IList<IDictionary<string, object>> ExecuteSchema(IDbConnection dbConnection, IDbTransaction dbTransaction, CommandType commandType, string commandText, IEnumerable<IDbDataParameter> commandParameters, out int recordsAffected)
		{
			int _recordsAffected = -1;
			var records = this.ExecuteSchema(dbConnection, dbTransaction, commandType, commandText, commandParameters, (ra) => _recordsAffected = ra).ToList(); // FORCE EAGER LOADING HERE
			recordsAffected = _recordsAffected;
			return records;
		}

		/// <summary>
		/// Execute a command against a data source, mapping the data reader GetSchemaTable() result to an enumerable of dictionaries.
		/// This overload is for backwards compatability; this overload perfoms LAZY LOADING/DEFERRED EXECUTION.
		/// DO NOT DISPOSE OF CONNECTION/TRANSACTION - UP TO THE CALLER.
		/// </summary>
		/// <param name="dbConnection"> The database connection. </param>
		/// <param name="dbTransaction"> An optional local database transaction. </param>
		/// <param name="commandType"> The type of the command. </param>
		/// <param name="commandText"> The SQL text or stored procedure name. </param>
		/// <param name="commandParameters"> The parameters to use during the operation. </param>
		/// <param name="recordsAffectedCallback"> Executed when the output count of records affected is available to return (post enumeration). </param>
		/// <returns> An enumerable of dictionary instances, containing key/value pairs of schema metadata. </returns>
		public IEnumerable<IDictionary<string, object>> ExecuteSchema(IDbConnection dbConnection, IDbTransaction dbTransaction, CommandType commandType, string commandText, IEnumerable<IDbDataParameter> commandParameters, Action<int> recordsAffectedCallback)
		{
			IEnumerable<IDictionary<string, object>> records;
			IDataReader dataReader;

			// force no preparation
			const bool COMMAND_PREPARE = false;

			// force provider default timeout
			const object COMMAND_TIMEOUT = null; /*int?*/

			// force command behavior to default; the unit of work will manage connection lifetime
			const CommandBehavior COMMAND_BEHAVIOR = CommandBehavior.SchemaOnly;

			if ((object)dbConnection == null)
				throw new ArgumentNullException("dbConnection");

			// DO NOT DISPOSE OF DATA READER HERE - THE YIELD STATE MACHINE BELOW WILL DO THIS
			dataReader = this.ExecuteReader(dbConnection, dbTransaction, commandType, commandText, commandParameters, COMMAND_BEHAVIOR, (int?)COMMAND_TIMEOUT, COMMAND_PREPARE);
			records = this.GetSchemaEnumerableDictionary(dataReader, recordsAffectedCallback);

			return records;
		}

		/// <summary>
		/// Execute a command against a data source, mapping the data reader to an enumerable of dictionaries.
		/// This state machine method (yield return) perfoms LAZY LOADING/DEFERRED EXECUTION.
		/// THE DATA READER WILL BE DISPOSED UPON ENUMERATION OR FOREACH BRANCH OUT.
		/// </summary>
		/// <param name="dataReader"> The target data reader. </param>
		/// <param name="recordsAffectedCallback"> Executed when the output count of records affected is available to return (post enumeration). </param>
		/// <returns> An enumerable of dictionary instances, containing key/value pairs of data. </returns>
		public IEnumerable<IDictionary<string, object>> GetEnumerableDictionary(IDataReader dataReader, Action<int> recordsAffectedCallback)
		{
			IDictionary<string, object> record;
			int recordsAffected;
			int resultsetIndex = 0;
			string key;
			object value;

			if ((object)dataReader == null)
				throw new ArgumentNullException("dataReader");

			//Trace.WriteLine("[+++ before yield: GetEnumerableDictionary +++]");

			using (dataReader)
			{
				do
				{
					while (dataReader.Read())
					{
						record = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

						key = ResultsetIndexRecordKey;
						value = resultsetIndex;

						record.Add(key, value);

						for (int index = 0; index < dataReader.FieldCount; index++)
						{
							key = dataReader.GetName(index);
							value = dataReader.GetValue(index).ChangeType<object>();

							if (record.ContainsKey(key))
								key = string.Format("Column_{0:0000}", index);

							record.Add(key, value);
						}

						yield return record; // LAZY PROCESSING INTENT HERE / DO NOT FORCE EAGER LOAD
					}

					resultsetIndex++;
				}
				while (dataReader.NextResult());
			}

			//Trace.WriteLine("[+++ after yield: GetEnumerableDictionary +++]");

			recordsAffected = dataReader.RecordsAffected;

			if ((object)recordsAffectedCallback != null)
				recordsAffectedCallback(recordsAffected);
		}

		/// <summary>
		/// Execute a command against a data source, mapping the data reader GetSchemaTable() result to an enumerable of dictionaries.
		/// This state machine method (yield return) perfoms LAZY LOADING/DEFERRED EXECUTION.
		/// THE DATA READER WILL BE DISPOSED UPON ENUMERATION OR FOREACH BRANCH OUT.
		/// </summary>
		/// <param name="dataReader"> The target data reader. </param>
		/// <param name="recordsAffectedCallback"> Executed when the output count of records affected is available to return (post enumeration). </param>
		/// <returns> An enumerable of dictionary instances, containing key/value pairs of schema metadata. </returns>
		public IEnumerable<IDictionary<string, object>> GetSchemaEnumerableDictionary(IDataReader dataReader, Action<int> recordsAffectedCallback)
		{
			IDictionary<string, object> record;
			int resultsetIndex = 0;
			string key;
			object value;

			if ((object)dataReader == null)
				throw new ArgumentNullException("dataReader");

			using (dataReader)
			{
				do
				{
					using (DataTable dataTable = dataReader.GetSchemaTable())
					{
						if ((object)dataTable != null)
						{
							foreach (DataRow dataRow in dataTable.Rows)
							{
								record = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

								key = ResultsetIndexRecordKey;
								value = resultsetIndex;

								record.Add(key, value);

								for (int index = 0; index < dataTable.Columns.Count; index++)
								{
									key = dataTable.Columns[index].ColumnName;
									value = dataRow[index].ChangeType<object>();

									record.Add(key, value);
								}

								yield return record; // LAZY PROCESSING INTENT HERE / DO NOT FORCE EAGER LOAD
							}
						}
					}

					resultsetIndex++;
				}
				while (dataReader.NextResult());
			}
		}

		/// <summary>
		/// Returns a DbType mapping for a Type.
		/// An InvalidOperationException is thrown for unmappable types.
		/// </summary>
		/// <param name="clrType"> The CLR type to map to a DbType. </param>
		/// <returns> The mapped DbType. </returns>
		public DbType InferDbTypeForClrType(Type clrType)
		{
			if ((object)clrType == null)
				throw new ArgumentNullException("clrType");

			if (clrType.IsByRef /* || type.IsPointer || type.IsArray */)
				return this.InferDbTypeForClrType(clrType.GetElementType());
			else if (clrType.IsGenericType &&
					!clrType.IsGenericTypeDefinition &&
					clrType.GetGenericTypeDefinition() == typeof(Nullable<>))
				return this.InferDbTypeForClrType(Nullable.GetUnderlyingType(clrType));
			else if (clrType.IsEnum)
				return this.InferDbTypeForClrType(Enum.GetUnderlyingType(clrType));
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