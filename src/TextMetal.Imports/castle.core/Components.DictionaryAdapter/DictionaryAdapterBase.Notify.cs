// Copyright 2004-2012 Castle Project - http://www.castleproject.org/
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
using System.ComponentModel;
using System.Linq;

namespace Castle.Components.DictionaryAdapter
{
	using System;

	public abstract partial class DictionaryAdapterBase
	{
		#region Fields/Constants

		[ThreadStatic]
		private static TrackPropertyChangeScope readOnlyTrackingScope;

		private int suppressNotificationCount = 0;

		#endregion

		#region Properties/Indexers/Events

		public event PropertyChangedEventHandler PropertyChanged;
		public event PropertyChangingEventHandler PropertyChanging;

		public bool CanNotify
		{
			get;
			set;
		}

		public bool ShouldNotify
		{
			get
			{
				return this.CanNotify && this.suppressNotificationCount == 0;
			}
		}

		#endregion

		#region Methods/Operators

		protected void NotifyPropertyChanged(PropertyDescriptor property, object oldValue, object newValue)
		{
			if (property.SuppressNotifications)
				return;

			var propertyChanged = this.PropertyChanged;
			if (propertyChanged == null)
				return;

			propertyChanged(this, new PropertyChangedEventArgsEx(property.PropertyName, oldValue, newValue));
		}

		protected void NotifyPropertyChanged(string propertyName)
		{
			if (!this.ShouldNotify)
				return;

			var propertyChanged = this.PropertyChanged;
			if (propertyChanged == null)
				return;

			propertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		protected bool NotifyPropertyChanging(PropertyDescriptor property, object oldValue, object newValue)
		{
			if (property.SuppressNotifications)
				return true;

			var propertyChanging = this.PropertyChanging;
			if (propertyChanging == null)
				return true;

			var args = new PropertyChangingEventArgsEx(property.PropertyName, oldValue, newValue);
			propertyChanging(this, args);
			return !args.Cancel;
		}

		public void ResumeNotifications()
		{
			--this.suppressNotificationCount;
		}

		public void SuppressNotifications()
		{
			++this.suppressNotificationCount;
		}

		public IDisposable SuppressNotificationsBlock()
		{
			return new NotificationSuppressionScope(this);
		}

		protected TrackPropertyChangeScope TrackPropertyChange(PropertyDescriptor property,
			object oldValue, object newValue)
		{
			if (!this.ShouldNotify || property.SuppressNotifications)
				return null;

			return new TrackPropertyChangeScope(this, property, oldValue);
		}

		protected TrackPropertyChangeScope TrackReadonlyPropertyChanges()
		{
			if (!this.ShouldNotify || readOnlyTrackingScope != null)
				return null;

			return readOnlyTrackingScope = new TrackPropertyChangeScope(this);
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		private class NotificationSuppressionScope : IDisposable
		{
			#region Constructors/Destructors

			public NotificationSuppressionScope(DictionaryAdapterBase adapter)
			{
				this.adapter = adapter;
				this.adapter.SuppressNotifications();
			}

			#endregion

			#region Fields/Constants

			private readonly DictionaryAdapterBase adapter;

			#endregion

			#region Methods/Operators

			public void Dispose()
			{
				this.adapter.ResumeNotifications();
			}

			#endregion
		}

		public class TrackPropertyChangeScope : IDisposable
		{
			#region Constructors/Destructors

			public TrackPropertyChangeScope(DictionaryAdapterBase adapter)
			{
				this.adapter = adapter;
				this.readOnlyProperties = adapter.This.Properties.Values
					.Where(
						pd => !pd.Property.CanWrite || pd.IsDynamicProperty
					)
					.ToDictionary(
						pd => pd,
						pd => this.GetEffectivePropertyValue(pd)
					);
			}

			public TrackPropertyChangeScope(DictionaryAdapterBase adapter, PropertyDescriptor property, object existingValue)
				: this(adapter)
			{
				this.property = property;
				this.existingValue = existingValue;
				existingValue = adapter.GetProperty(property.PropertyName, true); // TODO: This looks unnecessary
			}

			#endregion

			#region Fields/Constants

			private readonly DictionaryAdapterBase adapter;
			private readonly object existingValue;
			private readonly PropertyDescriptor property;
			private Dictionary<PropertyDescriptor, object> readOnlyProperties;

			#endregion

			#region Methods/Operators

			public void Dispose()
			{
				this.Notify();
			}

			private object GetEffectivePropertyValue(PropertyDescriptor property)
			{
				var value = this.adapter.GetProperty(property.PropertyName, true);
				if (value == null || !property.IsDynamicProperty)
					return value;

				var dynamicValue = value as IDynamicValue;
				if (dynamicValue == null)
					return value;

				return dynamicValue.GetValue();
			}

			public bool Notify()
			{
				if (readOnlyTrackingScope == this)
				{
					readOnlyTrackingScope = null;
					return this.NotifyReadonly();
				}

				var newValue = this.GetEffectivePropertyValue(this.property);

				if (!this.NotifyIfChanged(this.property, this.existingValue, newValue))
					return false;

				if (readOnlyTrackingScope == null)
					this.NotifyReadonly();

				return true;
			}

			private bool NotifyIfChanged(PropertyDescriptor descriptor, object oldValue, object newValue)
			{
				if (Equals(oldValue, newValue))
					return false;

				this.adapter.NotifyPropertyChanged(descriptor, oldValue, newValue);
				return true;
			}

			private bool NotifyReadonly()
			{
				var changed = false;

				foreach (var readOnlyProperty in this.readOnlyProperties)
				{
					var descriptor = readOnlyProperty.Key;
					var currentValue = this.GetEffectivePropertyValue(descriptor);
					changed |= this.NotifyIfChanged(descriptor, readOnlyProperty.Value, currentValue);
				}

				this.adapter.Invalidate();
				return changed;
			}

			#endregion
		}

		#endregion
	}
}