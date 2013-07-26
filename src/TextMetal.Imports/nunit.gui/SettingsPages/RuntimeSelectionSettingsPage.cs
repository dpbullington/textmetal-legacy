// ****************************************************************
// Copyright 2010, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;

using NUnit.UiKit;

namespace NUnit.Gui.SettingsPages
{
	public partial class RuntimeSelectionSettingsPage : SettingsPage
	{
		#region Constructors/Destructors

		public RuntimeSelectionSettingsPage(string key)
			: base(key)
		{
			this.InitializeComponent();
		}

		#endregion

		#region Fields/Constants

		private static readonly string RUNTIME_SELECTION_ENABLED =
			"Options.TestLoader.RuntimeSelectionEnabled";

		#endregion

		#region Methods/Operators

		public override void ApplySettings()
		{
			this.settings.SaveSetting(RUNTIME_SELECTION_ENABLED, this.runtimeSelectionCheckBox.Checked);
		}

		public override void LoadSettings()
		{
			this.runtimeSelectionCheckBox.Checked = this.settings.GetSetting(RUNTIME_SELECTION_ENABLED, true);
		}

		#endregion
	}
}