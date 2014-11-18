/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using TextMetal.Common.Core;

namespace TextMetal.Common.Data.Framework
{
	/// <summary>
	/// Provides a contract for model objects (domain, data, service, transfer, etc.).
	/// </summary
	public interface IModelObject : IBasicObject
	{
		#region Properties/Indexers/Events

		/// <summary>
		/// Gets or sets a value indicating whether the current model object instance is
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
		/// Called prior to any remote non-idempotent operation.
		/// </summary>
		void Mark();

		#endregion
	}
}