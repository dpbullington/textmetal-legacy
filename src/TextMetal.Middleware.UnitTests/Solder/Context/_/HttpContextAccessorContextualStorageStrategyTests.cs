/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using Microsoft.AspNet.Http;

using NMock;

using NUnit.Framework;

using TextMetal.Middleware.Solder.Context;

namespace TextMetal.Middleware.UnitTests.Solder.Context._
{
	[TestFixture]
	public class HttpContextAccessorContextualStorageStrategyTests
	{
		#region Constructors/Destructors

		public HttpContextAccessorContextualStorageStrategyTests()
		{
		}

		#endregion

		#region Methods/Operators

		[Test]
		public void ShouldCreateTest()
		{
			HttpContextAccessorContextualStorageStrategy httpContextAccessorContextualStorageStrategy;
			const string KEY = "somekey";
			bool result;
			string value;
			string expected;
			IHttpContextAccessor mockHttpContextAccessor;
			HttpContext mockHttpContext;
			IDictionary<object, object> mockDictionary;

			MockFactory mockFactory;

			const string _unsusedString = null;

			mockFactory = new MockFactory();
			mockHttpContextAccessor = mockFactory.CreateInstance<IHttpContextAccessor>();
			mockHttpContext = mockFactory.CreateInstance<HttpContext>();
			mockDictionary = mockFactory.CreateInstance<IDictionary<object, object>>();

			Expect.On(mockHttpContextAccessor).Any.GetProperty(p => p.HttpContext).WillReturn(mockHttpContext);
			Expect.On(mockHttpContext).Any.GetProperty(p => p.Items).WillReturn(mockDictionary);

			Expect.On(mockDictionary).One.Method(m => m.ContainsKey(_unsusedString)).With(KEY).WillReturn(false);
			// TODO need to flush out expectations

			httpContextAccessorContextualStorageStrategy = new HttpContextAccessorContextualStorageStrategy(mockHttpContextAccessor);

			Assert.IsNotNull(httpContextAccessorContextualStorageStrategy);

			// has unset
			result = httpContextAccessorContextualStorageStrategy.HasValue(KEY);
			Assert.IsFalse(result);

			// get unset
			value = httpContextAccessorContextualStorageStrategy.GetValue<string>(KEY);
			Assert.IsNull(value);

			// remove unset
			httpContextAccessorContextualStorageStrategy.RemoveValue(KEY);

			// set unset
			expected = Guid.NewGuid().ToString("N");
			httpContextAccessorContextualStorageStrategy.SetValue(KEY, expected);

			// has isset
			result = httpContextAccessorContextualStorageStrategy.HasValue(KEY);
			Assert.IsTrue(result);

			// get isset
			value = httpContextAccessorContextualStorageStrategy.GetValue<string>(KEY);
			Assert.IsNotNull(value);
			Assert.AreEqual(expected, value);

			// set isset
			expected = Guid.NewGuid().ToString("N");
			httpContextAccessorContextualStorageStrategy.SetValue(KEY, expected);

			result = httpContextAccessorContextualStorageStrategy.HasValue(KEY);
			Assert.IsTrue(result);

			value = httpContextAccessorContextualStorageStrategy.GetValue<string>(KEY);
			Assert.IsNotNull(value);
			Assert.AreEqual(expected, value);

			// remove isset
			httpContextAccessorContextualStorageStrategy.RemoveValue(KEY);

			// verify remove
			result = httpContextAccessorContextualStorageStrategy.HasValue(KEY);
			Assert.IsFalse(result);

			value = httpContextAccessorContextualStorageStrategy.GetValue<string>(KEY);
			Assert.IsNull(value);
		}

		#endregion
	}
}