/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

using TextMetal.Framework.Naming;
using TextMetal.Middleware.Datazoid.Extensions;
using TextMetal.Middleware.Solder.Extensions;

namespace TextMetal.Framework.Source.DatabaseSchema
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
				throw new ArgumentNullException(nameof(type));

			var _typeInfo = type.GetTypeInfo();

			if (_typeInfo.IsGenericType)
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

				return String.Format("{0}<{1}>", Regex.Replace(type.Name, "([A-Za-z0-9_]+)(`[0-1]+)", "$1"), String.Join(", ", s.ToArray()));
			}

			return type.Name;
		}

		private static string GetAllAssemblyResourceFileText(Type type, string folder, string name)
		{
			string resourcePath;
			string sqlText;

			if ((object)type == null)
				throw new ArgumentNullException(nameof(type));

			if ((object)name == null)
				throw new ArgumentNullException(nameof(name));

			if (SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.IsWhiteSpace(name))
				throw new ArgumentOutOfRangeException(nameof(name));

			resourcePath = String.Format("{0}.DML.{1}.{2}.sql", type.Namespace, folder, name);

			if (!type.TryGetStringFromAssemblyResource(resourcePath, out sqlText))
				throw new InvalidOperationException(String.Format("Failed to obtain assembly manifest (embedded) resource '{0}'.", resourcePath));

			return sqlText;
		}

		protected abstract int CoreCalculateColumnSize(string dataSourceTag, Column column);

		protected abstract int CoreCalculateParameterSize(string dataSourceTag, Parameter parameter);

		protected abstract IEnumerable<DbParameter> CoreGetColumnParameters(Type connectionType, string dataSourceTag, Server server, Database database, Schema schema, Table table);

		protected abstract IEnumerable<DbParameter> CoreGetColumnParameters(Type connectionType, string dataSourceTag, Server server, Database database, Schema schema, View view);

		protected abstract IEnumerable<DbParameter> CoreGetDatabaseParameters(Type connectionType, string dataSourceTag, Server server);

		protected abstract IEnumerable<DbParameter> CoreGetDdlTriggerParameters(Type connectionType, string dataSourceTag, Server server, Database database);

		protected abstract IEnumerable<DbParameter> CoreGetDmlTriggerParameters(Type connectionType, string dataSourceTag, Server server, Database database, Schema schema, Table table);

		protected abstract bool CoreGetEmitImplicitReturnParameter(string dataSourceTag);

		protected abstract IEnumerable<DbParameter> CoreGetForeignKeyColumnParameters(Type connectionType, string dataSourceTag, Server server, Database database, Schema schema, Table table, ForeignKey foreignKey);

		protected abstract IEnumerable<DbParameter> CoreGetForeignKeyParameters(Type connectionType, string dataSourceTag, Server server, Database database, Schema schema, Table table);

		protected abstract IEnumerable<DbParameter> CoreGetParameterParameters(Type connectionType, string dataSourceTag, Server server, Database database, Schema schema, Procedure procedure);

		protected abstract string CoreGetParameterPrefix(string dataSourceTag);

		protected abstract IEnumerable<DbParameter> CoreGetProcedureParameters(Type connectionType, string dataSourceTag, Server server, Database database, Schema schema);

		protected abstract IEnumerable<DbParameter> CoreGetSchemaParameters(Type connectionType, string dataSourceTag, Server server, Database database);

		protected abstract IEnumerable<DbParameter> CoreGetServerParameters(Type connectionType, string dataSourceTag);

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
			const string PROP_DISABLE_NAME_MANGLING = "DisableNameMangling";

			string connectionAqtn;
			Type connectionType = null;
			string connectionString = null;
			string dataSourceTag;
			string[] serverFilter;
			string[] databaseFilter;
			string[] schemaFilter;
			string[] objectFilter;
			bool disableProcSchDisc, enableDatabaseFilter, disableNameMangling;
			IList<string> values;

			if ((object)sourceFilePath == null)
				throw new ArgumentNullException(nameof(sourceFilePath));

			if ((object)properties == null)
				throw new ArgumentNullException(nameof(properties));

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
				throw new InvalidOperationException(String.Format("Failed to load the connection type '{0}' via Type.GetType(..).", connectionAqtn));

			if (!typeof(DbConnection).IsAssignableFrom(connectionType))
				throw new InvalidOperationException(String.Format("The connection type is not assignable to type '{0}'.", typeof(DbConnection).FullName));

			if (properties.TryGetValue(PROP_TOKEN_CONNECTION_STRING, out values))
			{
				if ((object)values != null && values.Count == 1)
					connectionString = values[0];
			}

			if (SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.IsNullOrWhiteSpace(connectionString))
				connectionString = sourceFilePath;

			if (SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.IsWhiteSpace(connectionString))
				throw new InvalidOperationException(String.Format("The connection string cannot be null or whitespace."));

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
					serverFilter = Enumerable.ToArray(values);
			}

			databaseFilter = null;
			if (properties.TryGetValue(PROP_TOKEN_DATABASE_FILTER, out values))
			{
				if ((object)values != null && values.Count > 0)
					databaseFilter = Enumerable.ToArray(values);
			}

			schemaFilter = null;
			if (properties.TryGetValue(PROP_TOKEN_SCHEMA_FILTER, out values))
			{
				if ((object)values != null && values.Count > 0)
					schemaFilter = Enumerable.ToArray(values);
			}

			objectFilter = null;
			if (properties.TryGetValue(PROP_TOKEN_OBJECT_FILTER, out values))
			{
				if ((object)values != null && values.Count > 0)
					objectFilter = Enumerable.ToArray(values);
			}

			disableProcSchDisc = false;
			if (properties.TryGetValue(PROP_DISABLE_PROCEDURE_SCHEMA_DISCOVERY, out values))
			{
				if ((object)values != null && values.Count > 0)
					SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.TryParse<bool>(values[0], out disableProcSchDisc);
			}

			enableDatabaseFilter = false;
			if (properties.TryGetValue(PROP_ENABLE_DATABASE_FILTER, out values))
			{
				if ((object)values != null && values.Count > 0)
					SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.TryParse<bool>(values[0], out enableDatabaseFilter);
			}

			disableNameMangling = false;
			if (properties.TryGetValue(PROP_DISABLE_NAME_MANGLING, out values))
			{
				if ((object)values != null && values.Count > 0)
					SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.TryParse<bool>(values[0], out disableNameMangling);
			}

			return this.GetSchemaModel(connectionString, connectionType, dataSourceTag, serverFilter, databaseFilter, schemaFilter, objectFilter, disableProcSchDisc, enableDatabaseFilter, disableNameMangling);
		}

		protected abstract IEnumerable<DbParameter> CoreGetTableParameters(Type connectionType, string dataSourceTag, Server server, Database database, Schema schema);

		protected abstract IEnumerable<DbParameter> CoreGetUniqueKeyColumnParameters(Type connectionType, string dataSourceTag, Server server, Database database, Schema schema, Table table, UniqueKey uniqueKey);

		protected abstract IEnumerable<DbParameter> CoreGetUniqueKeyParameters(Type connectionType, string dataSourceTag, Server server, Database database, Schema schema, Table table);

		protected abstract Type CoreInferClrTypeForSqlType(string dataSourceTag, string sqlType, int sqlPrecision);

		private object GetSchemaModel(string connectionString, Type connectionType, string dataSourceTag,
			string[] serverFilter, string[] databaseFilter,
			string[] schemaFilter, string[] objectFilter,
			bool disableProcSchDisc, bool enableDatabaseFilter, bool disableNameMangling)
		{
			Server server;
			Type clrType;
			StandardCanonicalNaming effectiveStandardCanonicalNaming;

			if ((object)connectionString == null)
				throw new ArgumentNullException(nameof(connectionString));

			if ((object)connectionType == null)
				throw new ArgumentNullException(nameof(connectionType));

			if ((object)dataSourceTag == null)
				throw new ArgumentNullException(nameof(dataSourceTag));

			effectiveStandardCanonicalNaming = disableNameMangling ? StandardCanonicalNaming.InstanceDisableNameMangling : StandardCanonicalNaming.Instance;

			{
				server = new Server();
				server.ConnectionString = connectionString;
				server.ConnectionType = connectionType.FullName;

				var dictEnumServer = DatazoidLegacyInstanceAccessor.AdoNetBufferingLegacyInstance.ExecuteRecords(false, connectionType, connectionString, false, IsolationLevel.Unspecified, CommandType.Text, GetAllAssemblyResourceFileText(this.GetType(), dataSourceTag, "Server"), this.CoreGetServerParameters(connectionType, dataSourceTag));
				{
					var dictDataServer = (IDictionary<string, object>)null;

					if ((object)dictEnumServer != null &&
						(object)(dictDataServer = Enumerable.ToList(dictEnumServer).SingleOrDefault()) != null)
					{
						server.ServerName = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<string>(dictDataServer[SchemaInfoConstants.SERVER_NAME]);
						server.MachineName = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<string>(dictDataServer[SchemaInfoConstants.MACHINE_NAME]);
						server.InstanceName = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<string>(dictDataServer[SchemaInfoConstants.INSTANCE_NAME]);
						server.ServerVersion = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<string>(dictDataServer[SchemaInfoConstants.SERVER_VERSION]);
						server.ServerLevel = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<string>(dictDataServer[SchemaInfoConstants.SERVER_LEVEL]);
						server.ServerEdition = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<string>(dictDataServer[SchemaInfoConstants.SERVER_EDITION]);
						server.DefaultDatabaseName = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<string>(dictDataServer[SchemaInfoConstants.DEFAULT_DATABASE_NAME]);

						// filter unwanted servers
						if ((object)serverFilter != null)
						{
							if (!Enumerable.Any(serverFilter, f => Regex.IsMatch(server.ServerName, f)))
								return null;
						}

						var dictEnumDatabase = DatazoidLegacyInstanceAccessor.AdoNetBufferingLegacyInstance.ExecuteRecords(false, connectionType, connectionString, false, IsolationLevel.Unspecified, CommandType.Text, GetAllAssemblyResourceFileText(this.GetType(), dataSourceTag, "Databases"), this.CoreGetDatabaseParameters(connectionType, dataSourceTag, server));
						{
							if ((object)dictEnumDatabase != null)
							{
								foreach (var dictDataDatabase in Enumerable.ToList(dictEnumDatabase))
								{
									Database database;

									database = new Database();
									database.DatabaseId = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<int>(dictDataDatabase["DatabaseId"]);
									database.DatabaseName = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<string>(dictDataDatabase["DatabaseName"]);
									database.CreationTimestamp = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<DateTime>(dictDataDatabase["CreationTimestamp"]);
									database.DatabaseNamePascalCase = effectiveStandardCanonicalNaming.GetPascalCase(database.DatabaseName);
									database.DatabaseNameCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(database.DatabaseName);
									database.DatabaseNameConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(database.DatabaseName);
									database.DatabaseNameSingularPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(database.DatabaseName));
									database.DatabaseNameSingularCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(database.DatabaseName));
									database.DatabaseNameSingularConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetSingularForm(database.DatabaseName));
									database.DatabaseNamePluralPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(database.DatabaseName));
									database.DatabaseNamePluralCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(database.DatabaseName));
									database.DatabaseNamePluralConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetPluralForm(database.DatabaseName));

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
											if (!Enumerable.Any(databaseFilter, f => Regex.IsMatch(database.DatabaseName, f)))
												continue;
										}
									}

									server.Databases.Add(database);

									DatazoidLegacyInstanceAccessor.AdoNetBufferingLegacyInstance.ExecuteRecords(false, connectionType, connectionString, false, IsolationLevel.Unspecified, CommandType.Text, String.Format(GetAllAssemblyResourceFileText(this.GetType(), dataSourceTag, "UseDatabase"), server.ServerName, database.DatabaseName), null);

									var dictEnumDdlTrigger = DatazoidLegacyInstanceAccessor.AdoNetBufferingLegacyInstance.ExecuteRecords(false, connectionType, connectionString, false, IsolationLevel.Unspecified, CommandType.Text, GetAllAssemblyResourceFileText(this.GetType(), dataSourceTag, "DdlTriggers"), this.CoreGetDdlTriggerParameters(connectionType, dataSourceTag, server, database));
									{
										if ((object)dictEnumDdlTrigger != null)
										{
											foreach (var dictDataTrigger in Enumerable.ToList(dictEnumDdlTrigger))
											{
												Trigger trigger;

												trigger = new Trigger();

												trigger.TriggerId = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<int>(dictDataTrigger["TriggerId"]);
												trigger.TriggerName = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<string>(dictDataTrigger["TriggerName"]);
												trigger.IsClrTrigger = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<bool>(dictDataTrigger["IsClrTrigger"]);
												trigger.IsTriggerDisabled = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<bool>(dictDataTrigger["IsTriggerDisabled"]);
												trigger.IsTriggerNotForReplication = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<bool>(dictDataTrigger["IsTriggerNotForReplication"]);
												trigger.IsInsteadOfTrigger = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<bool>(dictDataTrigger["IsInsteadOfTrigger"]);
												trigger.TriggerNamePascalCase = effectiveStandardCanonicalNaming.GetPascalCase(trigger.TriggerName);
												trigger.TriggerNameCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(trigger.TriggerName);
												trigger.TriggerNameConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(trigger.TriggerName);
												trigger.TriggerNameSingularPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(trigger.TriggerName));
												trigger.TriggerNameSingularCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(trigger.TriggerName));
												trigger.TriggerNameSingularConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetSingularForm(trigger.TriggerName));
												trigger.TriggerNamePluralPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(trigger.TriggerName));
												trigger.TriggerNamePluralCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(trigger.TriggerName));
												trigger.TriggerNamePluralConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetPluralForm(trigger.TriggerName));

												database.Triggers.Add(trigger);
											}
										}
									}

									var dictEnumSchema = DatazoidLegacyInstanceAccessor.AdoNetBufferingLegacyInstance.ExecuteRecords(false, connectionType, connectionString, false, IsolationLevel.Unspecified, CommandType.Text, GetAllAssemblyResourceFileText(this.GetType(), dataSourceTag, "Schemas"), this.CoreGetSchemaParameters(connectionType, dataSourceTag, server, database));
									{
										if ((object)dictEnumSchema != null)
										{
											foreach (var dictDataSchema in Enumerable.ToList(dictEnumSchema))
											{
												Schema schema;

												schema = new Schema();
												schema.SchemaId = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<int>(dictDataSchema["SchemaId"]);
												schema.OwnerId = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<int>(dictDataSchema["OwnerId"]);
												schema.SchemaName = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<string>(dictDataSchema["SchemaName"]);
												schema.SchemaNamePascalCase = effectiveStandardCanonicalNaming.GetPascalCase(schema.SchemaName);
												schema.SchemaNameCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(schema.SchemaName);
												schema.SchemaNameConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(schema.SchemaName);
												schema.SchemaNameSingularPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(schema.SchemaName));
												schema.SchemaNameSingularCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(schema.SchemaName));
												schema.SchemaNameSingularConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetSingularForm(schema.SchemaName));
												schema.SchemaNamePluralPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(schema.SchemaName));
												schema.SchemaNamePluralCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(schema.SchemaName));
												schema.SchemaNamePluralConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetPluralForm(schema.SchemaName));

												// filter unwanted schemas
												if ((object)schemaFilter != null)
												{
													if (!Enumerable.Any(schemaFilter, f => Regex.IsMatch(schema.SchemaName, f)))
														continue;
												}

												database.Schemas.Add(schema);

												var dictEnumTable = DatazoidLegacyInstanceAccessor.AdoNetBufferingLegacyInstance.ExecuteRecords(false, connectionType, connectionString, false, IsolationLevel.Unspecified, CommandType.Text, GetAllAssemblyResourceFileText(this.GetType(), dataSourceTag, "Tables"), this.CoreGetTableParameters(connectionType, dataSourceTag, server, database, schema));
												{
													foreach (var dictDataTable in Enumerable.ToList(dictEnumTable))
													{
														Table table;

														table = new Table();
														table.TableId = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<int>(dictDataTable["TableId"]);
														table.TableName = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<string>(dictDataTable["TableName"]);
														table.CreationTimestamp = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<DateTime>(dictDataTable["CreationTimestamp"]);
														table.ModificationTimestamp = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<DateTime>(dictDataTable["ModificationTimestamp"]);
														table.IsImplementationDetail = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<bool>(dictDataTable["IsImplementationDetail"]);
														table.TableNamePascalCase = effectiveStandardCanonicalNaming.GetPascalCase(table.TableName);
														table.TableNameCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(table.TableName);
														table.TableNameConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(table.TableName);
														table.TableNameSingularPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(table.TableName));
														table.TableNameSingularCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(table.TableName));
														table.TableNameSingularConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetSingularForm(table.TableName));
														table.TableNamePluralPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(table.TableName));
														table.TableNamePluralCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(table.TableName));
														table.TableNamePluralConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetPluralForm(table.TableName));

														var pkId = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<int?>(dictDataTable["PrimaryKeyId"]);
														if ((object)pkId != null)
														{
															table.PrimaryKey = new PrimaryKey();
															table.PrimaryKey.PrimaryKeyId = (int)pkId;

															table.PrimaryKey.PrimaryKeyIsSystemNamed = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<bool>(dictDataTable["PrimaryKeyIsSystemNamed"]);
															table.PrimaryKey.PrimaryKeyName = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<string>(dictDataTable["PrimaryKeyName"]);

															table.PrimaryKey.PrimaryKeyNamePascalCase = effectiveStandardCanonicalNaming.GetPascalCase(table.PrimaryKey.PrimaryKeyName);
															table.PrimaryKey.PrimaryKeyNameCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(table.PrimaryKey.PrimaryKeyName);
															table.PrimaryKey.PrimaryKeyNameConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(table.PrimaryKey.PrimaryKeyName);
															table.PrimaryKey.PrimaryKeyNameSingularPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(table.PrimaryKey.PrimaryKeyName));
															table.PrimaryKey.PrimaryKeyNameSingularCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(table.PrimaryKey.PrimaryKeyName));
															table.PrimaryKey.PrimaryKeyNameSingularConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetSingularForm(table.PrimaryKey.PrimaryKeyName));
															table.PrimaryKey.PrimaryKeyNamePluralPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(table.PrimaryKey.PrimaryKeyName));
															table.PrimaryKey.PrimaryKeyNamePluralCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(table.PrimaryKey.PrimaryKeyName));
															table.PrimaryKey.PrimaryKeyNamePluralConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetPluralForm(table.PrimaryKey.PrimaryKeyName));
														}

														// filter unwanted tables (objects)
														if ((object)objectFilter != null)
														{
															if (!Enumerable.Any(objectFilter, f => Regex.IsMatch(table.TableName, f)))
																continue;
														}

														schema._Tables.Add(table);

														var dictEnumColumn = DatazoidLegacyInstanceAccessor.AdoNetBufferingLegacyInstance.ExecuteRecords(false, connectionType, connectionString, false, IsolationLevel.Unspecified, CommandType.Text, GetAllAssemblyResourceFileText(this.GetType(), dataSourceTag, "TableColumns"), this.CoreGetColumnParameters(connectionType, dataSourceTag, server, database, schema, table));
														{
															if ((object)dictEnumColumn != null)
															{
																foreach (var dictDataColumn in Column.FixupDuplicateColumns(Enumerable.ToList(dictEnumColumn)))
																{
																	TableColumn column;

																	column = new TableColumn();

																	column.ColumnOrdinal = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<int>(dictDataColumn["ColumnOrdinal"]);
																	column.ColumnName = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<string>(dictDataColumn["ColumnName"]);
																	column.ColumnIsAnonymous = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<bool>(dictDataColumn["ColumnIsAnonymous"]);
																	column.ColumnCSharpIsAnonymousLiteral = column.ColumnIsAnonymous.ToString().ToLower();
																	column.ColumnNullable = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<bool>(dictDataColumn["ColumnNullable"]);
																	column.ColumnSize = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<int>(dictDataColumn["ColumnSize"]);
																	column.ColumnPrecision = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<int>(dictDataColumn["ColumnPrecision"]);
																	column.ColumnScale = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<int>(dictDataColumn["ColumnScale"]);
																	column.ColumnSqlType = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<string>(dictDataColumn["ColumnSqlType"]);
																	column.ColumnIsUserDefinedType = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<bool>(dictDataColumn["ColumnIsUserDefinedType"]);
																	column.ColumnIsIdentity = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<bool>(dictDataColumn["ColumnIsIdentity"]);
																	column.ColumnIsComputed = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<bool>(dictDataColumn["ColumnIsComputed"]);
																	column.ColumnHasDefault = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<bool>(dictDataColumn["ColumnHasDefault"]);
																	column.ColumnHasCheck = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<bool>(dictDataColumn["ColumnHasCheck"]);
																	column.ColumnIsPrimaryKey = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<bool>(dictDataColumn["ColumnIsPrimaryKey"]);
																	column.ColumnPrimaryKeyOrdinal = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<int>(dictDataColumn["ColumnPrimaryKeyOrdinal"]);
																	column.ColumnNamePascalCase = effectiveStandardCanonicalNaming.GetPascalCase(column.ColumnName);
																	column.ColumnNameCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(column.ColumnName);
																	column.ColumnNameConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(column.ColumnName);
																	column.ColumnNameSingularPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(column.ColumnName));
																	column.ColumnNameSingularCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(column.ColumnName));
																	column.ColumnNameSingularConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetSingularForm(column.ColumnName));
																	column.ColumnNamePluralPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(column.ColumnName));
																	column.ColumnNamePluralCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(column.ColumnName));
																	column.ColumnNamePluralConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetPluralForm(column.ColumnName));

																	clrType = this.CoreInferClrTypeForSqlType(dataSourceTag, column.ColumnSqlType, column.ColumnPrecision);
																	column.ColumnDbType = DatazoidLegacyInstanceAccessor.AdoNetBufferingLegacyInstance.InferDbTypeForClrType(clrType);
																	column.ColumnSize = this.CoreCalculateColumnSize(dataSourceTag, column); //recalculate

																	column.ColumnClrType = clrType ?? typeof(object);
																	column.ColumnClrNullableType = SolderLegacyInstanceAccessor.ReflectionFascadeLegacyInstance.MakeNullableType(clrType);
																	column.ColumnClrNonNullableType = SolderLegacyInstanceAccessor.ReflectionFascadeLegacyInstance.MakeNonNullableType(clrType);
																	column.ColumnCSharpNullableLiteral = column.ColumnNullable.ToString().ToLower();
																	column.ColumnCSharpIsPrimaryKeyLiteral = column.ColumnIsPrimaryKey.ToString().ToLower();
																	column.ColumnCSharpIsComputedLiteral = column.ColumnIsComputed.ToString().ToLower();
																	column.ColumnCSharpIsIdentityLiteral = column.ColumnIsIdentity.ToString().ToLower();
																	column.ColumnCSharpDbType = String.Format("{0}.{1}", typeof(DbType).Name, column.ColumnDbType);
																	column.ColumnCSharpClrType = (object)column.ColumnClrType != null ? FormatCSharpType(column.ColumnClrType) : FormatCSharpType(typeof(object));
																	column.ColumnCSharpClrNullableType = (object)column.ColumnClrNullableType != null ? FormatCSharpType(column.ColumnClrNullableType) : FormatCSharpType(typeof(object));
																	column.ColumnCSharpClrNonNullableType = (object)column.ColumnClrNonNullableType != null ? FormatCSharpType(column.ColumnClrNonNullableType) : FormatCSharpType(typeof(object));

																	table.Columns.Add(column);

																	if ((object)table.PrimaryKey != null &&
																		(object)table.PrimaryKey.PrimaryKeyId != null &&
																		column.ColumnIsPrimaryKey)
																	{
																		table.PrimaryKey.PrimaryKeyColumns.Add(new PrimaryKeyColumn()
																												{
																													ColumnOrdinal = column.ColumnOrdinal,
																													ColumnName = column.ColumnName,
																													PrimaryKeyColumnOrdinal = column.ColumnPrimaryKeyOrdinal
																												});
																	}
																}
															}
														}

														if (table.Columns.Count(c => c.ColumnIsPrimaryKey) < 1)
														{
															table.HasNoDefinedPrimaryKeyColumns = true;
															table.Columns.ForEach(c => c.ColumnIsPrimaryKey = true);
														}

														var dictEnumDmlTrigger = DatazoidLegacyInstanceAccessor.AdoNetBufferingLegacyInstance.ExecuteRecords(false, connectionType, connectionString, false, IsolationLevel.Unspecified, CommandType.Text, GetAllAssemblyResourceFileText(this.GetType(), dataSourceTag, "DmlTriggers"), this.CoreGetDmlTriggerParameters(connectionType, dataSourceTag, server, database, schema, table));
														{
															if ((object)dictEnumDmlTrigger != null)
															{
																foreach (var dictDataTrigger in Enumerable.ToList(dictEnumDmlTrigger))
																{
																	Trigger trigger;

																	trigger = new Trigger();

																	trigger.TriggerId = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<int>(dictDataTrigger["TriggerId"]);
																	trigger.TriggerName = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<string>(dictDataTrigger["TriggerName"]);
																	trigger.IsClrTrigger = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<bool>(dictDataTrigger["IsClrTrigger"]);
																	trigger.IsTriggerDisabled = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<bool>(dictDataTrigger["IsTriggerDisabled"]);
																	trigger.IsTriggerNotForReplication = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<bool>(dictDataTrigger["IsTriggerNotForReplication"]);
																	trigger.IsInsteadOfTrigger = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<bool>(dictDataTrigger["IsInsteadOfTrigger"]);
																	trigger.TriggerNamePascalCase = effectiveStandardCanonicalNaming.GetPascalCase(trigger.TriggerName);
																	trigger.TriggerNameCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(trigger.TriggerName);
																	trigger.TriggerNameConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(trigger.TriggerName);
																	trigger.TriggerNameSingularPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(trigger.TriggerName));
																	trigger.TriggerNameSingularCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(trigger.TriggerName));
																	trigger.TriggerNameSingularConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetSingularForm(trigger.TriggerName));
																	trigger.TriggerNamePluralPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(trigger.TriggerName));
																	trigger.TriggerNamePluralCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(trigger.TriggerName));
																	trigger.TriggerNamePluralConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetPluralForm(trigger.TriggerName));

																	table.Triggers.Add(trigger);
																}
															}
														}

														var dictEnumForeignKey = DatazoidLegacyInstanceAccessor.AdoNetBufferingLegacyInstance.ExecuteRecords(false, connectionType, connectionString, false, IsolationLevel.Unspecified, CommandType.Text, GetAllAssemblyResourceFileText(this.GetType(), dataSourceTag, "ForeignKeys"), this.CoreGetForeignKeyParameters(connectionType, dataSourceTag, server, database, schema, table));
														{
															if ((object)dictEnumForeignKey != null)
															{
																foreach (var dictDataForeignKey in Enumerable.ToList(dictEnumForeignKey))
																{
																	ForeignKey foreignKey;

																	foreignKey = new ForeignKey();

																	foreignKey.ForeignKeyName = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<string>(dictDataForeignKey["ForeignKeyName"]);
																	foreignKey.ForeignKeyIsDisabled = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<bool>(dictDataForeignKey["ForeignKeyIsDisabled"]);
																	foreignKey.ForeignKeyIsSystemNamed = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<bool>(dictDataForeignKey["ForeignKeyIsSystemNamed"]);
																	foreignKey.ForeignKeyIsForReplication = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<bool>(dictDataForeignKey["ForeignKeyIsForReplication"]);
																	foreignKey.ForeignKeyOnDeleteRefIntAction = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<byte>(dictDataForeignKey["ForeignKeyOnDeleteRefIntAction"]);
																	foreignKey.ForeignKeyOnDeleteRefIntActionSqlName = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<string>(dictDataForeignKey["ForeignKeyOnDeleteRefIntActionSqlName"]);
																	foreignKey.ForeignKeyOnUpdateRefIntAction = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<byte>(dictDataForeignKey["ForeignKeyOnUpdateRefIntAction"]);
																	foreignKey.ForeignKeyOnUpdateRefIntActionSqlName = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<string>(dictDataForeignKey["ForeignKeyOnUpdateRefIntActionSqlName"]);
																	foreignKey.ForeignKeyNamePascalCase = effectiveStandardCanonicalNaming.GetPascalCase(foreignKey.ForeignKeyName);
																	foreignKey.ForeignKeyNameCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(foreignKey.ForeignKeyName);
																	foreignKey.ForeignKeyNameConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(foreignKey.ForeignKeyName);
																	foreignKey.ForeignKeyNameSingularPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(foreignKey.ForeignKeyName));
																	foreignKey.ForeignKeyNameSingularCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(foreignKey.ForeignKeyName));
																	foreignKey.ForeignKeyNameSingularConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetSingularForm(foreignKey.ForeignKeyName));
																	foreignKey.ForeignKeyNamePluralPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(foreignKey.ForeignKeyName));
																	foreignKey.ForeignKeyNamePluralCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(foreignKey.ForeignKeyName));
																	foreignKey.ForeignKeyNamePluralConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetPluralForm(foreignKey.ForeignKeyName));

																	foreignKey.TargetSchemaName = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<string>(dictDataForeignKey["TargetSchemaName"]);
																	foreignKey.TargetSchemaNamePascalCase = effectiveStandardCanonicalNaming.GetPascalCase(foreignKey.TargetSchemaName);
																	foreignKey.TargetSchemaNameCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(foreignKey.TargetSchemaName);
																	foreignKey.TargetSchemaNameConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(foreignKey.TargetSchemaName);
																	foreignKey.TargetSchemaNameSingularPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(foreignKey.TargetSchemaName));
																	foreignKey.TargetSchemaNameSingularCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(foreignKey.TargetSchemaName));
																	foreignKey.TargetSchemaNameSingularConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetSingularForm(foreignKey.TargetSchemaName));
																	foreignKey.TargetSchemaNamePluralPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(foreignKey.TargetSchemaName));
																	foreignKey.TargetSchemaNamePluralCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(foreignKey.TargetSchemaName));
																	foreignKey.TargetSchemaNamePluralConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetPluralForm(foreignKey.TargetSchemaName));

																	foreignKey.TargetTableName = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<string>(dictDataForeignKey["TargetTableName"]);
																	foreignKey.TargetTableNamePascalCase = effectiveStandardCanonicalNaming.GetPascalCase(foreignKey.TargetTableName);
																	foreignKey.TargetTableNameCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(foreignKey.TargetTableName);
																	foreignKey.TargetTableNameConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(foreignKey.TargetTableName);
																	foreignKey.TargetTableNameSingularPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(foreignKey.TargetTableName));
																	foreignKey.TargetTableNameSingularCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(foreignKey.TargetTableName));
																	foreignKey.TargetTableNameSingularConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetSingularForm(foreignKey.TargetTableName));
																	foreignKey.TargetTableNamePluralPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(foreignKey.TargetTableName));
																	foreignKey.TargetTableNamePluralCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(foreignKey.TargetTableName));
																	foreignKey.TargetTableNamePluralConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetPluralForm(foreignKey.TargetTableName));

																	table.ForeignKeys.Add(foreignKey);

																	var dictEnumForeignKeyColumn = DatazoidLegacyInstanceAccessor.AdoNetBufferingLegacyInstance.ExecuteRecords(false, connectionType, connectionString, false, IsolationLevel.Unspecified, CommandType.Text, GetAllAssemblyResourceFileText(this.GetType(), dataSourceTag, "ForeignKeyColumns"), this.CoreGetForeignKeyColumnParameters(connectionType, dataSourceTag, server, database, schema, table, foreignKey));
																	{
																		if ((object)dictEnumForeignKeyColumn != null)
																		{
																			foreach (var dictDataForeignKeyColumn in Enumerable.ToList(dictEnumForeignKeyColumn))
																			{
																				ForeignKeyColumn foreignKeyColumn;

																				foreignKeyColumn = new ForeignKeyColumn();

																				foreignKeyColumn.ForeignKeyColumnOrdinal = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<int>(dictDataForeignKeyColumn["ForeignKeyColumnOrdinal"]);
																				foreignKeyColumn.ColumnOrdinal = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<int>(dictDataForeignKeyColumn["ColumnOrdinal"]);
																				foreignKeyColumn.ColumnName = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<string>(dictDataForeignKeyColumn["ColumnName"]);
																				foreignKeyColumn.TargetColumnOrdinal = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<int>(dictDataForeignKeyColumn["TargetColumnOrdinal"]);
																				foreignKeyColumn.TargetColumnName = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<string>(dictDataForeignKeyColumn["TargetColumnName"]);

																				foreignKey.ForeignKeyColumns.Add(foreignKeyColumn);
																			}
																		}
																	}
																}
															}
														}

														var dictEnumUniqueKey = DatazoidLegacyInstanceAccessor.AdoNetBufferingLegacyInstance.ExecuteRecords(false, connectionType, connectionString, false, IsolationLevel.Unspecified, CommandType.Text, GetAllAssemblyResourceFileText(this.GetType(), dataSourceTag, "UniqueKeys"), this.CoreGetUniqueKeyParameters(connectionType, dataSourceTag, server, database, schema, table));
														{
															if ((object)dictEnumUniqueKey != null)
															{
																foreach (var dictDataUniqueKey in Enumerable.ToList(dictEnumUniqueKey))
																{
																	UniqueKey uniqueKey;

																	uniqueKey = new UniqueKey();

																	uniqueKey.UniqueKeyId = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<int>(dictDataUniqueKey["UniqueKeyId"]);
																	uniqueKey.UniqueKeyName = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<string>(dictDataUniqueKey["UniqueKeyName"]);
																	uniqueKey.UniqueKeyIsSystemNamed = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<bool>(dictDataUniqueKey["UniqueKeyIsSystemNamed"]);
																	uniqueKey.UniqueKeyNamePascalCase = effectiveStandardCanonicalNaming.GetPascalCase(uniqueKey.UniqueKeyName);
																	uniqueKey.UniqueKeyNameCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(uniqueKey.UniqueKeyName);
																	uniqueKey.UniqueKeyNameConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(uniqueKey.UniqueKeyName);
																	uniqueKey.UniqueKeyNameSingularPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(uniqueKey.UniqueKeyName));
																	uniqueKey.UniqueKeyNameSingularCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(uniqueKey.UniqueKeyName));
																	uniqueKey.UniqueKeyNameSingularConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetSingularForm(uniqueKey.UniqueKeyName));
																	uniqueKey.UniqueKeyNamePluralPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(uniqueKey.UniqueKeyName));
																	uniqueKey.UniqueKeyNamePluralCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(uniqueKey.UniqueKeyName));
																	uniqueKey.UniqueKeyNamePluralConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetPluralForm(uniqueKey.UniqueKeyName));

																	table.UniqueKeys.Add(uniqueKey);

																	var dictEnumUniqueKeyColumn = DatazoidLegacyInstanceAccessor.AdoNetBufferingLegacyInstance.ExecuteRecords(false, connectionType, connectionString, false, IsolationLevel.Unspecified, CommandType.Text, GetAllAssemblyResourceFileText(this.GetType(), dataSourceTag, "UniqueKeyColumns"), this.CoreGetUniqueKeyColumnParameters(connectionType, dataSourceTag, server, database, schema, table, uniqueKey));
																	{
																		if ((object)dictEnumUniqueKeyColumn != null)
																		{
																			foreach (var dictDataUniqueKeyColumn in Enumerable.ToList(dictEnumUniqueKeyColumn))
																			{
																				UniqueKeyColumn uniqueKeyColumn;

																				uniqueKeyColumn = new UniqueKeyColumn();

																				uniqueKeyColumn.UniqueKeyColumnOrdinal = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<int>(dictDataUniqueKeyColumn["UniqueKeyColumnOrdinal"]);
																				uniqueKeyColumn.ColumnOrdinal = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<int>(dictDataUniqueKeyColumn["ColumnOrdinal"]);
																				uniqueKeyColumn.ColumnName = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<string>(dictDataUniqueKeyColumn["ColumnName"]);
																				uniqueKeyColumn.UniqueKeyColumnDescendingSort = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<bool>(dictDataUniqueKeyColumn["UniqueKeyColumnDescendingSort"]);

																				uniqueKey.UniqueKeyColumns.Add(uniqueKeyColumn);
																			}
																		}
																	}
																}
															}
														}
													}
												}

												var dictEnumView = DatazoidLegacyInstanceAccessor.AdoNetBufferingLegacyInstance.ExecuteRecords(false, connectionType, connectionString, false, IsolationLevel.Unspecified, CommandType.Text, GetAllAssemblyResourceFileText(this.GetType(), dataSourceTag, "Views"), this.CoreGetTableParameters(connectionType, dataSourceTag, server, database, schema));
												{
													foreach (var dictDataView in Enumerable.ToList(dictEnumView))
													{
														View view;

														view = new View();
														view.ViewId = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<int>(dictDataView["ViewId"]);
														view.ViewName = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<string>(dictDataView["ViewName"]);
														view.CreationTimestamp = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<DateTime>(dictDataView["CreationTimestamp"]);
														view.ModificationTimestamp = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<DateTime>(dictDataView["ModificationTimestamp"]);
														view.IsImplementationDetail = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<bool>(dictDataView["IsImplementationDetail"]);
														view.ViewNamePascalCase = effectiveStandardCanonicalNaming.GetPascalCase(view.ViewName);
														view.ViewNameCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(view.ViewName);
														view.ViewNameConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(view.ViewName);
														view.ViewNameSingularPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(view.ViewName));
														view.ViewNameSingularCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(view.ViewName));
														view.ViewNameSingularConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetSingularForm(view.ViewName));
														view.ViewNamePluralPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(view.ViewName));
														view.ViewNamePluralCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(view.ViewName));
														view.ViewNamePluralConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetPluralForm(view.ViewName));

														// filter unwanted views (objects)
														if ((object)objectFilter != null)
														{
															if (!Enumerable.Any(objectFilter, f => Regex.IsMatch(view.ViewName, f)))
																continue;
														}

														schema.Views.Add(view);

														var dictEnumColumn = DatazoidLegacyInstanceAccessor.AdoNetBufferingLegacyInstance.ExecuteRecords(false, connectionType, connectionString, false, IsolationLevel.Unspecified, CommandType.Text, GetAllAssemblyResourceFileText(this.GetType(), dataSourceTag, "ViewColumns"), this.CoreGetColumnParameters(connectionType, dataSourceTag, server, database, schema, view));
														{
															if ((object)dictEnumColumn != null)
															{
																foreach (var dictDataColumn in Column.FixupDuplicateColumns(Enumerable.ToList(dictEnumColumn)))
																{
																	ViewColumn column;

																	column = new ViewColumn();

																	column.ColumnOrdinal = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<int>(dictDataColumn["ColumnOrdinal"]);
																	column.ColumnName = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<string>(dictDataColumn["ColumnName"]);
																	column.ColumnIsAnonymous = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<bool>(dictDataColumn["ColumnIsAnonymous"]);
																	column.ColumnCSharpIsAnonymousLiteral = column.ColumnIsAnonymous.ToString().ToLower();
																	column.ColumnNullable = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<bool>(dictDataColumn["ColumnNullable"]);
																	column.ColumnSize = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<int>(dictDataColumn["ColumnSize"]);
																	column.ColumnPrecision = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<int>(dictDataColumn["ColumnPrecision"]);
																	column.ColumnScale = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<int>(dictDataColumn["ColumnScale"]);
																	column.ColumnSqlType = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<string>(dictDataColumn["ColumnSqlType"]);
																	column.ColumnIsUserDefinedType = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<bool>(dictDataColumn["ColumnIsUserDefinedType"]);
																	column.ColumnNamePascalCase = effectiveStandardCanonicalNaming.GetPascalCase(column.ColumnName);
																	column.ColumnNameCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(column.ColumnName);
																	column.ColumnNameConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(column.ColumnName);
																	column.ColumnNameSingularPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(column.ColumnName));
																	column.ColumnNameSingularCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(column.ColumnName));
																	column.ColumnNameSingularConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetSingularForm(column.ColumnName));
																	column.ColumnNamePluralPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(column.ColumnName));
																	column.ColumnNamePluralCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(column.ColumnName));
																	column.ColumnNamePluralConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetPluralForm(column.ColumnName));

																	clrType = this.CoreInferClrTypeForSqlType(dataSourceTag, column.ColumnSqlType, column.ColumnPrecision);
																	column.ColumnDbType = DatazoidLegacyInstanceAccessor.AdoNetBufferingLegacyInstance.InferDbTypeForClrType(clrType);
																	column.ColumnSize = this.CoreCalculateColumnSize(dataSourceTag, column); //recalculate

																	column.ColumnClrType = clrType ?? typeof(object);
																	column.ColumnClrNullableType = SolderLegacyInstanceAccessor.ReflectionFascadeLegacyInstance.MakeNullableType(clrType);
																	column.ColumnClrNonNullableType = SolderLegacyInstanceAccessor.ReflectionFascadeLegacyInstance.MakeNonNullableType(clrType);
																	column.ColumnCSharpNullableLiteral = column.ColumnNullable.ToString().ToLower();
																	column.ColumnCSharpDbType = String.Format("{0}.{1}", typeof(DbType).Name, column.ColumnDbType);
																	column.ColumnCSharpClrType = (object)column.ColumnClrType != null ? FormatCSharpType(column.ColumnClrType) : FormatCSharpType(typeof(object));
																	column.ColumnCSharpClrNullableType = (object)column.ColumnClrNullableType != null ? FormatCSharpType(column.ColumnClrNullableType) : FormatCSharpType(typeof(object));
																	column.ColumnCSharpClrNonNullableType = (object)column.ColumnClrNonNullableType != null ? FormatCSharpType(column.ColumnClrNonNullableType) : FormatCSharpType(typeof(object));

																	view.Columns.Add(column);
																}
															}
														}
													}
												}

												var dictEnumProcedure = DatazoidLegacyInstanceAccessor.AdoNetBufferingLegacyInstance.ExecuteRecords(false, connectionType, connectionString, false, IsolationLevel.Unspecified, CommandType.Text, GetAllAssemblyResourceFileText(this.GetType(), dataSourceTag, "Procedures"), this.CoreGetProcedureParameters(connectionType, dataSourceTag, server, database, schema));
												{
													if ((object)dictEnumProcedure != null)
													{
														foreach (var dictDataProcedure in Enumerable.ToList(dictEnumProcedure))
														{
															Procedure procedure;

															procedure = new Procedure();
															procedure.ProcedureName = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<string>(dictDataProcedure["ProcedureName"]);
															procedure.ProcedureNamePascalCase = effectiveStandardCanonicalNaming.GetPascalCase(procedure.ProcedureName);
															procedure.ProcedureNameCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(procedure.ProcedureName);
															procedure.ProcedureNameConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(procedure.ProcedureName);
															procedure.ProcedureNameSingularPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(procedure.ProcedureName));
															procedure.ProcedureNameSingularCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(procedure.ProcedureName));
															procedure.ProcedureNameSingularConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetSingularForm(procedure.ProcedureName));
															procedure.ProcedureNamePluralPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(procedure.ProcedureName));
															procedure.ProcedureNamePluralCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(procedure.ProcedureName));
															procedure.ProcedureNamePluralConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetPluralForm(procedure.ProcedureName));

															// filter unwanted procedures (objects)
															if ((object)objectFilter != null)
															{
																if (!Enumerable.Any(objectFilter, f => Regex.IsMatch(procedure.ProcedureName, f)))
																	continue;
															}

															schema.Procedures.Add(procedure);

															var dictEnumParameter = DatazoidLegacyInstanceAccessor.AdoNetBufferingLegacyInstance.ExecuteRecords(false, connectionType, connectionString, false, IsolationLevel.Unspecified, CommandType.Text, GetAllAssemblyResourceFileText(this.GetType(), dataSourceTag, "ProcedureParameters"), this.CoreGetParameterParameters(connectionType, dataSourceTag, server, database, schema, procedure));
															{
																if ((object)dictEnumParameter != null)
																{
																	foreach (var dictDataParameter in Enumerable.ToList(dictEnumParameter))
																	{
																		Parameter parameter;

																		parameter = new Parameter();

																		parameter.ParameterPrefix = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<string>(dictDataParameter["ParameterName"]).Substring(0, 1);
																		parameter.ParameterOrdinal = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<int>(dictDataParameter["ParameterOrdinal"]);
																		parameter.ParameterName = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<string>(dictDataParameter["ParameterName"]).Substring(1);
																		parameter.ParameterSize = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<int>(dictDataParameter["ParameterSize"]);
																		parameter.ParameterPrecision = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<byte>(dictDataParameter["ParameterPrecision"]);
																		parameter.ParameterScale = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<byte>(dictDataParameter["ParameterScale"]);
																		parameter.ParameterSqlType = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<string>(dictDataParameter["ParameterSqlType"]);
																		parameter.ParameterIsUserDefinedType = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<bool>(dictDataParameter["ParameterIsUserDefinedType"]);
																		parameter.ParameterIsOutput = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<bool>(dictDataParameter["ParameterIsOutput"]);
																		parameter.ParameterIsReadOnly = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<bool>(dictDataParameter["ParameterIsReadOnly"]);
																		parameter.ParameterIsCursorRef = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<bool>(dictDataParameter["ParameterIsCursorRef"]);
																		parameter.ParameterIsReturnValue = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<bool>(dictDataParameter["ParameterIsReturnValue"]);
																		parameter.ParameterHasDefault = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<bool>(dictDataParameter["ParameterHasDefault"]);
																		parameter.ParameterNullable = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<bool?>(dictDataParameter["ParameterNullable"]) ?? true;
																		parameter.ParameterDefaultValue = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<string>(dictDataParameter["ParameterDefaultValue"]);
																		parameter.ParameterIsResultColumn = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<bool>(dictDataParameter["ParameterIsResultColumn"]);
																		parameter.ParameterNamePascalCase = effectiveStandardCanonicalNaming.GetPascalCase(parameter.ParameterName);
																		parameter.ParameterNameCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(parameter.ParameterName);
																		parameter.ParameterNameConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(parameter.ParameterName);
																		parameter.ParameterNameSingularPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(parameter.ParameterName));
																		parameter.ParameterNameSingularCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(parameter.ParameterName));
																		parameter.ParameterNameSingularConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetSingularForm(parameter.ParameterName));
																		parameter.ParameterNamePluralPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(parameter.ParameterName));
																		parameter.ParameterNamePluralCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(parameter.ParameterName));
																		parameter.ParameterNamePluralConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetPluralForm(parameter.ParameterName));

																		parameter.ParameterDirection = !parameter.ParameterIsOutput ? ParameterDirection.Input : (!parameter.ParameterIsReadOnly ? ParameterDirection.InputOutput : ParameterDirection.Output);

																		clrType = this.CoreInferClrTypeForSqlType(dataSourceTag, parameter.ParameterSqlType, parameter.ParameterPrecision);
																		parameter.ParameterDbType = DatazoidLegacyInstanceAccessor.AdoNetBufferingLegacyInstance.InferDbTypeForClrType(clrType);
																		parameter.ParameterSize = this.CoreCalculateParameterSize(dataSourceTag, parameter);

																		parameter.ParameterClrType = clrType;
																		parameter.ParameterClrNullableType = SolderLegacyInstanceAccessor.ReflectionFascadeLegacyInstance.MakeNullableType(clrType);
																		parameter.ParameterClrNonNullableType = SolderLegacyInstanceAccessor.ReflectionFascadeLegacyInstance.MakeNonNullableType(clrType);
																		parameter.ParameterCSharpDbType = String.Format("{0}.{1}", typeof(DbType).Name, parameter.ParameterDbType);
																		parameter.ParameterCSharpDirection = String.Format("{0}.{1}", typeof(ParameterDirection).Name, parameter.ParameterDirection);
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
																	parameter.ParameterOrdinal = Int32.MaxValue;
																	parameter.ParameterName = SchemaInfoConstants.PARAMETER_NAME_RETURN_VALUE;
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
																	parameter.ParameterNamePascalCase = effectiveStandardCanonicalNaming.GetPascalCase(SchemaInfoConstants.PARAMETER_NAME_RETURN_VALUE);
																	parameter.ParameterNameCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(SchemaInfoConstants.PARAMETER_NAME_RETURN_VALUE);
																	parameter.ParameterNameConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(SchemaInfoConstants.PARAMETER_NAME_RETURN_VALUE);
																	parameter.ParameterNameSingularPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(SchemaInfoConstants.PARAMETER_NAME_RETURN_VALUE));
																	parameter.ParameterNameSingularCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(SchemaInfoConstants.PARAMETER_NAME_RETURN_VALUE));
																	parameter.ParameterNameSingularConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetSingularForm(SchemaInfoConstants.PARAMETER_NAME_RETURN_VALUE));
																	parameter.ParameterNamePluralPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(SchemaInfoConstants.PARAMETER_NAME_RETURN_VALUE));
																	parameter.ParameterNamePluralCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(SchemaInfoConstants.PARAMETER_NAME_RETURN_VALUE));
																	parameter.ParameterNamePluralConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetPluralForm(SchemaInfoConstants.PARAMETER_NAME_RETURN_VALUE));

																	parameter.ParameterNullable = true;
																	parameter.ParameterDirection = ParameterDirection.ReturnValue;

																	clrType = this.CoreInferClrTypeForSqlType(dataSourceTag, parameter.ParameterSqlType, parameter.ParameterPrecision);
																	parameter.ParameterDbType = DatazoidLegacyInstanceAccessor.AdoNetBufferingLegacyInstance.InferDbTypeForClrType(clrType);
																	parameter.ParameterSize = this.CoreCalculateParameterSize(dataSourceTag, parameter);

																	parameter.ParameterClrType = clrType;
																	parameter.ParameterClrNullableType = SolderLegacyInstanceAccessor.ReflectionFascadeLegacyInstance.MakeNullableType(clrType);
																	parameter.ParameterClrNonNullableType = SolderLegacyInstanceAccessor.ReflectionFascadeLegacyInstance.MakeNonNullableType(clrType);
																	parameter.ParameterCSharpDbType = String.Format("{0}.{1}", typeof(DbType).Name, parameter.ParameterDbType);
																	parameter.ParameterCSharpDirection = String.Format("{0}.{1}", typeof(ParameterDirection).Name, parameter.ParameterDirection);
																	parameter.ParameterCSharpClrType = (object)parameter.ParameterClrType != null ? FormatCSharpType(parameter.ParameterClrType) : FormatCSharpType(typeof(object));
																	parameter.ParameterCSharpClrNullableType = (object)parameter.ParameterClrNullableType != null ? FormatCSharpType(parameter.ParameterClrNullableType) : FormatCSharpType(typeof(object));
																	parameter.ParameterCSharpClrNonNullableType = (object)parameter.ParameterClrNonNullableType != null ? FormatCSharpType(parameter.ParameterClrNonNullableType) : FormatCSharpType(typeof(object));
																	parameter.ParameterCSharpNullableLiteral = parameter.ParameterNullable.ToString().ToLower();

																	procedure.Parameters.Add(parameter);
																}
															}

															// re-map result column parameters into first class columns
															Parameter[] columnParameters;
															columnParameters = Enumerable.ToArray(procedure.Parameters.Where(p => p.ParameterIsResultColumn));

															if ((object)columnParameters != null && columnParameters.Length > 0)
															{
																foreach (Parameter columnParameter in columnParameters)
																{
																	ProcedureColumn column;

																	column = new ProcedureColumn();

																	column.ColumnOrdinal = columnParameter.ParameterOrdinal;
																	column.ColumnName = columnParameter.ParameterName;
																	column.ColumnCSharpIsAnonymousLiteral = column.ColumnIsAnonymous.ToString().ToLower(); // should be false always
																	column.ColumnNullable = columnParameter.ParameterNullable;
																	column.ColumnSize = columnParameter.ParameterSize;
																	column.ColumnPrecision = columnParameter.ParameterPrecision;
																	column.ColumnScale = columnParameter.ParameterScale;
																	column.ColumnSqlType = columnParameter.ParameterSqlType;
																	column.ColumnIsUserDefinedType = columnParameter.ParameterIsUserDefinedType;
																	column.ColumnHasDefault = !SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.IsNullOrWhiteSpace(columnParameter.ParameterDefaultValue);
																	column.ColumnNamePascalCase = effectiveStandardCanonicalNaming.GetPascalCase(columnParameter.ParameterName);
																	column.ColumnNameCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(columnParameter.ParameterName);
																	column.ColumnNameConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(columnParameter.ParameterName);
																	column.ColumnNameSingularPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(columnParameter.ParameterName));
																	column.ColumnNameSingularCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(columnParameter.ParameterName));
																	column.ColumnNameSingularConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetSingularForm(columnParameter.ParameterName));
																	column.ColumnNamePluralPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(columnParameter.ParameterName));
																	column.ColumnNamePluralCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(columnParameter.ParameterName));
																	column.ColumnNamePluralConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetPluralForm(columnParameter.ParameterName));

																	clrType = this.CoreInferClrTypeForSqlType(dataSourceTag, columnParameter.ParameterSqlType, columnParameter.ParameterPrecision);
																	column.ColumnDbType = DatazoidLegacyInstanceAccessor.AdoNetBufferingLegacyInstance.InferDbTypeForClrType(clrType);
																	column.ColumnSize = this.CoreCalculateColumnSize(dataSourceTag, column); //recalculate

																	column.ColumnClrType = clrType ?? typeof(object);
																	column.ColumnClrNullableType = SolderLegacyInstanceAccessor.ReflectionFascadeLegacyInstance.MakeNullableType(clrType);
																	column.ColumnClrNonNullableType = SolderLegacyInstanceAccessor.ReflectionFascadeLegacyInstance.MakeNonNullableType(clrType);
																	column.ColumnCSharpNullableLiteral = column.ColumnNullable.ToString().ToLower();
																	column.ColumnCSharpDbType = String.Format("{0}.{1}", typeof(DbType).Name, column.ColumnDbType);
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
																DbParameter[] parameters;
																parameters = Enumerable.ToArray<DbParameter>(procedure.Parameters.Where(p => !p.ParameterIsReturnValue && !p.ParameterIsResultColumn).Select(p => DatazoidLegacyInstanceAccessor.AdoNetBufferingLegacyInstance.CreateParameter(connectionType, null, p.ParameterIsOutput ? ParameterDirection.Output : ParameterDirection.Input, p.ParameterDbType, p.ParameterSize, (byte)p.ParameterPrecision, (byte)p.ParameterScale, p.ParameterNullable, p.ParameterName, null)));

																try
																{
																	int resultsetIndex = Int32.MinValue;

																	var dictEnumResultsets = DatazoidLegacyInstanceAccessor.AdoNetBufferingLegacyInstance.ExecuteRecords(true, connectionType, connectionString, false, IsolationLevel.Unspecified, CommandType.StoredProcedure, String.Format(GetAllAssemblyResourceFileText(this.GetType(), dataSourceTag, "ProcedureSchema"), server.ServerName, database.DatabaseName, schema.SchemaName, procedure.ProcedureName), parameters, (ri) => resultsetIndex = ri);
																	{
																		if ((object)dictEnumResultsets != null)
																		{
																			foreach (var dictDataResultset in dictEnumResultsets)
																			{
																				ProcedureResultset procedureResultset;

																				procedureResultset = new ProcedureResultset();
																				procedureResultset.ResultsetIndex = resultsetIndex;

																				foreach (var dictDataMetadata in Column.FixupDuplicateColumns(dictEnumResultsets))
																				{
																					ProcedureColumn column;

																					column = new ProcedureColumn();

																					column.ColumnOrdinal = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<int>(dictDataMetadata[SchemaTableColumn.ColumnOrdinal]);
																					column.ColumnName = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<string>(dictDataMetadata[SchemaTableColumn.ColumnName]);
																					column.ColumnIsAnonymous = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<bool>(dictDataMetadata["ColumnIsAnonymous"]);
																					column.ColumnCSharpIsAnonymousLiteral = column.ColumnIsAnonymous.ToString().ToLower();
																					column.ColumnSize = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<int>(dictDataMetadata[SchemaTableColumn.ColumnSize]);
																					column.ColumnPrecision = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<int>(dictDataMetadata[SchemaTableColumn.NumericPrecision]);
																					column.ColumnScale = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<int>(dictDataMetadata[SchemaTableColumn.NumericScale]);
																					column.ColumnSqlType = String.Empty;
																					//column.ColumnXXX = DataTypeFascade.ReflectionFascadeLegacyInstance.ChangeType<object>(dictDataMetadata[SchemaTableColumn.ProviderType]);
																					//column.ColumnXXX = DataTypeFascade.ReflectionFascadeLegacyInstance.ChangeType<object>(dictDataMetadata[SchemaTableColumn.NonVersionedProviderType]);
																					//column.ColumnXXX = DataTypeFascade.ReflectionFascadeLegacyInstance.ChangeType<object>(dictDataMetadata[SchemaTableColumn.IsLong]);
																					column.ColumnNullable = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<bool>(dictDataMetadata[SchemaTableColumn.AllowDBNull]);
																					//column.ColumnXXX = DataTypeFascade.ReflectionFascadeLegacyInstance.ChangeType<object>(dictDataMetadata[SchemaTableColumn.IsAliased]);
																					//column.ColumnXXX = DataTypeFascade.ReflectionFascadeLegacyInstance.ChangeType<object>(dictDataMetadata[SchemaTableColumn.IsExpression]);
																					//column.ColumnXXX = DataTypeFascade.ReflectionFascadeLegacyInstance.ChangeType<object>(dictDataMetadata[SchemaTableColumn.IsKey]);
																					//column.ColumnXXX = DataTypeFascade.ReflectionFascadeLegacyInstance.ChangeType<object>(dictDataMetadata[SchemaTableColumn.IsUnique]);
																					//column.ColumnXXX = DataTypeFascade.ReflectionFascadeLegacyInstance.ChangeType<object>(dictDataMetadata[SchemaTableColumn.BaseSchemaName]);
																					//column.ColumnXXX = DataTypeFascade.ReflectionFascadeLegacyInstance.ChangeType<object>(dictDataMetadata[SchemaTableColumn.BaseTableName]);
																					//column.ColumnXXX = DataTypeFascade.ReflectionFascadeLegacyInstance.ChangeType<object>(dictDataMetadata[SchemaTableColumn.BaseColumnName]);

																					column.ColumnNamePascalCase = effectiveStandardCanonicalNaming.GetPascalCase(column.ColumnName);
																					column.ColumnNameCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(column.ColumnName);
																					column.ColumnNameConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(column.ColumnName);
																					column.ColumnNameSingularPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(column.ColumnName));
																					column.ColumnNameSingularCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(column.ColumnName));
																					column.ColumnNameSingularConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetSingularForm(column.ColumnName));
																					column.ColumnNamePluralPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(column.ColumnName));
																					column.ColumnNamePluralCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(column.ColumnName));
																					column.ColumnNamePluralConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetPluralForm(column.ColumnName));

																					clrType = SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.ChangeType<Type>(dictDataMetadata[SchemaTableColumn.DataType]);
																					column.ColumnDbType = DatazoidLegacyInstanceAccessor.AdoNetBufferingLegacyInstance.InferDbTypeForClrType(clrType);
																					column.ColumnSize = this.CoreCalculateColumnSize(dataSourceTag, column); //recalculate

																					column.ColumnClrType = clrType ?? typeof(object);
																					column.ColumnClrNullableType = SolderLegacyInstanceAccessor.ReflectionFascadeLegacyInstance.MakeNullableType(clrType);
																					column.ColumnClrNonNullableType = SolderLegacyInstanceAccessor.ReflectionFascadeLegacyInstance.MakeNonNullableType(clrType);
																					column.ColumnCSharpNullableLiteral = column.ColumnNullable.ToString().ToLower();
																					column.ColumnCSharpDbType = String.Format("{0}.{1}", typeof(DbType).Name, column.ColumnDbType);
																					column.ColumnCSharpClrType = (object)column.ColumnClrType != null ? FormatCSharpType(column.ColumnClrType) : FormatCSharpType(typeof(object));
																					column.ColumnCSharpClrNullableType = (object)column.ColumnClrNullableType != null ? FormatCSharpType(column.ColumnClrNullableType) : FormatCSharpType(typeof(object));
																					column.ColumnCSharpClrNonNullableType = (object)column.ColumnClrNonNullableType != null ? FormatCSharpType(column.ColumnClrNonNullableType) : FormatCSharpType(typeof(object));

																					procedureResultset.Columns.Add(column);
																				}

																				procedure.Resultsets.Add(procedureResultset);
																			}
																		}
																	}
																}
																catch (Exception ex)
																{
																	procedure.ProcedureExecuteSchemaThrewException = true;
																	procedure.ProcedureExecuteSchemaExceptionText = SolderLegacyInstanceAccessor.ReflectionFascadeLegacyInstance.GetErrors(ex, 0);
																	//Console.Error.WriteLine(ReflectionFascade.ReflectionFascadeLegacyInstance.GetErrors(ex, 0));
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

		#region Classes/Structs/Interfaces/Enums/Delegates

		private static class SchemaTableColumn
		{
			#region Properties/Indexers/Events

			public static string AllowDBNull
			{
				get;
				set;
			}

			public static string ColumnName
			{
				get;
				set;
			}

			public static string ColumnOrdinal
			{
				get;
				set;
			}

			public static string ColumnSize
			{
				get;
				set;
			}

			public static string DataType
			{
				get;
				set;
			}

			public static string NumericPrecision
			{
				get;
				set;
			}

			public static string NumericScale
			{
				get;
				set;
			}

			#endregion
		}

		#endregion
	}
}