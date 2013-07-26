// ****************************************************************
// Copyright 2008, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;

using NUnit.Core;

namespace NUnit.Util
{
	/// <summary>
	/// 	Enumeration used to report AgentStatus
	/// </summary>
	public enum AgentStatus
	{
		Unknown,
		Starting,
		Ready,
		Busy,
		Stopping
	}

	/// <summary>
	/// 	The TestAgency class provides RemoteTestAgents
	/// 	on request and tracks their status. Agents
	/// 	are wrapped in an instance of the TestAgent
	/// 	class. Multiple agent types are supported
	/// 	but only one, ProcessAgent is implemented
	/// 	at this time.
	/// </summary>
	public class TestAgency : ServerBase, IAgency, IService
	{
		#region Constructors/Destructors

		public TestAgency()
			: this("TestAgency", 0)
		{
		}

		public TestAgency(string uri, int port)
			: base(uri, port)
		{
		}

		#endregion

		#region Fields/Constants

		private static Logger log = InternalTrace.GetLogger(typeof(TestAgency));

		private AgentDataBase agentData = new AgentDataBase();

		#endregion

		#region Methods/Operators

		/// <summary>
		/// 	Return the NUnit Bin Directory for a particular
		/// 	runtime version, or null if it's not installed.
		/// 	For normal installations, there are only 1.1 and
		/// 	2.0 directories. However, this method accomodates
		/// 	3.5 and 4.0 directories for the benefit of NUnit
		/// 	developers using those runtimes.
		/// </summary>
		private static string GetNUnitBinDirectory(Version v)
		{
			// Get current bin directory
			string dir = NUnitConfiguration.NUnitBinDirectory;

			// Return current directory if current and requested
			// versions are both >= 2 or both 1
			if ((Environment.Version.Major >= 2) == (v.Major >= 2))
				return dir;

			// Check whether special support for version 1 is installed
			if (v.Major == 1)
			{
				string altDir = Path.Combine(dir, "net-1.1");
				if (Directory.Exists(altDir))
					return altDir;

				// The following is only applicable to the dev environment,
				// which uses parallel build directories. We try to substitute
				// one version number for another in the path.
				string[] search = new string[] { "2.0", "3.0", "3.5", "4.0" };
				string[] replace = v.Minor == 0
					                   ? new string[] { "1.0", "1.1" }
					                   : new string[] { "1.1", "1.0" };

				// Look for current value in path so it can be replaced
				string current = null;
				foreach (string s in search)
				{
					if (dir.IndexOf(s) >= 0)
					{
						current = s;
						break;
					}
				}

				// Try the substitution
				if (current != null)
				{
					foreach (string target in replace)
					{
						altDir = dir.Replace(current, target);
						if (Directory.Exists(altDir))
							return altDir;
					}
				}
			}

			return null;
		}

		private static string GetTestAgentExePath(Version v)
		{
			string binDir = GetNUnitBinDirectory(v);
			if (binDir == null)
				return null;

#if CLR_2_0 || CLR_4_0
			Assembly a = Assembly.GetEntryAssembly();
			string agentName = v.Major > 1 && a != null && a.GetName().ProcessorArchitecture == ProcessorArchitecture.X86
				                   ? "nunit-agent-x86.exe"
				                   : "nunit-agent.exe";
#else
            string agentName = "nunit-agent.exe";
#endif

			string agentExePath = Path.Combine(binDir, agentName);
			return File.Exists(agentExePath) ? agentExePath : null;
		}

		private TestAgent CreateRemoteAgent(RuntimeFramework framework, int waitTime)
		{
			Guid agentId = this.LaunchAgentProcess(framework);

			log.Debug("Waiting for agent {0} to register", agentId.ToString("B"));

			int pollTime = 200;
			bool infinite = waitTime == Timeout.Infinite;

			while (infinite || waitTime > 0)
			{
				Thread.Sleep(pollTime);
				if (!infinite)
					waitTime -= pollTime;
				TestAgent agent = this.agentData[agentId].Agent;
				if (agent != null)
				{
					log.Debug("Returning new agent {0}", agentId.ToString("B"));
					return agent;
				}
			}

			return null;
		}

		private AgentRecord FindAvailableAgent()
		{
			foreach (AgentRecord r in this.agentData)
			{
				if (r.Status == AgentStatus.Ready)
				{
					log.Debug("Reusing agent {0}", r.Id);
					r.Status = AgentStatus.Busy;
					return r;
				}
			}

			return null;
		}

		public TestAgent GetAgent()
		{
			return this.GetAgent(RuntimeFramework.CurrentFramework, Timeout.Infinite);
		}

		public TestAgent GetAgent(int waitTime)
		{
			return this.GetAgent(RuntimeFramework.CurrentFramework, waitTime);
		}

		public TestAgent GetAgent(RuntimeFramework framework, int waitTime)
		{
			log.Info("Getting agent for use under {0}", framework);

			if (!framework.IsAvailable)
			{
				throw new ArgumentException(
					string.Format("The {0} framework is not available", framework),
					"framework");
			}

			// TODO: Decide if we should reuse agents
			//AgentRecord r = FindAvailableRemoteAgent(type);
			//if ( r == null )
			//    r = CreateRemoteAgent(type, framework, waitTime);
			return this.CreateRemoteAgent(framework, waitTime);
		}

		public void InitializeService()
		{
			this.Start();
		}

		/// <summary>
		/// 	Returns true if NUnit support for the runtime specified 
		/// 	is installed, independent of whether the runtime itself
		/// 	is installed on the system.
		/// 
		/// 	In the current implementation, only .NET 1.x requires
		/// 	special handling, since all higher runtimes are 
		/// 	supported normally.
		/// </summary>
		/// <param name="version"> The desired runtime version </param>
		/// <returns> True if NUnit support is installed </returns>
		public bool IsRuntimeVersionSupported(Version version)
		{
			return GetNUnitBinDirectory(version) != null;
		}

		//public void DestroyAgent( ITestAgent agent )
		//{
		//    AgentRecord r = agentData[agent.Id];
		//    if ( r != null )
		//    {
		//        if( !r.Process.HasExited )
		//            r.Agent.Stop();
		//        agentData[r.Id] = null;
		//    }
		//}

		private Guid LaunchAgentProcess(RuntimeFramework targetRuntime)
		{
			string agentExePath = GetTestAgentExePath(targetRuntime.ClrVersion);

			if (agentExePath == null)
			{
				throw new ArgumentException(
					string.Format("NUnit components for version {0} of the CLR are not installed",
					              targetRuntime.ClrVersion.ToString()), "targetRuntime");
			}

			log.Debug("Using nunit-agent at " + agentExePath);

			Process p = new Process();
			p.StartInfo.UseShellExecute = false;
			Guid agentId = Guid.NewGuid();
			string arglist = agentId.ToString() + " " + this.ServerUrl;
			if (Debugger.IsAttached)
				arglist += " --launch-debugger";

			switch (targetRuntime.Runtime)
			{
				case RuntimeType.Mono:
					p.StartInfo.FileName = NUnitConfiguration.MonoExePath;
					string monoOptions = "--runtime=v" + targetRuntime.ClrVersion.ToString(3);
					if (Debugger.IsAttached)
						monoOptions += " --debug";
					p.StartInfo.Arguments = string.Format("{0} \"{1}\" {2}", monoOptions, agentExePath, arglist);
					break;
				case RuntimeType.Net:
					p.StartInfo.FileName = agentExePath;

					if (targetRuntime.ClrVersion.Build < 0)
						targetRuntime = RuntimeFramework.GetBestAvailableFramework(targetRuntime);

					string envVar = "v" + targetRuntime.ClrVersion.ToString(3);
					p.StartInfo.EnvironmentVariables["COMPLUS_Version"] = envVar;

					p.StartInfo.Arguments = arglist;
					break;
				default:
					p.StartInfo.FileName = agentExePath;
					p.StartInfo.Arguments = arglist;
					break;
			}

			//p.Exited += new EventHandler(OnProcessExit);
			p.Start();
			log.Info("Launched Agent process {0} - see nunit-agent_{0}.log", p.Id);
			log.Info("Command line: \"{0}\" {1}", p.StartInfo.FileName, p.StartInfo.Arguments);

			this.agentData.Add(new AgentRecord(agentId, p, null, AgentStatus.Starting));
			return agentId;
		}

		public void Register(TestAgent agent)
		{
			AgentRecord r = this.agentData[agent.Id];
			if (r == null)
			{
				throw new ArgumentException(
					string.Format("Agent {0} is not in the agency database", agent.Id),
					"agentId");
			}
			r.Agent = agent;
		}

		public void ReleaseAgent(TestAgent agent)
		{
			AgentRecord r = this.agentData[agent.Id];
			if (r == null)
				log.Error(string.Format("Unable to release agent {0} - not in database", agent.Id));
			else
			{
				r.Status = AgentStatus.Ready;
				log.Debug("Releasing agent " + agent.Id.ToString());
			}
		}

		public void ReportStatus(Guid agentId, AgentStatus status)
		{
			AgentRecord r = this.agentData[agentId];

			if (r == null)
			{
				throw new ArgumentException(
					string.Format("Agent {0} is not in the agency database", agentId),
					"agentId");
			}

			r.Status = status;
		}

		public override void Stop()
		{
			foreach (AgentRecord r in this.agentData)
			{
				if (!r.Process.HasExited)
				{
					if (r.Agent != null)
					{
						r.Agent.Stop();
						r.Process.WaitForExit(10000);
					}

					if (!r.Process.HasExited)
						r.Process.Kill();
				}
			}

			this.agentData.Clear();

			base.Stop();
		}

		//private void OnProcessExit(object sender, EventArgs e)
		//{
		//    Process p = sender as Process;
		//    if (p != null)
		//        agentData.Remove(p.Id);
		//}

		public void UnloadService()
		{
			this.Stop();
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		/// <summary>
		/// 	A simple class that tracks data about this
		/// 	agencies active and available agents
		/// </summary>
		private class AgentDataBase : IEnumerable
		{
			#region Fields/Constants

			private ListDictionary agentData = new ListDictionary();

			#endregion

			#region Properties/Indexers/Events

			public AgentRecord this[Guid id]
			{
				get
				{
					return (AgentRecord)this.agentData[id];
				}
				set
				{
					if (value == null)
						this.agentData.Remove(id);
					else
						this.agentData[id] = value;
				}
			}

			public AgentRecord this[TestAgent agent]
			{
				get
				{
					foreach (DictionaryEntry entry in this.agentData)
					{
						AgentRecord r = (AgentRecord)entry.Value;
						if (r.Agent == agent)
							return r;
					}

					return null;
				}
			}

			#endregion

			#region Methods/Operators

			public void Add(AgentRecord r)
			{
				this.agentData[r.Id] = r;
			}

			public void Clear()
			{
				this.agentData.Clear();
			}

			public IEnumerator GetEnumerator()
			{
				return new AgentDataEnumerator(this.agentData);
			}

			public void Remove(Guid agentId)
			{
				this.agentData.Remove(agentId);
			}

			#endregion

			#region Classes/Structs/Interfaces/Enums/Delegates

			public class AgentDataEnumerator : IEnumerator
			{
				#region Constructors/Destructors

				public AgentDataEnumerator(IDictionary list)
				{
					this.innerEnum = list.GetEnumerator();
				}

				#endregion

				#region Fields/Constants

				private IEnumerator innerEnum;

				#endregion

				#region Properties/Indexers/Events

				public object Current
				{
					get
					{
						return ((DictionaryEntry)this.innerEnum.Current).Value;
					}
				}

				#endregion

				#region Methods/Operators

				public bool MoveNext()
				{
					return this.innerEnum.MoveNext();
				}

				public void Reset()
				{
					this.innerEnum.Reset();
				}

				#endregion
			}

			#endregion
		}

		private class AgentRecord
		{
			#region Constructors/Destructors

			public AgentRecord(Guid id, Process p, TestAgent a, AgentStatus s)
			{
				this.Id = id;
				this.Process = p;
				this.Agent = a;
				this.Status = s;
			}

			#endregion

			#region Fields/Constants

			public TestAgent Agent;

			public Guid Id;
			public Process Process;
			public AgentStatus Status;

			#endregion
		}

		#endregion
	}
}