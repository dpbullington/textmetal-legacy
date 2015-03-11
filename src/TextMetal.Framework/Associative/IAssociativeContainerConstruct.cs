/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Framework.Core;
using TextMetal.Framework.XmlDialect;

namespace TextMetal.Framework.Associative
{
	[XmlKnownElementMapping(LocalName = "AssociativeContainer", NamespaceUri = "http://www.textmetal.com/api/v6.0.0", ChildElementModel = ChildElementModel.Content)]
	public interface IAssociativeContainerConstruct : IContentContainerXmlObject<IAssociativeXmlObject>, IAssociativeXmlObject
	{
	}
}