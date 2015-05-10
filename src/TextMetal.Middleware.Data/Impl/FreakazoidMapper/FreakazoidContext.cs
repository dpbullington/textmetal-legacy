/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Middleware.Data.Impl.FreakazoidMapper
{
	public sealed class FreakazoidContext : IDisposable
	{
		#region Constructors/Destructors

		public FreakazoidContext()
		{
		}

		#endregion

		#region Methods/Operators

		void IDisposable.Dispose()
		{
			// do nothing
		}

		#endregion
	}
}