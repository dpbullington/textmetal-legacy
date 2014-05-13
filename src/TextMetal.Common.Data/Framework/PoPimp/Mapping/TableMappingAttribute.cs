/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Common.Data.Framework.PoPimp.Mapping
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

		private string databaseName;
		private bool isView;
		private string schemaName;
		private string tableName;

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

		#endregion
	}
}