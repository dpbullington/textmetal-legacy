// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Drawing;

namespace NUnit.Util
{
	public delegate void SettingsEventHandler(object sender, SettingsEventArgs args);

	public class SettingsEventArgs : EventArgs
	{
		#region Constructors/Destructors

		public SettingsEventArgs(string settingName)
		{
			this.settingName = settingName;
		}

		#endregion

		#region Fields/Constants

		private string settingName;

		#endregion

		#region Properties/Indexers/Events

		public string SettingName
		{
			get
			{
				return this.settingName;
			}
		}

		#endregion
	}

	/// <summary>
	/// The ISettings interface is used to access all user
	/// settings and options.
	/// </summary>
	public interface ISettings
	{
		#region Properties/Indexers/Events

		event SettingsEventHandler Changed;

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Load a setting from the storage.
		/// </summary>
		/// <param name="settingName"> Name of the setting to load </param>
		/// <returns> Value of the setting or null </returns>
		object GetSetting(string settingName);

		/// <summary>
		/// Load a setting from the storage or return a default value
		/// </summary>
		/// <param name="settingName"> Name of the setting to load </param>
		/// <param name="settingName"> Value to return if the setting is missing </param>
		/// <returns> Value of the setting or the default value </returns>
		object GetSetting(string settingName, object defaultValue);

		/// <summary>
		/// Load an integer setting from the storage or return a default value
		/// </summary>
		/// <param name="settingName"> Name of the setting to load </param>
		/// <param name="defaultValue"> Value to return if the setting is missing </param>
		/// <returns> Value of the setting or the default value </returns>
		int GetSetting(string settingName, int defaultValue);

		/// <summary>
		/// Load a float setting from the storage or return a default value
		/// </summary>
		/// <param name="settingName"> Name of the setting to load </param>
		/// <param name="defaultValue"> Value to return if the setting is missing </param>
		/// <returns> Value of the setting or the default value </returns>
		float GetSetting(string settingName, float defaultValue);

		/// <summary>
		/// Load a boolean setting or return a default value
		/// </summary>
		/// <param name="settingName"> Name of setting to load </param>
		/// <param name="defaultValue"> Value to return if the setting is missing </param>
		/// <returns> Value of the setting or the default value </returns>
		bool GetSetting(string settingName, bool defaultValue);

		/// <summary>
		/// Load a string setting from the storage or return a default value
		/// </summary>
		/// <param name="settingName"> Name of the setting to load </param>
		/// <param name="defaultValue"> Value to return if the setting is missing </param>
		/// <returns> Value of the setting or the default value </returns>
		string GetSetting(string settingName, string defaultValue);

		/// <summary>
		/// Load an enum setting from the storage or return a default value
		/// </summary>
		/// <param name="settingName"> Name of the setting to load </param>
		/// <param name="defaultValue"> Value to return if the setting is missing </param>
		/// <returns> Value of the setting or the default value </returns>
		Enum GetSetting(string settingName, Enum defaultValue);

		/// <summary>
		/// Load a Font setting from the storage or return a default value
		/// </summary>
		/// <param name="settingName"> Name of the setting to load </param>
		/// <param name="defaultFont"> Value to return if the setting is missing </param>
		/// <returns> Value of the setting or the default value </returns>
		Font GetSetting(string settingName, Font defaultFont);

		/// <summary>
		/// Remove an entire group of settings from the storage
		/// </summary>
		/// <param name="groupName"> Name of the group to remove </param>
		void RemoveGroup(string groupName);

		/// <summary>
		/// Remove a setting from the storage
		/// </summary>
		/// <param name="settingName"> Name of the setting to remove </param>
		void RemoveSetting(string settingName);

		/// <summary>
		/// Save a setting in the storage
		/// </summary>
		/// <param name="settingName"> Name of the setting to save </param>
		/// <param name="settingValue"> Value to be saved </param>
		void SaveSetting(string settingName, object settingValue);

		#endregion
	}
}