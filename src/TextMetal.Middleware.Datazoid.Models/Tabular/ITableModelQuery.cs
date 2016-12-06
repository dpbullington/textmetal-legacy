/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Linq.Expressions;

namespace TextMetal.Middleware.Datazoid.Models.Tabular
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