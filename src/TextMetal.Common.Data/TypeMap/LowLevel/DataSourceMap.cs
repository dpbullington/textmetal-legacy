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
	[XmlRoot(ElementName = "DataSourceMap", Namespace = "http://www.textmetal.com/api/v5.0.0")]
	public sealed class DataSourceMap
	{
		#region Constructors/Destructors

		public DataSourceMap()
		{
		}

		#endregion

		#region Fields/Constants

		private readonly List<Relationship> relationships = new List<Relationship>();
		private readonly List<For> selectFors = new List<For>();
		private Command delete;
		private Command insert;
		private Command selectAll;
		private Command selectOne;
		private Command update;

		#endregion

		#region Properties/Indexers/Events

		[XmlElement(ElementName = "Delete", Order = 2)]
		public Command Delete
		{
			get
			{
				return this.delete;
			}
			set
			{
				this.delete = value;
			}
		}

		[XmlElement(ElementName = "Insert", Order = 0)]
		public Command Insert
		{
			get
			{
				return this.insert;
			}
			set
			{
				this.insert = value;
			}
		}

		[XmlArray(ElementName = "Relationships", Order = Int32.MaxValue)]
		[XmlArrayItem(ElementName = "Relationship")]
		public List<Relationship> Relationships
		{
			get
			{
				return this.relationships;
			}
		}

		[XmlElement(ElementName = "SelectAll", Order = 3)]
		public Command SelectAll
		{
			get
			{
				return this.selectAll;
			}
			set
			{
				this.selectAll = value;
			}
		}

		[XmlArray(ElementName = "Select", Order = 5)]
		[XmlArrayItem(ElementName = "For")]
		public List<For> SelectFors
		{
			get
			{
				return this.selectFors;
			}
		}

		[XmlElement(ElementName = "SelectOne", Order = 4)]
		public Command SelectOne
		{
			get
			{
				return this.selectOne;
			}
			set
			{
				this.selectOne = value;
			}
		}

		[XmlElement(ElementName = "Update", Order = 1)]
		public Command Update
		{
			get
			{
				return this.update;
			}
			set
			{
				this.update = value;
			}
		}

		#endregion
	}
}