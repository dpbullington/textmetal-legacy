/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.IO;

namespace TextMetal.Common.Core
{
	public class FileSystemEnumerator
	{
		#region Constructors/Destructors

		public FileSystemEnumerator()
		{
		}

		#endregion

		#region Fields/Constants

		private bool cancel;

		#endregion

		#region Properties/Indexers/Events

		public event DirectoryFoundHandler DirectoryFound;
		public event FileFoundHandler FileFound;

		public bool Cancel
		{
			get
			{
				return this.cancel;
			}
			private set
			{
				this.cancel = true;
			}
		}

		#endregion

		#region Methods/Operators

		public void EnumerateFileSystem(string enumerationPath)
		{
			this.EnumerateFileSystem(enumerationPath, true);
		}

		public void EnumerateFileSystem(string enumerationPath, bool recurse)
		{
			string[] directoryPaths;
			string[] filePaths;

			enumerationPath = Path.GetFullPath(enumerationPath);

			if (this.Cancel)
				return;

			if (File.Exists(enumerationPath))
			{
				filePaths = new string[] { enumerationPath };
				directoryPaths = new string[] { };
			}
			else if (Directory.Exists(enumerationPath))
			{
				filePaths = Directory.GetFiles(enumerationPath);
				directoryPaths = Directory.GetDirectories(enumerationPath);
			}
			else
			{
				filePaths = new string[] { };
				directoryPaths = new string[] { };
			}

			if ((object)filePaths != null)
			{
				foreach (string filePath in filePaths)
				{
					if (this.Cancel)
						return;

					if (this.FileFound != null)
						this.FileFound(new FileInfo(filePath));
				}
			}

			if ((object)directoryPaths != null)
			{
				foreach (string directoryPath in directoryPaths)
				{
					if (this.Cancel)
						return;

					if (this.DirectoryFound != null)
						this.DirectoryFound(new DirectoryInfo(directoryPath));

					if (recurse)
						this.EnumerateFileSystem(directoryPath);
				}
			}
		}

		public void SignalCancel()
		{
			if (this.Cancel)
				throw new InvalidOperationException("Already canceled");

			this.Cancel = true;
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		public delegate void DirectoryFoundHandler(DirectoryInfo directoryInfo);

		public delegate void FileFoundHandler(FileInfo fileInfo);

		#endregion
	}
}