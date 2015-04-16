/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Data;

using TextMetal.Middleware.Common.Fascades.AdoNet.UoW;
using TextMetal.Middleware.Data.Models;
using TextMetal.Middleware.Solder.IoC;

namespace TextMetal.Middleware.Data.Containers
{
	public abstract class ServiceContainer<TRepository> : ServiceContainer, IServiceContainer<TRepository>
		where TRepository : class, IModelRepository
	{
		#region Constructors/Destructors

		protected ServiceContainer()
			: this(DependencyManager.AppDomainInstance.ResolveDependency<TRepository>(string.Empty))
		{
		}

		protected ServiceContainer(TRepository repository)
		{
			if ((object)repository == null)
				throw new ArgumentNullException("repository");

			this.repository = repository;
		}

		#endregion

		#region Fields/Constants

		private readonly TRepository repository;

		#endregion

		#region Properties/Indexers/Events

		public TRepository Repository
		{
			get
			{
				return this.repository;
			}
		}

		#endregion

		#region Methods/Operators

		public override IUnitOfWork GetUnitOfWork(bool transactional, IsolationLevel isolationLevel = IsolationLevel.Unspecified)
		{
			return this.Repository.GetUnitOfWork(transactional, isolationLevel);
		}

		#endregion
	}
}