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

namespace Castle.Components.DictionaryAdapter
{
#if !SILVERLIGHT
	using System.Collections.Generic;
	using System.ComponentModel;

	public class EditableBindingList<T> : System.ComponentModel.BindingList<T>, IList<T>, IEditableObject, IRevertibleChangeTracking
	{
		#region Constructors/Destructors

		public EditableBindingList()
		{
		}

		public EditableBindingList(IList<T> initial)
			: base(initial)
		{
		}

		#endregion

		#region Fields/Constants

		private bool isEditing;
		private List<T> snapshot;

		#endregion

		#region Properties/Indexers/Events

		public bool IsChanged
		{
			get
			{
				if (this.snapshot == null || this.snapshot.Count != this.Count)
					return false;

				var items = this.GetEnumerator();
				var snapshotItems = this.snapshot.GetEnumerator();

				while (items.MoveNext() && snapshotItems.MoveNext())
				{
					if (ReferenceEquals(items.Current, snapshotItems.Current) == false)
						return false;

					var tracked = items.Current as IChangeTracking;
					if (tracked != null && tracked.IsChanged)
						return true;
				}

				return false;
			}
		}

		#endregion

		#region Methods/Operators

		public void AcceptChanges()
		{
			this.BeginEdit();
		}

		public void BeginEdit()
		{
			if (this.isEditing == false)
			{
				this.snapshot = new List<T>(this);
				this.isEditing = true;
			}
		}

		public void CancelEdit()
		{
			if (this.isEditing)
			{
				this.Clear();
				foreach (var item in this.snapshot)
					this.Add(item);
				this.snapshot = null;
				this.isEditing = false;
			}
		}

		public void EndEdit()
		{
			this.isEditing = false;
			this.snapshot = null;
		}

		public void RejectChanges()
		{
			this.CancelEdit();
		}

		#endregion
	}
#endif
}