/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace TextMetal.Framework.Source.DatabaseSchema
{
	[Serializable]
	public class Table : ITabular
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the Table class.
		/// </summary>
		public Table()
			: this(false)
		{
		}

		/// <summary>
		/// Initializes a new instance of the Table class.
		/// </summary>
		[Obsolete("Provided for model breaking change compatability only.")]
		private Table(bool isView)
		{
			this.isView = isView;
		}

		#endregion

		#region Fields/Constants

		private readonly List<TableColumn> columns = new List<TableColumn>();
		private readonly List<ForeignKey> foreignKeys = new List<ForeignKey>();
		private readonly bool isView;
		private readonly List<Trigger> triggers = new List<Trigger>();
		private readonly List<UniqueKey> uniqueKeys = new List<UniqueKey>();
		private DateTime creationTimestamp;
		private bool hasNoDefinedPrimaryKeyColumns;
		private bool isImplementationDetail;
		private DateTime modificationTimestamp;
		private PrimaryKey primaryKey;
		private int tableId;
		private string tableName;
		private string tableNameCamelCase;
		private string tableNameConstantCase;
		private string tableNamePascalCase;
		private string tableNamePluralCamelCase;
		private string tableNamePluralConstantCase;
		private string tableNamePluralPascalCase;
		private string tableNameSingularCamelCase;
		private string tableNameSingularConstantCase;
		private string tableNameSingularPascalCase;
		private string tableNameSqlMetal;
		private string tableNameSqlMetalCamelCase;
		private string tableNameSqlMetalPascalCase;
		private string tableNameSqlMetalPluralCamelCase;
		private string tableNameSqlMetalPluralPascalCase;
		private string tableNameSqlMetalSingularCamelCase;
		private string tableNameSqlMetalSingularPascalCase;

		#endregion

		#region Properties/Indexers/Events

		[XmlArray(ElementName = "Columns")]
		[XmlArrayItem(ElementName = "Column")]
		public List<TableColumn> Columns
		{
			get
			{
				return this.columns;
			}
		}

		[Obsolete("Provided for model breaking change compatability only.")]
		[XmlIgnore]
		IEnumerable<Column> ITabular.Columns
		{
			get
			{
				return this.Columns;
			}
		}

		[Obsolete("Provided for model breaking change compatability only.")]
		[XmlIgnore]
		public string CSharpIsViewLiteral
		{
			get
			{
				return this.IsView.ToString().ToLower();
			}
		}

		[XmlArray(ElementName = "ForeignKeys")]
		[XmlArrayItem(ElementName = "ForeignKey")]
		public List<ForeignKey> ForeignKeys
		{
			get
			{
				return this.foreignKeys;
			}
		}

		[XmlIgnore]
		public bool HasAnyMappedColumns
		{
			get
			{
				return this.Columns.Any();
			}
		}

		[XmlIgnore]
		public bool HasIdentityColumns
		{
			get
			{
				return this.Columns.Count(c => c.ColumnIsIdentity) > 0;
			}
		}

		[XmlIgnore]
		public bool HasSingleColumnServerGeneratedPrimaryKey
		{
			get
			{
				return this.Columns.Count(c => c.IsColumnServerGeneratedPrimaryKey) == 1;
			}
		}

		[Obsolete("Provided for model breaking change compatability only.")]
		[XmlIgnore]
		public bool IsView
		{
			get
			{
				return this.isView;
			}
		}

		[Obsolete("Provided for model breaking change compatability only.")]
		[XmlIgnore]
		public string PrimaryKeyName
		{
			get
			{
				return (object)this.PrimaryKey != null ? this.PrimaryKey.PrimaryKeyName : null;
			}
		}

		[XmlArray(ElementName = "Triggers")]
		[XmlArrayItem(ElementName = "Trigger")]
		public List<Trigger> Triggers
		{
			get
			{
				return this.triggers;
			}
		}

		[XmlArray(ElementName = "UniqueKeys")]
		[XmlArrayItem(ElementName = "UniqueKey")]
		public List<UniqueKey> UniqueKeys
		{
			get
			{
				return this.uniqueKeys;
			}
		}

		[XmlAttribute]
		public DateTime CreationTimestamp
		{
			get
			{
				return this.creationTimestamp;
			}
			set
			{
				this.creationTimestamp = value;
			}
		}

		[XmlAttribute]
		public bool HasNoDefinedPrimaryKeyColumns
		{
			get
			{
				return this.hasNoDefinedPrimaryKeyColumns;
			}
			set
			{
				this.hasNoDefinedPrimaryKeyColumns = value;
			}
		}

		[XmlAttribute]
		public bool IsImplementationDetail
		{
			get
			{
				return this.isImplementationDetail;
			}
			set
			{
				this.isImplementationDetail = value;
			}
		}

		[XmlAttribute]
		public DateTime ModificationTimestamp
		{
			get
			{
				return this.modificationTimestamp;
			}
			set
			{
				this.modificationTimestamp = value;
			}
		}

		[XmlElement]
		public PrimaryKey PrimaryKey
		{
			get
			{
				return this.primaryKey;
			}
			set
			{
				this.primaryKey = value;
			}
		}

		[XmlAttribute]
		public int TableId
		{
			get
			{
				return this.tableId;
			}
			set
			{
				this.tableId = value;
			}
		}

		[XmlAttribute]
		public string TableName
		{
			get
			{
				return this.tableName;
			}
			set
			{
				this.tableName = value;
			}
		}

		[XmlAttribute]
		public string TableNameCamelCase
		{
			get
			{
				return this.tableNameCamelCase;
			}
			set
			{
				this.tableNameCamelCase = value;
			}
		}

		[XmlAttribute]
		public string TableNameConstantCase
		{
			get
			{
				return this.tableNameConstantCase;
			}
			set
			{
				this.tableNameConstantCase = value;
			}
		}

		[XmlAttribute]
		public string TableNamePascalCase
		{
			get
			{
				return this.tableNamePascalCase;
			}
			set
			{
				this.tableNamePascalCase = value;
			}
		}

		[XmlAttribute]
		public string TableNamePluralCamelCase
		{
			get
			{
				return this.tableNamePluralCamelCase;
			}
			set
			{
				this.tableNamePluralCamelCase = value;
			}
		}

		[XmlAttribute]
		public string TableNamePluralConstantCase
		{
			get
			{
				return this.tableNamePluralConstantCase;
			}
			set
			{
				this.tableNamePluralConstantCase = value;
			}
		}

		[XmlAttribute]
		public string TableNamePluralPascalCase
		{
			get
			{
				return this.tableNamePluralPascalCase;
			}
			set
			{
				this.tableNamePluralPascalCase = value;
			}
		}

		[XmlAttribute]
		public string TableNameSingularCamelCase
		{
			get
			{
				return this.tableNameSingularCamelCase;
			}
			set
			{
				this.tableNameSingularCamelCase = value;
			}
		}

		[XmlAttribute]
		public string TableNameSingularConstantCase
		{
			get
			{
				return this.tableNameSingularConstantCase;
			}
			set
			{
				this.tableNameSingularConstantCase = value;
			}
		}

		[XmlAttribute]
		public string TableNameSingularPascalCase
		{
			get
			{
				return this.tableNameSingularPascalCase;
			}
			set
			{
				this.tableNameSingularPascalCase = value;
			}
		}

		[XmlAttribute]
		public string TableNameSqlMetal
		{
			get
			{
				return this.tableNameSqlMetal;
			}
			set
			{
				this.tableNameSqlMetal = value;
			}
		}

		[XmlAttribute]
		public string TableNameSqlMetalCamelCase
		{
			get
			{
				return this.tableNameSqlMetalCamelCase;
			}
			set
			{
				this.tableNameSqlMetalCamelCase = value;
			}
		}

		[XmlAttribute]
		public string TableNameSqlMetalPascalCase
		{
			get
			{
				return this.tableNameSqlMetalPascalCase;
			}
			set
			{
				this.tableNameSqlMetalPascalCase = value;
			}
		}

		[XmlAttribute]
		public string TableNameSqlMetalPluralCamelCase
		{
			get
			{
				return this.tableNameSqlMetalPluralCamelCase;
			}
			set
			{
				this.tableNameSqlMetalPluralCamelCase = value;
			}
		}

		[XmlAttribute]
		public string TableNameSqlMetalPluralPascalCase
		{
			get
			{
				return this.tableNameSqlMetalPluralPascalCase;
			}
			set
			{
				this.tableNameSqlMetalPluralPascalCase = value;
			}
		}

		[XmlAttribute]
		public string TableNameSqlMetalSingularCamelCase
		{
			get
			{
				return this.tableNameSqlMetalSingularCamelCase;
			}
			set
			{
				this.tableNameSqlMetalSingularCamelCase = value;
			}
		}

		[XmlAttribute]
		public string TableNameSqlMetalSingularPascalCase
		{
			get
			{
				return this.tableNameSqlMetalSingularPascalCase;
			}
			set
			{
				this.tableNameSqlMetalSingularPascalCase = value;
			}
		}

		#endregion
	}
}