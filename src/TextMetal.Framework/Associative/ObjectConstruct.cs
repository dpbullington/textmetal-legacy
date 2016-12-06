/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Framework.XmlDialect;

namespace TextMetal.Framework.Associative
{
	/// <summary>
	/// Provides an XML construct for associative objects (not a base class however).
	/// </summary>
	[XmlElementMapping(LocalName = "Object", NamespaceUri = "http://www.textmetal.com/api/v6.0.0", ChildElementModel = ChildElementModel.Items)]
	public class ObjectConstruct : AssociativeXmlObject, IObjectReference
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the ObjectConstruct class.
		/// </summary>
		public ObjectConstruct()
		{
		}

		#endregion
	}
}