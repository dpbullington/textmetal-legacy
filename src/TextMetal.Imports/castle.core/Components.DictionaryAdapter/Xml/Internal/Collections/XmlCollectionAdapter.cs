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

#if !SILVERLIGHT && !MONO // Until support for other platforms is verified

namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;

	internal class XmlCollectionAdapter<T> : ICollectionAdapter<T>, IXmlNodeSource
	{
		#region Constructors/Destructors

		public XmlCollectionAdapter(
			IXmlNode parentNode,
			IDictionaryAdapter parentObject,
			IXmlCollectionAccessor accessor)
		{
			this.items = new List<XmlCollectionItem<T>>();

			this.accessor = accessor;
			this.cursor = accessor.SelectCollectionItems(parentNode, true);
			this.parentNode = parentNode;
			this.parentObject = parentObject;
			this.references = XmlAdapter.For(parentObject).References;

			while (this.cursor.MoveNext())
				this.items.Add(new XmlCollectionItem<T>(this.cursor.Save()));
		}

		#endregion

		#region Fields/Constants

		private readonly IXmlCollectionAccessor accessor;
		private readonly IXmlCursor cursor;
		private readonly IXmlNode parentNode;
		private readonly IDictionaryAdapter parentObject;
		private readonly XmlReferenceManager references;
		private ICollectionAdapterObserver<T> advisor;
		private List<XmlCollectionItem<T>> items;
		private List<XmlCollectionItem<T>> snapshot;

		#endregion

		#region Properties/Indexers/Events

		public T this[int index]
		{
			get
			{
				var item = this.items[index];

				if (!item.HasValue)
					this.items[index] = item = item.WithValue(this.GetValue(item.Node));

				return item.Value;
			}
			set
			{
				var item = this.items[index];
				this.cursor.MoveTo(item.Node);
				this.SetValue(this.cursor, item.Value, ref value);

				if (this.advisor.OnReplacing(item.Value, value))
				{
					// Commit the replacement
					this.items[index] = item.WithValue(value);
					this.advisor.OnReplaced(item.Value, value, index);
				}
				else
				{
					// Rollback the replacement
					var oldValue = item.Value;
					this.SetValue(this.cursor, value, ref oldValue);
					this.items[index] = item.WithValue(oldValue);
				}
			}
		}

		public IEqualityComparer<T> Comparer
		{
			get
			{
				return null;
			}
		}

		public int Count
		{
			get
			{
				return this.items.Count;
			}
		}

		public bool HasSnapshot
		{
			get
			{
				return this.snapshot != null;
			}
		}

		public IXmlNode Node
		{
			get
			{
				return this.parentNode;
			}
		}

		public XmlReferenceManager References
		{
			get
			{
				return this.references;
			}
		}

		public int SnapshotCount
		{
			get
			{
				return this.snapshot.Count;
			}
		}

		#endregion

		#region Methods/Operators

		private static Type GetTypeOrDefault(T value)
		{
			return (null == value)
				? typeof(T)
				: value.GetComponentType();
		}

		public bool Add(T value)
		{
			return this.InsertCore(this.Count, value, append: true);
		}

		public T AddNew()
		{
			this.cursor.MoveToEnd();
			this.cursor.Create(typeof(T));

			var node = this.cursor.Save();
			var value = this.GetValue(node);
			var index = this.items.Count;

			this.CommitInsert(index, node, value, true);
			return (T)value;
		}

		public void Clear()
		{
			foreach (var item in this.items)
				this.OnRemoving(item);

			this.cursor.Reset();
			this.cursor.RemoveAllNext();
			this.items.Clear();

			// Don't call OnRemoved. Caller is already going to fire a Reset shortly.
		}

		public void ClearReferences()
		{
			if (this.accessor.IsReference)
			{
				foreach (var item in this.items)
					this.references.OnAssigningNull(item.Node, item.Value);
			}
		}

		private bool CommitInsert(int index, IXmlNode node, T value, bool append)
		{
			var item = new XmlCollectionItem<T>(node, value);

			if (append)
				this.items.Add(item);
			else
				this.items.Insert(index, item);

			this.advisor.OnInserted(value, index);
			return true;
		}

		public void DropSnapshot()
		{
			this.snapshot = null;
		}

		public T GetCurrentItem(int index)
		{
			return this.items[index].Value;
		}

		public T GetSnapshotItem(int index)
		{
			return this.snapshot[index].Value;
		}

		private T GetValue(IXmlNode node)
		{
			return (T)(this.accessor.GetValue(node, this.parentObject, this.references, true, true) ?? default(T));
		}

		public void Initialize(ICollectionAdapterObserver<T> advisor)
		{
			this.advisor = advisor;
		}

		public bool Insert(int index, T value)
		{
			return this.InsertCore(index, value, append: false);
		}

		private bool InsertCore(int index, T value, bool append)
		{
			if (append)
				this.cursor.MoveToEnd();
			else
				this.cursor.MoveTo(this.items[index].Node);

			this.cursor.Create(GetTypeOrDefault(value));
			var node = this.cursor.Save();
			this.SetValue(this.cursor, default(T), ref value);

			return this.advisor.OnInserting(value)
				? this.CommitInsert(index, node, value, append)
				: this.RollbackInsert();
		}

		public void LoadSnapshot()
		{
			this.items = this.snapshot;
		}

		private void OnRemoving(XmlCollectionItem<T> item)
		{
			this.advisor.OnRemoving(item.Value);

			if (this.accessor.IsReference)
				this.references.OnAssigningNull(item.Node, item.Value);
		}

		public void Remove(int index)
		{
			var item = this.items[index];
			this.OnRemoving(item);

			this.cursor.MoveTo(item.Node);
			this.cursor.Remove();
			this.items.RemoveAt(index);

			this.advisor.OnRemoved(item.Value, index);
		}

		private bool RollbackInsert()
		{
			this.cursor.Remove();
			return false;
		}

		public void SaveSnapshot()
		{
			this.snapshot = new List<XmlCollectionItem<T>>(this.items);
		}

		private void SetValue(IXmlCursor cursor, object oldValue, ref T value)
		{
			object obj = value;
			this.accessor.SetValue(cursor, this.parentObject, this.references, true, oldValue, ref obj);
			value = (T)(obj ?? default(T));
		}

		#endregion
	}
}

#endif