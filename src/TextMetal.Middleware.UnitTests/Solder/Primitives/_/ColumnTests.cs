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
	public class ColumnTests
	{
		#region Constructors/Destructors

		public ColumnTests()
		{
		}

		#endregion

		#region Methods/Operators

		[Test]
		public void ShouldCreateTest()
		{
			Column column;
			const int COLUMN_INDEX = 99;
			const bool COLUMN_IS_NULLABLE = true;
			const string COLUMN_NAME = "theColumn";
			Type COLUMN_TYPE = typeof(String);
			object COLUMN_CONTEXT = new object();
			const int TABLE_INDEX = 10;

			column = new Column();
			column.ColumnIndex = COLUMN_INDEX;
			column.ColumnIsNullable = COLUMN_IS_NULLABLE;
			column.ColumnName = COLUMN_NAME;
			column.ColumnType = COLUMN_TYPE;
			column.Context = COLUMN_CONTEXT;
			column.TableIndex = TABLE_INDEX;

			Assert.AreEqual(COLUMN_INDEX, column.ColumnIndex);
			Assert.AreEqual(COLUMN_IS_NULLABLE, column.ColumnIsNullable);
			Assert.AreEqual(COLUMN_NAME, column.ColumnName);
			Assert.AreEqual(COLUMN_TYPE, column.ColumnType);
			Assert.AreEqual(COLUMN_CONTEXT, column.Context);
			Assert.AreEqual(TABLE_INDEX, column.TableIndex);
		}

		#endregion
	}
}