/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using NUnit.Framework;

using TextMetal.Middleware.Datazoid.Repositories.Impl.Mappings;

namespace TextMetal.Middleware.UnitTests.Datazoid.Impl.Mappings._
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