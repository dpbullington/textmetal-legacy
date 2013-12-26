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

	public class InterfaceProxyWithTargetGenerator : BaseProxyGenerator
	{
		protected FieldReference targetField;

		public InterfaceProxyWithTargetGenerator(ModuleScope scope, Type @interface)
			: base(scope, @interface)
		{
			this.CheckNotGenericTypeDefinition(@interface, "@interface");
		}

		protected virtual bool AllowChangeTarget
		{
			get
			{
				return false;
			}
		}

		protected virtual string GeneratorType
		{
			get
			{
				return ProxyTypeConstants.InterfaceWithTarget;
			}
		}

		public Type GenerateCode(Type proxyTargetType, Type[] interfaces, ProxyGenerationOptions options)
		{
			// make sure ProxyGenerationOptions is initialized
			options.Initialize();

			this.CheckNotGenericTypeDefinition(proxyTargetType, "proxyTargetType");
			this.CheckNotGenericTypeDefinitions(interfaces, "interfaces");
			this.EnsureValidBaseType(options.BaseTypeForInterfaceProxy);
			this.ProxyGenerationOptions = options;

			interfaces = TypeUtil.GetAllInterfaces(interfaces);
			var cacheKey = new CacheKey(proxyTargetType, this.targetType, interfaces, options);

			return this.ObtainProxyType(cacheKey, (n, s) => this.GenerateType(n, proxyTargetType, interfaces, s));
		}

		protected virtual ITypeContributor AddMappingForTargetType(IDictionary<Type, ITypeContributor> typeImplementerMapping,
			Type proxyTargetType, ICollection<Type> targetInterfaces,
			ICollection<Type> additionalInterfaces,
			INamingScope namingScope)
		{
			var contributor = new InterfaceProxyTargetContributor(proxyTargetType, this.AllowChangeTarget, namingScope)
							{ Logger = this.Logger };
			var proxiedInterfaces = this.targetType.GetAllInterfaces();
			foreach (var @interface in proxiedInterfaces)
			{
				contributor.AddInterfaceToProxy(@interface);
				this.AddMappingNoCheck(@interface, contributor, typeImplementerMapping);
			}

			foreach (var @interface in additionalInterfaces)
			{
				if (!this.ImplementedByTarget(targetInterfaces, @interface) || proxiedInterfaces.Contains(@interface))
					continue;

				contributor.AddInterfaceToProxy(@interface);
				this.AddMappingNoCheck(@interface, contributor, typeImplementerMapping);
			}
			return contributor;
		}

#if (!SILVERLIGHT)
		protected override void CreateTypeAttributes(ClassEmitter emitter)
		{
			base.CreateTypeAttributes(emitter);
			emitter.DefineCustomAttribute<SerializableAttribute>();
		}
#endif

		protected virtual Type GenerateType(string typeName, Type proxyTargetType, Type[] interfaces, INamingScope namingScope)
		{
			IEnumerable<ITypeContributor> contributors;
			var allInterfaces = this.GetTypeImplementerMapping(interfaces, proxyTargetType, out contributors, namingScope);

			ClassEmitter emitter;
			FieldReference interceptorsField;
			var baseType = this.Init(typeName, out emitter, proxyTargetType, out interceptorsField, allInterfaces);

			var model = new MetaType();
			// Collect methods
			foreach (var contributor in contributors)
				contributor.CollectElementsToProxy(this.ProxyGenerationOptions.Hook, model);

			this.ProxyGenerationOptions.Hook.MethodsInspected();

			// Constructor

			var cctor = this.GenerateStaticConstructor(emitter);
			var ctorArguments = new List<FieldReference>();

			foreach (var contributor in contributors)
			{
				contributor.Generate(emitter, this.ProxyGenerationOptions);

				// TODO: redo it
				if (contributor is MixinContributor)
					ctorArguments.AddRange((contributor as MixinContributor).Fields);
			}

			ctorArguments.Add(interceptorsField);
			ctorArguments.Add(this.targetField);
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

		protected virtual InterfaceProxyWithoutTargetContributor GetContributorForAdditionalInterfaces(
			INamingScope namingScope)
		{
			return new InterfaceProxyWithoutTargetContributor(namingScope, (c, m) => NullExpression.Instance) { Logger = this.Logger };
		}

		protected virtual IEnumerable<Type> GetTypeImplementerMapping(Type[] interfaces, Type proxyTargetType,
			out IEnumerable<ITypeContributor> contributors,
			INamingScope namingScope)
		{
			IDictionary<Type, ITypeContributor> typeImplementerMapping = new Dictionary<Type, ITypeContributor>();
			var mixins = new MixinContributor(namingScope, this.AllowChangeTarget) { Logger = this.Logger };
			// Order of interface precedence:
			// 1. first target
			var targetInterfaces = proxyTargetType.GetAllInterfaces();
			var additionalInterfaces = TypeUtil.GetAllInterfaces(interfaces);
			var target = this.AddMappingForTargetType(typeImplementerMapping, proxyTargetType, targetInterfaces, additionalInterfaces,
				namingScope);

			// 2. then mixins
			if (this.ProxyGenerationOptions.HasMixins)
			{
				foreach (var mixinInterface in this.ProxyGenerationOptions.MixinData.MixinInterfaces)
				{
					if (targetInterfaces.Contains(mixinInterface))
					{
						// OK, so the target implements this interface. We now do one of two things:
						if (additionalInterfaces.Contains(mixinInterface))
						{
							// we intercept the interface, and forward calls to the target type
							this.AddMapping(mixinInterface, target, typeImplementerMapping);
						}
						// we do not intercept the interface
						mixins.AddEmptyInterface(mixinInterface);
					}
					else
					{
						if (!typeImplementerMapping.ContainsKey(mixinInterface))
						{
							mixins.AddInterfaceToProxy(mixinInterface);
							typeImplementerMapping.Add(mixinInterface, mixins);
						}
					}
				}
			}

			var additionalInterfacesContributor = this.GetContributorForAdditionalInterfaces(namingScope);
			// 3. then additional interfaces
			foreach (var @interface in additionalInterfaces)
			{
				if (typeImplementerMapping.ContainsKey(@interface))
					continue;
				if (this.ProxyGenerationOptions.MixinData.ContainsMixin(@interface))
					continue;

				additionalInterfacesContributor.AddInterfaceToProxy(@interface);
				this.AddMappingNoCheck(@interface, additionalInterfacesContributor, typeImplementerMapping);
			}

			// 4. plus special interfaces
			var instance = new InterfaceProxyInstanceContributor(this.targetType, this.GeneratorType, interfaces);
			this.AddMappingForISerializable(typeImplementerMapping, instance);
			try
			{
				this.AddMappingNoCheck(typeof(IProxyTargetAccessor), instance, typeImplementerMapping);
			}
			catch (ArgumentException)
			{
				this.HandleExplicitlyPassedProxyTargetAccessor(targetInterfaces, additionalInterfaces);
			}

			contributors = new List<ITypeContributor>
							{
								target,
								additionalInterfacesContributor,
								mixins,
								instance
							};
			return typeImplementerMapping.Keys;
		}

		protected virtual Type Init(string typeName, out ClassEmitter emitter, Type proxyTargetType,
			out FieldReference interceptorsField, IEnumerable<Type> interfaces)
		{
			var baseType = this.ProxyGenerationOptions.BaseTypeForInterfaceProxy;

			emitter = this.BuildClassEmitter(typeName, baseType, interfaces);

			this.CreateFields(emitter, proxyTargetType);
			this.CreateTypeAttributes(emitter);

			interceptorsField = emitter.GetField("__interceptors");
			return baseType;
		}

		private void CreateFields(ClassEmitter emitter, Type proxyTargetType)
		{
			base.CreateFields(emitter);
			this.targetField = emitter.CreateField("__target", proxyTargetType);
#if !SILVERLIGHT
			emitter.DefineCustomAttributeFor<XmlIgnoreAttribute>(this.targetField);
#endif
		}

		private void EnsureValidBaseType(Type type)
		{
			if (type == null)
			{
				throw new ArgumentException(
					"Base type for proxy is null reference. Please set it to System.Object or some other valid type.");
			}

			if (!type.IsClass)
				this.ThrowInvalidBaseType(type, "it is not a class type");

			if (type.IsSealed)
				this.ThrowInvalidBaseType(type, "it is sealed");

			var constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
				null, Type.EmptyTypes, null);

			if (constructor == null || constructor.IsPrivate)
				this.ThrowInvalidBaseType(type, "it does not have accessible parameterless constructor");
		}

		private bool ImplementedByTarget(ICollection<Type> targetInterfaces, Type @interface)
		{
			return targetInterfaces.Contains(@interface);
		}

		private void ThrowInvalidBaseType(Type type, string doesNotHaveAccessibleParameterlessConstructor)
		{
			var format =
				"Type {0} is not valid base type for interface proxy, because {1}. Only a non-sealed class with non-private default constructor can be used as base type for interface proxy. Please use some other valid type.";
			throw new ArgumentException(string.Format(format, type, doesNotHaveAccessibleParameterlessConstructor));
		}
	}
}