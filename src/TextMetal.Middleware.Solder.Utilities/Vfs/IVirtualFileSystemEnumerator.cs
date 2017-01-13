/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System.Collections.Generic;

namespace TextMetal.Middleware.Solder.Utilities.Vfs
{
	public interface IVirtualFileSystemEnumerator
	{
		#region Methods/Operators

		IEnumerable<IVirtualFileSystemItem> EnumerateVirtualItems(string directoryPath, bool enableRecursion);

		#endregion
	}
}