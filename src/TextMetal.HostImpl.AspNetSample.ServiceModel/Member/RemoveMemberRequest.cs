/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.HostImpl.AspNetSample.ServiceModel.Member
{
	public sealed class RemoveMemberRequest : RequestBase
	{
		#region Fields/Constants

		private int? memberId;
		private int? organizationId;

		#endregion

		#region Properties/Indexers/Events

		public int? MemberId
		{
			get
			{
				return this.memberId;
			}
			set
			{
				this.memberId = value;
			}
		}

		public int? OrganizationId
		{
			get
			{
				return this.organizationId;
			}
			set
			{
				this.organizationId = value;
			}
		}

		#endregion
	}
}