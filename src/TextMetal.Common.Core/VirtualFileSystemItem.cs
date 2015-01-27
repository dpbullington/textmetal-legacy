/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

namespace TextMetal.Common.Core
{
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
}