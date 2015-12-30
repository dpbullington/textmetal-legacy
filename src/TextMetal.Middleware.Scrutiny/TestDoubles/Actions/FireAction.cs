#region Using

using System;
using System.IO;

using NMock.Monitoring;

#endregion

namespace NMock.Actions
{
	/// <summary>
	/// Action that fires an event.
	/// </summary>
	[Obsolete]
	public class FireAction : IAction
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="FireAction" /> class.
		/// </summary>
		/// <param name="eventName"> Name of the event. </param>
		/// <param name="eventArgs"> The event args. </param>
		public FireAction(string eventName, params object[] eventArgs)
		{
			this.eventName = eventName;
			this.eventArgs = eventArgs;
		}

		#endregion

		#region Fields/Constants

		/// <summary>
		/// Stores the event arguments.
		/// </summary>
		private readonly object[] eventArgs;

		/// <summary>
		/// Stores the name of the event to fire.
		/// </summary>
		private readonly string eventName;

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Describes this object.
		/// </summary>
		/// <param name="writer"> The text writer the description is added to. </param>
		void ISelfDescribing.DescribeTo(TextWriter writer)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");

			writer.Write("fire ");
			writer.Write(this.eventName);
		}

		/// <summary>
		/// Invokes this object. The event is fired on the receiver of the invocation.
		/// </summary>
		/// <param name="invocation"> The invocation. </param>
		void IAction.Invoke(Invocation invocation)
		{
			if (invocation == null)
				throw new ArgumentNullException("invocation");

			((IMockObject)invocation.Receiver).RaiseEvent(this.eventName, this.eventArgs);
		}

		#endregion
	}
}