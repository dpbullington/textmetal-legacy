/*
	Copyright �2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;

using TextMetal.Middleware.Common.Fascades.AdoNet.UoW;
using TextMetal.Middleware.Data.Models;

namespace TextMetal.Middleware.Data.Impl.MicrosoftLinq
{
	/// <summary>
	/// Entity Framework 6.0+
	/// NOTE: TDbContext must support GetConstructor(new Type[] { typeof(DbConnection), typeof(bool) }).
	/// </summary>
	/// <typeparam name="TDbContext"> </typeparam>
	public abstract class DbContextModelRepository<TDbContext> : ContextModelRepository<TDbContext>
		where TDbContext : DbContext
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the DbContextModelRepository`1 class.
		/// </summary>
		protected DbContextModelRepository()
		{
		}

		#endregion

		#region Methods/Operators

		public override bool Discard<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject tableModelObject)
		{
			bool wasNew;
			DbSet<TTableModelObject> dbSet;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)tableModelObject == null)
				throw new ArgumentNullException("tableModelObject");

			wasNew = tableModelObject.IsNew;
			tableModelObject.Mark();

			if (wasNew)
				return true;

			using (AmbientUnitOfWorkAwareContextWrapper<TDbContext> wrapper = this.GetContext(unitOfWork))
			{
				this.OnPreDeleteTableModel<TTableModelObject>(unitOfWork, tableModelObject);

				dbSet = wrapper.DisposableContext.Set<TTableModelObject>();
				dbSet.Remove(tableModelObject);

				try
				{
					wrapper.DisposableContext.SaveChanges();
				}
				catch (DbUpdateException ex)
				{
					this.OnDiscardConflictTableModel<TTableModelObject>(unitOfWork, tableModelObject);

					return false;
				}

				this.OnPostDeleteTableModel<TTableModelObject>(unitOfWork, tableModelObject);

				tableModelObject.IsNew = false;

				return true;
			}
		}

		public override TReturnProcedureModelObject Execute<TCallProcedureModelObject, TResultProcedureModelObject, TReturnProcedureModelObject>(IUnitOfWork unitOfWork, TCallProcedureModelObject callProcedureModel)
		{
			throw new NotSupportedException();
		}

		public override bool Fill<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject tableModelObject)
		{
			DbSet<TTableModelObject> dbSet;
			DbEntityEntry<TTableModelObject> dbEntityEntry;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			using (AmbientUnitOfWorkAwareContextWrapper<TDbContext> wrapper = this.GetContext(unitOfWork))
			{
				dbSet = wrapper.DisposableContext.Set<TTableModelObject>();

				dbEntityEntry = wrapper.DisposableContext.Entry(tableModelObject);
				dbEntityEntry.Reload();

				return true;
			}
		}

		public override IEnumerable<TTableModelObject> Find<TTableModelObject>(IUnitOfWork unitOfWork, ITableModelQuery tableModelQuery)
		{
			IQueryable<TTableModelObject> tableModelObjects;
			DbSet<TTableModelObject> dbSet;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			using (AmbientUnitOfWorkAwareContextWrapper<TDbContext> wrapper = this.GetContext(unitOfWork))
			{
				dbSet = wrapper.DisposableContext.Set<TTableModelObject>();
				tableModelObjects = dbSet.Where(((LinqTableModelQuery<TTableModelObject>)tableModelQuery).Predicate);

				if ((object)tableModelObjects == null)
					throw new InvalidOperationException(string.Format("The table models returned were invalid."));

				// DOES NOT FORCE EXECUTION AGAINST STORE
				foreach (TTableModelObject tableModelObject in tableModelObjects)
				{
					this.OnSelectTableModel<TTableModelObject>(unitOfWork, tableModelObject);

					yield return tableModelObject; // LAZY PROCESSING INTENT HERE / DO NOT FORCE EAGER LOAD
				}
			}
		}

		protected override TDbContext GetContext(Type contextType, DbConnection dbConnection, DbTransaction dbTransaction)
		{
			TDbContext dbContext;
			ConstructorInfo constructorInfo;

			if ((object)contextType == null)
				throw new ArgumentNullException("contextType");

			if ((object)dbConnection == null)
				throw new ArgumentNullException("dbConnection");

			constructorInfo = contextType.GetConstructor(new Type[] { typeof(DbConnection), typeof(bool) });

			if ((object)constructorInfo == null)
				throw new InvalidOperationException("constructorInfo");

			dbContext = (TDbContext)constructorInfo.Invoke(new object[] { dbConnection, false });

			if ((object)dbTransaction != null)
				dbContext.Database.UseTransaction(dbTransaction);

			return dbContext;
		}

		public override TTableModelObject Load<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject prototypeTableModel)
		{
			TTableModelObject tableModelObject;
			DbSet<TTableModelObject> dbSet;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			using (AmbientUnitOfWorkAwareContextWrapper<TDbContext> wrapper = this.GetContext(unitOfWork))
			{
				dbSet = wrapper.DisposableContext.Set<TTableModelObject>();
				tableModelObject = dbSet.Find(prototypeTableModel.IdValues);

				return tableModelObject;
			}
		}

		protected override bool OnCreateNativeDatabaseFile(string databaseFilePath)
		{
			throw new NotSupportedException();
		}

		public override TProjection Query<TProjection>(IUnitOfWork unitOfWork, Func<TDbContext, TProjection> dbContextQueryCallback)
		{
			TProjection projection;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)dbContextQueryCallback == null)
				throw new ArgumentNullException("dbContextQueryCallback");

			using (AmbientUnitOfWorkAwareContextWrapper<TDbContext> wrapper = this.GetContext(unitOfWork))
			{
				projection = dbContextQueryCallback(wrapper.DisposableContext);

				// do not check for null as this is a valid state for the projection
				return projection;
			}
		}

		public override bool Save<TTableModelObject>(IUnitOfWork unitOfWork, TTableModelObject tableModelObject)
		{
			bool wasNew;
			DbSet dbSet;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)tableModelObject == null)
				throw new ArgumentNullException("tableModelObject");

			wasNew = tableModelObject.IsNew;
			tableModelObject.Mark();

			using (AmbientUnitOfWorkAwareContextWrapper<TDbContext> wrapper = this.GetContext(unitOfWork))
			{
				dbSet = wrapper.DisposableContext.Set<TTableModelObject>();

				if (wasNew)
				{
					this.OnPreInsertTableModel<TTableModelObject>(unitOfWork, tableModelObject);

					dbSet.Add(tableModelObject);
				}
				else
				{
					this.OnPreUpdateTableModel<TTableModelObject>(unitOfWork, tableModelObject);

					//dbSet.Nop(tableModelObject);
				}

				try
				{
					wrapper.DisposableContext.SaveChanges();
				}
				catch (DbUpdateException ex)
				{
					this.OnSaveConflictTableModel<TTableModelObject>(unitOfWork, tableModelObject);

					return false;
				}

				if (wasNew)
					this.OnPostInsertTableModel<TTableModelObject>(unitOfWork, tableModelObject);
				else
					this.OnPostUpdateTableModel<TTableModelObject>(unitOfWork, tableModelObject);

				return true;
			}
		}

		#endregion
	}
}