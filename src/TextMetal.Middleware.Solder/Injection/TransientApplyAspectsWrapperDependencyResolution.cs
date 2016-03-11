/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Middleware.Solder.Interception;

namespace TextMetal.Middleware.Solder.Injection
{
	public sealed class TransientApplyAspectsWrapperDependencyResolution : IDependencyResolution
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the TransientApplyAspectsWrapperDependencyResolution class.
		/// </summary>
		/// <param name="chainedDependencyResolution"> The singleton instance. </param>
		public TransientApplyAspectsWrapperDependencyResolution(IDependencyResolution chainedDependencyResolution)
		{
			if ((object)chainedDependencyResolution == null)
				throw new ArgumentNullException(nameof(chainedDependencyResolution));

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

		public void Dispose()
		{
			// do nothing
		}

		private object GetDynamicProxy(Type interceptedInstanceType, LoggingAspectDynamicInvoker loggingAspectDynamicInvoker)
		{
			return interceptedInstanceType;
		}

		/// <summary>
		/// Resolves a dependency.
		/// </summary>
		/// <param name="dependencyManager"> The current in-effect dependency manager requesting this resolution. </param>
		/// <returns> An instance of an object or null. </returns>
		public object Resolve(IDependencyManager dependencyManager)
		{
			object interceptedInstance;
			Type interceptedInstanceType;
			object wrapperInstance;

			if ((object)dependencyManager == null)
				throw new ArgumentNullException(nameof(dependencyManager));

			interceptedInstance = this.ChainedDependencyResolution.Resolve(dependencyManager);

			if ((object)interceptedInstance == null)
				return null;

			interceptedInstanceType = interceptedInstance.GetType();
			wrapperInstance = this.GetDynamicProxy(interceptedInstanceType, new LoggingAspectDynamicInvoker(interceptedInstance));

			return wrapperInstance;
		}

		#endregion
	}
}