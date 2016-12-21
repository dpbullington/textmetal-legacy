/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Messaging.Core.PipelineModel
{
	internal sealed class PipeliningContext : IntegrationComponent, IPipeliningContext
	{
		#region Constructors/Destructors

		public PipeliningContext(Guid correlationId)
		{
			this.correlationId = correlationId;
		}

		#endregion

		#region Fields/Constants

		private readonly Guid correlationId;

		#endregion

		#region Properties/Indexers/Events

		public Guid CorrelationId
		{
			get
			{
				return this.correlationId;
			}
		}

		#endregion

		#region Methods/Operators

		protected override void CoreInitialize()
		{
		}

		protected override void CoreTerminate()
		{
		}

		#endregion
	}
}