/*
	Copyright ©2002-2013 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace TextMetal.Common.Data.TypeMap.LowLevel
{
	[Serializable]
	public sealed class Relationship
	{
		#region Constructors/Destructors

		public Relationship()
		{
		}

		#endregion

		#region Fields/Constants

		private readonly List<Mapping> mappings = new List<Mapping>();
		private string foreign;
		private string kind;
		private string property;

		#endregion

		#region Properties/Indexers/Events

		[XmlAttribute("foreign")]
		public string Foreign
		{
			get
			{
				return this.foreign ?? "";
			}
			set
			{
				this.foreign = (value ?? "").Trim();
			}
		}

		[XmlAttribute("kind")]
		public string Kind
		{
			get
			{
				return this.kind ?? "";
			}
			set
			{
				this.kind = (value ?? "").Trim();
			}
		}

		[XmlArray(ElementName = "Mappings", Order = 0)]
		[XmlArrayItem(ElementName = "Mapping")]
		public List<Mapping> Mappings
		{
			get
			{
				return this.mappings;
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