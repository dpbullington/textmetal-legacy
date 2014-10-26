/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Reflection;

using NMock2;

using NUnit.Framework;

using TextMetal.Common.Core;
using TextMetal.Common.UnitTests.TestingInfrastructure;

namespace TextMetal.Common.UnitTests.Core._
{
	[TestFixture]
	public class ReflexionTests
	{
		#region Constructors/Destructors

		public ReflexionTests()
		{
		}

		#endregion

		#region Methods/Operators

		//[Test]
		public void ShouldCreateFromSingletonTest()
		{
			IReflexion reflexion;

			reflexion = Reflexion.Instance;

			Assert.IsNotNull(reflexion);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnCreateNullDataType()
		{
			IReflexion reflexion;
			IDataType mockDataType;

			mockDataType = null;

			reflexion = new Reflexion(mockDataType);
		}

		[Test]
		public void ShouldAssociativeOnlyGetLogicalPropertyTypeTest()
		{
			Reflexion reflexion;
			Dictionary<string, object> mockObject;
			string propertyName;
			Type propertyType;
			bool result;

			Mockery mockery;
			IDataType mockDataType;

			mockery = new Mockery();
			mockDataType = mockery.NewMock<IDataType>();

			Expect.On(mockDataType).Method("IsNullOrWhiteSpace").With((object)null).Will(Return.Value(true));
			Expect.On(mockDataType).Method("IsNullOrWhiteSpace").With(string.Empty).Will(Return.Value(true));
			Expect.On(mockDataType).Method("IsNullOrWhiteSpace").With("FirstName").Will(Return.Value(false));

			reflexion = new Reflexion(mockDataType);

			// null, null
			mockObject = null;
			propertyName = null;

			result = reflexion.GetLogicalPropertyType(mockObject, propertyName, out propertyType);

			Assert.IsFalse(result);
			Assert.IsNull(propertyType);

			// null, ""
			mockObject = null;
			propertyName = string.Empty;

			result = reflexion.GetLogicalPropertyType(mockObject, propertyName, out propertyType);

			Assert.IsFalse(result);
			Assert.IsNull(propertyType);

			// null, "PropName"
			mockObject = null;
			propertyName = "FirstName";

			result = reflexion.GetLogicalPropertyType(mockObject, propertyName, out propertyType);

			Assert.IsFalse(result);
			Assert.IsNull(propertyType);

			// !null, null
			mockObject = new Dictionary<string, object>();
			propertyName = null;

			result = reflexion.GetLogicalPropertyType(mockObject, propertyName, out propertyType);

			Assert.IsFalse(result);
			Assert.IsNull(propertyType);

			// !null, ""
			mockObject = new Dictionary<string, object>();
			propertyName = string.Empty;

			result = reflexion.GetLogicalPropertyType(mockObject, propertyName, out propertyType);

			Assert.IsFalse(result);
			Assert.IsNull(propertyType);

			// !null, "PropName"
			mockObject = new Dictionary<string, object>();
			mockObject["FirstName"] = "john";
			propertyName = "FirstName";

			result = reflexion.GetLogicalPropertyType(mockObject, propertyName, out propertyType);

			Assert.IsTrue(result);
			Assert.IsNotNull(propertyType);
			Assert.AreEqual(typeof(string), propertyType);
			
			mockery.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldAssociativeOnlyGetLogicalPropertyValueTest()
		{
			Reflexion reflexion;
			Dictionary<string, object> mockObject;
			string propertyName;
			object propertyValue;
			bool result;

			Mockery mockery;
			IDataType mockDataType;

			mockery = new Mockery();
			mockDataType = mockery.NewMock<IDataType>();

			Expect.On(mockDataType).Method("IsNullOrWhiteSpace").With((object)null).Will(Return.Value(true));
			Expect.On(mockDataType).Method("IsNullOrWhiteSpace").With(string.Empty).Will(Return.Value(true));
			Expect.On(mockDataType).Method("IsNullOrWhiteSpace").With("FirstName").Will(Return.Value(false));

			reflexion = new Reflexion(mockDataType);

			// null, null
			mockObject = null;
			propertyName = null;

			result = reflexion.GetLogicalPropertyValue(mockObject, propertyName, out propertyValue);

			Assert.IsFalse(result);
			Assert.IsNull(propertyValue);

			// null, ""
			mockObject = null;
			propertyName = string.Empty;

			result = reflexion.GetLogicalPropertyValue(mockObject, propertyName, out propertyValue);

			Assert.IsFalse(result);
			Assert.IsNull(propertyValue);

			// null, "PropName"
			mockObject = null;
			propertyName = "FirstName";

			result = reflexion.GetLogicalPropertyValue(mockObject, propertyName, out propertyValue);

			Assert.IsFalse(result);
			Assert.IsNull(propertyValue);

			// !null, null
			mockObject = new Dictionary<string, object>();
			propertyName = null;

			result = reflexion.GetLogicalPropertyValue(mockObject, propertyName, out propertyValue);

			Assert.IsFalse(result);
			Assert.IsNull(propertyValue);

			// !null, ""
			mockObject = new Dictionary<string, object>();
			propertyName = string.Empty;

			result = reflexion.GetLogicalPropertyValue(mockObject, propertyName, out propertyValue);

			Assert.IsFalse(result);
			Assert.IsNull(propertyValue);

			// !null, "PropName"
			mockObject = new Dictionary<string, object>();
			mockObject["FirstName"] = "john";
			propertyName = "FirstName";

			result = reflexion.GetLogicalPropertyValue(mockObject, propertyName, out propertyValue);

			Assert.IsTrue(result);
			Assert.IsNotNull(propertyValue);
			Assert.AreEqual("john", propertyValue);

			mockery.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldAssociativeOnlySetLogicalPropertyValueTest()
		{
			Reflexion reflexion;
			Dictionary<string, object> mockObject;
			string propertyName;
			object propertyValue;
			bool result;

			Mockery mockery;
			IDataType mockDataType;

			mockery = new Mockery();
			mockDataType = mockery.NewMock<IDataType>();

			Expect.On(mockDataType).Method("IsNullOrWhiteSpace").With((object)null).Will(Return.Value(true));
			Expect.On(mockDataType).Method("IsNullOrWhiteSpace").With(string.Empty).Will(Return.Value(true));
			Expect.On(mockDataType).Method("IsNullOrWhiteSpace").With("FirstName").Will(Return.Value(false));

			reflexion = new Reflexion(mockDataType);

			propertyValue = null;

			// null, null
			mockObject = null;
			propertyName = null;

			result = reflexion.SetLogicalPropertyValue(mockObject, propertyName, propertyValue);

			Assert.IsFalse(result);
			Assert.IsNull(propertyValue);

			// null, ""
			mockObject = null;
			propertyName = string.Empty;

			result = reflexion.SetLogicalPropertyValue(mockObject, propertyName, propertyValue);

			Assert.IsFalse(result);
			Assert.IsNull(propertyValue);

			// null, "PropName"
			mockObject = null;
			propertyName = "FirstName";

			result = reflexion.SetLogicalPropertyValue(mockObject, propertyName, propertyValue);

			Assert.IsFalse(result);
			Assert.IsNull(propertyValue);

			// !null, null
			mockObject = new Dictionary<string, object>();
			propertyName = null;

			result = reflexion.SetLogicalPropertyValue(mockObject, propertyName, propertyValue);

			Assert.IsFalse(result);
			Assert.IsNull(propertyValue);

			// !null, ""
			mockObject = new Dictionary<string, object>();
			propertyName = string.Empty;

			result = reflexion.SetLogicalPropertyValue(mockObject, propertyName, propertyValue);

			Assert.IsFalse(result);
			Assert.IsNull(propertyValue);

			// !null, "PropName"
			mockObject = new Dictionary<string, object>();
			mockObject["FirstName"] = null;
			propertyName = "FirstName";
			propertyValue = "john";

			result = reflexion.SetLogicalPropertyValue(mockObject, propertyName, propertyValue);

			Assert.IsTrue(result);
			Assert.IsNotNull(propertyValue);
			Assert.AreEqual("john", propertyValue);

			// !null, "PropName" - !staySoft
			mockObject = new Dictionary<string, object>();
			propertyName = "FirstName";
			propertyValue = null;

			result = reflexion.SetLogicalPropertyValue(mockObject, propertyName, propertyValue, false, false);

			Assert.IsFalse(result);
			Assert.IsNull(propertyValue);

			mockery.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void ShouldFailOnDefinedGetNoAttributesTest()
		{
			Reflexion reflexion;

			Mockery mockery;
			IDataType mockDataType;

			mockery = new Mockery();
			mockDataType = mockery.NewMock<IDataType>();

			reflexion = new Reflexion(mockDataType);

			reflexion.GetZeroAttributes<MockMultipleTestAttibute>(typeof(MockTestAttributedClass));
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void ShouldFailOnMultiDefinedGetAttributeTest()
		{
			Reflexion reflexion;

			Mockery mockery;
			IDataType mockDataType;

			mockery = new Mockery();
			mockDataType = mockery.NewMock<IDataType>();

			reflexion = new Reflexion(mockDataType);
			
			reflexion.GetOneAttribute<MockMultipleTestAttibute>(typeof(MockTestAttributedClass));
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullConversionTypeMakeNonNullableTypeTest()
		{
			Reflexion reflexion;

			Mockery mockery;
			IDataType mockDataType;

			mockery = new Mockery();
			mockDataType = mockery.NewMock<IDataType>();

			reflexion = new Reflexion(mockDataType);
			
			reflexion.MakeNonNullableType(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullConversionTypeMakeNullableTypeTest()
		{
			Reflexion reflexion;

			Mockery mockery;
			IDataType mockDataType;

			mockery = new Mockery();
			mockDataType = mockery.NewMock<IDataType>();

			reflexion = new Reflexion(mockDataType);
			
			reflexion.MakeNullableType(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullTargetGetAttributeICustomAttributeProviderTest()
		{
			Reflexion reflexion;

			Mockery mockery;
			IDataType mockDataType;

			mockery = new Mockery();
			mockDataType = mockery.NewMock<IDataType>();

			reflexion = new Reflexion(mockDataType);
			
			reflexion.GetOneAttribute<MockMultipleTestAttibute>((ICustomAttributeProvider)null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullTargetGetAttributesICustomAttributeProviderTest()
		{
			Reflexion reflexion;

			Mockery mockery;
			IDataType mockDataType;

			mockery = new Mockery();
			mockDataType = mockery.NewMock<IDataType>();

			reflexion = new Reflexion(mockDataType);

			reflexion.GetAllAttributes<MockMultipleTestAttibute>((ICustomAttributeProvider)null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullTargetGetZeroAttributesICustomAttributeProviderTest()
		{
			Reflexion reflexion;

			Mockery mockery;
			IDataType mockDataType;

			mockery = new Mockery();
			mockDataType = mockery.NewMock<IDataType>();

			reflexion = new Reflexion(mockDataType);

			reflexion.GetZeroAttributes<MockMultipleTestAttibute>((ICustomAttributeProvider)null);
		}

		[Test]
		public void ShouldGetAttributeICustomAttributeProviderTest()
		{
			Reflexion reflexion;
			MockSingleTestAttibute sta;

			Mockery mockery;
			IDataType mockDataType;

			mockery = new Mockery();
			mockDataType = mockery.NewMock<IDataType>();

			reflexion = new Reflexion(mockDataType);
			
			sta = reflexion.GetOneAttribute<MockSingleTestAttibute>(typeof(MockTestAttributedClass));

			Assert.IsNotNull(sta);
			Assert.AreEqual(5, sta.Value);
		}

		[Test]
		public void ShouldGetAttributesICustomAttributeProviderTest()
		{
			Reflexion reflexion;
			MockMultipleTestAttibute[] tas;

			Mockery mockery;
			IDataType mockDataType;

			mockery = new Mockery();
			mockDataType = mockery.NewMock<IDataType>();

			reflexion = new Reflexion(mockDataType);

			tas = reflexion.GetAllAttributes<MockMultipleTestAttibute>(typeof(MockTestAttributedClass));

			Assert.IsNotNull(tas);
			Assert.AreEqual(2, tas.Length);
		}

		[Test]
		public void ShouldGetErrors()
		{
			Reflexion reflexion;
			MockException mockException;
			string message;

			Mockery mockery;
			IDataType mockDataType;

			mockery = new Mockery();
			mockDataType = mockery.NewMock<IDataType>();

			reflexion = new Reflexion(mockDataType);

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
				message = reflexion.GetErrors(ex, 0);

				Console.WriteLine(message);
				//Assert.AreEqual("[SwIsHw.Core.UnitTests.TestingInfrastructure.MockException]\r\nouter\r\n[System.Exception]\r\ncollected.outer\r\n[System.Exception]\r\ncollected.inner\r\n[System.Exception]\r\ninner", message);
			}
		}

		[Test]
		public void ShouldGetNoAttributesTest()
		{
			Reflexion reflexion;

			Mockery mockery;
			IDataType mockDataType;

			mockery = new Mockery();
			mockDataType = mockery.NewMock<IDataType>();

			reflexion = new Reflexion(mockDataType);

			reflexion.GetZeroAttributes<AssemblyDescriptionAttribute>(typeof(MockTestAttributedClass));
		}

		[Test]
		public void ShouldGetNullAttributeICustomAttributeProviderTest()
		{
			Reflexion reflexion;
			MockMultipleTestAttibute ta;

			Mockery mockery;
			IDataType mockDataType;

			mockery = new Mockery();
			mockDataType = mockery.NewMock<IDataType>();

			reflexion = new Reflexion(mockDataType);
			
			ta = reflexion.GetOneAttribute<MockMultipleTestAttibute>(typeof(Exception));

			Assert.IsNull(ta);
		}

		[Test]
		public void ShouldGetNullAttributesICustomAttributeProviderTest()
		{
			Reflexion reflexion;
			MockMultipleTestAttibute[] tas;

			Mockery mockery;
			IDataType mockDataType;

			mockery = new Mockery();
			mockDataType = mockery.NewMock<IDataType>();

			reflexion = new Reflexion(mockDataType);

			tas = reflexion.GetAllAttributes<MockMultipleTestAttibute>(typeof(Exception));

			Assert.IsNull(tas);
		}

		[Test]
		public void ShouldMakeNonNullableTypeTest()
		{
			Reflexion reflexion;
			Type conversionType;
			Type nonNullableType;

			Mockery mockery;
			IDataType mockDataType;

			mockery = new Mockery();
			mockDataType = mockery.NewMock<IDataType>();

			reflexion = new Reflexion(mockDataType);

			conversionType = typeof(int);
			nonNullableType = reflexion.MakeNonNullableType(conversionType);
			Assert.AreEqual(typeof(int), nonNullableType);

			conversionType = typeof(int?);
			nonNullableType = reflexion.MakeNonNullableType(conversionType);
			Assert.AreEqual(typeof(int), nonNullableType);

			conversionType = typeof(IDisposable);
			nonNullableType = reflexion.MakeNonNullableType(conversionType);
			Assert.AreEqual(typeof(IDisposable), nonNullableType);
		}

		[Test]
		public void ShouldMakeNullableTypeTest()
		{
			Reflexion reflexion;
			Type conversionType;
			Type nullableType;

			Mockery mockery;
			IDataType mockDataType;

			mockery = new Mockery();
			mockDataType = mockery.NewMock<IDataType>();

			reflexion = new Reflexion(mockDataType);

			conversionType = typeof(int);
			nullableType = reflexion.MakeNullableType(conversionType);
			Assert.AreEqual(typeof(int?), nullableType);

			conversionType = typeof(int?);
			nullableType = reflexion.MakeNullableType(conversionType);
			Assert.AreEqual(typeof(int?), nullableType);

			conversionType = typeof(IDisposable);
			nullableType = reflexion.MakeNullableType(conversionType);
			Assert.AreEqual(typeof(IDisposable), nullableType);
		}

		[Test]
		public void ShouldReflectionOnlyGetLogicalPropertyTypeTest()
		{
			Reflexion reflexion;
			MockObject mockObject;
			string propertyName;
			Type propertyType;
			bool result;

			Mockery mockery;
			IDataType mockDataType;

			mockery = new Mockery();
			mockDataType = mockery.NewMock<IDataType>();

			Expect.On(mockDataType).Method("IsNullOrWhiteSpace").With((object)null).Will(Return.Value(true));
			Expect.On(mockDataType).Method("IsNullOrWhiteSpace").With(string.Empty).Will(Return.Value(true));
			Expect.On(mockDataType).Method("IsNullOrWhiteSpace").With("FirstName").Will(Return.Value(false));

			Expect.On(mockDataType).Method("IsNullOrWhiteSpace").With("NoGetter").Will(Return.Value(false));
			Expect.On(mockDataType).Method("IsNullOrWhiteSpace").With("NoSetter").Will(Return.Value(false));

			reflexion = new Reflexion(mockDataType);

			// null, null
			mockObject = null;
			propertyName = null;

			result = reflexion.GetLogicalPropertyType(mockObject, propertyName, out propertyType);

			Assert.IsFalse(result);
			Assert.IsNull(propertyType);

			// null, ""
			mockObject = null;
			propertyName = string.Empty;

			result = reflexion.GetLogicalPropertyType(mockObject, propertyName, out propertyType);

			Assert.IsFalse(result);
			Assert.IsNull(propertyType);

			// null, "PropName"
			mockObject = null;
			propertyName = "FirstName";

			result = reflexion.GetLogicalPropertyType(mockObject, propertyName, out propertyType);

			Assert.IsFalse(result);
			Assert.IsNull(propertyType);

			// !null, null
			mockObject = new MockObject();
			propertyName = null;

			result = reflexion.GetLogicalPropertyType(mockObject, propertyName, out propertyType);

			Assert.IsFalse(result);
			Assert.IsNull(propertyType);

			// !null, ""
			mockObject = new MockObject();
			propertyName = string.Empty;

			result = reflexion.GetLogicalPropertyType(mockObject, propertyName, out propertyType);

			Assert.IsFalse(result);
			Assert.IsNull(propertyType);

			// !null, "PropName"
			mockObject = new MockObject();
			mockObject.FirstName = "john";
			propertyName = "FirstName";

			result = reflexion.GetLogicalPropertyType(mockObject, propertyName, out propertyType);

			Assert.IsTrue(result);
			Assert.IsNotNull(propertyType);
			Assert.AreEqual(typeof(string), propertyType);

			// !null, "PropName:PropName!!!getter"
			mockObject = new MockObject();
			mockObject.FirstName = "john";
			propertyName = "NoGetter";

			result = reflexion.GetLogicalPropertyType(mockObject, propertyName, out propertyType);

			Assert.IsTrue(result);
			Assert.IsNotNull(propertyType);
			Assert.AreEqual(typeof(object), propertyType);

			// !null, "PropName:PropName!!!setter"
			mockObject = new MockObject();
			mockObject.FirstName = "john";
			propertyName = "NoSetter";

			result = reflexion.GetLogicalPropertyType(mockObject, propertyName, out propertyType);

			Assert.IsTrue(result);
			Assert.IsNotNull(propertyType);
			Assert.AreEqual(typeof(object), propertyType);
		}

		[Test]
		public void ShouldReflectionOnlyGetLogicalPropertyValueTest()
		{
			Reflexion reflexion;
			MockObject mockObject;
			string propertyName;
			object propertyValue;
			bool result;

			Mockery mockery;
			IDataType mockDataType;

			mockery = new Mockery();
			mockDataType = mockery.NewMock<IDataType>();

			Expect.On(mockDataType).Method("IsNullOrWhiteSpace").With((object)null).Will(Return.Value(true));
			Expect.On(mockDataType).Method("IsNullOrWhiteSpace").With(string.Empty).Will(Return.Value(true));
			Expect.On(mockDataType).Method("IsNullOrWhiteSpace").With("FirstName").Will(Return.Value(false));

			Expect.On(mockDataType).Method("IsNullOrWhiteSpace").With("NoGetter").Will(Return.Value(false));
			Expect.On(mockDataType).Method("IsNullOrWhiteSpace").With("NoSetter").Will(Return.Value(false));

			reflexion = new Reflexion(mockDataType);

			// null, null
			mockObject = null;
			propertyName = null;

			result = reflexion.GetLogicalPropertyValue(mockObject, propertyName, out propertyValue);

			Assert.IsFalse(result);
			Assert.IsNull(propertyValue);

			// null, ""
			mockObject = null;
			propertyName = string.Empty;

			result = reflexion.GetLogicalPropertyValue(mockObject, propertyName, out propertyValue);

			Assert.IsFalse(result);
			Assert.IsNull(propertyValue);

			// null, "PropName"
			mockObject = null;
			propertyName = "FirstName";

			result = reflexion.GetLogicalPropertyValue(mockObject, propertyName, out propertyValue);

			Assert.IsFalse(result);
			Assert.IsNull(propertyValue);

			// !null, null
			mockObject = new MockObject();
			propertyName = null;

			result = reflexion.GetLogicalPropertyValue(mockObject, propertyName, out propertyValue);

			Assert.IsFalse(result);
			Assert.IsNull(propertyValue);

			// !null, ""
			mockObject = new MockObject();
			propertyName = string.Empty;

			result = reflexion.GetLogicalPropertyValue(mockObject, propertyName, out propertyValue);

			Assert.IsFalse(result);
			Assert.IsNull(propertyValue);

			// !null, "PropName"
			mockObject = new MockObject();
			mockObject.FirstName = "john";
			propertyName = "FirstName";

			result = reflexion.GetLogicalPropertyValue(mockObject, propertyName, out propertyValue);

			Assert.IsTrue(result);
			Assert.IsNotNull(propertyValue);
			Assert.AreEqual("john", propertyValue);

			// !null, "PropName:PropName!!!getter"
			mockObject = new MockObject();
			mockObject.FirstName = "john";
			propertyName = "NoGetter";

			result = reflexion.GetLogicalPropertyValue(mockObject, propertyName, out propertyValue);

			Assert.IsFalse(result);
			Assert.IsNull(propertyValue);

			// !null, "PropName:PropName!!!setter"
			mockObject = new MockObject();
			mockObject.FirstName = "john";
			propertyName = "NoSetter";

			result = reflexion.GetLogicalPropertyValue(mockObject, propertyName, out propertyValue);

			Assert.IsTrue(result);
			Assert.IsNotNull(propertyValue);
			Assert.AreEqual(1, propertyValue);
		}

		[Test]
		public void ShouldReflectionOnlySetLogicalPropertyValueTest()
		{
			Reflexion reflexion;
			MockObject mockObject;
			string propertyName;
			object propertyValue;
			bool result;

			Mockery mockery;
			IDataType mockDataType;

			mockery = new Mockery();
			mockDataType = mockery.NewMock<IDataType>();

			Expect.On(mockDataType).Method("IsNullOrWhiteSpace").With((object)null).Will(Return.Value(true));
			Expect.On(mockDataType).Method("IsNullOrWhiteSpace").With(string.Empty).Will(Return.Value(true));
			Expect.On(mockDataType).Method("IsNullOrWhiteSpace").With("FirstName").Will(Return.Value(false));
			Expect.On(mockDataType).Method("ChangeType").WithAnyArguments().Will(Return.Value(null));

			Expect.On(mockDataType).Method("IsNullOrWhiteSpace").With("NoGetter").Will(Return.Value(false));
			Expect.On(mockDataType).Method("IsNullOrWhiteSpace").With("NoSetter").Will(Return.Value(false));

			reflexion = new Reflexion(mockDataType);

			propertyValue = null;

			// null, null
			mockObject = null;
			propertyName = null;

			result = reflexion.SetLogicalPropertyValue(mockObject, propertyName, propertyValue);

			Assert.IsFalse(result);
			Assert.IsNull(propertyValue);

			// null, ""
			mockObject = null;
			propertyName = string.Empty;

			result = reflexion.SetLogicalPropertyValue(mockObject, propertyName, propertyValue);

			Assert.IsFalse(result);
			Assert.IsNull(propertyValue);

			// null, "PropName"
			mockObject = null;
			propertyName = "FirstName";

			result = reflexion.SetLogicalPropertyValue(mockObject, propertyName, propertyValue);

			Assert.IsFalse(result);
			Assert.IsNull(propertyValue);

			// !null, null
			mockObject = new MockObject();
			propertyName = null;

			result = reflexion.SetLogicalPropertyValue(mockObject, propertyName, propertyValue);

			Assert.IsFalse(result);
			Assert.IsNull(propertyValue);

			// !null, ""
			mockObject = new MockObject();
			propertyName = string.Empty;

			result = reflexion.SetLogicalPropertyValue(mockObject, propertyName, propertyValue);

			Assert.IsFalse(result);
			Assert.IsNull(propertyValue);

			// !null, "PropName"
			mockObject = new MockObject();
			propertyName = "FirstName";
			propertyValue = "john";

			result = reflexion.SetLogicalPropertyValue(mockObject, propertyName, propertyValue);

			Assert.IsTrue(result);
			Assert.IsNotNull(propertyValue);
			Assert.AreEqual("john", propertyValue);

			// !null, "PropName:PropName!!!getter"
			mockObject = new MockObject();
			propertyName = "NoGetter";
			propertyValue = "john";

			result = reflexion.SetLogicalPropertyValue(mockObject, propertyName, propertyValue);

			Assert.IsTrue(result);
			Assert.IsNotNull(propertyValue);
			Assert.AreEqual("john", propertyValue);

			// !null, "PropName:PropName!!!setter"
			mockObject = new MockObject();
			mockObject.FirstName = "john";
			propertyName = "NoSetter";

			result = reflexion.SetLogicalPropertyValue(mockObject, propertyName, propertyValue);

			Assert.IsFalse(result);
		}

		#endregion
	}
}