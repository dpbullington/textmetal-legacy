/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Middleware.Solder.IoC;
using TextMetal.Middleware.UnitTests.TestingInfrastructure;

using NMock;

using NUnit.Framework;

namespace TextMetal.Middleware.UnitTests.Solder.IoC._
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
			MockFactory mockFactory;
			object result;

			mockFactory = new MockFactory();
			mockDependencyManager = mockFactory.CreateInstance<IDependencyManager>();

			//Expect.Once.On(mockDependencyManager).Method("ResolveDependency").With(typeof(char[]), string.Empty).Will(Return.Value(new char[] { 'x', 'y', 'z' }));

			activatorDependencyResolution = ActivatorDependencyResolution.OfType<DateTime, long>();

			result = activatorDependencyResolution.Resolve(mockDependencyManager);

			Assert.IsNotNull(result);
			Assert.AreEqual(new DateTime(0), result);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldCreateAndEvaluateNonDefaultContructorOfTypeUsing1ArgsWithAutowireTest()
		{
			ActivatorDependencyResolution activatorDependencyResolution;
			IDependencyManager mockDependencyManager;
			MockFactory mockFactory;
			MockDependantObject result;

			mockFactory = new MockFactory();
			mockDependencyManager = mockFactory.CreateInstance<IDependencyManager>();

			Expect.Once.On(mockDependencyManager).Method("ResolveDependency").With(typeof(string), string.Empty).Will(Return.Value("turing tarpit"));

			activatorDependencyResolution = ActivatorDependencyResolution.OfType<MockDependantObject, string>();

			result = (MockDependantObject)activatorDependencyResolution.Resolve(mockDependencyManager);

			Assert.IsNotNull(result);
			Assert.AreEqual("turing tarpit", result.Text);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldCreateAndEvaluateNonDefaultContructorOfTypeUsing2ArgsTest()
		{
			ActivatorDependencyResolution activatorDependencyResolution;
			IDependencyManager mockDependencyManager;
			MockFactory mockFactory;
			object result;

			mockFactory = new MockFactory();
			mockDependencyManager = mockFactory.CreateInstance<IDependencyManager>();

			//Expect.Once.On(mockDependencyManager).Method("ResolveDependency").With(typeof(char), string.Empty).Will(Return.Value('x'));
			//Expect.Once.On(mockDependencyManager).Method("ResolveDependency").With(typeof(int), string.Empty).Will(Return.Value(10));

			activatorDependencyResolution = ActivatorDependencyResolution.OfType<string, char, int>();

			result = activatorDependencyResolution.Resolve(mockDependencyManager);

			Assert.IsNotNull(result);
			Assert.AreEqual(string.Empty, result);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldCreateAndEvaluateNonDefaultContructorOfTypeUsing2ArgsWithAutowireTest()
		{
			ActivatorDependencyResolution activatorDependencyResolution;
			IDependencyManager mockDependencyManager;
			MockFactory mockFactory;
			MockDependantObject result;

			mockFactory = new MockFactory();
			mockDependencyManager = mockFactory.CreateInstance<IDependencyManager>();

			Expect.Once.On(mockDependencyManager).Method("ResolveDependency").With(typeof(MockDependantObject), "named_dep_obj").Will(Return.Value(new MockDependantObject("this_is_named")));
			//Expect.Once.On(mockDependencyManager).Method("ResolveDependency").With(typeof(MockDependantObject), string.Empty).Will(Return.Value(new MockDependantObject("this_is_not_named")));

			activatorDependencyResolution = ActivatorDependencyResolution.OfType<MockDependantObject, MockDependantObject, MockDependantObject>();

			result = (MockDependantObject)activatorDependencyResolution.Resolve(mockDependencyManager);

			Assert.IsNotNull(result);
			Assert.AreEqual(string.Empty, result.Text);
			Assert.IsNotNull(result.Left);
			Assert.IsNotNull(result.Right);
			Assert.AreEqual("this_is_named", result.Left.Text);
			Assert.IsNull(result.Right.Text);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldCreateAndEvaluateTest()
		{
			ActivatorDependencyResolution activatorDependencyResolution;
			IDependencyManager mockDependencyManager;
			Type actualType;
			object result;
			MockFactory mockFactory;

			mockFactory = new MockFactory();
			mockDependencyManager = mockFactory.CreateInstance<IDependencyManager>();

			actualType = typeof(int);

			activatorDependencyResolution = new ActivatorDependencyResolution(actualType);

			Assert.IsNotNull(activatorDependencyResolution);

			result = activatorDependencyResolution.Resolve(mockDependencyManager);

			Assert.IsNotNull(result);
			Assert.AreEqual(0, result);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldCreateAndEvaluateWithAutowireTest()
		{
			ActivatorDependencyResolution activatorDependencyResolution;
			IDependencyManager mockDependencyManager;
			Type actualType;
			Type[] parameterTypes;
			object result;
			MockFactory mockFactory;

			mockFactory = new MockFactory();
			mockDependencyManager = mockFactory.CreateInstance<IDependencyManager>();

			actualType = typeof(int);
			parameterTypes = new Type[] { };

			activatorDependencyResolution = new ActivatorDependencyResolution(actualType, parameterTypes);

			Assert.IsNotNull(activatorDependencyResolution);

			result = activatorDependencyResolution.Resolve(mockDependencyManager);

			Assert.IsNotNull(result);
			Assert.AreEqual(0, result);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullActualTypeCreateTest()
		{
			ActivatorDependencyResolution activatorDependencyResolution;
			Type actualType;

			actualType = null;

			activatorDependencyResolution = new ActivatorDependencyResolution(actualType);
		}

		#endregion
	}
}