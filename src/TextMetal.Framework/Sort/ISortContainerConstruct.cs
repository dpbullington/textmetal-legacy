/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using TextMetal.Framework.Core;
using TextMetal.Framework.XmlDialect;

namespace TextMetal.Framework.Sort
{
	[XmlKnownElementMapping(LocalName = "SortContainer", NamespaceUri = "http://www.textmetal.com/api/v6.0.0", ChildElementModel = ChildElementModel.Items)]
	public interface ISortContainerConstruct : IItemsContainerXmlObject<ISortXmlObject>, ISortXmlObject
	{
	}
}