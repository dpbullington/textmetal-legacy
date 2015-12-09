/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Data;

using NUnit.Framework;

namespace TextMetal.Middleware.UnitTests.Datazoid.Impl.Mappings._
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

			parameterMappingAttribute.ParameterSqlType = "foo";
			Assert.AreEqual("foo", parameterMappingAttribute.ParameterSqlType);

			parameterMappingAttribute.ParameterOrdinal = 10;
			Assert.AreEqual(10, parameterMappingAttribute.ParameterOrdinal);

			parameterMappingAttribute.ParameterSize = 10;
			Assert.AreEqual(10, parameterMappingAttribute.ParameterSize);

			parameterMappingAttribute.ParameterDbType = DbType.Int32;
			Assert.AreEqual(DbType.Int32, parameterMappingAttribute.ParameterDbType);

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