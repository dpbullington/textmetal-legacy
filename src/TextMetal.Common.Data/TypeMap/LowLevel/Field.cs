/*
	Copyright ©2002-2013 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Xml.Serialization;

namespace TextMetal.Common.Data.TypeMap.LowLevel
{
	[Serializable]
	public sealed class Field
	{
		#region Constructors/Destructors

		public Field()
		{
		}

		#endregion

		#region Fields/Constants

		private string name;
		private string property;

		#endregion

		#region Properties/Indexers/Events

		[XmlAttribute("name")]
		public string Name
		{
			get
			{
				return this.name ?? "";
			}
			set
			{
				this.name = (value ?? "").Trim();
			}
		}

		[XmlAttribute("property")]
		public string Property
		{
			get
			{
				return this.property ?? "";
			}
			set
			{
				this.property = (value ?? "").Trim();
			}
		}

		#endregion
	}
}