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

using Castle.DynamicProxy.Generators;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Castle.DynamicProxy.Tokens;

namespace Castle.DynamicProxy.Contributors
{
	using System;

	public class InvocationWithGenericDelegateContributor : IInvocationCreationContributor
	{
		#region Constructors/Destructors

		public InvocationWithGenericDelegateContributor(Type delegateType, MetaMethod method, Reference targetReference)
		{
			Debug.Assert(delegateType.IsGenericType, "delegateType.IsGenericType");
			this.delegateType = delegateType;
			this.method = method;
			this.targetReference = targetReference;
		}

		#endregion

		#region Fields/Constants

		private readonly Type delegateType;
		private readonly MetaMethod method;
		private readonly Reference targetReference;

		#endregion

		#region Methods/Operators

		public ConstructorEmitter CreateConstructor(ArgumentReference[] baseCtorArguments, AbstractTypeEmitter invocation)
		{
			return invocation.CreateConstructor(baseCtorArguments);
		}

		public MethodInfo GetCallbackMethod()
		{
			return this.delegateType.GetMethod("Invoke");
		}

		public MethodInvocationExpression GetCallbackMethodInvocation(AbstractTypeEmitter invocation, Expression[] args,
			Reference targetField,
			MethodEmitter invokeMethodOnTarget)
		{
			var @delegate = this.GetDelegate(invocation, invokeMethodOnTarget);
			return new MethodInvocationExpression(@delegate, this.GetCallbackMethod(), args);
		}

		public Expression[] GetConstructorInvocationArguments(Expression[] arguments, ClassEmitter proxy)
		{
			return arguments;
		}

		private Reference GetDelegate(AbstractTypeEmitter invocation, MethodEmitter invokeMethodOnTarget)
		{
			var closedDelegateType = this.delegateType.MakeGenericType(invocation.GenericTypeParams);
			var localReference = invokeMethodOnTarget.CodeBuilder.DeclareLocal(closedDelegateType);
			var closedMethodOnTarget = this.method.MethodOnTarget.MakeGenericMethod(invocation.GenericTypeParams);
			var localTarget = new ReferenceExpression(this.targetReference);
			invokeMethodOnTarget.CodeBuilder.AddStatement(
				this.SetDelegate(localReference, localTarget, closedDelegateType, closedMethodOnTarget));
			return localReference;
		}

		private AssignStatement SetDelegate(LocalReference localDelegate, ReferenceExpression localTarget,
			Type closedDelegateType, MethodInfo closedMethodOnTarget)
		{
			var delegateCreateDelegate = new MethodInvocationExpression(
				null,
				DelegateMethods.CreateDelegate,
				new TypeTokenExpression(closedDelegateType),
				localTarget,
				new MethodTokenExpression(closedMethodOnTarget));
			return new AssignStatement(localDelegate, new ConvertExpression(closedDelegateType, delegateCreateDelegate));
		}

		#endregion
	}
}