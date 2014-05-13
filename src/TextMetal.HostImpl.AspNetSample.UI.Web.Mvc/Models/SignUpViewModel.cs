/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.HostImpl.AspNetSample.UI.Web.Mvc.Models
{
	public class SignUpViewModel : TextMetalViewModel
	{
		#region Fields/Constants

		private string emailAddress;
		private string emailAddressConfirm;
		private string memberName;
		private string organizationName;
		private string passwordClearText;
		private string passwordClearTextConfirm;
		private string securityAnswerClearText;
		private string securityAnswerClearTextConfirm;
		private string securityQuestion;
		private string securityQuestionConfirm;
		private bool? signUpCompleted;
		private int? stepCompleted;
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

		public bool? SignUpCompleted
		{
			get
			{
				return this.signUpCompleted;
			}
			set
			{
				this.signUpCompleted = value;
			}
		}

		public int? StepCompleted
		{
			get
			{
				return this.stepCompleted;
			}
			set
			{
				this.stepCompleted = value;
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