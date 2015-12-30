using System;

namespace NMock.Monitoring
{
	internal class InvocationRecorder : IDisposable
	{
		#region Constructors/Destructors

		internal InvocationRecorder()
		{
			_instance = this;
		}

		#endregion

		#region Fields/Constants

		[ThreadStatic]
		private static InvocationRecorder _instance;

		#endregion

		#region Properties/Indexers/Events

		internal static InvocationRecorder Current
		{
			get
			{
				return _instance;
			}
		}

		internal static bool Recording
		{
			get
			{
				return _instance != null;
			}
		}

		public Invocation Invocation
		{
			get;
			set;
		}

		#endregion

		#region Methods/Operators

		public void Dispose()
		{
			_instance = null;
		}

		#endregion
	}
}