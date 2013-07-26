// ****************************************************************
// Copyright 2010, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;

using NUnit.Core;
using NUnit.UiKit;

namespace NUnit.Gui.SettingsPages
{
	public partial class InternalTraceSettingsPage : SettingsPage
	{
		#region Constructors/Destructors

		public InternalTraceSettingsPage(string key)
			: base(key)
		{
			this.InitializeComponent();
		}

		#endregion

		#region Methods/Operators

		public override void ApplySettings()
		{
			InternalTraceLevel level = (InternalTraceLevel)this.traceLevelComboBox.SelectedIndex;
			this.settings.SaveSetting("Options.InternalTraceLevel", level);
			InternalTrace.Level = level;
		}

		public override void LoadSettings()
		{
			this.traceLevelComboBox.SelectedIndex = (int)(InternalTraceLevel)this.settings.GetSetting("Options.InternalTraceLevel", InternalTraceLevel.Default);
			this.logDirectoryLabel.Text = NUnitConfiguration.LogDirectory;
		}

		#endregion
	}
}