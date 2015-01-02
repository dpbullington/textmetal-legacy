/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Data;

using NMock2;

using NUnit.Framework;

using TextMetal.Common.UnitTests.TestingInfrastructure;

namespace TextMetal.Common.UnitTests.Data.LinqToSql._
{
	[TestFixture]
	public class LinqToSqlHelperTests
	{
		#region Constructors/Destructors

		public LinqToSqlHelperTests()
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
			Mockery mockery;
			LinqToSqlDataSource dataSource;
			IConnectionFactory mockConnectionFactory;

			mockery = new Mockery();
			mockConnectionFactory = mockery.NewMock<IConnectionFactory>();

			dataSource = new LinqToSqlDataSource(MOCK_CONNECTION_STRING, mockConnectionFactory);

			mockery.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void ShouldFailOnConnectionStringMismatchUnderAmbientDataSourceTransactionTest()
		{
			Mockery mockery;
			LinqToSqlDataSource dataSource;
			ContextWrapper<MockDataContext> contextWrapper;
			IConnectionFactory mockConnectionFactory;
			IDbConnection mockDbConnection;
			IDbTransaction mockDbTransaction;
			DataSourceTransaction transaction;

			mockery = new Mockery();
			mockConnectionFactory = mockery.NewMock<IConnectionFactory>();
			mockDbConnection = mockery.NewMock<IDbConnection>();
			mockDbTransaction = mockery.NewMock<IDbTransaction>();

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
			DataSourceTransaction.Current = transaction;
			Assert.IsNotNull(DataSourceTransaction.Current);

			transaction.Bind("xxx", mockDbConnection, mockDbTransaction, null);

			dataSource = new LinqToSqlDataSource(MOCK_CONNECTION_STRING, mockConnectionFactory);

			contextWrapper = dataSource.GetContext<MockDataContext>();
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void ShouldFailOnConnectionTypeMismatchUnderAmbientDataSourceTransactionTest()
		{
			Mockery mockery;
			LinqToSqlDataSource dataSource;
			ContextWrapper<MockDataContext> contextWrapper;
			IConnectionFactory mockConnectionFactory;
			IDbConnection mockDbConnection;
			IDbTransaction mockDbTransaction;
			DataSourceTransaction transaction;

			mockery = new Mockery();
			mockConnectionFactory = mockery.NewMock<IConnectionFactory>();
			mockDbConnection = mockery.NewMock<IDbConnection>();
			mockDbTransaction = mockery.NewMock<IDbTransaction>();

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
			DataSourceTransaction.Current = transaction;
			Assert.IsNotNull(DataSourceTransaction.Current);

			transaction.Bind(MOCK_CONNECTION_STRING, new MockConnection(), mockDbTransaction, null);

			dataSource = new LinqToSqlDataSource(MOCK_CONNECTION_STRING, mockConnectionFactory);

			contextWrapper = dataSource.GetContext<MockDataContext>();
		}

		[Test]
		[ExpectedException(typeof(NotSupportedException))]
		public void ShouldFailOnIncompatableContextTest()
		{
			Mockery mockery;
			LinqToSqlDataSource dataSource;
			IConnectionFactory mockConnectionFactory;
			ContextWrapper<IDisposable> contextWrapper;

			mockery = new Mockery();
			mockConnectionFactory = mockery.NewMock<IConnectionFactory>();

			Expect.Once.On(mockConnectionFactory).Method("GetConnection").Will(Return.Value(null));

			dataSource = new LinqToSqlDataSource(MOCK_CONNECTION_STRING, mockConnectionFactory);

			contextWrapper = dataSource.GetContext<IDisposable>();
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullConnectionFactoryCreateTest()
		{
			Mockery mockery;
			LinqToSqlDataSource dataSource;
			IConnectionFactory mockConnectionFactory;

			mockery = new Mockery();
			mockConnectionFactory = null;

			dataSource = new LinqToSqlDataSource(MOCK_CONNECTION_STRING, mockConnectionFactory);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void ShouldFailOnNullConnectionFromFactoryGetOpenConnectionTest()
		{
			Mockery mockery;
			LinqToSqlDataSource dataSource;
			IConnectionFactory mockConnectionFactory;
			ContextWrapper<MockDataContext> contextWrapper;

			mockery = new Mockery();
			mockConnectionFactory = mockery.NewMock<IConnectionFactory>();

			Expect.Once.On(mockConnectionFactory).Method("GetConnection").Will(Return.Value(null));

			dataSource = new LinqToSqlDataSource(MOCK_CONNECTION_STRING, mockConnectionFactory);

			contextWrapper = dataSource.GetContext<MockDataContext>();
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullConnectionStringCreateTest()
		{
			Mockery mockery;
			LinqToSqlDataSource dataSource;
			IConnectionFactory mockConnectionFactory;

			mockery = new Mockery();
			mockConnectionFactory = mockery.NewMock<IConnectionFactory>();

			dataSource = new LinqToSqlDataSource(null, mockConnectionFactory);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullConnectionStringGetForTest()
		{
			LinqToSqlDataSource dataSource;

			dataSource = LinqToSqlDataSource.GetForSql(null);
		}

		[Test]
		public void ShouldGetForSqlTest()
		{
			Mockery mockery;
			LinqToSqlDataSource dataSource;

			mockery = new Mockery();

			dataSource = LinqToSqlDataSource.GetForSql(MOCK_CONNECTION_STRING);

			Assert.IsNotNull(dataSource);
		}

		[SetUp]
		public void TestSetUp()
		{
			DataSourceTransaction.Current = null;
		}

		[TearDown]
		public void TestTearDown()
		{
			DataSourceTransaction.Current = null;
		}

		#endregion
	}
}