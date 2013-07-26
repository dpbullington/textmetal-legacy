// ****************************************************************
// This is free software licensed under the NUnit license. You
// may obtain a copy of the license as well as information regarding
// copyright ownership at http://nunit.org.
// ****************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;

using NUnit.Core;

namespace NUnit.Util
{
	/// <summary>
	/// 	AssemblyWatcher keeps track of one or more assemblies to 
	/// 	see if they have changed. It incorporates a delayed notification
	/// 	and uses a standard event to notify any interested parties
	/// 	about the change. The path to the assembly is provided as
	/// 	an argument to the event handler so that one routine can
	/// 	be used to handle events from multiple watchers.
	/// </summary>
	public class AssemblyWatcher : IAssemblyWatcher
	{
		private static Logger log = InternalTrace.GetLogger(typeof(AssemblyWatcher));

		private FileSystemWatcher[] fileWatchers;
		private FileInfo[] files;

		protected Timer timer;
		protected string changedAssemblyPath;

		protected FileInfo GetFileInfo(int index)
		{
			return this.files[index];
		}

		public void Setup(int delay, string assemblyFileName)
		{
			this.Setup(delay, new string[] { assemblyFileName });
		}

#if CLR_2_0 || CLR_4_0
		public void Setup(int delay, IList<string> assemblies)
#else
        public void Setup(int delay, System.Collections.IList assemblies)
#endif
		{
			log.Info("Setting up watcher");

			this.files = new FileInfo[assemblies.Count];
			this.fileWatchers = new FileSystemWatcher[assemblies.Count];

			for (int i = 0; i < assemblies.Count; i++)
			{
				log.Debug("Setting up FileSystemWatcher for {0}", assemblies[i]);

				this.files[i] = new FileInfo((string)assemblies[i]);

				this.fileWatchers[i] = new FileSystemWatcher();
				this.fileWatchers[i].Path = this.files[i].DirectoryName;
				this.fileWatchers[i].Filter = this.files[i].Name;
				this.fileWatchers[i].NotifyFilter = NotifyFilters.Size | NotifyFilters.LastWrite;
				this.fileWatchers[i].Changed += new FileSystemEventHandler(this.OnChanged);
				this.fileWatchers[i].EnableRaisingEvents = false;
			}

			this.timer = new Timer(delay);
			this.timer.AutoReset = false;
			this.timer.Enabled = false;
			this.timer.Elapsed += new ElapsedEventHandler(this.OnTimer);
		}

		public void Start()
		{
			this.EnableWatchers(true);
		}

		public void Stop()
		{
			this.EnableWatchers(false);
		}

		private void EnableWatchers(bool enable)
		{
			if (this.fileWatchers != null)
			{
				foreach (FileSystemWatcher watcher in this.fileWatchers)
					watcher.EnableRaisingEvents = enable;
			}
		}

		public void FreeResources()
		{
			log.Info("FreeResources");

			this.Stop();

			if (this.fileWatchers != null)
			{
				foreach (FileSystemWatcher watcher in this.fileWatchers)
				{
					if (watcher != null)
					{
						watcher.Changed -= new FileSystemEventHandler(this.OnChanged);
						watcher.Dispose();
					}
				}
			}

			if (this.timer != null)
			{
				this.timer.Stop();
				this.timer.Close();
			}

			this.fileWatchers = null;
			this.timer = null;
		}

		public event AssemblyChangedHandler AssemblyChanged;

		protected void OnTimer(Object source, ElapsedEventArgs e)
		{
			lock (this)
			{
				log.Info("Timer expired");
				this.PublishEvent();
				this.timer.Enabled = false;
			}
		}

		protected void OnChanged(object source, FileSystemEventArgs e)
		{
			log.Info("File {0} changed", e.Name);

			this.changedAssemblyPath = e.FullPath;
			if (this.timer != null)
			{
				lock (this)
				{
					if (!this.timer.Enabled)
						this.timer.Enabled = true;
					log.Info("Setting timer");
					this.timer.Start();
				}
			}
			else
				this.PublishEvent();
		}

		protected void PublishEvent()
		{
			if (this.AssemblyChanged != null)
			{
				log.Debug("Publishing Event to {0} listeners", this.AssemblyChanged.GetInvocationList().Length);
				this.AssemblyChanged(this.changedAssemblyPath);
			}
		}
	}
}