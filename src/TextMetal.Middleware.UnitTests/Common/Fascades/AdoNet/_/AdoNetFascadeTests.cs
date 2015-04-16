/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Data;

using TextMetal.Middleware.Common.Fascades.AdoNet;
using TextMetal.Middleware.Common.Fascades.Utilities;

using NMock;

using NUnit.Framework;

namespace TextMetal.Middleware.UnitTests.Common.Fascades.AdoNet._
{
	[TestFixture]
	public class AdoNetFascadeTests
	{
		#region Constructors/Destructors

		public AdoNetFascadeTests()
		{
		}

		#endregion

		#region Methods/Operators

		[Test]
		public void ShouldExecuteReaderCloseConnectionSprocWithParametersTest()
		{
			IAdoNetFascade adoNetFascade;
			MockFactory mockFactory;
			IDbConnection mockDbConnection;
			IDbCommand mockDbCommand;
			IDataParameterCollection mockDbDataParameterCollection;
			IDbDataParameter[] mockDbDataParameters;
			IDbDataParameter mockDbDataParameter0;
			IDbDataParameter mockDbDataParameter1;
			IDataReader mockDataReader;
			IReflectionFascade mockReflectionFascade;

			IDataReader dataReader;

			mockFactory = new MockFactory();
			mockDbConnection = mockFactory.CreateInstance<IDbConnection>();
			mockDbCommand = mockFactory.CreateInstance<IDbCommand>();
			mockDbDataParameterCollection = mockFactory.CreateInstance<IDataParameterCollection>();
			mockDbDataParameter0 = mockFactory.CreateInstance<IDbDataParameter>();
			mockDbDataParameter1 = mockFactory.CreateInstance<IDbDataParameter>();
			mockDbDataParameters = new IDbDataParameter[] { mockDbDataParameter0, mockDbDataParameter1 };
			mockDataReader = mockFactory.CreateInstance<IDataReader>();
			mockReflectionFascade = mockFactory.CreateInstance<IReflectionFascade>();

			Expect.AtLeastOnce.On(mockDbConnection).Method("CreateCommand").WithNoArguments().Will(Return.Value(mockDbCommand));
			Expect.AtLeastOnce.On(mockDbCommand).GetProperty("Parameters").Will(Return.Value(mockDbDataParameterCollection));
			Expect.AtLeastOnce.On(mockDbDataParameterCollection).Method("Clear").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbCommand).Method("Dispose").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("Connection").To(mockDbConnection);
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("CommandType").To(CommandType.StoredProcedure);
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("CommandText").To("blah blah blah");
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("Transaction");
			Expect.AtLeastOnce.On(mockDbCommand).Method("ExecuteReader").With(CommandBehavior.CloseConnection).Will(Return.Value(mockDataReader));
			Expect.AtLeastOnce.On(mockDbDataParameter0).GetProperty("Value").Will(Return.Value(1));
			Expect.AtLeastOnce.On(mockDbDataParameter1).GetProperty("Value").Will(Return.Value(null));
			Expect.AtLeastOnce.On(mockDbDataParameter1).SetProperty("Value").To(DBNull.Value);
			Expect.AtLeastOnce.On(mockDbDataParameterCollection).Method("Add").With(mockDbDataParameter0).Will(Return.Value(0));
			Expect.AtLeastOnce.On(mockDbDataParameterCollection).Method("Add").With(mockDbDataParameter1).Will(Return.Value(0));
			Expect.AtLeastOnce.On(mockDataReader).Method("Dispose").WithNoArguments();

			adoNetFascade = new AdoNetFascade(mockReflectionFascade);

			dataReader = adoNetFascade.ExecuteReader(mockDbConnection, null, CommandType.StoredProcedure, "blah blah blah", mockDbDataParameters, CommandBehavior.CloseConnection, null, false);

			Assert.IsNotNull(dataReader);

			dataReader.Dispose();

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldExecuteReaderCloseConnectionTextNoParametersTest()
		{
			IAdoNetFascade adoNetFascade;
			MockFactory mockFactory;
			IDbConnection mockDbConnection;
			IDbCommand mockDbCommand;
			IDataParameterCollection mockDbDataParameterCollection;
			IDataReader mockDataReader;
			IReflectionFascade mockReflectionFascade;

			IDataReader dataReader;

			mockFactory = new MockFactory();
			mockDbConnection = mockFactory.CreateInstance<IDbConnection>();
			mockDbCommand = mockFactory.CreateInstance<IDbCommand>();
			mockDbDataParameterCollection = mockFactory.CreateInstance<IDataParameterCollection>();
			mockDataReader = mockFactory.CreateInstance<IDataReader>();
			mockReflectionFascade = mockFactory.CreateInstance<IReflectionFascade>();

			Expect.AtLeastOnce.On(mockDbConnection).Method("CreateCommand").WithNoArguments().Will(Return.Value(mockDbCommand));
			Expect.AtLeastOnce.On(mockDbCommand).GetProperty("Parameters").Will(Return.Value(mockDbDataParameterCollection));
			Expect.AtLeastOnce.On(mockDbDataParameterCollection).Method("Clear").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbCommand).Method("Dispose").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("Connection").To(mockDbConnection);
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("CommandType").To(CommandType.Text);
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("CommandText").To("blah blah blah");
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("Transaction");
			Expect.AtLeastOnce.On(mockDbCommand).Method("ExecuteReader").With(CommandBehavior.CloseConnection).Will(Return.Value(mockDataReader));
			Expect.AtLeastOnce.On(mockDataReader).Method("Dispose").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("CommandTimeout").To(15);
			Expect.AtLeastOnce.On(mockDbCommand).Method("Prepare").WithNoArguments();

			adoNetFascade = new AdoNetFascade(mockReflectionFascade);

			dataReader = adoNetFascade.ExecuteReader(mockDbConnection, null, CommandType.Text, "blah blah blah", null, CommandBehavior.CloseConnection, 15, true);

			Assert.IsNotNull(dataReader);

			dataReader.Dispose();

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldExecuteReaderCloseConnectionTextNoParametersWithTransactionTest()
		{
			IAdoNetFascade adoNetFascade;
			MockFactory mockFactory;
			IDbConnection mockDbConnection;
			IDbCommand mockDbCommand;
			IDataParameterCollection mockDbDataParameterCollection;
			IDataReader mockDataReader;
			IDbTransaction mockDbTransaction;
			IReflectionFascade mockReflectionFascade;

			IDataReader dataReader;

			mockFactory = new MockFactory();
			mockDbConnection = mockFactory.CreateInstance<IDbConnection>();
			mockDbCommand = mockFactory.CreateInstance<IDbCommand>();
			mockDbDataParameterCollection = mockFactory.CreateInstance<IDataParameterCollection>();
			mockDataReader = mockFactory.CreateInstance<IDataReader>();
			mockDbTransaction = mockFactory.CreateInstance<IDbTransaction>();
			mockReflectionFascade = mockFactory.CreateInstance<IReflectionFascade>();

			Expect.AtLeastOnce.On(mockDbConnection).Method("CreateCommand").WithNoArguments().Will(Return.Value(mockDbCommand));
			Expect.AtLeastOnce.On(mockDbCommand).GetProperty("Parameters").Will(Return.Value(mockDbDataParameterCollection));
			Expect.AtLeastOnce.On(mockDbDataParameterCollection).Method("Clear").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbCommand).Method("Dispose").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("Connection").To(mockDbConnection);
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("CommandType").To(CommandType.Text);
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("CommandText").To("blah blah blah");
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("Transaction").To(mockDbTransaction);
			Expect.AtLeastOnce.On(mockDbCommand).Method("ExecuteReader").With(CommandBehavior.CloseConnection).Will(Return.Value(mockDataReader));
			Expect.AtLeastOnce.On(mockDataReader).Method("Dispose").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("CommandTimeout").To(15);
			Expect.AtLeastOnce.On(mockDbCommand).Method("Prepare").WithNoArguments();

			adoNetFascade = new AdoNetFascade(mockReflectionFascade);

			dataReader = adoNetFascade.ExecuteReader(mockDbConnection, mockDbTransaction, CommandType.Text, "blah blah blah", null, CommandBehavior.CloseConnection, 15, true);

			Assert.IsNotNull(dataReader);

			dataReader.Dispose();

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldExecuteReaderNoCloseConnectionSprocWithParametersTest()
		{
			IAdoNetFascade adoNetFascade;
			MockFactory mockFactory;
			IDbConnection mockDbConnection;
			IDbCommand mockDbCommand;
			IDataParameterCollection mockDbDataParameterCollection;
			IDbDataParameter[] mockDbDataParameters;
			IDbDataParameter mockDbDataParameter0;
			IDbDataParameter mockDbDataParameter1;
			IDataReader mockDataReader;
			IReflectionFascade mockReflectionFascade;

			IDataReader dataReader;

			mockFactory = new MockFactory();
			mockDbConnection = mockFactory.CreateInstance<IDbConnection>();
			mockDbCommand = mockFactory.CreateInstance<IDbCommand>();
			mockDbDataParameterCollection = mockFactory.CreateInstance<IDataParameterCollection>();
			mockDbDataParameter0 = mockFactory.CreateInstance<IDbDataParameter>();
			mockDbDataParameter1 = mockFactory.CreateInstance<IDbDataParameter>();
			mockDbDataParameters = new IDbDataParameter[] { mockDbDataParameter0, mockDbDataParameter1 };
			mockDataReader = mockFactory.CreateInstance<IDataReader>();
			mockReflectionFascade = mockFactory.CreateInstance<IReflectionFascade>();

			Expect.AtLeastOnce.On(mockDbConnection).Method("CreateCommand").WithNoArguments().Will(Return.Value(mockDbCommand));
			Expect.AtLeastOnce.On(mockDbCommand).GetProperty("Parameters").Will(Return.Value(mockDbDataParameterCollection));
			Expect.AtLeastOnce.On(mockDbDataParameterCollection).Method("Clear").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbCommand).Method("Dispose").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("Connection").To(mockDbConnection);
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("CommandType").To(CommandType.StoredProcedure);
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("CommandText").To("blah blah blah");
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("Transaction");
			Expect.AtLeastOnce.On(mockDbCommand).Method("ExecuteReader").With(CommandBehavior.SingleRow).Will(Return.Value(mockDataReader));
			Expect.AtLeastOnce.On(mockDbDataParameter0).GetProperty("Value").Will(Return.Value(1));
			Expect.AtLeastOnce.On(mockDbDataParameter1).GetProperty("Value").Will(Return.Value(null));
			Expect.AtLeastOnce.On(mockDbDataParameter1).SetProperty("Value").To(DBNull.Value);
			Expect.AtLeastOnce.On(mockDbDataParameterCollection).Method("Add").With(mockDbDataParameter0).Will(Return.Value(0));
			Expect.AtLeastOnce.On(mockDbDataParameterCollection).Method("Add").With(mockDbDataParameter1).Will(Return.Value(0));
			Expect.AtLeastOnce.On(mockDataReader).Method("Dispose").WithNoArguments();

			adoNetFascade = new AdoNetFascade(mockReflectionFascade);

			dataReader = adoNetFascade.ExecuteReader(mockDbConnection, null, CommandType.StoredProcedure, "blah blah blah", mockDbDataParameters, CommandBehavior.SingleRow, null, false);

			Assert.IsNotNull(dataReader);

			dataReader.Dispose();

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldExecuteReaderNoCloseConnectionTextNoParametersTest()
		{
			IAdoNetFascade adoNetFascade;
			MockFactory mockFactory;
			IDbConnection mockDbConnection;
			IDbCommand mockDbCommand;
			IDataParameterCollection mockDbDataParameterCollection;
			IDataReader mockDataReader;
			IReflectionFascade mockReflectionFascade;

			IDataReader dataReader;

			mockFactory = new MockFactory();
			mockDbConnection = mockFactory.CreateInstance<IDbConnection>();
			mockDbCommand = mockFactory.CreateInstance<IDbCommand>();
			mockDbDataParameterCollection = mockFactory.CreateInstance<IDataParameterCollection>();
			mockDataReader = mockFactory.CreateInstance<IDataReader>();
			mockReflectionFascade = mockFactory.CreateInstance<IReflectionFascade>();

			Expect.AtLeastOnce.On(mockDbConnection).Method("CreateCommand").WithNoArguments().Will(Return.Value(mockDbCommand));
			Expect.AtLeastOnce.On(mockDbCommand).GetProperty("Parameters").Will(Return.Value(mockDbDataParameterCollection));
			Expect.AtLeastOnce.On(mockDbDataParameterCollection).Method("Clear").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbCommand).Method("Dispose").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("Connection").To(mockDbConnection);
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("CommandType").To(CommandType.Text);
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("CommandText").To("blah blah blah");
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("Transaction");
			Expect.AtLeastOnce.On(mockDbCommand).Method("ExecuteReader").With(CommandBehavior.SingleRow).Will(Return.Value(mockDataReader));
			Expect.AtLeastOnce.On(mockDataReader).Method("Dispose").WithNoArguments();

			adoNetFascade = new AdoNetFascade(mockReflectionFascade);

			dataReader = adoNetFascade.ExecuteReader(mockDbConnection, null, CommandType.Text, "blah blah blah", null, CommandBehavior.SingleRow, null, false);

			Assert.IsNotNull(dataReader);

			dataReader.Dispose();

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void ShouldFailOnInvalidTypeInferDbTypeForClrTypeTest()
		{
			IAdoNetFascade adoNetFascade;
			MockFactory mockFactory;
			IReflectionFascade mockReflectionFascade;

			mockFactory = new MockFactory();
			mockReflectionFascade = mockFactory.CreateInstance<IReflectionFascade>();

			adoNetFascade = new AdoNetFascade(mockReflectionFascade);

			adoNetFascade.InferDbTypeForClrType(typeof(Exception));
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullConnectionStaticExecuteReaderTest()
		{
			IAdoNetFascade adoNetFascade;
			MockFactory mockFactory;
			IReflectionFascade mockReflectionFascade;

			mockFactory = new MockFactory();
			mockReflectionFascade = mockFactory.CreateInstance<IReflectionFascade>();

			adoNetFascade = new AdoNetFascade(mockReflectionFascade);

			adoNetFascade.ExecuteReader(null, null, CommandType.StoredProcedure, null, null, CommandBehavior.CloseConnection, null, false);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullTypeInferDbTypeForClrTypeTest()
		{
			IAdoNetFascade adoNetFascade;
			MockFactory mockFactory;
			IReflectionFascade mockReflectionFascade;

			mockFactory = new MockFactory();
			mockReflectionFascade = mockFactory.CreateInstance<IReflectionFascade>();

			adoNetFascade = new AdoNetFascade(mockReflectionFascade);

			adoNetFascade.InferDbTypeForClrType(null);
		}

		[Test]
		public void ShouldInferDbTypeForClrTypeTest()
		{
			IAdoNetFascade adoNetFascade;
			MockFactory mockFactory;
			IReflectionFascade mockReflectionFascade;

			DbType dbType;

			mockFactory = new MockFactory();
			mockReflectionFascade = mockFactory.CreateInstance<IReflectionFascade>();

			adoNetFascade = new AdoNetFascade(mockReflectionFascade);

			dbType = adoNetFascade.InferDbTypeForClrType(typeof(Boolean));
			Assert.AreEqual(DbType.Boolean, dbType);

			dbType = adoNetFascade.InferDbTypeForClrType(typeof(Byte));
			Assert.AreEqual(DbType.Byte, dbType);

			dbType = adoNetFascade.InferDbTypeForClrType(typeof(DateTime));
			Assert.AreEqual(DbType.DateTime, dbType);

			dbType = adoNetFascade.InferDbTypeForClrType(typeof(DateTimeOffset));
			Assert.AreEqual(DbType.DateTimeOffset, dbType);

			dbType = adoNetFascade.InferDbTypeForClrType(typeof(Decimal));
			Assert.AreEqual(DbType.Decimal, dbType);

			dbType = adoNetFascade.InferDbTypeForClrType(typeof(Double));
			Assert.AreEqual(DbType.Double, dbType);

			dbType = adoNetFascade.InferDbTypeForClrType(typeof(Guid));
			Assert.AreEqual(DbType.Guid, dbType);

			dbType = adoNetFascade.InferDbTypeForClrType(typeof(Int16));
			Assert.AreEqual(DbType.Int16, dbType);

			dbType = adoNetFascade.InferDbTypeForClrType(typeof(Int32));
			Assert.AreEqual(DbType.Int32, dbType);

			dbType = adoNetFascade.InferDbTypeForClrType(typeof(Int64));
			Assert.AreEqual(DbType.Int64, dbType);

			dbType = adoNetFascade.InferDbTypeForClrType(typeof(SByte));
			Assert.AreEqual(DbType.SByte, dbType);

			dbType = adoNetFascade.InferDbTypeForClrType(typeof(Single));
			Assert.AreEqual(DbType.Single, dbType);

			dbType = adoNetFascade.InferDbTypeForClrType(typeof(UInt16));
			Assert.AreEqual(DbType.UInt16, dbType);

			dbType = adoNetFascade.InferDbTypeForClrType(typeof(UInt32));
			Assert.AreEqual(DbType.UInt32, dbType);

			dbType = adoNetFascade.InferDbTypeForClrType(typeof(UInt64));
			Assert.AreEqual(DbType.UInt64, dbType);

			dbType = adoNetFascade.InferDbTypeForClrType(typeof(Byte[]));
			Assert.AreEqual(DbType.Binary, dbType);

			dbType = adoNetFascade.InferDbTypeForClrType(typeof(Object));
			Assert.AreEqual(DbType.Object, dbType);

			dbType = adoNetFascade.InferDbTypeForClrType(typeof(UInt64));
			Assert.AreEqual(DbType.UInt64, dbType);

			dbType = adoNetFascade.InferDbTypeForClrType(typeof(DbType));
			Assert.AreEqual(DbType.Int32, dbType);

			dbType = adoNetFascade.InferDbTypeForClrType(typeof(Int32?));
			Assert.AreEqual(DbType.Int32, dbType);

			dbType = adoNetFascade.InferDbTypeForClrType(typeof(String));
			Assert.AreEqual(DbType.String, dbType);

			dbType = adoNetFascade.InferDbTypeForClrType(typeof(String).MakeByRefType());
			Assert.AreEqual(DbType.String, dbType);
		}

		#endregion
	}
}