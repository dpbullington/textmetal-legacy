/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

/* CERTIFICATION OF UNIT TESTING: dpbullington@gmail.com / 2016-03-10 / 99% code coverage */

namespace TextMetal.Middleware.Solder.Injection
{
	/// <summary>
	/// Provides dependency registration and resolution services.
	/// Uses reader-writer lock for asynchronous protection (i.e. thread-safety).
	/// </summary>
	public sealed class DependencyManager : IDependencyManager
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the DependencyManager class.
		/// </summary>
		public DependencyManager()
		{
		}

		#endregion

		#region Fields/Constants

		private readonly Dictionary<Tuple<Type, string>, IDependencyResolution> dependencyResolutionRegistrations = new Dictionary<Tuple<Type, string>, IDependencyResolution>();
		private readonly ReaderWriterLockSlim readerWriterLock = new ReaderWriterLockSlim();
		private bool disposed;

		#endregion

		#region Properties/Indexers/Events

		private Dictionary<Tuple<Type, string>, IDependencyResolution> DependencyResolutionRegistrations
		{
			get
			{
				return this.dependencyResolutionRegistrations;
			}
		}

		private ReaderWriterLockSlim ReaderWriterLock
		{
			get
			{
				return this.readerWriterLock;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the current instance has been disposed.
		/// </summary>
		public bool Disposed
		{
			get
			{
				return this.disposed;
			}
			private set
			{
				this.disposed = value;
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Adds a new dependency resolution for a given target type and selector key. Throws a DependencyException if the target type and selector key combination has been previously registered in this instance. This is the generic overload.
		/// </summary>
		/// <typeparam name="TObject"> The target type of resolution. </typeparam>
		/// <param name="selectorKey"> An non-null, zero or greater length string selector key. </param>
		/// <param name="includeAssignableTypes"> A boolean value indicating whether to include assignable types in the candidate resolution existence check. </param>
		/// <param name="dependencyResolution"> The dependency resolution. </param>
		public void AddResolution<TObject>(string selectorKey, bool includeAssignableTypes, IDependencyResolution dependencyResolution)
		{
			Type targetType;

			if (this.Disposed)
				throw new ObjectDisposedException(typeof(DependencyManager).FullName);

			if ((object)selectorKey == null)
				throw new ArgumentNullException(nameof(selectorKey));

			if ((object)dependencyResolution == null)
				throw new ArgumentNullException(nameof(dependencyResolution));

			targetType = typeof(TObject);

			this.AddResolution(targetType, selectorKey, includeAssignableTypes, dependencyResolution);
		}

		/// <summary>
		/// Adds a new dependency resolution for a given target type and selector key. Throws a DependencyException if the target type and selector key combination has been previously registered in this instance. This is the non-generic overload.
		/// </summary>
		/// <param name="targetType"> The target type of resolution. </param>
		/// <param name="selectorKey"> An non-null, zero or greater length string selector key. </param>
		/// <param name="includeAssignableTypes"> A boolean value indicating whether to include assignable types in the candidate resolution existence check. </param>
		/// <param name="dependencyResolution"> The dependency resolution. </param>
		public void AddResolution(Type targetType, string selectorKey, bool includeAssignableTypes, IDependencyResolution dependencyResolution)
		{
			Tuple<Type, string> trait;
			IEnumerable<KeyValuePair<Tuple<Type, string>, IDependencyResolution>> candidateResolutions;

			if (this.Disposed)
				throw new ObjectDisposedException(typeof(DependencyManager).FullName);

			if ((object)targetType == null)
				throw new ArgumentNullException(nameof(targetType));

			if ((object)selectorKey == null)
				throw new ArgumentNullException(nameof(selectorKey));

			if ((object)dependencyResolution == null)
				throw new ArgumentNullException(nameof(dependencyResolution));

			// cop a reader lock
			this.ReaderWriterLock.EnterUpgradeableReadLock();

			try
			{
				// cop a writer lock
				this.ReaderWriterLock.EnterWriteLock();

				try
				{
					trait = new Tuple<Type, string>(targetType, selectorKey);
					candidateResolutions = this.GetCandidateResolutions(targetType, selectorKey, includeAssignableTypes);

					if (candidateResolutions.Any())
						throw new DependencyException(string.Format("Dependency resolution already exists in the dependency manager for target type '{0}' and selector key '{1}' (include assignable types: '{2}').", targetType.FullName, selectorKey, includeAssignableTypes));

					this.DependencyResolutionRegistrations.Add(trait, dependencyResolution);
				}
				finally
				{
					this.ReaderWriterLock.ExitWriteLock();
				}
			}
			finally
			{
				this.ReaderWriterLock.ExitUpgradeableReadLock();
			}
		}

		/// <summary>
		/// Clears all registered dependency resolutions from this instance.
		/// </summary>
		/// <returns> A value indicating if at least one dependency resolution was removed. </returns>
		public bool ClearAllResolutions()
		{
			bool result;

			if (this.Disposed)
				throw new ObjectDisposedException(typeof(DependencyManager).FullName);

			// cop a reader lock
			this.ReaderWriterLock.EnterUpgradeableReadLock();

			try
			{
				// cop a writer lock
				this.ReaderWriterLock.EnterWriteLock();

				try
				{
					result = this.FreeDependencyResolutions();
					return result;
				}
				finally
				{
					this.ReaderWriterLock.ExitWriteLock();
				}
			}
			finally
			{
				this.ReaderWriterLock.ExitUpgradeableReadLock();
			}
		}

		/// <summary>
		/// Clears all registered dependency resolutions of the specified target type from this instance. This is the generic overload.
		/// </summary>
		/// <typeparam name="TObject"> The target type of removal. </typeparam>
		/// <param name="includeAssignableTypes"> A boolean value indicating whether to include assignable types in the candidate resolution removal list. </param>
		/// <returns> A value indicating if at least one dependency resolution was removed. </returns>
		public bool ClearTypeResolutions<TObject>(bool includeAssignableTypes)
		{
			Type targetType;

			if (this.Disposed)
				throw new ObjectDisposedException(typeof(DependencyManager).FullName);

			targetType = typeof(TObject);

			return this.ClearTypeResolutions(targetType, includeAssignableTypes);
		}

		/// <summary>
		/// Clears all registered dependency resolutions of the specified target type from this instance. This is the non-generic overload.
		/// </summary>
		/// <param name="targetType"> The target type of removal. </param>
		/// <param name="includeAssignableTypes"> A boolean value indicating whether to include assignable types in the candidate resolution removal list. </param>
		/// <returns> A value indicating if at least one dependency resolution was removed. </returns>
		public bool ClearTypeResolutions(Type targetType, bool includeAssignableTypes)
		{
			int count = 0;
			IEnumerable<KeyValuePair<Tuple<Type, string>, IDependencyResolution>> candidateResolutions;

			if (this.Disposed)
				throw new ObjectDisposedException(typeof(DependencyManager).FullName);

			if ((object)targetType == null)
				throw new ArgumentNullException(nameof(targetType));

			// cop a reader lock
			this.ReaderWriterLock.EnterUpgradeableReadLock();

			try
			{
				// cop a writer lock
				this.ReaderWriterLock.EnterWriteLock();

				try
				{
					// force execution to prevent 'System.InvalidOperationException : Collection was modified; enumeration operation may not execute.'
					candidateResolutions = this.GetCandidateResolutions(targetType, null, includeAssignableTypes).ToArray();

					foreach (KeyValuePair<Tuple<Type, string>, IDependencyResolution> dependencyResolutionRegistration in candidateResolutions)
					{
						if ((object)dependencyResolutionRegistration.Value != null)
							dependencyResolutionRegistration.Value.Dispose();

						this.DependencyResolutionRegistrations.Remove(dependencyResolutionRegistration.Key);

						count++;
					}

					return count > 0;
				}
				finally
				{
					this.ReaderWriterLock.ExitWriteLock();
				}
			}
			finally
			{
				this.ReaderWriterLock.ExitUpgradeableReadLock();
			}
		}

		/// <summary>
		/// Clears all dependency resolutions and cleans up any resources. Once disposed, the instance cannot be reused.
		/// </summary>
		public void Dispose()
		{
			if (this.Disposed)
				return;

			// cop a reader lock
			this.ReaderWriterLock.EnterUpgradeableReadLock();

			try
			{
				// cop a writer lock
				this.ReaderWriterLock.EnterWriteLock();

				try
				{
					this.FreeDependencyResolutions();
				}
				finally
				{
					this.ReaderWriterLock.ExitWriteLock();
				}
			}
			finally
			{
				this.ReaderWriterLock.ExitUpgradeableReadLock();
				this.Disposed = true;
				GC.SuppressFinalize(this);
			}
		}

		private bool FreeDependencyResolutions()
		{
			bool result;

			result = this.DependencyResolutionRegistrations.Count > 0;

			foreach (KeyValuePair<Tuple<Type, string>, IDependencyResolution> dependencyResolutionRegistration in this.DependencyResolutionRegistrations)
			{
				if ((object)dependencyResolutionRegistration.Value != null)
					dependencyResolutionRegistration.Value.Dispose();
			}

			this.DependencyResolutionRegistrations.Clear();

			return result;
		}

		private IEnumerable<KeyValuePair<Tuple<Type, string>, IDependencyResolution>> GetCandidateResolutions(Type targetType, string selectorKey, bool includeAssignableTypes)
		{
			IEnumerable<KeyValuePair<Tuple<Type, string>, IDependencyResolution>> candidateResolutions;

			if ((object)targetType == null)
				throw new ArgumentNullException(nameof(targetType));

			// selector key can be null in this context
			//if ((object)selectorKey == null)
			//throw new ArgumentNullException(nameof(selectorKey));

			candidateResolutions = this.DependencyResolutionRegistrations.Where(drr =>
				(drr.Key.Item1 == targetType || (includeAssignableTypes && targetType.IsAssignableFrom(drr.Key.Item1))) &&
				((object)selectorKey == null || drr.Key.Item2 == selectorKey)
				);

			return candidateResolutions;
		}

		private IDependencyResolution GetDependencyResolution(Type targetType, string selectorKey, bool includeAssignableTypes)
		{
			IEnumerable<KeyValuePair<Tuple<Type, string>, IDependencyResolution>> candidateResolutions;
			IDependencyResolution dependencyResolution;
			Tuple<Type, string> trait;

			// selector key can be null in this context
			//if ((object)selectorKey == null)
			//throw new ArgumentNullException(nameof(selectorKey));

			trait = new Tuple<Type, string>(targetType, selectorKey);
			candidateResolutions = this.GetCandidateResolutions(targetType, selectorKey, includeAssignableTypes);

			// first attempt direct resolution: exact type and selector key
			dependencyResolution = candidateResolutions.OrderBy(drr => drr.Key == trait).Select(drr => drr.Value).FirstOrDefault();

			return dependencyResolution;
		}

		/// <summary>
		/// Gets a value indicating whether there are any registered dependency resolutions of the specified target type in this instance. If selector key is a null value, then this method will return true if any resolution exists for the specified target type, regardless of selector key; otherwise, this method will return true only if a resolution exists for the specified target type and selector key. This is the generic overload.
		/// </summary>
		/// <param name="selectorKey"> An null or zero or greater length string selector key. </param>
		/// <param name="includeAssignableTypes"> A boolean value indicating whether to include assignable types in the candidate resolution existence check. </param>
		/// <typeparam name="TObject"> The target type of the check. </typeparam>
		/// <returns> A value indicating if at least one dependency resolution is present. </returns>
		public bool HasTypeResolution<TObject>(string selectorKey, bool includeAssignableTypes)
		{
			Type targetType;

			if (this.Disposed)
				throw new ObjectDisposedException(typeof(DependencyManager).FullName);

			// selector key can be null in this context
			//if ((object)selectorKey == null)
			//throw new ArgumentNullException(nameof(selectorKey));

			targetType = typeof(TObject);

			return this.HasTypeResolution(targetType, selectorKey, includeAssignableTypes);
		}

		/// <summary>
		/// Gets a value indicating whether there are any registered dependency resolutions of the specified target type in this instance. If selector key is a null value, then this method will return true if any resolution exists for the specified target type, regardless of selector key; otherwise, this method will return true only if a resolution exists for the specified target type and selector key. This is the non-generic overload.
		/// </summary>
		/// <param name="targetType"> The target type of the check. </param>
		/// <param name="selectorKey"> An null or zero or greater length string selector key. </param>
		/// <param name="includeAssignableTypes"> A boolean value indicating whether to include assignable types in the candidate resolution existence check. </param>
		/// <returns> A value indicating if at least one dependency resolution is present. </returns>
		public bool HasTypeResolution(Type targetType, string selectorKey, bool includeAssignableTypes)
		{
			IDependencyResolution dependencyResolution;

			if (this.Disposed)
				throw new ObjectDisposedException(typeof(DependencyManager).FullName);

			if ((object)targetType == null)
				throw new ArgumentNullException(nameof(targetType));

			// selector key can be null in this context
			//if ((object)selectorKey == null)
			//throw new ArgumentNullException(nameof(selectorKey));

			// cop a reader lock
			this.ReaderWriterLock.EnterUpgradeableReadLock();

			try
			{
				dependencyResolution = this.GetDependencyResolution(targetType, selectorKey, includeAssignableTypes);

				return (object)dependencyResolution != null;
			}
			finally
			{
				this.ReaderWriterLock.ExitUpgradeableReadLock();
			}
		}

		/// <summary>
		/// Removes the registered dependency resolution of the specified target type and selector key from this instance. Throws a DependencyException if the target type and selector key combination has not been previously registered in this instance. This is the generic overload.
		/// </summary>
		/// <typeparam name="TObject"> The target type of removal. </typeparam>
		/// <param name="includeAssignableTypes"> A boolean value indicating whether to include assignable types in the candidate resolution removal list. </param>
		/// <param name="selectorKey"> An non-null, zero or greater length string selector key. </param>
		public void RemoveResolution<TObject>(string selectorKey, bool includeAssignableTypes)
		{
			Type targetType;

			if (this.Disposed)
				throw new ObjectDisposedException(typeof(DependencyManager).FullName);

			if ((object)selectorKey == null)
				throw new ArgumentNullException(nameof(selectorKey));

			targetType = typeof(TObject);

			this.RemoveResolution(targetType, selectorKey, includeAssignableTypes);
		}

		/// <summary>
		/// Removes the registered dependency resolution of the specified target type and selector key from this instance. Throws a DependencyException if the target type and selector key combination has not been previously registered in this instance. This is the non-generic overload.
		/// </summary>
		/// <param name="targetType"> The target type of removal. </param>
		/// <param name="selectorKey"> An non-null, zero or greater length string selector key. </param>
		/// <param name="includeAssignableTypes"> A boolean value indicating whether to include assignable types in the candidate resolution removal list. </param>
		public void RemoveResolution(Type targetType, string selectorKey, bool includeAssignableTypes)
		{
			Tuple<Type, string> trait;
			IDependencyResolution dependencyResolution;

			if (this.Disposed)
				throw new ObjectDisposedException(typeof(DependencyManager).FullName);

			if ((object)targetType == null)
				throw new ArgumentNullException(nameof(targetType));

			if ((object)selectorKey == null)
				throw new ArgumentNullException(nameof(selectorKey));

			// cop a reader lock
			this.ReaderWriterLock.EnterUpgradeableReadLock();

			try
			{
				// cop a writer lock
				this.ReaderWriterLock.EnterWriteLock();

				try
				{
					trait = new Tuple<Type, string>(targetType, selectorKey);
					dependencyResolution = this.GetDependencyResolution(targetType, selectorKey, includeAssignableTypes);

					if ((object)dependencyResolution == null) // nothing to offer up
						throw new DependencyException(string.Format("Dependency resolution in the in-effect dependency manager failed to match for target type '{0}' and selector key '{1}' (include assignable types: '{2}').", targetType.FullName, selectorKey, includeAssignableTypes));

					dependencyResolution.Dispose();
					this.DependencyResolutionRegistrations.Remove(trait);
				}
				finally
				{
					this.ReaderWriterLock.ExitWriteLock();
				}
			}
			finally
			{
				this.ReaderWriterLock.ExitUpgradeableReadLock();
			}
		}

		/// <summary>
		/// Resolves a dependency for the given target type and selector key combination. Throws a DependencyException if the target type and selector key combination has not been previously registered in this instance. Throws a DependencyException if the resolved value cannot be assigned to the target type. This is the non-generic overload.
		/// </summary>
		/// <typeparam name="TObject"> The target type of resolution. </typeparam>
		/// <param name="selectorKey"> An non-null, zero or greater length string selector key. </param>
		/// <param name="includeAssignableTypes"> A boolean value indicating whether to include assignable types in the candidate resolution lookup list. </param>
		/// <returns> An object instance of assisgnable to the target type. </returns>
		public TObject ResolveDependency<TObject>(string selectorKey, bool includeAssignableTypes)
		{
			Type targetType;
			TObject value;

			if (this.Disposed)
				throw new ObjectDisposedException(typeof(DependencyManager).FullName);

			if ((object)selectorKey == null)
				throw new ArgumentNullException(nameof(selectorKey));

			targetType = typeof(TObject);

			value = (TObject)this.ResolveDependency(targetType, selectorKey, includeAssignableTypes);

			return value;
		}

		/// <summary>
		/// Resolves a dependency for the given target type and selector key combination. Throws a DependencyException if the target type and selector key combination has not been previously registered in this instance. Throws a DependencyException if the resolved value cannot be assigned to the target type. This is the non-generic overload.
		/// </summary>
		/// <param name="targetType"> The target type of resolution. </param>
		/// <param name="selectorKey"> An non-null, zero or greater length string selector key. </param>
		/// <param name="includeAssignableTypes"> A boolean value indicating whether to include assignable types in the candidate resolution lookup list. </param>
		/// <returns> An object instance of assisgnable to the target type. </returns>
		public object ResolveDependency(Type targetType, string selectorKey, bool includeAssignableTypes)
		{
			object value;
			IDependencyResolution dependencyResolution;
			Type typeOfValue;

			if (this.Disposed)
				throw new ObjectDisposedException(typeof(DependencyManager).FullName);

			if ((object)targetType == null)
				throw new ArgumentNullException(nameof(targetType));

			if ((object)selectorKey == null)
				throw new ArgumentNullException(nameof(selectorKey));

			// cop a reader lock
			this.ReaderWriterLock.EnterUpgradeableReadLock();

			try
			{
				dependencyResolution = this.GetDependencyResolution(targetType, selectorKey, includeAssignableTypes);

				if ((object)dependencyResolution == null) // nothing to offer up
					throw new DependencyException(string.Format("Dependency resolution in the in-effect dependency manager failed to match for target type '{0}' and selector key '{1}' (include assignable types: '{2}').", targetType.FullName, selectorKey, includeAssignableTypes));

				value = dependencyResolution.Resolve(this);

				if ((object)value != null)
				{
					typeOfValue = value.GetType();

					if (!targetType.IsAssignableFrom(typeOfValue))
						throw new DependencyException(string.Format("Dependency resolution in the dependency manager matched for target type '{0}' and selector key '{1}' but the resolved value type '{2}' is not assignable the target type '{3}'.", targetType.FullName, selectorKey, typeOfValue.FullName, targetType.FullName));
				}

				return value;
			}
			finally
			{
				this.ReaderWriterLock.ExitUpgradeableReadLock();
			}
		}

		#endregion
	}
}