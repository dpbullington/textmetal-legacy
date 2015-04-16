/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Data;

using NMock;

namespace TextMetal.Middleware.UnitTests.TestingInfrastructure
{
	public class MockConnection : IDbConnection
	{
		#region Constructors/Destructors

		public MockConnection()
		{
		}

		#endregion

		#region Properties/Indexers/Events

		public int ConnectionTimeout
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public string Database
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public ConnectionState State
		{
			get
			{
				return ConnectionState.Open;
			}
		}

		public string ConnectionString
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
			}
		}

		#endregion

		#region Methods/Operators

		public IDbTransaction BeginTransaction(IsolationLevel il)
		{
			return this.BeginTransaction();
		}

		public IDbTransaction BeginTransaction()
		{
			MockFactory mocks;
			IDbTransaction mockDbTransaction;

			mocks = new MockFactory();
			mockDbTransaction = mocks.CreateInstance<IDbTransaction>();
			Stub.On(mockDbTransaction);
			return mockDbTransaction;
		}

		public void ChangeDatabase(string databaseName)
		{
			throw new NotImplementedException();
		}

		public virtual void Close()
		{
			this.Dispose(true);
			GC.SuppressFinalize((object)this);
		}

		public IDbCommand CreateCommand()
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize((object)this);
		}

		protected virtual void Dispose(bool disposing)
		{
		}

		public void Open()
		{
		}

		#endregion
	}
}