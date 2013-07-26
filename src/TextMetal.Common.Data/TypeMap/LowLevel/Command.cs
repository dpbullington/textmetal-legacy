/*
	Copyright ©2002-2013 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Xml.Serialization;

using TextMetal.Common.Core;

namespace TextMetal.Common.Data.TypeMap.LowLevel
{
	[Serializable]
	public sealed class Command
	{
		#region Constructors/Destructors

		public Command()
		{
		}

		#endregion

		#region Fields/Constants

		private readonly List<Field> fields = new List<Field>();
		private readonly List<Parameter> parameters = new List<Parameter>();
		private string identity;
		private bool prepare;
		private string text;
		private int? timeout;
		private CommandType type = CommandType.Text;

		#endregion

		#region Properties/Indexers/Events

		[XmlArray(ElementName = "Fields", Order = 2)]
		[XmlArrayItem(ElementName = "Field")]
		public List<Field> Fields
		{
			get
			{
				return this.fields;
			}
		}

		[XmlElement("Identity", Order = Int32.MaxValue)]
		public string Identity
		{
			get
			{
				return this.identity ?? "";
			}
			set
			{
				this.identity = value ?? "";
			}
		}

		[XmlArray(ElementName = "Parameters", Order = 1)]
		[XmlArrayItem(ElementName = "Parameter")]
		public List<Parameter> Parameters
		{
			get
			{
				return this.parameters;
			}
		}

		[XmlAttribute("prepare")]
		public bool Prepare
		{
			get
			{
				return this.prepare;
			}
			set
			{
				this.prepare = value;
			}
		}

		[XmlElement("Text", Order = 0)]
		public string Text
		{
			get
			{
				return this.text ?? "";
			}
			set
			{
				this.text = value ?? "";
			}
		}

		[XmlIgnore]
		public int? Timeout
		{
			get
			{
				return this.timeout;
			}
			set
			{
				this.timeout = value;
			}
		}

		[XmlAttribute("type")]
		public CommandType Type
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

		[XmlAttribute("timeout")]
		public string _Timeout
		{
			get
			{
				return (object)this.timeout == null ? "" : ((int)this.timeout).ToString();
			}
			set
			{
				int ivalue;

				if (DataType.IsNullOrWhiteSpace(value))
					this.timeout = null;
				else
				{
					if (!DataType.TryParse<int>(value, out ivalue))
						this.timeout = null;
					else
						this.timeout = ivalue;
				}
			}
		}

		#endregion
	}
}