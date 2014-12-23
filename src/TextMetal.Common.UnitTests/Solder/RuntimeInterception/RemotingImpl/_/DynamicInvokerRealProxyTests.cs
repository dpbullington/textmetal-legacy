/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;

using NMock2;
using NMock2.Matchers;

using NUnit.Framework;

using TextMetal.Common.Solder.RuntimeInterception;
using TextMetal.Common.Solder.RuntimeInterception.RemotingImpl;
using TextMetal.Common.UnitTests.TestingInfrastructure;
using TextMetal.TestFramework;

namespace TextMetal.Common.UnitTests.Solder.RuntimeInterception.RemotingImpl._
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
			Mockery mockery;
			DynamicInvokerRealProxy<IMockObject> dynamicInvokerRealProxy;
			IDynamicInvocation mockDynamicInvocation;

			mockery = new Mockery();
			mockDynamicInvocation = mockery.NewMock<IDynamicInvocation>();

			Expect.Once.On(mockDynamicInvocation).Method("Dispose").WithNoArguments();

			dynamicInvokerRealProxy = new DynamicInvokerRealProxy<IMockObject>(mockDynamicInvocation);
			Assert.IsFalse(dynamicInvokerRealProxy.Disposed);
			Assert.IsNotNull(dynamicInvokerRealProxy);

			dynamicInvokerRealProxy.Dispose();
			Assert.IsTrue(dynamicInvokerRealProxy.Disposed);

			mockery.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		[ExpectedException(typeof(ObjectDisposedException))]
		public void ShouldFailOnDisposedInvokeTest()
		{
			Mockery mockery;
			DynamicInvokerRealProxy<IMockObject> dynamicInvokerRealProxy;
			IDynamicInvocation mockDynamicInvocation;
			IMethodCallMessage mockMessage;
			MethodInfo invokedMethodInfo;

			mockery = new Mockery();
			mockDynamicInvocation = mockery.NewMock<IDynamicInvocation>();

			mockMessage = mockery.NewMock<IMethodCallMessage>();
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
			Mockery mockery;
			IDynamicInvocation mockDynamicInvocation;
			string value;

			mockery = new Mockery();
			mockDynamicInvocation = mockery.NewMock<IDynamicInvocation>();

			realProxy = new DynamicInvokerRealProxy<IMockObject>(mockDynamicInvocation);
			Assert.IsNotNull(realProxy);

			value = realProxy.TypeName;
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldFailOnNullInvokeDynamicCreateTest()
		{
			Mockery mockery;
			DynamicInvokerRealProxy<IMockObject> dynamicInvokerRealProxy;
			IDynamicInvocation mockDynamicInvocation;

			mockery = new Mockery();
			mockDynamicInvocation = null;

			dynamicInvokerRealProxy = new DynamicInvokerRealProxy<IMockObject>(mockDynamicInvocation);
		}

		[Test]
		[ExpectedException(typeof(NotSupportedException))]
		public void ShouldFailOnSetTypeNameTest()
		{
			DynamicInvokerRealProxy<IMockObject> realProxy;
			Mockery mockery;
			IDynamicInvocation mockDynamicInvocation;

			mockery = new Mockery();
			mockDynamicInvocation = mockery.NewMock<IDynamicInvocation>();

			realProxy = new DynamicInvokerRealProxy<IMockObject>(mockDynamicInvocation);
			Assert.IsNotNull(realProxy);

			realProxy.TypeName = null;
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldIFailOnNullInvocationParametersInGetOutputParametersTest()
		{
			Mockery mockery;
			DynamicInvokerRealProxy<IMockObject> dynamicInvokerRealProxy;
			IDynamicInvocation mockDynamicInvocation;
			IMethodCallMessage mockMessage;
			MethodInfo invokedMethodInfo;

			mockery = new Mockery();
			mockDynamicInvocation = mockery.NewMock<IDynamicInvocation>();
			mockMessage = mockery.NewMock<IMethodCallMessage>();

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
			Mockery mockery;
			DynamicInvokerRealProxy<IMockObject> dynamicInvokerRealProxy;
			IDynamicInvocation mockDynamicInvocation;
			IMethodCallMessage mockMessage;
			MethodInfo invokedMethodInfo;

			mockery = new Mockery();
			mockDynamicInvocation = mockery.NewMock<IDynamicInvocation>();
			mockMessage = mockery.NewMock<IMethodCallMessage>();

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
			Mockery mockery;
			DynamicInvokerRealProxy<IMockObject> dynamicInvokerRealProxy;
			IDynamicInvocation mockDynamicInvocation;
			IMethodCallMessage mockMessage;
			MethodInfo invokedMethodInfo;

			mockery = new Mockery();
			mockDynamicInvocation = mockery.NewMock<IDynamicInvocation>();
			mockMessage = mockery.NewMock<IMethodCallMessage>();

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

			mockery.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldInvokeAsIMockObjectTest()
		{
			Mockery mockery;
			DynamicInvokerRealProxy<IMockObject> dynamicInvokerRealProxy;
			IDynamicInvocation mockDynamicInvocation;
			IMethodCallMessage mockMessage;
			MethodInfo invokedMethodInfo;

			mockery = new Mockery();
			mockDynamicInvocation = mockery.NewMock<IDynamicInvocation>();
			mockMessage = mockery.NewMock<IMethodCallMessage>();

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

			mockery.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldInvokeAsNonObjectTest()
		{
			Mockery mockery;
			DynamicInvokerRealProxy<IMockObject> dynamicInvokerRealProxy;
			IDynamicInvocation mockDynamicInvocation;
			IMethodCallMessage mockMessage;
			MethodInfo invokedMethodInfo;

			mockery = new Mockery();
			mockDynamicInvocation = mockery.NewMock<IDynamicInvocation>();
			mockMessage = mockery.NewMock<IMethodCallMessage>();

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

			mockery.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldInvokeAsObjectTest()
		{
			Mockery mockery;
			DynamicInvokerRealProxy<IMockObject> dynamicInvokerRealProxy;
			IDynamicInvocation mockDynamicInvocation;
			IMethodCallMessage mockMessage;
			MethodInfo invokedMethodInfo;

			mockery = new Mockery();
			mockDynamicInvocation = mockery.NewMock<IDynamicInvocation>();
			mockMessage = mockery.NewMock<IMethodCallMessage>();

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

			mockery.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldNotFailOnDoubleDisposeTest()
		{
			Mockery mockery;
			DynamicInvokerRealProxy<IMockObject> dynamicInvokerRealProxy;
			IDynamicInvocation mockDynamicInvocation;

			mockery = new Mockery();
			mockDynamicInvocation = mockery.NewMock<IDynamicInvocation>();

			Expect.Once.On(mockDynamicInvocation).Method("Dispose").WithNoArguments();

			dynamicInvokerRealProxy = new DynamicInvokerRealProxy<IMockObject>(mockDynamicInvocation);

			Assert.IsNotNull(dynamicInvokerRealProxy);

			dynamicInvokerRealProxy.Dispose();
			dynamicInvokerRealProxy.Dispose();

			mockery.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldValidateAssumptionAboutRealProxyObjectCastingTest()
		{
			DynamicInvokerRealProxy<IMockObject> realProxy;
			IMockObject transparentProxy;
			Mockery mockery;
			IDynamicInvocation mockDynamicInvocation;
			IDisposable disposable;
			MethodInfo invokedMethodInfo;

			mockery = new Mockery();
			mockDynamicInvocation = mockery.NewMock<IDynamicInvocation>();

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

			mockery.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void ShouldValidateAssumptionAboutRealProxyObjectIdTest()
		{
			DynamicInvokerRealProxy<IMockObject> realProxy;
			IMockObject transparentProxy0, transparentProxy1;
			Mockery mockery;
			IDynamicInvocation mockDynamicInvocation;

			mockery = new Mockery();
			mockDynamicInvocation = mockery.NewMock<IDynamicInvocation>();

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

			mockery.VerifyAllExpectationsHaveBeenMet();
		}

		#endregion
	}
}