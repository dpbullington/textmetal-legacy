/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Middleware.Solder.Primitives
{
	public interface IColumn
	{
		#region Properties/Indexers/Events

		int ColumnIndex
		{
			get;
		}

		bool? ColumnIsNullable
		{
			get;
		}

		string ColumnName
		{
			get;
		}

		Type ColumnType
		{
			get;
		}

		object Context
		{
			get;
		}

		int TableIndex
		{
			get;
		}

		#endregion
	}
}