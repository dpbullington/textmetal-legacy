/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

/* CERTIFICATION OF UNIT TESTING: dpbullington@gmail.com / 2016-04-01 / 83% code coverage */

namespace TextMetal.Middleware.Solder.Injection
{
	/// <summary>
	/// A dependency resolution implementation that uses Activator.CreateInstance(...)
	/// on the activation type each time a dependency resolution occurs and is the only
	/// implementation that allows for auto-wiring using the DependencyInjectionAttribute.
	/// </summary>
	public sealed class TransientActivatorAutoWiringDependencyResolution<TResolution> : DependencyResolution<TResolution>
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the TransientActivatorAutoWiringDependencyResolution`1 class.
		/// </summary>
		public TransientActivatorAutoWiringDependencyResolution()
			: base(DependencyLifetime.Transient)
		{
		}

		#endregion

		#region Methods/Operators

		protected override TResolution CoreResolve(IDependencyManager dependencyManager, string selectorKey)
		{
			if ((object)dependencyManager == null)
				throw new ArgumentNullException(nameof(dependencyManager));

			if ((object)selectorKey == null)
				throw new ArgumentNullException(nameof(selectorKey));

			return TransientActivatorAutoWiringDependencyResolution.AutoWireResolve<TResolution>(typeof(TResolution), dependencyManager, typeof(TResolution), selectorKey);
		}

		protected override void Dispose(bool disposing)
		{
			// do nothing
		}

		#endregion
	}
}