// ****************************************************************
// Copyright 2002-2003, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.IO;
using System.Text;
using System.Xml;

namespace NUnit.Util
{
	/// <summary>
	/// 	This class allows loading information about
	/// 	configurations and assemblies in a Visual
	/// 	Studio project file and inspecting them.
	/// 	Only the most common project types are
	/// 	supported and an exception is thrown if
	/// 	an attempt is made to load an invalid
	/// 	file or one of an unknown type.
	/// </summary>
	public class VSProject
	{
		#region Constructors/Destructors

		public VSProject(string projectPath)
		{
			this.projectPath = Path.GetFullPath(projectPath);
			this.configs = new VSProjectConfigCollection();

			this.Load();
		}

		#endregion

		#region Fields/Constants

		/// <summary>
		/// 	VS Solution extension
		/// </summary>
		private static readonly string solutionExtension = ".sln";

		/// <summary>
		/// 	VS Project extentions
		/// </summary>
		private static readonly string[] validExtensions = { ".csproj", ".vbproj", ".vjsproj", ".vcproj" };

		/// <summary>
		/// 	Collection of configs for the project
		/// </summary>
		private VSProjectConfigCollection configs;

		/// <summary>
		/// 	Path to the file storing this project
		/// </summary>
		private string projectPath;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// 	Our collection of configurations
		/// </summary>
		public VSProjectConfigCollection Configs
		{
			get
			{
				return this.configs;
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

		/// <summary>
		/// 	The path to the project
		/// </summary>
		public string ProjectPath
		{
			get
			{
				return this.projectPath;
			}
		}

		#endregion

		#region Methods/Operators

		public static bool IsProjectFile(string path)
		{
#if CLR_2_0 || CLR_4_0
			if (path.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
#else
			if ( path.IndexOfAny( Path.InvalidPathChars ) >= 0 )
#endif
				return false;

			if (path.ToLower().IndexOf("http:") >= 0)
				return false;

			string extension = Path.GetExtension(path);

			foreach (string validExtension in validExtensions)
			{
				if (extension == validExtension)
					return true;
			}

			return false;
		}

		public static bool IsSolutionFile(string path)
		{
			return Path.GetExtension(path) == solutionExtension;
		}

		private void Load()
		{
			if (!IsProjectFile(this.projectPath))
				this.ThrowInvalidFileType(this.projectPath);

			string projectDirectory = Path.GetFullPath(Path.GetDirectoryName(this.projectPath));
			StreamReader rdr = new StreamReader(this.projectPath, Encoding.UTF8);
			string[] extensions = { "", ".exe", ".dll", ".lib", "" };

			try
			{
				XmlDocument doc = new XmlDocument();
				doc.Load(rdr);

				string extension = Path.GetExtension(this.projectPath);
				string assemblyName = null;

				switch (extension)
				{
					case ".vcproj":

						// TODO: This is all very hacked up... replace it.
						foreach (XmlNode configNode in doc.SelectNodes("/VisualStudioProject/Configurations/Configuration"))
						{
							string name = this.RequiredAttributeValue(configNode, "Name");
							int config_type = Convert.ToInt32(this.RequiredAttributeValue(configNode, "ConfigurationType"));
							string dirName = name;
							int bar = dirName.IndexOf('|');
							if (bar >= 0)
								dirName = dirName.Substring(0, bar);
							string outputPath = this.RequiredAttributeValue(configNode, "OutputDirectory");
							outputPath = outputPath.Replace("$(SolutionDir)", Path.GetFullPath(Path.GetDirectoryName(this.projectPath)) + Path.DirectorySeparatorChar);
							outputPath = outputPath.Replace("$(ConfigurationName)", dirName);

							string outputDirectory = Path.Combine(projectDirectory, outputPath);
							XmlNode toolNode = configNode.SelectSingleNode("Tool[@Name='VCLinkerTool']");
							if (toolNode != null)
							{
								assemblyName = this.SafeAttributeValue(toolNode, "OutputFile");
								if (assemblyName != null)
									assemblyName = Path.GetFileName(assemblyName);
								else
									assemblyName = Path.GetFileNameWithoutExtension(this.projectPath) + extensions[config_type];
							}
							else
							{
								toolNode = configNode.SelectSingleNode("Tool[@Name='VCNMakeTool']");
								if (toolNode != null)
									assemblyName = Path.GetFileName(this.RequiredAttributeValue(toolNode, "Output"));
							}

							assemblyName = assemblyName.Replace("$(OutDir)", outputPath);
							assemblyName = assemblyName.Replace("$(ProjectName)", this.Name);

							VSProjectConfig config = new VSProjectConfig(name);
							if (assemblyName != null)
								config.Assemblies.Add(Path.Combine(outputDirectory, assemblyName));

							this.configs.Add(config);
						}

						break;

					case ".csproj":
					case ".vbproj":
					case ".vjsproj":
						this.LoadProject(projectDirectory, doc);
						break;

					default:
						break;
				}
			}
			catch (FileNotFoundException)
			{
				throw;
			}
			catch (Exception e)
			{
				this.ThrowInvalidFormat(this.projectPath, e);
			}
			finally
			{
				rdr.Close();
			}
		}

		private bool LoadMSBuildProject(string projectDirectory, XmlDocument doc)
		{
			XmlNamespaceManager namespaceManager = new XmlNamespaceManager(doc.NameTable);
			namespaceManager.AddNamespace("msbuild", "http://schemas.microsoft.com/developer/msbuild/2003");

			XmlNodeList nodes = doc.SelectNodes("/msbuild:Project/msbuild:PropertyGroup", namespaceManager);
			if (nodes == null)
				return false;

			XmlElement assemblyNameElement = (XmlElement)doc.SelectSingleNode("/msbuild:Project/msbuild:PropertyGroup/msbuild:AssemblyName", namespaceManager);
			string assemblyName = assemblyNameElement.InnerText;

			XmlElement outputTypeElement = (XmlElement)doc.SelectSingleNode("/msbuild:Project/msbuild:PropertyGroup/msbuild:OutputType", namespaceManager);
			string outputType = outputTypeElement.InnerText;

			if (outputType == "Exe" || outputType == "WinExe")
				assemblyName = assemblyName + ".exe";
			else
				assemblyName = assemblyName + ".dll";

			string commonOutputPath = null;

			foreach (XmlElement configNode in nodes)
			{
				if (configNode.Name != "PropertyGroup")
					continue;

				string configurationName = null;
				XmlAttribute conditionAttribute = configNode.Attributes["Condition"];
				if (conditionAttribute != null)
				{
					string condition = conditionAttribute.Value;
					if (condition.IndexOf("$(Configuration)") >= 0)
					{
						int start = condition.IndexOf("==");
						if (start >= 0)
						{
							configurationName = condition.Substring(start + 2).Trim(new char[] { ' ', '\'' });
							if (configurationName.EndsWith("|AnyCPU"))
								configurationName = configurationName.Substring(0, configurationName.Length - 7);
						}
					}
				}

				XmlElement outputPathElement = (XmlElement)configNode.SelectSingleNode("msbuild:OutputPath", namespaceManager);
				string outputPath = null;
				if (outputPathElement != null)
					outputPath = outputPathElement.InnerText;

				if (configurationName == null)
				{
					commonOutputPath = outputPath;
					continue;
				}

				if (outputPath == null)
					outputPath = commonOutputPath;

				if (outputPath == null)
					continue;

				string outputDirectory = Path.Combine(projectDirectory, outputPath);
				string assemblyPath = Path.Combine(outputDirectory, assemblyName);

				VSProjectConfig config = new VSProjectConfig(configurationName);
				config.Assemblies.Add(assemblyPath);

				this.configs.Add(config);
			}

			return true;
		}

		private bool LoadProject(string projectDirectory, XmlDocument doc)
		{
			bool loaded = this.LoadVS2003Project(projectDirectory, doc);
			if (loaded)
				return true;

			loaded = this.LoadMSBuildProject(projectDirectory, doc);
			if (loaded)
				return true;

			return false;
		}

		private bool LoadVS2003Project(string projectDirectory, XmlDocument doc)
		{
			XmlNode settingsNode = doc.SelectSingleNode("/VisualStudioProject/*/Build/Settings");
			if (settingsNode == null)
				return false;

			string assemblyName = this.RequiredAttributeValue(settingsNode, "AssemblyName");
			string outputType = this.RequiredAttributeValue(settingsNode, "OutputType");

			if (outputType == "Exe" || outputType == "WinExe")
				assemblyName = assemblyName + ".exe";
			else
				assemblyName = assemblyName + ".dll";

			XmlNodeList nodes = settingsNode.SelectNodes("Config");
			if (nodes != null)
			{
				foreach (XmlNode configNode in nodes)
				{
					string name = this.RequiredAttributeValue(configNode, "Name");
					string outputPath = this.RequiredAttributeValue(configNode, "OutputPath");
					string outputDirectory = Path.Combine(projectDirectory, outputPath);
					string assemblyPath = Path.Combine(outputDirectory, assemblyName);

					VSProjectConfig config = new VSProjectConfig(name);
					config.Assemblies.Add(assemblyPath);

					this.configs.Add(config);
				}
			}

			return true;
		}

		private string RequiredAttributeValue(XmlNode node, string name)
		{
			string result = this.SafeAttributeValue(node, name);
			if (result != null)
				return result;

			throw new ApplicationException("Missing required attribute " + name);
		}

		private string SafeAttributeValue(XmlNode node, string attrName)
		{
			XmlNode attrNode = node.Attributes[attrName];
			return attrNode == null ? null : attrNode.Value;
		}

		private void ThrowInvalidFileType(string projectPath)
		{
			throw new ArgumentException(
				string.Format("Invalid project file type: {0}",
				              Path.GetFileName(projectPath)));
		}

		private void ThrowInvalidFormat(string projectPath, Exception e)
		{
			throw new ArgumentException(
				string.Format("Invalid project file format: {0}",
				              Path.GetFileName(projectPath)), e);
		}

		#endregion
	}
}