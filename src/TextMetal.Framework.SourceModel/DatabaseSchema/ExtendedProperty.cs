/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Xml.Serialization;

namespace TextMetal.Framework.SourceModel.DatabaseSchema
{
	[Serializable]
	[XmlRoot(ElementName = "ExtendedProperty", Namespace = "http://www.textmetal.com/api/v6.0.0")]
	public class ExtendedProperty
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the ExtendedProperty class.
		/// </summary>
		public ExtendedProperty()
		{
		}

		#endregion

		#region Fields/Constants

		private string name;
		private string value;

		#endregion

		#region Properties/Indexers/Events

		[XmlAttribute]
		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		[XmlAttribute]
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
	}
}