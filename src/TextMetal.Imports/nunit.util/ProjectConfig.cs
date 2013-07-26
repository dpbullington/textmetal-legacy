// ****************************************************************
// This is free software licensed under the NUnit license. You
// may obtain a copy of the license as well as information regarding
// copyright ownership at http://nunit.org.
// ****************************************************************

using System;
using System.IO;

using NUnit.Core;

namespace NUnit.Util
{
	public enum BinPathType
	{
		Auto,
		Manual,
		None
	}

	public class ProjectConfig
	{
		#region Constructors/Destructors

		public ProjectConfig(string name)
		{
			this.name = name;
			this.assemblies = new AssemblyList();
			this.assemblies.Changed += new EventHandler(this.assemblies_Changed);
		}

		#endregion

		#region Fields/Constants

		/// <summary>
		/// 	List of the names of the assemblies
		/// </summary>
		private AssemblyList assemblies;

		/// <summary>
		/// 	Base path specific to this configuration
		/// </summary>
		private string basePath;

		/// <summary>
		/// 	Private bin path, if specified
		/// </summary>
		private string binPath;

		/// <summary>
		/// 	True if assembly paths should be added to bin path
		/// </summary>
		private BinPathType binPathType = BinPathType.Auto;

		/// <summary>
		/// 	Our configuration file, if specified
		/// </summary>
		private string configFile;

		/// <summary>
		/// 	The name of this config
		/// </summary>
		private string name;

		/// <summary>
		/// 	IProject interface of containing project
		/// </summary>
		protected NUnitProject project = null;

		/// <summary>
		/// 	The CLR under which tests are to be run
		/// </summary>
		private RuntimeFramework runtimeFramework;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// 	Return our AssemblyList
		/// </summary>
		public AssemblyList Assemblies
		{
			get
			{
				return this.assemblies;
			}
		}

		/// <summary>
		/// 	The base directory for this config - used
		/// 	as the application base for loading tests.
		/// </summary>
		public string BasePath
		{
			get
			{
				if (this.project == null || this.project.BasePath == null)
					return this.basePath;

				if (this.basePath == null)
					return this.project.BasePath;

				return Path.Combine(this.project.BasePath, this.basePath);
			}
			set
			{
				if (this.BasePath != value)
				{
					this.basePath = value;
					this.NotifyProjectOfChange();
				}
			}
		}

		private bool BasePathSpecified
		{
			get
			{
				return this.project.BasePathSpecified || this.basePath != null && this.basePath != "";
			}
		}

		/// <summary>
		/// 	How our PrivateBinPath is generated
		/// </summary>
		public BinPathType BinPathType
		{
			get
			{
				return this.binPathType;
			}
			set
			{
				if (this.binPathType != value)
				{
					this.binPathType = value;
					this.NotifyProjectOfChange();
				}
			}
		}

		public string ConfigurationFile
		{
			get
			{
				return this.configFile == null && this.project != null
					       ? this.project.ConfigurationFile
					       : this.configFile;
			}
			set
			{
				if (this.ConfigurationFile != value)
				{
					this.configFile = value;
					this.NotifyProjectOfChange();
				}
			}
		}

		public string ConfigurationFilePath
		{
			get
			{
				return this.BasePath != null && this.ConfigurationFile != null
					       ? Path.Combine(this.BasePath, this.ConfigurationFile)
					       : this.ConfigurationFile;
			}
		}

		private bool ConfigurationFileSpecified
		{
			get
			{
				return this.configFile != null;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				if (this.name != value)
				{
					this.name = value;
					this.NotifyProjectOfChange();
				}
			}
		}

		/// <summary>
		/// 	The Path.PathSeparator-separated path containing all the
		/// 	assemblies in the list.
		/// </summary>
		public string PrivateBinPath
		{
			get
			{
				return this.binPath;
			}
			set
			{
				if (this.binPath != value)
				{
					this.binPath = value;
					this.binPathType = this.binPath == null ? BinPathType.Auto : BinPathType.Manual;
					this.NotifyProjectOfChange();
				}
			}
		}

		private bool PrivateBinPathSpecified
		{
			get
			{
				return this.binPath != null;
			}
		}

		public NUnitProject Project
		{
			set
			{
				this.project = value;
			}
		}

		/// <summary>
		/// 	The base path relative to the project base
		/// </summary>
		public string RelativeBasePath
		{
			get
			{
				if (this.project == null || this.basePath == null || !Path.IsPathRooted(this.basePath))
					return this.basePath;

				return PathUtils.RelativePath(this.project.BasePath, this.basePath);
			}
		}

		public RuntimeFramework RuntimeFramework
		{
			get
			{
				return this.runtimeFramework;
			}
			set
			{
				if (this.runtimeFramework != value)
				{
					this.runtimeFramework = value;
					this.NotifyProjectOfChange();
				}
			}
		}

		#endregion

		#region Methods/Operators

		public TestPackage MakeTestPackage()
		{
			TestPackage package = new TestPackage(this.project.ProjectPath);

			if (!this.project.IsAssemblyWrapper)
			{
				foreach (string assembly in this.Assemblies)
					package.Assemblies.Add(assembly);
			}

			if (this.BasePathSpecified || this.PrivateBinPathSpecified || this.ConfigurationFileSpecified)
			{
				package.BasePath = this.BasePath;
				package.PrivateBinPath = this.PrivateBinPath;
				package.ConfigurationFile = this.ConfigurationFile;
			}

			package.AutoBinPath = this.BinPathType == BinPathType.Auto;
			if (this.RuntimeFramework != null)
				package.Settings["RuntimeFramework"] = this.RuntimeFramework;

			if (this.project.ProcessModel != ProcessModel.Default)
				package.Settings["ProcessModel"] = this.project.ProcessModel;

			if (this.project.DomainUsage != DomainUsage.Default)
				package.Settings["DomainUsage"] = this.project.DomainUsage;

			return package;
		}

		private void NotifyProjectOfChange()
		{
			if (this.project != null)
			{
				this.project.IsDirty = true;
				if (ReferenceEquals(this, this.project.ActiveConfig))
					this.project.HasChangesRequiringReload = true;
			}
		}

		private void assemblies_Changed(object sender, EventArgs e)
		{
			this.NotifyProjectOfChange();
		}

		#endregion
	}
}