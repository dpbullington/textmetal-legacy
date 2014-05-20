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
	public class LinqToSqlModelRepository<TDataContext> : ModelRepository
		where TDataContext : DataContext
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the LinqToSqlModelRepository`1 class.
		/// </summary>
		public LinqToSqlModelRepository()
		{
		}

		#endregion

		#region Methods/Operators

		public bool Discard<TModel, TTable>(TModel model, Func<IQueryable<TTable>, TModel, IQueryable<TTable>> queryableCallback)
			where TModel : class, IModelObject
			where TTable : class, new()
		{
			bool retval;

			if ((object)UnitOfWork.Current == null)
			{
				using (IUnitOfWork unitOfWork = this.GetUnitOfWork())
				{
					retval = this.Discard<TModel, TTable>(unitOfWork, model, queryableCallback);

					unitOfWork.Complete();
				}
			}
			else
				retval = this.Discard<TModel, TTable>(UnitOfWork.Current, model, queryableCallback);

			return retval;
		}

		public bool Discard<TModel, TTable>(IUnitOfWork unitOfWork, TModel model,
			Func<IQueryable<TTable>, TModel, IQueryable<TTable>> queryableCallback)
			where TModel : class, IModelObject
			where TTable : class, new()
		{
			IQueryable<TTable> queryable;
			TTable table;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)model == null)
				throw new ArgumentNullException("model");

			if (model.IsNew)
				return true;

			if ((object)queryableCallback == null)
				throw new ArgumentNullException("queryableCallback");

			using (AmbientUnitOfWorkAwareDisposableWrapper<TDataContext> wrapper = unitOfWork.GetContext<TDataContext>())
			{
				this.OnPreDeleteModel<TModel>(unitOfWork, model);

				// cannot just do SingleOrDefault(predicateCallback)
				queryable = queryableCallback(wrapper.Disposable.GetTable<TTable>(), model);

				if ((object)queryable == null)
					throw new InvalidOperationException(string.Format("The queryable returned was invalid."));

				table = queryable.SingleOrDefault();

				if ((object)table == null)
					throw new InvalidOperationException(string.Format("The table returned was invalid."));

				wrapper.Disposable.GetTable<TTable>().DeleteOnSubmit(table);

				try
				{
					wrapper.Disposable.SubmitChanges(ConflictMode.FailOnFirstConflict);
				}
				catch (ChangeConflictException ccex)
				{
					this.OnDiscardConflictModel<TModel>(unitOfWork, model);

					return false;
				}

				this.OnPostDeleteModel<TModel>(unitOfWork, model);

				model.IsNew = false;

				return true;
			}
		}

		public override bool Discard<TModel>(IUnitOfWork unitOfWork, TModel model)
		{
			throw new NotImplementedException();
		}

		public override TResponseModel ExecuteImperative<TRequestModel, TResultModel, TResponseModel>(IUnitOfWork unitOfWork, TRequestModel requestModel)
		{
			throw new NotImplementedException();
		}

		public override TModel Fill<TModel>(IUnitOfWork unitOfWork, TModel model)
		{
			throw new NotImplementedException();
		}

		public override IEnumerable<TModel> Find<TModel>(IUnitOfWork unitOfWork, IModelQuery query)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<TModel> Find<TModel, TTable>(Func<IQueryable<TTable>, IQueryable<TTable>> queryableCallback,
			Expression<Func<TTable, TModel>> selectorCallback)
			where TModel : class, IModelObject
			where TTable : class, new()
		{
			IEnumerable<TModel> models;

			if ((object)queryableCallback == null)
				throw new ArgumentNullException("queryableCallback");

			if ((object)selectorCallback == null)
				throw new ArgumentNullException("selectorCallback");

			if ((object)UnitOfWork.Current == null)
			{
				using (IUnitOfWork unitOfWork = this.GetUnitOfWork())
				{
					models = this.Find<TModel, TTable>(unitOfWork, queryableCallback, selectorCallback);

					models = models.ToList(); // FORCE EAGER LOAD

					unitOfWork.Complete();
				}
			}
			else
			{
				models = this.Find<TModel, TTable>(UnitOfWork.Current, queryableCallback, selectorCallback);

				// DO NOT FORCE EAGER LOAD
			}

			return models;
		}

		public IEnumerable<TModel> Find<TModel, TTable>(IUnitOfWork unitOfWork,
			Func<IQueryable<TTable>, IQueryable<TTable>> queryableCallback,
			Expression<Func<TTable, TModel>> selectorCallback)
			where TModel : class, IModelObject
			where TTable : class, new()
		{
			IEnumerable<TModel> models;
			IQueryable<TTable> queryable;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)queryableCallback == null)
				throw new ArgumentNullException("queryableCallback");

			if ((object)selectorCallback == null)
				throw new ArgumentNullException("selectorCallback");

			using (AmbientUnitOfWorkAwareDisposableWrapper<TDataContext> wrapper = unitOfWork.GetContext<TDataContext>())
			{
				queryable = queryableCallback(wrapper.Disposable.GetTable<TTable>());

				if ((object)queryable == null)
					throw new InvalidOperationException(string.Format("The queryable returned was invalid."));

				models = queryable.Select(selectorCallback); // DOES NOT FORCE EXECUTION AGAINST STORE

				foreach (TModel model in models)
				{
					this.OnSelectModel<TModel>(unitOfWork, model);
					//model.Mark();
					yield return model; // LAZY PROCESSING INTENT HERE / DO NOT FORCE EAGER LOAD
				}
			}
		}

		public TModel Load<TModel, TTable>(TModel prototype, Expression<Func<TTable, TModel>> selectorCallback,
			Func<IQueryable<TTable>, TModel, IQueryable<TTable>> queryableCallback)
			where TModel : class, IModelObject
			where TTable : class, new()
		{
			TModel retval;

			if ((object)UnitOfWork.Current == null)
			{
				using (IUnitOfWork unitOfWork = this.GetUnitOfWork())
				{
					retval = this.Load<TModel, TTable>(unitOfWork, prototype, selectorCallback, queryableCallback);

					unitOfWork.Complete();
				}
			}
			else
				retval = this.Load<TModel, TTable>(UnitOfWork.Current, prototype, selectorCallback, queryableCallback);

			return retval;
		}

		public TModel Load<TModel, TTable>(IUnitOfWork unitOfWork, TModel prototype,
			Expression<Func<TTable, TModel>> selectorCallback,
			Func<IQueryable<TTable>, TModel, IQueryable<TTable>> queryableCallback)
			where TModel : class, IModelObject
			where TTable : class, new()
		{
			IQueryable<TTable> queryable;
			TTable table;
			TModel model;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)prototype == null)
				throw new ArgumentNullException("prototype");

			if ((object)selectorCallback == null)
				throw new ArgumentNullException("selectorCallback");

			if ((object)queryableCallback == null)
				throw new ArgumentNullException("queryableCallback");

			using (AmbientUnitOfWorkAwareDisposableWrapper<TDataContext> wrapper = unitOfWork.GetContext<TDataContext>())
			{
				// cannot just do SingleOrDefault(predicateCallback)
				queryable = queryableCallback(wrapper.Disposable.GetTable<TTable>(), prototype);

				if ((object)queryable == null)
					throw new InvalidOperationException(string.Format("The queryable returned was invalid."));

				table = queryable.SingleOrDefault();

				if ((object)table == null)
					throw new InvalidOperationException(string.Format("The table returned was invalid."));

				model = selectorCallback.Compile().Invoke(table); // is this approapriate?

				return model;
			}
		}

		public override TModel Load<TModel>(IUnitOfWork unitOfWork, TModel prototype)
		{
			throw new NotImplementedException();
		}

		public bool Save<TModel, TTable>(TModel model, Func<IQueryable<TTable>, TModel, IQueryable<TTable>> queryableCallback,
			Action<TTable, TModel> modelToTableMappingCallback,
			Action<TModel, TTable> tableToModelMappingCallback)
			where TModel : class, IModelObject
			where TTable : class, new()
		{
			bool retval;

			if ((object)UnitOfWork.Current == null)
			{
				using (IUnitOfWork unitOfWork = this.GetUnitOfWork())
				{
					retval = this.Save<TModel, TTable>(unitOfWork, model, queryableCallback, modelToTableMappingCallback, tableToModelMappingCallback);

					unitOfWork.Complete();
				}
			}
			else
				retval = this.Save<TModel, TTable>(UnitOfWork.Current, model, queryableCallback, modelToTableMappingCallback, tableToModelMappingCallback);

			return retval;
		}

		public bool Save<TModel, TTable>(IUnitOfWork unitOfWork, TModel model,
			Func<IQueryable<TTable>, TModel, IQueryable<TTable>> queryableCallback,
			Action<TTable, TModel> modelToTableMappingCallback,
			Action<TModel, TTable> tableToModelMappingCallback)
			where TModel : class, IModelObject
			where TTable : class, new()
		{
			bool wasNew;
			IQueryable<TTable> queryable;
			TTable table;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)model == null)
				throw new ArgumentNullException("model");

			if ((object)queryableCallback == null)
				throw new ArgumentNullException("queryableCallback");

			if ((object)modelToTableMappingCallback == null)
				throw new ArgumentNullException("modelToTableMappingCallback");

			if ((object)tableToModelMappingCallback == null)
				throw new ArgumentNullException("tableToModelMappingCallback");

			using (AmbientUnitOfWorkAwareDisposableWrapper<TDataContext> wrapper = unitOfWork.GetContext<TDataContext>())
			{
				wasNew = model.IsNew;

				if (wasNew)
				{
					this.OnPreInsertModel<TModel>(unitOfWork, model);

					table = new TTable();

					wrapper.Disposable.GetTable<TTable>().InsertOnSubmit(table);
				}
				else
				{
					this.OnPreUpdateModel<TModel>(unitOfWork, model);

					queryable = queryableCallback(wrapper.Disposable.GetTable<TTable>(), model);

					if ((object)queryable == null)
						throw new InvalidOperationException(string.Format("The queryable returned was invalid."));

					table = queryable.SingleOrDefault();

					if ((object)table == null)
						throw new InvalidOperationException(string.Format("The table returned was invalid."));
				}

				// map caller POCO changes to L2S object
				modelToTableMappingCallback(table, model); // (destination, source)

				try
				{
					wrapper.Disposable.SubmitChanges(ConflictMode.FailOnFirstConflict);
				}
				catch (ChangeConflictException ccex)
				{
					this.OnSaveConflictModel<TModel>(unitOfWork, model);

					return false;
				}

				// map server changes back to POCO from L2S object
				tableToModelMappingCallback(model, table); // (destination, source)

				if (wasNew)
					this.OnPostInsertModel<TModel>(unitOfWork, model);
				else
					this.OnPostUpdateModel<TModel>(unitOfWork, model);

				return true;
			}
		}

		public override bool Save<TModel>(IUnitOfWork unitOfWork, TModel model)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}