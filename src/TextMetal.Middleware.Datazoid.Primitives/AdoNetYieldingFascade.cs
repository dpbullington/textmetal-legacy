/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

using TextMetal.Middleware.Solder.Extensions;
using TextMetal.Middleware.Solder.Injection;
using TextMetal.Middleware.Solder.Primitives;
using TextMetal.Middleware.Solder.Utilities;

namespace TextMetal.Middleware.Datazoid.Primitives
{
	public class AdoNetYieldingFascade : IAdoNetYieldingFascade
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the AdoNetFascade class.
		/// </summary>
		/// <param name="reflectionFascade"> The reflection instance to use. </param>
		[DependencyInjection]
		public AdoNetYieldingFascade([DependencyInjection] IReflectionFascade reflectionFascade)
		{
			if ((object)reflectionFascade == null)
				throw new ArgumentNullException(nameof(reflectionFascade));

			this.reflectionFascade = reflectionFascade;
		}

		#endregion

		#region Fields/Constants

		private readonly IReflectionFascade reflectionFascade;

		#endregion

		#region Properties/Indexers/Events

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
		public DbParameter CreateParameter(DbConnection dbConnection, DbTransaction dbTransaction, ParameterDirection parameterDirection, DbType parameterDbType, int parameterSize, byte parameterPrecision, byte parameterScale, bool parameterNullable, string parameterName, object parameterValue)
		{
			DbParameter dbParameter;

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::CreateParameter(...): enter", typeof(AdoNetYieldingFascade).Name));

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

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::CreateParameter(...): return parameter", typeof(AdoNetYieldingFascade).Name));

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

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::ExecuteReader(...): enter", typeof(AdoNetYieldingFascade).Name));

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
				dbDataReader = new WrappedDbDataReader(dbDataReader);

				// clean out parameters
				//dbCommand.Parameters.Clear();

				OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::ExecuteReader(...): return reader", typeof(AdoNetYieldingFascade).Name));

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

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::ExecuteRecords(...): enter", typeof(AdoNetYieldingFascade).Name));

			if ((object)dbConnection == null)
				throw new ArgumentNullException(nameof(dbConnection));

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::ExecuteRecords(...): before yield", typeof(AdoNetYieldingFascade).Name));

			// MUST DISPOSE WITHIN A NEW YIELD STATE MACHINE
			using (dbDataReader = this.ExecuteReader(dbConnection, dbTransaction, commandType, commandText, commandParameters, COMMAND_BEHAVIOR, (int?)COMMAND_TIMEOUT, COMMAND_PREPARE))
			{
				OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::ExecuteRecords(...): use reader", typeof(AdoNetYieldingFascade).Name));

				records = this.GetRecordsFromReader(dbDataReader, recordsAffectedCallback);

				foreach (IRecord record in records)
				{
					OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::ExecuteRecords(...): on yield", typeof(AdoNetYieldingFascade).Name));

					yield return record;
				}

				OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::ExecuteRecords(...): dispose reader", typeof(AdoNetYieldingFascade).Name));
			}

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::ExecuteRecords(...): after yield", typeof(AdoNetYieldingFascade).Name));

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::ExecuteRecords(...): leave", typeof(AdoNetYieldingFascade).Name));
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

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::ExecuteResultsets(...): enter", typeof(AdoNetYieldingFascade).Name));

			if ((object)dbConnection == null)
				throw new ArgumentNullException(nameof(dbConnection));

			// DO NOT DISPOSE OF DATA READER HERE - THE YIELD STATE MACHINE BELOW WILL DO THIS
			dbDataReader = this.ExecuteReader(dbConnection, dbTransaction, commandType, commandText, commandParameters, COMMAND_BEHAVIOR, (int?)COMMAND_TIMEOUT, COMMAND_PREPARE);
			resultsets = this.GetResultsetsFromReader(dbDataReader);

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::ExecuteResultsets(...): return resultsets", typeof(AdoNetYieldingFascade).Name));

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

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::ExecuteSchemaRecords(...): enter", typeof(AdoNetYieldingFascade).Name));

			if ((object)dbConnection == null)
				throw new ArgumentNullException(nameof(dbConnection));

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::ExecuteSchemaRecords: before yield", typeof(AdoNetYieldingFascade).Name));

			// MUST DISPOSE WITHIN A NEW YIELD STATE MACHINE
			using (dbDataReader = this.ExecuteReader(dbConnection, dbTransaction, commandType, commandText, commandParameters, COMMAND_BEHAVIOR, (int?)COMMAND_TIMEOUT, COMMAND_PREPARE))
			{
				OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::ExecuteSchemaRecords: use reader", typeof(AdoNetYieldingFascade).Name));

				records = this.GetSchemaRecordsFromReader(dbDataReader, recordsAffectedCallback);

				foreach (IRecord record in records)
				{
					OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::ExecuteSchemaRecords: on yield", typeof(AdoNetYieldingFascade).Name));

					yield return record;
				}

				OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::ExecuteSchemaRecords: dispose reader", typeof(AdoNetYieldingFascade).Name));
			}

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::ExecuteSchemaRecords: after yield", typeof(AdoNetYieldingFascade).Name));

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::ExecuteSchemaRecords(...): leave", typeof(AdoNetYieldingFascade).Name));
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

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::ExecuteSchemaResultsets(...): enter", typeof(AdoNetYieldingFascade).Name));

			if ((object)dbConnection == null)
				throw new ArgumentNullException(nameof(dbConnection));

			// DO NOT DISPOSE OF DATA READER HERE - THE YIELD STATE MACHINE BELOW WILL DO THIS
			dbDataReader = this.ExecuteReader(dbConnection, dbTransaction, commandType, commandText, commandParameters, COMMAND_BEHAVIOR, (int?)COMMAND_TIMEOUT, COMMAND_PREPARE);
			resultsets = this.GetSchemaResultsetsFromReader(dbDataReader);

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::ExecuteSchemaResultsets(...): return resultsets", typeof(AdoNetYieldingFascade).Name));

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

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetRecordsFromReader(...): enter", typeof(AdoNetYieldingFascade).Name));

			if ((object)dbDataReader == null)
				throw new ArgumentNullException(nameof(dbDataReader));

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetRecordsFromReader(...): before yield", typeof(AdoNetYieldingFascade).Name));

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

				OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetRecordsFromReader(...): on yield", typeof(AdoNetYieldingFascade).Name));

				yield return record; // LAZY PROCESSING INTENT HERE / DO NOT FORCE EAGER LOAD
			}

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetRecordsFromReader(...): after yield", typeof(AdoNetYieldingFascade).Name));

			recordsAffected = dbDataReader.RecordsAffected;

			if ((object)recordsAffectedCallback != null)
				recordsAffectedCallback(recordsAffected);

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetRecordsFromReader(...): leave", typeof(AdoNetYieldingFascade).Name));
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

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetResultsetsFromReader(...): enter", typeof(AdoNetYieldingFascade).Name));

			if ((object)dbDataReader == null)
				throw new ArgumentNullException(nameof(dbDataReader));

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetResultsetsFromReader(...): before yield", typeof(AdoNetYieldingFascade).Name));

			using (dbDataReader)
			{
				OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetResultsetsFromReader(...): use reader", typeof(AdoNetYieldingFascade).Name));

				do
				{
					Resultset resultset = new Resultset(resultsetIndex++); // prevent modified closure
					resultset.Records = this.GetRecordsFromReader(dbDataReader, (ra) => resultset.RecordsAffected = ra);

					OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetResultsetsFromReader(...): on yield", typeof(AdoNetYieldingFascade).Name));

					yield return resultset; // LAZY PROCESSING INTENT HERE / DO NOT FORCE EAGER LOAD
				}
				while (dbDataReader.NextResult());

				OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetResultsetsFromReader(...): dispose reader", typeof(AdoNetYieldingFascade).Name));
			}

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetResultsetsFromReader(...): after yield", typeof(AdoNetYieldingFascade).Name));

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetResultsetsFromReader(...): leave", typeof(AdoNetYieldingFascade).Name));
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
			IRecord record;
			int recordsAffected;
			string key;
			object value;

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetSchemaRecordsFromReader(...): enter", typeof(AdoNetYieldingFascade).Name));

			if ((object)dbDataReader == null)
				throw new ArgumentNullException(nameof(dbDataReader));

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetSchemaRecordsFromReader(...): before yield", typeof(AdoNetYieldingFascade).Name));

			throw new NotSupportedException(string.Format("Not supported on CoreCLR."));
			/*using (DataTable dataTable = dbDataReader.GetSchemaTable())
			{
				OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetSchemaRecordsFromReader(...): use table", typeof(AdoNetYieldingFascade).Name));

				if ((object)dataTable != null)
				{
					foreach (DataRow dataRow in dataTable.Rows)
					{
						record = new Record();

						for (int index = 0; index < dataTable.Columns.Count; index++)
						{
							key = dataTable.Columns[index].ColumnName;
							value = dataRow[index].ChangeType<object>();

							record.Add(key, value);
						}

						OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetSchemaRecordsFromReader(...): on yield", typeof(AdoNetYieldingFascade).Name));

						yield return record; // LAZY PROCESSING INTENT HERE / DO NOT FORCE EAGER LOAD
					}
				}

				OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetSchemaRecordsFromReader(...): dispose table", typeof(AdoNetYieldingFascade).Name));
			}*/

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetSchemaRecordsFromReader(...): after yield", typeof(AdoNetYieldingFascade).Name));

			recordsAffected = dbDataReader.RecordsAffected;

			if ((object)recordsAffectedCallback != null)
				recordsAffectedCallback(recordsAffected);

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetSchemaRecordsFromReader(...): leave", typeof(AdoNetYieldingFascade).Name));
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

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetSchemaResultsetsFromReader(...): enter", typeof(AdoNetYieldingFascade).Name));

			if ((object)dbDataReader == null)
				throw new ArgumentNullException(nameof(dbDataReader));

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetSchemaResultsetsFromReader(...): before yield", typeof(AdoNetYieldingFascade).Name));

			using (dbDataReader)
			{
				OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetSchemaResultsetsFromReader(...): use reader", typeof(AdoNetYieldingFascade).Name));

				do
				{
					Resultset resultset = new Resultset(resultsetIndex++); // prevent modified closure
					resultset.Records = this.GetSchemaRecordsFromReader(dbDataReader, (ra) => resultset.RecordsAffected = ra);

					OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetSchemaResultsetsFromReader(...): on yield", typeof(AdoNetYieldingFascade).Name));

					yield return resultset; // LAZY PROCESSING INTENT HERE / DO NOT FORCE EAGER LOAD
				}
				while (dbDataReader.NextResult());

				OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetSchemaResultsetsFromReader(...): dispose reader", typeof(AdoNetYieldingFascade).Name));
			}

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetSchemaResultsetsFromReader(...): after yield", typeof(AdoNetYieldingFascade).Name));

			OnlyWhen._PROFILE_ThenPrint(string.Format("{0}::GetSchemaResultsetsFromReader(...): leave", typeof(AdoNetYieldingFascade).Name));
		}

		/// <summary>
		/// Returns a DbType mapping for a Type.
		/// An InvalidOperationException is thrown for unmappable types.
		/// </summary>
		/// <param name="clrType"> The CLR type to map to a DbType. </param>
		/// <returns> The mapped DbType. </returns>
		public DbType InferDbTypeForClrType(Type clrType)
		{
			return LegacyInstanceAccessor.AdoNetLiteLegacyInstance.InferDbTypeForClrType(clrType);
		}

		#endregion
	}
}