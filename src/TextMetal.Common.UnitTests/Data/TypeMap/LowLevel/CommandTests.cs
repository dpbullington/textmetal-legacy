/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Data;

using NUnit.Framework;

using TextMetal.Common.Data.TypeMap.LowLevel;

namespace TextMetal.Common.UnitTests.Data.TypeMap.LowLevel
{
	[TestFixture]
	public class CommandTests
	{
		#region Constructors/Destructors

		public CommandTests()
		{
		}

		#endregion

		#region Methods/Operators

		[Test]
		public void ShouldCreateTest()
		{
			Command command;

			command = new Command();

			Assert.IsNotNull(command);

			command._Timeout = "30";
			Assert.AreEqual("30", command._Timeout);

			command._Timeout = "";
			Assert.IsNull(command.Timeout);

			command._Timeout = "not a number";
			Assert.IsNull(command.Timeout);

			Assert.IsNotNull(command.Parameters);
			Assert.AreEqual(0, command.Parameters.Count);

			command.Prepare = true;
			Assert.AreEqual(true, command.Prepare);

			command.Text = "xxx";
			Assert.AreEqual("xxx", command.Text);

			command.Timeout = 30;
			Assert.AreEqual(30, command.Timeout);

			command.Type = CommandType.StoredProcedure;
			Assert.AreEqual(CommandType.StoredProcedure, command.Type);

			command.Identity = "www";
			Assert.AreEqual("www", command.Identity);
		}

		#endregion
	}
}