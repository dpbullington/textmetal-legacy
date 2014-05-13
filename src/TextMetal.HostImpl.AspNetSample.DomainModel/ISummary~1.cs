/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.HostImpl.AspNetSample.DomainModel
{
	public interface ISummary
	{
		#region Properties/Indexers/Events

		DateTime? EntityCreationTimestamp
		{
			get;
			set;
		}

		int? EntityId
		{
			get;
			set;
		}

		DateTime? EntityModificationTimestamp
		{
			get;
			set;
		}

		string EntitySummaryText
		{
			get;
			set;
		}

		#endregion
	}

	public interface ISummary<TSummarizedObject> : ISummary
	{
	}
}