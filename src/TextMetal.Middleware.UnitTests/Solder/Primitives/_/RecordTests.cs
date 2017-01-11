/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using NUnit.Framework;

using TextMetal.Middleware.Solder.Primitives;

namespace TextMetal.Middleware.UnitTests.Solder.Primitives._
{
	[TestFixture]
	public class RecordTests
	{
		#region Constructors/Destructors

		public RecordTests()
		{
		}

		#endregion

		#region Methods/Operators

		[Test]
		public void ShouldCreateTest()
		{
			Record record;
			object RECORD_CONTEXT = new object();

			record = new Record();
			record.Context = RECORD_CONTEXT;

			Assert.AreEqual(RECORD_CONTEXT, record.Context);
		}

		#endregion
	}
}