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

using System.Collections.Generic;
using System.Diagnostics;

using Castle.DynamicProxy.Generators;
using Castle.DynamicProxy.Generators.Emitters;

namespace Castle.DynamicProxy.Contributors
{
	using System;

	public class InterfaceProxyTargetContributor : CompositeTypeContributor
	{
		#region Constructors/Destructors

		public InterfaceProxyTargetContributor(Type proxyTargetType, bool canChangeTarget, INamingScope namingScope)
			: base(namingScope)
		{
			this.proxyTargetType = proxyTargetType;
			this.canChangeTarget = canChangeTarget;
		}

		#endregion

		#region Fields/Constants

		private readonly bool canChangeTarget;
		private readonly Type proxyTargetType;

		#endregion

		#region Methods/Operators

		protected override IEnumerable<MembersCollector> CollectElementsToProxyInternal(IProxyGenerationHook hook)
		{
			Debug.Assert(hook != null, "hook != null");

			foreach (var @interface in this.interfaces)
			{
				var item = this.GetCollectorForInterface(@interface);
				item.Logger = this.Logger;
				item.CollectMembersToProxy(hook);
				yield return item;
			}
		}

		protected virtual MembersCollector GetCollectorForInterface(Type @interface)
		{
			return new InterfaceMembersOnClassCollector(@interface, false, this.proxyTargetType.GetInterfaceMap(@interface));
		}

		private Type GetInvocationType(MetaMethod method, ClassEmitter @class, ProxyGenerationOptions options)
		{
			var scope = @class.ModuleScope;

			Type[] invocationInterfaces;
			if (this.canChangeTarget)
				invocationInterfaces = new[] { typeof(IInvocation), typeof(IChangeProxyTarget) };
			else
				invocationInterfaces = new[] { typeof(IInvocation) };

			var key = new CacheKey(method.Method, CompositionInvocationTypeGenerator.BaseType, invocationInterfaces, null);

			// no locking required as we're already within a lock

			var invocation = scope.GetFromCache(key);
			if (invocation != null)
				return invocation;

			invocation = new CompositionInvocationTypeGenerator(method.Method.DeclaringType,
				method,
				method.Method,
				this.canChangeTarget,
				null)
				.Generate(@class, options, this.namingScope).BuildType();

			scope.RegisterInCache(key, invocation);

			return invocation;
		}

		protected override MethodGenerator GetMethodGenerator(MetaMethod method, ClassEmitter @class,
			ProxyGenerationOptions options,
			OverrideMethodDelegate overrideMethod)
		{
			if (!method.Proxyable)
			{
				return new ForwardingMethodGenerator(method,
					overrideMethod,
					(c, m) => c.GetField("__target"));
			}

			var invocation = this.GetInvocationType(method, @class, options);

			return new MethodWithInvocationGenerator(method,
				@class.GetField("__interceptors"),
				invocation,
				(c, m) => c.GetField("__target").ToExpression(),
				overrideMethod,
				null);
		}

		#endregion
	}
}