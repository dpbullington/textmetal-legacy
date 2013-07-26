// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.IO;

using NUnit.Core;

namespace NUnit.Util
{
	/// <summary>
	/// 	Summary description for ProcessRunner.
	/// </summary>
	public class ProcessRunner : ProxyTestRunner
	{
		#region Constructors/Destructors

		public ProcessRunner()
			: base(0)
		{
		}

		public ProcessRunner(int runnerID)
			: base(runnerID)
		{
		}

		#endregion

		#region Fields/Constants

		private static Logger log = InternalTrace.GetLogger(typeof(ProcessRunner));

		private TestAgent agent;

		private RuntimeFramework runtimeFramework;

		#endregion

		#region Properties/Indexers/Events

		public RuntimeFramework RuntimeFramework
		{
			get
			{
				return this.runtimeFramework;
			}
		}

		#endregion

		#region Methods/Operators

		public override void Dispose()
		{
			// Do this first, because the next step will
			// make the downstream runner inaccessible.
			base.Dispose();

			if (this.agent != null)
			{
				log.Info("Stopping remote agent");
				try
				{
					this.agent.Stop();
				}
				catch
				{
					// Ignore any exception
				}
				finally
				{
					this.agent = null;
				}
			}
		}

		public override bool Load(TestPackage package)
		{
			log.Info("Loading " + package.Name);
			this.Unload();

			this.runtimeFramework = package.Settings["RuntimeFramework"] as RuntimeFramework;
			if (this.runtimeFramework == null)
				this.runtimeFramework = RuntimeFramework.CurrentFramework;

			bool loaded = false;

			try
			{
				if (this.agent == null)
				{
					this.agent = Services.TestAgency.GetAgent(
						this.runtimeFramework,
						30000);

					if (this.agent == null)
						return false;
				}

				if (this.TestRunner == null)
					this.TestRunner = this.agent.CreateRunner(this.runnerID);

				loaded = base.Load(package);
				return loaded;
			}
			finally
			{
				// Clean up if the load failed
				if (!loaded)
					this.Unload();
			}
		}

		public override void Unload()
		{
			if (this.Test != null)
			{
				log.Info("Unloading " + Path.GetFileName(this.Test.TestName.Name));
				this.TestRunner.Unload();
				this.TestRunner = null;
			}
		}

		#endregion
	}
}