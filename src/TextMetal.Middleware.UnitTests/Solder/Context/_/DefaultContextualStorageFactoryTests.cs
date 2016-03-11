/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using NUnit.Framework;

using TextMetal.Middleware.Solder.Context;

namespace TextMetal.Middleware.UnitTests.Solder.Context._
{
	[TestFixture]
	public class DefaultContextualStorageFactoryTests
	{
		#region Constructors/Destructors

		public DefaultContextualStorageFactoryTests()
		{
		}

		#endregion

		#region Methods/Operators

		[Test]
		public void ShouldCreateTest()
		{
			DefaultContextualStorageFactory defaultContextualStorageFactory;

			defaultContextualStorageFactory = new DefaultContextualStorageFactory();

			Assert.IsNotNull(defaultContextualStorageFactory);
			Assert.IsNotNull(defaultContextualStorageFactory.GetContextualStorage());
		}

		#endregion
	}
}