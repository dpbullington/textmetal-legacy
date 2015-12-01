/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace TextMetal.Framework.Source.DatabaseSchema
{
	[Serializable]
	public class Schema
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the Schema class.
		/// </summary>
		public Schema()
		{
		}

		#endregion

		#region Fields/Constants

		private readonly List<Procedure> procedures = new List<Procedure>();
		private readonly List<Table> tables = new List<Table>();
		private readonly List<View> views = new List<View>();
		private int ownerId;
		private int schemaId;
		private string schemaName;
		private string schemaNameCamelCase;
		private string schemaNameConstantCase;
		private string schemaNamePascalCase;
		private string schemaNamePluralCamelCase;
		private string schemaNamePluralConstantCase;
		private string schemaNamePluralPascalCase;
		private string schemaNameSingularCamelCase;
		private string schemaNameSingularConstantCase;
		private string schemaNameSingularPascalCase;
		private string schemaNameSqlMetalCamelCase;
		private string schemaNameSqlMetalPascalCase;
		private string schemaNameSqlMetalPluralCamelCase;
		private string schemaNameSqlMetalPluralPascalCase;
		private string schemaNameSqlMetalSingularCamelCase;
		private string schemaNameSqlMetalSingularPascalCase;

		#endregion

		#region Properties/Indexers/Events

		[XmlArray(ElementName = "Tables")]
		[XmlArrayItem(ElementName = "Table")]
		public List<Table> _Tables
		{
			get
			{
				return this.tables;
			}
		}

		[XmlArray(ElementName = "Procedures")]
		[XmlArrayItem(ElementName = "Procedure")]
		public List<Procedure> Procedures
		{
			get
			{
				return this.procedures;
			}
		}

		[Obsolete("Provided for model breaking change compatability only.")]
		[XmlIgnore]
		public IEnumerable<ITabular> Tables
		{
			get
			{
				if ((object)this._Tables != null)
				{
					foreach (Table table in this._Tables)
						yield return table;
				}

				if ((object)this.Views != null)
				{
					foreach (View view in this.Views)
						yield return view;
				}
			}
		}

		[XmlArray(ElementName = "Views")]
		[XmlArrayItem(ElementName = "View")]
		public List<View> Views
		{
			get
			{
				return this.views;
			}
		}

		[XmlAttribute]
		public int OwnerId
		{
			get
			{
				return this.ownerId;
			}
			set
			{
				this.ownerId = value;
			}
		}

		[XmlAttribute]
		public int SchemaId
		{
			get
			{
				return this.schemaId;
			}
			set
			{
				this.schemaId = value;
			}
		}

		[XmlAttribute]
		public string SchemaName
		{
			get
			{
				return this.schemaName;
			}
			set
			{
				this.schemaName = value;
			}
		}

		[XmlAttribute]
		public string SchemaNameCamelCase
		{
			get
			{
				return this.schemaNameCamelCase;
			}
			set
			{
				this.schemaNameCamelCase = value;
			}
		}

		[XmlAttribute]
		public string SchemaNameConstantCase
		{
			get
			{
				return this.schemaNameConstantCase;
			}
			set
			{
				this.schemaNameConstantCase = value;
			}
		}

		[XmlAttribute]
		public string SchemaNamePascalCase
		{
			get
			{
				return this.schemaNamePascalCase;
			}
			set
			{
				this.schemaNamePascalCase = value;
			}
		}

		[XmlAttribute]
		public string SchemaNamePluralCamelCase
		{
			get
			{
				return this.schemaNamePluralCamelCase;
			}
			set
			{
				this.schemaNamePluralCamelCase = value;
			}
		}

		[XmlAttribute]
		public string SchemaNamePluralConstantCase
		{
			get
			{
				return this.schemaNamePluralConstantCase;
			}
			set
			{
				this.schemaNamePluralConstantCase = value;
			}
		}

		[XmlAttribute]
		public string SchemaNamePluralPascalCase
		{
			get
			{
				return this.schemaNamePluralPascalCase;
			}
			set
			{
				this.schemaNamePluralPascalCase = value;
			}
		}

		[XmlAttribute]
		public string SchemaNameSingularCamelCase
		{
			get
			{
				return this.schemaNameSingularCamelCase;
			}
			set
			{
				this.schemaNameSingularCamelCase = value;
			}
		}

		[XmlAttribute]
		public string SchemaNameSingularConstantCase
		{
			get
			{
				return this.schemaNameSingularConstantCase;
			}
			set
			{
				this.schemaNameSingularConstantCase = value;
			}
		}

		[XmlAttribute]
		public string SchemaNameSingularPascalCase
		{
			get
			{
				return this.schemaNameSingularPascalCase;
			}
			set
			{
				this.schemaNameSingularPascalCase = value;
			}
		}

		[XmlAttribute]
		public string SchemaNameSqlMetalCamelCase
		{
			get
			{
				return this.schemaNameSqlMetalCamelCase;
			}
			set
			{
				this.schemaNameSqlMetalCamelCase = value;
			}
		}

		[XmlAttribute]
		public string SchemaNameSqlMetalPascalCase
		{
			get
			{
				return this.schemaNameSqlMetalPascalCase;
			}
			set
			{
				this.schemaNameSqlMetalPascalCase = value;
			}
		}

		[XmlAttribute]
		public string SchemaNameSqlMetalPluralCamelCase
		{
			get
			{
				return this.schemaNameSqlMetalPluralCamelCase;
			}
			set
			{
				this.schemaNameSqlMetalPluralCamelCase = value;
			}
		}

		[XmlAttribute]
		public string SchemaNameSqlMetalPluralPascalCase
		{
			get
			{
				return this.schemaNameSqlMetalPluralPascalCase;
			}
			set
			{
				this.schemaNameSqlMetalPluralPascalCase = value;
			}
		}

		[XmlAttribute]
		public string SchemaNameSqlMetalSingularCamelCase
		{
			get
			{
				return this.schemaNameSqlMetalSingularCamelCase;
			}
			set
			{
				this.schemaNameSqlMetalSingularCamelCase = value;
			}
		}

		[XmlAttribute]
		public string SchemaNameSqlMetalSingularPascalCase
		{
			get
			{
				return this.schemaNameSqlMetalSingularPascalCase;
			}
			set
			{
				this.schemaNameSqlMetalSingularPascalCase = value;
			}
		}

		#endregion
	}
}