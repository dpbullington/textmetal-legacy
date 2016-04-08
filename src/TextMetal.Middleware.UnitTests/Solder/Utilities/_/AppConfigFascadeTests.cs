/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Runtime.InteropServices;

using NUnit.Framework;

using TextMetal.Middleware.Solder.Utilities;
using TextMetal.Middleware.UnitTests.TestingInfrastructure;

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
		public void ShouldCreateTest__todo_mock_configuration_root()
		{
			Assert.Ignore("TODO: This test case has not been implemented yet.");
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnAppSettingsFileNameCreateTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = null;
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnDataTypeCreateTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = string.Empty;
			mockDataTypeFascade = null;
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnInvalidValueGetTypedBooleanTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<Boolean>("BadAppConfigFascadeValueBoolean");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnInvalidValueGetTypedByteTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<Byte>("BadAppConfigFascadeValueByte");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnInvalidValueGetTypedCharTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<Char>("BadAppConfigFascadeValueChar");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnInvalidValueGetTypedDateTimeTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<DateTime>("BadAppConfigFascadeValueDateTime");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnInvalidValueGetTypedDecimalTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<Decimal>("BadAppConfigFascadeValueDecimal");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnInvalidValueGetTypedDoubleTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<Double>("BadAppConfigFascadeValueDouble");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnInvalidValueGetTypedEnumTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<CharSet>("BadAppConfigFascadeValueEnum");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnInvalidValueGetTypedGuidTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<Guid>("BadAppConfigFascadeValueGuid");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnInvalidValueGetTypedInt16Test()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<Int16>("BadAppConfigFascadeValueInt16");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnInvalidValueGetTypedInt32Test()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<Int32>("BadAppConfigFascadeValueInt32");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnInvalidValueGetTypedInt64Test()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<Int64>("BadAppConfigFascadeValueInt64");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnInvalidValueGetTypedSByteTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<SByte>("BadAppConfigFascadeValueSByte");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnInvalidValueGetTypedSingleTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<Single>("BadAppConfigFascadeValueSingle");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnInvalidValueGetTypedTimeSpanTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<TimeSpan>("BadAppConfigFascadeValueTimeSpan");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnInvalidValueGetTypedUInt16Test()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<UInt16>("BadAppConfigFascadeValueUInt16");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnInvalidValueGetTypedUInt32Test()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<UInt32>("BadAppConfigFascadeValueUInt32");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnInvalidValueGetTypedUInt64Test()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<UInt64>("BadAppConfigFascadeValueUInt64");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnInvalidValueGetUntypedBooleanTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(typeof(Boolean), "BadAppConfigFascadeValueBoolean");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnInvalidValueGetUntypedByteTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(typeof(Byte), "BadAppConfigFascadeValueByte");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnInvalidValueGetUntypedCharTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(typeof(Char), "BadAppConfigFascadeValueChar");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnInvalidValueGetUntypedDateTimeTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(typeof(DateTime), "BadAppConfigFascadeValueDateTime");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnInvalidValueGetUntypedDecimalTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(typeof(Decimal), "BadAppConfigFascadeValueDecimal");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnInvalidValueGetUntypedDoubleTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(typeof(Double), "BadAppConfigFascadeValueDouble");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnInvalidValueGetUntypedEnumTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(typeof(CharSet), "BadAppConfigFascadeValueEnum");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnInvalidValueGetUntypedGuidTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(typeof(Guid), "BadAppConfigFascadeValueGuid");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnInvalidValueGetUntypedInt16Test()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(typeof(Int16), "BadAppConfigFascadeValueInt16");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnInvalidValueGetUntypedInt32Test()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(typeof(Int32), "BadAppConfigFascadeValueInt32");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnInvalidValueGetUntypedInt64Test()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(typeof(Int64), "BadAppConfigFascadeValueInt64");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnInvalidValueGetUntypedSByteTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(typeof(SByte), "BadAppConfigFascadeValueSByte");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnInvalidValueGetUntypedSingleTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(typeof(Single), "BadAppConfigFascadeValueSingle");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnInvalidValueGetUntypedTimeSpanTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(typeof(TimeSpan), "BadAppConfigFascadeValueTimeSpan");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnInvalidValueGetUntypedUInt16Test()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(typeof(UInt16), "BadAppConfigFascadeValueUInt16");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnInvalidValueGetUntypedUInt32Test()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(typeof(UInt32), "BadAppConfigFascadeValueUInt32");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnInvalidValueGetUntypedUInt64Test()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(typeof(UInt64), "BadAppConfigFascadeValueUInt64");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnNonExistKeyGetTypedBooleanTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<Boolean>("NotThereAppConfigFascadeValueBoolean");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnNonExistKeyGetTypedByteTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<Byte>("NotThereAppConfigFascadeValueByte");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnNonExistKeyGetTypedCharTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<Char>("NotThereAppConfigFascadeValueChar");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnNonExistKeyGetTypedDateTimeTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<DateTime>("NotThereAppConfigFascadeValueDateTime");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnNonExistKeyGetTypedDecimalTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<Decimal>("NotThereAppConfigFascadeValueDecimal");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnNonExistKeyGetTypedDoubleTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<Double>("NotThereAppConfigFascadeValueDouble");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnNonExistKeyGetTypedEnumTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<CharSet>("NotThereAppConfigFascadeValueEnum");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnNonExistKeyGetTypedGuidTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<Guid>("NotThereAppConfigFascadeValueGuid");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnNonExistKeyGetTypedInt16Test()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<Int16>("NotThereAppConfigFascadeValueInt16");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnNonExistKeyGetTypedInt32Test()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<Int32>("NotThereAppConfigFascadeValueInt32");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnNonExistKeyGetTypedInt64Test()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<Int64>("NotThereAppConfigFascadeValueInt64");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnNonExistKeyGetTypedSByteTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<SByte>("NotThereAppConfigFascadeValueSByte");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnNonExistKeyGetTypedSingleTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<Single>("NotThereAppConfigFascadeValueSingle");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnNonExistKeyGetTypedTimeSpanTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<TimeSpan>("NotThereAppConfigFascadeValueTimeSpan");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnNonExistKeyGetTypedUInt16Test()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<UInt16>("NotThereAppConfigFascadeValueUInt16");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnNonExistKeyGetTypedUInt32Test()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<UInt32>("NotThereAppConfigFascadeValueUInt32");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnNonExistKeyGetTypedUInt64Test()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<UInt64>("NotThereAppConfigFascadeValueUInt64");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnNonExistKeyGetUntypedBooleanTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(typeof(Boolean), "NotThereAppConfigFascadeValueBoolean");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnNonExistKeyGetUntypedByteTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(typeof(Byte), "NotThereAppConfigFascadeValueByte");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnNonExistKeyGetUntypedCharTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(typeof(Char), "NotThereAppConfigFascadeValueChar");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnNonExistKeyGetUntypedDateTimeTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(typeof(DateTime), "NotThereAppConfigFascadeValueDateTime");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnNonExistKeyGetUntypedDecimalTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(typeof(Decimal), "NotThereAppConfigFascadeValueDecimal");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnNonExistKeyGetUntypedDoubleTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(typeof(Double), "NotThereAppConfigFascadeValueDouble");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnNonExistKeyGetUntypedEnumTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(typeof(CharSet), "NotThereAppConfigFascadeValueEnum");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnNonExistKeyGetUntypedGuidTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(typeof(Guid), "NotThereAppConfigFascadeValueGuid");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnNonExistKeyGetUntypedInt16Test()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(typeof(Int16), "NotThereAppConfigFascadeValueInt16");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnNonExistKeyGetUntypedInt32Test()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(typeof(Int32), "NotThereAppConfigFascadeValueInt32");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnNonExistKeyGetUntypedInt64Test()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(typeof(Int64), "NotThereAppConfigFascadeValueInt64");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnNonExistKeyGetUntypedSByteTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(typeof(SByte), "NotThereAppConfigFascadeValueSByte");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnNonExistKeyGetUntypedSingleTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(typeof(Single), "NotThereAppConfigFascadeValueSingle");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnNonExistKeyGetUntypedTimeSpanTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(typeof(TimeSpan), "NotThereAppConfigFascadeValueTimeSpan");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnNonExistKeyGetUntypedUInt16Test()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(typeof(UInt16), "NotThereAppConfigFascadeValueUInt16");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnNonExistKeyGetUntypedUInt32Test()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(typeof(UInt32), "NotThereAppConfigFascadeValueUInt32");
		}

		[Test]
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnNonExistKeyGetUntypedUInt64Test()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(typeof(UInt64), "NotThereAppConfigFascadeValueUInt64");
		}

		//[Test]
		//[ExpectedException(typeof(ArgumentNullException))]
		//public void ShouldFailOnNullArgsParseCommandLineArgumentsTest()
		//{
		//	AppConfigFascade.Instance.ParseCommandLineArguments(null);
		//}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyGetTypedBooleanTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<Boolean>(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyGetTypedByteTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<Byte>(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyGetTypedCharTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<Char>(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyGetTypedDateTimeTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<DateTime>(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyGetTypedDecimalTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<Decimal>(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyGetTypedDoubleTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<Double>(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyGetTypedEnumTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<CharSet>(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyGetTypedGuidTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<Guid>(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyGetTypedInt16Test()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<Int16>(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyGetTypedInt32Test()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<Int32>(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyGetTypedInt64Test()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<Int64>(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyGetTypedSByteTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<SByte>(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyGetTypedSingleTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<Single>(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyGetTypedTimeSpanTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<TimeSpan>(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyGetTypedUInt16Test()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<UInt16>(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyGetTypedUInt32Test()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<UInt32>(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyGetTypedUInt64Test()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<UInt64>(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyGetUntypedBooleanTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(typeof(Boolean), null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyGetUntypedByteTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(typeof(Byte), null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyGetUntypedCharTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(typeof(Char), null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyGetUntypedDateTimeTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(typeof(DateTime), null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyGetUntypedDecimalTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(typeof(Decimal), null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyGetUntypedDoubleTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(typeof(Double), null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyGetUntypedEnumTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(typeof(CharSet), null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyGetUntypedGuidTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(typeof(Guid), null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyGetUntypedInt16Test()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(typeof(Int16), null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyGetUntypedInt32Test()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(typeof(Int32), null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyGetUntypedInt64Test()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(typeof(Int64), null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyGetUntypedSByteTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(typeof(SByte), null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyGetUntypedSingleTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(typeof(Single), null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyGetUntypedTimeSpanTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(typeof(TimeSpan), null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyGetUntypedUInt16Test()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(typeof(UInt16), null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyGetUntypedUInt32Test()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(typeof(UInt32), null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyGetUntypedUInt64Test()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(typeof(UInt64), null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyHasAppSettingTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.HasAppSetting(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullNameGetTypedAppSettingTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<string>(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullNameGetUntypedAppSettingTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(typeof(string), null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullTypeGetUntypedAppSettingTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting(null, string.Empty);
		}

		//[Test]
		//public void Should__GetArgsParseCommandLineArgumentsTest()
		//{
		//	List<string> args;
		//	IDictionary<string, IList<string>> cmdlnargs;

		//	args = new List<string>();
		//	args.Add("arg0");
		//	args.Add(string.Empty);
		//	args.Add("-");
		//	args.Add("-arg1");
		//	args.Add("-arg2:");
		//	args.Add("-arg3:");
		//	args.Add("-arg4:value4");
		//	args.Add("-arg5:value5");
		//	args.Add("-arg4:value4");
		//	args.Add("arg6:value6");
		//	args.Add("-:value7");
		//	args.Add("-arg8:value8a");
		//	args.Add("-arg8:value8b");
		//	args.Add("-arg8:value8c");
		//	args.Add("-arg8:value8a");

		//	cmdlnargs = AppConfigFascade.Instance.ParseCommandLineArguments(args.ToArray());

		//	Assert.IsNotNull(cmdlnargs);
		//	Assert.AreEqual(3, cmdlnargs.Count);

		//	Assert.AreEqual(1, cmdlnargs["arg4"].Count);
		//	Assert.AreEqual("value4", cmdlnargs["arg4"][0]);

		//	Assert.AreEqual(1, cmdlnargs["arg5"].Count);
		//	Assert.AreEqual("value5", cmdlnargs["arg5"][0]);

		//	Assert.AreEqual(3, cmdlnargs["arg8"].Count);
		//	Assert.AreEqual("value8a", cmdlnargs["arg8"][0]);
		//	Assert.AreEqual("value8b", cmdlnargs["arg8"][1]);
		//	Assert.AreEqual("value8c", cmdlnargs["arg8"][2]);
		//}

		[Test]
		public void ShouldGetTypedBooleanTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			Assert.AreEqual(appConfigFascade.GetAppSetting<Boolean>("AppConfigFascadeValueBoolean"), true);
		}

		[Test]
		public void ShouldGetTypedByteTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			Assert.AreEqual(appConfigFascade.GetAppSetting<Byte>("AppConfigFascadeValueByte"), 0);
		}

		[Test]
		public void ShouldGetTypedCharTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			Assert.AreEqual(appConfigFascade.GetAppSetting<Char>("AppConfigFascadeValueChar"), '0');
		}

		[Test]
		public void ShouldGetTypedDateTimeTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			Assert.AreEqual(appConfigFascade.GetAppSetting<DateTime>("AppConfigFascadeValueDateTime"), new DateTime(2003, 6, 22));
		}

		[Test]
		public void ShouldGetTypedDecimalTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			Assert.AreEqual(appConfigFascade.GetAppSetting<Decimal>("AppConfigFascadeValueDecimal"), 0);
		}

		[Test]
		public void ShouldGetTypedDoubleTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			Assert.AreEqual(appConfigFascade.GetAppSetting<Double>("AppConfigFascadeValueDouble"), 0);
		}

		[Test]
		public void ShouldGetTypedEnumTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			Assert.AreEqual(appConfigFascade.GetAppSetting<CharSet>("AppConfigFascadeValueEnum"), CharSet.Unicode);
		}

		[Test]
		public void ShouldGetTypedGuidTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			Assert.AreEqual(appConfigFascade.GetAppSetting<Guid>("AppConfigFascadeValueGuid"), Guid.Empty);
		}

		[Test]
		public void ShouldGetTypedInt16Test()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			Assert.AreEqual(appConfigFascade.GetAppSetting<Int16>("AppConfigFascadeValueInt16"), 0);
		}

		[Test]
		public void ShouldGetTypedInt32Test()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			Assert.AreEqual(appConfigFascade.GetAppSetting<Int32>("AppConfigFascadeValueInt32"), 0);
		}

		[Test]
		public void ShouldGetTypedInt64Test()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			Assert.AreEqual(appConfigFascade.GetAppSetting<Int64>("AppConfigFascadeValueInt64"), 0);
		}

		[Test]
		public void ShouldGetTypedSByteTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			Assert.AreEqual(appConfigFascade.GetAppSetting<SByte>("AppConfigFascadeValueSByte"), 0);
		}

		[Test]
		public void ShouldGetTypedSingleTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			Assert.AreEqual(appConfigFascade.GetAppSetting<Single>("AppConfigFascadeValueSingle"), 0);
		}

		[Test]
		public void ShouldGetTypedStringTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			Assert.AreEqual(appConfigFascade.GetAppSetting<string>("AppConfigFascadeValueString"), "foobar");
		}

		[Test]
		public void ShouldGetTypedTimeSpanTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			Assert.AreEqual(appConfigFascade.GetAppSetting<TimeSpan>("AppConfigFascadeValueTimeSpan"), TimeSpan.Zero);
		}

		[Test]
		public void ShouldGetTypedUInt16Test()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			Assert.AreEqual(appConfigFascade.GetAppSetting<UInt16>("AppConfigFascadeValueUInt16"), 0);
		}

		[Test]
		public void ShouldGetTypedUInt32Test()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			Assert.AreEqual(appConfigFascade.GetAppSetting<UInt32>("AppConfigFascadeValueUInt32"), 0);
		}

		[Test]
		public void ShouldGetTypedUInt64Test()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			Assert.AreEqual(appConfigFascade.GetAppSetting<UInt64>("AppConfigFascadeValueUInt64"), 0L);
		}

		[Test]
		public void ShouldGetUntypedBooleanTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			Assert.AreEqual(appConfigFascade.GetAppSetting(typeof(Boolean), "AppConfigFascadeValueBoolean"), true);
		}

		[Test]
		public void ShouldGetUntypedByteTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			Assert.AreEqual(appConfigFascade.GetAppSetting(typeof(Byte), "AppConfigFascadeValueByte"), 0);
		}

		[Test]
		public void ShouldGetUntypedCharTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			Assert.AreEqual(appConfigFascade.GetAppSetting(typeof(Char), "AppConfigFascadeValueChar"), '0');
		}

		[Test]
		public void ShouldGetUntypedDateTimeTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			Assert.AreEqual(appConfigFascade.GetAppSetting(typeof(DateTime), "AppConfigFascadeValueDateTime"), new DateTime(2003, 6, 22));
		}

		[Test]
		public void ShouldGetUntypedDecimalTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			Assert.AreEqual(appConfigFascade.GetAppSetting(typeof(Decimal), "AppConfigFascadeValueDecimal"), 0);
		}

		[Test]
		public void ShouldGetUntypedDoubleTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			Assert.AreEqual(appConfigFascade.GetAppSetting(typeof(Double), "AppConfigFascadeValueDouble"), 0);
		}

		[Test]
		public void ShouldGetUntypedEnumTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			Assert.AreEqual(appConfigFascade.GetAppSetting(typeof(CharSet), "AppConfigFascadeValueEnum"), CharSet.Unicode);
		}

		[Test]
		public void ShouldGetUntypedGuidTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			Assert.AreEqual(appConfigFascade.GetAppSetting(typeof(Guid), "AppConfigFascadeValueGuid"), Guid.Empty);
		}

		[Test]
		public void ShouldGetUntypedInt16Test()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			Assert.AreEqual(appConfigFascade.GetAppSetting(typeof(Int16), "AppConfigFascadeValueInt16"), 0);
		}

		[Test]
		public void ShouldGetUntypedInt32Test()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			Assert.AreEqual(appConfigFascade.GetAppSetting(typeof(Int32), "AppConfigFascadeValueInt32"), 0);
		}

		[Test]
		public void ShouldGetUntypedInt64Test()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			Assert.AreEqual(appConfigFascade.GetAppSetting(typeof(Int64), "AppConfigFascadeValueInt64"), 0);
		}

		[Test]
		public void ShouldGetUntypedSByteTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			Assert.AreEqual(appConfigFascade.GetAppSetting(typeof(SByte), "AppConfigFascadeValueSByte"), 0);
		}

		[Test]
		public void ShouldGetUntypedSingleTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			Assert.AreEqual(appConfigFascade.GetAppSetting(typeof(Single), "AppConfigFascadeValueSingle"), 0);
		}

		[Test]
		public void ShouldGetUntypedStringTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			Assert.AreEqual(appConfigFascade.GetAppSetting(typeof(string), "AppConfigFascadeValueString"), "foobar");
		}

		[Test]
		public void ShouldGetUntypedTimeSpanTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			Assert.AreEqual(appConfigFascade.GetAppSetting(typeof(TimeSpan), "AppConfigFascadeValueTimeSpan"), TimeSpan.Zero);
		}

		[Test]
		public void ShouldGetUntypedUInt16Test()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			Assert.AreEqual(appConfigFascade.GetAppSetting(typeof(UInt16), "AppConfigFascadeValueUInt16"), 0);
		}

		[Test]
		public void ShouldGetUntypedUInt32Test()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			Assert.AreEqual(appConfigFascade.GetAppSetting(typeof(UInt32), "AppConfigFascadeValueUInt32"), 0);
		}

		[Test]
		public void ShouldGetUntypedUInt64Test()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			Assert.AreEqual(appConfigFascade.GetAppSetting(typeof(UInt64), "AppConfigFascadeValueUInt64"), 0L);
		}

		[Test]
		public void ShouldHaveFalseHasAppSettingTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			Assert.IsFalse(appConfigFascade.HasAppSetting("AppConfigFascadeValueBooleanFalse"));
		}

		[Test]
		public void ShouldHaveTrueHasAppSettingTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			Assert.IsTrue(appConfigFascade.HasAppSetting("AppConfigFascadeValueBoolean"));
		}

		#endregion
	}
}