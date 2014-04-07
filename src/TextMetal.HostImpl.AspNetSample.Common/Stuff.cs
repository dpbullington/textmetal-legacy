/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Common.Solder.DependencyManagement;

namespace TextMetal.HostImpl.AspNetSample.Common
{
	public static class Stuff
	{
		#region Properties/Indexers/Events

		public static ISession Session
		{
			get
			{
				return DependencyManager.AppDomainInstance.ResolveDependency<ISession>("NonStickySession");
			}
		}

		public static ISession StickySession
		{
			get
			{
				return DependencyManager.AppDomainInstance.ResolveDependency<ISession>("StickySession");
			}
		}

		#endregion

		#region Methods/Operators

		public static T Get<T>(string selectorKey)
		{
			return DependencyManager.AppDomainInstance.ResolveDependency<T>(selectorKey);
		}

		public static void Set<Tx, Ty>(string selectorKey)
			where Ty : Tx, new()
		{
			DependencyManager.AppDomainInstance.AddResolution<Tx>(selectorKey, new ConstructorDependencyResolution<Ty>());
		}

		public static void SetNewSgltn<Tx, Ty>(string selectorKey)
			where Ty : Tx, new()
		{
			// HACK
			DependencyManager.AppDomainInstance.AddResolution<Tx>(selectorKey, new SingletonDependencyResolution(new Ty()));
		}

		#endregion
	}
}