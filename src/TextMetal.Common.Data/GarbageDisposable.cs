/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Common.Data
{
	public sealed class GarbageDisposable : IDisposable
	{
		#region Constructors/Destructors

		private GarbageDisposable()
		{
		}

		#endregion

		#region Fields/Constants

		private static readonly IDisposable instance = new GarbageDisposable();

		#endregion

		#region Properties/Indexers/Events

		public static IDisposable Instance
		{
			get
			{
				return instance;
			}
		}

		#endregion

		#region Methods/Operators

		public void Dispose()
		{
			// do nothing
		}

		#endregion
	}
}