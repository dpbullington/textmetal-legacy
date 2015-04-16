/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Data;

using TextMetal.Middleware.Common.Fascades.AdoNet.UoW;

namespace TextMetal.Middleware.Data.Containers
{
	public abstract class ServiceContainer : IServiceContainer
	{
		#region Constructors/Destructors

		protected ServiceContainer()
		{
		}

		#endregion

		#region Methods/Operators

		public abstract IUnitOfWork GetUnitOfWork(bool transactional, IsolationLevel isolationLevel = IsolationLevel.Unspecified);

		#endregion
	}
}