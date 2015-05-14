/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;

using NMock;
using NMock.Matchers;

using NUnit.Framework;

using TextMetal.Middleware.Solder.SoC;
using TextMetal.Middleware.Solder.SoC.RemotingImpl;
using TextMetal.Middleware.Testing;

using IMockObject = TextMetal.Middleware.UnitTests.TestingInfrastructure.IMockObject;

namespace TextMetal.Middleware.UnitTests.Solder.SoC.RemotingImpl._
{
	[TestFixture]
	public class DynamicInvokerRealProxyTests
	{
		#region Constructors/Destructors

		public DynamicInvokerRealProxyTests()
		{
		}

		#endregion

		#region Methods/Operators

		[Test]
		public void ShouldCreateInstanceWithInvokeDynamicAndDisposeInnerDisposableTest()
		{
			MockFactory mockFactory;
			DynamicInvokerRealProxy<IMockObject> dynamicInvokerRealProxy;
			IDynamicInvocation mockDynamicInvocation;

			mockFactory = new MockFactory();
			mockDynamicInvocation = mockFactory.CreateInstance<IDynamicInvocation>();

			Expect.Once.On(mockDynamicInvocation).Method("Dispose").WithNoArguments();

			dynamicInvokerRealProxy = new DynamicInvokerRealProxy<IMockObject>(mockDynamicInvocation);
			Assert.IsFalse(dynamicInvokerRealProxy.Disposed);
			Assert.IsNotNull(dynamicInvokerRealProxy);

			dynamicInvokerRealProxy.Dispose();
			Assert.IsTrue(dynamicInvokerRealProxy.Disposed);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		[ExpectedException(typeof(ObjectDisposedException))]
		public void ShouldFailOnDisposedInvokeTest()
		{
			MockFactory mockFactory;
			DynamicInvokerRealProxy<IMockObject> dynamicInvokerRealProxy;
			IDynamicInvocation mockDynamicInvocation;
			IMethodCallMessage mockMessage;
			MethodInfo invokedMethodInfo;

			mockFactory = new MockFactory();
			mockDynamicInvocation = mockFactory.CreateInstance<IDynamicInvocation>();

			mockMessage = mockFactory.CreateInstance<IMethodCallMessage>();
			invokedMethodInfo = (MethodInfo)MemberInfoProxy<IDisposable>.GetLastMemberInfo(exec => exec.Dispose());

			Expect.Once.On(mockDynamicInvocation).Method("Dispose").WithNoArguments();

			dynamicInvokerRealProxy = new DynamicInvokerRealProxy<IMockObject>(mockDynamicInvocation);

			Assert.IsNotNull(dynamicInvokerRealProxy);

			dynamicInvokerRealProxy.Dispose();

			dynamicInvokerRealProxy.Invoke(mockMessage);
		}

		[Test]
		[ExpectedException(typeof(NotSupportedException))]
		public void ShouldFailOnGetTypeNameTest()
		{
			DynamicInvokerRealProxy<IMockObject> realProxy;
			MockFactory mockFactory;
			IDynamicInvocation mockDynamicInvocation;
			string value;

			mockFactory = new MockFactory();
			mockDynamicInvocation = mockFactory.CreateInstance<IDynamicInvocation>();

			realProxy = new DynamicInvokerRealProxy<IMockObject>(mockDynamicInvocation);
			Assert.IsNotNull(realProxy);

			value = realProxy.TypeName;
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullInvokeDynamicCreateTest()
		{
			MockFactory mockFactory;
			DynamicInvokerRealProxy<IMockObject> dynamicInvokerRealProxy;
			IDynamicInvocation mockDynamicInvocation;

			mockFactory = new MockFactory();
			mockDynamicInvocation = null;

			dynamicInvokerRealProxy = new DynamicInvokerRealProxy<IMockObject>(mockDynamicInvocation);
		}

		[Test]
		[ExpectedException(typeof(NotSupportedException))]
		public void ShouldFailOnSetTypeNameTest()
		{
			DynamicInvokerRealProxy<IMockObject> realProxy;
			MockFactory mockFactory;
			IDynamicInvocation mockDynamicInvocation;

			mockFactory = new MockFactory();
			mockDynamicInvocation = mockFactory.CreateInstance<IDynamicInvocation>();

			realProxy = new DynamicInvokerRealProxy<IMockObject>(mockDynamicInvocation);
			Assert.IsNotNull(realProxy);

			realProxy.TypeName = null;
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldIFailOnNullInvocationParametersInGetOutputParametersTest()
		{
			MockFactory mockFactory;
			DynamicInvokerRealProxy<IMockObject> dynamicInvokerRealProxy;
			IDynamicInvocation mockDynamicInvocation;
			IMethodCallMessage mockMessage;
			MethodInfo invokedMethodInfo;

			mockFactory = new MockFactory();
			mockDynamicInvocation = mockFactory.CreateInstance<IDynamicInvocation>();
			mockMessage = mockFactory.CreateInstance<IMethodCallMessage>();

			invokedMethodInfo = (MethodInfo)MemberInfoProxy<IMockObject>.GetLastMemberInfo(exec =>
																							{
																								byte bdummy = default(byte);
																								int idummy = default(int);
																								string sdummy = default(string);
																								object odummy = default(object);
																								bdummy = exec.SomeMethodWithVarietyOfParameters(idummy, out sdummy, ref odummy);
																							});

			Expect.Once.On(mockMessage).GetProperty("Args").Will(Return.Value(null));
			Expect.Exactly(2).On(mockMessage).GetProperty("MethodBase").Will(Return.Value(invokedMethodInfo));
			Expect.Once.On(mockMessage).GetProperty("LogicalCallContext").Will(Return.Value(null));
			Expect.Once.On(mockMessage).GetProperty("Uri").Will(Return.Value(null));
			Expect.Once.On(mockMessage).GetProperty("MethodName").Will(Return.Value(null));
			Expect.Once.On(mockMessage).GetProperty("TypeName").Will(Return.Value(null));
			Expect.Once.On(mockMessage).GetProperty("HasVarArgs").Will(Return.Value(false));

			dynamicInvokerRealProxy = new DynamicInvokerRealProxy<IMockObject>(mockDynamicInvocation);
			Assert.IsNotNull(dynamicInvokerRealProxy);

			Expect.Once.On(mockDynamicInvocation).Method("Invoke").With(new EqualMatcher(typeof(IMockObject)), new EqualMatcher(invokedMethodInfo), new EqualMatcher(dynamicInvokerRealProxy.GetTransparentProxy()), new EqualMatcher(null)).Will(Return.Value(null));

			dynamicInvokerRealProxy.Invoke(mockMessage);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldIFailOnNullMethodInfoInGetOutputParametersTest()
		{
			MockFactory mockFactory;
			DynamicInvokerRealProxy<IMockObject> dynamicInvokerRealProxy;
			IDynamicInvocation mockDynamicInvocation;
			IMethodCallMessage mockMessage;
			MethodInfo invokedMethodInfo;

			mockFactory = new MockFactory();
			mockDynamicInvocation = mockFactory.CreateInstance<IDynamicInvocation>();
			mockMessage = mockFactory.CreateInstance<IMethodCallMessage>();

			invokedMethodInfo = null;

			Expect.Once.On(mockMessage).GetProperty("Args").Will(Return.Value(new object[] { 10, "100", (object)"1000" }));
			Expect.Exactly(2).On(mockMessage).GetProperty("MethodBase").Will(Return.Value(invokedMethodInfo));
			Expect.Once.On(mockMessage).GetProperty("LogicalCallContext").Will(Return.Value(null));
			Expect.Once.On(mockMessage).GetProperty("Uri").Will(Return.Value(null));
			Expect.Once.On(mockMessage).GetProperty("MethodName").Will(Return.Value(null));
			Expect.Once.On(mockMessage).GetProperty("TypeName").Will(Return.Value(null));
			Expect.Once.On(mockMessage).GetProperty("HasVarArgs").Will(Return.Value(false));

			dynamicInvokerRealProxy = new DynamicInvokerRealProxy<IMockObject>(mockDynamicInvocation);
			Assert.IsNotNull(dynamicInvokerRealProxy);

			Expect.Once.On(mockDynamicInvocation).Method("Invoke").With(new EqualMatcher(typeof(IMockObject)), new EqualMatcher(invokedMethodInfo), new EqualMatcher(dynamicInvokerRealProxy.GetTransparentProxy()), new EqualMatcher(new object[] { 10, "100", (object)"1000" })).Will(Return.Value(null));

			dynamicInvokerRealProxy.Invoke(mockMessage);
		}

		[Test]
		public void ShouldInvokeAsIDiposableTest()
		{
			MockFactory mockFactory;
			DynamicInvokerRealProxy<IMockObject> dynamicInvokerRealProxy;
			IDynamicInvocation mockDynamicInvocation;
			IMethodCallMessage mockMessage;
			MethodInfo invokedMethodInfo;

			mockFactory = new MockFactory();
			mockDynamicInvocation = mockFactory.CreateInstance<IDynamicInvocation>();
			mockMessage = mockFactory.CreateInstance<IMethodCallMessage>();

			invokedMethodInfo = (MethodInfo)MemberInfoProxy<IDisposable>.GetLastMemberInfo(exec => exec.Dispose());

			Expect.Once.On(mockMessage).GetProperty("Args").Will(Return.Value(new object[] { }));
			Expect.Exactly(2).On(mockMessage).GetProperty("MethodBase").Will(Return.Value(invokedMethodInfo));
			Expect.Once.On(mockMessage).GetProperty("LogicalCallContext").Will(Return.Value(null));
			Expect.Once.On(mockMessage).GetProperty("Uri").Will(Return.Value(null));
			Expect.Once.On(mockMessage).GetProperty("MethodName").Will(Return.Value(null));
			Expect.Once.On(mockMessage).GetProperty("TypeName").Will(Return.Value(null));
			Expect.Once.On(mockMessage).GetProperty("HasVarArgs").Will(Return.Value(false));

			dynamicInvokerRealProxy = new DynamicInvokerRealProxy<IMockObject>(mockDynamicInvocation);
			Assert.IsNotNull(dynamicInvokerRealProxy);

			Expect.Once.On(mockDynamicInvocation).Method("Invoke").With(new EqualMatcher(typeof(IMockObject)), new EqualMatcher(invokedMethodInfo), new EqualMatcher(dynamicInvokerRealProxy.GetTransparentProxy()), new EqualMatcher(new object[] { })).Will(Return.Value(null));

			dynamicInvokerRealProxy.Invoke(mockMessage);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldInvokeAsIMockObjectTest()
		{
			MockFactory mockFactory;
			DynamicInvokerRealProxy<IMockObject> dynamicInvokerRealProxy;
			IDynamicInvocation mockDynamicInvocation;
			IMethodCallMessage mockMessage;
			MethodInfo invokedMethodInfo;

			mockFactory = new MockFactory();
			mockDynamicInvocation = mockFactory.CreateInstance<IDynamicInvocation>();
			mockMessage = mockFactory.CreateInstance<IMethodCallMessage>();

			invokedMethodInfo = (MethodInfo)MemberInfoProxy<IMockObject>.GetLastMemberInfo(exec =>
																							{
																								byte bdummy = default(byte);
																								int idummy = default(int);
																								string sdummy = default(string);
																								object odummy = default(object);
																								bdummy = exec.SomeMethodWithVarietyOfParameters(idummy, out sdummy, ref odummy);
																							});

			Expect.Once.On(mockMessage).GetProperty("Args").Will(Return.Value(new object[] { 10, "100", (object)"1000" }));
			Expect.Exactly(2).On(mockMessage).GetProperty("MethodBase").Will(Return.Value(invokedMethodInfo));
			Expect.Once.On(mockMessage).GetProperty("LogicalCallContext").Will(Return.Value(null));
			Expect.Once.On(mockMessage).GetProperty("Uri").Will(Return.Value(null));
			Expect.Once.On(mockMessage).GetProperty("MethodName").Will(Return.Value(null));
			Expect.Once.On(mockMessage).GetProperty("TypeName").Will(Return.Value(null));
			Expect.Once.On(mockMessage).GetProperty("HasVarArgs").Will(Return.Value(false));

			dynamicInvokerRealProxy = new DynamicInvokerRealProxy<IMockObject>(mockDynamicInvocation);
			Assert.IsNotNull(dynamicInvokerRealProxy);

			Expect.Once.On(mockDynamicInvocation).Method("Invoke").With(new EqualMatcher(typeof(IMockObject)), new EqualMatcher(invokedMethodInfo), new EqualMatcher(dynamicInvokerRealProxy.GetTransparentProxy()), new EqualMatcher(new object[] { 10, "100", (object)"1000" })).Will(Return.Value(null));

			dynamicInvokerRealProxy.Invoke(mockMessage);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldInvokeAsNonObjectTest()
		{
			MockFactory mockFactory;
			DynamicInvokerRealProxy<IMockObject> dynamicInvokerRealProxy;
			IDynamicInvocation mockDynamicInvocation;
			IMethodCallMessage mockMessage;
			MethodInfo invokedMethodInfo;

			mockFactory = new MockFactory();
			mockDynamicInvocation = mockFactory.CreateInstance<IDynamicInvocation>();
			mockMessage = mockFactory.CreateInstance<IMethodCallMessage>();

			invokedMethodInfo = (MethodInfo)MemberInfoProxy<ICloneable>.GetLastMemberInfo(exec => exec.Clone());

			Expect.Once.On(mockMessage).GetProperty("Args").Will(Return.Value(new object[] { }));
			Expect.Exactly(2).On(mockMessage).GetProperty("MethodBase").Will(Return.Value(invokedMethodInfo));
			Expect.Once.On(mockMessage).GetProperty("LogicalCallContext").Will(Return.Value(null));
			Expect.Once.On(mockMessage).GetProperty("Uri").Will(Return.Value(null));
			Expect.Once.On(mockMessage).GetProperty("MethodName").Will(Return.Value(null));
			Expect.Once.On(mockMessage).GetProperty("TypeName").Will(Return.Value(null));
			Expect.Once.On(mockMessage).GetProperty("HasVarArgs").Will(Return.Value(false));

			dynamicInvokerRealProxy = new DynamicInvokerRealProxy<IMockObject>(mockDynamicInvocation);

			Assert.IsNotNull(dynamicInvokerRealProxy);

			Expect.Once.On(mockDynamicInvocation).Method("Invoke").With(new EqualMatcher(typeof(IMockObject)), new EqualMatcher(invokedMethodInfo), new EqualMatcher(dynamicInvokerRealProxy.GetTransparentProxy()), new EqualMatcher(new object[] { })).Will(Return.Value(null));

			dynamicInvokerRealProxy.Invoke(mockMessage);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldInvokeAsObjectTest()
		{
			MockFactory mockFactory;
			DynamicInvokerRealProxy<IMockObject> dynamicInvokerRealProxy;
			IDynamicInvocation mockDynamicInvocation;
			IMethodCallMessage mockMessage;
			MethodInfo invokedMethodInfo;

			mockFactory = new MockFactory();
			mockDynamicInvocation = mockFactory.CreateInstance<IDynamicInvocation>();
			mockMessage = mockFactory.CreateInstance<IMethodCallMessage>();

			invokedMethodInfo = (MethodInfo)MemberInfoProxy<IMockObject>.GetLastMemberInfo(exec => exec.GetType());

			Expect.Once.On(mockMessage).GetProperty("Args").Will(Return.Value(new object[] { }));
			Expect.Exactly(2).On(mockMessage).GetProperty("MethodBase").Will(Return.Value(invokedMethodInfo));
			Expect.Once.On(mockMessage).GetProperty("LogicalCallContext").Will(Return.Value(null));
			Expect.Once.On(mockMessage).GetProperty("Uri").Will(Return.Value(null));
			Expect.Once.On(mockMessage).GetProperty("MethodName").Will(Return.Value(null));
			Expect.Once.On(mockMessage).GetProperty("TypeName").Will(Return.Value(null));
			Expect.Once.On(mockMessage).GetProperty("HasVarArgs").Will(Return.Value(false));

			dynamicInvokerRealProxy = new DynamicInvokerRealProxy<IMockObject>(mockDynamicInvocation);

			Expect.Once.On(mockDynamicInvocation).Method("Invoke").With(new EqualMatcher(typeof(IMockObject)), new EqualMatcher(invokedMethodInfo), new EqualMatcher(dynamicInvokerRealProxy.GetTransparentProxy()), new EqualMatcher(new object[] { })).Will(Return.Value(null));

			Assert.IsNotNull(dynamicInvokerRealProxy);

			dynamicInvokerRealProxy.Invoke(mockMessage);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldNotFailOnDoubleDisposeTest()
		{
			MockFactory mockFactory;
			DynamicInvokerRealProxy<IMockObject> dynamicInvokerRealProxy;
			IDynamicInvocation mockDynamicInvocation;

			mockFactory = new MockFactory();
			mockDynamicInvocation = mockFactory.CreateInstance<IDynamicInvocation>();

			Expect.Once.On(mockDynamicInvocation).Method("Dispose").WithNoArguments();

			dynamicInvokerRealProxy = new DynamicInvokerRealProxy<IMockObject>(mockDynamicInvocation);

			Assert.IsNotNull(dynamicInvokerRealProxy);

			dynamicInvokerRealProxy.Dispose();
			dynamicInvokerRealProxy.Dispose();

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldValidateAssumptionAboutRealProxyObjectCastingTest()
		{
			DynamicInvokerRealProxy<IMockObject> realProxy;
			IMockObject transparentProxy;
			MockFactory mockFactory;
			IDynamicInvocation mockDynamicInvocation;
			IDisposable disposable;
			MethodInfo invokedMethodInfo;

			mockFactory = new MockFactory();
			mockDynamicInvocation = mockFactory.CreateInstance<IDynamicInvocation>();

			invokedMethodInfo = (MethodInfo)MemberInfoProxy<IDisposable>.GetLastMemberInfo(exec => exec.Dispose());

			realProxy = new DynamicInvokerRealProxy<IMockObject>(mockDynamicInvocation);
			Assert.IsNotNull(realProxy);

			Expect.Once.On(mockDynamicInvocation).Method("Invoke").With(new EqualMatcher(typeof(IMockObject)), new EqualMatcher(invokedMethodInfo), new EqualMatcher(realProxy.GetTransparentProxy()), new EqualMatcher(new object[] { })).Will(Return.Value(null));

			transparentProxy = (IMockObject)realProxy.GetTransparentProxy();
			Assert.IsNotNull(transparentProxy);
			Assert.IsTrue(RemotingServices.IsTransparentProxy((object)transparentProxy));
			Assert.IsTrue(RemotingServices.GetRealProxy((object)transparentProxy) == (object)realProxy);
			Assert.IsFalse((object)realProxy == (object)transparentProxy);

			Assert.IsTrue(realProxy.CanCastTo(typeof(IDisposable), null));
			Assert.IsFalse(realProxy.CanCastTo(typeof(IConvertible), null));
			Assert.IsTrue(transparentProxy is IDisposable);

			disposable = (IDisposable)transparentProxy;
			disposable.Dispose();

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldValidateAssumptionAboutRealProxyObjectIdTest()
		{
			DynamicInvokerRealProxy<IMockObject> realProxy;
			IMockObject transparentProxy0, transparentProxy1;
			MockFactory mockFactory;
			IDynamicInvocation mockDynamicInvocation;

			mockFactory = new MockFactory();
			mockDynamicInvocation = mockFactory.CreateInstance<IDynamicInvocation>();

			realProxy = new DynamicInvokerRealProxy<IMockObject>(mockDynamicInvocation);
			Assert.IsNotNull(realProxy);

			transparentProxy0 = (IMockObject)realProxy.GetTransparentProxy();
			Assert.IsNotNull(transparentProxy0);
			Assert.IsTrue(RemotingServices.IsTransparentProxy((object)transparentProxy0));
			Assert.IsTrue(RemotingServices.GetRealProxy((object)transparentProxy0) == (object)realProxy);
			Assert.IsFalse((object)realProxy == (object)transparentProxy0);

			transparentProxy1 = (IMockObject)realProxy.GetTransparentProxy();
			Assert.IsNotNull(transparentProxy1);
			Assert.IsTrue(RemotingServices.IsTransparentProxy((object)transparentProxy1));
			Assert.IsTrue(RemotingServices.GetRealProxy((object)transparentProxy1) == (object)realProxy);
			Assert.IsFalse((object)realProxy == (object)transparentProxy1);

			Assert.IsTrue((object)transparentProxy0 == (object)transparentProxy1);

			mockFactory.VerifyAllExpectationsHaveBeenMet();
		}

		#endregion
	}
}