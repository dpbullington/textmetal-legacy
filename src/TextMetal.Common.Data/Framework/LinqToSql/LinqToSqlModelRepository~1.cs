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
	public abstract class LinqToSqlModelRepository<TDataContext> : ModelRepository
		where TDataContext : DataContext
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the LinqToSqlModelRepository`1 class.
		/// </summary>
		protected LinqToSqlModelRepository()
		{
		}

		#endregion

		#region Methods/Operators

		protected bool LinqDiscard<TModel, TTable>(TModel model,
			Expression<Func<TTable, bool>> filterPredicateCallback)
			where TModel : class, IModelObject
			where TTable : class, new()
		{
			bool retval;

			if ((object)UnitOfWork.Current == null)
			{
				using (IUnitOfWork unitOfWork = this.GetUnitOfWork())
				{
					retval = this.LinqDiscard<TModel, TTable>(unitOfWork, model, filterPredicateCallback);

					unitOfWork.Complete();
				}
			}
			else
				retval = this.LinqDiscard<TModel, TTable>(UnitOfWork.Current, model, filterPredicateCallback);

			return retval;
		}

		protected bool LinqDiscard<TModel, TTable>(IUnitOfWork unitOfWork, TModel model,
			Expression<Func<TTable, bool>> filterPredicateCallback)
			where TModel : class, IModelObject
			where TTable : class, new()
		{
			bool wasNew;
			TTable table;
			Table<TTable> linqTable;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)model == null)
				throw new ArgumentNullException("model");

			if ((object)filterPredicateCallback == null)
				throw new ArgumentNullException("filterPredicateCallback");

			wasNew = model.IsNew;
			model.Mark();

			if (wasNew)
				return true;

			using (AmbientUnitOfWorkAwareDisposableWrapper<TDataContext> wrapper = unitOfWork.GetContext<TDataContext>())
			{
				this.OnPreDeleteModel<TModel>(unitOfWork, model);

				linqTable = wrapper.Disposable.GetTable<TTable>();
				table = linqTable.SingleOrDefault(filterPredicateCallback);

				if ((object)table == null)
					return false;

				linqTable.DeleteOnSubmit(table);

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

		protected bool LinqFill<TModel, TTable>(TModel model,
			Expression<Func<TTable, bool>> prototypePredicateCallback,
			Action<TModel, TTable> tableToModelMappingCallback)
			where TModel : class, IModelObject
			where TTable : class, new()
		{
			bool retval;

			if ((object)UnitOfWork.Current == null)
			{
				using (IUnitOfWork unitOfWork = this.GetUnitOfWork())
				{
					retval = this.LinqFill<TModel, TTable>(unitOfWork, model, prototypePredicateCallback, tableToModelMappingCallback);

					unitOfWork.Complete();
				}
			}
			else
				retval = this.LinqFill<TModel, TTable>(UnitOfWork.Current, model, prototypePredicateCallback, tableToModelMappingCallback);

			return retval;
		}

		protected bool LinqFill<TModel, TTable>(IUnitOfWork unitOfWork, TModel model,
			Expression<Func<TTable, bool>> prototypePredicateCallback,
			Action<TModel, TTable> tableToModelMappingCallback)
			where TModel : class, IModelObject
			where TTable : class, new()
		{
			TTable table;
			Table<TTable> linqTable;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)prototypePredicateCallback == null)
				throw new ArgumentNullException("prototypePredicateCallback");

			if ((object)tableToModelMappingCallback == null)
				throw new ArgumentNullException("tableToModelMappingCallback");

			using (AmbientUnitOfWorkAwareDisposableWrapper<TDataContext> wrapper = unitOfWork.GetContext<TDataContext>())
			{
				linqTable = wrapper.Disposable.GetTable<TTable>();
				table = linqTable.SingleOrDefault(prototypePredicateCallback);

				if ((object)table == null)
					return false;

				// map to POCO model from L2S table (destination, source)
				tableToModelMappingCallback(model, table);

				return true;
			}
		}

		protected IEnumerable<TModel> LinqFind<TModel, TTable>(
			Expression<Func<TTable, bool>> filterPredicateCallback,
			Action<TModel, TTable> tableToModelMappingCallback)
			where TModel : class, IModelObject
			where TTable : class, new()
		{
			IEnumerable<TModel> models;

			if ((object)filterPredicateCallback == null)
				throw new ArgumentNullException("filterPredicateCallback");

			if ((object)tableToModelMappingCallback == null)
				throw new ArgumentNullException("tableToModelMappingCallback");

			if ((object)UnitOfWork.Current == null)
			{
				using (IUnitOfWork unitOfWork = this.GetUnitOfWork())
				{
					models = this.LinqFind<TModel, TTable>(unitOfWork, filterPredicateCallback, tableToModelMappingCallback);

					models = models.ToList(); // FORCE EAGER LOAD

					unitOfWork.Complete();
				}
			}
			else
			{
				models = this.LinqFind<TModel, TTable>(UnitOfWork.Current, filterPredicateCallback, tableToModelMappingCallback);

				// DO NOT FORCE EAGER LOAD
			}

			return models;
		}

		protected IEnumerable<TModel> LinqFind<TModel, TTable>(IUnitOfWork unitOfWork,
			Expression<Func<TTable, bool>> filterPredicateCallback,
			Action<TModel, TTable> tableToModelMappingCallback)
			where TModel : class, IModelObject
			where TTable : class, new()
		{
			IQueryable<TTable> queryable;
			TModel model;
			Table<TTable> linqTable;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)filterPredicateCallback == null)
				throw new ArgumentNullException("filterPredicateCallback");

			if ((object)tableToModelMappingCallback == null)
				throw new ArgumentNullException("tableToModelMappingCallback");

			using (AmbientUnitOfWorkAwareDisposableWrapper<TDataContext> wrapper = unitOfWork.GetContext<TDataContext>())
			{
				linqTable = wrapper.Disposable.GetTable<TTable>();
				queryable = linqTable.Where(filterPredicateCallback);

				if ((object)queryable == null)
					throw new InvalidOperationException(string.Format("The queryable returned was invalid."));

				// DOES NOT FORCE EXECUTION AGAINST STORE
				foreach (TTable table in queryable)
				{
					model = this.CreateModel<TModel>();

					// map to POCO model from L2S table (destination, source)
					tableToModelMappingCallback(model, table);

					this.OnSelectModel<TModel>(unitOfWork, model);

					yield return model; // LAZY PROCESSING INTENT HERE / DO NOT FORCE EAGER LOAD
				}
			}
		}

		protected TModel LinqLoad<TModel, TTable>(
			Expression<Func<TTable, bool>> prototypePredicateCallback,
			Action<TModel, TTable> tableToModelMappingCallback)
			where TModel : class, IModelObject
			where TTable : class, new()
		{
			TModel retval;

			if ((object)UnitOfWork.Current == null)
			{
				using (IUnitOfWork unitOfWork = this.GetUnitOfWork())
				{
					retval = this.LinqLoad<TModel, TTable>(unitOfWork, prototypePredicateCallback, tableToModelMappingCallback);

					unitOfWork.Complete();
				}
			}
			else
				retval = this.LinqLoad<TModel, TTable>(UnitOfWork.Current, prototypePredicateCallback, tableToModelMappingCallback);

			return retval;
		}

		protected TModel LinqLoad<TModel, TTable>(IUnitOfWork unitOfWork,
			Expression<Func<TTable, bool>> prototypePredicateCallback,
			Action<TModel, TTable> tableToModelMappingCallback)
			where TModel : class, IModelObject
			where TTable : class, new()
		{
			TTable table;
			TModel model;
			Table<TTable> linqTable;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)prototypePredicateCallback == null)
				throw new ArgumentNullException("prototypePredicateCallback");

			if ((object)tableToModelMappingCallback == null)
				throw new ArgumentNullException("tableToModelMappingCallback");

			using (AmbientUnitOfWorkAwareDisposableWrapper<TDataContext> wrapper = unitOfWork.GetContext<TDataContext>())
			{
				linqTable = wrapper.Disposable.GetTable<TTable>();
				table = linqTable.SingleOrDefault(prototypePredicateCallback);

				if ((object)table == null)
					return null;

				model = this.CreateModel<TModel>();

				// map to POCO model from L2S table (destination, source)
				tableToModelMappingCallback(model, table);

				return model;
			}
		}

		protected bool LinqSave<TModel, TTable>(TModel model,
			Expression<Func<TTable, bool>> filterPredicateCallback,
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
					retval = this.LinqSave<TModel, TTable>(unitOfWork, model, filterPredicateCallback, modelToTableMappingCallback, tableToModelMappingCallback);

					unitOfWork.Complete();
				}
			}
			else
				retval = this.LinqSave<TModel, TTable>(UnitOfWork.Current, model, filterPredicateCallback, modelToTableMappingCallback, tableToModelMappingCallback);

			return retval;
		}

		protected bool LinqSave<TModel, TTable>(IUnitOfWork unitOfWork, TModel model,
			Expression<Func<TTable, bool>> filterPredicateCallback,
			Action<TTable, TModel> modelToTableMappingCallback,
			Action<TModel, TTable> tableToModelMappingCallback)
			where TModel : class, IModelObject
			where TTable : class, new()
		{
			bool wasNew;
			TTable table;
			Table<TTable> linqTable;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)model == null)
				throw new ArgumentNullException("model");

			if ((object)filterPredicateCallback == null)
				throw new ArgumentNullException("filterPredicateCallback");

			if ((object)modelToTableMappingCallback == null)
				throw new ArgumentNullException("modelToTableMappingCallback");

			if ((object)tableToModelMappingCallback == null)
				throw new ArgumentNullException("tableToModelMappingCallback");

			wasNew = model.IsNew;
			model.Mark();

			using (AmbientUnitOfWorkAwareDisposableWrapper<TDataContext> wrapper = unitOfWork.GetContext<TDataContext>())
			{
				linqTable = wrapper.Disposable.GetTable<TTable>();

				if (wasNew)
				{
					this.OnPreInsertModel<TModel>(unitOfWork, model);

					table = new TTable();

					linqTable.InsertOnSubmit(table);
				}
				else
				{
					this.OnPreUpdateModel<TModel>(unitOfWork, model);

					table = linqTable.SingleOrDefault(filterPredicateCallback);

					if ((object)table == null)
						return false;
				}

				// map to L2S table from POCO model (destination, source)
				modelToTableMappingCallback(table, model);

				try
				{
					wrapper.Disposable.SubmitChanges(ConflictMode.FailOnFirstConflict);
				}
				catch (ChangeConflictException ccex)
				{
					this.OnSaveConflictModel<TModel>(unitOfWork, model);

					return false;
				}

				// map to POCO model from L2S table (destination, source)
				tableToModelMappingCallback(model, table);

				if (wasNew)
					this.OnPostInsertModel<TModel>(unitOfWork, model);
				else
					this.OnPostUpdateModel<TModel>(unitOfWork, model);

				return true;
			}
		}

		#endregion
	}
}