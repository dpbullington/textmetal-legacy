/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Messaging.Core.MessageModel;

namespace TextMetal.Messaging.Core.AdapterModel
{
	public sealed class InboundMessageEventArgs : EventArgs
	{
		#region Constructors/Destructors

		public InboundMessageEventArgs(IntegrationEndpoint integrationEndpoint, IIntegrationMessage integrationMessage)
		{
			if ((object)integrationEndpoint == null)
				throw new ArgumentNullException(nameof(integrationEndpoint));

			if ((object)integrationMessage == null)
				throw new ArgumentNullException(nameof(integrationMessage));

			this.integrationEndpoint = integrationEndpoint;
			this.integrationMessage = integrationMessage;
		}

		#endregion

		#region Fields/Constants

		private readonly IntegrationEndpoint integrationEndpoint;
		private readonly IIntegrationMessage integrationMessage;

		#endregion

		#region Properties/Indexers/Events

		public IntegrationEndpoint IntegrationEndpoint
		{
			get
			{
				return this.integrationEndpoint;
			}
		}

		public IIntegrationMessage IntegrationMessage
		{
			get
			{
				return this.integrationMessage;
			}
		}

		#endregion
	}
}