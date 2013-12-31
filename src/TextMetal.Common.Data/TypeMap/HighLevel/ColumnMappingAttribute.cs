/*
	Copyright ©2002-2014 Daniel Bullington
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Data;
using System.Reflection;

namespace TextMetal.Common.Data.TypeMap.HighLevel
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

		private DbType dbType;
		private int foreignKeySelector;
		private Type foreignKeyType;
		private bool isForeignKey;
		private bool isPrimaryKey;
		private bool isReadOnly;
		private string name;
		private bool nullable;
		private int ordinal;
		private byte precision;
		private string previousVersionPath;
		private byte scale;
		private int size;
		private PropertyInfo targetProperty;
		private bool useInConcurrencyCheck;

		#endregion

		#region Properties/Indexers/Events

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

		public int ForeignKeySelector
		{
			get
			{
				return this.foreignKeySelector;
			}
			set
			{
				this.foreignKeySelector = value;
			}
		}

		public Type ForeignKeyType
		{
			get
			{
				return this.foreignKeyType;
			}
			set
			{
				this.foreignKeyType = value;
			}
		}

		public bool IsForeignKey
		{
			get
			{
				return this.isForeignKey;
			}
			set
			{
				this.isForeignKey = value;
			}
		}

		public bool IsPrimaryKey
		{
			get
			{
				return this.isPrimaryKey;
			}
			set
			{
				this.isPrimaryKey = value;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return this.isReadOnly;
			}
			set
			{
				this.isReadOnly = value;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		/// <summary>
		/// Gets or sets the nullable-ness of the column mapping.
		/// </summary>
		public bool Nullable
		{
			get
			{
				return this.nullable;
			}
			set
			{
				this.nullable = value;
			}
		}

		public int Ordinal
		{
			get
			{
				return this.ordinal;
			}
			set
			{
				this.ordinal = value;
			}
		}

		/// <summary>
		/// Gets or sets the precision of the column mapping.
		/// </summary>
		public byte Precision
		{
			get
			{
				return this.precision;
			}
			set
			{
				this.precision = value;
			}
		}

		public string PreviousVersionPath
		{
			get
			{
				return this.previousVersionPath;
			}
			set
			{
				this.previousVersionPath = value;
			}
		}

		/// <summary>
		/// Gets or sets the scale of the column mapping.
		/// </summary>
		public byte Scale
		{
			get
			{
				return this.scale;
			}
			set
			{
				this.scale = value;
			}
		}

		public int Size
		{
			get
			{
				return this.size;
			}
			set
			{
				this.size = value;
			}
		}

		public PropertyInfo TargetProperty
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

		public bool UseInConcurrencyCheck
		{
			get
			{
				return this.useInConcurrencyCheck;
			}
			set
			{
				this.useInConcurrencyCheck = value;
			}
		}

		#endregion
	}
}