/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System.Collections.Generic;

namespace TextMetal.Middleware.Solder.Primitives
{
	public interface IResultset
	{
		#region Properties/Indexers/Events

		object Context
		{
			get;
		}

		int Index
		{
			get;
		}

		IEnumerable<IRecord> Records
		{
			get;
		}

		int? RecordsAffected
		{
			get;
		}

		#endregion
	}
}