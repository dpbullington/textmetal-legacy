/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Middleware.Solder.Utilities.Vfs
{
	public sealed class VirtualFileSystemItem : IVirtualFileSystemItem
	{
		#region Constructors/Destructors

		public VirtualFileSystemItem(VirtualFileSystemItemType itemType, string itemName, string itemPath)
		{
			if ((object)itemName == null)
				throw new ArgumentNullException(nameof(itemName));

			if ((object)itemPath == null)
				throw new ArgumentNullException(nameof(itemPath));

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
}