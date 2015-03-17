/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.IO;

namespace LeastViable.VsIdeConv.ConsoleTool
{
	[Obsolete("Use VirtualFileSystemEnumerator instead")]
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

		public event Action<DirectoryInfo> DirectoryFound;
		public event Action<FileInfo> FileFound;

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
			VirtualFileSystemEnumerator virtualFileSystemEnumerator;
			IEnumerable<VirtualFileSystemItem> virtualFileSystemItems;

			if ((object)enumerationPath == null)
				throw new ArgumentNullException("enumerationPath");

			if (this.Cancel)
				return;

			virtualFileSystemEnumerator = new VirtualFileSystemEnumerator();
			virtualFileSystemItems = virtualFileSystemEnumerator.EnumerateVirtualItems(enumerationPath, true);

			if ((object)virtualFileSystemItems != null)
			{
				foreach (VirtualFileSystemItem virtualFileSystemItem in virtualFileSystemItems)
				{
					if (this.Cancel)
						return;

					switch (virtualFileSystemItem.ItemType)
					{
						case VirtualFileSystemItemType.File:

							if (this.FileFound != null)
								this.FileFound(new FileInfo(virtualFileSystemItem.ItemPath));

							break;
						case VirtualFileSystemItemType.Directory:

							if (this.DirectoryFound != null)
								this.DirectoryFound(new DirectoryInfo(virtualFileSystemItem.ItemPath));

							if (recurse)
								this.EnumerateFileSystem(virtualFileSystemItem.ItemPath, recurse);

							break;
						default:
							throw new ArgumentOutOfRangeException(virtualFileSystemItem.ItemType.ToString());
					}
				}
			}
		}

		public void SignalCancel()
		{
			if (this.Cancel)
				throw new InvalidOperationException("Already canceled.");

			this.Cancel = true;
		}

		#endregion
	}
}