/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using TextMetal.Middleware.Datazoid.Models.Functional;
using TextMetal.Middleware.Datazoid.Models.Tabular;
using TextMetal.Middleware.Datazoid.Repositories.Impl.Expressions;
using TextMetal.Middleware.Datazoid.Repositories.Impl.Mappings;
using TextMetal.Middleware.Datazoid.Repositories.Impl.Tactics;
using TextMetal.Middleware.Datazoid.UoW;

namespace TextMetal.Middleware.Datazoid.Repositories.Impl.Strategies
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

		void FixupParameter(IUnitOfWork unitOfWork, ITacticParameter tacticParameter, string originalSqlType);

		ITableTacticCommand<TTableModelObject> GetDeleteTacticCommand<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject modelValue, ITableModelQuery tableModelQuery)
			where TTableModelObject : class, ITableModelObject, new();

		IProcedureTacticCommand<TCallProcedureModelObject, TResultProcedureModelObject, TReturnProcedureModelObject> GetExecuteTacticCommand<TCallProcedureModelObject, TResultProcedureModelObject, TReturnProcedureModelObject>(IUnitOfWork unitOfWork, TCallProcedureModelObject callProcedureModelValue)
			where TCallProcedureModelObject : class, ICallProcedureModelObject, new()
			where TResultProcedureModelObject : class, IResultProcedureModelObject, new()
			where TReturnProcedureModelObject : class, IReturnProcedureModelObject<DefaultResultsetModelObject<TResultProcedureModelObject>, TResultProcedureModelObject>, new();

		ITableTacticCommand<TTableModelObject> GetIdentifyTacticCommand<TTableModelObject>(IUnitOfWork unitOfWork)
			where TTableModelObject : class, ITableModelObject, new();

		ITableTacticCommand<TTableModelObject> GetInsertTacticCommand<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject modelValue, ITableModelQuery tableModelQuery)
			where TTableModelObject : class, ITableModelObject, new();

		string GetOrderByListFragment(IEnumerable<SortOrder> sortOrders);

		ITableTacticCommand<TTableModelObject> GetSelectTacticCommand<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject modelValue, ITableModelQuery tableModelQuery)
			where TTableModelObject : class, ITableModelObject, new();

		ITableTacticCommand<TTableModelObject> GetUpdateTacticCommand<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject modelValue, ITableModelQuery tableModelQuery)
			where TTableModelObject : class, ITableModelObject, new();

		string GetWherePredicateFragment(TableMappingAttribute tableMappingAttribute, IUnitOfWork unitOfWork, IDictionary<string, ITacticParameter> tacticParameters, IExpression expression);

		#endregion
	}
}