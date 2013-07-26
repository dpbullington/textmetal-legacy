// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Configuration;
using System.IO;

using Microsoft.Win32;

using NUnit.Core;

namespace NUnit.Util
{
	/// <summary>
	/// 	Summary description for UserSettingsService.
	/// </summary>
	public class SettingsService : SettingsGroup, IService
	{
		#region Constructors/Destructors

		public SettingsService()
			: this(true)
		{
		}

		public SettingsService(bool writeable)
		{
			this.writeable = writeable;
#if CLR_2_0 || CLR_4_0
			string settingsFile = ConfigurationManager.AppSettings["settingsFile"];
#else
			string settingsFile = System.Configuration.ConfigurationSettings.AppSettings["settingsFile"];
#endif
			if (settingsFile == null)
				settingsFile = Path.Combine(NUnitConfiguration.ApplicationDirectory, settingsFileName);

			this.storage = new XmlSettingsStorage(settingsFile, writeable);

			if (File.Exists(settingsFile))
				this.storage.LoadSettings();
			else if (writeable)
				this.ConvertLegacySettings();
		}

		#endregion

		#region Fields/Constants

		private static readonly string settingsFileName = "NUnitSettings.xml";

		private bool writeable;

		#endregion

		#region Methods/Operators

		private void ConvertLegacySettings()
		{
			RegistryKey key = Registry.CurrentUser.OpenSubKey(NUnitRegistry.KEY);
			if (key == null)
				key = Registry.CurrentUser.OpenSubKey(NUnitRegistry.LEGACY_KEY);

			if (key != null)
			{
				using (ISettingsStorage legacyStorage = new RegistrySettingsStorage(key))
					new LegacySettingsConverter(legacyStorage, this.storage).Convert();

				this.storage.SaveSettings();
			}
		}

		public void InitializeService()
		{
		}

		public void UnloadService()
		{
			if (this.writeable)
				this.storage.SaveSettings();

			this.Dispose();
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		private class LegacySettingsConverter : SettingsGroup
		{
			#region Constructors/Destructors

			public LegacySettingsConverter(ISettingsStorage legacy, ISettingsStorage current)
				: base(current)
			{
				this.legacy = legacy;
			}

			#endregion

			#region Fields/Constants

			private ISettingsStorage legacy;

			#endregion

			#region Methods/Operators

			public void Convert()
			{
				this.Convert("Form.x-location", "Gui.MainForm.Left");
				this.Convert("Form.x-location", "Gui.MiniForm.Left");
				this.Convert("Form.y-location", "Gui.MainForm.Top");
				this.Convert("Form.y-location", "Gui.MiniForm.Top");
				this.Convert("Form.width", "Gui.MainForm.Width");
				this.Convert("Form.width", "Gui.MiniForm.Width");
				this.Convert("Form.height", "Gui.MainForm.Height");
				this.Convert("Form.height", "Gui.MiniForm.Height");
				this.Convert("Form.maximized", "Gui.MainForm.Maximized", "False", "True");
				this.Convert("Form.maximized", "Gui.MiniForm.Maximized", "False", "True");
				this.Convert("Form.font", "Gui.MainForm.Font");
				this.Convert("Form.font", "Gui.MiniForm.Font");
				this.Convert("Form.tree-splitter-position", "Gui.MainForm.SplitPosition");
				this.Convert("Form.tab-splitter-position", "Gui.ResultTabs.ErrorsTabSplitterPosition");
				this.Convert("Options.TestLabels", "Gui.ResultTabs.DisplayTestLabels", "False", "True");
				this.Convert("Options.FailureToolTips", "Gui.ResultTabs.ErrorTab.ToolTipsEnabled", "False", "True");
				this.Convert("Options.EnableWordWrapForFailures", "Gui.ResultTabs.ErrorTab.WordWrapEnabled", "False", "True");
				this.Convert("Options.InitialTreeDisplay", "Gui.TestTree.InitialTreeDisplay", "Auto", "Expand", "Collapse", "HideTests");
				this.Convert("Options.ShowCheckBoxes", "Gui.TestTree.ShowCheckBoxes", "False", "True");
				this.Convert("Options.LoadLastProject", "Options.LoadLastProject", "False", "True");
				this.Convert("Options.ClearResults", "Options.TestLoader.ClearResultsOnReload", "False", "True");
				this.Convert("Options.ReloadOnChange", "Options.TestLoader.ReloadOnChange", "False", "True");
				this.Convert("Options.RerunOnChange", "Options.TestLoader.RerunOnChange", "False", "True");
				this.Convert("Options.ReloadOnRun", "Options.TestLoader.ReloadOnRun", "False", "True");
				this.Convert("Options.MergeAssemblies", "Options.TestLoader.MergeAssemblies", "False", "True");
				//Convert( "Options.MultiDomain", "Options.TestLoader.MultiDomain", "False", "True" );
				this.Convert("Options.AutoNamespaceSuites", "Options.TestLoader.AutoNamespaceSuites", "False", "True");
				this.Convert("Options.VisualStudioSupport", "Options.TestLoader.VisualStudioSupport", "False", "True");
				this.Convert("Recent-Projects.MaxFiles", "RecentProjects.MaxFiles");

				object val = this.legacy.GetSetting("Options.MultiDomain");
				if (val != null && (bool)val)
					this.SaveSetting("Options.TestLoader.DomainUsage", DomainUsage.Multiple);

				int maxFiles = this.GetSetting("RecentProjects.MaxFiles", 5);
				for (int i = 1; i <= maxFiles; i++)
				{
					string fileKey = string.Format("File{0}", i);
					object fileEntry = this.legacy.GetSetting("Recent-Projects." + fileKey);
					if (fileEntry != null)
						this.SaveSetting("RecentProjects." + fileKey, fileEntry);
				}
			}

			private void Convert(string legacyName, string currentName, params string[] values)
			{
				object val = this.legacy.GetSetting(legacyName);
				if (val != null)
				{
					if (val is int && values != null)
					{
						int ival = (int)val;
						if (ival >= 0 && ival < values.Length)
							val = values[(int)val];
					}

					this.SaveSetting(currentName, val);
				}
			}

			#endregion
		}

		#endregion
	}
}