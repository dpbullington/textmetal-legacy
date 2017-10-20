/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Xml;

using TextMetal.Middleware.Solder.Extensions;
using TextMetal.Middleware.Solder.Utilities;

namespace TextMetal.Framework.Source.DatabaseSchema.Sql
{
	public class SqlSchemaSourceStrategy : SchemaSourceStrategy
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the SqlSchemaSourceStrategy class.
		/// </summary>
		public SqlSchemaSourceStrategy()
		{
		}

		#endregion

		#region Fields/Constants

		private const string NET_SQL_SERVER_DATA_SOURCE_TAG = "net.sqlserver";

		#endregion

		#region Methods/Operators

		protected override int CoreCalculateColumnSize(string dataSourceTag, Column column)
		{
			if ((object)dataSourceTag == null)
				throw new ArgumentNullException(nameof(dataSourceTag));

			if ((object)column == null)
				throw new ArgumentNullException(nameof(column));

			if (dataSourceTag.SafeToString().ToLower() == NET_SQL_SERVER_DATA_SOURCE_TAG)
			{
				return column.ColumnSqlType == "image" ||
						column.ColumnSqlType == "text" ||
						column.ColumnSqlType == "ntext" ? (int)0 :
				(column.ColumnDbType == DbType.String &&
				column.ColumnSqlType.SafeToString().StartsWith("n") &&
				column.ColumnSize != 0 ?
					(int)(column.ColumnSize / 2) :
					column.ColumnSize);
			}

			throw new ArgumentOutOfRangeException(string.Format("dataSourceTag: '{0}'", dataSourceTag));
		}

		protected override int CoreCalculateParameterSize(string dataSourceTag, Parameter parameter)
		{
			if ((object)dataSourceTag == null)
				throw new ArgumentNullException(nameof(dataSourceTag));

			if ((object)parameter == null)
				throw new ArgumentNullException(nameof(parameter));

			if (dataSourceTag.SafeToString().ToLower() == NET_SQL_SERVER_DATA_SOURCE_TAG)
			{
				return parameter.ParameterSqlType == "image" ||
						parameter.ParameterSqlType == "text" ||
						parameter.ParameterSqlType == "ntext" ? (int)0 :
				(parameter.ParameterDbType == DbType.String &&
				parameter.ParameterSqlType.SafeToString().StartsWith("n") &&
				parameter.ParameterSize != 0 ?
					(int)(parameter.ParameterSize / 2) :
					parameter.ParameterSize);
			}
			throw new ArgumentOutOfRangeException(string.Format("dataSourceTag: '{0}'", dataSourceTag));
		}

		protected override IEnumerable<DbParameter> CoreGetColumnParameters(Type connectionType, string dataSourceTag, Server server, Database database, Schema schema, Table table)
		{
			if ((object)connectionType == null)
				throw new ArgumentNullException(nameof(connectionType));

			if ((object)dataSourceTag == null)
				throw new ArgumentNullException(nameof(dataSourceTag));

			if ((object)server == null)
				throw new ArgumentNullException(nameof(server));

			if ((object)database == null)
				throw new ArgumentNullException(nameof(database));

			if ((object)schema == null)
				throw new ArgumentNullException(nameof(schema));

			if ((object)table == null)
				throw new ArgumentNullException(nameof(table));

			if (dataSourceTag.SafeToString().ToLower() == NET_SQL_SERVER_DATA_SOURCE_TAG)
			{
				return new DbParameter[]
						{
							AdoNetBufferingFascade.LegacyInstanceAccessor.AdoNetBufferingLegacyInstance.CreateParameter(connectionType, null, ParameterDirection.Input, DbType.String, 100, 0, 0, true, "@ServerName", server.ServerName),
							AdoNetBufferingFascade.LegacyInstanceAccessor.AdoNetBufferingLegacyInstance.CreateParameter(connectionType, null, ParameterDirection.Input, DbType.String, 100, 0, 0, true, "@DatabaseName", database.DatabaseName),
							AdoNetBufferingFascade.LegacyInstanceAccessor.AdoNetBufferingLegacyInstance.CreateParameter(connectionType, null, ParameterDirection.Input, DbType.String, 100, 0, 0, true, "@SchemaName", schema.SchemaName),
							AdoNetBufferingFascade.LegacyInstanceAccessor.AdoNetBufferingLegacyInstance.CreateParameter(connectionType, null, ParameterDirection.Input, DbType.String, 100, 0, 0, true, "@TableName", table.TableName)
						};
			}

			throw new ArgumentOutOfRangeException(string.Format("dataSourceTag: '{0}'", dataSourceTag));
		}

		protected override IEnumerable<DbParameter> CoreGetColumnParameters(Type connectionType, string dataSourceTag, Server server, Database database, Schema schema, View view)
		{
			if ((object)connectionType == null)
				throw new ArgumentNullException(nameof(connectionType));

			if ((object)dataSourceTag == null)
				throw new ArgumentNullException(nameof(dataSourceTag));

			if ((object)server == null)
				throw new ArgumentNullException(nameof(server));

			if ((object)database == null)
				throw new ArgumentNullException(nameof(database));

			if ((object)schema == null)
				throw new ArgumentNullException(nameof(schema));

			if ((object)view == null)
				throw new ArgumentNullException(nameof(view));

			if (dataSourceTag.SafeToString().ToLower() == NET_SQL_SERVER_DATA_SOURCE_TAG)
			{
				return new DbParameter[]
						{
							AdoNetBufferingFascade.LegacyInstanceAccessor.AdoNetBufferingLegacyInstance.CreateParameter(connectionType, null, ParameterDirection.Input, DbType.String, 100, 0, 0, true, "@ServerName", server.ServerName),
							AdoNetBufferingFascade.LegacyInstanceAccessor.AdoNetBufferingLegacyInstance.CreateParameter(connectionType, null, ParameterDirection.Input, DbType.String, 100, 0, 0, true, "@DatabaseName", database.DatabaseName),
							AdoNetBufferingFascade.LegacyInstanceAccessor.AdoNetBufferingLegacyInstance.CreateParameter(connectionType, null, ParameterDirection.Input, DbType.String, 100, 0, 0, true, "@SchemaName", schema.SchemaName),
							AdoNetBufferingFascade.LegacyInstanceAccessor.AdoNetBufferingLegacyInstance.CreateParameter(connectionType, null, ParameterDirection.Input, DbType.String, 100, 0, 0, true, "@ViewName", view.ViewName)
						};
			}

			throw new ArgumentOutOfRangeException(string.Format("dataSourceTag: '{0}'", dataSourceTag));
		}

		protected override IEnumerable<DbParameter> CoreGetDatabaseParameters(Type connectionType, string dataSourceTag, Server server)
		{
			if ((object)connectionType == null)
				throw new ArgumentNullException(nameof(connectionType));

			if ((object)dataSourceTag == null)
				throw new ArgumentNullException(nameof(dataSourceTag));

			if ((object)server == null)
				throw new ArgumentNullException(nameof(server));

			if (dataSourceTag.SafeToString().ToLower() == NET_SQL_SERVER_DATA_SOURCE_TAG)
			{
				return new DbParameter[]
						{
							AdoNetBufferingFascade.LegacyInstanceAccessor.AdoNetBufferingLegacyInstance.CreateParameter(connectionType, null, ParameterDirection.Input, DbType.String, 100, 0, 0, true, "@ServerName", server.ServerName)
						};
			}

			throw new ArgumentOutOfRangeException(string.Format("dataSourceTag: '{0}'", dataSourceTag));
		}

		protected override IEnumerable<DbParameter> CoreGetDdlTriggerParameters(Type connectionType, string dataSourceTag, Server server, Database database)
		{
			if ((object)connectionType == null)
				throw new ArgumentNullException(nameof(connectionType));

			if ((object)dataSourceTag == null)
				throw new ArgumentNullException(nameof(dataSourceTag));

			if (dataSourceTag.SafeToString().ToLower() == NET_SQL_SERVER_DATA_SOURCE_TAG)
			{
				return new DbParameter[]
						{
							AdoNetBufferingFascade.LegacyInstanceAccessor.AdoNetBufferingLegacyInstance.CreateParameter(connectionType, null, ParameterDirection.Input, DbType.String, 100, 0, 0, true, "@ServerName", server.ServerName),
							AdoNetBufferingFascade.LegacyInstanceAccessor.AdoNetBufferingLegacyInstance.CreateParameter(connectionType, null, ParameterDirection.Input, DbType.String, 100, 0, 0, true, "@DatabaseName", database.DatabaseName)
						};
			}

			throw new ArgumentOutOfRangeException(string.Format("dataSourceTag: '{0}'", dataSourceTag));
		}

		protected override IEnumerable<DbParameter> CoreGetDmlTriggerParameters(Type connectionType, string dataSourceTag, Server server, Database database, Schema schema, Table table)
		{
			if ((object)connectionType == null)
				throw new ArgumentNullException(nameof(connectionType));

			if ((object)dataSourceTag == null)
				throw new ArgumentNullException(nameof(dataSourceTag));

			if ((object)server == null)
				throw new ArgumentNullException(nameof(server));

			if ((object)database == null)
				throw new ArgumentNullException(nameof(database));

			if ((object)schema == null)
				throw new ArgumentNullException(nameof(schema));

			if ((object)table == null)
				throw new ArgumentNullException(nameof(table));

			if (dataSourceTag.SafeToString().ToLower() == NET_SQL_SERVER_DATA_SOURCE_TAG)
			{
				return new DbParameter[]
						{
							AdoNetBufferingFascade.LegacyInstanceAccessor.AdoNetBufferingLegacyInstance.CreateParameter(connectionType, null, ParameterDirection.Input, DbType.String, 100, 0, 0, true, "@ServerName", server.ServerName),
							AdoNetBufferingFascade.LegacyInstanceAccessor.AdoNetBufferingLegacyInstance.CreateParameter(connectionType, null, ParameterDirection.Input, DbType.String, 100, 0, 0, true, "@DatabaseName", database.DatabaseName),
							AdoNetBufferingFascade.LegacyInstanceAccessor.AdoNetBufferingLegacyInstance.CreateParameter(connectionType, null, ParameterDirection.Input, DbType.String, 100, 0, 0, true, "@SchemaName", schema.SchemaName),
							AdoNetBufferingFascade.LegacyInstanceAccessor.AdoNetBufferingLegacyInstance.CreateParameter(connectionType, null, ParameterDirection.Input, DbType.String, 100, 0, 0, true, "@TableName", table.TableName)
						};
			}

			throw new ArgumentOutOfRangeException(string.Format("dataSourceTag: '{0}'", dataSourceTag));
		}

		protected override bool CoreGetEmitImplicitReturnParameter(string dataSourceTag)
		{
			if ((object)dataSourceTag == null)
				throw new ArgumentNullException(nameof(dataSourceTag));

			if (dataSourceTag.SafeToString().ToLower() == NET_SQL_SERVER_DATA_SOURCE_TAG)
				return true;

			throw new ArgumentOutOfRangeException(string.Format("dataSourceTag: '{0}'", dataSourceTag));
		}

		protected override IEnumerable<DbParameter> CoreGetForeignKeyColumnParameters(Type connectionType, string dataSourceTag, Server server, Database database, Schema schema, Table table, ForeignKey foreignKey)
		{
			if ((object)connectionType == null)
				throw new ArgumentNullException(nameof(connectionType));

			if ((object)dataSourceTag == null)
				throw new ArgumentNullException(nameof(dataSourceTag));

			if ((object)server == null)
				throw new ArgumentNullException(nameof(server));

			if ((object)database == null)
				throw new ArgumentNullException(nameof(database));

			if ((object)schema == null)
				throw new ArgumentNullException(nameof(schema));

			if ((object)table == null)
				throw new ArgumentNullException(nameof(table));

			if ((object)foreignKey == null)
				throw new ArgumentNullException(nameof(foreignKey));

			if (dataSourceTag.SafeToString().ToLower() == NET_SQL_SERVER_DATA_SOURCE_TAG)
			{
				return new DbParameter[]
						{
							AdoNetBufferingFascade.LegacyInstanceAccessor.AdoNetBufferingLegacyInstance.CreateParameter(connectionType, null, ParameterDirection.Input, DbType.String, 100, 0, 0, true, "@ServerName", server.ServerName),
							AdoNetBufferingFascade.LegacyInstanceAccessor.AdoNetBufferingLegacyInstance.CreateParameter(connectionType, null, ParameterDirection.Input, DbType.String, 100, 0, 0, true, "@DatabaseName", database.DatabaseName),
							AdoNetBufferingFascade.LegacyInstanceAccessor.AdoNetBufferingLegacyInstance.CreateParameter(connectionType, null, ParameterDirection.Input, DbType.String, 100, 0, 0, true, "@SchemaName", schema.SchemaName),
							AdoNetBufferingFascade.LegacyInstanceAccessor.AdoNetBufferingLegacyInstance.CreateParameter(connectionType, null, ParameterDirection.Input, DbType.String, 100, 0, 0, true, "@TableName", table.TableName),
							AdoNetBufferingFascade.LegacyInstanceAccessor.AdoNetBufferingLegacyInstance.CreateParameter(connectionType, null, ParameterDirection.Input, DbType.String, 100, 0, 0, true, "@ForeignKeyName", foreignKey.ForeignKeyName)
						};
			}

			throw new ArgumentOutOfRangeException(string.Format("dataSourceTag: '{0}'", dataSourceTag));
		}

		protected override IEnumerable<DbParameter> CoreGetForeignKeyParameters(Type connectionType, string dataSourceTag, Server server, Database database, Schema schema, Table table)
		{
			if ((object)connectionType == null)
				throw new ArgumentNullException(nameof(connectionType));

			if ((object)dataSourceTag == null)
				throw new ArgumentNullException(nameof(dataSourceTag));

			if ((object)server == null)
				throw new ArgumentNullException(nameof(server));

			if ((object)database == null)
				throw new ArgumentNullException(nameof(database));

			if ((object)schema == null)
				throw new ArgumentNullException(nameof(schema));

			if ((object)table == null)
				throw new ArgumentNullException(nameof(table));

			if (dataSourceTag.SafeToString().ToLower() == NET_SQL_SERVER_DATA_SOURCE_TAG)
			{
				return new DbParameter[]
						{
							AdoNetBufferingFascade.LegacyInstanceAccessor.AdoNetBufferingLegacyInstance.CreateParameter(connectionType, null, ParameterDirection.Input, DbType.String, 100, 0, 0, true, "@ServerName", server.ServerName),
							AdoNetBufferingFascade.LegacyInstanceAccessor.AdoNetBufferingLegacyInstance.CreateParameter(connectionType, null, ParameterDirection.Input, DbType.String, 100, 0, 0, true, "@DatabaseName", database.DatabaseName),
							AdoNetBufferingFascade.LegacyInstanceAccessor.AdoNetBufferingLegacyInstance.CreateParameter(connectionType, null, ParameterDirection.Input, DbType.String, 100, 0, 0, true, "@SchemaName", schema.SchemaName),
							AdoNetBufferingFascade.LegacyInstanceAccessor.AdoNetBufferingLegacyInstance.CreateParameter(connectionType, null, ParameterDirection.Input, DbType.String, 100, 0, 0, true, "@TableName", table.TableName)
						};
			}

			throw new ArgumentOutOfRangeException(string.Format("dataSourceTag: '{0}'", dataSourceTag));
		}

		protected override IEnumerable<DbParameter> CoreGetParameterParameters(Type connectionType, string dataSourceTag, Server server, Database database, Schema schema, Procedure procedure)
		{
			if ((object)connectionType == null)
				throw new ArgumentNullException(nameof(connectionType));

			if ((object)dataSourceTag == null)
				throw new ArgumentNullException(nameof(dataSourceTag));

			if ((object)server == null)
				throw new ArgumentNullException(nameof(server));

			if ((object)database == null)
				throw new ArgumentNullException(nameof(database));

			if ((object)schema == null)
				throw new ArgumentNullException(nameof(schema));

			if ((object)procedure == null)
				throw new ArgumentNullException(nameof(procedure));

			if (dataSourceTag.SafeToString().ToLower() == NET_SQL_SERVER_DATA_SOURCE_TAG)
			{
				return new DbParameter[]
						{
							AdoNetBufferingFascade.LegacyInstanceAccessor.AdoNetBufferingLegacyInstance.CreateParameter(connectionType, null, ParameterDirection.Input, DbType.String, 100, 0, 0, true, "@ServerName", server.ServerName),
							AdoNetBufferingFascade.LegacyInstanceAccessor.AdoNetBufferingLegacyInstance.CreateParameter(connectionType, null, ParameterDirection.Input, DbType.String, 100, 0, 0, true, "@DatabaseName", database.DatabaseName),
							AdoNetBufferingFascade.LegacyInstanceAccessor.AdoNetBufferingLegacyInstance.CreateParameter(connectionType, null, ParameterDirection.Input, DbType.String, 100, 0, 0, true, "@SchemaName", schema.SchemaName),
							AdoNetBufferingFascade.LegacyInstanceAccessor.AdoNetBufferingLegacyInstance.CreateParameter(connectionType, null, ParameterDirection.Input, DbType.String, 100, 0, 0, true, "@ProcedureName", procedure.ProcedureName)
						};
			}

			throw new ArgumentOutOfRangeException(string.Format("dataSourceTag: '{0}'", dataSourceTag));
		}

		protected override string CoreGetParameterPrefix(string dataSourceTag)
		{
			if ((object)dataSourceTag == null)
				throw new ArgumentNullException(nameof(dataSourceTag));

			if (dataSourceTag.SafeToString().ToLower() == NET_SQL_SERVER_DATA_SOURCE_TAG)
				return "@";

			throw new ArgumentOutOfRangeException(string.Format("dataSourceTag: '{0}'", dataSourceTag));
		}

		protected override IEnumerable<DbParameter> CoreGetProcedureParameters(Type connectionType, string dataSourceTag, Server server, Database database, Schema schema)
		{
			if ((object)connectionType == null)
				throw new ArgumentNullException(nameof(connectionType));

			if ((object)dataSourceTag == null)
				throw new ArgumentNullException(nameof(dataSourceTag));

			if ((object)server == null)
				throw new ArgumentNullException(nameof(server));

			if ((object)database == null)
				throw new ArgumentNullException(nameof(database));

			if ((object)schema == null)
				throw new ArgumentNullException(nameof(schema));

			if (dataSourceTag.SafeToString().ToLower() == NET_SQL_SERVER_DATA_SOURCE_TAG)
			{
				return new DbParameter[]
						{
							AdoNetBufferingFascade.LegacyInstanceAccessor.AdoNetBufferingLegacyInstance.CreateParameter(connectionType, null, ParameterDirection.Input, DbType.String, 100, 0, 0, true, "@ServerName", server.ServerName),
							AdoNetBufferingFascade.LegacyInstanceAccessor.AdoNetBufferingLegacyInstance.CreateParameter(connectionType, null, ParameterDirection.Input, DbType.String, 100, 0, 0, true, "@DatabaseName", database.DatabaseName),
							AdoNetBufferingFascade.LegacyInstanceAccessor.AdoNetBufferingLegacyInstance.CreateParameter(connectionType, null, ParameterDirection.Input, DbType.String, 100, 0, 0, true, "@SchemaName", schema.SchemaName)
						};
			}

			throw new ArgumentOutOfRangeException(string.Format("dataSourceTag: '{0}'", dataSourceTag));
		}

		protected override IEnumerable<DbParameter> CoreGetSchemaParameters(Type connectionType, string dataSourceTag, Server server, Database database)
		{
			if ((object)server == null)
				throw new ArgumentNullException(nameof(server));

			if ((object)database == null)
				throw new ArgumentNullException(nameof(database));

			if (dataSourceTag.SafeToString().ToLower() == NET_SQL_SERVER_DATA_SOURCE_TAG)
			{
				return new DbParameter[]
						{
							AdoNetBufferingFascade.LegacyInstanceAccessor.AdoNetBufferingLegacyInstance.CreateParameter(connectionType, null, ParameterDirection.Input, DbType.String, 100, 0, 0, true, "@ServerName", server.ServerName),
							AdoNetBufferingFascade.LegacyInstanceAccessor.AdoNetBufferingLegacyInstance.CreateParameter(connectionType, null, ParameterDirection.Input, DbType.String, 100, 0, 0, true, "@DatabaseName", database.DatabaseName)
						};
			}

			throw new ArgumentOutOfRangeException(string.Format("dataSourceTag: '{0}'", dataSourceTag));
		}

		protected override IEnumerable<DbParameter> CoreGetServerParameters(Type connectionType, string dataSourceTag)
		{
			if ((object)connectionType == null)
				throw new ArgumentNullException(nameof(connectionType));

			if ((object)dataSourceTag == null)
				throw new ArgumentNullException(nameof(dataSourceTag));

			if (dataSourceTag.SafeToString().ToLower() == NET_SQL_SERVER_DATA_SOURCE_TAG)
				return null;

			throw new ArgumentOutOfRangeException(string.Format("dataSourceTag: '{0}'", dataSourceTag));
		}

		protected override IEnumerable<DbParameter> CoreGetTableParameters(Type connectionType, string dataSourceTag, Server server, Database database, Schema schema)
		{
			if ((object)connectionType == null)
				throw new ArgumentNullException(nameof(connectionType));

			if ((object)dataSourceTag == null)
				throw new ArgumentNullException(nameof(dataSourceTag));

			if ((object)server == null)
				throw new ArgumentNullException(nameof(server));

			if ((object)database == null)
				throw new ArgumentNullException(nameof(database));

			if ((object)schema == null)
				throw new ArgumentNullException(nameof(schema));

			if (dataSourceTag.SafeToString().ToLower() == NET_SQL_SERVER_DATA_SOURCE_TAG)
			{
				return new DbParameter[]
						{
							AdoNetBufferingFascade.LegacyInstanceAccessor.AdoNetBufferingLegacyInstance.CreateParameter(connectionType, null, ParameterDirection.Input, DbType.String, 100, 0, 0, true, "@ServerName", server.ServerName),
							AdoNetBufferingFascade.LegacyInstanceAccessor.AdoNetBufferingLegacyInstance.CreateParameter(connectionType, null, ParameterDirection.Input, DbType.String, 100, 0, 0, true, "@DatabaseName", database.DatabaseName),
							AdoNetBufferingFascade.LegacyInstanceAccessor.AdoNetBufferingLegacyInstance.CreateParameter(connectionType, null, ParameterDirection.Input, DbType.String, 100, 0, 0, true, "@SchemaName", schema.SchemaName)
						};
			}

			throw new ArgumentOutOfRangeException(string.Format("dataSourceTag: '{0}'", dataSourceTag));
		}

		protected override IEnumerable<DbParameter> CoreGetUniqueKeyColumnParameters(Type connectionType, string dataSourceTag, Server server, Database database, Schema schema, Table table, UniqueKey uniqueKey)
		{
			if ((object)connectionType == null)
				throw new ArgumentNullException(nameof(connectionType));

			if ((object)dataSourceTag == null)
				throw new ArgumentNullException(nameof(dataSourceTag));

			if ((object)server == null)
				throw new ArgumentNullException(nameof(server));

			if ((object)database == null)
				throw new ArgumentNullException(nameof(database));

			if ((object)schema == null)
				throw new ArgumentNullException(nameof(schema));

			if ((object)table == null)
				throw new ArgumentNullException(nameof(table));

			if ((object)uniqueKey == null)
				throw new ArgumentNullException(nameof(uniqueKey));

			if (dataSourceTag.SafeToString().ToLower() == NET_SQL_SERVER_DATA_SOURCE_TAG)
			{
				return new DbParameter[]
						{
							AdoNetBufferingFascade.LegacyInstanceAccessor.AdoNetBufferingLegacyInstance.CreateParameter(connectionType, null, ParameterDirection.Input, DbType.String, 100, 0, 0, true, "@ServerName", server.ServerName),
							AdoNetBufferingFascade.LegacyInstanceAccessor.AdoNetBufferingLegacyInstance.CreateParameter(connectionType, null, ParameterDirection.Input, DbType.String, 100, 0, 0, true, "@DatabaseName", database.DatabaseName),
							AdoNetBufferingFascade.LegacyInstanceAccessor.AdoNetBufferingLegacyInstance.CreateParameter(connectionType, null, ParameterDirection.Input, DbType.String, 100, 0, 0, true, "@SchemaName", schema.SchemaName),
							AdoNetBufferingFascade.LegacyInstanceAccessor.AdoNetBufferingLegacyInstance.CreateParameter(connectionType, null, ParameterDirection.Input, DbType.String, 100, 0, 0, true, "@TableName", table.TableName),
							AdoNetBufferingFascade.LegacyInstanceAccessor.AdoNetBufferingLegacyInstance.CreateParameter(connectionType, null, ParameterDirection.Input, DbType.String, 100, 0, 0, true, "@UniqueKeyName", uniqueKey.UniqueKeyName)
						};
			}

			throw new ArgumentOutOfRangeException(string.Format("dataSourceTag: '{0}'", dataSourceTag));
		}

		protected override IEnumerable<DbParameter> CoreGetUniqueKeyParameters(Type connectionType, string dataSourceTag, Server server, Database database, Schema schema, Table table)
		{
			if ((object)connectionType == null)
				throw new ArgumentNullException(nameof(connectionType));

			if ((object)dataSourceTag == null)
				throw new ArgumentNullException(nameof(dataSourceTag));

			if ((object)server == null)
				throw new ArgumentNullException(nameof(server));

			if ((object)database == null)
				throw new ArgumentNullException(nameof(database));

			if ((object)schema == null)
				throw new ArgumentNullException(nameof(schema));

			if ((object)table == null)
				throw new ArgumentNullException(nameof(table));

			if (dataSourceTag.SafeToString().ToLower() == NET_SQL_SERVER_DATA_SOURCE_TAG)
			{
				return new DbParameter[]
						{
							AdoNetBufferingFascade.LegacyInstanceAccessor.AdoNetBufferingLegacyInstance.CreateParameter(connectionType, null, ParameterDirection.Input, DbType.String, 100, 0, 0, true, "@ServerName", server.ServerName),
							AdoNetBufferingFascade.LegacyInstanceAccessor.AdoNetBufferingLegacyInstance.CreateParameter(connectionType, null, ParameterDirection.Input, DbType.String, 100, 0, 0, true, "@DatabaseName", database.DatabaseName),
							AdoNetBufferingFascade.LegacyInstanceAccessor.AdoNetBufferingLegacyInstance.CreateParameter(connectionType, null, ParameterDirection.Input, DbType.String, 100, 0, 0, true, "@SchemaName", schema.SchemaName),
							AdoNetBufferingFascade.LegacyInstanceAccessor.AdoNetBufferingLegacyInstance.CreateParameter(connectionType, null, ParameterDirection.Input, DbType.String, 100, 0, 0, true, "@TableName", table.TableName)
						};
			}

			throw new ArgumentOutOfRangeException(string.Format("dataSourceTag: '{0}'", dataSourceTag));
		}

		protected override Type CoreInferClrTypeForSqlType(string dataSourceTag, string sqlType, int sqlPrecision)
		{
			if ((object)dataSourceTag == null)
				throw new ArgumentNullException(nameof(dataSourceTag));

			if (dataSourceTag.SafeToString().ToLower() == NET_SQL_SERVER_DATA_SOURCE_TAG)
			{
				switch (sqlType = sqlType.SafeToString().ToUpper())
				{
					case "BIGINT":
						return typeof(Int64);
					case "BINARY":
						return typeof(Byte[]);
					case "BIT":
						return typeof(Boolean);
					case "CHARACTER":
					case "CHAR":
						return typeof(String);
					case "CURSOR":
						return typeof(Object);
					case "DATE":
						return typeof(DateTime);
					case "DATETIME":
						return typeof(DateTime);
					case "DATETIME2":
						return typeof(DateTime);
					case "DATETIMEOFFSET":
						return typeof(DateTimeOffset);
					case "DEC":
					case "DECIMAL":
						return typeof(Decimal);
					case "DOUBLE PRECISION":
					case "FLOAT":
						return sqlPrecision >= 0 && sqlPrecision <= 24 ? typeof(Single) : (sqlPrecision >= 25 && sqlPrecision <= 53 ? typeof(Double) : typeof(Object));
					case "GEOGRAPHY":
						return typeof(Object);
					case "GEOMETRY":
						return typeof(Object);
					case "HIERARCHYID":
						return typeof(Object);
					case "IMAGE":
						return typeof(Byte[]);
					case "INTEGER":
					case "INT":
						return typeof(Int32);
					case "MONEY":
						return typeof(Decimal);
					case "NATIONAL CHARACTER":
					case "NATIONAL CHAR":
					case "NCHAR":
						return typeof(String);
					case "NATIONAL TEXT":
					case "NTEXT":
						return typeof(String);
					case "NUMERIC":
						return typeof(Decimal);
					case "NATIONAL CHARACTER VARYING":
					case "NATIONAL CHAR VARYING":
					case "NVARCHAR":
						return typeof(String);
					case "REAL":
						return typeof(Single);
					case "TIMESTAMP":
					case "ROWVERSION":
						return typeof(Byte[]);
					case "SMALLDATETIME":
						return typeof(DateTime);
					case "SMALLINT":
						return typeof(Int16);
					case "SMALLMONEY":
						return typeof(Decimal);
					case "SQL_VARIANT":
						return typeof(Object);
					case "SYSNAME":
						return typeof(String);
					case "TABLE":
						return typeof(Object);
					case "TEXT":
						return typeof(String);
					case "TIME":
						return typeof(TimeSpan);
					case "TINYINT":
						return typeof(Byte);
					case "UNIQUEIDENTIFIER":
						return typeof(Guid);
					case "BINARY VARYING":
					case "VARBINARY":
						return typeof(Byte[]);
					case "CHARACTER VARYING":
					case "CHAR VARYING":
					case "VARCHAR":
						return typeof(String);
					case "XML":
						return typeof(XmlDocument);
					default:
						return typeof(Object); // dpb: 2014-05-13/changed behavior here to not throw exception
				}
			}

			throw new ArgumentOutOfRangeException(string.Format("dataSourceTag: '{0}'", dataSourceTag));
		}

		#endregion
	}
}