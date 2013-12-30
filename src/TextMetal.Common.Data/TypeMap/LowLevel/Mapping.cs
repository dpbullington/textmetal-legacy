/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Xml.Serialization;

namespace TextMetal.Common.Data.TypeMap.LowLevel
{
	[Serializable]
	public sealed class Mapping
	{
		#region Constructors/Destructors

		public Mapping()
		{
		}

		#endregion

		#region Fields/Constants

		private string that;
		private string @this;

		#endregion

		#region Properties/Indexers/Events

		[XmlAttribute("that")]
		public string That
		{
			get
			{
				return this.that ?? "";
			}
			set
			{
				this.that = (value ?? "").Trim();
			}
		}

		[XmlAttribute("this")]
		public string This
		{
			get
			{
				return this.@this ?? "";
			}
			set
			{
				this.@this = (value ?? "").Trim();
			}
		}

		#endregion
	}
}