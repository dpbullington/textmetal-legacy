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

using System.Diagnostics;

namespace Castle.Components.DictionaryAdapter
{
	using System;

	internal sealed class ListProjectionDebugView<T>
	{
		#region Constructors/Destructors

		public ListProjectionDebugView(ListProjection<T> projection)
		{
			if (projection == null)
				throw new ArgumentNullException("projection");

			this.projection = projection;
		}

		#endregion

		#region Fields/Constants

		private readonly ListProjection<T> projection;

		#endregion

		#region Properties/Indexers/Events

		public ICollectionAdapter<T> Adapter
		{
			get
			{
				return this.projection.Adapter;
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public T[] Items
		{
			get
			{
				T[] array = new T[this.projection.Count];
				this.projection.CopyTo(array, 0);
				return array;
			}
		}

		#endregion
	}
}