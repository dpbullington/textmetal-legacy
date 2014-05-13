/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Common.Data.Framework
{
	/// <summary>
	/// Provides a contract for model queries (trees, LINQ, etc.).
	/// </summary
	public interface IModelQuery
	{
		#region Methods/Operators

		object GetNativeReduction();

		#endregion
	}
}