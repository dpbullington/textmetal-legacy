/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Messaging.Core.MessageModel;

namespace TextMetal.Messaging.Core.WorkflowModel
{
	public abstract class IntegrationMessageRouter : IntegrationComponent, IIntegrationMessageRouter
	{
		#region Constructors/Destructors

		protected IntegrationMessageRouter()
		{
		}

		#endregion

		#region Methods/Operators

		protected override void CoreInitialize()
		{
		}

		protected abstract void CoreRoute(IIntegrationMessage integrationMessage);

		protected override void CoreTerminate()
		{
		}

		public void Route(IIntegrationMessage integrationMessage)
		{
			if ((object)integrationMessage == null)
				throw new ArgumentNullException("integrationMessage");

			this.CoreRoute(integrationMessage);
		}

		#endregion
	}
}