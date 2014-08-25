/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Data;

using TextMetal.Common.Core;
using TextMetal.Common.Data.Framework.Mapping;

namespace TextMetal.Common.Data.Framework.Strategy
{
	public sealed class NetSqlServerDataSourceTagStrategy : DataSourceTagStrategy
	{
		#region Constructors/Destructors

		private NetSqlServerDataSourceTagStrategy() :
			base(NET_SQL_SERVER_DATA_SOURCE_TAG, false)
		{
		}

		#endregion

		#region Fields/Constants

		private const string NET_SQL_SERVER_COLUMN_ALIASED_FORMAT = "{0}.[{1}]";
		private const string NET_SQL_SERVER_COLUMN_NAME_FORMAT = "[{0}]";
		private const string NET_SQL_SERVER_DATA_SOURCE_TAG = "net.sqlserver";
		private const string NET_SQL_SERVER_IDENTITY_COMMAND = "@@IDENTITY"; // warning: 'SELECT SCOPE_IDENTITY() AS PK' should be used in the SAME BATCH if there is any chance of triggers on any tables causing identity creation
		private const string NET_SQL_SERVER_PARAMETER_NAME_FORMAT = "@{0}";
		private const int NET_SQL_SERVER_PERSIST_NOT_EXPECTED_RECORDS_AFFECTED = 0;
		private const int NET_SQL_SERVER_QUERY_EXPECTED_RECORDS_AFFECTED = -1;
		private const string NET_SQL_SERVER_SCHEMA_TABLE_NAME_FORMAT = "[{0}].[{1}]";
		private const string NET_SQL_SERVER_TABLE_ALIAS_FORMAT = "{0}";
		private const string NET_SQL_SERVER_TABLE_NAME_FORMAT = "[{0}]";

		private static readonly NetSqlServerDataSourceTagStrategy instance = new NetSqlServerDataSourceTagStrategy();

		#endregion

		#region Properties/Indexers/Events

		public static NetSqlServerDataSourceTagStrategy Instance
		{
			get
			{
				return instance;
			}
		}

		public string DataSourceTag
		{
			get
			{
				return NET_SQL_SERVER_DATA_SOURCE_TAG;
			}
		}

		#endregion

		#region Methods/Operators

		protected override int GetExpectedRecordsAffected(bool isNullipotent)
		{
			if (!isNullipotent)
				return NET_SQL_SERVER_PERSIST_NOT_EXPECTED_RECORDS_AFFECTED;
			else
				return NET_SQL_SERVER_QUERY_EXPECTED_RECORDS_AFFECTED;
		}

		public bool CreateNativeDatabaseFile(string databaseFilePath)
		{
			return false;
		}

		protected override string GetAliasedColumnName(string tableAlias, string columnName)
		{
			string retVal;

			retVal = string.Format(NET_SQL_SERVER_COLUMN_ALIASED_FORMAT, this.GetTableAlias(tableAlias), columnName);

			return retVal;
		}

		protected override string GetColumnName(string columnName)
		{
			string retVal;

			retVal = string.Format(NET_SQL_SERVER_COLUMN_NAME_FORMAT, columnName);

			return retVal;
		}

		protected override string GetIdentityCommand()
		{
			string retVal;

			retVal = NET_SQL_SERVER_IDENTITY_COMMAND;

			return retVal;
		}

		protected override string GetParameterName(string parameterName)
		{
			string retVal;

			retVal = string.Format(NET_SQL_SERVER_PARAMETER_NAME_FORMAT, parameterName);

			return retVal;
		}

		protected override string GetTableAlias(string tableAlias)
		{
			string retVal;

			retVal = string.Format(NET_SQL_SERVER_TABLE_ALIAS_FORMAT, tableAlias);

			return retVal;
		}

		protected override string GetTableName(string schemaName, string tableName)
		{
			string retVal;

			retVal = !DataType.IsNullOrWhiteSpace(schemaName) ?
				string.Format(NET_SQL_SERVER_SCHEMA_TABLE_NAME_FORMAT, schemaName, tableName) :
				string.Format(NET_SQL_SERVER_TABLE_NAME_FORMAT, tableName);

			return retVal;
		}

		public void ParameterMagic(IUnitOfWork unitOfWork, IDataParameter commandParameter, string generatedFromColumnNativeType)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)commandParameter == null)
				throw new ArgumentNullException("commandParameter");

			if (generatedFromColumnNativeType.SafeToString().ToUpper() == "NTEXT")
				Reflexion.SetLogicalPropertyValue(commandParameter, "SqlDbType", Enum.Parse(Type.GetType("System.Data.SqlDbType, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", true), "NText", true));
			else if (generatedFromColumnNativeType.SafeToString().ToUpper() == "TEXT")
				Reflexion.SetLogicalPropertyValue(commandParameter, "SqlDbType", Enum.Parse(Type.GetType("System.Data.SqlDbType, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", true), "Text", true));
			else if (generatedFromColumnNativeType.SafeToString().ToUpper() == "IMAGE")
				Reflexion.SetLogicalPropertyValue(commandParameter, "SqlDbType", Enum.Parse(Type.GetType("System.Data.SqlDbType, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", true), "Image", true));
		}

		#endregion
	}
}