/*
	Copyright ©2002-2013 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Data;

using NMock2;

using NUnit.Framework;

using TextMetal.Common.Data;
using TextMetal.Common.Data.TypeMap;
using TextMetal.Common.Data.TypeMap.LowLevel;
using TextMetal.Common.UnitTests.TestingInfrastructure;

namespace TextMetal.Common.UnitTests.Data.TypeMap.LowLevel
{
	[TestFixture]
	public class MappingUtilsTests
	{
		#region Constructors/Destructors

		public MappingUtilsTests()
		{
		}

		#endregion

		#region Methods/Operators

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void ShouldFailOnMissingPropertyMapObjectFromOutputParametersTest()
		{
			Mockery mockery;
			IList<IDataParameter> mockDataParameters;
			IEnumerator<IDataParameter> mockEnumerator;
			Command mockCommand;
			DataOperation dataOperation;
			MockPlainObject targetInstance;
			IDataParameter mockDataParameter;

			mockery = new Mockery();

			mockDataParameters = mockery.NewMock<IList<IDataParameter>>();
			mockEnumerator = mockery.NewMock<IEnumerator<IDataParameter>>();
			mockDataParameter = mockery.NewMock<IDataParameter>();
			mockCommand = new Command();
			dataOperation = DataOperation.None;
			targetInstance = new MockPlainObject();

			mockCommand.Parameters.Add(new Parameter()
			                           {
				                           Direction = ParameterDirection.Output,
				                           Name = "@waxman",
				                           Property = "waxman_dne"
			                           });

			Expect.Once.On(mockDataParameters).Method("GetEnumerator").WithNoArguments().Will(Return.Value(mockEnumerator));
			Expect.Once.On(mockEnumerator).Method("Dispose").WithNoArguments();
			Expect.Once.On(mockEnumerator).GetProperty("Current").Will(Return.Value(mockDataParameter));
			Expect.Once.On(mockEnumerator).Method("MoveNext").WithNoArguments().Will(Return.Value(true));
			Expect.Once.On(mockEnumerator).Method("MoveNext").WithNoArguments().Will(Return.Value(false));
			Expect.Once.On(mockDataParameter).GetProperty("ParameterName").Will(Return.Value("@waxman"));
			Expect.Once.On(mockDataParameter).GetProperty("Value").Will(Return.Value("junk"));

			MappingUtils.MapObjectFromOutputParameters(mockDataParameters, mockCommand, dataOperation, targetInstance);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void ShouldFailOnMissingPropertyMapObjectFromRecordTest()
		{
			Mockery mockery;
			IDataRecord mockDataRecord;
			Command mockCommand;
			DataOperation dataOperation;
			MockPlainObject targetInstance;

			mockery = new Mockery();

			mockDataRecord = mockery.NewMock<IDataRecord>();
			mockCommand = new Command();
			dataOperation = DataOperation.None;
			targetInstance = new MockPlainObject();

			mockCommand.Fields.Add(new Field()
			                       {
				                       Name = "@waxman",
				                       Property = "waxman_dne"
			                       });

			Expect.Once.On(mockDataRecord).Get["@waxman"].Will(Return.Value("waxman_dne"));

			MappingUtils.MapObjectFromRecord(mockDataRecord, mockCommand, dataOperation, targetInstance);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void ShouldFailOnMissingPropertyMapObjectToInputParametersTest()
		{
			Mockery mockery;
			IList<IDataParameter> mockDataParameters;
			Command mockCommand;
			DataOperation dataOperation;
			MockPlainObject targetInstance;
			IUnitOfWorkContext mockUnitOfWorkContext;

			mockery = new Mockery();

			mockDataParameters = mockery.NewMock<IList<IDataParameter>>();
			mockCommand = new Command();
			dataOperation = DataOperation.None;
			targetInstance = new MockPlainObject();
			mockUnitOfWorkContext = mockery.NewMock<IUnitOfWorkContext>();

			mockCommand.Parameters.Add(new Parameter()
			                           {
				                           Direction = ParameterDirection.Input,
				                           Name = "@waxman",
				                           Property = "waxman_dne"
			                           });

			MappingUtils.MapObjectToInputParameters(mockDataParameters, mockCommand, dataOperation, targetInstance, mockUnitOfWorkContext);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void ShouldFailOnNoGetterPropertyMapObjectFromOutputParametersTest()
		{
			Mockery mockery;
			IList<IDataParameter> mockDataParameters;
			Command mockCommand;
			DataOperation dataOperation;
			MockPlainObject targetInstance;
			IEnumerator<IDataParameter> mockEnumerator;
			IDataParameter mockDataParameter;

			mockery = new Mockery();

			mockDataParameters = mockery.NewMock<IList<IDataParameter>>();
			mockCommand = new Command();
			dataOperation = DataOperation.None;
			targetInstance = new MockPlainObject();
			mockEnumerator = mockery.NewMock<IEnumerator<IDataParameter>>();
			mockDataParameter = mockery.NewMock<IDataParameter>();

			mockCommand.Parameters.Add(new Parameter()
			                           {
				                           Direction = ParameterDirection.Output,
				                           Name = "@GetOnly",
				                           Property = "MyGetOnlyProp"
			                           });

			Expect.Once.On(mockDataParameters).Method("GetEnumerator").WithNoArguments().Will(Return.Value(mockEnumerator));
			Expect.Once.On(mockEnumerator).Method("Dispose").WithNoArguments();
			Expect.Once.On(mockEnumerator).GetProperty("Current").Will(Return.Value(mockDataParameter));
			Expect.Once.On(mockEnumerator).Method("MoveNext").WithNoArguments().Will(Return.Value(true));
			Expect.Once.On(mockEnumerator).Method("MoveNext").WithNoArguments().Will(Return.Value(false));
			Expect.Once.On(mockDataParameter).GetProperty("ParameterName").Will(Return.Value("@FirstName"));
			Expect.Once.On(mockDataParameter).GetProperty("Value").Will(Return.Value("junk"));

			MappingUtils.MapObjectFromOutputParameters(mockDataParameters, mockCommand, dataOperation, targetInstance);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void ShouldFailOnNoGetterPropertyMapObjectToInputParametersTest()
		{
			Mockery mockery;
			IList<IDataParameter> mockDataParameters;
			Command mockCommand;
			DataOperation dataOperation;
			MockPlainObject targetInstance;
			IUnitOfWorkContext mockUnitOfWorkContext;

			mockery = new Mockery();

			mockDataParameters = mockery.NewMock<IList<IDataParameter>>();
			mockCommand = new Command();
			dataOperation = DataOperation.None;
			targetInstance = new MockPlainObject();
			mockUnitOfWorkContext = mockery.NewMock<IUnitOfWorkContext>();

			mockCommand.Parameters.Add(new Parameter()
			                           {
				                           Direction = ParameterDirection.Input,
				                           Name = "@SetOnly",
				                           Property = "MySetOnlyProp"
			                           });

			MappingUtils.MapObjectToInputParameters(mockDataParameters, mockCommand, dataOperation, targetInstance, mockUnitOfWorkContext);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void ShouldFailOnNoSetterPropertyMapObjectFromRecordTest()
		{
			Mockery mockery;
			IDataRecord mockDataRecord;
			Command mockCommand;
			DataOperation dataOperation;
			MockPlainObject targetInstance;

			mockery = new Mockery();

			mockDataRecord = mockery.NewMock<IDataRecord>();
			mockCommand = new Command();
			dataOperation = DataOperation.None;
			targetInstance = new MockPlainObject();

			mockCommand.Fields.Add(new Field()
			                       {
				                       Name = "@GetOnly",
				                       Property = "MyGetOnlyProp"
			                       });

			Expect.Once.On(mockDataRecord).Get["@GetOnly"].Will(Return.Value("junk"));

			MappingUtils.MapObjectFromRecord(mockDataRecord, mockCommand, dataOperation, targetInstance);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullCommandMapObjectFromOutputParametersNoTargetInstanceTest()
		{
			Mockery mockery;
			IList<IDataParameter> mockDataParameters;
			Command mockCommand;
			DataOperation dataOperation;

			mockery = new Mockery();

			mockDataParameters = mockery.NewMock<IList<IDataParameter>>();
			mockCommand = null;
			dataOperation = DataOperation.None;

			MappingUtils.MapObjectFromOutputParameters(mockDataParameters, mockCommand, dataOperation);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullCommandMapObjectFromOutputParametersTest()
		{
			Mockery mockery;
			IList<IDataParameter> mockDataParameters;
			Command mockCommand;
			DataOperation dataOperation;
			object targetInstance;

			mockery = new Mockery();

			mockDataParameters = mockery.NewMock<IList<IDataParameter>>();
			mockCommand = null;
			dataOperation = DataOperation.None;
			targetInstance = new object();

			MappingUtils.MapObjectFromOutputParameters(mockDataParameters, mockCommand, dataOperation, targetInstance);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullCommandMapObjectFromRecordTest()
		{
			Mockery mockery;
			IDataRecord mockDataRecord;
			Command mockCommand;
			DataOperation dataOperation;
			object targetInstance;

			mockery = new Mockery();

			mockDataRecord = mockery.NewMock<IDataRecord>();
			mockCommand = null;
			dataOperation = DataOperation.None;
			targetInstance = new object();

			MappingUtils.MapObjectFromRecord(mockDataRecord, mockCommand, dataOperation, targetInstance);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullCommandMapObjectToInputParametersNoTargetInstanceTest()
		{
			Mockery mockery;
			IList<IDataParameter> mockDataParameters;
			Command mockCommand;
			DataOperation dataOperation;
			IUnitOfWorkContext mockUnitOfWorkContext;

			mockery = new Mockery();

			mockDataParameters = mockery.NewMock<IList<IDataParameter>>();
			mockCommand = null;
			dataOperation = DataOperation.None;
			mockUnitOfWorkContext = mockery.NewMock<IUnitOfWorkContext>();

			MappingUtils.MapObjectToInputParameters(mockDataParameters, mockCommand, dataOperation, mockUnitOfWorkContext);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullCommandMapObjectToInputParametersTest()
		{
			Mockery mockery;
			IList<IDataParameter> mockDataParameters;
			Command mockCommand;
			DataOperation dataOperation;
			object targetInstance;
			IUnitOfWorkContext mockUnitOfWorkContext;

			mockery = new Mockery();

			mockDataParameters = mockery.NewMock<IList<IDataParameter>>();
			mockCommand = null;
			dataOperation = DataOperation.None;
			targetInstance = new object();
			mockUnitOfWorkContext = mockery.NewMock<IUnitOfWorkContext>();

			MappingUtils.MapObjectToInputParameters(mockDataParameters, mockCommand, dataOperation, targetInstance, mockUnitOfWorkContext);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullCommandMapObjectToOutputParametersNoTargetInstanceTest()
		{
			Mockery mockery;
			IList<IDataParameter> mockDataParameters;
			Command mockCommand;
			DataOperation dataOperation;
			IUnitOfWorkContext mockUnitOfWorkContext;

			mockery = new Mockery();

			mockDataParameters = mockery.NewMock<IList<IDataParameter>>();
			mockCommand = null;
			dataOperation = DataOperation.None;
			mockUnitOfWorkContext = mockery.NewMock<IUnitOfWorkContext>();

			MappingUtils.MapObjectToOutputParameters(mockDataParameters, mockCommand, dataOperation, mockUnitOfWorkContext);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullCommandMapObjectToOutputParametersTest()
		{
			Mockery mockery;
			IList<IDataParameter> mockDataParameters;
			Command mockCommand;
			DataOperation dataOperation;
			object targetInstance;
			IUnitOfWorkContext mockUnitOfWorkContext;

			mockery = new Mockery();

			mockDataParameters = mockery.NewMock<IList<IDataParameter>>();
			mockCommand = null;
			dataOperation = DataOperation.None;
			targetInstance = new object();
			mockUnitOfWorkContext = mockery.NewMock<IUnitOfWorkContext>();

			MappingUtils.MapObjectToOutputParameters(mockDataParameters, mockCommand, dataOperation, targetInstance, mockUnitOfWorkContext);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullDataParametersMapObjectFromOutputParametersNoTargetInstanceTest()
		{
			Mockery mockery;
			IList<IDataParameter> mockDataParameters;
			Command mockCommand;
			DataOperation dataOperation;

			mockery = new Mockery();

			mockDataParameters = null;
			mockCommand = new Command();
			dataOperation = DataOperation.None;

			MappingUtils.MapObjectFromOutputParameters(mockDataParameters, mockCommand, dataOperation);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullDataParametersMapObjectFromOutputParametersTest()
		{
			Mockery mockery;
			IList<IDataParameter> mockDataParameters;
			Command mockCommand;
			DataOperation dataOperation;
			object targetInstance;

			mockery = new Mockery();

			mockDataParameters = null;
			mockCommand = new Command();
			dataOperation = DataOperation.None;
			targetInstance = new object();

			MappingUtils.MapObjectFromOutputParameters(mockDataParameters, mockCommand, dataOperation, targetInstance);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullDataParametersMapObjectToInputParametersNoTargetInstanceTest()
		{
			Mockery mockery;
			IList<IDataParameter> mockDataParameters;
			Command mockCommand;
			DataOperation dataOperation;
			IUnitOfWorkContext mockUnitOfWorkContext;

			mockery = new Mockery();

			mockDataParameters = null;
			mockCommand = new Command();
			dataOperation = DataOperation.None;
			mockUnitOfWorkContext = mockery.NewMock<IUnitOfWorkContext>();

			MappingUtils.MapObjectToInputParameters(mockDataParameters, mockCommand, dataOperation, mockUnitOfWorkContext);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullDataParametersMapObjectToInputParametersTest()
		{
			Mockery mockery;
			IList<IDataParameter> mockDataParameters;
			Command mockCommand;
			DataOperation dataOperation;
			object targetInstance;
			IUnitOfWorkContext mockUnitOfWorkContext;

			mockery = new Mockery();

			mockDataParameters = null;
			mockCommand = new Command();
			dataOperation = DataOperation.None;
			targetInstance = new object();
			mockUnitOfWorkContext = mockery.NewMock<IUnitOfWorkContext>();

			MappingUtils.MapObjectToInputParameters(mockDataParameters, mockCommand, dataOperation, targetInstance, mockUnitOfWorkContext);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullDataParametersMapObjectToOutputParametersNoTargetInstanceTest()
		{
			Mockery mockery;
			IList<IDataParameter> mockDataParameters;
			Command mockCommand;
			DataOperation dataOperation;
			IUnitOfWorkContext mockUnitOfWorkContext;

			mockery = new Mockery();

			mockDataParameters = null;
			mockCommand = new Command();
			dataOperation = DataOperation.None;
			mockUnitOfWorkContext = mockery.NewMock<IUnitOfWorkContext>();

			MappingUtils.MapObjectToOutputParameters(mockDataParameters, mockCommand, dataOperation, mockUnitOfWorkContext);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullDataParametersMapObjectToOutputParametersTest()
		{
			Mockery mockery;
			IList<IDataParameter> mockDataParameters;
			Command mockCommand;
			DataOperation dataOperation;
			object targetInstance;
			IUnitOfWorkContext mockUnitOfWorkContext;

			mockery = new Mockery();

			mockDataParameters = null;
			mockCommand = new Command();
			dataOperation = DataOperation.None;
			targetInstance = new object();
			mockUnitOfWorkContext = mockery.NewMock<IUnitOfWorkContext>();

			MappingUtils.MapObjectToOutputParameters(mockDataParameters, mockCommand, dataOperation, targetInstance, mockUnitOfWorkContext);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullUnitOfWorkContextMapObjectToInputParametersNoTargetInstanceTest()
		{
			Mockery mockery;
			IList<IDataParameter> mockDataParameters;
			Command mockCommand;
			DataOperation dataOperation;
			IUnitOfWorkContext mockUnitOfWorkContext;

			mockery = new Mockery();

			mockDataParameters = mockery.NewMock<IList<IDataParameter>>();
			mockCommand = new Command();
			dataOperation = DataOperation.None;
			mockUnitOfWorkContext = null;

			MappingUtils.MapObjectToInputParameters(mockDataParameters, mockCommand, dataOperation, mockUnitOfWorkContext);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullUnitOfWorkContextMapObjectToInputParametersTest()
		{
			Mockery mockery;
			IList<IDataParameter> mockDataParameters;
			Command mockCommand;
			DataOperation dataOperation;
			object targetInstance;
			IUnitOfWorkContext mockUnitOfWorkContext;

			mockery = new Mockery();

			mockDataParameters = mockery.NewMock<IList<IDataParameter>>();
			mockCommand = new Command();
			dataOperation = DataOperation.None;
			targetInstance = new object();
			mockUnitOfWorkContext = null;

			MappingUtils.MapObjectToInputParameters(mockDataParameters, mockCommand, dataOperation, targetInstance, mockUnitOfWorkContext);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullUnitOfWorkContextMapObjectToOutputParametersNoTargetInstanceTest()
		{
			Mockery mockery;
			IList<IDataParameter> mockDataParameters;
			Command mockCommand;
			DataOperation dataOperation;
			IUnitOfWorkContext mockUnitOfWorkContext;

			mockery = new Mockery();

			mockDataParameters = mockery.NewMock<IList<IDataParameter>>();
			mockCommand = new Command();
			dataOperation = DataOperation.None;
			mockUnitOfWorkContext = null;

			MappingUtils.MapObjectToOutputParameters(mockDataParameters, mockCommand, dataOperation, mockUnitOfWorkContext);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullUnitOfWorkContextMapObjectToOutputParametersTest()
		{
			Mockery mockery;
			IList<IDataParameter> mockDataParameters;
			Command mockCommand;
			DataOperation dataOperation;
			object targetInstance;
			IUnitOfWorkContext mockUnitOfWorkContext;

			mockery = new Mockery();

			mockDataParameters = mockery.NewMock<IList<IDataParameter>>();
			mockCommand = new Command();
			dataOperation = DataOperation.None;
			targetInstance = new object();
			mockUnitOfWorkContext = null;

			MappingUtils.MapObjectToOutputParameters(mockDataParameters, mockCommand, dataOperation, targetInstance, mockUnitOfWorkContext);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullTargetInstanceMapObjectFromOutputParametersTest()
		{
			Mockery mockery;
			IList<IDataParameter> mockDataParameters;
			Command mockCommand;
			DataOperation dataOperation;
			object targetInstance;

			mockery = new Mockery();

			mockDataParameters = mockery.NewMock<IList<IDataParameter>>();
			mockCommand = new Command();
			dataOperation = DataOperation.None;
			targetInstance = null;

			MappingUtils.MapObjectFromOutputParameters(mockDataParameters, mockCommand, dataOperation, targetInstance);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullTargetInstanceMapObjectFromRecordTest()
		{
			Mockery mockery;
			IDataRecord mockDataRecord;
			Command mockCommand;
			DataOperation dataOperation;
			object targetInstance;

			mockery = new Mockery();

			mockDataRecord = mockery.NewMock<IDataRecord>();
			mockCommand = new Command();
			dataOperation = DataOperation.None;
			targetInstance = null;

			MappingUtils.MapObjectFromRecord(mockDataRecord, mockCommand, dataOperation, targetInstance);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullTargetInstanceMapObjectToInputParametersTest()
		{
			Mockery mockery;
			IList<IDataParameter> mockDataParameters;
			Command mockCommand;
			DataOperation dataOperation;
			object targetInstance;
			IUnitOfWorkContext mockUnitOfWorkContext;

			mockery = new Mockery();

			mockDataParameters = mockery.NewMock<IList<IDataParameter>>();
			mockCommand = new Command();
			dataOperation = DataOperation.None;
			targetInstance = null;
			mockUnitOfWorkContext = mockery.NewMock<IUnitOfWorkContext>();

			MappingUtils.MapObjectToInputParameters(mockDataParameters, mockCommand, dataOperation, targetInstance, mockUnitOfWorkContext);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullTargetInstanceMapObjectToOutputParametersTest()
		{
			Mockery mockery;
			IList<IDataParameter> mockDataParameters;
			Command mockCommand;
			DataOperation dataOperation;
			object targetInstance;
			IUnitOfWorkContext mockUnitOfWorkContext;

			mockery = new Mockery();

			mockDataParameters = mockery.NewMock<IList<IDataParameter>>();
			mockCommand = new Command();
			dataOperation = DataOperation.None;
			targetInstance = null;
			mockUnitOfWorkContext = mockery.NewMock<IUnitOfWorkContext>();

			MappingUtils.MapObjectToOutputParameters(mockDataParameters, mockCommand, dataOperation, targetInstance, mockUnitOfWorkContext);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNulldataRecordMapObjectFromRecordTest()
		{
			Mockery mockery;
			IDataRecord mockDataRecord;
			Command mockCommand;
			DataOperation dataOperation;
			object targetInstance;

			mockery = new Mockery();

			mockDataRecord = null;
			mockCommand = new Command();
			dataOperation = DataOperation.None;
			targetInstance = new object();

			MappingUtils.MapObjectFromRecord(mockDataRecord, mockCommand, dataOperation, targetInstance);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void ShouldFailOnUnknownParameterNameMapObjectFromOutputParametersTest()
		{
			Mockery mockery;
			IList<IDataParameter> mockDataParameters;
			Command mockCommand;
			DataOperation dataOperation;
			MockPlainObject targetInstance;
			IEnumerator<IDataParameter> mockEnumerator;
			IDataParameter mockDataParameter;

			mockery = new Mockery();

			mockDataParameters = mockery.NewMock<IList<IDataParameter>>();
			mockCommand = new Command();
			dataOperation = DataOperation.None;
			targetInstance = new MockPlainObject();
			mockEnumerator = mockery.NewMock<IEnumerator<IDataParameter>>();
			mockDataParameter = mockery.NewMock<IDataParameter>();

			mockCommand.Parameters.Add(new Parameter()
			                           {
				                           Direction = ParameterDirection.Output,
				                           Name = "@FirstName",
				                           Property = "FirstName"
			                           });

			mockCommand.Parameters.Add(new Parameter()
			                           {
				                           Direction = ParameterDirection.Input,
				                           Name = "@OutJunks",
				                           Property = "OutJunks"
			                           });

			Expect.Once.On(mockDataParameters).Method("GetEnumerator").WithNoArguments().Will(Return.Value(mockEnumerator));
			Expect.Once.On(mockEnumerator).Method("Dispose").WithNoArguments();
			Expect.Once.On(mockEnumerator).Method("MoveNext").WithNoArguments().Will(Return.Value(false));

			MappingUtils.MapObjectFromOutputParameters(mockDataParameters, mockCommand, dataOperation, targetInstance);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void ShouldFailOnUnknownParameterNameMapObjectFromRecordTest()
		{
			Mockery mockery;
			IDataRecord mockDataRecord;
			Command mockCommand;
			DataOperation dataOperation;
			MockPlainObject targetInstance;

			mockery = new Mockery();

			mockDataRecord = mockery.NewMock<IDataRecord>();
			mockCommand = new Command();
			dataOperation = DataOperation.None;
			targetInstance = new MockPlainObject();

			mockCommand.Fields.Add(new Field()
			                       {
				                       Name = "@FirstName",
				                       Property = "FirstName"
			                       });

			Expect.Once.On(mockDataRecord).Get["@FirstName"].Will(Throw.Exception(new IndexOutOfRangeException()));

			MappingUtils.MapObjectFromRecord(mockDataRecord, mockCommand, dataOperation, targetInstance);
		}

		[Test]
		public void ShouldMapObjectFromOutputParametersNoTargetInstanceTest()
		{
			Mockery mockery;
			IList<IDataParameter> mockDataParameters;
			Command mockCommand;
			DataOperation dataOperation;
			IEnumerator<IDataParameter> mockEnumerator;
			IDataParameter mockDataParameter;

			mockery = new Mockery();

			mockDataParameters = mockery.NewMock<IList<IDataParameter>>();
			mockCommand = new Command();
			dataOperation = DataOperation.None;
			mockEnumerator = mockery.NewMock<IEnumerator<IDataParameter>>();
			mockDataParameter = mockery.NewMock<IDataParameter>();

			mockCommand.Parameters.Add(new Parameter()
			                           {
				                           Direction = ParameterDirection.Output,
				                           Name = "@FirstName",
				                           Property = "FirstName"
			                           });

			mockCommand.Parameters.Add(new Parameter()
			                           {
				                           Direction = ParameterDirection.Input,
				                           Name = "@OutJunks",
				                           Property = "OutJunks"
			                           });

			MappingUtils.MapObjectFromOutputParameters(mockDataParameters, mockCommand, dataOperation);

			mockery.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldMapObjectFromOutputParametersTest()
		{
			Mockery mockery;
			IList<IDataParameter> mockDataParameters;
			Command mockCommand;
			DataOperation dataOperation;
			MockPlainObject targetInstance;
			IEnumerator<IDataParameter> mockEnumerator;
			IDataParameter mockDataParameter;

			mockery = new Mockery();

			mockDataParameters = mockery.NewMock<IList<IDataParameter>>();
			mockCommand = new Command();
			dataOperation = DataOperation.None;
			targetInstance = new MockPlainObject();
			mockEnumerator = mockery.NewMock<IEnumerator<IDataParameter>>();
			mockDataParameter = mockery.NewMock<IDataParameter>();

			mockCommand.Parameters.Add(new Parameter()
			                           {
				                           Direction = ParameterDirection.Output,
				                           Name = "@FirstName",
				                           Property = "FirstName"
			                           });

			mockCommand.Parameters.Add(new Parameter()
			                           {
				                           Direction = ParameterDirection.Input,
				                           Name = "@OutJunks",
				                           Property = "OutJunks"
			                           });

			Expect.Once.On(mockDataParameters).Method("GetEnumerator").WithNoArguments().Will(Return.Value(mockEnumerator));
			Expect.Once.On(mockEnumerator).Method("Dispose").WithNoArguments();
			Expect.Once.On(mockEnumerator).GetProperty("Current").Will(Return.Value(mockDataParameter));
			Expect.Once.On(mockEnumerator).Method("MoveNext").WithNoArguments().Will(Return.Value(true));
			Expect.Once.On(mockEnumerator).Method("MoveNext").WithNoArguments().Will(Return.Value(false));
			Expect.Once.On(mockDataParameter).GetProperty("ParameterName").Will(Return.Value("@FirstName"));
			Expect.Once.On(mockDataParameter).GetProperty("Value").Will(Return.Value("junk"));

			Assert.AreNotEqual("junk", targetInstance.FirstName);

			MappingUtils.MapObjectFromOutputParameters(mockDataParameters, mockCommand, dataOperation, targetInstance);

			Assert.AreEqual("junk", targetInstance.FirstName);

			mockery.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldMapObjectFromRecordTest()
		{
			Mockery mockery;
			IDataRecord mockDataRecord;
			Command mockCommand;
			DataOperation dataOperation;
			MockPlainObject targetInstance;

			mockery = new Mockery();

			mockDataRecord = mockery.NewMock<IDataRecord>();
			mockCommand = new Command();
			dataOperation = DataOperation.None;
			targetInstance = new MockPlainObject();

			mockCommand.Fields.Add(new Field()
			                       {
				                       Name = "@FirstName",
				                       Property = "FirstName"
			                       });

			Expect.Once.On(mockDataRecord).Get["@FirstName"].Will(Return.Value("junk"));

			Assert.AreNotEqual("junk", targetInstance.FirstName);

			MappingUtils.MapObjectFromRecord(mockDataRecord, mockCommand, dataOperation, targetInstance);

			Assert.AreEqual("junk", targetInstance.FirstName);

			mockery.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldMapObjectToInputParametersNoTargetInstanceTest()
		{
			Mockery mockery;
			IList<IDataParameter> mockDataParameters;
			Command mockCommand;
			DataOperation dataOperation;
			IUnitOfWorkContext mockUnitOfWorkContext;
			IDataParameter mockDataParameter;

			mockery = new Mockery();

			mockDataParameters = mockery.NewMock<IList<IDataParameter>>();
			mockCommand = new Command();
			dataOperation = DataOperation.None;
			mockUnitOfWorkContext = mockery.NewMock<IUnitOfWorkContext>();
			mockDataParameter = mockery.NewMock<IDataParameter>();

			mockCommand.Parameters.Add(new Parameter()
			                           {
				                           Direction = ParameterDirection.Input,
				                           Name = "@FirstName",
				                           Property = "FirstName"
			                           });

			mockCommand.Parameters.Add(new Parameter()
			                           {
				                           Direction = ParameterDirection.Output,
				                           Name = "@OutJunks",
				                           Property = "OutJunks"
			                           });

			Expect.Once.On(mockUnitOfWorkContext).Method("CreateParameter").With(mockCommand.Parameters[0].Direction, mockCommand.Parameters[0].Type, mockCommand.Parameters[0].Size, mockCommand.Parameters[0].Precision, mockCommand.Parameters[0].Scale, mockCommand.Parameters[0].Nullable, mockCommand.Parameters[0].Name, null).Will(Return.Value(mockDataParameter));
			Expect.Once.On(mockDataParameters).Method("Add").With(mockDataParameter);

			MappingUtils.MapObjectToInputParameters(mockDataParameters, mockCommand, dataOperation, mockUnitOfWorkContext);

			mockery.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldMapObjectToInputParametersTest()
		{
			Mockery mockery;
			IList<IDataParameter> mockDataParameters;
			Command mockCommand;
			DataOperation dataOperation;
			MockPlainObject targetInstance;
			IUnitOfWorkContext mockUnitOfWorkContext;
			IDataParameter mockDataParameter;

			mockery = new Mockery();

			mockDataParameters = mockery.NewMock<IList<IDataParameter>>();
			mockCommand = new Command();
			dataOperation = DataOperation.None;
			targetInstance = new MockPlainObject();
			mockUnitOfWorkContext = mockery.NewMock<IUnitOfWorkContext>();
			mockDataParameter = mockery.NewMock<IDataParameter>();

			mockCommand.Parameters.Add(new Parameter()
			                           {
				                           Direction = ParameterDirection.Input,
				                           Name = "@FirstName",
				                           Property = "FirstName"
			                           });

			mockCommand.Parameters.Add(new Parameter()
			                           {
				                           Direction = ParameterDirection.Output,
				                           Name = "@OutJunks",
				                           Property = "OutJunks"
			                           });

			Expect.Once.On(mockUnitOfWorkContext).Method("CreateParameter").With(mockCommand.Parameters[0].Direction, mockCommand.Parameters[0].Type, mockCommand.Parameters[0].Size, mockCommand.Parameters[0].Precision, mockCommand.Parameters[0].Scale, mockCommand.Parameters[0].Nullable, mockCommand.Parameters[0].Name, targetInstance.FirstName).Will(Return.Value(mockDataParameter));
			Expect.Once.On(mockDataParameters).Method("Add").With(mockDataParameter);

			MappingUtils.MapObjectToInputParameters(mockDataParameters, mockCommand, dataOperation, targetInstance, mockUnitOfWorkContext);

			mockery.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldMapObjectToOutputParametersNoTargetInstanceTest()
		{
			Mockery mockery;
			IList<IDataParameter> mockDataParameters;
			Command mockCommand;
			DataOperation dataOperation;
			IUnitOfWorkContext mockUnitOfWorkContext;
			IDataParameter mockDataParameter;

			mockery = new Mockery();

			mockDataParameters = mockery.NewMock<IList<IDataParameter>>();
			mockCommand = new Command();
			dataOperation = DataOperation.None;
			mockUnitOfWorkContext = mockery.NewMock<IUnitOfWorkContext>();
			mockDataParameter = mockery.NewMock<IDataParameter>();

			mockCommand.Parameters.Add(new Parameter()
			                           {
				                           Direction = ParameterDirection.Output,
				                           Name = "@OutJunks",
				                           Property = "OutJunks"
			                           });

			mockCommand.Parameters.Add(new Parameter()
			                           {
				                           Direction = ParameterDirection.Input,
				                           Name = "@FirstName",
				                           Property = "FirstName"
			                           });

			Expect.Once.On(mockUnitOfWorkContext).Method("CreateParameter").With(mockCommand.Parameters[0].Direction, mockCommand.Parameters[0].Type, mockCommand.Parameters[0].Size, mockCommand.Parameters[0].Precision, mockCommand.Parameters[0].Scale, mockCommand.Parameters[0].Nullable, mockCommand.Parameters[0].Name, null).Will(Return.Value(mockDataParameter));
			Expect.Once.On(mockDataParameters).Method("Add").With(mockDataParameter);

			MappingUtils.MapObjectToOutputParameters(mockDataParameters, mockCommand, dataOperation, mockUnitOfWorkContext);

			mockery.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldMapObjectToOutputParametersTest()
		{
			Mockery mockery;
			IList<IDataParameter> mockDataParameters;
			Command mockCommand;
			DataOperation dataOperation;
			MockPlainObject targetInstance;
			IUnitOfWorkContext mockUnitOfWorkContext;
			IDataParameter mockDataParameter;

			mockery = new Mockery();

			mockDataParameters = mockery.NewMock<IList<IDataParameter>>();
			mockCommand = new Command();
			dataOperation = DataOperation.None;
			targetInstance = new MockPlainObject();
			mockUnitOfWorkContext = mockery.NewMock<IUnitOfWorkContext>();
			mockDataParameter = mockery.NewMock<IDataParameter>();

			mockCommand.Parameters.Add(new Parameter()
			                           {
				                           Direction = ParameterDirection.Output,
				                           Name = "@OutJunks",
				                           Property = "OutJunks"
			                           });

			mockCommand.Parameters.Add(new Parameter()
			                           {
				                           Direction = ParameterDirection.Input,
				                           Name = "@FirstName",
				                           Property = "FirstName"
			                           });

			Expect.Once.On(mockUnitOfWorkContext).Method("CreateParameter").With(mockCommand.Parameters[0].Direction, mockCommand.Parameters[0].Type, mockCommand.Parameters[0].Size, mockCommand.Parameters[0].Precision, mockCommand.Parameters[0].Scale, mockCommand.Parameters[0].Nullable, mockCommand.Parameters[0].Name, targetInstance.FirstName).Will(Return.Value(mockDataParameter));
			Expect.Once.On(mockDataParameters).Method("Add").With(mockDataParameter);

			MappingUtils.MapObjectToOutputParameters(mockDataParameters, mockCommand, dataOperation, targetInstance, mockUnitOfWorkContext);

			mockery.VerifyAllExpectationsHaveBeenMet();
		}

		#endregion
	}
}