/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Data;

namespace TextMetal.Common.Data
{
	/// <summary>
	/// Represents an atomic scoped set of data operations on a single connection/transaction.
	/// </summary>
	public sealed class AmbientUnitOfWorkScope : IDisposable
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the AmbientUnitOfWorkScope class.
		/// </summary>
		/// <param name="unitOfWorkFactory"> The unit of work factory instance. </param>
		/// <param name="isolationLevel"> An option isolation level for the unit of work transaction. </param>
		public AmbientUnitOfWorkScope(IUnitOfWorkFactory unitOfWorkFactory, IsolationLevel isolationLevel = IsolationLevel.Unspecified)
		{
			if ((object)unitOfWorkFactory == null)
				throw new ArgumentNullException("unitOfWorkFactory");

			if ((object)UnitOfWork.Current != null)
				throw new InvalidOperationException("An ambient unit of work already exists on the current thread and application domain.");

			UnitOfWork.Current = unitOfWorkFactory.GetUnitOfWork(isolationLevel);
		}

		#endregion

		#region Fields/Constants

		private bool disposed;

		#endregion

		#region Properties/Indexers/Events

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
		/// Ends the data source transaction scope and performs a commit or rollback.
		/// </summary>
		public void Dispose()
		{
			if (this.Disposed)
				return;

			try
			{
				if ((object)UnitOfWork.Current != null)
				{
					try
					{
						UnitOfWork.Current.Dispose();
					}
					finally
					{
						UnitOfWork.Current = null;
					}
				}
			}
			finally
			{
				this.Disposed = true;
				GC.SuppressFinalize(this);
			}
		}

		/// <summary>
		/// Indicates that all operations within the unit of work scope have completed successfully. This method should only be called once.
		/// </summary>
		public void ScopeComplete()
		{
			if ((object)UnitOfWork.Current != null)
				UnitOfWork.Current.Complete();
		}

		/// <summary>
		/// Indicates that at least one operation within the unit of work scope cause a failure in data concurrency or idempotency. This forces the entire unit of work to yield an incomplete status. This method can be called any number of times.
		/// </summary>
		public void ScopeDivergent()
		{
			if ((object)UnitOfWork.Current != null)
				UnitOfWork.Current.Divergent();
		}

		#endregion
	}
}