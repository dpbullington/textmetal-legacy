﻿/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections;

using TextMetal.Framework.Core;
using TextMetal.Framework.XmlDialect;
using TextMetal.Middleware.Solder.Extensions;

namespace TextMetal.Framework.Associative
{
	/// <summary>
	/// Provides an XML construct for associative properties.
	/// </summary>
	[XmlElementMapping(LocalName = "Property", NamespaceUri = "http://www.textmetal.com/api/v6.0.0", ChildElementModel = ChildElementModel.Sterile)]
	public sealed class PropertyConstruct : AssociativeXmlObject
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the PropertyConstruct class.
		/// </summary>
		public PropertyConstruct()
		{
		}

		#endregion

		#region Fields/Constants

		private string type;
		private string value;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// Gets an object representation of the value of this associative XML object value.
		/// </summary>
		public object RawValue
		{
			get
			{
				return this.GetAssociativeObjectValue(NullTemplatingContext.Instance);
			}
			set
			{
				this.Type = (value ?? string.Empty).GetType().FullName;
				this.Value = value.SafeToString();
			}
		}

		/// <summary>
		/// Gts or sets the assembly qualified type of the value, used during strongly-typed parsing.
		/// </summary>
		[XmlAttributeMapping(LocalName = "type", NamespaceUri = "")]
		public string Type
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
			}
		}

		/// <summary>
		/// Gets a string representation of the value of this associative XML object value.
		/// </summary>
		[XmlAttributeMapping(LocalName = "value", NamespaceUri = "")]
		public string Value
		{
			get
			{
				return this.value;
			}
			set
			{
				this.value = value;
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Determines whether a left-hand-side associative XML object is equal to any right-hand-side object using value semantics (instead of reference semantics).
		/// </summary>
		/// <param name="leftAssociativeXmlObject"> The left-hand-side associative XML object to test. </param>
		/// <param name="rightObject"> The right-hand-side object to test. </param>
		/// <returns> A value indicating whether the two objects equate using value semantics. </returns>
		public static bool CommonEquals(IAssociativeXmlObject leftAssociativeXmlObject, object rightObject)
		{
			object thisValue, thatValue;
			PropertyConstruct propertyConstruct;

			if ((object)leftAssociativeXmlObject == null)
				throw new ArgumentNullException(nameof(leftAssociativeXmlObject));

			thisValue = leftAssociativeXmlObject.GetAssociativeObjectValue(NullTemplatingContext.Instance);

			if ((object)rightObject == null)
				return (object)thisValue == null;

			if ((propertyConstruct = rightObject as PropertyConstruct) != null)
				thatValue = propertyConstruct.GetAssociativeObjectValue(NullTemplatingContext.Instance);
			else
				thatValue = rightObject;

			return SolderFascadeAccessor.DataTypeFascade.ObjectsEqualValueSemantics(thisValue, thatValue);
		}

		/// <summary>
		/// Determines the hash code for an associative XML object.
		/// </summary>
		/// <param name="associativeXmlObject"> The associative XML object used to get a hash code. </param>
		/// <returns> The hash code of the associative XML object's value. </returns>
		public static int CommonGetHashCode(IAssociativeXmlObject associativeXmlObject)
		{
			object value;

			if ((object)associativeXmlObject == null)
				throw new ArgumentNullException(nameof(associativeXmlObject));

			value = associativeXmlObject.GetAssociativeObjectValue(NullTemplatingContext.Instance);

			if ((object)value == null)
				return 0;

			return value.GetHashCode();
		}

		/// <summary>
		/// Gets a string representation for an associative XML object value.
		/// </summary>
		/// <param name="associativeXmlObject"> The associative XML object used to get a string representation. </param>
		/// <returns> The string representation of the associative XML object's value. </returns>
		public static string CommonToString(IAssociativeXmlObject associativeXmlObject)
		{
			object value;

			if ((object)associativeXmlObject == null)
				throw new ArgumentNullException(nameof(associativeXmlObject));

			value = associativeXmlObject.GetAssociativeObjectValue(NullTemplatingContext.Instance);

			if ((object)value == null)
				return null;

			return value.SafeToString();
		}

		/// <summary>
		/// Gets the enumerator for the current associative object instance. Overrides the default behavior to always return null.
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
		/// Gets the value of the current associative object instance. Overrides the default behavior to return a strongly-typed, parsed value from using the Type and Value properties.
		/// </summary>
		/// <param name="templatingContext"> The templating context. </param>
		/// <returns> A value or null. </returns>
		protected override object CoreGetAssociativeObjectValue(ITemplatingContext templatingContext)
		{
			object value;
			Type valueType;

			if ((object)templatingContext == null)
				throw new ArgumentNullException(nameof(templatingContext));

			if (SolderFascadeAccessor.DataTypeFascade.IsNullOrWhiteSpace(this.Type))
				valueType = typeof(String);
			else
				valueType = System.Type.GetType(this.Type, false);

			if ((object)valueType == null)
				throw new InvalidOperationException(string.Format("Failed to determine or load the type '{0}' of the associative property value.", this.Type));

			if (!SolderFascadeAccessor.DataTypeFascade.TryParse(valueType, this.Value, out value))
				throw new InvalidOperationException(string.Format("Failed to parse the associative property value '{0}' into the specified type '{1}'.", value, this.Type));

			return value;
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
			return CommonEquals(this, obj);
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <returns>
		/// A hash code for the current <see cref="T:System.Object" /> .
		/// </returns>
		public override int GetHashCode()
		{
			return CommonGetHashCode(this);
		}

		/// <summary>
		/// Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" /> .
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" /> .
		/// </returns>
		public override string ToString()
		{
			return CommonToString(this);
		}

		#endregion
	}
}