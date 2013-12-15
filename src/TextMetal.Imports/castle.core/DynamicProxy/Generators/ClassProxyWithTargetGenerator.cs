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
using System.Linq;
using System.Reflection;

using Castle.DynamicProxy.Contributors;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Castle.DynamicProxy.Internal;
using Castle.DynamicProxy.Serialization;

namespace Castle.DynamicProxy.Generators
{
	using System;
#if !SILVERLIGHT
	using System.Xml.Serialization;
#endif

	public class ClassProxyWithTargetGenerator : BaseProxyGenerator
	{
		#region Constructors/Destructors

		public ClassProxyWithTargetGenerator(ModuleScope scope, Type classToProxy, Type[] additionalInterfacesToProxy,
			ProxyGenerationOptions options)
			: base(scope, classToProxy)
		{
			this.CheckNotGenericTypeDefinition(this.targetType, "targetType");
			this.EnsureDoesNotImplementIProxyTargetAccessor(this.targetType, "targetType");
			this.CheckNotGenericTypeDefinitions(additionalInterfacesToProxy, "additionalInterfacesToProxy");

			options.Initialize();
			this.ProxyGenerationOptions = options;
			this.additionalInterfacesToProxy = TypeUtil.GetAllInterfaces(additionalInterfacesToProxy);
		}

		#endregion

		#region Fields/Constants

		private readonly Type[] additionalInterfacesToProxy;

		#endregion

		#region Methods/Operators

		private FieldReference CreateTargetField(ClassEmitter emitter)
		{
			var targetField = emitter.CreateField("__target", this.targetType);
#if !SILVERLIGHT
			emitter.DefineCustomAttributeFor<XmlIgnoreAttribute>(targetField);
#endif
			return targetField;
		}

		private void EnsureDoesNotImplementIProxyTargetAccessor(Type type, string name)
		{
			if (!typeof(IProxyTargetAccessor).IsAssignableFrom(type))
				return;
			var message =
				string.Format(
					"Target type for the proxy implements {0} which is a DynamicProxy infrastructure interface and you should never implement it yourself. Are you trying to proxy an existing proxy?",
					typeof(IProxyTargetAccessor));
			throw new ArgumentException(message, name);
		}

		private Type GenerateType(string name, INamingScope namingScope)
		{
			IEnumerable<ITypeContributor> contributors;
			var implementedInterfaces = this.GetTypeImplementerMapping(out contributors, namingScope);

			var model = new MetaType();
			// Collect methods
			foreach (var contributor in contributors)
				contributor.CollectElementsToProxy(this.ProxyGenerationOptions.Hook, model);
			this.ProxyGenerationOptions.Hook.MethodsInspected();

			var emitter = this.BuildClassEmitter(name, this.targetType, implementedInterfaces);

			this.CreateFields(emitter);
			this.CreateTypeAttributes(emitter);

			// Constructor
			var cctor = this.GenerateStaticConstructor(emitter);

			var targetField = this.CreateTargetField(emitter);
			var constructorArguments = new List<FieldReference> { targetField };

			foreach (var contributor in contributors)
			{
				contributor.Generate(emitter, this.ProxyGenerationOptions);

				// TODO: redo it
				if (contributor is MixinContributor)
					constructorArguments.AddRange((contributor as MixinContributor).Fields);
			}

			// constructor arguments
			var interceptorsField = emitter.GetField("__interceptors");
			constructorArguments.Add(interceptorsField);
			var selector = emitter.GetField("__selector");
			if (selector != null)
				constructorArguments.Add(selector);

			this.GenerateConstructors(emitter, this.targetType, constructorArguments.ToArray());
			this.GenerateParameterlessConstructor(emitter, this.targetType, interceptorsField);

			// Complete type initializer code body
			this.CompleteInitCacheMethod(cctor.CodeBuilder);

			// Crosses fingers and build type

			var proxyType = emitter.BuildType();
			this.InitializeStaticFields(proxyType);
			return proxyType;
		}

		public Type GetGeneratedType()
		{
			var cacheKey = new CacheKey(this.targetType, this.targetType, this.additionalInterfacesToProxy, this.ProxyGenerationOptions);
			return this.ObtainProxyType(cacheKey, this.GenerateType);
		}

		protected virtual IEnumerable<Type> GetTypeImplementerMapping(out IEnumerable<ITypeContributor> contributors,
			INamingScope namingScope)
		{
			var methodsToSkip = new List<MethodInfo>();
			var proxyInstance = new ClassProxyInstanceContributor(this.targetType, methodsToSkip, this.additionalInterfacesToProxy,
				ProxyTypeConstants.ClassWithTarget);
			// TODO: the trick with methodsToSkip is not very nice...
			var proxyTarget = new ClassProxyWithTargetTargetContributor(this.targetType, methodsToSkip, namingScope)
							{ Logger = this.Logger };
			IDictionary<Type, ITypeContributor> typeImplementerMapping = new Dictionary<Type, ITypeContributor>();

			// Order of interface precedence:
			// 1. first target
			// target is not an interface so we do nothing

			var targetInterfaces = this.targetType.GetAllInterfaces();
			// 2. then mixins
			var mixins = new MixinContributor(namingScope, false) { Logger = this.Logger };
			if (this.ProxyGenerationOptions.HasMixins)
			{
				foreach (var mixinInterface in this.ProxyGenerationOptions.MixinData.MixinInterfaces)
				{
					if (targetInterfaces.Contains(mixinInterface))
					{
						// OK, so the target implements this interface. We now do one of two things:
						if (this.additionalInterfacesToProxy.Contains(mixinInterface) &&
							typeImplementerMapping.ContainsKey(mixinInterface) == false)
						{
							this.AddMappingNoCheck(mixinInterface, proxyTarget, typeImplementerMapping);
							proxyTarget.AddInterfaceToProxy(mixinInterface);
						}
						// we do not intercept the interface
						mixins.AddEmptyInterface(mixinInterface);
					}
					else
					{
						if (!typeImplementerMapping.ContainsKey(mixinInterface))
						{
							mixins.AddInterfaceToProxy(mixinInterface);
							this.AddMappingNoCheck(mixinInterface, mixins, typeImplementerMapping);
						}
					}
				}
			}
			var additionalInterfacesContributor = new InterfaceProxyWithoutTargetContributor(namingScope,
				(c, m) => NullExpression.Instance)
												{ Logger = this.Logger };
			// 3. then additional interfaces
			foreach (var @interface in this.additionalInterfacesToProxy)
			{
				if (targetInterfaces.Contains(@interface))
				{
					if (typeImplementerMapping.ContainsKey(@interface))
						continue;

					// we intercept the interface, and forward calls to the target type
					this.AddMappingNoCheck(@interface, proxyTarget, typeImplementerMapping);
					proxyTarget.AddInterfaceToProxy(@interface);
				}
				else if (this.ProxyGenerationOptions.MixinData.ContainsMixin(@interface) == false)
				{
					additionalInterfacesContributor.AddInterfaceToProxy(@interface);
					this.AddMapping(@interface, additionalInterfacesContributor, typeImplementerMapping);
				}
			}
#if !SILVERLIGHT
			// 4. plus special interfaces
			if (this.targetType.IsSerializable)
				this.AddMappingForISerializable(typeImplementerMapping, proxyInstance);
#endif
			try
			{
				this.AddMappingNoCheck(typeof(IProxyTargetAccessor), proxyInstance, typeImplementerMapping);
			}
			catch (ArgumentException)
			{
				this.HandleExplicitlyPassedProxyTargetAccessor(targetInterfaces, this.additionalInterfacesToProxy);
			}

			contributors = new List<ITypeContributor>
							{
								proxyTarget,
								mixins,
								additionalInterfacesContributor,
								proxyInstance
							};
			return typeImplementerMapping.Keys;
		}

		#endregion
	}
}