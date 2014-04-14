/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.HostImpl.AspNetSample.ServiceModel.User
{
	public sealed class AuthenticateUserResponse : ResponseBase
	{
		#region Fields/Constants

		private int failedLoginCount;
		private int? memberId;
		private bool mustChangePassword;
		private int? organizationId;
		private Tuple<int?, int?> rememberMeToken;
		private int? userId;

		#endregion

		#region Properties/Indexers/Events

		public int FailedLoginCount
		{
			get
			{
				return this.failedLoginCount;
			}
			set
			{
				this.failedLoginCount = value;
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

		public bool MustChangePassword
		{
			get
			{
				return this.mustChangePassword;
			}
			set
			{
				this.mustChangePassword = value;
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

		public Tuple<int?, int?> RememberMeToken
		{
			get
			{
				return this.rememberMeToken;
			}
			set
			{
				this.rememberMeToken = value;
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

		#endregion
	}
}