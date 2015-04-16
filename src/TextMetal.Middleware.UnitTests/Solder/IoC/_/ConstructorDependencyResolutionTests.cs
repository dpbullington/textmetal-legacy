/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Middleware.Solder.IoC;

using NMock;

using NUnit.Framework;

namespace TextMetal.Middleware.UnitTests.Solder.IoC._
{
	[TestFixture]
	public class ConstructorDependencyResolutionTests
	{
		#region Constructors/Destructors

		public ConstructorDependencyResolutionTests()
		{
		}

		#endregion

		#region Methods/Operators

		[Test]
		public void ShouldCreateAndEvaluateTest()
		{
			ConstructorDependencyResolution<int> constructorDependencyResolution;
			IDependencyManager mockDependencyManager;
			object result;
			MockFactory mockFactory;

			mockFactory = new MockFactory();
			mockDependencyManager = mockFactory.CreateInstance<IDependencyManager>();

			constructorDependencyResolution = new ConstructorDependencyResolution<int>();

			Assert.IsNotNull(constructorDependencyResolution);

			result = constructorDependencyResolution.Resolve(mockDependencyManager);

			Assert.IsNotNull(result);
			Assert.AreEqual(0, result);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		#endregion
	}
}