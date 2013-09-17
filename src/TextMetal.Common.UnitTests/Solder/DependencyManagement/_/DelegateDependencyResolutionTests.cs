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
			Mockery mockery;

			mockery = new Mockery();
			mockDependencyManager = mockery.NewMock<IDependencyManager>();

			value = () => 11;

			delegateDependencyResolution = DelegateDependencyResolution.FromFunc<object>(value);

			Assert.IsNotNull(delegateDependencyResolution);

			result = delegateDependencyResolution.Resolve(mockDependencyManager);

			Assert.IsNotNull(result);
			Assert.AreEqual(11, result);

			mockery.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldCreateAndEvaluateTest()
		{
			DelegateDependencyResolution delegateDependencyResolution;
			IDependencyManager mockDependencyManager;
			Func<object> value;
			object result;
			Mockery mockery;

			mockery = new Mockery();
			mockDependencyManager = mockery.NewMock<IDependencyManager>();
			
			value = () => 11;

			delegateDependencyResolution = new DelegateDependencyResolution(value);

			Assert.IsNotNull(delegateDependencyResolution);

			result = delegateDependencyResolution.Resolve(mockDependencyManager);

			Assert.IsNotNull(result);
			Assert.AreEqual(11, result);

			mockery.VerifyAllExpectationsHaveBeenMet();
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