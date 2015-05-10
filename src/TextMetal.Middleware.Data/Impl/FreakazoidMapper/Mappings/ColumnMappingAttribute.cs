/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Data;
using System.Reflection;

namespace TextMetal.Middleware.Data.Impl.FreakazoidMapper.Mappings
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

		private DbType columnDbType;
		private bool columnIsAnonymous;
		private bool columnIsComputed;
		private bool columnIsIdentity;
		private bool columnIsPrimaryKey;
		private string columnName;
		private bool columnNullable;
		private int columnOrdinal;
		private byte columnPrecision;
		private byte columnScale;
		private int columnSize;
		private string columnSqlType;
		private PropertyInfo targetProperty;

		#endregion

		#region Properties/Indexers/Events

		public bool IsColumnServerGeneratedPrimaryKey
		{
			get
			{
				return this.ColumnIsPrimaryKey && this.ColumnIsIdentity;
			}
		}

		public PropertyInfo _TargetProperty
		{
			get
			{
				return this.targetProperty;
			}
			set
			{
				this.targetProperty = value;
			}
		}

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
	}
}