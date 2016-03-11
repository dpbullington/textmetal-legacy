/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Threading.Tasks;

using NUnit.Framework;

using TextMetal.Middleware.Solder.Context;

namespace TextMetal.Middleware.UnitTests.Solder.Context._
{
	[TestFixture]
	public class AsyncLocalContextualStorageStrategyTests
	{
		#region Constructors/Destructors

		public AsyncLocalContextualStorageStrategyTests()
		{
		}

		#endregion

		#region Methods/Operators

		[Test]
		public void ShouldCreateCrossThreadTest()
		{
			AsyncLocalContextualStorageStrategy asyncLocalContextualStorageStrategy;
			const string KEY = "somekey";
			bool result;
			string value;
			string expected;

			Task task;

			asyncLocalContextualStorageStrategy = new AsyncLocalContextualStorageStrategy();

			Assert.IsNotNull(asyncLocalContextualStorageStrategy);

			// set unset
			expected = Guid.NewGuid().ToString("N");
			asyncLocalContextualStorageStrategy.SetValue(KEY, expected);

			// has isset
			result = asyncLocalContextualStorageStrategy.HasValue(KEY);
			Assert.IsTrue(result);

			// get isset
			value = asyncLocalContextualStorageStrategy.GetValue<string>(KEY);
			Assert.IsNotNull(value);
			Assert.AreEqual(expected, value);

			task = Task.Run(() =>
							{
								// has isset(other thread)
								result = asyncLocalContextualStorageStrategy.HasValue(KEY);
								Assert.IsTrue(result);

								// get isset(other thread)
								value = asyncLocalContextualStorageStrategy.GetValue<string>(KEY);
								Assert.IsNotNull(value);
								Assert.AreEqual(expected, value);
							});

			task.Wait();
		}

		[Test]
		public void ShouldCreateTest()
		{
			AsyncLocalContextualStorageStrategy asyncLocalContextualStorageStrategy;
			const string KEY = "somekey";
			bool result;
			string value;
			string expected;

			asyncLocalContextualStorageStrategy = new AsyncLocalContextualStorageStrategy();
			Assert.IsNotNull(asyncLocalContextualStorageStrategy);

			// has unset
			result = asyncLocalContextualStorageStrategy.HasValue(KEY);
			Assert.IsFalse(result);

			// get unset
			value = asyncLocalContextualStorageStrategy.GetValue<string>(KEY);
			Assert.IsNull(value);

			// remove unset
			asyncLocalContextualStorageStrategy.RemoveValue(KEY);

			// set unset
			expected = Guid.NewGuid().ToString("N");
			asyncLocalContextualStorageStrategy.SetValue(KEY, expected);

			// has isset
			result = asyncLocalContextualStorageStrategy.HasValue(KEY);
			Assert.IsTrue(result);

			// get isset
			value = asyncLocalContextualStorageStrategy.GetValue<string>(KEY);
			Assert.IsNotNull(value);
			Assert.AreEqual(expected, value);

			// set isset
			expected = Guid.NewGuid().ToString("N");
			asyncLocalContextualStorageStrategy.SetValue(KEY, expected);

			result = asyncLocalContextualStorageStrategy.HasValue(KEY);
			Assert.IsTrue(result);

			value = asyncLocalContextualStorageStrategy.GetValue<string>(KEY);
			Assert.IsNotNull(value);
			Assert.AreEqual(expected, value);

			// remove isset
			asyncLocalContextualStorageStrategy.RemoveValue(KEY);

			// verify remove
			result = asyncLocalContextualStorageStrategy.HasValue(KEY);
			Assert.IsFalse(result);

			value = asyncLocalContextualStorageStrategy.GetValue<string>(KEY);
			Assert.IsNull(value);
		}

		#endregion
	}
}