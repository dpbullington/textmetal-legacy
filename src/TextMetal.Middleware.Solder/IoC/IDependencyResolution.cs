/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Middleware.Solder.IoC
{
	/// <summary>
	/// Provides the Factory Method pattern used to resolve dependencies.
	/// </summary>
	public interface IDependencyResolution
	{
		#region Methods/Operators

		/// <summary>
		/// Resolves a dependency.
		/// </summary>
		/// <param name="dependencyManager"> The current in-effect dependency manager requesting this resolution. </param>
		/// <returns> An instance of an object or null. </returns>
		object Resolve(IDependencyManager dependencyManager);

		#endregion
	}
}