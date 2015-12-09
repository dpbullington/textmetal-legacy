/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Data;

using NUnit.Framework;

using TextMetal.Middleware.UnitTests.TestingInfrastructure;

namespace TextMetal.Middleware.UnitTests.Datazoid.Impl.Mappings._
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
			TableTacticCommand<MockModelObject> tableTacticCommand;

			tableTacticCommand = new TableTacticCommand<MockModelObject>();

			Assert.IsNotNull(tableTacticCommand);

			tableTacticCommand.CommandBehavior = CommandBehavior.CloseConnection;
			Assert.AreEqual(CommandBehavior.CloseConnection, tableTacticCommand.CommandBehavior);

			tableTacticCommand.CommandPrepare = true;
			Assert.IsTrue(tableTacticCommand.CommandPrepare);

			tableTacticCommand.CommandText = "select";
			Assert.AreEqual("select", tableTacticCommand.CommandText);

			tableTacticCommand.CommandTimeout = 100;
			Assert.AreEqual(100, tableTacticCommand.CommandTimeout);

			tableTacticCommand.CommandType = CommandType.Text;
			Assert.AreEqual(CommandType.Text, tableTacticCommand.CommandType);

			tableTacticCommand.TacticParameters = new ITacticParameter[] { };
			Assert.IsNotNull(tableTacticCommand.TacticParameters);

			tableTacticCommand.ExpectedRecordsAffected = 123;
			Assert.AreEqual(123, tableTacticCommand.ExpectedRecordsAffected);

			tableTacticCommand.IsNullipotent = true;
			Assert.IsTrue(tableTacticCommand.IsNullipotent);
		}

		#endregion
	}
}