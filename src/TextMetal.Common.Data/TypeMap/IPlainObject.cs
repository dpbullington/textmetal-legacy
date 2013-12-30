/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Common.Data.TypeMap
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

		/// <summary>
		/// Gets or sets the current plain object state.
		/// </summary>
		ObjectState ObjectState
		{
			get;
			set;
		}

		#endregion
	}
}