// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License";
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

namespace Castle.Components.DictionaryAdapter
{
	using System;

	public class GenericDictionaryAdapter<TValue> : AbstractDictionaryAdapter
	{
		#region Constructors/Destructors

		public GenericDictionaryAdapter(IDictionary<string, TValue> dictionary)
		{
			this.dictionary = dictionary;
		}

		#endregion

		#region Fields/Constants

		private readonly IDictionary<string, TValue> dictionary;

		#endregion

		#region Properties/Indexers/Events

		public override object this[object key]
		{
			get
			{
				TValue value;
				return this.dictionary.TryGetValue(GetKey(key), out value) ? value : default(TValue);
			}
			set
			{
				this.dictionary[GetKey(key)] = (TValue)value;
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				return this.dictionary.IsReadOnly;
			}
		}

		#endregion

		#region Methods/Operators

		private static string GetKey(object key)
		{
			if (key == null)
				throw new ArgumentNullException("key");
			return key.ToString();
		}

		public override bool Contains(object key)
		{
			return this.dictionary.Keys.Contains(GetKey(key));
		}

		#endregion
	}

	public static class GenericDictionaryAdapter
	{
		#region Methods/Operators

		public static GenericDictionaryAdapter<TValue> ForDictionaryAdapter<TValue>(this IDictionary<string, TValue> dictionary)
		{
			return new GenericDictionaryAdapter<TValue>(dictionary);
		}

		#endregion
	}
}