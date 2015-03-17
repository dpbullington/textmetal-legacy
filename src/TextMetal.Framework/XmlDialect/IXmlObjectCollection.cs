/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using TextMetal.Framework.Core.ObjectTaxonomy;

namespace TextMetal.Framework.XmlDialect
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