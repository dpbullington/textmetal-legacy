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

namespace Castle.Components.DictionaryAdapter
{
	using System;
#if !SILVERLIGHT
	using System.Collections.Specialized;

#else
	using HybridDictionary = System.Collections.Generic.Dictionary<object, object>;
#endif

	public abstract partial class DictionaryAdapterBase : IDictionaryCreate
	{
		#region Methods/Operators

		public T Create<T>()
		{
			return this.Create<T>(new HybridDictionary());
		}

		public object Create(Type type)
		{
			return this.Create(type, new HybridDictionary());
		}

		public T Create<T>(IDictionary dictionary)
		{
			return (T)this.Create(typeof(T), dictionary ?? new HybridDictionary());
		}

		public object Create(Type type, IDictionary dictionary)
		{
			if (this.This.CreateStrategy != null)
			{
				var created = this.This.CreateStrategy.Create(this, type, dictionary);
				if (created != null)
					return created;
			}
			dictionary = dictionary ?? new HybridDictionary();
			return this.This.Factory.GetAdapter(type, dictionary, this.This.Descriptor);
		}

		public T Create<T>(Action<T> init)
		{
			return this.Create<T>(new HybridDictionary(), init);
		}

		public T Create<T>(IDictionary dictionary, Action<T> init)
		{
			var adapter = Create<T>(dictionary ?? new HybridDictionary());
			if (init != null)
				init(adapter);
			return adapter;
		}

		#endregion
	}
}