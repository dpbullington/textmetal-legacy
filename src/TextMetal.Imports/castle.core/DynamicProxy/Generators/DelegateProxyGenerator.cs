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
using Castle.DynamicProxy.Serialization;

namespace Castle.DynamicProxy.Generators
{
	using System;
#if !SILVERLIGHT
	using System.Xml.Serialization;

#endif

	public class DelegateProxyGenerator : BaseProxyGenerator
	{
		#region Constructors/Destructors

		public DelegateProxyGenerator(ModuleScope scope, Type delegateType)
			: base(scope, delegateType)
		{
			this.ProxyGenerationOptions = new ProxyGenerationOptions(new DelegateProxyGenerationHook());
			this.ProxyGenerationOptions.Initialize();
		}

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

		private Type GenerateType(string name, INamingScope namingScope)
		{
			IEnumerable<ITypeContributor> contributors;
			var implementedInterfaces = this.GetTypeImplementerMapping(out contributors, namingScope);

			var model = new MetaType();
			// Collect methods
			foreach (var contributor in contributors)
				contributor.CollectElementsToProxy(this.ProxyGenerationOptions.Hook, model);
			this.ProxyGenerationOptions.Hook.MethodsInspected();

			var emitter = this.BuildClassEmitter(name, typeof(object), implementedInterfaces);

			this.CreateFields(emitter);
			this.CreateTypeAttributes(emitter);

			// Constructor
			var cctor = this.GenerateStaticConstructor(emitter);

			var targetField = this.CreateTargetField(emitter);
			var constructorArguments = new List<FieldReference> { targetField };

			foreach (var contributor in contributors)
				contributor.Generate(emitter, this.ProxyGenerationOptions);

			// constructor arguments
			var interceptorsField = emitter.GetField("__interceptors");
			constructorArguments.Add(interceptorsField);
			var selector = emitter.GetField("__selector");
			if (selector != null)
				constructorArguments.Add(selector);

			this.GenerateConstructor(emitter, null, constructorArguments.ToArray());
			this.GenerateParameterlessConstructor(emitter, this.targetType, interceptorsField);

			// Complete type initializer code body
			this.CompleteInitCacheMethod(cctor.CodeBuilder);

			// Crosses fingers and build type

			var proxyType = emitter.BuildType();
			this.InitializeStaticFields(proxyType);
			return proxyType;
		}

		public Type GetProxyType()
		{
			var cacheKey = new CacheKey(this.targetType, null, null);
			return this.ObtainProxyType(cacheKey, this.GenerateType);
		}

		protected virtual IEnumerable<Type> GetTypeImplementerMapping(out IEnumerable<ITypeContributor> contributors,
			INamingScope namingScope)
		{
			var methodsToSkip = new List<MethodInfo>();
			var proxyInstance = new ClassProxyInstanceContributor(this.targetType, methodsToSkip, Type.EmptyTypes,
				ProxyTypeConstants.ClassWithTarget);
			var proxyTarget = new DelegateProxyTargetContributor(this.targetType, namingScope) { Logger = this.Logger };
			IDictionary<Type, ITypeContributor> typeImplementerMapping = new Dictionary<Type, ITypeContributor>();

			// Order of interface precedence:
			// 1. first target, target is not an interface so we do nothing
			// 2. then mixins - we support none so we do nothing
			// 3. then additional interfaces - we support none so we do nothing
#if !SILVERLIGHT
			// 4. plus special interfaces
			if (this.targetType.IsSerializable)
				this.AddMappingForISerializable(typeImplementerMapping, proxyInstance);
#endif
			this.AddMappingNoCheck(typeof(IProxyTargetAccessor), proxyInstance, typeImplementerMapping);

			contributors = new List<ITypeContributor>
							{
								proxyTarget,
								proxyInstance
							};
			return typeImplementerMapping.Keys;
		}

		#endregion
	}
}