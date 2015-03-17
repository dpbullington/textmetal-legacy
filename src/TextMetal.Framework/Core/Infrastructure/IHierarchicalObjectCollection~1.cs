/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

namespace TextMetal.Framework.Core.ObjectTaxonomy
{
	/// <summary>
	/// Represents a hierarchical object collection.
	/// </summary>
	/// <typeparam name="THierarchicalObject"> </typeparam>
	public interface IHierarchicalObjectCollection<THierarchicalObject> : IHierarchicalObjectCollection, IList<THierarchicalObject>
		where THierarchicalObject : IHierarchicalObject
	{
	}
}