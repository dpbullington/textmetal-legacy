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

#if !SILVERLIGHT && !MONO

namespace Castle.Core.Internal
{
	using System;

	internal class WeakKeyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
		where TKey : class
	{
		#region Constructors/Destructors

		public WeakKeyDictionary()
			: this(0, EqualityComparer<TKey>.Default)
		{
		}

		public WeakKeyDictionary(int capacity)
			: this(capacity, EqualityComparer<TKey>.Default)
		{
		}

		public WeakKeyDictionary(IEqualityComparer<TKey> comparer)
			: this(0, comparer)
		{
		}

		public WeakKeyDictionary(int capacity, IEqualityComparer<TKey> comparer)
		{
			this.comparer = new WeakKeyComparer<TKey>(comparer);
			this.dictionary = new Dictionary<object, TValue>(capacity, this.comparer);
		}

		#endregion

		#region Fields/Constants

		private const int AgeThreshold = 128; // Age at which to trim dead objects
		private readonly WeakKeyComparer<TKey> comparer;
		private readonly Dictionary<object, TValue> dictionary;
		private int age; // Incremented by operations
		private KeyCollection keys;

		#endregion

		#region Properties/Indexers/Events

		public TValue this[TKey key]
		{
			get
			{
				this.Age(1);
				return this.dictionary[key];
			}
			set
			{
				this.Age(4);
				this.dictionary[this.comparer.Wrap(key)] = value;
			}
		}

		public int Count
		{
			get
			{
				this.Age(1);
				return this.dictionary.Count;
			}
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public ICollection<TKey> Keys
		{
			get
			{
				return this.keys ?? (this.keys = new KeyCollection(this.dictionary.Keys));
			}
		}

		public ICollection<TValue> Values
		{
			get
			{
				return this.dictionary.Values;
			}
		}

		#endregion

		#region Methods/Operators

		public void Add(TKey key, TValue value)
		{
			this.Age(2);
			this.dictionary.Add(this.comparer.Wrap(key), value);
		}

		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
		{
			this.Add(item.Key, item.Value);
		}

		private void Age(int amount)
		{
			if ((this.age += amount) > AgeThreshold)
				this.TrimDeadObjects();
		}

		public void Clear()
		{
			this.age = 0;
			this.dictionary.Clear();
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
		{
			this.Age(1);
			TValue candidate;
			return this.dictionary.TryGetValue(item.Key, out candidate)
					&& EqualityComparer<TValue>.Default.Equals(candidate, item.Value);
		}

		public bool ContainsKey(TKey key)
		{
			this.Age(1);
			return this.dictionary.ContainsKey(key);
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
		{
			foreach (var item in this)
				array[index++] = item;
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			var hasDeadObjects = false;

			foreach (var wrapped in this.dictionary)
			{
				var item = new KeyValuePair<TKey, TValue>
					(
					this.comparer.Unwrap(wrapped.Key),
					wrapped.Value
					);

				if (item.Key == null)
					hasDeadObjects = true;
				else
					yield return item;
			}

			if (hasDeadObjects)
				this.TrimDeadObjects();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public bool Remove(TKey key)
		{
			this.Age(4);
			return this.dictionary.Remove(key);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
		{
			ICollection<KeyValuePair<TKey, TValue>> collection = this;
			return collection.Contains(item) && this.Remove(item.Key);
		}

		public void TrimDeadObjects()
		{
			this.age = 0;
			var removals = null as List<object>;

			foreach (var key in this.dictionary.Keys)
			{
				if (this.comparer.Unwrap(key) == null)
				{
					if (removals == null)
						removals = new List<object>();
					removals.Add(key);
				}
			}

			if (removals != null)
			{
				foreach (var key in removals)
					this.dictionary.Remove(key);
			}
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			this.Age(1);
			return this.dictionary.TryGetValue(key, out value);
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		private class KeyCollection : ICollection<TKey>
		{
			#region Constructors/Destructors

			public KeyCollection(ICollection<object> keys)
			{
				this.keys = keys;
			}

			#endregion

			#region Fields/Constants

			private readonly ICollection<object> keys;

			#endregion

			#region Properties/Indexers/Events

			public int Count
			{
				get
				{
					return this.keys.Count;
				}
			}

			bool ICollection<TKey>.IsReadOnly
			{
				get
				{
					return true;
				}
			}

			#endregion

			#region Methods/Operators

			private static Exception ReadOnlyCollectionError()
			{
				return new NotSupportedException("The collection is read-only.");
			}

			void ICollection<TKey>.Add(TKey item)
			{
				throw ReadOnlyCollectionError();
			}

			void ICollection<TKey>.Clear()
			{
				throw ReadOnlyCollectionError();
			}

			public bool Contains(TKey item)
			{
				return this.keys.Contains(item);
			}

			public void CopyTo(TKey[] array, int index)
			{
				foreach (var key in this)
					array[index++] = key;
			}

			public IEnumerator<TKey> GetEnumerator()
			{
				foreach (var key in this.keys)
				{
					var target = (TKey)((WeakKey)key).Target;
					if (target != null)
						yield return target;
				}
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			bool ICollection<TKey>.Remove(TKey item)
			{
				throw ReadOnlyCollectionError();
			}

			#endregion
		}

		#endregion
	}
}

#endif