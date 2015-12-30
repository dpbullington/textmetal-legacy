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

using Castle.DynamicProxy.Generators;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Castle.DynamicProxy.Internal;

namespace Castle.DynamicProxy.Contributors
{
	using System;

	public class DelegateTypeGenerator : IGenerator<AbstractTypeEmitter>
	{
		#region Constructors/Destructors

		public DelegateTypeGenerator(MetaMethod method, Type targetType)
		{
			this.method = method;
			this.targetType = targetType;
		}

		#endregion

		#region Fields/Constants

		private const TypeAttributes DelegateFlags = TypeAttributes.Class |
													TypeAttributes.Public |
													TypeAttributes.Sealed |
													TypeAttributes.AnsiClass |
													TypeAttributes.AutoClass;

		private readonly MetaMethod method;
		private readonly Type targetType;

		#endregion

		#region Methods/Operators

		private void BuildConstructor(AbstractTypeEmitter emitter)
		{
			var constructor = emitter.CreateConstructor(new ArgumentReference(typeof(object)),
				new ArgumentReference(typeof(IntPtr)));
			constructor.ConstructorBuilder.SetImplementationFlags(MethodImplAttributes.Runtime | MethodImplAttributes.Managed);
		}

		private void BuildInvokeMethod(AbstractTypeEmitter @delegate)
		{
			var paramTypes = this.GetParamTypes(@delegate);
			var invoke = @delegate.CreateMethod("Invoke",
				MethodAttributes.Public |
				MethodAttributes.HideBySig |
				MethodAttributes.NewSlot |
				MethodAttributes.Virtual,
				@delegate.GetClosedParameterType(this.method.MethodOnTarget.ReturnType),
				paramTypes);
			invoke.MethodBuilder.SetImplementationFlags(MethodImplAttributes.Runtime | MethodImplAttributes.Managed);
		}

		public AbstractTypeEmitter Generate(ClassEmitter @class, ProxyGenerationOptions options, INamingScope namingScope)
		{
			var emitter = this.GetEmitter(@class, namingScope);
			this.BuildConstructor(emitter);
			this.BuildInvokeMethod(emitter);
			return emitter;
		}

		private AbstractTypeEmitter GetEmitter(ClassEmitter @class, INamingScope namingScope)
		{
			var methodInfo = this.method.MethodOnTarget;
			var suggestedName = string.Format("Castle.Proxies.Delegates.{0}_{1}",
				methodInfo.DeclaringType.Name, this.method.Method.Name);
			var uniqueName = namingScope.ParentScope.GetUniqueName(suggestedName);

			var @delegate = new ClassEmitter(@class.ModuleScope,
				uniqueName,
				typeof(MulticastDelegate),
				Type.EmptyTypes,
				DelegateFlags);
			@delegate.CopyGenericParametersFromMethod(this.method.Method);
			return @delegate;
		}

		private Type[] GetParamTypes(AbstractTypeEmitter @delegate)
		{
			var parameters = this.method.MethodOnTarget.GetParameters();
#if NETCORE
			if (@delegate.TypeBuilder.IsGenericType)
#else
			if (@delegate.TypeBuilder.GetTypeInfo().IsGenericType)
#endif
			{
				var types = new Type[parameters.Length];

				for (var i = 0; i < parameters.Length; i++)
					types[i] = @delegate.GetClosedParameterType(parameters[i].ParameterType);
				return types;
			}
			var paramTypes = new Type[parameters.Length + 1];
			paramTypes[0] = this.targetType;
			for (var i = 0; i < parameters.Length; i++)
				paramTypes[i + 1] = @delegate.GetClosedParameterType(parameters[i].ParameterType);
			return paramTypes;
		}

		#endregion
	}
}