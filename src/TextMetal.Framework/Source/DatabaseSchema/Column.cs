/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Serialization;

using TextMetal.Middleware.Solder.Utilities;

namespace TextMetal.Framework.Source.DatabaseSchema
{
	public abstract class Column
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the Column class.
		/// </summary>
		protected Column()
		{
		}

		#endregion

		#region Fields/Constants

		private Type columnClrNonNullableType;
		private Type columnClrNullableType;
		private Type columnClrType;
		private string columnCSharpClrNonNullableType;
		private string columnCSharpClrNullableType;
		private string columnCSharpClrType;
		private string columnCSharpDbType;
		private string columnCSharpIsAnonymousLiteral;
		private string columnCSharpNullableLiteral;
		private DbType columnDbType;
		private bool columnIsAnonymous;
		private bool columnIsUserDefinedType;
		private string columnName;
		private string columnNameCamelCase;
		private string columnNameConstantCase;
		private string columnNamePascalCase;
		private string columnNamePluralCamelCase;
		private string columnNamePluralConstantCase;
		private string columnNamePluralPascalCase;
		private string columnNameSingularCamelCase;
		private string columnNameSingularConstantCase;
		private string columnNameSingularPascalCase;
		private bool columnNullable;
		private int columnOrdinal;
		private int columnPrecision;
		private int columnScale;
		private int columnSize;
		private string columnSqlType;

		#endregion

		#region Properties/Indexers/Events

		[XmlAttribute("ColumnClrNonNullableType")]
		public string _ColumnClrNotNullableType
		{
			get
			{
				return this.ColumnClrNonNullableType.SafeToString();
			}
			set
			{
				if (DataTypeFascade.Instance.IsNullOrWhiteSpace(value))
					this.ColumnClrNonNullableType = null;
				else
					this.ColumnClrNonNullableType = Type.GetType(value, false);
			}
		}

		[XmlAttribute("ColumnClrNullableType")]
		public string _ColumnClrNullableType
		{
			get
			{
				return this.ColumnClrNullableType.SafeToString();
			}
			set
			{
				if (DataTypeFascade.Instance.IsNullOrWhiteSpace(value))
					this.ColumnClrNullableType = null;
				else
					this.ColumnClrNullableType = Type.GetType(value, false);
			}
		}

		[XmlAttribute("ColumnClrType")]
		public string _ColumnClrType
		{
			get
			{
				return this.ColumnClrType.SafeToString();
			}
			set
			{
				if (DataTypeFascade.Instance.IsNullOrWhiteSpace(value))
					this.ColumnClrType = null;
				else
					this.ColumnClrType = Type.GetType(value, false);
			}
		}

		[XmlIgnore]
		public Type ColumnClrNonNullableType
		{
			get
			{
				return this.columnClrNonNullableType;
			}
			set
			{
				this.columnClrNonNullableType = value;
			}
		}

		[XmlIgnore]
		public Type ColumnClrNullableType
		{
			get
			{
				return this.columnClrNullableType;
			}
			set
			{
				this.columnClrNullableType = value;
			}
		}

		[XmlIgnore]
		public Type ColumnClrType
		{
			get
			{
				return this.columnClrType;
			}
			set
			{
				this.columnClrType = value;
			}
		}

		[XmlAttribute]
		public string ColumnCSharpClrNonNullableType
		{
			get
			{
				return this.columnCSharpClrNonNullableType;
			}
			set
			{
				this.columnCSharpClrNonNullableType = value;
			}
		}

		[XmlAttribute]
		public string ColumnCSharpClrNullableType
		{
			get
			{
				return this.columnCSharpClrNullableType;
			}
			set
			{
				this.columnCSharpClrNullableType = value;
			}
		}

		[XmlAttribute]
		public string ColumnCSharpClrType
		{
			get
			{
				return this.columnCSharpClrType;
			}
			set
			{
				this.columnCSharpClrType = value;
			}
		}

		[XmlAttribute]
		public string ColumnCSharpDbType
		{
			get
			{
				return this.columnCSharpDbType;
			}
			set
			{
				this.columnCSharpDbType = value;
			}
		}

		[XmlAttribute]
		public string ColumnCSharpIsAnonymousLiteral
		{
			get
			{
				return this.columnCSharpIsAnonymousLiteral;
			}
			set
			{
				this.columnCSharpIsAnonymousLiteral = value;
			}
		}

		[XmlAttribute]
		public string ColumnCSharpNullableLiteral
		{
			get
			{
				return this.columnCSharpNullableLiteral;
			}
			set
			{
				this.columnCSharpNullableLiteral = value;
			}
		}

		[XmlAttribute]
		public DbType ColumnDbType
		{
			get
			{
				return this.columnDbType;
			}
			set
			{
				this.columnDbType = value;
			}
		}

		[XmlAttribute]
		public bool ColumnIsAnonymous
		{
			get
			{
				return this.columnIsAnonymous;
			}
			set
			{
				this.columnIsAnonymous = value;
			}
		}

		[XmlAttribute]
		public bool ColumnIsUserDefinedType
		{
			get
			{
				return this.columnIsUserDefinedType;
			}
			set
			{
				this.columnIsUserDefinedType = value;
			}
		}

		[XmlAttribute]
		public string ColumnName
		{
			get
			{
				return this.columnName;
			}
			set
			{
				this.columnName = value;
			}
		}

		[XmlAttribute]
		public string ColumnNameCamelCase
		{
			get
			{
				return this.columnNameCamelCase;
			}
			set
			{
				this.columnNameCamelCase = value;
			}
		}

		[XmlAttribute]
		public string ColumnNameConstantCase
		{
			get
			{
				return this.columnNameConstantCase;
			}
			set
			{
				this.columnNameConstantCase = value;
			}
		}

		[XmlAttribute]
		public string ColumnNamePascalCase
		{
			get
			{
				return this.columnNamePascalCase;
			}
			set
			{
				this.columnNamePascalCase = value;
			}
		}

		[XmlAttribute]
		public string ColumnNamePluralCamelCase
		{
			get
			{
				return this.columnNamePluralCamelCase;
			}
			set
			{
				this.columnNamePluralCamelCase = value;
			}
		}

		[XmlAttribute]
		public string ColumnNamePluralConstantCase
		{
			get
			{
				return this.columnNamePluralConstantCase;
			}
			set
			{
				this.columnNamePluralConstantCase = value;
			}
		}

		[XmlAttribute]
		public string ColumnNamePluralPascalCase
		{
			get
			{
				return this.columnNamePluralPascalCase;
			}
			set
			{
				this.columnNamePluralPascalCase = value;
			}
		}

		[XmlAttribute]
		public string ColumnNameSingularCamelCase
		{
			get
			{
				return this.columnNameSingularCamelCase;
			}
			set
			{
				this.columnNameSingularCamelCase = value;
			}
		}

		[XmlAttribute]
		public string ColumnNameSingularConstantCase
		{
			get
			{
				return this.columnNameSingularConstantCase;
			}
			set
			{
				this.columnNameSingularConstantCase = value;
			}
		}

		[XmlAttribute]
		public string ColumnNameSingularPascalCase
		{
			get
			{
				return this.columnNameSingularPascalCase;
			}
			set
			{
				this.columnNameSingularPascalCase = value;
			}
		}

		[XmlAttribute]
		public bool ColumnNullable
		{
			get
			{
				return this.columnNullable;
			}
			set
			{
				this.columnNullable = value;
			}
		}

		[XmlAttribute]
		public int ColumnOrdinal
		{
			get
			{
				return this.columnOrdinal;
			}
			set
			{
				this.columnOrdinal = value;
			}
		}

		[XmlAttribute]
		public int ColumnPrecision
		{
			get
			{
				return this.columnPrecision;
			}
			set
			{
				this.columnPrecision = value;
			}
		}

		[XmlAttribute]
		public int ColumnScale
		{
			get
			{
				return this.columnScale;
			}
			set
			{
				this.columnScale = value;
			}
		}

		[XmlAttribute]
		public int ColumnSize
		{
			get
			{
				return this.columnSize;
			}
			set
			{
				this.columnSize = value;
			}
		}

		[XmlAttribute]
		public string ColumnSqlType
		{
			get
			{
				return this.columnSqlType;
			}
			set
			{
				this.columnSqlType = value;
			}
		}

		#endregion

		#region Methods/Operators

		public static IEnumerable<IDictionary<string, object>> FixupDuplicateColumns(IEnumerable<IDictionary<string, object>> records)
		{
			IEnumerable<IGrouping<string, IDictionary<string, object>>> groups;

			object columnOrdinal;
			object columnName;
			bool isDuplicateColumn;
			bool isAnonymousColumn;

			if ((object)records == null)
				throw new ArgumentNullException(nameof(records));

			groups = records.GroupBy(record => (string)record[SchemaInfoConstants.COLUMN_NAME_RECORD_KEY]);

			foreach (var group in groups)
			{
				isDuplicateColumn = group.Count() > 1;

				foreach (IDictionary<string, object> record in group)
				{
					record.TryGetValue(SchemaInfoConstants.COLUMN_ORDINAL_RECORD_KEY, out columnOrdinal);
					record.TryGetValue(SchemaInfoConstants.COLUMN_NAME_RECORD_KEY, out columnName);

					isAnonymousColumn = DataTypeFascade.Instance.IsNullOrEmpty(columnName.ChangeType<string>());

					if (isAnonymousColumn || isDuplicateColumn)
					{
						if ((object)columnOrdinal != null)
							columnName = string.Format("Column_{0:0000}", columnOrdinal.ChangeType<int>());
						else
							columnName = string.Format("Column_{0:N}", Guid.NewGuid());

						if (record.ContainsKey(SchemaInfoConstants.COLUMN_NAME_RECORD_KEY))
							record.Remove(SchemaInfoConstants.COLUMN_NAME_RECORD_KEY);

						record.Add(SchemaInfoConstants.COLUMN_NAME_RECORD_KEY, columnName);

						if (record.ContainsKey(SchemaInfoConstants.COLUMN_IS_ANONYMOUS_RECORD_KEY))
							record.Remove(SchemaInfoConstants.COLUMN_IS_ANONYMOUS_RECORD_KEY);

						record.Add(SchemaInfoConstants.COLUMN_IS_ANONYMOUS_RECORD_KEY, true);
					}
				}
			}

			return records;
		}

		#endregion
	}
}