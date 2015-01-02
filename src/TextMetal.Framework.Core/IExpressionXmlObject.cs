/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Common.Syntax.Expressions;
using TextMetal.Common.Xml;

namespace TextMetal.Framework.Core
{
	/// <summary>
	/// Represents an expression XML object.
	/// </summary>
	public interface IExpressionXmlObject : IExpressionMechanism, IXmlObject, IExpression
	{
	}
}