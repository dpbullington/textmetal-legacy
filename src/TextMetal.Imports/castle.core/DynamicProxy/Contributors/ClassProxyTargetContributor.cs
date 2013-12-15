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
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using Castle.DynamicProxy.Generators;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Castle.DynamicProxy.Tokens;

namespace Castle.DynamicProxy.Contributors
{
	using System;

	public class ClassProxyTargetContributor : CompositeTypeContributor
	{
		#region Constructors/Destructors

		public ClassProxyTargetContributor(Type targetType, IList<MethodInfo> methodsToSkip, INamingScope namingScope)
			: base(namingScope)
		{
			this.targetType = targetType;
			this.methodsToSkip = methodsToSkip;
		}

		#endregion

		#region Fields/Constants

		private readonly IList<MethodInfo> methodsToSkip;
		private readonly Type targetType;

		#endregion

		#region Methods/Operators

		private Type BuildInvocationType(MetaMethod method, ClassEmitter @class, ProxyGenerationOptions options)
		{
			var methodInfo = method.Method;
			if (!method.HasTarget)
			{
				return new InheritanceInvocationTypeGenerator(this.targetType,
					method,
					null, null)
					.Generate(@class, options, this.namingScope)
					.BuildType();
			}
			var callback = this.CreateCallbackMethod(@class, methodInfo, method.MethodOnTarget);
			return new InheritanceInvocationTypeGenerator(callback.DeclaringType,
				method,
				callback, null)
				.Generate(@class, options, this.namingScope)
				.BuildType();
		}

		protected override IEnumerable<MembersCollector> CollectElementsToProxyInternal(IProxyGenerationHook hook)
		{
			Debug.Assert(hook != null, "hook != null");

			var targetItem = new ClassMembersCollector(this.targetType) { Logger = this.Logger };
			targetItem.CollectMembersToProxy(hook);
			yield return targetItem;

			foreach (var @interface in this.interfaces)
			{
				var item = new InterfaceMembersOnClassCollector(@interface,
					true,
					this.targetType.GetInterfaceMap(@interface)) { Logger = this.Logger };
				item.CollectMembersToProxy(hook);
				yield return item;
			}
		}

		private MethodBuilder CreateCallbackMethod(ClassEmitter emitter, MethodInfo methodInfo, MethodInfo methodOnTarget)
		{
			var targetMethod = methodOnTarget ?? methodInfo;
			var callBackMethod = emitter.CreateMethod(this.namingScope.GetUniqueName(methodInfo.Name + "_callback"), targetMethod);

			if (targetMethod.IsGenericMethod)
				targetMethod = targetMethod.MakeGenericMethod(callBackMethod.GenericTypeParams);

			var exps = new Expression[callBackMethod.Arguments.Length];
			for (var i = 0; i < callBackMethod.Arguments.Length; i++)
				exps[i] = callBackMethod.Arguments[i].ToExpression();

			// invocation on base class

			callBackMethod.CodeBuilder.AddStatement(
				new ReturnStatement(
					new MethodInvocationExpression(SelfReference.Self,
						targetMethod,
						exps)));

			return callBackMethod.MethodBuilder;
		}

		private bool ExplicitlyImplementedInterfaceMethod(MetaMethod method)
		{
			return method.MethodOnTarget.IsPrivate;
		}

		private MethodGenerator ExplicitlyImplementedInterfaceMethodGenerator(MetaMethod method, ClassEmitter @class,
			ProxyGenerationOptions options,
			OverrideMethodDelegate overrideMethod)
		{
			var @delegate = this.GetDelegateType(method, @class, options);
			var contributor = this.GetContributor(@delegate, method);
			var invocation = new InheritanceInvocationTypeGenerator(this.targetType, method, null, contributor)
				.Generate(@class, options, this.namingScope)
				.BuildType();
			return new MethodWithInvocationGenerator(method,
				@class.GetField("__interceptors"),
				invocation,
				(c, m) => new TypeTokenExpression(this.targetType),
				overrideMethod,
				contributor);
		}

		private IInvocationCreationContributor GetContributor(Type @delegate, MetaMethod method)
		{
			if (@delegate.IsGenericType == false)
				return new InvocationWithDelegateContributor(@delegate, this.targetType, method, this.namingScope);
			return new InvocationWithGenericDelegateContributor(@delegate,
				method,
				new FieldReference(InvocationMethods.ProxyObject));
		}

		private Type GetDelegateType(MetaMethod method, ClassEmitter @class, ProxyGenerationOptions options)
		{
			var scope = @class.ModuleScope;
			var key = new CacheKey(
				typeof(Delegate),
				this.targetType,
				new[] { method.MethodOnTarget.ReturnType }
					.Concat(ArgumentsUtil.GetTypes(method.MethodOnTarget.GetParameters())).
					ToArray(),
				null);

			var type = scope.GetFromCache(key);
			if (type != null)
				return type;

			type = new DelegateTypeGenerator(method, this.targetType)
				.Generate(@class, options, this.namingScope)
				.BuildType();

			scope.RegisterInCache(key, type);

			return type;
		}

		private Type GetInvocationType(MetaMethod method, ClassEmitter @class, ProxyGenerationOptions options)
		{
			// NOTE: No caching since invocation is tied to this specific proxy type via its invocation method
			return this.BuildInvocationType(method, @class, options);
		}

		protected override MethodGenerator GetMethodGenerator(MetaMethod method, ClassEmitter @class,
			ProxyGenerationOptions options,
			OverrideMethodDelegate overrideMethod)
		{
			if (this.methodsToSkip.Contains(method.Method))
				return null;

			if (!method.Proxyable)
			{
				return new MinimialisticMethodGenerator(method,
					overrideMethod);
			}

			if (this.ExplicitlyImplementedInterfaceMethod(method))
			{
#if SILVERLIGHT
				return null;
#else
				return this.ExplicitlyImplementedInterfaceMethodGenerator(method, @class, options, overrideMethod);
#endif
			}

			var invocation = this.GetInvocationType(method, @class, options);

			return new MethodWithInvocationGenerator(method,
				@class.GetField("__interceptors"),
				invocation,
				(c, m) => new TypeTokenExpression(this.targetType),
				overrideMethod,
				null);
		}

		#endregion
	}
}