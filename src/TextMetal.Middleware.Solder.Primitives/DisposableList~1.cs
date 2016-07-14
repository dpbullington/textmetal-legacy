/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

namespace TextMetal.Middleware.Solder.Primitives
{
	public class DisposableList<TDisposable> : List<TDisposable>, IDisposable
		where TDisposable : IDisposable
	{
		#region Constructors/Destructors

		public DisposableList()
		{
		}

		#endregion

		#region Methods/Operators

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				foreach (TDisposable disposable in this)
					disposable.Dispose();
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}