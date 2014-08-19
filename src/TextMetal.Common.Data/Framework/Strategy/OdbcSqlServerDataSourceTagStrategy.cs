/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Data;

using TextMetal.Common.Core;

namespace TextMetal.Common.Data.Framework.Strategy
{
	public sealed class OdbcSqlServerDataSourceTagStrategy : IDataSourceTagStrategy
	{
		#region Constructors/Destructors

		private OdbcSqlServerDataSourceTagStrategy()
		{
		}

		#endregion

		#region Fields/Constants

		private const string ODBC_SQL_SERVER_COLUMN_ALIASED_FORMAT = "{0}.[{1}]";
		private const string ODBC_SQL_SERVER_COLUMN_NAME_FORMAT = "[{0}]";
		private const string ODBC_SQL_SERVER_DATA_SOURCE_TAG = "odbc.sqlserver";
		private const string ODBC_SQL_SERVER_IDENTITY_COMMAND = "@@IDENTITY"; // warning: 'SELECT SCOPE_IDENTITY() AS PK' should be used in the SAME BATCH if there is any chance of triggers on any tables causing identity creation
		private const string ODBC_SQL_SERVER_PARAMETER_NAME_FORMAT = "?";
		private const int ODBC_SQL_SERVER_PERSIST_NOT_EXPECTED_RECORDS_AFFECTED = 0;
		private const int ODBC_SQL_SERVER_QUERY_EXPECTED_RECORDS_AFFECTED = -1;
		private const string ODBC_SQL_SERVER_SCHEMA_TABLE_NAME_FORMAT = "[{0}].[{1}]";
		private const string ODBC_SQL_SERVER_TABLE_ALIAS_FORMAT = "{0}";
		private const string ODBC_SQL_SERVER_TABLE_NAME_FORMAT = "[{0}]";

		private static readonly OdbcSqlServerDataSourceTagStrategy instance = new OdbcSqlServerDataSourceTagStrategy();

		#endregion

		#region Properties/Indexers/Events

		public static OdbcSqlServerDataSourceTagStrategy Instance
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
				return ODBC_SQL_SERVER_DATA_SOURCE_TAG;
			}
		}

		#endregion

		#region Methods/Operators

		public void CommandMagic(IUnitOfWork unitOfWork, bool executeAsCud, out int thisOrThatRecordsAffected)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if (executeAsCud)
				thisOrThatRecordsAffected = ODBC_SQL_SERVER_PERSIST_NOT_EXPECTED_RECORDS_AFFECTED;
			else
				thisOrThatRecordsAffected = ODBC_SQL_SERVER_QUERY_EXPECTED_RECORDS_AFFECTED;
		}

		public bool CreateNativeDatabaseFile(string databaseFilePath)
		{
			return false;
		}

		public string GetAliasedColumnName(string tableAlias, string columnName)
		{
			string retVal;

			retVal = string.Format(ODBC_SQL_SERVER_COLUMN_ALIASED_FORMAT, this.GetTableAlias(tableAlias), columnName);

			return retVal;
		}

		public string GetColumnName(string columnName)
		{
			string retVal;

			retVal = string.Format(ODBC_SQL_SERVER_COLUMN_NAME_FORMAT, columnName);

			return retVal;
		}

		public string GetIdentityCommand()
		{
			string retVal;

			retVal = ODBC_SQL_SERVER_IDENTITY_COMMAND;

			return retVal;
		}

		public string GetParameterName(string parameterName)
		{
			string retVal;

			retVal = string.Format(ODBC_SQL_SERVER_PARAMETER_NAME_FORMAT, parameterName);

			return retVal;
		}

		public string GetTableAlias(string tableAlias)
		{
			string retVal;

			retVal = string.Format(ODBC_SQL_SERVER_TABLE_ALIAS_FORMAT, tableAlias);

			return retVal;
		}

		public string GetTableName(string schemaName, string tableName)
		{
			string retVal;

			retVal = !DataType.IsNullOrWhiteSpace(schemaName) ?
				string.Format(ODBC_SQL_SERVER_SCHEMA_TABLE_NAME_FORMAT, schemaName, tableName) :
				string.Format(ODBC_SQL_SERVER_TABLE_NAME_FORMAT, tableName);

			return retVal;
		}

		public void ParameterMagic(IUnitOfWork unitOfWork, IDataParameter commandParameter, string generatedFromColumnNativeType)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)commandParameter == null)
				throw new ArgumentNullException("commandParameter");

			if (generatedFromColumnNativeType.SafeToString().ToUpper() == "NTEXT")
				Reflexion.SetLogicalPropertyValue(commandParameter, "OdbcType", Enum.Parse(Type.GetType("System.Data.Odbc.OdbcType, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", true), "NText", true));
			else if (generatedFromColumnNativeType.SafeToString().ToUpper() == "TEXT")
				Reflexion.SetLogicalPropertyValue(commandParameter, "OdbcType", Enum.Parse(Type.GetType("System.Data.Odbc.OdbcType, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", true), "Text", true));
			else if (generatedFromColumnNativeType.SafeToString().ToUpper() == "IMAGE")
				Reflexion.SetLogicalPropertyValue(commandParameter, "OdbcType", Enum.Parse(Type.GetType("System.Data.Odbc.OdbcType, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", true), "Image", true));
		}

		#endregion
	}
}