/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

/* CERTIFICATION OF UNIT TESTING: dpbullington@gmail.com / 2016-03-10 / 100% code coverage */

namespace TextMetal.Middleware.Solder.Injection
{
	/// <summary>
	/// A dependency resolution implementation that executes a
	/// factory method callback each time a dependency resolution occurs.
	/// From 'Dependency Injection in ASP.NET MVC6':
	/// Transient lifetime services are created each time they are requested. This lifetime works best for lightweight, stateless service.
	/// </summary>
	public sealed class TransientFactoryMethodDependencyResolution : IDependencyResolution
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the TransientFactoryMethodDependencyResolution class.
		/// </summary>
		/// <param name="factoryMethod"> The callback method to execute during resolution. </param>
		public TransientFactoryMethodDependencyResolution(Delegate factoryMethod)
		{
			if ((object)factoryMethod == null)
				throw new ArgumentNullException(nameof(factoryMethod));

			this.factoryMethod = factoryMethod;
		}

		#endregion

		#region Fields/Constants

		private readonly Delegate factoryMethod;

		#endregion

		#region Properties/Indexers/Events

		private Delegate FactoryMethod
		{
			get
			{
				return this.factoryMethod;
			}
		}

		#endregion

		#region Methods/Operators

		public static TransientFactoryMethodDependencyResolution Create<TObject>(Func<TObject> factoryMethod)
		{
			if ((object)factoryMethod == null)
				throw new ArgumentNullException(nameof(factoryMethod));

			return new TransientFactoryMethodDependencyResolution(factoryMethod);
		}

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
				throw new ArgumentNullException(nameof(dependencyManager));

			return this.FactoryMethod.DynamicInvoke(null);
		}

		#endregion
	}
}