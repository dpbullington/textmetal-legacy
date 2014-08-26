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
	public class ColumnMappingAttributeTests
	{
		#region Constructors/Destructors

		public ColumnMappingAttributeTests()
		{
		}

		#endregion

		#region Methods/Operators

		[Test]
		public void ShouldCreateTest()
		{
			ColumnMappingAttribute attribute;

			attribute = new ColumnMappingAttribute();

			Assert.IsNotNull(attribute);

			attribute.ColumnName = "x";
			Assert.AreEqual("x", attribute.ColumnName);

			attribute.ColumnSqlType = "x";
			Assert.AreEqual("x", attribute.ColumnSqlType);

			attribute.ColumnSize = 1;
			Assert.AreEqual(1, attribute.ColumnSize);

			attribute.ColumnDbType = DbType.Guid;
			Assert.AreEqual(DbType.Guid, attribute.ColumnDbType);

			attribute.ColumnIsComputed = true;
			Assert.AreEqual(true, attribute.ColumnIsComputed);

			attribute.ColumnIsIdentity = true;
			Assert.AreEqual(true, attribute.ColumnIsIdentity);

			attribute.ColumnIsPrimaryKey = true;
			Assert.AreEqual(true, attribute.ColumnIsPrimaryKey);

			attribute.ColumnNullable = true;
			Assert.AreEqual(true, attribute.ColumnNullable);

			attribute.ColumnPrecision = 1;
			Assert.AreEqual(1, attribute.ColumnPrecision);

			attribute.ColumnScale = 3;
			Assert.AreEqual(3, attribute.ColumnScale);

			attribute.ColumnOrdinal = 4;
			Assert.AreEqual(4, attribute.ColumnOrdinal);
		}

		#endregion
	}
}