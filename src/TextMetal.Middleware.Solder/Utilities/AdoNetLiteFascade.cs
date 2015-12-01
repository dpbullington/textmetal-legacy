/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Data;
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
				throw new ArgumentNullException("reflectionFascade");

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

		public IDbDataParameter CreateParameter(Type connectionType, ParameterDirection parameterDirection, DbType parameterDbType, int parameterSize, byte parameterPrecision, byte parameterScale, bool parameterNullable, string parameterName, object parameterValue)
		{
			IDbDataParameter dbDataParameter;

			if ((object)connectionType == null)
				throw new ArgumentNullException("connectionType");

			using (IDbConnection dbConnection = (IDbConnection)Activator.CreateInstance(connectionType))
			{
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
		}

		public IEnumerable<IDictionary<string, object>> ExecuteRecords(bool schemaOnly, Type connectionType, string connectionString, bool transactional, IsolationLevel isolationLevel, CommandType commandType, string commandText, IEnumerable<IDbDataParameter> commandParameters, Action<int> resultsetCallback = null)
		{
			IDbTransaction dbTransaction;
			const bool OPEN = true;

			IList<IDictionary<string, object>> records;

			// force no preparation
			const bool COMMAND_PREPARE = false;

			// force provider default timeout
			const object COMMAND_TIMEOUT = null; /*int?*/

			CommandBehavior commandBehavior;
			int resultsetIndex = 0;

			if ((object)connectionType == null)
				throw new ArgumentNullException("connectionType");

			if ((object)connectionString == null)
				throw new ArgumentNullException("connectionString");

			using (IDbConnection dbConnection = (IDbConnection)Activator.CreateInstance(connectionType))
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

				using (IDbCommand dbCommand = dbConnection.CreateCommand())
				{
					dbCommand.Transaction = dbTransaction;
					dbCommand.CommandType = commandType;
					dbCommand.CommandText = commandText;

					if ((object)COMMAND_TIMEOUT != null)
						dbCommand.CommandTimeout = (int)COMMAND_TIMEOUT;

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

					if (COMMAND_PREPARE)
						dbCommand.Prepare();

					records = new List<IDictionary<string, object>>();

					commandBehavior = schemaOnly ? CommandBehavior.SchemaOnly : CommandBehavior.Default;

					using (IDataReader dataReader = dbCommand.ExecuteReader(commandBehavior))
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

								while (dataReader.Read())
								{
									record = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

									for (int columnIndex = 0; columnIndex < dataReader.FieldCount; columnIndex++)
									{
										key = dataReader.GetName(columnIndex);
										value = dataReader.GetValue(columnIndex).ChangeType<object>();

										if (record.ContainsKey(key) || (key ?? string.Empty).Length == 0)
											key = string.Format("Column_{0:0000}", columnIndex);

										record.Add(key, value);
									}

									records.Add(record);
								}
							}
							while (dataReader.NextResult());
						}
						else
						{
							using (DataTable dataTable = dataReader.GetSchemaTable())
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
							}
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