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
using System.Text;

namespace Castle.Components.DictionaryAdapter
{
	using System;

	/// <summary>
	/// Identifies a property should be represented as a delimited string value.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class StringListAttribute : DictionaryBehaviorAttribute, IDictionaryPropertyGetter, IDictionaryPropertySetter
	{
		#region Constructors/Destructors

		public StringListAttribute()
		{
			this.Separator = ',';
		}

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// Gets the separator.
		/// </summary>
		public char Separator
		{
			get;
			set;
		}

		#endregion

		#region Methods/Operators

		internal static string BuildString(IEnumerable enumerable, char separator)
		{
			bool first = true;
			var builder = new StringBuilder();

			foreach (object item in enumerable)
			{
				if (first)
					first = false;
				else
					builder.Append(separator);

				builder.Append(item.ToString());
			}

			return builder.ToString();
		}

		object IDictionaryPropertyGetter.GetPropertyValue(IDictionaryAdapter dictionaryAdapter,
			string key, object storedValue, PropertyDescriptor property, bool ifExists)
		{
			var propertyType = property.PropertyType;

			if (storedValue == null || !storedValue.GetType().IsInstanceOfType(propertyType))
			{
				if (propertyType.IsGenericType)
				{
					var genericDef = propertyType.GetGenericTypeDefinition();

					if (genericDef == typeof(IList<>) || genericDef == typeof(ICollection<>) ||
						genericDef == typeof(List<>) || genericDef == typeof(IEnumerable<>))
					{
						var paramType = propertyType.GetGenericArguments()[0];
						var converter = TypeDescriptor.GetConverter(paramType);

						if (converter != null && converter.CanConvertFrom(typeof(string)))
						{
							var genericList = typeof(StringListWrapper<>).MakeGenericType(new[] { paramType });
							return Activator.CreateInstance(genericList, key, storedValue, this.Separator, dictionaryAdapter.This.Dictionary);
						}
					}
				}
			}

			return storedValue;
		}

		bool IDictionaryPropertySetter.SetPropertyValue(IDictionaryAdapter dictionaryAdapter,
			string key, ref object value, PropertyDescriptor property)
		{
			var enumerable = value as IEnumerable;
			if (enumerable != null)
				value = BuildString(enumerable, this.Separator);
			return true;
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		private class StringListWrapper<T> : IList<T>
		{
			#region Constructors/Destructors

			public StringListWrapper(string key, string list, char separator, IDictionary dictionary)
			{
				this.key = key;
				this.separator = separator;
				this.dictionary = dictionary;
				this.inner = new List<T>();

				this.ParseList(list);
			}

			#endregion

			#region Fields/Constants

			private readonly IDictionary dictionary;
			private readonly List<T> inner;
			private readonly string key;
			private readonly char separator;

			#endregion

			#region Properties/Indexers/Events

			public T this[int index]
			{
				get
				{
					return this.inner[index];
				}
				set
				{
					this.inner[index] = value;
					this.SynchronizeDictionary();
				}
			}

			public int Count
			{
				get
				{
					return this.inner.Count;
				}
			}

			public bool IsReadOnly
			{
				get
				{
					return false;
				}
			}

			#endregion

			#region Methods/Operators

			public void Add(T item)
			{
				this.inner.Add(item);
				this.SynchronizeDictionary();
			}

			public void Clear()
			{
				this.inner.Clear();
				this.SynchronizeDictionary();
			}

			public bool Contains(T item)
			{
				return this.inner.Contains(item);
			}

			public void CopyTo(T[] array, int arrayIndex)
			{
				this.inner.CopyTo(array, arrayIndex);
			}

			public IEnumerator<T> GetEnumerator()
			{
				return this.inner.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.inner.GetEnumerator();
			}

			public int IndexOf(T item)
			{
				return this.inner.IndexOf(item);
			}

			public void Insert(int index, T item)
			{
				this.inner.Insert(index, item);
				this.SynchronizeDictionary();
			}

			private void ParseList(string list)
			{
				if (list != null)
				{
					var converter = TypeDescriptor.GetConverter(typeof(T));

					foreach (var item in list.Split(this.separator))
						this.inner.Add((T)converter.ConvertFrom(item));
				}
			}

			public bool Remove(T item)
			{
				if (this.inner.Remove(item))
				{
					this.SynchronizeDictionary();
					return true;
				}
				return false;
			}

			public void RemoveAt(int index)
			{
				this.inner.RemoveAt(index);
				this.SynchronizeDictionary();
			}

			private void SynchronizeDictionary()
			{
				this.dictionary[this.key] = BuildString(this.inner, this.separator);
			}

			#endregion
		}

		#endregion
	}
}