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

using Castle.DynamicProxy.Contributors;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Castle.DynamicProxy.Internal;
using Castle.DynamicProxy.Serialization;

namespace Castle.DynamicProxy.Generators
{
	using System;

	public class InterfaceProxyWithoutTargetGenerator : InterfaceProxyWithTargetGenerator
	{
		#region Constructors/Destructors

		public InterfaceProxyWithoutTargetGenerator(ModuleScope scope, Type @interface)
			: base(scope, @interface)
		{
		}

		#endregion

		#region Properties/Indexers/Events

		protected override string GeneratorType
		{
			get
			{
				return ProxyTypeConstants.InterfaceWithoutTarget;
			}
		}

		#endregion

		#region Methods/Operators

		protected override ITypeContributor AddMappingForTargetType(
			IDictionary<Type, ITypeContributor> interfaceTypeImplementerMapping, Type proxyTargetType,
			ICollection<Type> targetInterfaces, ICollection<Type> additionalInterfaces, INamingScope namingScope)
		{
			var contributor = new InterfaceProxyWithoutTargetContributor(namingScope, (c, m) => NullExpression.Instance)
							{ Logger = this.Logger };
			foreach (var @interface in this.targetType.GetAllInterfaces())
			{
				contributor.AddInterfaceToProxy(@interface);
				this.AddMappingNoCheck(@interface, contributor, interfaceTypeImplementerMapping);
			}
			return contributor;
		}

		protected override Type GenerateType(string typeName, Type proxyTargetType, Type[] interfaces,
			INamingScope namingScope)
		{
			IEnumerable<ITypeContributor> contributors;
			var allInterfaces = this.GetTypeImplementerMapping(interfaces, this.targetType, out contributors, namingScope);
			var model = new MetaType();
			// collect elements
			foreach (var contributor in contributors)
				contributor.CollectElementsToProxy(this.ProxyGenerationOptions.Hook, model);

			this.ProxyGenerationOptions.Hook.MethodsInspected();

			ClassEmitter emitter;
			FieldReference interceptorsField;
			var baseType = this.Init(typeName, out emitter, proxyTargetType, out interceptorsField, allInterfaces);

			// Constructor

			var cctor = this.GenerateStaticConstructor(emitter);
			var mixinFieldsList = new List<FieldReference>();

			foreach (var contributor in contributors)
			{
				contributor.Generate(emitter, this.ProxyGenerationOptions);

				// TODO: redo it
				if (contributor is MixinContributor)
					mixinFieldsList.AddRange((contributor as MixinContributor).Fields);
			}

			var ctorArguments = new List<FieldReference>(mixinFieldsList) { interceptorsField, this.targetField };
			var selector = emitter.GetField("__selector");
			if (selector != null)
				ctorArguments.Add(selector);

			this.GenerateConstructors(emitter, baseType, ctorArguments.ToArray());

			// Complete type initializer code body
			this.CompleteInitCacheMethod(cctor.CodeBuilder);

			// Crosses fingers and build type
			var generatedType = emitter.BuildType();

			this.InitializeStaticFields(generatedType);
			return generatedType;
		}

		#endregion
	}
}