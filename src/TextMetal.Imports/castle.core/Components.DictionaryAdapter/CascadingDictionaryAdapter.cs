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
	public class CascadingDictionaryAdapter : AbstractDictionaryAdapter
	{
		#region Constructors/Destructors

		public CascadingDictionaryAdapter(IDictionary primary, IDictionary secondary)
		{
			this.primary = primary;
			this.secondary = secondary;
		}

		#endregion

		#region Fields/Constants

		private readonly IDictionary primary;
		private readonly IDictionary secondary;

		#endregion

		#region Properties/Indexers/Events

		public override object this[object key]
		{
			get
			{
				return this.primary[key] ?? this.secondary[key];
			}
			set
			{
				this.primary[key] = value;
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				return this.primary.IsReadOnly;
			}
		}

		public IDictionary Primary
		{
			get
			{
				return this.primary;
			}
		}

		public IDictionary Secondary
		{
			get
			{
				return this.secondary;
			}
		}

		#endregion

		#region Methods/Operators

		public override bool Contains(object key)
		{
			return this.primary.Contains(key) || this.secondary.Contains(key);
		}

		#endregion
	}
}