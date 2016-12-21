/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Threading;

using TextMetal.Messaging.Core.MessageModel;
using TextMetal.Middleware.Solder.Extensions;
using TextMetal.Middleware.Solder.Utilities;

namespace TextMetal.Messaging.Store
{
	public class MongoDbMessageStore : IntegrationMessageStore
	{
		#region Constructors/Destructors

		public MongoDbMessageStore()
		{
		}

		#endregion

		#region Methods/Operators

		private MongoCollection<BsonDocument> AquireMessageCollection()
		{
			string connectionString;
			MongoClient client;
			MongoServer server;
			MongoDatabase database;
			MongoCollection<BsonDocument> collection;

			connectionString = AppConfigFascade.Instance.GetConnectionString("_2ndAsset.MessagingEngine.Core.StorageModel::ConnectionString#MongoDbMessageStore");
			client = new MongoClient(connectionString);
			server = client.GetServer();
			database = server.GetDatabase("2ndAsset");
			collection = database.GetCollection("message_box");

			return collection;
		}

		protected override void CoreInitialize()
		{
			MongoCollection<BsonDocument> collection;

			base.CoreInitialize();

			collection = this.AquireMessageCollection();

			collection.Validate();
		}

		protected override void DispatchMessagesInternal(Action<object, IIntegrationMessage> postMessageCallback)
		{
			if ((object)postMessageCallback == null)
				throw new ArgumentNullException("postMessageCallback");

			Console.WriteLine("MONGODBMESSAGESTORE: dispatching messages on thread '{0}'.", Thread.CurrentThread.ManagedThreadId);
		}

		protected override void DropMessageInternal(IIntegrationMessage integrationMessage)
		{
			DateTime nowUtc = DateTime.UtcNow;
			StreamReader streamReader;
			dynamic expando;
			IDictionary<string, object> associative;

			MongoCollection<BsonDocument> collection;

			expando = new ExpandoObject();
			associative = new ExpandoObject();

			streamReader = new StreamReader(integrationMessage.DataStream);

			foreach (KeyValuePair<string, object> metadata in integrationMessage.Metadata)
				associative.Add(metadata.Key, metadata.Value.SafeToString(null, null));

			expando.Id = integrationMessage.RunTimeId;
			expando.CreationTimestamp = nowUtc;
			expando.ModificationTimestamp = nowUtc;
			expando.Charset = integrationMessage.ContentEncoding = streamReader.CurrentEncoding.WebName;
			expando.ContentType = integrationMessage.ContentType;
			expando.MessageClass = "unknown";
			expando.MessageStateId = new Guid(MessageStateConstants.MESSAGE_STATE_CREATED);
			expando.MessageText = streamReader.ReadToEnd(); // TODO: inefficient buffered approach
			expando.Metadata = (IDictionary<string, object>)associative;

			expando = new BsonDocument((IDictionary<string, object>)expando);

			collection = this.AquireMessageCollection();
			collection.Insert(expando);
		}

		protected override void MarkMessageInternal(IIntegrationMessage integrationMessage, Guid messageStateId)
		{
			// TODO
		}

		#endregion
	}
}