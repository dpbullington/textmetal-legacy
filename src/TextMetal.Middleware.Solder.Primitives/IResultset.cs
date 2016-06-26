/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System.Collections.Generic;

namespace TextMetal.Middleware.Solder.Primitives
{
	public interface IResultset
	{
		#region Properties/Indexers/Events

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