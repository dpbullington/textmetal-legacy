/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Framework.XmlDialect
{
	/// <summary>
	/// Marks an interface as an XML object which is mapped to/from an XML element.
	/// </summary>
	[AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
	public sealed class XmlKnownElementMappingAttribute : XmlElementMappingAttribute
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the XmlKnownElementMappingAttribute class.
		/// </summary>
		public XmlKnownElementMappingAttribute()
		{
		}

		#endregion
	}
}