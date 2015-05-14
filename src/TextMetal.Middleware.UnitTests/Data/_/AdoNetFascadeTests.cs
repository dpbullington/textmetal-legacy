/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Data;

using NMock;

using NUnit.Framework;

using TextMetal.Middleware.Common.Utilities;
using TextMetal.Middleware.Data;

namespace TextMetal.Middleware.UnitTests.Data._
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