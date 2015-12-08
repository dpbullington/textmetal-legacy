/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Reflection;

using NUnit.Framework;

using TextMetal.Middleware.Solder.Interception;
using TextMetal.Middleware.UnitTests.TestingInfrastructure;

using IMockObject = TextMetal.Middleware.UnitTests.TestingInfrastructure.IMockObject;

namespace TextMetal.Middleware.UnitTests.Solder.Interception._
{
	[TestFixture]
	public class ProxyFactoryTests
	{
		#region Constructors/Destructors

		public ProxyFactoryTests()
		{
		}

		#endregion

		#region Methods/Operators

		[Test]
		public void ShouldCreateInstanceWithCacheKeyAndInvokeDynamicFactoryTest()
		{
			MockFactory mockFactory;
			MockProxyFactory factory;
			IMockObject objectContract;
			MockProxyFactory.IInvokeDynamicFactory mockInvokeDynamicFactory;
			IDynamicInvocation mockDynamicInvocation;
			MethodInfo invokedMethodInfo;

			mockFactory = new MockFactory();
			mockDynamicInvocation = mockFactory.CreateInstance<IDynamicInvocation>();
			mockInvokeDynamicFactory = mockFactory.CreateInstance<MockProxyFactory.IInvokeDynamicFactory>();

			invokedMethodInfo = (MethodInfo)MemberInfoProxy<IDisposable>.GetLastMemberInfo(exec => exec.Dispose());

			Expect.Once.On(mockInvokeDynamicFactory).Method("GetDynamicInvoker").With("myCacheKey", typeof(IMockObject)).Will(Return.Value(mockDynamicInvocation));

			factory = new MockProxyFactory();

			Assert.IsNotNull(factory);

			objectContract = factory.CreateInstance("myCacheKey", mockInvokeDynamicFactory);
			Assert.IsNotNull(objectContract);

			factory.Dispose();

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldCreateInstanceWithCacheKeyTest()
		{
			MockProxyFactory factory;
			IMockObject objectContract;

			factory = new MockProxyFactory();

			Assert.IsNotNull(factory);
			Assert.IsFalse(factory.Disposed);

			objectContract = factory.CreateInstance("myCacheKey");

			Assert.IsNotNull(objectContract);

			factory.Dispose();
			Assert.IsTrue(factory.Disposed);
		}

		[Test]
		public void ShouldCreateInstanceWithInvokeDynamicTest()
		{
			MockFactory mockFactory;
			MockProxyFactory factory;
			IMockObject objectContract;
			IDynamicInvocation mockDynamicInvocation;

			mockFactory = new MockFactory();
			mockDynamicInvocation = mockFactory.CreateInstance<IDynamicInvocation>();

			factory = new MockProxyFactory();

			Assert.IsNotNull(factory);

			objectContract = factory.CreateInstance(mockDynamicInvocation);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldEnsureCachingTest()
		{
			MockFactory mockFactory;
			MockProxyFactory factory;
			IMockObject objectContract0, objectContract1, objectContract2, objectContract3;
			MockProxyFactory.IInvokeDynamicFactory mockInvokeDynamicFactory;
			IDynamicInvocation mockDynamicInvocation;
			MethodInfo invokedMethodInfo;

			mockFactory = new MockFactory();
			mockDynamicInvocation = mockFactory.CreateInstance<IDynamicInvocation>();
			mockInvokeDynamicFactory = mockFactory.CreateInstance<MockProxyFactory.IInvokeDynamicFactory>();

			invokedMethodInfo = (MethodInfo)MemberInfoProxy<IDisposable>.GetLastMemberInfo(exec => exec.Dispose());

			Expect.Exactly(1).On(mockInvokeDynamicFactory).Method("GetDynamicInvoker").With("myCacheKey", typeof(IMockObject)).Will(Return.Value(mockDynamicInvocation));
			Expect.Exactly(1).On(mockInvokeDynamicFactory).Method("GetDynamicInvoker").With("myCacheKey_different", typeof(IMockObject)).Will(Return.Value(mockDynamicInvocation));

			factory = new MockProxyFactory();

			Assert.IsNotNull(factory);

			objectContract0 = factory.CreateInstance("myCacheKey", mockInvokeDynamicFactory);
			objectContract1 = factory.CreateInstance("myCacheKey_different", mockInvokeDynamicFactory);
			objectContract2 = factory.CreateInstance("myCacheKey", mockInvokeDynamicFactory);
			objectContract3 = factory.CreateInstance(mockDynamicInvocation);

			Assert.IsNotNull(objectContract0);
			Assert.IsNotNull(objectContract1);
			Assert.IsNotNull(objectContract2);
			Assert.IsNotNull(objectContract3);

			Assert.AreNotSame(objectContract0, objectContract1);
			Assert.AreSame(objectContract0, objectContract2);
			Assert.AreNotSame(objectContract1, objectContract2);

			Assert.AreNotSame(objectContract0, objectContract3);
			Assert.AreNotSame(objectContract1, objectContract3);
			Assert.AreNotSame(objectContract2, objectContract3);

			factory.Dispose();

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldEnsureNoCachingTest()
		{
			MockFactory mockFactory;
			MockProxyFactory factory;
			IMockObject objectContract0, objectContract1, objectContract2;
			MockProxyFactory.IInvokeDynamicFactory mockInvokeDynamicFactory;
			IDynamicInvocation mockDynamicInvocation;
			MethodInfo invokedMethodInfo;

			mockFactory = new MockFactory();
			mockDynamicInvocation = mockFactory.CreateInstance<IDynamicInvocation>();
			mockInvokeDynamicFactory = mockFactory.CreateInstance<MockProxyFactory.IInvokeDynamicFactory>();

			invokedMethodInfo = (MethodInfo)MemberInfoProxy<IDisposable>.GetLastMemberInfo(exec => exec.Dispose());

			Expect.Once.On(mockInvokeDynamicFactory).Method("GetDynamicInvoker").With("myCacheKey", typeof(IMockObject)).Will(Return.Value(mockDynamicInvocation));

			factory = new MockProxyFactory();

			Assert.IsNotNull(factory);

			objectContract0 = factory.CreateInstance(mockDynamicInvocation);
			objectContract1 = factory.CreateInstance(mockDynamicInvocation);
			objectContract2 = factory.CreateInstance("myCacheKey", mockInvokeDynamicFactory);

			Assert.IsNotNull(objectContract0);
			Assert.IsNotNull(objectContract1);
			Assert.IsNotNull(objectContract2);

			Assert.AreNotSame(objectContract0, objectContract1);
			Assert.AreNotSame(objectContract0, objectContract2);
			Assert.AreNotSame(objectContract1, objectContract2);

			factory.Dispose();

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		[ExpectedException(typeof(ObjectDisposedException))]
		public void ShouldFailOnDisposedCreateInstanceWithCacheKeyAndInvokeDynamicFactoryTest()
		{
			MockFactory mockFactory;
			MockProxyFactory factory;
			MockProxyFactory.IInvokeDynamicFactory mockInvokeDynamicFactory;

			mockFactory = new MockFactory();
			mockInvokeDynamicFactory = mockFactory.CreateInstance<MockProxyFactory.IInvokeDynamicFactory>();

			factory = new MockProxyFactory();

			Assert.IsNotNull(factory);

			factory.Dispose();

			factory.CreateInstance("test", mockInvokeDynamicFactory, true);
		}

		[Test]
		[ExpectedException(typeof(ObjectDisposedException))]
		public void ShouldFailOnDisposedCreateInstanceWithCacheKeyTest()
		{
			MockFactory mockFactory;
			MockProxyFactory factory;
			MockProxyFactory.IInvokeDynamicFactory mockInvokeDynamicFactory;

			mockFactory = new MockFactory();
			mockInvokeDynamicFactory = mockFactory.CreateInstance<MockProxyFactory.IInvokeDynamicFactory>();

			factory = new MockProxyFactory();

			Assert.IsNotNull(factory);

			factory.Dispose();

			factory.CreateInstance("test");
		}

		[Test]
		[ExpectedException(typeof(ObjectDisposedException))]
		public void ShouldFailOnDisposedCreateInstanceWithInvokeDynamicTest()
		{
			MockFactory mockFactory;
			MockProxyFactory factory;
			IDynamicInvocation mockDynamicInvocation;

			mockFactory = new MockFactory();
			mockDynamicInvocation = mockFactory.CreateInstance<IDynamicInvocation>();

			factory = new MockProxyFactory();

			Assert.IsNotNull(factory);

			factory.Dispose();

			factory.CreateInstance(mockDynamicInvocation);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullCacheKeyCreateInstanceWithCacheKeyAndInvokeDynamicFactoryTest()
		{
			MockFactory mockFactory;
			MockProxyFactory factory;
			IMockObject objectContract;
			MockProxyFactory.IInvokeDynamicFactory mockInvokeDynamicFactory;

			mockFactory = new MockFactory();
			mockInvokeDynamicFactory = mockFactory.CreateInstance<MockProxyFactory.IInvokeDynamicFactory>();

			factory = new MockProxyFactory();

			Assert.IsNotNull(factory);

			objectContract = factory.CreateInstance(null, mockInvokeDynamicFactory);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullInvokeDynamicFactoryCreateInstanceWithCacheKeyAndInvokeDynamicFactoryTest()
		{
			MockFactory mockFactory;
			MockProxyFactory factory;
			IMockObject objectContract;
			MockProxyFactory.IInvokeDynamicFactory mockInvokeDynamicFactory;

			mockFactory = new MockFactory();
			mockInvokeDynamicFactory = null;

			factory = new MockProxyFactory();

			Assert.IsNotNull(factory);

			objectContract = factory.CreateInstance("myCacheKey", mockInvokeDynamicFactory);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullServiceCreateInstanceWithInvokeDynamicTest()
		{
			MockFactory mockFactory;
			MockProxyFactory factory;
			IMockObject objectContract;
			IDynamicInvocation mockDynamicInvocation;

			mockFactory = new MockFactory();
			mockDynamicInvocation = null;

			factory = new MockProxyFactory();

			Assert.IsNotNull(factory);

			objectContract = factory.CreateInstance(mockDynamicInvocation);
		}

		[Test]
		public void ShouldNotFailOnDoubleDisposeTest()
		{
			MockFactory mockFactory;
			MockProxyFactory factory;
			IMockObject objectContract;
			MockProxyFactory.IInvokeDynamicFactory mockInvokeDynamicFactory;
			IDynamicInvocation mockDynamicInvocation;
			MethodInfo invokedMethodInfo;

			mockFactory = new MockFactory();
			mockDynamicInvocation = mockFactory.CreateInstance<IDynamicInvocation>();
			mockInvokeDynamicFactory = mockFactory.CreateInstance<MockProxyFactory.IInvokeDynamicFactory>();

			invokedMethodInfo = (MethodInfo)MemberInfoProxy<IDisposable>.GetLastMemberInfo(exec => exec.Dispose());

			Expect.Once.On(mockInvokeDynamicFactory).Method("GetDynamicInvoker").With("myCacheKey", typeof(IMockObject)).Will(Return.Value(mockDynamicInvocation));

			factory = new MockProxyFactory();

			Assert.IsNotNull(factory);

			objectContract = factory.CreateInstance("myCacheKey", mockInvokeDynamicFactory);
			Assert.IsNotNull(objectContract);

			factory.Dispose();
			factory.Dispose();

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		#endregion
	}
}