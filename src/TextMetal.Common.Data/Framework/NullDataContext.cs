/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Common.Data.Framework
{
	public sealed class NullDataContext : IDisposable
	{
		#region Constructors/Destructors

		private NullDataContext()
		{
		}

		#endregion

		#region Methods/Operators

		void IDisposable.Dispose()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}