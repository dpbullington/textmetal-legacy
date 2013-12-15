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

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Castle.Components.DictionaryAdapter
{
	using System;

	using SysPropertyDescriptor = System.ComponentModel.PropertyDescriptor;

	[DebuggerDisplay("Count = {Count}, Adapter = {Adapter}")]
	[DebuggerTypeProxy(typeof(ListProjectionDebugView<>))]
	public class ListProjection<T> :
		IBindingList<T>, // Castle
#if SILVERLIGHT
		IList,
#else
		IBindingList, // System
#endif
		IEditableObject,
		IRevertibleChangeTracking,
		ICollectionProjection,
		ICollectionAdapterObserver<T>
	{
		private readonly ICollectionAdapter<T> adapter;
		private int addNewIndex = NoIndex;
		private int addedIndex = NoIndex;
		private int suspendLevel = 0;
#if !SILVERLIGHT
		private int changedIndex = NoIndex;
		private PropertyChangedEventHandler propertyHandler;
		private static PropertyDescriptorCollection itemProperties;
#endif
		private const int NoIndex = -1;

		public ListProjection(ICollectionAdapter<T> adapter)
		{
			if (adapter == null)
				throw new ArgumentNullException("adapter");

			this.adapter = adapter;
			adapter.Initialize(this);
		}

		public int Count
		{
			get
			{
				return this.adapter.Count;
			}
		}

#if !SILVERLIGHT
		public IBindingList AsBindingList
		{
			get
			{
				return this;
			}
		}
#endif

		public ICollectionAdapter<T> Adapter
		{
			get
			{
				return this.adapter;
			}
		}

		public IEqualityComparer<T> Comparer
		{
			get
			{
				return this.adapter.Comparer ?? EqualityComparer<T>.Default;
			}
		}

		// Generic IBindingList Properties
		bool IBindingList<T>.AllowEdit
		{
			get
			{
				return true;
			}
		}

		bool IBindingList<T>.AllowNew
		{
			get
			{
				return true;
			}
		}

		bool IBindingList<T>.AllowRemove
		{
			get
			{
				return true;
			}
		}

		bool IBindingList<T>.SupportsChangeNotification
		{
			get
			{
				return true;
			}
		}

		bool IBindingList<T>.SupportsSearching
		{
			get
			{
				return false;
			}
		}

		bool IBindingList<T>.SupportsSorting
		{
			get
			{
				return false;
			}
		}

		bool IBindingList<T>.IsSorted
		{
			get
			{
				return false;
			}
		}

		SysPropertyDescriptor IBindingList<T>.SortProperty
		{
			get
			{
				return null;
			}
		}

		ListSortDirection IBindingList<T>.SortDirection
		{
			get
			{
				return ListSortDirection.Ascending;
			}
		}

#if !SILVERLIGHT
		// System IBindingList Properties
		bool IBindingList.AllowEdit
		{
			get
			{
				return true;
			}
		}

		bool IBindingList.AllowNew
		{
			get
			{
				return true;
			}
		}

		bool IBindingList.AllowRemove
		{
			get
			{
				return true;
			}
		}

		bool IBindingList.SupportsChangeNotification
		{
			get
			{
				return true;
			}
		}

		bool IBindingList.SupportsSearching
		{
			get
			{
				return false;
			}
		}

		bool IBindingList.SupportsSorting
		{
			get
			{
				return false;
			}
		}

		bool IBindingList.IsSorted
		{
			get
			{
				return false;
			}
		}

		SysPropertyDescriptor IBindingList.SortProperty
		{
			get
			{
				return null;
			}
		}

		ListSortDirection IBindingList.SortDirection
		{
			get
			{
				return ListSortDirection.Ascending;
			}
		}

		// Other Binding Properties
		bool IRaiseItemChangedEvents.RaisesItemChangedEvents
		{
			get
			{
				return true;
			}
		}
#endif

		// IList Properties
		bool IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		bool ICollection<T>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this;
			}
		}

		public virtual bool Contains(T item)
		{
			return this.IndexOf(item) >= 0;
		}

		bool IList.Contains(object item)
		{
			return this.Contains((T)item);
		}

		public int IndexOf(T item)
		{
			var count = this.Count;
			var comparer = this.Comparer;

			for (var i = 0; i < count; i++)
			{
				if (comparer.Equals(this[i], item))
					return i;
			}

			return -1;
		}

		int IList.IndexOf(object item)
		{
			return this.IndexOf((T)item);
		}

		public void CopyTo(T[] array, int index)
		{
			var count = this.Count;

			for (int i = 0, j = index; i < count; i++, j++)
				array[j] = this[i];
		}

		void ICollection.CopyTo(Array array, int index)
		{
			this.CopyTo((T[])array, index);
		}

		public IEnumerator<T> GetEnumerator()
		{
			var count = this.Count;

			for (var i = 0; i < count; i++)
				yield return this[i];
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public T this[int index]
		{
			get
			{
				return this.adapter[index];
			}
			set
			{
				this.adapter[index] = value;
			}
		}

		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				this[index] = (T)value;
			}
		}

		public void Replace(IEnumerable<T> items)
		{
			(this as ICollectionProjection).Replace(items);
		}

		void ICollectionProjection.Replace(IEnumerable items)
		{
			this.SuspendEvents();
			try
			{
				this.Clear();
				foreach (T item in items)
					this.Add(item);
			}
			finally
			{
				this.ResumeEvents();
			}
		}

		protected virtual bool OnReplacing(T oldValue, T newValue)
		{
			return true;
		}

		bool ICollectionAdapterObserver<T>.OnReplacing(T oldValue, T newValue)
		{
			return this.OnReplacing(oldValue, newValue);
		}

		protected virtual void OnReplaced(T oldValue, T newValue, int index)
		{
			this.DetachPropertyChanged(oldValue);
			this.AttachPropertyChanged(newValue);
			this.NotifyListChanged(ListChangedType.ItemChanged, index);
		}

		void ICollectionAdapterObserver<T>.OnReplaced(T oldValue, T newValue, int index)
		{
			this.OnReplaced(oldValue, newValue, index);
		}

		public virtual T AddNew()
		{
			var item = (T)this.adapter.AddNew();
			this.addNewIndex = this.addedIndex;
			return item;
		}

#if !SILVERLIGHT
		object IBindingList.AddNew()
		{
			return this.AddNew();
		}
#endif

		public bool IsNew(int index)
		{
			return index == this.addNewIndex
					&& index >= 0;
		}

		public virtual void EndNew(int index)
		{
			if (this.IsNew(index))
				this.addNewIndex = NoIndex;
		}

		public virtual void CancelNew(int index)
		{
			if (this.IsNew(index))
			{
				this.RemoveAt(this.addNewIndex);
				this.addNewIndex = NoIndex;
			}
		}

		public virtual bool Add(T item)
		{
			return this.adapter.Add(item);
		}

		void ICollection<T>.Add(T item)
		{
			this.Add(item);
		}

		int IList.Add(object item)
		{
			this.Add((T)item);
			return this.addedIndex;
		}

		public void Insert(int index, T item)
		{
			if (index < 0)
				throw new ArgumentOutOfRangeException("index");

			var count = this.Count;
			if (index > count)
				throw new ArgumentOutOfRangeException("index");

			this.EndNew(this.addNewIndex);
			if (index == count)
				this.adapter.Add(item);
			else
				this.adapter.Insert(index, item);
		}

		void IList.Insert(int index, object item)
		{
			this.Insert(index, (T)item);
		}

		protected virtual bool OnInserting(T value)
		{
			return true;
		}

		bool ICollectionAdapterObserver<T>.OnInserting(T value)
		{
			return this.OnInserting(value);
		}

		protected virtual void OnInserted(T newValue, int index)
		{
			this.addedIndex = index;
			this.AttachPropertyChanged(newValue);
			this.NotifyListChanged(ListChangedType.ItemAdded, index);
		}

		void ICollectionAdapterObserver<T>.OnInserted(T newValue, int index)
		{
			this.OnInserted(newValue, index);
		}

		public virtual bool Remove(T item)
		{
			var index = this.IndexOf(item);
			if (index < 0)
				return false;
			this.RemoveAt(index);
			return true;
		}

		void IList.Remove(object value)
		{
			this.Remove((T)value);
		}

		public virtual void RemoveAt(int index)
		{
			this.EndNew(this.addNewIndex);
			this.adapter.Remove(index);
		}

		public virtual void Clear()
		{
			this.EndNew(this.addNewIndex);
			this.adapter.Clear();
			this.NotifyListReset();
		}

		void ICollectionProjection.ClearReferences()
		{
			this.adapter.ClearReferences();
		}

		protected virtual void OnRemoving(T oldValue)
		{
			this.DetachPropertyChanged(oldValue);
		}

		void ICollectionAdapterObserver<T>.OnRemoving(T oldValue)
		{
			this.OnRemoving(oldValue);
		}

		protected virtual void OnRemoved(T oldValue, int index)
		{
			this.NotifyListChanged(ListChangedType.ItemDeleted, index);
		}

		void ICollectionAdapterObserver<T>.OnRemoved(T oldValue, int index)
		{
			this.OnRemoved(oldValue, index);
		}

		public bool IsChanged
		{
			get
			{
				if (this.adapter.HasSnapshot == false)
					return false;

				var count = this.Count;
				if (this.adapter.SnapshotCount != count)
					return true;

				var comparer = this.Comparer;
				for (var i = 0; i < count; i++)
				{
					var currentItem = this.adapter.GetCurrentItem(i);
					var snapshotItem = this.adapter.GetSnapshotItem(i);

					if (comparer.Equals(currentItem, snapshotItem) == false)
						return true;

					var tracked = currentItem as IChangeTracking;
					if (tracked != null && tracked.IsChanged)
						return true;
				}

				return false;
			}
		}

		public void BeginEdit()
		{
			if (!this.adapter.HasSnapshot)
				this.adapter.SaveSnapshot();
		}

		public void EndEdit()
		{
			this.adapter.DropSnapshot();
		}

		public void CancelEdit()
		{
			if (this.adapter.HasSnapshot)
			{
				this.adapter.LoadSnapshot();
				this.adapter.DropSnapshot();
				this.NotifyListReset();
			}
		}

		public void AcceptChanges()
		{
			this.BeginEdit();
		}

		public void RejectChanges()
		{
			this.CancelEdit();
		}

#if SILVERLIGHT
		[Conditional("NOP")]
		private void AttachPropertyChanged(T value) { }

		[Conditional("NOP")]
		private void DetachPropertyChanged(T value) { }
#else
		private void AttachPropertyChanged(T value)
		{
			if (typeof(T).IsValueType)
				return;

			var notifier = value as INotifyPropertyChanged;
			if (notifier == null)
				return;

			if (this.propertyHandler == null)
				this.propertyHandler = this.HandlePropertyChanged;

			notifier.PropertyChanged += this.propertyHandler;
		}

		private void DetachPropertyChanged(T value)
		{
			if (typeof(T).IsValueType)
				return;

			var notifier = value as INotifyPropertyChanged;
			if (notifier == null || this.propertyHandler == null)
				return;

			notifier.PropertyChanged -= this.propertyHandler;
		}

		private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			T item;
			var notify
				= this.EventsEnabled
				&& this.CanHandle(sender, e)
				&& this.TryGetChangedItem(sender, out item)
				&& this.TryGetChangedIndex(item);

			if (notify)
			{
				var property = GetChangedProperty(e);
				var change = new ListChangedEventArgs(ListChangedType.ItemChanged, this.changedIndex, property);
				this.OnListChanged(change);
			}
		}

		private bool CanHandle(object sender, PropertyChangedEventArgs e)
		{
			if (sender == null || e == null || string.IsNullOrEmpty(e.PropertyName))
			{
				this.NotifyListReset();
				return false;
			}
			return true;
		}

		private bool TryGetChangedItem(object sender, out T item)
		{
			try
			{
				item = (T)sender;
				return true;
			}
			catch (InvalidCastException)
			{
				this.NotifyListReset();
				item = default(T);
				return false;
			}
		}

		private bool TryGetChangedIndex(T item)
		{
			var isSameItem
				= this.changedIndex >= 0
				&& this.changedIndex < this.Count
				&& this.Comparer.Equals(this[this.changedIndex], item);
			if (isSameItem)
				return true;

			this.changedIndex = this.IndexOf(item);
			if (this.changedIndex >= 0)
				return true;

			this.DetachPropertyChanged(item);
			this.NotifyListReset();
			return false;
		}

		private static SysPropertyDescriptor GetChangedProperty(PropertyChangedEventArgs e)
		{
			if (itemProperties == null)
				itemProperties = TypeDescriptor.GetProperties(typeof(T));

			return itemProperties.Find(e.PropertyName, true);
		}
#endif

#if !SILVERLIGHT
		public event ListChangedEventHandler ListChanged;

		protected virtual void OnListChanged(ListChangedEventArgs args)
		{
			var handler = this.ListChanged;
			if (handler != null)
				handler(this, args);
		}
#endif

#if SILVERLIGHT
		protected enum ListChangedType
		{
			ItemAdded,
			ItemChanged,
			ItemDeleted
		}

		[Conditional("NOP")]
		protected void NotifyListChanged(ListChangedType type, int index) { }

		[Conditional("NOP")]
		protected void NotifyListReset() { }
#else
		protected void NotifyListChanged(ListChangedType type, int index)
		{
			if (this.EventsEnabled)
				this.OnListChanged(new ListChangedEventArgs(type, index));
		}

		protected void NotifyListReset()
		{
			if (this.EventsEnabled)
				this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
		}
#endif

		public bool EventsEnabled
		{
			get
			{
				return this.suspendLevel == 0;
			}
		}

		public void SuspendEvents()
		{
			this.suspendLevel++;
		}

		public bool ResumeEvents()
		{
			var enabled
				= this.suspendLevel == 0
				|| --this.suspendLevel == 0;

			if (enabled)
				this.NotifyListReset();

			return enabled;
		}

		void IBindingList<T>.AddIndex(SysPropertyDescriptor property)
		{
			// Do nothing
		}

#if !SILVERLIGHT
		void IBindingList.AddIndex(SysPropertyDescriptor property)
		{
			// Do nothing
		}
#endif

		void IBindingList<T>.RemoveIndex(SysPropertyDescriptor property)
		{
			// Do nothing
		}

#if !SILVERLIGHT
		void IBindingList.RemoveIndex(SysPropertyDescriptor property)
		{
			// Do nothing
		}
#endif

		int IBindingList<T>.Find(SysPropertyDescriptor property, object key)
		{
			throw new NotSupportedException();
		}

#if !SILVERLIGHT
		int IBindingList.Find(SysPropertyDescriptor property, object key)
		{
			throw new NotSupportedException();
		}
#endif

		void IBindingList<T>.ApplySort(SysPropertyDescriptor property, ListSortDirection direction)
		{
			throw new NotSupportedException();
		}

#if !SILVERLIGHT
		void IBindingList.ApplySort(SysPropertyDescriptor property, ListSortDirection direction)
		{
			throw new NotSupportedException();
		}
#endif

		void IBindingList<T>.RemoveSort()
		{
			throw new NotSupportedException();
		}

#if !SILVERLIGHT
		void IBindingList.RemoveSort()
		{
			throw new NotSupportedException();
		}
#endif
	}
}