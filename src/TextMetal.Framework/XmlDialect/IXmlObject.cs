﻿/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Framework.XmlDialect
{
	/// <summary>
	/// Represents an XML object and it's "schema".
	/// </summary>
	public interface IXmlObject
	{
		#region Properties/Indexers/Events

		/// <summary>
		/// Gets an array of allowed child XML object types.
		/// </summary>
		Type[] AllowedChildTypes
		{
			get;
		}

		/// <summary>
		/// Gets a list of XML object items.
		/// </summary>
		IXmlObjectCollection<IXmlObject> Items
		{
			get;
		}

		/// <summary>
		/// Gets or sets the optional single XML object content.
		/// </summary>
		IXmlObject Content
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the parent XML object or null if this is the document root.
		/// </summary>
		IXmlObject Parent
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the surround XML object or null if this is not surrounded (in a collection).
		/// </summary>
		IXmlObjectCollection Surround
		{
			get;
			set;
		}

		#endregion
	}
}