/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.AspNetCore.StaticFiles;
using TextMetal.Messaging.Core.MessageModel;
using TextMetal.Middleware.Solder.Utilities.Vfs;

namespace TextMetal.Messaging.Core.AdapterModel
{
	public class FileSystemInboundAdapter : InboundAdapter
	{
		#region Constructors/Destructors

		public FileSystemInboundAdapter()
		{
		}

		#endregion

		#region Fields/Constants

		private readonly IDictionary<IntegrationEndpoint, FileSystemWatcherEx> fileSystemWatchers = new Dictionary<IntegrationEndpoint, FileSystemWatcherEx>();

		#endregion

		#region Properties/Indexers/Events

		private IDictionary<IntegrationEndpoint, FileSystemWatcherEx> FileSystemWatchers
		{
			get
			{
				return this.fileSystemWatchers;
			}
		}

		#endregion

		#region Methods/Operators

		private static bool FileReady(string filePath)
		{
			try
			{
				if (!File.Exists(filePath))
					return false; //throw new InvalidOperationException(string.Format("{0}", filePath));

				using (File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
					return true;
			}
			catch (IOException)
			{
				return false;
			}
			catch (UnauthorizedAccessException)
			{
				return false;
			}
		}

		private static bool TryRetryUntil(DateTime startUtc)
		{
			const int MAX_RETRY_PERIOD_MS = 1000 * 60;
			const int RETRY_DELAY_MS = 1000;
			TimeSpan timeElapsed = DateTime.UtcNow - startUtc;

			if (timeElapsed.TotalMilliseconds > MAX_RETRY_PERIOD_MS)
				return false;

			Thread.Sleep(RETRY_DELAY_MS);
			return true;
		}

		protected override void CoreInitialize()
		{
			FileSystemWatcherEx fileSystemWatcher;
			string path, name;

			base.CoreInitialize();
			this.WriteLogSynchronized("INBOUND: Initialized on thread '{0}'.", Thread.CurrentThread.ManagedThreadId);

			foreach (IntegrationEndpoint endpoint in this.GetEndpoints())
			{
				path = Path.GetFullPath(endpoint.Address.Uri.LocalPath);
				name = Path.GetFileName(path);

				if (this.FileSystemWatchers.ContainsKey(endpoint))
					throw new InvalidOperationException(string.Format("The inbound adapter instance already contains an endpoint configuration '{0}'.", endpoint));

				if (File.Exists(path))
					throw new InvalidOperationException(string.Format("The inbound adapter instance expected a directory (not a file) endpoint configuration '{0}'.", endpoint));

				if (!Directory.Exists(path))
					Directory.CreateDirectory(path);

				fileSystemWatcher = new FileSystemWatcherEx(endpoint, path);
				fileSystemWatcher.Error += this.FileSystemWatcherOnError;
				fileSystemWatcher.Created += this.FileSystemWatcherOnCreated;
				fileSystemWatcher.IncludeSubdirectories = false;

				this.FileSystemWatchers.Add(endpoint, fileSystemWatcher);

				// manually trigger for existing files in drop location
				path = Path.GetDirectoryName(path); // special handling here
				this.ForceProcessing(fileSystemWatcher, new FileSystemEventArgs(WatcherChangeTypes.Created, path, name));

				// allow eventing to tke over
				fileSystemWatcher.EnableRaisingEvents = true;
			}
		}

		protected override void CoreTerminate()
		{
			this.WriteLogSynchronized("INBOUND: Terminated on thread '{0}'.", Thread.CurrentThread.ManagedThreadId);
			// TODO: block until all pending files processed

			lock (this.SyncLock) // TODO: change this to reader/writer lock
			{
				foreach (KeyValuePair<IntegrationEndpoint, FileSystemWatcherEx> endpointMap in this.FileSystemWatchers)
				{
					endpointMap.Value.EnableRaisingEvents = false;
					endpointMap.Value.Created -= this.FileSystemWatcherOnCreated;
					endpointMap.Value.Error -= this.FileSystemWatcherOnError;
					endpointMap.Value.Dispose();
				}

				this.FileSystemWatchers.Clear();
			}

			base.CoreTerminate();
		}

		private void FileSystemWatcherOnCreated(object sender, FileSystemEventArgs fileSystemEventArgs)
		{
			// need to multi-thread this as one file processing blocks all others on this watcher
			ThreadPool.QueueUserWorkItem((o) =>
										{
											this.FileSystemWatcherOnCreatedAsync(sender, fileSystemEventArgs);
										});
		}

		private void FileSystemWatcherOnCreatedAsync(object sender, FileSystemEventArgs fileSystemEventArgs)
		{
			FileSystemWatcherEx fileSystemWatcherEx;
			bool continueRetry;
			IIntegrationMessage integrationMessage;
			DateTime messageReceivedUtc = DateTime.UtcNow;
			string filePath, fileName, contentType;
			Stream stream;
			IContentTypeProvider contentTypeProvider;

			this.WriteLogSynchronized("INBOUND: File system event processed via thread pool thread '{0}'.", Thread.CurrentThread.ManagedThreadId);
			fileSystemWatcherEx = (FileSystemWatcherEx)sender;

			filePath = Path.GetFullPath(fileSystemEventArgs.FullPath);
			fileName = Path.GetFileName(filePath);

			// ignore new directories
			if (Directory.Exists(filePath))
				return;

			integrationMessage = this.IntegrationFactory.CreateMessage();

			this.WriteLogSynchronized("INBOUND: Integration message ID '{0}' created on thread pool thread '{1}'.", integrationMessage.RunTimeId, Thread.CurrentThread.ManagedThreadId);

			integrationMessage.Metadata.Add(AdapterMetadataConstants.INBOUND_ORIGINAL_FILE_PATH, fileSystemEventArgs.FullPath);
			integrationMessage.Metadata.Add(AdapterMetadataConstants.INBOUND_ORIGINAL_FILE_NAME, fileSystemEventArgs.Name);
			integrationMessage.Metadata.Add(AdapterMetadataConstants.INBOUND_ORIGINAL_DIRECTORY_PATH, Path.GetDirectoryName(fileSystemEventArgs.FullPath));

			continueRetry = true;

			while (!FileReady(filePath))
			{
				if (!(continueRetry = TryRetryUntil(messageReceivedUtc)))
					break; //throw new InvalidOperationException(string.Format("{0}", fileSystemEventArgs.FullPath));
			}

			if (!continueRetry)
				return; // give up

			// stream is not owned here, deleted on close...???
			stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None, 1024, FileOptions.SequentialScan | FileOptions.DeleteOnClose);

			contentTypeProvider = new FileExtensionContentTypeProvider();

			integrationMessage.DataStream = stream;
			contentTypeProvider.TryGetContentType(filePath, out contentType);
			integrationMessage.ContentType = contentType;
			integrationMessage.ContentEncoding = null; // deferred until stream to dropbox

			this.PostInboundMessage(fileSystemWatcherEx.Endpoint, integrationMessage);
		}

		private void FileSystemWatcherOnError(object sender, ErrorEventArgs errorEventArgs)
		{
			throw errorEventArgs.GetException();
		}

		private void ForceProcessing(FileSystemWatcher fileSystemWatcher, FileSystemEventArgs fileSystemEventArgs)
		{
			// need to multi-thread this as one file processing block main thread
			ThreadPool.QueueUserWorkItem((o) =>
										{
											this.ForceProcessingAsync(fileSystemWatcher, fileSystemEventArgs);
										});
		}

		private void ForceProcessingAsync(FileSystemWatcher fileSystemWatcher, FileSystemEventArgs fileSystemEventArgs)
		{
			VirtualFileSystemEnumerator virtualFileSystemEnumerator;
			IEnumerable<VirtualFileSystemItem> virtualFileSystemItems;

			if ((object)fileSystemWatcher == null)
				throw new ArgumentNullException("fileSystemWatcher");

			if ((object)fileSystemEventArgs == null)
				throw new ArgumentNullException("fileSystemEventArgs");

			this.WriteLogSynchronized("INBOUND: Force processing for endpoint '{0}' started on thread pool thread '{1}'.", fileSystemEventArgs.FullPath, Thread.CurrentThread.ManagedThreadId);
			
			virtualFileSystemEnumerator = new VirtualFileSystemEnumerator();

			// this not block - lazy loading but sync
			virtualFileSystemItems = virtualFileSystemEnumerator.EnumerateVirtualItems(fileSystemEventArgs.FullPath, false);

			foreach (VirtualFileSystemItem virtualFileSystemItem in virtualFileSystemItems)
			{
				// force to not spawn yet another TPT
				this.FileSystemWatcherOnCreatedAsync(fileSystemWatcher, new FileSystemEventArgs(WatcherChangeTypes.Created, virtualFileSystemItem.ItemPath, virtualFileSystemItem.ItemName));
			}
		}

		protected override void CoreStartedInboundMessaging()
		{
		}

		protected override void CoreStartingInboundMessaging()
		{
		}

		protected override void CoreStoppedInboundMessaging()
		{
		}

		protected override void CoreStoppingInboundMessaging()
		{
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		private sealed class FileSystemWatcherEx : FileSystemWatcher
		{
			#region Constructors/Destructors

			public FileSystemWatcherEx(IntegrationEndpoint endpoint, string path)
				: base(path)
			{
				if ((object)endpoint == null)
					throw new ArgumentNullException("endpoint");

				this.endpoint = endpoint;
			}

			#endregion

			#region Fields/Constants

			private readonly IntegrationEndpoint endpoint;

			#endregion

			#region Properties/Indexers/Events

			public IntegrationEndpoint Endpoint
			{
				get
				{
					return this.endpoint;
				}
			}

			#endregion
		}

		#endregion
	}
}