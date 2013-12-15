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

using System.Collections.Generic;
using System.Threading;

using Castle.Core.Internal;

#if !SILVERLIGHT && !MONO // Until support for other platforms is verified

namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;

	public class SingletonDispenser<TKey, TItem>
		where TItem : class
	{
		#region Constructors/Destructors

		public SingletonDispenser(Func<TKey, TItem> factory)
		{
			if (factory == null)
				throw Error.ArgumentNull("factory");

			this.locker = new SlimReadWriteLock();
			this.items = new Dictionary<TKey, object>();
			this.factory = factory;
		}

		#endregion

		#region Fields/Constants

		private readonly Func<TKey, TItem> factory;
		private readonly Dictionary<TKey, object> items;
		private readonly Lock locker;

		#endregion

		#region Properties/Indexers/Events

		public TItem this[TKey key]
		{
			get
			{
				return this.GetOrCreate(key);
			}
			protected set
			{
				this.items[key] = value;
			}
		}

		#endregion

		#region Methods/Operators

		private TItem Create(TKey key, object item)
		{
			var handle = (ManualResetEvent)item;

			var result = this.factory(key);

			using (this.locker.ForWriting())
				this.items[key] = result;

			handle.Set();
			return result;
		}

		private TItem GetOrCreate(TKey key)
		{
			object item;
			return this.TryGetExistingItem(key, out item)
				? item as TItem ?? this.WaitForCreate(key, item)
				: this.Create(key, item);
		}

		private bool TryGetExistingItem(TKey key, out object item)
		{
			using (this.locker.ForReading())
			{
				if (this.items.TryGetValue(key, out item))
					return true;
			}

			using (var hold = this.locker.ForReadingUpgradeable())
			{
				if (this.items.TryGetValue(key, out item))
					return true;

				using (hold.Upgrade())
					this.items[key] = item = new ManualResetEvent(false);
			}

			return false;
		}

		private TItem WaitForCreate(TKey key, object item)
		{
			var handle = (ManualResetEvent)item;

			handle.WaitOne();

			using (this.locker.ForReading())
				return (TItem)this.items[key];
		}

		#endregion
	}
}

#endif