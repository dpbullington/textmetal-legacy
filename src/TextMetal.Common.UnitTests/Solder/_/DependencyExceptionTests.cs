/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using NUnit.Framework;

using TextMetal.Common.Solder;

namespace TextMetal.Common.UnitTests.Solder._
{
	[TestFixture]
	public class DependencyExceptionTests
	{
		#region Constructors/Destructors

		public DependencyExceptionTests()
		{
		}

		#endregion

		#region Methods/Operators

		[Test]
		public void ShouldCreateTest()
		{
			DependencyException exception;

			exception = new DependencyException("msg");

			Assert.IsNotNull(exception);
			Assert.IsNotNull(exception.Message);
			Assert.AreEqual("msg", exception.Message);
		}

		#endregion
	}
}