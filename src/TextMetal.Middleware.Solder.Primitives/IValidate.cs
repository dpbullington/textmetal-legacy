/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

namespace TextMetal.Middleware.Solder.Primitives
{
	public interface IValidate
	{
		#region Methods/Operators

		/// <summary>
		/// Validates this instance.
		/// </summary>
		/// <returns> A enumerable of zero or more messages. </returns>
		IEnumerable<Message> Validate();

		#endregion
	}
}