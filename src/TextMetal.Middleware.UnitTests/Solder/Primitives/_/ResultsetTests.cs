/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using NUnit.Framework;

using TextMetal.Middleware.Solder.Primitives;

namespace TextMetal.Middleware.UnitTests.Solder.Primitives._
{
	[TestFixture]
	public class ResultsetTests
	{
		#region Constructors/Destructors

		public ResultsetTests()
		{
		}

		#endregion

		#region Methods/Operators

		[Test]
		public void ShouldCreateTest()
		{
			Resultset resultset;
			const int RESULTSET_INDEX = 10;
			object RESULTSET_CONTEXT = new object();
			IEnumerable<IRecord> RESULTSET_RECORDS_ENUMERABLE = new Record[10];
			const int RESULTSET_RECORDS_AFFECTED = -1;

			resultset = new Resultset(RESULTSET_INDEX);
			resultset.Context = RESULTSET_CONTEXT;
			resultset.Records = RESULTSET_RECORDS_ENUMERABLE;
			resultset.RecordsAffected = RESULTSET_RECORDS_AFFECTED;

			Assert.AreEqual(RESULTSET_INDEX, resultset.Index);
			Assert.AreEqual(RESULTSET_CONTEXT, resultset.Context);
			Assert.AreEqual(RESULTSET_RECORDS_ENUMERABLE, resultset.Records);
			Assert.AreSame(RESULTSET_RECORDS_ENUMERABLE, resultset.Records);
			Assert.IsNotEmpty(resultset.Records);
			Assert.AreEqual(RESULTSET_RECORDS_AFFECTED, resultset.RecordsAffected);
		}

		#endregion
	}
}