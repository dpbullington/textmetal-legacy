/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using NMock;

using NUnit.Framework;

using TextMetal.Middleware.Solder.Context;
using TextMetal.Middleware.Solder.Injection;
using TextMetal.Middleware.UnitTests.TestingInfrastructure;

namespace TextMetal.Middleware.UnitTests.Solder.Injection._
{
	[TestFixture]
	public class ContextWrapperDependencyResolutionTests
	{
		#region Constructors/Destructors

		public ContextWrapperDependencyResolutionTests()
		{
		}

		#endregion

		#region Methods/Operators

		[Test]
		public void ShouldCreateTest__todo_mock_contextual_storage_strategy()
		{
			Assert.Ignore("TODO: This test case has not been implemented yet.");
		}

		[Test]
		public void ShouldCreateAndEvaluateTest()
		{
			ContextWrapperDependencyResolution contextWrapperDependencyResolution;
			IDependencyManager mockDependencyManager;
			IDependencyResolution mockDependencyResolution;
			IDependencyManager _unusedDependencyManager = null;
			Type _unusedType = null;
			string _unusedString = null;
			object result;
			MockFactory mockFactory;

			mockFactory = new MockFactory();
			mockDependencyManager = mockFactory.CreateInstance<IDependencyManager>();
			mockDependencyResolution = mockFactory.CreateInstance<IDependencyResolution>();

			Expect.On(mockDependencyResolution).One.Method(m => m.Resolve(_unusedDependencyManager, _unusedType, _unusedString)).With(mockDependencyManager, typeof(object), string.Empty).WillReturn(11);
			Expect.On(mockDependencyResolution).One.Method(m => m.Dispose());

			contextWrapperDependencyResolution = new ContextWrapperDependencyResolution(ContextScope.LocalThreadSafe, mockDependencyResolution);

			Assert.AreEqual(DependencyLifetime.Singleton, contextWrapperDependencyResolution.DependencyLifetime);

			// should be thawed at this point
			result = contextWrapperDependencyResolution.Resolve(mockDependencyManager, typeof(object), string.Empty);

			Assert.IsNotNull(result);
			Assert.AreEqual(11, result);

			// should be frozen at this point
			result = contextWrapperDependencyResolution.Resolve(mockDependencyManager, typeof(object), string.Empty);

			Assert.IsNotNull(result);
			Assert.AreEqual(11, result);

			contextWrapperDependencyResolution.Dispose();

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullDependencyManagerResolveTest()
		{
			ContextWrapperDependencyResolution contextWrapperDependencyResolution;
			IDependencyManager mockDependencyManager;
			IDependencyResolution mockDependencyResolution;
			object result;
			MockFactory mockFactory;

			mockFactory = new MockFactory();
			mockDependencyManager = null;
			mockDependencyResolution = mockFactory.CreateInstance<IDependencyResolution>();

			contextWrapperDependencyResolution = new ContextWrapperDependencyResolution(ContextScope.LocalThreadSafe, mockDependencyResolution);

			result = contextWrapperDependencyResolution.Resolve(mockDependencyManager, typeof(object), string.Empty);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyResolveTest()
		{
			ContextWrapperDependencyResolution contextWrapperDependencyResolution;
			IDependencyManager mockDependencyManager;
			IDependencyResolution mockDependencyResolution;
			object result;
			MockFactory mockFactory;

			mockFactory = new MockFactory();
			mockDependencyManager = mockFactory.CreateInstance<IDependencyManager>();
			mockDependencyResolution = mockFactory.CreateInstance<IDependencyResolution>();

			contextWrapperDependencyResolution = new ContextWrapperDependencyResolution(ContextScope.LocalThreadSafe, mockDependencyResolution);

			result = contextWrapperDependencyResolution.Resolve(mockDependencyManager, typeof(object), null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullTypeResolveTest()
		{
			ContextWrapperDependencyResolution contextWrapperDependencyResolution;
			IDependencyManager mockDependencyManager;
			IDependencyResolution mockDependencyResolution;
			object result;
			MockFactory mockFactory;

			mockFactory = new MockFactory();
			mockDependencyManager = mockFactory.CreateInstance<IDependencyManager>();
			mockDependencyResolution = mockFactory.CreateInstance<IDependencyResolution>();

			contextWrapperDependencyResolution = new ContextWrapperDependencyResolution(ContextScope.LocalThreadSafe, mockDependencyResolution);

			result = contextWrapperDependencyResolution.Resolve(mockDependencyManager, null, string.Empty);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullValueCreateTest()
		{
			ContextWrapperDependencyResolution contextWrapperDependencyResolution;
			IDependencyResolution mockDependencyResolution;

			mockDependencyResolution = null;

			contextWrapperDependencyResolution = new ContextWrapperDependencyResolution(ContextScope.LocalThreadSafe, mockDependencyResolution);
		}

		#endregion
	}
}