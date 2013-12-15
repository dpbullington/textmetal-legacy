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
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

using Castle.Core.Internal;

namespace Castle.Components.DictionaryAdapter
{
	using System;

	/// <summary>
	/// Describes a dictionary property.
	/// </summary>
	[DebuggerDisplay("{Property.DeclaringType.FullName,nq}.{PropertyName,nq}")]
	public class PropertyDescriptor : IDictionaryKeyBuilder, IDictionaryPropertyGetter, IDictionaryPropertySetter
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes an empty <see cref="PropertyDescriptor" /> class.
		/// </summary>
		public PropertyDescriptor()
		{
			this.Annotations = NoAnnotations;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyDescriptor" /> class.
		/// </summary>
		/// <param name="property"> The property. </param>
		/// <param name="annotations"> The annotations. </param>
		public PropertyDescriptor(PropertyInfo property, object[] annotations)
			: this()
		{
			this.Property = property;
			this.Annotations = annotations ?? NoAnnotations;
			this.IsDynamicProperty = typeof(IDynamicValue).IsAssignableFrom(property.PropertyType);
			this.ObtainTypeConverter();
		}

		/// <summary>
		/// Initializes a new instance <see cref="PropertyDescriptor" /> class.
		/// </summary>
		public PropertyDescriptor(object[] annotations)
		{
			this.Annotations = annotations ?? NoAnnotations;
		}

		/// <summary>
		/// Copies an existinginstance of the <see cref="PropertyDescriptor" /> class.
		/// </summary>
		/// <param name="source"> </param>
		/// <param name="copyBehaviors"> </param>
		public PropertyDescriptor(PropertyDescriptor source, bool copyBehaviors)
		{
			this.Property = source.Property;
			this.Annotations = source.Annotations;
			this.IsDynamicProperty = source.IsDynamicProperty;
			this.TypeConverter = source.TypeConverter;
			this.SuppressNotifications = source.SuppressNotifications;
			this.state = source.state;
			this.IfExists = source.IfExists;
			this.Fetch = source.Fetch;

			if (source.extendedProperties != null)
				this.extendedProperties = new Dictionary<object, object>(source.extendedProperties);

			if (copyBehaviors && source.dictionaryBehaviors != null)
				this.dictionaryBehaviors = new List<IDictionaryBehavior>(source.dictionaryBehaviors);
		}

		#endregion

		#region Fields/Constants

		private static readonly object[] NoAnnotations = new object[0];
		protected List<IDictionaryBehavior> dictionaryBehaviors;
		private Dictionary<object, object> extendedProperties;
		private IDictionary state;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// Gets the property behaviors.
		/// </summary>
		public object[] Annotations
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the setter.
		/// </summary>
		/// <value> The setter. </value>
		public IEnumerable<IDictionaryBehavior> Behaviors
		{
			get
			{
				return this.dictionaryBehaviors ?? Enumerable.Empty<IDictionaryBehavior>();
			}
		}

		internal List<IDictionaryBehavior> BehaviorsInternal
		{
			get
			{
				return this.dictionaryBehaviors;
			}
		}

		/// <summary>
		/// </summary>
		public int ExecutionOrder
		{
			get
			{
				return 0;
			}
		}

		/// <summary>
		/// Gets the extended properties.
		/// </summary>
		public IDictionary ExtendedProperties
		{
			get
			{
				return this.extendedProperties ?? (this.extendedProperties = new Dictionary<object, object>());
			}
		}

		/// <summary>
		/// Determines if property should be fetched.
		/// </summary>
		public bool Fetch
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the getter.
		/// </summary>
		/// <value> The getter. </value>
		public IEnumerable<IDictionaryPropertyGetter> Getters
		{
			get
			{
				return (this.dictionaryBehaviors != null)
					? this.dictionaryBehaviors.OfType<IDictionaryPropertyGetter>()
					: Enumerable.Empty<IDictionaryPropertyGetter>();
			}
		}

		/// <summary>
		/// Determines if property must exist first.
		/// </summary>
		public bool IfExists
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the initializers.
		/// </summary>
		/// <value> The initializers. </value>
		public IEnumerable<IDictionaryInitializer> Initializers
		{
			get
			{
				return (this.dictionaryBehaviors != null)
					? this.dictionaryBehaviors.OfType<IDictionaryInitializer>()
					: Enumerable.Empty<IDictionaryInitializer>();
			}
		}

		/// <summary>
		/// Returns true if the property is dynamic.
		/// </summary>
		public bool IsDynamicProperty
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the key builders.
		/// </summary>
		/// <value> The key builders. </value>
		public IEnumerable<IDictionaryKeyBuilder> KeyBuilders
		{
			get
			{
				return (this.dictionaryBehaviors != null)
					? this.dictionaryBehaviors.OfType<IDictionaryKeyBuilder>()
					: Enumerable.Empty<IDictionaryKeyBuilder>();
			}
		}

		/// <summary>
		/// Gets the meta-data initializers.
		/// </summary>
		/// <value> The meta-data initializers. </value>
		public IEnumerable<IDictionaryMetaInitializer> MetaInitializers
		{
			get
			{
				return (this.dictionaryBehaviors != null)
					? this.dictionaryBehaviors.OfType<IDictionaryMetaInitializer>()
					: Enumerable.Empty<IDictionaryMetaInitializer>();
			}
		}

		/// <summary>
		/// Gets the property.
		/// </summary>
		/// <value> The property. </value>
		public PropertyInfo Property
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the property name.
		/// </summary>
		public string PropertyName
		{
			get
			{
				return (this.Property != null) ? this.Property.Name : null;
			}
		}

		/// <summary>
		/// Gets the property type.
		/// </summary>
		public Type PropertyType
		{
			get
			{
				return (this.Property != null) ? this.Property.PropertyType : null;
			}
		}

		/// <summary>
		/// Gets the setter.
		/// </summary>
		/// <value> The setter. </value>
		public IEnumerable<IDictionaryPropertySetter> Setters
		{
			get
			{
				return (this.dictionaryBehaviors != null)
					? this.dictionaryBehaviors.OfType<IDictionaryPropertySetter>()
					: Enumerable.Empty<IDictionaryPropertySetter>();
			}
		}

		/// <summary>
		/// Gets additional state.
		/// </summary>
		public IDictionary State
		{
			get
			{
				return this.state ?? (this.state = new Dictionary<object, object>());
			}
		}

		/// <summary>
		/// Determines if notifications should occur.
		/// </summary>
		public bool SuppressNotifications
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the type converter.
		/// </summary>
		/// <value> The type converter. </value>
		public TypeConverter TypeConverter
		{
			get;
			private set;
		}

		#endregion

		#region Methods/Operators

		public static void MergeBehavior<T>(ref List<T> dictionaryBehaviors, T behavior)
			where T : class, IDictionaryBehavior
		{
			var behaviors = dictionaryBehaviors;
			if (behaviors == null)
			{
				behaviors = new List<T>();
				behaviors.Add(behavior);
				dictionaryBehaviors = behaviors;
				return;
			}

			// The following is ugly but supposedly optimized

			// Locals using ldloc.#
			var index = 0;
			int candidatePriority;
			var targetOrder = behavior.ExecutionOrder;
			// Locals using ldloc.s
			var count = behaviors.Count;
			IDictionaryBehavior candidateBehavior;

			// Skip while order < behavior.ExecutionOrder
			for (;;)
			{
				candidateBehavior = behaviors[index];
				candidatePriority = candidateBehavior.ExecutionOrder;

				if (candidatePriority >= targetOrder)
					break;

				if (++index == count)
				{
					behaviors.Add(behavior);
					return;
				}
			}

			// Skip while order == behavior.ExecutionOrder
			for (;;)
			{
				if (candidatePriority != targetOrder)
					break;

				if (candidateBehavior == behavior)
					return; // Duplicate

				if (++index == count)
				{
					behaviors.Add(behavior);
					return;
				}

				candidateBehavior = behaviors[index];
				candidatePriority = candidateBehavior.ExecutionOrder;
			}

			// Insert at found index
			behaviors.Insert(index, behavior);
			return;
		}

		/// <summary>
		/// Adds a single behavior.
		/// </summary>
		/// <param name="behavior"> The behavior. </param>
		public PropertyDescriptor AddBehavior(IDictionaryBehavior behavior)
		{
			if (behavior == null)
				return this;

			var builder = behavior as IDictionaryBehaviorBuilder;
			if (builder == null)
				MergeBehavior(ref this.dictionaryBehaviors, behavior);
			else
			{
				foreach (var item in builder.BuildBehaviors())
					this.AddBehavior(item as IDictionaryBehavior);
			}

			return this;
		}

		/// <summary>
		/// Adds the behaviors.
		/// </summary>
		/// <param name="behaviors"> The behaviors. </param>
		public PropertyDescriptor AddBehaviors(params IDictionaryBehavior[] behaviors)
		{
			// DO NOT REFACTOR. Compiler will emit optimized iterator here.
			foreach (var behavior in behaviors)
				this.AddBehavior(behavior);
			return this;
		}

		/// <summary>
		/// Adds the behaviors.
		/// </summary>
		/// <param name="behaviors"> The behaviors. </param>
		public PropertyDescriptor AddBehaviors(IEnumerable<IDictionaryBehavior> behaviors)
		{
			if (behaviors != null)
			{
				foreach (var behavior in behaviors)
					this.AddBehavior(behavior);
			}
			return this;
		}

		/// <summary>
		/// Copies the <see cref="PropertyDescriptor" />
		/// </summary>
		/// <returns> </returns>
		public IDictionaryBehavior Copy()
		{
			return new PropertyDescriptor(this, true);
		}

		/// <summary>
		/// Copies the behaviors to the other <see cref="PropertyDescriptor" />
		/// </summary>
		/// <param name="other"> </param>
		/// <returns> </returns>
		public PropertyDescriptor CopyBehaviors(PropertyDescriptor other)
		{
			var behaviors = this.dictionaryBehaviors;
			if (behaviors != null)
			{
				var count = behaviors.Count;
				for (var i = 0; i < count; i++)
				{
					var behavior = behaviors[i].Copy();
					if (behavior != null)
						other.AddBehavior(behavior);
				}
			}
			return this;
		}

		/// <summary>
		/// Gets the key.
		/// </summary>
		/// <param name="dictionaryAdapter"> The dictionary adapter. </param>
		/// <param name="key"> The key. </param>
		/// <param name="descriptor"> The descriptor. </param>
		/// <returns> </returns>
		public string GetKey(IDictionaryAdapter dictionaryAdapter, String key, PropertyDescriptor descriptor)
		{
			var behaviors = this.dictionaryBehaviors;
			if (behaviors != null)
			{
				var count = behaviors.Count;
				for (int i = 0; i < count; i++)
				{
					var builder = behaviors[i] as IDictionaryKeyBuilder;
					if (builder != null)
						key = builder.GetKey(dictionaryAdapter, key, this);
				}
			}
			return key;
		}

		/// <summary>
		/// Gets the property value.
		/// </summary>
		/// <param name="dictionaryAdapter"> The dictionary adapter. </param>
		/// <param name="key"> The key. </param>
		/// <param name="storedValue"> The stored value. </param>
		/// <param name="descriptor"> The descriptor. </param>
		/// <param name="ifExists"> true if return only existing. </param>
		/// <returns> </returns>
		public object GetPropertyValue(IDictionaryAdapter dictionaryAdapter, string key, object storedValue, PropertyDescriptor descriptor, bool ifExists)
		{
			key = this.GetKey(dictionaryAdapter, key, descriptor);
			storedValue = storedValue ?? dictionaryAdapter.ReadProperty(key);

			var behaviors = this.dictionaryBehaviors;
			if (behaviors != null)
			{
				var count = behaviors.Count;
				for (int i = 0; i < count; i++)
				{
					var getter = behaviors[i] as IDictionaryPropertyGetter;
					if (getter != null)
						storedValue = getter.GetPropertyValue(dictionaryAdapter, key, storedValue, this, this.IfExists || ifExists);
				}
			}

			return storedValue;
		}

		private void ObtainTypeConverter()
		{
			var converterType = AttributesUtil.GetTypeConverter(this.Property);

			this.TypeConverter = (converterType != null)
				? (TypeConverter)Activator.CreateInstance(converterType)
				: TypeDescriptor.GetConverter(this.PropertyType);
		}

		/// <summary>
		/// Sets the property value.
		/// </summary>
		/// <param name="dictionaryAdapter"> The dictionary adapter. </param>
		/// <param name="key"> The key. </param>
		/// <param name="value"> The value. </param>
		/// <param name="descriptor"> The descriptor. </param>
		/// <returns> </returns>
		public bool SetPropertyValue(IDictionaryAdapter dictionaryAdapter, string key, ref object value, PropertyDescriptor descriptor)
		{
			key = this.GetKey(dictionaryAdapter, key, descriptor);

			var behaviors = this.dictionaryBehaviors;
			if (behaviors != null)
			{
				var count = behaviors.Count;
				for (int i = 0; i < count; i++)
				{
					var setter = behaviors[i] as IDictionaryPropertySetter;
					if (setter != null)
					{
						if (setter.SetPropertyValue(dictionaryAdapter, key, ref value, this) == false)
							return false;
					}
				}
			}

			dictionaryAdapter.StoreProperty(this, key, value);
			return true;
		}

		#endregion
	}
}