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

using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

using Castle.DynamicProxy.Generators;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

namespace Castle.DynamicProxy.Contributors
{
	using System;

	public class MixinContributor : CompositeTypeContributor
	{
		#region Constructors/Destructors

		public MixinContributor(INamingScope namingScope, bool canChangeTarget)
			: base(namingScope)
		{
			this.canChangeTarget = canChangeTarget;
			this.getTargetExpression = this.BuildGetTargetExpression();
		}

		#endregion

		#region Fields/Constants

		private readonly bool canChangeTarget;
		private readonly IList<Type> empty = new List<Type>();
		private readonly IDictionary<Type, FieldReference> fields = new Dictionary<Type, FieldReference>();
		private readonly GetTargetExpressionDelegate getTargetExpression;

		#endregion

		#region Properties/Indexers/Events

		public IEnumerable<FieldReference> Fields
		{
			get
			{
				return this.fields.Values;
			}
		}

		#endregion

		#region Methods/Operators

		public void AddEmptyInterface(Type @interface)
		{
			Debug.Assert(@interface != null, "@interface == null", "Shouldn't be adding empty interfaces...");
			Debug.Assert(@interface.GetTypeInfo().IsInterface, "@interface.IsInterface", "Should be adding interfaces only...");

			Debug.Assert(!this.interfaces.Contains(@interface), "!interfaces.Contains(@interface)",
				"Shouldn't be adding same interface twice...");
			Debug.Assert(!this.empty.Contains(@interface), "!empty.Contains(@interface)",
				"Shouldn't be adding same interface twice...");
			this.empty.Add(@interface);
		}

		private GetTargetExpressionDelegate BuildGetTargetExpression()
		{
			if (!this.canChangeTarget)
				return (c, m) => this.fields[m.DeclaringType].ToExpression();

			return (c, m) => new NullCoalescingOperatorExpression(
				new AsTypeReference(c.GetField("__target"), m.DeclaringType).ToExpression(), this.fields[m.DeclaringType].ToExpression());
		}

		private FieldReference BuildTargetField(ClassEmitter @class, Type type)
		{
			var name = "__mixin_" + type.FullName.Replace(".", "_");
			return @class.CreateField(this.namingScope.GetUniqueName(name), type);
		}

		protected override IEnumerable<MembersCollector> CollectElementsToProxyInternal(IProxyGenerationHook hook)
		{
			foreach (var @interface in this.interfaces)
			{
				var item = new InterfaceMembersCollector(@interface);
				item.CollectMembersToProxy(hook);
				yield return item;
			}
		}

		public override void Generate(ClassEmitter @class, ProxyGenerationOptions options)
		{
			foreach (var @interface in this.interfaces)
				this.fields[@interface] = this.BuildTargetField(@class, @interface);

			foreach (var emptyInterface in this.empty)
				this.fields[emptyInterface] = this.BuildTargetField(@class, emptyInterface);

			base.Generate(@class, options);
		}

		private Type GetInvocationType(MetaMethod method, ClassEmitter emitter, ProxyGenerationOptions options)
		{
			var scope = emitter.ModuleScope;
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
				method.Method, this.canChangeTarget,
				null)
				.Generate(emitter, options, this.namingScope)
				.BuildType();

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
					(c, i) => this.fields[i.DeclaringType]);
			}

			var invocation = this.GetInvocationType(method, @class, options);
			return new MethodWithInvocationGenerator(method,
				@class.GetField("__interceptors"),
				invocation, this.getTargetExpression,
				overrideMethod,
				null);
		}

		#endregion
	}
}