#region Using

using System;
using System.IO;

using NMock.Monitoring;

#endregion

namespace NMock.Actions
{
	/// <summary>
	/// Action that executes the delegate passed to the constructor to get argments of executed method.
	/// </summary>
	public class GetArgumentsAction : IAction
	{
		#region Constructors/Destructors

		/// <summary>
		/// constustor of GetArgumentsAction
		/// </summary>
		/// <param name="handler"> delegate used to get argments of executed method </param>
		public GetArgumentsAction(Action<ParameterList> handler)
		{
			this.handler = handler;
		}

		#endregion

		#region Fields/Constants

		private readonly Action<ParameterList> handler;

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

			writer.Write("Get arguments");
		}

		/// <summary>
		/// Invokes this object.
		/// </summary>
		/// <param name="invocation"> The invocation. </param>
		void IAction.Invoke(Invocation invocation)
		{
			if (this.handler != null)
				this.handler.Invoke(invocation.Parameters);
		}

		#endregion
	}
}