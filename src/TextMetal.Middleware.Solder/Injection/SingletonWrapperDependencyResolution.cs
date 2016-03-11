/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Threading;

using TextMetal.Middleware.Solder.Runtime;

/* CERTIFICATION OF UNIT TESTING: dpbullington@gmail.com / 2016-03-10 / 99% code coverage */

namespace TextMetal.Middleware.Solder.Injection
{
	/// <summary>
	/// A composing dependency resolution implementation that allows only a specific instance
	/// to be lazy created on demand/first use, cached, and reused; the specific instance is
	/// retrieved from the inner dependency resolution once per the lifetime of this dependency resolution instance.
	/// Uses reader-writer lock for asynchronous protection (i.e. thread-safety).
	/// From 'Dependency Injection in ASP.NET MVC6':
	/// Singleton lifetime services are created the first time they are requested, and then every subsequent request will use the same instance. If your application requires singleton behavior, allowing the services container to manage the service’s lifetime is recommended instead of implementing the singleton design pattern and managing your object’s lifetime in the class yourself.
	/// </summary>
	public sealed class SingletonWrapperDependencyResolution : IDependencyResolution
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the SingletonWrapperDependencyResolution class.
		/// Lazy loading semantics are enabled by default.
		/// </summary>
		/// <param name="innerDependencyResolution"> The chained dependency resolution which is called only once. </param>
		public SingletonWrapperDependencyResolution(IDependencyResolution innerDependencyResolution)
			: this(false, innerDependencyResolution)
		{
		}

		/// <summary>
		/// Initializes a new instance of the SingletonWrapperDependencyResolution class.
		/// </summary>
		/// <param name="eagerLoad"> Force eager loading using the assembly loader container context dependency manager. </param>
		/// <param name="innerDependencyResolution"> The chained dependency resolution which is called only once. </param>
		public SingletonWrapperDependencyResolution(bool eagerLoad, IDependencyResolution innerDependencyResolution)
		{
			if ((object)innerDependencyResolution == null)
				throw new ArgumentNullException(nameof(innerDependencyResolution));

			this.innerDependencyResolution = innerDependencyResolution;

			if (eagerLoad)
				this.ThreadSafeLoadSingleton(AssemblyLoaderContainerContext.TheOnlyAllowedInstance.DependencyManager);
		}

		#endregion

		#region Fields/Constants

		private readonly IDependencyResolution innerDependencyResolution;

		private readonly ReaderWriterLockSlim readerWriterLock = new ReaderWriterLockSlim();
		private bool frozen;
		private object instance;

		#endregion

		#region Properties/Indexers/Events

		private IDependencyResolution InnerDependencyResolution
		{
			get
			{
				return this.innerDependencyResolution;
			}
		}

		private ReaderWriterLockSlim ReaderWriterLock
		{
			get
			{
				return this.readerWriterLock;
			}
		}

		private bool Frozen
		{
			get
			{
				return this.frozen;
			}
			set
			{
				this.frozen = value;
			}
		}

		private object Instance
		{
			get
			{
				return this.instance;
			}
			set
			{
				this.instance = value;
			}
		}

		#endregion

		#region Methods/Operators

		public void Dispose()
		{
			if ((object)this.InnerDependencyResolution != null)
				this.InnerDependencyResolution.Dispose();
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

			return this.ThreadSafeLoadSingleton(dependencyManager);
		}

		private object ThreadSafeLoadSingleton(IDependencyManager dependencyManager)
		{
			if ((object)dependencyManager == null)
				throw new ArgumentNullException(nameof(dependencyManager));

			// cop a reader lock
			this.ReaderWriterLock.EnterUpgradeableReadLock();

			try
			{
				if (this.Frozen)
					return this.Instance;

				// cop a writer lock
				this.ReaderWriterLock.EnterWriteLock();

				try
				{
					this.Instance = this.InnerDependencyResolution.Resolve(dependencyManager);
					return this.Instance;
				}
				finally
				{
					this.Frozen = true;
					this.ReaderWriterLock.ExitWriteLock();
				}
			}
			finally
			{
				this.ReaderWriterLock.ExitUpgradeableReadLock();
			}
		}

		#endregion
	}
}