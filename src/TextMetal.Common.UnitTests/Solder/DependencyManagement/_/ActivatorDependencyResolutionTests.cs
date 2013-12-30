/*
	Copyright �2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using NMock2;

using NUnit.Framework;

using TextMetal.Common.Solder.DependencyManagement;

namespace TextMetal.Common.UnitTests.Solder.DependencyManagement._
{
	[TestFixture]
	public class ActivatorDependencyResolutionTests
	{
		#region Constructors/Destructors

		public ActivatorDependencyResolutionTests()
		{
		}

		#endregion

		#region Methods/Operators

		[Test]
		public void ShouldCreateAndEvaluateNonDefaultContructorOfTypeUsing1ArgsTest()
		{
			ActivatorDependencyResolution activatorDependencyResolution;
			IDependencyManager mockDependencyManager;
			Mockery mockery;
			object result;

			mockery = new Mockery();
			mockDependencyManager = mockery.NewMock<IDependencyManager>();

			Expect.Once.On(mockDependencyManager).Method("ResolveDependency").With(typeof(char[]), "").Will(Return.Value(new char[] { 'x', 'y', 'z' }));

			activatorDependencyResolution = ActivatorDependencyResolution.OfType<string, char[]>();

			result = activatorDependencyResolution.Resolve(mockDependencyManager);

			Assert.IsNotNull(result);
			Assert.AreEqual("xyz", result);

			mockery.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldCreateAndEvaluateNonDefaultContructorOfTypeUsing2ArgsTest()
		{
			ActivatorDependencyResolution activatorDependencyResolution;
			IDependencyManager mockDependencyManager;
			Mockery mockery;
			object result;

			mockery = new Mockery();
			mockDependencyManager = mockery.NewMock<IDependencyManager>();

			Expect.Once.On(mockDependencyManager).Method("ResolveDependency").With(typeof(char), "").Will(Return.Value('x'));
			Expect.Once.On(mockDependencyManager).Method("ResolveDependency").With(typeof(int), "").Will(Return.Value(10));

			activatorDependencyResolution = ActivatorDependencyResolution.OfType<string, char, int>();

			result = activatorDependencyResolution.Resolve(mockDependencyManager);

			Assert.IsNotNull(result);
			Assert.AreEqual("xxxxxxxxxx", result);

			mockery.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldCreateAndEvaluateTest()
		{
			ActivatorDependencyResolution activatorDependencyResolution;
			IDependencyManager mockDependencyManager;
			Type value;
			object result;
			Mockery mockery;

			mockery = new Mockery();
			mockDependencyManager = mockery.NewMock<IDependencyManager>();

			value = typeof(int);

			activatorDependencyResolution = new ActivatorDependencyResolution(value);

			Assert.IsNotNull(activatorDependencyResolution);

			result = activatorDependencyResolution.Resolve(mockDependencyManager);

			Assert.IsNotNull(result);
			Assert.AreEqual(0, result);

			mockery.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullValueCreateTest()
		{
			ActivatorDependencyResolution activatorDependencyResolution;
			Type value;

			value = null;

			activatorDependencyResolution = new ActivatorDependencyResolution(value);
		}

		#endregion
	}
}