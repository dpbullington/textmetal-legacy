/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Data;

using NUnit.Framework;

using TextMetal.Common.Data.Framework.Mapping;
using TextMetal.Common.UnitTests.TestingInfrastructure;

namespace TextMetal.Common.UnitTests.Data.Framework.Mapping._
{
	[TestFixture]
	public class TacticCommandTests
	{
		#region Constructors/Destructors

		public TacticCommandTests()
		{
		}

		#endregion

		#region Methods/Operators

		[Test]
		public void ShouldCreateTickOneTest()
		{
			TacticCommand<MockModelObject> tacticCommand;

			tacticCommand = new TacticCommand<MockModelObject>();

			Assert.IsNotNull(tacticCommand);

			tacticCommand.CommandBehavior = CommandBehavior.CloseConnection;
			Assert.AreEqual(CommandBehavior.CloseConnection, tacticCommand.CommandBehavior);

			tacticCommand.CommandPrepare = true;
			Assert.IsTrue(tacticCommand.CommandPrepare);

			tacticCommand.CommandText = "select";
			Assert.AreEqual("select", tacticCommand.CommandText);

			tacticCommand.CommandTimeout = 100;
			Assert.AreEqual(100, tacticCommand.CommandTimeout);

			tacticCommand.CommandType = CommandType.Text;
			Assert.AreEqual(CommandType.Text, tacticCommand.CommandType);

			tacticCommand.CommandParameters = new IDbDataParameter[] { };
			Assert.IsNotEmpty(tacticCommand.CommandParameters);

			tacticCommand.ExpectedRecordsAffected = 123;
			Assert.AreEqual(123, tacticCommand.ExpectedRecordsAffected);

			tacticCommand.IsNullipotent = true;
			Assert.IsTrue(tacticCommand.IsNullipotent);
		}

		#endregion
	}
}