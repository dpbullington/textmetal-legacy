/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

namespace TextMetal.HostImpl.AspNetSample.ServiceModel.Member
{
	public sealed class RemoveMemberResponse : ResponseBase
	{
		#region Properties/Indexers/Events

		public int? MemberId
		{
			get;
			set;
		}

		#endregion
	}
}