/*
	Copyright �2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Linq.Expressions;

namespace TextMetal.Middleware.Data.Models
{
	/// <summary>
	/// Provides a contract for model queries (classic expression trees, prototypes, LINQ, etc.).
	/// </summary
	public interface ITableModelQuery
	{
		#region Methods/Operators

		Expression GetCanonicalExpression();

		#endregion
	}
}