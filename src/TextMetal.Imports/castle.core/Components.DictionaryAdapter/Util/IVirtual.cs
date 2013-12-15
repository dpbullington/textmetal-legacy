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

namespace Castle.Components.DictionaryAdapter
{
	using System;

	public interface IVirtual
	{
		#region Properties/Indexers/Events

		event EventHandler Realized;

		bool IsReal
		{
			get;
		}

		#endregion

		#region Methods/Operators

		void Realize();

		#endregion
	}

	public interface IVirtual<T> : IVirtual
	{
		#region Methods/Operators

		void AddSite(IVirtualSite<T> site);

		new T Realize();

		void RemoveSite(IVirtualSite<T> site);

		#endregion
	}
}