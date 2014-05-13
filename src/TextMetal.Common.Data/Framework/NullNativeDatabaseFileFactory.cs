/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Common.Data.Framework
{
	public sealed class NullNativeDatabaseFileFactory : INativeDatabaseFileFactory
	{
		#region Constructors/Destructors

		public NullNativeDatabaseFileFactory()
		{
		}

		#endregion

		#region Methods/Operators

		public bool CreateNativeDatabaseFile(string databaseFilePath)
		{
			return false; // do nothing
		}

		#endregion
	}
}