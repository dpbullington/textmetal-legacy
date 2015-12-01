/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Middleware.Solder.Injection
{
	/// <summary>
	/// Provides the Factory Method pattern used to resolve dependencies.
	/// This implementation executes a callback each time a dependency resolution occurs.
	/// </summary>
	public sealed class DelegateDependencyResolution<TObject> : IDependencyResolution
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the DelegateDependencyResolution`1 class.
		/// </summary>
		/// <param name="factoryCallback"> The callback method to execute during resolution. </param>
		public DelegateDependencyResolution(Func<TObject> factoryCallback)
		{
			if ((object)factoryCallback == null)
				throw new ArgumentNullException("factoryCallback");

			this.factoryCallback = factoryCallback;
		}

		#endregion

		#region Fields/Constants

		private readonly Func<TObject> factoryCallback;

		#endregion

		#region Properties/Indexers/Events

		private Func<TObject> FactoryCallback
		{
			get
			{
				return this.factoryCallback;
			}
		}

		#endregion

		#region Methods/Operators

		public void Dispose()
		{
			// do nothing
		}

		/// <summary>
		/// Resolves a dependency.
		/// </summary>
		/// <param name="dependencyManager"> The current in-effect dependency manager requesting this resolution. </param>
		/// <returns> An instance of an object or null. </returns>
		public object Resolve(IDependencyManager dependencyManager)
		{
			if ((object)dependencyManager == null)
				throw new ArgumentNullException("dependencyManager");

			return this.FactoryCallback();
		}

		#endregion
	}
}