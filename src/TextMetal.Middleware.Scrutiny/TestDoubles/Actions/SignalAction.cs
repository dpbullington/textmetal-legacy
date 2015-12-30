#region Using

using System.IO;
using System.Threading;

using NMock.Monitoring;

#endregion

namespace NMock.Actions
{
	/// <summary>
	/// Action that signals an event.
	/// You can use this action to synchronize threads when an expectation is invoked.
	/// </summary>
	public class SignalAction : IAction
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="SignalAction" /> class.
		/// </summary>
		/// <param name="signal"> The signal. </param>
		public SignalAction(EventWaitHandle signal)
		{
			this.signal = signal;
		}

		#endregion

		#region Fields/Constants

		/// <summary>
		/// Stores the wait handle to be signalled.
		/// </summary>
		private readonly EventWaitHandle signal;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// Gets the signal.
		/// You can use this signal to wait for this action beeing invoked.
		/// </summary>
		/// <value> The signal. </value>
		public EventWaitHandle Signal
		{
			get
			{
				return this.signal;
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Describes this object.
		/// </summary>
		/// <param name="writer"> The text writer the description is added to. </param>
		void ISelfDescribing.DescribeTo(TextWriter writer)
		{
			writer.Write("signals");
		}

		/// <summary>
		/// Invokes this object by signaling the event.
		/// </summary>
		/// <param name="invocation"> The invocation. </param>
		void IAction.Invoke(Invocation invocation)
		{
			this.signal.Set();
		}

		#endregion
	}
}