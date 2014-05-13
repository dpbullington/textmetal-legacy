/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.HostImpl.AspNetSample.UI.Web.Shared;

namespace TextMetal.HostImpl.AspNetSample.UI.Web.Mvc.Models
{
	public abstract class TextMetalViewModel : BaseViewModel
	{
		#region Fields/Constants

		private bool userWasAuthenticated;

		#endregion

		#region Properties/Indexers/Events

		public bool UserWasAuthenticated
		{
			get
			{
				return this.userWasAuthenticated;
			}
			set
			{
				this.userWasAuthenticated = value;
			}
		}

		#endregion
	}
}