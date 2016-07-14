/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Middleware.Solder.Primitives
{
	public sealed class Column : IColumn
	{
		#region Constructors/Destructors

		public Column()
		{
		}

		#endregion

		#region Fields/Constants

		private int columnIndex;
		private bool? columnIsNullable;
		private string columnName;
		private Type columnType;
		private object context;
		private int tableIndex;

		#endregion

		#region Properties/Indexers/Events

		public int ColumnIndex
		{
			get
			{
				return this.columnIndex;
			}
			set
			{
				this.columnIndex = value;
			}
		}

		public bool? ColumnIsNullable
		{
			get
			{
				return this.columnIsNullable;
			}
			set
			{
				this.columnIsNullable = value;
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

		public Type ColumnType
		{
			get
			{
				return this.columnType;
			}
			set
			{
				this.columnType = value;
			}
		}

		public object Context
		{
			get
			{
				return this.context;
			}
			set
			{
				this.context = value;
			}
		}

		public int TableIndex
		{
			get
			{
				return this.tableIndex;
			}
			set
			{
				this.tableIndex = value;
			}
		}

		#endregion
	}
}