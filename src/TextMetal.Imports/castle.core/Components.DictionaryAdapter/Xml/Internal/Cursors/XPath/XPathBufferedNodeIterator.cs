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
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.f
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Collections.Generic;
using System.Xml.XPath;

#if !SILVERLIGHT && !MONO // Until support for other platforms is verified
#if !SL3

namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;

	internal class XPathBufferedNodeIterator : XPathNodeIterator
	{
		#region Constructors/Destructors

		public XPathBufferedNodeIterator(XPathNodeIterator iterator)
		{
			this.items = new List<XPathNavigator>();
			do
				this.items.Add(iterator.Current.Clone());
			while (iterator.MoveNext());
		}

		private XPathBufferedNodeIterator(XPathBufferedNodeIterator iterator)
		{
			this.items = iterator.items;
			this.index = iterator.index;
		}

		#endregion

		#region Fields/Constants

		private readonly IList<XPathNavigator> items;
		private int index;

		#endregion

		#region Properties/Indexers/Events

		public override int Count
		{
			get
			{
				return this.items.Count - 1;
			}
		}

		public override XPathNavigator Current
		{
			get
			{
				return this.items[this.index];
			}
		}

		public override int CurrentPosition
		{
			get
			{
				return this.index;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.items.Count == 1;
			}
		}

		#endregion

		#region Methods/Operators

		public override XPathNodeIterator Clone()
		{
			return new XPathBufferedNodeIterator(this);
		}

		public override bool MoveNext()
		{
			if (++this.index < this.items.Count)
				return true;
			if (this.index > this.items.Count)
				this.index--;
			return false;
		}

		public void MoveToEnd()
		{
			this.index = this.items.Count;
		}

		public void Reset()
		{
			this.index = 0;
		}

		#endregion
	}
}

#endif
#endif