/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System.Collections.Generic;

namespace TextMetal.Framework.XmlDialect
{
	/// <summary>
	/// Represents an XML object collection.
	/// NOTE: This interface is invariant due to its use of IList`1,
	/// thus it should NOT derive/implement the non-generic version.
	/// This will be left to the class implementation for which to solve.
	/// </summary>
	/// <typeparam name="TXmlObject"> </typeparam>
	public interface IXmlObjectCollection<TXmlObject> : IList<TXmlObject>
		where TXmlObject : IXmlObject
	{
	}
}