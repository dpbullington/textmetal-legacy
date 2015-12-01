/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Xml.Serialization;

namespace TextMetal.Framework.Source.DatabaseSchema
{
	[Serializable]
	public class UniqueKeyColumn
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the UniqueKeyColumn class.
		/// </summary>
		public UniqueKeyColumn()
		{
		}

		#endregion

		#region Fields/Constants

		private string columnName;
		private int columnOrdinal;
		private bool uniqueKeyColumnDescendingSort;
		private int uniqueKeyColumnOrdinal;

		#endregion

		#region Properties/Indexers/Events

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
		public bool UniqueKeyColumnDescendingSort
		{
			get
			{
				return this.uniqueKeyColumnDescendingSort;
			}
			set
			{
				this.uniqueKeyColumnDescendingSort = value;
			}
		}

		[XmlAttribute]
		public int UniqueKeyColumnOrdinal
		{
			get
			{
				return this.uniqueKeyColumnOrdinal;
			}
			set
			{
				this.uniqueKeyColumnOrdinal = value;
			}
		}

		#endregion
	}
}