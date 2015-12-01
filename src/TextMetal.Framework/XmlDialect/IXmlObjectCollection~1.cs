/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System.Collections.Generic;

namespace TextMetal.Framework.XmlDialect
{
	/// <summary>
	/// Represents an XML object collection.
	/// </summary>
	/// <typeparam name="TXmlObject"> </typeparam>
	public interface IXmlObjectCollection<TXmlObject> : IList<TXmlObject>, IXmlObjectCollection
		where TXmlObject : IXmlObject
	{
	}
}