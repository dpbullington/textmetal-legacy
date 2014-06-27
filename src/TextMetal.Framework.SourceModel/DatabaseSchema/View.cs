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
	public class View : ITabular
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the View class.
		/// </summary>
		public View()
		{
		}

		#endregion

		#region Fields/Constants

		private readonly List<ViewColumn> columns = new List<ViewColumn>();
		private DateTime creationTimestamp;
		private bool isImplementationDetail;
		private DateTime modificationTimestamp;
		private int viewId;
		private string viewName;
		private string viewNameCamelCase;
		private string viewNameConstantCase;
		private string viewNamePascalCase;
		private string viewNamePluralCamelCase;
		private string viewNamePluralConstantCase;
		private string viewNamePluralPascalCase;
		private string viewNameSingularCamelCase;
		private string viewNameSingularConstantCase;
		private string viewNameSingularPascalCase;
		private string viewNameSqlMetalCamelCase;
		private string viewNameSqlMetalPascalCase;
		private string viewNameSqlMetalPluralCamelCase;
		private string viewNameSqlMetalPluralPascalCase;
		private string viewNameSqlMetalSingularCamelCase;
		private string viewNameSqlMetalSingularPascalCase;

		#endregion

		#region Properties/Indexers/Events

		[Obsolete("Provided for model breaking change compatability only.")]
		[XmlIgnore]
		public string PrimaryKeyName
		{
			get
			{
				return null;
			}
		}

		[XmlArray(ElementName = "Columns")]
		[XmlArrayItem(ElementName = "Column")]
		public List<ViewColumn> Columns
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

		[Obsolete("Provided for model breaking change compatability only.")]
		[XmlIgnore]
		public bool HasIdentityColumns
		{
			get
			{
				return false;
			}
		}

		[Obsolete("Provided for model breaking change compatability only.")]
		[XmlIgnore]
		public bool HasNoDefinedPrimaryKeyColumns
		{
			get
			{
				return true;
			}
		}

		[Obsolete("Provided for model breaking change compatability only.")]
		[XmlIgnore]
		public bool HasSingleColumnServerGeneratedPrimaryKey
		{
			get
			{
				return false;
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

		[Obsolete("Provided for model breaking change compatability only.")]
		[XmlIgnore]
		public bool IsView
		{
			get
			{
				return true;
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

		[Obsolete("Provided for model breaking change compatability only.")]
		[XmlIgnore]
		public int TableId
		{
			get
			{
				return this.ViewId;
			}
		}

		[Obsolete("Provided for model breaking change compatability only.")]
		[XmlIgnore]
		public string TableName
		{
			get
			{
				return this.ViewName;
			}
		}

		[Obsolete("Provided for model breaking change compatability only.")]
		[XmlIgnore]
		public string TableNameCamelCase
		{
			get
			{
				return this.ViewNameCamelCase;
			}
		}

		[Obsolete("Provided for model breaking change compatability only.")]
		[XmlIgnore]
		public string TableNameConstantCase
		{
			get
			{
				return this.ViewNameConstantCase;
			}
		}

		[Obsolete("Provided for model breaking change compatability only.")]
		[XmlIgnore]
		public string TableNamePascalCase
		{
			get
			{
				return this.ViewNamePascalCase;
			}
		}

		[Obsolete("Provided for model breaking change compatability only.")]
		[XmlIgnore]
		public string TableNamePluralCamelCase
		{
			get
			{
				return this.ViewNamePluralCamelCase;
			}
		}

		[Obsolete("Provided for model breaking change compatability only.")]
		[XmlIgnore]
		public string TableNamePluralConstantCase
		{
			get
			{
				return this.ViewNamePluralConstantCase;
			}
		}

		[Obsolete("Provided for model breaking change compatability only.")]
		[XmlIgnore]
		public string TableNamePluralPascalCase
		{
			get
			{
				return this.ViewNamePluralPascalCase;
			}
		}

		[Obsolete("Provided for model breaking change compatability only.")]
		[XmlIgnore]
		public string TableNameSingularCamelCase
		{
			get
			{
				return this.ViewNameSingularCamelCase;
			}
		}

		[Obsolete("Provided for model breaking change compatability only.")]
		[XmlIgnore]
		public string TableNameSingularConstantCase
		{
			get
			{
				return this.ViewNameSingularConstantCase;
			}
		}

		[Obsolete("Provided for model breaking change compatability only.")]
		[XmlIgnore]
		public string TableNameSingularPascalCase
		{
			get
			{
				return this.ViewNameSingularPascalCase;
			}
		}

		[Obsolete("Provided for model breaking change compatability only.")]
		[XmlIgnore]
		public string TableNameSqlMetalCamelCase
		{
			get
			{
				return this.ViewNameSqlMetalCamelCase;
			}
		}

		[Obsolete("Provided for model breaking change compatability only.")]
		[XmlIgnore]
		public string TableNameSqlMetalPascalCase
		{
			get
			{
				return this.ViewNameSqlMetalPascalCase;
			}
		}

		[Obsolete("Provided for model breaking change compatability only.")]
		[XmlIgnore]
		public string TableNameSqlMetalPluralCamelCase
		{
			get
			{
				return this.ViewNameSqlMetalPluralCamelCase;
			}
		}

		[Obsolete("Provided for model breaking change compatability only.")]
		[XmlIgnore]
		public string TableNameSqlMetalPluralPascalCase
		{
			get
			{
				return this.ViewNameSqlMetalPluralPascalCase;
			}
		}

		[Obsolete("Provided for model breaking change compatability only.")]
		[XmlIgnore]
		public string TableNameSqlMetalSingularCamelCase
		{
			get
			{
				return this.ViewNameSqlMetalSingularCamelCase;
			}
		}

		[Obsolete("Provided for model breaking change compatability only.")]
		[XmlIgnore]
		public string TableNameSqlMetalSingularPascalCase
		{
			get
			{
				return this.ViewName;
			}
		}

		[XmlAttribute]
		public int ViewId
		{
			get
			{
				return this.viewId;
			}
			set
			{
				this.viewId = value;
			}
		}

		[XmlAttribute]
		public string ViewName
		{
			get
			{
				return this.viewName;
			}
			set
			{
				this.viewName = value;
			}
		}

		[XmlAttribute]
		public string ViewNameCamelCase
		{
			get
			{
				return this.viewNameCamelCase;
			}
			set
			{
				this.viewNameCamelCase = value;
			}
		}

		[XmlAttribute]
		public string ViewNameConstantCase
		{
			get
			{
				return this.viewNameConstantCase;
			}
			set
			{
				this.viewNameConstantCase = value;
			}
		}

		[XmlAttribute]
		public string ViewNamePascalCase
		{
			get
			{
				return this.viewNamePascalCase;
			}
			set
			{
				this.viewNamePascalCase = value;
			}
		}

		[XmlAttribute]
		public string ViewNamePluralCamelCase
		{
			get
			{
				return this.viewNamePluralCamelCase;
			}
			set
			{
				this.viewNamePluralCamelCase = value;
			}
		}

		[XmlAttribute]
		public string ViewNamePluralConstantCase
		{
			get
			{
				return this.viewNamePluralConstantCase;
			}
			set
			{
				this.viewNamePluralConstantCase = value;
			}
		}

		[XmlAttribute]
		public string ViewNamePluralPascalCase
		{
			get
			{
				return this.viewNamePluralPascalCase;
			}
			set
			{
				this.viewNamePluralPascalCase = value;
			}
		}

		[XmlAttribute]
		public string ViewNameSingularCamelCase
		{
			get
			{
				return this.viewNameSingularCamelCase;
			}
			set
			{
				this.viewNameSingularCamelCase = value;
			}
		}

		[XmlAttribute]
		public string ViewNameSingularConstantCase
		{
			get
			{
				return this.viewNameSingularConstantCase;
			}
			set
			{
				this.viewNameSingularConstantCase = value;
			}
		}

		[XmlAttribute]
		public string ViewNameSingularPascalCase
		{
			get
			{
				return this.viewNameSingularPascalCase;
			}
			set
			{
				this.viewNameSingularPascalCase = value;
			}
		}

		[XmlAttribute]
		public string ViewNameSqlMetalCamelCase
		{
			get
			{
				return this.viewNameSqlMetalCamelCase;
			}
			set
			{
				this.viewNameSqlMetalCamelCase = value;
			}
		}

		[XmlAttribute]
		public string ViewNameSqlMetalPascalCase
		{
			get
			{
				return this.viewNameSqlMetalPascalCase;
			}
			set
			{
				this.viewNameSqlMetalPascalCase = value;
			}
		}

		[XmlAttribute]
		public string ViewNameSqlMetalPluralCamelCase
		{
			get
			{
				return this.viewNameSqlMetalPluralCamelCase;
			}
			set
			{
				this.viewNameSqlMetalPluralCamelCase = value;
			}
		}

		[XmlAttribute]
		public string ViewNameSqlMetalPluralPascalCase
		{
			get
			{
				return this.viewNameSqlMetalPluralPascalCase;
			}
			set
			{
				this.viewNameSqlMetalPluralPascalCase = value;
			}
		}

		[XmlAttribute]
		public string ViewNameSqlMetalSingularCamelCase
		{
			get
			{
				return this.viewNameSqlMetalSingularCamelCase;
			}
			set
			{
				this.viewNameSqlMetalSingularCamelCase = value;
			}
		}

		[XmlAttribute]
		public string ViewNameSqlMetalSingularPascalCase
		{
			get
			{
				return this.viewNameSqlMetalSingularPascalCase;
			}
			set
			{
				this.viewNameSqlMetalSingularPascalCase = value;
			}
		}

		#endregion
	}
}