using System;
using System.IO;

using NMock.Monitoring;

namespace NMock.Actions
{
	/// <summary>
	/// An <see cref="IAction" /> that can invoke an <see cref="Action" /> when the expectation is met.
	/// </summary>
	public class InvokeAction : IAction
	{
		#region Constructors/Destructors

		/// <summary>
		/// Creates an <see cref="IAction" /> that will invoke the <paramref name="action" />.
		/// </summary>
		/// <param name="action"> The action to invoke. </param>
		public InvokeAction(Action action)
		{
			this._action = action;
		}

		#endregion

		#region Fields/Constants

		private readonly Action _action;

		#endregion

		#region Methods/Operators

		void ISelfDescribing.DescribeTo(TextWriter writer)
		{
			writer.Write("Invokes the action supplied to the constructor.");
		}

		void IAction.Invoke(Invocation invocation)
		{
			this._action();
		}

		#endregion
	}
}