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

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Castle.Components.DictionaryAdapter
{
	using System;

	public abstract partial class DictionaryAdapterBase
	{
		#region Fields/Constants

		private HashSet<IEditableObject> editDependencies;
		private int suppressEditingCount = 0;
		private Stack<Dictionary<string, Edit>> updates;

		#endregion

		#region Properties/Indexers/Events

		public bool CanEdit
		{
			get
			{
				return this.suppressEditingCount == 0 && this.updates != null;
			}
			set
			{
				this.updates = value ? new Stack<Dictionary<string, Edit>>() : null;
			}
		}

		public bool IsChanged
		{
			get
			{
				if (this.IsEditing && this.updates.Any(level => level.Count > 0))
					return true;

				return this.This.Properties.Values
					.Where(prop => typeof(IChangeTracking).IsAssignableFrom(prop.PropertyType))
					.Select(prop => this.GetProperty(prop.PropertyName, true))
					.Cast<IChangeTracking>().Any(track => track != null && track.IsChanged);
			}
		}

		public bool IsEditing
		{
			get
			{
				return this.CanEdit && this.updates != null && this.updates.Count > 0;
			}
		}

		public bool SupportsMultiLevelEdit
		{
			get;
			set;
		}

		#endregion

		#region Methods/Operators

		public void AcceptChanges()
		{
			this.EndEdit();
		}

		protected void AddEditDependency(IEditableObject editDependency)
		{
			if (this.IsEditing)
			{
				if (this.editDependencies == null)
					this.editDependencies = new HashSet<IEditableObject>();

				if (this.editDependencies.Add(editDependency))
					editDependency.BeginEdit();
			}
		}

		public void BeginEdit()
		{
			if (this.CanEdit && (this.IsEditing == false || this.SupportsMultiLevelEdit))
				this.updates.Push(new Dictionary<string, Edit>());
		}

		public void CancelEdit()
		{
			if (this.IsEditing)
			{
				if (this.editDependencies != null)
				{
					foreach (var editDependency in this.editDependencies.ToArray())
						editDependency.CancelEdit();
					this.editDependencies.Clear();
				}

				using (this.SuppressEditingBlock())
				{
					using (this.TrackReadonlyPropertyChanges())
					{
						var top = this.updates.Peek();

						if (top.Count > 0)
						{
							foreach (var update in top.Values)
							{
								var existing = update;
								existing.PropertyValue = this.GetProperty(existing.Property.PropertyName, true);
							}

							this.updates.Pop();

							foreach (var update in top.Values.ToArray())
							{
								var oldValue = update.PropertyValue;
								var newValue = this.GetProperty(update.Property.PropertyName, true);

								if (!Equals(oldValue, newValue))
								{
									this.NotifyPropertyChanging(update.Property, oldValue, newValue);
									this.NotifyPropertyChanged(update.Property, oldValue, newValue);
								}
							}
						}
					}
				}
			}
		}

		protected bool ClearEditProperty(PropertyDescriptor property, string key)
		{
			if (this.IsEditing)
			{
				this.updates.Peek().Remove(key);
				return true;
			}
			return false;
		}

		protected bool EditProperty(PropertyDescriptor property, string key, object propertyValue)
		{
			if (this.IsEditing)
			{
				this.updates.Peek()[key] = new Edit(property, propertyValue);
				return true;
			}
			return false;
		}

		public void EndEdit()
		{
			if (this.IsEditing)
			{
				using (this.SuppressEditingBlock())
				{
					var top = this.updates.Pop();

					if (top.Count > 0)
					{
						foreach (var update in top.ToArray())
							this.StoreProperty(null, update.Key, update.Value.PropertyValue);
					}
				}

				if (this.editDependencies != null)
				{
					foreach (var editDependency in this.editDependencies.ToArray())
						editDependency.EndEdit();
					this.editDependencies.Clear();
				}
			}
		}

		protected bool GetEditedProperty(string propertyName, out object propertyValue)
		{
			if (this.updates != null)
			{
				foreach (var level in this.updates.ToArray())
				{
					Edit edit;
					if (level.TryGetValue(propertyName, out edit))
					{
						propertyValue = edit.PropertyValue;
						return true;
					}
				}
			}
			propertyValue = null;
			return false;
		}

		public void RejectChanges()
		{
			this.CancelEdit();
		}

		public void ResumeEditing()
		{
			--this.suppressEditingCount;
		}

		public void SuppressEditing()
		{
			++this.suppressEditingCount;
		}

		public IDisposable SuppressEditingBlock()
		{
			return new SuppressEditingScope(this);
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		private struct Edit
		{
			#region Constructors/Destructors

			public Edit(PropertyDescriptor property, object propertyValue)
			{
				this.Property = property;
				this.PropertyValue = propertyValue;
			}

			#endregion

			#region Fields/Constants

			public readonly PropertyDescriptor Property;
			public object PropertyValue;

			#endregion
		}

		private class SuppressEditingScope : IDisposable
		{
			#region Constructors/Destructors

			public SuppressEditingScope(DictionaryAdapterBase adapter)
			{
				this.adapter = adapter;
				this.adapter.SuppressEditing();
			}

			#endregion

			#region Fields/Constants

			private readonly DictionaryAdapterBase adapter;

			#endregion

			#region Methods/Operators

			public void Dispose()
			{
				this.adapter.ResumeEditing();
			}

			#endregion
		}

		#endregion
	}
}