/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Threading;

using TextMetal.Messaging.Core.MessageModel;

namespace TextMetal.Messaging.Core.PipelineModel
{
	public abstract class ReceivePipeline : Pipeline, IReceivePipeline
	{
		#region Constructors/Destructors

		protected ReceivePipeline()
		{
		}

		#endregion

		#region Methods/Operators

		public IEnumerable<IIntegrationMessage> Execute(IPipeliningContext pipeliningContext, IIntegrationMessage integrationMessage)
		{
			IReceiveStage receiveStage;

			IEnumerable<IIntegrationMessage> stagedIntegrationMessages;
			IEnumerable<IIntegrationMessage> splitIntegrationMessages;

			List<IIntegrationMessage> accumulatedIntegrationMessages;

			this.WriteLogSynchronized("PIPELINE: Excuting receive pipeline on thread '{0}'.", Thread.CurrentThread.ManagedThreadId);

			if ((object)pipeliningContext == null)
				throw new ArgumentNullException(nameof(pipeliningContext));

			if ((object)integrationMessage == null)
				throw new ArgumentNullException(nameof(integrationMessage));

			accumulatedIntegrationMessages = new List<IIntegrationMessage>();
			stagedIntegrationMessages = new IIntegrationMessage[] { integrationMessage };

			foreach (Type receiveStageType in this.GetStageTypes().Values)
			{
				receiveStage = Activator.CreateInstance(receiveStageType, true) as IReceiveStage;

				if ((object)receiveStage == null)
					throw new InvalidOperationException("Bad receive stage type.");

				this.TrackStageInstance(receiveStage);

				try
				{
					receiveStage.Initialize();
					receiveStage.Freeze();

					accumulatedIntegrationMessages.Clear();
					foreach (IIntegrationMessage stagedIntegrationMessage in stagedIntegrationMessages)
					{
						splitIntegrationMessages = receiveStage.Execute(pipeliningContext, stagedIntegrationMessage);
						accumulatedIntegrationMessages.AddRange(splitIntegrationMessages);
					}
				}
				finally
				{
					receiveStage.Terminate(); // dispose
				}
			}

			return accumulatedIntegrationMessages;
		}

		#endregion
	}
}