/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Framework.Core.ObjectTaxonomy
{
	/// <summary>
	/// Provides a base for all hierarchical objects.
	/// </summary>
	public abstract class HierarchicalObject : IHierarchicalObject
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the HierarchicalObject class.
		/// </summary>
		protected HierarchicalObject()
		{
		}

		#endregion

		#region Fields/Constants

		private IHierarchicalObject parent;
		private IHierarchicalObjectCollection surround;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// Gets or sets the parent hierarchical object or null if this is the hierarchy root.
		/// </summary>
		public IHierarchicalObject Parent
		{
			get
			{
				return this.parent;
			}
			set
			{
				this.parent = value;
			}
		}

		/// <summary>
		/// Gets or sets the surround hierarchical object or null if this is not surrounded (in a collection).
		/// </summary>
		public IHierarchicalObjectCollection Surround
		{
			get
			{
				return this.surround;
			}
			set
			{
				this.surround = value;
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Ensures that for any hierarchical object property, the correct parent instance is set/unset.
		/// Should be called in the setter for all hierarchical object properties, before assigning the value.
		/// Example:
		/// set { this.EnsureParentOnPropertySet(this.content, value); this.content = value; }
		/// </summary>
		/// <param name="oldValueObj"> The old hierarchical object value (the backing field). </param>
		/// <param name="newValueObj"> The new hierarchical object value (value). </param>
		protected void EnsureParentOnPropertySet(IHierarchicalObject oldValueObj, IHierarchicalObject newValueObj)
		{
			if ((object)oldValueObj != null)
			{
				oldValueObj.Surround = null;
				oldValueObj.Parent = null;
			}

			if ((object)newValueObj != null)
			{
				newValueObj.Surround = null;
				newValueObj.Parent = this;
			}
		}

		#endregion
	}
}