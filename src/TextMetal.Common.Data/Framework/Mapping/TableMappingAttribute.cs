/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

namespace TextMetal.Common.Data.Framework.Mapping
{
	[AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
	public sealed class TableMappingAttribute : Attribute
	{
		#region Constructors/Destructors

		public TableMappingAttribute()
		{
		}

		#endregion

		#region Fields/Constants

		private readonly IList<ColumnMappingAttribute> columnMappingAttributes = new List<ColumnMappingAttribute>();

		private string databaseName;
		private bool isView;
		private string schemaName;
		private string tableName;
		private Type targetType;

		#endregion

		#region Properties/Indexers/Events

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

		public bool IsView
		{
			get
			{
				return this.isView;
			}
			set
			{
				this.isView = value;
			}
		}

		public string SchemaName
		{
			get
			{
				return this.schemaName;
			}
			set
			{
				this.schemaName = value;
			}
		}

		public string TableName
		{
			get
			{
				return this.tableName;
			}
			set
			{
				this.tableName = value;
			}
		}

		public IList<ColumnMappingAttribute> _ColumnMappingAttributes
		{
			get
			{
				return this.columnMappingAttributes;
			}
		}

		public Type _TargetType
		{
			get
			{
				return this.targetType;
			}
			set
			{
				this.targetType = value;
			}
		}

		#endregion
	}
}