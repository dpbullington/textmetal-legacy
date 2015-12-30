#region Using

using System;
using System.IO;

using NMock.Monitoring;

#endregion

namespace NMock.Actions
{
	/// <summary>
	/// Action that sets the exception of an invocation.
	/// </summary>
	public class ThrowAction : IAction
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ThrowAction" /> class.
		/// </summary>
		/// <param name="exception"> The exception. </param>
		public ThrowAction(Exception exception)
		{
			this.exception = exception;
		}

		#endregion

		#region Fields/Constants

		/// <summary>
		/// Stores the exception to be thrown.
		/// </summary>
		private readonly Exception exception;

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Describes this object.
		/// </summary>
		/// <param name="writer"> The text writer the description is added to. </param>
		void ISelfDescribing.DescribeTo(TextWriter writer)
		{
			writer.Write("throw ");
			writer.Write(this.exception);
		}

		/// <summary>
		/// Invokes this object. Sets the exception the invocation will throw.
		/// </summary>
		/// <param name="invocation"> The invocation. </param>
		void IAction.Invoke(Invocation invocation)
		{
			invocation.Exception = this.exception;
		}

		#endregion
	}
}