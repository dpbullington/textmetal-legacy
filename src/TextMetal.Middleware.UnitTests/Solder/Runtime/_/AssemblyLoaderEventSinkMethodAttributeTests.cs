/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using NUnit.Framework;

using TextMetal.Middleware.Solder.Runtime;

namespace TextMetal.Middleware.UnitTests.Solder.Runtime._
{
	[TestFixture]
	public class AssemblyLoaderEventSinkMethodAttributeTests
	{
		#region Constructors/Destructors

		public AssemblyLoaderEventSinkMethodAttributeTests()
		{
		}

		#endregion

		#region Methods/Operators

		[Test]
		public void ShouldCreateTest()
		{
			AssemblyLoaderEventSinkMethodAttribute attribute;

			attribute = new AssemblyLoaderEventSinkMethodAttribute();
			Assert.IsNotNull(attribute);
		}

		#endregion
	}
}