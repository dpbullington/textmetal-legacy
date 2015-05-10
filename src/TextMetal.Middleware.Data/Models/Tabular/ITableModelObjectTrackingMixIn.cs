/*
	Copyright ©2002-2015 Daniel Bullington (dpbullingtongmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Middleware.Data.Models.Tabular
{
	public interface ITableModelObjectTrackingMixIn
	{
		#region Properties/Indexers/Events

		DateTime? CreationTimestamp
		{
			get;
			set;
		}

		long? CreationUserId
		{
			get;
			set;
		}

		bool? LogicalDelete
		{
			get;
			set;
		}

		DateTime? ModificationTimestamp
		{
			get;
			set;
		}

		long? ModificationUserId
		{
			get;
			set;
		}

		byte? SortOrder
		{
			get;
			set;
		}

		#endregion
	}
}