/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using NUnit.Framework;

using TextMetal.Middleware.Solder.IoC;

namespace TextMetal.Middleware.UnitTests.Solder.IoC._
{
	[TestFixture]
	public class DependencyRegistrationAttributeTests
	{
		#region Constructors/Destructors

		public DependencyRegistrationAttributeTests()
		{
		}

		#endregion

		#region Methods/Operators

		[Test]
		public void ShouldCreateTest()
		{
			DependencyRegistrationAttribute attribute;

			attribute = new DependencyRegistrationAttribute();

			Assert.IsNotNull(attribute);
		}

		#endregion
	}
}