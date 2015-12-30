#region Using

using System;
using System.IO;

using NMock.Monitoring;

#endregion

namespace NMock.Actions
{
	/// <summary>
	/// Action that set the result value of an invocation to a clone of the specified prototype.
	/// </summary>
	public class ReturnCloneAction : IReturnAction
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ReturnCloneAction" /> class.
		/// </summary>
		/// <param name="prototype"> The prototype. </param>
		public ReturnCloneAction(object prototype)
		{
			this.prototype = prototype;
		}

		#endregion

		#region Fields/Constants

		/// <summary>
		/// Stores the prototype that will be cloned.
		/// </summary>
		private readonly object prototype;

		#endregion

		#region Properties/Indexers/Events

		public Type ReturnType
		{
			get
			{
				return this.prototype.GetType();
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
			writer.Write("a clone of ");
			writer.Write(this.prototype);
		}

		void IAction.Invoke(Invocation invocation)
		{
			invocation.Result = this.prototype; // ???
		}

		#endregion
	}
}