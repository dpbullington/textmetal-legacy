/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Xml;

namespace TextMetal.Middleware.Solder.Utilities
{
	public class AdoNetLiteFascade : IAdoNetLiteFascade
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the AdoNetLiteFascade class.
		/// </summary>
		/// <param name="reflectionFascade"> The reflection instance to use. </param>
		public AdoNetLiteFascade(IReflectionFascade reflectionFascade)
		{
			if ((object)reflectionFascade == null)
				throw new ArgumentNullException(nameof(reflectionFascade));

			this.reflectionFascade = reflectionFascade;
		}

		/// <summary>
		/// Initializes a new instance of the AdoNetLiteFascade class.
		/// </summary>
		private AdoNetLiteFascade()
			: this(Utilities.ReflectionFascade.Instance)
		{
		}

		#endregion

		#region Fields/Constants

		private static readonly AdoNetLiteFascade instance = new AdoNetLiteFascade();
		private readonly IReflectionFascade reflectionFascade;

		#endregion

		#region Properties/Indexers/Events

		public static AdoNetLiteFascade Instance
		{
			get
			{
				return instance;
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

		public DbParameter CreateParameter(Type connectionType, ParameterDirection parameterDirection, DbType parameterDbType, int parameterSize, byte parameterPrecision, byte parameterScale, bool parameterNullable, string parameterName, object parameterValue)
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
				this.ReflectionFascade.SetLogicalPropertyValue(dbParameter, "IsNullable", parameterNullable, true, false);
				dbParameter.Precision = parameterPrecision;
				dbParameter.Scale = parameterScale;

				return dbParameter;
			}
		}

		public IEnumerable<IDictionary<string, object>> ExecuteRecords(bool schemaOnly, Type connectionType, string connectionString, bool transactional, IsolationLevel isolationLevel, CommandType commandType, string commandText, IEnumerable<DbParameter> commandParameters, Action<int> resultsetCallback = null)
		{
			DbTransaction dbTransaction;
			const bool OPEN = true;

			IList<IDictionary<string, object>> records;

			// force no preparation
			const bool COMMAND_PREPARE = false;

			// force provider default timeout
			object COMMAND_TIMEOUT = null; /*int?*/

			CommandBehavior commandBehavior;
			int resultsetIndex = 0;

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

					records = new List<IDictionary<string, object>>();

					commandBehavior = schemaOnly ? CommandBehavior.SchemaOnly : CommandBehavior.Default;

					using (DbDataReader dbDataReader = dbCommand.ExecuteReader(commandBehavior))
					{
						IDictionary<string, object> record;
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
									record = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

									for (int columnIndex = 0; columnIndex < dbDataReader.FieldCount; columnIndex++)
									{
										key = dbDataReader.GetName(columnIndex);
										value = dbDataReader.GetValue(columnIndex).ChangeType<object>();

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
							/*using (DataTable dataTable = dbDataReader.GetSchemaTable())
							{
								if ((object)dataTable != null)
								{
									foreach (DataRow dataRow in dataTable.Rows)
									{
										record = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

										for (int index = 0; index < dataTable.Columns.Count; index++)
										{
											key = dataTable.Columns[index].ColumnName;
											value = dataRow[index].ChangeType<object>();

											record.Add(key, value);
										}

										records.Add(record);
									}
								}
							}*/
						}
					}

					return records;
				}
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
				throw new ArgumentNullException(nameof(clrType));

			var _clrTypeInfo = clrType.GetTypeInfo();

			if (clrType.IsByRef /* || type.IsPointer || type.IsArray */)
				return this.InferDbTypeForClrType(clrType.GetElementType());
			else if (_clrTypeInfo.IsGenericType &&
					!_clrTypeInfo.IsGenericTypeDefinition &&
					clrType.GetGenericTypeDefinition() == typeof(Nullable<>))
				return this.InferDbTypeForClrType(Nullable.GetUnderlyingType(clrType));
			else if (_clrTypeInfo.IsEnum)
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