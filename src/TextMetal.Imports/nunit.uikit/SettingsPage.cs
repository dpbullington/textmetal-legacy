// ****************************************************************
// Copyright 2008, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using NUnit.Util;

namespace NUnit.UiKit
{
	/// <summary>
	/// NUnitSettingsPage is the base class for all pages used
	/// in a tabbed or tree-structured SettingsDialog.
	/// </summary>
	public class SettingsPage : UserControl
	{
		#region Constructors/Destructors

		public SettingsPage()
		{
			// This call is required by the Windows.Forms Form Designer.
			this.InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
		}

		// Constructor we use in creating page for a Tabbed
		// or TreeBased dialog.
		public SettingsPage(string key)
			: this()
		{
			this.key = key;
			this.title = key;
			int dot = key.LastIndexOf('.');
			if (dot >= 0)
				this.title = key.Substring(dot + 1);
			this.messageDisplay = new MessageDisplay("NUnit Settings");
		}

		#endregion

		#region Fields/Constants

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		private string key;

		private MessageDisplay messageDisplay;

		/// <summary>
		/// Settings are available to derived classes
		/// </summary>
		protected ISettings settings;

		private string title;

		#endregion

		// Constructor used by the Windows.Forms Designer

		#region Properties/Indexers/Events

		public virtual bool HasChangesRequiringReload
		{
			get
			{
				return false;
			}
		}

		public string Key
		{
			get
			{
				return this.key;
			}
		}

		public IMessageDisplay MessageDisplay
		{
			get
			{
				return this.messageDisplay;
			}
		}

		public bool SettingsLoaded
		{
			get
			{
				return this.settings != null;
			}
		}

		public string Title
		{
			get
			{
				return this.title;
			}
		}

		#endregion

		#region Methods/Operators

		public virtual void ApplySettings()
		{
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.components != null)
					this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// SettingsPage
			// 
			this.Name = "SettingsPage";
			this.Size = new Size(456, 336);
		}

		public virtual void LoadSettings()
		{
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			if (!this.DesignMode)
			{
				this.settings = Services.UserSettings;
				this.LoadSettings();
			}
		}

		#endregion
	}
}