/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using TextMetal.Common.Core.HierarchicalObjects;

namespace TextMetal.Common.Xml
{
	/// <summary>
	/// Represents an XML object collection.
	/// </summary>
	public interface IXmlObjectCollection : IHierarchicalObjectCollection
	{
		#region Properties/Indexers/Events

		/// <summary>
		/// Gets the site XML object or null if this is unattached.
		/// </summary>
		new IXmlObject Site
		{
			get;
		}

		#endregion
	}
}