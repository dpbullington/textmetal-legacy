﻿/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Xml.Serialization;

namespace TextMetal.Framework.Source.DatabaseSchema
{
	public class ForeignKeyColumn
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the ForeignKeyColumn class.
		/// </summary>
		public ForeignKeyColumn()
		{
		}

		#endregion

		#region Fields/Constants

		private string columnName;
		private int columnOrdinal;
		private int foreignKeyColumnOrdinal;
		private string targetColumnName;
		private int targetColumnOrdinal;

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
		public int ForeignKeyColumnOrdinal
		{
			get
			{
				return this.foreignKeyColumnOrdinal;
			}
			set
			{
				this.foreignKeyColumnOrdinal = value;
			}
		}

		[XmlAttribute]
		public string TargetColumnName
		{
			get
			{
				return this.targetColumnName;
			}
			set
			{
				this.targetColumnName = value;
			}
		}

		[XmlAttribute]
		public int TargetColumnOrdinal
		{
			get
			{
				return this.targetColumnOrdinal;
			}
			set
			{
				this.targetColumnOrdinal = value;
			}
		}

		#endregion
	}
}