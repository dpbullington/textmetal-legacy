/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

using TextMetal.Messaging.Core.MessageModel;

namespace TextMetal.Messaging.Core.AdapterModel
{
	public class FileSystemOutboundAdapter : OutboundAdapter
	{
		#region Constructors/Destructors

		public FileSystemOutboundAdapter()
		{
		}

		#endregion

		#region Fields/Constants

		private readonly IDictionary<IntegrationEndpoint, FileSystemDumpster> fileSystemDumpsters = new Dictionary<IntegrationEndpoint, FileSystemDumpster>();

		#endregion

		#region Properties/Indexers/Events

		private IDictionary<IntegrationEndpoint, FileSystemDumpster> FileSystemDumpsters
		{
			get
			{
				return this.fileSystemDumpsters;
			}
		}

		#endregion

		#region Methods/Operators

		protected override void CoreInitialize()
		{
			FileSystemDumpster fileSystemDumpster;
			string path;

			base.CoreInitialize();
			this.WriteLogSynchronized("OUTBOUND: Initialized on thread '{0}'.", Thread.CurrentThread.ManagedThreadId);

			lock (this.SyncLock)
			{
				foreach (IntegrationEndpoint endpoint in this.GetEndpoints())
				{
					path = endpoint.Address.Uri.LocalPath;

					if (this.FileSystemDumpsters.ContainsKey(endpoint))
						throw new InvalidOperationException(string.Format("The outbound adapter instance already contains an endpoint configuration '{0}'.", endpoint));

					if (File.Exists(path))
						throw new InvalidOperationException(string.Format("The outbound adapter instance expected a directory (not a file) endpoint configuration '{0}'.", endpoint));

					if (!Directory.Exists(path))
						Directory.CreateDirectory(path);

					fileSystemDumpster = new FileSystemDumpster(path);
					fileSystemDumpster.Disposed += this.FileSystemDumpsterOnDisposed;
					fileSystemDumpster.Error += this.FileSystemDumpsterOnError;
					fileSystemDumpster.Requested += this.FileSystemDumpsterOnRequested;

					this.FileSystemDumpsters.Add(endpoint, fileSystemDumpster);

					fileSystemDumpster.EnableRaisingEvents = true;
				}
			}
		}

		protected override void CoreTerminate()
		{
			this.WriteLogSynchronized("OUTBOUND: Terminated on thread '{0}'.", Thread.CurrentThread.ManagedThreadId);
			// TODO block until all pending files processed

			lock (this.SyncLock)
			{
				foreach (KeyValuePair<IntegrationEndpoint, FileSystemDumpster> endpointMap in this.FileSystemDumpsters)
				{
					endpointMap.Value.EnableRaisingEvents = false;
					endpointMap.Value.Error -= this.FileSystemDumpsterOnError;
					endpointMap.Value.Requested -= this.FileSystemDumpsterOnRequested;
					endpointMap.Value.Dispose();
					endpointMap.Value.Disposed -= this.FileSystemDumpsterOnDisposed;
				}

				this.FileSystemDumpsters.Clear();
			}

			base.CoreTerminate();
		}

		protected override void EnqueueOutboundMessageInternal(IIntegrationMessage integrationMessage)
		{
			if ((object)integrationMessage == null)
				throw new ArgumentNullException(nameof(integrationMessage));

			this.WriteLogSynchronized("OUTBOUND: Integration message ID '{0}' enqueued on thread '{1}'.", integrationMessage.RunTimeId, Thread.CurrentThread.ManagedThreadId);

			// TODO: HACK -- manual trigger should be timer based
			this.FileSystemDumpsters.Values.ToList().ForEach(fsd => fsd.Trigger());
		}

		private void FileSystemDumpsterOnDisposed(object sender, EventArgs eventArgs)
		{
			// do nothing
		}

		private void FileSystemDumpsterOnError(object sender, ErrorEventArgs errorEventArgs)
		{
			throw errorEventArgs.GetException();
		}

		private void FileSystemDumpsterOnRequested(object sender, FileSystemEventArgs fileSystemEventArgs)
		{
			this.WriteLogSynchronized("OUTBOUND: File system event received on thread '{0}'.", Thread.CurrentThread.ManagedThreadId);

			// need to multi-thread this as one file processing blocks all others on this dumpster
			ThreadPool.QueueUserWorkItem((o) =>
										{
											this.FileSystemDumpsterOnRequestedAsync(sender, fileSystemEventArgs);
										});
		}

		private void FileSystemDumpsterOnRequestedAsync(object sender, FileSystemEventArgs fileSystemEventArgs)
		{
			IIntegrationMessage integrationMessage;
			string stagingDirectoryPath;
			string stagingFilePath;
			DateTime messageSentUtc = DateTime.UtcNow;
			const int BUFFER_SIZE = 4096;
			object metadata;
			string fileName;

			this.WriteLogSynchronized("OUTBOUND: File system event processed via thread pool thread '{0}'.", Thread.CurrentThread.ManagedThreadId);

			stagingDirectoryPath = Path.GetDirectoryName(fileSystemEventArgs.FullPath);

			if (!Directory.Exists(stagingDirectoryPath))
				Directory.CreateDirectory(stagingDirectoryPath);

			integrationMessage = this.PostThenDispatchOutboundMessage();

			if ((object)integrationMessage == null)
				throw new InvalidOperationException("Inbound message was invalid.");

			using (integrationMessage.DataStream)
			{
				if (integrationMessage.Metadata.TryGetValue(AdapterMetadataConstants.INBOUND_ORIGINAL_FILE_NAME, out metadata))
					fileName = (string)metadata;
				else
					fileName = string.Format("{0}.{1}", integrationMessage.RunTimeId, "outbound");

				stagingFilePath = Path.Combine(stagingDirectoryPath, fileName);

				if (File.Exists(stagingFilePath))
					File.Delete(stagingFilePath);

				using (Stream stream = new FileStream(stagingFilePath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 1024, FileOptions.WriteThrough))
				{
					if ((object)integrationMessage.DataStream != null)
						integrationMessage.DataStream.CopyTo(stream, BUFFER_SIZE);
				}
			}

			// mark as processed
			this.IntegrationMessageStore.MarkMessage(integrationMessage, new Guid(MessageStateConstants.MESSAGE_STATE_PROCESSED));
		}

		#endregion
	}
}