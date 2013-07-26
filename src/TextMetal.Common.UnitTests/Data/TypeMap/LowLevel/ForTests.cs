/*
	Copyright ©2002-2013 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using NUnit.Framework;

using TextMetal.Common.Data.TypeMap.LowLevel;

namespace TextMetal.Common.UnitTests.Data.TypeMap.LowLevel
{
	[TestFixture]
	public class ForTests
	{
		#region Constructors/Destructors

		public ForTests()
		{
		}

		#endregion

		#region Methods/Operators

		[Test]
		public void ShouldCreateTest()
		{
			For @for;

			@for = new For();

			Assert.IsNotNull(@for);

			@for.Id = "myField";
			Assert.AreEqual("myField", @for.Id);

			Assert.IsNull(@for.Command);
			@for.Command = new Command();
			Assert.IsNotNull(@for.Command);
		}

		#endregion
	}
}