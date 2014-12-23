/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace TextMetal.Framework.SourceModel.DatabaseSchema
{
	[Serializable]
	public class ForeignKey
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the ForeignKey class.
		/// </summary>
		public ForeignKey()
		{
		}

		#endregion

		#region Fields/Constants

		private bool foreignKeyIsDisabled;
		private bool foreignKeyIsForReplication;
		private bool foreignKeyIsSystemNamed;
		private string foreignKeyName;
		private string foreignKeyNameCamelCase;
		private string foreignKeyNameConstantCase;
		private string foreignKeyNamePascalCase;
		private string foreignKeyNamePluralCamelCase;
		private string foreignKeyNamePluralConstantCase;
		private string foreignKeyNamePluralPascalCase;
		private string foreignKeyNameSingularCamelCase;
		private string foreignKeyNameSingularConstantCase;
		private string foreignKeyNameSingularPascalCase;
		private string foreignKeyNameSqlMetalCamelCase;
		private string foreignKeyNameSqlMetalPascalCase;
		private string foreignKeyNameSqlMetalPluralCamelCase;
		private string foreignKeyNameSqlMetalPluralPascalCase;
		private string foreignKeyNameSqlMetalSingularCamelCase;
		private string foreignKeyNameSqlMetalSingularPascalCase;
		private byte foreignKeyOnDeleteRefIntAction;
		private string foreignKeyOnDeleteRefIntActionSqlName;
		private byte foreignKeyOnUpdateRefIntAction;
		private string foreignKeyOnUpdateRefIntActionSqlName;
		private string targetSchemaName;
		private string targetSchemaNameCamelCase;
		private string targetSchemaNameConstantCase;
		private string targetSchemaNamePascalCase;
		private string targetSchemaNamePluralCamelCase;
		private string targetSchemaNamePluralConstantCase;
		private string targetSchemaNamePluralPascalCase;
		private string targetSchemaNameSingularCamelCase;
		private string targetSchemaNameSingularConstantCase;
		private string targetSchemaNameSingularPascalCase;
		private string targetSchemaNameSqlMetalCamelCase;
		private string targetSchemaNameSqlMetalPascalCase;
		private string targetSchemaNameSqlMetalPluralCamelCase;
		private string targetSchemaNameSqlMetalPluralPascalCase;
		private string targetSchemaNameSqlMetalSingularCamelCase;
		private string targetSchemaNameSqlMetalSingularPascalCase;
		private string targetTableName;
		private string targetTableNameCamelCase;
		private string targetTableNameConstantCase;
		private string targetTableNamePascalCase;
		private string targetTableNamePluralCamelCase;
		private string targetTableNamePluralConstantCase;
		private string targetTableNamePluralPascalCase;
		private string targetTableNameSingularCamelCase;
		private string targetTableNameSingularConstantCase;
		private string targetTableNameSingularPascalCase;
		private string targetTableNameSqlMetalCamelCase;
		private string targetTableNameSqlMetalPascalCase;
		private string targetTableNameSqlMetalPluralCamelCase;
		private string targetTableNameSqlMetalPluralPascalCase;
		private string targetTableNameSqlMetalSingularCamelCase;
		private string targetTableNameSqlMetalSingularPascalCase;
		private readonly List<ForeignKeyColumn> foreignKeyColumn = new List<ForeignKeyColumn>();

		#endregion

		#region Properties/Indexers/Events

		[XmlAttribute]
		public bool ForeignKeyIsDisabled
		{
			get
			{
				return this.foreignKeyIsDisabled;
			}
			set
			{
				this.foreignKeyIsDisabled = value;
			}
		}

		[XmlAttribute]
		public bool ForeignKeyIsForReplication
		{
			get
			{
				return this.foreignKeyIsForReplication;
			}
			set
			{
				this.foreignKeyIsForReplication = value;
			}
		}

		[XmlAttribute]
		public bool ForeignKeyIsSystemNamed
		{
			get
			{
				return this.foreignKeyIsSystemNamed;
			}
			set
			{
				this.foreignKeyIsSystemNamed = value;
			}
		}

		[XmlAttribute]
		public string ForeignKeyName
		{
			get
			{
				return this.foreignKeyName;
			}
			set
			{
				this.foreignKeyName = value;
			}
		}

		[XmlAttribute]
		public string ForeignKeyNameCamelCase
		{
			get
			{
				return this.foreignKeyNameCamelCase;
			}
			set
			{
				this.foreignKeyNameCamelCase = value;
			}
		}

		[XmlAttribute]
		public string ForeignKeyNameConstantCase
		{
			get
			{
				return this.foreignKeyNameConstantCase;
			}
			set
			{
				this.foreignKeyNameConstantCase = value;
			}
		}

		[XmlAttribute]
		public string ForeignKeyNamePascalCase
		{
			get
			{
				return this.foreignKeyNamePascalCase;
			}
			set
			{
				this.foreignKeyNamePascalCase = value;
			}
		}

		[XmlAttribute]
		public string ForeignKeyNamePluralCamelCase
		{
			get
			{
				return this.foreignKeyNamePluralCamelCase;
			}
			set
			{
				this.foreignKeyNamePluralCamelCase = value;
			}
		}

		[XmlAttribute]
		public string ForeignKeyNamePluralConstantCase
		{
			get
			{
				return this.foreignKeyNamePluralConstantCase;
			}
			set
			{
				this.foreignKeyNamePluralConstantCase = value;
			}
		}

		[XmlAttribute]
		public string ForeignKeyNamePluralPascalCase
		{
			get
			{
				return this.foreignKeyNamePluralPascalCase;
			}
			set
			{
				this.foreignKeyNamePluralPascalCase = value;
			}
		}

		[XmlAttribute]
		public string ForeignKeyNameSingularCamelCase
		{
			get
			{
				return this.foreignKeyNameSingularCamelCase;
			}
			set
			{
				this.foreignKeyNameSingularCamelCase = value;
			}
		}

		[XmlAttribute]
		public string ForeignKeyNameSingularConstantCase
		{
			get
			{
				return this.foreignKeyNameSingularConstantCase;
			}
			set
			{
				this.foreignKeyNameSingularConstantCase = value;
			}
		}

		[XmlAttribute]
		public string ForeignKeyNameSingularPascalCase
		{
			get
			{
				return this.foreignKeyNameSingularPascalCase;
			}
			set
			{
				this.foreignKeyNameSingularPascalCase = value;
			}
		}

		[XmlAttribute]
		public string ForeignKeyNameSqlMetalCamelCase
		{
			get
			{
				return this.foreignKeyNameSqlMetalCamelCase;
			}
			set
			{
				this.foreignKeyNameSqlMetalCamelCase = value;
			}
		}

		[XmlAttribute]
		public string ForeignKeyNameSqlMetalPascalCase
		{
			get
			{
				return this.foreignKeyNameSqlMetalPascalCase;
			}
			set
			{
				this.foreignKeyNameSqlMetalPascalCase = value;
			}
		}

		[XmlAttribute]
		public string ForeignKeyNameSqlMetalPluralCamelCase
		{
			get
			{
				return this.foreignKeyNameSqlMetalPluralCamelCase;
			}
			set
			{
				this.foreignKeyNameSqlMetalPluralCamelCase = value;
			}
		}

		[XmlAttribute]
		public string ForeignKeyNameSqlMetalPluralPascalCase
		{
			get
			{
				return this.foreignKeyNameSqlMetalPluralPascalCase;
			}
			set
			{
				this.foreignKeyNameSqlMetalPluralPascalCase = value;
			}
		}

		[XmlAttribute]
		public string ForeignKeyNameSqlMetalSingularCamelCase
		{
			get
			{
				return this.foreignKeyNameSqlMetalSingularCamelCase;
			}
			set
			{
				this.foreignKeyNameSqlMetalSingularCamelCase = value;
			}
		}

		[XmlAttribute]
		public string ForeignKeyNameSqlMetalSingularPascalCase
		{
			get
			{
				return this.foreignKeyNameSqlMetalSingularPascalCase;
			}
			set
			{
				this.foreignKeyNameSqlMetalSingularPascalCase = value;
			}
		}

		[XmlAttribute]
		public byte ForeignKeyOnDeleteRefIntAction
		{
			get
			{
				return this.foreignKeyOnDeleteRefIntAction;
			}
			set
			{
				this.foreignKeyOnDeleteRefIntAction = value;
			}
		}

		[XmlAttribute]
		public string ForeignKeyOnDeleteRefIntActionSqlName
		{
			get
			{
				return this.foreignKeyOnDeleteRefIntActionSqlName;
			}
			set
			{
				this.foreignKeyOnDeleteRefIntActionSqlName = value;
			}
		}

		[XmlAttribute]
		public byte ForeignKeyOnUpdateRefIntAction
		{
			get
			{
				return this.foreignKeyOnUpdateRefIntAction;
			}
			set
			{
				this.foreignKeyOnUpdateRefIntAction = value;
			}
		}

		[XmlAttribute]
		public string ForeignKeyOnUpdateRefIntActionSqlName
		{
			get
			{
				return this.foreignKeyOnUpdateRefIntActionSqlName;
			}
			set
			{
				this.foreignKeyOnUpdateRefIntActionSqlName = value;
			}
		}

		[XmlAttribute]
		public string TargetSchemaName
		{
			get
			{
				return this.targetSchemaName;
			}
			set
			{
				this.targetSchemaName = value;
			}
		}

		[XmlAttribute]
		public string TargetSchemaNameCamelCase
		{
			get
			{
				return this.targetSchemaNameCamelCase;
			}
			set
			{
				this.targetSchemaNameCamelCase = value;
			}
		}

		[XmlAttribute]
		public string TargetSchemaNameConstantCase
		{
			get
			{
				return this.targetSchemaNameConstantCase;
			}
			set
			{
				this.targetSchemaNameConstantCase = value;
			}
		}

		[XmlAttribute]
		public string TargetSchemaNamePascalCase
		{
			get
			{
				return this.targetSchemaNamePascalCase;
			}
			set
			{
				this.targetSchemaNamePascalCase = value;
			}
		}

		[XmlAttribute]
		public string TargetSchemaNamePluralCamelCase
		{
			get
			{
				return this.targetSchemaNamePluralCamelCase;
			}
			set
			{
				this.targetSchemaNamePluralCamelCase = value;
			}
		}

		[XmlAttribute]
		public string TargetSchemaNamePluralConstantCase
		{
			get
			{
				return this.targetSchemaNamePluralConstantCase;
			}
			set
			{
				this.targetSchemaNamePluralConstantCase = value;
			}
		}

		[XmlAttribute]
		public string TargetSchemaNamePluralPascalCase
		{
			get
			{
				return this.targetSchemaNamePluralPascalCase;
			}
			set
			{
				this.targetSchemaNamePluralPascalCase = value;
			}
		}

		[XmlAttribute]
		public string TargetSchemaNameSingularCamelCase
		{
			get
			{
				return this.targetSchemaNameSingularCamelCase;
			}
			set
			{
				this.targetSchemaNameSingularCamelCase = value;
			}
		}

		[XmlAttribute]
		public string TargetSchemaNameSingularConstantCase
		{
			get
			{
				return this.targetSchemaNameSingularConstantCase;
			}
			set
			{
				this.targetSchemaNameSingularConstantCase = value;
			}
		}

		[XmlAttribute]
		public string TargetSchemaNameSingularPascalCase
		{
			get
			{
				return this.targetSchemaNameSingularPascalCase;
			}
			set
			{
				this.targetSchemaNameSingularPascalCase = value;
			}
		}

		[XmlAttribute]
		public string TargetSchemaNameSqlMetalCamelCase
		{
			get
			{
				return this.targetSchemaNameSqlMetalCamelCase;
			}
			set
			{
				this.targetSchemaNameSqlMetalCamelCase = value;
			}
		}

		[XmlAttribute]
		public string TargetSchemaNameSqlMetalPascalCase
		{
			get
			{
				return this.targetSchemaNameSqlMetalPascalCase;
			}
			set
			{
				this.targetSchemaNameSqlMetalPascalCase = value;
			}
		}

		[XmlAttribute]
		public string TargetSchemaNameSqlMetalPluralCamelCase
		{
			get
			{
				return this.targetSchemaNameSqlMetalPluralCamelCase;
			}
			set
			{
				this.targetSchemaNameSqlMetalPluralCamelCase = value;
			}
		}

		[XmlAttribute]
		public string TargetSchemaNameSqlMetalPluralPascalCase
		{
			get
			{
				return this.targetSchemaNameSqlMetalPluralPascalCase;
			}
			set
			{
				this.targetSchemaNameSqlMetalPluralPascalCase = value;
			}
		}

		[XmlAttribute]
		public string TargetSchemaNameSqlMetalSingularCamelCase
		{
			get
			{
				return this.targetSchemaNameSqlMetalSingularCamelCase;
			}
			set
			{
				this.targetSchemaNameSqlMetalSingularCamelCase = value;
			}
		}

		[XmlAttribute]
		public string TargetSchemaNameSqlMetalSingularPascalCase
		{
			get
			{
				return this.targetSchemaNameSqlMetalSingularPascalCase;
			}
			set
			{
				this.targetSchemaNameSqlMetalSingularPascalCase = value;
			}
		}

		[XmlAttribute]
		public string TargetTableName
		{
			get
			{
				return this.targetTableName;
			}
			set
			{
				this.targetTableName = value;
			}
		}

		[XmlAttribute]
		public string TargetTableNameCamelCase
		{
			get
			{
				return this.targetTableNameCamelCase;
			}
			set
			{
				this.targetTableNameCamelCase = value;
			}
		}

		[XmlAttribute]
		public string TargetTableNameConstantCase
		{
			get
			{
				return this.targetTableNameConstantCase;
			}
			set
			{
				this.targetTableNameConstantCase = value;
			}
		}

		[XmlAttribute]
		public string TargetTableNamePascalCase
		{
			get
			{
				return this.targetTableNamePascalCase;
			}
			set
			{
				this.targetTableNamePascalCase = value;
			}
		}

		[XmlAttribute]
		public string TargetTableNamePluralCamelCase
		{
			get
			{
				return this.targetTableNamePluralCamelCase;
			}
			set
			{
				this.targetTableNamePluralCamelCase = value;
			}
		}

		[XmlAttribute]
		public string TargetTableNamePluralConstantCase
		{
			get
			{
				return this.targetTableNamePluralConstantCase;
			}
			set
			{
				this.targetTableNamePluralConstantCase = value;
			}
		}

		[XmlAttribute]
		public string TargetTableNamePluralPascalCase
		{
			get
			{
				return this.targetTableNamePluralPascalCase;
			}
			set
			{
				this.targetTableNamePluralPascalCase = value;
			}
		}

		[XmlAttribute]
		public string TargetTableNameSingularCamelCase
		{
			get
			{
				return this.targetTableNameSingularCamelCase;
			}
			set
			{
				this.targetTableNameSingularCamelCase = value;
			}
		}

		[XmlAttribute]
		public string TargetTableNameSingularConstantCase
		{
			get
			{
				return this.targetTableNameSingularConstantCase;
			}
			set
			{
				this.targetTableNameSingularConstantCase = value;
			}
		}

		[XmlAttribute]
		public string TargetTableNameSingularPascalCase
		{
			get
			{
				return this.targetTableNameSingularPascalCase;
			}
			set
			{
				this.targetTableNameSingularPascalCase = value;
			}
		}

		[XmlAttribute]
		public string TargetTableNameSqlMetalCamelCase
		{
			get
			{
				return this.targetTableNameSqlMetalCamelCase;
			}
			set
			{
				this.targetTableNameSqlMetalCamelCase = value;
			}
		}

		[XmlAttribute]
		public string TargetTableNameSqlMetalPascalCase
		{
			get
			{
				return this.targetTableNameSqlMetalPascalCase;
			}
			set
			{
				this.targetTableNameSqlMetalPascalCase = value;
			}
		}

		[XmlAttribute]
		public string TargetTableNameSqlMetalPluralCamelCase
		{
			get
			{
				return this.targetTableNameSqlMetalPluralCamelCase;
			}
			set
			{
				this.targetTableNameSqlMetalPluralCamelCase = value;
			}
		}

		[XmlAttribute]
		public string TargetTableNameSqlMetalPluralPascalCase
		{
			get
			{
				return this.targetTableNameSqlMetalPluralPascalCase;
			}
			set
			{
				this.targetTableNameSqlMetalPluralPascalCase = value;
			}
		}

		[XmlAttribute]
		public string TargetTableNameSqlMetalSingularCamelCase
		{
			get
			{
				return this.targetTableNameSqlMetalSingularCamelCase;
			}
			set
			{
				this.targetTableNameSqlMetalSingularCamelCase = value;
			}
		}

		[XmlAttribute]
		public string TargetTableNameSqlMetalSingularPascalCase
		{
			get
			{
				return this.targetTableNameSqlMetalSingularPascalCase;
			}
			set
			{
				this.targetTableNameSqlMetalSingularPascalCase = value;
			}
		}

		[XmlArray(ElementName = "ForeignKeyColumns")]
		[XmlArrayItem(ElementName = "ForeignKeyColumn")]
		public List<ForeignKeyColumn> ForeignKeyColumns
		{
			get
			{
				return this.foreignKeyColumn;
			}
		}

		#endregion
	}
}