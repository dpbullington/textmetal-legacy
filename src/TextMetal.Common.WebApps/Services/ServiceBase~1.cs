/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Common.Data.Framework;
using TextMetal.Common.Solder.DependencyManagement;

namespace TextMetal.Common.WebApps.Services
{
	public abstract class ServiceBase<TRepository>
		where TRepository : class, IModelRepository
	{
		#region Constructors/Destructors

		protected ServiceBase()
			: this(DependencyManager.AppDomainInstance.ResolveDependency<TRepository>(string.Empty))
		{
		}

		protected ServiceBase(TRepository repository)
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

		protected TRepository Repository
		{
			get
			{
				return this.repository;
			}
		}

		#endregion
	}
}