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
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.ComponentModel;

namespace Castle.Components.DictionaryAdapter
{
	public class PropertyChangedEventArgsEx : PropertyChangedEventArgs
	{
		#region Constructors/Destructors

		public PropertyChangedEventArgsEx(string propertyName, object oldValue, object newValue)
			: base(propertyName)
		{
			this.oldValue = oldValue;
			this.newValue = newValue;
		}

		#endregion

		#region Fields/Constants

		private readonly object newValue;
		private readonly object oldValue;

		#endregion

		#region Properties/Indexers/Events

		public object NewValue
		{
			get
			{
				return this.newValue;
			}
		}

		public object OldValue
		{
			get
			{
				return this.oldValue;
			}
		}

		#endregion
	}
}