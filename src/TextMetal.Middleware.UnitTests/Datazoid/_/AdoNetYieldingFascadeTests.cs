/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Data;

using NMock;

using NUnit.Framework;

using TextMetal.Middleware.Datazoid;
using TextMetal.Middleware.Solder.Utilities;

namespace TextMetal.Middleware.UnitTests.Datazoid._
{
	[TestFixture]
	public class AdoNetYieldingFascadeTests
	{
		#region Constructors/Destructors

		public AdoNetYieldingFascadeTests()
		{
		}

		#endregion

		#region Methods/Operators

		[Test]
		public void ShouldExecuteReaderCloseConnectionSprocWithParametersTest()
		{
			IAdoNetYieldingFascade adoNetYieldingFascade;
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

			CommandBehavior _unusedCommandBehavior = CommandBehavior.Default;
			IDbDataParameter _unusedDbDataParameter = null;

			mockFactory = new MockFactory();
			mockDbConnection = mockFactory.CreateInstance<IDbConnection>();
			mockDbCommand = mockFactory.CreateInstance<IDbCommand>();
			mockDbDataParameterCollection = mockFactory.CreateInstance<IDataParameterCollection>();
			mockDbDataParameter0 = mockFactory.CreateInstance<IDbDataParameter>();
			mockDbDataParameter1 = mockFactory.CreateInstance<IDbDataParameter>();
			mockDbDataParameters = new IDbDataParameter[] { mockDbDataParameter0, mockDbDataParameter1 };
			mockDataReader = mockFactory.CreateInstance<IDataReader>();
			mockReflectionFascade = mockFactory.CreateInstance<IReflectionFascade>();

			Expect.On(mockDbConnection).One.Method(x => x.CreateCommand()).Will(Return.Value(mockDbCommand));
			Expect.On(mockDbCommand).Exactly(2).GetProperty(x => x.Parameters).Will(Return.Value(mockDbDataParameterCollection));
			Expect.On(mockDbCommand).One.Method(x => x.Dispose());
			Expect.On(mockDbCommand).One.SetProperty(x => x.CommandType).To(CommandType.StoredProcedure);
			Expect.On(mockDbCommand).One.SetProperty(x => x.CommandText).To("blah blah blah");
			Expect.On(mockDbCommand).One.SetProperty(x => x.Transaction).To(null);
			Expect.On(mockDbCommand).One.Method(x => x.ExecuteReader(_unusedCommandBehavior)).With(CommandBehavior.CloseConnection).Will(Return.Value(mockDataReader));
			Expect.On(mockDbDataParameter0).One.GetProperty(x => x.Value).Will(Return.Value(1));
			Expect.On(mockDbDataParameter1).One.GetProperty(x => x.Value).Will(Return.Value(null));
			Expect.On(mockDbDataParameter1).One.SetProperty(x => x.Value).To(DBNull.Value);
			Expect.On(mockDbDataParameterCollection).One.Method(x => x.Add(_unusedDbDataParameter)).With(mockDbDataParameter0).Will(Return.Value(0));
			Expect.On(mockDbDataParameterCollection).One.Method(x => x.Add(_unusedDbDataParameter)).With(mockDbDataParameter1).Will(Return.Value(0));
			Expect.On(mockDataReader).One.Method(x => x.Dispose());

			adoNetYieldingFascade = new AdoNetYieldingFascade(mockReflectionFascade);

			dataReader = adoNetYieldingFascade.ExecuteReader(mockDbConnection, null, CommandType.StoredProcedure, "blah blah blah", mockDbDataParameters, CommandBehavior.CloseConnection, null, false);

			Assert.IsNotNull(dataReader);

			dataReader.Dispose();

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldExecuteReaderCloseConnectionTextNoParametersTest()
		{
			IAdoNetYieldingFascade adoNetYieldingFascade;
			MockFactory mockFactory;
			IDbConnection mockDbConnection;
			IDbCommand mockDbCommand;
			IDataReader mockDataReader;
			IReflectionFascade mockReflectionFascade;

			IDataReader dataReader;

			CommandBehavior _unusedCommandBehavior = CommandBehavior.Default;

			mockFactory = new MockFactory();
			mockDbConnection = mockFactory.CreateInstance<IDbConnection>();
			mockDbCommand = mockFactory.CreateInstance<IDbCommand>();
			mockDataReader = mockFactory.CreateInstance<IDataReader>();
			mockReflectionFascade = mockFactory.CreateInstance<IReflectionFascade>();

			Expect.On(mockDbConnection).One.Method(x => x.CreateCommand()).WithNoArguments().Will(Return.Value(mockDbCommand));
			Expect.On(mockDbCommand).One.Method(x => x.Dispose());
			Expect.On(mockDbCommand).One.SetProperty(x => x.CommandType).To(CommandType.Text);
			Expect.On(mockDbCommand).One.SetProperty(x => x.CommandText).To("blah blah blah");
			Expect.On(mockDbCommand).One.SetProperty(x => x.Transaction).To(null);
			Expect.On(mockDbCommand).One.Method(x => x.ExecuteReader(_unusedCommandBehavior)).With(CommandBehavior.CloseConnection).Will(Return.Value(mockDataReader));
			Expect.On(mockDataReader).One.Method(x => x.Dispose());
			Expect.On(mockDbCommand).One.SetProperty(x => x.CommandTimeout).To(15);
			Expect.On(mockDbCommand).One.Method(x => x.Prepare());

			adoNetYieldingFascade = new AdoNetYieldingFascade(mockReflectionFascade);

			dataReader = adoNetYieldingFascade.ExecuteReader(mockDbConnection, null, CommandType.Text, "blah blah blah", null, CommandBehavior.CloseConnection, 15, true);

			Assert.IsNotNull(dataReader);

			dataReader.Dispose();

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldExecuteReaderCloseConnectionTextNoParametersWithTransactionTest()
		{
			IAdoNetYieldingFascade adoNetYieldingFascade;
			MockFactory mockFactory;
			IDbConnection mockDbConnection;
			IDbCommand mockDbCommand;
			IDataReader mockDataReader;
			IDbTransaction mockDbTransaction;
			IReflectionFascade mockReflectionFascade;

			IDataReader dataReader;

			CommandBehavior _unusedCommandBehavior = CommandBehavior.Default;

			mockFactory = new MockFactory();
			mockDbConnection = mockFactory.CreateInstance<IDbConnection>();
			mockDbCommand = mockFactory.CreateInstance<IDbCommand>();
			mockDataReader = mockFactory.CreateInstance<IDataReader>();
			mockDbTransaction = mockFactory.CreateInstance<IDbTransaction>();
			mockReflectionFascade = mockFactory.CreateInstance<IReflectionFascade>();

			Expect.On(mockDbConnection).One.Method(x => x.CreateCommand()).WithNoArguments().Will(Return.Value(mockDbCommand));
			Expect.On(mockDbCommand).One.Method(x => x.Dispose());
			Expect.On(mockDbCommand).One.SetProperty(x => x.CommandType).To(CommandType.Text);
			Expect.On(mockDbCommand).One.SetProperty(x => x.CommandText).To("blah blah blah");
			Expect.On(mockDbCommand).One.SetProperty(x => x.Transaction).To(mockDbTransaction);
			Expect.On(mockDbCommand).One.Method(x => x.ExecuteReader(_unusedCommandBehavior)).With(CommandBehavior.CloseConnection).Will(Return.Value(mockDataReader));
			Expect.On(mockDataReader).One.Method(x => x.Dispose());
			Expect.On(mockDbCommand).One.SetProperty(x => x.CommandTimeout).To(15);
			Expect.On(mockDbCommand).One.Method(x => x.Prepare());

			adoNetYieldingFascade = new AdoNetYieldingFascade(mockReflectionFascade);

			dataReader = adoNetYieldingFascade.ExecuteReader(mockDbConnection, mockDbTransaction, CommandType.Text, "blah blah blah", null, CommandBehavior.CloseConnection, 15, true);

			Assert.IsNotNull(dataReader);

			dataReader.Dispose();

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldExecuteReaderNoCloseConnectionSprocWithParametersTest()
		{
			IAdoNetYieldingFascade adoNetYieldingFascade;
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

			CommandBehavior _unusedCommandBehavior = CommandBehavior.Default;
			IDbDataParameter _unusedDbDataParameter = null;

			mockFactory = new MockFactory();
			mockDbConnection = mockFactory.CreateInstance<IDbConnection>();
			mockDbCommand = mockFactory.CreateInstance<IDbCommand>();
			mockDbDataParameterCollection = mockFactory.CreateInstance<IDataParameterCollection>();
			mockDbDataParameter0 = mockFactory.CreateInstance<IDbDataParameter>();
			mockDbDataParameter1 = mockFactory.CreateInstance<IDbDataParameter>();
			mockDbDataParameters = new IDbDataParameter[] { mockDbDataParameter0, mockDbDataParameter1 };
			mockDataReader = mockFactory.CreateInstance<IDataReader>();
			mockReflectionFascade = mockFactory.CreateInstance<IReflectionFascade>();

			Expect.On(mockDbConnection).One.Method(x => x.CreateCommand()).WithNoArguments().Will(Return.Value(mockDbCommand));
			Expect.On(mockDbCommand).Exactly(2).GetProperty(x => x.Parameters).Will(Return.Value(mockDbDataParameterCollection));
			Expect.On(mockDbCommand).One.Method(x => x.Dispose());
			Expect.On(mockDbCommand).One.SetProperty(x => x.CommandType).To(CommandType.StoredProcedure);
			Expect.On(mockDbCommand).One.SetProperty(x => x.CommandText).To("blah blah blah");
			Expect.On(mockDbCommand).One.SetProperty(x => x.Transaction).To(null);
			Expect.On(mockDbCommand).One.Method(x => x.ExecuteReader(_unusedCommandBehavior)).With(CommandBehavior.SingleRow).Will(Return.Value(mockDataReader));
			Expect.On(mockDbDataParameter0).One.GetProperty(x => x.Value).Will(Return.Value(1));
			Expect.On(mockDbDataParameter1).One.GetProperty(x => x.Value).Will(Return.Value(null));
			Expect.On(mockDbDataParameter1).One.SetProperty(x => x.Value).To(DBNull.Value);
			Expect.On(mockDbDataParameterCollection).One.Method(x => x.Add(_unusedDbDataParameter)).With(mockDbDataParameter0).Will(Return.Value(0));
			Expect.On(mockDbDataParameterCollection).One.Method(x => x.Add(_unusedDbDataParameter)).With(mockDbDataParameter1).Will(Return.Value(0));
			Expect.On(mockDataReader).One.Method(x => x.Dispose());

			adoNetYieldingFascade = new AdoNetYieldingFascade(mockReflectionFascade);

			dataReader = adoNetYieldingFascade.ExecuteReader(mockDbConnection, null, CommandType.StoredProcedure, "blah blah blah", mockDbDataParameters, CommandBehavior.SingleRow, null, false);

			Assert.IsNotNull(dataReader);

			dataReader.Dispose();

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldExecuteReaderNoCloseConnectionTextNoParametersTest()
		{
			IAdoNetYieldingFascade adoNetYieldingFascade;
			MockFactory mockFactory;
			IDbConnection mockDbConnection;
			IDbCommand mockDbCommand;
			IDataReader mockDataReader;
			IReflectionFascade mockReflectionFascade;

			IDataReader dataReader;

			CommandBehavior _unusedCommandBehavior = CommandBehavior.Default;

			mockFactory = new MockFactory();
			mockDbConnection = mockFactory.CreateInstance<IDbConnection>();
			mockDbCommand = mockFactory.CreateInstance<IDbCommand>();
			mockDataReader = mockFactory.CreateInstance<IDataReader>();
			mockReflectionFascade = mockFactory.CreateInstance<IReflectionFascade>();

			Expect.On(mockDbConnection).One.Method(x => x.CreateCommand()).WithNoArguments().Will(Return.Value(mockDbCommand));
			Expect.On(mockDbCommand).One.Method(x => x.Dispose());
			Expect.On(mockDbCommand).One.SetProperty(x => x.CommandType).To(CommandType.Text);
			Expect.On(mockDbCommand).One.SetProperty(x => x.CommandText).To("blah blah blah");
			Expect.On(mockDbCommand).One.SetProperty(x => x.Transaction).To(null);
			Expect.On(mockDbCommand).One.Method(x => x.ExecuteReader(_unusedCommandBehavior)).With(CommandBehavior.SingleRow).Will(Return.Value(mockDataReader));
			Expect.On(mockDataReader).One.Method(x => x.Dispose());

			adoNetYieldingFascade = new AdoNetYieldingFascade(mockReflectionFascade);

			dataReader = adoNetYieldingFascade.ExecuteReader(mockDbConnection, null, CommandType.Text, "blah blah blah", null, CommandBehavior.SingleRow, null, false);

			Assert.IsNotNull(dataReader);

			dataReader.Dispose();

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void ShouldFailOnInvalidTypeInferDbTypeForClrTypeTest()
		{
			IAdoNetYieldingFascade adoNetYieldingFascade;
			MockFactory mockFactory;
			IReflectionFascade mockReflectionFascade;

			mockFactory = new MockFactory();
			mockReflectionFascade = mockFactory.CreateInstance<IReflectionFascade>();

			adoNetYieldingFascade = new AdoNetYieldingFascade(mockReflectionFascade);

			adoNetYieldingFascade.InferDbTypeForClrType(typeof(Exception));
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullConnectionStaticExecuteReaderTest()
		{
			IAdoNetYieldingFascade adoNetYieldingFascade;
			MockFactory mockFactory;
			IReflectionFascade mockReflectionFascade;

			mockFactory = new MockFactory();
			mockReflectionFascade = mockFactory.CreateInstance<IReflectionFascade>();

			adoNetYieldingFascade = new AdoNetYieldingFascade(mockReflectionFascade);

			adoNetYieldingFascade.ExecuteReader(null, null, CommandType.StoredProcedure, null, null, CommandBehavior.CloseConnection, null, false);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullTypeInferDbTypeForClrTypeTest()
		{
			IAdoNetYieldingFascade adoNetYieldingFascade;
			MockFactory mockFactory;
			IReflectionFascade mockReflectionFascade;

			mockFactory = new MockFactory();
			mockReflectionFascade = mockFactory.CreateInstance<IReflectionFascade>();

			adoNetYieldingFascade = new AdoNetYieldingFascade(mockReflectionFascade);

			adoNetYieldingFascade.InferDbTypeForClrType(null);
		}

		[Test]
		public void ShouldInferDbTypeForClrTypeTest()
		{
			IAdoNetYieldingFascade adoNetYieldingFascade;
			MockFactory mockFactory;
			IReflectionFascade mockReflectionFascade;

			DbType dbType;

			mockFactory = new MockFactory();
			mockReflectionFascade = mockFactory.CreateInstance<IReflectionFascade>();

			adoNetYieldingFascade = new AdoNetYieldingFascade(mockReflectionFascade);

			dbType = adoNetYieldingFascade.InferDbTypeForClrType(typeof(Boolean));
			Assert.AreEqual(DbType.Boolean, dbType);

			dbType = adoNetYieldingFascade.InferDbTypeForClrType(typeof(Byte));
			Assert.AreEqual(DbType.Byte, dbType);

			dbType = adoNetYieldingFascade.InferDbTypeForClrType(typeof(DateTime));
			Assert.AreEqual(DbType.DateTime, dbType);

			dbType = adoNetYieldingFascade.InferDbTypeForClrType(typeof(DateTimeOffset));
			Assert.AreEqual(DbType.DateTimeOffset, dbType);

			dbType = adoNetYieldingFascade.InferDbTypeForClrType(typeof(Decimal));
			Assert.AreEqual(DbType.Decimal, dbType);

			dbType = adoNetYieldingFascade.InferDbTypeForClrType(typeof(Double));
			Assert.AreEqual(DbType.Double, dbType);

			dbType = adoNetYieldingFascade.InferDbTypeForClrType(typeof(Guid));
			Assert.AreEqual(DbType.Guid, dbType);

			dbType = adoNetYieldingFascade.InferDbTypeForClrType(typeof(Int16));
			Assert.AreEqual(DbType.Int16, dbType);

			dbType = adoNetYieldingFascade.InferDbTypeForClrType(typeof(Int32));
			Assert.AreEqual(DbType.Int32, dbType);

			dbType = adoNetYieldingFascade.InferDbTypeForClrType(typeof(Int64));
			Assert.AreEqual(DbType.Int64, dbType);

			dbType = adoNetYieldingFascade.InferDbTypeForClrType(typeof(SByte));
			Assert.AreEqual(DbType.SByte, dbType);

			dbType = adoNetYieldingFascade.InferDbTypeForClrType(typeof(Single));
			Assert.AreEqual(DbType.Single, dbType);

			dbType = adoNetYieldingFascade.InferDbTypeForClrType(typeof(UInt16));
			Assert.AreEqual(DbType.UInt16, dbType);

			dbType = adoNetYieldingFascade.InferDbTypeForClrType(typeof(UInt32));
			Assert.AreEqual(DbType.UInt32, dbType);

			dbType = adoNetYieldingFascade.InferDbTypeForClrType(typeof(UInt64));
			Assert.AreEqual(DbType.UInt64, dbType);

			dbType = adoNetYieldingFascade.InferDbTypeForClrType(typeof(Byte[]));
			Assert.AreEqual(DbType.Binary, dbType);

			dbType = adoNetYieldingFascade.InferDbTypeForClrType(typeof(Object));
			Assert.AreEqual(DbType.Object, dbType);

			dbType = adoNetYieldingFascade.InferDbTypeForClrType(typeof(UInt64));
			Assert.AreEqual(DbType.UInt64, dbType);

			dbType = adoNetYieldingFascade.InferDbTypeForClrType(typeof(DbType));
			Assert.AreEqual(DbType.Int32, dbType);

			dbType = adoNetYieldingFascade.InferDbTypeForClrType(typeof(Int32?));
			Assert.AreEqual(DbType.Int32, dbType);

			dbType = adoNetYieldingFascade.InferDbTypeForClrType(typeof(String));
			Assert.AreEqual(DbType.String, dbType);

			dbType = adoNetYieldingFascade.InferDbTypeForClrType(typeof(String).MakeByRefType());
			Assert.AreEqual(DbType.String, dbType);
		}

		#endregion
	}
}