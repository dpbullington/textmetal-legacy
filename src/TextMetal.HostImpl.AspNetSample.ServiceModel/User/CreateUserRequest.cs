/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.HostImpl.AspNetSample.ServiceModel.User
{
	public sealed class CreateUserRequest : RequestBase
	{
		#region Properties/Indexers/Events

		public string EmailAddress
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

		public string UserName
		{
			get;
			set;
		}

		#endregion
	}
}