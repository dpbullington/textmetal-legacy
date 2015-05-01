/*
	Copyright �2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using NUnit.Framework;

using TextMetal.Middleware.Data.Impl.FreakazoidMapper.Mappings;

namespace TextMetal.Middleware.UnitTests.Data.Impl.FreakazoidMapper.Mappings._
{
	[TestFixture]
	public class TableMappingAttributeTests
	{
		#region Constructors/Destructors

		public TableMappingAttributeTests()
		{
		}

		#endregion

		#region Methods/Operators

		[Test]
		public void ShouldCreateTest()
		{
			TableMappingAttribute tableMappingAttribute;

			tableMappingAttribute = new TableMappingAttribute();

			Assert.IsNotNull(tableMappingAttribute);

			tableMappingAttribute.TableName = "x";
			Assert.AreEqual("x", tableMappingAttribute.TableName);

			tableMappingAttribute.SchemaName = "y";
			Assert.AreEqual("y", tableMappingAttribute.SchemaName);

			tableMappingAttribute.IsView = true;
			Assert.AreEqual(true, tableMappingAttribute.IsView);
		}

		#endregion
	}
}