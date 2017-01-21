/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Middleware.Solder.Utilities;

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
			: this(AssemblyDependencyDomain.Default.DependencyManager.ResolveDependency<IReflectionFascade>(string.Empty, false))
		{
		}

		public TransientActivatorAutoWiringDependencyResolution(IReflectionFascade reflectionFascade)
			: base(DependencyLifetime.Transient)
		{
			if ((object)reflectionFascade == null)
				throw new ArgumentNullException(nameof(reflectionFascade));

			this.reflectionFascade = reflectionFascade;
		}

		#endregion

		#region Fields/Constants

		private readonly IReflectionFascade reflectionFascade;

		#endregion

		#region Properties/Indexers/Events

		private IReflectionFascade ReflectionFascade
		{
			get
			{
				return this.reflectionFascade;
			}
		}

		#endregion

		#region Methods/Operators

		protected override TResolution CoreResolve(IDependencyManager dependencyManager, string selectorKey)
		{
			if ((object)dependencyManager == null)
				throw new ArgumentNullException(nameof(dependencyManager));

			if ((object)selectorKey == null)
				throw new ArgumentNullException(nameof(selectorKey));

			return TransientActivatorAutoWiringDependencyResolution.AutoWireResolve<TResolution>(this.ReflectionFascade, typeof(TResolution), dependencyManager, typeof(TResolution), selectorKey);
		}

		protected override void Dispose(bool disposing)
		{
			// do nothing
		}

		#endregion
	}
}