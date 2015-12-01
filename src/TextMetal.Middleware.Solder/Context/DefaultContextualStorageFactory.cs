/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

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

		#region Fields/Constants

		private static readonly DefaultContextualStorageFactory instance = new DefaultContextualStorageFactory();

		#endregion

		#region Properties/Indexers/Events

		public static DefaultContextualStorageFactory Instance
		{
			get
			{
				return instance;
			}
		}

		#endregion

		#region Methods/Operators

		public IContextualStorageStrategy GetContextualStorage()
		{
			IContextualStorageStrategy contextualStorageStrategy;

			if (WcfContextContextualStorageStrategy.IsInWcfContext)
				contextualStorageStrategy = new WcfContextContextualStorageStrategy();
			else if (HttpContextContextualStorageStrategy.IsInHttpContext)
				contextualStorageStrategy = new HttpContextContextualStorageStrategy();
			else /* WinForms/WPF/Console/Service */
				contextualStorageStrategy = new ThreadStaticContextualStorageStrategy();

			return contextualStorageStrategy;
		}

		#endregion
	}
}