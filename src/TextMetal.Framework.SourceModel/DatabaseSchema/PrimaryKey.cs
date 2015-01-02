/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace TextMetal.Framework.SourceModel.DatabaseSchema
{
	[Serializable]
	public class PrimaryKey
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the PrimaryKey class.
		/// </summary>
		public PrimaryKey()
		{
		}

		#endregion

		#region Fields/Constants

		private int primaryKeyId;
		private bool primaryKeyIsSystemNamed;
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
		private readonly List<PrimaryKeyColumn> primaryKeyColumn = new List<PrimaryKeyColumn>();

		#endregion

		#region Properties/Indexers/Events

		[XmlAttribute]
		public int PrimaryKeyId
		{
			get
			{
				return this.primaryKeyId;
			}
			set
			{
				this.primaryKeyId = value;
			}
		}

		[XmlAttribute]
		public bool PrimaryKeyIsSystemNamed
		{
			get
			{
				return this.primaryKeyIsSystemNamed;
			}
			set
			{
				this.primaryKeyIsSystemNamed = value;
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

		[XmlArray(ElementName = "PrimaryKeyColumns")]
		[XmlArrayItem(ElementName = "PrimaryKeyColumn")]
		public List<PrimaryKeyColumn> PrimaryKeyColumns
		{
			get
			{
				return this.primaryKeyColumn;
			}
		}

		#endregion
	}
}