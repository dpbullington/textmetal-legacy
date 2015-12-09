/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Data;

using NUnit.Framework;

namespace TextMetal.Middleware.UnitTests.Datazoid.UoW._
{
	[TestFixture]
	public class AmbientUnitOfWorkScopeTests
	{
		#region Constructors/Destructors

		public AmbientUnitOfWorkScopeTests()
		{
		}

		#endregion

		#region Methods/Operators

		[Test]
		public void ShouldAbortTest()
		{
			MockFactory mockFactory;
			DataSourceTransaction transaction;
			string mockConnectionString;
			IDbConnection mockDbConnection;
			IDbTransaction mockDbTransaction;

			mockFactory = new MockFactory();
			mockConnectionString = "db=local";
			mockDbConnection = mockFactory.CreateInstance<IDbConnection>();
			mockDbTransaction = mockFactory.CreateInstance<IDbTransaction>();

			Expect.AtLeastOnce.On(mockDbConnection).Method("Dispose").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbTransaction).Method("Dispose").WithNoArguments();

			transaction = new DataSourceTransaction();

			Assert.IsNotNull(transaction);

			Assert.IsNull(transaction.ConnectionString);
			Assert.IsNull(transaction.Connection);
			Assert.IsNull(transaction.Transaction);
			Assert.IsFalse(transaction.Bound);
			Assert.IsNull(transaction.Context);

			transaction.Bind(mockConnectionString, mockDbConnection, mockDbTransaction, null);

			Assert.IsNotNull(transaction.ConnectionString);
			Assert.IsNotNull(transaction.Connection);
			Assert.IsNotNull(transaction.Transaction);
			Assert.IsTrue(transaction.Bound);
			Assert.IsNull(transaction.Context);

			transaction.Dispose();

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldCommitTest()
		{
			MockFactory mockFactory;
			DataSourceTransaction transaction;
			string mockConnectionString;
			IDbConnection mockDbConnection;
			IDbTransaction mockDbTransaction;
			IDataSourceTransactionContext mockDataSourceTransactionContext;

			mockFactory = new MockFactory();
			mockConnectionString = "db=local";
			mockDbConnection = mockFactory.CreateInstance<IDbConnection>();
			mockDbTransaction = mockFactory.CreateInstance<IDbTransaction>();
			mockDataSourceTransactionContext = mockFactory.CreateInstance<IDataSourceTransactionContext>();

			Expect.AtLeastOnce.On(mockDbConnection).Method("Dispose").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbTransaction).Method("Commit").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbTransaction).Method("Dispose").WithNoArguments();
			Expect.AtLeastOnce.On(mockDataSourceTransactionContext).Method("Dispose").WithNoArguments();

			transaction = new DataSourceTransaction();

			Assert.IsNotNull(transaction);

			Assert.IsNull(transaction.ConnectionString);
			Assert.IsNull(transaction.Connection);
			Assert.IsNull(transaction.Transaction);
			Assert.IsFalse(transaction.Bound);
			Assert.IsNull(transaction.Context);

			transaction.Bind(mockConnectionString, mockDbConnection, mockDbTransaction, mockDataSourceTransactionContext);

			Assert.IsNotNull(transaction.ConnectionString);
			Assert.IsNotNull(transaction.Connection);
			Assert.IsNotNull(transaction.Transaction);
			Assert.IsTrue(transaction.Bound);
			Assert.IsNotNull(transaction.Context);

			transaction.Commit();
			transaction.Dispose();

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldCreateTest()
		{
			DataSourceTransaction transaction;

			transaction = new DataSourceTransaction();

			Assert.IsNotNull(transaction);

			Assert.IsNull(transaction.ConnectionString);
			Assert.IsNull(transaction.Connection);
			Assert.IsNull(transaction.Transaction);
			Assert.IsFalse(transaction.Bound);
			Assert.IsNull(transaction.Context);
			Assert.AreEqual(IsolationLevel.Unspecified, transaction.IsolationLevel);

			transaction.Dispose();

			Assert.IsTrue(transaction.Adjudicated);
			Assert.IsTrue(transaction.Disposed);

			transaction = new DataSourceTransaction(IsolationLevel.Serializable);

			Assert.IsNotNull(transaction);
			Assert.AreEqual(IsolationLevel.Serializable, transaction.IsolationLevel);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void ShouldFailOnAlreadyAdjudicatedBindTest()
		{
			MockFactory mockFactory;
			DataSourceTransaction transaction;
			string mockConnectionString;
			IDbConnection mockDbConnection;
			IDbTransaction mockDbTransaction;

			mockFactory = new MockFactory();
			mockConnectionString = "db=local";
			mockDbConnection = mockFactory.CreateInstance<IDbConnection>();
			mockDbTransaction = mockFactory.CreateInstance<IDbTransaction>();

			transaction = new DataSourceTransaction();

			Assert.IsNotNull(transaction);

			Assert.IsNull(transaction.ConnectionString);
			Assert.IsNull(transaction.Connection);
			Assert.IsNull(transaction.Transaction);
			Assert.IsFalse(transaction.Bound);
			Assert.IsNull(transaction.Context);

			transaction.Commit();
			transaction.Bind(mockConnectionString, mockDbConnection, mockDbTransaction, null);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void ShouldFailOnAlreadyAdjudicatedTest()
		{
			MockFactory mockFactory;
			DataSourceTransaction transaction;
			string mockConnectionString;
			IDbConnection mockDbConnection;
			IDbTransaction mockDbTransaction;

			mockFactory = new MockFactory();
			mockConnectionString = "db=local";
			mockDbConnection = mockFactory.CreateInstance<IDbConnection>();
			mockDbTransaction = mockFactory.CreateInstance<IDbTransaction>();

			Expect.AtLeastOnce.On(mockDbConnection).Method("Dispose").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbTransaction).Method("Rollback").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbTransaction).Method("Dispose").WithNoArguments();

			transaction = new DataSourceTransaction();

			Assert.IsNotNull(transaction);

			Assert.IsNull(transaction.ConnectionString);
			Assert.IsNull(transaction.Connection);
			Assert.IsNull(transaction.Transaction);
			Assert.IsFalse(transaction.Bound);
			Assert.IsNull(transaction.Context);

			transaction.Bind(mockConnectionString, mockDbConnection, mockDbTransaction, null);

			Assert.IsNotNull(transaction.ConnectionString);
			Assert.IsNotNull(transaction.Connection);
			Assert.IsNotNull(transaction.Transaction);
			Assert.IsTrue(transaction.Bound);
			Assert.IsNull(transaction.Context);

			transaction.Rollback();
			transaction.Commit();
			transaction.Dispose();
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void ShouldFailOnAlreadyBoundBindTest()
		{
			MockFactory mockFactory;
			DataSourceTransaction transaction;
			string mockConnectionString;
			IDbConnection mockDbConnection;
			IDbTransaction mockDbTransaction;

			mockFactory = new MockFactory();
			mockConnectionString = "db=local";
			mockDbConnection = mockFactory.CreateInstance<IDbConnection>();
			mockDbTransaction = mockFactory.CreateInstance<IDbTransaction>();

			transaction = new DataSourceTransaction();

			Assert.IsNotNull(transaction);

			Assert.IsNull(transaction.ConnectionString);
			Assert.IsNull(transaction.Connection);
			Assert.IsNull(transaction.Transaction);
			Assert.IsFalse(transaction.Bound);
			Assert.IsNull(transaction.Context);

			transaction.Bind(mockConnectionString, mockDbConnection, mockDbTransaction, null);
			transaction.Bind(mockConnectionString, mockDbConnection, mockDbTransaction, null);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void ShouldFailOnAlreadyContextSetBindTest()
		{
			MockFactory mockFactory;
			DataSourceTransaction transaction;
			IDataSourceTransactionContext mockDataSourceTransactionContext;

			mockFactory = new MockFactory();
			mockDataSourceTransactionContext = mockFactory.CreateInstance<IDataSourceTransactionContext>();

			transaction = new DataSourceTransaction();

			Assert.IsNotNull(transaction);

			Assert.IsNull(transaction.ConnectionString);
			Assert.IsNull(transaction.Connection);
			Assert.IsNull(transaction.Transaction);
			Assert.IsFalse(transaction.Bound);
			Assert.IsNull(transaction.Context);

			transaction.Context = mockDataSourceTransactionContext;

			Assert.IsNotNull(transaction.Context);

			transaction.Context = null; // ok

			transaction.Context = mockDataSourceTransactionContext;

			mockDataSourceTransactionContext = mockFactory.CreateInstance<IDataSourceTransactionContext>();
			transaction.Context = mockDataSourceTransactionContext; // not ok
		}

		[Test]
		[ExpectedException(typeof(ObjectDisposedException))]
		public void ShouldFailOnAlreadyDisposedAdjudicateTest()
		{
			MockFactory mockFactory;
			DataSourceTransaction transaction;
			string mockConnectionString;
			IDbConnection mockDbConnection;
			IDbTransaction mockDbTransaction;

			mockFactory = new MockFactory();
			mockConnectionString = "db=local";
			mockDbConnection = mockFactory.CreateInstance<IDbConnection>();
			mockDbTransaction = mockFactory.CreateInstance<IDbTransaction>();

			Expect.AtLeastOnce.On(mockDbConnection).Method("Dispose").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbTransaction).Method("Rollback").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbTransaction).Method("Dispose").WithNoArguments();

			transaction = new DataSourceTransaction();

			Assert.IsNotNull(transaction);

			Assert.IsNull(transaction.ConnectionString);
			Assert.IsNull(transaction.Connection);
			Assert.IsNull(transaction.Transaction);
			Assert.IsFalse(transaction.Bound);
			Assert.IsNull(transaction.Context);

			transaction.Bind(mockConnectionString, mockDbConnection, mockDbTransaction, null);

			Assert.IsNotNull(transaction.ConnectionString);
			Assert.IsNotNull(transaction.Connection);
			Assert.IsNotNull(transaction.Transaction);
			Assert.IsTrue(transaction.Bound);
			Assert.IsNull(transaction.Context);

			transaction.Dispose();
			transaction.Rollback();
		}

		[Test]
		[ExpectedException(typeof(ObjectDisposedException))]
		public void ShouldFailOnAlreadyDisposedBindTest()
		{
			MockFactory mockFactory;
			DataSourceTransaction transaction;
			string mockConnectionString;
			IDbConnection mockDbConnection;
			IDbTransaction mockDbTransaction;

			mockFactory = new MockFactory();
			mockConnectionString = "db=local";
			mockDbConnection = mockFactory.CreateInstance<IDbConnection>();
			mockDbTransaction = mockFactory.CreateInstance<IDbTransaction>();

			transaction = new DataSourceTransaction();

			Assert.IsNotNull(transaction);

			Assert.IsNull(transaction.ConnectionString);
			Assert.IsNull(transaction.Connection);
			Assert.IsNull(transaction.Transaction);
			Assert.IsFalse(transaction.Bound);
			Assert.IsNull(transaction.Context);

			transaction.Dispose();
			transaction.Bind(mockConnectionString, mockDbConnection, mockDbTransaction, null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullConnectionBindTest()
		{
			MockFactory mockFactory;
			DataSourceTransaction transaction;
			string mockConnectionString;
			IDbConnection mockDbConnection;
			IDbTransaction mockDbTransaction;

			mockFactory = new MockFactory();
			mockConnectionString = "db=local";
			mockDbConnection = null;
			mockDbTransaction = mockFactory.CreateInstance<IDbTransaction>();

			transaction = new DataSourceTransaction();

			Assert.IsNotNull(transaction);

			Assert.IsNull(transaction.ConnectionString);
			Assert.IsNull(transaction.Connection);
			Assert.IsNull(transaction.Transaction);
			Assert.IsFalse(transaction.Bound);
			Assert.IsNull(transaction.Context);

			transaction.Bind(mockConnectionString, mockDbConnection, mockDbTransaction, null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullConnectionStringBindTest()
		{
			MockFactory mockFactory;
			DataSourceTransaction transaction;
			string mockConnectionString;
			IDbConnection mockDbConnection;
			IDbTransaction mockDbTransaction;

			mockFactory = new MockFactory();
			mockConnectionString = null;
			mockDbConnection = mockFactory.CreateInstance<IDbConnection>();
			mockDbTransaction = mockFactory.CreateInstance<IDbTransaction>();

			transaction = new DataSourceTransaction();

			Assert.IsNotNull(transaction);

			Assert.IsNull(transaction.ConnectionString);
			Assert.IsNull(transaction.Connection);
			Assert.IsNull(transaction.Transaction);
			Assert.IsFalse(transaction.Bound);
			Assert.IsNull(transaction.Context);

			transaction.Bind(mockConnectionString, mockDbConnection, mockDbTransaction, null);
		}

		[Test]
		public void ShouldNotFailOnDoubleDisposeTest()
		{
			MockFactory mockFactory;
			DataSourceTransaction transaction;
			string mockConnectionString;
			IDbConnection mockDbConnection;
			IDbTransaction mockDbTransaction;

			mockFactory = new MockFactory();
			mockConnectionString = "db=local";
			mockDbConnection = mockFactory.CreateInstance<IDbConnection>();
			mockDbTransaction = mockFactory.CreateInstance<IDbTransaction>();

			Expect.AtLeastOnce.On(mockDbConnection).Method("Dispose").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbTransaction).Method("Dispose").WithNoArguments();

			transaction = new DataSourceTransaction();

			Assert.IsNotNull(transaction);

			Assert.IsNull(transaction.ConnectionString);
			Assert.IsNull(transaction.Connection);
			Assert.IsNull(transaction.Transaction);
			Assert.IsFalse(transaction.Bound);
			Assert.IsNull(transaction.Context);

			transaction.Bind(mockConnectionString, mockDbConnection, mockDbTransaction, null);

			Assert.IsNotNull(transaction.ConnectionString);
			Assert.IsNotNull(transaction.Connection);
			Assert.IsNotNull(transaction.Transaction);
			Assert.IsTrue(transaction.Bound);
			Assert.IsNull(transaction.Context);

			transaction.Dispose();
			transaction.Dispose();

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldRollbackTest()
		{
			MockFactory mockFactory;
			DataSourceTransaction transaction;
			string mockConnectionString;
			IDbConnection mockDbConnection;
			IDbTransaction mockDbTransaction;

			mockFactory = new MockFactory();
			mockConnectionString = "db=local";
			mockDbConnection = mockFactory.CreateInstance<IDbConnection>();
			mockDbTransaction = mockFactory.CreateInstance<IDbTransaction>();

			Expect.AtLeastOnce.On(mockDbConnection).Method("Dispose").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbTransaction).Method("Rollback").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbTransaction).Method("Dispose").WithNoArguments();

			transaction = new DataSourceTransaction();

			Assert.IsNotNull(transaction);

			Assert.IsNull(transaction.ConnectionString);
			Assert.IsNull(transaction.Connection);
			Assert.IsNull(transaction.Transaction);
			Assert.IsFalse(transaction.Bound);
			Assert.IsNull(transaction.Context);

			transaction.Bind(mockConnectionString, mockDbConnection, mockDbTransaction, null);

			Assert.IsNotNull(transaction.ConnectionString);
			Assert.IsNotNull(transaction.Connection);
			Assert.IsNotNull(transaction.Transaction);
			Assert.IsTrue(transaction.Bound);
			Assert.IsNull(transaction.Context);

			transaction.Rollback();
			transaction.Dispose();

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldSetAndGetAmbientTransactionTest()
		{
			DataSourceTransaction transaction;

			Assert.IsNull(DataSourceTransaction.Current);

			transaction = new DataSourceTransaction();
			DataSourceTransaction.FrameTransaction(transaction);

			Assert.IsNotNull(DataSourceTransaction.Current);
		}

		[SetUp]
		public void TestSetUp()
		{
			while (!DataSourceTransaction.UnwindTransaction())
				; // do nothing
		}

		[TearDown]
		public void TestTearDown()
		{
			while (!DataSourceTransaction.UnwindTransaction())
				; // do nothing
		}

		#endregion
	}
	using

System.Data;
	using System.Transactions;

	using NMock2;

	using NUnit.Framework;

	using SwIsHw.Data.AdoNet;
	using SwIsHw.Data.Transactions;
	using SwIsHw.Data.UnitTests.TestingInfrastructure;

	using IsolationLevel = System.Data.IsolationLevel;

	[TestFixture]
	public class AdoNetDataSourceTests
	{
		#region Constructors/Destructors

		public AdoNetDataSourceTests()
		{
		}

		#endregion

		#region Fields/Constants

		private const string MOCK_CONNECTION_STRING = "myConnectionString";

		#endregion

		#region Methods/Operators

		[Test]
		public void ShouldCreateParameterTest()
		{
			MockFactory mockFactory;
			AdoNetDataSource dataSource;
			IConnectionFactory mockConnectionFactory;
			IDbConnection mockDbConnection;
			IDbCommand mockDbCommand;

			IDbDataParameter p;
			IDbDataParameter mockDbParameter;

			mockFactory = new MockFactory();
			mockConnectionFactory = mockFactory.CreateInstance<IConnectionFactory>();
			mockDbConnection = mockFactory.CreateInstance<IDbConnection>();
			mockDbCommand = mockFactory.CreateInstance<IDbCommand>();
			mockDbParameter = mockFactory.CreateInstance<IDbDataParameter>();

			Expect.Once.On(mockConnectionFactory).Method("GetConnection").Will(Return.Value(mockDbConnection));
			Expect.Once.On(mockDbConnection).Method("CreateCommand").Will(Return.Value(mockDbCommand));
			Expect.Once.On(mockDbConnection).Method("Dispose");
			Expect.Once.On(mockDbCommand).Method("CreateParameter").Will(Return.Value(mockDbParameter));
			Expect.Once.On(mockDbCommand).Method("Dispose");

			Expect.Once.On(mockDbParameter).SetProperty("ParameterName").To("@bob");
			Expect.Once.On(mockDbParameter).SetProperty("Size").To(1);
			Expect.Once.On(mockDbParameter).SetProperty("Value").To("test");
			Expect.Once.On(mockDbParameter).SetProperty("Direction").To(ParameterDirection.Input);
			Expect.Once.On(mockDbParameter).SetProperty("DbType").To(DbType.String);
			Expect.Once.On(mockDbParameter).SetProperty("Precision").To((byte)2);
			Expect.Once.On(mockDbParameter).SetProperty("Scale").To((byte)3);
			//Expect.Once.On(mockDbParameter).SetProperty("IsNullable").To(true);

			Expect.Once.On(mockDbParameter).GetProperty("ParameterName").Will(Return.Value("@bob"));
			Expect.Once.On(mockDbParameter).GetProperty("Size").Will(Return.Value(1));
			Expect.Once.On(mockDbParameter).GetProperty("Value").Will(Return.Value("test"));
			Expect.Once.On(mockDbParameter).GetProperty("Direction").Will(Return.Value(ParameterDirection.Input));
			Expect.Once.On(mockDbParameter).GetProperty("DbType").Will(Return.Value(DbType.String));
			Expect.Once.On(mockDbParameter).GetProperty("Precision").Will(Return.Value((byte)2));
			Expect.Once.On(mockDbParameter).GetProperty("Scale").Will(Return.Value((byte)3));
			//Expect.Once.On(mockDbParameter).GetProperty("IsNullable").Will(Return.Value(true));

			dataSource = new AdoNetDataSource(MOCK_CONNECTION_STRING, mockConnectionFactory);

			p = dataSource.CreateParameter(null, ParameterDirection.Input, DbType.String, 1, 2, 3, true, "@bob", "test");

			Assert.IsNotNull(p);

			Assert.AreEqual(ParameterDirection.Input, p.Direction);
			Assert.AreEqual("@bob", p.ParameterName);
			Assert.AreEqual("test", p.Value);
			Assert.AreEqual(1, ((IDbDataParameter)p).Size);
			Assert.AreEqual(DbType.String, p.DbType);
			Assert.AreEqual((byte)2, ((IDbDataParameter)p).Precision);
			Assert.AreEqual((byte)3, ((IDbDataParameter)p).Scale);
			//Assert.AreEqual(true, ((IDbDataParameter)p).IsNullable);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldCreateTest()
		{
			MockFactory mockFactory;
			AdoNetDataSource dataSource;
			IConnectionFactory mockConnectionFactory;

			mockFactory = new MockFactory();
			mockConnectionFactory = mockFactory.CreateInstance<IConnectionFactory>();

			dataSource = new AdoNetDataSource(MOCK_CONNECTION_STRING, mockConnectionFactory);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldExecuteNonQuerySprocWithParametersTest()
		{
			MockFactory mockFactory;
			AdoNetDataSource dataSource;
			IConnectionFactory mockConnectionFactory;
			IDbConnection mockDbConnection;
			IDbCommand mockDbCommand;
			IDataParameterCollection mockDbDataParameterCollection;
			IDbDataParameter[] mockDbDataParameters;
			IDbDataParameter mockDbDataParameter0;
			IDbDataParameter mockDbDataParameter1;

			int recordsAffected;

			mockFactory = new MockFactory();
			mockConnectionFactory = mockFactory.CreateInstance<IConnectionFactory>();
			mockDbConnection = mockFactory.CreateInstance<IDbConnection>();
			mockDbCommand = mockFactory.CreateInstance<IDbCommand>();
			mockDbDataParameterCollection = mockFactory.CreateInstance<IDataParameterCollection>();
			mockDbDataParameter0 = mockFactory.CreateInstance<IDbDataParameter>();
			mockDbDataParameter1 = mockFactory.CreateInstance<IDbDataParameter>();
			mockDbDataParameters = new IDbDataParameter[] { mockDbDataParameter0, mockDbDataParameter1 };

			Expect.Once.On(mockConnectionFactory).Method("GetConnection").Will(Return.Value(mockDbConnection));
			//Expect.AtLeastOnce.On(mockDbConnection).GetProperty("State").Will(Return.Value(ConnectionState.Open));
			Expect.AtLeastOnce.On(mockDbConnection).Method("Dispose").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbConnection).SetProperty("ConnectionString").To("myConnectionString");
			Expect.AtLeastOnce.On(mockDbConnection).Method("Open").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbConnection).Method("CreateCommand").WithNoArguments().Will(Return.Value(mockDbCommand));
			Expect.AtLeastOnce.On(mockDbCommand).GetProperty("Parameters").Will(Return.Value(mockDbDataParameterCollection));
			Expect.AtLeastOnce.On(mockDbDataParameterCollection).Method("Clear").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbCommand).Method("Dispose").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("Connection").To(mockDbConnection);
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("CommandType").To(CommandType.StoredProcedure);
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("CommandText").To("blah blah blah");
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("Transaction");
			Expect.AtLeastOnce.On(mockDbCommand).Method("ExecuteNonQuery").WithNoArguments().Will(Return.Value(1));

			Expect.AtLeastOnce.On(mockDbDataParameter0).GetProperty("Value").Will(Return.Value(1));
			Expect.AtLeastOnce.On(mockDbDataParameter1).GetProperty("Value").Will(Return.Value(null));
			Expect.AtLeastOnce.On(mockDbDataParameter1).SetProperty("Value").To(DBNull.Value);
			Expect.AtLeastOnce.On(mockDbDataParameterCollection).Method("Add").With(mockDbDataParameter0).Will(Return.Value(0));
			Expect.AtLeastOnce.On(mockDbDataParameterCollection).Method("Add").With(mockDbDataParameter1).Will(Return.Value(0));

			dataSource = new AdoNetDataSource(MOCK_CONNECTION_STRING, mockConnectionFactory);

			recordsAffected = dataSource.ExecuteNonQuery(CommandType.StoredProcedure, "blah blah blah", mockDbDataParameters, null, false);

			Assert.AreEqual(1, recordsAffected);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldExecuteNonQueryTextNoParametersTest()
		{
			MockFactory mockFactory;
			AdoNetDataSource dataSource;
			IConnectionFactory mockConnectionFactory;
			IDbConnection mockDbConnection;
			IDbCommand mockDbCommand;
			IDataParameterCollection mockDbDataParameterCollection;
			int recordsAffected;

			mockFactory = new MockFactory();
			mockConnectionFactory = mockFactory.CreateInstance<IConnectionFactory>();
			mockDbConnection = mockFactory.CreateInstance<IDbConnection>();
			mockDbCommand = mockFactory.CreateInstance<IDbCommand>();
			mockDbDataParameterCollection = mockFactory.CreateInstance<IDataParameterCollection>();

			Expect.Once.On(mockConnectionFactory).Method("GetConnection").Will(Return.Value(mockDbConnection));
			//Expect.AtLeastOnce.On(mockDbConnection).GetProperty("State").Will(Return.Value(ConnectionState.Open));
			Expect.AtLeastOnce.On(mockDbConnection).Method("Dispose").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbConnection).SetProperty("ConnectionString").To("myConnectionString");
			Expect.AtLeastOnce.On(mockDbConnection).Method("Open").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbConnection).Method("CreateCommand").WithNoArguments().Will(Return.Value(mockDbCommand));
			Expect.AtLeastOnce.On(mockDbCommand).GetProperty("Parameters").Will(Return.Value(mockDbDataParameterCollection));
			Expect.AtLeastOnce.On(mockDbDataParameterCollection).Method("Clear").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbCommand).Method("Dispose").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("Connection").To(mockDbConnection);
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("CommandType").To(CommandType.Text);
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("CommandText").To("blah blah blah");
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("Transaction");
			Expect.AtLeastOnce.On(mockDbCommand).Method("ExecuteNonQuery").WithNoArguments().Will(Return.Value(1));
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("CommandTimeout").To(15);
			Expect.AtLeastOnce.On(mockDbCommand).Method("Prepare").WithNoArguments();

			dataSource = new AdoNetDataSource(MOCK_CONNECTION_STRING, mockConnectionFactory);

			recordsAffected = dataSource.ExecuteNonQuery(CommandType.Text, "blah blah blah", null, 15, true);

			Assert.AreEqual(1, recordsAffected);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldExecuteReaderCloseConnectionSprocWithParametersTest()
		{
			MockFactory mockFactory;
			AdoNetDataSource dataSource;
			IConnectionFactory mockConnectionFactory;
			IDbConnection mockDbConnection;
			IDbCommand mockDbCommand;
			IDataParameterCollection mockDbDataParameterCollection;
			IDbDataParameter[] mockDbDataParameters;
			IDbDataParameter mockDbDataParameter0;
			IDbDataParameter mockDbDataParameter1;
			IDataReader mockDataReader;

			IDataReader dataReader;

			mockFactory = new MockFactory();
			mockConnectionFactory = mockFactory.CreateInstance<IConnectionFactory>();
			mockDbConnection = mockFactory.CreateInstance<IDbConnection>();
			mockDbCommand = mockFactory.CreateInstance<IDbCommand>();
			mockDbDataParameterCollection = mockFactory.CreateInstance<IDataParameterCollection>();
			mockDbDataParameter0 = mockFactory.CreateInstance<IDbDataParameter>();
			mockDbDataParameter1 = mockFactory.CreateInstance<IDbDataParameter>();
			mockDbDataParameters = new IDbDataParameter[] { mockDbDataParameter0, mockDbDataParameter1 };
			mockDataReader = mockFactory.CreateInstance<IDataReader>();

			Expect.Once.On(mockConnectionFactory).Method("GetConnection").Will(Return.Value(mockDbConnection));
			//Expect.AtLeastOnce.On(mockDbConnection).GetProperty("State").Will(Return.Value(ConnectionState.Open));
			//Expect.AtLeastOnce.On(mockDbConnection).Method("Dispose").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbConnection).SetProperty("ConnectionString").To("myConnectionString");
			Expect.AtLeastOnce.On(mockDbConnection).Method("Open").WithNoArguments();
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

			dataSource = new AdoNetDataSource(MOCK_CONNECTION_STRING, mockConnectionFactory);

			dataReader = dataSource.ExecuteReader(CommandType.StoredProcedure, "blah blah blah", mockDbDataParameters, CommandBehavior.CloseConnection, null, false);

			Assert.IsNotNull(dataReader);

			dataReader.Dispose();

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldExecuteReaderCloseConnectionTextNoParametersTest()
		{
			MockFactory mockFactory;
			AdoNetDataSource dataSource;
			IConnectionFactory mockConnectionFactory;
			IDbConnection mockDbConnection;
			IDbCommand mockDbCommand;
			IDataParameterCollection mockDbDataParameterCollection;
			IDataReader mockDataReader;

			IDataReader dataReader;

			mockFactory = new MockFactory();
			mockConnectionFactory = mockFactory.CreateInstance<IConnectionFactory>();
			mockDbConnection = mockFactory.CreateInstance<IDbConnection>();
			mockDbCommand = mockFactory.CreateInstance<IDbCommand>();
			mockDbDataParameterCollection = mockFactory.CreateInstance<IDataParameterCollection>();
			mockDataReader = mockFactory.CreateInstance<IDataReader>();

			Expect.Once.On(mockConnectionFactory).Method("GetConnection").Will(Return.Value(mockDbConnection));
			//Expect.AtLeastOnce.On(mockDbConnection).GetProperty("State").Will(Return.Value(ConnectionState.Open));
			//Expect.AtLeastOnce.On(mockDbConnection).Method("Dispose").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbConnection).SetProperty("ConnectionString").To("myConnectionString");
			Expect.AtLeastOnce.On(mockDbConnection).Method("Open").WithNoArguments();
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

			dataSource = new AdoNetDataSource(MOCK_CONNECTION_STRING, mockConnectionFactory);

			dataReader = dataSource.ExecuteReader(CommandType.Text, "blah blah blah", null, CommandBehavior.CloseConnection, 15, true);

			Assert.IsNotNull(dataReader);

			dataReader.Dispose();

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldExecuteReaderNoCloseConnectionSprocWithParametersTest()
		{
			MockFactory mockFactory;
			AdoNetDataSource dataSource;
			IConnectionFactory mockConnectionFactory;
			IDbConnection mockDbConnection;
			IDbCommand mockDbCommand;
			IDataParameterCollection mockDbDataParameterCollection;
			IDbDataParameter[] mockDbDataParameters;
			IDbDataParameter mockDbDataParameter0;
			IDbDataParameter mockDbDataParameter1;
			IDataReader mockDataReader;

			IDataReader dataReader;

			mockFactory = new MockFactory();
			mockConnectionFactory = mockFactory.CreateInstance<IConnectionFactory>();
			mockDbConnection = mockFactory.CreateInstance<IDbConnection>();
			mockDbCommand = mockFactory.CreateInstance<IDbCommand>();
			mockDbDataParameterCollection = mockFactory.CreateInstance<IDataParameterCollection>();
			mockDbDataParameter0 = mockFactory.CreateInstance<IDbDataParameter>();
			mockDbDataParameter1 = mockFactory.CreateInstance<IDbDataParameter>();
			mockDbDataParameters = new IDbDataParameter[] { mockDbDataParameter0, mockDbDataParameter1 };
			mockDataReader = mockFactory.CreateInstance<IDataReader>();

			Expect.Once.On(mockConnectionFactory).Method("GetConnection").Will(Return.Value(mockDbConnection));
			//Expect.AtLeastOnce.On(mockDbConnection).GetProperty("State").Will(Return.Value(ConnectionState.Open));
			Expect.AtLeastOnce.On(mockDbConnection).Method("Dispose").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbConnection).SetProperty("ConnectionString").To("myConnectionString");
			Expect.AtLeastOnce.On(mockDbConnection).Method("Open").WithNoArguments();
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

			dataSource = new AdoNetDataSource(MOCK_CONNECTION_STRING, mockConnectionFactory);

			dataReader = dataSource.ExecuteReader(CommandType.StoredProcedure, "blah blah blah", mockDbDataParameters, CommandBehavior.SingleRow, null, false);

			Assert.IsNotNull(dataReader);

			dataReader.Dispose();

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldExecuteReaderNoCloseConnectionTextNoParametersTest()
		{
			MockFactory mockFactory;
			AdoNetDataSource dataSource;
			IConnectionFactory mockConnectionFactory;
			IDbConnection mockDbConnection;
			IDbCommand mockDbCommand;
			IDataParameterCollection mockDbDataParameterCollection;
			IDataReader mockDataReader;

			IDataReader dataReader;

			mockFactory = new MockFactory();
			mockConnectionFactory = mockFactory.CreateInstance<IConnectionFactory>();
			mockDbConnection = mockFactory.CreateInstance<IDbConnection>();
			mockDbCommand = mockFactory.CreateInstance<IDbCommand>();
			mockDbDataParameterCollection = mockFactory.CreateInstance<IDataParameterCollection>();
			mockDataReader = mockFactory.CreateInstance<IDataReader>();

			Expect.Once.On(mockConnectionFactory).Method("GetConnection").Will(Return.Value(mockDbConnection));
			//Expect.AtLeastOnce.On(mockDbConnection).GetProperty("State").Will(Return.Value(ConnectionState.Open));
			Expect.AtLeastOnce.On(mockDbConnection).Method("Dispose").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbConnection).SetProperty("ConnectionString").To("myConnectionString");
			Expect.AtLeastOnce.On(mockDbConnection).Method("Open").WithNoArguments();
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

			dataSource = new AdoNetDataSource(MOCK_CONNECTION_STRING, mockConnectionFactory);

			dataReader = dataSource.ExecuteReader(CommandType.Text, "blah blah blah", null, CommandBehavior.SingleRow, null, false);

			Assert.IsNotNull(dataReader);

			dataReader.Dispose();

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldExecuteScalarSprocWithParametersTest()
		{
			MockFactory mockFactory;
			AdoNetDataSource dataSource;
			IConnectionFactory mockConnectionFactory;
			IDbConnection mockDbConnection;
			IDbCommand mockDbCommand;
			IDataParameterCollection mockDbDataParameterCollection;
			IDbDataParameter[] mockDbDataParameters;
			IDbDataParameter mockDbDataParameter0;
			IDbDataParameter mockDbDataParameter1;

			object returnValue;

			mockFactory = new MockFactory();
			mockConnectionFactory = mockFactory.CreateInstance<IConnectionFactory>();
			mockDbConnection = mockFactory.CreateInstance<IDbConnection>();
			mockDbCommand = mockFactory.CreateInstance<IDbCommand>();
			mockDbDataParameterCollection = mockFactory.CreateInstance<IDataParameterCollection>();
			mockDbDataParameter0 = mockFactory.CreateInstance<IDbDataParameter>();
			mockDbDataParameter1 = mockFactory.CreateInstance<IDbDataParameter>();
			mockDbDataParameters = new IDbDataParameter[] { mockDbDataParameter0, mockDbDataParameter1 };

			Expect.Once.On(mockConnectionFactory).Method("GetConnection").Will(Return.Value(mockDbConnection));
			//Expect.AtLeastOnce.On(mockDbConnection).GetProperty("State").Will(Return.Value(ConnectionState.Open));
			Expect.AtLeastOnce.On(mockDbConnection).Method("Dispose").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbConnection).SetProperty("ConnectionString").To("myConnectionString");
			Expect.AtLeastOnce.On(mockDbConnection).Method("Open").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbConnection).Method("CreateCommand").WithNoArguments().Will(Return.Value(mockDbCommand));
			Expect.AtLeastOnce.On(mockDbCommand).GetProperty("Parameters").Will(Return.Value(mockDbDataParameterCollection));
			Expect.AtLeastOnce.On(mockDbDataParameterCollection).Method("Clear").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbCommand).Method("Dispose").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("Connection").To(mockDbConnection);
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("CommandType").To(CommandType.StoredProcedure);
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("CommandText").To("blah blah blah");
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("Transaction");
			Expect.AtLeastOnce.On(mockDbCommand).Method("ExecuteScalar").WithNoArguments().Will(Return.Value(1));

			Expect.AtLeastOnce.On(mockDbDataParameter0).GetProperty("Value").Will(Return.Value(1));
			Expect.AtLeastOnce.On(mockDbDataParameter1).GetProperty("Value").Will(Return.Value(null));
			Expect.AtLeastOnce.On(mockDbDataParameter1).SetProperty("Value").To(DBNull.Value);
			Expect.AtLeastOnce.On(mockDbDataParameterCollection).Method("Add").With(mockDbDataParameter0).Will(Return.Value(0));
			Expect.AtLeastOnce.On(mockDbDataParameterCollection).Method("Add").With(mockDbDataParameter1).Will(Return.Value(0));

			dataSource = new AdoNetDataSource(MOCK_CONNECTION_STRING, mockConnectionFactory);

			returnValue = dataSource.ExecuteScalar(CommandType.StoredProcedure, "blah blah blah", mockDbDataParameters, null, false);

			Assert.AreEqual(1, returnValue);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldExecuteScalarTextNoParametersTest()
		{
			MockFactory mockFactory;
			AdoNetDataSource dataSource;
			IConnectionFactory mockConnectionFactory;
			IDbConnection mockDbConnection;
			IDbCommand mockDbCommand;
			IDataParameterCollection mockDbDataParameterCollection;

			object returnValue;

			mockFactory = new MockFactory();
			mockConnectionFactory = mockFactory.CreateInstance<IConnectionFactory>();
			mockDbConnection = mockFactory.CreateInstance<IDbConnection>();
			mockDbCommand = mockFactory.CreateInstance<IDbCommand>();
			mockDbDataParameterCollection = mockFactory.CreateInstance<IDataParameterCollection>();

			Expect.Once.On(mockConnectionFactory).Method("GetConnection").Will(Return.Value(mockDbConnection));
			//Expect.AtLeastOnce.On(mockDbConnection).GetProperty("State").Will(Return.Value(ConnectionState.Open));
			Expect.AtLeastOnce.On(mockDbConnection).Method("Dispose").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbConnection).SetProperty("ConnectionString").To("myConnectionString");
			Expect.AtLeastOnce.On(mockDbConnection).Method("Open").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbConnection).Method("CreateCommand").WithNoArguments().Will(Return.Value(mockDbCommand));
			Expect.AtLeastOnce.On(mockDbCommand).GetProperty("Parameters").Will(Return.Value(mockDbDataParameterCollection));
			Expect.AtLeastOnce.On(mockDbDataParameterCollection).Method("Clear").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbCommand).Method("Dispose").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("Connection").To(mockDbConnection);
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("CommandType").To(CommandType.Text);
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("CommandText").To("blah blah blah");
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("Transaction");
			Expect.AtLeastOnce.On(mockDbCommand).Method("ExecuteScalar").WithNoArguments().Will(Return.Value(1));
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("CommandTimeout").To(15);
			Expect.AtLeastOnce.On(mockDbCommand).Method("Prepare").WithNoArguments();

			dataSource = new AdoNetDataSource(MOCK_CONNECTION_STRING, mockConnectionFactory);

			returnValue = dataSource.ExecuteScalar(CommandType.Text, "blah blah blah", null, 15, true);

			Assert.AreEqual(1, returnValue);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldExecuteUnderAmbientDataSourceTransactionAbortTest()
		{
			MockFactory mockFactory;
			AdoNetDataSource dataSource;
			IConnectionFactory mockConnectionFactory;
			IDbConnection mockDbConnection;
			IDbCommand mockDbCommand;
			IDataParameterCollection mockDbDataParameterCollection;
			IDbDataParameter[] mockDbDataParameters;
			IDbDataParameter mockDbDataParameter0;
			IDbDataParameter mockDbDataParameter1;
			IDbTransaction mockDbTransaction;

			int recordsAffected;

			mockFactory = new MockFactory();
			mockConnectionFactory = mockFactory.CreateInstance<IConnectionFactory>();
			mockDbConnection = mockFactory.CreateInstance<IDbConnection>();
			mockDbCommand = mockFactory.CreateInstance<IDbCommand>();
			mockDbDataParameterCollection = mockFactory.CreateInstance<IDataParameterCollection>();
			mockDbDataParameter0 = mockFactory.CreateInstance<IDbDataParameter>();
			mockDbDataParameter1 = mockFactory.CreateInstance<IDbDataParameter>();
			mockDbTransaction = mockFactory.CreateInstance<IDbTransaction>();
			mockDbDataParameters = new IDbDataParameter[] { mockDbDataParameter0, mockDbDataParameter1 };

			Expect.Once.On(mockConnectionFactory).Method("GetConnection").Will(Return.Value(mockDbConnection));
			Expect.Once.On(mockConnectionFactory).GetProperty("ConnectionType").Will(Return.Value(mockDbConnection.GetType()));
			//Expect.AtLeastOnce.On(mockDbConnection).GetProperty("State").Will(Return.Value(ConnectionState.Open));
			Expect.AtLeastOnce.On(mockDbConnection).Method("Dispose").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbConnection).SetProperty("ConnectionString").To("myConnectionString");
			Expect.AtLeastOnce.On(mockDbConnection).Method("Open").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbConnection).Method("CreateCommand").WithNoArguments().Will(Return.Value(mockDbCommand));
			Expect.AtLeastOnce.On(mockDbConnection).Method("BeginTransaction").With(IsolationLevel.Unspecified).Will(Return.Value(mockDbTransaction));
			Expect.AtLeastOnce.On(mockDbCommand).GetProperty("Parameters").Will(Return.Value(mockDbDataParameterCollection));
			Expect.AtLeastOnce.On(mockDbDataParameterCollection).Method("Clear").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbCommand).Method("Dispose").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("Connection").To(mockDbConnection);
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("CommandType").To(CommandType.StoredProcedure);
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("CommandText").To("blah blah blah");
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("Transaction");
			Expect.AtLeastOnce.On(mockDbCommand).Method("ExecuteNonQuery").WithNoArguments().Will(Return.Value(1));
			Expect.AtLeastOnce.On(mockDbTransaction).Method("Dispose").WithNoArguments();

			Expect.AtLeastOnce.On(mockDbDataParameter0).GetProperty("Value").Will(Return.Value(1));
			Expect.AtLeastOnce.On(mockDbDataParameter1).GetProperty("Value").Will(Return.Value(null));
			Expect.AtLeastOnce.On(mockDbDataParameter1).SetProperty("Value").To(DBNull.Value);
			Expect.AtLeastOnce.On(mockDbDataParameterCollection).Method("Add").With(mockDbDataParameter0).Will(Return.Value(0));
			Expect.AtLeastOnce.On(mockDbDataParameterCollection).Method("Add").With(mockDbDataParameter1).Will(Return.Value(0));

			using (DataSourceTransaction transaction = new DataSourceTransaction())
			{
				Assert.IsNull(DataSourceTransaction.Current);
				DataSourceTransaction.FrameTransaction(transaction);
				Assert.IsNotNull(DataSourceTransaction.Current);

				dataSource = new AdoNetDataSource(MOCK_CONNECTION_STRING, mockConnectionFactory);

				recordsAffected = dataSource.ExecuteNonQuery(CommandType.StoredProcedure, "blah blah blah", mockDbDataParameters, null, false);
			}

			Assert.AreEqual(1, recordsAffected);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldExecuteUnderAmbientDataSourceTransactionCompleteTest()
		{
			MockFactory mockFactory;
			AdoNetDataSource dataSource;
			IConnectionFactory mockConnectionFactory;
			IDbConnection mockDbConnection;
			IDbCommand mockDbCommand;
			IDataParameterCollection mockDbDataParameterCollection;
			IDbDataParameter[] mockDbDataParameters;
			IDbDataParameter mockDbDataParameter0;
			IDbDataParameter mockDbDataParameter1;
			IDbTransaction mockDbTransaction;

			int recordsAffected;

			mockFactory = new MockFactory();
			mockConnectionFactory = mockFactory.CreateInstance<IConnectionFactory>();
			mockDbConnection = mockFactory.CreateInstance<IDbConnection>();
			mockDbCommand = mockFactory.CreateInstance<IDbCommand>();
			mockDbDataParameterCollection = mockFactory.CreateInstance<IDataParameterCollection>();
			mockDbDataParameter0 = mockFactory.CreateInstance<IDbDataParameter>();
			mockDbDataParameter1 = mockFactory.CreateInstance<IDbDataParameter>();
			mockDbTransaction = mockFactory.CreateInstance<IDbTransaction>();
			mockDbDataParameters = new IDbDataParameter[] { mockDbDataParameter0, mockDbDataParameter1 };

			Expect.Once.On(mockConnectionFactory).Method("GetConnection").Will(Return.Value(mockDbConnection));
			Expect.Once.On(mockConnectionFactory).GetProperty("ConnectionType").Will(Return.Value(mockDbConnection.GetType()));
			//Expect.AtLeastOnce.On(mockDbConnection).GetProperty("State").Will(Return.Value(ConnectionState.Open));
			Expect.AtLeastOnce.On(mockDbConnection).Method("Dispose").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbConnection).SetProperty("ConnectionString").To("myConnectionString");
			Expect.AtLeastOnce.On(mockDbConnection).Method("Open").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbConnection).Method("CreateCommand").WithNoArguments().Will(Return.Value(mockDbCommand));
			Expect.AtLeastOnce.On(mockDbConnection).Method("BeginTransaction").With(IsolationLevel.Unspecified).Will(Return.Value(mockDbTransaction));
			Expect.AtLeastOnce.On(mockDbCommand).GetProperty("Parameters").Will(Return.Value(mockDbDataParameterCollection));
			Expect.AtLeastOnce.On(mockDbDataParameterCollection).Method("Clear").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbCommand).Method("Dispose").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("Connection").To(mockDbConnection);
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("CommandType").To(CommandType.StoredProcedure);
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("CommandText").To("blah blah blah");
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("Transaction");
			Expect.AtLeastOnce.On(mockDbCommand).Method("ExecuteNonQuery").WithNoArguments().Will(Return.Value(1));
			Expect.AtLeastOnce.On(mockDbTransaction).Method("Commit").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbTransaction).Method("Dispose").WithNoArguments();

			Expect.AtLeastOnce.On(mockDbDataParameter0).GetProperty("Value").Will(Return.Value(1));
			Expect.AtLeastOnce.On(mockDbDataParameter1).GetProperty("Value").Will(Return.Value(null));
			Expect.AtLeastOnce.On(mockDbDataParameter1).SetProperty("Value").To(DBNull.Value);
			Expect.AtLeastOnce.On(mockDbDataParameterCollection).Method("Add").With(mockDbDataParameter0).Will(Return.Value(0));
			Expect.AtLeastOnce.On(mockDbDataParameterCollection).Method("Add").With(mockDbDataParameter1).Will(Return.Value(0));

			using (DataSourceTransactionScope dsts = new DataSourceTransactionScope())
			{
				dataSource = new AdoNetDataSource(MOCK_CONNECTION_STRING, mockConnectionFactory);

				recordsAffected = dataSource.ExecuteNonQuery(CommandType.StoredProcedure, "blah blah blah", mockDbDataParameters, null, false);

				dsts.Complete();
			}

			Assert.AreEqual(1, recordsAffected);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldExecuteUnderAmbientDataSourceTransactionNotCompleteTest()
		{
			MockFactory mockFactory;
			AdoNetDataSource dataSource;
			IConnectionFactory mockConnectionFactory;
			IDbConnection mockDbConnection;
			IDbCommand mockDbCommand;
			IDataParameterCollection mockDbDataParameterCollection;
			IDbDataParameter[] mockDbDataParameters;
			IDbDataParameter mockDbDataParameter0;
			IDbDataParameter mockDbDataParameter1;
			IDbTransaction mockDbTransaction;

			int recordsAffected;

			mockFactory = new MockFactory();
			mockConnectionFactory = mockFactory.CreateInstance<IConnectionFactory>();
			mockDbConnection = mockFactory.CreateInstance<IDbConnection>();
			mockDbCommand = mockFactory.CreateInstance<IDbCommand>();
			mockDbDataParameterCollection = mockFactory.CreateInstance<IDataParameterCollection>();
			mockDbDataParameter0 = mockFactory.CreateInstance<IDbDataParameter>();
			mockDbDataParameter1 = mockFactory.CreateInstance<IDbDataParameter>();
			mockDbTransaction = mockFactory.CreateInstance<IDbTransaction>();
			mockDbDataParameters = new IDbDataParameter[] { mockDbDataParameter0, mockDbDataParameter1 };

			Expect.Once.On(mockConnectionFactory).Method("GetConnection").Will(Return.Value(mockDbConnection));
			Expect.AtLeastOnce.On(mockConnectionFactory).GetProperty("ConnectionType").Will(Return.Value(mockDbConnection.GetType()));
			//Expect.AtLeastOnce.On(mockDbConnection).GetProperty("State").Will(Return.Value(ConnectionState.Open));
			Expect.AtLeastOnce.On(mockDbConnection).Method("Dispose").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbConnection).SetProperty("ConnectionString").To("myConnectionString");
			Expect.AtLeastOnce.On(mockDbConnection).Method("Open").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbConnection).Method("CreateCommand").WithNoArguments().Will(Return.Value(mockDbCommand));
			Expect.AtLeastOnce.On(mockDbConnection).Method("BeginTransaction").With(IsolationLevel.Unspecified).Will(Return.Value(mockDbTransaction));
			Expect.AtLeastOnce.On(mockDbCommand).GetProperty("Parameters").Will(Return.Value(mockDbDataParameterCollection));
			Expect.AtLeastOnce.On(mockDbDataParameterCollection).Method("Clear").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbCommand).Method("Dispose").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("Connection").To(mockDbConnection);
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("CommandType").To(CommandType.StoredProcedure);
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("CommandText").To("blah blah blah");
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("Transaction");
			Expect.AtLeastOnce.On(mockDbCommand).Method("ExecuteNonQuery").WithNoArguments().Will(Return.Value(1));
			Expect.AtLeastOnce.On(mockDbTransaction).Method("Rollback").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbTransaction).Method("Dispose").WithNoArguments();

			Expect.AtLeastOnce.On(mockDbDataParameter0).GetProperty("Value").Will(Return.Value(1));
			Expect.AtLeastOnce.On(mockDbDataParameter1).GetProperty("Value").Will(Return.Value(null));
			Expect.AtLeastOnce.On(mockDbDataParameter1).SetProperty("Value").To(DBNull.Value);
			Expect.AtLeastOnce.On(mockDbDataParameterCollection).Method("Add").With(mockDbDataParameter0).Will(Return.Value(0));
			Expect.AtLeastOnce.On(mockDbDataParameterCollection).Method("Add").With(mockDbDataParameter1).Will(Return.Value(0));

			using (DataSourceTransactionScope dsts = new DataSourceTransactionScope())
			{
				dataSource = new AdoNetDataSource(MOCK_CONNECTION_STRING, mockConnectionFactory);

				recordsAffected = dataSource.ExecuteNonQuery(CommandType.StoredProcedure, "blah blah blah", mockDbDataParameters, null, false);

				recordsAffected = dataSource.ExecuteNonQuery(CommandType.StoredProcedure, "blah blah blah", mockDbDataParameters, null, false);
			}

			Assert.AreEqual(1, recordsAffected);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldExecuteUnderAmbientDistributedTransactionAndDataSourceTransactionCompleteTest()
		{
			MockFactory mockFactory;
			AdoNetDataSource dataSource;
			IConnectionFactory mockConnectionFactory;
			IDbConnection mockDbConnection;
			IDbCommand mockDbCommand;
			IDataParameterCollection mockDbDataParameterCollection;
			IDbDataParameter[] mockDbDataParameters;
			IDbDataParameter mockDbDataParameter0;
			IDbDataParameter mockDbDataParameter1;

			int recordsAffected;

			mockFactory = new MockFactory();
			mockConnectionFactory = mockFactory.CreateInstance<IConnectionFactory>();
			mockDbConnection = mockFactory.CreateInstance<IDbConnection>();
			mockDbCommand = mockFactory.CreateInstance<IDbCommand>();
			mockDbDataParameterCollection = mockFactory.CreateInstance<IDataParameterCollection>();
			mockDbDataParameter0 = mockFactory.CreateInstance<IDbDataParameter>();
			mockDbDataParameter1 = mockFactory.CreateInstance<IDbDataParameter>();
			mockDbDataParameters = new IDbDataParameter[] { mockDbDataParameter0, mockDbDataParameter1 };

			Expect.Once.On(mockConnectionFactory).Method("GetConnection").Will(Return.Value(mockDbConnection));
			Expect.Once.On(mockConnectionFactory).GetProperty("ConnectionType").Will(Return.Value(mockDbConnection.GetType()));
			//Expect.AtLeastOnce.On(mockDbConnection).GetProperty("State").Will(Return.Value(ConnectionState.Open));
			Expect.AtLeastOnce.On(mockDbConnection).Method("Dispose").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbConnection).SetProperty("ConnectionString").To("myConnectionString");
			Expect.AtLeastOnce.On(mockDbConnection).Method("Open").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbConnection).Method("CreateCommand").WithNoArguments().Will(Return.Value(mockDbCommand));
			Expect.AtLeastOnce.On(mockDbCommand).GetProperty("Parameters").Will(Return.Value(mockDbDataParameterCollection));
			Expect.AtLeastOnce.On(mockDbDataParameterCollection).Method("Clear").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbCommand).Method("Dispose").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("Connection").To(mockDbConnection);
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("CommandType").To(CommandType.StoredProcedure);
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("CommandText").To("blah blah blah");
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("Transaction");
			Expect.AtLeastOnce.On(mockDbCommand).Method("ExecuteNonQuery").WithNoArguments().Will(Return.Value(1));

			Expect.AtLeastOnce.On(mockDbDataParameter0).GetProperty("Value").Will(Return.Value(1));
			Expect.AtLeastOnce.On(mockDbDataParameter1).GetProperty("Value").Will(Return.Value(null));
			Expect.AtLeastOnce.On(mockDbDataParameter1).SetProperty("Value").To(DBNull.Value);
			Expect.AtLeastOnce.On(mockDbDataParameterCollection).Method("Add").With(mockDbDataParameter0).Will(Return.Value(0));
			Expect.AtLeastOnce.On(mockDbDataParameterCollection).Method("Add").With(mockDbDataParameter1).Will(Return.Value(0));

			using (TransactionScope ts = new TransactionScope())
			{
				using (DataSourceTransactionScope dsts = new DataSourceTransactionScope())
				{
					dataSource = new AdoNetDataSource(MOCK_CONNECTION_STRING, mockConnectionFactory);

					recordsAffected = dataSource.ExecuteNonQuery(CommandType.StoredProcedure, "blah blah blah", mockDbDataParameters, null, false);

					dsts.Complete();
					ts.Complete();
				}
			}

			Assert.AreEqual(1, recordsAffected);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void ShouldFailOnConnectionStringMismatchUnderAmbientDataSourceTransactionTest()
		{
			MockFactory mockFactory;
			AdoNetDataSource dataSource;
			IConnectionFactory mockConnectionFactory;
			IDbConnection mockDbConnection;
			IDbCommand mockDbCommand;
			IDataParameterCollection mockDbDataParameterCollection;
			IDbDataParameter[] mockDbDataParameters;
			IDbDataParameter mockDbDataParameter0;
			IDbDataParameter mockDbDataParameter1;
			IDbTransaction mockDbTransaction;
			DataSourceTransaction transaction;

			int recordsAffected;

			mockFactory = new MockFactory();
			mockConnectionFactory = mockFactory.CreateInstance<IConnectionFactory>();
			mockDbConnection = mockFactory.CreateInstance<IDbConnection>();
			mockDbCommand = mockFactory.CreateInstance<IDbCommand>();
			mockDbDataParameterCollection = mockFactory.CreateInstance<IDataParameterCollection>();
			mockDbDataParameter0 = mockFactory.CreateInstance<IDbDataParameter>();
			mockDbDataParameter1 = mockFactory.CreateInstance<IDbDataParameter>();
			mockDbTransaction = mockFactory.CreateInstance<IDbTransaction>();
			mockDbDataParameters = new IDbDataParameter[] { mockDbDataParameter0, mockDbDataParameter1 };

			Expect.Once.On(mockConnectionFactory).Method("GetConnection").Will(Return.Value(mockDbConnection));
			Expect.Once.On(mockConnectionFactory).GetProperty("ConnectionType").Will(Return.Value(mockDbConnection.GetType()));
			//Expect.AtLeastOnce.On(mockDbConnection).GetProperty("State").Will(Return.Value(ConnectionState.Open));
			Expect.AtLeastOnce.On(mockDbConnection).Method("Dispose").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbConnection).SetProperty("ConnectionString").To("myConnectionString");
			Expect.AtLeastOnce.On(mockDbConnection).Method("Open").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbConnection).Method("CreateCommand").WithNoArguments().Will(Return.Value(mockDbCommand));
			Expect.AtLeastOnce.On(mockDbConnection).Method("BeginTransaction").WithNoArguments().Will(Return.Value(mockDbTransaction));
			Expect.AtLeastOnce.On(mockDbCommand).GetProperty("Parameters").Will(Return.Value(mockDbDataParameterCollection));
			Expect.AtLeastOnce.On(mockDbDataParameterCollection).Method("Clear").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbCommand).Method("Dispose").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("Connection").To(mockDbConnection);
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("CommandType").To(CommandType.StoredProcedure);
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("CommandText").To("blah blah blah");
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("Transaction");
			Expect.AtLeastOnce.On(mockDbCommand).Method("ExecuteNonQuery").WithNoArguments().Will(Return.Value(1));
			Expect.AtLeastOnce.On(mockDbTransaction).Method("Commit").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbTransaction).Method("Dispose").WithNoArguments();

			Expect.AtLeastOnce.On(mockDbDataParameter0).GetProperty("Value").Will(Return.Value(1));
			Expect.AtLeastOnce.On(mockDbDataParameter1).GetProperty("Value").Will(Return.Value(null));
			Expect.AtLeastOnce.On(mockDbDataParameter1).SetProperty("Value").To(DBNull.Value);
			Expect.AtLeastOnce.On(mockDbDataParameterCollection).Method("Add").With(mockDbDataParameter0).Will(Return.Value(0));
			Expect.AtLeastOnce.On(mockDbDataParameterCollection).Method("Add").With(mockDbDataParameter1).Will(Return.Value(0));

			transaction = new DataSourceTransaction();

			Assert.IsFalse(transaction.Bound);

			Assert.IsNull(DataSourceTransaction.Current);
			DataSourceTransaction.FrameTransaction(transaction);
			Assert.IsNotNull(DataSourceTransaction.Current);

			transaction.Bind("xxx", mockDbConnection, mockDbTransaction, null);

			dataSource = new AdoNetDataSource(MOCK_CONNECTION_STRING, mockConnectionFactory);

			recordsAffected = dataSource.ExecuteNonQuery(CommandType.StoredProcedure, "blah blah blah", mockDbDataParameters, null, false);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void ShouldFailOnConnectionTypeMismatchUnderAmbientDataSourceTransactionTest()
		{
			MockFactory mockFactory;
			AdoNetDataSource dataSource;
			IConnectionFactory mockConnectionFactory;
			IDbConnection mockDbConnection;
			IDbCommand mockDbCommand;
			IDataParameterCollection mockDbDataParameterCollection;
			IDbDataParameter[] mockDbDataParameters;
			IDbDataParameter mockDbDataParameter0;
			IDbDataParameter mockDbDataParameter1;
			IDbTransaction mockDbTransaction;
			DataSourceTransaction transaction;

			int recordsAffected;

			mockFactory = new MockFactory();
			mockConnectionFactory = mockFactory.CreateInstance<IConnectionFactory>();
			mockDbConnection = mockFactory.CreateInstance<IDbConnection>();
			mockDbCommand = mockFactory.CreateInstance<IDbCommand>();
			mockDbDataParameterCollection = mockFactory.CreateInstance<IDataParameterCollection>();
			mockDbDataParameter0 = mockFactory.CreateInstance<IDbDataParameter>();
			mockDbDataParameter1 = mockFactory.CreateInstance<IDbDataParameter>();
			mockDbTransaction = mockFactory.CreateInstance<IDbTransaction>();
			mockDbDataParameters = new IDbDataParameter[] { mockDbDataParameter0, mockDbDataParameter1 };

			Expect.Once.On(mockConnectionFactory).Method("GetConnection").Will(Return.Value(mockDbConnection));
			Expect.Once.On(mockConnectionFactory).GetProperty("ConnectionType").Will(Return.Value(mockDbConnection.GetType()));
			//Expect.AtLeastOnce.On(mockDbConnection).GetProperty("State").Will(Return.Value(ConnectionState.Open));
			Expect.AtLeastOnce.On(mockDbConnection).Method("Dispose").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbConnection).SetProperty("ConnectionString").To("myConnectionString");
			Expect.AtLeastOnce.On(mockDbConnection).Method("Open").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbConnection).Method("CreateCommand").WithNoArguments().Will(Return.Value(mockDbCommand));
			Expect.AtLeastOnce.On(mockDbConnection).Method("BeginTransaction").WithNoArguments().Will(Return.Value(mockDbTransaction));
			Expect.AtLeastOnce.On(mockDbCommand).GetProperty("Parameters").Will(Return.Value(mockDbDataParameterCollection));
			Expect.AtLeastOnce.On(mockDbDataParameterCollection).Method("Clear").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbCommand).Method("Dispose").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("Connection").To(mockDbConnection);
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("CommandType").To(CommandType.StoredProcedure);
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("CommandText").To("blah blah blah");
			Expect.AtLeastOnce.On(mockDbCommand).SetProperty("Transaction");
			Expect.AtLeastOnce.On(mockDbCommand).Method("ExecuteNonQuery").WithNoArguments().Will(Return.Value(1));
			Expect.AtLeastOnce.On(mockDbTransaction).Method("Commit").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbTransaction).Method("Dispose").WithNoArguments();

			Expect.AtLeastOnce.On(mockDbDataParameter0).GetProperty("Value").Will(Return.Value(1));
			Expect.AtLeastOnce.On(mockDbDataParameter1).GetProperty("Value").Will(Return.Value(null));
			Expect.AtLeastOnce.On(mockDbDataParameter1).SetProperty("Value").To(DBNull.Value);
			Expect.AtLeastOnce.On(mockDbDataParameterCollection).Method("Add").With(mockDbDataParameter0).Will(Return.Value(0));
			Expect.AtLeastOnce.On(mockDbDataParameterCollection).Method("Add").With(mockDbDataParameter1).Will(Return.Value(0));

			transaction = new DataSourceTransaction();

			Assert.IsFalse(transaction.Bound);

			Assert.IsNull(DataSourceTransaction.Current);
			DataSourceTransaction.FrameTransaction(transaction);
			Assert.IsNotNull(DataSourceTransaction.Current);

			transaction.Bind(MOCK_CONNECTION_STRING, new MockConnection(), mockDbTransaction, null);

			dataSource = new AdoNetDataSource(MOCK_CONNECTION_STRING, mockConnectionFactory);

			recordsAffected = dataSource.ExecuteNonQuery(CommandType.StoredProcedure, "blah blah blah", mockDbDataParameters, null, false);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullConnectionFactoryCreateTest()
		{
			MockFactory mockFactory;
			AdoNetDataSource dataSource;
			IConnectionFactory mockConnectionFactory;

			mockFactory = new MockFactory();
			mockConnectionFactory = null;

			dataSource = new AdoNetDataSource(MOCK_CONNECTION_STRING, mockConnectionFactory);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void ShouldFailOnNullConnectionFromFactoryGetOpenConnectionTest()
		{
			MockFactory mockFactory;
			AdoNetDataSource dataSource;
			IConnectionFactory mockConnectionFactory;

			mockFactory = new MockFactory();
			mockConnectionFactory = mockFactory.CreateInstance<IConnectionFactory>();

			Expect.Once.On(mockConnectionFactory).Method("GetConnection").Will(Return.Value(null));

			dataSource = new AdoNetDataSource(MOCK_CONNECTION_STRING, mockConnectionFactory);

			dataSource.ExecuteNonQuery(CommandType.Text, "blah blah blah", null, null, false);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void ShouldFailOnNullConnectionGetConnectionFromFactoryTest()
		{
			MockFactory mockFactory;
			AdoNetDataSource dataSource;
			IConnectionFactory mockConnectionFactory;

			mockFactory = new MockFactory();
			mockConnectionFactory = mockFactory.CreateInstance<IConnectionFactory>();

			Expect.Once.On(mockConnectionFactory).Method("GetConnection").Will(Return.Value(null));

			dataSource = new AdoNetDataSource(MOCK_CONNECTION_STRING, mockConnectionFactory);

			dataSource.CreateParameter(null, ParameterDirection.Input, DbType.String, 1, 2, 3, true, "@bob", "test");
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullConnectionStringCreateTest()
		{
			MockFactory mockFactory;
			AdoNetDataSource dataSource;
			IConnectionFactory mockConnectionFactory;

			mockFactory = new MockFactory();
			mockConnectionFactory = mockFactory.CreateInstance<IConnectionFactory>();

			dataSource = new AdoNetDataSource(null, mockConnectionFactory);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullConnectionStringGetForTest()
		{
			AdoNetDataSource dataSource;

			dataSource = AdoNetDataSource.GetFor<MockConnection>(null);
		}

		[Test]
		public void ShouldGetForTest()
		{
			MockFactory mockFactory;
			AdoNetDataSource dataSource;

			mockFactory = new MockFactory();

			dataSource = AdoNetDataSource.GetFor<MockConnection>(MOCK_CONNECTION_STRING);

			Assert.IsNotNull(dataSource);
		}

		[SetUp]
		public void TestSetUp()
		{
			while (!DataSourceTransaction.UnwindTransaction())
				; // do nothing
		}

		[TearDown]
		public void TestTearDown()
		{
			while (!DataSourceTransaction.UnwindTransaction())
				; // do nothing
		}

		#endregion
	}
	using

System.Data;
	using System.Transactions;

	using NMock2;

	using NUnit.Framework;

	using SwIsHw.Data.AdoNet;
	using SwIsHw.Data.Transactions;
	using SwIsHw.Data.UnitTests.TestingInfrastructure;

	using IsolationLevel = System.Data.IsolationLevel;

	[TestFixture]
	public class AdoNetAmbientAwareTests
	{
		#region Constructors/Destructors

		public AdoNetAmbientAwareTests()
		{
		}

		#endregion

		#region Fields/Constants

		private const string MOCK_CONNECTION_STRING = "myConnectionString";

		#endregion

		#region Methods/Operators

		[Test]
		public void ShouldCreateTest()
		{
			MockFactory mockFactory;
			MockAdoNetAmbientAware ambientAware;
			IConnectionFactory mockConnectionFactory;

			mockFactory = new MockFactory();
			mockConnectionFactory = mockFactory.CreateInstance<IConnectionFactory>();

			ambientAware = new MockAdoNetAmbientAware(MOCK_CONNECTION_STRING, mockConnectionFactory);

			Assert.AreEqual(MOCK_CONNECTION_STRING, ambientAware.BypassConnectionString);
			Assert.AreEqual(mockConnectionFactory, ambientAware.BypassConnectionFactory);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void ShouldFailOnConnectionStringMismatchUnderAmbientDataSourceTransactionTest()
		{
			MockFactory mockFactory;
			MockAdoNetAmbientAware ambientAware;
			IConnectionFactory mockConnectionFactory;
			IDbConnection mockDbConnection;
			IDbTransaction mockDbTransaction;
			DataSourceTransaction transaction;

			IDbConnection dbConnection;
			IDbTransaction dbTransaction;

			mockFactory = new MockFactory();
			mockConnectionFactory = mockFactory.CreateInstance<IConnectionFactory>();
			mockDbConnection = mockFactory.CreateInstance<IDbConnection>();
			mockDbTransaction = mockFactory.CreateInstance<IDbTransaction>();

			Expect.Once.On(mockConnectionFactory).Method("GetConnection").Will(Return.Value(mockDbConnection));
			Expect.Once.On(mockConnectionFactory).GetProperty("ConnectionType").Will(Return.Value(mockDbConnection.GetType()));
			//Expect.AtLeastOnce.On(mockDbConnection).GetProperty("State").Will(Return.Value(ConnectionState.Open));
			Expect.AtLeastOnce.On(mockDbConnection).Method("Dispose").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbConnection).SetProperty("ConnectionString").To("myConnectionString");
			Expect.AtLeastOnce.On(mockDbConnection).Method("Open").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbConnection).Method("BeginTransaction").WithNoArguments().Will(Return.Value(mockDbTransaction));
			Expect.AtLeastOnce.On(mockDbTransaction).Method("Commit").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbTransaction).Method("Dispose").WithNoArguments();

			transaction = new DataSourceTransaction();

			Assert.IsFalse(transaction.Bound);

			Assert.IsNull(DataSourceTransaction.Current);
			DataSourceTransaction.FrameTransaction(transaction);
			Assert.IsNotNull(DataSourceTransaction.Current);

			transaction.Bind("xxx", mockDbConnection, mockDbTransaction, null);

			ambientAware = new MockAdoNetAmbientAware(MOCK_CONNECTION_STRING, mockConnectionFactory);

			ambientAware.BypassGetConnectionAndTransaction(out dbConnection, out dbTransaction);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void ShouldFailOnConnectionTypeMismatchUnderAmbientDataSourceTransactionTest()
		{
			MockFactory mockFactory;
			MockAdoNetAmbientAware ambientAware;
			IConnectionFactory mockConnectionFactory;
			IDbConnection mockDbConnection;
			IDbTransaction mockDbTransaction;
			DataSourceTransaction transaction;

			IDbConnection dbConnection;
			IDbTransaction dbTransaction;

			mockFactory = new MockFactory();
			mockConnectionFactory = mockFactory.CreateInstance<IConnectionFactory>();
			mockDbConnection = mockFactory.CreateInstance<IDbConnection>();
			mockDbTransaction = mockFactory.CreateInstance<IDbTransaction>();

			Expect.Once.On(mockConnectionFactory).Method("GetConnection").Will(Return.Value(mockDbConnection));
			Expect.Once.On(mockConnectionFactory).GetProperty("ConnectionType").Will(Return.Value(mockDbConnection.GetType()));
			//Expect.AtLeastOnce.On(mockDbConnection).GetProperty("State").Will(Return.Value(ConnectionState.Open));
			Expect.AtLeastOnce.On(mockDbConnection).Method("Dispose").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbConnection).SetProperty("ConnectionString").To("myConnectionString");
			Expect.AtLeastOnce.On(mockDbConnection).Method("Open").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbConnection).Method("BeginTransaction").WithNoArguments().Will(Return.Value(mockDbTransaction));
			Expect.AtLeastOnce.On(mockDbTransaction).Method("Commit").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbTransaction).Method("Dispose").WithNoArguments();

			transaction = new DataSourceTransaction();

			Assert.IsFalse(transaction.Bound);

			Assert.IsNull(DataSourceTransaction.Current);
			DataSourceTransaction.FrameTransaction(transaction);
			Assert.IsNotNull(DataSourceTransaction.Current);

			transaction.Bind(MOCK_CONNECTION_STRING, new MockConnection(), mockDbTransaction, null);

			ambientAware = new MockAdoNetAmbientAware(MOCK_CONNECTION_STRING, mockConnectionFactory);

			ambientAware.BypassGetConnectionAndTransaction(out dbConnection, out dbTransaction);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullConnectionFactoryCreateTest()
		{
			MockFactory mockFactory;
			MockAdoNetAmbientAware ambientAware;
			IConnectionFactory mockConnectionFactory;

			mockFactory = new MockFactory();
			mockConnectionFactory = null;

			ambientAware = new MockAdoNetAmbientAware(MOCK_CONNECTION_STRING, mockConnectionFactory);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void ShouldFailOnNullConnectionFromFactoryGetOpenConnectionTest()
		{
			MockFactory mockFactory;
			MockAdoNetAmbientAware ambientAware;
			IConnectionFactory mockConnectionFactory;
			IDbConnection dbConnection;
			IDbTransaction dbTransaction;

			mockFactory = new MockFactory();
			mockConnectionFactory = mockFactory.CreateInstance<IConnectionFactory>();

			Expect.Once.On(mockConnectionFactory).Method("GetConnection").Will(Return.Value(null));

			ambientAware = new MockAdoNetAmbientAware(MOCK_CONNECTION_STRING, mockConnectionFactory);

			ambientAware.BypassGetConnectionAndTransaction(out dbConnection, out dbTransaction);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void ShouldFailOnNullConnectionGetConnectionFromFactoryTest()
		{
			MockFactory mockFactory;
			MockAdoNetAmbientAware ambientAware;
			IConnectionFactory mockConnectionFactory;

			mockFactory = new MockFactory();
			mockConnectionFactory = mockFactory.CreateInstance<IConnectionFactory>();

			Expect.Once.On(mockConnectionFactory).Method("GetConnection").Will(Return.Value(null));

			ambientAware = new MockAdoNetAmbientAware(MOCK_CONNECTION_STRING, mockConnectionFactory);

			ambientAware.BypassGetConnection(false);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullConnectionStringCreateTest()
		{
			MockFactory mockFactory;
			MockAdoNetAmbientAware ambientAware;
			IConnectionFactory mockConnectionFactory;

			mockFactory = new MockFactory();
			mockConnectionFactory = mockFactory.CreateInstance<IConnectionFactory>();

			ambientAware = new MockAdoNetAmbientAware(null, mockConnectionFactory);
		}

		[Test]
		public void ShouldGetConnectionAndTransactionNotAmbientTest()
		{
			MockFactory mockFactory;
			MockAdoNetAmbientAware ambientAware;
			IConnectionFactory mockConnectionFactory;
			IDbConnection mockDbConnection;

			IDbConnection dbConnection;
			IDbTransaction dbTransaction;

			mockFactory = new MockFactory();
			mockConnectionFactory = mockFactory.CreateInstance<IConnectionFactory>();
			mockDbConnection = mockFactory.CreateInstance<IDbConnection>();

			Expect.Once.On(mockConnectionFactory).Method("GetConnection").Will(Return.Value(mockDbConnection));
			//Expect.AtLeastOnce.On(mockDbConnection).GetProperty("State").Will(Return.Value(ConnectionState.Open));
			Expect.AtLeastOnce.On(mockDbConnection).SetProperty("ConnectionString").To("myConnectionString");
			Expect.AtLeastOnce.On(mockDbConnection).Method("Open").WithNoArguments();

			ambientAware = new MockAdoNetAmbientAware(MOCK_CONNECTION_STRING, mockConnectionFactory);

			ambientAware.BypassGetConnectionAndTransaction(out dbConnection, out dbTransaction);

			Assert.IsNotNull(dbConnection);
			Assert.IsNull(dbTransaction);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldGetConnectionAndTransactionUnderAmbientDataSourceTransactionTest()
		{
			MockFactory mockFactory;
			MockAdoNetAmbientAware ambientAware;
			IConnectionFactory mockConnectionFactory;
			IDbConnection mockDbConnection;
			IDbTransaction mockDbTransaction;

			IDbConnection dbConnection;
			IDbTransaction dbTransaction;

			mockFactory = new MockFactory();
			mockConnectionFactory = mockFactory.CreateInstance<IConnectionFactory>();
			mockDbConnection = mockFactory.CreateInstance<IDbConnection>();
			mockDbTransaction = mockFactory.CreateInstance<IDbTransaction>();

			Expect.Once.On(mockConnectionFactory).Method("GetConnection").Will(Return.Value(mockDbConnection));
			Expect.AtLeastOnce.On(mockConnectionFactory).GetProperty("ConnectionType").Will(Return.Value(mockDbConnection.GetType()));
			//Expect.AtLeastOnce.On(mockDbConnection).GetProperty("State").Will(Return.Value(ConnectionState.Open));
			Expect.AtLeastOnce.On(mockDbConnection).Method("Dispose").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbConnection).SetProperty("ConnectionString").To("myConnectionString");
			Expect.AtLeastOnce.On(mockDbConnection).Method("Open").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbConnection).Method("BeginTransaction").With(IsolationLevel.Unspecified).Will(Return.Value(mockDbTransaction));
			Expect.AtLeastOnce.On(mockDbTransaction).Method("Rollback").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbTransaction).Method("Dispose").WithNoArguments();

			ambientAware = new MockAdoNetAmbientAware(MOCK_CONNECTION_STRING, mockConnectionFactory);

			Assert.True(ambientAware.IsShouldDisposeResources);

			using (DataSourceTransactionScope dsts = new DataSourceTransactionScope())
			{
				ambientAware.BypassGetConnectionAndTransaction(out dbConnection, out dbTransaction);

				Assert.IsFalse(ambientAware.IsShouldDisposeResources);

				Assert.IsNotNull(dbConnection);
				Assert.IsNotNull(dbTransaction);
			}
			Assert.True(ambientAware.IsShouldDisposeResources);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldGetConnectionAndTransactionUnderAmbientDistributedTransactionAndDataSourceTransactionTest()
		{
			MockFactory mockFactory;
			MockAdoNetAmbientAware ambientAware;
			IConnectionFactory mockConnectionFactory;
			IDbConnection mockDbConnection;

			IDbConnection dbConnection;
			IDbTransaction dbTransaction;

			mockFactory = new MockFactory();
			mockConnectionFactory = mockFactory.CreateInstance<IConnectionFactory>();
			mockDbConnection = mockFactory.CreateInstance<IDbConnection>();

			Expect.Once.On(mockConnectionFactory).Method("GetConnection").Will(Return.Value(mockDbConnection));
			Expect.Once.On(mockConnectionFactory).GetProperty("ConnectionType").Will(Return.Value(mockDbConnection.GetType()));
			//Expect.AtLeastOnce.On(mockDbConnection).GetProperty("State").Will(Return.Value(ConnectionState.Open));
			Expect.AtLeastOnce.On(mockDbConnection).Method("Dispose").WithNoArguments();
			Expect.AtLeastOnce.On(mockDbConnection).SetProperty("ConnectionString").To("myConnectionString");
			Expect.AtLeastOnce.On(mockDbConnection).Method("Open").WithNoArguments();

			ambientAware = new MockAdoNetAmbientAware(MOCK_CONNECTION_STRING, mockConnectionFactory);

			Assert.True(ambientAware.IsShouldDisposeResources);
			using (TransactionScope ts = new TransactionScope())
			{
				Assert.IsTrue(ambientAware.IsShouldDisposeResources);
				using (DataSourceTransactionScope dsts = new DataSourceTransactionScope())
				{
					Assert.IsFalse(ambientAware.IsShouldDisposeResources);
					ambientAware.BypassGetConnectionAndTransaction(out dbConnection, out dbTransaction);

					Assert.IsNotNull(dbConnection);
					Assert.IsNull(dbTransaction);
				}
				Assert.True(ambientAware.IsShouldDisposeResources);
			}
			Assert.True(ambientAware.IsShouldDisposeResources);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldGetConnectionNoOpenTest()
		{
			MockFactory mockFactory;
			MockAdoNetAmbientAware ambientAware;
			IConnectionFactory mockConnectionFactory;
			IDbConnection mockDbConnection;

			mockFactory = new MockFactory();
			mockConnectionFactory = mockFactory.CreateInstance<IConnectionFactory>();
			mockDbConnection = mockFactory.CreateInstance<IDbConnection>();

			Expect.Once.On(mockConnectionFactory).Method("GetConnection").Will(Return.Value(mockDbConnection));

			ambientAware = new MockAdoNetAmbientAware(MOCK_CONNECTION_STRING, mockConnectionFactory);

			ambientAware.BypassGetConnection(false);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldGetConnectionWithOpenTest()
		{
			MockFactory mockFactory;
			MockAdoNetAmbientAware ambientAware;
			IConnectionFactory mockConnectionFactory;
			IDbConnection mockDbConnection;

			mockFactory = new MockFactory();
			mockConnectionFactory = mockFactory.CreateInstance<IConnectionFactory>();
			mockDbConnection = mockFactory.CreateInstance<IDbConnection>();

			Expect.Once.On(mockConnectionFactory).Method("GetConnection").Will(Return.Value(mockDbConnection));
			Expect.AtLeastOnce.On(mockDbConnection).SetProperty("ConnectionString").To("myConnectionString");
			Expect.AtLeastOnce.On(mockDbConnection).Method("Open").WithNoArguments();

			ambientAware = new MockAdoNetAmbientAware(MOCK_CONNECTION_STRING, mockConnectionFactory);

			ambientAware.BypassGetConnection(true);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[SetUp]
		public void TestSetUp()
		{
			while (!DataSourceTransaction.UnwindTransaction())
				; // do nothing
		}

		[TearDown]
		public void TestTearDown()
		{
			while (!DataSourceTransaction.UnwindTransaction())
				; // do nothing
		}

		#endregion
	}
}