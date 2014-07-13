/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

using TextMetal.Common.Core;
using TextMetal.HostImpl.Tool;

namespace TextMetal.HostImpl.ConsoleTool
{
	/// <summary>
	/// Entry point class for the application.
	/// </summary>
	internal class Program
	{
		#region Methods/Operators

		/// <summary>
		/// When called, displays an interactive folder browser dialog to prompt for a directory path.
		/// </summary>
		/// <param name="directoryPath"> The resulting directory path or null if the user canceled. </param>
		/// <returns> A value indicating whether the user canceled the dialog. </returns>
		private static bool GetDirectoryPathInteractive(out string directoryPath)
		{
			directoryPath = null;

			using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
			{
				folderBrowserDialog.ShowNewFolderButton = true;

				if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
				{
					directoryPath = folderBrowserDialog.SelectedPath;

					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// When called, displays an interactive open file dialog to prompt for a file path.
		/// </summary>
		/// <param name="filePath"> The resulting file path or null if the user canceled. </param>
		/// <returns> A value indicating whether the user canceled the dialog. </returns>
		private static bool GetFilePathInteractive(out string filePath)
		{
			filePath = null;

			using (OpenFileDialog openFileDialog = new OpenFileDialog())
			{
				openFileDialog.Multiselect = false;
				openFileDialog.RestoreDirectory = true;
				openFileDialog.Title = "Choose File...";

				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					filePath = openFileDialog.FileName;

					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// The entry point method for this application.
		/// </summary>
		/// <param name="args"> The command line arguments passed from the executing environment. </param>
		/// <returns> The resulting exit code. </returns>
		[STAThread]
		private static int Main(string[] args)
		{
#if !DEBUG
			return TryStartup(args);
#else
			return Startup(args);
#endif
		}

		/// <summary>
		/// The indirect entry point method for this application. Code is wrapped in this method to leverage the 'TryStartup'/'Startup' pattern. This method contains the TextMetal console application host environment setup code (logic that is specific to a console application to transition to the more generic 'tool' host code).
		/// </summary>
		/// <param name="args"> The command line arguments passed from the executing environment. </param>
		/// <returns> The resulting exit code. </returns>
		private static int Startup(string[] args)
		{
			Dictionary<string, object> argz;
			IDictionary<string, IList<string>> arguments;
			string templateFilePath;
			string sourceFilePath;
			string baseDirectoryPath;
			string sourceStrategyAqtn;
			bool strictMatching;
			bool debuggerLaunch = false;
			IDictionary<string, IList<string>> properties;
			IList<string> _arguments;
			IList<string> propertyValues;
			bool hasProperties;

			const string CMDLN_TOKEN_TEMPLATEFILE = "templatefile";
			const string CMDLN_TOKEN_SOURCEFILE = "sourcefile";
			const string CMDLN_TOKEN_BASEDIR = "basedir";
			const string CMDLN_TOKEN_SOURCESTRATEGY_AQTN = "sourcestrategy";
			const string CMDLN_TOKEN_STRICT = "strict";
			const string CMDLN_TOKEN_PROPERTY = "property";
			const string CMDLN_DEBUGGER_LAUNCH = "debug";

			arguments = AppConfig.ParseCommandLineArguments(args);

			if ((object)arguments == null ||
				!arguments.ContainsKey(CMDLN_TOKEN_TEMPLATEFILE) ||
				!arguments.ContainsKey(CMDLN_TOKEN_SOURCEFILE) ||
				!arguments.ContainsKey(CMDLN_TOKEN_BASEDIR) ||
				!arguments.ContainsKey(CMDLN_TOKEN_SOURCESTRATEGY_AQTN) ||
				!arguments.ContainsKey(CMDLN_TOKEN_STRICT))
			{
				Console.WriteLine("USAGE: textmetal.exe\r\n\t-{0}:\"<filepath>|?\"\r\n\t-{1}:\"<filepath>|?\"\r\n\t-{2}:\"<directorypath>|?\"\r\n\t-{3}:\"<asmqualtypename>\"\r\n\t-{4}:\"true|false\"",
					CMDLN_TOKEN_TEMPLATEFILE,
					CMDLN_TOKEN_SOURCEFILE,
					CMDLN_TOKEN_BASEDIR,
					CMDLN_TOKEN_SOURCESTRATEGY_AQTN,
					CMDLN_TOKEN_STRICT);

				return -1;
			}

			if (arguments.ContainsKey(CMDLN_DEBUGGER_LAUNCH))
				DataType.TryParse<bool>(arguments[CMDLN_DEBUGGER_LAUNCH].Single(), out debuggerLaunch);

			if (debuggerLaunch)
				Console.WriteLine("Debugger launch result: '{0}'", Debugger.Launch() && Debugger.IsAttached);

			// required
			properties = new Dictionary<string, IList<string>>();

			templateFilePath = arguments[CMDLN_TOKEN_TEMPLATEFILE].Single();
			sourceFilePath = arguments[CMDLN_TOKEN_SOURCEFILE].Single();
			baseDirectoryPath = arguments[CMDLN_TOKEN_BASEDIR].Single();
			sourceStrategyAqtn = arguments[CMDLN_TOKEN_SOURCESTRATEGY_AQTN].Single();
			DataType.TryParse<bool>(arguments[CMDLN_TOKEN_STRICT].Single(), out strictMatching);
			hasProperties = arguments.TryGetValue(CMDLN_TOKEN_PROPERTY, out _arguments);

			argz = new Dictionary<string, object>();
			argz.Add(CMDLN_TOKEN_TEMPLATEFILE, templateFilePath);
			argz.Add(CMDLN_TOKEN_SOURCEFILE, sourceFilePath);
			argz.Add(CMDLN_TOKEN_BASEDIR, baseDirectoryPath);
			argz.Add(CMDLN_TOKEN_SOURCESTRATEGY_AQTN, sourceStrategyAqtn);
			argz.Add(CMDLN_TOKEN_STRICT, strictMatching);
			argz.Add(CMDLN_DEBUGGER_LAUNCH, arguments.ContainsKey(CMDLN_DEBUGGER_LAUNCH) ? (object)debuggerLaunch : null);
			argz.Add(CMDLN_TOKEN_PROPERTY, hasProperties ? (object)_arguments : null);

			if (hasProperties)
			{
				if ((object)_arguments != null)
				{
					foreach (string argument in _arguments)
					{
						string key, value;

						if (!AppConfig.TryParseCommandLineArgumentProperty(argument, out key, out value))
							continue;

						if (!properties.ContainsKey(key))
							properties.Add(key, propertyValues = new List<string>());
						else
							propertyValues = properties[key];

						// duplicate values are ignored
						if (propertyValues.Contains(value))
							continue;

						propertyValues.Add(value);
					}
				}
			}

			if (templateFilePath == "?")
			{
				if (!GetFilePathInteractive(out templateFilePath))
					return 0;
			}

			if (sourceFilePath == "?")
			{
				if (!GetFilePathInteractive(out sourceFilePath))
					return 0;
			}

			if (baseDirectoryPath == "?")
			{
				if (!GetDirectoryPathInteractive(out baseDirectoryPath))
					return 0;
			}

			using(IToolHost toolHost = new ToolHost())
				toolHost.Host((object)args != null ? args.Length : -1, args, argz, templateFilePath, sourceFilePath, baseDirectoryPath, sourceStrategyAqtn, strictMatching, properties);

			return 0;
		}

		/// <summary>
		/// The indirect entry point method for this application. Code is wrapped in this method to leverage the 'TryStartup'/'Startup' pattern. This method, if used, wraps the Startup() method in an exception handler. The handler will catch all exceptions and report a full detailed stack trace to the Console.Error stream; -1 is then returned as the exit code. Otherwise, if no exception is thrown, the exit code returned is that which is returned by Startup().
		/// </summary>
		/// <param name="args"> The command line arguments passed from the executing environment. </param>
		/// <returns> The resulting exit code. </returns>
		private static int TryStartup(string[] args)
		{
			try
			{
				return Startup(args);
			}
			catch (Exception ex)
			{
				string message;

				message = Reflexion.GetErrors(ex, 0);

				Console.Error.WriteLine(message);
				Console.WriteLine("The operation failed to complete.");
			}

			return -1;
		}

		#endregion
	}
}