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

namespace Castle.Components.DictionaryAdapter
{
	using System;

	public class MemberwiseEqualityHashCodeStrategy : DictionaryBehaviorAttribute,
		IDictionaryEqualityHashCodeStrategy, IDictionaryInitializer, IEqualityComparer<IDictionaryAdapter>
	{
		#region Methods/Operators

		public bool Equals(IDictionaryAdapter adapter1, IDictionaryAdapter adapter2)
		{
			if (ReferenceEquals(adapter1, adapter2))
				return true;

			if ((adapter1 == null) ^ (adapter2 == null))
				return false;

			if (adapter1.Meta.Type != adapter2.Meta.Type)
				return false;

			return this.GetHashCode(adapter1) == this.GetHashCode(adapter2);
		}

		public int GetHashCode(IDictionaryAdapter adapter)
		{
			int hashCode;
			this.GetHashCode(adapter, out hashCode);
			return hashCode;
		}

		public bool GetHashCode(IDictionaryAdapter adapter, out int hashCode)
		{
			hashCode = new HashCodeVisitor().CalculateHashCode(adapter);
			return true;
		}

		void IDictionaryInitializer.Initialize(IDictionaryAdapter dictionaryAdapter, object[] behaviors)
		{
			dictionaryAdapter.This.EqualityHashCodeStrategy = this;
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		private class HashCodeVisitor : AbstractDictionaryAdapterVisitor
		{
			#region Fields/Constants

			private int hashCode;

			#endregion

			#region Methods/Operators

			public int CalculateHashCode(IDictionaryAdapter dictionaryAdapter)
			{
				if (dictionaryAdapter == null)
					return 0;

				this.hashCode = 27;
				return this.VisitDictionaryAdapter(dictionaryAdapter, null) ? this.hashCode : 0;
			}

			private void CollectHashCode(PropertyDescriptor property, int valueHashCode)
			{
				unchecked
				{
					this.hashCode = (13 * this.hashCode) + property.PropertyName.GetHashCode();
					this.hashCode = (13 * this.hashCode) + valueHashCode;
				}
			}

			private int GetCollectionHashcode(IEnumerable collection)
			{
				if (collection == null)
					return 0;

				var collectionHashCode = 0;

				foreach (var value in collection)
				{
					var valueHashCode = this.GetValueHashCode(value);
					unchecked
					{
						collectionHashCode = (13 * collectionHashCode) + valueHashCode;
					}
				}

				return collectionHashCode;
			}

			private int GetNestedHashCode(IDictionaryAdapter nested)
			{
				var currentHashCode = this.hashCode;
				var nestedHashCode = this.CalculateHashCode(nested);
				this.hashCode = currentHashCode;
				return nestedHashCode;
			}

			private int GetValueHashCode(object value)
			{
				if (value == null)
					return 0;

				if (value is IDictionaryAdapter)
					return this.GetNestedHashCode((IDictionaryAdapter)value);

				if ((value is IEnumerable) && (value is string) == false)
					return this.GetCollectionHashcode((IEnumerable)value);

				return value.GetHashCode();
			}

			protected override void VisitCollection(IDictionaryAdapter dictionaryAdapter, PropertyDescriptor property, Type collectionItemType, object state)
			{
				var collection = (IEnumerable)dictionaryAdapter.GetProperty(property.PropertyName, true);
				this.CollectHashCode(property, this.GetCollectionHashcode(collection));
			}

			protected override void VisitInterface(IDictionaryAdapter dictionaryAdapter, PropertyDescriptor property, object state)
			{
				var nested = (IDictionaryAdapter)dictionaryAdapter.GetProperty(property.PropertyName, true);
				this.CollectHashCode(property, this.GetNestedHashCode(nested));
			}

			protected override void VisitProperty(IDictionaryAdapter dictionaryAdapter, PropertyDescriptor property, object state)
			{
				var value = dictionaryAdapter.GetProperty(property.PropertyName, true);
				this.CollectHashCode(property, this.GetValueHashCode(value));
			}

			#endregion
		}

		#endregion
	}
}