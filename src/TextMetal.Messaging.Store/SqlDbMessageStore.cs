/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Text;
using System.Threading;

using TextMetal.Messaging.Core.MessageModel;
using TextMetal.Middleware.Datazoid.Extensions;
using TextMetal.Middleware.Datazoid.Repositories;
using TextMetal.Middleware.Datazoid.Repositories.Impl;
using TextMetal.Middleware.Datazoid.Repositories.Impl.Strategies;
using TextMetal.Middleware.Solder.Extensions;
using TextMetal.Middleware.Solder.Injection;
using TextMetal.Middleware.Solder.Utilities;

namespace TextMetal.Messaging.Store
{
	public class SqlDbMessageStore : IntegrationMessageStore
	{
		#region Constructors/Destructors

		public SqlDbMessageStore()
		{
		}

		#endregion

		#region Methods/Operators

		protected override void CoreInitialize()
		{
			DzModelRepository repository;
			IDataTypeFascade dataTypeFascade;
			IAppConfigFascade appConfigFascade;
			IDataSourceTagStrategy dataSourceTagStrategy;

			dataTypeFascade = AgnosticAppDomain.Default.DependencyManager.ResolveDependency<IDataTypeFascade>(string.Empty, true);
			appConfigFascade = AgnosticAppDomain.Default.DependencyManager.ResolveDependency<IAppConfigFascade>(string.Empty, true);
			dataSourceTagStrategy = AgnosticAppDomain.Default.DependencyManager.ResolveDependency<IDataSourceTagStrategy>(string.Empty, true);

			base.CoreInitialize();

			repository = new DzModelRepository(dataTypeFascade, appConfigFascade, dataSourceTagStrategy);

			using (var uow = repository.GetUnitOfWork(false))
				uow.ExecuteScalar<int>(CommandType.Text, "", new DbParameter[] { });
		}

		protected override void DispatchMessagesInternal(Action<object, IIntegrationMessage> postMessageCallback)
		{
			if ((object)postMessageCallback == null)
				throw new ArgumentNullException("postMessageCallback");

			this.WriteLogSynchronized("SQLDBMESSAGESTORE: dispatching messages on thread '{0}'.", Thread.CurrentThread.ManagedThreadId);

			/*IIntegrationMessage integrationMessage;
			IEnumerable<IDropbox> dropboxes;
			IEnumerable<IDropboxMetadata> dropboxMetadatas;
			IRepository repository;
			IModelQuery linqTableQuery;

			repository = new Repository();

			using (var scope = new AmbientUnitOfWorkScope(repository))
			{
				linqTableQuery = new LinqTableQuery<Messaging_Dropbox>(t => (t.Messaging_MessageState.MessageStateId == new Guid(MessageStateConstants.MESSAGE_STATE_CREATED)));
				dropboxes = repository.Find<IDropbox>(linqTableQuery);

				foreach (Dropbox dropbox in dropboxes)
				{
					linqTableQuery = new LinqTableQuery<Messaging_DropboxMetadata>(t => (t.DropboxId == dropbox.DropboxId));
					dropboxMetadatas = repository.Find<IDropboxMetadata>(linqTableQuery);

					integrationMessage = this.IntegrationFactory.CreateMessage((Guid)dropbox.DropboxId);
					integrationMessage.ContentEncoding = dropbox.ContentEncoding;
					integrationMessage.ContentType = dropbox.ContentType;

					// TODO: load via getstream
					integrationMessage.DataStream = new MemoryStream(Encoding.Unicode.GetBytes(dropbox.MessageText));

					if ((object)dropboxMetadatas != null)
					{
						foreach (DropboxMetadata dropboxMetadata in dropboxMetadatas)
							integrationMessage.Metadata.Add(dropboxMetadata.MetadataKey, dropboxMetadata.MetadataValue);
					}

					postMessageCallback(this, integrationMessage);
				}

				scope.ScopeComplete();
			}*/
		}

		protected override void DropMessageInternal(IIntegrationMessage integrationMessage)
		{
			/*IDropbox dropbox;
			IDropboxMetadata dropboxMetadata;
			IRepository repository;
			DateTime nowUtc = DateTime.UtcNow;
			StreamReader streamReader;

			if ((object)integrationMessage == null)
				throw new ArgumentNullException("integrationMessage");

			this.WriteLogSynchronized("SQLDBMESSAGESTORE: dropping message on thread '{0}'.", Thread.CurrentThread.ManagedThreadId);

			repository = new Repository();

			using (var scope = new AmbientUnitOfWorkScope(repository))
			{
				streamReader = new StreamReader(integrationMessage.DataStream);

				dropbox = repository.CreateModel<IDropbox>(m =>
															{
																m.DropboxId = integrationMessage.RunTimeId;
																m.CreationTimestamp = nowUtc;
																m.ModificationTimestamp = nowUtc;
																m.IsNew = true;
																m.LogicalDelete = false;

																m.ContentEncoding = integrationMessage.ContentEncoding = streamReader.CurrentEncoding.WebName;
																m.ContentLanguage = "";
																m.ContentLength = integrationMessage.DataStream.Length;
																m.ContentLocation = "";
																m.ContentHash = "";
																m.ContentDisposition = "";
																m.ContentRange = "";
																m.ContentType = integrationMessage.ContentType;

																m.MessageSchemaId = new Guid(MessageStateConstants.MESSAGE_SCHEMA_UNKNOWN);
																m.MessageStateId = new Guid(MessageStateConstants.MESSAGE_STATE_CREATED);
																m.MessageText = streamReader.ReadToEnd(); // TODO: inefficient buffered approach
															});

				repository.Save<IDropbox>(dropbox);

				foreach (KeyValuePair<string, object> metadata in integrationMessage.Metadata)
				{
					dropboxMetadata = repository.CreateModel<IDropboxMetadata>(m =>
																				{
																					m.DropboxId = integrationMessage.RunTimeId;
																					m.IsNew = true;
																					m.MetadataKey = metadata.Key;
																					m.MetadataOrdinal = 0;
																					m.MetadataValue = metadata.Value.SafeToString(null, null);
																				});

					repository.Save<IDropboxMetadata>(dropboxMetadata);
				}

				scope.ScopeComplete();
			}*/
		}

		protected override void MarkMessageInternal(IIntegrationMessage integrationMessage, Guid messageStateId)
		{
			/*IDropbox dropbox, prototype;
			IRepository repository;

			if ((object)integrationMessage == null)
				throw new ArgumentNullException("integrationMessage");

			this.WriteLogSynchronized("SQLDBMESSAGESTORE: marking message on thread '{0}'.", Thread.CurrentThread.ManagedThreadId);

			repository = new Repository();

			using (var scope = new AmbientUnitOfWorkScope(repository))
			{
				prototype = repository.CreateModel<IDropbox>(m =>
															{
																m.DropboxId = (Guid)integrationMessage.RunTimeId;
															});

				dropbox = repository.Load<IDropbox>(prototype);

				if ((object)dropbox == null)
					throw new InvalidOperationException("not found");

				dropbox.IsNew = false;
				dropbox.MessageStateId = messageStateId;
				repository.Save<IDropbox>(dropbox);

				scope.ScopeComplete();
			}*/
		}

		#endregion
	}
}