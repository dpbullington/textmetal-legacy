// ****************************************************************
// Copyright 2011, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

using NUnit.Core;
using NUnit.Util;

namespace NUnit.Gui
{
	/// <summary>
	/// 	NUnitPresenter does all file opening and closing that
	/// 	involves interacting with the user.
	/// 
	/// 	NOTE: This class originated as the static class
	/// 	TestLoaderUI and is slowly being converted to a
	/// 	true presenter. Current limitations include:
	/// 
	/// 	1. At this time, the presenter is created by
	/// 	the form and interacts with it directly, rather
	/// 	than through an interface. 
	/// 
	/// 	2. Many functions, which should properly be in
	/// 	the presenter, remain in the form.
	/// 
	/// 	3. The presenter creates dialogs itself, which
	/// 	limits testability.
	/// </summary>
	public class NUnitPresenter
	{
		#region Constructors/Destructors

		public NUnitPresenter(NUnitForm form, TestLoader loader)
		{
			this.form = form;
			this.loader = loader;
		}

		#endregion

		#region Fields/Constants

		private NUnitForm form;
		private TestLoader loader;

		// Our nunit project watcher
		private FileWatcher projectWatcher;

		#endregion

		#region Properties/Indexers/Events

		private static bool VisualStudioSupport
		{
			get
			{
				return Services.UserSettings.GetSetting("Options.TestLoader.VisualStudioSupport", false);
			}
		}

		// TODO: Use an interface for view and model

		public NUnitForm Form
		{
			get
			{
				return this.form;
			}
		}

		#endregion

		#region Methods/Operators

		private static bool CanWriteProjectFile(string path)
		{
			return !File.Exists(path) ||
			       (File.GetAttributes(path) & FileAttributes.ReadOnly) == 0;
		}

		private static string GetProjectEditorPath()
		{
			string editorPath = (string)Services.UserSettings.GetSetting("Options.ProjectEditor.EditorPath");
			if (editorPath == null)
				editorPath = Path.Combine(NUnitConfiguration.NUnitBinDirectory, "nunit-editor.exe");

			return editorPath;
		}

		private static string Quoted(string s)
		{
			return "\"" + s + "\"";
		}

		public void AddAssembly()
		{
			this.AddAssembly(null);
		}

		public void AddAssembly(string configName)
		{
			ProjectConfig config = configName == null
				                       ? this.loader.TestProject.ActiveConfig
				                       : this.loader.TestProject.Configs[configName];

			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Title = "Add Assembly";
			dlg.InitialDirectory = config.BasePath;
			dlg.Filter = "Assemblies (*.dll,*.exe)|*.dll;*.exe";
			dlg.FilterIndex = 1;
			dlg.FileName = "";

			if (dlg.ShowDialog(this.Form) == DialogResult.OK)
				config.Assemblies.Add(dlg.FileName);
		}

		public void AddToProject()
		{
			this.AddToProject(null);
		}

		// TODO: Not used?
		public void AddToProject(string configName)
		{
			ProjectConfig config = configName == null
				                       ? this.loader.TestProject.ActiveConfig
				                       : this.loader.TestProject.Configs[configName];

			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Title = "Add Assemblies To Project";
			dlg.InitialDirectory = config.BasePath;

			if (VisualStudioSupport)
			{
				dlg.Filter =
					"Projects & Assemblies(*.csproj,*.vbproj,*.vjsproj, *.vcproj,*.dll,*.exe )|*.csproj;*.vjsproj;*.vbproj;*.vcproj;*.dll;*.exe|" +
					"Visual Studio Projects (*.csproj,*.vjsproj,*.vbproj,*.vcproj)|*.csproj;*.vjsproj;*.vbproj;*.vcproj|" +
					"C# Projects (*.csproj)|*.csproj|" +
					"J# Projects (*.vjsproj)|*.vjsproj|" +
					"VB Projects (*.vbproj)|*.vbproj|" +
					"C++ Projects (*.vcproj)|*.vcproj|" +
					"Assemblies (*.dll,*.exe)|*.dll;*.exe";
			}
			else
				dlg.Filter = "Assemblies (*.dll,*.exe)|*.dll;*.exe";

			dlg.FilterIndex = 1;
			dlg.FileName = "";

			if (dlg.ShowDialog(this.Form) != DialogResult.OK)
				return;

			if (PathUtils.IsAssemblyFileType(dlg.FileName))
			{
				config.Assemblies.Add(dlg.FileName);
				return;
			}
			else if (VSProject.IsProjectFile(dlg.FileName))
			{
				try
				{
					VSProject vsProject = new VSProject(dlg.FileName);
					MessageBoxButtons buttons;
					string msg;

					if (configName != null && vsProject.Configs.Contains(configName))
					{
						msg = "The project being added may contain multiple configurations;\r\r" +
						      "Select\tYes to add all configurations found.\r" +
						      "\tNo to add only the " + configName + " configuration.\r" +
						      "\tCancel to exit without modifying the project.";
						buttons = MessageBoxButtons.YesNoCancel;
					}
					else
					{
						msg = "The project being added may contain multiple configurations;\r\r" +
						      "Select\tOK to add all configurations found.\r" +
						      "\tCancel to exit without modifying the project.";
						buttons = MessageBoxButtons.OKCancel;
					}

					DialogResult result = this.Form.MessageDisplay.Ask(msg, buttons);
					if (result == DialogResult.Yes || result == DialogResult.OK)
					{
						this.loader.TestProject.Add(vsProject);
						return;
					}
					else if (result == DialogResult.No)
					{
						foreach (string assembly in vsProject.Configs[configName].Assemblies)
							config.Assemblies.Add(assembly);
						return;
					}
				}
				catch (Exception ex)
				{
					this.Form.MessageDisplay.Error("Invalid VS Project", ex);
				}
			}
		}

		public void AddVSProject()
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Title = "Add Visual Studio Project";

			dlg.Filter =
				"All Project Types (*.csproj,*.vjsproj,*.vbproj,*.vcproj)|*.csproj;*.vjsproj;*.vbproj;*.vcproj|" +
				"C# Projects (*.csproj)|*.csproj|" +
				"J# Projects (*.vjsproj)|*.vjsproj|" +
				"VB Projects (*.vbproj)|*.vbproj|" +
				"C++ Projects (*.vcproj)|*.vcproj|" +
				"All Files (*.*)|*.*";

			dlg.FilterIndex = 1;
			dlg.FileName = "";

			if (dlg.ShowDialog(this.Form) == DialogResult.OK)
			{
				try
				{
					VSProject vsProject = new VSProject(dlg.FileName);
					this.loader.TestProject.Add(vsProject);
				}
				catch (Exception ex)
				{
					this.Form.MessageDisplay.Error("Invalid VS Project", ex);
				}
			}
		}

		public DialogResult CloseProject()
		{
			DialogResult result = this.SaveProjectIfDirty();

			if (result != DialogResult.Cancel)
				this.loader.UnloadProject();

			return result;
		}

		public void EditProject()
		{
			NUnitProject project = this.loader.TestProject;

			string editorPath = GetProjectEditorPath();
			if (!File.Exists(editorPath))
			{
				string NL = Environment.NewLine;
				string message =
					"Unable to locate the specified Project Editor:" + NL + NL + editorPath + NL + NL +
					(Services.UserSettings.GetSetting("Options.ProjectEditor.EditorPath") == null
						 ? "Verify that nunit.editor.exe is properly installed in the NUnit bin directory."
						 : "Verify that you have set the path to the editor correctly.");

				this.Form.MessageDisplay.Error(message);

				return;
			}

			if (!NUnitProject.IsNUnitProjectFile(project.ProjectPath))
			{
				if (this.Form.MessageDisplay.Display(
					"The project has not yet been saved. In order to edit the project, it must first be saved. Click OK to save the project or Cancel to exit.",
					MessageBoxButtons.OKCancel) == DialogResult.OK)
					project.Save();
			}
			else if (!File.Exists(project.ProjectPath))
				project.Save();
			else if (project.IsDirty)
			{
				switch (this.Form.MessageDisplay.Ask(
					"There are unsaved changes. Do you want to save them before running the editor?",
					MessageBoxButtons.YesNoCancel))
				{
					case DialogResult.Yes:
						project.Save();
						break;

					case DialogResult.Cancel:
						return;
				}
			}

			// In case we tried to save project and failed
			if (NUnitProject.IsNUnitProjectFile(project.ProjectPath) && File.Exists(project.ProjectPath))
			{
				Process p = new Process();

				p.StartInfo.FileName = Quoted(editorPath);
				p.StartInfo.Arguments = Quoted(project.ProjectPath);
				p.Start();
			}
		}

		public void NewProject()
		{
			if (this.loader.IsProjectLoaded)
				this.CloseProject();

			SaveFileDialog dlg = new SaveFileDialog();
			dlg.Title = "New Test Project";
			dlg.Filter = "NUnit Test Project (*.nunit)|*.nunit|All Files (*.*)|*.*";
			dlg.FileName = Services.ProjectService.GenerateProjectName();
			dlg.DefaultExt = "nunit";
			dlg.ValidateNames = true;
			dlg.OverwritePrompt = true;

			if (dlg.ShowDialog(this.Form) == DialogResult.OK)
				this.loader.NewProject(dlg.FileName);
		}

		private void OnTestProjectChanged(string filePath)
		{
			string message = filePath + Environment.NewLine + Environment.NewLine +
			                 "This file has been modified outside of NUnit." + Environment.NewLine +
			                 "Do you want to reload it?";

			if (this.Form.MessageDisplay.Ask(message) == DialogResult.Yes)
				this.ReloadProject();
		}

		public void OpenProject()
		{
			OpenFileDialog dlg = new OpenFileDialog();
			ISite site = this.Form == null ? null : this.Form.Site;
			if (site != null)
				dlg.Site = site;
			dlg.Title = "Open Project";

			if (VisualStudioSupport)
			{
				dlg.Filter =
					"Projects & Assemblies(*.nunit,*.csproj,*.vbproj,*.vjsproj, *.vcproj,*.sln,*.dll,*.exe )|*.nunit;*.csproj;*.vjsproj;*.vbproj;*.vcproj;*.sln;*.dll;*.exe|" +
					"All Project Types (*.nunit,*.csproj,*.vbproj,*.vjsproj,*.vcproj,*.sln)|*.nunit;*.csproj;*.vjsproj;*.vbproj;*.vcproj;*.sln|" +
					"Test Projects (*.nunit)|*.nunit|" +
					"Solutions (*.sln)|*.sln|" +
					"C# Projects (*.csproj)|*.csproj|" +
					"J# Projects (*.vjsproj)|*.vjsproj|" +
					"VB Projects (*.vbproj)|*.vbproj|" +
					"C++ Projects (*.vcproj)|*.vcproj|" +
					"Assemblies (*.dll,*.exe)|*.dll;*.exe";
			}
			else
			{
				dlg.Filter =
					"Projects & Assemblies(*.nunit,*.dll,*.exe)|*.nunit;*.dll;*.exe|" +
					"Test Projects (*.nunit)|*.nunit|" +
					"Assemblies (*.dll,*.exe)|*.dll;*.exe";
			}

			dlg.FilterIndex = 1;
			dlg.FileName = "";

			if (dlg.ShowDialog(this.Form) == DialogResult.OK)
				this.OpenProject(dlg.FileName);
		}

		public void OpenProject(string testFileName, string configName, string testName)
		{
			if (this.loader.IsProjectLoaded && this.SaveProjectIfDirty() == DialogResult.Cancel)
				return;

			this.loader.LoadProject(testFileName, configName);
			if (this.loader.IsProjectLoaded)
			{
				NUnitProject testProject = this.loader.TestProject;
				if (testProject.Configs.Count == 0)
					this.Form.MessageDisplay.Info("Loaded project contains no configuration data");
				else if (testProject.ActiveConfig == null)
					this.Form.MessageDisplay.Info("Loaded project has no active configuration");
				else if (testProject.ActiveConfig.Assemblies.Count == 0)
					this.Form.MessageDisplay.Info("Active configuration contains no assemblies");
				else
					this.loader.LoadTest(testName);
			}
		}

		public void OpenProject(string testFileName)
		{
			this.OpenProject(testFileName, null, null);
		}

		public void ReloadProject()
		{
			NUnitProject project = this.loader.TestProject;

			bool wrapper = project.IsAssemblyWrapper;
			string projectPath = project.ProjectPath;
			string activeConfigName = project.ActiveConfigName;

			// Unload first to avoid message asking about saving
			this.loader.UnloadProject();

			if (wrapper)
				this.OpenProject(projectPath);
			else
				this.OpenProject(projectPath, activeConfigName, null);
		}

		public void RemoveWatcher()
		{
			if (this.projectWatcher != null)
			{
				this.projectWatcher.Stop();
				this.projectWatcher.Dispose();
				this.projectWatcher = null;
			}
		}

		public void SaveLastResult()
		{
			//TODO: Save all results
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.Title = "Save Test Results as XML";
			dlg.Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*";
			dlg.FileName = "TestResult.xml";
			dlg.InitialDirectory = Path.GetDirectoryName(this.loader.TestFileName);
			dlg.DefaultExt = "xml";
			dlg.ValidateNames = true;
			dlg.OverwritePrompt = true;

			if (dlg.ShowDialog(this.Form) == DialogResult.OK)
			{
				try
				{
					string fileName = dlg.FileName;

					this.loader.SaveLastResult(fileName);

					this.Form.MessageDisplay.Info(String.Format("Results saved as {0}", fileName));
				}
				catch (Exception exception)
				{
					this.Form.MessageDisplay.Error("Unable to Save Results", exception);
				}
			}
		}

//		public static void OpenResults( Form owner )
//		{
//			OpenFileDialog dlg = new OpenFileDialog();
//			System.ComponentModel.ISite site = owner == null ? null : owner.Site;
//			if ( site != null ) dlg.Site = site;
//			dlg.Title = "Open Test Results";
//
//			dlg.Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*";
//			dlg.FilterIndex = 1;
//			dlg.FileName = "";
//
//			if ( dlg.ShowDialog( owner ) == DialogResult.OK ) 
//				OpenProject( owner, dlg.FileName );
		//		}

		public void SaveProject()
		{
			if (Path.IsPathRooted(this.loader.TestProject.ProjectPath) &&
			    NUnitProject.IsNUnitProjectFile(this.loader.TestProject.ProjectPath) &&
			    CanWriteProjectFile(this.loader.TestProject.ProjectPath))
				this.loader.TestProject.Save();
			else
				this.SaveProjectAs();
		}

		public void SaveProjectAs()
		{
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.Title = "Save Test Project";
			dlg.Filter = "NUnit Test Project (*.nunit)|*.nunit|All Files (*.*)|*.*";
			string path = NUnitProject.ProjectPathFromFile(this.loader.TestProject.ProjectPath);
			if (CanWriteProjectFile(path))
				dlg.FileName = path;
			dlg.DefaultExt = "nunit";
			dlg.ValidateNames = true;
			dlg.OverwritePrompt = true;

			while (dlg.ShowDialog(this.Form) == DialogResult.OK)
			{
				if (!CanWriteProjectFile(dlg.FileName))
					this.Form.MessageDisplay.Info(string.Format("File {0} is write-protected. Select another file name.", dlg.FileName));
				else
				{
					this.loader.TestProject.Save(dlg.FileName);
					this.ReloadProject();
					return;
				}
			}
		}

		private DialogResult SaveProjectIfDirty()
		{
			DialogResult result = DialogResult.OK;
			NUnitProject project = this.loader.TestProject;

			if (project.IsDirty)
			{
				string msg = string.Format(
					"Project {0} has been changed. Do you want to save changes?", project.Name);

				result = this.Form.MessageDisplay.Ask(msg, MessageBoxButtons.YesNoCancel);
				if (result == DialogResult.Yes)
					this.SaveProject();
			}

			return result;
		}

		public void WatchProject(string projectPath)
		{
			this.projectWatcher = new FileWatcher(projectPath, 100);

			this.projectWatcher.Changed += new FileChangedHandler(this.OnTestProjectChanged);
			this.projectWatcher.Start();
		}

		#endregion
	}
}