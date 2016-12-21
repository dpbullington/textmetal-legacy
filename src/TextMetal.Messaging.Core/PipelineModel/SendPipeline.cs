/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Threading;

using TextMetal.Messaging.Core.MessageModel;

namespace TextMetal.Messaging.Core.PipelineModel
{
	public abstract class SendPipeline : Pipeline, ISendPipeline
	{
		#region Constructors/Destructors

		protected SendPipeline()
		{
		}

		#endregion

		#region Methods/Operators

		public IIntegrationMessage Execute(IPipeliningContext pipeliningContext)
		{
			this.WriteLogSynchronized("PIPELINE: Excuting send pipeline on thread '{0}'.", Thread.CurrentThread.ManagedThreadId);

			if ((object)pipeliningContext == null)
				throw new ArgumentNullException("pipeliningContext");

			return null;
		}

		#endregion
	}
}