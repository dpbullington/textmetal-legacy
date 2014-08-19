/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Data;

namespace TextMetal.Common.Data.Framework.Mapping
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class ColumnMappingAttribute : Attribute
	{
		#region Constructors/Destructors

		public ColumnMappingAttribute()
		{
		}

		#endregion

		#region Fields/Constants

		private bool columnIsPrimaryKey;
		private bool columnIsReadOnly;
		private string columnName;
		private bool columnNullable;
		private int columnOrdinal;
		private byte columnPrecision;
		private byte columnScale;
		private int columnSize;
		private DbType dbType;

		#endregion

		#region Properties/Indexers/Events

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

		public bool ColumnIsReadOnly
		{
			get
			{
				return this.columnIsReadOnly;
			}
			set
			{
				this.columnIsReadOnly = value;
			}
		}

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

		public byte ColumnPrecision
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

		public byte ColumnScale
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

		public DbType DbType
		{
			get
			{
				return this.dbType;
			}
			set
			{
				this.dbType = value;
			}
		}

		#endregion
	}
}