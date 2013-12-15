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

using Castle.Core;

namespace Castle.Components.DictionaryAdapter
{
	using System;

	/// <summary>
	/// Abstract implementation of <see cref="IDictionaryAdapterVisitor" />.
	/// </summary>
	public abstract class AbstractDictionaryAdapterVisitor : IDictionaryAdapterVisitor
	{
		#region Constructors/Destructors

		protected AbstractDictionaryAdapterVisitor()
		{
			this.scopes = new Dictionary<IDictionaryAdapter, int>(ReferenceEqualityComparer<IDictionaryAdapter>.Instance);
		}

		protected AbstractDictionaryAdapterVisitor(AbstractDictionaryAdapterVisitor parent)
		{
			this.scopes = parent.scopes;
		}

		#endregion

		#region Fields/Constants

		private readonly Dictionary<IDictionaryAdapter, int> scopes;

		#endregion

		#region Properties/Indexers/Events

		protected bool Cancelled
		{
			get;
			set;
		}

		#endregion

		#region Methods/Operators

		private static bool IsCollection(PropertyDescriptor property, out Type collectionItemType)
		{
			collectionItemType = null;
			var propertyType = property.PropertyType;
			if (propertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(propertyType))
			{
				if (propertyType.IsArray)
					collectionItemType = propertyType.GetElementType();
				else if (propertyType.IsGenericType)
				{
					var arguments = propertyType.GetGenericArguments();
					collectionItemType = arguments[0];
				}
				else
					collectionItemType = typeof(object);
				return true;
			}
			return false;
		}

		private void PopScope(IDictionaryAdapter dictionaryAdapter)
		{
			this.scopes.Remove(dictionaryAdapter);
		}

		private bool PushScope(IDictionaryAdapter dictionaryAdapter)
		{
			if (this.scopes.ContainsKey(dictionaryAdapter))
				return false;
			this.scopes.Add(dictionaryAdapter, 0);
			return true;
		}

		void IDictionaryAdapterVisitor.VisitCollection(IDictionaryAdapter dictionaryAdapter, PropertyDescriptor property, Type collectionItemType, object state)
		{
			this.VisitCollection(dictionaryAdapter, property, collectionItemType, state);
		}

		protected virtual void VisitCollection(IDictionaryAdapter dictionaryAdapter, PropertyDescriptor property, Type collectionItemType, object state)
		{
			this.VisitProperty(dictionaryAdapter, property, state);
		}

		public virtual bool VisitDictionaryAdapter(IDictionaryAdapter dictionaryAdapter, object state)
		{
			return this.VisitDictionaryAdapter(dictionaryAdapter, null, null);
		}

		public virtual bool VisitDictionaryAdapter(IDictionaryAdapter dictionaryAdapter, Func<PropertyDescriptor, bool> selector, object state)
		{
			if (this.PushScope(dictionaryAdapter) == false)
				return false;

			try
			{
				foreach (var property in dictionaryAdapter.This.Properties.Values)
				{
					if (this.Cancelled)
						break;

					if (selector != null && selector(property) == false)
						continue;

					Type collectionItemType;
					if (IsCollection(property, out collectionItemType))
						this.VisitCollection(dictionaryAdapter, property, collectionItemType, state);
					else if (property.PropertyType.IsInterface)
						this.VisitInterface(dictionaryAdapter, property, state);
					else
						this.VisitProperty(dictionaryAdapter, property, state);
				}
			}
			finally
			{
				this.PopScope(dictionaryAdapter);
			}

			return true;
		}

		void IDictionaryAdapterVisitor.VisitInterface(IDictionaryAdapter dictionaryAdapter, PropertyDescriptor property, object state)
		{
			this.VisitInterface(dictionaryAdapter, property, state);
		}

		protected virtual void VisitInterface(IDictionaryAdapter dictionaryAdapter, PropertyDescriptor property, object state)
		{
			this.VisitProperty(dictionaryAdapter, property, state);
		}

		void IDictionaryAdapterVisitor.VisitProperty(IDictionaryAdapter dictionaryAdapter, PropertyDescriptor property, object state)
		{
			this.VisitProperty(dictionaryAdapter, property, state);
		}

		protected virtual void VisitProperty(IDictionaryAdapter dictionaryAdapter, PropertyDescriptor property, object state)
		{
		}

		#endregion
	}
}