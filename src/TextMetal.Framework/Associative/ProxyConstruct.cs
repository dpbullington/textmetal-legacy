﻿/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections;

using TextMetal.Framework.Core;
using TextMetal.Framework.XmlDialect;

namespace TextMetal.Framework.Associative
{
	/// <summary>
	/// Provides an XML construct for associative proxies. DO NOT SERIALIZE THIS CLASS TO AN XML STREAM.
	/// </summary>
	[XmlElementMapping(LocalName = "Proxy", NamespaceUri = "http://www.textmetal.com/api/v6.0.0", ChildElementModel = ChildElementModel.Sterile)]
	public sealed class ProxyConstruct : AssociativeXmlObject, IObjectReference
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the ProxyConstruct class.
		/// </summary>
		public ProxyConstruct(object value)
		{
			if ((object)value == null)
				throw new ArgumentNullException(nameof(value));

			this.value = value;
		}

		#endregion

		#region Fields/Constants

		private readonly object value;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// Gets or sets the proxied value.
		/// </summary>
		public object Value
		{
			get
			{
				return this.value;
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Gets the enumerator for the current associative object instance. Overrides the default behavior and always return null.
		/// </summary>
		/// <param name="templatingContext"> The templating context. </param>
		/// <returns> An instance of IEnumerator or null. </returns>
		protected override IEnumerator CoreGetAssociativeObjectEnumerator(ITemplatingContext templatingContext)
		{
			if ((object)templatingContext == null)
				throw new ArgumentNullException(nameof(templatingContext));

			return null;
		}

		/// <summary>
		/// Gets the value of the current associative object instance. Overrides the default behavior to return the Value property.
		/// </summary>
		/// <param name="templatingContext"> The templating context. </param>
		/// <returns> A value or null. </returns>
		protected override object CoreGetAssociativeObjectValue(ITemplatingContext templatingContext)
		{
			if ((object)templatingContext == null)
				throw new ArgumentNullException(nameof(templatingContext));

			return this.Value;
		}

		/// <summary>
		/// Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" /> .
		/// </summary>
		/// <returns>
		/// true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" /> ; otherwise, false.
		/// </returns>
		/// <param name="obj">
		/// The <see cref="T:System.Object" /> to compare with the current <see cref="T:System.Object" /> .
		/// </param>
		/// <filterpriority> 2 </filterpriority>
		public override bool Equals(object obj)
		{
			return PropertyConstruct.CommonEquals(this, obj);
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <returns>
		/// A hash code for the current <see cref="T:System.Object" /> .
		/// </returns>
		public override int GetHashCode()
		{
			return PropertyConstruct.CommonGetHashCode(this);
		}

		/// <summary>
		/// Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" /> .
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" /> .
		/// </returns>
		public override string ToString()
		{
			return PropertyConstruct.CommonToString(this);
		}

		#endregion
	}
}