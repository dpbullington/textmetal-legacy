using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NMock.Monitoring;

namespace NMock.Proxy.Reflective
{
	internal class ReflectiveInterceptor : InterceptorBase, IInterceptor
	{
		public ReflectiveInterceptor(MockFactory mockFactory, CompositeType mockedType, string name, MockStyle mockStyle)
			: base(mockFactory, mockedType, name, mockStyle)
		{

		}

		public void Intercept(IInvocation invocation)
		{
			if (InvocationRecorder.Recording)
			{
				InvocationRecorder.Current.Invocation = new Invocation(invocation.Method, invocation.Arguments);
				return;
			}

			// Check for calls to basic NMock infrastructure
			if (MockObjectMethods.ContainsKey(invocation.Method))
			{
				try
				{
					invocation.ReturnValue = invocation.Method.Invoke(this, invocation.Arguments);
				}
				catch (TargetInvocationException tie)
				{
#if !SILVERLIGHT
					// replace stack trace with original stack trace
					FieldInfo remoteStackTraceString = typeof(Exception).GetField("_remoteStackTraceString", BindingFlags.Instance | BindingFlags.NonPublic);
					remoteStackTraceString.SetValue(tie.InnerException, tie.InnerException.StackTrace + Environment.NewLine);

					throw tie.InnerException;
#else
					throw;
#endif
				}

				return;
			}

			// Ok, this call is targeting a member of the mocked class
			object invocationTarget;
			if (MockedTypes.PrimaryType == invocation.Method.DeclaringType && invocation.InvocationTarget != null)
			{
				invocationTarget = invocation.InvocationTarget;
			}
			else
			{
				invocationTarget = invocation.Proxy;
			}

			Invocation invocationForMock = new Invocation(invocationTarget, invocation.Method, invocation.Arguments);

			if (ShouldCallInvokeImplementation(invocationForMock))
			{
				invocation.Proceed();

				if (!MockFactory.HasExpectationFor(invocationForMock))
					return;
			}

			invocation.ReturnValue = ProcessCallAgainstExpectations(invocationForMock);
		}
	}
}