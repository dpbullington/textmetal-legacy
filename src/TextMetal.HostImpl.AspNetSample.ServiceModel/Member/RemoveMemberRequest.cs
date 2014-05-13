/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.HostImpl.AspNetSample.ServiceModel.Member
{
	public sealed class RemoveMemberRequest : RequestBase
	{
		private int? memberId;
		private int? organizationId;

		public int? MemberId
		{
			get
			{
				return memberId;
			}
			set
			{
				memberId = value;
			}
		}

		public int? OrganizationId
		{
			get
			{
				return organizationId;
			}
			set
			{
				organizationId = value;
			}
		}
	}
}