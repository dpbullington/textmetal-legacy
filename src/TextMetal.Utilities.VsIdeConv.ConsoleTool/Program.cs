/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using TextMetal.Common.Core;
using TextMetal.Common.WinForms;
using TextMetal.Utilities.VsIdeConv.ConsoleTool.FileHandlers;
using TextMetal.Utilities.VsIdeConv.ConsoleTool.Utilities;

namespace TextMetal.Utilities.VsIdeConv.ConsoleTool
{
	/// <summary>
	/// Entry point class for the application.
	/// </summary>
	internal class Program : ConsoleApplication
	{
		#region Constructors/Destructors

		public Program()
		{
		}

		#endregion

		#region Fields/Constants

		private const string CMDLN_TOKEN_ROOT_WC = "wcrootdir";
		private readonly Dictionary<string, IFileHandler> registeredFileExtensionHandlers = new Dictionary<string, IFileHandler>(StringComparer.InvariantCultureIgnoreCase);
		private readonly Dictionary<string, IFileHandler> registeredFileNameHandlers = new Dictionary<string, IFileHandler>(StringComparer.InvariantCultureIgnoreCase);

		#endregion

		#region Properties/Indexers/Events

		private Dictionary<string, IFileHandler> RegisteredFileExtensionHandlers
		{
			get
			{
				return this.registeredFileExtensionHandlers;
			}
		}

		private Dictionary<string, IFileHandler> RegisteredFileNameHandlers
		{
			get
			{
				return this.registeredFileNameHandlers;
			}
		}

		#endregion

		#region Methods/Operators

		private void fse_DirectoryFound(DirectoryInfo directoryInfo)
		{
			// do nothing
		}

		private void fse_FileFound(FileInfo fileInfo)
		{
			IFileHandler fileHandler;

			FsClearRoFileHandler.Instance.Execute(fileInfo);

			if (this.RegisteredFileNameHandlers.ContainsKey(fileInfo.Name))
				fileHandler = this.RegisteredFileNameHandlers[fileInfo.Name];
			else
			{
				if (this.RegisteredFileExtensionHandlers.ContainsKey(fileInfo.Extension))
					fileHandler = this.RegisteredFileExtensionHandlers[fileInfo.Extension];
				else
					fileHandler = null;
			}

			if ((object)fileHandler != null)
				fileHandler.Execute(fileInfo);
		}

		protected override IDictionary<string, ArgumentSpec> GetArgumentMap()
		{
			IDictionary<string, ArgumentSpec> argumentMap;

			argumentMap = new Dictionary<string, ArgumentSpec>();
			argumentMap.Add(CMDLN_TOKEN_ROOT_WC, new ArgumentSpec<string>(true, true));
			/*argumentMap.Add(CMDLN_TOKEN_SOURCEFILE, new ArgumentSpec<string>(true, true));
			argumentMap.Add(CMDLN_TOKEN_BASEDIR, new ArgumentSpec<string>(true, true));
			argumentMap.Add(CMDLN_TOKEN_SOURCESTRATEGY_AQTN, new ArgumentSpec<string>(true, true));
			argumentMap.Add(CMDLN_TOKEN_STRICT, new ArgumentSpec<bool>(true, true));
			argumentMap.Add(CMDLN_TOKEN_PROPERTY, new ArgumentSpec<string>(false, false));
			argumentMap.Add(CMDLN_DEBUGGER_LAUNCH, new ArgumentSpec<bool>(false, true));*/

			return argumentMap;
		}

		protected override int OnStartup(string[] args, IDictionary<string, IList<object>> arguments)
		{
			string rootWorkingCopyDirectoryPath;
			FileSystemEnumerator fse;

			if ((object)args == null)
				throw new ArgumentNullException("args");

			if ((object)arguments == null)
				throw new ArgumentNullException("arguments");

			rootWorkingCopyDirectoryPath = (string)arguments[CMDLN_TOKEN_ROOT_WC].Single();

			if (ConversionConfig.ConversionSettings.VersionControlBindingAction == VersionControlBindingAction.Remove)
			{
				this.RegisteredFileExtensionHandlers.Add(".vspscc", ChainFileHandler.GetChain(FsClearRoFileHandler.Instance, FsDeleteFileHandler.Instance));
				this.RegisteredFileExtensionHandlers.Add(".vssscc", ChainFileHandler.GetChain(FsClearRoFileHandler.Instance, FsDeleteFileHandler.Instance));
				this.RegisteredFileExtensionHandlers.Add(".vsscc", ChainFileHandler.GetChain(FsClearRoFileHandler.Instance, FsDeleteFileHandler.Instance));
				this.RegisteredFileNameHandlers.Add("MSSCCPRJ.SCC", ChainFileHandler.GetChain(FsClearRoFileHandler.Instance, FsDeleteFileHandler.Instance));
				this.RegisteredFileNameHandlers.Add("VssVer.scc", ChainFileHandler.GetChain(FsClearRoFileHandler.Instance, FsDeleteFileHandler.Instance));
				this.RegisteredFileNameHandlers.Add("VssVer2.scc", ChainFileHandler.GetChain(FsClearRoFileHandler.Instance, FsDeleteFileHandler.Instance));
			}

			this.RegisteredFileExtensionHandlers.Add(".sln", ChainFileHandler.GetChain(FsClearRoFileHandler.Instance, VsSolutionFileHandler.Instance));
			this.RegisteredFileExtensionHandlers.Add(".csproj", ChainFileHandler.GetChain(FsClearRoFileHandler.Instance, MsBuildProjectFileHandler.Instance));
			this.RegisteredFileExtensionHandlers.Add(".vbproj", ChainFileHandler.GetChain(FsClearRoFileHandler.Instance, MsBuildProjectFileHandler.Instance));

			if (DataType.Instance.IsNullOrWhiteSpace(rootWorkingCopyDirectoryPath))
				throw new InvalidOperationException("Invalid root working copy path");

			if (!File.Exists(rootWorkingCopyDirectoryPath) &&
				!Directory.Exists(rootWorkingCopyDirectoryPath))
				throw new InvalidOperationException("Root working copy path directory path does not exist");

			fse = new FileSystemEnumerator();
			fse.FileFound += this.fse_FileFound;
			fse.DirectoryFound += this.fse_DirectoryFound;

			fse.EnumerateFileSystem(rootWorkingCopyDirectoryPath);

			fse.FileFound -= this.fse_FileFound;
			fse.DirectoryFound -= this.fse_DirectoryFound;

			return 0;
		}

		/// <summary>
		/// The entry point method for this application.
		/// </summary>
		/// <param name="args"> The command line arguments passed from the executing environment. </param>
		/// <returns> The resulting exit code. </returns>
		[STAThread]
		public static int Main(string[] args)
		{
			using (Program program = new Program())
				return program.EntryPoint(args);
		}

		#endregion
	}
}