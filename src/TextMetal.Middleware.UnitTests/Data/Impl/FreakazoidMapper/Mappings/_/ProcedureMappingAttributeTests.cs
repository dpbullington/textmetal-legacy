/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using NUnit.Framework;

using TextMetal.Middleware.Data.Impl.FreakazoidMapper.Mappings;

namespace TextMetal.Middleware.UnitTests.Data.Impl.FreakazoidMapper.Mappings._
{
	[TestFixture]
	public class ProcedureMappingAttributeTests
	{
		#region Constructors/Destructors

		public ProcedureMappingAttributeTests()
		{
		}

		#endregion

		#region Methods/Operators

		[Test]
		public void ShouldCreateTest()
		{
			ProcedureMappingAttribute procedureMappingAttribute;

			procedureMappingAttribute = new ProcedureMappingAttribute();

			Assert.IsNotNull(procedureMappingAttribute);

			procedureMappingAttribute.ProcedureName = "x";
			Assert.AreEqual("x", procedureMappingAttribute.ProcedureName);

			procedureMappingAttribute.SchemaName = "y";
			Assert.AreEqual("y", procedureMappingAttribute.SchemaName);

			procedureMappingAttribute.DatabaseName = "z";
			Assert.AreEqual("z", procedureMappingAttribute.DatabaseName);

			procedureMappingAttribute.IsFunction = true;
			Assert.AreEqual(true, procedureMappingAttribute.IsFunction);
		}

		#endregion
	}
}