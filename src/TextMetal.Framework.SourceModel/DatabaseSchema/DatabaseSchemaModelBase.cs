/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace TextMetal.Framework.SourceModel.DatabaseSchema
{
	[Serializable]
	public abstract class DatabaseSchemaModelBase
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the DatabaseSchemaModelBase class.
		/// </summary>
		protected DatabaseSchemaModelBase()
		{
		}

		#endregion

		#region Fields/Constants

		private readonly List<ExtendedProperty> extendedProperties = new List<ExtendedProperty>();

		#endregion

		#region Properties/Indexers/Events

		[XmlArray(ElementName = "ExtendedProperties")]
		[XmlArrayItem(ElementName = "ExtendedProperty")]
		public List<ExtendedProperty> ExtendedProperties
		{
			get
			{
				return this.extendedProperties;
			}
		}

		[XmlIgnore]
		public bool HasExtendedProperties
		{
			get
			{
				return this.ExtendedProperties.Count() > 0;
			}
		}

		#endregion
	}
}