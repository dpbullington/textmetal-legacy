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
using System.Xml;

using TextMetal.Middleware.Solder.Extensions;
using TextMetal.Middleware.Solder.Injection;
using TextMetal.Middleware.Solder.Primitives;
using TextMetal.Middleware.Solder.Utilities;

namespace TextMetal.Middleware.Datazoid.Primitives
{
	public class AdoNetBufferingFascade : IAdoNetBufferingFascade
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the AdoNetBufferingFascade class.
		/// </summary>
		/// <param name="reflectionFascade"> The reflection fascade instance to use. </param>
		/// <param name="dataTypeFascade"> The data type fascade instance to use. </param>
		[DependencyInjection]
		public AdoNetBufferingFascade([DependencyInjection] IReflectionFascade reflectionFascade, [DependencyInjection] IDataTypeFascade dataTypeFascade)
		{
			if ((object)reflectionFascade == null)
				throw new ArgumentNullException(nameof(reflectionFascade));

			if ((object)dataTypeFascade == null)
				throw new ArgumentNullException(nameof(dataTypeFascade));

			this.reflectionFascade = reflectionFascade;
			this.dataTypeFascade = dataTypeFascade;
		}

		#endregion

		#region Fields/Constants

		private readonly IDataTypeFascade dataTypeFascade;
		private readonly IReflectionFascade reflectionFascade;

		#endregion

		#region Properties/Indexers/Events

		protected IDataTypeFascade DataTypeFascade
		{
			get
			{
				return this.dataTypeFascade;
			}
		}

		protected IReflectionFascade ReflectionFascade
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

			IList<IRecord> records;

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

					records = new List<IRecord>();

					commandBehavior = schemaOnly ? CommandBehavior.SchemaOnly : CommandBehavior.Default;

					// wrap reader with proxy
					using (DbDataReader dbDataReader = new WrappedDbDataReader.__(dbCommand.ExecuteReader(commandBehavior)))
					{
						Record record;
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
									record = new Record();

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