/*
	Copyright �2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.ObjectModel;

namespace TextMetal.Framework.XmlDialect
{
	/// <summary>
	/// Provides a concrete implementation for XML object collections.
	/// </summary>
	/// <typeparam name="TXmlObject"> </typeparam>
	public class XmlObjectCollection<TXmlObject> : Collection<TXmlObject>, IXmlObjectCollection<TXmlObject>, IXmlObjectCollection
		where TXmlObject : IXmlObject
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the XmlObjectCollection class.
		/// </summary>
		/// <param name="site"> The containing site XML object. </param>
		public XmlObjectCollection(IXmlObject site)
		{
			if ((object)site == null)
				throw new ArgumentNullException(nameof(site));

			this.site = site;
		}

		#endregion

		#region Fields/Constants

		private readonly IXmlObject site;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// Gets the site XML object or null if this is unattached.
		/// </summary>
		public IXmlObject Site
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
			foreach (TXmlObject item in this.Items)
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
		protected override void InsertItem(int index, TXmlObject item)
		{
			if ((object)item == null)
				throw new ArgumentNullException(nameof(item));

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
			TXmlObject item;

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
		protected override void SetItem(int index, TXmlObject item)
		{
			if ((object)item == null)
				throw new ArgumentNullException(nameof(item));

			item.Surround = this;
			item.Parent = this.Site;

			base.SetItem(index, item);
		}

		#endregion
	}
}