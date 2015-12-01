/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections;

using TextMetal.Framework.Core;

namespace TextMetal.Framework.Sort
{
	/// <summary>
	/// Provides for sort mechanics.
	/// </summary>
	public interface ISortMechanism
	{
		#region Methods/Operators

		/// <summary>
		/// Re-orders an enumerable of values, yielding a re-ordered enumerable.
		/// </summary>
		/// <param name="templatingContext"> The templating context. </param>
		/// <param name="values"> </param>
		/// <returns> </returns>
		IEnumerable EvaluateSort(ITemplatingContext templatingContext, IEnumerable values);

		#endregion
	}
}