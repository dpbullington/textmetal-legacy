/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Common.Core.Hierarchy
{
	/// <summary>
	/// Represents a hierarchical object.
	/// </summary>
	public interface IHierarchicalObject
	{
		#region Properties/Indexers/Events

		/// <summary>
		/// Gets or sets the parent hierarchical object or null if this is the hierarchy root.
		/// </summary>
		IHierarchicalObject Parent
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the surround hierarchical object or null if this is not surrounded (in a collection).
		/// </summary>
		IHierarchicalObjectCollection Surround
		{
			get;
			set;
		}

		#endregion
	}
}