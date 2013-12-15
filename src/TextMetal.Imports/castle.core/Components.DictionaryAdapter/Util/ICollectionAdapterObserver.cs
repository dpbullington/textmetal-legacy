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
	public interface ICollectionAdapterObserver<T>
	{
		#region Methods/Operators

		void OnInserted(T newValue, int index);

		bool OnInserting(T newValue);

		void OnRemoved(T oldValue, int index);

		void OnRemoving(T oldValue);

		void OnReplaced(T oldValue, T newValue, int index);

		bool OnReplacing(T oldValue, T newValue);

		#endregion
	}
}