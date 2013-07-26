// ****************************************************************
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Collections;
using System.Collections.Generic;

namespace NUnit.UiException
{
	/// <summary>
	/// 	(formerly named ExceptionItemCollection)
	/// 
	/// 	Manages an ordered set of ErrorItem.
	/// </summary>
	public class ErrorItemCollection :
		IEnumerable
	{
		#region Constructors/Destructors

		/// <summary>
		/// 	Build a new ErrorItemCollection.
		/// </summary>
		public ErrorItemCollection()
		{
			this._items = new List<ErrorItem>();

			return;
		}

		#endregion

		#region Fields/Constants

		/// <summary>
		/// 	The underlying item list.
		/// </summary>
		private List<ErrorItem> _items;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// 	Gets the ErrorItem at the specified index.
		/// </summary>
		/// <param name="index"> The index of the wanted ErrorItem. </param>
		/// <returns> The ErrorItem. </returns>
		public ErrorItem this[int index]
		{
			get
			{
				return (this._items[index]);
			}
		}

		/// <summary>
		/// 	Gets the number of item in this collection.
		/// </summary>
		public int Count
		{
			get
			{
				return (this._items.Count);
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// 	Appends the given item to the end of the collection.
		/// </summary>
		/// <param name="item"> The ErrorItem to be added to the collection. </param>
		public void Add(ErrorItem item)
		{
			UiExceptionHelper.CheckNotNull(item, "item");
			this._items.Add(item);

			return;
		}

		/// <summary>
		/// 	Clears all items from this collection.
		/// </summary>
		public void Clear()
		{
			if (this._items.Count == 0)
				return;

			this._items.Clear();

			return;
		}

		/// <summary>
		/// 	Checks whether the given item belongs to this collection.
		/// </summary>
		/// <param name="item"> The item to be checked. </param>
		/// <returns> True if the item belongs to this collection. </returns>
		public bool Contains(ErrorItem item)
		{
			return (this._items.Contains(item));
		}

		/// <summary>
		/// 	Gets an IEnumerator able to iterate through all ExceptionItems
		/// 	managed by this collection.
		/// </summary>
		/// <returns> An iterator to be used to iterator through all items in this collection. </returns>
		public IEnumerator GetEnumerator()
		{
			return (this._items.GetEnumerator());
		}

		/// <summary>
		/// 	Reverses the sequence order of this collection.
		/// </summary>
		public void Reverse()
		{
			this._items.Reverse();
		}

		#endregion
	}
}