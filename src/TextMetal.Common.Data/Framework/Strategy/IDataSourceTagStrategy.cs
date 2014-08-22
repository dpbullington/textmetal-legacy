/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Common.Data.Framework.Mapping;

namespace TextMetal.Common.Data.Framework.Strategy
{
	public interface IDataSourceTagStrategy
	{
		#region Properties/Indexers/Events

		bool CanCreateNativeDatabaseFile
		{
			get;
		}

		string DataSourceTag
		{
			get;
		}

		#endregion

		#region Methods/Operators

		bool CreateNativeDatabaseFile(string databaseFilePath);

		TacticCommand<TModel> GetDeleteTacticCommand<TModel>(IUnitOfWork unitOfWork, Type modelType, object modelValue, IModelQuery modelQuery)
			where TModel : class, IModelObject;

		TacticCommand<TModel> GetInsertTacticCommand<TModel>(IUnitOfWork unitOfWork, Type modelType, object modelValue, IModelQuery modelQuery)
			where TModel : class, IModelObject;

		TacticCommand<TModel> GetSelectTacticCommand<TModel>(IUnitOfWork unitOfWork, Type modelType, object modelValue, IModelQuery modelQuery)
			where TModel : class, IModelObject;

		TacticCommand<TModel> GetUpdateTacticCommand<TModel>(IUnitOfWork unitOfWork, Type modelType, object modelValue, IModelQuery modelQuery)
			where TModel : class, IModelObject;

		#endregion

		//string GetAliasedColumnName(string tableAlias, string columnName);
		//string GetColumnName(string columnName);
		//string GetParameterName(string parameterName);
		//string GetTableAlias(string tableAlias);
		//string GetTableName(string schemaName, string tableName);
	}
}