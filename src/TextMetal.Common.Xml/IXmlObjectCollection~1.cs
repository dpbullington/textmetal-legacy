/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using TextMetal.Common.Core.Hierarchy;

namespace TextMetal.Common.Xml
{
	/// <summary>
	/// Represents an XML object collection.
	/// </summary>
	/// <typeparam name="TXmlObject"> </typeparam>
	public interface IXmlObjectCollection<TXmlObject> : IHierarchicalObjectCollection<TXmlObject>, IXmlObjectCollection
		where TXmlObject : IXmlObject
	{
	}
}