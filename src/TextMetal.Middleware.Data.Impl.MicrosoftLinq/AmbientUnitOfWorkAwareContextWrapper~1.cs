/*
	Copyright �2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Middleware.Common.Fascades.AdoNet.UoW;

namespace TextMetal.Middleware.Data.Impl.MicrosoftLinq
{
	/// <summary>
	/// Used to 'wrap' a disposable (e.g. DataContext, DbContext, SessionImpl, etc.) in a manner such that consuming code can leverage a 'using' block which respects an ambient unit of work, if one is present.
	/// Essentially, the disposal of this object forwards disposal to the wrapped disposable if an ambient unit of work is NOT present; otherwise, no action is performed leaving disposal of the disposable up to the adjudication of the ambient unit of work.
	/// </summary>
	/// <typeparam name="TDisposableContext"> The type of the underlying or 'wrapped' disposable. </typeparam>
	public sealed class AmbientUnitOfWorkAwareContextWrapper<TDisposableContext> : IDisposable
		where TDisposableContext : class, IDisposable
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the AmbientUnitOfWorkAwareContextWrapper`1 class.
		/// </summary>
		/// <param name="sourceUnitOfWork">
		/// The unit of work triggering the creation of this 'wrapped' disposable.
		/// NOTE: Any ambient unit of work may not reference the same object instance as the incoming unit of work.
		/// In this case, the ambient unit of work is ignored as the unit of work instances differ.
		/// </param>
		/// <param name="disposableContext"> The underlying or 'wrapped' disposable. </param>
		public AmbientUnitOfWorkAwareContextWrapper(IUnitOfWork sourceUnitOfWork, TDisposableContext disposableContext)
		{
			if ((object)sourceUnitOfWork == null)
				throw new ArgumentNullException("sourceUnitOfWork");

			if ((object)disposableContext == null)
				throw new ArgumentNullException("disposableContext");

			this.sourceUnitOfWork = sourceUnitOfWork;
			this.disposableContext = disposableContext;
			this.IsUsableWrappedContext = true;
		}

		#endregion

		#region Fields/Constants

		private readonly TDisposableContext disposableContext;
		private readonly IUnitOfWork sourceUnitOfWork;
		private bool disposed;
		private bool isUsableWrappedContext;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// Gets the underlying or 'wrapped' disposable.
		/// </summary>
		public TDisposableContext DisposableContext
		{
			get
			{
				if (this.Disposed)
					throw new ObjectDisposedException(typeof(AmbientUnitOfWorkAwareContextWrapper<TDisposableContext>).FullName);

				return this.disposableContext;
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

		public bool IsUsableWrappedContext
		{
			get
			{
				return this.isUsableWrappedContext;
			}
			private set
			{
				this.isUsableWrappedContext = value;
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
				{
					try
					{
						MulticastDisposableContext<TDisposableContext> multicastDisposableContext;

						multicastDisposableContext = this.SourceUnitOfWork.Context as MulticastDisposableContext<TDisposableContext>;

						// will fail if not correct type (e.g. DataContext, DbContext, etc.)
						if ((object)multicastDisposableContext == null)
							throw new InvalidOperationException("Multicast context type obtained from the current data source transaction context does not match the current multicast context type.");

						multicastDisposableContext.ClearContext(this.DisposableContext.GetType());

						this.DisposableContext.Dispose();
					}
					finally
					{
						this.IsUsableWrappedContext = false;
					}
				}
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