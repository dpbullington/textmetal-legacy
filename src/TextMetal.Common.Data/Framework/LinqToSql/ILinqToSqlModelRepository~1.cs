/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;

namespace TextMetal.Common.Data.Framework.LinqToSql
{
	public interface ILinqToSqlModelRepository<TDataContext> : IModelRepository
		where TDataContext : DataContext
	{
		#region Methods/Operators

		bool LinqDiscard<TModel, TTable>(TModel model,
			Expression<Func<TTable, bool>> filterPredicateCallback)
			where TModel : class, IModelObject
			where TTable : class, new();

		bool LinqDiscard<TModel, TTable>(IUnitOfWork unitOfWork, TModel model,
			Expression<Func<TTable, bool>> filterPredicateCallback)
			where TModel : class, IModelObject
			where TTable : class, new();

		bool LinqFill<TModel, TTable>(TModel model,
			Expression<Func<TTable, bool>> prototypePredicateCallback,
			Action<TModel, TTable> tableToModelMappingCallback)
			where TModel : class, IModelObject
			where TTable : class, new();

		bool LinqFill<TModel, TTable>(IUnitOfWork unitOfWork, TModel model,
			Expression<Func<TTable, bool>> prototypePredicateCallback,
			Action<TModel, TTable> tableToModelMappingCallback)
			where TModel : class, IModelObject
			where TTable : class, new();

		IEnumerable<TModel> LinqFind<TModel, TTable>(
			Expression<Func<TTable, bool>> filterPredicateCallback,
			Action<TModel, TTable> tableToModelMappingCallback)
			where TModel : class, IModelObject
			where TTable : class, new();

		IEnumerable<TModel> LinqFind<TModel, TTable>(IUnitOfWork unitOfWork,
			Expression<Func<TTable, bool>> filterPredicateCallback,
			Action<TModel, TTable> tableToModelMappingCallback)
			where TModel : class, IModelObject
			where TTable : class, new();

		TModel LinqLoad<TModel, TTable>(
			Expression<Func<TTable, bool>> prototypePredicateCallback,
			Action<TModel, TTable> tableToModelMappingCallback)
			where TModel : class, IModelObject
			where TTable : class, new();

		TModel LinqLoad<TModel, TTable>(IUnitOfWork unitOfWork,
			Expression<Func<TTable, bool>> prototypePredicateCallback,
			Action<TModel, TTable> tableToModelMappingCallback)
			where TModel : class, IModelObject
			where TTable : class, new();

		IEnumerable<T> LinqQuery<T>(Func<TDataContext, IQueryable<T>> query);

		IEnumerable<T> LinqQuery<T>(IUnitOfWork unitOfWork,
			Func<TDataContext, IQueryable<T>> query);

		bool LinqSave<TModel, TTable>(TModel model,
			Expression<Func<TTable, bool>> filterPredicateCallback,
			Action<TTable, TModel> modelToTableMappingCallback,
			Action<TModel, TTable> tableToModelMappingCallback)
			where TModel : class, IModelObject
			where TTable : class, new();

		bool LinqSave<TModel, TTable>(IUnitOfWork unitOfWork, TModel model,
			Expression<Func<TTable, bool>> filterPredicateCallback,
			Action<TTable, TModel> modelToTableMappingCallback,
			Action<TModel, TTable> tableToModelMappingCallback)
			where TModel : class, IModelObject
			where TTable : class, new();

		#endregion
	}
}