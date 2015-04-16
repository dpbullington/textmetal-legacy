/*
	Copyright �2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Middleware.Solder.SoC;
using TextMetal.Middleware.Solder.SoC.RemotingImpl;

namespace TextMetal.Middleware.Solder.IoC
{
	/// <summary>
	/// TODO
	/// </summary>
	/// <typeparam name="TObject"> The actual type of the resolution. </typeparam>
	public sealed class LoggingAspectConstructorDependencyResolution<TObject> : IDependencyResolution
		where TObject : class
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the LoggingAspectConstructorDependencyResolution`1 class.
		/// </summary>
		/// <param name="chainedDependencyResolution"> The singleton instance. </param>
		public LoggingAspectConstructorDependencyResolution(IDependencyResolution chainedDependencyResolution)
		{
			if ((object)chainedDependencyResolution == null)
				throw new ArgumentNullException("chainedDependencyResolution");

			this.chainedDependencyResolution = chainedDependencyResolution;
		}

		#endregion

		#region Fields/Constants

		private readonly IDependencyResolution chainedDependencyResolution;

		#endregion

		#region Properties/Indexers/Events

		private IDependencyResolution ChainedDependencyResolution
		{
			get
			{
				return this.chainedDependencyResolution;
			}
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
			object interceptedInstance;
			TObject wrapperInstance;

			if ((object)dependencyManager == null)
				throw new ArgumentNullException("dependencyManager");

			interceptedInstance = this.ChainedDependencyResolution.Resolve(dependencyManager);

			if ((object)interceptedInstance == null)
				return null;

			wrapperInstance = (TObject)new DynamicInvokerRealProxy<TObject>(new LoggingAspectDynamicInvoker(interceptedInstance)).GetTransparentProxy();

			return wrapperInstance;
		}

		#endregion
	}
}