/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using NMock2;

using NUnit.Framework;

using TextMetal.Common.Solder;

namespace TextMetal.Common.UnitTests.Solder._
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
			Mockery mockery;

			mockery = new Mockery();
			mockDependencyManager = mockery.NewMock<IDependencyManager>();

			constructorDependencyResolution = new ConstructorDependencyResolution<int>();

			Assert.IsNotNull(constructorDependencyResolution);

			result = constructorDependencyResolution.Resolve(mockDependencyManager);

			Assert.IsNotNull(result);
			Assert.AreEqual(0, result);

			mockery.VerifyAllExpectationsHaveBeenMet();
		}

		#endregion
	}
}