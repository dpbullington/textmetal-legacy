/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.InteropServices;

using NUnit.Framework;

using TextMetal.Middleware.Solder.Utilities;

namespace TextMetal.Middleware.UnitTests.Solder.Utilities._
{
	[TestFixture]
	public class AppConfigFascadeTests
	{
		#region Constructors/Destructors

		public AppConfigFascadeTests()
		{
		}

		#endregion

		#region Methods/Operators

		[Test]
		[ExpectedException(typeof(ConfigurationErrorsException))]
		public void ShouldFailOnInvalidValueGetBooleanTest()
		{
			AppConfigFascade.Instance.GetAppSetting<Boolean>("BadAppConfigFascadeValueBoolean");
		}

		[Test]
		[ExpectedException(typeof(ConfigurationErrorsException))]
		public void ShouldFailOnInvalidValueGetByteTest()
		{
			AppConfigFascade.Instance.GetAppSetting<Byte>("BadAppConfigFascadeValueByte");
		}

		[Test]
		[ExpectedException(typeof(ConfigurationErrorsException))]
		public void ShouldFailOnInvalidValueGetCharTest()
		{
			AppConfigFascade.Instance.GetAppSetting<Char>("BadAppConfigFascadeValueChar");
		}

		[Test]
		[ExpectedException(typeof(ConfigurationErrorsException))]
		public void ShouldFailOnInvalidValueGetConnectionProviderTest()
		{
			AppConfigFascade.Instance.GetConnectionProvider("BadMyProvider");
		}

		[Test]
		[ExpectedException(typeof(ConfigurationErrorsException))]
		public void ShouldFailOnInvalidValueGetConnectionStringTest()
		{
			AppConfigFascade.Instance.GetConnectionString("BadMyProvider");
		}

		[Test]
		[ExpectedException(typeof(ConfigurationErrorsException))]
		public void ShouldFailOnInvalidValueGetDateTimeTest()
		{
			AppConfigFascade.Instance.GetAppSetting<DateTime>("BadAppConfigFascadeValueDateTime");
		}

		[Test]
		[ExpectedException(typeof(ConfigurationErrorsException))]
		public void ShouldFailOnInvalidValueGetDecimalTest()
		{
			AppConfigFascade.Instance.GetAppSetting<Decimal>("BadAppConfigFascadeValueDecimal");
		}

		[Test]
		[ExpectedException(typeof(ConfigurationErrorsException))]
		public void ShouldFailOnInvalidValueGetDoubleTest()
		{
			AppConfigFascade.Instance.GetAppSetting<Double>("BadAppConfigFascadeValueDouble");
		}

		[Test]
		[ExpectedException(typeof(ConfigurationErrorsException))]
		public void ShouldFailOnInvalidValueGetEnumTest()
		{
			AppConfigFascade.Instance.GetAppSetting<CharSet>("BadAppConfigFascadeValueEnum");
		}

		[Test]
		[ExpectedException(typeof(ConfigurationErrorsException))]
		public void ShouldFailOnInvalidValueGetGuidTest()
		{
			AppConfigFascade.Instance.GetAppSetting<Guid>("BadAppConfigFascadeValueGuid");
		}

		[Test]
		[ExpectedException(typeof(ConfigurationErrorsException))]
		public void ShouldFailOnInvalidValueGetInt16Test()
		{
			AppConfigFascade.Instance.GetAppSetting<Int16>("BadAppConfigFascadeValueInt16");
		}

		[Test]
		[ExpectedException(typeof(ConfigurationErrorsException))]
		public void ShouldFailOnInvalidValueGetInt32Test()
		{
			AppConfigFascade.Instance.GetAppSetting<Int32>("BadAppConfigFascadeValueInt32");
		}

		[Test]
		[ExpectedException(typeof(ConfigurationErrorsException))]
		public void ShouldFailOnInvalidValueGetInt64Test()
		{
			AppConfigFascade.Instance.GetAppSetting<Int64>("BadAppConfigFascadeValueInt64");
		}

		[Test]
		[ExpectedException(typeof(ConfigurationErrorsException))]
		public void ShouldFailOnInvalidValueGetSByteTest()
		{
			AppConfigFascade.Instance.GetAppSetting<SByte>("BadAppConfigFascadeValueSByte");
		}

		[Test]
		[ExpectedException(typeof(ConfigurationErrorsException))]
		public void ShouldFailOnInvalidValueGetSingleTest()
		{
			AppConfigFascade.Instance.GetAppSetting<Single>("BadAppConfigFascadeValueSingle");
		}

		[Test]
		[ExpectedException(typeof(ConfigurationErrorsException))]
		public void ShouldFailOnInvalidValueGetTimeSpanTest()
		{
			AppConfigFascade.Instance.GetAppSetting<TimeSpan>("BadAppConfigFascadeValueTimeSpan");
		}

		[Test]
		[ExpectedException(typeof(ConfigurationErrorsException))]
		public void ShouldFailOnInvalidValueGetUInt16Test()
		{
			AppConfigFascade.Instance.GetAppSetting<UInt16>("BadAppConfigFascadeValueUInt16");
		}

		[Test]
		[ExpectedException(typeof(ConfigurationErrorsException))]
		public void ShouldFailOnInvalidValueGetUInt32Test()
		{
			AppConfigFascade.Instance.GetAppSetting<UInt32>("BadAppConfigFascadeValueUInt32");
		}

		[Test]
		[ExpectedException(typeof(ConfigurationErrorsException))]
		public void ShouldFailOnInvalidValueGetUInt64Test()
		{
			AppConfigFascade.Instance.GetAppSetting<UInt64>("BadAppConfigFascadeValueUInt64");
		}

		[Test]
		[ExpectedException(typeof(ConfigurationErrorsException))]
		public void ShouldFailOnNonExistKeyGetBooleanTest()
		{
			AppConfigFascade.Instance.GetAppSetting<Boolean>("NotThereAppConfigFascadeValueBoolean");
		}

		[Test]
		[ExpectedException(typeof(ConfigurationErrorsException))]
		public void ShouldFailOnNonExistKeyGetByteTest()
		{
			AppConfigFascade.Instance.GetAppSetting<Byte>("NotThereAppConfigFascadeValueByte");
		}

		[Test]
		[ExpectedException(typeof(ConfigurationErrorsException))]
		public void ShouldFailOnNonExistKeyGetCharTest()
		{
			AppConfigFascade.Instance.GetAppSetting<Char>("NotThereAppConfigFascadeValueChar");
		}

		[Test]
		[ExpectedException(typeof(ConfigurationErrorsException))]
		public void ShouldFailOnNonExistKeyGetConnectionProviderTest()
		{
			AppConfigFascade.Instance.GetConnectionProvider("NotThereMyProvider");
		}

		[Test]
		[ExpectedException(typeof(ConfigurationErrorsException))]
		public void ShouldFailOnNonExistKeyGetConnectionStringTest()
		{
			AppConfigFascade.Instance.GetConnectionString("NotThereMyProvider");
		}

		[Test]
		[ExpectedException(typeof(ConfigurationErrorsException))]
		public void ShouldFailOnNonExistKeyGetDateTimeTest()
		{
			AppConfigFascade.Instance.GetAppSetting<DateTime>("NotThereAppConfigFascadeValueDateTime");
		}

		[Test]
		[ExpectedException(typeof(ConfigurationErrorsException))]
		public void ShouldFailOnNonExistKeyGetDecimalTest()
		{
			AppConfigFascade.Instance.GetAppSetting<Decimal>("NotThereAppConfigFascadeValueDecimal");
		}

		[Test]
		[ExpectedException(typeof(ConfigurationErrorsException))]
		public void ShouldFailOnNonExistKeyGetDoubleTest()
		{
			AppConfigFascade.Instance.GetAppSetting<Double>("NotThereAppConfigFascadeValueDouble");
		}

		[Test]
		[ExpectedException(typeof(ConfigurationErrorsException))]
		public void ShouldFailOnNonExistKeyGetEnumTest()
		{
			AppConfigFascade.Instance.GetAppSetting<CharSet>("NotThereAppConfigFascadeValueEnum");
		}

		[Test]
		[ExpectedException(typeof(ConfigurationErrorsException))]
		public void ShouldFailOnNonExistKeyGetGuidTest()
		{
			AppConfigFascade.Instance.GetAppSetting<Guid>("NotThereAppConfigFascadeValueGuid");
		}

		[Test]
		[ExpectedException(typeof(ConfigurationErrorsException))]
		public void ShouldFailOnNonExistKeyGetInt16Test()
		{
			AppConfigFascade.Instance.GetAppSetting<Int16>("NotThereAppConfigFascadeValueInt16");
		}

		[Test]
		[ExpectedException(typeof(ConfigurationErrorsException))]
		public void ShouldFailOnNonExistKeyGetInt32Test()
		{
			AppConfigFascade.Instance.GetAppSetting<Int32>("NotThereAppConfigFascadeValueInt32");
		}

		[Test]
		[ExpectedException(typeof(ConfigurationErrorsException))]
		public void ShouldFailOnNonExistKeyGetInt64Test()
		{
			AppConfigFascade.Instance.GetAppSetting<Int64>("NotThereAppConfigFascadeValueInt64");
		}

		[Test]
		[ExpectedException(typeof(ConfigurationErrorsException))]
		public void ShouldFailOnNonExistKeyGetSByteTest()
		{
			AppConfigFascade.Instance.GetAppSetting<SByte>("NotThereAppConfigFascadeValueSByte");
		}

		[Test]
		[ExpectedException(typeof(ConfigurationErrorsException))]
		public void ShouldFailOnNonExistKeyGetSingleTest()
		{
			AppConfigFascade.Instance.GetAppSetting<Single>("NotThereAppConfigFascadeValueSingle");
		}

		[Test]
		[ExpectedException(typeof(ConfigurationErrorsException))]
		public void ShouldFailOnNonExistKeyGetTimeSpanTest()
		{
			AppConfigFascade.Instance.GetAppSetting<TimeSpan>("NotThereAppConfigFascadeValueTimeSpan");
		}

		[Test]
		[ExpectedException(typeof(ConfigurationErrorsException))]
		public void ShouldFailOnNonExistKeyGetUInt16Test()
		{
			AppConfigFascade.Instance.GetAppSetting<UInt16>("NotThereAppConfigFascadeValueUInt16");
		}

		[Test]
		[ExpectedException(typeof(ConfigurationErrorsException))]
		public void ShouldFailOnNonExistKeyGetUInt32Test()
		{
			AppConfigFascade.Instance.GetAppSetting<UInt32>("NotThereAppConfigFascadeValueUInt32");
		}

		[Test]
		[ExpectedException(typeof(ConfigurationErrorsException))]
		public void ShouldFailOnNonExistKeyGetUInt64Test()
		{
			AppConfigFascade.Instance.GetAppSetting<UInt64>("NotThereAppConfigFascadeValueUInt64");
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullArgsParseCommandLineArgumentsTest()
		{
			AppConfigFascade.Instance.ParseCommandLineArguments(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyGetBooleanTest()
		{
			AppConfigFascade.Instance.GetAppSetting<Boolean>(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyGetByteTest()
		{
			AppConfigFascade.Instance.GetAppSetting<Byte>(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyGetCharTest()
		{
			AppConfigFascade.Instance.GetAppSetting<Char>(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyGetDateTimeTest()
		{
			AppConfigFascade.Instance.GetAppSetting<DateTime>(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyGetDecimalTest()
		{
			AppConfigFascade.Instance.GetAppSetting<Decimal>(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyGetDoubleTest()
		{
			AppConfigFascade.Instance.GetAppSetting<Double>(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyGetEnumTest()
		{
			AppConfigFascade.Instance.GetAppSetting<CharSet>(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyGetGuidTest()
		{
			AppConfigFascade.Instance.GetAppSetting<Guid>(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyGetInt16Test()
		{
			AppConfigFascade.Instance.GetAppSetting<Int16>(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyGetInt32Test()
		{
			AppConfigFascade.Instance.GetAppSetting<Int32>(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyGetInt64Test()
		{
			AppConfigFascade.Instance.GetAppSetting<Int64>(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyGetSByteTest()
		{
			AppConfigFascade.Instance.GetAppSetting<SByte>(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyGetSingleTest()
		{
			AppConfigFascade.Instance.GetAppSetting<Single>(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyGetTimeSpanTest()
		{
			AppConfigFascade.Instance.GetAppSetting<TimeSpan>(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyGetUInt16Test()
		{
			AppConfigFascade.Instance.GetAppSetting<UInt16>(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyGetUInt32Test()
		{
			AppConfigFascade.Instance.GetAppSetting<UInt32>(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyGetUInt64Test()
		{
			AppConfigFascade.Instance.GetAppSetting<UInt64>(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyHasAppSettingTest()
		{
			AppConfigFascade.Instance.HasAppSetting(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullNameGetAppSettingTest()
		{
			AppConfigFascade.Instance.GetAppSetting<string>(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullNameGetConnectionProviderTest()
		{
			AppConfigFascade.Instance.GetConnectionProvider(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullNameGetConnectionStringTest()
		{
			AppConfigFascade.Instance.GetConnectionString(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullNameHasConnectionStringTest()
		{
			AppConfigFascade.Instance.HasConnectionString(null);
		}

		[Test]
		public void ShouldGetArgsParseCommandLineArgumentsTest()
		{
			List<string> args;
			IDictionary<string, IList<string>> cmdlnargs;

			args = new List<string>();
			args.Add("arg0");
			args.Add(string.Empty);
			args.Add("-");
			args.Add("-arg1");
			args.Add("-arg2:");
			args.Add("-arg3:");
			args.Add("-arg4:value4");
			args.Add("-arg5:value5");
			args.Add("-arg4:value4");
			args.Add("arg6:value6");
			args.Add("-:value7");
			args.Add("-arg8:value8a");
			args.Add("-arg8:value8b");
			args.Add("-arg8:value8c");
			args.Add("-arg8:value8a");

			cmdlnargs = AppConfigFascade.Instance.ParseCommandLineArguments(args.ToArray());

			Assert.IsNotNull(cmdlnargs);
			Assert.AreEqual(3, cmdlnargs.Count);

			Assert.AreEqual(1, cmdlnargs["arg4"].Count);
			Assert.AreEqual("value4", cmdlnargs["arg4"][0]);

			Assert.AreEqual(1, cmdlnargs["arg5"].Count);
			Assert.AreEqual("value5", cmdlnargs["arg5"][0]);

			Assert.AreEqual(3, cmdlnargs["arg8"].Count);
			Assert.AreEqual("value8a", cmdlnargs["arg8"][0]);
			Assert.AreEqual("value8b", cmdlnargs["arg8"][1]);
			Assert.AreEqual("value8c", cmdlnargs["arg8"][2]);
		}

		[Test]
		public void ShouldGetBooleanTest()
		{
			Assert.AreEqual(AppConfigFascade.Instance.GetAppSetting<Boolean>("AppConfigFascadeValueBoolean"), true);
		}

		[Test]
		public void ShouldGetByteTest()
		{
			Assert.AreEqual(AppConfigFascade.Instance.GetAppSetting<Byte>("AppConfigFascadeValueByte"), 0);
		}

		[Test]
		public void ShouldGetCharTest()
		{
			Assert.AreEqual(AppConfigFascade.Instance.GetAppSetting<Char>("AppConfigFascadeValueChar"), '0');
		}

		[Test]
		public void ShouldGetConnectionProviderTest()
		{
			Assert.AreEqual(AppConfigFascade.Instance.GetConnectionProvider("MyProvider"), "sqlkiller");
		}

		[Test]
		public void ShouldGetConnectionStringTest()
		{
			Assert.AreEqual(AppConfigFascade.Instance.GetConnectionString("MyProvider"), "sql=con");
		}

		[Test]
		public void ShouldGetDateTimeTest()
		{
			Assert.AreEqual(AppConfigFascade.Instance.GetAppSetting<DateTime>("AppConfigFascadeValueDateTime"), new DateTime(2003, 6, 22));
		}

		[Test]
		public void ShouldGetDecimalTest()
		{
			Assert.AreEqual(AppConfigFascade.Instance.GetAppSetting<Decimal>("AppConfigFascadeValueDecimal"), 0);
		}

		[Test]
		public void ShouldGetDoubleTest()
		{
			Assert.AreEqual(AppConfigFascade.Instance.GetAppSetting<Double>("AppConfigFascadeValueDouble"), 0);
		}

		[Test]
		public void ShouldGetEnumTest()
		{
			Assert.AreEqual(AppConfigFascade.Instance.GetAppSetting<CharSet>("AppConfigFascadeValueEnum"), CharSet.Unicode);
		}

		[Test]
		public void ShouldGetGuidTest()
		{
			Assert.AreEqual(AppConfigFascade.Instance.GetAppSetting<Guid>("AppConfigFascadeValueGuid"), Guid.Empty);
		}

		[Test]
		public void ShouldGetInt16Test()
		{
			Assert.AreEqual(AppConfigFascade.Instance.GetAppSetting<Int16>("AppConfigFascadeValueInt16"), 0);
		}

		[Test]
		public void ShouldGetInt32Test()
		{
			Assert.AreEqual(AppConfigFascade.Instance.GetAppSetting<Int32>("AppConfigFascadeValueInt32"), 0);
		}

		[Test]
		public void ShouldGetInt64Test()
		{
			Assert.AreEqual(AppConfigFascade.Instance.GetAppSetting<Int64>("AppConfigFascadeValueInt64"), 0);
		}

		[Test]
		public void ShouldGetSByteTest()
		{
			Assert.AreEqual(AppConfigFascade.Instance.GetAppSetting<SByte>("AppConfigFascadeValueSByte"), 0);
		}

		[Test]
		public void ShouldGetSingleTest()
		{
			Assert.AreEqual(AppConfigFascade.Instance.GetAppSetting<Single>("AppConfigFascadeValueSingle"), 0);
		}

		[Test]
		public void ShouldGetStringTest()
		{
			Assert.AreEqual(AppConfigFascade.Instance.GetAppSetting<string>("AppConfigFascadeValueString"), "foobar");
		}

		[Test]
		public void ShouldGetTimeSpanTest()
		{
			Assert.AreEqual(AppConfigFascade.Instance.GetAppSetting<TimeSpan>("AppConfigFascadeValueTimeSpan"), TimeSpan.Zero);
		}

		[Test]
		public void ShouldGetUInt16Test()
		{
			Assert.AreEqual(AppConfigFascade.Instance.GetAppSetting<UInt16>("AppConfigFascadeValueUInt16"), 0);
		}

		[Test]
		public void ShouldGetUInt32Test()
		{
			Assert.AreEqual(AppConfigFascade.Instance.GetAppSetting<UInt32>("AppConfigFascadeValueUInt32"), 0);
		}

		[Test]
		public void ShouldGetUInt64Test()
		{
			Assert.AreEqual(AppConfigFascade.Instance.GetAppSetting<UInt64>("AppConfigFascadeValueUInt64"), 0L);
		}

		[Test]
		public void ShouldHaveFalseHasAppSettingTest()
		{
			Assert.IsFalse(AppConfigFascade.Instance.HasAppSetting("AppConfigFascadeValueBooleanFalse"));
		}

		[Test]
		public void ShouldHaveFalseHasConnectionStringTest()
		{
			Assert.IsFalse(AppConfigFascade.Instance.HasConnectionString("MyProviderFalse"));
		}

		[Test]
		public void ShouldHaveTrueHasAppSettingTest()
		{
			Assert.IsTrue(AppConfigFascade.Instance.HasAppSetting("AppConfigFascadeValueBoolean"));
		}

		[Test]
		public void ShouldHaveTrueHasConnectionStringTest()
		{
			Assert.IsTrue(AppConfigFascade.Instance.HasConnectionString("MyProvider"));
		}

		#endregion
	}
}