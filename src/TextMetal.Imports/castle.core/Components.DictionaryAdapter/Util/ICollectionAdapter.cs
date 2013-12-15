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
	public interface ICollectionAdapter<T>
	{
		// Configuration

		#region Properties/Indexers/Events

		T this[int index]
		{
			get;
			set;
		}

		IEqualityComparer<T> Comparer
		{
			get;
		}

		// Collection Access
		int Count
		{
			get;
		}

		bool HasSnapshot
		{
			get;
		}

		int SnapshotCount
		{
			get;
		}

		#endregion

		#region Methods/Operators

		bool Add(T value);

		T AddNew();

		void Clear();

		void ClearReferences();

		void DropSnapshot();

		// A bit of a hack. Make this nicer in a future version.

		// Snapshot Support

		T GetCurrentItem(int index);

		T GetSnapshotItem(int index);

		void Initialize(ICollectionAdapterObserver<T> advisor);

		bool Insert(int index, T value);

		void LoadSnapshot();

		void Remove(int index);

		void SaveSnapshot();

		#endregion
	}
}