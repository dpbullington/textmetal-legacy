/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using NMock2;

using NUnit.Framework;

using TextMetal.Common.Data.TypeMap;
using TextMetal.Common.UnitTests.TestingInfrastructure;

namespace TextMetal.Common.UnitTests.Data.TypeMap
{
	[TestFixture]
	public class PlainObjectTests
	{
		#region Constructors/Destructors

		public PlainObjectTests()
		{
		}

		#endregion

		#region Methods/Operators

		[Test]
		public void ShouldCreateTest()
		{
			MockPlainObject obj;

			obj = new MockPlainObject();

			Assert.IsTrue(obj.IsNew);
			obj.NameId = 1;
			Assert.IsFalse(obj.IsNew);

			obj.IsNew = true;
			Assert.IsTrue(obj.IsNew);

			Assert.AreEqual(ObjectState.Consistent, obj.ObjectState);
			obj.ObjectState = ObjectState.Modified;
			Assert.AreEqual(ObjectState.Modified, obj.ObjectState);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailNullOnPersistentObjectDataOperationDetermineBeforeTest()
		{
			Mockery mockery;
			IPlainObject mockPersistentObject;

			mockery = new Mockery();
			mockPersistentObject = null;

			PlainObject.DetermineBefore(mockPersistentObject);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailNullOnPersistentObjectDetermineAfterTest()
		{
			Mockery mockery;
			IPlainObject mockPersistentObject;

			mockery = new Mockery();
			mockPersistentObject = null;

			PlainObject.DetermineAfter(mockPersistentObject);
		}

		[Test]
		public void ShouldGetConsistentObjectStateDetermineAfterTest()
		{
			Mockery mockery;
			IPlainObject mockPersistentObject;
			ObjectState result;

			mockery = new Mockery();
			mockPersistentObject = mockery.NewMock<IPlainObject>();

			Expect.AtLeastOnce.On(mockPersistentObject).GetProperty("ObjectState").Will(Return.Value(ObjectState.Modified));

			result = PlainObject.DetermineAfter(mockPersistentObject);

			Assert.AreEqual(ObjectState.Consistent, result);

			mockery.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldGetConsistentOriginalObjectStateDetermineAfterTest()
		{
			Mockery mockery;
			IPlainObject mockPersistentObject;
			ObjectState result;

			mockery = new Mockery();
			mockPersistentObject = mockery.NewMock<IPlainObject>();

			Expect.AtLeastOnce.On(mockPersistentObject).GetProperty("ObjectState").Will(Return.Value(ObjectState.Consistent));

			result = PlainObject.DetermineAfter(mockPersistentObject);

			Assert.AreEqual(ObjectState.Consistent, result);

			mockery.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldGetDeleteDataOperationDetermineBeforeTest()
		{
			Mockery mockery;
			IPlainObject mockPersistentObject;
			DataOperation result;

			mockery = new Mockery();
			mockPersistentObject = mockery.NewMock<IPlainObject>();

			Expect.AtLeastOnce.On(mockPersistentObject).GetProperty("ObjectState").Will(Return.Value(ObjectState.Removed));
			Expect.AtLeastOnce.On(mockPersistentObject).GetProperty("IsNew").Will(Return.Value(false));

			result = PlainObject.DetermineBefore(mockPersistentObject);

			Assert.AreEqual(DataOperation.Delete, result);

			mockery.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldGetErrorDataOperationDetermineBeforeTest()
		{
			Mockery mockery;
			IPlainObject mockPersistentObject;
			DataOperation result;

			mockery = new Mockery();
			mockPersistentObject = mockery.NewMock<IPlainObject>();

			Expect.AtLeastOnce.On(mockPersistentObject).GetProperty("ObjectState").Will(Return.Value((ObjectState)999));
			//Expect.AtLeastOnce.On(mockPersistentObject).GetProperty("IsNew").Will(Return.Value(false));

			result = PlainObject.DetermineBefore(mockPersistentObject);

			Assert.AreEqual(DataOperation.StateError, result);

			mockery.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldGetFaultyObjectStateDetermineAfterTest()
		{
			Mockery mockery;
			IPlainObject mockPersistentObject;
			ObjectState result;

			mockery = new Mockery();
			mockPersistentObject = mockery.NewMock<IPlainObject>();

			Expect.AtLeastOnce.On(mockPersistentObject).GetProperty("ObjectState").Will(Return.Value((ObjectState)999));
			Expect.AtLeastOnce.On(mockPersistentObject).GetProperty("IsNew").Will(Return.Value(false));

			result = PlainObject.DetermineAfter(mockPersistentObject);
		}

		[Test]
		public void ShouldGetInsertDataOperationDetermineBeforeTest()
		{
			Mockery mockery;
			IPlainObject mockPersistentObject;
			DataOperation result;

			mockery = new Mockery();
			mockPersistentObject = mockery.NewMock<IPlainObject>();

			Expect.AtLeastOnce.On(mockPersistentObject).GetProperty("ObjectState").Will(Return.Value(ObjectState.Modified));
			Expect.AtLeastOnce.On(mockPersistentObject).GetProperty("IsNew").Will(Return.Value(true));

			result = PlainObject.DetermineBefore(mockPersistentObject);

			Assert.AreEqual(DataOperation.Insert, result);

			mockery.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldGetNoneConsistentDataOperationDetermineBeforeTest()
		{
			Mockery mockery;
			IPlainObject mockPersistentObject;
			DataOperation result;

			mockery = new Mockery();
			mockPersistentObject = mockery.NewMock<IPlainObject>();

			Expect.AtLeastOnce.On(mockPersistentObject).GetProperty("ObjectState").Will(Return.Value(ObjectState.Consistent));

			result = PlainObject.DetermineBefore(mockPersistentObject);

			Assert.AreEqual(DataOperation.None, result);

			mockery.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldGetNoneNewRemovedDataOperationDetermineBeforeTest()
		{
			Mockery mockery;
			IPlainObject mockPersistentObject;
			DataOperation result;

			mockery = new Mockery();
			mockPersistentObject = mockery.NewMock<IPlainObject>();

			Expect.AtLeastOnce.On(mockPersistentObject).GetProperty("ObjectState").Will(Return.Value(ObjectState.Removed));
			Expect.AtLeastOnce.On(mockPersistentObject).GetProperty("IsNew").Will(Return.Value(true));

			result = PlainObject.DetermineBefore(mockPersistentObject);

			Assert.AreEqual(DataOperation.None, result);

			mockery.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldGetNoneObsoletedDataOperationDetermineBeforeTest()
		{
			Mockery mockery;
			IPlainObject mockPersistentObject;
			DataOperation result;

			mockery = new Mockery();
			mockPersistentObject = mockery.NewMock<IPlainObject>();

			Expect.AtLeastOnce.On(mockPersistentObject).GetProperty("ObjectState").Will(Return.Value(ObjectState.Obsoleted));

			result = PlainObject.DetermineBefore(mockPersistentObject);

			Assert.AreEqual(DataOperation.None, result);

			mockery.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldGetObsoletedObjectStateDetermineAfterTest()
		{
			Mockery mockery;
			IPlainObject mockPersistentObject;
			ObjectState result;

			mockery = new Mockery();
			mockPersistentObject = mockery.NewMock<IPlainObject>();

			Expect.AtLeastOnce.On(mockPersistentObject).GetProperty("ObjectState").Will(Return.Value(ObjectState.Removed));

			result = PlainObject.DetermineAfter(mockPersistentObject);

			Assert.AreEqual(ObjectState.Obsoleted, result);

			mockery.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldGetObsoletedOriginalObjectStateDetermineAfterTest()
		{
			Mockery mockery;
			IPlainObject mockPersistentObject;
			ObjectState result;

			mockery = new Mockery();
			mockPersistentObject = mockery.NewMock<IPlainObject>();

			Expect.AtLeastOnce.On(mockPersistentObject).GetProperty("ObjectState").Will(Return.Value(ObjectState.Obsoleted));

			result = PlainObject.DetermineAfter(mockPersistentObject);

			Assert.AreEqual(ObjectState.Obsoleted, result);

			mockery.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldGetUpdateDataOperationDetermineBeforeTest()
		{
			Mockery mockery;
			IPlainObject mockPersistentObject;
			DataOperation result;

			mockery = new Mockery();
			mockPersistentObject = mockery.NewMock<IPlainObject>();

			Expect.AtLeastOnce.On(mockPersistentObject).GetProperty("ObjectState").Will(Return.Value(ObjectState.Modified));
			Expect.AtLeastOnce.On(mockPersistentObject).GetProperty("IsNew").Will(Return.Value(false));

			result = PlainObject.DetermineBefore(mockPersistentObject);

			Assert.AreEqual(DataOperation.Update, result);

			mockery.VerifyAllExpectationsHaveBeenMet();
		}

		#endregion
	}
}