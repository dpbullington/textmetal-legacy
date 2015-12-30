using System.Collections.Generic;
using System.Reflection;

using NMock.Internal;
using NMock.Monitoring;

namespace NMock.Proxy
{
	internal abstract class InterceptorBase : MockObject
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes static members of the <see cref="InterceptorBase" /> class.
		/// </summary>
		static InterceptorBase()
		{
			// We want to be able to quickly recognize any later invocations
			// on methods that belong to IMockObject or IInvokable, so we cache
			// their definitions here.
			foreach (MethodInfo methodInfo in typeof(IMockObject).GetMethods())
				MockObjectMethods.Add(methodInfo, null);

			foreach (MethodInfo methodInfo in typeof(IInvokable).GetMethods())
				MockObjectMethods.Add(methodInfo, null);
		}

		protected InterceptorBase(MockFactory mockFactory, CompositeType mockedType, string name, MockStyle mockStyle)
			: base(mockFactory, mockedType, name, mockStyle)
		{
		}

		#endregion

		#region Fields/Constants

		protected static readonly Dictionary<MethodBase, object> MockObjectMethods = new Dictionary<MethodBase, object>();

		#endregion

		#region Methods/Operators

		protected object ProcessCallAgainstExpectations(Invocation invocation)
		{
			if (PropertyStorageMarker.UsePropertyStorage)
			{
				var interceptedValues = ((IMockObject)invocation.Receiver).InterceptedValues;
				if (interceptedValues.ContainsKey(invocation.MethodSignatureForSetter))
					return interceptedValues[invocation.MethodSignatureForSetter];
			}

			((IInvokable)this).Invoke(invocation);

			if (invocation.IsThrowing)
				throw invocation.Exception;

			if (invocation.Result == Missing.Value && invocation.Method.ReturnType != typeof(void))
			{
				//var exception = new UnexpectedInvocationException(invocation,"No matching expectation was found for this invocation.  " + invocation);

				//MockFactory.OrderingException = exception;

				//throw exception;
			}

			if (invocation.IsPropertySetAccessor)
			{
				var interceptedValues = ((IMockObject)invocation.Receiver).InterceptedValues;

				if (interceptedValues.ContainsKey(invocation.MethodSignature))
					interceptedValues[invocation.MethodSignature] = invocation.SetterResult;
				else
					interceptedValues.Add(invocation.MethodSignature, invocation.SetterResult);
			}

			return invocation.Result;
		}

		protected bool ShouldCallInvokeImplementation(Invocation invocation)
		{
			var _type = this.MockedTypes.PrimaryType.GetTypeInfo();

			// Only transparent mocks of classes can have their implemenation invoked
			if (this.MockStyle == MockStyle.Transparent && !_type.IsInterface)
			{
				// Events can go through because invoking them is different than their binding
				// (and because that is kind of how I think it should work...but technically invoking them maybe shouldn't in a transparent mock.)
				// The implementation should only be invoked if no expectations
				// have been set for this method
				if (invocation.IsEventAccessor || !this.MockFactory.HasExpectationFor(invocation))
				{
					// As classes and interfaces can be combined into a single mock, we have
					// to be sure that the target method actually belongs to a class
					if (this.MockedTypes.PrimaryType == invocation.Method.DeclaringType
						//|| MockedTypes.PrimaryType.IsSubclassOf(invocationForMock.Method.DeclaringType)
						|| invocation.Method.DeclaringType.IsAssignableFrom(this.MockedTypes.PrimaryType)
						//invocationForMock.Method.DeclaringType.IsInterface
						)
						return true;
				}
			}

			return false;
		}

		#endregion
	}
}