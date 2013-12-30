/*
	Copyright ©2002-2013 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Common.Data
{
	/// <summary>
	/// Used to 'wrap' a disposable (e.g. DataContext, ObjectContext, SessionImpl, etc.) in a manner such that consuming code can leverage a 'using' block which respects an ambient unit of work, if one is present.
	/// Essentially, the disposal of this object forwards disposal to the wrapped disposable if an ambient unit of work is NOT present; otherwise, no action is performed leaving disposal of the disposable up to the adjudication of the ambient unit of work.
	/// </summary>
	/// <typeparam name="TDisposable"> The type of the underlying or 'wrapped' disposable. </typeparam>
	public sealed class AmbientUnitOfWorkAwareDisposableWrapper<TDisposable> : IDisposable
		where TDisposable : class, IDisposable
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the AmbientUnitOfWorkAwareDisposableWrapper`1 class.
		/// </summary>
		/// <param name="sourceUnitOfWork">
		/// The unit of work triggering the creation of this 'wrapped' disposable.
		/// NOTE: Any ambient unit of work may not reference the same object instance as the incoming unit of work.
		/// In this case, the ambient unit of work is ignored as the unit of work instances differ.
		/// </param>
		/// <param name="disposable"> The underlying or 'wrapped' disposable. </param>
		public AmbientUnitOfWorkAwareDisposableWrapper(IUnitOfWork sourceUnitOfWork, TDisposable disposable)
		{
			if ((object)sourceUnitOfWork == null)
				throw new ArgumentNullException("sourceUnitOfWork");

			if ((object)disposable == null)
				throw new ArgumentNullException("disposable");

			this.sourceUnitOfWork = sourceUnitOfWork;
			this.disposable = disposable;
		}

		#endregion

		#region Fields/Constants

		private readonly TDisposable disposable;
		private readonly IUnitOfWork sourceUnitOfWork;
		private bool disposed;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// Gets the underlying or 'wrapped' disposable.
		/// </summary>
		public TDisposable Disposable
		{
			get
			{
				if (this.Disposed)
					throw new ObjectDisposedException(typeof(AmbientUnitOfWorkAwareDisposableWrapper<TDisposable>).FullName);

				return this.disposable;
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

		/// <summary>
		/// Gets a value indicating whether resources need to be disposed of.
		/// </summary>
		private bool ShouldDisposeResources
		{
			get
			{
				return
					(object)UnitOfWork.Current == null ||
					((object)UnitOfWork.Current != this.SourceUnitOfWork);
			}
		}

		private IUnitOfWork SourceUnitOfWork
		{
			get
			{
				return this.sourceUnitOfWork;
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Disposes of the inner context. Once disposed, the instance cannot be reused.
		/// </summary>
		public void Dispose()
		{
			if (this.Disposed)
				return;

			try
			{
				if (this.ShouldDisposeResources)
					this.Disposable.Dispose();
			}
			finally
			{
				this.Disposed = true;
				GC.SuppressFinalize(this);
			}
		}

		#endregion
	}
}