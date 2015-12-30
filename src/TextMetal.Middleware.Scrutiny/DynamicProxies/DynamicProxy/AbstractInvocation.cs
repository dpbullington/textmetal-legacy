// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Diagnostics;
using System.Reflection;

namespace Castle.DynamicProxy
{
	using System;

#if FEATURE_SERIALIZATION
	using System.Runtime.Serialization;
#if DOTNET40
	using System.Security;
#endif

	using Castle.DynamicProxy.Serialization;
#endif

	public abstract class AbstractInvocation : IInvocation
#if FEATURE_SERIALIZATION
		, ISerializable
#endif
	{
		private readonly IInterceptor[] interceptors;
		private readonly object[] arguments;
		private int currentInterceptorIndex = -1;
		private Type[] genericMethodArguments;
		private readonly MethodInfo proxiedMethod;
		protected readonly object proxyObject;

		protected AbstractInvocation(
			object proxy,
			IInterceptor[] interceptors,
			MethodInfo proxiedMethod,
			object[] arguments)
		{
			Debug.Assert(proxiedMethod != null);
			this.proxyObject = proxy;
			this.interceptors = interceptors;
			this.proxiedMethod = proxiedMethod;
			this.arguments = arguments;
		}

		public void SetGenericMethodArguments(Type[] arguments)
		{
			this.genericMethodArguments = arguments;
		}

		public abstract object InvocationTarget
		{
			get;
		}

		public abstract Type TargetType
		{
			get;
		}

		public abstract MethodInfo MethodInvocationTarget
		{
			get;
		}

		public Type[] GenericArguments
		{
			get
			{
				return this.genericMethodArguments;
			}
		}

		public object Proxy
		{
			get
			{
				return this.proxyObject;
			}
		}

		public MethodInfo Method
		{
			get
			{
				return this.proxiedMethod;
			}
		}

		public MethodInfo GetConcreteMethod()
		{
			return this.EnsureClosedMethod(this.Method);
		}

		public MethodInfo GetConcreteMethodInvocationTarget()
		{
			// it is ensured by the InvocationHelper that method will be closed
			var method = this.MethodInvocationTarget;
			Debug.Assert(method == null || method.IsGenericMethodDefinition == false,
				"method == null || method.IsGenericMethodDefinition == false");
			return method;
		}

		public object ReturnValue
		{
			get;
			set;
		}

		public object[] Arguments
		{
			get
			{
				return this.arguments;
			}
		}

		public void SetArgumentValue(int index, object value)
		{
			this.arguments[index] = value;
		}

		public object GetArgumentValue(int index)
		{
			return this.arguments[index];
		}

		public void Proceed()
		{
			if (this.interceptors == null)
				// not yet fully initialized? probably, an intercepted method is called while we are being deserialized
			{
				this.InvokeMethodOnTarget();
				return;
			}

			this.currentInterceptorIndex++;
			try
			{
				if (this.currentInterceptorIndex == this.interceptors.Length)
					this.InvokeMethodOnTarget();
				else if (this.currentInterceptorIndex > this.interceptors.Length)
				{
					string interceptorsCount;
					if (this.interceptors.Length > 1)
						interceptorsCount = " each one of " + this.interceptors.Length + " interceptors";
					else
						interceptorsCount = " interceptor";

					var message = "This is a DynamicProxy2 error: invocation.Proceed() has been called more times than expected." +
								"This usually signifies a bug in the calling code. Make sure that" + interceptorsCount +
								" selected for the method '" + this.Method + "'" +
								"calls invocation.Proceed() at most once.";
					throw new InvalidOperationException(message);
				}
				else
					this.interceptors[this.currentInterceptorIndex].Intercept(this);
			}
			finally
			{
				this.currentInterceptorIndex--;
			}
		}

#if FEATURE_SERIALIZATION
#if DOTNET40
		[SecurityCritical]
#endif
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.SetType(typeof(RemotableInvocation));
			info.AddValue("invocation", new RemotableInvocation(this));
		}
#endif

		protected abstract void InvokeMethodOnTarget();

		protected void ThrowOnNoTarget()
		{
			// let's try to build as friendly message as we can
			string interceptorsMessage;
			if (this.interceptors.Length == 0)
				interceptorsMessage = "There are no interceptors specified";
			else
				interceptorsMessage = "The interceptor attempted to 'Proceed'";

			string methodKindIs;
			string methodKindDescription;
			if (this.Method.DeclaringType.GetTypeInfo().IsClass && this.Method.IsAbstract)
			{
				methodKindIs = "is abstract";
				methodKindDescription = "an abstract method";
			}
			else
			{
				methodKindIs = "has no target";
				methodKindDescription = "method without target";
			}

			var message = string.Format("This is a DynamicProxy2 error: {0} for method '{1}' which {2}. " +
										"When calling {3} there is no implementation to 'proceed' to and " +
										"it is the responsibility of the interceptor to mimic the implementation " +
										"(set return value, out arguments etc)",
				interceptorsMessage, this.Method, methodKindIs, methodKindDescription);

			throw new NotImplementedException(message);
		}

		private MethodInfo EnsureClosedMethod(MethodInfo method)
		{
			if (method.ContainsGenericParameters)
			{
				Debug.Assert(this.genericMethodArguments != null);
				return method.GetGenericMethodDefinition().MakeGenericMethod(this.genericMethodArguments);
			}
			return method;
		}
	}
}