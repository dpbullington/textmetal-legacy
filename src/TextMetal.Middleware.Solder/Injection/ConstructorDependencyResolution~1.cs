/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Middleware.Solder.Injection
{
	/// <summary>
	/// Provides the Factory Method pattern used to resolve dependencies.
	/// Requires a public, default constructor on the target type.
	/// </summary>
	/// <typeparam name="TObject"> The actual type of the resolution. </typeparam>
	public sealed class ConstructorDependencyResolution<TObject> : IDependencyResolution
		where TObject : new()
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the ConstructorDependencyResolution`1 class.
		/// </summary>
		public ConstructorDependencyResolution()
		{
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Resolves a dependency.
		/// </summary>
		/// <param name="dependencyManager"> The current in-effect dependency manager requesting this resolution. </param>
		/// <returns> An instance of an object or null. </returns>
		public object Resolve(IDependencyManager dependencyManager)
		{
			if ((object)dependencyManager == null)
				throw new ArgumentNullException("dependencyManager");

			return new TObject();
		}

		#endregion
	}
}