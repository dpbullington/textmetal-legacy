/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.HostImpl.AspNetSample.UI.Web.Mvc.Models
{
	public class UserViewModel : TextMetalViewModel
	{
		#region Fields/Constants

		private string emailAddress;
		private string emailAddressConfirm;
		private string passwordClearText;
		private string passwordClearTextConfirm;
		private bool rememberMe;
		private string securityAnswerClearText;
		private string securityAnswerClearTextConfirm;
		private string securityQuestion;
		private string securityQuestionConfirm;
		private bool suspendAccount;
		private string username;
		private string usernameConfirm;

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

		public string EmailAddressConfirm
		{
			get
			{
				return this.emailAddressConfirm;
			}
			set
			{
				this.emailAddressConfirm = value;
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

		public string PasswordClearTextConfirm
		{
			get
			{
				return this.passwordClearTextConfirm;
			}
			set
			{
				this.passwordClearTextConfirm = value;
			}
		}

		public bool RememberMe
		{
			get
			{
				return this.rememberMe;
			}
			set
			{
				this.rememberMe = value;
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

		public string SecurityAnswerClearTextConfirm
		{
			get
			{
				return this.securityAnswerClearTextConfirm;
			}
			set
			{
				this.securityAnswerClearTextConfirm = value;
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

		public string SecurityQuestionConfirm
		{
			get
			{
				return this.securityQuestionConfirm;
			}
			set
			{
				this.securityQuestionConfirm = value;
			}
		}

		public bool SuspendAccount
		{
			get
			{
				return this.suspendAccount;
			}
			set
			{
				this.suspendAccount = value;
			}
		}

		public string Username
		{
			get
			{
				return this.username;
			}
			set
			{
				this.username = value;
			}
		}

		public string UsernameConfirm
		{
			get
			{
				return this.usernameConfirm;
			}
			set
			{
				this.usernameConfirm = value;
			}
		}

		#endregion
	}
}