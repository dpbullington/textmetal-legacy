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
using System.Reflection.Emit;

using Castle.DynamicProxy.Generators.Emitters.CodeBuilders;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Castle.DynamicProxy.Internal;

namespace Castle.DynamicProxy.Generators.Emitters
{
	using System;

	[DebuggerDisplay("{builder.Name}")]
	public class MethodEmitter : IMemberEmitter
	{
		#region Constructors/Destructors

		protected internal MethodEmitter(MethodBuilder builder)
		{
			this.builder = builder;
		}

		internal MethodEmitter(AbstractTypeEmitter owner, String name, MethodAttributes attributes)
			: this(owner.TypeBuilder.DefineMethod(name, attributes))
		{
		}

		internal MethodEmitter(AbstractTypeEmitter owner, String name,
			MethodAttributes attributes, Type returnType,
			params Type[] argumentTypes)
			: this(owner, name, attributes)
		{
			this.SetParameters(argumentTypes);
			this.SetReturnType(returnType);
		}

		internal MethodEmitter(AbstractTypeEmitter owner, String name,
			MethodAttributes attributes, MethodInfo methodToUseAsATemplate)
			: this(owner, name, attributes)
		{
			var name2GenericType = GenericUtil.GetGenericArgumentsMap(owner);

			var returnType = GenericUtil.ExtractCorrectType(methodToUseAsATemplate.ReturnType, name2GenericType);
			var baseMethodParameters = methodToUseAsATemplate.GetParameters();
			var parameters = GenericUtil.ExtractParametersTypes(baseMethodParameters, name2GenericType);

			this.genericTypeParams = GenericUtil.CopyGenericArguments(methodToUseAsATemplate, this.builder, name2GenericType);
			this.SetParameters(parameters);
			this.SetReturnType(returnType);
			this.SetSignature(returnType, methodToUseAsATemplate.ReturnParameter, parameters, baseMethodParameters);
			this.DefineParameters(baseMethodParameters);
		}

		#endregion

		#region Fields/Constants

		private readonly MethodBuilder builder;
		private readonly GenericTypeParameterBuilder[] genericTypeParams;

		private ArgumentReference[] arguments;

		private MethodCodeBuilder codebuilder;

		#endregion

		#region Properties/Indexers/Events

		public ArgumentReference[] Arguments
		{
			get
			{
				return this.arguments;
			}
		}

		public virtual MethodCodeBuilder CodeBuilder
		{
			get
			{
				if (this.codebuilder == null)
					this.codebuilder = new MethodCodeBuilder(this.builder.GetILGenerator());
				return this.codebuilder;
			}
		}

		public GenericTypeParameterBuilder[] GenericTypeParams
		{
			get
			{
				return this.genericTypeParams;
			}
		}

		private bool ImplementedByRuntime
		{
			get
			{
#if NETCORE
				var attributes = this.builder.MethodImplementationFlags;
#else
				var attributes = builder.GetMethodImplementationFlags();
#endif
				return (attributes & MethodImplAttributes.Runtime) != 0;
			}
		}

		public MemberInfo Member
		{
			get
			{
				return this.builder;
			}
		}

		public MethodBuilder MethodBuilder
		{
			get
			{
				return this.builder;
			}
		}

		public Type ReturnType
		{
			get
			{
				return this.builder.ReturnType;
			}
		}

		#endregion

		#region Methods/Operators

		public void DefineCustomAttribute(CustomAttributeBuilder attribute)
		{
			this.builder.SetCustomAttribute(attribute);
		}

		private void DefineParameters(ParameterInfo[] parameters)
		{
			foreach (var parameter in parameters)
			{
				var parameterBuilder = this.builder.DefineParameter(parameter.Position + 1, parameter.Attributes, parameter.Name);
				foreach (var attribute in parameter.GetNonInheritableAttributes())
					parameterBuilder.SetCustomAttribute(attribute);
#if DOTNET45
				if (parameter.HasDefaultValue && parameter.DefaultValue != null)
				{
					if (parameter.ParameterType == typeof(decimal) || parameter.ParameterType == typeof(decimal?))
					{
						/*
						 * Because of a limitation of the .NET Framework, a decimal value may not 
						 * be passed to SetConstant(), so omit the call in this instance.
						 * 
						 * See https://github.com/castleproject/Core/issues/87 and
						 * https://msdn.microsoft.com/en-au/library/system.reflection.emit.parameterbuilder.setconstant(v=vs.110).aspx
						 * for additional information.
						 */
						continue;
					}
					parameterBuilder.SetConstant(parameter.DefaultValue);
				}
#endif
			}
		}

		public virtual void EnsureValidCodeBlock()
		{
			if (this.ImplementedByRuntime == false && this.CodeBuilder.IsEmpty)
			{
				this.CodeBuilder.AddStatement(new NopStatement());
				this.CodeBuilder.AddStatement(new ReturnStatement());
			}
		}

		public virtual void Generate()
		{
			if (this.ImplementedByRuntime)
				return;

			this.codebuilder.Generate(this, this.builder.GetILGenerator());
		}

		public void SetParameters(Type[] paramTypes)
		{
			this.builder.SetParameters(paramTypes);
			this.arguments = ArgumentsUtil.ConvertToArgumentReference(paramTypes);
			ArgumentsUtil.InitializeArgumentsByPosition(this.arguments, this.MethodBuilder.IsStatic);
		}

		private void SetReturnType(Type returnType)
		{
			this.builder.SetReturnType(returnType);
		}

		private void SetSignature(Type returnType, ParameterInfo returnParameter, Type[] parameters,
			ParameterInfo[] baseMethodParameters)
		{
			this.builder.SetSignature(
				returnType,
#if SILVERLIGHT || NETCORE
				null,
				null,
#else
				returnParameter.GetRequiredCustomModifiers(),
				returnParameter.GetOptionalCustomModifiers(),
#endif
				parameters,
#if SILVERLIGHT || NETCORE
				null,
				null
#else
				baseMethodParameters.Select(x => x.GetRequiredCustomModifiers()).ToArray(),
				baseMethodParameters.Select(x => x.GetOptionalCustomModifiers()).ToArray()
#endif
				);
		}

		#endregion
	}
}