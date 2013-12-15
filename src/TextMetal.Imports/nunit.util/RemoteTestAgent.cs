// ****************************************************************
// Copyright 2008, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Threading;

using NUnit.Core;

namespace NUnit.Util
{
	/// <summary>
	/// RemoteTestAgent represents a remote agent executing in another process
	/// and communicating with NUnit by TCP. Although it is similar to a
	/// TestServer, it does not publish a Uri at which clients may connect
	/// to it. Rather, it reports back to the sponsoring TestAgency upon
	/// startup so that the agency may in turn provide it to clients for use.
	/// </summary>
	public class RemoteTestAgent : TestAgent
	{
		#region Constructors/Destructors

		/// <summary>
		/// Construct a RemoteTestAgent
		/// </summary>
		public RemoteTestAgent(Guid agentId, TestAgency agency)
			: base(agentId, agency)
		{
		}

		#endregion

		#region Fields/Constants

		private static Logger log = InternalTrace.GetLogger(typeof(RemoteTestAgent));

		private ManualResetEvent stopSignal = new ManualResetEvent(false);

		#endregion

		#region Properties/Indexers/Events

		public int ProcessId
		{
			get
			{
				return Process.GetCurrentProcess().Id;
			}
		}

		#endregion

		#region Methods/Operators

		public override TestRunner CreateRunner(int runnerID)
		{
			return new AgentRunner(runnerID);
		}

		public override bool Start()
		{
			log.Info("Agent starting");

			try
			{
				this.Agency.Register(this);
				log.Debug("Registered with TestAgency");
			}
			catch (Exception ex)
			{
				log.Error("RemoteTestAgent: Failed to register with TestAgency", ex);
				return false;
			}

			return true;
		}

		[OneWay]
		public override void Stop()
		{
			log.Info("Stopping");
			// This causes an error in the client because the agent 
			// database is not thread-safe.
			//if ( agency != null )
			//    agency.ReportStatus(this.ProcessId, AgentStatus.Stopping);

			this.stopSignal.Set();
		}

		public void WaitForStop()
		{
			this.stopSignal.WaitOne();
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		private class AgentRunner : ProxyTestRunner
		{
			#region Constructors/Destructors

			public AgentRunner(int runnerID)
				: base(runnerID)
			{
				this.factory = new InProcessTestRunnerFactory();
			}

			#endregion

			#region Fields/Constants

			private ITestRunnerFactory factory;

			#endregion

			#region Properties/Indexers/Events

			public override IList AssemblyInfo
			{
				get
				{
					IList result = base.AssemblyInfo;
					string name = Path.GetFileName(Assembly.GetEntryAssembly().Location);

					foreach (TestAssemblyInfo info in result)
						info.ModuleName = name;

					return result;
				}
			}

			#endregion

			#region Methods/Operators

			public override bool Load(TestPackage package)
			{
				this.TestRunner = this.factory.MakeTestRunner(package);

				return base.Load(package);
			}

			#endregion
		}

		#endregion
	}
}