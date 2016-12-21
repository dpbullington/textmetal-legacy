/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.IO;

namespace TextMetal.Messaging.Core.MessageModel
{
	public sealed class IntegrationMessage : IntegrationComponent, IIntegrationMessage
	{
		#region Constructors/Destructors

		internal IntegrationMessage()
		{
		}

		private IntegrationMessage(Guid rehydratedRunTimeId)
			: base(rehydratedRunTimeId)
		{
		}

		#endregion

		#region Fields/Constants

		private readonly IDictionary<string, object> metadata = new Dictionary<string, object>();
		private string contentEncoding;
		private string contentType;
		private Stream dataStream;

		#endregion

		#region Properties/Indexers/Events

		public IDictionary<string, object> Metadata
		{
			get
			{
				return this.metadata;
			}
		}

		public string ContentEncoding
		{
			get
			{
				return this.contentEncoding;
			}
			set
			{
				this.contentEncoding = value;
			}
		}

		public string ContentType
		{
			get
			{
				return this.contentType;
			}
			set
			{
				this.contentType = value;
			}
		}

		public Stream DataStream
		{
			get
			{
				return this.dataStream;
			}
			set
			{
				this.dataStream = value;
			}
		}

		#endregion

		#region Methods/Operators

		internal static IntegrationMessage RehydrateInstance(Guid rehydratedRunTimeId)
		{
			return new IntegrationMessage(rehydratedRunTimeId);
		}

		protected override void CoreInitialize()
		{
		}

		protected override void CoreTerminate()
		{
			if ((object)this.DataStream != null)
			{
				this.DataStream.Dispose();
				this.DataStream = null;
			}
		}

		#endregion
	}
}