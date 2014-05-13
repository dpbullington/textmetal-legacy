/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Data;

using NUnit.Framework;

using TextMetal.Common.Data.Framework.PoPimp.Mapping;

namespace TextMetal.Common.UnitTests.Data.Framework.ModelMap.Mapping._
{
	[TestFixture]
	public class CommandMappingAttributeTests
	{
		#region Constructors/Destructors

		public CommandMappingAttributeTests()
		{
		}

		#endregion

		#region Methods/Operators

		[Test]
		public void ShouldCreateTest()
		{
			CommandMappingAttribute commandMappingAttribute;

			commandMappingAttribute = new CommandMappingAttribute();

			Assert.IsNotNull(commandMappingAttribute);

			commandMappingAttribute.CommandBehavior = CommandBehavior.CloseConnection;
			Assert.AreEqual(CommandBehavior.CloseConnection, commandMappingAttribute.CommandBehavior);

			commandMappingAttribute.CommandPrepare = true;
			Assert.IsTrue(commandMappingAttribute.CommandPrepare);

			commandMappingAttribute.Text = "select";
			Assert.AreEqual("select", commandMappingAttribute.Text);

			commandMappingAttribute.CommandTimeout = 100;
			Assert.AreEqual(100, commandMappingAttribute.CommandTimeout);

			commandMappingAttribute.CommandType = CommandType.Text;
			Assert.AreEqual(CommandType.Text, commandMappingAttribute.CommandType);
		}

		#endregion
	}
}