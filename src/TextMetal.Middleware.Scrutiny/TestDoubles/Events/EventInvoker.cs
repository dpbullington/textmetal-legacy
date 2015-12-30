using System;
using System.Diagnostics;

using NMock.Syntax;

namespace NMock
{
	/// <summary>
	/// EventInvoker is used to invoke events that definded by the <see cref="System.EventHandler" />.
	/// </summary>
	/// <remarks>
	/// An EventInvoker is created as a result of a call to the <see cref="IStubSyntax{TInterface}.EventBinding" /> method.
	/// </remarks>
	public class EventInvoker : EventInvoker<EventArgs>
	{
		#region Constructors/Destructors

		internal EventInvoker(string eventName, IActionSyntax expectation)
			: base(eventName, expectation)
		{
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Invokes the referenced event
		/// </summary>
		[DebuggerStepThrough]
		public void Invoke()
		{
			this.Invoke(null, EventArgs.Empty);
		}

		#endregion
	}
}