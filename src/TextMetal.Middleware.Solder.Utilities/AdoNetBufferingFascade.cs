/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;

using __Record = System.Collections.Generic.IDictionary<string, object>;
using __Record__ = System.Collections.Generic.Dictionary<string, object>;

namespace TextMetal.Middleware.Solder.Utilities
{
	public class AdoNetBufferingFascade : IAdoNetBufferingFascade
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the AdoNetBufferingFascade class.
		/// </summary>
		/// <param name="dataTypeFascade"> The data type fascade instance to use. </param>
		public AdoNetBufferingFascade(IDataTypeFascade dataTypeFascade)
		{
			if ((object)dataTypeFascade == null)
				throw new ArgumentNullException(nameof(dataTypeFascade));

			this.dataTypeFascade = dataTypeFascade;
		}

		#endregion

		#region Fields/Constants

		private readonly IDataTypeFascade dataTypeFascade;

		#endregion

		#region Properties/Indexers/Events

		protected IDataTypeFascade DataTypeFascade
		{
			get
			{
				return this.dataTypeFascade;
			}
		}

		#endregion

		#region Methods/Operators

		public DbParameter CreateParameter(Type connectionType, string sourceColumn, ParameterDirection parameterDirection, DbType parameterDbType, int parameterSize, byte parameterPrecision, byte parameterScale, bool parameterNullable, string parameterName, object parameterValue)
		{
			DbParameter dbParameter;

			if ((object)connectionType == null)
				throw new ArgumentNullException(nameof(connectionType));

			using (DbConnection dbConnection = (DbConnection)Activator.CreateInstance(connectionType))
			{
				using (DbCommand dbCommand = dbConnection.CreateCommand())
					dbParameter = dbCommand.CreateParameter();

				dbParameter.ParameterName = parameterName;
				dbParameter.Size = parameterSize;
				dbParameter.Value = parameterValue;
				dbParameter.Direction = parameterDirection;
				dbParameter.DbType = parameterDbType;
				dbParameter.IsNullable = parameterNullable;
				dbParameter.Precision = parameterPrecision;
				dbParameter.Scale = parameterScale;
				dbParameter.SourceColumn = sourceColumn;

				return dbParameter;
			}
		}

		public IEnumerable<__Record> ExecuteRecords(bool schemaOnly, Type connectionType, string connectionString, bool transactional, IsolationLevel isolationLevel, CommandType commandType, string commandText, IEnumerable<DbParameter> commandParameters, Action<int> resultsetCallback = null)
		{
			DbTransaction dbTransaction;
			const bool OPEN = true;

			IList<__Record> records;

			// force no preparation
			const bool COMMAND_PREPARE = false;

			// force provider default timeout
			object COMMAND_TIMEOUT = null; /*int?*/

			CommandBehavior commandBehavior;
			int resultsetIndex = 0;

			ReadOnlyCollection<DbColumn> dbColumns;
			DbColumn dbColumn;
			PropertyInfo[] propertyInfos;
			PropertyInfo propertyInfo;

			if ((object)connectionType == null)
				throw new ArgumentNullException(nameof(connectionType));

			if ((object)connectionString == null)
				throw new ArgumentNullException(nameof(connectionString));

			using (DbConnection dbConnection = (DbConnection)Activator.CreateInstance(connectionType))
			{
				if (OPEN)
				{
					dbConnection.ConnectionString = connectionString;
					dbConnection.Open();

					if (transactional)
						dbTransaction = dbConnection.BeginTransaction(isolationLevel);
					else
						dbTransaction = null;
				}

				using (DbCommand dbCommand = dbConnection.CreateCommand())
				{
					dbCommand.Transaction = dbTransaction;
					dbCommand.CommandType = commandType;
					dbCommand.CommandText = commandText;

					if ((object)COMMAND_TIMEOUT != null)
						dbCommand.CommandTimeout = (int)COMMAND_TIMEOUT;

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

					if (COMMAND_PREPARE)
						dbCommand.Prepare();

					records = new List<__Record>();

					commandBehavior = schemaOnly ? CommandBehavior.SchemaOnly : CommandBehavior.Default;

					using (DbDataReader dbDataReader = (dbCommand.ExecuteReader(commandBehavior)))
					{
						__Record__ record;
						string key;
						object value;

						if (!schemaOnly)
						{
							do
							{
								if ((object)resultsetCallback != null)
									resultsetCallback(resultsetIndex++);

								while (dbDataReader.Read())
								{
									record = new __Record__();

									for (int columnIndex = 0; columnIndex < dbDataReader.FieldCount; columnIndex++)
									{
										key = dbDataReader.GetName(columnIndex);
										value = dbDataReader.GetValue(columnIndex);
										value = this.DataTypeFascade.ChangeType<object>(value);

										if (record.ContainsKey(key) || (key ?? string.Empty).Length == 0)
											key = string.Format("Column_{0:0000}", columnIndex);

										record.Add(key, value);
									}

									records.Add(record);
								}
							}
							while (dbDataReader.NextResult());
						}
						else
						{
							if (!dbDataReader.CanGetColumnSchema())
								throw new NotSupportedException(string.Format("The connection command type '{0}' does not support schema access.", dbDataReader.GetType().FullName));

							dbColumns = dbDataReader.GetColumnSchema();
							{
								if ((object)dbColumns != null)
								{
									for (int index = 0; index < dbColumns.Count; index++)
									{
										dbColumn = dbColumns[index];

										propertyInfos = dbColumn.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

										record = new __Record__();
										record.Add(string.Empty, dbColumn);

										if ((object)propertyInfos != null)
										{
											for (int i = 0; i < propertyInfos.Length; i++)
											{
												propertyInfo = propertyInfos[i];

												if (propertyInfo.GetIndexParameters().Any())
													continue;

												key = propertyInfo.Name;
												value = propertyInfo.GetValue(dbColumn);
												value = this.DataTypeFascade.ChangeType<object>(value);

												record.Add(key, value);
											}
										}
									}
								}
							}
						}
					}

					return records;
				}
			}
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		public static class LegacyInstanceAccessor
		{
			#region Fields/Constants

			private static readonly Lazy<IAdoNetBufferingFascade> adoNetStreamingFascadeFactory = new Lazy<IAdoNetBufferingFascade>(() => new AdoNetBufferingFascade(new DataTypeFascade()));

			#endregion

			#region Properties/Indexers/Events

			private static Lazy<IAdoNetBufferingFascade> AdoNetBufferingFascadeFactory
			{
				get
				{
					return adoNetStreamingFascadeFactory;
				}
			}

			public static IAdoNetBufferingFascade AdoNetBufferingLegacyInstance
			{
				get
				{
					return AdoNetBufferingFascadeFactory.Value;
				}
			}

			#endregion
		}

		#endregion
	}
}