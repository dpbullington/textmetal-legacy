/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Data;

using NMock2;

using NUnit.Framework;

using TextMetal.Common.Data;

namespace TextMetal.Common.UnitTests.Data._
{
	[TestFixture]
	public class AdoNetHelperTests
	{
		#region Constructors/Destructors

		public AdoNetHelperTests()
		{
		}

		#endregion

		#region Methods/Operators

		[Test]
		public void ShouldExecuteReaderCloseConnectionSprocWithParametersTest()
		{
			Mockery mockery;
			IDbConnection mockDbConnection;
			IDbCommand mockDbCommand;
			IDataParameterCollection mockDataParameterCollection;
			IDataParameter[] mockDataParameters;
			IDataParameter mockDataParameter0;
			IDataParameter mockDataParameter1;
			IDataReader mockDataReader;

			IDataReader dataReader;

			mockery = new Mockery();
			mockDbConnection = mockery.NewMock<IDbConnection>();
			mockDbCommand = mockery.NewMock<IDbCommand>();
			mockDataParameterCollection = mockery.NewMock<IDataParameterCollection>();
			mockDataParameter0 = mockery.NewMock<IDataParameter>();
			mockDataParameter1 = mockery.NewMock<IDataParameter>();
			mockDataParameters = new IDataParameter[] { mockDataParameter0, mockDataParameter1 };
			mockDataReader = mockery.NewMock<IDataReader>();

			Expect.AtLeastOnce.On(mockDbConnection).Method("CreateCommand").WithNoArguments().Will(Return.Value(mockDbCommand));
			Expect.AtLeastOnce.On(mockDbCommand).GetProperty("Parameters").Will(Return.Value(mockDataParameterCollection));
			Expect.AtLeastOnce.On(mockDataParameterCollection).Method("Clear").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbCommand).Method("Dispose").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("Connection").To(mockDbConnection);
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("CommandType").To(CommandType.StoredProcedure);
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("CommandText").To("blah blah blah");
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("Transaction");
			Expect.AtLeastOnce.On(mockDbCommand).Method("ExecuteReader").With(CommandBehavior.CloseConnection).Will(Return.Value(mockDataReader));
			Expect.AtLeastOnce.On(mockDataParameter0).GetProperty("Value").Will(Return.Value(1));
			Expect.AtLeastOnce.On(mockDataParameter1).GetProperty("Value").Will(Return.Value(null));
			Expect.AtLeastOnce.On(mockDataParameter1).SetProperty("Value").To(DBNull.Value);
			Expect.AtLeastOnce.On(mockDataParameterCollection).Method("Add").With(mockDataParameter0).Will(Return.Value(0));
			Expect.AtLeastOnce.On(mockDataParameterCollection).Method("Add").With(mockDataParameter1).Will(Return.Value(0));
			Expect.AtLeastOnce.On(mockDataReader).Method("Dispose").WithNoArguments();

			dataReader = AdoNetHelper.ExecuteReader(mockDbConnection, null, CommandType.StoredProcedure, "blah blah blah", mockDataParameters, CommandBehavior.CloseConnection, null, false);

			Assert.IsNotNull(dataReader);

			dataReader.Dispose();

			mockery.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldExecuteReaderCloseConnectionTextNoParametersTest()
		{
			Mockery mockery;
			IDbConnection mockDbConnection;
			IDbCommand mockDbCommand;
			IDataParameterCollection mockDataParameterCollection;
			IDataReader mockDataReader;

			IDataReader dataReader;

			mockery = new Mockery();
			mockDbConnection = mockery.NewMock<IDbConnection>();
			mockDbCommand = mockery.NewMock<IDbCommand>();
			mockDataParameterCollection = mockery.NewMock<IDataParameterCollection>();
			mockDataReader = mockery.NewMock<IDataReader>();

			Expect.AtLeastOnce.On(mockDbConnection).Method("CreateCommand").WithNoArguments().Will(Return.Value(mockDbCommand));
			Expect.AtLeastOnce.On(mockDbCommand).GetProperty("Parameters").Will(Return.Value(mockDataParameterCollection));
			Expect.AtLeastOnce.On(mockDataParameterCollection).Method("Clear").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbCommand).Method("Dispose").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("Connection").To(mockDbConnection);
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("CommandType").To(CommandType.Text);
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("CommandText").To("blah blah blah");
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("Transaction");
			Expect.AtLeastOnce.On(mockDbCommand).Method("ExecuteReader").With(CommandBehavior.CloseConnection).Will(Return.Value(mockDataReader));
			Expect.AtLeastOnce.On(mockDataReader).Method("Dispose").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("CommandTimeout").To(15);
			Expect.AtLeastOnce.On(mockDbCommand).Method("Prepare").WithNoArguments();

			dataReader = AdoNetHelper.ExecuteReader(mockDbConnection, null, CommandType.Text, "blah blah blah", null, CommandBehavior.CloseConnection, 15, true);

			Assert.IsNotNull(dataReader);

			dataReader.Dispose();

			mockery.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldExecuteReaderCloseConnectionTextNoParametersWithTransactionTest()
		{
			Mockery mockery;
			IDbConnection mockDbConnection;
			IDbCommand mockDbCommand;
			IDataParameterCollection mockDataParameterCollection;
			IDataReader mockDataReader;
			IDbTransaction mockDbTransaction;

			IDataReader dataReader;

			mockery = new Mockery();
			mockDbConnection = mockery.NewMock<IDbConnection>();
			mockDbCommand = mockery.NewMock<IDbCommand>();
			mockDataParameterCollection = mockery.NewMock<IDataParameterCollection>();
			mockDataReader = mockery.NewMock<IDataReader>();
			mockDbTransaction = mockery.NewMock<IDbTransaction>();

			Expect.AtLeastOnce.On(mockDbConnection).Method("CreateCommand").WithNoArguments().Will(Return.Value(mockDbCommand));
			Expect.AtLeastOnce.On(mockDbCommand).GetProperty("Parameters").Will(Return.Value(mockDataParameterCollection));
			Expect.AtLeastOnce.On(mockDataParameterCollection).Method("Clear").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbCommand).Method("Dispose").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("Connection").To(mockDbConnection);
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("CommandType").To(CommandType.Text);
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("CommandText").To("blah blah blah");
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("Transaction").To(mockDbTransaction);
			Expect.AtLeastOnce.On(mockDbCommand).Method("ExecuteReader").With(CommandBehavior.CloseConnection).Will(Return.Value(mockDataReader));
			Expect.AtLeastOnce.On(mockDataReader).Method("Dispose").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("CommandTimeout").To(15);
			Expect.AtLeastOnce.On(mockDbCommand).Method("Prepare").WithNoArguments();

			dataReader = AdoNetHelper.ExecuteReader(mockDbConnection, mockDbTransaction, CommandType.Text, "blah blah blah", null, CommandBehavior.CloseConnection, 15, true);

			Assert.IsNotNull(dataReader);

			dataReader.Dispose();

			mockery.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldExecuteReaderNoCloseConnectionSprocWithParametersTest()
		{
			Mockery mockery;
			IDbConnection mockDbConnection;
			IDbCommand mockDbCommand;
			IDataParameterCollection mockDataParameterCollection;
			IDataParameter[] mockDataParameters;
			IDataParameter mockDataParameter0;
			IDataParameter mockDataParameter1;
			IDataReader mockDataReader;

			IDataReader dataReader;

			mockery = new Mockery();
			mockDbConnection = mockery.NewMock<IDbConnection>();
			mockDbCommand = mockery.NewMock<IDbCommand>();
			mockDataParameterCollection = mockery.NewMock<IDataParameterCollection>();
			mockDataParameter0 = mockery.NewMock<IDataParameter>();
			mockDataParameter1 = mockery.NewMock<IDataParameter>();
			mockDataParameters = new IDataParameter[] { mockDataParameter0, mockDataParameter1 };
			mockDataReader = mockery.NewMock<IDataReader>();

			Expect.AtLeastOnce.On(mockDbConnection).Method("CreateCommand").WithNoArguments().Will(Return.Value(mockDbCommand));
			Expect.AtLeastOnce.On(mockDbCommand).GetProperty("Parameters").Will(Return.Value(mockDataParameterCollection));
			Expect.AtLeastOnce.On(mockDataParameterCollection).Method("Clear").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbCommand).Method("Dispose").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("Connection").To(mockDbConnection);
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("CommandType").To(CommandType.StoredProcedure);
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("CommandText").To("blah blah blah");
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("Transaction");
			Expect.AtLeastOnce.On(mockDbCommand).Method("ExecuteReader").With(CommandBehavior.SingleRow).Will(Return.Value(mockDataReader));
			Expect.AtLeastOnce.On(mockDataParameter0).GetProperty("Value").Will(Return.Value(1));
			Expect.AtLeastOnce.On(mockDataParameter1).GetProperty("Value").Will(Return.Value(null));
			Expect.AtLeastOnce.On(mockDataParameter1).SetProperty("Value").To(DBNull.Value);
			Expect.AtLeastOnce.On(mockDataParameterCollection).Method("Add").With(mockDataParameter0).Will(Return.Value(0));
			Expect.AtLeastOnce.On(mockDataParameterCollection).Method("Add").With(mockDataParameter1).Will(Return.Value(0));
			Expect.AtLeastOnce.On(mockDataReader).Method("Dispose").WithNoArguments();

			dataReader = AdoNetHelper.ExecuteReader(mockDbConnection, null, CommandType.StoredProcedure, "blah blah blah", mockDataParameters, CommandBehavior.SingleRow, null, false);

			Assert.IsNotNull(dataReader);

			dataReader.Dispose();

			mockery.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldExecuteReaderNoCloseConnectionTextNoParametersTest()
		{
			Mockery mockery;
			IDbConnection mockDbConnection;
			IDbCommand mockDbCommand;
			IDataParameterCollection mockDataParameterCollection;
			IDataReader mockDataReader;

			IDataReader dataReader;

			mockery = new Mockery();
			mockDbConnection = mockery.NewMock<IDbConnection>();
			mockDbCommand = mockery.NewMock<IDbCommand>();
			mockDataParameterCollection = mockery.NewMock<IDataParameterCollection>();
			mockDataReader = mockery.NewMock<IDataReader>();

			Expect.AtLeastOnce.On(mockDbConnection).Method("CreateCommand").WithNoArguments().Will(Return.Value(mockDbCommand));
			Expect.AtLeastOnce.On(mockDbCommand).GetProperty("Parameters").Will(Return.Value(mockDataParameterCollection));
			Expect.AtLeastOnce.On(mockDataParameterCollection).Method("Clear").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbCommand).Method("Dispose").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("Connection").To(mockDbConnection);
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("CommandType").To(CommandType.Text);
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("CommandText").To("blah blah blah");
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("Transaction");
			Expect.AtLeastOnce.On(mockDbCommand).Method("ExecuteReader").With(CommandBehavior.SingleRow).Will(Return.Value(mockDataReader));
			Expect.AtLeastOnce.On(mockDataReader).Method("Dispose").WithNoArguments();

			dataReader = AdoNetHelper.ExecuteReader(mockDbConnection, null, CommandType.Text, "blah blah blah", null, CommandBehavior.SingleRow, null, false);

			Assert.IsNotNull(dataReader);

			dataReader.Dispose();

			mockery.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void ShouldFailOnInvalidTypeInferDbTypeForClrTypeTest()
		{
			AdoNetHelper.InferDbTypeForClrType(typeof(Exception));
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullConnectionStaticExecuteReaderTest()
		{
			AdoNetHelper.ExecuteReader(null, null, CommandType.StoredProcedure, null, null, CommandBehavior.CloseConnection, null, false);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullTypeInferDbTypeForClrTypeTest()
		{
			AdoNetHelper.InferDbTypeForClrType(null);
		}

		[Test]
		public void ShouldInferDbTypeForClrTypeTest()
		{
			DbType dbType;

			dbType = AdoNetHelper.InferDbTypeForClrType(typeof(Boolean));
			Assert.AreEqual(DbType.Boolean, dbType);

			dbType = AdoNetHelper.InferDbTypeForClrType(typeof(Byte));
			Assert.AreEqual(DbType.Byte, dbType);

			dbType = AdoNetHelper.InferDbTypeForClrType(typeof(DateTime));
			Assert.AreEqual(DbType.DateTime, dbType);

			dbType = AdoNetHelper.InferDbTypeForClrType(typeof(DateTimeOffset));
			Assert.AreEqual(DbType.DateTimeOffset, dbType);

			dbType = AdoNetHelper.InferDbTypeForClrType(typeof(Decimal));
			Assert.AreEqual(DbType.Decimal, dbType);

			dbType = AdoNetHelper.InferDbTypeForClrType(typeof(Double));
			Assert.AreEqual(DbType.Double, dbType);

			dbType = AdoNetHelper.InferDbTypeForClrType(typeof(Guid));
			Assert.AreEqual(DbType.Guid, dbType);

			dbType = AdoNetHelper.InferDbTypeForClrType(typeof(Int16));
			Assert.AreEqual(DbType.Int16, dbType);

			dbType = AdoNetHelper.InferDbTypeForClrType(typeof(Int32));
			Assert.AreEqual(DbType.Int32, dbType);

			dbType = AdoNetHelper.InferDbTypeForClrType(typeof(Int64));
			Assert.AreEqual(DbType.Int64, dbType);

			dbType = AdoNetHelper.InferDbTypeForClrType(typeof(SByte));
			Assert.AreEqual(DbType.SByte, dbType);

			dbType = AdoNetHelper.InferDbTypeForClrType(typeof(Single));
			Assert.AreEqual(DbType.Single, dbType);

			dbType = AdoNetHelper.InferDbTypeForClrType(typeof(UInt16));
			Assert.AreEqual(DbType.UInt16, dbType);

			dbType = AdoNetHelper.InferDbTypeForClrType(typeof(UInt32));
			Assert.AreEqual(DbType.UInt32, dbType);

			dbType = AdoNetHelper.InferDbTypeForClrType(typeof(UInt64));
			Assert.AreEqual(DbType.UInt64, dbType);

			dbType = AdoNetHelper.InferDbTypeForClrType(typeof(Byte[]));
			Assert.AreEqual(DbType.Binary, dbType);

			dbType = AdoNetHelper.InferDbTypeForClrType(typeof(Object));
			Assert.AreEqual(DbType.Object, dbType);

			dbType = AdoNetHelper.InferDbTypeForClrType(typeof(UInt64));
			Assert.AreEqual(DbType.UInt64, dbType);

			dbType = AdoNetHelper.InferDbTypeForClrType(typeof(DbType));
			Assert.AreEqual(DbType.Int32, dbType);

			dbType = AdoNetHelper.InferDbTypeForClrType(typeof(Int32?));
			Assert.AreEqual(DbType.Int32, dbType);

			dbType = AdoNetHelper.InferDbTypeForClrType(typeof(String));
			Assert.AreEqual(DbType.String, dbType);

			dbType = AdoNetHelper.InferDbTypeForClrType(typeof(String).MakeByRefType());
			Assert.AreEqual(DbType.String, dbType);
		}

		#endregion
	}
}