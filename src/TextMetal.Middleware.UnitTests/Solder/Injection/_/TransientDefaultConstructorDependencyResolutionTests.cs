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
	public class TransientDefaultConstructorDependencyResolutionTests
	{
		#region Constructors/Destructors

		public TransientDefaultConstructorDependencyResolutionTests()
		{
		}

		#endregion

		#region Methods/Operators

		[Test]
		public void ShouldCreateAndEvaluateStronglyTypedAsNewGenericConstrainedTest()
		{
			IDependencyResolution transientDefaultConstructorDependencyResolution;
			IDependencyManager mockDependencyManager;
			object result;
			MockFactory mockFactory;

			mockFactory = new MockFactory();
			mockDependencyManager = mockFactory.CreateInstance<IDependencyManager>();

			transientDefaultConstructorDependencyResolution = TransientDefaultConstructorDependencyResolution.New<int>();

			Assert.IsNotNull(transientDefaultConstructorDependencyResolution);

			result = transientDefaultConstructorDependencyResolution.Resolve(mockDependencyManager);

			Assert.IsNotNull(result);
			Assert.AreEqual(0, result);

			transientDefaultConstructorDependencyResolution.Dispose();

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldCreateAndEvaluateStronglyTypedTest()
		{
			TransientDefaultConstructorDependencyResolution transientDefaultConstructorDependencyResolution;
			IDependencyManager mockDependencyManager;
			object result;
			MockFactory mockFactory;

			mockFactory = new MockFactory();
			mockDependencyManager = mockFactory.CreateInstance<IDependencyManager>();

			transientDefaultConstructorDependencyResolution = TransientDefaultConstructorDependencyResolution.Create<int>();

			Assert.IsNotNull(transientDefaultConstructorDependencyResolution);

			result = transientDefaultConstructorDependencyResolution.Resolve(mockDependencyManager);

			Assert.IsNotNull(result);
			Assert.AreEqual(0, result);

			transientDefaultConstructorDependencyResolution.Dispose();

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldCreateAndEvaluateTest()
		{
			TransientDefaultConstructorDependencyResolution transientDefaultConstructorDependencyResolution;
			IDependencyManager mockDependencyManager;
			object result;
			MockFactory mockFactory;

			mockFactory = new MockFactory();
			mockDependencyManager = mockFactory.CreateInstance<IDependencyManager>();

			transientDefaultConstructorDependencyResolution = new TransientDefaultConstructorDependencyResolution(typeof(int));

			Assert.IsNotNull(transientDefaultConstructorDependencyResolution);

			result = transientDefaultConstructorDependencyResolution.Resolve(mockDependencyManager);

			Assert.IsNotNull(result);
			Assert.AreEqual(0, result);

			transientDefaultConstructorDependencyResolution.Dispose();

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullDependencyManagerResolveTest()
		{
			TransientDefaultConstructorDependencyResolution transientDefaultConstructorDependencyResolution;
			IDependencyManager mockDependencyManager;
			object result;
			MockFactory mockFactory;

			mockFactory = new MockFactory();
			mockDependencyManager = null;

			transientDefaultConstructorDependencyResolution = new TransientDefaultConstructorDependencyResolution(typeof(int));

			result = transientDefaultConstructorDependencyResolution.Resolve(mockDependencyManager);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullValueCreateTest()
		{
			TransientDefaultConstructorDependencyResolution transientFactoryMethodDependencyResolution;
			Type type;

			type = null;

			transientFactoryMethodDependencyResolution = new TransientDefaultConstructorDependencyResolution(type);
		}

		#endregion
	}
}