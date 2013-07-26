/*
	Copyright ©2002-2013 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Data;

using NUnit.Framework;

using TextMetal.Common.Data.TypeMap.LowLevel;

namespace TextMetal.Common.UnitTests.Data.TypeMap.LowLevel
{
	[TestFixture]
	public class ParameterTests
	{
		#region Constructors/Destructors

		public ParameterTests()
		{
		}

		#endregion

		#region Methods/Operators

		[Test]
		public void ShouldCreateTest()
		{
			Parameter parameter;

			parameter = new Parameter();

			Assert.IsNotNull(parameter);

			parameter.Direction = ParameterDirection.InputOutput;
			Assert.AreEqual(ParameterDirection.InputOutput, parameter.Direction);

			parameter.Name = "value";
			Assert.AreEqual("value", parameter.Name);

			parameter.Size = 1;
			Assert.AreEqual(1, parameter.Size);

			parameter.Type = DbType.Int64;
			Assert.AreEqual(DbType.Int64, parameter.Type);

			parameter.Property = "myProp";
			Assert.AreEqual("myProp", parameter.Property);
		}

		#endregion
	}
}