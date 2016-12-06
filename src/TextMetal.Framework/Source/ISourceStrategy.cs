/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using TextMetal.Framework.Core;

namespace TextMetal.Framework.Source
{
	/// <summary>
	/// Provides a strategy pattern around aquiring source objects.
	/// </summary>
	public interface ISourceStrategy
	{
		#region Methods/Operators

		/// <summary>
		/// Gets the source object.
		/// </summary>
		/// <param name="templatingContext"> The templating context. </param>
		/// <param name="sourceFilePath"> The source file path or lossely, a URI to the source repository (e.g. database). </param>
		/// <param name="properties"> A list of arbitrary properties (key/value pairs). </param>
		/// <returns> An source object instance or null. </returns>
		object GetSourceObject(/*ITemplatingContext templatingContext, */string sourceFilePath, IDictionary<string, IList<string>> properties);

		#endregion
	}
}