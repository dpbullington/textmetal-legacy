/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

namespace TextMetal.HostImpl.AspNetSample.ServiceModel.Organization
{
	public sealed class EditOrganizationRequest : RequestBase
	{
		#region Properties/Indexers/Events

		public string EmailAddress
		{
			get;
			set;
		}

		public bool MustChangePassword
		{
			get;
			set;
		}

		public string PasswordClearText
		{
			get;
			set;
		}

		public string SecurityAnswerClearText
		{
			get;
			set;
		}

		public string SecurityQuestion
		{
			get;
			set;
		}

		public int? OrganizationId
		{
			get;
			set;
		}

		public string OrganizationName
		{
			get;
			set;
		}

		#endregion
	}
}