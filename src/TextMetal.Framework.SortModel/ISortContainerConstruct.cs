/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using TextMetal.Common.Xml;
using TextMetal.Framework.Core;

namespace TextMetal.Framework.SortModel
{
	[XmlKnownElementMapping(LocalName = "SortContainer", NamespaceUri = "http://www.textmetal.com/api/v6.0.0", ChildElementModel = ChildElementModel.Items)]
	public interface ISortContainerConstruct : IItemsContainerXmlObject<ISortXmlObject>, ISortXmlObject
	{
	}
}