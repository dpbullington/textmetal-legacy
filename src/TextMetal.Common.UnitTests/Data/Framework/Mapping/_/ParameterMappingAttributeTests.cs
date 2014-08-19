/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Data;

using NUnit.Framework;

using TextMetal.Common.Data.Framework.Mapping;

namespace TextMetal.Common.UnitTests.Data.Framework.Mapping._
{
	[TestFixture]
	public class ParameterMappingAttributeTests
	{
		#region Constructors/Destructors

		public ParameterMappingAttributeTests()
		{
		}

		#endregion

		#region Methods/Operators

		[Test]
		public void ShouldCreateTest()
		{
			ParameterMappingAttribute parameterMappingAttribute;

			parameterMappingAttribute = new ParameterMappingAttribute();

			Assert.IsNotNull(parameterMappingAttribute);

			parameterMappingAttribute.ParameterDirection = ParameterDirection.Input;
			Assert.AreEqual(ParameterDirection.Input, parameterMappingAttribute.ParameterDirection);

			parameterMappingAttribute.ParameterName = "@ItemId";
			Assert.AreEqual("@ItemId", parameterMappingAttribute.ParameterName);

			parameterMappingAttribute.ParameterSize = 10;
			Assert.AreEqual(10, parameterMappingAttribute.ParameterSize);

			parameterMappingAttribute.DbType = DbType.Int32;
			Assert.AreEqual(DbType.Int32, parameterMappingAttribute.DbType);

			parameterMappingAttribute.ParameterNullable = true;
			Assert.AreEqual(true, parameterMappingAttribute.ParameterNullable);

			parameterMappingAttribute.ParameterPrecision = 1;
			Assert.AreEqual(1, parameterMappingAttribute.ParameterPrecision);

			parameterMappingAttribute.ParameterScale = 3;
			Assert.AreEqual(3, parameterMappingAttribute.ParameterScale);
		}

		#endregion
	}
}