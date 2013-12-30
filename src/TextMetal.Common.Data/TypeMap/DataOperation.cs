/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Common.Data.TypeMap
{
	/// <summary>
	/// Indicates standard data operations.
	/// </summary>
	[Serializable]
	public enum DataOperation : uint
	{
		/// <summary>
		/// Perform NO data operation.
		/// </summary>
		None = 0x00000000,

		/// <summary>
		/// Perform an INSERT data operation.
		/// </summary>
		Insert = 1,

		/// <summary>
		/// Perform an UPDATE data operation.
		/// </summary>
		Update = 2,

		/// <summary>
		/// Perform a DELETE data operation.
		/// </summary>
		Delete = 3,

		/// <summary>
		/// Perform a SELECT-all data operation.
		/// </summary>
		SelectAll = 4,

		/// <summary>
		/// Perform a SELECT-one data operation.
		/// </summary>
		SelectOne = 5,

		/// <summary>
		/// Perform a SELECT-for data operation.
		/// </summary>
		SelectFor = 6,

		/// <summary>
		/// Perform a SELECT-#id data operation.
		/// </summary>
		SelectId = 7,

		/// <summary>
		/// A concurrency conflict resulted from a previous data operation.
		/// </summary>
		ChangeConflict = 0xFFFFFFFE,

		/// <summary>
		/// An invalid object state was encountered.
		/// </summary>
		StateError = 0xFFFFFFFF
	}
}