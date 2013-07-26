// ****************************************************************
// Copyright 2011, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.IO;
using System.Timers;

namespace NUnit.Util
{
	public delegate void FileChangedHandler(string filePath);

	public interface IWatcher
	{
		#region Properties/Indexers/Events

		event FileChangedHandler Changed;

		int Delay
		{
			get;
			set;
		}

		#endregion

		#region Methods/Operators

		void Start();

		void Stop();

		#endregion
	}

	public class FileWatcher : IDisposable
	{
		#region Constructors/Destructors

		public FileWatcher(string filePath, int delay)
		{
			this.filePath = filePath;
			this.watcher = new FileSystemWatcher();

			this.watcher.Path = Path.GetDirectoryName(filePath);
			this.watcher.Filter = Path.GetFileName(filePath);
			this.watcher.NotifyFilter = NotifyFilters.Size | NotifyFilters.LastWrite;
			this.watcher.EnableRaisingEvents = false;
			this.watcher.Changed += new FileSystemEventHandler(this.OnChange);

			this.timer = new Timer(delay);
			this.timer.AutoReset = false;
			this.timer.Enabled = false;
			this.timer.Elapsed += new ElapsedEventHandler(this.OnTimer);
		}

		#endregion

		#region Fields/Constants

		private string filePath;
		private Timer timer;
		private FileSystemWatcher watcher;

		#endregion

		#region Properties/Indexers/Events

		public event FileChangedHandler Changed;

		#endregion

		#region Methods/Operators

		public void Dispose()
		{
			this.watcher.Dispose();
		}

		private void OnChange(object sender, FileSystemEventArgs e)
		{
			if (!this.timer.Enabled)
				this.timer.Enabled = true;
			this.timer.Start();
		}

		private void OnTimer(object sender, ElapsedEventArgs e)
		{
			this.timer.Enabled = false;

			if (this.Changed != null)
				this.Changed(this.filePath);
		}

		public void Start()
		{
			this.watcher.EnableRaisingEvents = true;
		}

		public void Stop()
		{
			this.watcher.EnableRaisingEvents = false;
		}

		#endregion
	}
}