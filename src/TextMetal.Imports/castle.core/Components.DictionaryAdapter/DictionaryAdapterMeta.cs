// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Castle.Components.DictionaryAdapter
{
	using System;

	[DebuggerDisplay("Type: {Type.FullName,nq}")]
	public class DictionaryAdapterMeta
	{
		#region Constructors/Destructors

		public DictionaryAdapterMeta(Type type, Type implementation, object[] behaviors, IDictionaryMetaInitializer[] metaInitializers,
			IDictionaryInitializer[] initializers, IDictionary<String, PropertyDescriptor> properties,
			IDictionaryAdapterFactory factory, Func<DictionaryAdapterInstance, IDictionaryAdapter> creator)
		{
			this.Type = type;
			this.Implementation = implementation;
			this.Behaviors = behaviors;
			this.MetaInitializers = metaInitializers;
			this.Initializers = initializers;
			this.Properties = properties;
			this.Factory = factory;
			this.creator = creator;

			this.InitializeMeta();
		}

		#endregion

		#region Fields/Constants

		private readonly Func<DictionaryAdapterInstance, IDictionaryAdapter> creator;
		private IDictionary extendedProperties;

		#endregion

		#region Properties/Indexers/Events

		public object[] Behaviors
		{
			get;
			private set;
		}

		public IDictionary ExtendedProperties
		{
			get
			{
				if (this.extendedProperties == null)
					this.extendedProperties = new Dictionary<object, object>();
				return this.extendedProperties;
			}
		}

		public IDictionaryAdapterFactory Factory
		{
			get;
			private set;
		}

		public Type Implementation
		{
			get;
			private set;
		}

		public IDictionaryInitializer[] Initializers
		{
			get;
			private set;
		}

		public IDictionaryMetaInitializer[] MetaInitializers
		{
			get;
			private set;
		}

		public IDictionary<string, PropertyDescriptor> Properties
		{
			get;
			private set;
		}

		public Type Type
		{
			get;
			private set;
		}

		#endregion

		#region Methods/Operators

		private static List<T> CollectSharedBehaviors<T>(T[] source, IDictionaryMetaInitializer[] predicates)
		{
			var results = null as List<T>;

			foreach (var candidate in source)
			{
				foreach (var predicate in predicates)
				{
					if (predicate.ShouldHaveBehavior(candidate))
					{
						if (results == null)
							results = new List<T>(source.Length);

						results.Add(candidate);
						break; // next candidate
					}
				}
			}

			return results;
		}

		public PropertyDescriptor CreateDescriptor()
		{
			var metaInitializers = this.MetaInitializers;
			var sharedAnnotations = CollectSharedBehaviors(this.Behaviors, metaInitializers);
			var sharedInitializers = CollectSharedBehaviors(this.Initializers, metaInitializers);

			var descriptor = (sharedAnnotations != null)
				? new PropertyDescriptor(sharedAnnotations.ToArray())
				: new PropertyDescriptor();

			descriptor.AddBehaviors(metaInitializers);

			if (sharedInitializers != null)
#if DOTNET40
				descriptor.AddBehaviors(sharedInitializers);
#else
				descriptor.AddBehaviors(sharedInitializers.Cast<IDictionaryBehavior>());
#endif

			return descriptor;
		}

		public object CreateInstance(IDictionary dictionary, PropertyDescriptor descriptor)
		{
			var instance = new DictionaryAdapterInstance(dictionary, this, descriptor, this.Factory);
			return this.creator(instance);
		}

		public DictionaryAdapterMeta GetAdapterMeta(Type type)
		{
			return this.Factory.GetAdapterMeta(type, this);
		}

		private void InitializeMeta()
		{
			foreach (var metaInitializer in this.MetaInitializers)
				metaInitializer.Initialize(this.Factory, this);
		}

		#endregion
	}
}