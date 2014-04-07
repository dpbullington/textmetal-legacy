/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

namespace TextMetal.HostImpl.AspNetSample.ServiceModel.User
{
	public sealed class AuthenticateUserRequest : RequestBase
	{
		#region Properties/Indexers/Events

		public int FailedLoginCount
		{
			get;
			set;
		}

		public string PasswordClearText
		{
			get;
			set;
		}

		public bool RememberMe
		{
			get;
			set;
		}

		public string UserName
		{
			get;
			set;
		}

		#endregion
	}
}