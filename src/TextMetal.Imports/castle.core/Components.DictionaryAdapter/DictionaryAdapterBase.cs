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
using System.ComponentModel;
using System.Linq;

namespace Castle.Components.DictionaryAdapter
{
	public abstract partial class DictionaryAdapterBase : IDictionaryAdapter
	{
		#region Constructors/Destructors

		public DictionaryAdapterBase(DictionaryAdapterInstance instance)
		{
			this.This = instance;

			this.CanEdit = typeof(IEditableObject).IsAssignableFrom(this.Meta.Type);
			this.CanNotify = typeof(INotifyPropertyChanged).IsAssignableFrom(this.Meta.Type);
			this.CanValidate = typeof(IDataErrorInfo).IsAssignableFrom(this.Meta.Type);

			this.Initialize();
		}

		#endregion

		#region Properties/Indexers/Events

		public abstract DictionaryAdapterMeta Meta
		{
			get;
		}

		public DictionaryAdapterInstance This
		{
			get;
			private set;
		}

		#endregion

		#region Methods/Operators

		private static IDictionary GetDictionary(IDictionary dictionary, ref string key)
		{
			if (key.StartsWith("!") == false)
			{
				var parts = key.Split(',');
				for (var i = 0; i < parts.Length - 1; ++i)
				{
					dictionary = dictionary[parts[i]] as IDictionary;
					if (dictionary == null)
						return null;
				}
				key = parts[parts.Length - 1];
			}
			return dictionary;
		}

		public void ClearProperty(PropertyDescriptor property, string key)
		{
			key = key ?? this.GetKey(property.PropertyName);
			if (property == null || this.ClearEditProperty(property, key) == false)
			{
				var dictionary = GetDictionary(this.This.Dictionary, ref key);
				if (dictionary != null)
					dictionary.Remove(key);
			}
		}

		public override bool Equals(object obj)
		{
			var other = obj as IDictionaryAdapter;

			if (other == null)
				return false;

			if (ReferenceEquals(this, obj))
				return true;

			if (this.Meta.Type != other.Meta.Type)
				return false;

			if (this.This.EqualityHashCodeStrategy != null)
				return this.This.EqualityHashCodeStrategy.Equals(this, other);

			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			if (this.This.OldHashCode.HasValue)
				return this.This.OldHashCode.Value;

			int hashCode;
			if (this.This.EqualityHashCodeStrategy == null ||
				this.This.EqualityHashCodeStrategy.GetHashCode(this, out hashCode) == false)
				hashCode = base.GetHashCode();

			this.This.OldHashCode = hashCode;
			return hashCode;
		}

		public string GetKey(string propertyName)
		{
			PropertyDescriptor descriptor;
			if (this.This.Properties.TryGetValue(propertyName, out descriptor))
				return descriptor.GetKey(this, propertyName, this.This.Descriptor);
			return null;
		}

		public virtual object GetProperty(string propertyName, bool ifExists)
		{
			PropertyDescriptor descriptor;
			if (this.This.Properties.TryGetValue(propertyName, out descriptor))
			{
				var propertyValue = descriptor.GetPropertyValue(this, propertyName, null, this.This.Descriptor, ifExists);
				if (propertyValue is IEditableObject)
					this.AddEditDependency((IEditableObject)propertyValue);
				return propertyValue;
			}
			return null;
		}

		public T GetPropertyOfType<T>(string propertyName)
		{
			var propertyValue = this.GetProperty(propertyName, false);
			return propertyValue != null ? (T)propertyValue : default(T);
		}

		protected void Initialize()
		{
			var metaBehaviors = this.Meta.Behaviors;
			var initializers = this.This.Initializers;

			foreach (var initializer in initializers)
				initializer.Initialize(this, metaBehaviors);

			foreach (var property in this.This.Properties.Values)
			{
				if (property.Fetch)
					this.GetProperty(property.PropertyName, false);
			}
		}

		public object ReadProperty(string key)
		{
			object propertyValue = null;
			if (this.GetEditedProperty(key, out propertyValue) == false)
			{
				var dictionary = GetDictionary(this.This.Dictionary, ref key);
				if (dictionary != null)
					propertyValue = dictionary[key];
			}
			return propertyValue;
		}

		public virtual bool SetProperty(string propertyName, ref object value)
		{
			bool stored = false;

			PropertyDescriptor descriptor;
			if (this.This.Properties.TryGetValue(propertyName, out descriptor))
			{
				if (this.ShouldNotify == false)
				{
					stored = descriptor.SetPropertyValue(this, propertyName, ref value, this.This.Descriptor);
					this.Invalidate();
					return stored;
				}

				var existingValue = this.GetProperty(propertyName, true);
				if (this.NotifyPropertyChanging(descriptor, existingValue, value) == false)
					return false;

				var trackPropertyChange = this.TrackPropertyChange(descriptor, existingValue, value);

				stored = descriptor.SetPropertyValue(this, propertyName, ref value, this.This.Descriptor);

				if (stored && trackPropertyChange != null)
					trackPropertyChange.Notify();
			}

			return stored;
		}

		public bool ShouldClearProperty(PropertyDescriptor property, object value)
		{
			return property == null ||
					property.Setters.OfType<RemoveIfAttribute>().Where(remove => remove.ShouldRemove(value)).Any();
		}

		public void StoreProperty(PropertyDescriptor property, string key, object value)
		{
			if (property == null || this.EditProperty(property, key, value) == false)
			{
				var dictionary = GetDictionary(this.This.Dictionary, ref key);
				if (dictionary != null)
					dictionary[key] = value;
			}
		}

		#endregion
	}
}