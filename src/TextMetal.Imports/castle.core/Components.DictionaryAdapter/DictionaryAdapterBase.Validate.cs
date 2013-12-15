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
using System.Linq;

namespace Castle.Components.DictionaryAdapter
{
	using System;

	public partial class DictionaryAdapterBase : IDictionaryValidate
	{
		#region Fields/Constants

		private ICollection<IDictionaryValidator> validators;

		#endregion

		#region Properties/Indexers/Events

		public string this[String columnName]
		{
			get
			{
				if (this.CanValidate && this.validators != null)
				{
					PropertyDescriptor property;
					if (this.This.Properties.TryGetValue(columnName, out property))
					{
						return string.Join(Environment.NewLine, this.validators.Select(
							v => v.Validate(this, property)).Where(e => !string.IsNullOrEmpty(e))
							.ToArray());
					}
				}
				return String.Empty;
			}
		}

		public bool CanValidate
		{
			get;
			set;
		}

		public string Error
		{
			get
			{
				if (this.CanValidate && this.validators != null)
				{
					return string.Join(Environment.NewLine, this.validators.Select(
						v => v.Validate(this)).Where(e => !string.IsNullOrEmpty(e)).ToArray());
				}
				return String.Empty;
			}
		}

		public bool IsValid
		{
			get
			{
				if (this.CanValidate && this.validators != null)
					return !this.validators.Any(v => !v.IsValid(this));
				return !this.CanValidate;
			}
		}

		public IEnumerable<IDictionaryValidator> Validators
		{
			get
			{
				return this.validators ?? Enumerable.Empty<IDictionaryValidator>();
			}
		}

		#endregion

		#region Methods/Operators

		public void AddValidator(IDictionaryValidator validator)
		{
			if (this.validators == null)
				this.validators = new HashSet<IDictionaryValidator>();
			this.validators.Add(validator);
		}

		protected internal void Invalidate()
		{
			if (this.CanValidate)
			{
				if (this.validators != null)
				{
					foreach (var validator in this.validators)
						validator.Invalidate(this);
				}

				this.NotifyPropertyChanged("IsValid");
			}
		}

		public DictionaryValidateGroup ValidateGroups(params object[] groups)
		{
			return new DictionaryValidateGroup(groups, this);
		}

		#endregion
	}
}