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
using System.Diagnostics;

using Castle.Core.Logging;
using Castle.DynamicProxy.Generators;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Internal;

namespace Castle.DynamicProxy.Contributors
{
	using System;

	public abstract class CompositeTypeContributor : ITypeContributor
	{
		#region Constructors/Destructors

		protected CompositeTypeContributor(INamingScope namingScope)
		{
			this.namingScope = namingScope;
		}

		#endregion

		#region Fields/Constants

		private readonly ICollection<MetaEvent> events = new TypeElementCollection<MetaEvent>();
		protected readonly ICollection<Type> interfaces = new HashSet<Type>();
		private readonly ICollection<MetaMethod> methods = new TypeElementCollection<MetaMethod>();
		protected readonly INamingScope namingScope;
		private readonly ICollection<MetaProperty> properties = new TypeElementCollection<MetaProperty>();
		private ILogger logger = NullLogger.Instance;

		#endregion

		#region Properties/Indexers/Events

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

		public void AddInterfaceToProxy(Type @interface)
		{
			Debug.Assert(@interface != null, "@interface == null", "Shouldn't be adding empty interfaces...");
			Debug.Assert(@interface.IsInterface, "@interface.IsInterface", "Should be adding interfaces only...");
			Debug.Assert(!this.interfaces.Contains(@interface), "!interfaces.ContainsKey(@interface)",
				"Shouldn't be adding same interface twice...");

			this.interfaces.Add(@interface);
		}

		public void CollectElementsToProxy(IProxyGenerationHook hook, MetaType model)
		{
			foreach (var collector in this.CollectElementsToProxyInternal(hook))
			{
				foreach (var method in collector.Methods)
				{
					model.AddMethod(method);
					this.methods.Add(method);
				}
				foreach (var @event in collector.Events)
				{
					model.AddEvent(@event);
					this.events.Add(@event);
				}
				foreach (var property in collector.Properties)
				{
					model.AddProperty(property);
					this.properties.Add(property);
				}
			}
		}

		protected abstract IEnumerable<MembersCollector> CollectElementsToProxyInternal(IProxyGenerationHook hook);

		public virtual void Generate(ClassEmitter @class, ProxyGenerationOptions options)
		{
			foreach (var method in this.methods)
			{
				if (!method.Standalone)
					continue;

				this.ImplementMethod(method,
					@class,
					options,
					@class.CreateMethod);
			}

			foreach (var property in this.properties)
				this.ImplementProperty(@class, property, options);

			foreach (var @event in this.events)
				this.ImplementEvent(@class, @event, options);
		}

		protected abstract MethodGenerator GetMethodGenerator(MetaMethod method, ClassEmitter @class,
			ProxyGenerationOptions options,
			OverrideMethodDelegate overrideMethod);

		private void ImplementEvent(ClassEmitter emitter, MetaEvent @event, ProxyGenerationOptions options)
		{
			@event.BuildEventEmitter(emitter);
			this.ImplementMethod(@event.Adder, emitter, options, @event.Emitter.CreateAddMethod);
			this.ImplementMethod(@event.Remover, emitter, options, @event.Emitter.CreateRemoveMethod);
		}

		private void ImplementMethod(MetaMethod method, ClassEmitter @class, ProxyGenerationOptions options,
			OverrideMethodDelegate overrideMethod)
		{
			{
				var generator = this.GetMethodGenerator(method, @class, options, overrideMethod);
				if (generator == null)
					return;
				var proxyMethod = generator.Generate(@class, options, this.namingScope);
				foreach (var attribute in method.Method.GetNonInheritableAttributes())
					proxyMethod.DefineCustomAttribute(attribute);
			}
		}

		private void ImplementProperty(ClassEmitter emitter, MetaProperty property, ProxyGenerationOptions options)
		{
			property.BuildPropertyEmitter(emitter);
			if (property.CanRead)
				this.ImplementMethod(property.Getter, emitter, options, property.Emitter.CreateGetMethod);

			if (property.CanWrite)
				this.ImplementMethod(property.Setter, emitter, options, property.Emitter.CreateSetMethod);
		}

		#endregion
	}
}