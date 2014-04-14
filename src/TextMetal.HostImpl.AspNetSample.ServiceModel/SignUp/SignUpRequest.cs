/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.HostImpl.AspNetSample.ServiceModel.SignUp
{
	public sealed class SignUpRequest : RequestBase
	{
		#region Fields/Constants

		private string emailAddress;
		private string memberMemberName;
		private string organizationName;
		private string passwordClearText;
		private string securityAnswerClearText;
		private string securityQuestion;
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

		public string OrganizationName
		{
			get
			{
				return this.organizationName;
			}
			set
			{
				this.organizationName = value;
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