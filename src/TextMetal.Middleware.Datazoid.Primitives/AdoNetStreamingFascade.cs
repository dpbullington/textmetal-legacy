/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;

using TextMetal.Middleware.Solder.Extensions;
using TextMetal.Middleware.Solder.Injection;
using TextMetal.Middleware.Solder.Primitives;
using TextMetal.Middleware.Solder.Utilities;

namespace TextMetal.Middleware.Datazoid.Primitives
{
	public class AdoNetStreamingFascade : AdoNetBufferingFascade, IAdoNetStreamingFascade
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the AdoNetStreamingFascade class.
		/// </summary>
		/// <param name="reflectionFascade"> The reflection fascade instance to use. </param>
		/// <param name="dataTypeFascade"> The data type fascade instance to use. </param>
		[DependencyInjection]
		public AdoNetStreamingFascade([DependencyInjection] IReflectionFascade reflectionFascade, [DependencyInjection] IDataTypeFascade dataTypeFascade)
			: base(reflectionFascade, dataTypeFascade)
		{
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Create a new data parameter from the data source.
		/// </summary>
		/// <param name="dbConnection"> The database connection. </param>
		/// <param name="dbTransaction"> An optional local database transaction. </param>
		/// <param name="sourceColumn"> Specifies the source column. </param>
		/// <param name="parameterDirection"> Specifies the parameter direction. </param>
		/// <param name="parameterDbType"> Specifies the parameter provider-(in)dependent type. </param>
		/// <param name="parameterSize"> Specifies the parameter size. </param>
		/// <param name="parameterPrecision"> Specifies the parameter precision. </param>
		/// <param name="parameterScale"> Specifies the parameter scale. </param>
		/// <param name="parameterNullable"> Specifies the parameter nullable-ness. </param>
		/// <param name="parameterName"> Specifies the parameter name. </param>
		/// <param name="parameterValue"> Specifies the parameter value. </param>
		/// <returns> The data parameter with the specified properties set. </returns>
		public DbParameter CreateParameter(DbConnection dbConnection, DbTransaction dbTransaction, string sourceColumn, ParameterDirection parameterDirection, DbType parameterDbType, int parameterSize, byte parameterPrecision, byte parameterScale, bool parameterNullable, string parameterName, object parameterValue)
		{
			DbParameter dbParameter;

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::CreateParameter(...): enter", typeof(AdoNetStreamingFascade).Name));

			if ((object)dbConnection == null)
				throw new ArgumentNullException(nameof(dbConnection));

			using (DbCommand dbCommand = dbConnection.CreateCommand())
				dbParameter = dbCommand.CreateParameter();

			dbParameter.ParameterName = parameterName;
			dbParameter.Size = parameterSize;
			dbParameter.Value = parameterValue;
			dbParameter.Direction = parameterDirection;
			dbParameter.DbType = parameterDbType;
			this.ReflectionFascade.SetLogicalPropertyValue(dbParameter, "IsNullable", parameterNullable, true, false);
			dbParameter.Precision = parameterPrecision;
			dbParameter.Scale = parameterScale;
			dbParameter.SourceColumn = sourceColumn;

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::CreateParameter(...): return parameter", typeof(AdoNetStreamingFascade).Name));

			return dbParameter;
		}

		/// <summary>
		/// Executes a command, returning a data reader, against a data source.
		/// This method DOES NOT DISPOSE OF CONNECTION/TRANSACTION - UP TO THE CALLER.
		/// This method DOES NOT DISPOSE OF DATA READER - UP TO THE CALLER.
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
		public DbDataReader ExecuteReader(DbConnection dbConnection, DbTransaction dbTransaction, CommandType commandType, string commandText, IEnumerable<DbParameter> commandParameters, CommandBehavior commandBehavior, int? commandTimeout, bool commandPrepare)
		{
			DbDataReader dbDataReader;

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::ExecuteReader(...): enter", typeof(AdoNetStreamingFascade).Name));

			if ((object)dbConnection == null)
				throw new ArgumentNullException(nameof(dbConnection));

			using (DbCommand dbCommand = dbConnection.CreateCommand())
			{
				dbCommand.Transaction = dbTransaction;
				dbCommand.CommandType = commandType;
				dbCommand.CommandText = commandText;

				if ((object)commandTimeout != null)
					dbCommand.CommandTimeout = (int)commandTimeout;

				// add parameters
				if ((object)commandParameters != null)
				{
					foreach (DbParameter commandParameter in commandParameters)
					{
						if ((object)commandParameter.Value == null)
							commandParameter.Value = DBNull.Value;

						dbCommand.Parameters.Add(commandParameter);
					}
				}

				if (commandPrepare)
					dbCommand.Prepare();

				// do the database work
				dbDataReader = dbCommand.ExecuteReader(commandBehavior);

				// wrap reader with proxy
				dbDataReader = new WrappedDbDataReader.__(dbDataReader);

				// clean out parameters
				//dbCommand.Parameters.Clear();

				OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::ExecuteReader(...): return reader", typeof(AdoNetStreamingFascade).Name));

				return dbDataReader;
			}
		}

		/// <summary>
		/// Execute a command against a data source, mapping the data reader to an enumerable of record dictionaries.
		/// This method perfoms LAZY LOADING/DEFERRED EXECUTION.
		/// This method DOES NOT DISPOSE OF CONNECTION/TRANSACTION - UP TO THE CALLER.
		/// </summary>
		/// <param name="dbConnection"> The database connection. </param>
		/// <param name="dbTransaction"> An optional local database transaction. </param>
		/// <param name="commandType"> The type of the command. </param>
		/// <param name="commandText"> The SQL text or stored procedure name. </param>
		/// <param name="commandParameters"> The parameters to use during the operation. </param>
		/// ///
		/// <param name="recordsAffectedCallback"> Executed when the output count of records affected is available to return (post enumeration). </param>
		/// <returns> An enumerable of resultset instances, each containing an enumerable of dictionaries with record key/value pairs of schema metadata. </returns>
		public IEnumerable<IRecord> ExecuteRecords(DbConnection dbConnection, DbTransaction dbTransaction, CommandType commandType, string commandText, IEnumerable<DbParameter> commandParameters, Action<int> recordsAffectedCallback)
		{
			IEnumerable<IRecord> records;
			DbDataReader dbDataReader;

			// force no preparation
			const bool COMMAND_PREPARE = false;

			// force provider default timeout
			const object COMMAND_TIMEOUT = null; /*int?*/

			// force command behavior to default; the unit of work will manage connection lifetime
			const CommandBehavior COMMAND_BEHAVIOR = CommandBehavior.Default;

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::ExecuteRecords(...): enter", typeof(AdoNetStreamingFascade).Name));

			if ((object)dbConnection == null)
				throw new ArgumentNullException(nameof(dbConnection));

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::ExecuteRecords(...): before yield", typeof(AdoNetStreamingFascade).Name));

			// MUST DISPOSE WITHIN A NEW YIELD STATE MACHINE
			using (dbDataReader = this.ExecuteReader(dbConnection, dbTransaction, commandType, commandText, commandParameters, COMMAND_BEHAVIOR, (int?)COMMAND_TIMEOUT, COMMAND_PREPARE))
			{
				OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::ExecuteRecords(...): use reader", typeof(AdoNetStreamingFascade).Name));

				records = this.GetRecordsFromReader(dbDataReader, recordsAffectedCallback);

				foreach (IRecord record in records)
				{
					OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::ExecuteRecords(...): on yield", typeof(AdoNetStreamingFascade).Name));

					yield return record;
				}

				OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::ExecuteRecords(...): dispose reader", typeof(AdoNetStreamingFascade).Name));
			}

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::ExecuteRecords(...): after yield", typeof(AdoNetStreamingFascade).Name));

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::ExecuteRecords(...): leave", typeof(AdoNetStreamingFascade).Name));
		}

		/// <summary>
		/// Execute a command against a data source, mapping the data reader to an enumerable of resultsets, each with an enumerable of record dictionaries.
		/// This method perfoms LAZY LOADING/DEFERRED EXECUTION.
		/// This method DOES NOT DISPOSE OF CONNECTION/TRANSACTION - UP TO THE CALLER.
		/// </summary>
		/// <param name="dbConnection"> The database connection. </param>
		/// <param name="dbTransaction"> An optional local database transaction. </param>
		/// <param name="commandType"> The type of the command. </param>
		/// <param name="commandText"> The SQL text or stored procedure name. </param>
		/// <param name="commandParameters"> The parameters to use during the operation. </param>
		/// <returns> An enumerable of resultset instances, each containing an enumerable of dictionaries with record key/value pairs of data. </returns>
		public IEnumerable<IResultset> ExecuteResultsets(DbConnection dbConnection, DbTransaction dbTransaction, CommandType commandType, string commandText, IEnumerable<DbParameter> commandParameters)
		{
			IEnumerable<IResultset> resultsets;
			DbDataReader dbDataReader;

			// force no preparation
			const bool COMMAND_PREPARE = false;

			// force provider default timeout
			const object COMMAND_TIMEOUT = null; /*int?*/

			// force command behavior to default; the unit of work will manage connection lifetime
			const CommandBehavior COMMAND_BEHAVIOR = CommandBehavior.Default;

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::ExecuteResultsets(...): enter", typeof(AdoNetStreamingFascade).Name));

			if ((object)dbConnection == null)
				throw new ArgumentNullException(nameof(dbConnection));

			// DO NOT DISPOSE OF DATA READER HERE - THE YIELD STATE MACHINE BELOW WILL DO THIS
			dbDataReader = this.ExecuteReader(dbConnection, dbTransaction, commandType, commandText, commandParameters, COMMAND_BEHAVIOR, (int?)COMMAND_TIMEOUT, COMMAND_PREPARE);
			resultsets = this.GetResultsetsFromReader(dbDataReader);

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::ExecuteResultsets(...): return resultsets", typeof(AdoNetStreamingFascade).Name));

			return resultsets;
		}

		/// <summary>
		/// Execute a command against a data source, mapping the data reader GetSchemaTable() result to an enumerable of enumerable of record dictionaries.
		/// This method perfoms LAZY LOADING/DEFERRED EXECUTION.
		/// This method DOES NOT DISPOSE OF CONNECTION/TRANSACTION - UP TO THE CALLER.
		/// </summary>
		/// <param name="dbConnection"> The database connection. </param>
		/// <param name="dbTransaction"> An optional local database transaction. </param>
		/// <param name="commandType"> The type of the command. </param>
		/// <param name="commandText"> The SQL text or stored procedure name. </param>
		/// <param name="commandParameters"> The parameters to use during the operation. </param>
		/// <param name="recordsAffectedCallback"> Executed when the output count of records affected is available to return (post enumeration). </param>
		/// <returns> An enumerable of resultset instances, each containing an enumerable of dictionaries with record key/value pairs of schema metadata. </returns>
		public IEnumerable<IRecord> ExecuteSchemaRecords(DbConnection dbConnection, DbTransaction dbTransaction, CommandType commandType, string commandText, IEnumerable<DbParameter> commandParameters, Action<int> recordsAffectedCallback)
		{
			IEnumerable<IRecord> records;
			DbDataReader dbDataReader;

			// force no preparation
			const bool COMMAND_PREPARE = false;

			// force provider default timeout
			const object COMMAND_TIMEOUT = null; /*int?*/

			// force command behavior to default; the unit of work will manage connection lifetime
			const CommandBehavior COMMAND_BEHAVIOR = CommandBehavior.Default;

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::ExecuteSchemaRecords(...): enter", typeof(AdoNetStreamingFascade).Name));

			if ((object)dbConnection == null)
				throw new ArgumentNullException(nameof(dbConnection));

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::ExecuteSchemaRecords: before yield", typeof(AdoNetStreamingFascade).Name));

			// MUST DISPOSE WITHIN A NEW YIELD STATE MACHINE
			using (dbDataReader = this.ExecuteReader(dbConnection, dbTransaction, commandType, commandText, commandParameters, COMMAND_BEHAVIOR, (int?)COMMAND_TIMEOUT, COMMAND_PREPARE))
			{
				OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::ExecuteSchemaRecords: use reader", typeof(AdoNetStreamingFascade).Name));

				records = this.GetSchemaRecordsFromReader(dbDataReader, recordsAffectedCallback);

				foreach (IRecord record in records)
				{
					OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::ExecuteSchemaRecords: on yield", typeof(AdoNetStreamingFascade).Name));

					yield return record;
				}

				OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::ExecuteSchemaRecords: dispose reader", typeof(AdoNetStreamingFascade).Name));
			}

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::ExecuteSchemaRecords: after yield", typeof(AdoNetStreamingFascade).Name));

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::ExecuteSchemaRecords(...): leave", typeof(AdoNetStreamingFascade).Name));
		}

		/// <summary>
		/// Execute a command against a data source, mapping the data reader GetSchemaTable() result to an resultsets, each with an enumerable of record dictionaries.
		/// This method perfoms LAZY LOADING/DEFERRED EXECUTION.
		/// This method DOES NOT DISPOSE OF CONNECTION/TRANSACTION - UP TO THE CALLER.
		/// </summary>
		/// <param name="dbConnection"> The database connection. </param>
		/// <param name="dbTransaction"> An optional local database transaction. </param>
		/// <param name="commandType"> The type of the command. </param>
		/// <param name="commandText"> The SQL text or stored procedure name. </param>
		/// <param name="commandParameters"> The parameters to use during the operation. </param>
		/// <returns> An enumerable of resultset instances, each containing an enumerable of dictionaries with record key/value pairs of schema metadata. </returns>
		public IEnumerable<IResultset> ExecuteSchemaResultsets(DbConnection dbConnection, DbTransaction dbTransaction, CommandType commandType, string commandText, IEnumerable<DbParameter> commandParameters)
		{
			IEnumerable<IResultset> resultsets;
			DbDataReader dbDataReader;

			// force no preparation
			const bool COMMAND_PREPARE = false;

			// force provider default timeout
			const object COMMAND_TIMEOUT = null; /*int?*/

			// force command behavior to default; the unit of work will manage connection lifetime
			const CommandBehavior COMMAND_BEHAVIOR = CommandBehavior.SchemaOnly;

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::ExecuteSchemaResultsets(...): enter", typeof(AdoNetStreamingFascade).Name));

			if ((object)dbConnection == null)
				throw new ArgumentNullException(nameof(dbConnection));

			// DO NOT DISPOSE OF DATA READER HERE - THE YIELD STATE MACHINE BELOW WILL DO THIS
			dbDataReader = this.ExecuteReader(dbConnection, dbTransaction, commandType, commandText, commandParameters, COMMAND_BEHAVIOR, (int?)COMMAND_TIMEOUT, COMMAND_PREPARE);
			resultsets = this.GetSchemaResultsetsFromReader(dbDataReader);

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::ExecuteSchemaResultsets(...): return resultsets", typeof(AdoNetStreamingFascade).Name));

			return resultsets;
		}

		/// <summary>
		/// Execute a command against a data source, mapping the data reader to an enumerable of record dictionaries.
		/// This method perfoms LAZY LOADING/DEFERRED EXECUTION.
		/// Note that THE DATA READER WILL NOT BE DISPOSED UPON ENUMERATION OR FOREACH BRANCH OUT.
		/// </summary>
		/// <param name="dbDataReader"> The target data reader. </param>
		/// <param name="recordsAffectedCallback"> Executed when the output count of records affected is available to return (post enumeration). </param>
		/// <returns> An enumerable of record dictionary instances, containing key/value pairs of data. </returns>
		public IEnumerable<IRecord> GetRecordsFromReader(DbDataReader dbDataReader, Action<int> recordsAffectedCallback)
		{
			IRecord record;
			int recordsAffected;
			int recordIndex = 0;
			string key;
			object value;

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetRecordsFromReader(...): enter", typeof(AdoNetStreamingFascade).Name));

			if ((object)dbDataReader == null)
				throw new ArgumentNullException(nameof(dbDataReader));

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetRecordsFromReader(...): before yield", typeof(AdoNetStreamingFascade).Name));

			while (dbDataReader.Read())
			{
				record = new Record();

				for (int columnIndex = 0; columnIndex < dbDataReader.FieldCount; columnIndex++)
				{
					key = dbDataReader.GetName(columnIndex);
					value = dbDataReader.GetValue(columnIndex).ChangeType<object>();

					if (record.ContainsKey(key) || (key ?? string.Empty).Length == 0)
						key = string.Format("Column_{0:0000}", columnIndex);

					record.Add(key, value);
				}

				OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetRecordsFromReader(...): on yield", typeof(AdoNetStreamingFascade).Name));

				yield return record; // LAZY PROCESSING INTENT HERE / DO NOT FORCE EAGER LOAD
			}

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetRecordsFromReader(...): after yield", typeof(AdoNetStreamingFascade).Name));

			recordsAffected = dbDataReader.RecordsAffected;

			if ((object)recordsAffectedCallback != null)
				recordsAffectedCallback(recordsAffected);

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetRecordsFromReader(...): leave", typeof(AdoNetStreamingFascade).Name));
		}

		/// <summary>
		/// Execute a command against a data source, mapping the data reader to an enumerable of resultsets, each with an enumerable of records dictionaries.
		/// This method perfoms LAZY LOADING/DEFERRED EXECUTION.
		/// </summary>
		/// <param name="dbDataReader"> The target data reader. </param>
		/// <returns> An enumerable of resultset instances, each containing an enumerable of dictionaries with record key/value pairs of data. </returns>
		public IEnumerable<IResultset> GetResultsetsFromReader(DbDataReader dbDataReader)
		{
			int resultsetIndex = 0;

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetResultsetsFromReader(...): enter", typeof(AdoNetStreamingFascade).Name));

			if ((object)dbDataReader == null)
				throw new ArgumentNullException(nameof(dbDataReader));

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetResultsetsFromReader(...): before yield", typeof(AdoNetStreamingFascade).Name));

			using (dbDataReader)
			{
				OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetResultsetsFromReader(...): use reader", typeof(AdoNetStreamingFascade).Name));

				do
				{
					Resultset resultset = new Resultset(resultsetIndex++); // prevent modified closure
					resultset.Records = this.GetRecordsFromReader(dbDataReader, (ra) => resultset.RecordsAffected = ra);

					OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetResultsetsFromReader(...): on yield", typeof(AdoNetStreamingFascade).Name));

					yield return resultset; // LAZY PROCESSING INTENT HERE / DO NOT FORCE EAGER LOAD
				}
				while (dbDataReader.NextResult());

				OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetResultsetsFromReader(...): dispose reader", typeof(AdoNetStreamingFascade).Name));
			}

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetResultsetsFromReader(...): after yield", typeof(AdoNetStreamingFascade).Name));

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetResultsetsFromReader(...): leave", typeof(AdoNetStreamingFascade).Name));
		}

		/// <summary>
		/// Execute a command against a data source, mapping the data reader GetSchemaTable() result to an enumerable of record dictionaries.
		/// This method perfoms LAZY LOADING/DEFERRED EXECUTION.
		/// Note that THE DATA READER WILL NOT BE DISPOSED UPON ENUMERATION OR FOREACH BRANCH OUT.
		/// </summary>
		/// <param name="dbDataReader"> The target data reader. </param>
		/// <param name="recordsAffectedCallback"> Executed when the output count of records affected is available to return (post enumeration). </param>
		/// <returns> An enumerable of record dictionary instances, containing key/value pairs of schema metadata. </returns>
		public IEnumerable<IRecord> GetSchemaRecordsFromReader(DbDataReader dbDataReader, Action<int> recordsAffectedCallback)
		{
			ReadOnlyCollection<DbColumn> dbColumns;
			DbColumn dbColumn;
			PropertyInfo[] propertyInfos;
			PropertyInfo propertyInfo;
			Record record;
			int recordsAffected;
			string key;
			object value;

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetSchemaRecordsFromReader(...): enter", typeof(AdoNetStreamingFascade).Name));

			if ((object)dbDataReader == null)
				throw new ArgumentNullException(nameof(dbDataReader));

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetSchemaRecordsFromReader(...): before yield", typeof(AdoNetStreamingFascade).Name));

			if(!dbDataReader.CanGetColumnSchema())
				throw new NotSupportedException(string.Format("The connection command type '{0}' does not support schema access.", dbDataReader.GetType().FullName));

			dbColumns = dbDataReader.GetColumnSchema();
			{
				OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetSchemaRecordsFromReader(...): use table", typeof(AdoNetStreamingFascade).Name));

				if ((object)dbColumns != null)
				{
					for (int index = 0; index < dbColumns.Count; index++)
					{
						dbColumn = dbColumns[index];

						propertyInfos = dbColumn.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);

						record = new Record();
						record.TagContext = dbColumn;

						if ((object)propertyInfos != null)
						{
							for (int i = 0; i < propertyInfos.Length; i++)
							{
								propertyInfo = propertyInfos[i];

								if (propertyInfo.GetIndexParameters().Any())
									continue;

								key = propertyInfo.Name;
								value = propertyInfo.GetValue(dbColumn);
								value = value.ChangeType<object>();

								record.Add(key, value);
							}
						}

						OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetSchemaRecordsFromReader(...): on yield", typeof(AdoNetStreamingFascade).Name));

						yield return record; // LAZY PROCESSING INTENT HERE / DO NOT FORCE EAGER LOAD
					}
				}

				OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetSchemaRecordsFromReader(...): dispose table", typeof(AdoNetStreamingFascade).Name));
			}

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetSchemaRecordsFromReader(...): after yield", typeof(AdoNetStreamingFascade).Name));

			recordsAffected = dbDataReader.RecordsAffected;

			if ((object)recordsAffectedCallback != null)
				recordsAffectedCallback(recordsAffected);

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetSchemaRecordsFromReader(...): leave", typeof(AdoNetStreamingFascade).Name));
		}

		/// <summary>
		/// Execute a command against a data source, mapping the data reader GetSchemaTable() result to an enumerable of resultsets, each with an enumerable of records dictionaries.
		/// This method perfoms LAZY LOADING/DEFERRED EXECUTION.
		/// </summary>
		/// <param name="dbDataReader"> The target data reader. </param>
		/// <returns> An enumerable of resultset instances, each containing an enumerable of dictionaries with record key/value pairs of schema metadata. </returns>
		public IEnumerable<IResultset> GetSchemaResultsetsFromReader(DbDataReader dbDataReader)
		{
			int resultsetIndex = 0;

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetSchemaResultsetsFromReader(...): enter", typeof(AdoNetStreamingFascade).Name));

			if ((object)dbDataReader == null)
				throw new ArgumentNullException(nameof(dbDataReader));

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetSchemaResultsetsFromReader(...): before yield", typeof(AdoNetStreamingFascade).Name));

			using (dbDataReader)
			{
				OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetSchemaResultsetsFromReader(...): use reader", typeof(AdoNetStreamingFascade).Name));

				do
				{
					Resultset resultset = new Resultset(resultsetIndex++); // prevent modified closure
					resultset.Records = this.GetSchemaRecordsFromReader(dbDataReader, (ra) => resultset.RecordsAffected = ra);

					OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetSchemaResultsetsFromReader(...): on yield", typeof(AdoNetStreamingFascade).Name));

					yield return resultset; // LAZY PROCESSING INTENT HERE / DO NOT FORCE EAGER LOAD
				}
				while (dbDataReader.NextResult());

				OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetSchemaResultsetsFromReader(...): dispose reader", typeof(AdoNetStreamingFascade).Name));
			}

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetSchemaResultsetsFromReader(...): after yield", typeof(AdoNetStreamingFascade).Name));

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetSchemaResultsetsFromReader(...): leave", typeof(AdoNetStreamingFascade).Name));
		}

		#endregion
	}
}