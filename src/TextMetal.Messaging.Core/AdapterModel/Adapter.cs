/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Threading;

using TextMetal.Messaging.Core.MessageModel;
using TextMetal.Middleware.Solder.Injection;

namespace TextMetal.Messaging.Core.AdapterModel
{
	public abstract class Adapter : IntegrationComponent, IAdapter
	{
		#region Constructors/Destructors

		protected Adapter()
		{
		}

		#endregion

		#region Fields/Constants

		private readonly List<IntegrationEndpoint> endpoints = new List<IntegrationEndpoint>();

		#endregion

		#region Properties/Indexers/Events

		private List<IntegrationEndpoint> Endpoints
		{
			get
			{
				return this.endpoints;
			}
		}

		protected IIntegrationMessageStore IntegrationMessageStore
		{
			get
			{
				return AgnosticAppDomain.TheOnlyAllowedInstance.DependencyManager.ResolveDependency<IIntegrationMessageStore>(string.Empty, false);
			}
		}

		#endregion

		#region Methods/Operators

		public void ClearEndpoints()
		{
			this.AssertMutable();
			this.WriteLogSynchronized("ADAPTER: Clearing adapter endpoints on thread '{0}'.", Thread.CurrentThread.ManagedThreadId);

			this.Endpoints.Clear();
		}

		public IReadOnlyCollection<IntegrationEndpoint> GetEndpoints()
		{
			this.WriteLogSynchronized("ADAPTER: Getting adapter endpoints on thread '{0}'.", Thread.CurrentThread.ManagedThreadId);

			return this.Endpoints;
		}

		public void SetEndpoints(IEnumerable<IntegrationEndpoint> endpoints)
		{
			if ((object)endpoints == null)
				throw new ArgumentNullException("endpoints");

			this.AssertMutable();
			this.WriteLogSynchronized("ADAPTER: Setting adapter endpoints on thread '{0}'.", Thread.CurrentThread.ManagedThreadId);

			this.Endpoints.AddRange(endpoints);
		}

		#endregion
	}
}