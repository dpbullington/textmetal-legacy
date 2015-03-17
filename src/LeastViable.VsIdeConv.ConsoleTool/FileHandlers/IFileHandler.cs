/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.IO;

namespace LeastViable.VsIdeConv.ConsoleTool.FileHandlers
{
	public interface IFileHandler
	{
		#region Methods/Operators

		void Execute(FileInfo fileInfo);

		#endregion
	}
}