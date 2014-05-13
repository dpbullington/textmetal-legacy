/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Common.Core;

namespace TextMetal.HostImpl.AspNetSample.UI.Web.Shared
{
	public static class WebConfig
	{
		#region Properties/Indexers/Events

		public static bool AllowSignUps
		{
			get
			{
				if (!AppConfig.HasAppSetting("TextMetal.HostImpl.AspNetSample.UI.Web.Shared::AllowSignUps"))
					return true;

				return AppConfig.GetAppSetting<bool>("TextMetal.HostImpl.AspNetSample.UI.Web.Shared::AllowSignUps");
			}
		}

		public static bool RequireHttps
		{
			get
			{
				if (!AppConfig.HasAppSetting("TextMetal.HostImpl.AspNetSample.UI.Web.Shared::RequireHttps"))
					return false;

				return AppConfig.GetAppSetting<bool>("TextMetal.HostImpl.AspNetSample.UI.Web.Shared::RequireHttps");
			}
		}

		public static bool ShowAds
		{
			get
			{
				if (!AppConfig.HasAppSetting("TextMetal.HostImpl.AspNetSample.UI.Web.Shared::ShowAds"))
					return true;

				return AppConfig.GetAppSetting<bool>("TextMetal.HostImpl.AspNetSample.UI.Web.Shared::ShowAds");
			}
		}

		#endregion
	}
}