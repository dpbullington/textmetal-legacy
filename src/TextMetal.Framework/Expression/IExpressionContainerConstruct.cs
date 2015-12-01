/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Framework.Core;
using TextMetal.Framework.XmlDialect;

namespace TextMetal.Framework.Expression
{
	[XmlKnownElementMapping(LocalName = "ExpressionContainer", NamespaceUri = "http://www.textmetal.com/api/v6.0.0", ChildElementModel = ChildElementModel.Content)]
	public interface IExpressionContainerConstruct : IContentContainerXmlObject<IExpressionXmlObject>, IExpressionXmlObject
	{
	}
}