/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.IO;

namespace TextMetal.Common.Core
{
	public enum VirtualFileSystemItemType
	{
		None = 0,
		File = 1,
		Directory = 2,
		Volume = 3,
		Link = 4
	}

	public sealed class VirtualFileSystemItem
	{
		#region Constructors/Destructors

		public VirtualFileSystemItem(VirtualFileSystemItemType itemType, string itemName, string itemPath)
		{
			this.itemType = itemType;
			this.itemName = itemName;
			this.itemPath = itemPath;
		}

		#endregion

		#region Fields/Constants

		private readonly string itemName;
		private readonly string itemPath;
		private readonly VirtualFileSystemItemType itemType;

		#endregion

		#region Properties/Indexers/Events

		public string ItemName
		{
			get
			{
				return this.itemName;
			}
		}

		public string ItemPath
		{
			get
			{
				return this.itemPath;
			}
		}

		public VirtualFileSystemItemType ItemType
		{
			get
			{
				return this.itemType;
			}
		}

		#endregion
	}

	public sealed class VirtualFileSystemEnumerator
	{
		#region Constructors/Destructors

		public VirtualFileSystemEnumerator()
		{
		}

		#endregion

		#region Methods/Operators

		public IEnumerable<VirtualFileSystemItem> EnumerateVirtualItems(string directoryPath, bool enableRecursion)
		{
			IEnumerable<string> directoryNames;
			IEnumerable<string> fileNames;

			if ((object)directoryPath == null)
				throw new ArgumentNullException("directoryPath");

			directoryPath = Path.GetFullPath(directoryPath);

			if (File.Exists(directoryPath))
				throw new DirectoryNotFoundException(directoryPath);

			if (!Directory.Exists(directoryPath))
				throw new DirectoryNotFoundException(directoryPath);

			directoryNames = Directory.EnumerateDirectories(directoryPath);

			foreach (string directoryName in directoryNames)
			{
				string tempDirectoryPath = Path.Combine(directoryPath, directoryName);
				yield return new VirtualFileSystemItem(VirtualFileSystemItemType.Directory, directoryName, tempDirectoryPath);

				if (enableRecursion)
				{
					var items = this.EnumerateVirtualItems(tempDirectoryPath, true);

					foreach (var item in items)
						yield return item;
				}
			}

			fileNames = Directory.EnumerateFiles(directoryPath);

			foreach (string fileName in fileNames)
				yield return new VirtualFileSystemItem(VirtualFileSystemItemType.File, fileName, Path.Combine(directoryPath, fileName));
		}

		#endregion
	}

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