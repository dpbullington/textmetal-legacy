/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Common.Expressions;
using TextMetal.Common.Xml;

namespace TextMetal.Framework.Core
{
	/// <summary>
	/// Represents a sort XML object.
	/// </summary>
	public interface ISortXmlObject : ISortMechanism, IXmlObject, ISequence
	{
	}
}