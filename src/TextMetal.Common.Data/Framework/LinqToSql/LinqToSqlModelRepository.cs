/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;

namespace TextMetal.Common.Data.Framework.LinqToSql
{
	public class LinqToSqlModelRepository<TDataContext> : ModelRepository
		where TDataContext : DataContext, IDisposable
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

		public override TModel Discard<TModel>(TModel model)
		{
			throw new NotImplementedException();
		}

		public override TModel Discard<TModel>(IUnitOfWork unitOfWork, TModel model)
		{
			throw new NotImplementedException();
		}

		public override TResponseModel ExecuteImperative<TRequestModel, TResultModel, TResponseModel>(TRequestModel requestModel)
		{
			throw new NotImplementedException();
		}

		public override TResponseModel ExecuteImperative<TRequestModel, TResultModel, TResponseModel>(IUnitOfWork unitOfWork, TRequestModel requestModel)
		{
			throw new NotImplementedException();
		}

		public override TModel Fill<TModel>(TModel model)
		{
			throw new NotImplementedException();
		}

		public override TModel Fill<TModel>(IUnitOfWork unitOfWork, TModel model)
		{
			throw new NotImplementedException();
		}

		public override IEnumerable<TModel> Find<TModel>(IModelQuery query)
		{
			IEnumerable<TModel> models;

			if ((object)query == null)
				throw new ArgumentNullException("query");

			if ((object)UnitOfWork.Current == null)
			{
				using (IUnitOfWork unitOfWork = this.GetUnitOfWork())
				{
					models = this.Find<TModel>(unitOfWork, query);

					models = models.ToList(); // FORCE EAGER LOAD

					unitOfWork.Complete();
				}
			}
			else
			{
				models = this.Find<TModel>(UnitOfWork.Current, query);

				// DO NOT FORCE EAGER LOAD
			}

			return models;
		}

		public override IEnumerable<TModel> Find<TModel>(IUnitOfWork unitOfWork, IModelQuery query)
		{
			return null;
			/*IEnumerable<TModel> models;
			IQueryable<TTable> queryable;
			Func<IQueryable<TTable>, IQueryable<TTable>> callback;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)query == null)
				throw new ArgumentNullException("query");

			callback = (Func<IQueryable<TTable>, IQueryable<TTable>>)query.GetNativeReduction();

			using (AmbientUnitOfWorkAwareDisposableWrapper<TDataContext> wrapper = unitOfWork.GetContext<TDataContext>())
			{
				queryable = callback((TTable)wrapper.Disposable);

				if ((object)queryable == null)
					throw new InvalidOperationException(string.Format("The queryable returned was invalid."));

				models = queryable.Select(blahblahblah); // DOES NOT FORCE EXECUTION AGAINST STORE

				foreach (TModel model in models)
				{
					this.OnSelectModel<TModel>(unitOfWork, model);
					//model.Mark();
					yield return model; // LAZY PROCESSING INTENT HERE / DO NOT FORCE EAGER LOAD
				}
			}*/
		}

		public override TModel Load<TModel>(TModel prototype)
		{
			throw new NotImplementedException();
		}

		public override TModel Load<TModel>(IUnitOfWork unitOfWork, TModel prototype)
		{
			throw new NotImplementedException();
		}

		public override TModel Save<TModel>(TModel model)
		{
			throw new NotImplementedException();
		}

		public override TModel Save<TModel>(IUnitOfWork unitOfWork, TModel model)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}