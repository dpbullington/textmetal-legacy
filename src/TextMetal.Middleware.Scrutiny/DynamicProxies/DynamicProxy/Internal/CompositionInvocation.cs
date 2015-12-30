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

using System.Reflection;

namespace Castle.DynamicProxy.Internal
{
	using System;

	public abstract class CompositionInvocation : AbstractInvocation
	{
		#region Constructors/Destructors

		protected CompositionInvocation(
			object target,
			object proxy,
			IInterceptor[] interceptors,
			MethodInfo proxiedMethod,
			object[] arguments)
			: base(proxy, interceptors, proxiedMethod, arguments)
		{
			this.target = target;
		}

		#endregion

		#region Fields/Constants

		protected object target;

		#endregion

		#region Properties/Indexers/Events

		public override object InvocationTarget
		{
			get
			{
				return this.target;
			}
		}

		public override MethodInfo MethodInvocationTarget
		{
			get
			{
				return InvocationHelper.GetMethodOnObject(this.target, this.Method);
			}
		}

		public override Type TargetType
		{
			get
			{
				return TypeUtil.GetTypeOrNull(this.target);
			}
		}

		#endregion

		#region Methods/Operators

		protected void EnsureValidProxyTarget(object newTarget)
		{
			if (newTarget == null)
				throw new ArgumentNullException("newTarget");

			if (!ReferenceEquals(newTarget, this.proxyObject))
				return;

			var message = "This is a DynamicProxy2 error: target of proxy has been set to the proxy itself. " +
						"This would result in recursively calling proxy methods over and over again until stack overflow, which may destabilize your program." +
						"This usually signifies a bug in the calling code. Make sure no interceptor sets proxy as its own target.";
			throw new InvalidOperationException(message);
		}

		protected void EnsureValidTarget()
		{
			if (this.target == null)
				this.ThrowOnNoTarget();

			if (!ReferenceEquals(this.target, this.proxyObject))
				return;

			var message = "This is a DynamicProxy2 error: target of invocation has been set to the proxy itself. " +
						"This may result in recursively calling the method over and over again until stack overflow, which may destabilize your program." +
						"This usually signifies a bug in the calling code. Make sure no interceptor sets proxy as its invocation target.";
			throw new InvalidOperationException(message);
		}

		#endregion
	}
}