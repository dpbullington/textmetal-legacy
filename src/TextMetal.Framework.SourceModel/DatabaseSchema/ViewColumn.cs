/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Xml.Serialization;

namespace TextMetal.Framework.SourceModel.DatabaseSchema
{
	[Serializable]
	public class ViewColumn : Column, ITabularColumn
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the ViewColumn class.
		/// </summary>
		public ViewColumn()
		{
		}

		#endregion

		#region Properties/Indexers/Events

		[Obsolete("Provided for model breaking change compatability only.")]
		[XmlIgnore]
		public string ColumnCSharpIsComputedLiteral
		{
			get
			{
				return false.ToString().ToLower();
			}
		}

		[Obsolete("Provided for model breaking change compatability only.")]
		[XmlIgnore]
		public string ColumnCSharpIsIdentityLiteral
		{
			get
			{
				return false.ToString().ToLower();
			}
		}

		[Obsolete("Provided for model breaking change compatability only.")]
		[XmlIgnore]
		public string ColumnCSharpIsPrimaryKeyLiteral
		{
			get
			{
				return false.ToString().ToLower();
			}
		}

		[Obsolete("Provided for model breaking change compatability only.")]
		[XmlIgnore]
		public bool ColumnHasCheck
		{
			get
			{
				return false;
			}
		}

		[Obsolete("Provided for model breaking change compatability only.")]
		[XmlIgnore]
		public bool ColumnHasDefault
		{
			get
			{
				return false;
			}
		}

		[Obsolete("Provided for model breaking change compatability only.")]
		[XmlIgnore]
		public bool ColumnIsComputed
		{
			get
			{
				return false;
			}
		}

		[Obsolete("Provided for model breaking change compatability only.")]
		[XmlIgnore]
		public bool ColumnIsIdentity
		{
			get
			{
				return false;
			}
		}

		[Obsolete("Provided for model breaking change compatability only.")]
		[XmlIgnore]
		public bool ColumnIsPrimaryKey
		{
			get
			{
				return false;
			}
		}

		[Obsolete("Provided for model breaking change compatability only.")]
		[XmlIgnore]
		public int ColumnPrimaryKeyOrdinal
		{
			get
			{
				return 0;
			}
		}

		[Obsolete("Provided for model breaking change compatability only.")]
		[XmlIgnore]
		public bool IsColumnServerGeneratedPrimaryKey
		{
			get
			{
				return false;
			}
		}

		#endregion
	}
}