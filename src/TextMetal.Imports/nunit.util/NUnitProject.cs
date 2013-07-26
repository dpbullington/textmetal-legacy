// ****************************************************************
// This is free software licensed under the NUnit license. You
// may obtain a copy of the license as well as information regarding
// copyright ownership at http://nunit.org.
// ****************************************************************

using System;
using System.IO;
using System.Text;
using System.Xml;

using NUnit.Core;

namespace NUnit.Util
{
	/// <summary>
	/// 	Class that represents an NUnit test project
	/// </summary>
	public class NUnitProject
	{
		#region Constructors/Destructors

		public NUnitProject(string projectPath)
		{
			this.projectPath = Path.GetFullPath(projectPath);
			this.configs = new ProjectConfigCollection(this);
		}

		#endregion

		#region Fields/Constants

		public static readonly string Extension = ".nunit";

		/// <summary>
		/// 	The currently active configuration
		/// </summary>
		private ProjectConfig activeConfig;

		/// <summary>
		/// 	True for NUnit-related projects that follow the config
		/// 	of the NUnit build under which they are running.
		/// </summary>
		private bool autoConfig;

		/// <summary>
		/// 	Application Base for the project. Since this
		/// 	can be null, always fetch from the property
		/// 	rather than using the field directly.
		/// </summary>
		private string basePath;

		/// <summary>
		/// 	Collection of configs for the project
		/// </summary>
		private ProjectConfigCollection configs;

		/// <summary>
		/// 	The DomainUsage setting to be used in loading this project
		/// </summary>
		private DomainUsage domainUsage;

		/// <summary>
		/// 	Flag indicating that this project is a
		/// 	temporary wrapper for an assembly.
		/// </summary>
		private bool isAssemblyWrapper = false;

		/// <summary>
		/// 	Whether the project is dirty
		/// </summary>
		private bool isDirty = false;

		/// <summary>
		/// 	The ProcessModel to be used in loading this project
		/// </summary>
		private ProcessModel processModel;

		/// <summary>
		/// 	Path to the file storing this project
		/// </summary>
		private string projectPath;

		/// <summary>
		/// 	Whether canges have been made requiring a reload
		/// </summary>
		private bool reloadRequired = false;

		#endregion

		#region Properties/Indexers/Events

		public ProjectConfig ActiveConfig
		{
			get
			{
				// In case the previous active config was removed
				if (this.activeConfig != null && !this.configs.Contains(this.activeConfig))
					this.activeConfig = null;

				// In case no active config is set or it was removed
				if (this.activeConfig == null && this.configs.Count > 0)
					this.activeConfig = this.configs[0];

				return this.activeConfig;
			}
		}

		// Safe access to name of the active config
		public string ActiveConfigName
		{
			get
			{
				ProjectConfig config = this.ActiveConfig;
				return config == null ? null : config.Name;
			}
		}

		public bool AutoConfig
		{
			get
			{
				return this.autoConfig;
			}
			set
			{
				this.autoConfig = value;
			}
		}

		/// <summary>
		/// 	The base path for the project. Constructor sets
		/// 	it to the directory part of the project path.
		/// </summary>
		public string BasePath
		{
			get
			{
				if (!this.BasePathSpecified)
					return this.DefaultBasePath;
				return this.basePath;
			}
			set
			{
				this.basePath = value;

				if (this.basePath != null && this.basePath != string.Empty
				    && !Path.IsPathRooted(this.basePath))
				{
					this.basePath = Path.Combine(
						this.DefaultBasePath,
						this.basePath);
				}

				this.basePath = PathUtils.Canonicalize(this.basePath);
				this.HasChangesRequiringReload = this.IsDirty = true;
			}
		}

		/// <summary>
		/// 	Indicates whether a base path was specified for the project
		/// </summary>
		public bool BasePathSpecified
		{
			get
			{
				return this.basePath != null && this.basePath != string.Empty;
			}
		}

		public ProjectConfigCollection Configs
		{
			get
			{
				return this.configs;
			}
		}

		public string ConfigurationFile
		{
			get
			{
				// TODO: Check this
				return this.isAssemblyWrapper
					       ? Path.GetFileName(this.projectPath) + ".config"
					       : Path.GetFileNameWithoutExtension(this.projectPath) + ".config";
			}
		}

		public string DefaultBasePath
		{
			get
			{
				return Path.GetDirectoryName(this.projectPath);
			}
		}

		public DomainUsage DomainUsage
		{
			get
			{
				return this.domainUsage;
			}
			set
			{
				this.domainUsage = value;
				this.HasChangesRequiringReload = this.IsDirty = true;
			}
		}

		public bool HasChangesRequiringReload
		{
			get
			{
				return this.reloadRequired;
			}
			set
			{
				this.reloadRequired = value;
			}
		}

		public bool IsAssemblyWrapper
		{
			get
			{
				return this.isAssemblyWrapper;
			}
			set
			{
				this.isAssemblyWrapper = value;
			}
		}

		public bool IsDirty
		{
			get
			{
				return this.isDirty;
			}
			set
			{
				this.isDirty = value;

				if (this.isAssemblyWrapper && value == true)
				{
					this.projectPath = Path.ChangeExtension(this.projectPath, ".nunit");
					this.isAssemblyWrapper = false;
					this.HasChangesRequiringReload = true;
				}
			}
		}

		public bool IsLoadable
		{
			get
			{
				return this.ActiveConfig != null &&
				       this.ActiveConfig.Assemblies.Count > 0;
			}
		}

		/// <summary>
		/// 	The name of the project.
		/// </summary>
		public string Name
		{
			get
			{
				return Path.GetFileNameWithoutExtension(this.projectPath);
			}
		}

		public ProcessModel ProcessModel
		{
			get
			{
				return this.processModel;
			}
			set
			{
				this.processModel = value;
				this.HasChangesRequiringReload = this.IsDirty = true;
			}
		}

		/// <summary>
		/// 	The path to which a project will be saved.
		/// </summary>
		public string ProjectPath
		{
			get
			{
				return this.projectPath;
			}
			set
			{
				this.projectPath = Path.GetFullPath(value);
				this.isDirty = true;
			}
		}

		#endregion

		#region Methods/Operators

		public static bool IsNUnitProjectFile(string path)
		{
			return Path.GetExtension(path) == Extension;
		}

		public static string ProjectPathFromFile(string path)
		{
			string fileName = Path.GetFileNameWithoutExtension(path) + Extension;
			return Path.Combine(Path.GetDirectoryName(path), fileName);
		}

		public void Add(VSProject vsProject)
		{
			foreach (VSProjectConfig vsConfig in vsProject.Configs)
			{
				string name = vsConfig.Name;

				if (!this.configs.Contains(name))
					this.configs.Add(name);

				ProjectConfig config = this.Configs[name];

				foreach (string assembly in vsConfig.Assemblies)
					config.Assemblies.Add(assembly);
			}
		}

		public void Load()
		{
			XmlTextReader reader = new XmlTextReader(this.projectPath);

			string activeConfigName = null;
			ProjectConfig currentConfig = null;

			try
			{
				reader.MoveToContent();
				if (reader.NodeType != XmlNodeType.Element || reader.Name != "NUnitProject")
				{
					throw new ProjectFormatException(
						"Invalid project format: <NUnitProject> expected.",
						reader.LineNumber, reader.LinePosition);
				}

				while (reader.Read())
				{
					if (reader.NodeType == XmlNodeType.Element)
					{
						switch (reader.Name)
						{
							case "Settings":
								if (reader.NodeType == XmlNodeType.Element)
								{
									activeConfigName = reader.GetAttribute("activeconfig");

									string autoConfig = reader.GetAttribute("autoconfig");
									if (autoConfig != null)
										this.AutoConfig = autoConfig.ToLower() == "true";
									if (this.AutoConfig)
										activeConfigName = NUnitConfiguration.BuildConfiguration;

									string appbase = reader.GetAttribute("appbase");
									if (appbase != null)
										this.BasePath = appbase;

									string processModel = reader.GetAttribute("processModel");
									if (processModel != null)
										this.ProcessModel = (ProcessModel)Enum.Parse(typeof(ProcessModel), processModel);

									string domainUsage = reader.GetAttribute("domainUsage");
									if (domainUsage != null)
										this.DomainUsage = (DomainUsage)Enum.Parse(typeof(DomainUsage), domainUsage);
								}
								break;

							case "Config":
								if (reader.NodeType == XmlNodeType.Element)
								{
									string configName = reader.GetAttribute("name");
									currentConfig = new ProjectConfig(configName);
									currentConfig.BasePath = reader.GetAttribute("appbase");
									currentConfig.ConfigurationFile = reader.GetAttribute("configfile");

									string binpath = reader.GetAttribute("binpath");
									currentConfig.PrivateBinPath = binpath;
									string type = reader.GetAttribute("binpathtype");
									if (type == null)
									{
										if (binpath == null)
											currentConfig.BinPathType = BinPathType.Auto;
										else
											currentConfig.BinPathType = BinPathType.Manual;
									}
									else
										currentConfig.BinPathType = (BinPathType)Enum.Parse(typeof(BinPathType), type, true);

									string runtime = reader.GetAttribute("runtimeFramework");
									if (runtime != null)
										currentConfig.RuntimeFramework = RuntimeFramework.Parse(runtime);

									this.Configs.Add(currentConfig);
									if (configName == activeConfigName)
										this.activeConfig = currentConfig;
								}
								else if (reader.NodeType == XmlNodeType.EndElement)
									currentConfig = null;
								break;

							case "assembly":
								if (reader.NodeType == XmlNodeType.Element && currentConfig != null)
								{
									string path = reader.GetAttribute("path");
									currentConfig.Assemblies.Add(
										Path.Combine(currentConfig.BasePath, path));
								}
								break;

							default:
								break;
						}
					}
				}

				this.IsDirty = false;
				this.reloadRequired = false;
			}
			catch (FileNotFoundException)
			{
				throw;
			}
			catch (XmlException e)
			{
				throw new ProjectFormatException(
					string.Format("Invalid project format: {0}", e.Message),
					e.LineNumber, e.LinePosition);
			}
			catch (Exception e)
			{
				throw new ProjectFormatException(
					string.Format("Invalid project format: {0} Line {1}, Position {2}",
					              e.Message, reader.LineNumber, reader.LinePosition),
					reader.LineNumber, reader.LinePosition);
			}
			finally
			{
				reader.Close();
			}
		}

		public void Save()
		{
			this.projectPath = ProjectPathFromFile(this.projectPath);

			XmlTextWriter writer = new XmlTextWriter(this.projectPath, Encoding.UTF8);
			writer.Formatting = Formatting.Indented;

			writer.WriteStartElement("NUnitProject");

			if (this.configs.Count > 0 || this.BasePath != this.DefaultBasePath)
			{
				writer.WriteStartElement("Settings");
				if (this.configs.Count > 0)
					writer.WriteAttributeString("activeconfig", this.ActiveConfigName);
				if (this.BasePath != this.DefaultBasePath)
					writer.WriteAttributeString("appbase", this.BasePath);
				if (this.AutoConfig)
					writer.WriteAttributeString("autoconfig", "true");
				if (this.ProcessModel != ProcessModel.Default)
					writer.WriteAttributeString("processModel", this.ProcessModel.ToString());
				if (this.DomainUsage != DomainUsage.Default)
					writer.WriteAttributeString("domainUsage", this.DomainUsage.ToString());
				writer.WriteEndElement();
			}

			foreach (ProjectConfig config in this.Configs)
			{
				writer.WriteStartElement("Config");
				writer.WriteAttributeString("name", config.Name);
				string appbase = config.BasePath;
				if (!PathUtils.SamePathOrUnder(this.BasePath, appbase))
					writer.WriteAttributeString("appbase", appbase);
				else if (config.RelativeBasePath != null)
					writer.WriteAttributeString("appbase", config.RelativeBasePath);

				string configFile = config.ConfigurationFile;
				if (configFile != null && configFile != this.ConfigurationFile)
					writer.WriteAttributeString("configfile", config.ConfigurationFile);

				if (config.BinPathType == BinPathType.Manual)
					writer.WriteAttributeString("binpath", config.PrivateBinPath);
				else
					writer.WriteAttributeString("binpathtype", config.BinPathType.ToString());

				if (config.RuntimeFramework != null)
					writer.WriteAttributeString("runtimeFramework", config.RuntimeFramework.ToString());

				foreach (string assembly in config.Assemblies)
				{
					writer.WriteStartElement("assembly");
					writer.WriteAttributeString("path", PathUtils.RelativePath(config.BasePath, assembly));
					writer.WriteEndElement();
				}

				writer.WriteEndElement();
			}

			writer.WriteEndElement();

			writer.Close();
			this.IsDirty = false;

			// Once we save a project, it's no longer
			// loaded as an assembly wrapper on reload.
			this.isAssemblyWrapper = false;
		}

		public void Save(string projectPath)
		{
			this.ProjectPath = projectPath;
			this.Save();
		}

		public void SetActiveConfig(int index)
		{
			this.activeConfig = this.configs[index];
			this.HasChangesRequiringReload = this.IsDirty = true;
		}

		public void SetActiveConfig(string name)
		{
			foreach (ProjectConfig config in this.configs)
			{
				if (config.Name == name)
				{
					this.activeConfig = config;
					this.HasChangesRequiringReload = this.IsDirty = true;
					break;
				}
			}
		}

		#endregion
	}
}