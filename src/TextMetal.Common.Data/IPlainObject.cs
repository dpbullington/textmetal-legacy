/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using TextMetal.Common.Core;

namespace TextMetal.Common.Data
{
	/// <summary>
	/// Provides a contract for plain objects (domain, transfer, etc.).
	/// </summary>
	public interface IPlainObject
	{
		#region Properties/Indexers/Events

		/// <summary>
		/// Gets or sets a value indicating whether the current plain object instance is
		/// new (never been persisted) or old (has been persisted).
		/// </summary>
		bool IsNew
		{
			get;
			set;
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Mark an instance prior to a data operation.
		/// </summary>
		void Mark();

		/// <summary>
		/// Validate the current plain object, returning an enumerable of messages.
		/// </summary>
		/// <returns> An enumerable of messages. </returns>
		IEnumerable<Message> Validate();

		#endregion
	}
}