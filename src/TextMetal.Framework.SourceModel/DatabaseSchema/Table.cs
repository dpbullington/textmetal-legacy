/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace TextMetal.Framework.SourceModel.DatabaseSchema
{
	[Serializable]
	public class Table : DatabaseSchemaModelBase
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the Table class.
		/// </summary>
		public Table()
		{
		}

		#endregion

		#region Fields/Constants

		private readonly List<Column> columns = new List<Column>();
		private readonly List<ForeignKey> foreignKeys = new List<ForeignKey>();
		private readonly List<Trigger> triggers = new List<Trigger>();
		private readonly List<UniqueKey> uniqueKeys = new List<UniqueKey>();
		private bool hasNoDefinedPrimaryKeyColumns;
		private bool isView;
		private string primaryKeyName;
		private string primaryKeyNameCamelCase;
		private string primaryKeyNameConstantCase;
		private string primaryKeyNamePascalCase;
		private string primaryKeyNamePluralCamelCase;
		private string primaryKeyNamePluralConstantCase;
		private string primaryKeyNamePluralPascalCase;
		private string primaryKeyNameSingularCamelCase;
		private string primaryKeyNameSingularConstantCase;
		private string primaryKeyNameSingularPascalCase;
		private string primaryKeyNameSqlMetalCamelCase;
		private string primaryKeyNameSqlMetalPascalCase;
		private string primaryKeyNameSqlMetalPluralCamelCase;
		private string primaryKeyNameSqlMetalPluralPascalCase;
		private string primaryKeyNameSqlMetalSingularCamelCase;
		private string primaryKeyNameSqlMetalSingularPascalCase;
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
		public List<Column> Columns
		{
			get
			{
				return this.columns;
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
		public bool HasIdentityColumns
		{
			get
			{
				return this.Columns.Count(c => c.ColumnIsIdentity) > 0;
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

		public bool HasSingleColumnServerGeneratedPrimaryKey
		{
			get
			{
				return this.Columns.Count(c => c.IsColumnServerGeneratedPrimaryKey) == 1;
			}
		}

		[XmlAttribute]
		public bool IsView
		{
			get
			{
				return this.isView;
			}
			set
			{
				this.isView = value;
			}
		}

		[XmlAttribute]
		public string PrimaryKeyName
		{
			get
			{
				return this.primaryKeyName;
			}
			set
			{
				this.primaryKeyName = value;
			}
		}

		[XmlAttribute]
		public string PrimaryKeyNameCamelCase
		{
			get
			{
				return this.primaryKeyNameCamelCase;
			}
			set
			{
				this.primaryKeyNameCamelCase = value;
			}
		}

		[XmlAttribute]
		public string PrimaryKeyNameConstantCase
		{
			get
			{
				return this.primaryKeyNameConstantCase;
			}
			set
			{
				this.primaryKeyNameConstantCase = value;
			}
		}

		[XmlAttribute]
		public string PrimaryKeyNamePascalCase
		{
			get
			{
				return this.primaryKeyNamePascalCase;
			}
			set
			{
				this.primaryKeyNamePascalCase = value;
			}
		}

		[XmlAttribute]
		public string PrimaryKeyNamePluralCamelCase
		{
			get
			{
				return this.primaryKeyNamePluralCamelCase;
			}
			set
			{
				this.primaryKeyNamePluralCamelCase = value;
			}
		}

		[XmlAttribute]
		public string PrimaryKeyNamePluralConstantCase
		{
			get
			{
				return this.primaryKeyNamePluralConstantCase;
			}
			set
			{
				this.primaryKeyNamePluralConstantCase = value;
			}
		}

		[XmlAttribute]
		public string PrimaryKeyNamePluralPascalCase
		{
			get
			{
				return this.primaryKeyNamePluralPascalCase;
			}
			set
			{
				this.primaryKeyNamePluralPascalCase = value;
			}
		}

		[XmlAttribute]
		public string PrimaryKeyNameSingularCamelCase
		{
			get
			{
				return this.primaryKeyNameSingularCamelCase;
			}
			set
			{
				this.primaryKeyNameSingularCamelCase = value;
			}
		}

		[XmlAttribute]
		public string PrimaryKeyNameSingularConstantCase
		{
			get
			{
				return this.primaryKeyNameSingularConstantCase;
			}
			set
			{
				this.primaryKeyNameSingularConstantCase = value;
			}
		}

		[XmlAttribute]
		public string PrimaryKeyNameSingularPascalCase
		{
			get
			{
				return this.primaryKeyNameSingularPascalCase;
			}
			set
			{
				this.primaryKeyNameSingularPascalCase = value;
			}
		}

		[XmlAttribute]
		public string PrimaryKeyNameSqlMetalCamelCase
		{
			get
			{
				return this.primaryKeyNameSqlMetalCamelCase;
			}
			set
			{
				this.primaryKeyNameSqlMetalCamelCase = value;
			}
		}

		[XmlAttribute]
		public string PrimaryKeyNameSqlMetalPascalCase
		{
			get
			{
				return this.primaryKeyNameSqlMetalPascalCase;
			}
			set
			{
				this.primaryKeyNameSqlMetalPascalCase = value;
			}
		}

		[XmlAttribute]
		public string PrimaryKeyNameSqlMetalPluralCamelCase
		{
			get
			{
				return this.primaryKeyNameSqlMetalPluralCamelCase;
			}
			set
			{
				this.primaryKeyNameSqlMetalPluralCamelCase = value;
			}
		}

		[XmlAttribute]
		public string PrimaryKeyNameSqlMetalPluralPascalCase
		{
			get
			{
				return this.primaryKeyNameSqlMetalPluralPascalCase;
			}
			set
			{
				this.primaryKeyNameSqlMetalPluralPascalCase = value;
			}
		}

		[XmlAttribute]
		public string PrimaryKeyNameSqlMetalSingularCamelCase
		{
			get
			{
				return this.primaryKeyNameSqlMetalSingularCamelCase;
			}
			set
			{
				this.primaryKeyNameSqlMetalSingularCamelCase = value;
			}
		}

		[XmlAttribute]
		public string PrimaryKeyNameSqlMetalSingularPascalCase
		{
			get
			{
				return this.primaryKeyNameSqlMetalSingularPascalCase;
			}
			set
			{
				this.primaryKeyNameSqlMetalSingularPascalCase = value;
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

		#endregion
	}
}