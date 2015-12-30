// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Copyright 2004-2014 Castle Project - http://www.castleproject.org/
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
using System.Reflection;

using Castle.Core.Internal;
using Castle.Core.Logging;
using Castle.DynamicProxy.Generators;
using Castle.DynamicProxy.Internal;

namespace Castle.DynamicProxy
{
	using System;

	/// <summary>
	/// Default implementation of <see cref="IProxyBuilder" /> interface producing in-memory proxy assemblies.
	/// </summary>
	public class DefaultProxyBuilder : IProxyBuilder
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultProxyBuilder" /> class with new <see cref="ModuleScope" />.
		/// </summary>
		public DefaultProxyBuilder()
			: this(new ModuleScope())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultProxyBuilder" /> class.
		/// </summary>
		/// <param name="scope"> The module scope for generated proxy types. </param>
		public DefaultProxyBuilder(ModuleScope scope)
		{
			this.scope = scope;
		}

		#endregion

		#region Fields/Constants

		private readonly ModuleScope scope;
		private ILogger logger = NullLogger.Instance;

		#endregion

		#region Properties/Indexers/Events

		public ModuleScope ModuleScope
		{
			get
			{
				return this.scope;
			}
		}

		public ILogger Logger
		{
			get
			{
				return this.logger;
			}
			set
			{
				this.logger = value;
			}
		}

		#endregion

		#region Methods/Operators

		private static bool IsInternal(Type target)
		{
			var isTargetNested = target.IsNested;
			var isNestedAndInternal = isTargetNested && (target.GetTypeInfo().IsNestedAssembly || target.GetTypeInfo().IsNestedFamORAssem);
#if NETCORE
			var isInternalNotNested = target.GetTypeInfo().IsVisible == false && isTargetNested == false;
#else
			var isInternalNotNested = target.IsVisible == false && isTargetNested == false;
#endif
			return isInternalNotNested || isNestedAndInternal;
		}

		private void AssertValidType(Type target)
		{
			this.AssertValidTypeForTarget(target, target);
		}

		private void AssertValidTypeForTarget(Type type, Type target)
		{
			if (type.GetTypeInfo().IsGenericTypeDefinition)

			{
				throw new GeneratorException(string.Format("Can not create proxy for type {0} because type {1} is an open generic type.",
					target.GetBestName(), type.GetBestName()));
			}
			if (this.IsPublic(type) == false && this.IsAccessible(type) == false)
				throw new GeneratorException(ExceptionMessageBuilder.CreateMessageForInaccessibleType(type, target));
			foreach (var typeArgument in type.GetGenericArguments())
				this.AssertValidTypeForTarget(typeArgument, target);
		}

		private void AssertValidTypes(IEnumerable<Type> targetTypes)
		{
			if (targetTypes != null)
			{
				foreach (var t in targetTypes)
					this.AssertValidType(t);
			}
		}

		public Type CreateClassProxyType(Type classToProxy, Type[] additionalInterfacesToProxy, ProxyGenerationOptions options)
		{
			this.AssertValidType(classToProxy);
			this.AssertValidTypes(additionalInterfacesToProxy);

			var generator = new ClassProxyGenerator(this.scope, classToProxy) { Logger = this.logger };
			return generator.GenerateCode(additionalInterfacesToProxy, options);
		}

		public Type CreateClassProxyTypeWithTarget(Type classToProxy, Type[] additionalInterfacesToProxy,
			ProxyGenerationOptions options)
		{
			this.AssertValidType(classToProxy);
			this.AssertValidTypes(additionalInterfacesToProxy);
			var generator = new ClassProxyWithTargetGenerator(this.scope, classToProxy, additionalInterfacesToProxy, options)
							{ Logger = this.logger };
			return generator.GetGeneratedType();
		}

		public Type CreateInterfaceProxyTypeWithoutTarget(Type interfaceToProxy, Type[] additionalInterfacesToProxy,
			ProxyGenerationOptions options)
		{
			this.AssertValidType(interfaceToProxy);
			this.AssertValidTypes(additionalInterfacesToProxy);

			var generator = new InterfaceProxyWithoutTargetGenerator(this.scope, interfaceToProxy) { Logger = this.logger };
			return generator.GenerateCode(typeof(object), additionalInterfacesToProxy, options);
		}

		public Type CreateInterfaceProxyTypeWithTarget(Type interfaceToProxy, Type[] additionalInterfacesToProxy,
			Type targetType,
			ProxyGenerationOptions options)
		{
			this.AssertValidType(interfaceToProxy);
			this.AssertValidTypes(additionalInterfacesToProxy);

			var generator = new InterfaceProxyWithTargetGenerator(this.scope, interfaceToProxy) { Logger = this.logger };
			return generator.GenerateCode(targetType, additionalInterfacesToProxy, options);
		}

		public Type CreateInterfaceProxyTypeWithTargetInterface(Type interfaceToProxy, Type[] additionalInterfacesToProxy,
			ProxyGenerationOptions options)
		{
			this.AssertValidType(interfaceToProxy);
			this.AssertValidTypes(additionalInterfacesToProxy);

			var generator = new InterfaceProxyWithTargetInterfaceGenerator(this.scope, interfaceToProxy) { Logger = this.logger };
			return generator.GenerateCode(interfaceToProxy, additionalInterfacesToProxy, options);
		}

		private bool IsAccessible(Type target)
		{
#if NETCORE
			return IsInternal(target) && target.GetTypeInfo().Assembly.IsInternalToDynamicProxy();
#else
			return IsInternal(target) && target.Assembly.IsInternalToDynamicProxy();
#endif
		}

		private bool IsPublic(Type target)
		{
			return target.GetTypeInfo().IsPublic || target.GetTypeInfo().IsNestedPublic;
		}

		#endregion
	}
}