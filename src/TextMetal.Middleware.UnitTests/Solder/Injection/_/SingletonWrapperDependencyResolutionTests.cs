/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using NMock;

using NUnit.Framework;

using TextMetal.Middleware.Solder.Injection;
using TextMetal.Middleware.Solder.Runtime;
using TextMetal.Middleware.UnitTests.TestingInfrastructure;

namespace TextMetal.Middleware.UnitTests.Solder.Injection._
{
	[TestFixture]
	public class SingletonWrapperDependencyResolutionTests
	{
		#region Constructors/Destructors

		public SingletonWrapperDependencyResolutionTests()
		{
		}

		#endregion

		#region Methods/Operators

		[Test]
		public void ShouldCreateAndEvaluateExplicitEagerLoadTest()
		{
			SingletonWrapperDependencyResolution singletonWrapperDependencyResolution;
			IDependencyManager mockDependencyManager;
			IDependencyResolution mockDependencyResolution;
			IDependencyManager _unusedDependencyManager = null;
			object result;
			MockFactory mockFactory;

			mockFactory = new MockFactory();
			mockDependencyManager = AssemblyLoaderContainerContext.TheOnlyAllowedInstance.DependencyManager;
			mockDependencyResolution = mockFactory.CreateInstance<IDependencyResolution>();

			Expect.On(mockDependencyResolution).One.Method(m => m.Resolve(_unusedDependencyManager)).With(mockDependencyManager).WillReturn(11);
			Expect.On(mockDependencyResolution).One.Method(m => m.Dispose());

			singletonWrapperDependencyResolution = new SingletonWrapperDependencyResolution(true, mockDependencyResolution);

			// should be frozen at this point
			result = singletonWrapperDependencyResolution.Resolve(mockDependencyManager);

			Assert.IsNotNull(result);
			Assert.AreEqual(11, result);

			// should be frozen at this point
			result = singletonWrapperDependencyResolution.Resolve(mockDependencyManager);

			Assert.IsNotNull(result);
			Assert.AreEqual(11, result);

			singletonWrapperDependencyResolution.Dispose();

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldCreateAndEvaluateTest()
		{
			SingletonWrapperDependencyResolution singletonWrapperDependencyResolution;
			IDependencyManager mockDependencyManager;
			IDependencyResolution mockDependencyResolution;
			IDependencyManager _unusedDependencyManager = null;
			object result;
			MockFactory mockFactory;

			mockFactory = new MockFactory();
			mockDependencyManager = mockFactory.CreateInstance<IDependencyManager>();
			mockDependencyResolution = mockFactory.CreateInstance<IDependencyResolution>();

			Expect.On(mockDependencyResolution).One.Method(m => m.Resolve(_unusedDependencyManager)).With(mockDependencyManager).WillReturn(11);
			Expect.On(mockDependencyResolution).One.Method(m => m.Dispose());

			singletonWrapperDependencyResolution = new SingletonWrapperDependencyResolution(mockDependencyResolution);

			// should be thawed at this point
			result = singletonWrapperDependencyResolution.Resolve(mockDependencyManager);

			Assert.IsNotNull(result);
			Assert.AreEqual(11, result);

			// should be frozen at this point
			result = singletonWrapperDependencyResolution.Resolve(mockDependencyManager);

			Assert.IsNotNull(result);
			Assert.AreEqual(11, result);

			singletonWrapperDependencyResolution.Dispose();

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullDependencyManagerResolveTest()
		{
			SingletonWrapperDependencyResolution singletonWrapperDependencyResolution;
			IDependencyManager mockDependencyManager;
			IDependencyResolution mockDependencyResolution;
			Func<object> value;
			object result;
			MockFactory mockFactory;

			mockFactory = new MockFactory();
			mockDependencyManager = null;
			mockDependencyResolution = mockFactory.CreateInstance<IDependencyResolution>();

			singletonWrapperDependencyResolution = new SingletonWrapperDependencyResolution(mockDependencyResolution);

			result = singletonWrapperDependencyResolution.Resolve(mockDependencyManager);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullValueCreateTest()
		{
			SingletonWrapperDependencyResolution singletonWrapperDependencyResolution;
			IDependencyResolution mockDependencyResolution;

			mockDependencyResolution = null;

			singletonWrapperDependencyResolution = new SingletonWrapperDependencyResolution(mockDependencyResolution);
		}

		#endregion
	}
}