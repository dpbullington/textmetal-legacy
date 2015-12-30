using System.IO;

using NMock.Monitoring;

namespace NMock.Actions
{
	internal class CaptureValueAction : IAction
	{
		#region Methods/Operators

		void ISelfDescribing.DescribeTo(TextWriter writer)
		{
			writer.Write("capture the setter value");
		}

		void IAction.Invoke(Invocation invocation)
		{
			invocation.SetterResult = invocation.Arguments[invocation.Arguments.Length - 1];
		}

		#endregion
	}
}