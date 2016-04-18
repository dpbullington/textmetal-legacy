/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Threading;

using NUnit.Framework;

using TextMetal.Middleware.Solder.Context;
using TextMetal.Middleware.UnitTests.TestingInfrastructure;

namespace TextMetal.Middleware.UnitTests.Solder.Context._
{
	[TestFixture]
	public class SharedStaticContextualStorageStrategyTests
	{
		#region Constructors/Destructors

		public SharedStaticContextualStorageStrategyTests()
		{
		}

		#endregion

		#region Methods/Operators

		[Test]
		public void ShouldCreateCrossThreadTest()
		{
			SharedStaticContextualStorageStrategy sharedStaticContextualStorageStrategy;
			const string KEY = "somekey";
			bool result;
			string value;
			string expected;

			Thread thread;
			int managedThreadId;

			managedThreadId = Thread.CurrentThread.ManagedThreadId;
			sharedStaticContextualStorageStrategy = new SharedStaticContextualStorageStrategy();
			sharedStaticContextualStorageStrategy.ResetValues();

			// set unset
			expected = Guid.NewGuid().ToString("N");
			sharedStaticContextualStorageStrategy.SetValue(KEY, expected);

			// has isset
			result = sharedStaticContextualStorageStrategy.HasValue(KEY);
			Assert.IsTrue(result);

			// get isset
			value = sharedStaticContextualStorageStrategy.GetValue<string>(KEY);
			Assert.IsNotNull(value);
			Assert.AreEqual(expected, value);

			thread = new Thread(() =>
								{
									int _managedThreadId = Thread.CurrentThread.ManagedThreadId;

									Assert.AreNotEqual(_managedThreadId, managedThreadId);

									// has isset(other thread)
									result = sharedStaticContextualStorageStrategy.HasValue(KEY);
									Assert.IsTrue(result);
								});

			thread.Start();
			thread.Join();
		}

		[Test]
		public void ShouldCreateTest()
		{
			SharedStaticContextualStorageStrategy sharedStaticContextualStorageStrategy;
			const string KEY = "somekey";
			bool result;
			string value;
			string expected;

			sharedStaticContextualStorageStrategy = new SharedStaticContextualStorageStrategy();
			sharedStaticContextualStorageStrategy.ResetValues();

			// has unset
			result = sharedStaticContextualStorageStrategy.HasValue(KEY);
			Assert.IsFalse(result);

			// get unset
			value = sharedStaticContextualStorageStrategy.GetValue<string>(KEY);
			Assert.IsNull(value);

			// remove unset
			sharedStaticContextualStorageStrategy.RemoveValue(KEY);

			// set unset
			expected = Guid.NewGuid().ToString("N");
			sharedStaticContextualStorageStrategy.SetValue(KEY, expected);

			// has isset
			result = sharedStaticContextualStorageStrategy.HasValue(KEY);
			Assert.IsTrue(result);

			// get isset
			value = sharedStaticContextualStorageStrategy.GetValue<string>(KEY);
			Assert.IsNotNull(value);
			Assert.AreEqual(expected, value);

			// set isset
			expected = Guid.NewGuid().ToString("N");
			sharedStaticContextualStorageStrategy.SetValue(KEY, expected);

			result = sharedStaticContextualStorageStrategy.HasValue(KEY);
			Assert.IsTrue(result);

			value = sharedStaticContextualStorageStrategy.GetValue<string>(KEY);
			Assert.IsNotNull(value);
			Assert.AreEqual(expected, value);

			// remove isset
			sharedStaticContextualStorageStrategy.RemoveValue(KEY);

			// verify remove
			result = sharedStaticContextualStorageStrategy.HasValue(KEY);
			Assert.IsFalse(result);

			value = sharedStaticContextualStorageStrategy.GetValue<string>(KEY);
			Assert.IsNull(value);
		}

		[Test]
		public void ShouldCreateTest__todo_mock_shared_static()
		{
			Assert.Ignore("TODO: This test case has not been implemented yet.");
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullSharedStaticCreateTest()
		{
			IDictionary<string, object> mockSharedStatic;
			SharedStaticContextualStorageStrategy sharedStaticContextualStorageStrategy;

			mockSharedStatic = null;

			sharedStaticContextualStorageStrategy = new SharedStaticContextualStorageStrategy(mockSharedStatic);
		}

		#endregion
	}
}