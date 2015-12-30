#region Using

using System.IO;

using NMock.Monitoring;

#endregion

namespace NMock.Actions
{
	/// <summary>
	/// Action that returns the n-th element of the arguments to an invocation.
	/// </summary>
	public class CollectAction : IAction
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="CollectAction" /> class.
		/// </summary>
		/// <param name="argumentIndex"> Index of the argument to collect. </param>
		public CollectAction(int argumentIndex)
		{
			this.argumentIndex = argumentIndex;
		}

		#endregion

		#region Fields/Constants

		/// <summary>
		/// Stores the index of the argument.
		/// </summary>
		private readonly int argumentIndex;

		/// <summary>
		/// Stores the parameter when this action gets invoked.
		/// </summary>
		private object collectedArgumentValue;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// Gets the collected parameter.
		/// </summary>
		/// <value> The collected parameter (n-th parameter of parameter list of the method's call. </value>
		public object Parameter
		{
			get
			{
				return this.collectedArgumentValue;
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
			writer.Write("collect argument at index ");
			writer.Write(this.argumentIndex);
		}

		/// <summary>
		/// Invokes this object.
		/// </summary>
		/// <param name="invocation"> The invocation. </param>
		void IAction.Invoke(Invocation invocation)
		{
			this.collectedArgumentValue = invocation.Parameters[this.argumentIndex];
		}

		#endregion
	}
}