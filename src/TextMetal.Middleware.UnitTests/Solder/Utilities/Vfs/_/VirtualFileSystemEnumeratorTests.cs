/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using NUnit.Framework;

using TextMetal.Middleware.Solder.Utilities.Vfs;
using TextMetal.Middleware.UnitTests.TestingInfrastructure;

namespace TextMetal.Middleware.UnitTests.Solder.Utilities.Vfs._
{
	[TestFixture]
	public class VirtualFileSystemEnumeratorTests
	{
		#region Constructors/Destructors

		public VirtualFileSystemEnumeratorTests()
		{
		}

		#endregion

		#region Methods/Operators

		[Test]
		public void ShouldCreateTest()
		{
			VirtualFileSystemEnumerator virtualFileSystemEnumerator;

			const VirtualFileSystemItemType ITEM_TYPE = VirtualFileSystemItemType.Link;
			const string ITEM_NAME = "myItem";
			const string ITEM_PATH = "/myPath/";

			virtualFileSystemEnumerator = new VirtualFileSystemEnumerator();

			Assert.AreEqual(ITEM_TYPE, virtualFileSystemItem.ItemType);
			Assert.AreEqual(ITEM_NAME, virtualFileSystemItem.ItemName);
			Assert.AreEqual(ITEM_PATH, virtualFileSystemItem.ItemPath);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullItemNameCreateTest()
		{
			VirtualFileSystemItem virtualFileSystemItem;

			const VirtualFileSystemItemType ITEM_TYPE = VirtualFileSystemItemType.Link;
			const string ITEM_NAME = null;
			const string ITEM_PATH = "/myPath/";

			virtualFileSystemItem = new VirtualFileSystemItem(ITEM_TYPE, ITEM_NAME, ITEM_PATH);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullItemPathCreateTest()
		{
			VirtualFileSystemItem virtualFileSystemItem;

			const VirtualFileSystemItemType ITEM_TYPE = VirtualFileSystemItemType.Link;
			const string ITEM_NAME = "myItem";
			const string ITEM_PATH = null;

			virtualFileSystemItem = new VirtualFileSystemItem(ITEM_TYPE, ITEM_NAME, ITEM_PATH);
		}

		#endregion
	}
}