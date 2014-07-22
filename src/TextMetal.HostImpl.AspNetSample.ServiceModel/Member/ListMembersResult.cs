/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.HostImpl.AspNetSample.ServiceModel.Member
{
	public class ListMembersResult : ResultBase
	{
		#region Fields/Constants

		private DateTime? creationTimestamp;
		private string emailAddress;
		private int? memberId;
		private string memberName;
		private DateTime? modificationTimestamp;
		private int? organizationId;
		private string securityQuestion;
		private int? userId;
		private string userName;

		#endregion

		#region Properties/Indexers/Events

		public DateTime? CreationTimestamp
		{
			get
			{
				return this.creationTimestamp;
			}
			set
			{
				this.creationTimestamp = value;
			}
		}

		public string EmailAddress
		{
			get
			{
				return this.emailAddress;
			}
			set
			{
				this.emailAddress = value;
			}
		}

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

		public string MemberName
		{
			get
			{
				return this.memberName;
			}
			set
			{
				this.memberName = value;
			}
		}

		public DateTime? ModificationTimestamp
		{
			get
			{
				return this.modificationTimestamp;
			}
			set
			{
				this.modificationTimestamp = value;
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

		public string SecurityQuestion
		{
			get
			{
				return this.securityQuestion;
			}
			set
			{
				this.securityQuestion = value;
			}
		}

		public int? UserId
		{
			get
			{
				return this.userId;
			}
			set
			{
				this.userId = value;
			}
		}

		public string UserName
		{
			get
			{
				return this.userName;
			}
			set
			{
				this.userName = value;
			}
		}

		#endregion
	}
}