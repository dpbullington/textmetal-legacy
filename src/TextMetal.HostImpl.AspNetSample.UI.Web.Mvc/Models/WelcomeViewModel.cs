/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.HostImpl.AspNetSample.UI.Web.Mvc.Models
{
	public class WelcomeViewModel : TextMetalViewModel
	{
		#region Fields/Constants

		private string emailAddress;
		private bool optInMarketingEmails;

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

		public bool OptInMarketingEmails
		{
			get
			{
				return this.optInMarketingEmails;
			}
			set
			{
				this.optInMarketingEmails = value;
			}
		}

		#endregion
	}
}