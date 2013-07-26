// ****************************************************************
// This is free software licensed under the NUnit license. You
// may obtain a copy of the license as well as information regarding
// copyright ownership at http://nunit.org.
// ****************************************************************

using System;
using System.Windows.Forms;

using NUnit.Util;

namespace NUnit.Gui
{
	public class RecentFileMenuHandler
	{
		#region Constructors/Destructors

		public RecentFileMenuHandler(MenuItem menu, RecentFiles recentFiles)
		{
			this.menu = menu;
			this.recentFiles = recentFiles;
		}

		#endregion

		#region Fields/Constants

		private bool checkFilesExist = true;

		private MenuItem menu;
		private RecentFiles recentFiles;
		private bool showNonRunnableFiles = false;

		#endregion

		#region Properties/Indexers/Events

		public string this[int index]
		{
			get
			{
				return this.menu.MenuItems[index].Text.Substring(2);
			}
		}

		public bool CheckFilesExist
		{
			get
			{
				return this.checkFilesExist;
			}
			set
			{
				this.checkFilesExist = value;
			}
		}

		public MenuItem Menu
		{
			get
			{
				return this.menu;
			}
		}

		public bool ShowNonRunnableFiles
		{
			get
			{
				return this.showNonRunnableFiles;
			}
			set
			{
				this.showNonRunnableFiles = value;
			}
		}

		#endregion

		#region Methods/Operators

		public void Load()
		{
			if (this.recentFiles.Count == 0)
				this.Menu.Enabled = false;
			else
			{
				this.Menu.Enabled = true;
				this.Menu.MenuItems.Clear();
				int index = 1;
				foreach (RecentFileEntry entry in this.recentFiles.Entries)
				{
					// Rather than show files that don't exist, we skip them. As
					// new recent files are opened, they will be pushed down and
					// eventually fall off the list unless the file is re-created
					// and subsequently opened.
					if (!this.checkFilesExist || entry.Exists)
					{
						// NOTE: In the current version, all the files listed should
						// have a compatible version, since we are using separate
						// settings for V1 and V2. This code will be changed in
						// a future release to allow running under other runtimes.
						if (this.showNonRunnableFiles || entry.IsCompatibleCLRVersion)
						{
							MenuItem item = new MenuItem(String.Format("{0} {1}", index++, entry.Path));
							item.Click += new EventHandler(this.OnRecentFileClick);
							this.Menu.MenuItems.Add(item);
						}
					}
				}
			}
		}

		private void OnRecentFileClick(object sender, EventArgs e)
		{
			MenuItem item = (MenuItem)sender;
			string testFileName = item.Text.Substring(2);

			// TODO: Figure out a better way
			NUnitForm form = item.GetMainMenu().GetForm() as NUnitForm;
			if (form != null)
				form.Presenter.OpenProject(testFileName);
		}

		#endregion
	}
}