/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace TextMetal.Framework.Source.DatabaseSchema
{
	[Serializable]
	public class ProcedureResultset
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the ProcedureResultset class.
		/// </summary>
		public ProcedureResultset()
		{
		}

		#endregion

		#region Fields/Constants

		private readonly List<ProcedureColumn> columns = new List<ProcedureColumn>();
		private int resultsetIndex;

		#endregion

		#region Properties/Indexers/Events

		[XmlArray(ElementName = "Columns")]
		[XmlArrayItem(ElementName = "Column")]
		public List<ProcedureColumn> Columns
		{
			get
			{
				return this.columns;
			}
		}

		[XmlIgnore]
		public bool HasAnyMappedResultColumns
		{
			get
			{
				return this.Columns.Any();
			}
		}

		[XmlAttribute]
		public int ResultsetIndex
		{
			get
			{
				return this.resultsetIndex;
			}
			set
			{
				this.resultsetIndex = value;
			}
		}

		#endregion
	}
}