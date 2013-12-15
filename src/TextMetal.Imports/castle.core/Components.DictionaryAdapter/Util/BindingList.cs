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

using System.Collections;
using System.Collections.Generic;

#if !SILVERLIGHT && !MONO // Until support for other platforms is verified

namespace Castle.Components.DictionaryAdapter
{
	using System;

	using SCM = System.ComponentModel;

	/// <summary>
	/// Provides a generic collection that supports data binding.
	/// </summary>
	/// <remarks>
	/// This class wraps the CLR <see cref="System.ComponentModel.BindingList{T}" />
	/// in order to implement the Castle-specific <see cref="IBindingList{T}" />.
	/// </remarks>
	/// <typeparam name="T"> The type of elements in the list. </typeparam>
	public class BindingList<T> : IBindingList<T>, IList
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="BindingList{T}" /> class
		/// using default values.
		/// </summary>
		public BindingList()
		{
			this.list = new SCM.BindingList<T>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BindingList{T}" /> class
		/// with the specified list.
		/// </summary>
		/// <param name="list">
		/// An <see cref="IList{T}" /> of items
		/// to be contained in the <see cref="BindingList{T}" />.
		/// </param>
		public BindingList(IList<T> list)
		{
			this.list = new SCM.BindingList<T>(list);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BindingList{T}" /> class
		/// wrapping the specified <see cref="System.ComponentModel.BindingList{T}" /> instance.
		/// </summary>
		/// <param name="list">
		/// A <see cref="System.ComponentModel.BindingList{T}" />
		/// to be wrapped by the <see cref="BindingList{T}" />.
		/// </param>
		public BindingList(SCM.BindingList<T> list)
		{
			if (list == null)
				throw new ArgumentNullException("list");

			this.list = list;
		}

		#endregion

		#region Fields/Constants

		private readonly SCM.BindingList<T> list;

		#endregion

		#region Properties/Indexers/Events

		public event SCM.AddingNewEventHandler AddingNew
		{
			add
			{
				this.list.AddingNew += value;
			}
			remove
			{
				this.list.AddingNew -= value;
			}
		}

		public event SCM.ListChangedEventHandler ListChanged
		{
			add
			{
				this.list.ListChanged += value;
			}
			remove
			{
				this.list.ListChanged -= value;
			}
		}

		public T this[int index]
		{
			get
			{
				return this.list[index];
			}
			set
			{
				this.list[index] = value;
			}
		}

		object IList.this[int index]
		{
			get
			{
				return ((IList)this.list)[index];
			}
			set
			{
				((IList)this.list)[index] = value;
			}
		}

		public bool AllowEdit
		{
			get
			{
				return this.list.AllowEdit;
			}
			set
			{
				this.list.AllowEdit = value;
			}
		}

		public bool AllowNew
		{
			get
			{
				return this.list.AllowNew;
			}
			set
			{
				this.list.AllowNew = value;
			}
		}

		public bool AllowRemove
		{
			get
			{
				return this.list.AllowRemove;
			}
			set
			{
				this.list.AllowRemove = value;
			}
		}

		public SCM.IBindingList AsBindingList
		{
			get
			{
				return this.list;
			}
		}

		public int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		public SCM.BindingList<T> InnerList
		{
			get
			{
				return this.list;
			}
		}

		bool IList.IsFixedSize
		{
			get
			{
				return ((IList)this.list).IsFixedSize;
			}
		}

		bool ICollection<T>.IsReadOnly
		{
			get
			{
				return ((ICollection<T>)this.list).IsReadOnly;
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				return ((IList)this.list).IsReadOnly;
			}
		}

		bool IBindingList<T>.IsSorted
		{
			get
			{
				return this.AsBindingList.IsSorted;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return ((ICollection)this.list).IsSynchronized;
			}
		}

		public bool RaiseListChangedEvents
		{
			get
			{
				return this.list.RaiseListChangedEvents;
			}
			set
			{
				this.list.RaiseListChangedEvents = value;
			}
		}

		bool SCM.IRaiseItemChangedEvents.RaisesItemChangedEvents
		{
			get
			{
				return ((SCM.IRaiseItemChangedEvents)this.list).RaisesItemChangedEvents;
			}
		}

		SCM.ListSortDirection IBindingList<T>.SortDirection
		{
			get
			{
				return this.AsBindingList.SortDirection;
			}
		}

		SCM.PropertyDescriptor IBindingList<T>.SortProperty
		{
			get
			{
				return this.AsBindingList.SortProperty;
			}
		}

		bool IBindingList<T>.SupportsChangeNotification
		{
			get
			{
				return this.AsBindingList.SupportsChangeNotification;
			}
		}

		bool IBindingList<T>.SupportsSearching
		{
			get
			{
				return this.AsBindingList.SupportsSearching;
			}
		}

		bool IBindingList<T>.SupportsSorting
		{
			get
			{
				return this.AsBindingList.SupportsSorting;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return ((ICollection)this.list).SyncRoot;
			}
		}

		#endregion

		#region Methods/Operators

		public void Add(T item)
		{
			this.list.Add(item);
		}

		int IList.Add(object item)
		{
			return ((IList)this.list).Add(item);
		}

		void IBindingList<T>.AddIndex(SCM.PropertyDescriptor property)
		{
			this.AsBindingList.AddIndex(property);
		}

		public T AddNew()
		{
			return this.list.AddNew();
		}

		void IBindingList<T>.ApplySort(SCM.PropertyDescriptor property, SCM.ListSortDirection direction)
		{
			this.AsBindingList.ApplySort(property, direction);
		}

		public void CancelNew(int index)
		{
			this.list.CancelNew(index);
		}

		public void Clear()
		{
			this.list.Clear();
		}

		public bool Contains(T item)
		{
			return this.list.Contains(item);
		}

		bool IList.Contains(object value)
		{
			return ((IList)this.list).Contains(value);
		}

		public void CopyTo(T[] array, int index)
		{
			this.list.CopyTo(array, index);
		}

		void ICollection.CopyTo(Array array, int index)
		{
			((IList)this.list).CopyTo(array, index);
		}

		public void EndNew(int index)
		{
			this.list.EndNew(index);
		}

		int IBindingList<T>.Find(SCM.PropertyDescriptor property, object key)
		{
			return this.AsBindingList.Find(property, key);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		public int IndexOf(T item)
		{
			return this.list.IndexOf(item);
		}

		int IList.IndexOf(object value)
		{
			return ((IList)this.list).IndexOf(value);
		}

		public void Insert(int index, T item)
		{
			this.list.Insert(index, item);
		}

		void IList.Insert(int index, object item)
		{
			((IList)this.list).Insert(index, item);
		}

		public bool Remove(T item)
		{
			return this.list.Remove(item);
		}

		void IList.Remove(object item)
		{
			((IList)this.list).Remove(item);
		}

		public void RemoveAt(int index)
		{
			this.list.RemoveAt(index);
		}

		void IBindingList<T>.RemoveIndex(SCM.PropertyDescriptor property)
		{
			this.AsBindingList.RemoveIndex(property);
		}

		void IBindingList<T>.RemoveSort()
		{
			this.AsBindingList.RemoveSort();
		}

		public void ResetBindings()
		{
			this.list.ResetBindings();
		}

		public void ResetItem(int index)
		{
			this.list.ResetItem(index);
		}

		#endregion
	}
}

#endif