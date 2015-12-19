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
		[ExpectedException(typeof(AppConfigException))]
		public void ShouldFailOnInvalidValueGetBooleanTest()
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
		public void ShouldFailOnInvalidValueGetByteTest()
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
		public void ShouldFailOnInvalidValueGetCharTest()
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
		public void ShouldFailOnInvalidValueGetDateTimeTest()
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
		public void ShouldFailOnInvalidValueGetDecimalTest()
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
		public void ShouldFailOnInvalidValueGetDoubleTest()
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
		public void ShouldFailOnInvalidValueGetEnumTest()
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
		public void ShouldFailOnInvalidValueGetGuidTest()
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
		public void ShouldFailOnInvalidValueGetInt16Test()
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
		public void ShouldFailOnInvalidValueGetInt32Test()
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
		public void ShouldFailOnInvalidValueGetInt64Test()
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
		public void ShouldFailOnInvalidValueGetSByteTest()
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
		public void ShouldFailOnInvalidValueGetSingleTest()
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
		public void ShouldFailOnInvalidValueGetTimeSpanTest()
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
		public void ShouldFailOnInvalidValueGetUInt16Test()
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
		public void ShouldFailOnInvalidValueGetUInt32Test()
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
		public void ShouldFailOnInvalidValueGetUInt64Test()
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
		public void ShouldFailOnNonExistKeyGetBooleanTest()
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
		public void ShouldFailOnNonExistKeyGetByteTest()
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
		public void ShouldFailOnNonExistKeyGetCharTest()
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
		public void ShouldFailOnNonExistKeyGetDateTimeTest()
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
		public void ShouldFailOnNonExistKeyGetDecimalTest()
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
		public void ShouldFailOnNonExistKeyGetDoubleTest()
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
		public void ShouldFailOnNonExistKeyGetEnumTest()
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
		public void ShouldFailOnNonExistKeyGetGuidTest()
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
		public void ShouldFailOnNonExistKeyGetInt16Test()
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
		public void ShouldFailOnNonExistKeyGetInt32Test()
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
		public void ShouldFailOnNonExistKeyGetInt64Test()
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
		public void ShouldFailOnNonExistKeyGetSByteTest()
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
		public void ShouldFailOnNonExistKeyGetSingleTest()
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
		public void ShouldFailOnNonExistKeyGetTimeSpanTest()
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
		public void ShouldFailOnNonExistKeyGetUInt16Test()
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
		public void ShouldFailOnNonExistKeyGetUInt32Test()
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
		public void ShouldFailOnNonExistKeyGetUInt64Test()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<UInt64>("NotThereAppConfigFascadeValueUInt64");
		}

		//[Test]
		//[ExpectedException(typeof(ArgumentNullException))]
		//public void ShouldFailOnNullArgsParseCommandLineArgumentsTest()
		//{
		//	AppConfigFascade.Instance.ParseCommandLineArguments(null);
		//}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyGetBooleanTest()
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
		public void ShouldFailOnNullKeyGetByteTest()
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
		public void ShouldFailOnNullKeyGetCharTest()
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
		public void ShouldFailOnNullKeyGetDateTimeTest()
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
		public void ShouldFailOnNullKeyGetDecimalTest()
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
		public void ShouldFailOnNullKeyGetDoubleTest()
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
		public void ShouldFailOnNullKeyGetEnumTest()
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
		public void ShouldFailOnNullKeyGetGuidTest()
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
		public void ShouldFailOnNullKeyGetInt16Test()
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
		public void ShouldFailOnNullKeyGetInt32Test()
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
		public void ShouldFailOnNullKeyGetInt64Test()
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
		public void ShouldFailOnNullKeyGetSByteTest()
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
		public void ShouldFailOnNullKeyGetSingleTest()
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
		public void ShouldFailOnNullKeyGetTimeSpanTest()
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
		public void ShouldFailOnNullKeyGetUInt16Test()
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
		public void ShouldFailOnNullKeyGetUInt32Test()
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
		public void ShouldFailOnNullKeyGetUInt64Test()
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
		public void ShouldFailOnNullNameGetAppSettingTest()
		{
			AppConfigFascade appConfigFascade;
			string mockAppConfigFilePath;
			IDataTypeFascade mockDataTypeFascade;

			mockAppConfigFilePath = "appconfig.json";
			mockDataTypeFascade = new DataTypeFascade();
			appConfigFascade = new AppConfigFascade(mockAppConfigFilePath, mockDataTypeFascade);

			appConfigFascade.GetAppSetting<string>(null);
		}

		//[Test]
		//public void ShouldGetArgsParseCommandLineArgumentsTest()
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
		public void ShouldGetBooleanTest()
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
		public void ShouldGetByteTest()
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
		public void ShouldGetCharTest()
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
		public void ShouldGetDateTimeTest()
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
		public void ShouldGetDecimalTest()
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
		public void ShouldGetDoubleTest()
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
		public void ShouldGetEnumTest()
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
		public void ShouldGetGuidTest()
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
		public void ShouldGetInt16Test()
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
		public void ShouldGetInt32Test()
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
		public void ShouldGetInt64Test()
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
		public void ShouldGetSByteTest()
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
		public void ShouldGetSingleTest()
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
		public void ShouldGetStringTest()
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
		public void ShouldGetTimeSpanTest()
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
		public void ShouldGetUInt16Test()
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
		public void ShouldGetUInt32Test()
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
		public void ShouldGetUInt64Test()
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