#region Using

using System;

using NMock.Syntax;

#endregion

namespace NMock
{
	/// <summary>
	/// </summary>
	public class DelegateInvoker : EventInvokerBase
	{
		#region Constructors/Destructors

		internal DelegateInvoker(string eventName, IActionSyntax expectation)
			: base(eventName, expectation, true)
		{
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Invokes the delegate with the specified parameters.
		/// </summary>
		/// <exception cref="InvalidOperationException" />
		//[DebuggerStepThrough]
		public void Invoke(params object[] args)
		{
			this.RaiseEvent(args);
		}

		#endregion
	}
}