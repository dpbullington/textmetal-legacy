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

using System.Linq;

namespace Castle.Components.DictionaryAdapter
{
	using System;

	public abstract partial class DictionaryAdapterBase
	{
		#region Methods/Operators

		public void CopyTo(IDictionaryAdapter other)
		{
			this.CopyTo(other, null);
		}

		public void CopyTo(IDictionaryAdapter other, Func<PropertyDescriptor, bool> selector)
		{
			if (ReferenceEquals(this, other))
				return;

			if (other.Meta.Type.IsAssignableFrom(this.Meta.Type) == false)
			{
				throw new ArgumentException(string.Format(
					"Unable to copy to {0}.  The type must be assignable from {1}.",
					other.Meta.Type.FullName, this.Meta.Type.FullName));
			}

			if (this.This.CopyStrategies.Aggregate(false, (copied, s) => copied | s.Copy(this, other, ref selector)))
				return;

			selector = selector ?? (property => true);

			foreach (var property in this.This.Properties.Values.Where(property => selector(property)))
			{
				var propertyValue = this.GetProperty(property.PropertyName, true);
				other.SetProperty(property.PropertyName, ref propertyValue);
			}
		}

		#endregion
	}
}