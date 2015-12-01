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
	public class Procedure
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the Procedure class.
		/// </summary>
		public Procedure()
		{
		}

		#endregion

		#region Fields/Constants

		private readonly List<Parameter> parameters = new List<Parameter>();
		private readonly List<ProcedureResultset> resultsets = new List<ProcedureResultset>();
		private string procedureExecuteSchemaExceptionText;
		private bool procedureExecuteSchemaThrewException;
		private string procedureName;
		private string procedureNameCamelCase;
		private string procedureNameConstantCase;
		private string procedureNamePascalCase;
		private string procedureNamePluralCamelCase;
		private string procedureNamePluralConstantCase;
		private string procedureNamePluralPascalCase;
		private string procedureNameSingularCamelCase;
		private string procedureNameSingularConstantCase;
		private string procedureNameSingularPascalCase;
		private string procedureNameSqlMetalCamelCase;
		private string procedureNameSqlMetalPascalCase;
		private string procedureNameSqlMetalPluralCamelCase;
		private string procedureNameSqlMetalPluralPascalCase;
		private string procedureNameSqlMetalSingularCamelCase;
		private string procedureNameSqlMetalSingularPascalCase;

		#endregion

		#region Properties/Indexers/Events

		[Obsolete("Provided for model breaking change compatability only.")]
		[XmlIgnore]
		public List<ProcedureColumn> Columns
		{
			get
			{
				ProcedureResultset procedureResultset;

				procedureResultset = this.Resultsets.SingleOrDefault(rs => rs.ResultsetIndex == 0);

				if ((object)procedureResultset == null)
					return null;

				return procedureResultset.Columns;
			}
		}

		[Obsolete("Provided for model breaking change compatability only.")]
		[XmlIgnore]
		public bool HasAnyMappedResultColumns
		{
			get
			{
				if ((object)this.Columns == null)
					return false;

				return this.Columns.Any();
			}
		}

		[XmlArray(ElementName = "Parameters")]
		[XmlArrayItem(ElementName = "Parameter")]
		public List<Parameter> Parameters
		{
			get
			{
				return this.parameters;
			}
		}

		[XmlArray(ElementName = "Resultsets")]
		[XmlArrayItem(ElementName = "Resultset")]
		public List<ProcedureResultset> Resultsets
		{
			get
			{
				return this.resultsets;
			}
		}

		[XmlAttribute]
		public string ProcedureExecuteSchemaExceptionText
		{
			get
			{
				return this.procedureExecuteSchemaExceptionText;
			}
			set
			{
				this.procedureExecuteSchemaExceptionText = value;
			}
		}

		[XmlAttribute]
		public bool ProcedureExecuteSchemaThrewException
		{
			get
			{
				return this.procedureExecuteSchemaThrewException;
			}
			set
			{
				this.procedureExecuteSchemaThrewException = value;
			}
		}

		[XmlAttribute]
		public string ProcedureName
		{
			get
			{
				return this.procedureName;
			}
			set
			{
				this.procedureName = value;
			}
		}

		[XmlAttribute]
		public string ProcedureNameCamelCase
		{
			get
			{
				return this.procedureNameCamelCase;
			}
			set
			{
				this.procedureNameCamelCase = value;
			}
		}

		[XmlAttribute]
		public string ProcedureNameConstantCase
		{
			get
			{
				return this.procedureNameConstantCase;
			}
			set
			{
				this.procedureNameConstantCase = value;
			}
		}

		[XmlAttribute]
		public string ProcedureNamePascalCase
		{
			get
			{
				return this.procedureNamePascalCase;
			}
			set
			{
				this.procedureNamePascalCase = value;
			}
		}

		[XmlAttribute]
		public string ProcedureNamePluralCamelCase
		{
			get
			{
				return this.procedureNamePluralCamelCase;
			}
			set
			{
				this.procedureNamePluralCamelCase = value;
			}
		}

		[XmlAttribute]
		public string ProcedureNamePluralConstantCase
		{
			get
			{
				return this.procedureNamePluralConstantCase;
			}
			set
			{
				this.procedureNamePluralConstantCase = value;
			}
		}

		[XmlAttribute]
		public string ProcedureNamePluralPascalCase
		{
			get
			{
				return this.procedureNamePluralPascalCase;
			}
			set
			{
				this.procedureNamePluralPascalCase = value;
			}
		}

		[XmlAttribute]
		public string ProcedureNameSingularCamelCase
		{
			get
			{
				return this.procedureNameSingularCamelCase;
			}
			set
			{
				this.procedureNameSingularCamelCase = value;
			}
		}

		[XmlAttribute]
		public string ProcedureNameSingularConstantCase
		{
			get
			{
				return this.procedureNameSingularConstantCase;
			}
			set
			{
				this.procedureNameSingularConstantCase = value;
			}
		}

		[XmlAttribute]
		public string ProcedureNameSingularPascalCase
		{
			get
			{
				return this.procedureNameSingularPascalCase;
			}
			set
			{
				this.procedureNameSingularPascalCase = value;
			}
		}

		[XmlAttribute]
		public string ProcedureNameSqlMetalCamelCase
		{
			get
			{
				return this.procedureNameSqlMetalCamelCase;
			}
			set
			{
				this.procedureNameSqlMetalCamelCase = value;
			}
		}

		[XmlAttribute]
		public string ProcedureNameSqlMetalPascalCase
		{
			get
			{
				return this.procedureNameSqlMetalPascalCase;
			}
			set
			{
				this.procedureNameSqlMetalPascalCase = value;
			}
		}

		[XmlAttribute]
		public string ProcedureNameSqlMetalPluralCamelCase
		{
			get
			{
				return this.procedureNameSqlMetalPluralCamelCase;
			}
			set
			{
				this.procedureNameSqlMetalPluralCamelCase = value;
			}
		}

		[XmlAttribute]
		public string ProcedureNameSqlMetalPluralPascalCase
		{
			get
			{
				return this.procedureNameSqlMetalPluralPascalCase;
			}
			set
			{
				this.procedureNameSqlMetalPluralPascalCase = value;
			}
		}

		[XmlAttribute]
		public string ProcedureNameSqlMetalSingularCamelCase
		{
			get
			{
				return this.procedureNameSqlMetalSingularCamelCase;
			}
			set
			{
				this.procedureNameSqlMetalSingularCamelCase = value;
			}
		}

		[XmlAttribute]
		public string ProcedureNameSqlMetalSingularPascalCase
		{
			get
			{
				return this.procedureNameSqlMetalSingularPascalCase;
			}
			set
			{
				this.procedureNameSqlMetalSingularPascalCase = value;
			}
		}

		#endregion
	}
}