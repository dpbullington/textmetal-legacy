#region License

// Copyright (c) 2007 James Newton-King
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
#if NET20
using Newtonsoft.Json.Utilities.LinqBridge;
#else
using System.Linq;

#endif

namespace Newtonsoft.Json.Utilities
{
	internal interface IWrappedDictionary
		: IDictionary
	{
		#region Properties/Indexers/Events

		object UnderlyingDictionary
		{
			get;
		}

		#endregion
	}

	internal class DictionaryWrapper<TKey, TValue> : IDictionary<TKey, TValue>, IWrappedDictionary
	{
		private readonly IDictionary _dictionary;
		private readonly IDictionary<TKey, TValue> _genericDictionary;
#if !(NET40 || NET35 || NET20 || SILVERLIGHT || WINDOWS_PHONE || PORTABLE40)
    private readonly IReadOnlyDictionary<TKey, TValue> _readOnlyDictionary; 
#endif
		private object _syncRoot;

		public DictionaryWrapper(IDictionary dictionary)
		{
			ValidationUtils.ArgumentNotNull(dictionary, "dictionary");

			this._dictionary = dictionary;
		}

		public DictionaryWrapper(IDictionary<TKey, TValue> dictionary)
		{
			ValidationUtils.ArgumentNotNull(dictionary, "dictionary");

			this._genericDictionary = dictionary;
		}

#if !(NET40 || NET35 || NET20 || SILVERLIGHT || WINDOWS_PHONE || PORTABLE40)
    public DictionaryWrapper(IReadOnlyDictionary<TKey, TValue> dictionary)
    {
      ValidationUtils.ArgumentNotNull(dictionary, "dictionary");

      _readOnlyDictionary = dictionary;
    }
#endif

		public void Add(TKey key, TValue value)
		{
			if (this._dictionary != null)
				this._dictionary.Add(key, value);
			else if (this._genericDictionary != null)
				this._genericDictionary.Add(key, value);
			else
				throw new NotSupportedException();
		}

		public bool ContainsKey(TKey key)
		{
			if (this._dictionary != null)
				return this._dictionary.Contains(key);
#if !(NET40 || NET35 || NET20 || SILVERLIGHT || WINDOWS_PHONE || PORTABLE40)
      else if (_readOnlyDictionary != null)
        return _readOnlyDictionary.ContainsKey(key);
#endif
			else
				return this._genericDictionary.ContainsKey(key);
		}

		public ICollection<TKey> Keys
		{
			get
			{
				if (this._dictionary != null)
					return this._dictionary.Keys.Cast<TKey>().ToList();
#if !(NET40 || NET35 || NET20 || SILVERLIGHT || WINDOWS_PHONE || PORTABLE40)
        else if (_readOnlyDictionary != null)
          return _readOnlyDictionary.Keys.ToList();
#endif
				else
					return this._genericDictionary.Keys;
			}
		}

		public bool Remove(TKey key)
		{
			if (this._dictionary != null)
			{
				if (this._dictionary.Contains(key))
				{
					this._dictionary.Remove(key);
					return true;
				}
				else
					return false;
			}
#if !(NET40 || NET35 || NET20 || SILVERLIGHT || WINDOWS_PHONE || PORTABLE40)
      else if (_readOnlyDictionary != null)
      {
        throw new NotSupportedException();
      }
#endif
			else
				return this._genericDictionary.Remove(key);
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			if (this._dictionary != null)
			{
				if (!this._dictionary.Contains(key))
				{
					value = default(TValue);
					return false;
				}
				else
				{
					value = (TValue)this._dictionary[key];
					return true;
				}
			}
#if !(NET40 || NET35 || NET20 || SILVERLIGHT || WINDOWS_PHONE || PORTABLE40)
      else if (_readOnlyDictionary != null)
      {
        throw new NotSupportedException();
      }
#endif
			else
				return this._genericDictionary.TryGetValue(key, out value);
		}

		public ICollection<TValue> Values
		{
			get
			{
				if (this._dictionary != null)
					return this._dictionary.Values.Cast<TValue>().ToList();
#if !(NET40 || NET35 || NET20 || SILVERLIGHT || WINDOWS_PHONE || PORTABLE40)
        else if (_readOnlyDictionary != null)
          return _readOnlyDictionary.Values.ToList();
#endif
				else
					return this._genericDictionary.Values;
			}
		}

		public TValue this[TKey key]
		{
			get
			{
				if (this._dictionary != null)
					return (TValue)this._dictionary[key];
#if !(NET40 || NET35 || NET20 || SILVERLIGHT || WINDOWS_PHONE || PORTABLE40)
        else if (_readOnlyDictionary != null)
          return _readOnlyDictionary[key];
#endif
				else
					return this._genericDictionary[key];
			}
			set
			{
				if (this._dictionary != null)
					this._dictionary[key] = value;
#if !(NET40 || NET35 || NET20 || SILVERLIGHT || WINDOWS_PHONE || PORTABLE40)
        else if (_readOnlyDictionary != null)
          throw new NotSupportedException();
#endif
				else
					this._genericDictionary[key] = value;
			}
		}

		public void Add(KeyValuePair<TKey, TValue> item)
		{
			if (this._dictionary != null)
				((IList)this._dictionary).Add(item);
#if !(NET40 || NET35 || NET20 || SILVERLIGHT || WINDOWS_PHONE || PORTABLE40)
      else if (_readOnlyDictionary != null)
        throw new NotSupportedException();
#endif
			else if (this._genericDictionary != null)
				this._genericDictionary.Add(item);
		}

		public void Clear()
		{
			if (this._dictionary != null)
				this._dictionary.Clear();
#if !(NET40 || NET35 || NET20 || SILVERLIGHT || WINDOWS_PHONE || PORTABLE40)
      else if (_readOnlyDictionary != null)
        throw new NotSupportedException();
#endif
			else
				this._genericDictionary.Clear();
		}

		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			if (this._dictionary != null)
				return ((IList)this._dictionary).Contains(item);
#if !(NET40 || NET35 || NET20 || SILVERLIGHT || WINDOWS_PHONE || PORTABLE40)
      else if (_readOnlyDictionary != null)
        return _readOnlyDictionary.Contains(item);
#endif
			else
				return this._genericDictionary.Contains(item);
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			if (this._dictionary != null)
			{
				foreach (DictionaryEntry item in this._dictionary)
					array[arrayIndex++] = new KeyValuePair<TKey, TValue>((TKey)item.Key, (TValue)item.Value);
			}
#if !(NET40 || NET35 || NET20 || SILVERLIGHT || WINDOWS_PHONE || PORTABLE40)
      else if (_readOnlyDictionary != null)
      {
        throw new NotSupportedException();
      }
#endif
			else
				this._genericDictionary.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get
			{
				if (this._dictionary != null)
					return this._dictionary.Count;
#if !(NET40 || NET35 || NET20 || SILVERLIGHT || WINDOWS_PHONE || PORTABLE40)
        else if (_readOnlyDictionary != null)
          return _readOnlyDictionary.Count;
#endif
				else
					return this._genericDictionary.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				if (this._dictionary != null)
					return this._dictionary.IsReadOnly;
#if !(NET40 || NET35 || NET20 || SILVERLIGHT || WINDOWS_PHONE || PORTABLE40)
        else if (_readOnlyDictionary != null)
          return true;
#endif
				else
					return this._genericDictionary.IsReadOnly;
			}
		}

		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			if (this._dictionary != null)
			{
				if (this._dictionary.Contains(item.Key))
				{
					object value = this._dictionary[item.Key];

					if (Equals(value, item.Value))
					{
						this._dictionary.Remove(item.Key);
						return true;
					}
					else
						return false;
				}
				else
					return true;
			}
#if !(NET40 || NET35 || NET20 || SILVERLIGHT || WINDOWS_PHONE || PORTABLE40)
      else if (_readOnlyDictionary != null)
      {
        throw new NotSupportedException();
      }
#endif
			else
				return this._genericDictionary.Remove(item);
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			if (this._dictionary != null)
				return this._dictionary.Cast<DictionaryEntry>().Select(de => new KeyValuePair<TKey, TValue>((TKey)de.Key, (TValue)de.Value)).GetEnumerator();
#if !(NET40 || NET35 || NET20 || SILVERLIGHT || WINDOWS_PHONE || PORTABLE40)
      else if (_readOnlyDictionary != null)
        return _readOnlyDictionary.GetEnumerator();
#endif
			else
				return this._genericDictionary.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		void IDictionary.Add(object key, object value)
		{
			if (this._dictionary != null)
				this._dictionary.Add(key, value);
#if !(NET40 || NET35 || NET20 || SILVERLIGHT || WINDOWS_PHONE || PORTABLE40)
      else if (_readOnlyDictionary != null)
        throw new NotSupportedException();
#endif
			else
				this._genericDictionary.Add((TKey)key, (TValue)value);
		}

		object IDictionary.this[object key]
		{
			get
			{
				if (this._dictionary != null)
					return this._dictionary[key];
#if !(NET40 || NET35 || NET20 || SILVERLIGHT || WINDOWS_PHONE || PORTABLE40)
        else if (_readOnlyDictionary != null)
          return _readOnlyDictionary[(TKey)key];
#endif
				else
					return this._genericDictionary[(TKey)key];
			}
			set
			{
				if (this._dictionary != null)
					this._dictionary[key] = value;
#if !(NET40 || NET35 || NET20 || SILVERLIGHT || WINDOWS_PHONE || PORTABLE40)
        else if (_readOnlyDictionary != null)
          throw new NotSupportedException();
#endif
				else
					this._genericDictionary[(TKey)key] = (TValue)value;
			}
		}

		private struct DictionaryEnumerator<TEnumeratorKey, TEnumeratorValue> : IDictionaryEnumerator
		{
			#region Constructors/Destructors

			public DictionaryEnumerator(IEnumerator<KeyValuePair<TEnumeratorKey, TEnumeratorValue>> e)
			{
				ValidationUtils.ArgumentNotNull(e, "e");
				this._e = e;
			}

			#endregion

			#region Fields/Constants

			private readonly IEnumerator<KeyValuePair<TEnumeratorKey, TEnumeratorValue>> _e;

			#endregion

			#region Properties/Indexers/Events

			public object Current
			{
				get
				{
					return new DictionaryEntry(this._e.Current.Key, this._e.Current.Value);
				}
			}

			public DictionaryEntry Entry
			{
				get
				{
					return (DictionaryEntry)this.Current;
				}
			}

			public object Key
			{
				get
				{
					return this.Entry.Key;
				}
			}

			public object Value
			{
				get
				{
					return this.Entry.Value;
				}
			}

			#endregion

			#region Methods/Operators

			public bool MoveNext()
			{
				return this._e.MoveNext();
			}

			public void Reset()
			{
				this._e.Reset();
			}

			#endregion
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			if (this._dictionary != null)
				return this._dictionary.GetEnumerator();
#if !(NET40 || NET35 || NET20 || SILVERLIGHT || WINDOWS_PHONE || PORTABLE40)
      else if (_readOnlyDictionary != null)
        return new DictionaryEnumerator<TKey, TValue>(_readOnlyDictionary.GetEnumerator());
#endif
			else
				return new DictionaryEnumerator<TKey, TValue>(this._genericDictionary.GetEnumerator());
		}

		bool IDictionary.Contains(object key)
		{
			if (this._genericDictionary != null)
				return this._genericDictionary.ContainsKey((TKey)key);
#if !(NET40 || NET35 || NET20 || SILVERLIGHT || WINDOWS_PHONE || PORTABLE40)
      else if (_readOnlyDictionary != null)
        return _readOnlyDictionary.ContainsKey((TKey)key);
#endif
			else
				return this._dictionary.Contains(key);
		}

		bool IDictionary.IsFixedSize
		{
			get
			{
				if (this._genericDictionary != null)
					return false;
#if !(NET40 || NET35 || NET20 || SILVERLIGHT || WINDOWS_PHONE || PORTABLE40)
        else if (_readOnlyDictionary != null)
          return true;
#endif
				else
					return this._dictionary.IsFixedSize;
			}
		}

		ICollection IDictionary.Keys
		{
			get
			{
				if (this._genericDictionary != null)
					return this._genericDictionary.Keys.ToList();
#if !(NET40 || NET35 || NET20 || SILVERLIGHT || WINDOWS_PHONE || PORTABLE40)
        else if (_readOnlyDictionary != null)
          return _readOnlyDictionary.Keys.ToList();
#endif
				else
					return this._dictionary.Keys;
			}
		}

		public void Remove(object key)
		{
			if (this._dictionary != null)
				this._dictionary.Remove(key);
#if !(NET40 || NET35 || NET20 || SILVERLIGHT || WINDOWS_PHONE || PORTABLE40)
      else if (_readOnlyDictionary != null)
        throw new NotSupportedException();
#endif
			else
				this._genericDictionary.Remove((TKey)key);
		}

		ICollection IDictionary.Values
		{
			get
			{
				if (this._genericDictionary != null)
					return this._genericDictionary.Values.ToList();
#if !(NET40 || NET35 || NET20 || SILVERLIGHT || WINDOWS_PHONE || PORTABLE40)
        else if (_readOnlyDictionary != null)
          return _readOnlyDictionary.Values.ToList();
#endif
				else
					return this._dictionary.Values;
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			if (this._dictionary != null)
				this._dictionary.CopyTo(array, index);
#if !(NET40 || NET35 || NET20 || SILVERLIGHT || WINDOWS_PHONE || PORTABLE40)
      else if (_readOnlyDictionary != null)
        throw new NotSupportedException();
#endif
			else
				this._genericDictionary.CopyTo((KeyValuePair<TKey, TValue>[])array, index);
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				if (this._dictionary != null)
					return this._dictionary.IsSynchronized;
				else
					return false;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				if (this._syncRoot == null)
					Interlocked.CompareExchange(ref this._syncRoot, new object(), null);

				return this._syncRoot;
			}
		}

		public object UnderlyingDictionary
		{
			get
			{
				if (this._dictionary != null)
					return this._dictionary;
#if !(NET40 || NET35 || NET20 || SILVERLIGHT || WINDOWS_PHONE || PORTABLE40)
        else if (_readOnlyDictionary != null)
          return _readOnlyDictionary;
#endif
				else
					return this._genericDictionary;
			}
		}
	}
}