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

namespace Castle.DynamicProxy.Generators
{
	using System;

	public class TypeElementCollection<TElement> : ICollection<TElement>
		where TElement : MetaTypeElement, IEquatable<TElement>
	{
		#region Fields/Constants

		private readonly ICollection<TElement> items = new List<TElement>();

		#endregion

		#region Properties/Indexers/Events

		public int Count
		{
			get
			{
				return this.items.Count;
			}
		}

		bool ICollection<TElement>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		#endregion

		#region Methods/Operators

		public void Add(TElement item)
		{
			if (item.CanBeImplementedExplicitly == false)
			{
				this.items.Add(item);
				return;
			}
			if (this.Contains(item))
			{
				item.SwitchToExplicitImplementation();
				if (this.Contains(item))
				{
					// there is something reaaaly wrong going on here
					throw new ProxyGenerationException("Duplicate element: " + item);
				}
			}
			this.items.Add(item);
		}

		void ICollection<TElement>.Clear()
		{
			throw new NotSupportedException();
		}

		public bool Contains(TElement item)
		{
			foreach (var element in this.items)
			{
				if (element.Equals(item))
					return true;
			}

			return false;
		}

		void ICollection<TElement>.CopyTo(TElement[] array, int arrayIndex)
		{
			throw new NotSupportedException();
		}

		public IEnumerator<TElement> GetEnumerator()
		{
			return this.items.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		bool ICollection<TElement>.Remove(TElement item)
		{
			throw new NotSupportedException();
		}

		#endregion
	}
}