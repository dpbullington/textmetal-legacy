/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Tasks.ExecutePackageTask;
using Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask;

using TextMetal.Common.Core;
using TextMetal.HostImpl.SsisGenPkg.ConsoleTool.Config;

using Application = Microsoft.SqlServer.Dts.Runtime.Application;
using Configuration = TextMetal.HostImpl.SsisGenPkg.ConsoleTool.Config.Configuration;

namespace TextMetal.HostImpl.SsisGenPkg.ConsoleTool
{
	internal class Program
	{
		#region Fields/Constants

		private const string SSIS_ARCHIVE_FILE_NAME_FORMAT = "{0}.ispac";
		private const string SSIS_DATABASE_PACKAGE_FILE_NAME_FORMAT = "02.Ox(database)_{0}+{1}.dtsx";
		private const string SSIS_OBJECT_PACKAGE_FILE_NAME_FORMAT = "05.Ox(object)_{0}+{1}+{2}+{3}+{4}.dtsx";
		private const string SSIS_OBJECT_TYPE_PACKAGE_FILE_NAME_FORMAT = "04.Ox(objtyp)_{0}+{1}+{2}+{3}.dtsx";
		private const string SSIS_PROJECT_NAME_FORMAT = "Obfuscation.SSIS";

		private const string SSIS_ROOT_PACKAGE_FILE_NAME = "00.Ox(root).dtsx";
		private const string SSIS_SCHEMA_PACKAGE_FILE_NAME_FORMAT = "03.Ox(schema)_{0}+{1}+{2}.dtsx";
		private const string SSIS_SERVER_PACKAGE_FILE_NAME_FORMAT = "01.Ox(server)_{0}.dtsx";

		private const string TRUNCATE_TABLE_COMMAND_TEXT_FORMAT = "TRUNCATE TABLE {0}";
		private static string CONNECTION_STRING_FORMAT = "Data Source={0};Initial Catalog={1};Provider=SQLNCLI11;Integrated Security=SSPI;";

		#endregion

		#region Properties/Indexers/Events

		private static string SsisArchiveFileName
		{
			get
			{
				string value;

				value = string.Format(SSIS_ARCHIVE_FILE_NAME_FORMAT, SsisProjectName);

				return value;
			}
		}

		private static string SsisProjectName
		{
			get
			{
				string value;

				value = string.Format(SSIS_PROJECT_NAME_FORMAT);

				return value;
			}
		}

		private static string SsisRootPackageName
		{
			get
			{
				string value;

				value = string.Format(SSIS_ROOT_PACKAGE_FILE_NAME);

				return value;
			}
		}

		#endregion

		#region Methods/Operators

		private static void Execute(string sourceFilePath, string baseDirectoryPath)
		{
			Configuration configuration;

			if ((object)sourceFilePath == null)
				throw new ArgumentNullException("sourceFilePath");

			if ((object)baseDirectoryPath == null)
				throw new ArgumentNullException("baseDirectoryPath");

			if (DataType.IsWhiteSpace(sourceFilePath))
				throw new ArgumentOutOfRangeException("sourceFilePath");

			if (DataType.IsWhiteSpace(baseDirectoryPath))
				throw new ArgumentOutOfRangeException("baseDirectoryPath");

			sourceFilePath = Path.GetFullPath(sourceFilePath);
			baseDirectoryPath = Path.GetFullPath(baseDirectoryPath);

			if (!Directory.Exists(baseDirectoryPath))
				Directory.CreateDirectory(baseDirectoryPath);

			/*if (!Directory.Exists(baseDirectoryPath))
				Directory.Delete(baseDirectoryPath, true);

			Directory.CreateDirectory(baseDirectoryPath);*/

			configuration = Configuration.FromJsonFile(sourceFilePath);

			WriteProject(baseDirectoryPath, configuration);
		}

		private static string GetDestinationConnectionString(FourPartName fourPartName)
		{
			string value;

			if ((object)fourPartName == null)
				throw new ArgumentNullException("fourPartName");

			value = string.Format(CONNECTION_STRING_FORMAT, fourPartName.ServerName, fourPartName.DatabaseName);

			return value;
		}

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

		private static string GetSourceConnectionString(FourPartName fourPartName)
		{
			string value;

			if ((object)fourPartName == null)
				throw new ArgumentNullException("fourPartName");

			value = string.Format(CONNECTION_STRING_FORMAT, fourPartName.ServerName, fourPartName.DatabaseName);

			return value;
		}

		private static string GetSsisDatabasePackageName(FourPartName fourPartName)
		{
			string value;

			if ((object)fourPartName == null)
				throw new ArgumentNullException("fourPartName");

			value = string.Format(SSIS_DATABASE_PACKAGE_FILE_NAME_FORMAT, Name.GetPascalCase(fourPartName.ServerName), Name.GetPascalCase(fourPartName.DatabaseName));

			return value;
		}

		private static string GetSsisObjectPackageName(FourPartName fourPartName)
		{
			string value;

			if ((object)fourPartName == null)
				throw new ArgumentNullException("fourPartName");

			value = string.Format(SSIS_OBJECT_PACKAGE_FILE_NAME_FORMAT, Name.GetPascalCase(fourPartName.ServerName), Name.GetPascalCase(fourPartName.DatabaseName), Name.GetPascalCase(fourPartName.SchemaName), Name.GetPascalCase(fourPartName.ObjectType.ToString()), Name.GetPascalCase(fourPartName.ObjectName));

			return value;
		}

		private static string GetSsisObjectTypePackageName(FourPartName fourPartName)
		{
			string value;

			if ((object)fourPartName == null)
				throw new ArgumentNullException("fourPartName");

			value = string.Format(SSIS_OBJECT_TYPE_PACKAGE_FILE_NAME_FORMAT, Name.GetPascalCase(fourPartName.ServerName), Name.GetPascalCase(fourPartName.DatabaseName), Name.GetPascalCase(fourPartName.SchemaName), Name.GetPascalCase(fourPartName.ObjectType.ToString()));

			return value;
		}

		private static string GetSsisSchemaPackageName(FourPartName fourPartName)
		{
			string value;

			if ((object)fourPartName == null)
				throw new ArgumentNullException("fourPartName");

			value = string.Format(SSIS_SCHEMA_PACKAGE_FILE_NAME_FORMAT, Name.GetPascalCase(fourPartName.ServerName), Name.GetPascalCase(fourPartName.DatabaseName), Name.GetPascalCase(fourPartName.SchemaName));

			return value;
		}

		private static string GetSsisServerPackageName(FourPartName fourPartName)
		{
			string value;

			if ((object)fourPartName == null)
				throw new ArgumentNullException("fourPartName");

			value = string.Format(SSIS_SERVER_PACKAGE_FILE_NAME_FORMAT, Name.GetPascalCase(fourPartName.ServerName));

			return value;
		}

		private static string GetTruncateTableCommandText(FourPartName fourPartName)
		{
			string value;

			if ((object)fourPartName == null)
				throw new ArgumentNullException("fourPartName");

			value = string.Format(TRUNCATE_TABLE_COMMAND_TEXT_FORMAT, fourPartName.ToString(false));

			return value;
		}

		public static int Main(string[] args)
		{
			IDictionary<string, IList<string>> arguments;
			IList<string> _arguments;
			string sourceFilePath;
			string baseDirectoryPath;

			const string CMDLN_TOKEN_SOURCEFILE = "sourcefile";
			const string CMDLN_TOKEN_BASEDIR = "basedir";

			arguments = AppConfig.ParseCommandLineArguments(args);

			if ((object)arguments == null ||
				!arguments.ContainsKey(CMDLN_TOKEN_SOURCEFILE) ||
				!arguments.ContainsKey(CMDLN_TOKEN_BASEDIR))
			{
				Console.WriteLine("USAGE: ssisgeneratortool.exe\r\n\t-{0}:\"<filepath>\"\r\n\t-{1}:\"<filepath>\"",
					CMDLN_TOKEN_SOURCEFILE,
					CMDLN_TOKEN_BASEDIR);

				return -1;
			}

			sourceFilePath = arguments[CMDLN_TOKEN_SOURCEFILE].Single();
			baseDirectoryPath = arguments[CMDLN_TOKEN_BASEDIR].Single();

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

			Execute(sourceFilePath, baseDirectoryPath);

			return 0;
		}

		public static void WritePackage(bool validateExternalMetadata, string baseDirectoryPath, Project project, DataTransfer @object
			/*,	ConnectionManager sourceConnectionManager, ConnectionManager destinationConnectionManager*/)
		{
			if ((object)project == null)
				throw new ArgumentNullException("project");

			if ((object)@object == null)
				throw new ArgumentNullException("object");

			/*if ((object)sourceConnectionManager == null)
				throw new ArgumentNullException("sourceConnectionManager");

			if ((object)destinationConnectionManager == null)
				throw new ArgumentNullException("destinationConnectionManager");*/

			Console.WriteLine("*** {0} ***", @object.Source.ToString());

			using (Package package = new Package())
			{
				project.PackageItems.Add(package, GetSsisObjectPackageName(@object.Source));

				ConnectionManager sourceConnectionManager;
				ConnectionManager destinationConnectionManager;

				TaskHost sqlTaskHost;
				ExecuteSQLTask executeSqlTask;

				TaskHost dataFlowTaskHost;
				MainPipe mainPipe;
				IDTSComponentEvents dtsComponentEvents;

				PrecedenceConstraint sqlTask_dataFlowTask_PrecedenceConstraint;

				IDTSComponentMetaData100 oleDbSourceDtsComponentMetaData100;
				IDTSDesigntimeComponent100 oleDbSourceDtsDesigntimeComponent100;
				IDTSComponentMetaData100 transformDtsComponentMetaData100;
				IDTSDesigntimeComponent100 transformDtsDesigntimeComponent100;
				IDTSComponentMetaData100 oleDbDestinationDtsComponentMetaData100;
				IDTSDesigntimeComponent100 oleDbDestinationDtsDesigntimeComponent100;
				
				IDTSPath100 source_transform_DtsPath100;
				IDTSPath100 transform_destination_DtsPath100;
				IDTSPath100 source_destination_DtsPath100;

				IDTSInput100 dtsInput100;
				IDTSVirtualInput100 dtsVirtualInput100;
				IDTSInputColumnCollection100 dtsInputColumnCollection100;
				IDTSExternalMetadataColumnCollection100 dtsExternalMetadataColumnCollection100;
				IDTSOutputColumnCollection100 dtsOutputColumnCollection100;
				IDTSInputColumn100 dtsInputColumn100;
				IDTSExternalMetadataColumn100 dtsExternalMetadataColumn100;

				// create source connection
				sourceConnectionManager = package.Connections.Add("OLEDB");
				sourceConnectionManager.Name = "Source OLEDB Connection Manager";
				sourceConnectionManager.ConnectionString = GetSourceConnectionString(@object.Source);

				// create destination connection
				destinationConnectionManager = package.Connections.Add("OLEDB");
				destinationConnectionManager.Name = "Destination OLEDB Connection Manager";
				destinationConnectionManager.ConnectionString = GetDestinationConnectionString(@object.Destination);

				// create SQL task
				sqlTaskHost = (TaskHost)package.Executables.Add("STOCK:SQLTask");
				sqlTaskHost.Name = "Execute SQL Task";

				// get inner object
				executeSqlTask = (ExecuteSQLTask)sqlTaskHost.InnerObject;
				executeSqlTask.Connection = destinationConnectionManager.ID;
				executeSqlTask.SqlStatementSourceType = SqlStatementSourceType.DirectInput;
				executeSqlTask.SqlStatementSource = GetTruncateTableCommandText(@object.Destination);

				// create data flow task
				dataFlowTaskHost = (TaskHost)package.Executables.Add("STOCK:PipelineTask");
				dataFlowTaskHost.Name = "Data Flow Task";

				// get inner object
				mainPipe = (MainPipe)dataFlowTaskHost.InnerObject;

				// capture COM events
				dtsComponentEvents = new ConsoleComponentEventHandler();
				mainPipe.Events = DtsConvert.GetExtendedInterface(dtsComponentEvents);

				// wire together
				sqlTask_dataFlowTask_PrecedenceConstraint = package.PrecedenceConstraints.Add(sqlTaskHost, dataFlowTaskHost);
				sqlTask_dataFlowTask_PrecedenceConstraint.Value = DTSExecResult.Success;

				// -----------------------------------------------------------
				// meanwhile, inside the data flow task...
				// -----------------------------------------------------------

				// create OLEDB source component metadata in pipeline
				oleDbSourceDtsComponentMetaData100 = mainPipe.ComponentMetaDataCollection.New();
				oleDbSourceDtsComponentMetaData100.ComponentClassID = "DTSAdapter.OleDbSource";
				oleDbSourceDtsComponentMetaData100.ValidateExternalMetadata = validateExternalMetadata;
				oleDbSourceDtsComponentMetaData100.Name = "OLE DB Source";

				// create OLEDB source design-time component in pipeline
				oleDbSourceDtsDesigntimeComponent100 = oleDbSourceDtsComponentMetaData100.Instantiate();
				oleDbSourceDtsDesigntimeComponent100.ProvideComponentProperties();
				oleDbSourceDtsDesigntimeComponent100.SetComponentProperty("AccessMode", 0);
				oleDbSourceDtsDesigntimeComponent100.SetComponentProperty("OpenRowset", @object.Source.ToString(false));

				// set OLEDB source connection manager
				oleDbSourceDtsComponentMetaData100.RuntimeConnectionCollection[0].ConnectionManager = DtsConvert.GetExtendedInterface(sourceConnectionManager);
				oleDbSourceDtsComponentMetaData100.RuntimeConnectionCollection[0].ConnectionManagerID = sourceConnectionManager.ID;

				// get the column metadata
				if (validateExternalMetadata)
				{
					oleDbSourceDtsDesigntimeComponent100.AcquireConnections(null);
					oleDbSourceDtsDesigntimeComponent100.ReinitializeMetaData();
					oleDbSourceDtsDesigntimeComponent100.ReleaseConnections();
				}

				if (false)
				{
					// create transform component metadata in pipeline
					transformDtsComponentMetaData100 = mainPipe.ComponentMetaDataCollection.New();
					transformDtsComponentMetaData100.ComponentClassID = "???";
					transformDtsComponentMetaData100.ValidateExternalMetadata = validateExternalMetadata;
					transformDtsComponentMetaData100.Name = "???";

					// create transform design-time component in pipeline
					transformDtsDesigntimeComponent100 = transformDtsComponentMetaData100.Instantiate();
					transformDtsDesigntimeComponent100.ProvideComponentProperties();
					transformDtsDesigntimeComponent100.SetComponentProperty("???", new object());
					transformDtsDesigntimeComponent100.SetComponentProperty("???", new object());

					// get the column metadata
					if (validateExternalMetadata)
					{
						transformDtsDesigntimeComponent100.AcquireConnections(null);
						transformDtsDesigntimeComponent100.ReinitializeMetaData();
						transformDtsDesigntimeComponent100.ReleaseConnections();
					}
				}
				else
				{
					// do nothing
				}

				// create OLEDB destination component metadata in pipeline
				oleDbDestinationDtsComponentMetaData100 = mainPipe.ComponentMetaDataCollection.New();
				oleDbDestinationDtsComponentMetaData100.ComponentClassID = "DTSAdapter.OleDbDestination";
				oleDbDestinationDtsComponentMetaData100.ValidateExternalMetadata = validateExternalMetadata;
				oleDbDestinationDtsComponentMetaData100.Name = "OLE DB Destination";

				// create OLEDB destination design-time component in pipeline
				oleDbDestinationDtsDesigntimeComponent100 = oleDbDestinationDtsComponentMetaData100.Instantiate();
				oleDbDestinationDtsDesigntimeComponent100.ProvideComponentProperties();
				oleDbDestinationDtsDesigntimeComponent100.SetComponentProperty("AccessMode", 3);
				oleDbDestinationDtsDesigntimeComponent100.SetComponentProperty("FastLoadKeepIdentity", true);
				oleDbDestinationDtsDesigntimeComponent100.SetComponentProperty("FastLoadKeepNulls", true);
				oleDbDestinationDtsDesigntimeComponent100.SetComponentProperty("FastLoadOptions", "TABLOCK,CHECK_CONSTRAINTS,FIRE_TRIGGERS");
				oleDbDestinationDtsDesigntimeComponent100.SetComponentProperty("OpenRowset", @object.Destination.ToString(false));

				// set OLEDB destination connection manager
				oleDbDestinationDtsComponentMetaData100.RuntimeConnectionCollection[0].ConnectionManager = DtsConvert.GetExtendedInterface(destinationConnectionManager);
				oleDbDestinationDtsComponentMetaData100.RuntimeConnectionCollection[0].ConnectionManagerID = destinationConnectionManager.ID;

				// get the column metadata
				if (validateExternalMetadata)
				{
					oleDbDestinationDtsDesigntimeComponent100.AcquireConnections(null);
					oleDbDestinationDtsDesigntimeComponent100.ReinitializeMetaData();
					oleDbDestinationDtsDesigntimeComponent100.ReleaseConnections();
				}

				if (false)
				{
					// connect the dots: OLEDB source -> obfuscation strategy transform
					source_transform_DtsPath100 = mainPipe.PathCollection.New();
					source_transform_DtsPath100.AttachPathAndPropagateNotifications(transformDtsComponentMetaData100.OutputCollection[0], transformDtsComponentMetaData100.InputCollection[0]);

					// connect the dots: obfuscation strategy transform -> OLEDB destination
					transform_destination_DtsPath100 = mainPipe.PathCollection.New();
					transform_destination_DtsPath100.AttachPathAndPropagateNotifications(transformDtsComponentMetaData100.OutputCollection[0], oleDbDestinationDtsComponentMetaData100.InputCollection[0]);
				}
				else
				{
					// connect the dots: OLEDB source -> OLEDB destination
					source_destination_DtsPath100 = mainPipe.PathCollection.New();
					source_destination_DtsPath100.AttachPathAndPropagateNotifications(oleDbSourceDtsComponentMetaData100.OutputCollection[0], oleDbDestinationDtsComponentMetaData100.InputCollection[0]);
				}

				// for the OLEDB destination: requires hooking up the external columns
				dtsInput100 = oleDbDestinationDtsComponentMetaData100.InputCollection[0];
				dtsVirtualInput100 = dtsInput100.GetVirtualInput();
				dtsInputColumnCollection100 = dtsInput100.InputColumnCollection;
				dtsExternalMetadataColumnCollection100 = dtsInput100.ExternalMetadataColumnCollection;
				dtsOutputColumnCollection100 = oleDbSourceDtsComponentMetaData100.OutputCollection[0].OutputColumnCollection;

				foreach (IDTSOutputColumn100 dtsOutputColumn100 in dtsOutputColumnCollection100)
				{
					// [optional place to skip columns here]
					if (@object.ExcludeMemberNames.Any(c => c.SafeToString().Trim().ToLower() ==
															dtsOutputColumn100.Name.SafeToString().Trim().ToLower()))
						continue;

					// get the external column ID
					dtsExternalMetadataColumn100 = dtsExternalMetadataColumnCollection100[dtsOutputColumn100.Name];

					if ((object)dtsExternalMetadataColumn100 != null)
					{
						// create an input column from an output column of previous component.
						dtsVirtualInput100.SetUsageType(dtsOutputColumn100.ID, DTSUsageType.UT_READONLY);
						dtsInputColumn100 = dtsInputColumnCollection100.GetInputColumnByLineageID(dtsOutputColumn100.ID);

						if ((object)dtsInputColumn100 != null)
						{
							// map the input column with an external metadata column
							oleDbDestinationDtsDesigntimeComponent100.MapInputColumn(dtsInput100.ID, dtsInputColumn100.ID, dtsExternalMetadataColumn100.ID);
						}
					}
				}

				WritePackageApplication(baseDirectoryPath, @object.Source, package);
			}
		}

		private static void WritePackageApplication(string baseDirectoryPath, FourPartName fourPartName, Package package)
		{
			Application application;
			IDTSEvents dtsEvents;

			if ((object)package == null)
				throw new ArgumentNullException("package");

			if ((object)fourPartName == null)
				throw new ArgumentNullException("fourPartName");

			dtsEvents = new ConsoleEvents();
			application = new Application();

			application.SaveToXml(Path.Combine(baseDirectoryPath, GetSsisObjectPackageName(fourPartName)), package, dtsEvents);
		}

		private static void WriteParentPackage(string baseDirectoryPath, Project project,
			string parentPackageName, string[] childPackageNames)
		{
			if ((object)project == null)
				throw new ArgumentNullException("project");

			if ((object)parentPackageName == null)
				throw new ArgumentNullException("parentPackageName");

			if ((object)childPackageNames == null)
				throw new ArgumentNullException("childPackageNames");

			Console.WriteLine("*** {0} ***", parentPackageName);

			using (Package package = new Package())
			{
				project.PackageItems.Add(package, parentPackageName);

				TaskHost executePackageTaskHost;
				ExecutePackageTask executePackageTask;
				int index;

				index = 0;
				foreach (string childPackageName in childPackageNames)
				{
					// create SQL task
					executePackageTaskHost = (TaskHost)package.Executables.Add("STOCK:ExecutePackageTask");
					executePackageTaskHost.Name = string.Format("Execute Package Task ({0})", index++);

					// get inner object
					executePackageTask = (ExecutePackageTask)executePackageTaskHost.InnerObject;
					executePackageTask.PackageName = childPackageName;
				}

				WriteParentPackageApplication(baseDirectoryPath, package, parentPackageName);
			}
		}

		private static void WriteParentPackageApplication(string baseDirectoryPath, Package package, string parentPackageName)
		{
			Application application;
			IDTSEvents dtsEvents;

			if ((object)package == null)
				throw new ArgumentNullException("package");

			if ((object)parentPackageName == null)
				throw new ArgumentNullException("parentPackageName");

			dtsEvents = new ConsoleEvents();
			application = new Application();

			application.SaveToXml(Path.Combine(baseDirectoryPath, parentPackageName), package, dtsEvents);
		}

		private static void WriteProject(string baseDirectoryPath, Configuration configuration)
		{
			if ((object)configuration == null)
				throw new ArgumentNullException("configuration");

			using (Project project = Project.CreateProject())
			{
				/*ConnectionManagerItem sourceConnectionManagerItem;
				ConnectionManagerItem destinationConnectionManagerItem;
				ConnectionManager sourceConnectionManager;
				ConnectionManager destinationConnectionManager;*/

				project.Name = string.Format(SsisProjectName);

				/*sourceConnectionManagerItem = project.ConnectionManagerItems.Add("OLEDB", "Source.conmgr");
				sourceConnectionManagerItem.ConnectionManager.Name = "Source OLEDB Connection Manager";
				sourceConnectionManagerItem.ConnectionManager.ConnectionString = GetSourceConnectionString(FourPartName);
				sourceConnectionManager = sourceConnectionManagerItem.ConnectionManager;

				destinationConnectionManagerItem = project.ConnectionManagerItems.Add("OLEDB", "Destination.conmgr");
				destinationConnectionManagerItem.ConnectionManager.Name = "Destination OLEDB Connection Manager";
				destinationConnectionManagerItem.ConnectionManager.ConnectionString = GetDestinationConnectionString(FourPartName);
				destinationConnectionManager = destinationConnectionManagerItem.ConnectionManager;*/

				foreach (var @object in configuration.Objects)
					WritePackage(configuration.ValidateExternalMetadata ?? true, baseDirectoryPath, project, @object);

				// MASTER: server ...
				// SERVER: database ...
				// DATABASE: schema ...
				// SCHEMA: objtype...
				// OBJTYPE: ...
				// OBJECT .

				var serverNameCts = configuration.Objects
					.GroupBy(o => o.Source.ServerName)
					.Select(cl => new
								{
									_4PN = cl.First().Source,
									ServerName = cl.First().Source.ServerName,
									Count = cl.Count()
								});

				WriteParentPackage(baseDirectoryPath, project, SsisRootPackageName, serverNameCts.Select(cl => GetSsisServerPackageName(cl._4PN)).ToArray());

				foreach (var serverNameCt in serverNameCts)
				{
					Console.WriteLine("{0}{1}({2})", new string(' ', 0), serverNameCt.ServerName, serverNameCt.Count);

					var databaseNameCts = configuration.Objects.Where(o => o.Source.ServerName == serverNameCt.ServerName)
						.GroupBy(o => o.Source.DatabaseName)
						.Select(cl => new
									{
										_4PN = cl.First().Source,
										DatabaseName = cl.First().Source.DatabaseName,
										Count = cl.Count()
									});

					WriteParentPackage(baseDirectoryPath, project, GetSsisServerPackageName(serverNameCt._4PN), databaseNameCts.Select(cl => GetSsisDatabasePackageName(cl._4PN)).ToArray());

					foreach (var databaseNameCt in databaseNameCts)
					{
						Console.WriteLine("{0}{1}({2})", new string(' ', 1), databaseNameCt.DatabaseName, databaseNameCt.Count);

						var schemaNameCts = configuration.Objects.Where(o => o.Source.ServerName == serverNameCt.ServerName && o.Source.DatabaseName == databaseNameCt.DatabaseName)
							.GroupBy(o => o.Source.SchemaName)
							.Select(cl => new
										{
											_4PN = cl.First().Source,
											SchemaName = cl.First().Source.SchemaName,
											Count = cl.Count()
										});

						WriteParentPackage(baseDirectoryPath, project, GetSsisDatabasePackageName(databaseNameCt._4PN), schemaNameCts.Select(cl => GetSsisSchemaPackageName(cl._4PN)).ToArray());

						foreach (var schemaNameCt in schemaNameCts)
						{
							Console.WriteLine("{0}{1}({2})", new string(' ', 2), schemaNameCt.SchemaName, schemaNameCt.Count);

							var objectTypeCts = configuration.Objects.Where(o => o.Source.ServerName == serverNameCt.ServerName && o.Source.DatabaseName == databaseNameCt.DatabaseName && o.Source.SchemaName == schemaNameCt.SchemaName)
								.GroupBy(o => o.Source.ObjectType)
								.Select(cl => new
											{
												_4PN = cl.First().Source,
												ObjectType = cl.First().Source.ObjectType,
												Count = cl.Count()
											});

							WriteParentPackage(baseDirectoryPath, project, GetSsisSchemaPackageName(schemaNameCt._4PN), objectTypeCts.Select(cl => GetSsisObjectTypePackageName(cl._4PN)).ToArray());

							foreach (var objectTypeCt in objectTypeCts)
							{
								Console.WriteLine("{0}{1}({2})", new string(' ', 3), objectTypeCt.ObjectType, objectTypeCt.Count);

								var objectCts = configuration.Objects.Where(o => o.Source.ServerName == serverNameCt.ServerName && o.Source.DatabaseName == databaseNameCt.DatabaseName && o.Source.SchemaName == schemaNameCt.SchemaName && o.Source.ObjectType == objectTypeCt.ObjectType)
									.GroupBy(o => o.Source.ObjectName)
									.Select(cl => new
												{
													_4PN = cl.First().Source,
													ObjectName = cl.First().Source.ObjectName,
													Count = cl.Count()
												});

								WriteParentPackage(baseDirectoryPath, project, GetSsisObjectTypePackageName(objectTypeCt._4PN), objectCts.Select(cl => GetSsisObjectPackageName(cl._4PN)).ToArray());

								// complete
							}
						}
					}
				}

				project.SaveTo(Path.Combine(baseDirectoryPath, SsisArchiveFileName));
			}
		}

		#endregion
	}
}