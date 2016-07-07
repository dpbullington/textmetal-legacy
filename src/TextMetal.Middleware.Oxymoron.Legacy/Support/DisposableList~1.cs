/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

namespace TextMetal.Middleware.Oxymoron.Legacy.Support
{
	public class DisposableList<T> : List<T>, IDisposable
		where T : IDisposable
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
				foreach (IDisposable disposable in this)
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