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
	public class TransientActivatorAutoWiringDependencyResolutionTests
	{
		#region Constructors/Destructors

		public TransientActivatorAutoWiringDependencyResolutionTests()
		{
		}

		#endregion

		#region Methods/Operators

		[Test]
		public void ShouldCreateAndEvaluateNonDefaultContructorOfTypeUsing1ArgsTest()
		{
			TransientActivatorAutoWiringDependencyResolution<DateTime> transientActivatorAutoWiringDependencyResolution;
			IDependencyManager mockDependencyManager;
			MockFactory mockFactory;
			object result;
			Type _unusedType = null;
			string _unusedString = null;
			bool _unusedBoolean = false;

			mockFactory = new MockFactory();
			mockDependencyManager = mockFactory.CreateInstance<IDependencyManager>();

			Expect.On(mockDependencyManager).Any.Method(m => m.HasTypeResolution(_unusedType, _unusedString, _unusedBoolean)).WithAnyArguments().Will(Return.Value(false));

			transientActivatorAutoWiringDependencyResolution = new TransientActivatorAutoWiringDependencyResolution<DateTime>();

			result = transientActivatorAutoWiringDependencyResolution.Resolve(mockDependencyManager, typeof(object), string.Empty);

			Assert.IsNotNull(result);
			Assert.AreEqual(new DateTime(0), result);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldCreateAndEvaluateNonDefaultContructorOfTypeUsing1ArgsWithAutowireTest()
		{
			TransientActivatorAutoWiringDependencyResolution<MockDependantObject> transientActivatorAutoWiringDependencyResolution;
			IDependencyManager mockDependencyManager;
			MockFactory mockFactory;
			MockDependantObject result;
			string _unusedString = null;
			bool _unusedBoolean = false;
			Type _unusedType = null;

			mockFactory = new MockFactory();
			mockDependencyManager = mockFactory.CreateInstance<IDependencyManager>();

			Expect.On(mockDependencyManager).Any.Method(m => m.HasTypeResolution(_unusedType, _unusedString, _unusedBoolean)).WithAnyArguments().Will(Return.Value(false));

			Expect.On(mockDependencyManager).One.Method(m => m.ResolveDependency<string>(_unusedString, _unusedBoolean)).With(string.Empty, true).Will(Return.Value("turing tarpit"));

			transientActivatorAutoWiringDependencyResolution = new TransientActivatorAutoWiringDependencyResolution<MockDependantObject>();

			result = (MockDependantObject)transientActivatorAutoWiringDependencyResolution.Resolve(mockDependencyManager, typeof(object), string.Empty);

			Assert.IsNotNull(result);
			Assert.AreEqual("turing tarpit", result.Text);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldCreateAndEvaluateNonDefaultContructorOfTypeUsing2ArgsTest()
		{
			TransientActivatorAutoWiringDependencyResolution<string> transientActivatorAutoWiringDependencyResolution;
			IDependencyManager mockDependencyManager;
			MockFactory mockFactory;
			object result;
			string _unusedString = null;
			bool _unusedBoolean = false;
			Type _unusedType = null;


			mockFactory = new MockFactory();
			mockDependencyManager = mockFactory.CreateInstance<IDependencyManager>();

			Expect.On(mockDependencyManager).Any.Method(m => m.HasTypeResolution(_unusedType, _unusedString, _unusedBoolean)).WithAnyArguments().Will(Return.Value(false));

			transientActivatorAutoWiringDependencyResolution = new TransientActivatorAutoWiringDependencyResolution<string>();

			result = transientActivatorAutoWiringDependencyResolution.Resolve(mockDependencyManager, typeof(object), string.Empty);

			Assert.IsNotNull(result);
			Assert.AreEqual(string.Empty, result);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldCreateAndEvaluateNonDefaultContructorOfTypeUsing2ArgsWithAutowireTest()
		{
			TransientActivatorAutoWiringDependencyResolution<MockDependantObject> transientActivatorAutoWiringDependencyResolution;
			IDependencyManager mockDependencyManager;
			MockFactory mockFactory;
			MockDependantObject result;
			string _unusedString = null;
			bool _unusedBoolean = false;
			Type _unusedType = null;

			mockFactory = new MockFactory();
			mockDependencyManager = mockFactory.CreateInstance<IDependencyManager>();

			Expect.On(mockDependencyManager).Any.Method(m => m.HasTypeResolution(_unusedType, _unusedString, _unusedBoolean)).WithAnyArguments().Will(Return.Value(false));

			Expect.On(mockDependencyManager).One.Method(m => m.ResolveDependency<MockDependantObject>(_unusedString, _unusedBoolean)).With("named_dep_obj", true).Will(Return.Value(new MockDependantObject("this_is_named")));

			transientActivatorAutoWiringDependencyResolution = new TransientActivatorAutoWiringDependencyResolution<MockDependantObject>();

			result = (MockDependantObject)transientActivatorAutoWiringDependencyResolution.Resolve(mockDependencyManager, typeof(object), string.Empty);

			Assert.IsNotNull(result);
			Assert.AreEqual(string.Empty, result.Text);
			Assert.IsNotNull(result.Left);
			Assert.IsNotNull(result.Right);
			Assert.AreEqual("this_is_named", result.Left.Text);
			Assert.IsNull(result.Right.Text);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldCreateAndEvaluateValueTypeTest()
		{
			TransientActivatorAutoWiringDependencyResolution transientActivatorAutoWiringDependencyResolution;
			IDependencyManager mockDependencyManager;
			Type actualType;
			object result;
			MockFactory mockFactory;

			mockFactory = new MockFactory();
			mockDependencyManager = mockFactory.CreateInstance<IDependencyManager>();

			actualType = typeof(int);

			transientActivatorAutoWiringDependencyResolution = new TransientActivatorAutoWiringDependencyResolution(actualType);

			result = transientActivatorAutoWiringDependencyResolution.Resolve(mockDependencyManager, typeof(object), string.Empty);

			Assert.IsNotNull(result);
			Assert.AreEqual(default(int), result);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldCreateAndEvaluateWithAutowireTest()
		{
			TransientActivatorAutoWiringDependencyResolution<int> transientActivatorAutoWiringDependencyResolution;
			IDependencyManager mockDependencyManager;
			object result;
			MockFactory mockFactory;

			mockFactory = new MockFactory();
			mockDependencyManager = mockFactory.CreateInstance<IDependencyManager>();

			transientActivatorAutoWiringDependencyResolution = new TransientActivatorAutoWiringDependencyResolution<int>();

			result = transientActivatorAutoWiringDependencyResolution.Resolve(mockDependencyManager, typeof(object), string.Empty);

			Assert.IsNotNull(result);
			Assert.AreEqual(0, result);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullActualTypeCreateTest()
		{
			TransientActivatorAutoWiringDependencyResolution transientActivatorAutoWiringDependencyResolution;
			Type actualType;

			actualType = null;

			transientActivatorAutoWiringDependencyResolution = new TransientActivatorAutoWiringDependencyResolution(actualType);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullDependencyManagerResolveTest()
		{
			TransientActivatorAutoWiringDependencyResolution transientActivatorAutoWiringDependencyResolution;
			IDependencyManager mockDependencyManager;
			object result;
			MockFactory mockFactory;

			mockFactory = new MockFactory();
			mockDependencyManager = null;

			transientActivatorAutoWiringDependencyResolution = new TransientActivatorAutoWiringDependencyResolution(typeof(int));

			result = transientActivatorAutoWiringDependencyResolution.Resolve(mockDependencyManager, typeof(object), string.Empty);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyResolveTest()
		{
			TransientActivatorAutoWiringDependencyResolution transientActivatorAutoWiringDependencyResolution;
			IDependencyManager mockDependencyManager;
			object result;
			MockFactory mockFactory;

			mockFactory = new MockFactory();
			mockDependencyManager = mockFactory.CreateInstance<IDependencyManager>();

			transientActivatorAutoWiringDependencyResolution = new TransientActivatorAutoWiringDependencyResolution(typeof(int));

			result = transientActivatorAutoWiringDependencyResolution.Resolve(mockDependencyManager, typeof(object), null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullTypeResolveTest()
		{
			TransientActivatorAutoWiringDependencyResolution transientActivatorAutoWiringDependencyResolution;
			IDependencyManager mockDependencyManager;
			object result;
			MockFactory mockFactory;

			mockFactory = new MockFactory();
			mockDependencyManager = mockFactory.CreateInstance<IDependencyManager>();

			transientActivatorAutoWiringDependencyResolution = new TransientActivatorAutoWiringDependencyResolution(typeof(int));

			result = transientActivatorAutoWiringDependencyResolution.Resolve(mockDependencyManager, null, string.Empty);
		}

		#endregion
	}
}