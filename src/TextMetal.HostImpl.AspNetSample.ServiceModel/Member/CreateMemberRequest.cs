/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.HostImpl.AspNetSample.ServiceModel.Member
{
	public sealed class CreateMemberRequest : RequestBase
	{
		#region Fields/Constants

		private string emailAddress;
		private DateTime? memberCreationTimestamp;
		private string memberMemberName;
		private int? memberId;
		private DateTime? memberModificationTimestamp;
		private int? memberSecurityRoleId;
		private int? organizationId;
		private string passwordClearText;
		private string securityAnswerClearText;
		private string securityQuestion;
		private int? userId;
		private string userName;

		#endregion

		#region Properties/Indexers/Events

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

		public DateTime? MemberCreationTimestamp
		{
			get
			{
				return this.memberCreationTimestamp;
			}
			set
			{
				this.memberCreationTimestamp = value;
			}
		}

		public string MemberName
		{
			get
			{
				return this.memberMemberName;
			}
			set
			{
				this.memberMemberName = value;
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

		public DateTime? MemberModificationTimestamp
		{
			get
			{
				return this.memberModificationTimestamp;
			}
			set
			{
				this.memberModificationTimestamp = value;
			}
		}

		public int? MemberSecurityRoleId
		{
			get
			{
				return this.memberSecurityRoleId;
			}
			set
			{
				this.memberSecurityRoleId = value;
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

		public string PasswordClearText
		{
			get
			{
				return this.passwordClearText;
			}
			set
			{
				this.passwordClearText = value;
			}
		}

		public string SecurityAnswerClearText
		{
			get
			{
				return this.securityAnswerClearText;
			}
			set
			{
				this.securityAnswerClearText = value;
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