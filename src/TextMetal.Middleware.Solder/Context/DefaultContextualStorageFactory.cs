/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

/* CERTIFICATION OF UNIT TESTING: dpbullington@gmail.com / 2016-02-22 / 86% code coverage */

namespace TextMetal.Middleware.Solder.Context
{
	/// <summary>
	/// Manages execution path storage of objects in a manner which is safe in standard executables, libraries, ASP.NET, and WCF code.
	/// </summary>
	public class DefaultContextualStorageFactory : IContextualStorageFactory
	{
		#region Constructors/Destructors

		public DefaultContextualStorageFactory()
		{
		}

		#endregion

		#region Methods/Operators

		public IContextualStorageStrategy GetContextualStorage()
		{
			HttpContextAccessorContextualStorageStrategy httpContextAccessorContextualStorageStrategy;
			IContextualStorageStrategy contextualStorageStrategy;

			httpContextAccessorContextualStorageStrategy = new HttpContextAccessorContextualStorageStrategy(null);

			if (httpContextAccessorContextualStorageStrategy.IsValidHttpContext)
				contextualStorageStrategy = httpContextAccessorContextualStorageStrategy;
			else
				contextualStorageStrategy = new ThreadLocalContextualStorageStrategy();

			return contextualStorageStrategy;
		}

		#endregion
	}
}