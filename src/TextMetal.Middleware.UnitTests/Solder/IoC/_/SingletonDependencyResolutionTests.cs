/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using NMock;

using NUnit.Framework;

using TextMetal.Middleware.Solder.IoC;

namespace TextMetal.Middleware.UnitTests.Solder.IoC._
{
	[TestFixture]
	public class SingletonDependencyResolutionTests
	{
		#region Constructors/Destructors

		public SingletonDependencyResolutionTests()
		{
		}

		#endregion

		#region Methods/Operators

		[Test]
		public void ShouldCreateAndEvaluateOfTypeTest()
		{
			SingletonDependencyResolution singletonDependencyResolution;
			IDependencyManager mockDependencyManager;
			object value;
			object result;
			MockFactory mockFactory;

			mockFactory = new MockFactory();
			mockDependencyManager = mockFactory.CreateInstance<IDependencyManager>();

			value = 11;

			singletonDependencyResolution = SingletonDependencyResolution.OfType<object>(value);

			Assert.IsNotNull(singletonDependencyResolution);

			result = singletonDependencyResolution.Resolve(mockDependencyManager);

			Assert.IsNotNull(result);
			Assert.AreEqual(11, result);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldCreateAndEvaluateTest()
		{
			SingletonDependencyResolution singletonDependencyResolution;
			IDependencyManager mockDependencyManager;
			object value;
			object result;
			MockFactory mockFactory;

			mockFactory = new MockFactory();
			mockDependencyManager = mockFactory.CreateInstance<IDependencyManager>();

			value = 11;

			singletonDependencyResolution = new SingletonDependencyResolution(value);

			Assert.IsNotNull(singletonDependencyResolution);

			result = singletonDependencyResolution.Resolve(mockDependencyManager);

			Assert.IsNotNull(result);
			Assert.AreEqual(11, result);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		#endregion
	}
}