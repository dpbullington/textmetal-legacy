/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using NUnit.Framework;

namespace TextMetal.Middleware.UnitTests.Datazoid.Impl.Mappings._
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