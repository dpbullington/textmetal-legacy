/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace TextMetal.Framework.Source.DatabaseSchema
{
	public class Database
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the Database class.
		/// </summary>
		public Database()
		{
		}

		#endregion

		#region Fields/Constants

		private readonly List<Schema> schemas = new List<Schema>();
		private readonly List<Trigger> triggers = new List<Trigger>();
		private DateTime creationTimestamp;
		private int databaseId;
		private string databaseName;
		private string databaseNameCamelCase;
		private string databaseNameConstantCase;
		private string databaseNamePascalCase;
		private string databaseNamePluralCamelCase;
		private string databaseNamePluralConstantCase;
		private string databaseNamePluralPascalCase;
		private string databaseNameSingularCamelCase;
		private string databaseNameSingularConstantCase;
		private string databaseNameSingularPascalCase;

		#endregion

		#region Properties/Indexers/Events

		[XmlIgnore]
		public bool HasProcedures
		{
			get
			{
				return this.Schemas.Count(s => s.Procedures.Count() > 0) > 0;
			}
		}

		[XmlIgnore]
		public bool HasTables
		{
			get
			{
				return this.Schemas.Count(s => s._Tables.Any()) > 0;
			}
		}

		[XmlIgnore]
		public bool HasViews
		{
			get
			{
				return this.Schemas.Count(s => s.Views.Any()) > 0;
			}
		}

		[XmlArray(ElementName = "Schemas")]
		[XmlArrayItem(ElementName = "Schema")]
		public List<Schema> Schemas
		{
			get
			{
				return this.schemas;
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
		public int DatabaseId
		{
			get
			{
				return this.databaseId;
			}
			set
			{
				this.databaseId = value;
			}
		}

		[XmlAttribute]
		public string DatabaseName
		{
			get
			{
				return this.databaseName;
			}
			set
			{
				this.databaseName = value;
			}
		}

		[XmlAttribute]
		public string DatabaseNameCamelCase
		{
			get
			{
				return this.databaseNameCamelCase;
			}
			set
			{
				this.databaseNameCamelCase = value;
			}
		}

		[XmlAttribute]
		public string DatabaseNameConstantCase
		{
			get
			{
				return this.databaseNameConstantCase;
			}
			set
			{
				this.databaseNameConstantCase = value;
			}
		}

		[XmlAttribute]
		public string DatabaseNamePascalCase
		{
			get
			{
				return this.databaseNamePascalCase;
			}
			set
			{
				this.databaseNamePascalCase = value;
			}
		}

		[XmlAttribute]
		public string DatabaseNamePluralCamelCase
		{
			get
			{
				return this.databaseNamePluralCamelCase;
			}
			set
			{
				this.databaseNamePluralCamelCase = value;
			}
		}

		[XmlAttribute]
		public string DatabaseNamePluralConstantCase
		{
			get
			{
				return this.databaseNamePluralConstantCase;
			}
			set
			{
				this.databaseNamePluralConstantCase = value;
			}
		}

		[XmlAttribute]
		public string DatabaseNamePluralPascalCase
		{
			get
			{
				return this.databaseNamePluralPascalCase;
			}
			set
			{
				this.databaseNamePluralPascalCase = value;
			}
		}

		[XmlAttribute]
		public string DatabaseNameSingularCamelCase
		{
			get
			{
				return this.databaseNameSingularCamelCase;
			}
			set
			{
				this.databaseNameSingularCamelCase = value;
			}
		}

		[XmlAttribute]
		public string DatabaseNameSingularConstantCase
		{
			get
			{
				return this.databaseNameSingularConstantCase;
			}
			set
			{
				this.databaseNameSingularConstantCase = value;
			}
		}

		[XmlAttribute]
		public string DatabaseNameSingularPascalCase
		{
			get
			{
				return this.databaseNameSingularPascalCase;
			}
			set
			{
				this.databaseNameSingularPascalCase = value;
			}
		}

		#endregion
	}
}