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
#if NET20
using Newtonsoft.Json.Utilities.LinqBridge;
#endif
using System.Collections.Generic;
using System.Threading;

namespace Newtonsoft.Json.Utilities
{
	internal class ThreadSafeStore<TKey, TValue>
	{
		#region Constructors/Destructors

		public ThreadSafeStore(Func<TKey, TValue> creator)
		{
			if (creator == null)
				throw new ArgumentNullException("creator");

			this._creator = creator;
			this._store = new Dictionary<TKey, TValue>();
		}

		#endregion

		#region Fields/Constants

		private readonly Func<TKey, TValue> _creator;

		private readonly object _lock = new object();
		private Dictionary<TKey, TValue> _store;

		#endregion

		#region Methods/Operators

		private TValue AddValue(TKey key)
		{
			TValue value = this._creator(key);

			lock (this._lock)
			{
				if (this._store == null)
				{
					this._store = new Dictionary<TKey, TValue>();
					this._store[key] = value;
				}
				else
				{
					// double check locking
					TValue checkValue;
					if (this._store.TryGetValue(key, out checkValue))
						return checkValue;

					Dictionary<TKey, TValue> newStore = new Dictionary<TKey, TValue>(this._store);
					newStore[key] = value;

#if !(NETFX_CORE || PORTABLE)
					Thread.MemoryBarrier();
#endif
					this._store = newStore;
				}

				return value;
			}
		}

		public TValue Get(TKey key)
		{
			TValue value;
			if (!this._store.TryGetValue(key, out value))
				return this.AddValue(key);

			return value;
		}

		#endregion
	}
}