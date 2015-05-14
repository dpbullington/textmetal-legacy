/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Reflection;

using NMock;

using NUnit.Framework;

using TextMetal.Middleware.Common.Utilities;
using TextMetal.Middleware.UnitTests.TestingInfrastructure;

namespace TextMetal.Middleware.UnitTests.Common.Fascades.Utilities._
{
	[TestFixture]
	public class ReflectionFascadeTests
	{
		#region Constructors/Destructors

		public ReflectionFascadeTests()
		{
		}

		#endregion

		#region Methods/Operators

		[Test]
		public void ShouldAssociativeOnlyGetLogicalPropertyTypeTest()
		{
			ReflectionFascade reflectionFascade;
			Dictionary<string, object> mockObject;
			string propertyName;
			Type propertyType;
			bool result;

			MockFactory mockFactory;
			IDataTypeFascade mockDataTypeFascade;

			string _unusedString = null;

			mockFactory = new MockFactory();
			mockDataTypeFascade = mockFactory.CreateInstance<IDataTypeFascade>();

			Expect.On(mockDataTypeFascade).One.Method(m => m.IsNullOrWhiteSpace(_unusedString)).With((object)null).Will(Return.Value(true));
			Expect.On(mockDataTypeFascade).One.Method(m => m.IsNullOrWhiteSpace(_unusedString)).With(string.Empty).Will(Return.Value(true));
			Expect.On(mockDataTypeFascade).One.Method(m => m.IsNullOrWhiteSpace(_unusedString)).With("FirstName").Will(Return.Value(false));

			reflectionFascade = new ReflectionFascade(mockDataTypeFascade);

			// null, null
			mockObject = null;
			propertyName = null;

			result = reflectionFascade.GetLogicalPropertyType(mockObject, propertyName, out propertyType);

			Assert.IsFalse(result);
			Assert.IsNull(propertyType);

			// null, ""
			mockObject = null;
			propertyName = string.Empty;

			result = reflectionFascade.GetLogicalPropertyType(mockObject, propertyName, out propertyType);

			Assert.IsFalse(result);
			Assert.IsNull(propertyType);

			// null, "PropName"
			mockObject = null;
			propertyName = "FirstName";

			result = reflectionFascade.GetLogicalPropertyType(mockObject, propertyName, out propertyType);

			Assert.IsFalse(result);
			Assert.IsNull(propertyType);

			// !null, null
			mockObject = new Dictionary<string, object>();
			propertyName = null;

			result = reflectionFascade.GetLogicalPropertyType(mockObject, propertyName, out propertyType);

			Assert.IsFalse(result);
			Assert.IsNull(propertyType);

			// !null, ""
			mockObject = new Dictionary<string, object>();
			propertyName = string.Empty;

			result = reflectionFascade.GetLogicalPropertyType(mockObject, propertyName, out propertyType);

			Assert.IsFalse(result);
			Assert.IsNull(propertyType);

			// !null, "PropName"
			mockObject = new Dictionary<string, object>();
			mockObject["FirstName"] = "john";
			propertyName = "FirstName";

			result = reflectionFascade.GetLogicalPropertyType(mockObject, propertyName, out propertyType);

			Assert.IsTrue(result);
			Assert.IsNotNull(propertyType);
			Assert.AreEqual(typeof(string), propertyType);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldAssociativeOnlyGetLogicalPropertyValueTest()
		{
			ReflectionFascade reflectionFascade;
			Dictionary<string, object> mockObject;
			string propertyName;
			object propertyValue;
			bool result;

			MockFactory mockFactory;
			IDataTypeFascade mockDataTypeFascade;

			string _unusedString = null;

			mockFactory = new MockFactory();
			mockDataTypeFascade = mockFactory.CreateInstance<IDataTypeFascade>();

			Expect.On(mockDataTypeFascade).One.Method(m => m.IsNullOrWhiteSpace(_unusedString)).With((object)null).Will(Return.Value(true));
			Expect.On(mockDataTypeFascade).One.Method(m => m.IsNullOrWhiteSpace(_unusedString)).With(string.Empty).Will(Return.Value(true));
			Expect.On(mockDataTypeFascade).One.Method(m => m.IsNullOrWhiteSpace(_unusedString)).With("FirstName").Will(Return.Value(false));

			reflectionFascade = new ReflectionFascade(mockDataTypeFascade);

			// null, null
			mockObject = null;
			propertyName = null;

			result = reflectionFascade.GetLogicalPropertyValue(mockObject, propertyName, out propertyValue);

			Assert.IsFalse(result);
			Assert.IsNull(propertyValue);

			// null, ""
			mockObject = null;
			propertyName = string.Empty;

			result = reflectionFascade.GetLogicalPropertyValue(mockObject, propertyName, out propertyValue);

			Assert.IsFalse(result);
			Assert.IsNull(propertyValue);

			// null, "PropName"
			mockObject = null;
			propertyName = "FirstName";

			result = reflectionFascade.GetLogicalPropertyValue(mockObject, propertyName, out propertyValue);

			Assert.IsFalse(result);
			Assert.IsNull(propertyValue);

			// !null, null
			mockObject = new Dictionary<string, object>();
			propertyName = null;

			result = reflectionFascade.GetLogicalPropertyValue(mockObject, propertyName, out propertyValue);

			Assert.IsFalse(result);
			Assert.IsNull(propertyValue);

			// !null, ""
			mockObject = new Dictionary<string, object>();
			propertyName = string.Empty;

			result = reflectionFascade.GetLogicalPropertyValue(mockObject, propertyName, out propertyValue);

			Assert.IsFalse(result);
			Assert.IsNull(propertyValue);

			// !null, "PropName"
			mockObject = new Dictionary<string, object>();
			mockObject["FirstName"] = "john";
			propertyName = "FirstName";

			result = reflectionFascade.GetLogicalPropertyValue(mockObject, propertyName, out propertyValue);

			Assert.IsTrue(result);
			Assert.IsNotNull(propertyValue);
			Assert.AreEqual("john", propertyValue);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldAssociativeOnlySetLogicalPropertyValueTest()
		{
			ReflectionFascade reflectionFascade;
			Dictionary<string, object> mockObject;
			string propertyName;
			object propertyValue;
			bool result;

			MockFactory mockFactory;
			IDataTypeFascade mockDataTypeFascade;

			string _unusedString = null;

			mockFactory = new MockFactory();
			mockDataTypeFascade = mockFactory.CreateInstance<IDataTypeFascade>();

			Expect.On(mockDataTypeFascade).One.Method(m => m.IsNullOrWhiteSpace(_unusedString)).With((object)null).Will(Return.Value(true));
			Expect.On(mockDataTypeFascade).One.Method(m => m.IsNullOrWhiteSpace(_unusedString)).With(string.Empty).Will(Return.Value(true));
			Expect.On(mockDataTypeFascade).Exactly(2).Method(m => m.IsNullOrWhiteSpace(_unusedString)).With("FirstName").Will(Return.Value(false));

			reflectionFascade = new ReflectionFascade(mockDataTypeFascade);

			propertyValue = null;

			// null, null
			mockObject = null;
			propertyName = null;

			result = reflectionFascade.SetLogicalPropertyValue(mockObject, propertyName, propertyValue);

			Assert.IsFalse(result);
			Assert.IsNull(propertyValue);

			// null, ""
			mockObject = null;
			propertyName = string.Empty;

			result = reflectionFascade.SetLogicalPropertyValue(mockObject, propertyName, propertyValue);

			Assert.IsFalse(result);
			Assert.IsNull(propertyValue);

			// null, "PropName"
			mockObject = null;
			propertyName = "FirstName";

			result = reflectionFascade.SetLogicalPropertyValue(mockObject, propertyName, propertyValue);

			Assert.IsFalse(result);
			Assert.IsNull(propertyValue);

			// !null, null
			mockObject = new Dictionary<string, object>();
			propertyName = null;

			result = reflectionFascade.SetLogicalPropertyValue(mockObject, propertyName, propertyValue);

			Assert.IsFalse(result);
			Assert.IsNull(propertyValue);

			// !null, ""
			mockObject = new Dictionary<string, object>();
			propertyName = string.Empty;

			result = reflectionFascade.SetLogicalPropertyValue(mockObject, propertyName, propertyValue);

			Assert.IsFalse(result);
			Assert.IsNull(propertyValue);

			// !null, "PropName"
			mockObject = new Dictionary<string, object>();
			mockObject["FirstName"] = null;
			propertyName = "FirstName";
			propertyValue = "john";

			result = reflectionFascade.SetLogicalPropertyValue(mockObject, propertyName, propertyValue);

			Assert.IsTrue(result);
			Assert.IsNotNull(propertyValue);
			Assert.AreEqual("john", propertyValue);

			// !null, "PropName" - !staySoft
			mockObject = new Dictionary<string, object>();
			propertyName = "FirstName";
			propertyValue = null;

			result = reflectionFascade.SetLogicalPropertyValue(mockObject, propertyName, propertyValue, false, false);

			Assert.IsFalse(result);
			Assert.IsNull(propertyValue);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		public void ShouldCreateFromSingletonTest()
		{
			IReflectionFascade reflectionFascade;

			reflectionFascade = ReflectionFascade.Instance;

			Assert.IsNotNull(reflectionFascade);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnCreateNullDataType()
		{
			IReflectionFascade reflectionFascade;
			IDataTypeFascade mockDataTypeFascade;

			mockDataTypeFascade = null;

			reflectionFascade = new ReflectionFascade(mockDataTypeFascade);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void ShouldFailOnDefinedGetNoAttributesTest()
		{
			ReflectionFascade reflectionFascade;

			MockFactory mockFactory;
			IDataTypeFascade mockDataTypeFascade;

			mockFactory = new MockFactory();
			mockDataTypeFascade = mockFactory.CreateInstance<IDataTypeFascade>();

			reflectionFascade = new ReflectionFascade(mockDataTypeFascade);

			reflectionFascade.GetZeroAttributes<MockMultipleTestAttibute>(typeof(MockTestAttributedClass));
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void ShouldFailOnMultiDefinedGetAttributeTest()
		{
			ReflectionFascade reflectionFascade;

			MockFactory mockFactory;
			IDataTypeFascade mockDataTypeFascade;

			mockFactory = new MockFactory();
			mockDataTypeFascade = mockFactory.CreateInstance<IDataTypeFascade>();

			reflectionFascade = new ReflectionFascade(mockDataTypeFascade);

			reflectionFascade.GetOneAttribute<MockMultipleTestAttibute>(typeof(MockTestAttributedClass));
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullConversionTypeMakeNonNullableTypeTest()
		{
			ReflectionFascade reflectionFascade;

			MockFactory mockFactory;
			IDataTypeFascade mockDataTypeFascade;

			mockFactory = new MockFactory();
			mockDataTypeFascade = mockFactory.CreateInstance<IDataTypeFascade>();

			reflectionFascade = new ReflectionFascade(mockDataTypeFascade);

			reflectionFascade.MakeNonNullableType(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullConversionTypeMakeNullableTypeTest()
		{
			ReflectionFascade reflectionFascade;

			MockFactory mockFactory;
			IDataTypeFascade mockDataTypeFascade;

			mockFactory = new MockFactory();
			mockDataTypeFascade = mockFactory.CreateInstance<IDataTypeFascade>();

			reflectionFascade = new ReflectionFascade(mockDataTypeFascade);

			reflectionFascade.MakeNullableType(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullTargetGetAttributeICustomAttributeProviderTest()
		{
			ReflectionFascade reflectionFascade;

			MockFactory mockFactory;
			IDataTypeFascade mockDataTypeFascade;

			mockFactory = new MockFactory();
			mockDataTypeFascade = mockFactory.CreateInstance<IDataTypeFascade>();

			reflectionFascade = new ReflectionFascade(mockDataTypeFascade);

			reflectionFascade.GetOneAttribute<MockMultipleTestAttibute>((ICustomAttributeProvider)null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullTargetGetAttributesICustomAttributeProviderTest()
		{
			ReflectionFascade reflectionFascade;

			MockFactory mockFactory;
			IDataTypeFascade mockDataTypeFascade;

			mockFactory = new MockFactory();
			mockDataTypeFascade = mockFactory.CreateInstance<IDataTypeFascade>();

			reflectionFascade = new ReflectionFascade(mockDataTypeFascade);

			reflectionFascade.GetAllAttributes<MockMultipleTestAttibute>((ICustomAttributeProvider)null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullTargetGetZeroAttributesICustomAttributeProviderTest()
		{
			ReflectionFascade reflectionFascade;

			MockFactory mockFactory;
			IDataTypeFascade mockDataTypeFascade;

			mockFactory = new MockFactory();
			mockDataTypeFascade = mockFactory.CreateInstance<IDataTypeFascade>();

			reflectionFascade = new ReflectionFascade(mockDataTypeFascade);

			reflectionFascade.GetZeroAttributes<MockMultipleTestAttibute>((ICustomAttributeProvider)null);
		}

		[Test]
		public void ShouldGetAttributeICustomAttributeProviderTest()
		{
			ReflectionFascade reflectionFascade;
			MockSingleTestAttibute sta;

			MockFactory mockFactory;
			IDataTypeFascade mockDataTypeFascade;

			mockFactory = new MockFactory();
			mockDataTypeFascade = mockFactory.CreateInstance<IDataTypeFascade>();

			reflectionFascade = new ReflectionFascade(mockDataTypeFascade);

			sta = reflectionFascade.GetOneAttribute<MockSingleTestAttibute>(typeof(MockTestAttributedClass));

			Assert.IsNotNull(sta);
			Assert.AreEqual(5, sta.Value);
		}

		[Test]
		public void ShouldGetAttributesICustomAttributeProviderTest()
		{
			ReflectionFascade reflectionFascade;
			MockMultipleTestAttibute[] tas;

			MockFactory mockFactory;
			IDataTypeFascade mockDataTypeFascade;

			mockFactory = new MockFactory();
			mockDataTypeFascade = mockFactory.CreateInstance<IDataTypeFascade>();

			reflectionFascade = new ReflectionFascade(mockDataTypeFascade);

			tas = reflectionFascade.GetAllAttributes<MockMultipleTestAttibute>(typeof(MockTestAttributedClass));

			Assert.IsNotNull(tas);
			Assert.AreEqual(2, tas.Length);
		}

		[Test]
		public void ShouldGetErrors()
		{
			ReflectionFascade reflectionFascade;
			MockException mockException;
			string message;

			MockFactory mockFactory;
			IDataTypeFascade mockDataTypeFascade;

			mockFactory = new MockFactory();
			mockDataTypeFascade = mockFactory.CreateInstance<IDataTypeFascade>();

			reflectionFascade = new ReflectionFascade(mockDataTypeFascade);

			try
			{
				try
				{
					throw new InvalidOperationException("ioe.collected.outer", new DivideByZeroException("dbze.collected.inner"));
				}
				catch (Exception ex)
				{
					mockException = new MockException("me.outer", new BadImageFormatException("bie.inner"));
					mockException.CollectedExceptions.Add(ex);

					throw mockException;
				}
			}
			catch (Exception ex)
			{
				message = reflectionFascade.GetErrors(ex, 0);

				Console.WriteLine(message);
				//Assert.AreEqual("[SwIsHw.Core.UnitTests.TestingInfrastructure.MockException]\r\nouter\r\n[System.Exception]\r\ncollected.outer\r\n[System.Exception]\r\ncollected.inner\r\n[System.Exception]\r\ninner", message);
			}
		}

		[Test]
		public void ShouldGetNoAttributesTest()
		{
			ReflectionFascade reflectionFascade;

			MockFactory mockFactory;
			IDataTypeFascade mockDataTypeFascade;

			mockFactory = new MockFactory();
			mockDataTypeFascade = mockFactory.CreateInstance<IDataTypeFascade>();

			reflectionFascade = new ReflectionFascade(mockDataTypeFascade);

			reflectionFascade.GetZeroAttributes<AssemblyDescriptionAttribute>(typeof(MockTestAttributedClass));
		}

		[Test]
		public void ShouldGetNullAttributeICustomAttributeProviderTest()
		{
			ReflectionFascade reflectionFascade;
			MockMultipleTestAttibute ta;

			MockFactory mockFactory;
			IDataTypeFascade mockDataTypeFascade;

			mockFactory = new MockFactory();
			mockDataTypeFascade = mockFactory.CreateInstance<IDataTypeFascade>();

			reflectionFascade = new ReflectionFascade(mockDataTypeFascade);

			ta = reflectionFascade.GetOneAttribute<MockMultipleTestAttibute>(typeof(Exception));

			Assert.IsNull(ta);
		}

		[Test]
		public void ShouldGetNullAttributesICustomAttributeProviderTest()
		{
			ReflectionFascade reflectionFascade;
			MockMultipleTestAttibute[] tas;

			MockFactory mockFactory;
			IDataTypeFascade mockDataTypeFascade;

			mockFactory = new MockFactory();
			mockDataTypeFascade = mockFactory.CreateInstance<IDataTypeFascade>();

			reflectionFascade = new ReflectionFascade(mockDataTypeFascade);

			tas = reflectionFascade.GetAllAttributes<MockMultipleTestAttibute>(typeof(Exception));

			Assert.IsNull(tas);
		}

		[Test]
		public void ShouldMakeNonNullableTypeTest()
		{
			ReflectionFascade reflectionFascade;
			Type conversionType;
			Type nonNullableType;

			MockFactory mockFactory;
			IDataTypeFascade mockDataTypeFascade;

			mockFactory = new MockFactory();
			mockDataTypeFascade = mockFactory.CreateInstance<IDataTypeFascade>();

			reflectionFascade = new ReflectionFascade(mockDataTypeFascade);

			conversionType = typeof(int);
			nonNullableType = reflectionFascade.MakeNonNullableType(conversionType);
			Assert.AreEqual(typeof(int), nonNullableType);

			conversionType = typeof(int?);
			nonNullableType = reflectionFascade.MakeNonNullableType(conversionType);
			Assert.AreEqual(typeof(int), nonNullableType);

			conversionType = typeof(IDisposable);
			nonNullableType = reflectionFascade.MakeNonNullableType(conversionType);
			Assert.AreEqual(typeof(IDisposable), nonNullableType);
		}

		[Test]
		public void ShouldMakeNullableTypeTest()
		{
			ReflectionFascade reflectionFascade;
			Type conversionType;
			Type nullableType;

			MockFactory mockFactory;
			IDataTypeFascade mockDataTypeFascade;

			mockFactory = new MockFactory();
			mockDataTypeFascade = mockFactory.CreateInstance<IDataTypeFascade>();

			reflectionFascade = new ReflectionFascade(mockDataTypeFascade);

			conversionType = typeof(int);
			nullableType = reflectionFascade.MakeNullableType(conversionType);
			Assert.AreEqual(typeof(int?), nullableType);

			conversionType = typeof(int?);
			nullableType = reflectionFascade.MakeNullableType(conversionType);
			Assert.AreEqual(typeof(int?), nullableType);

			conversionType = typeof(IDisposable);
			nullableType = reflectionFascade.MakeNullableType(conversionType);
			Assert.AreEqual(typeof(IDisposable), nullableType);
		}

		[Test]
		public void ShouldReflectionOnlyGetLogicalPropertyTypeTest()
		{
			ReflectionFascade reflectionFascade;
			MockObject mockObject;
			string propertyName;
			Type propertyType;
			bool result;

			MockFactory mockFactory;
			IDataTypeFascade mockDataTypeFascade;

			string _unusedString = null;

			mockFactory = new MockFactory();
			mockDataTypeFascade = mockFactory.CreateInstance<IDataTypeFascade>();

			Expect.On(mockDataTypeFascade).One.Method(m => m.IsNullOrWhiteSpace(_unusedString)).With((object)null).Will(Return.Value(true));
			Expect.On(mockDataTypeFascade).One.Method(m => m.IsNullOrWhiteSpace(_unusedString)).With(string.Empty).Will(Return.Value(true));
			Expect.On(mockDataTypeFascade).One.Method(m => m.IsNullOrWhiteSpace(_unusedString)).With("FirstName").Will(Return.Value(false));

			Expect.On(mockDataTypeFascade).One.Method(m => m.IsNullOrWhiteSpace(_unusedString)).With("NoGetter").Will(Return.Value(false));
			Expect.On(mockDataTypeFascade).One.Method(m => m.IsNullOrWhiteSpace(_unusedString)).With("NoSetter").Will(Return.Value(false));

			reflectionFascade = new ReflectionFascade(mockDataTypeFascade);

			// null, null
			mockObject = null;
			propertyName = null;

			result = reflectionFascade.GetLogicalPropertyType(mockObject, propertyName, out propertyType);

			Assert.IsFalse(result);
			Assert.IsNull(propertyType);

			// null, ""
			mockObject = null;
			propertyName = string.Empty;

			result = reflectionFascade.GetLogicalPropertyType(mockObject, propertyName, out propertyType);

			Assert.IsFalse(result);
			Assert.IsNull(propertyType);

			// null, "PropName"
			mockObject = null;
			propertyName = "FirstName";

			result = reflectionFascade.GetLogicalPropertyType(mockObject, propertyName, out propertyType);

			Assert.IsFalse(result);
			Assert.IsNull(propertyType);

			// !null, null
			mockObject = new MockObject();
			propertyName = null;

			result = reflectionFascade.GetLogicalPropertyType(mockObject, propertyName, out propertyType);

			Assert.IsFalse(result);
			Assert.IsNull(propertyType);

			// !null, ""
			mockObject = new MockObject();
			propertyName = string.Empty;

			result = reflectionFascade.GetLogicalPropertyType(mockObject, propertyName, out propertyType);

			Assert.IsFalse(result);
			Assert.IsNull(propertyType);

			// !null, "PropName"
			mockObject = new MockObject();
			mockObject.FirstName = "john";
			propertyName = "FirstName";

			result = reflectionFascade.GetLogicalPropertyType(mockObject, propertyName, out propertyType);

			Assert.IsTrue(result);
			Assert.IsNotNull(propertyType);
			Assert.AreEqual(typeof(string), propertyType);

			// !null, "PropName:PropName!!!getter"
			mockObject = new MockObject();
			mockObject.FirstName = "john";
			propertyName = "NoGetter";

			result = reflectionFascade.GetLogicalPropertyType(mockObject, propertyName, out propertyType);

			Assert.IsTrue(result);
			Assert.IsNotNull(propertyType);
			Assert.AreEqual(typeof(object), propertyType);

			// !null, "PropName:PropName!!!setter"
			mockObject = new MockObject();
			mockObject.FirstName = "john";
			propertyName = "NoSetter";

			result = reflectionFascade.GetLogicalPropertyType(mockObject, propertyName, out propertyType);

			Assert.IsTrue(result);
			Assert.IsNotNull(propertyType);
			Assert.AreEqual(typeof(object), propertyType);
		}

		[Test]
		public void ShouldReflectionOnlyGetLogicalPropertyValueTest()
		{
			ReflectionFascade reflectionFascade;
			MockObject mockObject;
			string propertyName;
			object propertyValue;
			bool result;

			MockFactory mockFactory;
			IDataTypeFascade mockDataTypeFascade;

			string _unusedString = null;

			mockFactory = new MockFactory();
			mockDataTypeFascade = mockFactory.CreateInstance<IDataTypeFascade>();

			Expect.On(mockDataTypeFascade).One.Method(m => m.IsNullOrWhiteSpace(_unusedString)).With((object)null).Will(Return.Value(true));
			Expect.On(mockDataTypeFascade).One.Method(m => m.IsNullOrWhiteSpace(_unusedString)).With(string.Empty).Will(Return.Value(true));
			Expect.On(mockDataTypeFascade).One.Method(m => m.IsNullOrWhiteSpace(_unusedString)).With("FirstName").Will(Return.Value(false));

			Expect.On(mockDataTypeFascade).One.Method(m => m.IsNullOrWhiteSpace(_unusedString)).With("NoGetter").Will(Return.Value(false));
			Expect.On(mockDataTypeFascade).One.Method(m => m.IsNullOrWhiteSpace(_unusedString)).With("NoSetter").Will(Return.Value(false));

			reflectionFascade = new ReflectionFascade(mockDataTypeFascade);

			// null, null
			mockObject = null;
			propertyName = null;

			result = reflectionFascade.GetLogicalPropertyValue(mockObject, propertyName, out propertyValue);

			Assert.IsFalse(result);
			Assert.IsNull(propertyValue);

			// null, ""
			mockObject = null;
			propertyName = string.Empty;

			result = reflectionFascade.GetLogicalPropertyValue(mockObject, propertyName, out propertyValue);

			Assert.IsFalse(result);
			Assert.IsNull(propertyValue);

			// null, "PropName"
			mockObject = null;
			propertyName = "FirstName";

			result = reflectionFascade.GetLogicalPropertyValue(mockObject, propertyName, out propertyValue);

			Assert.IsFalse(result);
			Assert.IsNull(propertyValue);

			// !null, null
			mockObject = new MockObject();
			propertyName = null;

			result = reflectionFascade.GetLogicalPropertyValue(mockObject, propertyName, out propertyValue);

			Assert.IsFalse(result);
			Assert.IsNull(propertyValue);

			// !null, ""
			mockObject = new MockObject();
			propertyName = string.Empty;

			result = reflectionFascade.GetLogicalPropertyValue(mockObject, propertyName, out propertyValue);

			Assert.IsFalse(result);
			Assert.IsNull(propertyValue);

			// !null, "PropName"
			mockObject = new MockObject();
			mockObject.FirstName = "john";
			propertyName = "FirstName";

			result = reflectionFascade.GetLogicalPropertyValue(mockObject, propertyName, out propertyValue);

			Assert.IsTrue(result);
			Assert.IsNotNull(propertyValue);
			Assert.AreEqual("john", propertyValue);

			// !null, "PropName:PropName!!!getter"
			mockObject = new MockObject();
			mockObject.FirstName = "john";
			propertyName = "NoGetter";

			result = reflectionFascade.GetLogicalPropertyValue(mockObject, propertyName, out propertyValue);

			Assert.IsFalse(result);
			Assert.IsNull(propertyValue);

			// !null, "PropName:PropName!!!setter"
			mockObject = new MockObject();
			mockObject.FirstName = "john";
			propertyName = "NoSetter";

			result = reflectionFascade.GetLogicalPropertyValue(mockObject, propertyName, out propertyValue);

			Assert.IsTrue(result);
			Assert.IsNotNull(propertyValue);
			Assert.AreEqual(1, propertyValue);
		}

		[Test]
		public void ShouldReflectionOnlySetLogicalPropertyValueTest()
		{
			ReflectionFascade reflectionFascade;
			MockObject mockObject;
			string propertyName;
			object propertyValue;
			bool result;

			MockFactory mockFactory;
			IDataTypeFascade mockDataTypeFascade;

			string _unusedString = null;
			object _unusedObject = null;
			Type _unusedType = null;

			mockFactory = new MockFactory();
			mockDataTypeFascade = mockFactory.CreateInstance<IDataTypeFascade>();

			Expect.On(mockDataTypeFascade).One.Method(m => m.IsNullOrWhiteSpace(_unusedString)).With((object)null).Will(Return.Value(true));
			Expect.On(mockDataTypeFascade).One.Method(m => m.IsNullOrWhiteSpace(_unusedString)).With(string.Empty).Will(Return.Value(true));
			Expect.On(mockDataTypeFascade).One.Method(m => m.IsNullOrWhiteSpace(_unusedString)).With("FirstName").Will(Return.Value(false));
			Expect.On(mockDataTypeFascade).Exactly(2).Method(m => m.ChangeType(_unusedObject, _unusedType)).WithAnyArguments().Will(Return.Value(null));

			Expect.On(mockDataTypeFascade).One.Method(m => m.IsNullOrWhiteSpace(_unusedString)).With("NoGetter").Will(Return.Value(false));
			Expect.On(mockDataTypeFascade).One.Method(m => m.IsNullOrWhiteSpace(_unusedString)).With("NoSetter").Will(Return.Value(false));

			reflectionFascade = new ReflectionFascade(mockDataTypeFascade);

			propertyValue = null;

			// null, null
			mockObject = null;
			propertyName = null;

			result = reflectionFascade.SetLogicalPropertyValue(mockObject, propertyName, propertyValue);

			Assert.IsFalse(result);
			Assert.IsNull(propertyValue);

			// null, ""
			mockObject = null;
			propertyName = string.Empty;

			result = reflectionFascade.SetLogicalPropertyValue(mockObject, propertyName, propertyValue);

			Assert.IsFalse(result);
			Assert.IsNull(propertyValue);

			// null, "PropName"
			mockObject = null;
			propertyName = "FirstName";

			result = reflectionFascade.SetLogicalPropertyValue(mockObject, propertyName, propertyValue);

			Assert.IsFalse(result);
			Assert.IsNull(propertyValue);

			// !null, null
			mockObject = new MockObject();
			propertyName = null;

			result = reflectionFascade.SetLogicalPropertyValue(mockObject, propertyName, propertyValue);

			Assert.IsFalse(result);
			Assert.IsNull(propertyValue);

			// !null, ""
			mockObject = new MockObject();
			propertyName = string.Empty;

			result = reflectionFascade.SetLogicalPropertyValue(mockObject, propertyName, propertyValue);

			Assert.IsFalse(result);
			Assert.IsNull(propertyValue);

			// !null, "PropName"
			mockObject = new MockObject();
			propertyName = "FirstName";
			propertyValue = "john";

			result = reflectionFascade.SetLogicalPropertyValue(mockObject, propertyName, propertyValue);

			Assert.IsTrue(result);
			Assert.IsNotNull(propertyValue);
			Assert.AreEqual("john", propertyValue);

			// !null, "PropName:PropName!!!getter"
			mockObject = new MockObject();
			propertyName = "NoGetter";
			propertyValue = "john";

			result = reflectionFascade.SetLogicalPropertyValue(mockObject, propertyName, propertyValue);

			Assert.IsTrue(result);
			Assert.IsNotNull(propertyValue);
			Assert.AreEqual("john", propertyValue);

			// !null, "PropName:PropName!!!setter"
			mockObject = new MockObject();
			mockObject.FirstName = "john";
			propertyName = "NoSetter";

			result = reflectionFascade.SetLogicalPropertyValue(mockObject, propertyName, propertyValue);

			Assert.IsFalse(result);
		}

		#endregion
	}
}