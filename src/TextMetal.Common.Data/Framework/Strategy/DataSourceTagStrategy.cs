/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Data;

using TextMetal.Common.Data.Framework.Mapping;

namespace TextMetal.Common.Data.Framework.Strategy
{
	public abstract class DataSourceTagStrategy : IDataSourceTagStrategy
	{
		#region Constructors/Destructors

		protected DataSourceTagStrategy(string dataSourceTag, bool canCreateNativeDatabaseFile)
		{
			this.dataSourceTag = dataSourceTag;
			this.canCreateNativeDatabaseFile = canCreateNativeDatabaseFile;
		}

		#endregion

		#region Fields/Constants

		private readonly bool canCreateNativeDatabaseFile;
		private readonly string dataSourceTag;

		#endregion

		#region Properties/Indexers/Events

		public bool CanCreateNativeDatabaseFile
		{
			get
			{
				return this.canCreateNativeDatabaseFile;
			}
		}

		public string DataSourceTag
		{
			get
			{
				return this.dataSourceTag;
			}
		}

		#endregion

		#region Methods/Operators

		protected abstract string GetAliasedColumnName(string tableAlias, string columnName);

		protected abstract string GetColumnName(string columnName);

		protected abstract string GetIdentityCommand();

		protected abstract string GetParameterName(string parameterName);

		protected abstract string GetTableAlias(string tableAlias);

		protected abstract string GetTableName(string schemaName, string tableName);

		public bool CreateNativeDatabaseFile(string databaseFilePath)
		{
			// do nothing
			return false;
		}

		public TacticCommand<TModel> GetDeleteTacticCommand<TModel>(IUnitOfWork unitOfWork, Type modelType, object modelValue, IModelQuery modelQuery) where TModel : class, IModelObject
		{
			return new TacticCommand<TModel>()
					{
						CommandBehavior = CommandBehavior.Default,
						CommandParameters = new IDataParameter[] { },
						CommandPrepare = false,
						CommandText = "",
						CommandTimeout = null,
						CommandType = CommandType.Text,
						ExpectedRecordsAffected = -1,
						IsNullipotent = true,
						TableToModelMappingCallback = (t, m) =>
													{
													}
					};
		}

		public TacticCommand<TModel> GetInsertTacticCommand<TModel>(IUnitOfWork unitOfWork, Type modelType, object modelValue, IModelQuery modelQuery) where TModel : class, IModelObject
		{
			return new TacticCommand<TModel>()
			{
				CommandBehavior = CommandBehavior.Default,
				CommandParameters = new IDataParameter[] { },
				CommandPrepare = false,
				CommandText = "",
				CommandTimeout = null,
				CommandType = CommandType.Text,
				ExpectedRecordsAffected = -1,
				IsNullipotent = true,
				TableToModelMappingCallback = (t, m) =>
				{
				}
			};
		}

		public TacticCommand<TModel> GetSelectTacticCommand<TModel>(IUnitOfWork unitOfWork, Type modelType, object modelValue, IModelQuery modelQuery) where TModel : class, IModelObject
		{
			return new TacticCommand<TModel>()
			{
				CommandBehavior = CommandBehavior.Default,
				CommandParameters = new IDataParameter[] { },
				CommandPrepare = false,
				CommandText = GetSelectOneCommandText(),
				CommandTimeout = null,
				CommandType = CommandType.Text,
				ExpectedRecordsAffected = -1,
				IsNullipotent = true,
				TableToModelMappingCallback = (t, m) =>
				{
				}
			};
		}

		private string GetSelectOneCommandText()
		{
			string commandText;
			const string TABLE_ALIAS = "t0";
			string schemaName = "dbo", tableName = "tab";
			string[] columnNames = new string[] { "a", "b", "c" };

			commandText = @"SELECT ";

			for (int index = 0; index < columnNames.Length; index++)
			{
				commandText += this.GetAliasedColumnName(TABLE_ALIAS, columnNames[index]) + @" AS " + this.GetColumnName(columnNames[index]);

				if(index != (columnNames.Length - 1))
					commandText += @", ";
			}

			commandText += @" FROM ";
			
			commandText += this.GetTableName(schemaName, tableName) + @" " + this.GetTableAlias(TABLE_ALIAS);
			
			commandText += @" WHERE ";

			for (int index = 0; index < columnNames.Length; index++)
			{
				commandText += this.GetAliasedColumnName(TABLE_ALIAS, columnNames[index]) + @" = " + this.GetParameterName(columnNames[index]);

				if (index != (columnNames.Length - 1))
					commandText += @" AND ";
			}

			commandText += @" ORDER BY ";

			for (int index = 0; index < columnNames.Length; index++)
			{
				commandText += this.GetAliasedColumnName(TABLE_ALIAS, columnNames[index]) + @" ASC";

				if (index != (columnNames.Length - 1))
					commandText += @", ";
			}

			commandText += @";";

			return commandText;
		}

		public TacticCommand<TModel> GetUpdateTacticCommand<TModel>(IUnitOfWork unitOfWork, Type modelType, object modelValue, IModelQuery modelQuery) where TModel : class, IModelObject
		{
			return new TacticCommand<TModel>()
			{
				CommandBehavior = CommandBehavior.Default,
				CommandParameters = new IDataParameter[] { },
				CommandPrepare = false,
				CommandText = "",
				CommandTimeout = null,
				CommandType = CommandType.Text,
				ExpectedRecordsAffected = -1,
				IsNullipotent = true,
				TableToModelMappingCallback = (t, m) =>
				{
				}
			};
		}

		#endregion
	}
}