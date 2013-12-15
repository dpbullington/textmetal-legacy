// Copyright 2004-2012 Castle Project - http://www.castleproject.org/
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

namespace Castle.Components.DictionaryAdapter
{
	using System;

	public abstract class VirtualObject<TNode> : IVirtual<TNode>
	{
		#region Constructors/Destructors

		protected VirtualObject()
		{
		}

		protected VirtualObject(IVirtualSite<TNode> site)
		{
			this.sites = new List<IVirtualSite<TNode>> { site };
		}

		#endregion

		#region Fields/Constants

		private List<IVirtualSite<TNode>> sites;

		#endregion

		#region Properties/Indexers/Events

		public event EventHandler Realized;

		public abstract bool IsReal
		{
			get;
		}

		#endregion

		#region Methods/Operators

		protected void AddSite(IVirtualSite<TNode> site)
		{
			if (this.sites != null)
				this.sites.Add(site);
		}

		void IVirtual<TNode>.AddSite(IVirtualSite<TNode> site)
		{
			this.AddSite(site);
		}

		protected virtual void OnRealized()
		{
			var handler = this.Realized;
			if (handler != null)
			{
				handler(this, EventArgs.Empty);
				this.Realized = null;
			}
		}

		public TNode Realize()
		{
			TNode node;
			if (this.TryRealize(out node))
			{
				if (this.sites != null)
				{
					var count = this.sites.Count;
					for (var i = 0; i < count; i++)
						this.sites[i].OnRealizing(node);
					this.sites = null;
				}

				this.OnRealized();
			}
			return node;
		}

		void IVirtual.Realize()
		{
			this.Realize();
		}

		protected void RemoveSite(IVirtualSite<TNode> site)
		{
			if (this.sites != null)
			{
				var index = this.sites.IndexOf(site);
				if (index != -1)
					this.sites.RemoveAt(index);
			}
		}

		void IVirtual<TNode>.RemoveSite(IVirtualSite<TNode> site)
		{
			this.RemoveSite(site);
		}

		protected abstract bool TryRealize(out TNode node);

		#endregion
	}
}