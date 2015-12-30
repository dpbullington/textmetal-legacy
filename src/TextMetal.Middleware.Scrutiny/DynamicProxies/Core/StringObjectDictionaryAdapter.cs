// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
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

namespace Castle.Core
{
	using System;

	public sealed class StringObjectDictionaryAdapter : IDictionary<string, object>
	{
		#region Constructors/Destructors

		public StringObjectDictionaryAdapter(IDictionary dictionary)
		{
			this.dictionary = dictionary;
		}

		#endregion

		#region Fields/Constants

		private readonly IDictionary dictionary;

		#endregion

		#region Properties/Indexers/Events

		object IDictionary<string, object>.this[string key]
		{
			get
			{
				return this.dictionary[key];
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public object this[object key]
		{
			get
			{
				return this.dictionary[key];
			}
			set
			{
				this.dictionary[key] = value;
			}
		}

		public int Count
		{
			get
			{
				return this.dictionary.Count;
			}
		}

		public bool IsFixedSize
		{
			get
			{
				return this.dictionary.IsFixedSize;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return this.dictionary.IsReadOnly;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return this.dictionary.IsSynchronized;
			}
		}

		ICollection<string> IDictionary<string, object>.Keys
		{
			get
			{
				string[] keys = new string[this.Count];
				this.dictionary.Keys.CopyTo(keys, 0);
				return keys;
			}
		}

		public ICollection Keys
		{
			get
			{
				return this.dictionary.Keys;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this.dictionary.SyncRoot;
			}
		}

		ICollection<object> IDictionary<string, object>.Values
		{
			get
			{
				object[] values = new object[this.Count];
				this.dictionary.Values.CopyTo(values, 0);
				return values;
			}
		}

		public ICollection Values
		{
			get
			{
				return this.dictionary.Values;
			}
		}

		#endregion

		#region Methods/Operators

		void IDictionary<string, object>.Add(string key, object value)
		{
			throw new NotImplementedException();
		}

		void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
		{
			throw new NotImplementedException();
		}

		public void Add(object key, object value)
		{
			this.dictionary.Add(key, value);
		}

		public void Clear()
		{
			this.dictionary.Clear();
		}

		bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
		{
			throw new NotImplementedException();
		}

		public bool Contains(object key)
		{
			return this.dictionary.Contains(key);
		}

		bool IDictionary<string, object>.ContainsKey(string key)
		{
			return this.dictionary.Contains(key);
		}

		void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public void CopyTo(Array array, int index)
		{
			this.dictionary.CopyTo(array, index);
		}

		IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
		{
			return new EnumeratorAdapter(this);
		}

		public IEnumerator GetEnumerator()
		{
			return ((IEnumerable)this.dictionary).GetEnumerator();
		}

		bool IDictionary<string, object>.Remove(string key)
		{
			throw new NotImplementedException();
		}

		bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
		{
			throw new NotImplementedException();
		}

		public void Remove(object key)
		{
			this.dictionary.Remove(key);
		}

		bool IDictionary<string, object>.TryGetValue(string key, out object value)
		{
			value = null;
			if (this.dictionary.Contains(key))
			{
				value = this.dictionary[key];
				return true;
			}
			else
				return false;
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		internal class EnumeratorAdapter : IEnumerator<KeyValuePair<string, object>>
		{
			#region Constructors/Destructors

			public EnumeratorAdapter(StringObjectDictionaryAdapter adapter)
			{
				this.adapter = adapter;
				this.keyEnumerator = ((IDictionary<string, object>)adapter).Keys.GetEnumerator();
			}

			#endregion

			#region Fields/Constants

			private readonly StringObjectDictionaryAdapter adapter;
			private string currentKey;
			private object currentValue;
			private IEnumerator<string> keyEnumerator;

			#endregion

			#region Properties/Indexers/Events

			public object Current
			{
				get
				{
					return new KeyValuePair<string, object>(this.currentKey, this.currentValue);
				}
			}

			KeyValuePair<string, object> IEnumerator<KeyValuePair<string, object>>.Current
			{
				get
				{
					return new KeyValuePair<string, object>(this.currentKey, this.currentValue);
				}
			}

			#endregion

			#region Methods/Operators

			public void Dispose()
			{
				GC.SuppressFinalize(this);
			}

			public bool MoveNext()
			{
				if (this.keyEnumerator.MoveNext())
				{
					this.currentKey = this.keyEnumerator.Current;
					this.currentValue = this.adapter[this.currentKey];
					return true;
				}

				return false;
			}

			public void Reset()
			{
				this.keyEnumerator.Reset();
			}

			#endregion
		}

		#endregion
	}
}