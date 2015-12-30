using System;
using System.Collections.Generic;
using System.IO;

using NMock.Monitoring;

namespace NMock.Actions
{
	/// <summary>
	/// Action that returns an item from the queue
	/// </summary>
	public class QueueAction<T> : IReturnAction
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="QueueAction{T}" /> class with the queue of values.
		/// </summary>
		/// <param name="queue"> </param>
		public QueueAction(Queue<T> queue)
		{
			this._queue = queue;
		}

		#endregion

		#region Fields/Constants

		private readonly Queue<T> _queue;

		#endregion

		#region Properties/Indexers/Events

		Type IReturnAction.ReturnType
		{
			get
			{
				return typeof(T);
			}
		}

		#endregion

		#region Methods/Operators

		void ISelfDescribing.DescribeTo(TextWriter writer)
		{
			writer.Write("return a '" + typeof(T).Name + "' from a queue");
		}

		void IAction.Invoke(Invocation invocation)
		{
			invocation.Result = this._queue.Dequeue();
		}

		#endregion
	}
}