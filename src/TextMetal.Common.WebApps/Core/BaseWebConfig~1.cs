/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Common.Core;

namespace TextMetal.Common.WebApps.Core
{
	public abstract class BaseWebConfig<TPrefix>
	{
		#region Constructors/Destructors

		protected BaseWebConfig()
		{
		}

		#endregion

		#region Properties/Indexers/Events

		public bool RequireHttps
		{
			get
			{
				const string APP_SETTINGS_USN = "RequireHttps";
				return this.GetWebConfigValue<bool>(APP_SETTINGS_USN);
			}
		}

		#endregion

		#region Methods/Operators

		protected T GetWebConfigValue<T>(string appSettingsUsn)
		{
			string appSettingsFsn;
			Type type;

			type = typeof(TPrefix);
			appSettingsFsn = string.Format("{0}::{1}", type.Namespace, appSettingsUsn);

			if (!AppConfig.HasAppSetting(appSettingsFsn))
				return default(T);

			return AppConfig.GetAppSetting<T>(appSettingsFsn);
		}

		#endregion
	}
}