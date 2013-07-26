// ****************************************************************
// Copyright 2008, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Util;

namespace NUnit.UiKit
{
	public class TextDisplayTabSettings
	{
		#region Fields/Constants

		public static readonly string Prefix = "Gui.TextOutput.";
		private ISettings settings;
		private TabInfoCollection tabInfo;

		#endregion

		#region Properties/Indexers/Events

		public TabInfoCollection Tabs
		{
			get
			{
				return this.tabInfo;
			}
		}

		#endregion

		#region Methods/Operators

		public void ApplySettings()
		{
			StringBuilder tabNames = new StringBuilder();
			foreach (TabInfo tab in this.tabInfo)
			{
				if (tabNames.Length > 0)
					tabNames.Append(",");
				tabNames.Append(tab.Name);

				string prefix = Prefix + tab.Name;

				this.settings.SaveSetting(prefix + ".Title", tab.Title);
				this.settings.SaveSetting(prefix + ".Enabled", tab.Enabled);
				tab.Content.SaveSettings(tab.Name);
			}

			string oldNames = this.settings.GetSetting(Prefix + "TabList", string.Empty);
			this.settings.SaveSetting(Prefix + "TabList", tabNames.ToString());

			if (oldNames != string.Empty)
			{
				string[] oldTabs = oldNames.Split(new char[] { ',' });
				foreach (string tabName in oldTabs)
				{
					if (this.tabInfo[tabName] == null)
						this.settings.RemoveGroup(Prefix + tabName);
				}
			}
		}

		public void LoadDefaults()
		{
			this.tabInfo = new TabInfoCollection();

			TabInfo tab = this.tabInfo.AddNewTab("Text Output");
			tab.Content = new TextDisplayContent();
			tab.Content.Out = true;
			tab.Content.Error = true;
			tab.Content.Labels = TestLabelLevel.On;
			tab.Enabled = true;
		}

		public void LoadSettings()
		{
			this.LoadSettings(Services.UserSettings);
		}

		public void LoadSettings(ISettings settings)
		{
			this.settings = settings;

			TabInfoCollection info = new TabInfoCollection();
			string tabList = (string)settings.GetSetting(Prefix + "TabList");

			if (tabList != null)
			{
				string[] tabNames = tabList.Split(new char[] { ',' });
				foreach (string name in tabNames)
				{
					string prefix = Prefix + name;
					string text = (string)settings.GetSetting(prefix + ".Title");
					if (text == null)
						break;

					TabInfo tab = new TabInfo(name, text);

					tab.Content = TextDisplayContent.FromSettings(name);
					tab.Enabled = settings.GetSetting(prefix + ".Enabled", true);

					info.Add(tab);
				}
			}

			if (info.Count > 0)
				this.tabInfo = info;
			else
				this.LoadDefaults();
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		public class TabInfo
		{
			#region Constructors/Destructors

			public TabInfo(string name, string title)
			{
				this.Name = name;
				this.Title = title;
				this.Enabled = true;
				this.Content = new TextDisplayContent();
			}

			#endregion

			#region Properties/Indexers/Events

			public TextDisplayContent Content
			{
				get;
				set;
			}

			public bool Enabled
			{
				get;
				set;
			}

			public string Name
			{
				get;
				set;
			}

			public string Title
			{
				get;
				set;
			}

			#endregion
		}

		public class TabInfoCollection : List<TabInfo>
		{
			#region Properties/Indexers/Events

			public TabInfo this[string name]
			{
				get
				{
					foreach (TabInfo info in this)
					{
						if (info.Name == name)
							return info;
					}

					return null;
				}
			}

			#endregion

			#region Methods/Operators

			public TabInfo AddNewTab(string title)
			{
				TabInfo tabInfo = new TabInfo(this.GetNextName(), title);
				this.Add(tabInfo);
				return tabInfo;
			}

			public bool Contains(string name)
			{
				return this[name] != null;
			}

			private string GetNextName()
			{
				for (int i = 0;; i++)
				{
					string name = string.Format("Tab{0}", i);
					if (this[name] == null)
						return name;
				}
			}

			#endregion
		}

		#endregion
	}
}