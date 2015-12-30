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
using System.Reflection;

using Castle.DynamicProxy.Contributors;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Castle.DynamicProxy.Internal;
using Castle.DynamicProxy.Serialization;

namespace Castle.DynamicProxy.Generators
{
	using System;

	public class InterfaceProxyWithTargetInterfaceGenerator : InterfaceProxyWithTargetGenerator
	{
		#region Constructors/Destructors

		public InterfaceProxyWithTargetInterfaceGenerator(ModuleScope scope, Type @interface)
			: base(scope, @interface)
		{
		}

		#endregion

		#region Properties/Indexers/Events

		protected override bool AllowChangeTarget
		{
			get
			{
				return true;
			}
		}

		protected override string GeneratorType
		{
			get
			{
				return ProxyTypeConstants.InterfaceWithTargetInterface;
			}
		}

		#endregion

		#region Methods/Operators

		protected override ITypeContributor AddMappingForTargetType(
			IDictionary<Type, ITypeContributor> typeImplementerMapping, Type proxyTargetType, ICollection<Type> targetInterfaces,
			ICollection<Type> additionalInterfaces, INamingScope namingScope)
		{
			var contributor = new InterfaceProxyWithTargetInterfaceTargetContributor(
				proxyTargetType, this.AllowChangeTarget,
				namingScope) { Logger = this.Logger };
			foreach (var @interface in this.targetType.GetAllInterfaces())
			{
				contributor.AddInterfaceToProxy(@interface);
				this.AddMappingNoCheck(@interface, contributor, typeImplementerMapping);
			}

			return contributor;
		}

		protected override InterfaceProxyWithoutTargetContributor GetContributorForAdditionalInterfaces(
			INamingScope namingScope)
		{
			return new InterfaceProxyWithOptionalTargetContributor(namingScope, this.GetTargetExpression, this.GetTarget)
					{ Logger = this.Logger };
		}

		private Reference GetTarget(ClassEmitter @class, MethodInfo method)
		{
			return new AsTypeReference(@class.GetField("__target"), method.DeclaringType);
		}

		private Expression GetTargetExpression(ClassEmitter @class, MethodInfo method)
		{
			return this.GetTarget(@class, method).ToExpression();
		}

		#endregion
	}
}