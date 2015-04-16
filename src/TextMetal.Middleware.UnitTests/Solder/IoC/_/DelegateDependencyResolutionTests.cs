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
	public class DelegateDependencyResolutionTests
	{
		#region Constructors/Destructors

		public DelegateDependencyResolutionTests()
		{
		}

		#endregion

		#region Methods/Operators

		[Test]
		public void ShouldCreateAndEvaluateFromFuncTest()
		{
			DelegateDependencyResolution delegateDependencyResolution;
			IDependencyManager mockDependencyManager;
			Func<object> value;
			object result;
			MockFactory mockFactory;

			mockFactory = new MockFactory();
			mockDependencyManager = mockFactory.CreateInstance<IDependencyManager>();

			value = () => 11;

			delegateDependencyResolution = DelegateDependencyResolution.FromFunc<object>(value);

			Assert.IsNotNull(delegateDependencyResolution);

			result = delegateDependencyResolution.Resolve(mockDependencyManager);

			Assert.IsNotNull(result);
			Assert.AreEqual(11, result);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldCreateAndEvaluateTest()
		{
			DelegateDependencyResolution delegateDependencyResolution;
			IDependencyManager mockDependencyManager;
			Func<object> value;
			object result;
			MockFactory mockFactory;

			mockFactory = new MockFactory();
			mockDependencyManager = mockFactory.CreateInstance<IDependencyManager>();

			value = () => 11;

			delegateDependencyResolution = new DelegateDependencyResolution(value);

			Assert.IsNotNull(delegateDependencyResolution);

			result = delegateDependencyResolution.Resolve(mockDependencyManager);

			Assert.IsNotNull(result);
			Assert.AreEqual(11, result);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullValueCreateFromFuncTest()
		{
			DelegateDependencyResolution delegateDependencyResolution;
			Func<object> value;

			value = null;

			delegateDependencyResolution = DelegateDependencyResolution.FromFunc<object>(value);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullValueCreateTest()
		{
			DelegateDependencyResolution delegateDependencyResolution;
			Func<object[], object> value;

			value = null;

			delegateDependencyResolution = new DelegateDependencyResolution(value);
		}

		#endregion
	}
}