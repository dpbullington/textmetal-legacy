/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Data;
using System.Data.Common;

using NUnit.Framework;

using TextMetal.Middleware.UnitTests.TestingInfrastructure;

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
			DbConnection mockDbConnection;
			DbCommand mockDbCommand;
			IDataParameterCollection mockDbParameterCollection;
			DbParameter[] mockDbParameters;
			DbParameter mockDbParameter0;
			DbParameter mockDbParameter1;
			DbDataReader mockDbDataReader;
			IReflectionFascade mockReflectionFascade;

			DbDataReader dbDataReader;

			CommandBehavior _unusedCommandBehavior = CommandBehavior.Default;
			DbParameter _unusedDbParameter = null;

			mockFactory = new MockFactory();
			mockDbConnection = mockFactory.CreateInstance<DbConnection>();
			mockDbCommand = mockFactory.CreateInstance<DbCommand>();
			mockDbParameterCollection = mockFactory.CreateInstance<IDataParameterCollection>();
			mockDbParameter0 = mockFactory.CreateInstance<DbParameter>();
			mockDbParameter1 = mockFactory.CreateInstance<DbParameter>();
			mockDbParameters = new DbParameter[] { mockDbParameter0, mockDbParameter1 };
			mockDbDataReader = mockFactory.CreateInstance<DbDataReader>();
			mockReflectionFascade = mockFactory.CreateInstance<IReflectionFascade>();

			Expect.On(mockDbConnection).One.Method(x => x.CreateCommand()).Will(Return.Value(mockDbCommand));
			Expect.On(mockDbCommand).Exactly(2).GetProperty(x => x.Parameters).Will(Return.Value(mockDbParameterCollection));
			Expect.On(mockDbCommand).One.Method(x => x.Dispose());
			Expect.On(mockDbCommand).One.SetProperty(x => x.CommandType).To(CommandType.StoredProcedure);
			Expect.On(mockDbCommand).One.SetProperty(x => x.CommandText).To("blah blah blah");
			Expect.On(mockDbCommand).One.SetProperty(x => x.Transaction).To(null);
			Expect.On(mockDbCommand).One.Method(x => x.ExecuteReader(_unusedCommandBehavior)).With(CommandBehavior.CloseConnection).Will(Return.Value(mockDbDataReader));
			Expect.On(mockDbParameter0).One.GetProperty(x => x.Value).Will(Return.Value(1));
			Expect.On(mockDbParameter1).One.GetProperty(x => x.Value).Will(Return.Value(null));
			Expect.On(mockDbParameter1).One.SetProperty(x => x.Value).To(DBNull.Value);
			Expect.On(mockDbParameterCollection).One.Method(x => x.Add(_unusedDbParameter)).With(mockDbParameter0).Will(Return.Value(0));
			Expect.On(mockDbParameterCollection).One.Method(x => x.Add(_unusedDbParameter)).With(mockDbParameter1).Will(Return.Value(0));
			Expect.On(mockDbDataReader).One.Method(x => x.Dispose());

			adoNetYieldingFascade = new AdoNetYieldingFascade(mockReflectionFascade);

			dbDataReader = adoNetYieldingFascade.ExecuteReader(mockDbConnection, null, CommandType.StoredProcedure, "blah blah blah", mockDbParameters, CommandBehavior.CloseConnection, null, false);

			Assert.IsNotNull(dbDataReader);

			dbDataReader.Dispose();

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldExecuteReaderCloseConnectionTextNoParametersTest()
		{
			IAdoNetYieldingFascade adoNetYieldingFascade;
			MockFactory mockFactory;
			DbConnection mockDbConnection;
			DbCommand mockDbCommand;
			DbDataReader mockDbDataReader;
			IReflectionFascade mockReflectionFascade;

			DbDataReader dbDataReader;

			CommandBehavior _unusedCommandBehavior = CommandBehavior.Default;

			mockFactory = new MockFactory();
			mockDbConnection = mockFactory.CreateInstance<DbConnection>();
			mockDbCommand = mockFactory.CreateInstance<DbCommand>();
			mockDbDataReader = mockFactory.CreateInstance<DbDataReader>();
			mockReflectionFascade = mockFactory.CreateInstance<IReflectionFascade>();

			Expect.On(mockDbConnection).One.Method(x => x.CreateCommand()).WithNoArguments().Will(Return.Value(mockDbCommand));
			Expect.On(mockDbCommand).One.Method(x => x.Dispose());
			Expect.On(mockDbCommand).One.SetProperty(x => x.CommandType).To(CommandType.Text);
			Expect.On(mockDbCommand).One.SetProperty(x => x.CommandText).To("blah blah blah");
			Expect.On(mockDbCommand).One.SetProperty(x => x.Transaction).To(null);
			Expect.On(mockDbCommand).One.Method(x => x.ExecuteReader(_unusedCommandBehavior)).With(CommandBehavior.CloseConnection).Will(Return.Value(mockDbDataReader));
			Expect.On(mockDbDataReader).One.Method(x => x.Dispose());
			Expect.On(mockDbCommand).One.SetProperty(x => x.CommandTimeout).To(15);
			Expect.On(mockDbCommand).One.Method(x => x.Prepare());

			adoNetYieldingFascade = new AdoNetYieldingFascade(mockReflectionFascade);

			dbDataReader = adoNetYieldingFascade.ExecuteReader(mockDbConnection, null, CommandType.Text, "blah blah blah", null, CommandBehavior.CloseConnection, 15, true);

			Assert.IsNotNull(dbDataReader);

			dbDataReader.Dispose();

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldExecuteReaderCloseConnectionTextNoParametersWithTransactionTest()
		{
			IAdoNetYieldingFascade adoNetYieldingFascade;
			MockFactory mockFactory;
			DbConnection mockDbConnection;
			DbCommand mockDbCommand;
			DbDataReader mockDbDataReader;
			DbTransaction mockDbTransaction;
			IReflectionFascade mockReflectionFascade;

			DbDataReader dbDataReader;

			CommandBehavior _unusedCommandBehavior = CommandBehavior.Default;

			mockFactory = new MockFactory();
			mockDbConnection = mockFactory.CreateInstance<DbConnection>();
			mockDbCommand = mockFactory.CreateInstance<DbCommand>();
			mockDbDataReader = mockFactory.CreateInstance<DbDataReader>();
			mockDbTransaction = mockFactory.CreateInstance<DbTransaction>();
			mockReflectionFascade = mockFactory.CreateInstance<IReflectionFascade>();

			Expect.On(mockDbConnection).One.Method(x => x.CreateCommand()).WithNoArguments().Will(Return.Value(mockDbCommand));
			Expect.On(mockDbCommand).One.Method(x => x.Dispose());
			Expect.On(mockDbCommand).One.SetProperty(x => x.CommandType).To(CommandType.Text);
			Expect.On(mockDbCommand).One.SetProperty(x => x.CommandText).To("blah blah blah");
			Expect.On(mockDbCommand).One.SetProperty(x => x.Transaction).To(mockDbTransaction);
			Expect.On(mockDbCommand).One.Method(x => x.ExecuteReader(_unusedCommandBehavior)).With(CommandBehavior.CloseConnection).Will(Return.Value(mockDbDataReader));
			Expect.On(mockDbDataReader).One.Method(x => x.Dispose());
			Expect.On(mockDbCommand).One.SetProperty(x => x.CommandTimeout).To(15);
			Expect.On(mockDbCommand).One.Method(x => x.Prepare());

			adoNetYieldingFascade = new AdoNetYieldingFascade(mockReflectionFascade);

			dbDataReader = adoNetYieldingFascade.ExecuteReader(mockDbConnection, mockDbTransaction, CommandType.Text, "blah blah blah", null, CommandBehavior.CloseConnection, 15, true);

			Assert.IsNotNull(dbDataReader);

			dbDataReader.Dispose();

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldExecuteReaderNoCloseConnectionSprocWithParametersTest()
		{
			IAdoNetYieldingFascade adoNetYieldingFascade;
			MockFactory mockFactory;
			DbConnection mockDbConnection;
			DbCommand mockDbCommand;
			IDataParameterCollection mockDbParameterCollection;
			DbParameter[] mockDbParameters;
			DbParameter mockDbParameter0;
			DbParameter mockDbParameter1;
			DbDataReader mockDbDataReader;
			IReflectionFascade mockReflectionFascade;

			DbDataReader dbDataReader;

			CommandBehavior _unusedCommandBehavior = CommandBehavior.Default;
			DbParameter _unusedDbParameter = null;

			mockFactory = new MockFactory();
			mockDbConnection = mockFactory.CreateInstance<DbConnection>();
			mockDbCommand = mockFactory.CreateInstance<DbCommand>();
			mockDbParameterCollection = mockFactory.CreateInstance<IDataParameterCollection>();
			mockDbParameter0 = mockFactory.CreateInstance<DbParameter>();
			mockDbParameter1 = mockFactory.CreateInstance<DbParameter>();
			mockDbParameters = new DbParameter[] { mockDbParameter0, mockDbParameter1 };
			mockDbDataReader = mockFactory.CreateInstance<DbDataReader>();
			mockReflectionFascade = mockFactory.CreateInstance<IReflectionFascade>();

			Expect.On(mockDbConnection).One.Method(x => x.CreateCommand()).WithNoArguments().Will(Return.Value(mockDbCommand));
			Expect.On(mockDbCommand).Exactly(2).GetProperty(x => x.Parameters).Will(Return.Value(mockDbParameterCollection));
			Expect.On(mockDbCommand).One.Method(x => x.Dispose());
			Expect.On(mockDbCommand).One.SetProperty(x => x.CommandType).To(CommandType.StoredProcedure);
			Expect.On(mockDbCommand).One.SetProperty(x => x.CommandText).To("blah blah blah");
			Expect.On(mockDbCommand).One.SetProperty(x => x.Transaction).To(null);
			Expect.On(mockDbCommand).One.Method(x => x.ExecuteReader(_unusedCommandBehavior)).With(CommandBehavior.SingleRow).Will(Return.Value(mockDbDataReader));
			Expect.On(mockDbParameter0).One.GetProperty(x => x.Value).Will(Return.Value(1));
			Expect.On(mockDbParameter1).One.GetProperty(x => x.Value).Will(Return.Value(null));
			Expect.On(mockDbParameter1).One.SetProperty(x => x.Value).To(DBNull.Value);
			Expect.On(mockDbParameterCollection).One.Method(x => x.Add(_unusedDbParameter)).With(mockDbParameter0).Will(Return.Value(0));
			Expect.On(mockDbParameterCollection).One.Method(x => x.Add(_unusedDbParameter)).With(mockDbParameter1).Will(Return.Value(0));
			Expect.On(mockDbDataReader).One.Method(x => x.Dispose());

			adoNetYieldingFascade = new AdoNetYieldingFascade(mockReflectionFascade);

			dbDataReader = adoNetYieldingFascade.ExecuteReader(mockDbConnection, null, CommandType.StoredProcedure, "blah blah blah", mockDbParameters, CommandBehavior.SingleRow, null, false);

			Assert.IsNotNull(dbDataReader);

			dbDataReader.Dispose();

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldExecuteReaderNoCloseConnectionTextNoParametersTest()
		{
			IAdoNetYieldingFascade adoNetYieldingFascade;
			MockFactory mockFactory;
			DbConnection mockDbConnection;
			DbCommand mockDbCommand;
			DbDataReader mockDbDataReader;
			IReflectionFascade mockReflectionFascade;

			DbDataReader dbDataReader;

			CommandBehavior _unusedCommandBehavior = CommandBehavior.Default;

			mockFactory = new MockFactory();
			mockDbConnection = mockFactory.CreateInstance<DbConnection>();
			mockDbCommand = mockFactory.CreateInstance<DbCommand>();
			mockDbDataReader = mockFactory.CreateInstance<DbDataReader>();
			mockReflectionFascade = mockFactory.CreateInstance<IReflectionFascade>();

			Expect.On(mockDbConnection).One.Method(x => x.CreateCommand()).WithNoArguments().Will(Return.Value(mockDbCommand));
			Expect.On(mockDbCommand).One.Method(x => x.Dispose());
			Expect.On(mockDbCommand).One.SetProperty(x => x.CommandType).To(CommandType.Text);
			Expect.On(mockDbCommand).One.SetProperty(x => x.CommandText).To("blah blah blah");
			Expect.On(mockDbCommand).One.SetProperty(x => x.Transaction).To(null);
			Expect.On(mockDbCommand).One.Method(x => x.ExecuteReader(_unusedCommandBehavior)).With(CommandBehavior.SingleRow).Will(Return.Value(mockDbDataReader));
			Expect.On(mockDbDataReader).One.Method(x => x.Dispose());

			adoNetYieldingFascade = new AdoNetYieldingFascade(mockReflectionFascade);

			dbDataReader = adoNetYieldingFascade.ExecuteReader(mockDbConnection, null, CommandType.Text, "blah blah blah", null, CommandBehavior.SingleRow, null, false);

			Assert.IsNotNull(dbDataReader);

			dbDataReader.Dispose();

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