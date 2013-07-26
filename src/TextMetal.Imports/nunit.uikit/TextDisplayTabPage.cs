// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Drawing;
using System.Windows.Forms;

namespace NUnit.UiKit
{
	/// <summary>
	/// 	Summary description for TextDisplayTabPage.
	/// </summary>
	public class TextDisplayTabPage : TabPage
	{
		#region Constructors/Destructors

		public TextDisplayTabPage()
		{
			this.display = new TextBoxDisplay();
			this.display.Dock = DockStyle.Fill;
			this.Controls.Add(this.display);
		}

		public TextDisplayTabPage(TextDisplayTabSettings.TabInfo tabInfo)
			: this()
		{
			this.Name = tabInfo.Name;
			this.Text = tabInfo.Title;
			this.Display.Content = tabInfo.Content;
		}

		#endregion

		#region Fields/Constants

		private TextBoxDisplay display;

		#endregion

		#region Properties/Indexers/Events

		public TextDisplay Display
		{
			get
			{
				return this.display;
			}
		}

		public Font DisplayFont
		{
			get
			{
				return this.display.Font;
			}
			set
			{
				this.display.Font = value;
			}
		}

		#endregion
	}
}