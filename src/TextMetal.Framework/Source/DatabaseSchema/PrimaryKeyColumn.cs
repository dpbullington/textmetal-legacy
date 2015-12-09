/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Xml.Serialization;

using TextMetal.Middleware.Solder.Runtime;

namespace TextMetal.Framework.Source.DatabaseSchema
{
	[FxSpackleTypes.SerializableAttribute]
	public class PrimaryKeyColumn
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the PrimaryKeyColumn class.
		/// </summary>
		public PrimaryKeyColumn()
		{
		}

		#endregion

		#region Fields/Constants

		private string columnName;
		private int columnOrdinal;
		private int primaryKeyColumnOrdinal;

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
		public int PrimaryKeyColumnOrdinal
		{
			get
			{
				return this.primaryKeyColumnOrdinal;
			}
			set
			{
				this.primaryKeyColumnOrdinal = value;
			}
		}

		#endregion
	}
}