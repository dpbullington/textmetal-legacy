// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
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
using System.Linq;
using System.Reflection;

namespace Castle.Core
{
	using System;

	/// <summary>
	/// Readonly implementation of <see cref="IDictionary" /> which uses an anonymous object as its source. Uses names of properties as keys, and property values as... well - values. Keys are not case sensitive.
	/// </summary>
	public sealed class ReflectionBasedDictionaryAdapter : IDictionary
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ReflectionBasedDictionaryAdapter" /> class.
		/// </summary>
		/// <param name="target"> The target. </param>
		public ReflectionBasedDictionaryAdapter(object target)
		{
			if (target == null)
				throw new ArgumentNullException("target");
			Read(this.properties, target);
		}

		#endregion

		#region Fields/Constants

		private readonly Dictionary<string, object> properties =
			new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// Gets or sets the <see cref="Object" /> with the specified key.
		/// </summary>
		/// <value> </value>
		public object this[object key]
		{
			get
			{
				object value;
				this.properties.TryGetValue(key.ToString(), out value);
				return value;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Gets the number of elements contained in the <see cref="T:System.Collections.ICollection" />.
		/// </summary>
		/// <value> </value>
		/// <returns> The number of elements contained in the <see cref="T:System.Collections.ICollection" />. </returns>
		public int Count
		{
			get
			{
				return this.properties.Count;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the <see cref="T:System.Collections.IDictionary" /> object has a fixed size.
		/// </summary>
		/// <value> </value>
		/// <returns> true if the <see cref="T:System.Collections.IDictionary" /> object has a fixed size; otherwise, false. </returns>
		bool IDictionary.IsFixedSize
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Gets a value indicating whether the <see cref="T:System.Collections.IDictionary" /> object is read-only.
		/// </summary>
		/// <value> </value>
		/// <returns> true if the <see cref="T:System.Collections.IDictionary" /> object is read-only; otherwise, false. </returns>
		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe).
		/// </summary>
		/// <value> </value>
		/// <returns> true if access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe); otherwise, false. </returns>
		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Gets an <see cref="T:System.Collections.ICollection" /> object containing the keys of the
		/// <see
		///     cref="T:System.Collections.IDictionary" />
		/// object.
		/// </summary>
		/// <value> </value>
		/// <returns>
		/// An <see cref="T:System.Collections.ICollection" /> object containing the keys of the
		/// <see
		///     cref="T:System.Collections.IDictionary" />
		/// object.
		/// </returns>
		public ICollection Keys
		{
			get
			{
				return this.properties.Keys;
			}
		}

		/// <summary>
		/// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.
		/// </summary>
		/// <value> </value>
		/// <returns> An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />. </returns>
		public object SyncRoot
		{
			get
			{
				return this.properties;
			}
		}

		/// <summary>
		/// Gets an <see cref="T:System.Collections.ICollection" /> object containing the values in the
		/// <see
		///     cref="T:System.Collections.IDictionary" />
		/// object.
		/// </summary>
		/// <value> </value>
		/// <returns>
		/// An <see cref="T:System.Collections.ICollection" /> object containing the values in the
		/// <see
		///     cref="T:System.Collections.IDictionary" />
		/// object.
		/// </returns>
		public ICollection Values
		{
			get
			{
				return this.properties.Values;
			}
		}

		#endregion

		#region Methods/Operators

		private static object GetPropertyValue(object target, PropertyInfo property)
		{
			try
			{
				return property.GetValue(target, null);
			}
			catch (MethodAccessException
#if SILVERLIGHT
				e)
			{
				string message = "Could not read properties of anonymous object due to restrictive behavior of Silverlight. Make your assembly internal types visible to Castle.Core by adding the following attribute: [assembly: InternalsVisibleTo(InternalsVisible.ToCastleCore)]";
				throw new InvalidOperationException(message,e);
#else
				)
			{
				throw;
#endif
			}
		}

		private static IEnumerable<PropertyInfo> GetReadableProperties(Type targetType)
		{
			return targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(IsReadable);
		}

		private static bool IsReadable(PropertyInfo property)
		{
			return property.CanRead && property.GetIndexParameters().Length == 0;
		}

		/// <summary>
		/// Reads values of properties from <paramref name="valuesAsAnonymousObject" /> and inserts them into
		/// <paramref
		///     name="targetDictionary" />
		/// using property names as keys.
		/// </summary>
		/// <param name="targetDictionary"> </param>
		/// <param name="valuesAsAnonymousObject"> </param>
		public static void Read(IDictionary targetDictionary, object valuesAsAnonymousObject)
		{
			var targetType = valuesAsAnonymousObject.GetType();
			foreach (var property in GetReadableProperties(targetType))
			{
				var value = GetPropertyValue(valuesAsAnonymousObject, property);
				targetDictionary[property.Name] = value;
			}
		}

		/// <summary>
		/// Adds an element with the provided key and value to the <see cref="T:System.Collections.IDictionary" /> object.
		/// </summary>
		/// <param name="key"> The <see cref="T:System.Object" /> to use as the key of the element to add. </param>
		/// <param name="value"> The <see cref="T:System.Object" /> to use as the value of the element to add. </param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="key" /> is null.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		/// An element with the same key already exists in the
		/// <see
		///     cref="T:System.Collections.IDictionary" />
		/// object.
		/// </exception>
		/// <exception cref="T:System.NotSupportedException">
		/// The <see cref="T:System.Collections.IDictionary" /> is read-only.-or- The
		/// <see
		///     cref="T:System.Collections.IDictionary" />
		/// has a fixed size.
		/// </exception>
		public void Add(object key, object value)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Removes all elements from the <see cref="T:System.Collections.IDictionary" /> object.
		/// </summary>
		/// <exception cref="T:System.NotSupportedException"> The <see cref="T:System.Collections.IDictionary" /> object is read-only. </exception>
		public void Clear()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Determines whether the <see cref="T:System.Collections.IDictionary" /> object contains an element with the specified key.
		/// </summary>
		/// <param name="key"> The key to locate in the <see cref="T:System.Collections.IDictionary" /> object. </param>
		/// <returns>
		/// true if the <see cref="T:System.Collections.IDictionary" /> contains an element with the key; otherwise, false.
		/// </returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="key" /> is null.
		/// </exception>
		public bool Contains(object key)
		{
			return this.properties.ContainsKey(key.ToString());
		}

		/// <summary>
		/// Copies the elements of the <see cref="T:System.Collections.ICollection" /> to an <see cref="T:System.Array" />, starting at a particular
		/// <see
		///     cref="T:System.Array" />
		/// index.
		/// </summary>
		/// <param name="array">
		/// The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from
		/// <see
		///     cref="T:System.Collections.ICollection" />
		/// . The <see cref="T:System.Array" /> must have zero-based indexing.
		/// </param>
		/// <param name="index"> The zero-based index in <paramref name="array" /> at which copying begins. </param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="array" /> is null.
		/// </exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		/// <paramref name="index" /> is less than zero.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="array" /> is multidimensional.-or- <paramref name="index" /> is equal to or greater than the length of
		/// <paramref
		///     name="array" />
		/// .-or- The number of elements in the source <see cref="T:System.Collections.ICollection" /> is greater than the available space from
		/// <paramref
		///     name="index" />
		/// to the end of the destination <paramref name="array" />.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		/// The type of the source <see cref="T:System.Collections.ICollection" /> cannot be cast automatically to the type of the destination
		/// <paramref
		///     name="array" />
		/// .
		/// </exception>
		void ICollection.CopyTo(Array array, int index)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator GetEnumerator()
		{
			return new DictionaryEntryEnumeratorAdapter(this.properties.GetEnumerator());
		}

		/// <summary>
		/// Returns an <see cref="T:System.Collections.IDictionaryEnumerator" /> object for the
		/// <see
		///     cref="T:System.Collections.IDictionary" />
		/// object.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IDictionaryEnumerator" /> object for the
		/// <see
		///     cref="T:System.Collections.IDictionary" />
		/// object.
		/// </returns>
		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return new DictionaryEntryEnumeratorAdapter(this.properties.GetEnumerator());
		}

		/// <summary>
		/// Removes the element with the specified key from the <see cref="T:System.Collections.IDictionary" /> object.
		/// </summary>
		/// <param name="key"> The key of the element to remove. </param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="key" /> is null.
		/// </exception>
		/// <exception cref="T:System.NotSupportedException">
		/// The <see cref="T:System.Collections.IDictionary" /> object is read-only.-or- The
		/// <see
		///     cref="T:System.Collections.IDictionary" />
		/// has a fixed size.
		/// </exception>
		public void Remove(object key)
		{
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		private class DictionaryEntryEnumeratorAdapter : IDictionaryEnumerator
		{
			#region Constructors/Destructors

			public DictionaryEntryEnumeratorAdapter(IDictionaryEnumerator enumerator)
			{
				this.enumerator = enumerator;
			}

			#endregion

			#region Fields/Constants

			private readonly IDictionaryEnumerator enumerator;
			private KeyValuePair<string, object> current;

			#endregion

			#region Properties/Indexers/Events

			public object Current
			{
				get
				{
					return new DictionaryEntry(this.Key, this.Value);
				}
			}

			public DictionaryEntry Entry
			{
				get
				{
					return new DictionaryEntry(this.Key, this.Value);
				}
			}

			public object Key
			{
				get
				{
					return this.current.Key;
				}
			}

			public object Value
			{
				get
				{
					return this.current.Value;
				}
			}

			#endregion

			#region Methods/Operators

			public bool MoveNext()
			{
				var moved = this.enumerator.MoveNext();

				if (moved)
					this.current = (KeyValuePair<string, object>)this.enumerator.Current;

				return moved;
			}

			public void Reset()
			{
				this.enumerator.Reset();
			}

			#endregion
		}

		#endregion
	}
}