// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Collections;

namespace NUnit.Util
{
	/// <summary>
	/// MemorySettingsStorage is used to hold settings for
	/// the NUnit tests and also serves as the base class
	/// for XmlSettingsStorage.
	/// </summary>
	public class MemorySettingsStorage : ISettingsStorage
	{
		#region Fields/Constants

		protected Hashtable settings = new Hashtable();

		#endregion

		#region Methods/Operators

		public void Dispose()
		{
			// TODO:  Add MemorySettingsStorage.Dispose implementation
		}

		public object GetSetting(string settingName)
		{
			return this.settings[settingName];
		}

		public virtual void LoadSettings()
		{
			// No action required
		}

		public ISettingsStorage MakeChildStorage(string name)
		{
			return new MemorySettingsStorage();
		}

		public void RemoveGroup(string groupName)
		{
			ArrayList keysToRemove = new ArrayList();

			string prefix = groupName;
			if (!prefix.EndsWith("."))
				prefix = prefix + ".";

			foreach (string key in this.settings.Keys)
			{
				if (key.StartsWith(prefix))
					keysToRemove.Add(key);
			}

			foreach (string key in keysToRemove)
				this.settings.Remove(key);
		}

		public void RemoveSetting(string settingName)
		{
			this.settings.Remove(settingName);
		}

		public void SaveSetting(string settingName, object settingValue)
		{
			this.settings[settingName] = settingValue;
		}

		public virtual void SaveSettings()
		{
			// No action required
		}

		#endregion
	}
}