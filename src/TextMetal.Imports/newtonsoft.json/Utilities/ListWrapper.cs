#region License

// Copyright (c) 2007 James Newton-King
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

#endregion

using System.Collections;
using System.Collections.Generic;

namespace Newtonsoft.Json.Utilities
{
	internal interface IWrappedList : IList
	{
		#region Properties/Indexers/Events

		object UnderlyingList
		{
			get;
		}

		#endregion
	}

	internal class ListWrapper<T> : CollectionWrapper<T>, IList<T>, IWrappedList
	{
		#region Constructors/Destructors

		public ListWrapper(IList list)
			: base(list)
		{
			ValidationUtils.ArgumentNotNull(list, "list");

			if (list is IList<T>)
				this._genericList = (IList<T>)list;
		}

		public ListWrapper(IList<T> list)
			: base(list)
		{
			ValidationUtils.ArgumentNotNull(list, "list");

			this._genericList = list;
		}

		#endregion

		#region Fields/Constants

		private readonly IList<T> _genericList;

		#endregion

		#region Properties/Indexers/Events

		public T this[int index]
		{
			get
			{
				if (this._genericList != null)
					return this._genericList[index];
				else
					return (T)((IList)this)[index];
			}
			set
			{
				if (this._genericList != null)
					this._genericList[index] = value;
				else
					((IList)this)[index] = value;
			}
		}

		public override int Count
		{
			get
			{
				if (this._genericList != null)
					return this._genericList.Count;
				else
					return base.Count;
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				if (this._genericList != null)
					return this._genericList.IsReadOnly;
				else
					return base.IsReadOnly;
			}
		}

		public object UnderlyingList
		{
			get
			{
				if (this._genericList != null)
					return this._genericList;
				else
					return this.UnderlyingCollection;
			}
		}

		#endregion

		#region Methods/Operators

		public override void Add(T item)
		{
			if (this._genericList != null)
				this._genericList.Add(item);
			else
				base.Add(item);
		}

		public override void Clear()
		{
			if (this._genericList != null)
				this._genericList.Clear();
			else
				base.Clear();
		}

		public override bool Contains(T item)
		{
			if (this._genericList != null)
				return this._genericList.Contains(item);
			else
				return base.Contains(item);
		}

		public override void CopyTo(T[] array, int arrayIndex)
		{
			if (this._genericList != null)
				this._genericList.CopyTo(array, arrayIndex);
			else
				base.CopyTo(array, arrayIndex);
		}

		public override IEnumerator<T> GetEnumerator()
		{
			if (this._genericList != null)
				return this._genericList.GetEnumerator();

			return base.GetEnumerator();
		}

		public int IndexOf(T item)
		{
			if (this._genericList != null)
				return this._genericList.IndexOf(item);
			else
				return ((IList)this).IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			if (this._genericList != null)
				this._genericList.Insert(index, item);
			else
				((IList)this).Insert(index, item);
		}

		public override bool Remove(T item)
		{
			if (this._genericList != null)
				return this._genericList.Remove(item);
			else
			{
				bool contains = base.Contains(item);

				if (contains)
					base.Remove(item);

				return contains;
			}
		}

		public void RemoveAt(int index)
		{
			if (this._genericList != null)
				this._genericList.RemoveAt(index);
			else
				((IList)this).RemoveAt(index);
		}

		#endregion
	}
}