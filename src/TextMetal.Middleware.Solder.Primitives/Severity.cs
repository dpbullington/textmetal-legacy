﻿/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

namespace TextMetal.Middleware.Solder.Primitives
{
	/// <summary>
	/// Specifies the severity of a message.
	/// </summary>
	public enum Severity : uint
	{
		/// <summary>
		/// None.
		/// </summary>
		None = 0,

		/// <summary>
		/// Information.
		/// </summary>
		Information = 1,

		/// <summary>
		/// Warning.
		/// </summary>
		Warning = 2,

		/// <summary>
		/// Error.
		/// </summary>
		Error = 3,

		/// <summary>
		/// Hit.
		/// </summary>
		Hit = 0xF0F0F0F0,

		/// <summary>
		/// Debug.
		/// </summary>
		Debug = 0xFFFFFFFF
	}
}