/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Common.Xml;
using TextMetal.Framework.Core;

namespace TextMetal.Framework.AssociativeModel
{
	[XmlKnownElementMapping(LocalName = "AssociativeContainer", NamespaceUri = "http://www.textmetal.com/api/v5.0.0", ChildElementModel = ChildElementModel.Content)]
	public interface IAssociativeContainerConstruct : IContentContainerXmlObject<IAssociativeXmlObject>, IAssociativeXmlObject
	{
	}
}