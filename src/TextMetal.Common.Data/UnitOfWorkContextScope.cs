/*
	Copyright ©2002-2013 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Common.Data
{
	/// <summary>
	/// Represents an atomic scoped set of data operations on a single connection/transaction.
	/// </summary>
	public sealed class UnitOfWorkContextScope : IDisposable
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the UnitOfWorkContextScope class.
		/// </summary>
		public UnitOfWorkContextScope(UnitOfWorkContextScopeOption scopeOption, IUnitOfWorkContextFactory unitOfWorkContextFactory)
		{
			if ((object)unitOfWorkContextFactory == null)
				throw new ArgumentNullException("unitOfWorkContextFactory");

			this.previousUnitOfWorkContext = UnitOfWorkContext.Current;

			switch (scopeOption)
			{
				case UnitOfWorkContextScopeOption.Required:
					if ((object)UnitOfWorkContext.Current == null)
						throw new InvalidOperationException("Cannot create unit of work context scope; one DOES NOT already exists on the current thread and application domain.");
					break;
				case UnitOfWorkContextScopeOption.RequiresNone:
					if ((object)UnitOfWorkContext.Current != null)
						throw new InvalidOperationException("Cannot create unit of work context scope; one DOES already exists on the current thread and application domain.");
					UnitOfWorkContext.Current = unitOfWorkContextFactory.GetUnitOfWorkContext();
					break;
				case UnitOfWorkContextScopeOption.RequiresNew:
					UnitOfWorkContext.Current = unitOfWorkContextFactory.GetUnitOfWorkContext();
					break;
				case UnitOfWorkContextScopeOption.Suppress:
					UnitOfWorkContext.Current = null;
					break;
				default:
					throw new ArgumentOutOfRangeException("scopeOption");
			}
		}

		#endregion

		#region Fields/Constants

		private readonly IUnitOfWorkContext previousUnitOfWorkContext;
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

		private IUnitOfWorkContext PreviousUnitOfWorkContext
		{
			get
			{
				return this.previousUnitOfWorkContext;
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Indicates that all operations within the unit of work context scope have completed successfully. This method should only be called once.
		/// </summary>
		public void AmbientComplete()
		{
			if ((object)UnitOfWorkContext.Current != null)
				UnitOfWorkContext.Current.Complete();
		}

		/// <summary>
		/// Indicates that at least one operation within the unit of work context scope cause a failure in data concurrency or idempotency. This forces the entire unit of work to yield an incomplete status. This method can be called any number of times.
		/// </summary>
		public void AmbientDivergent()
		{
			if ((object)UnitOfWorkContext.Current != null)
				UnitOfWorkContext.Current.Divergent();
		}

		/// <summary>
		/// Ends the data source transaction scope and performs a commit or rollback.
		/// </summary>
		public void Dispose()
		{
			if (this.Disposed)
				return;

			try
			{
				if ((object)UnitOfWorkContext.Current != null)
				{
					try
					{
						UnitOfWorkContext.Current.Dispose();
					}
					finally
					{
						UnitOfWorkContext.Current = this.PreviousUnitOfWorkContext;
						//this.PreviousUnitOfWorkContext = null;
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