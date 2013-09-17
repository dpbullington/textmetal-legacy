/*
	Copyright ©2002-2013 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using NMock2;

using NUnit.Framework;

using TextMetal.Common.Solder.DependencyManagement;

namespace TextMetal.Common.UnitTests.Solder.DependencyManagement._
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
			Mockery mockery;

			mockery = new Mockery();
			mockDependencyManager = mockery.NewMock<IDependencyManager>();

			value = 11;

			singletonDependencyResolution = SingletonDependencyResolution.OfType<object>(value);

			Assert.IsNotNull(singletonDependencyResolution);

			result = singletonDependencyResolution.Resolve(mockDependencyManager);

			Assert.IsNotNull(result);
			Assert.AreEqual(11, result);

			mockery.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldCreateAndEvaluateTest()
		{
			SingletonDependencyResolution singletonDependencyResolution;
			IDependencyManager mockDependencyManager;
			object value;
			object result;
			Mockery mockery;

			mockery = new Mockery();
			mockDependencyManager = mockery.NewMock<IDependencyManager>();

			value = 11;

			singletonDependencyResolution = new SingletonDependencyResolution(value);

			Assert.IsNotNull(singletonDependencyResolution);

			result = singletonDependencyResolution.Resolve(mockDependencyManager);

			Assert.IsNotNull(result);
			Assert.AreEqual(11, result);

			mockery.VerifyAllExpectationsHaveBeenMet();
		}

		#endregion
	}
}