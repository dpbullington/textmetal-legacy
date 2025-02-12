﻿/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Xml.Serialization;

namespace TextMetal.Framework.Source.DatabaseSchema
{
	public class ProcedureColumn : Column
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the ProcedureColumn class.
		/// </summary>
		public ProcedureColumn()
		{
		}

		#endregion

		#region Fields/Constants

		private bool columnHasDefault;

		#endregion

		#region Properties/Indexers/Events

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

		#endregion
	}
}