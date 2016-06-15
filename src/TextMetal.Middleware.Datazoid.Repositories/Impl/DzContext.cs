/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Middleware.Datazoid.Repositories.Impl
{
	public sealed class DzContext : IDisposable
	{
		#region Constructors/Destructors

		public DzContext()
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