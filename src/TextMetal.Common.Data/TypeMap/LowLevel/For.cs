/*
	Copyright ©2002-2013 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Xml.Serialization;

namespace TextMetal.Common.Data.TypeMap.LowLevel
{
	[Serializable]
	public sealed class For
	{
		#region Constructors/Destructors

		public For()
		{
		}

		#endregion

		#region Fields/Constants

		private Command command;
		private string id;

		#endregion

		#region Properties/Indexers/Events

		[XmlElement(ElementName = "Command")]
		public Command Command
		{
			get
			{
				return this.command;
			}
			set
			{
				this.command = value;
			}
		}

		[XmlAttribute("id")]
		public string Id
		{
			get
			{
				return this.id ?? "";
			}
			set
			{
				this.id = (value ?? "").Trim();
			}
		}

		#endregion
	}
}