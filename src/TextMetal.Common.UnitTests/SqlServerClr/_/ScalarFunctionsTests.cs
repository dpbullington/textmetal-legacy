/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using NUnit.Framework;

using TextMetal.Common.SqlServerClr;

namespace TextMetal.Common.UnitTests.SqlServerClr._
{
	[TestFixture]
	public class ScalarFunctionsTests
	{
		#region Constructors/Destructors

		public ScalarFunctionsTests()
		{
		}

		#endregion

		#region Methods/Operators

		[Test]
		public void ShouldTest0()
		{
			int hashCode;

			var r = new Random(1);
			hashCode = r.Next(-10, 10);
			Console.WriteLine(hashCode);
			hashCode = r.Next(-10, 10);

			Console.WriteLine(hashCode);
		}

		[Test]
		public void ShouldTest1()
		{
			int? hashCode;

			hashCode = ScalarFunctions.GetHash(33, 1000, 5381, "textmetal");

			Assert.AreEqual(153, hashCode);
		}
	
		#endregion
	}
}