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

using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Castle.DynamicProxy.Internal;
using Castle.DynamicProxy.Tokens;

namespace Castle.DynamicProxy.Generators
{
	using System;

	public class CompositionInvocationTypeGenerator : InvocationTypeGenerator
	{
		#region Constructors/Destructors

		public CompositionInvocationTypeGenerator(Type target, MetaMethod method, MethodInfo callback, bool canChangeTarget,
			IInvocationCreationContributor contributor)
			: base(target, method, callback, canChangeTarget, contributor)
		{
		}

		#endregion

		#region Fields/Constants

		public static readonly Type BaseType = typeof(CompositionInvocation);

		#endregion

		#region Methods/Operators

		protected override ArgumentReference[] GetBaseCtorArguments(Type targetFieldType,
			ProxyGenerationOptions proxyGenerationOptions,
			out ConstructorInfo baseConstructor)
		{
			baseConstructor = InvocationMethods.CompositionInvocationConstructor;
			return new[]
					{
						new ArgumentReference(targetFieldType),
						new ArgumentReference(typeof(object)),
						new ArgumentReference(typeof(IInterceptor[])),
						new ArgumentReference(typeof(MethodInfo)),
						new ArgumentReference(typeof(object[])),
					};
		}

		protected override Type GetBaseType()
		{
			return BaseType;
		}

		protected override FieldReference GetTargetReference()
		{
			return new FieldReference(InvocationMethods.Target);
		}

		protected override void ImplementInvokeMethodOnTarget(AbstractTypeEmitter invocation, ParameterInfo[] parameters,
			MethodEmitter invokeMethodOnTarget, Reference targetField)
		{
			invokeMethodOnTarget.CodeBuilder.AddStatement(
				new ExpressionStatement(
					new MethodInvocationExpression(SelfReference.Self, InvocationMethods.EnsureValidTarget)));
			base.ImplementInvokeMethodOnTarget(invocation, parameters, invokeMethodOnTarget, targetField);
		}

		#endregion
	}
}