/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using NUnit.Framework;

using TextMetal.Common.Data.TypeMap.LowLevel;

namespace TextMetal.Common.UnitTests.Data.TypeMap.LowLevel
{
	[TestFixture]
	public class FieldTests
	{
		#region Constructors/Destructors

		public FieldTests()
		{
		}

		#endregion

		#region Methods/Operators

		[Test]
		public void ShouldCreateTest()
		{
			Field field;

			field = new Field();

			Assert.IsNotNull(field);

			field.Name = "myField";
			Assert.AreEqual("myField", field.Name);

			field.Property = "myProp";
			Assert.AreEqual("myProp", field.Property);
		}

		#endregion
	}
}