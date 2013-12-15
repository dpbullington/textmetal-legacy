// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace NUnit.Core
{
	/// <summary>
	/// TestAssemblyInfo holds information about a loaded test assembly
	/// </summary>
	[Serializable]
	public class TestAssemblyInfo
	{
		#region Constructors/Destructors

		/// <summary>
		/// Constructs a TestAssemblyInfo
		/// </summary>
		/// <param name="assemblyName"> The name of the assembly </param>
		/// <param name="imageRuntimeVersion"> The version of the runtime for which the assembly was built </param>
		/// <param name="runnerRuntimeFramework"> The runtime framework under which the assembly is loaded </param>
		/// <param name="testFrameworks"> A list of test framework useds by the assembly </param>
		public TestAssemblyInfo(string assemblyName, Version imageRuntimeVersion, RuntimeFramework runnerRuntimeFramework, IList testFrameworks)
		{
			this.assemblyName = assemblyName;
			this.imageRuntimeVersion = imageRuntimeVersion;
			this.runnerRuntimeFramework = runnerRuntimeFramework;
			this.testFrameworks = testFrameworks;
			Process p = Process.GetCurrentProcess();
			this.processId = p.Id;
			Assembly entryAssembly = Assembly.GetEntryAssembly();
			this.moduleName = entryAssembly != null
				? Path.GetFileName(Assembly.GetEntryAssembly().Location)
				: p.MainModule.ModuleName;
			this.domainName = AppDomain.CurrentDomain.FriendlyName;
			this.appBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
			this.configFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
			this.binPath = AppDomain.CurrentDomain.SetupInformation.PrivateBinPath;
		}

		#endregion

		#region Fields/Constants

		private string appBase;
		private string assemblyName;
		private string binPath;
		private string configFile;
		private string domainName;
		private Version imageRuntimeVersion;
		private string moduleName;
		private int processId;
		private RuntimeFramework runnerRuntimeFramework;
		private IList testFrameworks;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// The Application Base of the AppDomain in which the assembly is loaded
		/// </summary>
		public string ApplicationBase
		{
			get
			{
				return this.appBase;
			}
		}

		/// <summary>
		/// The ConfigurationFile of the AppDomain in which the assembly is loaded
		/// </summary>
		public string ConfigurationFile
		{
			get
			{
				return this.configFile;
			}
		}

		/// <summary>
		/// The friendly name of the AppDomain in which the assembly is loaded
		/// </summary>
		public string DomainName
		{
			get
			{
				return this.domainName;
			}
		}

		/// <summary>
		/// Gets the runtime version for which the assembly was built
		/// </summary>
		public Version ImageRuntimeVersion
		{
			get
			{
				return this.imageRuntimeVersion;
			}
		}

		/// <summary>
		/// The name of the main module of the process in which the assembly is loaded
		/// </summary>
		public string ModuleName
		{
			get
			{
				return this.moduleName;
			}
			set
			{
				this.moduleName = value;
			}
		}

		/// <summary>
		/// Gets the name of the assembly
		/// </summary>
		public string Name
		{
			get
			{
				return this.assemblyName;
			}
		}

		/// <summary>
		/// The PrivateBinPath of the AppDomain in which the assembly is loaded
		/// </summary>
		public string PrivateBinPath
		{
			get
			{
				return this.binPath;
			}
		}

		/// <summary>
		/// The Id of the process in which the assembly is loaded
		/// </summary>
		public int ProcessId
		{
			get
			{
				return this.processId;
			}
		}

		/// <summary>
		/// Gets the runtime framework under which the assembly is loaded
		/// </summary>
		public RuntimeFramework RunnerRuntimeFramework
		{
			get
			{
				return this.runnerRuntimeFramework;
			}
		}

		/// <summary>
		/// Gets the runtime version under which the assembly is loaded
		/// </summary>
		public Version RunnerRuntimeVersion
		{
			get
			{
				return this.runnerRuntimeFramework.ClrVersion;
			}
		}

		/// <summary>
		/// Gets a list of testframeworks referenced by the assembly
		/// </summary>
		public IList TestFrameworks
		{
			get
			{
				return this.testFrameworks;
			}
		}

		#endregion
	}
}