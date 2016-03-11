/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using NMock;

using NUnit.Framework;

using TextMetal.Middleware.Solder.Injection;
using TextMetal.Middleware.Solder.Runtime;
using TextMetal.Middleware.UnitTests.TestingInfrastructure;

namespace TextMetal.Middleware.UnitTests.Solder.Injection._
{
	[TestFixture]
	public class DependencyManagerTests
	{
		#region Constructors/Destructors

		public DependencyManagerTests()
		{
		}

		#endregion

		#region Methods/Operators

		public static void NotMarkedAsAssemblyLoaderEventSinkMethod(AssemblyLoaderEventType assemblyLoaderEventType, AssemblyLoaderContainerContext assemblyLoaderContainerContext)
		{
			throw new Exception();
		}

		[AssemblyLoaderEventSinkMethod]
		public static void ShouldNotMatchSignatureAssemblyLoaderEventSinkMethod(int unused)
		{
			throw new Exception();
		}

		[AssemblyLoaderEventSinkMethod]
		private static void ShouldNotMatchPrivateAssemblyLoaderEventSinkMethod(AssemblyLoaderEventType assemblyLoaderEventType, AssemblyLoaderContainerContext assemblyLoaderContainerContext)
		{
			throw new Exception();
		}

		[AssemblyLoaderEventSinkMethod]
		public static void IsValidAssemblyLoaderEventSinkMethod(AssemblyLoaderEventType assemblyLoaderEventType, AssemblyLoaderContainerContext assemblyLoaderContainerContext)
		{
			if((object)assemblyLoaderContainerContext == null)
				throw new ArgumentNullException(nameof(assemblyLoaderContainerContext));

			switch (assemblyLoaderEventType)
			{
				case AssemblyLoaderEventType.Startup:
					assemblyLoaderContainerContext.DependencyManager.AddResolution<IFormattable>("bob", false, new TransientFactoryMethodDependencyResolution((Func<double>)(() => 1234.56789)));
					break;
				case AssemblyLoaderEventType.Shutdown:
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(assemblyLoaderEventType), assemblyLoaderEventType, null);
			}
		}

		[Test]
		public void ShouldAddResolution1Test()
		{
			DependencyManager dependencyManager;
			MockFactory mockFactory;
			IDependencyResolution mockDependencyResolution;
			string selectorKey;

			mockFactory = new MockFactory();
			mockDependencyResolution = mockFactory.CreateInstance<IDependencyResolution>();

			dependencyManager = new DependencyManager();
			selectorKey = "x";

			dependencyManager.AddResolution<object>(selectorKey, false, mockDependencyResolution);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldAddResolutionTest()
		{
			DependencyManager dependencyManager;
			MockFactory mockFactory;
			IDependencyResolution mockDependencyResolution;
			Type targetType;
			string selectorKey;

			mockFactory = new MockFactory();
			mockDependencyResolution = mockFactory.CreateInstance<IDependencyResolution>();

			dependencyManager = new DependencyManager();
			targetType = typeof(object);
			selectorKey = "x";

			dependencyManager.AddResolution(targetType, selectorKey, false, mockDependencyResolution);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldCheckIfHasResolution1Test()
		{
			DependencyManager dependencyManager;
			MockFactory mockFactory;
			IDependencyResolution mockDependencyResolution;
			string selectorKey;
			bool result;

			mockFactory = new MockFactory();
			mockDependencyResolution = mockFactory.CreateInstance<IDependencyResolution>();

			Expect.On(mockDependencyResolution).One.Method(m => m.Dispose());

			dependencyManager = new DependencyManager();

			result = dependencyManager.HasTypeResolution<IDisposable>(null, false);
			Assert.IsFalse(result);

			result = dependencyManager.HasTypeResolution<IDisposable>(string.Empty, false);
			Assert.IsFalse(result);

			result = dependencyManager.HasTypeResolution<IDisposable>("x", false);
			Assert.IsFalse(result);

			selectorKey = "x";
			dependencyManager.AddResolution<IDisposable>(selectorKey, false, mockDependencyResolution);

			result = dependencyManager.HasTypeResolution<IDisposable>(null, false);
			Assert.IsTrue(result);

			result = dependencyManager.HasTypeResolution<IDisposable>(string.Empty, false);
			Assert.IsFalse(result);

			result = dependencyManager.HasTypeResolution<IDisposable>("x", false);
			Assert.IsTrue(result);

			dependencyManager.ClearAllResolutions();

			result = dependencyManager.HasTypeResolution<IDisposable>(null, false);
			Assert.IsFalse(result);

			result = dependencyManager.HasTypeResolution<IDisposable>(string.Empty, false);
			Assert.IsFalse(result);

			result = dependencyManager.HasTypeResolution<IDisposable>("x", false);
			Assert.IsFalse(result);

			selectorKey = string.Empty;
			dependencyManager.AddResolution<IDisposable>(selectorKey, false, mockDependencyResolution);

			result = dependencyManager.HasTypeResolution<IDisposable>(null, false);
			Assert.IsTrue(result);

			result = dependencyManager.HasTypeResolution<IDisposable>(string.Empty, false);
			Assert.IsTrue(result);

			result = dependencyManager.HasTypeResolution<IDisposable>("x", false);
			Assert.IsFalse(result);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldCheckIfHasResolutionTest()
		{
			DependencyManager dependencyManager;
			MockFactory mockFactory;
			IDependencyResolution mockDependencyResolution;
			Type targetType;
			string selectorKey;
			bool result;

			mockFactory = new MockFactory();
			mockDependencyResolution = mockFactory.CreateInstance<IDependencyResolution>();

			Expect.On(mockDependencyResolution).One.Method(m => m.Dispose());

			targetType = typeof(IDisposable);
			dependencyManager = new DependencyManager();

			result = dependencyManager.HasTypeResolution(targetType, null, false);
			Assert.IsFalse(result);

			result = dependencyManager.HasTypeResolution(targetType, string.Empty, false);
			Assert.IsFalse(result);

			result = dependencyManager.HasTypeResolution(targetType, "x", false);
			Assert.IsFalse(result);

			selectorKey = "x";
			dependencyManager.AddResolution(targetType, selectorKey, false, mockDependencyResolution);

			result = dependencyManager.HasTypeResolution(targetType, null, false);
			Assert.IsTrue(result);

			result = dependencyManager.HasTypeResolution(targetType, string.Empty, false);
			Assert.IsFalse(result);

			result = dependencyManager.HasTypeResolution(targetType, "x", false);
			Assert.IsTrue(result);

			dependencyManager.ClearAllResolutions();

			result = dependencyManager.HasTypeResolution(targetType, null, false);
			Assert.IsFalse(result);

			result = dependencyManager.HasTypeResolution(targetType, string.Empty, false);
			Assert.IsFalse(result);

			result = dependencyManager.HasTypeResolution(targetType, "x", false);
			Assert.IsFalse(result);

			selectorKey = string.Empty;
			dependencyManager.AddResolution(targetType, selectorKey, false, mockDependencyResolution);

			result = dependencyManager.HasTypeResolution(targetType, null, false);
			Assert.IsTrue(result);

			result = dependencyManager.HasTypeResolution(targetType, string.Empty, false);
			Assert.IsTrue(result);

			result = dependencyManager.HasTypeResolution(targetType, "x", false);
			Assert.IsFalse(result);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldClearAllResolutionsTest()
		{
			DependencyManager dependencyManager;
			MockFactory mockFactory;
			IDependencyResolution mockDependencyResolution;
			Type targetType;
			string selectorKey;
			bool result;

			mockFactory = new MockFactory();
			mockDependencyResolution = mockFactory.CreateInstance<IDependencyResolution>();

			Expect.On(mockDependencyResolution).One.Method(m => m.Dispose());

			dependencyManager = new DependencyManager();
			targetType = typeof(object);
			selectorKey = "ddd";

			result = dependencyManager.ClearAllResolutions();

			Assert.IsFalse(result);

			dependencyManager.AddResolution(targetType, selectorKey, false, mockDependencyResolution);

			result = dependencyManager.ClearAllResolutions();

			Assert.IsTrue(result);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldClearTypeResolutions1Test()
		{
			DependencyManager dependencyManager;
			MockFactory mockFactory;
			IDependencyResolution mockDependencyResolution;
			string selectorKey;
			bool result;

			mockFactory = new MockFactory();
			mockDependencyResolution = mockFactory.CreateInstance<IDependencyResolution>();

			Expect.On(mockDependencyResolution).One.Method(m => m.Dispose());

			dependencyManager = new DependencyManager();
			selectorKey = "ddd";

			result = dependencyManager.ClearTypeResolutions<object>(false);

			Assert.IsFalse(result);

			dependencyManager.AddResolution<object>(selectorKey, false, mockDependencyResolution);

			result = dependencyManager.ClearTypeResolutions<object>(false);

			Assert.IsTrue(result);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldClearTypeResolutionsTest()
		{
			DependencyManager dependencyManager;
			MockFactory mockFactory;
			IDependencyResolution mockDependencyResolution;
			Type targetType;
			string selectorKey;
			bool result;

			mockFactory = new MockFactory();
			mockDependencyResolution = mockFactory.CreateInstance<IDependencyResolution>();

			Expect.On(mockDependencyResolution).One.Method(m => m.Dispose());

			dependencyManager = new DependencyManager();
			targetType = typeof(object);
			selectorKey = "ddd";

			result = dependencyManager.ClearTypeResolutions(targetType, false);

			Assert.IsFalse(result);

			dependencyManager.AddResolution(targetType, selectorKey, false, mockDependencyResolution);

			result = dependencyManager.ClearTypeResolutions(targetType, false);

			Assert.IsTrue(result);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldCreateTest()
		{
			DependencyManager dependencyManager;

			dependencyManager = new DependencyManager();
		}

		[Test]
		[ExpectedException(typeof(ObjectDisposedException))]
		public void ShouldFailOnDisposedAddResolution1Test()
		{
			DependencyManager dependencyManager;
			MockFactory mockFactory;
			IDependencyResolution mockDependencyResolution;
			IDependencyManager _unusedDependencyManager = null;
			string selectorKey;

			mockFactory = new MockFactory();
			mockDependencyResolution = mockFactory.CreateInstance<IDependencyResolution>();

			dependencyManager = new DependencyManager();
			selectorKey = "x";

			Expect.On(mockDependencyResolution).One.Method(x => x.Resolve(_unusedDependencyManager)).With(dependencyManager).Will(Return.Value(null));

			dependencyManager.Dispose();
			dependencyManager.AddResolution<object>(selectorKey, false, mockDependencyResolution);
		}

		[Test]
		[ExpectedException(typeof(ObjectDisposedException))]
		public void ShouldFailOnDisposedAddResolutionTest()
		{
			DependencyManager dependencyManager;
			MockFactory mockFactory;
			IDependencyResolution mockDependencyResolution;
			IDependencyManager _unusedDependencyManager = null;
			Type targetType;
			string selectorKey;

			mockFactory = new MockFactory();
			mockDependencyResolution = mockFactory.CreateInstance<IDependencyResolution>();

			dependencyManager = new DependencyManager();
			targetType = typeof(object);
			selectorKey = "x";

			Expect.On(mockDependencyResolution).One.Method(x => x.Resolve(_unusedDependencyManager)).With(dependencyManager).Will(Return.Value(null));

			dependencyManager.Dispose();
			dependencyManager.AddResolution(targetType, selectorKey, false, mockDependencyResolution);
		}

		[Test]
		[ExpectedException(typeof(ObjectDisposedException))]
		public void ShouldFailOnDisposedCheckIfHasResolution1Test()
		{
			DependencyManager dependencyManager;
			MockFactory mockFactory;
			IDependencyResolution mockDependencyResolution;

			mockFactory = new MockFactory();
			mockDependencyResolution = mockFactory.CreateInstance<IDependencyResolution>();

			dependencyManager = new DependencyManager();

			dependencyManager.Dispose();
			dependencyManager.HasTypeResolution<object>(null, false);
		}

		[Test]
		[ExpectedException(typeof(ObjectDisposedException))]
		public void ShouldFailOnDisposedCheckIfHasResolutionTest()
		{
			DependencyManager dependencyManager;
			MockFactory mockFactory;
			IDependencyResolution mockDependencyResolution;
			Type targetType;

			mockFactory = new MockFactory();
			mockDependencyResolution = mockFactory.CreateInstance<IDependencyResolution>();

			dependencyManager = new DependencyManager();
			targetType = typeof(object);

			dependencyManager.Dispose();
			dependencyManager.HasTypeResolution(targetType, null, false);
		}

		[Test]
		[ExpectedException(typeof(ObjectDisposedException))]
		public void ShouldFailOnDisposedClearAllResolutionsTest()
		{
			DependencyManager dependencyManager;
			MockFactory mockFactory;
			IDependencyResolution mockDependencyResolution;
			bool result;

			mockFactory = new MockFactory();
			mockDependencyResolution = mockFactory.CreateInstance<IDependencyResolution>();

			dependencyManager = new DependencyManager();

			dependencyManager.Dispose();
			result = dependencyManager.ClearAllResolutions();
		}

		[Test]
		[ExpectedException(typeof(ObjectDisposedException))]
		public void ShouldFailOnDisposedClearTypeResolutions1Test()
		{
			DependencyManager dependencyManager;
			MockFactory mockFactory;
			IDependencyResolution mockDependencyResolution;
			bool result;

			mockFactory = new MockFactory();
			mockDependencyResolution = mockFactory.CreateInstance<IDependencyResolution>();

			dependencyManager = new DependencyManager();

			dependencyManager.Dispose();
			result = dependencyManager.ClearTypeResolutions<object>(false);
		}

		[Test]
		[ExpectedException(typeof(ObjectDisposedException))]
		public void ShouldFailOnDisposedClearTypeResolutionsTest()
		{
			DependencyManager dependencyManager;
			MockFactory mockFactory;
			IDependencyResolution mockDependencyResolution;
			Type targetType;
			bool result;

			mockFactory = new MockFactory();
			mockDependencyResolution = mockFactory.CreateInstance<IDependencyResolution>();

			dependencyManager = new DependencyManager();
			targetType = typeof(object);

			dependencyManager.Dispose();
			result = dependencyManager.ClearTypeResolutions(targetType, false);
		}

		[Test]
		[ExpectedException(typeof(ObjectDisposedException))]
		public void ShouldFailOnDisposedRemoveResolution1Test()
		{
			DependencyManager dependencyManager;
			MockFactory mockFactory;
			IDependencyResolution mockDependencyResolution;
			string selectorKey;

			mockFactory = new MockFactory();
			mockDependencyResolution = mockFactory.CreateInstance<IDependencyResolution>();

			dependencyManager = new DependencyManager();
			selectorKey = "x";

			dependencyManager.Dispose();
			dependencyManager.RemoveResolution<object>(selectorKey, false);
		}

		[Test]
		[ExpectedException(typeof(ObjectDisposedException))]
		public void ShouldFailOnDisposedRemoveResolutionTest()
		{
			DependencyManager dependencyManager;
			MockFactory mockFactory;
			IDependencyResolution mockDependencyResolution;
			Type targetType;
			string selectorKey;

			mockFactory = new MockFactory();
			mockDependencyResolution = mockFactory.CreateInstance<IDependencyResolution>();

			dependencyManager = new DependencyManager();
			targetType = typeof(object);
			selectorKey = "x";

			dependencyManager.Dispose();
			dependencyManager.RemoveResolution(targetType, selectorKey, false);
		}

		[Test]
		[ExpectedException(typeof(ObjectDisposedException))]
		public void ShouldFailOnDisposedResolveDependency1Test()
		{
			DependencyManager dependencyManager;
			MockFactory mockFactory;
			IDependencyResolution mockDependencyResolution;
			string selectorKey;
			object value;

			mockFactory = new MockFactory();
			mockDependencyResolution = mockFactory.CreateInstance<IDependencyResolution>();

			dependencyManager = new DependencyManager();
			selectorKey = "x";

			dependencyManager.Dispose();
			value = dependencyManager.ResolveDependency<object>(selectorKey, false);
		}

		[Test]
		[ExpectedException(typeof(ObjectDisposedException))]
		public void ShouldFailOnDisposedResolveDependencyTest()
		{
			DependencyManager dependencyManager;
			MockFactory mockFactory;
			IDependencyResolution mockDependencyResolution;
			Type targetType;
			string selectorKey;
			object value;

			mockFactory = new MockFactory();
			mockDependencyResolution = mockFactory.CreateInstance<IDependencyResolution>();

			dependencyManager = new DependencyManager();
			targetType = typeof(object);
			selectorKey = "x";

			dependencyManager.Dispose();
			value = dependencyManager.ResolveDependency(targetType, selectorKey, false);
		}

		[Test]
		[ExpectedException(typeof(DependencyException))]
		public void ShouldFailOnKeyExistsAddResolutionTest()
		{
			DependencyManager dependencyManager;
			MockFactory mockFactory;
			IDependencyResolution mockDependencyResolution;
			IDependencyManager _unusedDependencyManager = null;
			Type targetType;
			string selectorKey;

			mockFactory = new MockFactory();
			mockDependencyResolution = mockFactory.CreateInstance<IDependencyResolution>();

			dependencyManager = new DependencyManager();
			targetType = typeof(object);
			selectorKey = "x";

			Expect.On(mockDependencyResolution).One.Method(x => x.Resolve(_unusedDependencyManager)).With(dependencyManager).Will(Return.Value(null));

			dependencyManager.AddResolution(targetType, selectorKey, false, mockDependencyResolution);
			dependencyManager.AddResolution(targetType, selectorKey, false, mockDependencyResolution);
		}

		[Test]
		[ExpectedException(typeof(DependencyException))]
		public void ShouldFailOnKeyNotExistsRemoveResolutionTest()
		{
			DependencyManager dependencyManager;
			MockFactory mockFactory;
			IDependencyResolution mockDependencyResolution;
			Type targetType;
			string selectorKey;

			mockFactory = new MockFactory();
			mockDependencyResolution = mockFactory.CreateInstance<IDependencyResolution>();

			dependencyManager = new DependencyManager();
			targetType = typeof(object);
			selectorKey = "zzz";

			dependencyManager.RemoveResolution(targetType, selectorKey, false);
		}

		[Test]
		[ExpectedException(typeof(DependencyException))]
		public void ShouldFailOnKeyNotExistsResolveDependencyTest()
		{
			DependencyManager dependencyManager;
			MockFactory mockFactory;
			IDependencyResolution mockDependencyResolution;
			Type targetType;
			string selectorKey;
			object value;

			mockFactory = new MockFactory();
			mockDependencyResolution = mockFactory.CreateInstance<IDependencyResolution>();

			dependencyManager = new DependencyManager();
			targetType = typeof(object);
			selectorKey = "xxx";

			value = dependencyManager.ResolveDependency(targetType, selectorKey, false);
		}

		[Test]
		[ExpectedException(typeof(DependencyException))]
		public void ShouldFailOnNotAssignableResolveDependencyTest()
		{
			DependencyManager dependencyManager;
			MockFactory mockFactory;
			IDependencyResolution mockDependencyResolution;
			IDependencyManager _unusedDependencyManager = null;
			Type targetType;
			string selectorKey;
			object value;

			dependencyManager = new DependencyManager();

			mockFactory = new MockFactory();
			mockDependencyResolution = mockFactory.CreateInstance<IDependencyResolution>();

			Expect.On(mockDependencyResolution).One.Method(x => x.Resolve(_unusedDependencyManager)).With(dependencyManager).Will(Return.Value(1));

			targetType = typeof(IDisposable);
			selectorKey = "yyy";

			dependencyManager.AddResolution(targetType, selectorKey, false, mockDependencyResolution);
			value = dependencyManager.ResolveDependency<IDisposable>(selectorKey, false);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullDependencyResolutionAddResolution1Test()
		{
			DependencyManager dependencyManager;
			MockFactory mockFactory;
			IDependencyResolution mockDependencyResolution;
			string selectorKey;

			mockFactory = new MockFactory();
			mockDependencyResolution = null;

			dependencyManager = new DependencyManager();
			selectorKey = "x";

			dependencyManager.AddResolution<object>(selectorKey, false, mockDependencyResolution);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullDependencyResolutionAddResolutionTest()
		{
			DependencyManager dependencyManager;
			MockFactory mockFactory;
			IDependencyResolution mockDependencyResolution;
			Type targetType;
			string selectorKey;

			mockFactory = new MockFactory();
			mockDependencyResolution = null;

			dependencyManager = new DependencyManager();
			targetType = typeof(object);
			selectorKey = "x";

			dependencyManager.AddResolution(targetType, selectorKey, false, mockDependencyResolution);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyAddResolution1Test()
		{
			DependencyManager dependencyManager;
			MockFactory mockFactory;
			IDependencyResolution mockDependencyResolution;
			IDependencyManager _unusedDependencyManager = null;
			string selectorKey;

			mockFactory = new MockFactory();
			mockDependencyResolution = mockFactory.CreateInstance<IDependencyResolution>();

			dependencyManager = new DependencyManager();
			selectorKey = null;

			Expect.On(mockDependencyResolution).One.Method(x => x.Resolve(_unusedDependencyManager)).With(dependencyManager).Will(Return.Value(null));

			dependencyManager.AddResolution<object>(selectorKey, false, mockDependencyResolution);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyAddResolutionTest()
		{
			DependencyManager dependencyManager;
			MockFactory mockFactory;
			IDependencyResolution mockDependencyResolution;
			IDependencyManager _unusedDependencyManager = null;
			Type targetType;
			string selectorKey;

			mockFactory = new MockFactory();
			mockDependencyResolution = mockFactory.CreateInstance<IDependencyResolution>();

			dependencyManager = new DependencyManager();
			targetType = typeof(object);
			selectorKey = null;

			Expect.On(mockDependencyResolution).One.Method(x => x.Resolve(_unusedDependencyManager)).With(dependencyManager).Will(Return.Value(null));

			dependencyManager.AddResolution(targetType, selectorKey, false, mockDependencyResolution);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyRemoveResolution1Test()
		{
			DependencyManager dependencyManager;
			MockFactory mockFactory;
			IDependencyResolution mockDependencyResolution;
			string selectorKey;

			mockFactory = new MockFactory();
			mockDependencyResolution = mockFactory.CreateInstance<IDependencyResolution>();

			dependencyManager = new DependencyManager();
			selectorKey = null;

			dependencyManager.RemoveResolution<object>(selectorKey, false);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyRemoveResolutionTest()
		{
			DependencyManager dependencyManager;
			MockFactory mockFactory;
			IDependencyResolution mockDependencyResolution;
			Type targetType;
			string selectorKey;

			mockFactory = new MockFactory();
			mockDependencyResolution = mockFactory.CreateInstance<IDependencyResolution>();

			dependencyManager = new DependencyManager();
			targetType = typeof(object);
			selectorKey = null;

			dependencyManager.RemoveResolution(targetType, selectorKey, false);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyResolveDependency1Test()
		{
			DependencyManager dependencyManager;
			MockFactory mockFactory;
			IDependencyResolution mockDependencyResolution;
			string selectorKey;
			object value;

			mockFactory = new MockFactory();
			mockDependencyResolution = mockFactory.CreateInstance<IDependencyResolution>();

			dependencyManager = new DependencyManager();
			selectorKey = null;

			value = dependencyManager.ResolveDependency<object>(selectorKey, false);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullKeyResolveDependencyTest()
		{
			DependencyManager dependencyManager;
			MockFactory mockFactory;
			IDependencyResolution mockDependencyResolution;
			Type targetType;
			string selectorKey;
			object value;

			mockFactory = new MockFactory();
			mockDependencyResolution = mockFactory.CreateInstance<IDependencyResolution>();

			dependencyManager = new DependencyManager();
			targetType = typeof(object);
			selectorKey = null;

			value = dependencyManager.ResolveDependency(targetType, selectorKey, false);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullTypeAddResolutionTest()
		{
			DependencyManager dependencyManager;
			MockFactory mockFactory;
			IDependencyResolution mockDependencyResolution;
			IDependencyManager _unusedDependencyManager = null;
			Type targetType;
			string selectorKey;

			mockFactory = new MockFactory();
			mockDependencyResolution = mockFactory.CreateInstance<IDependencyResolution>();

			dependencyManager = new DependencyManager();
			targetType = null;
			selectorKey = "x";

			Expect.On(mockDependencyResolution).One.Method(x => x.Resolve(_unusedDependencyManager)).With(dependencyManager).Will(Return.Value(null));

			dependencyManager.AddResolution(targetType, selectorKey, false, mockDependencyResolution);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullTypeCheckIfHasResolutionTest()
		{
			DependencyManager dependencyManager;
			MockFactory mockFactory;
			IDependencyResolution mockDependencyResolution;
			Type targetType;

			mockFactory = new MockFactory();
			mockDependencyResolution = mockFactory.CreateInstance<IDependencyResolution>();

			dependencyManager = new DependencyManager();
			targetType = null;

			dependencyManager.HasTypeResolution(targetType, null, false);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullTypeClearTypeResolutionsTest()
		{
			DependencyManager dependencyManager;
			MockFactory mockFactory;
			IDependencyResolution mockDependencyResolution;
			Type targetType;

			mockFactory = new MockFactory();
			mockDependencyResolution = mockFactory.CreateInstance<IDependencyResolution>();

			dependencyManager = new DependencyManager();
			targetType = null;

			dependencyManager.ClearTypeResolutions(targetType, false);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullTypeRemoveResolutionTest()
		{
			DependencyManager dependencyManager;
			MockFactory mockFactory;
			IDependencyResolution mockDependencyResolution;
			Type targetType;
			string selectorKey;

			mockFactory = new MockFactory();
			mockDependencyResolution = mockFactory.CreateInstance<IDependencyResolution>();

			dependencyManager = new DependencyManager();
			targetType = null;
			selectorKey = "x";

			dependencyManager.RemoveResolution(targetType, selectorKey, false);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullTypeResolveDependencyTest()
		{
			DependencyManager dependencyManager;
			MockFactory mockFactory;
			IDependencyResolution mockDependencyResolution;
			Type targetType;
			string selectorKey;
			object value;

			mockFactory = new MockFactory();
			mockDependencyResolution = mockFactory.CreateInstance<IDependencyResolution>();

			dependencyManager = new DependencyManager();
			targetType = null;
			selectorKey = "x";

			value = dependencyManager.ResolveDependency(targetType, selectorKey, false);
		}

		[Test]
		public void ShouldNotFailOnDoubleDisposedAddResolution1Test()
		{
			DependencyManager dependencyManager;
			MockFactory mockFactory;
			IDependencyResolution mockDependencyResolution;

			mockFactory = new MockFactory();
			mockDependencyResolution = mockFactory.CreateInstance<IDependencyResolution>();

			dependencyManager = new DependencyManager();

			Assert.IsFalse(dependencyManager.Disposed);

			dependencyManager.Dispose();

			Assert.IsTrue(dependencyManager.Disposed);

			dependencyManager.Dispose();

			Assert.IsTrue(dependencyManager.Disposed);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldRemoveResolution1Test()
		{
			DependencyManager dependencyManager;
			MockFactory mockFactory;
			IDependencyResolution mockDependencyResolution;
			string selectorKey;

			mockFactory = new MockFactory();
			mockDependencyResolution = mockFactory.CreateInstance<IDependencyResolution>();

			Expect.On(mockDependencyResolution).One.Method(m => m.Dispose());

			dependencyManager = new DependencyManager();
			selectorKey = "x";

			dependencyManager.AddResolution<object>(selectorKey, false, mockDependencyResolution);
			dependencyManager.RemoveResolution<object>(selectorKey, false);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldRemoveResolutionTest()
		{
			DependencyManager dependencyManager;
			MockFactory mockFactory;
			IDependencyResolution mockDependencyResolution;
			Type targetType;
			string selectorKey;

			mockFactory = new MockFactory();
			mockDependencyResolution = mockFactory.CreateInstance<IDependencyResolution>();

			Expect.On(mockDependencyResolution).One.Method(m => m.Dispose());

			dependencyManager = new DependencyManager();
			targetType = typeof(object);
			selectorKey = "x";

			dependencyManager.AddResolution(targetType, selectorKey, false, mockDependencyResolution);
			dependencyManager.RemoveResolution(targetType, selectorKey, false);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldResolveDependency1Test()
		{
			DependencyManager dependencyManager;
			MockFactory mockFactory;
			IDependencyResolution mockDependencyResolution;
			IDependencyManager _unusedDependencyManager = null;
			string selectorKey;
			object value;

			dependencyManager = new DependencyManager();

			mockFactory = new MockFactory();
			mockDependencyResolution = mockFactory.CreateInstance<IDependencyResolution>();

			Expect.On(mockDependencyResolution).One.Method(x => x.Resolve(_unusedDependencyManager)).With(dependencyManager).Will(Return.Value(1));

			selectorKey = "x";

			dependencyManager.AddResolution<object>(selectorKey, false, mockDependencyResolution);
			value = dependencyManager.ResolveDependency<object>(selectorKey, false);

			Assert.IsNotNull(value);
			Assert.AreEqual(1, value);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldResolveDependencyTest()
		{
			DependencyManager dependencyManager;
			MockFactory mockFactory;
			IDependencyResolution mockDependencyResolution;
			IDependencyManager _unusedDependencyManager = null;
			Type targetType;
			string selectorKey;
			object value;

			dependencyManager = new DependencyManager();

			mockFactory = new MockFactory();
			mockDependencyResolution = mockFactory.CreateInstance<IDependencyResolution>();

			Expect.On(mockDependencyResolution).One.Method(x => x.Resolve(_unusedDependencyManager)).With(dependencyManager).Will(Return.Value(1));

			targetType = typeof(object);
			selectorKey = "x";

			dependencyManager.AddResolution(targetType, selectorKey, false, mockDependencyResolution);
			value = dependencyManager.ResolveDependency(targetType, selectorKey, false);

			Assert.IsNotNull(value);
			Assert.AreEqual(1, value);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldVerifyAutoWiringTest()
		{
			IFormattable formattable;

			AssemblyLoaderContainerContext.TheOnlyAllowedInstance.ScanAssembly<DependencyManagerTests>();

			formattable = AssemblyLoaderContainerContext.TheOnlyAllowedInstance.DependencyManager.ResolveDependency<IFormattable>("bob", false);

			Assert.IsNotNull(formattable);
			Assert.IsInstanceOf<double>(formattable);
			Assert.AreEqual(1234.56789, (double)formattable);
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		private static class Inner
		{
			#region Methods/Operators

			[AssemblyLoaderEventSinkMethod]
			public static void ShouldNotMatchPrivateType()
			{
				throw new Exception();
			}

			#endregion
		}

		#endregion
	}
}