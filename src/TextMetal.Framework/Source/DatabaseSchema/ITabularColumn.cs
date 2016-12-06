/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Framework.Source.DatabaseSchema
{
	[Obsolete("Provided for model breaking change compatability only.")]
	public interface ITabularColumn
	{
		#region Properties/Indexers/Events

		string ColumnCSharpIsComputedLiteral
		{
			get;
		}

		string ColumnCSharpIsIdentityLiteral
		{
			get;
		}

		string ColumnCSharpIsPrimaryKeyLiteral
		{
			get;
		}

		bool ColumnHasCheck
		{
			get;
		}

		bool ColumnHasDefault
		{
			get;
		}

		bool ColumnIsComputed
		{
			get;
		}

		bool ColumnIsIdentity
		{
			get;
		}

		bool ColumnIsPrimaryKey
		{
			get;
		}

		int ColumnPrimaryKeyOrdinal
		{
			get;
		}

		bool IsColumnServerGeneratedPrimaryKey
		{
			get;
		}

		#endregion
	}
}