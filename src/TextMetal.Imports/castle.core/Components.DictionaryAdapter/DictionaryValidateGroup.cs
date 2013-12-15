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

	public class DictionaryValidateGroup : IDictionaryValidate, INotifyPropertyChanged, IDisposable
	{
		#region Constructors/Destructors

		public DictionaryValidateGroup(object[] groups, IDictionaryAdapter adapter)
		{
			this.groups = groups;
			this.adapter = adapter;

			this.propertyNames = (from property in this.adapter.This.Properties.Values
				from groupings in property.Annotations.OfType<GroupAttribute>()
				where this.groups.Intersect(groupings.Group).Any()
				select property.PropertyName).Distinct().ToArray();

			if (this.propertyNames.Length > 0 && adapter.CanNotify)
			{
				this.propertyChanged += (sender, args) =>
										{
											if (this.PropertyChanged != null)
												this.PropertyChanged(this, args);
										};
				this.adapter.PropertyChanged += this.propertyChanged;
			}
		}

		#endregion

		#region Fields/Constants

		private readonly IDictionaryAdapter adapter;
		private readonly object[] groups;
		private readonly PropertyChangedEventHandler propertyChanged;
		private readonly string[] propertyNames;

		#endregion

		#region Properties/Indexers/Events

		public event PropertyChangedEventHandler PropertyChanged;

		public string this[string columnName]
		{
			get
			{
				if (Array.IndexOf(this.propertyNames, columnName) >= 0)
					return this.adapter[columnName];
				return string.Empty;
			}
		}

		public bool CanValidate
		{
			get
			{
				return this.adapter.CanValidate;
			}
			set
			{
				this.adapter.CanValidate = value;
			}
		}

		public string Error
		{
			get
			{
				return string.Join(Environment.NewLine,
					this.propertyNames.Select(propertyName => this.adapter[propertyName])
						.Where(errors => !string.IsNullOrEmpty(errors)).ToArray());
			}
		}

		public bool IsValid
		{
			get
			{
				return string.IsNullOrEmpty(this.Error);
			}
		}

		public IEnumerable<IDictionaryValidator> Validators
		{
			get
			{
				return this.adapter.Validators;
			}
		}

		#endregion

		#region Methods/Operators

		public void AddValidator(IDictionaryValidator validator)
		{
			throw new NotSupportedException();
		}

		public void Dispose()
		{
			this.adapter.PropertyChanged -= this.propertyChanged;
		}

		public DictionaryValidateGroup ValidateGroups(params object[] groups)
		{
			groups = this.groups.Union(groups).ToArray();
			return new DictionaryValidateGroup(groups, this.adapter);
		}

		#endregion
	}
}