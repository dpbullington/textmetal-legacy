/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

/* CERTIFICATION OF UNIT TESTING: dpbullington@gmail.com / 2016-03-10 / 100% code coverage */

namespace TextMetal.Middleware.Solder.Injection
{
	/// <summary>
	/// A dependency resolution implementation that executes a public, default constructor
	/// on the target type each time a dependency resolution occurs.
	/// From 'Dependency Injection in ASP.NET MVC6':
	/// Transient lifetime services are created each time they are requested. This lifetime works best for lightweight, stateless service.
	/// </summary>
	public class TransientDefaultConstructorDependencyResolution : IDependencyResolution

	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the TransientDefaultConstructorDependencyResolution class.
		/// </summary>
		public TransientDefaultConstructorDependencyResolution(Type targetType)
		{
			if ((object)targetType == null)
				throw new ArgumentNullException(nameof(targetType));

			this.targetType = targetType;
		}

		#endregion

		#region Fields/Constants

		private readonly Type targetType;

		#endregion

		#region Properties/Indexers/Events

		private Type TargetType
		{
			get
			{
				return this.targetType;
			}
		}

		#endregion

		#region Methods/Operators

		public static IDependencyResolution New<TObject>()
			where TObject : new()
		{
			return TransientFactoryMethodDependencyResolution.Create<TObject>(() => new TObject());
		}

		public static TransientDefaultConstructorDependencyResolution Create<TObject>()
		{
			return new TransientDefaultConstructorDependencyResolution(typeof(TObject));
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

			return Activator.CreateInstance(this.TargetType);
		}

		#endregion
	}
}