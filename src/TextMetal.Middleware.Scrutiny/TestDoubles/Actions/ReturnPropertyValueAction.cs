using System.IO;

using NMock.Internal;
using NMock.Monitoring;

namespace NMock.Actions
{
	internal class ReturnPropertyValueAction : IAction
	{
		#region Constructors/Destructors

		public ReturnPropertyValueAction(IMockObject mockObject)
		{
			this._mockObject = mockObject;
		}

		#endregion

		#region Fields/Constants

		private readonly IMockObject _mockObject;

		#endregion

		#region Methods/Operators

		void ISelfDescribing.DescribeTo(TextWriter writer)
		{
			writer.Write("return the set value");
		}

		void IAction.Invoke(Invocation invocation)
		{
			if (invocation.IsPropertyGetAccessor)
			{
				using (new PropertyStorageMarker())
					invocation.InvokeOn(this._mockObject);
			}
		}

		#endregion
	}
}