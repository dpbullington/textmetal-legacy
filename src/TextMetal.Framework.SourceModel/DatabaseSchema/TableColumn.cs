/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Xml.Serialization;

namespace TextMetal.Framework.SourceModel.DatabaseSchema
{
	[Serializable]
	public class TableColumn : Column, ITabularColumn
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the TableColumn class.
		/// </summary>
		public TableColumn()
		{
		}

		#endregion

		#region Fields/Constants

		private string columnCSharpIsComputedLiteral;
		private string columnCSharpIsIdentityLiteral;
		private string columnCSharpIsPrimaryKeyLiteral;
		private bool columnHasCheck;
		private bool columnHasDefault;
		private bool columnIsComputed;
		private bool columnIsIdentity;
		private bool columnIsPrimaryKey;
		private int columnPrimaryKeyOrdinal;

		#endregion

		#region Properties/Indexers/Events

		[XmlAttribute]
		public string ColumnCSharpIsComputedLiteral
		{
			get
			{
				return this.columnCSharpIsComputedLiteral;
			}
			set
			{
				this.columnCSharpIsComputedLiteral = value;
			}
		}

		[XmlAttribute]
		public string ColumnCSharpIsIdentityLiteral
		{
			get
			{
				return this.columnCSharpIsIdentityLiteral;
			}
			set
			{
				this.columnCSharpIsIdentityLiteral = value;
			}
		}

		[XmlAttribute]
		public string ColumnCSharpIsPrimaryKeyLiteral
		{
			get
			{
				return this.columnCSharpIsPrimaryKeyLiteral;
			}
			set
			{
				this.columnCSharpIsPrimaryKeyLiteral = value;
			}
		}

		[XmlAttribute]
		public bool ColumnHasCheck
		{
			get
			{
				return this.columnHasCheck;
			}
			set
			{
				this.columnHasCheck = value;
			}
		}

		[XmlAttribute]
		public bool ColumnHasDefault
		{
			get
			{
				return this.columnHasDefault;
			}
			set
			{
				this.columnHasDefault = value;
			}
		}

		[XmlAttribute]
		public bool ColumnIsComputed
		{
			get
			{
				return this.columnIsComputed;
			}
			set
			{
				this.columnIsComputed = value;
			}
		}

		[XmlAttribute]
		public bool ColumnIsIdentity
		{
			get
			{
				return this.columnIsIdentity;
			}
			set
			{
				this.columnIsIdentity = value;
			}
		}

		[XmlAttribute]
		public bool ColumnIsPrimaryKey
		{
			get
			{
				return this.columnIsPrimaryKey;
			}
			set
			{
				this.columnIsPrimaryKey = value;
			}
		}

		[XmlAttribute]
		public int ColumnPrimaryKeyOrdinal
		{
			get
			{
				return this.columnPrimaryKeyOrdinal;
			}
			set
			{
				this.columnPrimaryKeyOrdinal = value;
			}
		}

		[XmlIgnore]
		public bool IsColumnServerGeneratedPrimaryKey
		{
			get
			{
				return this.ColumnIsPrimaryKey && this.ColumnIsIdentity;
			}
		}

		#endregion
	}
}