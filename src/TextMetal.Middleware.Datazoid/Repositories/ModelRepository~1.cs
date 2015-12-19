/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using TextMetal.Middleware.Datazoid.Models.Functional;
using TextMetal.Middleware.Datazoid.Models.Tabular;
using TextMetal.Middleware.Datazoid.UoW;
using TextMetal.Middleware.Solder;

namespace TextMetal.Middleware.Datazoid.Repositories
{
	public abstract class ModelRepository<TDataContext> : ModelRepository, IModelRepository<TDataContext>
		where TDataContext : class, IDisposable
	{
		#region Constructors/Destructors

		protected ModelRepository()
		{
		}

		#endregion

		#region Methods/Operators

		public abstract bool Discard<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject tableModelObject) where TTableModelObject : class, ITableModelObject, new();

		public abstract TReturnProcedureModelObject Execute<TCallProcedureModelObject, TResultProcedureModelObject, TReturnProcedureModelObject>(IUnitOfWork unitOfWork, TCallProcedureModelObject callProcedureModel)
			where TCallProcedureModelObject : class, ICallProcedureModelObject, new()
			where TResultProcedureModelObject : class, IResultProcedureModelObject, new()
			where TReturnProcedureModelObject : class, IReturnProcedureModelObject<DefaultResultsetModelObject<TResultProcedureModelObject>, TResultProcedureModelObject>, new();

		public abstract bool Fill<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject tableModelObject) where TTableModelObject : class, ITableModelObject, new();

		public abstract IEnumerable<TTableModelObject> Find<TTableModelObject>(IUnitOfWork unitOfWork, ITableModelQuery tableModelQuery) where TTableModelObject : class, ITableModelObject, new();

		public abstract TTableModelObject Load<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject prototypeTableModel) where TTableModelObject : class, ITableModelObject, new();

		protected virtual void OnDiscardConflictTableModel<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject tableModelObject) where TTableModelObject : class, ITableModelObject, new()
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException(nameof(unitOfWork));

			if ((object)tableModelObject == null)
				throw new ArgumentNullException(nameof(tableModelObject));

			OnlyWhen._PROFILE_ThenPrint(string.Format("OnDiscardConflictTableModel <{0}>", typeof(TTableModelObject).Name));
		}

		protected virtual void OnPostDeleteTableModel<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject tableModelObject) where TTableModelObject : class, ITableModelObject, new()
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException(nameof(unitOfWork));

			if ((object)tableModelObject == null)
				throw new ArgumentNullException(nameof(tableModelObject));

			OnlyWhen._PROFILE_ThenPrint(string.Format("OnPostDeleteTableModel <{0}>", typeof(TTableModelObject).Name));
		}

		protected virtual void OnPostExecuteProcedureModel<TResultProcedureModelObject, TReturnProcedureModelObject>(IUnitOfWork unitOfWork, TReturnProcedureModelObject returnProcedureModelObject)
			where TResultProcedureModelObject : class, IResultProcedureModelObject, new()
			where TReturnProcedureModelObject : class, IReturnProcedureModelObject<DefaultResultsetModelObject<TResultProcedureModelObject>, TResultProcedureModelObject>, new()
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException(nameof(unitOfWork));

			if ((object)returnProcedureModelObject == null)
				throw new ArgumentNullException(nameof(returnProcedureModelObject));

			OnlyWhen._PROFILE_ThenPrint(string.Format("OnPostExecuteProcedureModel <{0}, {1}>", typeof(TResultProcedureModelObject).Name, typeof(TReturnProcedureModelObject).Name));
		}

		protected virtual void OnPostInsertTableModel<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject tableModelObject) where TTableModelObject : class, ITableModelObject, new()
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException(nameof(unitOfWork));

			if ((object)tableModelObject == null)
				throw new ArgumentNullException(nameof(tableModelObject));

			OnlyWhen._PROFILE_ThenPrint(string.Format("OnPostInsertTableModel <{0}>", typeof(TTableModelObject).Name));
		}

		protected virtual void OnPostResultsProcedureModel<TResultProcedureModelObject, TReturnProcedureModelObject>(IUnitOfWork unitOfWork, TReturnProcedureModelObject returnProcedureModelObject)
			where TResultProcedureModelObject : class, IResultProcedureModelObject, new()
			where TReturnProcedureModelObject : class, IReturnProcedureModelObject<DefaultResultsetModelObject<TResultProcedureModelObject>, TResultProcedureModelObject>, new()
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException(nameof(unitOfWork));

			if ((object)returnProcedureModelObject == null)
				throw new ArgumentNullException(nameof(returnProcedureModelObject));

			OnlyWhen._PROFILE_ThenPrint(string.Format("OnPostResultsProcedureModel <{0}, {1}>", typeof(TResultProcedureModelObject).Name, typeof(TReturnProcedureModelObject).Name));
		}

		protected virtual void OnPostSelectionTableModel<TTableModelObject>(IUnitOfWork unitOfWork) where TTableModelObject : class, ITableModelObject, new()
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException(nameof(unitOfWork));

			OnlyWhen._PROFILE_ThenPrint(string.Format("OnPostSelectionTableModel <{0}>", typeof(TTableModelObject).Name));
		}

		protected virtual void OnPostUpdateTableModel<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject tableModelObject) where TTableModelObject : class, ITableModelObject, new()
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException(nameof(unitOfWork));

			if ((object)tableModelObject == null)
				throw new ArgumentNullException(nameof(tableModelObject));

			OnlyWhen._PROFILE_ThenPrint(string.Format("OnPostUpdateTableModel <{0}>", typeof(TTableModelObject).Name));
		}

		protected virtual void OnPreDeleteTableModel<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject tableModelObject) where TTableModelObject : class, ITableModelObject, new()
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException(nameof(unitOfWork));

			if ((object)tableModelObject == null)
				throw new ArgumentNullException(nameof(tableModelObject));

			OnlyWhen._PROFILE_ThenPrint(string.Format("OnPreDeleteTableModel <{0}>", typeof(TTableModelObject).Name));
		}

		protected virtual void OnPreExecuteProcedureModel<TCallProcedureModelObject>(IUnitOfWork unitOfWork, TCallProcedureModelObject callProcedureModelObject) where TCallProcedureModelObject : class, ICallProcedureModelObject, new()
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException(nameof(unitOfWork));

			if ((object)callProcedureModelObject == null)
				throw new ArgumentNullException(nameof(callProcedureModelObject));

			OnlyWhen._PROFILE_ThenPrint(string.Format("OnPreExecuteProcedureModel <{0}>", typeof(TCallProcedureModelObject).Name));
		}

		public virtual void OnPreInsertTableModel<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject tableModelObject) where TTableModelObject : class, ITableModelObject, new()
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException(nameof(unitOfWork));

			if ((object)tableModelObject == null)
				throw new ArgumentNullException(nameof(tableModelObject));

			OnlyWhen._PROFILE_ThenPrint(string.Format("OnPreInsertTableModel <{0}>", typeof(TTableModelObject).Name));
			tableModelObject.Mark();
		}

		protected virtual void OnPreResultsProcedureModel<TCallProcedureModelObject>(IUnitOfWork unitOfWork, TCallProcedureModelObject callProcedureModelObject)
			where TCallProcedureModelObject : class, ICallProcedureModelObject, new()
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException(nameof(unitOfWork));

			if ((object)callProcedureModelObject == null)
				throw new ArgumentNullException(nameof(callProcedureModelObject));

			OnlyWhen._PROFILE_ThenPrint(string.Format("OnPreResultsProcedureModel <{0}>", typeof(TCallProcedureModelObject).Name));
		}

		protected virtual void OnPreSelectionTableModel<TTableModelObject>(IUnitOfWork unitOfWork) where TTableModelObject : class, ITableModelObject, new()
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException(nameof(unitOfWork));

			OnlyWhen._PROFILE_ThenPrint(string.Format("OnPreSelectionTableModel <{0}>", typeof(TTableModelObject).Name));
		}

		protected virtual void OnPreUpdateTableModel<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject tableModelObject) where TTableModelObject : class, ITableModelObject, new()
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException(nameof(unitOfWork));

			if ((object)tableModelObject == null)
				throw new ArgumentNullException(nameof(tableModelObject));

			OnlyWhen._PROFILE_ThenPrint(string.Format("OnPreUpdateTableModel <{0}>", typeof(TTableModelObject).Name));
			tableModelObject.Mark();
		}

		protected virtual void OnResultProcedureModel<TResultProcedureModelObject>(IUnitOfWork unitOfWork, TResultProcedureModelObject resultProcedureModelObject) where TResultProcedureModelObject : class, IResultProcedureModelObject, new()
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException(nameof(unitOfWork));

			if ((object)resultProcedureModelObject == null)
				throw new ArgumentNullException(nameof(resultProcedureModelObject));

			OnlyWhen._PROFILE_ThenPrint(string.Format("OnResultProcedureModel <{0}>", typeof(TResultProcedureModelObject).Name));
		}

		protected virtual void OnResultsetProcedureModel<TResultsetModelObject, TResultProcedureModelObject>(IUnitOfWork unitOfWork, TResultsetModelObject resultsetModelObject)
			where TResultsetModelObject : class, IResultsetModelObject<TResultProcedureModelObject>, new()
			where TResultProcedureModelObject : class, IResultProcedureModelObject, new()
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException(nameof(unitOfWork));

			if ((object)resultsetModelObject == null)
				throw new ArgumentNullException(nameof(resultsetModelObject));

			OnlyWhen._PROFILE_ThenPrint(string.Format("OnResultsetProcedureModel <{0}>", resultsetModelObject.Index));
		}

		protected virtual void OnSaveConflictTableModel<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject tableModelObject) where TTableModelObject : class, ITableModelObject, new()
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException(nameof(unitOfWork));

			if ((object)tableModelObject == null)
				throw new ArgumentNullException(nameof(tableModelObject));

			OnlyWhen._PROFILE_ThenPrint(string.Format("OnSaveConflictTableModel <{0}>", typeof(TTableModelObject).Name));
		}

		protected virtual void OnSelectTableModel<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject tableModelObject) where TTableModelObject : class, ITableModelObject, new()
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException(nameof(unitOfWork));

			if ((object)tableModelObject == null)
				throw new ArgumentNullException(nameof(tableModelObject));

			OnlyWhen._PROFILE_ThenPrint(string.Format("OnSelectTableModel <{0}>", typeof(TTableModelObject).Name));
		}

		public abstract TProjection Query<TProjection>(IUnitOfWork unitOfWork, Func<TDataContext, TProjection> dataContextQueryCallback);

		public abstract bool Save<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject tableModelObject) where TTableModelObject : class, ITableModelObject, new();

		#endregion
	}
}