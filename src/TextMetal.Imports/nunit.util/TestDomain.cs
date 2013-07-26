// ****************************************************************
// This is free software licensed under the NUnit license. You
// may obtain a copy of the license as well as information regarding
// copyright ownership at http://nunit.org
// ****************************************************************

using System;
using System.Diagnostics;

using NUnit.Core;

namespace NUnit.Util
{
	public class TestDomain : ProxyTestRunner, TestRunner
	{
		#region Constructors/Destructors

		public TestDomain()
			: base(0)
		{
		}

		public TestDomain(int runnerID)
			: base(runnerID)
		{
		}

		#endregion

		#region Fields/Constants

		private static Logger log = InternalTrace.GetLogger(typeof(TestDomain));

		/// <summary>
		/// 	The TestAgent in the domain
		/// </summary>
		private DomainAgent agent;

		/// <summary>
		/// 	The appdomain used  to load tests
		/// </summary>
		private AppDomain domain;

		#endregion

		#region Properties/Indexers/Events

		public AppDomain AppDomain
		{
			get
			{
				return this.domain;
			}
		}

		#endregion

		#region Methods/Operators

		public override void BeginRun(EventListener listener, ITestFilter filter, bool tracing, LoggingThreshold logLevel)
		{
			log.Info("BeginRun in AppDomain {0}", this.domain.FriendlyName);
			base.BeginRun(listener, filter, tracing, logLevel);
		}

		public override void Dispose()
		{
			base.Dispose();

			this.Unload();
		}

		public override bool Load(TestPackage package)
		{
			this.Unload();

			log.Info("Loading " + package.Name);
			try
			{
				if (this.domain == null)
					this.domain = Services.DomainManager.CreateDomain(package);

				if (this.agent == null)
				{
					this.agent = DomainAgent.CreateInstance(this.domain);
					this.agent.Start();
				}

				if (this.TestRunner == null)
					this.TestRunner = this.agent.CreateRunner(this.ID);

				log.Info(
					"Loading tests in AppDomain, see {0}_{1}.log",
					this.domain.FriendlyName,
					Process.GetCurrentProcess().Id);

				return this.TestRunner.Load(package);
			}
			catch
			{
				log.Error("Load failure");
				this.Unload();
				throw;
			}
		}

		public override void Unload()
		{
			if (this.TestRunner != null)
			{
				log.Info("Unloading");
				this.TestRunner.Unload();
				this.TestRunner = null;
			}

			if (this.agent != null)
			{
				log.Info("Stopping DomainAgent");
				this.agent.Dispose();
				this.agent = null;
			}

			if (this.domain != null)
			{
				log.Info("Unloading AppDomain " + this.domain.FriendlyName);
				Services.DomainManager.Unload(this.domain);
				this.domain = null;
			}
		}

		#endregion
	}
}