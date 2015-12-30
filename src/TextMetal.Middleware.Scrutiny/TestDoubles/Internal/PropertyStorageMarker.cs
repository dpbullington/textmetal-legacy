using System;

namespace NMock.Internal
{
	internal class PropertyStorageMarker : IDisposable
	{
		#region Constructors/Destructors

		internal PropertyStorageMarker()
		{
			_instance = this;
		}

		#endregion

		#region Fields/Constants

		[ThreadStatic]
		private static PropertyStorageMarker _instance;

		#endregion

		#region Properties/Indexers/Events

		internal static bool UsePropertyStorage
		{
			get
			{
				return _instance != null;
			}
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