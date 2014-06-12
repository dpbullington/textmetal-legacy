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
	public class View
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

		[XmlArray(ElementName = "Columns")]
		[XmlArrayItem(ElementName = "Column")]
		public List<ViewColumn> Columns
		{
			get
			{
				return this.columns;
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