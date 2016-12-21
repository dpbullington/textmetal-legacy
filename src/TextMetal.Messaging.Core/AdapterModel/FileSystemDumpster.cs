/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.IO;

namespace TextMetal.Messaging.Core.AdapterModel
{
	internal class FileSystemDumpster
	{
		#region Constructors/Destructors

		public FileSystemDumpster(string path)
		{
			this.path = path;
		}

		#endregion

		#region Fields/Constants

		private readonly string path;
		private bool enableRaisingEvents;

		#endregion

		#region Properties/Indexers/Events

		public event EventHandler Disposed;
		public event ErrorEventHandler Error;
		public event FileSystemEventHandler Requested;

		private string Path
		{
			get
			{
				return this.path;
			}
		}

		public bool EnableRaisingEvents
		{
			get
			{
				return this.enableRaisingEvents;
			}
			set
			{
				this.enableRaisingEvents = value;
			}
		}

		#endregion

		#region Methods/Operators

		public void Dispose()
		{
			if ((object)this.Disposed != null)
				this.Disposed(this, EventArgs.Empty);
		}

		public void Trigger()
		{
			if (this.EnableRaisingEvents)
			{
				if ((object)this.Requested != null)
					this.Requested(this, new FileSystemEventArgs(WatcherChangeTypes.Created, this.Path, null));
			}
		}

		#endregion
	}
}