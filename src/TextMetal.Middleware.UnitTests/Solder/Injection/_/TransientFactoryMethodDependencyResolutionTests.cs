/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using NMock;

using NUnit.Framework;

using TextMetal.Middleware.Solder.Injection;
using TextMetal.Middleware.UnitTests.TestingInfrastructure;

namespace TextMetal.Middleware.UnitTests.Solder.Injection._
{
	[TestFixture]
	public class TransientFactoryMethodDependencyResolutionTests
	{
		#region Constructors/Destructors

		public TransientFactoryMethodDependencyResolutionTests()
		{
		}

		#endregion

		#region Methods/Operators

		[Test]
		public void ShouldCreateAndEvaluateTest()
		{
			TransientFactoryMethodDependencyResolution transientFactoryMethodDependencyResolution;
			IDependencyManager mockDependencyManager;
			Func<object> value;
			object result;
			MockFactory mockFactory;

			mockFactory = new MockFactory();
			mockDependencyManager = mockFactory.CreateInstance<IDependencyManager>();

			value = () => 11;

			transientFactoryMethodDependencyResolution = new TransientFactoryMethodDependencyResolution(value);

			result = transientFactoryMethodDependencyResolution.Resolve(mockDependencyManager, typeof(object), string.Empty);

			Assert.IsNotNull(result);
			Assert.AreEqual(11, result);

			transientFactoryMethodDependencyResolution.Dispose();

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldCreateStronglyTypedTest()
		{
			TransientFactoryMethodDependencyResolution<int> transientFactoryMethodDependencyResolution;
			IDependencyManager mockDependencyManager;
			Func<int> value;
			object result;
			MockFactory mockFactory;

			mockFactory = new MockFactory();
			mockDependencyManager = mockFactory.CreateInstance<IDependencyManager>();

			value = () => 11;

			transientFactoryMethodDependencyResolution = new TransientFactoryMethodDependencyResolution<int>(value);

			result = transientFactoryMethodDependencyResolution.Resolve(mockDependencyManager, typeof(object), string.Empty);

			Assert.IsNotNull(result);
			Assert.AreEqual(11, result);

			transientFactoryMethodDependencyResolution.Dispose();

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullDependencyManagerResolveTest()
		{
			TransientFactoryMethodDependencyResolution transientFactoryMethodDependencyResolution;
			IDependencyManager mockDependencyManager;
			Func<object> value;
			object result;
			MockFactory mockFactory;

			mockFactory = new MockFactory();
			mockDependencyManager = null;

			value = () => 11;

			transientFactoryMethodDependencyResolution = new TransientFactoryMethodDependencyResolution(value);

			result = transientFactoryMethodDependencyResolution.Resolve(mockDependencyManager, typeof(object), string.Empty);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyResolveTest()
		{
			TransientFactoryMethodDependencyResolution transientFactoryMethodDependencyResolution;
			IDependencyManager mockDependencyManager;
			Func<object> value;
			object result;
			MockFactory mockFactory;

			mockFactory = new MockFactory();
			mockDependencyManager = mockFactory.CreateInstance<IDependencyManager>();

			value = () => 11;

			transientFactoryMethodDependencyResolution = new TransientFactoryMethodDependencyResolution(value);

			result = transientFactoryMethodDependencyResolution.Resolve(mockDependencyManager, typeof(object), null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullTypeResolveTest()
		{
			TransientFactoryMethodDependencyResolution transientFactoryMethodDependencyResolution;
			IDependencyManager mockDependencyManager;
			Func<object> value;
			object result;
			MockFactory mockFactory;

			mockFactory = new MockFactory();
			mockDependencyManager = mockFactory.CreateInstance<IDependencyManager>();

			value = () => 11;

			transientFactoryMethodDependencyResolution = new TransientFactoryMethodDependencyResolution(value);

			result = transientFactoryMethodDependencyResolution.Resolve(mockDependencyManager, null, string.Empty);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullValueCreateFromFuncTest()
		{
			TransientFactoryMethodDependencyResolution<int> transientFactoryMethodDependencyResolution;
			Func<int> value;

			value = null;

			transientFactoryMethodDependencyResolution = new TransientFactoryMethodDependencyResolution<int>(value);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullValueCreateTest()
		{
			TransientFactoryMethodDependencyResolution transientFactoryMethodDependencyResolution;
			Func<object> value;

			value = null;

			transientFactoryMethodDependencyResolution = new TransientFactoryMethodDependencyResolution(value);
		}

		#endregion
	}
}