/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.ObjectModel;

namespace TextMetal.Common.Core.Hierarchy
{
	/// <summary>
	/// Provides a concrete implementation for hierarchical object collections.
	/// </summary>
	/// <typeparam name="THierarchicalObject"> </typeparam>
	public class HierarchicalObjectCollection<THierarchicalObject> : Collection<THierarchicalObject>, IHierarchicalObjectCollection<THierarchicalObject>
		where THierarchicalObject : IHierarchicalObject
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the HierarchicalObjectCollection class.
		/// </summary>
		/// <param name="site"> The containing site hierarchical object. </param>
		public HierarchicalObjectCollection(IHierarchicalObject site)
		{
			if ((object)site == null)
				throw new ArgumentNullException("site");

			this.site = site;
		}

		#endregion

		#region Fields/Constants

		private readonly IHierarchicalObject site;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// Gets the site hierarchical object or null if this is unattached.
		/// </summary>
		public IHierarchicalObject Site
		{
			get
			{
				return this.site;
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Removes all elements from the collection.
		/// </summary>
		protected override void ClearItems()
		{
			foreach (THierarchicalObject item in base.Items)
			{
				item.Surround = null;
				item.Parent = null;
			}

			base.ClearItems();
		}

		/// <summary>
		/// Inserts an element into the collection at the specified index.
		/// </summary>
		/// <param name="index"> The zero-based index at which item should be inserted. </param>
		/// <param name="item"> The object to insert. The value can be null for reference types. </param>
		protected override void InsertItem(int index, THierarchicalObject item)
		{
			if ((object)item == null)
				throw new ArgumentNullException("item");

			item.Surround = this;
			item.Parent = this.Site;

			base.InsertItem(index, item);
		}

		/// <summary>
		/// Removes the element at the specified index of the collection.
		/// </summary>
		/// <param name="index"> The zero-based index of the element to remove. </param>
		protected override void RemoveItem(int index)
		{
			THierarchicalObject item;

			item = base[index];

			if ((object)item == null)
			{
				item.Surround = null;
				item.Parent = null;
			}

			base.RemoveItem(index);
		}

		/// <summary>
		/// Replaces the element at the specified index.
		/// </summary>
		/// <param name="index"> The zero-based index of the element to replace. </param>
		/// <param name="item"> The new value for the element at the specified index. The value can be null for reference types. </param>
		protected override void SetItem(int index, THierarchicalObject item)
		{
			if ((object)item == null)
				throw new ArgumentNullException("item");

			item.Surround = this;
			item.Parent = this.Site;

			base.SetItem(index, item);
		}

		#endregion
	}
}