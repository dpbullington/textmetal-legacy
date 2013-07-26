using System;
using System.Collections;
using System.Threading;

namespace NUnit.Core
{
	public class AsyncSynchronizationContext : SynchronizationContext
	{
		#region Fields/Constants

		private readonly AsyncOperationQueue _operations = new AsyncOperationQueue();
		private int _operationCount;

		#endregion

		#region Methods/Operators

		public override void OperationCompleted()
		{
			if (Interlocked.Decrement(ref this._operationCount) == 0)
				this._operations.MarkAsComplete();

			base.OperationCompleted();
		}

		public override void OperationStarted()
		{
			Interlocked.Increment(ref this._operationCount);
			base.OperationStarted();
		}

		public override void Post(SendOrPostCallback d, object state)
		{
			this._operations.Enqueue(new AsyncOperation(d, state));
		}

		public override void Send(SendOrPostCallback d, object state)
		{
			throw new InvalidOperationException("Sending to this synchronization context is not supported");
		}

		public void WaitForPendingOperationsToComplete()
		{
			this._operations.InvokeAll();
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		private class AsyncOperation
		{
			#region Constructors/Destructors

			public AsyncOperation(SendOrPostCallback action, object state)
			{
				this._action = action;
				this._state = state;
			}

			#endregion

			#region Fields/Constants

			private readonly SendOrPostCallback _action;
			private readonly object _state;

			#endregion

			#region Methods/Operators

			public void Invoke()
			{
				this._action(this._state);
			}

			#endregion
		}

		private class AsyncOperationQueue
		{
			#region Fields/Constants

			private readonly Queue _operations = Queue.Synchronized(new Queue());
			private readonly AutoResetEvent _operationsAvailable = new AutoResetEvent(false);
			private bool _run = true;

			#endregion

			#region Methods/Operators

			public void Enqueue(AsyncOperation asyncOperation)
			{
				this._operations.Enqueue(asyncOperation);
				this._operationsAvailable.Set();
			}

			public void InvokeAll()
			{
				while (this._run)
				{
					this.InvokePendingOperations();
					this._operationsAvailable.WaitOne();
				}

				this.InvokePendingOperations();
			}

			private void InvokePendingOperations()
			{
				while (this._operations.Count > 0)
				{
					AsyncOperation operation = (AsyncOperation)this._operations.Dequeue();
					operation.Invoke();
				}
			}

			public void MarkAsComplete()
			{
				this._run = false;
				this._operationsAvailable.Set();
			}

			#endregion
		}

		#endregion
	}
}