#if !SILVERLIGHT
using System;
using System.Reflection;
using LinFu.AOP.Cecil.Extensions;
using LinFu.AOP.Interfaces;
using NMock.Internal;
using NMock.Monitoring;

namespace NMock.Proxy.LinFu
{
	internal class LinFuMockObjectInterceptor : InterceptorBase, IInterceptor
	{
		internal LinFuMockObjectInterceptor(MockFactory mockFactory, CompositeType mockedType, string name, MockStyle mockStyle) : base(mockFactory, mockedType, name, mockStyle)
		{
		}

		public object Intercept(IInvocationInfo interceptedInvocation)
		{
			if (InvocationRecorder.Recording)
			{
				InvocationRecorder.Current.Invocation = new Invocation(interceptedInvocation.Target, interceptedInvocation.TargetMethod, interceptedInvocation.Arguments);
				return null;
			}

			// Check for calls to basic NMock infrastructure
			if (MockObjectMethods.ContainsKey(interceptedInvocation.TargetMethod))
			{
				try
				{
					return interceptedInvocation.TargetMethod.Invoke(this, interceptedInvocation.Arguments);
				}
				catch (TargetInvocationException tie)
				{
					// replace stack trace with original stack trace
					FieldInfo remoteStackTraceString = typeof(Exception).GetField("_remoteStackTraceString", BindingFlags.Instance | BindingFlags.NonPublic);
					remoteStackTraceString.SetValue(tie.InnerException, tie.InnerException.StackTrace + Environment.NewLine);
					throw tie.InnerException;
				}
			}

			// Ok, this call is targeting a member of the mocked class
			object invocationTarget;
			if (MockedTypes.PrimaryType == interceptedInvocation.TargetMethod.DeclaringType && interceptedInvocation.Target != null)
			{
				invocationTarget = interceptedInvocation.Target;
			}
			else
			{
				invocationTarget = interceptedInvocation.Target;
			}

			Invocation invocationForMock = new Invocation(invocationTarget, interceptedInvocation.TargetMethod, interceptedInvocation.Arguments);

			if (ShouldCallInvokeImplementation(invocationForMock))
			{
				interceptedInvocation.TargetMethod.Invoke(interceptedInvocation.Target, interceptedInvocation.Arguments);
				//interceptedInvocation.Proceed(interceptedInvocation.Target, interceptedInvocation.TargetMethod, interceptedInvocation.Arguments);

				if (!MockFactory.HasExpectationFor(invocationForMock))
					return null;
			}

			return ProcessCallAgainstExpectations(invocationForMock);
		}
	}
}
#endif