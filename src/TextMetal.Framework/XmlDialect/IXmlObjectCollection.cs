/*
	Copyright �2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System.Collections;

namespace TextMetal.Framework.XmlDialect
{
	/// <summary>
	/// Represents an XML object collection.
	/// </summary>
	public interface IXmlObjectCollection : IList
	{
		#region Properties/Indexers/Events

		/// <summary>
		/// Gets the site XML object or null if this is unattached.
		/// </summary>
		IXmlObject Site
		{
			get;
		}

		#endregion
	}
}