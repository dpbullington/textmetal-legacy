/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using Microsoft.Framework.Configuration;
using Microsoft.Framework.Configuration.Json;

using TextMetal.Middleware.Solder.Injection;

/*
BACKLOG (dpbullington@gmail.com / 2015-12-18):
Refactor the appConfigFilePath constructor parameter out, do IoC of IConfigurationRoot and leverage commented out static factory method instead.
This will enhance unit testability and allow any config types.
*/

/* CERTIFICATION OF UNIT TESTING: dpbullington@gmail.com / 2016-02-22 / 98% code coverage */

namespace TextMetal.Middleware.Solder.Utilities
{
	/// <summary>
	/// Provides static helper and/or extension methods for strongly typed read access to an app.config or web.config file.
	/// </summary>
	public class AppConfigFascade : IAppConfigFascade
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the AppConfigFascade class.
		/// </summary>
		/// <param name="appConfigFilePath"> The file path to the application configuration JSON file. </param>
		/// <param name="dataTypeFascade"> The data type instance to use. </param>
		[Obsolete("Stop using this in unit tests.")]
		public AppConfigFascade(string appConfigFilePath, IDataTypeFascade dataTypeFascade)
		{
			if ((object)appConfigFilePath == null)
				throw new ArgumentNullException(nameof(appConfigFilePath));

			if ((object)dataTypeFascade == null)
				throw new ArgumentNullException(nameof(dataTypeFascade));

			this.dataTypeFascade = dataTypeFascade;
			this.configurationRoot = LoadAppConfigFile(appConfigFilePath);
		}

		[DependencyInjection]
		public AppConfigFascade([DependencyInjection] IConfigurationRoot configurationRoot, [DependencyInjection] IDataTypeFascade dataTypeFascade)
		{
			if ((object)configurationRoot == null)
				throw new ArgumentNullException(nameof(configurationRoot));

			if ((object)dataTypeFascade == null)
				throw new ArgumentNullException(nameof(dataTypeFascade));

			this.dataTypeFascade = dataTypeFascade;
			this.configurationRoot = configurationRoot;
		}

		#endregion

		#region Fields/Constants

		private readonly IConfigurationRoot configurationRoot;
		private readonly IDataTypeFascade dataTypeFascade;

		#endregion

		#region Properties/Indexers/Events

		private IConfigurationRoot ConfigurationRoot
		{
			get
			{
				return this.configurationRoot;
			}
		}

		private IDataTypeFascade DataTypeFascade
		{
			get
			{
				return this.dataTypeFascade;
			}
		}

		#endregion

		#region Methods/Operators

		internal static IConfigurationRoot LoadAppConfigFile(string appConfigFilePath)
		{
			IConfigurationBuilder configurationBuilder;
			IConfigurationProvider configurationProvider;
			IConfigurationRoot configurationRoot;

			if ((object)appConfigFilePath == null)
				throw new ArgumentNullException(nameof(appConfigFilePath));

			configurationBuilder = new ConfigurationBuilder();
			configurationProvider = new JsonConfigurationProvider(appConfigFilePath);
			configurationBuilder.Add(configurationProvider);
			configurationRoot = configurationBuilder.Build();

			return configurationRoot;
		}

		/// <summary>
		/// Gets the value of an app settings for the current application's default configuration. A AppConfigException is thrown if the key does not exist.
		/// </summary>
		/// <typeparam name="TValue"> The type to convert the app settings value. </typeparam>
		/// <param name="key"> The key to get a value. </param>
		/// <returns> The app settings as type TValue. </returns>
		public TValue GetAppSetting<TValue>(string key)
		{
			string svalue;
			TValue ovalue;
			Type typeOfValue;

			if ((object)key == null)
				throw new ArgumentNullException(nameof(key));

			typeOfValue = typeof(TValue);
			svalue = this.ConfigurationRoot[key];

			if ((object)svalue == null)
				throw new AppConfigException(string.Format("Key '{0}' was not found in app.config file.", key));

			if (!this.DataTypeFascade.TryParse<TValue>(svalue, out ovalue))
				throw new AppConfigException(string.Format("App.config key '{0}' value '{1}' is not a valid '{2}'.", key, svalue, typeOfValue.FullName));

			return ovalue;
		}

		/// <summary>
		/// Gets the value of an app settings for the current application's default configuration. A AppConfigException is thrown if the key does not exist.
		/// </summary>
		/// <param name="valueType"> The type to convert the app settings value. </param>
		/// <param name="key"> The key to get a value. </param>
		/// <returns> The app settings as a string. </returns>
		public object GetAppSetting(Type valueType, string key)
		{
			string svalue;
			object ovalue;

			if ((object)valueType == null)
				throw new ArgumentNullException(nameof(valueType));

			if ((object)key == null)
				throw new ArgumentNullException(nameof(key));

			svalue = this.ConfigurationRoot[key];

			if ((object)svalue == null)
				throw new AppConfigException(string.Format("Key '{0}' was not found in app.config file.", key));

			if (!this.DataTypeFascade.TryParse(valueType, svalue, out ovalue))
				throw new AppConfigException(string.Format("App.config key '{0}' value '{1}' is not a valid '{2}'.", key, svalue, valueType.FullName));

			return ovalue;
		}

		/// <summary>
		/// Checks to see if an app settings key exists for the current application's default configuration.
		/// </summary>
		/// <param name="key"> The key to check. </param>
		/// <returns> A boolean value indicating the app setting key presence. </returns>
		public bool HasAppSetting(string key)
		{
			string value;

			if ((object)key == null)
				throw new ArgumentNullException(nameof(key));

			value = this.ConfigurationRoot[key];

			return ((object)value != null);
		}

		#endregion
	}
}