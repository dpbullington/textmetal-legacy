/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using TextMetal.Middleware.Common.Fascades.AdoNet.UoW;
using TextMetal.Middleware.Data.Models.Functional;
using TextMetal.Middleware.Data.Models.Tabular;

namespace TextMetal.Middleware.Data.Repositories
{
	public interface IModelRepository<TContext> : IModelRepository
		where TContext : class, IDisposable
	{
		#region Methods/Operators

		bool Discard<TTableModelObject>(TTableModelObject tableModelObject)
			where TTableModelObject : class, ITableModelObject, new();

		bool Discard<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject tableModelObject)
			where TTableModelObject : class, ITableModelObject, new();

		TReturnProcedureModelObject Execute<TCallProcedureModelObject, TResultProcedureModelObject, TReturnProcedureModelObject>(TCallProcedureModelObject callProcedureModel)
			where TCallProcedureModelObject : class, ICallProcedureModelObject, new()
			where TResultProcedureModelObject : class, IResultProcedureModelObject, new()
			where TReturnProcedureModelObject : class, IReturnProcedureModelObject<DefaultResultsetModelObject<TResultProcedureModelObject>, TResultProcedureModelObject>, new();

		TReturnProcedureModelObject Execute<TCallProcedureModelObject, TResultProcedureModelObject, TReturnProcedureModelObject>(IUnitOfWork unitOfWork, TCallProcedureModelObject callProcedureModel)
			where TCallProcedureModelObject : class, ICallProcedureModelObject, new()
			where TResultProcedureModelObject : class, IResultProcedureModelObject, new()
			where TReturnProcedureModelObject : class, IReturnProcedureModelObject<DefaultResultsetModelObject<TResultProcedureModelObject>, TResultProcedureModelObject>, new();

		bool Fill<TTableModelObject>(TTableModelObject tableModelObject)
			where TTableModelObject : class, ITableModelObject, new();

		bool Fill<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject tableModelObject)
			where TTableModelObject : class, ITableModelObject, new();

		IEnumerable<TTableModelObject> Find<TTableModelObject>(ITableModelQuery tableModelQuery)
			where TTableModelObject : class, ITableModelObject, new();

		IEnumerable<TTableModelObject> Find<TTableModelObject>(IUnitOfWork unitOfWork, ITableModelQuery tableModelQuery)
			where TTableModelObject : class, ITableModelObject, new();

		TTableModelObject Load<TTableModelObject>(TTableModelObject prototypeTableModel)
			where TTableModelObject : class, ITableModelObject, new();

		TTableModelObject Load<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject prototypeTableModel)
			where TTableModelObject : class, ITableModelObject, new();

		TProjection Query<TProjection>(Func<TContext, TProjection> contextQueryCallback);

		TProjection Query<TProjection>(IUnitOfWork unitOfWork,
			Func<TContext, TProjection> contextQueryCallback);

		bool Save<TTableModelObject>(TTableModelObject tableModelObject)
			where TTableModelObject : class, ITableModelObject, new();

		bool Save<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject tableModelObject)
			where TTableModelObject : class, ITableModelObject, new();

		#endregion
	}
}