/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

using TextMetal.Common.Data.Framework;

namespace TextMetal.Common.Data.LinqToSql
{
	public abstract class LinqToSqlModelRepository<TDataContext> : ModelRepository, ILinqToSqlModelRepository<TDataContext>
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

		public bool LinqDiscard<TModel, TTable>(TModel model,
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

			using (AmbientUnitOfWorkAwareDisposableWrapper<TDataContext> wrapper = GetContext(unitOfWork))
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

			using (AmbientUnitOfWorkAwareDisposableWrapper<TDataContext> wrapper = GetContext(unitOfWork))
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

			using (AmbientUnitOfWorkAwareDisposableWrapper<TDataContext> wrapper = GetContext(unitOfWork))
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

			using (AmbientUnitOfWorkAwareDisposableWrapper<TDataContext> wrapper = GetContext(unitOfWork))
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

		public TProjection LinqQuery<TProjection>(Func<TDataContext, TProjection> dataContextQueryCallback)
		{
			TProjection projection;

			if ((object)dataContextQueryCallback == null)
				throw new ArgumentNullException("dataContextQueryCallback");

			if ((object)UnitOfWork.Current == null)
			{
				using (IUnitOfWork unitOfWork = this.GetUnitOfWork())
				{
					projection = this.LinqQuery<TProjection>(unitOfWork, dataContextQueryCallback);

					// HACK ALERT: will this work as expected?
					if (projection is IEnumerable)
						((IEnumerable)projection).Cast<object>().ToList(); // FORCE EAGER LOAD

					unitOfWork.Complete();
				}
			}
			else
			{
				projection = this.LinqQuery<TProjection>(UnitOfWork.Current, dataContextQueryCallback);

				// DO NOT FORCE EAGER LOAD
			}

			return projection;
		}

		public TProjection LinqQuery<TProjection>(IUnitOfWork unitOfWork, Func<TDataContext, TProjection> dataContextQueryCallback)
		{
			TProjection projection;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)dataContextQueryCallback == null)
				throw new ArgumentNullException("dataContextQueryCallback");

			using (AmbientUnitOfWorkAwareDisposableWrapper<TDataContext> wrapper = GetContext(unitOfWork))
			{
				projection = dataContextQueryCallback(wrapper.Disposable);

				if ((object)projection == null)
					throw new InvalidOperationException(string.Format("The projection returned was invalid."));

				return projection;
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

			using (AmbientUnitOfWorkAwareDisposableWrapper<TDataContext> wrapper = GetContext(unitOfWork))
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

		/// <summary>
		/// For a given unitOfWork, this method returns a AmbientUnitOfWorkAwareDisposableWrapper`1 for a target data context type.
		/// </summary>
		/// <param name="unitOfWork"> The target unitOfWork. </param>
		/// <returns> An instance of a AmbientUnitOfWorkAwareDisposableWrapper`1 for the requested data context type, associated withthe unitOfWork. </returns>
		public static AmbientUnitOfWorkAwareDisposableWrapper<TDataContext> GetContext(IUnitOfWork unitOfWork)
		{
			Type dataContextType;
			TDataContext dataContext;
			AmbientUnitOfWorkAwareDisposableWrapper<TDataContext> ambientUnitOfWorkAwareDisposableWrapper;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			dataContextType = typeof(TDataContext);
			dataContext = (TDataContext)GetDataContext(unitOfWork, dataContextType);
			ambientUnitOfWorkAwareDisposableWrapper = new AmbientUnitOfWorkAwareDisposableWrapper<TDataContext>(unitOfWork, dataContext);

			return ambientUnitOfWorkAwareDisposableWrapper;
		}

		/// <summary>
		/// For a given unitOfWork, this method returns a DataContext of the target data context type.
		/// </summary>
		/// <param name="unitOfWork"> The target unitOfWork. </param>
		/// <param name="dataContextType"> The desired data context type. </param>
		/// <returns> An instance of the requested data context type, associated withthe unitOfWork. </returns>
		private static DataContext GetDataContext(IUnitOfWork unitOfWork, Type dataContextType)
		{
			DataContext dataContext;
			MulticastContext<DataContext> multicastContext;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)dataContextType == null)
				throw new ArgumentNullException("dataContextType");

			if ((object)unitOfWork.Context != null)
			{
				multicastContext = unitOfWork.Context as MulticastContext<DataContext>;

				// will fail if not correct type (e.g. DataContext, ObjectContext, etc.)
				if ((object)multicastContext == null)
					throw new InvalidOperationException("Multicast context type obtained from the current data source transaction context does not match the current multicast context type.");

				if (!multicastContext.HasContext(dataContextType))
				{
					// create DC and add to existing MCC
					dataContext = GetDataContext(dataContextType, unitOfWork.Connection, unitOfWork.Transaction);
					multicastContext.SetContext(dataContextType, dataContext);
				}
				else
				{
					// grab existing DC from existing MCC
					dataContext = multicastContext.GetContext(dataContextType);
				}
			}
			else
			{
				// create DC and add to new MCC
				multicastContext = new MulticastContext<DataContext>();
				dataContext = GetDataContext(dataContextType, unitOfWork.Connection, unitOfWork.Transaction);
				multicastContext.SetContext(dataContextType, dataContext);
				unitOfWork.Context = multicastContext;
			}

			return dataContext;
		}

		/// <summary>
		/// For a given unitOfWork, this method returns a DataContext of the target data context type.
		/// </summary>
		/// <param name="dataContextType"> The desired data context type. </param>
		/// <param name="dbConnection"> The target database connection. </param>
		/// <param name="dbTransaction"> The target database transaction. </param>
		/// <returns> An instance of the requested data context type, associated withthe unitOfWork. </returns>
		/// <returns> </returns>
		private static DataContext GetDataContext(Type dataContextType, IDbConnection dbConnection, IDbTransaction dbTransaction)
		{
			DataContext dataContext;
			MappingSource mappingSource;
			ConstructorInfo constructorInfo;

			if ((object)dataContextType == null)
				throw new ArgumentNullException("dataContextType");

			if ((object)dbConnection == null)
				throw new ArgumentNullException("dbConnection");

			mappingSource = new AttributeMappingSource();
			constructorInfo = dataContextType.GetConstructor(new Type[] { typeof(IDbConnection), typeof(MappingSource) });

			// assumption: reflection constructor contract/attribute-based mapping source
			dataContext = (DataContext)constructorInfo.Invoke(new object[] { dbConnection, mappingSource });

			if ((object)dbTransaction != null)
				dataContext.Transaction = (DbTransaction)dbTransaction;

			return dataContext;
		}

		protected static XElement ToXElement(XmlDocument xmlDocument)
		{
			if ((object)xmlDocument == null)
				throw new ArgumentNullException("xmlDocument");

			using (XmlNodeReader nodeReader = new XmlNodeReader(xmlDocument))
			{
				nodeReader.MoveToContent();
				return XElement.Load(nodeReader);
			}
		}

		protected static XmlDocument ToXmlDocument(XElement xElement)
		{
			XmlDocument xmlDocument;

			if ((object)xElement == null)
				throw new ArgumentNullException("xElement");

			xmlDocument = new XmlDocument();

			using (XmlReader xmlReader = xElement.CreateReader())
				xmlDocument.Load(xmlReader);

			return xmlDocument;
		}

		#endregion
	}
}