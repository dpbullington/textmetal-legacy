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

	public class InvocationWithDelegateContributor : IInvocationCreationContributor
	{
		#region Constructors/Destructors

		public InvocationWithDelegateContributor(Type delegateType, Type targetType, MetaMethod method,
			INamingScope namingScope)
		{
			Debug.Assert(delegateType.IsGenericType == false, "delegateType.IsGenericType == false");
			this.delegateType = delegateType;
			this.targetType = targetType;
			this.method = method;
			this.namingScope = namingScope;
		}

		#endregion

		#region Fields/Constants

		private readonly Type delegateType;
		private readonly MetaMethod method;
		private readonly INamingScope namingScope;
		private readonly Type targetType;

		#endregion

		#region Methods/Operators

		private FieldReference BuildDelegateToken(ClassEmitter proxy)
		{
			var callback = proxy.CreateStaticField(this.namingScope.GetUniqueName("callback_" + this.method.Method.Name), this.delegateType);
			var createDelegate = new MethodInvocationExpression(
				null,
				DelegateMethods.CreateDelegate,
				new TypeTokenExpression(this.delegateType),
				NullExpression.Instance,
				new MethodTokenExpression(this.method.MethodOnTarget));
			var bindDelegate = new AssignStatement(callback, new ConvertExpression(this.delegateType, createDelegate));

			proxy.ClassConstructor.CodeBuilder.AddStatement(bindDelegate);
			return callback;
		}

		public ConstructorEmitter CreateConstructor(ArgumentReference[] baseCtorArguments, AbstractTypeEmitter invocation)
		{
			var arguments = this.GetArguments(baseCtorArguments);
			var constructor = invocation.CreateConstructor(arguments);

			var delegateField = invocation.CreateField("delegate", this.delegateType);
			constructor.CodeBuilder.AddStatement(new AssignStatement(delegateField, new ReferenceExpression(arguments[0])));
			return constructor;
		}

		private Expression[] GetAllArgs(Expression[] args, Reference targetField)
		{
			var allArgs = new Expression[args.Length + 1];
			args.CopyTo(allArgs, 1);
			allArgs[0] = new ConvertExpression(this.targetType, targetField.ToExpression());
			return allArgs;
		}

		private ArgumentReference[] GetArguments(ArgumentReference[] baseCtorArguments)
		{
			var arguments = new ArgumentReference[baseCtorArguments.Length + 1];
			arguments[0] = new ArgumentReference(this.delegateType);
			baseCtorArguments.CopyTo(arguments, 1);
			return arguments;
		}

		public MethodInfo GetCallbackMethod()
		{
			return this.delegateType.GetMethod("Invoke");
		}

		public MethodInvocationExpression GetCallbackMethodInvocation(AbstractTypeEmitter invocation, Expression[] args,
			Reference targetField,
			MethodEmitter invokeMethodOnTarget)
		{
			var allArgs = this.GetAllArgs(args, targetField);
			var @delegate = (Reference)invocation.GetField("delegate");

			return new MethodInvocationExpression(@delegate, this.GetCallbackMethod(), allArgs);
		}

		public Expression[] GetConstructorInvocationArguments(Expression[] arguments, ClassEmitter proxy)
		{
			var allArguments = new Expression[arguments.Length + 1];
			allArguments[0] = new ReferenceExpression(this.BuildDelegateToken(proxy));
			Array.Copy(arguments, 0, allArguments, 1, arguments.Length);
			return allArguments;
		}

		#endregion
	}
}