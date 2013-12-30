/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Data;
using System.Xml.Serialization;

namespace TextMetal.Common.Data.TypeMap.LowLevel
{
	[Serializable]
	public sealed class Parameter
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the Parameter class.
		/// </summary>
		public Parameter()
		{
		}

		#endregion

		#region Fields/Constants

		private ParameterDirection direction = ParameterDirection.Input;
		private string name;
		private bool nullable;
		private byte precision;
		private string property;
		private byte scale;
		private int size;
		private DbType type = DbType.Object;

		#endregion

		#region Properties/Indexers/Events

		[XmlAttribute("direction")]
		public ParameterDirection Direction
		{
			get
			{
				return this.direction;
			}
			set
			{
				this.direction = value;
			}
		}

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

		[XmlAttribute("nullable")]
		public bool Nullable
		{
			get
			{
				return this.nullable;
			}
			set
			{
				this.nullable = value;
			}
		}

		[XmlAttribute("precision")]
		public byte Precision
		{
			get
			{
				return this.precision;
			}
			set
			{
				this.precision = value;
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

		[XmlAttribute("scale")]
		public byte Scale
		{
			get
			{
				return this.scale;
			}
			set
			{
				this.scale = value;
			}
		}

		[XmlAttribute("size")]
		public int Size
		{
			get
			{
				return this.size;
			}
			set
			{
				this.size = value;
			}
		}

		[XmlAttribute("type")]
		public DbType Type
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

		#endregion
	}
}