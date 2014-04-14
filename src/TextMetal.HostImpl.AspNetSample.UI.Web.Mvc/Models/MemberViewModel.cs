/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Web.Mvc;

using TextMetal.HostImpl.AspNetSample.ServiceModel.Member;

namespace TextMetal.HostImpl.AspNetSample.UI.Web.Mvc.Models
{
	public class MemberViewModel : TextMetalViewModel
	{
		#region Fields/Constants

		private string emailAddress;
		private string emailAddressConfirm;
		private string memberName;
		private IEnumerable<ListMembersResult> memberResults;
		private int? memberSecurityRoleId;
		private IEnumerable<SelectListItem> memberSecurityRoles;
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

		public IEnumerable<ListMembersResult> MemberResults
		{
			get
			{
				return this.memberResults;
			}
			set
			{
				this.memberResults = value;
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

		public IEnumerable<SelectListItem> MemberSecurityRoles
		{
			get
			{
				return this.memberSecurityRoles;
			}
			set
			{
				this.memberSecurityRoles = value;
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