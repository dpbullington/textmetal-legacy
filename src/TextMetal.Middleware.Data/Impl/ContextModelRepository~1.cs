/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Data.Common;

using TextMetal.Middleware.Data.Repositories;
using TextMetal.Middleware.Data.UoW;

namespace TextMetal.Middleware.Data.Impl
{
	public abstract class ContextModelRepository<TContext> : ModelRepository<TContext>
		where TContext : class, IDisposable
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the ContextModelRepository`1 class.
		/// </summary>
		protected ContextModelRepository()
		{
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// For a given unit of work, this method returns a AmbientUnitOfWorkAwareContextWrapper`1 for a target data context type.
		/// </summary>
		/// <param name="unitOfWork"> The target unitOfWork. </param>
		/// <returns> An instance of a AmbientUnitOfWorkAwareContextWrapper`1 for the requested data context type, associated withthe unitOfWork. </returns>
		protected AmbientUnitOfWorkAwareContextWrapper<TContext> GetContext(IUnitOfWork unitOfWork)
		{
			Type contextType;
			TContext context;
			AmbientUnitOfWorkAwareContextWrapper<TContext> ambientUnitOfWorkAwareContextWrapper;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			contextType = typeof(TContext);
			context = (TContext)this.GetContext(unitOfWork, contextType);
			ambientUnitOfWorkAwareContextWrapper = new AmbientUnitOfWorkAwareContextWrapper<TContext>(unitOfWork, context);

			return ambientUnitOfWorkAwareContextWrapper;
		}

		/// <summary>
		/// For a given unit of work, this method returns a Context of the target data context type.
		/// </summary>
		/// <param name="unitOfWork"> The target unitOfWork. </param>
		/// <param name="contextType"> The desired data context type. </param>
		/// <returns> An instance of the requested data context type, associated withthe unitOfWork. </returns>
		private TContext GetContext(IUnitOfWork unitOfWork, Type contextType)
		{
			TContext context;
			MulticastDisposableContext<TContext> multicastDisposableContext;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)contextType == null)
				throw new ArgumentNullException("contextType");

			if ((object)unitOfWork.Context != null)
			{
				multicastDisposableContext = unitOfWork.Context as MulticastDisposableContext<TContext>;

				// will fail if not correct type (e.g. DataContext, DbContext, etc.)
				if ((object)multicastDisposableContext == null)
					throw new InvalidOperationException("Multicast context type obtained from the current data source transaction context does not match the current multicast context type.");

				if (!multicastDisposableContext.HasContext(contextType))
				{
					// create DC and add to existing MCC
					context = this.GetContext(contextType, (DbConnection)unitOfWork.Connection, (DbTransaction)unitOfWork.Transaction);
					multicastDisposableContext.SetContext(contextType, context);
				}
				else
				{
					// grab existing DC from existing MCC
					context = multicastDisposableContext.GetContext(contextType);
				}
			}
			else
			{
				// create DC and add to new MCC
				multicastDisposableContext = new MulticastDisposableContext<TContext>();
				context = this.GetContext(contextType, (DbConnection)unitOfWork.Connection, (DbTransaction)unitOfWork.Transaction);
				multicastDisposableContext.SetContext(contextType, context);
				unitOfWork.Context = multicastDisposableContext;
			}

			return context;
		}

		protected abstract TContext GetContext(Type contextType, DbConnection dbConnection, DbTransaction dbTransaction);

		public override TProjection Query<TProjection>(IUnitOfWork unitOfWork, Func<TContext, TProjection> contextQueryCallback)
		{
			TProjection projection;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)contextQueryCallback == null)
				throw new ArgumentNullException("contextQueryCallback");

			using (AmbientUnitOfWorkAwareContextWrapper<TContext> wrapper = this.GetContext(unitOfWork))
			{
				projection = contextQueryCallback(wrapper.DisposableContext);

				// do not check for null as this is a valid state for the projection
				return projection;
			}
		}

		#endregion
	}
}