/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

namespace TextMetal.HostImpl.AspNetSample.ServiceModel.User
{
	public sealed class ForgotUsernameResponse : ResponseBase
	{
		#region Properties/Indexers/Events

		public bool EmailSent
		{
			get;
			set;
		}

		public int FailedLoginCount
		{
			get;
			set;
		}

		#endregion
	}
}