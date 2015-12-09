/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

using TextMetal.Middleware.Solder.Runtime;

namespace TextMetal.Framework.Source.DatabaseSchema
{
	[FxSpackleTypes.SerializableAttribute]
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

		#endregion

		#region Properties/Indexers/Events

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

		[Obsolete("Provided for model breaking change compatability only.")]
		[XmlIgnore]
		public string CSharpIsViewLiteral
		{
			get
			{
				return this.IsView.ToString().ToLower();
			}
		}

		[Obsolete("Provided for model breaking change compatability only.")]
		[XmlIgnore]
		public bool HasAnyMappedColumns
		{
			get
			{
				return this.Columns.Any();
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

		[Obsolete("Provided for model breaking change compatability only.")]
		[XmlIgnore]
		public bool IsView
		{
			get
			{
				return true;
			}
		}

		[Obsolete("Provided for model breaking change compatability only.")]
		[XmlIgnore]
		public string PrimaryKeyName
		{
			get
			{
				return null;
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

		#endregion
	}
}