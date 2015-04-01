/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;

using LeastViable.Common.Fascades.AdoNet;
using LeastViable.Common.Fascades.AdoNet.UoW;
using LeastViable.Common.Fascades.Utilities;
using LeastViable.Common.Strategies.Serialization;

using TextMetal.Framework.Core;

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

			if (DataTypeFascade.Instance.IsWhiteSpace(name))
				throw new ArgumentOutOfRangeException("name");

			resourcePath = string.Format("{0}.DML.{1}.{2}.sql", type.Namespace, folder, name);

			if (!type.TryGetStringFromAssemblyResource(resourcePath, out sqlText))
				throw new InvalidOperationException(string.Format("Failed to obtain assembly manifest (embedded) resource '{0}'.", resourcePath));

			return sqlText;
		}

		protected abstract int CoreCalculateColumnSize(string dataSourceTag, Column column);

		protected abstract int CoreCalculateParameterSize(string dataSourceTag, Parameter parameter);

		protected abstract IEnumerable<IDbDataParameter> CoreGetColumnParameters(IUnitOfWork unitOfWork, string dataSourceTag, Server server, Database database, Schema schema, Table table);

		protected abstract IEnumerable<IDbDataParameter> CoreGetColumnParameters(IUnitOfWork unitOfWork, string dataSourceTag, Server server, Database database, Schema schema, View view);

		protected abstract IEnumerable<IDbDataParameter> CoreGetDatabaseParameters(IUnitOfWork unitOfWork, string dataSourceTag, Server server);

		protected abstract IEnumerable<IDbDataParameter> CoreGetDdlTriggerParameters(IUnitOfWork unitOfWork, string dataSourceTag, Server server, Database database);

		protected abstract IEnumerable<IDbDataParameter> CoreGetDmlTriggerParameters(IUnitOfWork unitOfWork, string dataSourceTag, Server server, Database database, Schema schema, Table table);

		protected abstract bool CoreGetEmitImplicitReturnParameter(string dataSourceTag);

		protected abstract IEnumerable<IDbDataParameter> CoreGetForeignKeyColumnParameters(IUnitOfWork unitOfWork, string dataSourceTag, Server server, Database database, Schema schema, Table table, ForeignKey foreignKey);

		protected abstract IEnumerable<IDbDataParameter> CoreGetForeignKeyParameters(IUnitOfWork unitOfWork, string dataSourceTag, Server server, Database database, Schema schema, Table table);

		protected abstract IEnumerable<IDbDataParameter> CoreGetParameterParameters(IUnitOfWork unitOfWork, string dataSourceTag, Server server, Database database, Schema schema, Procedure procedure);

		protected abstract string CoreGetParameterPrefix(string dataSourceTag);

		protected abstract IEnumerable<IDbDataParameter> CoreGetProcedureParameters(IUnitOfWork unitOfWork, string dataSourceTag, Server server, Database database, Schema schema);

		protected abstract IEnumerable<IDbDataParameter> CoreGetSchemaParameters(IUnitOfWork unitOfWork, string dataSourceTag, Server server, Database database);

		protected abstract IEnumerable<IDbDataParameter> CoreGetServerParameters(IUnitOfWork unitOfWork, string dataSourceTag);

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

			if (DataTypeFascade.Instance.IsNullOrWhiteSpace(connectionString))
				connectionString = sourceFilePath;

			if (DataTypeFascade.Instance.IsWhiteSpace(connectionString))
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
					DataTypeFascade.Instance.TryParse<bool>(values[0], out disableProcSchDisc);
			}

			enableDatabaseFilter = false;
			if (properties.TryGetValue(PROP_ENABLE_DATABASE_FILTER, out values))
			{
				if ((object)values != null && values.Count > 0)
					DataTypeFascade.Instance.TryParse<bool>(values[0], out enableDatabaseFilter);
			}

			disableNameMangling = false;
			if (properties.TryGetValue(PROP_DISABLE_NAME_MANGLING, out values))
			{
				if ((object)values != null && values.Count > 0)
					DataTypeFascade.Instance.TryParse<bool>(values[0], out disableNameMangling);
			}

			return this.GetSchemaModel(connectionString, connectionType, dataSourceTag, serverFilter, databaseFilter, schemaFilter, objectFilter, disableProcSchDisc, enableDatabaseFilter, disableNameMangling);
		}

		protected abstract IEnumerable<IDbDataParameter> CoreGetTableParameters(IUnitOfWork unitOfWork, string dataSourceTag, Server server, Database database, Schema schema);

		protected abstract IEnumerable<IDbDataParameter> CoreGetUniqueKeyColumnParameters(IUnitOfWork unitOfWork, string dataSourceTag, Server server, Database database, Schema schema, Table table, UniqueKey uniqueKey);

		protected abstract IEnumerable<IDbDataParameter> CoreGetUniqueKeyParameters(IUnitOfWork unitOfWork, string dataSourceTag, Server server, Database database, Schema schema, Table table);

		protected abstract Type CoreInferClrTypeForSqlType(string dataSourceTag, string sqlType, int sqlPrecision);

		protected abstract Type CoreInferSqlMetalClrTypeForClrType(string dataSourceTag, Type clrType);

		private object GetSchemaModel(string connectionString, Type connectionType, string dataSourceTag,
			string[] serverFilter, string[] databaseFilter,
			string[] schemaFilter, string[] objectFilter,
			bool disableProcSchDisc, bool enableDatabaseFilter, bool disableNameMangling)
		{
			Server server;
			int recordsAffected;
			const string RETURN_VALUE = "ReturnValue";
			Type clrType;
			StandardCanonicalNaming effectiveStandardCanonicalNaming;

			if ((object)connectionString == null)
				throw new ArgumentNullException("connectionString");

			if ((object)connectionType == null)
				throw new ArgumentNullException("connectionType");

			if ((object)dataSourceTag == null)
				throw new ArgumentNullException("dataSourceTag");

			effectiveStandardCanonicalNaming = disableNameMangling ? StandardCanonicalNaming.InstanceDisableNameMangling : StandardCanonicalNaming.Instance;

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
						server.ServerName = DataTypeFascade.Instance.ChangeType<string>(dictDataServer["ServerName"]);
						server.MachineName = DataTypeFascade.Instance.ChangeType<string>(dictDataServer["MachineName"]);
						server.InstanceName = DataTypeFascade.Instance.ChangeType<string>(dictDataServer["InstanceName"]);
						server.ServerVersion = DataTypeFascade.Instance.ChangeType<string>(dictDataServer["ServerVersion"]);
						server.ServerLevel = DataTypeFascade.Instance.ChangeType<string>(dictDataServer["ServerLevel"]);
						server.ServerEdition = DataTypeFascade.Instance.ChangeType<string>(dictDataServer["ServerEdition"]);
						server.DefaultDatabaseName = DataTypeFascade.Instance.ChangeType<string>(dictDataServer["DefaultDatabaseName"]);

						// filter unwanted servers
						if ((object)serverFilter != null)
						{
							if (!serverFilter.Any(f => Regex.IsMatch(server.ServerName, f)))
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
									database.DatabaseId = DataTypeFascade.Instance.ChangeType<int>(dictDataDatabase["DatabaseId"]);
									database.DatabaseName = DataTypeFascade.Instance.ChangeType<string>(dictDataDatabase["DatabaseName"]);
									database.CreationTimestamp = DataTypeFascade.Instance.ChangeType<DateTime>(dictDataDatabase["CreationTimestamp"]);
									database.DatabaseNamePascalCase = effectiveStandardCanonicalNaming.GetPascalCase(database.DatabaseName);
									database.DatabaseNameCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(database.DatabaseName);
									database.DatabaseNameConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(database.DatabaseName);
									database.DatabaseNameSingularPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(database.DatabaseName));
									database.DatabaseNameSingularCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(database.DatabaseName));
									database.DatabaseNameSingularConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetSingularForm(database.DatabaseName));
									database.DatabaseNamePluralPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(database.DatabaseName));
									database.DatabaseNamePluralCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(database.DatabaseName));
									database.DatabaseNamePluralConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetPluralForm(database.DatabaseName));

									database.DatabaseNameSqlMetalPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(database.DatabaseName);
									database.DatabaseNameSqlMetalCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(database.DatabaseName);
									database.DatabaseNameSqlMetalSingularPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(database.DatabaseName));
									database.DatabaseNameSqlMetalSingularCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(database.DatabaseName));
									database.DatabaseNameSqlMetalPluralPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(database.DatabaseName));
									database.DatabaseNameSqlMetalPluralCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(database.DatabaseName));

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
											if (!databaseFilter.Any(f => Regex.IsMatch(database.DatabaseName, f)))
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

												trigger.TriggerId = DataTypeFascade.Instance.ChangeType<int>(dictDataTrigger["TriggerId"]);
												trigger.TriggerName = DataTypeFascade.Instance.ChangeType<string>(dictDataTrigger["TriggerName"]);
												trigger.IsClrTrigger = DataTypeFascade.Instance.ChangeType<bool>(dictDataTrigger["IsClrTrigger"]);
												trigger.IsTriggerDisabled = DataTypeFascade.Instance.ChangeType<bool>(dictDataTrigger["IsTriggerDisabled"]);
												trigger.IsTriggerNotForReplication = DataTypeFascade.Instance.ChangeType<bool>(dictDataTrigger["IsTriggerNotForReplication"]);
												trigger.IsInsteadOfTrigger = DataTypeFascade.Instance.ChangeType<bool>(dictDataTrigger["IsInsteadOfTrigger"]);
												trigger.TriggerNamePascalCase = effectiveStandardCanonicalNaming.GetPascalCase(trigger.TriggerName);
												trigger.TriggerNameCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(trigger.TriggerName);
												trigger.TriggerNameConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(trigger.TriggerName);
												trigger.TriggerNameSingularPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(trigger.TriggerName));
												trigger.TriggerNameSingularCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(trigger.TriggerName));
												trigger.TriggerNameSingularConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetSingularForm(trigger.TriggerName));
												trigger.TriggerNamePluralPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(trigger.TriggerName));
												trigger.TriggerNamePluralCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(trigger.TriggerName));
												trigger.TriggerNamePluralConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetPluralForm(trigger.TriggerName));

												trigger.TriggerNameSqlMetalPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(trigger.TriggerName);
												trigger.TriggerNameSqlMetalCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(trigger.TriggerName);
												trigger.TriggerNameSqlMetalSingularPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(trigger.TriggerName));
												trigger.TriggerNameSqlMetalSingularCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(trigger.TriggerName));
												trigger.TriggerNameSqlMetalPluralPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(trigger.TriggerName));
												trigger.TriggerNameSqlMetalPluralCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(trigger.TriggerName));

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
												schema.SchemaId = DataTypeFascade.Instance.ChangeType<int>(dictDataSchema["SchemaId"]);
												schema.OwnerId = DataTypeFascade.Instance.ChangeType<int>(dictDataSchema["OwnerId"]);
												schema.SchemaName = DataTypeFascade.Instance.ChangeType<string>(dictDataSchema["SchemaName"]);
												schema.SchemaNamePascalCase = effectiveStandardCanonicalNaming.GetPascalCase(schema.SchemaName);
												schema.SchemaNameCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(schema.SchemaName);
												schema.SchemaNameConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(schema.SchemaName);
												schema.SchemaNameSingularPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(schema.SchemaName));
												schema.SchemaNameSingularCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(schema.SchemaName));
												schema.SchemaNameSingularConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetSingularForm(schema.SchemaName));
												schema.SchemaNamePluralPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(schema.SchemaName));
												schema.SchemaNamePluralCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(schema.SchemaName));
												schema.SchemaNamePluralConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetPluralForm(schema.SchemaName));

												schema.SchemaNameSqlMetalPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(schema.SchemaName);
												schema.SchemaNameSqlMetalCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(schema.SchemaName);
												schema.SchemaNameSqlMetalSingularPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(schema.SchemaName));
												schema.SchemaNameSqlMetalSingularCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(schema.SchemaName));
												schema.SchemaNameSqlMetalPluralPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(schema.SchemaName));
												schema.SchemaNameSqlMetalPluralCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(schema.SchemaName));

												// filter unwanted schemas
												if ((object)schemaFilter != null)
												{
													if (!schemaFilter.Any(f => Regex.IsMatch(schema.SchemaName, f)))
														continue;
												}

												database.Schemas.Add(schema);

												var dictEnumTable = unitOfWork.ExecuteDictionary(CommandType.Text, GetAllAssemblyResourceFileText(this.GetType(), dataSourceTag, "Tables"), this.CoreGetTableParameters(unitOfWork, dataSourceTag, server, database, schema), out recordsAffected);
												{
													foreach (var dictDataTable in dictEnumTable)
													{
														Table table;

														table = new Table();
														table.TableId = DataTypeFascade.Instance.ChangeType<int>(dictDataTable["TableId"]);
														table.TableName = DataTypeFascade.Instance.ChangeType<string>(dictDataTable["TableName"]);
														table.CreationTimestamp = DataTypeFascade.Instance.ChangeType<DateTime>(dictDataTable["CreationTimestamp"]);
														table.ModificationTimestamp = DataTypeFascade.Instance.ChangeType<DateTime>(dictDataTable["ModificationTimestamp"]);
														table.IsImplementationDetail = DataTypeFascade.Instance.ChangeType<bool>(dictDataTable["IsImplementationDetail"]);
														table.TableNamePascalCase = effectiveStandardCanonicalNaming.GetPascalCase(table.TableName);
														table.TableNameCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(table.TableName);
														table.TableNameConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(table.TableName);
														table.TableNameSingularPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(table.TableName));
														table.TableNameSingularCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(table.TableName));
														table.TableNameSingularConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetSingularForm(table.TableName));
														table.TableNamePluralPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(table.TableName));
														table.TableNamePluralCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(table.TableName));
														table.TableNamePluralConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetPluralForm(table.TableName));

														table.TableNameSqlMetal = SqlMetalCanonicalNaming.Instance.GetObjectNamePascalCase(schema.SchemaName, table.TableName);
														table.TableNameSqlMetalPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(table.TableNameSqlMetal);
														table.TableNameSqlMetalCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(table.TableNameSqlMetal);
														table.TableNameSqlMetalSingularPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(table.TableNameSqlMetal));
														table.TableNameSqlMetalSingularCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(table.TableNameSqlMetal));
														table.TableNameSqlMetalPluralPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(table.TableNameSqlMetal));
														table.TableNameSqlMetalPluralCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(table.TableNameSqlMetal));

														var pkId = DataTypeFascade.Instance.ChangeType<int?>(dictDataTable["PrimaryKeyId"]);
														if ((object)pkId != null)
														{
															table.PrimaryKey = new PrimaryKey();
															table.PrimaryKey.PrimaryKeyId = (int)pkId;

															table.PrimaryKey.PrimaryKeyIsSystemNamed = DataTypeFascade.Instance.ChangeType<bool>(dictDataTable["PrimaryKeyIsSystemNamed"]);
															table.PrimaryKey.PrimaryKeyName = DataTypeFascade.Instance.ChangeType<string>(dictDataTable["PrimaryKeyName"]);

															table.PrimaryKey.PrimaryKeyNamePascalCase = effectiveStandardCanonicalNaming.GetPascalCase(table.PrimaryKey.PrimaryKeyName);
															table.PrimaryKey.PrimaryKeyNameCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(table.PrimaryKey.PrimaryKeyName);
															table.PrimaryKey.PrimaryKeyNameConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(table.PrimaryKey.PrimaryKeyName);
															table.PrimaryKey.PrimaryKeyNameSingularPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(table.PrimaryKey.PrimaryKeyName));
															table.PrimaryKey.PrimaryKeyNameSingularCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(table.PrimaryKey.PrimaryKeyName));
															table.PrimaryKey.PrimaryKeyNameSingularConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetSingularForm(table.PrimaryKey.PrimaryKeyName));
															table.PrimaryKey.PrimaryKeyNamePluralPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(table.PrimaryKey.PrimaryKeyName));
															table.PrimaryKey.PrimaryKeyNamePluralCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(table.PrimaryKey.PrimaryKeyName));
															table.PrimaryKey.PrimaryKeyNamePluralConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetPluralForm(table.PrimaryKey.PrimaryKeyName));

															table.PrimaryKey.PrimaryKeyNameSqlMetalPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(table.PrimaryKey.PrimaryKeyName);
															table.PrimaryKey.PrimaryKeyNameSqlMetalCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(table.PrimaryKey.PrimaryKeyName);
															table.PrimaryKey.PrimaryKeyNameSqlMetalSingularPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(table.PrimaryKey.PrimaryKeyName));
															table.PrimaryKey.PrimaryKeyNameSqlMetalSingularCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(table.PrimaryKey.PrimaryKeyName));
															table.PrimaryKey.PrimaryKeyNameSqlMetalPluralPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(table.PrimaryKey.PrimaryKeyName));
															table.PrimaryKey.PrimaryKeyNameSqlMetalPluralCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(table.PrimaryKey.PrimaryKeyName));
														}

														// filter unwanted tables (objects)
														if ((object)objectFilter != null)
														{
															if (!objectFilter.Any(f => Regex.IsMatch(table.TableName, f)))
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

																	column.ColumnOrdinal = DataTypeFascade.Instance.ChangeType<int>(dictDataColumn["ColumnOrdinal"]);
																	column.ColumnName = DataTypeFascade.Instance.ChangeType<string>(dictDataColumn["ColumnName"]);
																	column.ColumnCSharpIsAnonymousLiteral = column.ColumnIsAnonymous.ToString().ToLower();
																	column.ColumnNullable = DataTypeFascade.Instance.ChangeType<bool>(dictDataColumn["ColumnNullable"]);
																	column.ColumnSize = DataTypeFascade.Instance.ChangeType<int>(dictDataColumn["ColumnSize"]);
																	column.ColumnPrecision = DataTypeFascade.Instance.ChangeType<int>(dictDataColumn["ColumnPrecision"]);
																	column.ColumnScale = DataTypeFascade.Instance.ChangeType<int>(dictDataColumn["ColumnScale"]);
																	column.ColumnSqlType = DataTypeFascade.Instance.ChangeType<string>(dictDataColumn["ColumnSqlType"]);
																	column.ColumnIsUserDefinedType = DataTypeFascade.Instance.ChangeType<bool>(dictDataColumn["ColumnIsUserDefinedType"]);
																	column.ColumnIsIdentity = DataTypeFascade.Instance.ChangeType<bool>(dictDataColumn["ColumnIsIdentity"]);
																	column.ColumnIsComputed = DataTypeFascade.Instance.ChangeType<bool>(dictDataColumn["ColumnIsComputed"]);
																	column.ColumnHasDefault = DataTypeFascade.Instance.ChangeType<bool>(dictDataColumn["ColumnHasDefault"]);
																	column.ColumnHasCheck = DataTypeFascade.Instance.ChangeType<bool>(dictDataColumn["ColumnHasCheck"]);
																	column.ColumnIsPrimaryKey = DataTypeFascade.Instance.ChangeType<bool>(dictDataColumn["ColumnIsPrimaryKey"]);
																	column.ColumnPrimaryKeyOrdinal = DataTypeFascade.Instance.ChangeType<int>(dictDataColumn["ColumnPrimaryKeyOrdinal"]);
																	column.ColumnNamePascalCase = effectiveStandardCanonicalNaming.GetPascalCase(column.ColumnName);
																	column.ColumnNameCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(column.ColumnName);
																	column.ColumnNameConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(column.ColumnName);
																	column.ColumnNameSingularPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(column.ColumnName));
																	column.ColumnNameSingularCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(column.ColumnName));
																	column.ColumnNameSingularConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetSingularForm(column.ColumnName));
																	column.ColumnNamePluralPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(column.ColumnName));
																	column.ColumnNamePluralCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(column.ColumnName));
																	column.ColumnNamePluralConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetPluralForm(column.ColumnName));

																	column.ColumnNameSqlMetalPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(column.ColumnName);
																	column.ColumnNameSqlMetalCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(column.ColumnName);
																	column.ColumnNameSqlMetalSingularPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(column.ColumnName));
																	column.ColumnNameSqlMetalSingularCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(column.ColumnName));
																	column.ColumnNameSqlMetalPluralPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(column.ColumnName));
																	column.ColumnNameSqlMetalPluralCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(column.ColumnName));

																	clrType = this.CoreInferClrTypeForSqlType(dataSourceTag, column.ColumnSqlType, column.ColumnPrecision);
																	column.ColumnDbType = AdoNetFascade.Instance.InferDbTypeForClrType(clrType);
																	column.ColumnSize = this.CoreCalculateColumnSize(dataSourceTag, column); //recalculate

																	column.ColumnClrType = clrType ?? typeof(object);
																	column.ColumnClrNullableType = ReflectionFascade.Instance.MakeNullableType(clrType);
																	column.ColumnClrNonNullableType = ReflectionFascade.Instance.MakeNonNullableType(clrType);
																	column.ColumnCSharpNullableLiteral = column.ColumnNullable.ToString().ToLower();
																	column.ColumnCSharpIsPrimaryKeyLiteral = column.ColumnIsPrimaryKey.ToString().ToLower();
																	column.ColumnCSharpIsComputedLiteral = column.ColumnIsComputed.ToString().ToLower();
																	column.ColumnCSharpIsIdentityLiteral = column.ColumnIsIdentity.ToString().ToLower();
																	column.ColumnCSharpDbType = string.Format("{0}.{1}", typeof(DbType).Name, column.ColumnDbType);
																	column.ColumnCSharpClrType = (object)column.ColumnClrType != null ? FormatCSharpType(column.ColumnClrType) : FormatCSharpType(typeof(object));
																	column.ColumnCSharpClrNullableType = (object)column.ColumnClrNullableType != null ? FormatCSharpType(column.ColumnClrNullableType) : FormatCSharpType(typeof(object));
																	column.ColumnCSharpClrNonNullableType = (object)column.ColumnClrNonNullableType != null ? FormatCSharpType(column.ColumnClrNonNullableType) : FormatCSharpType(typeof(object));

																	clrType = this.CoreInferSqlMetalClrTypeForClrType(dataSourceTag, column.ColumnClrType);
																	column.ColumnSqlMetalClrType = clrType;
																	column.ColumnSqlMetalClrNullableType = ReflectionFascade.Instance.MakeNullableType(clrType);
																	column.ColumnSqlMetalClrNonNullableType = ReflectionFascade.Instance.MakeNonNullableType(clrType);
																	column.ColumnSqlMetalCSharpClrType = (object)column.ColumnSqlMetalClrType != null ? FormatCSharpType(column.ColumnSqlMetalClrType) : FormatCSharpType(typeof(object));
																	column.ColumnSqlMetalCSharpClrNullableType = (object)column.ColumnSqlMetalClrNullableType != null ? FormatCSharpType(column.ColumnSqlMetalClrNullableType) : FormatCSharpType(typeof(object));
																	column.ColumnSqlMetalCSharpClrNonNullableType = (object)column.ColumnSqlMetalClrNonNullableType != null ? FormatCSharpType(column.ColumnSqlMetalClrNonNullableType) : FormatCSharpType(typeof(object));

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

														var dictEnumDmlTrigger = unitOfWork.ExecuteDictionary(CommandType.Text, GetAllAssemblyResourceFileText(this.GetType(), dataSourceTag, "DmlTriggers"), this.CoreGetDmlTriggerParameters(unitOfWork, dataSourceTag, server, database, schema, table), out recordsAffected);
														{
															if ((object)dictEnumDmlTrigger != null)
															{
																foreach (var dictDataTrigger in dictEnumDmlTrigger)
																{
																	Trigger trigger;

																	trigger = new Trigger();

																	trigger.TriggerId = DataTypeFascade.Instance.ChangeType<int>(dictDataTrigger["TriggerId"]);
																	trigger.TriggerName = DataTypeFascade.Instance.ChangeType<string>(dictDataTrigger["TriggerName"]);
																	trigger.IsClrTrigger = DataTypeFascade.Instance.ChangeType<bool>(dictDataTrigger["IsClrTrigger"]);
																	trigger.IsTriggerDisabled = DataTypeFascade.Instance.ChangeType<bool>(dictDataTrigger["IsTriggerDisabled"]);
																	trigger.IsTriggerNotForReplication = DataTypeFascade.Instance.ChangeType<bool>(dictDataTrigger["IsTriggerNotForReplication"]);
																	trigger.IsInsteadOfTrigger = DataTypeFascade.Instance.ChangeType<bool>(dictDataTrigger["IsInsteadOfTrigger"]);
																	trigger.TriggerNamePascalCase = effectiveStandardCanonicalNaming.GetPascalCase(trigger.TriggerName);
																	trigger.TriggerNameCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(trigger.TriggerName);
																	trigger.TriggerNameConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(trigger.TriggerName);
																	trigger.TriggerNameSingularPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(trigger.TriggerName));
																	trigger.TriggerNameSingularCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(trigger.TriggerName));
																	trigger.TriggerNameSingularConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetSingularForm(trigger.TriggerName));
																	trigger.TriggerNamePluralPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(trigger.TriggerName));
																	trigger.TriggerNamePluralCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(trigger.TriggerName));
																	trigger.TriggerNamePluralConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetPluralForm(trigger.TriggerName));

																	trigger.TriggerNameSqlMetalPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(trigger.TriggerName);
																	trigger.TriggerNameSqlMetalCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(trigger.TriggerName);
																	trigger.TriggerNameSqlMetalSingularPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(trigger.TriggerName));
																	trigger.TriggerNameSqlMetalSingularCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(trigger.TriggerName));
																	trigger.TriggerNameSqlMetalPluralPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(trigger.TriggerName));
																	trigger.TriggerNameSqlMetalPluralCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(trigger.TriggerName));

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

																	foreignKey.ForeignKeyName = DataTypeFascade.Instance.ChangeType<string>(dictDataForeignKey["ForeignKeyName"]);
																	foreignKey.ForeignKeyIsDisabled = DataTypeFascade.Instance.ChangeType<bool>(dictDataForeignKey["ForeignKeyIsDisabled"]);
																	foreignKey.ForeignKeyIsSystemNamed = DataTypeFascade.Instance.ChangeType<bool>(dictDataForeignKey["ForeignKeyIsSystemNamed"]);
																	foreignKey.ForeignKeyIsForReplication = DataTypeFascade.Instance.ChangeType<bool>(dictDataForeignKey["ForeignKeyIsForReplication"]);
																	foreignKey.ForeignKeyOnDeleteRefIntAction = DataTypeFascade.Instance.ChangeType<byte>(dictDataForeignKey["ForeignKeyOnDeleteRefIntAction"]);
																	foreignKey.ForeignKeyOnDeleteRefIntActionSqlName = DataTypeFascade.Instance.ChangeType<string>(dictDataForeignKey["ForeignKeyOnDeleteRefIntActionSqlName"]);
																	foreignKey.ForeignKeyOnUpdateRefIntAction = DataTypeFascade.Instance.ChangeType<byte>(dictDataForeignKey["ForeignKeyOnUpdateRefIntAction"]);
																	foreignKey.ForeignKeyOnUpdateRefIntActionSqlName = DataTypeFascade.Instance.ChangeType<string>(dictDataForeignKey["ForeignKeyOnUpdateRefIntActionSqlName"]);
																	foreignKey.ForeignKeyNamePascalCase = effectiveStandardCanonicalNaming.GetPascalCase(foreignKey.ForeignKeyName);
																	foreignKey.ForeignKeyNameCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(foreignKey.ForeignKeyName);
																	foreignKey.ForeignKeyNameConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(foreignKey.ForeignKeyName);
																	foreignKey.ForeignKeyNameSingularPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(foreignKey.ForeignKeyName));
																	foreignKey.ForeignKeyNameSingularCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(foreignKey.ForeignKeyName));
																	foreignKey.ForeignKeyNameSingularConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetSingularForm(foreignKey.ForeignKeyName));
																	foreignKey.ForeignKeyNamePluralPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(foreignKey.ForeignKeyName));
																	foreignKey.ForeignKeyNamePluralCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(foreignKey.ForeignKeyName));
																	foreignKey.ForeignKeyNamePluralConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetPluralForm(foreignKey.ForeignKeyName));

																	foreignKey.ForeignKeyNameSqlMetalPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(foreignKey.ForeignKeyName);
																	foreignKey.ForeignKeyNameSqlMetalCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(foreignKey.ForeignKeyName);
																	foreignKey.ForeignKeyNameSqlMetalSingularPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(foreignKey.ForeignKeyName));
																	foreignKey.ForeignKeyNameSqlMetalSingularCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(foreignKey.ForeignKeyName));
																	foreignKey.ForeignKeyNameSqlMetalPluralPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(foreignKey.ForeignKeyName));
																	foreignKey.ForeignKeyNameSqlMetalPluralCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(foreignKey.ForeignKeyName));

																	foreignKey.TargetSchemaName = DataTypeFascade.Instance.ChangeType<string>(dictDataForeignKey["TargetSchemaName"]);
																	foreignKey.TargetSchemaNamePascalCase = effectiveStandardCanonicalNaming.GetPascalCase(foreignKey.TargetSchemaName);
																	foreignKey.TargetSchemaNameCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(foreignKey.TargetSchemaName);
																	foreignKey.TargetSchemaNameConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(foreignKey.TargetSchemaName);
																	foreignKey.TargetSchemaNameSingularPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(foreignKey.TargetSchemaName));
																	foreignKey.TargetSchemaNameSingularCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(foreignKey.TargetSchemaName));
																	foreignKey.TargetSchemaNameSingularConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetSingularForm(foreignKey.TargetSchemaName));
																	foreignKey.TargetSchemaNamePluralPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(foreignKey.TargetSchemaName));
																	foreignKey.TargetSchemaNamePluralCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(foreignKey.TargetSchemaName));
																	foreignKey.TargetSchemaNamePluralConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetPluralForm(foreignKey.TargetSchemaName));

																	foreignKey.TargetSchemaNameSqlMetalPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(foreignKey.TargetSchemaName);
																	foreignKey.TargetSchemaNameSqlMetalCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(foreignKey.TargetSchemaName);
																	foreignKey.TargetSchemaNameSqlMetalSingularPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(foreignKey.TargetSchemaName));
																	foreignKey.TargetSchemaNameSqlMetalSingularCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(foreignKey.TargetSchemaName));
																	foreignKey.TargetSchemaNameSqlMetalPluralPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(foreignKey.TargetSchemaName));
																	foreignKey.TargetSchemaNameSqlMetalPluralCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(foreignKey.TargetSchemaName));

																	foreignKey.TargetTableName = DataTypeFascade.Instance.ChangeType<string>(dictDataForeignKey["TargetTableName"]);
																	foreignKey.TargetTableNamePascalCase = effectiveStandardCanonicalNaming.GetPascalCase(foreignKey.TargetTableName);
																	foreignKey.TargetTableNameCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(foreignKey.TargetTableName);
																	foreignKey.TargetTableNameConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(foreignKey.TargetTableName);
																	foreignKey.TargetTableNameSingularPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(foreignKey.TargetTableName));
																	foreignKey.TargetTableNameSingularCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(foreignKey.TargetTableName));
																	foreignKey.TargetTableNameSingularConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetSingularForm(foreignKey.TargetTableName));
																	foreignKey.TargetTableNamePluralPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(foreignKey.TargetTableName));
																	foreignKey.TargetTableNamePluralCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(foreignKey.TargetTableName));
																	foreignKey.TargetTableNamePluralConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetPluralForm(foreignKey.TargetTableName));

																	foreignKey.TargetTableNameSqlMetalPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(foreignKey.TargetTableName);
																	foreignKey.TargetTableNameSqlMetalCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(foreignKey.TargetTableName);
																	foreignKey.TargetTableNameSqlMetalSingularPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(foreignKey.TargetTableName));
																	foreignKey.TargetTableNameSqlMetalSingularCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(foreignKey.TargetTableName));
																	foreignKey.TargetTableNameSqlMetalPluralPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(foreignKey.TargetTableName));
																	foreignKey.TargetTableNameSqlMetalPluralCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(foreignKey.TargetTableName));

																	table.ForeignKeys.Add(foreignKey);

																	var dictEnumForeignKeyColumn = unitOfWork.ExecuteDictionary(CommandType.Text, GetAllAssemblyResourceFileText(this.GetType(), dataSourceTag, "ForeignKeyColumns"), this.CoreGetForeignKeyColumnParameters(unitOfWork, dataSourceTag, server, database, schema, table, foreignKey), out recordsAffected);
																	{
																		if ((object)dictEnumForeignKeyColumn != null)
																		{
																			foreach (var dictDataForeignKeyColumn in dictEnumForeignKeyColumn)
																			{
																				ForeignKeyColumn foreignKeyColumn;

																				foreignKeyColumn = new ForeignKeyColumn();

																				foreignKeyColumn.ForeignKeyColumnOrdinal = DataTypeFascade.Instance.ChangeType<int>(dictDataForeignKeyColumn["ForeignKeyColumnOrdinal"]);
																				foreignKeyColumn.ColumnOrdinal = DataTypeFascade.Instance.ChangeType<int>(dictDataForeignKeyColumn["ColumnOrdinal"]);
																				foreignKeyColumn.ColumnName = DataTypeFascade.Instance.ChangeType<string>(dictDataForeignKeyColumn["ColumnName"]);
																				foreignKeyColumn.TargetColumnOrdinal = DataTypeFascade.Instance.ChangeType<int>(dictDataForeignKeyColumn["TargetColumnOrdinal"]);
																				foreignKeyColumn.TargetColumnName = DataTypeFascade.Instance.ChangeType<string>(dictDataForeignKeyColumn["TargetColumnName"]);

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

																	uniqueKey.UniqueKeyId = DataTypeFascade.Instance.ChangeType<int>(dictDataUniqueKey["UniqueKeyId"]);
																	uniqueKey.UniqueKeyName = DataTypeFascade.Instance.ChangeType<string>(dictDataUniqueKey["UniqueKeyName"]);
																	uniqueKey.UniqueKeyIsSystemNamed = DataTypeFascade.Instance.ChangeType<bool>(dictDataUniqueKey["UniqueKeyIsSystemNamed"]);
																	uniqueKey.UniqueKeyNamePascalCase = effectiveStandardCanonicalNaming.GetPascalCase(uniqueKey.UniqueKeyName);
																	uniqueKey.UniqueKeyNameCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(uniqueKey.UniqueKeyName);
																	uniqueKey.UniqueKeyNameConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(uniqueKey.UniqueKeyName);
																	uniqueKey.UniqueKeyNameSingularPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(uniqueKey.UniqueKeyName));
																	uniqueKey.UniqueKeyNameSingularCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(uniqueKey.UniqueKeyName));
																	uniqueKey.UniqueKeyNameSingularConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetSingularForm(uniqueKey.UniqueKeyName));
																	uniqueKey.UniqueKeyNamePluralPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(uniqueKey.UniqueKeyName));
																	uniqueKey.UniqueKeyNamePluralCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(uniqueKey.UniqueKeyName));
																	uniqueKey.UniqueKeyNamePluralConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetPluralForm(uniqueKey.UniqueKeyName));

																	uniqueKey.UniqueKeyNameSqlMetalPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(uniqueKey.UniqueKeyName);
																	uniqueKey.UniqueKeyNameSqlMetalCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(uniqueKey.UniqueKeyName);
																	uniqueKey.UniqueKeyNameSqlMetalSingularPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(uniqueKey.UniqueKeyName));
																	uniqueKey.UniqueKeyNameSqlMetalSingularCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(uniqueKey.UniqueKeyName));
																	uniqueKey.UniqueKeyNameSqlMetalPluralPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(uniqueKey.UniqueKeyName));
																	uniqueKey.UniqueKeyNameSqlMetalPluralCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(uniqueKey.UniqueKeyName));

																	table.UniqueKeys.Add(uniqueKey);

																	var dictEnumUniqueKeyColumn = unitOfWork.ExecuteDictionary(CommandType.Text, GetAllAssemblyResourceFileText(this.GetType(), dataSourceTag, "UniqueKeyColumns"), this.CoreGetUniqueKeyColumnParameters(unitOfWork, dataSourceTag, server, database, schema, table, uniqueKey), out recordsAffected);
																	{
																		if ((object)dictEnumUniqueKeyColumn != null)
																		{
																			foreach (var dictDataUniqueKeyColumn in dictEnumUniqueKeyColumn)
																			{
																				UniqueKeyColumn uniqueKeyColumn;

																				uniqueKeyColumn = new UniqueKeyColumn();

																				uniqueKeyColumn.UniqueKeyColumnOrdinal = DataTypeFascade.Instance.ChangeType<int>(dictDataUniqueKeyColumn["UniqueKeyColumnOrdinal"]);
																				uniqueKeyColumn.ColumnOrdinal = DataTypeFascade.Instance.ChangeType<int>(dictDataUniqueKeyColumn["ColumnOrdinal"]);
																				uniqueKeyColumn.ColumnName = DataTypeFascade.Instance.ChangeType<string>(dictDataUniqueKeyColumn["ColumnName"]);
																				uniqueKeyColumn.UniqueKeyColumnDescendingSort = DataTypeFascade.Instance.ChangeType<bool>(dictDataUniqueKeyColumn["UniqueKeyColumnDescendingSort"]);

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
														view.ViewId = DataTypeFascade.Instance.ChangeType<int>(dictDataView["ViewId"]);
														view.ViewName = DataTypeFascade.Instance.ChangeType<string>(dictDataView["ViewName"]);
														view.CreationTimestamp = DataTypeFascade.Instance.ChangeType<DateTime>(dictDataView["CreationTimestamp"]);
														view.ModificationTimestamp = DataTypeFascade.Instance.ChangeType<DateTime>(dictDataView["ModificationTimestamp"]);
														view.IsImplementationDetail = DataTypeFascade.Instance.ChangeType<bool>(dictDataView["IsImplementationDetail"]);
														view.ViewNamePascalCase = effectiveStandardCanonicalNaming.GetPascalCase(view.ViewName);
														view.ViewNameCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(view.ViewName);
														view.ViewNameConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(view.ViewName);
														view.ViewNameSingularPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(view.ViewName));
														view.ViewNameSingularCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(view.ViewName));
														view.ViewNameSingularConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetSingularForm(view.ViewName));
														view.ViewNamePluralPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(view.ViewName));
														view.ViewNamePluralCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(view.ViewName));
														view.ViewNamePluralConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetPluralForm(view.ViewName));

														view.ViewNameSqlMetal = SqlMetalCanonicalNaming.Instance.GetObjectNamePascalCase(schema.SchemaName, view.ViewName);
														view.ViewNameSqlMetalPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(view.ViewNameSqlMetal);
														view.ViewNameSqlMetalCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(view.ViewNameSqlMetal);
														view.ViewNameSqlMetalSingularPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(view.ViewNameSqlMetal));
														view.ViewNameSqlMetalSingularCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(view.ViewNameSqlMetal));
														view.ViewNameSqlMetalPluralPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(view.ViewNameSqlMetal));
														view.ViewNameSqlMetalPluralCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(view.ViewNameSqlMetal));

														// filter unwanted views (objects)
														if ((object)objectFilter != null)
														{
															if (!objectFilter.Any(f => Regex.IsMatch(view.ViewName, f)))
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

																	column.ColumnOrdinal = DataTypeFascade.Instance.ChangeType<int>(dictDataColumn["ColumnOrdinal"]);
																	column.ColumnName = DataTypeFascade.Instance.ChangeType<string>(dictDataColumn["ColumnName"]);
																	column.ColumnCSharpIsAnonymousLiteral = column.ColumnIsAnonymous.ToString().ToLower();
																	column.ColumnNullable = DataTypeFascade.Instance.ChangeType<bool>(dictDataColumn["ColumnNullable"]);
																	column.ColumnSize = DataTypeFascade.Instance.ChangeType<int>(dictDataColumn["ColumnSize"]);
																	column.ColumnPrecision = DataTypeFascade.Instance.ChangeType<int>(dictDataColumn["ColumnPrecision"]);
																	column.ColumnScale = DataTypeFascade.Instance.ChangeType<int>(dictDataColumn["ColumnScale"]);
																	column.ColumnSqlType = DataTypeFascade.Instance.ChangeType<string>(dictDataColumn["ColumnSqlType"]);
																	column.ColumnIsUserDefinedType = DataTypeFascade.Instance.ChangeType<bool>(dictDataColumn["ColumnIsUserDefinedType"]);
																	column.ColumnNamePascalCase = effectiveStandardCanonicalNaming.GetPascalCase(column.ColumnName);
																	column.ColumnNameCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(column.ColumnName);
																	column.ColumnNameConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(column.ColumnName);
																	column.ColumnNameSingularPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(column.ColumnName));
																	column.ColumnNameSingularCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(column.ColumnName));
																	column.ColumnNameSingularConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetSingularForm(column.ColumnName));
																	column.ColumnNamePluralPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(column.ColumnName));
																	column.ColumnNamePluralCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(column.ColumnName));
																	column.ColumnNamePluralConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetPluralForm(column.ColumnName));

																	column.ColumnNameSqlMetalPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(column.ColumnName);
																	column.ColumnNameSqlMetalCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(column.ColumnName);
																	column.ColumnNameSqlMetalSingularPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(column.ColumnName));
																	column.ColumnNameSqlMetalSingularCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(column.ColumnName));
																	column.ColumnNameSqlMetalPluralPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(column.ColumnName));
																	column.ColumnNameSqlMetalPluralCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(column.ColumnName));

																	clrType = this.CoreInferClrTypeForSqlType(dataSourceTag, column.ColumnSqlType, column.ColumnPrecision);
																	column.ColumnDbType = AdoNetFascade.Instance.InferDbTypeForClrType(clrType);
																	column.ColumnSize = this.CoreCalculateColumnSize(dataSourceTag, column); //recalculate

																	column.ColumnClrType = clrType ?? typeof(object);
																	column.ColumnClrNullableType = ReflectionFascade.Instance.MakeNullableType(clrType);
																	column.ColumnClrNonNullableType = ReflectionFascade.Instance.MakeNonNullableType(clrType);
																	column.ColumnCSharpNullableLiteral = column.ColumnNullable.ToString().ToLower();
																	column.ColumnCSharpDbType = string.Format("{0}.{1}", typeof(DbType).Name, column.ColumnDbType);
																	column.ColumnCSharpClrType = (object)column.ColumnClrType != null ? FormatCSharpType(column.ColumnClrType) : FormatCSharpType(typeof(object));
																	column.ColumnCSharpClrNullableType = (object)column.ColumnClrNullableType != null ? FormatCSharpType(column.ColumnClrNullableType) : FormatCSharpType(typeof(object));
																	column.ColumnCSharpClrNonNullableType = (object)column.ColumnClrNonNullableType != null ? FormatCSharpType(column.ColumnClrNonNullableType) : FormatCSharpType(typeof(object));

																	clrType = this.CoreInferSqlMetalClrTypeForClrType(dataSourceTag, column.ColumnClrType);
																	column.ColumnSqlMetalClrType = clrType;
																	column.ColumnSqlMetalClrNullableType = ReflectionFascade.Instance.MakeNullableType(clrType);
																	column.ColumnSqlMetalClrNonNullableType = ReflectionFascade.Instance.MakeNonNullableType(clrType);
																	column.ColumnSqlMetalCSharpClrType = (object)column.ColumnSqlMetalClrType != null ? FormatCSharpType(column.ColumnSqlMetalClrType) : FormatCSharpType(typeof(object));
																	column.ColumnSqlMetalCSharpClrNullableType = (object)column.ColumnSqlMetalClrNullableType != null ? FormatCSharpType(column.ColumnSqlMetalClrNullableType) : FormatCSharpType(typeof(object));
																	column.ColumnSqlMetalCSharpClrNonNullableType = (object)column.ColumnSqlMetalClrNonNullableType != null ? FormatCSharpType(column.ColumnSqlMetalClrNonNullableType) : FormatCSharpType(typeof(object));

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
															procedure.ProcedureName = DataTypeFascade.Instance.ChangeType<string>(dictDataProcedure["ProcedureName"]);
															procedure.ProcedureNamePascalCase = effectiveStandardCanonicalNaming.GetPascalCase(procedure.ProcedureName);
															procedure.ProcedureNameCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(procedure.ProcedureName);
															procedure.ProcedureNameConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(procedure.ProcedureName);
															procedure.ProcedureNameSingularPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(procedure.ProcedureName));
															procedure.ProcedureNameSingularCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(procedure.ProcedureName));
															procedure.ProcedureNameSingularConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetSingularForm(procedure.ProcedureName));
															procedure.ProcedureNamePluralPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(procedure.ProcedureName));
															procedure.ProcedureNamePluralCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(procedure.ProcedureName));
															procedure.ProcedureNamePluralConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetPluralForm(procedure.ProcedureName));

															procedure.ProcedureNameSqlMetalPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(procedure.ProcedureName);
															procedure.ProcedureNameSqlMetalCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(procedure.ProcedureName);
															procedure.ProcedureNameSqlMetalSingularPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(procedure.ProcedureName));
															procedure.ProcedureNameSqlMetalSingularCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(procedure.ProcedureName));
															procedure.ProcedureNameSqlMetalPluralPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(procedure.ProcedureName));
															procedure.ProcedureNameSqlMetalPluralCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(procedure.ProcedureName));

															// filter unwanted procedures (objects)
															if ((object)objectFilter != null)
															{
																if (!objectFilter.Any(f => Regex.IsMatch(procedure.ProcedureName, f)))
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

																		parameter.ParameterPrefix = DataTypeFascade.Instance.ChangeType<string>(dictDataParameter["ParameterName"]).Substring(0, 1);
																		parameter.ParameterOrdinal = DataTypeFascade.Instance.ChangeType<int>(dictDataParameter["ParameterOrdinal"]);
																		parameter.ParameterName = DataTypeFascade.Instance.ChangeType<string>(dictDataParameter["ParameterName"]).Substring(1);
																		parameter.ParameterSize = DataTypeFascade.Instance.ChangeType<int>(dictDataParameter["ParameterSize"]);
																		parameter.ParameterPrecision = DataTypeFascade.Instance.ChangeType<int>(dictDataParameter["ParameterPrecision"]);
																		parameter.ParameterScale = DataTypeFascade.Instance.ChangeType<int>(dictDataParameter["ParameterScale"]);
																		parameter.ParameterSqlType = DataTypeFascade.Instance.ChangeType<string>(dictDataParameter["ParameterSqlType"]);
																		parameter.ParameterIsUserDefinedType = DataTypeFascade.Instance.ChangeType<bool>(dictDataParameter["ParameterIsUserDefinedType"]);
																		parameter.ParameterIsOutput = DataTypeFascade.Instance.ChangeType<bool>(dictDataParameter["ParameterIsOutput"]);
																		parameter.ParameterIsReadOnly = DataTypeFascade.Instance.ChangeType<bool>(dictDataParameter["ParameterIsReadOnly"]);
																		parameter.ParameterIsCursorRef = DataTypeFascade.Instance.ChangeType<bool>(dictDataParameter["ParameterIsCursorRef"]);
																		parameter.ParameterIsReturnValue = DataTypeFascade.Instance.ChangeType<bool>(dictDataParameter["ParameterIsReturnValue"]);
																		parameter.ParameterHasDefault = DataTypeFascade.Instance.ChangeType<bool>(dictDataParameter["ParameterHasDefault"]);
																		parameter.ParameterNullable = DataTypeFascade.Instance.ChangeType<bool?>(dictDataParameter["ParameterNullable"]) ?? true;
																		parameter.ParameterDefaultValue = DataTypeFascade.Instance.ChangeType<string>(dictDataParameter["ParameterDefaultValue"]);
																		parameter.ParameterIsResultColumn = DataTypeFascade.Instance.ChangeType<bool>(dictDataParameter["ParameterIsResultColumn"]);
																		parameter.ParameterNamePascalCase = effectiveStandardCanonicalNaming.GetPascalCase(parameter.ParameterName);
																		parameter.ParameterNameCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(parameter.ParameterName);
																		parameter.ParameterNameConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(parameter.ParameterName);
																		parameter.ParameterNameSingularPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(parameter.ParameterName));
																		parameter.ParameterNameSingularCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(parameter.ParameterName));
																		parameter.ParameterNameSingularConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetSingularForm(parameter.ParameterName));
																		parameter.ParameterNamePluralPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(parameter.ParameterName));
																		parameter.ParameterNamePluralCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(parameter.ParameterName));
																		parameter.ParameterNamePluralConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetPluralForm(parameter.ParameterName));

																		parameter.ParameterNameSqlMetalPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(parameter.ParameterName);
																		parameter.ParameterNameSqlMetalCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(parameter.ParameterName);
																		parameter.ParameterNameSqlMetalSingularPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(parameter.ParameterName));
																		parameter.ParameterNameSqlMetalSingularCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(parameter.ParameterName));
																		parameter.ParameterNameSqlMetalPluralPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(parameter.ParameterName));
																		parameter.ParameterNameSqlMetalPluralCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(parameter.ParameterName));

																		parameter.ParameterDirection = !parameter.ParameterIsOutput ? ParameterDirection.Input : (!parameter.ParameterIsReadOnly ? ParameterDirection.InputOutput : ParameterDirection.Output);

																		clrType = this.CoreInferClrTypeForSqlType(dataSourceTag, parameter.ParameterSqlType, parameter.ParameterPrecision);
																		parameter.ParameterDbType = AdoNetFascade.Instance.InferDbTypeForClrType(clrType);
																		parameter.ParameterSize = this.CoreCalculateParameterSize(dataSourceTag, parameter);

																		parameter.ParameterClrType = clrType;
																		parameter.ParameterClrNullableType = ReflectionFascade.Instance.MakeNullableType(clrType);
																		parameter.ParameterClrNonNullableType = ReflectionFascade.Instance.MakeNonNullableType(clrType);
																		parameter.ParameterCSharpDbType = string.Format("{0}.{1}", typeof(DbType).Name, parameter.ParameterDbType);
																		parameter.ParameterCSharpDirection = string.Format("{0}.{1}", typeof(ParameterDirection).Name, parameter.ParameterDirection);
																		parameter.ParameterCSharpClrType = (object)parameter.ParameterClrType != null ? FormatCSharpType(parameter.ParameterClrType) : FormatCSharpType(typeof(object));
																		parameter.ParameterCSharpClrNullableType = (object)parameter.ParameterClrNullableType != null ? FormatCSharpType(parameter.ParameterClrNullableType) : FormatCSharpType(typeof(object));
																		parameter.ParameterCSharpClrNonNullableType = (object)parameter.ParameterClrNonNullableType != null ? FormatCSharpType(parameter.ParameterClrNonNullableType) : FormatCSharpType(typeof(object));
																		parameter.ParameterCSharpNullableLiteral = parameter.ParameterNullable.ToString().ToLower();

																		clrType = this.CoreInferSqlMetalClrTypeForClrType(dataSourceTag, parameter.ParameterClrType);
																		parameter.ParameterSqlMetalClrType = clrType;
																		parameter.ParameterSqlMetalClrNullableType = ReflectionFascade.Instance.MakeNullableType(clrType);
																		parameter.ParameterSqlMetalClrNonNullableType = ReflectionFascade.Instance.MakeNonNullableType(clrType);
																		parameter.ParameterSqlMetalCSharpClrType = (object)parameter.ParameterSqlMetalClrType != null ? FormatCSharpType(parameter.ParameterSqlMetalClrType) : FormatCSharpType(typeof(object));
																		parameter.ParameterSqlMetalCSharpClrNullableType = (object)parameter.ParameterSqlMetalClrNullableType != null ? FormatCSharpType(parameter.ParameterSqlMetalClrNullableType) : FormatCSharpType(typeof(object));
																		parameter.ParameterSqlMetalCSharpClrNonNullableType = (object)parameter.ParameterSqlMetalClrNonNullableType != null ? FormatCSharpType(parameter.ParameterSqlMetalClrNonNullableType) : FormatCSharpType(typeof(object));

																		procedure.Parameters.Add(parameter);
																	}
																}

																// implicit return value parameter
																if (this.CoreGetEmitImplicitReturnParameter(dataSourceTag))
																{
																	Parameter parameter;

																	parameter = new Parameter();

																	parameter.ParameterPrefix = this.CoreGetParameterPrefix(dataSourceTag);
																	parameter.ParameterOrdinal = int.MaxValue;
																	parameter.ParameterName = RETURN_VALUE;
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
																	parameter.ParameterNamePascalCase = effectiveStandardCanonicalNaming.GetPascalCase(RETURN_VALUE);
																	parameter.ParameterNameCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(RETURN_VALUE);
																	parameter.ParameterNameConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(RETURN_VALUE);
																	parameter.ParameterNameSingularPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(RETURN_VALUE));
																	parameter.ParameterNameSingularCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(RETURN_VALUE));
																	parameter.ParameterNameSingularConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetSingularForm(RETURN_VALUE));
																	parameter.ParameterNamePluralPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(RETURN_VALUE));
																	parameter.ParameterNamePluralCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(RETURN_VALUE));
																	parameter.ParameterNamePluralConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetPluralForm(RETURN_VALUE));

																	parameter.ParameterNameSqlMetalPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(RETURN_VALUE);
																	parameter.ParameterNameSqlMetalCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(RETURN_VALUE);
																	parameter.ParameterNameSqlMetalSingularPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(RETURN_VALUE));
																	parameter.ParameterNameSqlMetalSingularCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(RETURN_VALUE));
																	parameter.ParameterNameSqlMetalPluralPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(RETURN_VALUE));
																	parameter.ParameterNameSqlMetalPluralCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(RETURN_VALUE));

																	parameter.ParameterNullable = true;
																	parameter.ParameterDirection = ParameterDirection.ReturnValue;

																	clrType = this.CoreInferClrTypeForSqlType(dataSourceTag, parameter.ParameterSqlType, parameter.ParameterPrecision);
																	parameter.ParameterDbType = AdoNetFascade.Instance.InferDbTypeForClrType(clrType);
																	parameter.ParameterSize = this.CoreCalculateParameterSize(dataSourceTag, parameter);

																	parameter.ParameterClrType = clrType;
																	parameter.ParameterClrNullableType = ReflectionFascade.Instance.MakeNullableType(clrType);
																	parameter.ParameterClrNonNullableType = ReflectionFascade.Instance.MakeNonNullableType(clrType);
																	parameter.ParameterCSharpDbType = string.Format("{0}.{1}", typeof(DbType).Name, parameter.ParameterDbType);
																	parameter.ParameterCSharpDirection = string.Format("{0}.{1}", typeof(ParameterDirection).Name, parameter.ParameterDirection);
																	parameter.ParameterCSharpClrType = (object)parameter.ParameterClrType != null ? FormatCSharpType(parameter.ParameterClrType) : FormatCSharpType(typeof(object));
																	parameter.ParameterCSharpClrNullableType = (object)parameter.ParameterClrNullableType != null ? FormatCSharpType(parameter.ParameterClrNullableType) : FormatCSharpType(typeof(object));
																	parameter.ParameterCSharpClrNonNullableType = (object)parameter.ParameterClrNonNullableType != null ? FormatCSharpType(parameter.ParameterClrNonNullableType) : FormatCSharpType(typeof(object));
																	parameter.ParameterCSharpNullableLiteral = parameter.ParameterNullable.ToString().ToLower();

																	clrType = this.CoreInferSqlMetalClrTypeForClrType(dataSourceTag, parameter.ParameterClrType);
																	parameter.ParameterSqlMetalClrType = clrType;
																	parameter.ParameterSqlMetalClrNullableType = ReflectionFascade.Instance.MakeNullableType(clrType);
																	parameter.ParameterSqlMetalClrNonNullableType = ReflectionFascade.Instance.MakeNonNullableType(clrType);
																	parameter.ParameterSqlMetalCSharpClrType = (object)parameter.ParameterSqlMetalClrType != null ? FormatCSharpType(parameter.ParameterSqlMetalClrType) : FormatCSharpType(typeof(object));
																	parameter.ParameterSqlMetalCSharpClrNullableType = (object)parameter.ParameterSqlMetalClrNullableType != null ? FormatCSharpType(parameter.ParameterSqlMetalClrNullableType) : FormatCSharpType(typeof(object));
																	parameter.ParameterSqlMetalCSharpClrNonNullableType = (object)parameter.ParameterSqlMetalClrNonNullableType != null ? FormatCSharpType(parameter.ParameterSqlMetalClrNonNullableType) : FormatCSharpType(typeof(object));

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

																	column.ColumnOrdinal = columnParameter.ParameterOrdinal;
																	column.ColumnName = columnParameter.ParameterName;
																	column.ColumnCSharpIsAnonymousLiteral = column.ColumnIsAnonymous.ToString().ToLower();
																	column.ColumnNullable = columnParameter.ParameterNullable;
																	column.ColumnSize = columnParameter.ParameterSize;
																	column.ColumnPrecision = columnParameter.ParameterPrecision;
																	column.ColumnScale = columnParameter.ParameterScale;
																	column.ColumnSqlType = columnParameter.ParameterSqlType;
																	column.ColumnIsUserDefinedType = columnParameter.ParameterIsUserDefinedType;
																	column.ColumnHasDefault = !DataTypeFascade.Instance.IsNullOrWhiteSpace(columnParameter.ParameterDefaultValue);
																	column.ColumnNamePascalCase = effectiveStandardCanonicalNaming.GetPascalCase(columnParameter.ParameterName);
																	column.ColumnNameCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(columnParameter.ParameterName);
																	column.ColumnNameConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(columnParameter.ParameterName);
																	column.ColumnNameSingularPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(columnParameter.ParameterName));
																	column.ColumnNameSingularCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(columnParameter.ParameterName));
																	column.ColumnNameSingularConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetSingularForm(columnParameter.ParameterName));
																	column.ColumnNamePluralPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(columnParameter.ParameterName));
																	column.ColumnNamePluralCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(columnParameter.ParameterName));
																	column.ColumnNamePluralConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetPluralForm(columnParameter.ParameterName));

																	column.ColumnNameSqlMetalPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(columnParameter.ParameterName);
																	column.ColumnNameSqlMetalCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(columnParameter.ParameterName);
																	column.ColumnNameSqlMetalSingularPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(columnParameter.ParameterName));
																	column.ColumnNameSqlMetalSingularCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(columnParameter.ParameterName));
																	column.ColumnNameSqlMetalPluralPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(columnParameter.ParameterName));
																	column.ColumnNameSqlMetalPluralCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(columnParameter.ParameterName));

																	clrType = this.CoreInferClrTypeForSqlType(dataSourceTag, columnParameter.ParameterSqlType, columnParameter.ParameterPrecision);
																	column.ColumnDbType = AdoNetFascade.Instance.InferDbTypeForClrType(clrType);
																	column.ColumnSize = this.CoreCalculateColumnSize(dataSourceTag, column); //recalculate

																	column.ColumnClrType = clrType ?? typeof(object);
																	column.ColumnClrNullableType = ReflectionFascade.Instance.MakeNullableType(clrType);
																	column.ColumnClrNonNullableType = ReflectionFascade.Instance.MakeNonNullableType(clrType);
																	column.ColumnCSharpNullableLiteral = column.ColumnNullable.ToString().ToLower();
																	column.ColumnCSharpDbType = string.Format("{0}.{1}", typeof(DbType).Name, column.ColumnDbType);
																	column.ColumnCSharpClrType = (object)column.ColumnClrType != null ? FormatCSharpType(column.ColumnClrType) : FormatCSharpType(typeof(object));
																	column.ColumnCSharpClrNullableType = (object)column.ColumnClrNullableType != null ? FormatCSharpType(column.ColumnClrNullableType) : FormatCSharpType(typeof(object));
																	column.ColumnCSharpClrNonNullableType = (object)column.ColumnClrNonNullableType != null ? FormatCSharpType(column.ColumnClrNonNullableType) : FormatCSharpType(typeof(object));

																	clrType = this.CoreInferSqlMetalClrTypeForClrType(dataSourceTag, column.ColumnClrType);
																	column.ColumnSqlMetalClrType = clrType;
																	column.ColumnSqlMetalClrNullableType = ReflectionFascade.Instance.MakeNullableType(clrType);
																	column.ColumnSqlMetalClrNonNullableType = ReflectionFascade.Instance.MakeNonNullableType(clrType);
																	column.ColumnSqlMetalCSharpClrType = (object)column.ColumnSqlMetalClrType != null ? FormatCSharpType(column.ColumnSqlMetalClrType) : FormatCSharpType(typeof(object));
																	column.ColumnSqlMetalCSharpClrNullableType = (object)column.ColumnSqlMetalClrNullableType != null ? FormatCSharpType(column.ColumnSqlMetalClrNullableType) : FormatCSharpType(typeof(object));
																	column.ColumnSqlMetalCSharpClrNonNullableType = (object)column.ColumnSqlMetalClrNonNullableType != null ? FormatCSharpType(column.ColumnSqlMetalClrNonNullableType) : FormatCSharpType(typeof(object));

																	//procedure.Columns.Add(column);
																	procedure.Parameters.Remove(columnParameter);
																}
															}

															if (!disableProcSchDisc)
															{
																// REFERENCE:
																// http://connect.microsoft.com/VisualStudio/feedback/details/314650/sqm1014-sqlmetal-ignores-stored-procedures-that-use-temp-tables
																IDbDataParameter[] parameters;
																parameters = procedure.Parameters.Where(p => !p.ParameterIsReturnValue && !p.ParameterIsResultColumn).Select(p => unitOfWork.CreateParameter(p.ParameterIsOutput ? ParameterDirection.Output : ParameterDirection.Input, p.ParameterDbType, p.ParameterSize, (byte)p.ParameterPrecision, (byte)p.ParameterScale, p.ParameterNullable, p.ParameterName, null)).ToArray();

																try
																{
																	var dictEnumMetadata = unitOfWork.ExecuteSchema(CommandType.StoredProcedure, string.Format(GetAllAssemblyResourceFileText(this.GetType(), dataSourceTag, "ProcedureSchema"), server.ServerName, database.DatabaseName, schema.SchemaName, procedure.ProcedureName), parameters, out recordsAffected);
																	{
																		if ((object)dictEnumMetadata != null)
																		{
																			foreach (var dictDataMetadata in dictEnumMetadata)
																			{
																				ProcedureColumn column;

																				column = new ProcedureColumn();

																				column.ColumnOrdinal = DataTypeFascade.Instance.ChangeType<int>(dictDataMetadata[SchemaTableColumn.ColumnOrdinal]);
																				column.ColumnName = DataTypeFascade.Instance.ChangeType<string>(dictDataMetadata[SchemaTableColumn.ColumnName]);
																				column.ColumnCSharpIsAnonymousLiteral = column.ColumnIsAnonymous.ToString().ToLower();
																				column.ColumnSize = DataTypeFascade.Instance.ChangeType<int>(dictDataMetadata[SchemaTableColumn.ColumnSize]);
																				column.ColumnPrecision = DataTypeFascade.Instance.ChangeType<int>(dictDataMetadata[SchemaTableColumn.NumericPrecision]);
																				column.ColumnScale = DataTypeFascade.Instance.ChangeType<int>(dictDataMetadata[SchemaTableColumn.NumericScale]);
																				column.ColumnSqlType = string.Empty;
																				//column.ColumnXXX = DataTypeFascade.Instance.ChangeType<object>(dictDataMetadata[SchemaTableColumn.ProviderType]);
																				//column.ColumnXXX = DataTypeFascade.Instance.ChangeType<object>(dictDataMetadata[SchemaTableColumn.NonVersionedProviderType]);
																				//column.ColumnXXX = DataTypeFascade.Instance.ChangeType<object>(dictDataMetadata[SchemaTableColumn.IsLong]);
																				column.ColumnNullable = DataTypeFascade.Instance.ChangeType<bool>(dictDataMetadata[SchemaTableColumn.AllowDBNull]);
																				//column.ColumnXXX = DataTypeFascade.Instance.ChangeType<object>(dictDataMetadata[SchemaTableColumn.IsAliased]);
																				//column.ColumnXXX = DataTypeFascade.Instance.ChangeType<object>(dictDataMetadata[SchemaTableColumn.IsExpression]);
																				//column.ColumnXXX = DataTypeFascade.Instance.ChangeType<object>(dictDataMetadata[SchemaTableColumn.IsKey]);
																				//column.ColumnXXX = DataTypeFascade.Instance.ChangeType<object>(dictDataMetadata[SchemaTableColumn.IsUnique]);
																				//column.ColumnXXX = DataTypeFascade.Instance.ChangeType<object>(dictDataMetadata[SchemaTableColumn.BaseSchemaName]);
																				//column.ColumnXXX = DataTypeFascade.Instance.ChangeType<object>(dictDataMetadata[SchemaTableColumn.BaseTableName]);
																				//column.ColumnXXX = DataTypeFascade.Instance.ChangeType<object>(dictDataMetadata[SchemaTableColumn.BaseColumnName]);

																				column.ColumnNamePascalCase = effectiveStandardCanonicalNaming.GetPascalCase(column.ColumnName);
																				column.ColumnNameCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(column.ColumnName);
																				column.ColumnNameConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(column.ColumnName);
																				column.ColumnNameSingularPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(column.ColumnName));
																				column.ColumnNameSingularCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(column.ColumnName));
																				column.ColumnNameSingularConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetSingularForm(column.ColumnName));
																				column.ColumnNamePluralPascalCase = effectiveStandardCanonicalNaming.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(column.ColumnName));
																				column.ColumnNamePluralCamelCase = effectiveStandardCanonicalNaming.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(column.ColumnName));
																				column.ColumnNamePluralConstantCase = effectiveStandardCanonicalNaming.GetConstantCase(effectiveStandardCanonicalNaming.GetPluralForm(column.ColumnName));

																				column.ColumnNameSqlMetalPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(column.ColumnName);
																				column.ColumnNameSqlMetalCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(column.ColumnName);
																				column.ColumnNameSqlMetalSingularPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(effectiveStandardCanonicalNaming.GetSingularForm(column.ColumnName));
																				column.ColumnNameSqlMetalSingularCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(effectiveStandardCanonicalNaming.GetSingularForm(column.ColumnName));
																				column.ColumnNameSqlMetalPluralPascalCase = SqlMetalCanonicalNaming.Instance.GetPascalCase(effectiveStandardCanonicalNaming.GetPluralForm(column.ColumnName));
																				column.ColumnNameSqlMetalPluralCamelCase = SqlMetalCanonicalNaming.Instance.GetCamelCase(effectiveStandardCanonicalNaming.GetPluralForm(column.ColumnName));

																				clrType = DataTypeFascade.Instance.ChangeType<Type>(dictDataMetadata[SchemaTableColumn.DataType]);
																				column.ColumnDbType = AdoNetFascade.Instance.InferDbTypeForClrType(clrType);
																				column.ColumnSize = this.CoreCalculateColumnSize(dataSourceTag, column); //recalculate

																				column.ColumnClrType = clrType ?? typeof(object);
																				column.ColumnClrNullableType = ReflectionFascade.Instance.MakeNullableType(clrType);
																				column.ColumnClrNonNullableType = ReflectionFascade.Instance.MakeNonNullableType(clrType);
																				column.ColumnCSharpNullableLiteral = column.ColumnNullable.ToString().ToLower();
																				column.ColumnCSharpDbType = string.Format("{0}.{1}", typeof(DbType).Name, column.ColumnDbType);
																				column.ColumnCSharpClrType = (object)column.ColumnClrType != null ? FormatCSharpType(column.ColumnClrType) : FormatCSharpType(typeof(object));
																				column.ColumnCSharpClrNullableType = (object)column.ColumnClrNullableType != null ? FormatCSharpType(column.ColumnClrNullableType) : FormatCSharpType(typeof(object));
																				column.ColumnCSharpClrNonNullableType = (object)column.ColumnClrNonNullableType != null ? FormatCSharpType(column.ColumnClrNonNullableType) : FormatCSharpType(typeof(object));

																				clrType = this.CoreInferSqlMetalClrTypeForClrType(dataSourceTag, column.ColumnClrType);
																				column.ColumnSqlMetalClrType = clrType;
																				column.ColumnSqlMetalClrNullableType = ReflectionFascade.Instance.MakeNullableType(clrType);
																				column.ColumnSqlMetalClrNonNullableType = ReflectionFascade.Instance.MakeNonNullableType(clrType);
																				column.ColumnSqlMetalCSharpClrType = (object)column.ColumnSqlMetalClrType != null ? FormatCSharpType(column.ColumnSqlMetalClrType) : FormatCSharpType(typeof(object));
																				column.ColumnSqlMetalCSharpClrNullableType = (object)column.ColumnSqlMetalClrNullableType != null ? FormatCSharpType(column.ColumnSqlMetalClrNullableType) : FormatCSharpType(typeof(object));
																				column.ColumnSqlMetalCSharpClrNonNullableType = (object)column.ColumnSqlMetalClrNonNullableType != null ? FormatCSharpType(column.ColumnSqlMetalClrNonNullableType) : FormatCSharpType(typeof(object));

																				procedure.Columns.Add(column);
																			}
																		}
																	}
																}
																catch (Exception ex)
																{
																	procedure.ProcedureExecuteSchemaThrewException = true;
																	procedure.ProcedureExecuteSchemaExceptionText = ReflectionFascade.Instance.GetErrors(ex, 0);
																	//Console.Error.WriteLine(ReflectionFascade.Instance.GetErrors(ex, 0));
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