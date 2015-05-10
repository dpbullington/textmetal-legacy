/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Data;

using NMock;

using NUnit.Framework;

using TextMetal.Middleware.Common.Fascades.AdoNet.UoW;
using TextMetal.Middleware.Data.Impl.MicrosoftLinq;
using TextMetal.Middleware.UnitTests.TestingInfrastructure;

using _MockDataContext = System.IDisposable;

namespace TextMetal.Middleware.UnitTests.Data.Impl.MicrosoftLinq._
{
	[TestFixture]
	public class DataContextModelRepositoryTests
	{
		#region Constructors/Destructors

		public DataContextModelRepositoryTests()
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
			TestDataContextModelRepository dataContextModelRepository;
			IConnectionFactory mockConnectionFactory;

			mockFactory = new MockFactory();
			mockConnectionFactory = mockFactory.CreateInstance<IConnectionFactory>();

			dataContextModelRepository = new TestDataContextModelRepository(MOCK_CONNECTION_STRING, mockConnectionFactory);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void ShouldFailOnConnectionStringMismatchUnderAmbientDataSourceTransactionTest()
		{
			MockFactory mockFactory;
			TestDataContextModelRepository dataContextModelRepository;
			ContextWrapper<MockDataContext> contextWrapper;
			IConnectionFactory mockConnectionFactory;
			IDbConnection mockDbConnection;
			IDbTransaction mockDbTransaction;
			DataSourceTransaction transaction;

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
			DataSourceTransaction.Current = transaction;
			Assert.IsNotNull(DataSourceTransaction.Current);

			transaction.Bind("xxx", mockDbConnection, mockDbTransaction, null);

			dataContextModelRepository = new TestDataContextModelRepository(MOCK_CONNECTION_STRING, mockConnectionFactory);

			contextWrapper = dataContextModelRepository.GetContext<MockDataContext>();
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void ShouldFailOnConnectionTypeMismatchUnderAmbientDataSourceTransactionTest()
		{
			MockFactory mockFactory;
			TestDataContextModelRepository dataContextModelRepository;
			ContextWrapper<MockDataContext> contextWrapper;
			IConnectionFactory mockConnectionFactory;
			IDbConnection mockDbConnection;
			IDbTransaction mockDbTransaction;
			DataSourceTransaction transaction;

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
			DataSourceTransaction.Current = transaction;
			Assert.IsNotNull(DataSourceTransaction.Current);

			transaction.Bind(MOCK_CONNECTION_STRING, new MockConnection(), mockDbTransaction, null);

			dataContextModelRepository = new TestDataContextModelRepository(MOCK_CONNECTION_STRING, mockConnectionFactory);

			contextWrapper = dataContextModelRepository.GetContext<MockDataContext>();
		}

		[Test]
		[ExpectedException(typeof(NotSupportedException))]
		public void ShouldFailOnIncompatableContextTest()
		{
			MockFactory mockFactory;
			TestDataContextModelRepository dataContextModelRepository;
			IConnectionFactory mockConnectionFactory;
			ContextWrapper<IDisposable> contextWrapper;

			mockFactory = new MockFactory();
			mockConnectionFactory = mockFactory.CreateInstance<IConnectionFactory>();

			Expect.Once.On(mockConnectionFactory).Method("GetConnection").Will(Return.Value(null));

			dataContextModelRepository = new TestDataContextModelRepository(MOCK_CONNECTION_STRING, mockConnectionFactory);

			contextWrapper = dataContextModelRepository.GetContext<IDisposable>();
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullConnectionFactoryCreateTest()
		{
			MockFactory mockFactory;
			TestDataContextModelRepository dataContextModelRepository;
			IConnectionFactory mockConnectionFactory;

			mockFactory = new MockFactory();
			mockConnectionFactory = null;

			dataContextModelRepository = new TestDataContextModelRepository(MOCK_CONNECTION_STRING, mockConnectionFactory);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void ShouldFailOnNullConnectionFromFactoryGetOpenConnectionTest()
		{
			MockFactory mockFactory;
			TestDataContextModelRepository dataContextModelRepository;
			IConnectionFactory mockConnectionFactory;
			ContextWrapper<MockDataContext> contextWrapper;

			mockFactory = new MockFactory();
			mockConnectionFactory = mockFactory.CreateInstance<IConnectionFactory>();

			Expect.Once.On(mockConnectionFactory).Method("GetConnection").Will(Return.Value(null));

			dataContextModelRepository = new TestDataContextModelRepository(MOCK_CONNECTION_STRING, mockConnectionFactory);

			contextWrapper = dataContextModelRepository.GetContext<MockDataContext>();
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullConnectionStringCreateTest()
		{
			MockFactory mockFactory;
			TestDataContextModelRepository dataContextModelRepository;

			mockFactory = new MockFactory();

			dataContextModelRepository = new TestDataContextModelRepository();
		}

		[SetUp]
		public void TestSetUp()
		{
			UnitOfWork.Current = null;
		}

		[TearDown]
		public void TestTearDown()
		{
			UnitOfWork.Current = null;
		}

		public class TestDataContextModelRepository : DataContextModelRepository<MockDataContext>
		{
			public TestDataContextModelRepository()
			{
			}
		}

		#endregion
	}
}