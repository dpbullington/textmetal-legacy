/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

using TextMetal.Common.Cerealization;
using TextMetal.Common.Core;
using TextMetal.Common.Data;

namespace TextMetal.Framework.SourceModel.DatabaseSchema
{
	public abstract class SchemaSourceStrategy : SourceStrategy
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the SchemaSourceStrategy class.
		/// </summary>
		protected SchemaSourceStrategy()
		{
		}

		#endregion

		#region Methods/Operators

		private static string FormatCSharpType(Type type)
		{
			if ((object)type == null)
				throw new ArgumentNullException("type");

			if (type.IsGenericType)
			{
				Type[] args;
				List<string> s;

				s = new List<string>();
				args = type.GetGenericArguments();

				if ((object)args != null)
				{
					foreach (Type arg in args)
						s.Add(FormatCSharpType(arg));
				}

				return string.Format("{0}<{1}>", Regex.Replace(type.Name, "([A-Za-z0-9_]+)(`[0-1]+)", "$1"), string.Join(", ", s.ToArray()));
			}

			return type.Name;
		}

		private static string GetAllAssemblyResourceFileText(Type type, string folder, string name)
		{
			string resourcePath;
			string sqlText;

			if ((object)type == null)
				throw new ArgumentNullException("type");

			if ((object)name == null)
				throw new ArgumentNullException("name");

			if (DataType.IsWhiteSpace(name))
				throw new ArgumentOutOfRangeException("name");

			resourcePath = string.Format("{0}.DML.{1}.{2}.sql", type.Namespace, folder, name);

			if (!Cerealization.TryGetStringFromAssemblyResource(type, resourcePath, out sqlText))
				throw new InvalidOperationException(string.Format("Failed to obtain assembly manifest (embedded) resource '{0}'.", resourcePath));

			return sqlText;
		}

		protected abstract int CoreCalculateColumnSize(string dataSourceTag, Column column);

		protected abstract int CoreCalculateParameterSize(string dataSourceTag, Parameter parameter);

		protected abstract IEnumerable<IDataParameter> CoreGetColumnParameters(IUnitOfWork unitOfWork, string dataSourceTag, Server server, Database database, Schema schema, Table table);

		protected abstract IEnumerable<IDataParameter> CoreGetColumnParameters(IUnitOfWork unitOfWork, string dataSourceTag, Server server, Database database, Schema schema, View view);

		protected abstract IEnumerable<IDataParameter> CoreGetDatabaseParameters(IUnitOfWork unitOfWork, string dataSourceTag, Server server);

		protected abstract IEnumerable<IDataParameter> CoreGetDdlTriggerParameters(IUnitOfWork unitOfWork, string dataSourceTag, Server server, Database database);

		protected abstract IEnumerable<IDataParameter> CoreGetDmlTriggerParameters(IUnitOfWork unitOfWork, string dataSourceTag, Server server, Database database, Schema schema, Table table);

		protected abstract bool CoreGetEmitImplicitReturnParameter(string dataSourceTag);

		protected abstract IEnumerable<IDataParameter> CoreGetForeignKeyColumnParameters(IUnitOfWork unitOfWork, string dataSourceTag, Server server, Database database, Schema schema, Table table, ForeignKey foreignKey);

		protected abstract IEnumerable<IDataParameter> CoreGetForeignKeyParameters(IUnitOfWork unitOfWork, string dataSourceTag, Server server, Database database, Schema schema, Table table);

		protected abstract IEnumerable<IDataParameter> CoreGetParameterParameters(IUnitOfWork unitOfWork, string dataSourceTag, Server server, Database database, Schema schema, Procedure procedure);

		protected abstract string CoreGetParameterPrefix(string dataSourceTag);

		protected abstract IEnumerable<IDataParameter> CoreGetProcedureParameters(IUnitOfWork unitOfWork, string dataSourceTag, Server server, Database database, Schema schema);

		protected abstract IEnumerable<IDataParameter> CoreGetSchemaParameters(IUnitOfWork unitOfWork, string dataSourceTag, Server server, Database database);

		protected abstract IEnumerable<IDataParameter> CoreGetServerParameters(IUnitOfWork unitOfWork, string dataSourceTag);

		protected override object CoreGetSourceObject(string sourceFilePath, IDictionary<string, IList<string>> properties)
		{
			const string PROP_TOKEN_CONNECTION_AQTN = "ConnectionType";
			const string PROP_TOKEN_CONNECTION_STRING = "ConnectionString";
			const string PROP_TOKEN_DATA_SOURCE_TAG = "DataSourceTag";
			const string PROP_TOKEN_SERVER_FILTER = "ServerFilter";
			const string PROP_TOKEN_DATABASE_FILTER = "DatabaseFilter";
			const string PROP_TOKEN_SCHEMA_FILTER = "SchemaFilter";
			const string PROP_TOKEN_OBJECT_FILTER = "ObjectFilter";
			const string PROP_DISABLE_PROCEDURE_SCHEMA_DISCOVERY = "DisableProcedureSchemaDiscovery";
			const string PROP_ENABLE_DATABASE_FILTER = "EnableDatabaseFilter";
			string connectionAqtn;
			Type connectionType = null;
			string connectionString = null;
			string dataSourceTag;
			string[] serverFilter;
			string[] databaseFilter;
			string[] schemaFilter;
			string[] objectFilter;
			bool disableProcSchDisc, enableDatabaseFilter;
			IList<string> values;

			if ((object)sourceFilePath == null)
				throw new ArgumentNullException("sourceFilePath");

			if ((object)properties == null)
				throw new ArgumentNullException("properties");

			connectionAqtn = null;
			if (properties.TryGetValue(PROP_TOKEN_CONNECTION_AQTN, out values))
			{
				if ((object)values != null && values.Count == 1)
				{
					connectionAqtn = values[0];
					connectionType = Type.GetType(connectionAqtn, false);
				}
			}

			if ((object)connectionType == null)
				throw new InvalidOperationException(string.Format("Failed to load the connection type '{0}' via Type.GetType(..).", connectionAqtn));

			if (!typeof(IDbConnection).IsAssignableFrom(connectionType))
				throw new InvalidOperationException(string.Format("The connection type is not assignable to type '{0}'.", typeof(IDbConnection).FullName));

			if (properties.TryGetValue(PROP_TOKEN_CONNECTION_STRING, out values))
			{
				if ((object)values != null && values.Count == 1)
					connectionString = values[0];
			}

			if (DataType.IsNullOrWhiteSpace(connectionString))
				connectionString = sourceFilePath;

			if (DataType.IsWhiteSpace(connectionString))
				throw new InvalidOperationException(string.Format("The connection string cannot be null or whitespace."));

			dataSourceTag = null;
			if (properties.TryGetValue(PROP_TOKEN_DATA_SOURCE_TAG, out values))
			{
				if ((object)values != null && values.Count == 1)
					dataSourceTag = values[0];
			}

			serverFilter = null;
			if (properties.TryGetValue(PROP_TOKEN_SERVER_FILTER, out values))
			{
				if ((object)values != null && values.Count > 0)
					serverFilter = values.ToArray();
			}

			databaseFilter = null;
			if (properties.TryGetValue(PROP_TOKEN_DATABASE_FILTER, out values))
			{
				if ((object)values != null && values.Count > 0)
					databaseFilter = values.ToArray();
			}

			schemaFilter = null;
			if (properties.TryGetValue(PROP_TOKEN_SCHEMA_FILTER, out values))
			{
				if ((object)values != null && values.Count > 0)
					schemaFilter = values.ToArray();
			}

			objectFilter = null;
			if (properties.TryGetValue(PROP_TOKEN_OBJECT_FILTER, out values))
			{
				if ((object)values != null && values.Count > 0)
					objectFilter = values.ToArray();
			}

			disableProcSchDisc = false;
			if (properties.TryGetValue(PROP_DISABLE_PROCEDURE_SCHEMA_DISCOVERY, out values))
			{
				if ((object)values != null && values.Count > 0)
					DataType.TryParse<bool>(values[0], out disableProcSchDisc);
			}

			enableDatabaseFilter = false;
			if (properties.TryGetValue(PROP_ENABLE_DATABASE_FILTER, out values))
			{
				if ((object)values != null && values.Count > 0)
					DataType.TryParse<bool>(values[0], out enableDatabaseFilter);
			}

			return this.GetSchemaModel(connectionString, connectionType, dataSourceTag, serverFilter, databaseFilter, schemaFilter, objectFilter, disableProcSchDisc, enableDatabaseFilter);
		}

		protected abstract IEnumerable<IDataParameter> CoreGetTableParameters(IUnitOfWork unitOfWork, string dataSourceTag, Server server, Database database, Schema schema);

		protected abstract IEnumerable<IDataParameter> CoreGetUniqueKeyColumnParameters(IUnitOfWork unitOfWork, string dataSourceTag, Server server, Database database, Schema schema, Table table, UniqueKey uniqueKey);

		protected abstract IEnumerable<IDataParameter> CoreGetUniqueKeyParameters(IUnitOfWork unitOfWork, string dataSourceTag, Server server, Database database, Schema schema, Table table);

		protected abstract Type CoreInferClrTypeForSqlType(string dataSourceTag, string sqlType, int sqlPrecision);

		private object GetSchemaModel(string connectionString, Type connectionType, string dataSourceTag,
			string[] serverFilter, string[] databaseFilter,
			string[] schemaFilter, string[] objectFilter,
			bool disableProcSchDisc, bool enableDatabaseFilter)
		{
			Server server;
			int recordsAffected;
			const string RETURN_VALUE = "ReturnValue";
			Type clrType;

			if ((object)connectionString == null)
				throw new ArgumentNullException("connectionString");

			if ((object)connectionType == null)
				throw new ArgumentNullException("connectionType");

			if ((object)dataSourceTag == null)
				throw new ArgumentNullException("dataSourceTag");

			using (IUnitOfWork unitOfWork = UnitOfWork.Create(connectionType, connectionString, false))
			{
				server = new Server();
				server.ConnectionString = connectionString;
				server.ConnectionType = connectionType.FullName;

				var dictEnumServer = unitOfWork.ExecuteDictionary(CommandType.Text, GetAllAssemblyResourceFileText(this.GetType(), dataSourceTag, "Server"), this.CoreGetServerParameters(unitOfWork, dataSourceTag), out recordsAffected);
				{
					var dictDataServer = (IDictionary<string, object>)null;

					if ((object)dictEnumServer != null &&
						(object)(dictDataServer = dictEnumServer.SingleOrDefault()) != null)
					{
						server.ServerName = DataType.ChangeType<string>(dictDataServer["ServerName"]);
						server.MachineName = DataType.ChangeType<string>(dictDataServer["MachineName"]);
						server.InstanceName = DataType.ChangeType<string>(dictDataServer["InstanceName"]);
						server.ServerVersion = DataType.ChangeType<string>(dictDataServer["ServerVersion"]);
						server.ServerLevel = DataType.ChangeType<string>(dictDataServer["ServerLevel"]);
						server.ServerEdition = DataType.ChangeType<string>(dictDataServer["ServerEdition"]);
						server.DefaultDatabaseName = DataType.ChangeType<string>(dictDataServer["DefaultDatabaseName"]);

						// filter unwanted servers
						if ((object)serverFilter != null)
						{
							if (!serverFilter.Contains(server.ServerName))
								return null;
						}

						var dictEnumDatabase = unitOfWork.ExecuteDictionary(CommandType.Text, GetAllAssemblyResourceFileText(this.GetType(), dataSourceTag, "Databases"), this.CoreGetDatabaseParameters(unitOfWork, dataSourceTag, server), out recordsAffected);
						{
							if ((object)dictEnumDatabase != null)
							{
								foreach (var dictDataDatabase in dictEnumDatabase)
								{
									Database database;

									database = new Database();
									database.DatabaseId = DataType.ChangeType<int>(dictDataDatabase["DatabaseId"]);
									database.DatabaseName = DataType.ChangeType<string>(dictDataDatabase["DatabaseName"]);
									database.CreationTimestamp = DataType.ChangeType<DateTime>(dictDataDatabase["CreationTimestamp"]);
									database.DatabaseNamePascalCase = Name.GetPascalCase(database.DatabaseName);
									database.DatabaseNameCamelCase = Name.GetCamelCase(database.DatabaseName);
									database.DatabaseNameConstantCase = Name.GetConstantCase(database.DatabaseName);
									database.DatabaseNameSingularPascalCase = Name.GetPascalCase(Name.GetSingularForm(database.DatabaseName));
									database.DatabaseNameSingularCamelCase = Name.GetCamelCase(Name.GetSingularForm(database.DatabaseName));
									database.DatabaseNameSingularConstantCase = Name.GetConstantCase(Name.GetSingularForm(database.DatabaseName));
									database.DatabaseNamePluralPascalCase = Name.GetPascalCase(Name.GetPluralForm(database.DatabaseName));
									database.DatabaseNamePluralCamelCase = Name.GetCamelCase(Name.GetPluralForm(database.DatabaseName));
									database.DatabaseNamePluralConstantCase = Name.GetConstantCase(Name.GetPluralForm(database.DatabaseName));

									database.DatabaseNameSqlMetalPascalCase = Name.GetSqlMetalPascalCase(database.DatabaseName);
									database.DatabaseNameSqlMetalCamelCase = Name.GetSqlMetalCamelCase(database.DatabaseName);
									database.DatabaseNameSqlMetalSingularPascalCase = Name.GetSqlMetalPascalCase(Name.GetSingularForm(database.DatabaseName));
									database.DatabaseNameSqlMetalSingularCamelCase = Name.GetSqlMetalCamelCase(Name.GetSingularForm(database.DatabaseName));
									database.DatabaseNameSqlMetalPluralPascalCase = Name.GetSqlMetalPascalCase(Name.GetPluralForm(database.DatabaseName));
									database.DatabaseNameSqlMetalPluralCamelCase = Name.GetSqlMetalCamelCase(Name.GetPluralForm(database.DatabaseName));

									// preserve default behavior in that if NO filter is specified
									// contrain to only the DEFAULT DATABASE
									if (!enableDatabaseFilter)
									{
										if (database.DatabaseName.SafeToString().ToLower() != server.DefaultDatabaseName.SafeToString().ToLower())
											continue;
									}
									else
									{
										// filter unwanted databases
										if ((object)databaseFilter != null)
										{
											if (!databaseFilter.Contains(database.DatabaseName))
												continue;
										}
									}

									server.Databases.Add(database);

									unitOfWork.ExecuteDictionary(CommandType.Text, string.Format(GetAllAssemblyResourceFileText(this.GetType(), dataSourceTag, "UseDatabase"), server.ServerName, database.DatabaseName), null, out recordsAffected);

									var dictEnumDdlTrigger = unitOfWork.ExecuteDictionary(CommandType.Text, GetAllAssemblyResourceFileText(this.GetType(), dataSourceTag, "DdlTriggers"), this.CoreGetDdlTriggerParameters(unitOfWork, dataSourceTag, server, database), out recordsAffected);
									{
										if ((object)dictEnumDdlTrigger != null)
										{
											foreach (var dictDataTrigger in dictEnumDdlTrigger)
											{
												Trigger trigger;

												trigger = new Trigger();

												trigger.TriggerId = DataType.ChangeType<int>(dictDataTrigger["TriggerId"]);
												trigger.TriggerName = DataType.ChangeType<string>(dictDataTrigger["TriggerName"]);
												trigger.IsClrTrigger = DataType.ChangeType<bool>(dictDataTrigger["IsClrTrigger"]);
												trigger.IsTriggerDisabled = DataType.ChangeType<bool>(dictDataTrigger["IsTriggerDisabled"]);
												trigger.IsTriggerNotForReplication = DataType.ChangeType<bool>(dictDataTrigger["IsTriggerNotForReplication"]);
												trigger.IsInsteadOfTrigger = DataType.ChangeType<bool>(dictDataTrigger["IsInsteadOfTrigger"]);
												trigger.TriggerNamePascalCase = Name.GetPascalCase(trigger.TriggerName);
												trigger.TriggerNameCamelCase = Name.GetCamelCase(trigger.TriggerName);
												trigger.TriggerNameConstantCase = Name.GetConstantCase(trigger.TriggerName);
												trigger.TriggerNameSingularPascalCase = Name.GetPascalCase(Name.GetSingularForm(trigger.TriggerName));
												trigger.TriggerNameSingularCamelCase = Name.GetCamelCase(Name.GetSingularForm(trigger.TriggerName));
												trigger.TriggerNameSingularConstantCase = Name.GetConstantCase(Name.GetSingularForm(trigger.TriggerName));
												trigger.TriggerNamePluralPascalCase = Name.GetPascalCase(Name.GetPluralForm(trigger.TriggerName));
												trigger.TriggerNamePluralCamelCase = Name.GetCamelCase(Name.GetPluralForm(trigger.TriggerName));
												trigger.TriggerNamePluralConstantCase = Name.GetConstantCase(Name.GetPluralForm(trigger.TriggerName));

												trigger.TriggerNameSqlMetalPascalCase = Name.GetSqlMetalPascalCase(trigger.TriggerName);
												trigger.TriggerNameSqlMetalCamelCase = Name.GetSqlMetalCamelCase(trigger.TriggerName);
												trigger.TriggerNameSqlMetalSingularPascalCase = Name.GetSqlMetalPascalCase(Name.GetSingularForm(trigger.TriggerName));
												trigger.TriggerNameSqlMetalSingularCamelCase = Name.GetSqlMetalCamelCase(Name.GetSingularForm(trigger.TriggerName));
												trigger.TriggerNameSqlMetalPluralPascalCase = Name.GetSqlMetalPascalCase(Name.GetPluralForm(trigger.TriggerName));
												trigger.TriggerNameSqlMetalPluralCamelCase = Name.GetSqlMetalCamelCase(Name.GetPluralForm(trigger.TriggerName));

												database.Triggers.Add(trigger);
											}
										}
									}

									var dictEnumSchema = unitOfWork.ExecuteDictionary(CommandType.Text, GetAllAssemblyResourceFileText(this.GetType(), dataSourceTag, "Schemas"), this.CoreGetSchemaParameters(unitOfWork, dataSourceTag, server, database), out recordsAffected);
									{
										if ((object)dictEnumSchema != null)
										{
											foreach (var dictDataSchema in dictEnumSchema)
											{
												Schema schema;

												schema = new Schema();
												schema.SchemaId = DataType.ChangeType<int>(dictDataSchema["SchemaId"]);
												schema.OwnerId = DataType.ChangeType<int>(dictDataSchema["OwnerId"]);
												schema.SchemaName = DataType.ChangeType<string>(dictDataSchema["SchemaName"]);
												schema.SchemaNamePascalCase = Name.GetPascalCase(schema.SchemaName);
												schema.SchemaNameCamelCase = Name.GetCamelCase(schema.SchemaName);
												schema.SchemaNameConstantCase = Name.GetConstantCase(schema.SchemaName);
												schema.SchemaNameSingularPascalCase = Name.GetPascalCase(Name.GetSingularForm(schema.SchemaName));
												schema.SchemaNameSingularCamelCase = Name.GetCamelCase(Name.GetSingularForm(schema.SchemaName));
												schema.SchemaNameSingularConstantCase = Name.GetConstantCase(Name.GetSingularForm(schema.SchemaName));
												schema.SchemaNamePluralPascalCase = Name.GetPascalCase(Name.GetPluralForm(schema.SchemaName));
												schema.SchemaNamePluralCamelCase = Name.GetCamelCase(Name.GetPluralForm(schema.SchemaName));
												schema.SchemaNamePluralConstantCase = Name.GetConstantCase(Name.GetPluralForm(schema.SchemaName));

												schema.SchemaNameSqlMetalPascalCase = Name.GetSqlMetalPascalCase(schema.SchemaName);
												schema.SchemaNameSqlMetalCamelCase = Name.GetSqlMetalCamelCase(schema.SchemaName);
												schema.SchemaNameSqlMetalSingularPascalCase = Name.GetSqlMetalPascalCase(Name.GetSingularForm(schema.SchemaName));
												schema.SchemaNameSqlMetalSingularCamelCase = Name.GetSqlMetalCamelCase(Name.GetSingularForm(schema.SchemaName));
												schema.SchemaNameSqlMetalPluralPascalCase = Name.GetSqlMetalPascalCase(Name.GetPluralForm(schema.SchemaName));
												schema.SchemaNameSqlMetalPluralCamelCase = Name.GetSqlMetalCamelCase(Name.GetPluralForm(schema.SchemaName));

												// filter unwanted schemas
												if ((object)schemaFilter != null)
												{
													if (!schemaFilter.Contains(schema.SchemaName))
														continue;
												}

												database.Schemas.Add(schema);

												var dictEnumTable = unitOfWork.ExecuteDictionary(CommandType.Text, GetAllAssemblyResourceFileText(this.GetType(), dataSourceTag, "Tables"), this.CoreGetTableParameters(unitOfWork, dataSourceTag, server, database, schema), out recordsAffected);
												{
													foreach (var dictDataTable in dictEnumTable)
													{
														Table table;

														table = new Table();
														table.TableId = DataType.ChangeType<int>(dictDataTable["TableId"]);
														table.TableName = DataType.ChangeType<string>(dictDataTable["TableName"]);
														table.CreationTimestamp = DataType.ChangeType<DateTime>(dictDataTable["CreationTimestamp"]);
														table.ModificationTimestamp = DataType.ChangeType<DateTime>(dictDataTable["ModificationTimestamp"]);
														table.IsImplementationDetail = DataType.ChangeType<bool>(dictDataTable["IsImplementationDetail"]);
														table.TableNamePascalCase = Name.GetPascalCase(table.TableName);
														table.TableNameCamelCase = Name.GetCamelCase(table.TableName);
														table.TableNameConstantCase = Name.GetConstantCase(table.TableName);
														table.TableNameSingularPascalCase = Name.GetPascalCase(Name.GetSingularForm(table.TableName));
														table.TableNameSingularCamelCase = Name.GetCamelCase(Name.GetSingularForm(table.TableName));
														table.TableNameSingularConstantCase = Name.GetConstantCase(Name.GetSingularForm(table.TableName));
														table.TableNamePluralPascalCase = Name.GetPascalCase(Name.GetPluralForm(table.TableName));
														table.TableNamePluralCamelCase = Name.GetCamelCase(Name.GetPluralForm(table.TableName));
														table.TableNamePluralConstantCase = Name.GetConstantCase(Name.GetPluralForm(table.TableName));

														table.TableNameSqlMetalPascalCase = Name.GetSqlMetalPascalCase(table.TableName);
														table.TableNameSqlMetalCamelCase = Name.GetSqlMetalCamelCase(table.TableName);
														table.TableNameSqlMetalSingularPascalCase = Name.GetSqlMetalPascalCase(Name.GetSingularForm(table.TableName));
														table.TableNameSqlMetalSingularCamelCase = Name.GetSqlMetalCamelCase(Name.GetSingularForm(table.TableName));
														table.TableNameSqlMetalPluralPascalCase = Name.GetSqlMetalPascalCase(Name.GetPluralForm(table.TableName));
														table.TableNameSqlMetalPluralCamelCase = Name.GetSqlMetalCamelCase(Name.GetPluralForm(table.TableName));

														var pkId = DataType.ChangeType<int?>(dictDataTable["PrimaryKeyId"]);
														if ((object)pkId != null)
														{
															table.PrimaryKey = new PrimaryKey();
															table.PrimaryKey.PrimaryKeyId = (int)pkId;

															table.PrimaryKey.PrimaryKeyIsSystemNamed = DataType.ChangeType<bool>(dictDataTable["PrimaryKeyIsSystemNamed"]);
															table.PrimaryKey.PrimaryKeyName = DataType.ChangeType<string>(dictDataTable["PrimaryKeyName"]);
															
															table.PrimaryKey.PrimaryKeyNamePascalCase = Name.GetPascalCase(table.PrimaryKey.PrimaryKeyName);
															table.PrimaryKey.PrimaryKeyNameCamelCase = Name.GetCamelCase(table.PrimaryKey.PrimaryKeyName);
															table.PrimaryKey.PrimaryKeyNameConstantCase = Name.GetConstantCase(table.PrimaryKey.PrimaryKeyName);
															table.PrimaryKey.PrimaryKeyNameSingularPascalCase = Name.GetPascalCase(Name.GetSingularForm(table.PrimaryKey.PrimaryKeyName));
															table.PrimaryKey.PrimaryKeyNameSingularCamelCase = Name.GetCamelCase(Name.GetSingularForm(table.PrimaryKey.PrimaryKeyName));
															table.PrimaryKey.PrimaryKeyNameSingularConstantCase = Name.GetConstantCase(Name.GetSingularForm(table.PrimaryKey.PrimaryKeyName));
															table.PrimaryKey.PrimaryKeyNamePluralPascalCase = Name.GetPascalCase(Name.GetPluralForm(table.PrimaryKey.PrimaryKeyName));
															table.PrimaryKey.PrimaryKeyNamePluralCamelCase = Name.GetCamelCase(Name.GetPluralForm(table.PrimaryKey.PrimaryKeyName));
															table.PrimaryKey.PrimaryKeyNamePluralConstantCase = Name.GetConstantCase(Name.GetPluralForm(table.PrimaryKey.PrimaryKeyName));

															table.PrimaryKey.PrimaryKeyNameSqlMetalPascalCase = Name.GetSqlMetalPascalCase(table.PrimaryKey.PrimaryKeyName);
															table.PrimaryKey.PrimaryKeyNameSqlMetalCamelCase = Name.GetSqlMetalCamelCase(table.PrimaryKey.PrimaryKeyName);
															table.PrimaryKey.PrimaryKeyNameSqlMetalSingularPascalCase = Name.GetSqlMetalPascalCase(Name.GetSingularForm(table.PrimaryKey.PrimaryKeyName));
															table.PrimaryKey.PrimaryKeyNameSqlMetalSingularCamelCase = Name.GetSqlMetalCamelCase(Name.GetSingularForm(table.PrimaryKey.PrimaryKeyName));
															table.PrimaryKey.PrimaryKeyNameSqlMetalPluralPascalCase = Name.GetSqlMetalPascalCase(Name.GetPluralForm(table.PrimaryKey.PrimaryKeyName));
															table.PrimaryKey.PrimaryKeyNameSqlMetalPluralCamelCase = Name.GetSqlMetalCamelCase(Name.GetPluralForm(table.PrimaryKey.PrimaryKeyName));
														}
														
														// filter unwanted tables (objects)
														if ((object)objectFilter != null)
														{
															if (!objectFilter.Contains(table.TableName))
																continue;
														}

														schema._Tables.Add(table);

														var dictEnumColumn = unitOfWork.ExecuteDictionary(CommandType.Text, GetAllAssemblyResourceFileText(this.GetType(), dataSourceTag, "TableColumns"), this.CoreGetColumnParameters(unitOfWork, dataSourceTag, server, database, schema, table), out recordsAffected);
														{
															if ((object)dictEnumColumn != null)
															{
																foreach (var dictDataColumn in dictEnumColumn)
																{
																	TableColumn column;

																	column = new TableColumn();

																	column.ColumnName = DataType.ChangeType<string>(dictDataColumn["ColumnName"]);
																	column.ColumnOrdinal = DataType.ChangeType<int>(dictDataColumn["ColumnOrdinal"]);
																	column.ColumnNullable = DataType.ChangeType<bool>(dictDataColumn["ColumnNullable"]);
																	column.ColumnSize = DataType.ChangeType<int>(dictDataColumn["ColumnSize"]);
																	column.ColumnPrecision = DataType.ChangeType<int>(dictDataColumn["ColumnPrecision"]);
																	column.ColumnScale = DataType.ChangeType<int>(dictDataColumn["ColumnScale"]);
																	column.ColumnSqlType = DataType.ChangeType<string>(dictDataColumn["ColumnSqlType"]);
																	column.ColumnIsUserDefinedType = DataType.ChangeType<bool>(dictDataColumn["ColumnIsUserDefinedType"]);
																	column.ColumnIsIdentity = DataType.ChangeType<bool>(dictDataColumn["ColumnIsIdentity"]);
																	column.ColumnIsComputed = DataType.ChangeType<bool>(dictDataColumn["ColumnIsComputed"]);
																	column.ColumnHasDefault = DataType.ChangeType<bool>(dictDataColumn["ColumnHasDefault"]);
																	column.ColumnHasCheck = DataType.ChangeType<bool>(dictDataColumn["ColumnHasCheck"]);
																	column.ColumnIsPrimaryKey = DataType.ChangeType<bool>(dictDataColumn["ColumnIsPrimaryKey"]);
																	column.ColumnPrimaryKeyOrdinal = DataType.ChangeType<int>(dictDataColumn["ColumnPrimaryKeyOrdinal"]);
																	column.ColumnNamePascalCase = Name.GetPascalCase(column.ColumnName);
																	column.ColumnNameCamelCase = Name.GetCamelCase(column.ColumnName);
																	column.ColumnNameConstantCase = Name.GetConstantCase(column.ColumnName);
																	column.ColumnNameSingularPascalCase = Name.GetPascalCase(Name.GetSingularForm(column.ColumnName));
																	column.ColumnNameSingularCamelCase = Name.GetCamelCase(Name.GetSingularForm(column.ColumnName));
																	column.ColumnNameSingularConstantCase = Name.GetConstantCase(Name.GetSingularForm(column.ColumnName));
																	column.ColumnNamePluralPascalCase = Name.GetPascalCase(Name.GetPluralForm(column.ColumnName));
																	column.ColumnNamePluralCamelCase = Name.GetCamelCase(Name.GetPluralForm(column.ColumnName));
																	column.ColumnNamePluralConstantCase = Name.GetConstantCase(Name.GetPluralForm(column.ColumnName));

																	column.ColumnNameSqlMetalPascalCase = Name.GetSqlMetalPascalCase(column.ColumnName);
																	column.ColumnNameSqlMetalCamelCase = Name.GetSqlMetalCamelCase(column.ColumnName);
																	column.ColumnNameSqlMetalSingularPascalCase = Name.GetSqlMetalPascalCase(Name.GetSingularForm(column.ColumnName));
																	column.ColumnNameSqlMetalSingularCamelCase = Name.GetSqlMetalCamelCase(Name.GetSingularForm(column.ColumnName));
																	column.ColumnNameSqlMetalPluralPascalCase = Name.GetSqlMetalPascalCase(Name.GetPluralForm(column.ColumnName));
																	column.ColumnNameSqlMetalPluralCamelCase = Name.GetSqlMetalCamelCase(Name.GetPluralForm(column.ColumnName));

																	clrType = this.CoreInferClrTypeForSqlType(dataSourceTag, column.ColumnSqlType, column.ColumnPrecision);
																	column.ColumnDbType = AdoNetHelper.InferDbTypeForClrType(clrType);
																	column.ColumnSize = this.CoreCalculateColumnSize(dataSourceTag, column); //recalculate

																	column.ColumnClrType = clrType ?? typeof(object);
																	column.ColumnClrNullableType = Reflexion.MakeNullableType(clrType);
																	column.ColumnClrNonNullableType = Reflexion.MakeNonNullableType(clrType);
																	column.ColumnCSharpNullableLiteral = column.ColumnNullable.ToString().ToLower();
																	column.ColumnCSharpDbType = string.Format("{0}.{1}", typeof(DbType).Name, column.ColumnDbType);
																	column.ColumnCSharpClrType = (object)column.ColumnClrType != null ? FormatCSharpType(column.ColumnClrType) : FormatCSharpType(typeof(object));
																	column.ColumnCSharpClrNullableType = (object)column.ColumnClrNullableType != null ? FormatCSharpType(column.ColumnClrNullableType) : FormatCSharpType(typeof(object));
																	column.ColumnCSharpClrNonNullableType = (object)column.ColumnClrNonNullableType != null ? FormatCSharpType(column.ColumnClrNonNullableType) : FormatCSharpType(typeof(object));

																	table.Columns.Add(column);

																	if ((object)table.PrimaryKey != null &&
																		(object)table.PrimaryKey.PrimaryKeyId != null &&
																		column.ColumnIsPrimaryKey)
																		table.PrimaryKey.PrimaryKeyColumns.Add(new PrimaryKeyColumn()
																		{
																			ColumnName = column.ColumnName,
																			ColumnOrdinal = column.ColumnOrdinal,
																			PrimaryKeyColumnOrdinal = column.ColumnPrimaryKeyOrdinal
																		});
																}
															}
														}

														if (table.Columns.Count(c => c.ColumnIsPrimaryKey) < 1)
														{
															table.HasNoDefinedPrimaryKeyColumns = true;
															table.Columns.ForEach(c => c.ColumnIsPrimaryKey = true);
														}

														var dictEnumDmlTrigger = unitOfWork.ExecuteDictionary(CommandType.Text, GetAllAssemblyResourceFileText(this.GetType(), dataSourceTag, "DmlTriggers"), this.CoreGetDmlTriggerParameters(unitOfWork, dataSourceTag, server, database, schema, table), out recordsAffected);
														{
															if ((object)dictEnumDmlTrigger != null)
															{
																foreach (var dictDataTrigger in dictEnumDmlTrigger)
																{
																	Trigger trigger;

																	trigger = new Trigger();

																	trigger.TriggerId = DataType.ChangeType<int>(dictDataTrigger["TriggerId"]);
																	trigger.TriggerName = DataType.ChangeType<string>(dictDataTrigger["TriggerName"]);
																	trigger.IsClrTrigger = DataType.ChangeType<bool>(dictDataTrigger["IsClrTrigger"]);
																	trigger.IsTriggerDisabled = DataType.ChangeType<bool>(dictDataTrigger["IsTriggerDisabled"]);
																	trigger.IsTriggerNotForReplication = DataType.ChangeType<bool>(dictDataTrigger["IsTriggerNotForReplication"]);
																	trigger.IsInsteadOfTrigger = DataType.ChangeType<bool>(dictDataTrigger["IsInsteadOfTrigger"]);
																	trigger.TriggerNamePascalCase = Name.GetPascalCase(trigger.TriggerName);
																	trigger.TriggerNameCamelCase = Name.GetCamelCase(trigger.TriggerName);
																	trigger.TriggerNameConstantCase = Name.GetConstantCase(trigger.TriggerName);
																	trigger.TriggerNameSingularPascalCase = Name.GetPascalCase(Name.GetSingularForm(trigger.TriggerName));
																	trigger.TriggerNameSingularCamelCase = Name.GetCamelCase(Name.GetSingularForm(trigger.TriggerName));
																	trigger.TriggerNameSingularConstantCase = Name.GetConstantCase(Name.GetSingularForm(trigger.TriggerName));
																	trigger.TriggerNamePluralPascalCase = Name.GetPascalCase(Name.GetPluralForm(trigger.TriggerName));
																	trigger.TriggerNamePluralCamelCase = Name.GetCamelCase(Name.GetPluralForm(trigger.TriggerName));
																	trigger.TriggerNamePluralConstantCase = Name.GetConstantCase(Name.GetPluralForm(trigger.TriggerName));

																	trigger.TriggerNameSqlMetalPascalCase = Name.GetSqlMetalPascalCase(trigger.TriggerName);
																	trigger.TriggerNameSqlMetalCamelCase = Name.GetSqlMetalCamelCase(trigger.TriggerName);
																	trigger.TriggerNameSqlMetalSingularPascalCase = Name.GetSqlMetalPascalCase(Name.GetSingularForm(trigger.TriggerName));
																	trigger.TriggerNameSqlMetalSingularCamelCase = Name.GetSqlMetalCamelCase(Name.GetSingularForm(trigger.TriggerName));
																	trigger.TriggerNameSqlMetalPluralPascalCase = Name.GetSqlMetalPascalCase(Name.GetPluralForm(trigger.TriggerName));
																	trigger.TriggerNameSqlMetalPluralCamelCase = Name.GetSqlMetalCamelCase(Name.GetPluralForm(trigger.TriggerName));

																	table.Triggers.Add(trigger);
																}
															}
														}

														var dictEnumForeignKey = unitOfWork.ExecuteDictionary(CommandType.Text, GetAllAssemblyResourceFileText(this.GetType(), dataSourceTag, "ForeignKeys"), this.CoreGetForeignKeyParameters(unitOfWork, dataSourceTag, server, database, schema, table), out recordsAffected);
														{
															if ((object)dictEnumForeignKey != null)
															{
																foreach (var dictDataForeignKey in dictEnumForeignKey)
																{
																	ForeignKey foreignKey;

																	foreignKey = new ForeignKey();

																	foreignKey.ForeignKeyName = DataType.ChangeType<string>(dictDataForeignKey["ForeignKeyName"]);
																	foreignKey.ForeignKeyIsDisabled = DataType.ChangeType<bool>(dictDataForeignKey["ForeignKeyIsDisabled"]);
																	foreignKey.ForeignKeyIsSystemNamed = DataType.ChangeType<bool>(dictDataForeignKey["ForeignKeyIsSystemNamed"]);
																	foreignKey.ForeignKeyIsForReplication = DataType.ChangeType<bool>(dictDataForeignKey["ForeignKeyIsForReplication"]);
																	foreignKey.ForeignKeyOnDeleteRefIntAction = DataType.ChangeType<byte>(dictDataForeignKey["ForeignKeyOnDeleteRefIntAction"]);
																	foreignKey.ForeignKeyOnDeleteRefIntActionSqlName = DataType.ChangeType<string>(dictDataForeignKey["ForeignKeyOnDeleteRefIntActionSqlName"]);
																	foreignKey.ForeignKeyOnUpdateRefIntAction = DataType.ChangeType<byte>(dictDataForeignKey["ForeignKeyOnUpdateRefIntAction"]);
																	foreignKey.ForeignKeyOnUpdateRefIntActionSqlName = DataType.ChangeType<string>(dictDataForeignKey["ForeignKeyOnUpdateRefIntActionSqlName"]);
																	foreignKey.ForeignKeyNamePascalCase = Name.GetPascalCase(foreignKey.ForeignKeyName);
																	foreignKey.ForeignKeyNameCamelCase = Name.GetCamelCase(foreignKey.ForeignKeyName);
																	foreignKey.ForeignKeyNameConstantCase = Name.GetConstantCase(foreignKey.ForeignKeyName);
																	foreignKey.ForeignKeyNameSingularPascalCase = Name.GetPascalCase(Name.GetSingularForm(foreignKey.ForeignKeyName));
																	foreignKey.ForeignKeyNameSingularCamelCase = Name.GetCamelCase(Name.GetSingularForm(foreignKey.ForeignKeyName));
																	foreignKey.ForeignKeyNameSingularConstantCase = Name.GetConstantCase(Name.GetSingularForm(foreignKey.ForeignKeyName));
																	foreignKey.ForeignKeyNamePluralPascalCase = Name.GetPascalCase(Name.GetPluralForm(foreignKey.ForeignKeyName));
																	foreignKey.ForeignKeyNamePluralCamelCase = Name.GetCamelCase(Name.GetPluralForm(foreignKey.ForeignKeyName));
																	foreignKey.ForeignKeyNamePluralConstantCase = Name.GetConstantCase(Name.GetPluralForm(foreignKey.ForeignKeyName));

																	foreignKey.ForeignKeyNameSqlMetalPascalCase = Name.GetSqlMetalPascalCase(foreignKey.ForeignKeyName);
																	foreignKey.ForeignKeyNameSqlMetalCamelCase = Name.GetSqlMetalCamelCase(foreignKey.ForeignKeyName);
																	foreignKey.ForeignKeyNameSqlMetalSingularPascalCase = Name.GetSqlMetalPascalCase(Name.GetSingularForm(foreignKey.ForeignKeyName));
																	foreignKey.ForeignKeyNameSqlMetalSingularCamelCase = Name.GetSqlMetalCamelCase(Name.GetSingularForm(foreignKey.ForeignKeyName));
																	foreignKey.ForeignKeyNameSqlMetalPluralPascalCase = Name.GetSqlMetalPascalCase(Name.GetPluralForm(foreignKey.ForeignKeyName));
																	foreignKey.ForeignKeyNameSqlMetalPluralCamelCase = Name.GetSqlMetalCamelCase(Name.GetPluralForm(foreignKey.ForeignKeyName));


																	foreignKey.TargetSchemaName = DataType.ChangeType<string>(dictDataForeignKey["TargetSchemaName"]);
																	foreignKey.TargetSchemaNamePascalCase = Name.GetPascalCase(foreignKey.TargetSchemaName);
																	foreignKey.TargetSchemaNameCamelCase = Name.GetCamelCase(foreignKey.TargetSchemaName);
																	foreignKey.TargetSchemaNameConstantCase = Name.GetConstantCase(foreignKey.TargetSchemaName);
																	foreignKey.TargetSchemaNameSingularPascalCase = Name.GetPascalCase(Name.GetSingularForm(foreignKey.TargetSchemaName));
																	foreignKey.TargetSchemaNameSingularCamelCase = Name.GetCamelCase(Name.GetSingularForm(foreignKey.TargetSchemaName));
																	foreignKey.TargetSchemaNameSingularConstantCase = Name.GetConstantCase(Name.GetSingularForm(foreignKey.TargetSchemaName));
																	foreignKey.TargetSchemaNamePluralPascalCase = Name.GetPascalCase(Name.GetPluralForm(foreignKey.TargetSchemaName));
																	foreignKey.TargetSchemaNamePluralCamelCase = Name.GetCamelCase(Name.GetPluralForm(foreignKey.TargetSchemaName));
																	foreignKey.TargetSchemaNamePluralConstantCase = Name.GetConstantCase(Name.GetPluralForm(foreignKey.TargetSchemaName));

																	foreignKey.TargetSchemaNameSqlMetalPascalCase = Name.GetSqlMetalPascalCase(foreignKey.TargetSchemaName);
																	foreignKey.TargetSchemaNameSqlMetalCamelCase = Name.GetSqlMetalCamelCase(foreignKey.TargetSchemaName);
																	foreignKey.TargetSchemaNameSqlMetalSingularPascalCase = Name.GetSqlMetalPascalCase(Name.GetSingularForm(foreignKey.TargetSchemaName));
																	foreignKey.TargetSchemaNameSqlMetalSingularCamelCase = Name.GetSqlMetalCamelCase(Name.GetSingularForm(foreignKey.TargetSchemaName));
																	foreignKey.TargetSchemaNameSqlMetalPluralPascalCase = Name.GetSqlMetalPascalCase(Name.GetPluralForm(foreignKey.TargetSchemaName));
																	foreignKey.TargetSchemaNameSqlMetalPluralCamelCase = Name.GetSqlMetalCamelCase(Name.GetPluralForm(foreignKey.TargetSchemaName));

																	
																	foreignKey.TargetTableName = DataType.ChangeType<string>(dictDataForeignKey["TargetTableName"]);
																	foreignKey.TargetTableNamePascalCase = Name.GetPascalCase(foreignKey.TargetTableName);
																	foreignKey.TargetTableNameCamelCase = Name.GetCamelCase(foreignKey.TargetTableName);
																	foreignKey.TargetTableNameConstantCase = Name.GetConstantCase(foreignKey.TargetTableName);
																	foreignKey.TargetTableNameSingularPascalCase = Name.GetPascalCase(Name.GetSingularForm(foreignKey.TargetTableName));
																	foreignKey.TargetTableNameSingularCamelCase = Name.GetCamelCase(Name.GetSingularForm(foreignKey.TargetTableName));
																	foreignKey.TargetTableNameSingularConstantCase = Name.GetConstantCase(Name.GetSingularForm(foreignKey.TargetTableName));
																	foreignKey.TargetTableNamePluralPascalCase = Name.GetPascalCase(Name.GetPluralForm(foreignKey.TargetTableName));
																	foreignKey.TargetTableNamePluralCamelCase = Name.GetCamelCase(Name.GetPluralForm(foreignKey.TargetTableName));
																	foreignKey.TargetTableNamePluralConstantCase = Name.GetConstantCase(Name.GetPluralForm(foreignKey.TargetTableName));

																	foreignKey.TargetTableNameSqlMetalPascalCase = Name.GetSqlMetalPascalCase(foreignKey.TargetTableName);
																	foreignKey.TargetTableNameSqlMetalCamelCase = Name.GetSqlMetalCamelCase(foreignKey.TargetTableName);
																	foreignKey.TargetTableNameSqlMetalSingularPascalCase = Name.GetSqlMetalPascalCase(Name.GetSingularForm(foreignKey.TargetTableName));
																	foreignKey.TargetTableNameSqlMetalSingularCamelCase = Name.GetSqlMetalCamelCase(Name.GetSingularForm(foreignKey.TargetTableName));
																	foreignKey.TargetTableNameSqlMetalPluralPascalCase = Name.GetSqlMetalPascalCase(Name.GetPluralForm(foreignKey.TargetTableName));
																	foreignKey.TargetTableNameSqlMetalPluralCamelCase = Name.GetSqlMetalCamelCase(Name.GetPluralForm(foreignKey.TargetTableName));

																	table.ForeignKeys.Add(foreignKey);

																	var dictEnumForeignKeyColumn = unitOfWork.ExecuteDictionary(CommandType.Text, GetAllAssemblyResourceFileText(this.GetType(), dataSourceTag, "ForeignKeyColumns"), this.CoreGetForeignKeyColumnParameters(unitOfWork, dataSourceTag, server, database, schema, table, foreignKey), out recordsAffected);
																	{
																		if ((object)dictEnumForeignKeyColumn != null)
																		{
																			foreach (var dictDataForeignKeyColumn in dictEnumForeignKeyColumn)
																			{
																				ForeignKeyColumn foreignKeyColumn;

																				foreignKeyColumn = new ForeignKeyColumn();

																				foreignKeyColumn.ForeignKeyColumnOrdinal = DataType.ChangeType<int>(dictDataForeignKeyColumn["ForeignKeyColumnOrdinal"]);
																				foreignKeyColumn.ColumnOrdinal = DataType.ChangeType<int>(dictDataForeignKeyColumn["ColumnOrdinal"]);
																				foreignKeyColumn.ColumnName = DataType.ChangeType<string>(dictDataForeignKeyColumn["ColumnName"]);
																				foreignKeyColumn.TargetColumnOrdinal = DataType.ChangeType<int>(dictDataForeignKeyColumn["TargetColumnOrdinal"]);
																				foreignKeyColumn.TargetColumnName = DataType.ChangeType<string>(dictDataForeignKeyColumn["TargetColumnName"]);

																				foreignKey.ForeignKeyColumns.Add(foreignKeyColumn);
																			}
																		}
																	}
																}
															}
														}

														var dictEnumUniqueKey = unitOfWork.ExecuteDictionary(CommandType.Text, GetAllAssemblyResourceFileText(this.GetType(), dataSourceTag, "UniqueKeys"), this.CoreGetUniqueKeyParameters(unitOfWork, dataSourceTag, server, database, schema, table), out recordsAffected);
														{
															if ((object)dictEnumUniqueKey != null)
															{
																foreach (var dictDataUniqueKey in dictEnumUniqueKey)
																{
																	UniqueKey uniqueKey;

																	uniqueKey = new UniqueKey();

																	uniqueKey.UniqueKeyId = DataType.ChangeType<int>(dictDataUniqueKey["UniqueKeyId"]);
																	uniqueKey.UniqueKeyName = DataType.ChangeType<string>(dictDataUniqueKey["UniqueKeyName"]);
																	uniqueKey.UniqueKeyIsSystemNamed = DataType.ChangeType<bool>(dictDataUniqueKey["UniqueKeyIsSystemNamed"]);
																	uniqueKey.UniqueKeyNamePascalCase = Name.GetPascalCase(uniqueKey.UniqueKeyName);
																	uniqueKey.UniqueKeyNameCamelCase = Name.GetCamelCase(uniqueKey.UniqueKeyName);
																	uniqueKey.UniqueKeyNameConstantCase = Name.GetConstantCase(uniqueKey.UniqueKeyName);
																	uniqueKey.UniqueKeyNameSingularPascalCase = Name.GetPascalCase(Name.GetSingularForm(uniqueKey.UniqueKeyName));
																	uniqueKey.UniqueKeyNameSingularCamelCase = Name.GetCamelCase(Name.GetSingularForm(uniqueKey.UniqueKeyName));
																	uniqueKey.UniqueKeyNameSingularConstantCase = Name.GetConstantCase(Name.GetSingularForm(uniqueKey.UniqueKeyName));
																	uniqueKey.UniqueKeyNamePluralPascalCase = Name.GetPascalCase(Name.GetPluralForm(uniqueKey.UniqueKeyName));
																	uniqueKey.UniqueKeyNamePluralCamelCase = Name.GetCamelCase(Name.GetPluralForm(uniqueKey.UniqueKeyName));
																	uniqueKey.UniqueKeyNamePluralConstantCase = Name.GetConstantCase(Name.GetPluralForm(uniqueKey.UniqueKeyName));

																	uniqueKey.UniqueKeyNameSqlMetalPascalCase = Name.GetSqlMetalPascalCase(uniqueKey.UniqueKeyName);
																	uniqueKey.UniqueKeyNameSqlMetalCamelCase = Name.GetSqlMetalCamelCase(uniqueKey.UniqueKeyName);
																	uniqueKey.UniqueKeyNameSqlMetalSingularPascalCase = Name.GetSqlMetalPascalCase(Name.GetSingularForm(uniqueKey.UniqueKeyName));
																	uniqueKey.UniqueKeyNameSqlMetalSingularCamelCase = Name.GetSqlMetalCamelCase(Name.GetSingularForm(uniqueKey.UniqueKeyName));
																	uniqueKey.UniqueKeyNameSqlMetalPluralPascalCase = Name.GetSqlMetalPascalCase(Name.GetPluralForm(uniqueKey.UniqueKeyName));
																	uniqueKey.UniqueKeyNameSqlMetalPluralCamelCase = Name.GetSqlMetalCamelCase(Name.GetPluralForm(uniqueKey.UniqueKeyName));

																	table.UniqueKeys.Add(uniqueKey);

																	var dictEnumUniqueKeyColumn = unitOfWork.ExecuteDictionary(CommandType.Text, GetAllAssemblyResourceFileText(this.GetType(), dataSourceTag, "UniqueKeyColumns"), this.CoreGetUniqueKeyColumnParameters(unitOfWork, dataSourceTag, server, database, schema, table, uniqueKey), out recordsAffected);
																	{
																		if ((object)dictEnumUniqueKeyColumn != null)
																		{
																			foreach (var dictDataUniqueKeyColumn in dictEnumUniqueKeyColumn)
																			{
																				UniqueKeyColumn uniqueKeyColumn;

																				uniqueKeyColumn = new UniqueKeyColumn();

																				uniqueKeyColumn.UniqueKeyColumnOrdinal = DataType.ChangeType<int>(dictDataUniqueKeyColumn["UniqueKeyColumnOrdinal"]);
																				uniqueKeyColumn.ColumnOrdinal = DataType.ChangeType<int>(dictDataUniqueKeyColumn["ColumnOrdinal"]);
																				uniqueKeyColumn.ColumnName = DataType.ChangeType<string>(dictDataUniqueKeyColumn["ColumnName"]);
																				uniqueKeyColumn.UniqueKeyColumnDescendingSort = DataType.ChangeType<bool>(dictDataUniqueKeyColumn["UniqueKeyColumnDescendingSort"]);

																				uniqueKey.UniqueKeyColumns.Add(uniqueKeyColumn);
																			}
																		}
																	}
																}
															}
														}
													}
												}

												var dictEnumView = unitOfWork.ExecuteDictionary(CommandType.Text, GetAllAssemblyResourceFileText(this.GetType(), dataSourceTag, "Views"), this.CoreGetTableParameters(unitOfWork, dataSourceTag, server, database, schema), out recordsAffected);
												{
													foreach (var dictDataView in dictEnumView)
													{
														View view;

														view = new View();
														view.ViewId = DataType.ChangeType<int>(dictDataView["ViewId"]);
														view.ViewName = DataType.ChangeType<string>(dictDataView["ViewName"]);
														view.CreationTimestamp = DataType.ChangeType<DateTime>(dictDataView["CreationTimestamp"]);
														view.ModificationTimestamp = DataType.ChangeType<DateTime>(dictDataView["ModificationTimestamp"]);
														view.IsImplementationDetail = DataType.ChangeType<bool>(dictDataView["IsImplementationDetail"]);
														view.ViewNamePascalCase = Name.GetPascalCase(view.ViewName);
														view.ViewNameCamelCase = Name.GetCamelCase(view.ViewName);
														view.ViewNameConstantCase = Name.GetConstantCase(view.ViewName);
														view.ViewNameSingularPascalCase = Name.GetPascalCase(Name.GetSingularForm(view.ViewName));
														view.ViewNameSingularCamelCase = Name.GetCamelCase(Name.GetSingularForm(view.ViewName));
														view.ViewNameSingularConstantCase = Name.GetConstantCase(Name.GetSingularForm(view.ViewName));
														view.ViewNamePluralPascalCase = Name.GetPascalCase(Name.GetPluralForm(view.ViewName));
														view.ViewNamePluralCamelCase = Name.GetCamelCase(Name.GetPluralForm(view.ViewName));
														view.ViewNamePluralConstantCase = Name.GetConstantCase(Name.GetPluralForm(view.ViewName));

														view.ViewNameSqlMetalPascalCase = Name.GetSqlMetalPascalCase(view.ViewName);
														view.ViewNameSqlMetalCamelCase = Name.GetSqlMetalCamelCase(view.ViewName);
														view.ViewNameSqlMetalSingularPascalCase = Name.GetSqlMetalPascalCase(Name.GetSingularForm(view.ViewName));
														view.ViewNameSqlMetalSingularCamelCase = Name.GetSqlMetalCamelCase(Name.GetSingularForm(view.ViewName));
														view.ViewNameSqlMetalPluralPascalCase = Name.GetSqlMetalPascalCase(Name.GetPluralForm(view.ViewName));
														view.ViewNameSqlMetalPluralCamelCase = Name.GetSqlMetalCamelCase(Name.GetPluralForm(view.ViewName));

														// filter unwanted tables (objects)
														if ((object)objectFilter != null)
														{
															if (!objectFilter.Contains(view.ViewName))
																continue;
														}

														schema.Views.Add(view);

														var dictEnumColumn = unitOfWork.ExecuteDictionary(CommandType.Text, GetAllAssemblyResourceFileText(this.GetType(), dataSourceTag, "ViewColumns"), this.CoreGetColumnParameters(unitOfWork, dataSourceTag, server, database, schema, view), out recordsAffected);
														{
															if ((object)dictEnumColumn != null)
															{
																foreach (var dictDataColumn in dictEnumColumn)
																{
																	ViewColumn column;

																	column = new ViewColumn();

																	column.ColumnName = DataType.ChangeType<string>(dictDataColumn["ColumnName"]);
																	column.ColumnOrdinal = DataType.ChangeType<int>(dictDataColumn["ColumnOrdinal"]);
																	column.ColumnNullable = DataType.ChangeType<bool>(dictDataColumn["ColumnNullable"]);
																	column.ColumnSize = DataType.ChangeType<int>(dictDataColumn["ColumnSize"]);
																	column.ColumnPrecision = DataType.ChangeType<int>(dictDataColumn["ColumnPrecision"]);
																	column.ColumnScale = DataType.ChangeType<int>(dictDataColumn["ColumnScale"]);
																	column.ColumnSqlType = DataType.ChangeType<string>(dictDataColumn["ColumnSqlType"]);
																	column.ColumnIsUserDefinedType = DataType.ChangeType<bool>(dictDataColumn["ColumnIsUserDefinedType"]);
																	column.ColumnNamePascalCase = Name.GetPascalCase(column.ColumnName);
																	column.ColumnNameCamelCase = Name.GetCamelCase(column.ColumnName);
																	column.ColumnNameConstantCase = Name.GetConstantCase(column.ColumnName);
																	column.ColumnNameSingularPascalCase = Name.GetPascalCase(Name.GetSingularForm(column.ColumnName));
																	column.ColumnNameSingularCamelCase = Name.GetCamelCase(Name.GetSingularForm(column.ColumnName));
																	column.ColumnNameSingularConstantCase = Name.GetConstantCase(Name.GetSingularForm(column.ColumnName));
																	column.ColumnNamePluralPascalCase = Name.GetPascalCase(Name.GetPluralForm(column.ColumnName));
																	column.ColumnNamePluralCamelCase = Name.GetCamelCase(Name.GetPluralForm(column.ColumnName));
																	column.ColumnNamePluralConstantCase = Name.GetConstantCase(Name.GetPluralForm(column.ColumnName));

																	column.ColumnNameSqlMetalPascalCase = Name.GetSqlMetalPascalCase(column.ColumnName);
																	column.ColumnNameSqlMetalCamelCase = Name.GetSqlMetalCamelCase(column.ColumnName);
																	column.ColumnNameSqlMetalSingularPascalCase = Name.GetSqlMetalPascalCase(Name.GetSingularForm(column.ColumnName));
																	column.ColumnNameSqlMetalSingularCamelCase = Name.GetSqlMetalCamelCase(Name.GetSingularForm(column.ColumnName));
																	column.ColumnNameSqlMetalPluralPascalCase = Name.GetSqlMetalPascalCase(Name.GetPluralForm(column.ColumnName));
																	column.ColumnNameSqlMetalPluralCamelCase = Name.GetSqlMetalCamelCase(Name.GetPluralForm(column.ColumnName));

																	clrType = this.CoreInferClrTypeForSqlType(dataSourceTag, column.ColumnSqlType, column.ColumnPrecision);
																	column.ColumnDbType = AdoNetHelper.InferDbTypeForClrType(clrType);
																	column.ColumnSize = this.CoreCalculateColumnSize(dataSourceTag, column); //recalculate

																	column.ColumnClrType = clrType ?? typeof(object);
																	column.ColumnClrNullableType = Reflexion.MakeNullableType(clrType);
																	column.ColumnClrNonNullableType = Reflexion.MakeNonNullableType(clrType);
																	column.ColumnCSharpNullableLiteral = column.ColumnNullable.ToString().ToLower();
																	column.ColumnCSharpDbType = string.Format("{0}.{1}", typeof(DbType).Name, column.ColumnDbType);
																	column.ColumnCSharpClrType = (object)column.ColumnClrType != null ? FormatCSharpType(column.ColumnClrType) : FormatCSharpType(typeof(object));
																	column.ColumnCSharpClrNullableType = (object)column.ColumnClrNullableType != null ? FormatCSharpType(column.ColumnClrNullableType) : FormatCSharpType(typeof(object));
																	column.ColumnCSharpClrNonNullableType = (object)column.ColumnClrNonNullableType != null ? FormatCSharpType(column.ColumnClrNonNullableType) : FormatCSharpType(typeof(object));

																	view.Columns.Add(column);
																}
															}
														}
													}
												}

												var dictEnumProcedure = unitOfWork.ExecuteDictionary(CommandType.Text, GetAllAssemblyResourceFileText(this.GetType(), dataSourceTag, "Procedures"), this.CoreGetProcedureParameters(unitOfWork, dataSourceTag, server, database, schema), out recordsAffected);
												{
													if ((object)dictEnumProcedure != null)
													{
														foreach (var dictDataProcedure in dictEnumProcedure)
														{
															Procedure procedure;

															procedure = new Procedure();
															procedure.ProcedureName = DataType.ChangeType<string>(dictDataProcedure["ProcedureName"]);
															procedure.ProcedureNamePascalCase = Name.GetPascalCase(procedure.ProcedureName);
															procedure.ProcedureNameCamelCase = Name.GetCamelCase(procedure.ProcedureName);
															procedure.ProcedureNameConstantCase = Name.GetConstantCase(procedure.ProcedureName);
															procedure.ProcedureNameSingularPascalCase = Name.GetPascalCase(Name.GetSingularForm(procedure.ProcedureName));
															procedure.ProcedureNameSingularCamelCase = Name.GetCamelCase(Name.GetSingularForm(procedure.ProcedureName));
															procedure.ProcedureNameSingularConstantCase = Name.GetConstantCase(Name.GetSingularForm(procedure.ProcedureName));
															procedure.ProcedureNamePluralPascalCase = Name.GetPascalCase(Name.GetPluralForm(procedure.ProcedureName));
															procedure.ProcedureNamePluralCamelCase = Name.GetCamelCase(Name.GetPluralForm(procedure.ProcedureName));
															procedure.ProcedureNamePluralConstantCase = Name.GetConstantCase(Name.GetPluralForm(procedure.ProcedureName));

															procedure.ProcedureNameSqlMetalPascalCase = Name.GetSqlMetalPascalCase(procedure.ProcedureName);
															procedure.ProcedureNameSqlMetalCamelCase = Name.GetSqlMetalCamelCase(procedure.ProcedureName);
															procedure.ProcedureNameSqlMetalSingularPascalCase = Name.GetSqlMetalPascalCase(Name.GetSingularForm(procedure.ProcedureName));
															procedure.ProcedureNameSqlMetalSingularCamelCase = Name.GetSqlMetalCamelCase(Name.GetSingularForm(procedure.ProcedureName));
															procedure.ProcedureNameSqlMetalPluralPascalCase = Name.GetSqlMetalPascalCase(Name.GetPluralForm(procedure.ProcedureName));
															procedure.ProcedureNameSqlMetalPluralCamelCase = Name.GetSqlMetalCamelCase(Name.GetPluralForm(procedure.ProcedureName));

															// filter unwanted procedures (objects)
															if ((object)objectFilter != null)
															{
																if (!objectFilter.Contains(procedure.ProcedureName))
																	continue;
															}

															schema.Procedures.Add(procedure);

															var dictEnumParameter = unitOfWork.ExecuteDictionary(CommandType.Text, GetAllAssemblyResourceFileText(this.GetType(), dataSourceTag, "ProcedureParameters"), this.CoreGetParameterParameters(unitOfWork, dataSourceTag, server, database, schema, procedure), out recordsAffected);
															{
																if ((object)dictEnumParameter != null)
																{
																	foreach (var dictDataParameter in dictEnumParameter)
																	{
																		Parameter parameter;

																		parameter = new Parameter();

																		parameter.ParameterPrefix = DataType.ChangeType<string>(dictDataParameter["ParameterName"]).Substring(0, 1);
																		parameter.ParameterName = DataType.ChangeType<string>(dictDataParameter["ParameterName"]).Substring(1);
																		parameter.ParameterOrdinal = DataType.ChangeType<int>(dictDataParameter["ParameterOrdinal"]);
																		parameter.ParameterSize = DataType.ChangeType<int>(dictDataParameter["ParameterSize"]);
																		parameter.ParameterPrecision = DataType.ChangeType<int>(dictDataParameter["ParameterPrecision"]);
																		parameter.ParameterScale = DataType.ChangeType<int>(dictDataParameter["ParameterScale"]);
																		parameter.ParameterSqlType = DataType.ChangeType<string>(dictDataParameter["ParameterSqlType"]);
																		parameter.ParameterIsUserDefinedType = DataType.ChangeType<bool>(dictDataParameter["ParameterIsUserDefinedType"]);
																		parameter.ParameterIsOutput = DataType.ChangeType<bool>(dictDataParameter["ParameterIsOutput"]);
																		parameter.ParameterIsReadOnly = DataType.ChangeType<bool>(dictDataParameter["ParameterIsReadOnly"]);
																		parameter.ParameterIsCursorRef = DataType.ChangeType<bool>(dictDataParameter["ParameterIsCursorRef"]);
																		parameter.ParameterIsReturnValue = DataType.ChangeType<bool>(dictDataParameter["ParameterIsReturnValue"]);
																		parameter.ParameterHasDefault = DataType.ChangeType<bool>(dictDataParameter["ParameterHasDefault"]);
																		parameter.ParameterNullable = DataType.ChangeType<bool?>(dictDataParameter["ParameterNullable"]) ?? true;
																		parameter.ParameterDefaultValue = DataType.ChangeType<string>(dictDataParameter["ParameterDefaultValue"]);
																		parameter.ParameterIsResultColumn = DataType.ChangeType<bool>(dictDataParameter["ParameterIsResultColumn"]);
																		parameter.ParameterNamePascalCase = Name.GetPascalCase(parameter.ParameterName);
																		parameter.ParameterNameCamelCase = Name.GetCamelCase(parameter.ParameterName);
																		parameter.ParameterNameConstantCase = Name.GetConstantCase(parameter.ParameterName);
																		parameter.ParameterNameSingularPascalCase = Name.GetPascalCase(Name.GetSingularForm(parameter.ParameterName));
																		parameter.ParameterNameSingularCamelCase = Name.GetCamelCase(Name.GetSingularForm(parameter.ParameterName));
																		parameter.ParameterNameSingularConstantCase = Name.GetConstantCase(Name.GetSingularForm(parameter.ParameterName));
																		parameter.ParameterNamePluralPascalCase = Name.GetPascalCase(Name.GetPluralForm(parameter.ParameterName));
																		parameter.ParameterNamePluralCamelCase = Name.GetCamelCase(Name.GetPluralForm(parameter.ParameterName));
																		parameter.ParameterNamePluralConstantCase = Name.GetConstantCase(Name.GetPluralForm(parameter.ParameterName));

																		parameter.ParameterNameSqlMetalPascalCase = Name.GetSqlMetalPascalCase(parameter.ParameterName);
																		parameter.ParameterNameSqlMetalCamelCase = Name.GetSqlMetalCamelCase(parameter.ParameterName);
																		parameter.ParameterNameSqlMetalSingularPascalCase = Name.GetSqlMetalPascalCase(Name.GetSingularForm(parameter.ParameterName));
																		parameter.ParameterNameSqlMetalSingularCamelCase = Name.GetSqlMetalCamelCase(Name.GetSingularForm(parameter.ParameterName));
																		parameter.ParameterNameSqlMetalPluralPascalCase = Name.GetSqlMetalPascalCase(Name.GetPluralForm(parameter.ParameterName));
																		parameter.ParameterNameSqlMetalPluralCamelCase = Name.GetSqlMetalCamelCase(Name.GetPluralForm(parameter.ParameterName));

																		parameter.ParameterDirection = (parameter.ParameterIsOutput || parameter.ParameterIsReadOnly) ? ParameterDirection.Output : ParameterDirection.Input;

																		clrType = this.CoreInferClrTypeForSqlType(dataSourceTag, parameter.ParameterSqlType, parameter.ParameterPrecision);
																		parameter.ParameterDbType = AdoNetHelper.InferDbTypeForClrType(clrType);
																		parameter.ParameterSize = this.CoreCalculateParameterSize(dataSourceTag, parameter);

																		parameter.ParameterClrType = clrType;
																		parameter.ParameterClrNullableType = Reflexion.MakeNullableType(clrType);
																		parameter.ParameterClrNonNullableType = Reflexion.MakeNonNullableType(clrType);
																		parameter.ParameterCSharpDbType = string.Format("{0}.{1}", typeof(DbType).Name, parameter.ParameterDbType);
																		parameter.ParameterCSharpClrType = (object)parameter.ParameterClrType != null ? FormatCSharpType(parameter.ParameterClrType) : FormatCSharpType(typeof(object));
																		parameter.ParameterCSharpClrNullableType = (object)parameter.ParameterClrNullableType != null ? FormatCSharpType(parameter.ParameterClrNullableType) : FormatCSharpType(typeof(object));
																		parameter.ParameterCSharpClrNonNullableType = (object)parameter.ParameterClrNonNullableType != null ? FormatCSharpType(parameter.ParameterClrNonNullableType) : FormatCSharpType(typeof(object));
																		parameter.ParameterCSharpNullableLiteral = parameter.ParameterNullable.ToString().ToLower();

																		procedure.Parameters.Add(parameter);
																	}
																}

																// implicit return value parameter
																if (this.CoreGetEmitImplicitReturnParameter(dataSourceTag))
																{
																	Parameter parameter;

																	parameter = new Parameter();

																	parameter.ParameterPrefix = this.CoreGetParameterPrefix(dataSourceTag);
																	parameter.ParameterName = RETURN_VALUE;
																	parameter.ParameterOrdinal = int.MaxValue;
																	parameter.ParameterSize = 0;
																	parameter.ParameterPrecision = 0;
																	parameter.ParameterScale = 0;
																	parameter.ParameterSqlType = "int";
																	parameter.ParameterIsOutput = true;
																	parameter.ParameterIsReadOnly = true;
																	parameter.ParameterIsCursorRef = false;
																	parameter.ParameterIsReturnValue = true;
																	parameter.ParameterDefaultValue = null;
																	parameter.ParameterIsResultColumn = false;
																	parameter.ParameterNamePascalCase = Name.GetPascalCase(RETURN_VALUE);
																	parameter.ParameterNameCamelCase = Name.GetCamelCase(RETURN_VALUE);
																	parameter.ParameterNameConstantCase = Name.GetConstantCase(RETURN_VALUE);
																	parameter.ParameterNameSingularPascalCase = Name.GetPascalCase(Name.GetSingularForm(RETURN_VALUE));
																	parameter.ParameterNameSingularCamelCase = Name.GetCamelCase(Name.GetSingularForm(RETURN_VALUE));
																	parameter.ParameterNameSingularConstantCase = Name.GetConstantCase(Name.GetSingularForm(RETURN_VALUE));
																	parameter.ParameterNamePluralPascalCase = Name.GetPascalCase(Name.GetPluralForm(RETURN_VALUE));
																	parameter.ParameterNamePluralCamelCase = Name.GetCamelCase(Name.GetPluralForm(RETURN_VALUE));
																	parameter.ParameterNamePluralConstantCase = Name.GetConstantCase(Name.GetPluralForm(RETURN_VALUE));

																	parameter.ParameterNameSqlMetalPascalCase = Name.GetSqlMetalPascalCase(RETURN_VALUE);
																	parameter.ParameterNameSqlMetalCamelCase = Name.GetSqlMetalCamelCase(RETURN_VALUE);
																	parameter.ParameterNameSqlMetalSingularPascalCase = Name.GetSqlMetalPascalCase(Name.GetSingularForm(RETURN_VALUE));
																	parameter.ParameterNameSqlMetalSingularCamelCase = Name.GetSqlMetalCamelCase(Name.GetSingularForm(RETURN_VALUE));
																	parameter.ParameterNameSqlMetalPluralPascalCase = Name.GetSqlMetalPascalCase(Name.GetPluralForm(RETURN_VALUE));
																	parameter.ParameterNameSqlMetalPluralCamelCase = Name.GetSqlMetalCamelCase(Name.GetPluralForm(RETURN_VALUE));

																	parameter.ParameterNullable = true;
																	parameter.ParameterDirection = ParameterDirection.ReturnValue;

																	clrType = this.CoreInferClrTypeForSqlType(dataSourceTag, parameter.ParameterSqlType, parameter.ParameterPrecision);
																	parameter.ParameterDbType = AdoNetHelper.InferDbTypeForClrType(clrType);
																	parameter.ParameterSize = this.CoreCalculateParameterSize(dataSourceTag, parameter);

																	parameter.ParameterClrType = clrType;
																	parameter.ParameterClrNullableType = Reflexion.MakeNullableType(clrType);
																	parameter.ParameterClrNonNullableType = Reflexion.MakeNonNullableType(clrType);
																	parameter.ParameterCSharpDbType = string.Format("{0}.{1}", typeof(DbType).Name, parameter.ParameterDbType);
																	parameter.ParameterCSharpClrType = (object)parameter.ParameterClrType != null ? FormatCSharpType(parameter.ParameterClrType) : FormatCSharpType(typeof(object));
																	parameter.ParameterCSharpClrNullableType = (object)parameter.ParameterClrNullableType != null ? FormatCSharpType(parameter.ParameterClrNullableType) : FormatCSharpType(typeof(object));
																	parameter.ParameterCSharpClrNonNullableType = (object)parameter.ParameterClrNonNullableType != null ? FormatCSharpType(parameter.ParameterClrNonNullableType) : FormatCSharpType(typeof(object));
																	parameter.ParameterCSharpNullableLiteral = parameter.ParameterNullable.ToString().ToLower();

																	procedure.Parameters.Add(parameter);
																}
															}

															// re-map result column parameters into first class columns
															Parameter[] columnParameters;
															columnParameters = procedure.Parameters.Where(p => p.ParameterIsResultColumn).ToArray();

															if ((object)columnParameters != null && columnParameters.Length > 0)
															{
																foreach (Parameter columnParameter in columnParameters)
																{
																	ProcedureColumn column;

																	column = new ProcedureColumn();

																	column.ColumnName = columnParameter.ParameterName;
																	column.ColumnOrdinal = columnParameter.ParameterOrdinal;
																	column.ColumnNullable = columnParameter.ParameterNullable;
																	column.ColumnSize = columnParameter.ParameterSize;
																	column.ColumnPrecision = columnParameter.ParameterPrecision;
																	column.ColumnScale = columnParameter.ParameterScale;
																	column.ColumnSqlType = columnParameter.ParameterSqlType;
																	column.ColumnIsUserDefinedType = columnParameter.ParameterIsUserDefinedType;
																	column.ColumnHasDefault = !DataType.IsNullOrWhiteSpace(columnParameter.ParameterDefaultValue);
																	column.ColumnNamePascalCase = Name.GetPascalCase(columnParameter.ParameterName);
																	column.ColumnNameCamelCase = Name.GetCamelCase(columnParameter.ParameterName);
																	column.ColumnNameConstantCase = Name.GetConstantCase(columnParameter.ParameterName);
																	column.ColumnNameSingularPascalCase = Name.GetPascalCase(Name.GetSingularForm(columnParameter.ParameterName));
																	column.ColumnNameSingularCamelCase = Name.GetCamelCase(Name.GetSingularForm(columnParameter.ParameterName));
																	column.ColumnNameSingularConstantCase = Name.GetConstantCase(Name.GetSingularForm(columnParameter.ParameterName));
																	column.ColumnNamePluralPascalCase = Name.GetPascalCase(Name.GetPluralForm(columnParameter.ParameterName));
																	column.ColumnNamePluralCamelCase = Name.GetCamelCase(Name.GetPluralForm(columnParameter.ParameterName));
																	column.ColumnNamePluralConstantCase = Name.GetConstantCase(Name.GetPluralForm(columnParameter.ParameterName));

																	column.ColumnNameSqlMetalPascalCase = Name.GetSqlMetalPascalCase(columnParameter.ParameterName);
																	column.ColumnNameSqlMetalCamelCase = Name.GetSqlMetalCamelCase(columnParameter.ParameterName);
																	column.ColumnNameSqlMetalSingularPascalCase = Name.GetSqlMetalPascalCase(Name.GetSingularForm(columnParameter.ParameterName));
																	column.ColumnNameSqlMetalSingularCamelCase = Name.GetSqlMetalCamelCase(Name.GetSingularForm(columnParameter.ParameterName));
																	column.ColumnNameSqlMetalPluralPascalCase = Name.GetSqlMetalPascalCase(Name.GetPluralForm(columnParameter.ParameterName));
																	column.ColumnNameSqlMetalPluralCamelCase = Name.GetSqlMetalCamelCase(Name.GetPluralForm(columnParameter.ParameterName));

																	clrType = this.CoreInferClrTypeForSqlType(dataSourceTag, columnParameter.ParameterSqlType, columnParameter.ParameterPrecision);
																	column.ColumnDbType = AdoNetHelper.InferDbTypeForClrType(clrType);
																	column.ColumnSize = this.CoreCalculateColumnSize(dataSourceTag, column); //recalculate

																	column.ColumnClrType = clrType ?? typeof(object);
																	column.ColumnClrNullableType = Reflexion.MakeNullableType(clrType);
																	column.ColumnClrNonNullableType = Reflexion.MakeNonNullableType(clrType);
																	column.ColumnCSharpNullableLiteral = column.ColumnNullable.ToString().ToLower();
																	column.ColumnCSharpDbType = string.Format("{0}.{1}", typeof(DbType).Name, column.ColumnDbType);
																	column.ColumnCSharpClrType = (object)column.ColumnClrType != null ? FormatCSharpType(column.ColumnClrType) : FormatCSharpType(typeof(object));
																	column.ColumnCSharpClrNullableType = (object)column.ColumnClrNullableType != null ? FormatCSharpType(column.ColumnClrNullableType) : FormatCSharpType(typeof(object));
																	column.ColumnCSharpClrNonNullableType = (object)column.ColumnClrNonNullableType != null ? FormatCSharpType(column.ColumnClrNonNullableType) : FormatCSharpType(typeof(object));

																	//procedure.Columns.Add(column);
																	procedure.Parameters.Remove(columnParameter);
																}
															}

															if (!disableProcSchDisc)
															{
																// REFERENCE:
																// http://connect.microsoft.com/VisualStudio/feedback/details/314650/sqm1014-sqlmetal-ignores-stored-procedures-that-use-temp-tables
																IDataParameter[] parameters;
																parameters = procedure.Parameters.Where(p => !p.ParameterIsReturnValue && !p.ParameterIsResultColumn).Select(p => unitOfWork.CreateParameter(p.ParameterIsOutput ? ParameterDirection.Output : ParameterDirection.Input, p.ParameterDbType, p.ParameterSize, (byte)p.ParameterPrecision, (byte)p.ParameterScale, p.ParameterNullable, p.ParameterName, null)).ToArray();

																try
																{
																	var dictEnumMetadata = AdoNetHelper.ExecuteSchema(unitOfWork, CommandType.StoredProcedure, string.Format(GetAllAssemblyResourceFileText(this.GetType(), dataSourceTag, "ProcedureSchema"), server.ServerName, database.DatabaseName, schema.SchemaName, procedure.ProcedureName), parameters, out recordsAffected);
																	{
																		if ((object)dictEnumMetadata != null)
																		{
																			foreach (var dictDataMetadata in dictEnumMetadata)
																			{
																				ProcedureColumn column;

																				column = new ProcedureColumn();

																				column.ColumnName = DataType.ChangeType<string>(dictDataMetadata["ColumnName"]);
																				column.ColumnOrdinal = DataType.ChangeType<int>(dictDataMetadata["ColumnOrdinal"]);
																				column.ColumnNullable = DataType.ChangeType<bool>(dictDataMetadata["AllowDBNull"]);
																				column.ColumnSize = DataType.ChangeType<int>(dictDataMetadata["ColumnSize"]);
																				column.ColumnPrecision = DataType.ChangeType<int>(dictDataMetadata["NumericPrecision"]);
																				column.ColumnScale = DataType.ChangeType<int>(dictDataMetadata["NumericScale"]);
																				// TODO FIX
																				//column.ColumnSqlType = DataType.ChangeType<string>(dictDataMetadata["DataTypeName"]);
																				//column.ColumnIsUserDefinedType = DataType.ChangeType<string>(dictDataMetadata["IsUserDefinedType"]);
																				//column.ColumnHasDefault = DataType.ChangeType<bool>(dictDataMetadata["ColumnHasDefault"]);
																				column.ColumnNamePascalCase = Name.GetPascalCase(column.ColumnName);
																				column.ColumnNameCamelCase = Name.GetCamelCase(column.ColumnName);
																				column.ColumnNameConstantCase = Name.GetConstantCase(column.ColumnName);
																				column.ColumnNameSingularPascalCase = Name.GetPascalCase(Name.GetSingularForm(column.ColumnName));
																				column.ColumnNameSingularCamelCase = Name.GetCamelCase(Name.GetSingularForm(column.ColumnName));
																				column.ColumnNameSingularConstantCase = Name.GetConstantCase(Name.GetSingularForm(column.ColumnName));
																				column.ColumnNamePluralPascalCase = Name.GetPascalCase(Name.GetPluralForm(column.ColumnName));
																				column.ColumnNamePluralCamelCase = Name.GetCamelCase(Name.GetPluralForm(column.ColumnName));
																				column.ColumnNamePluralConstantCase = Name.GetConstantCase(Name.GetPluralForm(column.ColumnName));

																				column.ColumnNameSqlMetalPascalCase = Name.GetSqlMetalPascalCase(column.ColumnName);
																				column.ColumnNameSqlMetalCamelCase = Name.GetSqlMetalCamelCase(column.ColumnName);
																				column.ColumnNameSqlMetalSingularPascalCase = Name.GetSqlMetalPascalCase(Name.GetSingularForm(column.ColumnName));
																				column.ColumnNameSqlMetalSingularCamelCase = Name.GetSqlMetalCamelCase(Name.GetSingularForm(column.ColumnName));
																				column.ColumnNameSqlMetalPluralPascalCase = Name.GetSqlMetalPascalCase(Name.GetPluralForm(column.ColumnName));
																				column.ColumnNameSqlMetalPluralCamelCase = Name.GetSqlMetalCamelCase(Name.GetPluralForm(column.ColumnName));

																				clrType = this.CoreInferClrTypeForSqlType(dataSourceTag, column.ColumnSqlType, column.ColumnPrecision);
																				column.ColumnDbType = AdoNetHelper.InferDbTypeForClrType(clrType);
																				column.ColumnSize = this.CoreCalculateColumnSize(dataSourceTag, column); //recalculate

																				column.ColumnClrType = clrType ?? typeof(object);
																				column.ColumnClrNullableType = Reflexion.MakeNullableType(clrType);
																				column.ColumnClrNonNullableType = Reflexion.MakeNonNullableType(clrType);
																				column.ColumnCSharpNullableLiteral = column.ColumnNullable.ToString().ToLower();
																				column.ColumnCSharpDbType = string.Format("{0}.{1}", typeof(DbType).Name, column.ColumnDbType);
																				column.ColumnCSharpClrType = (object)column.ColumnClrType != null ? FormatCSharpType(column.ColumnClrType) : FormatCSharpType(typeof(object));
																				column.ColumnCSharpClrNullableType = (object)column.ColumnClrNullableType != null ? FormatCSharpType(column.ColumnClrNullableType) : FormatCSharpType(typeof(object));
																				column.ColumnCSharpClrNonNullableType = (object)column.ColumnClrNonNullableType != null ? FormatCSharpType(column.ColumnClrNonNullableType) : FormatCSharpType(typeof(object));

																				procedure.Columns.Add(column);
																			}
																		}
																	}
																}
																catch (Exception ex)
																{
																	Console.Error.WriteLine(Reflexion.GetErrors(ex, 0));
																}
															}
														}
													}
												}
											}
										}
									} // END SCHEMA
								}
							}
						}
					}
				}
			}

			return server;
		}

		#endregion
	}
}