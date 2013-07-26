// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;

using NUnit.Core;

namespace NUnit.Util
{
	/// <summary>
	/// 	Summary description for RecentFilesService.
	/// </summary>
	public class RecentFilesService : RecentFiles, IService
	{
		#region Constructors/Destructors

		public RecentFilesService()
			: this(Services.UserSettings)
		{
		}

		public RecentFilesService(ISettings settings)
		{
			this.settings = settings;
		}

		#endregion

		#region Fields/Constants

		public static readonly int DefaultSize = 5;
		public static readonly int MaxSize = 24;
		public static readonly int MinSize = 0;
		private RecentFilesCollection fileEntries = new RecentFilesCollection();

		private ISettings settings;

		#endregion

		#region Properties/Indexers/Events

		public int Count
		{
			get
			{
				return this.fileEntries.Count;
			}
		}

		public RecentFilesCollection Entries
		{
			get
			{
				return this.fileEntries;
			}
		}

		public int MaxFiles
		{
			get
			{
				int size = this.settings.GetSetting("Gui.RecentProjects.MaxFiles", -1);
				if (size < 0)
				{
					size = this.settings.GetSetting("RecentProjects.MaxFiles", DefaultSize);
					if (size != DefaultSize)
						this.settings.SaveSetting("Gui.RecentProjects.MaxFiles", size);
					this.settings.RemoveSetting("RecentProjects.MaxFiles");
				}

				if (size < MinSize)
					size = MinSize;
				if (size > MaxSize)
					size = MaxSize;

				return size;
			}
			set
			{
				int oldSize = this.MaxFiles;
				int newSize = value;

				if (newSize < MinSize)
					newSize = MinSize;
				if (newSize > MaxSize)
					newSize = MaxSize;

				this.settings.SaveSetting("Gui.RecentProjects.MaxFiles", newSize);
				if (newSize < oldSize)
					this.SaveEntriesToSettings(this.settings);
			}
		}

		#endregion

		#region Methods/Operators

		private void AddEntriesForPrefix(string prefix)
		{
			for (int index = 1; index < this.MaxFiles; index++)
			{
				if (this.fileEntries.Count >= this.MaxFiles)
					break;

				string fileSpec = this.settings.GetSetting(this.GetRecentFileKey(prefix, index)) as string;
				if (fileSpec != null)
					this.fileEntries.Add(RecentFileEntry.Parse(fileSpec));
			}
		}

		private string GetRecentFileKey(string prefix, int index)
		{
			return string.Format("{0}.File{1}", prefix, index);
		}

		public void InitializeService()
		{
			this.LoadEntriesFromSettings(this.settings);
		}

		private void LoadEntriesFromSettings(ISettings settings)
		{
			this.fileEntries.Clear();

			this.AddEntriesForPrefix("Gui.RecentProjects");

			// Try legacy entries if nothing was found
			if (this.fileEntries.Count == 0)
			{
				this.AddEntriesForPrefix("RecentProjects.V2");
				this.AddEntriesForPrefix("RecentProjects.V1");
			}

			// Try even older legacy format
			if (this.fileEntries.Count == 0)
				this.AddEntriesForPrefix("RecentProjects");
		}

		public void Remove(string fileName)
		{
			this.fileEntries.Remove(fileName);
		}

		private void SaveEntriesToSettings(ISettings settings)
		{
			string prefix = "Gui.RecentProjects";

			while (this.fileEntries.Count > this.MaxFiles)
				this.fileEntries.RemoveAt(this.fileEntries.Count - 1);

			for (int index = 0; index < MaxSize; index++)
			{
				string keyName = this.GetRecentFileKey(prefix, index + 1);
				if (index < this.fileEntries.Count)
					settings.SaveSetting(keyName, this.fileEntries[index].Path);
				else
					settings.RemoveSetting(keyName);
			}

			// Remove legacy entries here
			settings.RemoveGroup("RecentProjects");
		}

		public void SetMostRecent(string fileName)
		{
			this.SetMostRecent(new RecentFileEntry(fileName));
		}

		public void SetMostRecent(RecentFileEntry entry)
		{
			int index = this.fileEntries.IndexOf(entry.Path);

			if (index != -1)
				this.fileEntries.RemoveAt(index);

			this.fileEntries.Insert(0, entry);
			if (this.fileEntries.Count > this.MaxFiles)
				this.fileEntries.RemoveAt(this.MaxFiles);
		}

		public void UnloadService()
		{
			this.SaveEntriesToSettings(this.settings);
		}

		#endregion
	}
}