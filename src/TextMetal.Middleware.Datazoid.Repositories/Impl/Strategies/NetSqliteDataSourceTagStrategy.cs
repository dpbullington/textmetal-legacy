/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Reflection;

using TextMetal.Middleware.Datazoid.Repositories.Impl.Tactics;
using TextMetal.Middleware.Datazoid.UoW;
using TextMetal.Middleware.Solder.Extensions;

namespace TextMetal.Middleware.Datazoid.Repositories.Impl.Strategies
{
	public sealed class NetSqliteDataSourceTagStrategy : DataSourceTagStrategy
	{
		#region Constructors/Destructors

		private NetSqliteDataSourceTagStrategy()
			: base(NET_SQLITE_DATA_SOURCE_TAG, true, false)
		{
		}

		#endregion

		#region Fields/Constants

		private const string NET_SQLITE_COLUMN_ALIASED_FORMAT = "{0}.{1}";
		private const string NET_SQLITE_COLUMN_NAME_FORMAT = "{0}";
		private const string NET_SQLITE_DATA_SOURCE_TAG = "net.sqlite";
		private const string NET_SQLITE_IDENTITY_FUNCTION_NAME = "LAST_INSERT_ROWID()";
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

		#endregion

		#region Methods/Operators

		public override bool CreateNativeDatabaseFile(string databaseFilePath)
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

		public override void FixupParameter(IUnitOfWork unitOfWork, ITacticParameter tacticParameter, string originalSqlType)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException(nameof(unitOfWork));

			if ((object)tacticParameter == null)
				throw new ArgumentNullException(nameof(tacticParameter));
		}

		public override string GetAliasedColumnName(string tableAlias, string columnName)
		{
			string retVal;

			retVal = string.Format(NET_SQLITE_COLUMN_ALIASED_FORMAT, this.GetTableAlias(tableAlias), columnName);

			return retVal;
		}

		public override string GetColumnName(string columnName)
		{
			string retVal;

			retVal = string.Format(NET_SQLITE_COLUMN_NAME_FORMAT, columnName);

			return retVal;
		}

		public override int GetExpectedRecordsAffected(bool isNullipotent)
		{
			if (!isNullipotent)
				return NET_SQLITE_PERSIST_NOT_EXPECTED_RECORDS_AFFECTED;
			else
				return NET_SQLITE_QUERY_EXPECTED_RECORDS_AFFECTED;
		}

		public override string GetIdentityFunctionName()
		{
			string retVal;

			retVal = NET_SQLITE_IDENTITY_FUNCTION_NAME;

			return retVal;
		}

		public override string GetParameterName(string parameterName)
		{
			string retVal;

			retVal = string.Format(NET_SQLITE_PARAMETER_NAME_FORMAT, parameterName);

			return retVal;
		}

		public override string GetProcedureName(string schemaName, string procedureName)
		{
			throw new NotSupportedException(string.Format("Procedures are not supported by this data source tag strategy implementation."));
		}

		public override string GetTableAlias(string tableAlias)
		{
			string retVal;

			retVal = string.Format(NET_SQLITE_TABLE_ALIAS_FORMAT, tableAlias);

			return retVal;
		}

		public override string GetTableName(string schemaName, string tableName)
		{
			string retVal;

			retVal = !SolderFascadeAccessor.DataTypeFascade.IsNullOrWhiteSpace(schemaName) ?
				string.Format(NET_SQLITE_SCHEMA_TABLE_NAME_FORMAT, schemaName, tableName) :
				string.Format(NET_SQLITE_TABLE_NAME_FORMAT, tableName);

			return retVal;
		}

		#endregion
	}
}