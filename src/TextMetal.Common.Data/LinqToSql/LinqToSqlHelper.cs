/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Data;
using System.Data.Common;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Reflection;

namespace TextMetal.Common.Data.LinqToSql
{
	/// <summary>
	/// Provides static helper and/or extension methods for LINQ to SQL.
	/// </summary>
	public static class LinqToSqlHelper
	{
		#region Methods/Operators

		/// <summary>
		/// For a given unitOfWork, this method returns a AmbientUnitOfWorkAwareDisposableWrapper`1 for a target data context type.
		/// </summary>
		/// <typeparam name="TContext"> The desired data context type. </typeparam>
		/// <param name="unitOfWork"> The target unitOfWork. </param>
		/// <returns> An instance of a AmbientUnitOfWorkAwareDisposableWrapper`1 for the requested data context type, associated withthe unitOfWork. </returns>
		public static AmbientUnitOfWorkAwareDisposableWrapper<TContext> GetContext<TContext>(this IUnitOfWork unitOfWork)
			where TContext : class, IDisposable
		{
			Type contextType, dataContextType;
			TContext dataContext;
			AmbientUnitOfWorkAwareDisposableWrapper<TContext> ambientUnitOfWorkAwareDisposableWrapper;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			// assumption: LINQ to SQL DataContext derived ambientUnitOfWorkAwareDisposable types are only supported
			// will support Entity Framework *Context types later, if possible.
			dataContextType = typeof(DataContext);
			contextType = typeof(TContext);

			if (!dataContextType.IsAssignableFrom(contextType))
				throw new NotSupportedException(string.Format("The (data) ambientUnitOfWorkAwareDisposable type '{0}' is not supported.", contextType.FullName));

			dataContext = (TContext)(object)GetDataContext(unitOfWork, contextType);
			ambientUnitOfWorkAwareDisposableWrapper = new AmbientUnitOfWorkAwareDisposableWrapper<TContext>(unitOfWork, dataContext);

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

		#endregion
	}
}