/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Data;
using System.Reflection;

using TextMetal.Common.Core;

namespace TextMetal.Common.Data.Framework.Strategy
{
	public sealed class NetSqliteDataSourceTagStrategy : DataSourceTagStrategy
	{
		#region Constructors/Destructors

		private NetSqliteDataSourceTagStrategy() : 
			base(NET_SQLITE_DATA_SOURCE_TAG, true)
		{
		}

		#endregion

		#region Fields/Constants

		private const string NET_SQLITE_COLUMN_ALIASED_FORMAT = "{0}.{1}";
		private const string NET_SQLITE_COLUMN_NAME_FORMAT = "{0}";
		private const string NET_SQLITE_DATA_SOURCE_TAG = "net.sqlite";
		private const string NET_SQLITE_IDENTITY_COMMAND = "LAST_INSERT_ROWID()";
		private const string NET_SQLITE_PARAMETER_NAME_FORMAT = "@{0}";
		private const int NET_SQLITE_PERSIST_NOT_EXPECTED_RECORDS_AFFECTED = 0;
		private const int NET_SQLITE_QUERY_EXPECTED_RECORDS_AFFECTED = 0;
		private const string NET_SQLITE_SCHEMA_TABLE_NAME_FORMAT = "{1}";
		private const string NET_SQLITE_TABLE_ALIAS_FORMAT = "{0}";
		private const string NET_SQLITE_TABLE_NAME_FORMAT = "{0}";

		private static readonly NetSqliteDataSourceTagStrategy instance = new NetSqliteDataSourceTagStrategy();

		#endregion

		#region Properties/Indexers/Events

		public static NetSqliteDataSourceTagStrategy Instance
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
				return NET_SQLITE_DATA_SOURCE_TAG;
			}
		}

		#endregion

		#region Methods/Operators

		public void CommandMagic(IUnitOfWork unitOfWork, bool isNullipotent, out int expectedRecordsAffected)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if (!isNullipotent)
				expectedRecordsAffected = NET_SQLITE_PERSIST_NOT_EXPECTED_RECORDS_AFFECTED;
			else
				expectedRecordsAffected = NET_SQLITE_QUERY_EXPECTED_RECORDS_AFFECTED;
		}

		public bool CreateNativeDatabaseFile(string databaseFilePath)
		{
			Type type;
			MethodInfo methodInfo;

			//System.Data.SQLite.SQLiteConnection.CreateFile(databaseFilePath);
			type = Type.GetType("System.Data.SQLite.SQLiteConnection", false);

			if ((object)type == null)
				return false;

			methodInfo = type.GetMethod("CreateFile", new Type[] { typeof(string) });

			if ((object)methodInfo == null)
				return false;

			methodInfo.Invoke(null, new object[] { databaseFilePath });

			return true;
		}

		protected override string GetAliasedColumnName(string tableAlias, string columnName)
		{
			string retVal;

			retVal = string.Format(NET_SQLITE_COLUMN_ALIASED_FORMAT, this.GetTableAlias(tableAlias), columnName);

			return retVal;
		}

		protected override string GetColumnName(string columnName)
		{
			string retVal;

			retVal = string.Format(NET_SQLITE_COLUMN_NAME_FORMAT, columnName);

			return retVal;
		}

		protected override string GetIdentityCommand()
		{
			string retVal;

			retVal = NET_SQLITE_IDENTITY_COMMAND;

			return retVal;
		}

		protected override string GetParameterName(string parameterName)
		{
			string retVal;

			retVal = string.Format(NET_SQLITE_PARAMETER_NAME_FORMAT, parameterName);

			return retVal;
		}

		protected override string GetTableAlias(string tableAlias)
		{
			string retVal;

			retVal = string.Format(NET_SQLITE_TABLE_ALIAS_FORMAT, tableAlias);

			return retVal;
		}

		protected override string GetTableName(string schemaName, string tableName)
		{
			string retVal;

			retVal = !DataType.IsNullOrWhiteSpace(schemaName) ?
				string.Format(NET_SQLITE_SCHEMA_TABLE_NAME_FORMAT, schemaName, tableName) :
				string.Format(NET_SQLITE_TABLE_NAME_FORMAT, tableName);

			return retVal;
		}

		public void ParameterMagic(IUnitOfWork unitOfWork, IDataParameter commandParameter, string generatedFromColumnNativeType)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)commandParameter == null)
				throw new ArgumentNullException("commandParameter");
		}

		#endregion
	}
}