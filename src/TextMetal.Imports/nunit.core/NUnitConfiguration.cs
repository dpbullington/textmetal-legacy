// ****************************************************************
// Copyright 2008, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org.
// ****************************************************************

using System;
using System.IO;
using System.Reflection;

using Microsoft.Win32;

namespace NUnit.Core
{
	/// <summary>
	/// Provides static methods for accessing the NUnit config
	/// file
	/// </summary>
	public class NUnitConfiguration
	{
		#region Fields/Constants

		private static string addinDirectory;
		private static string applicationDirectory;
		private static string logDirectory;
		private static string monoExePath;
		private static string nunitBinDirectory;

		private static string nunitDocDirectory;
		private static string nunitLibDirectory;

		#endregion

		#region Properties/Indexers/Events

		public static string AddinDirectory
		{
			get
			{
				if (addinDirectory == null)
					addinDirectory = Path.Combine(NUnitBinDirectory, "addins");

				return addinDirectory;
			}
		}

		public static string ApplicationDirectory
		{
			get
			{
				if (applicationDirectory == null)
				{
					applicationDirectory = Path.Combine(
						Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
						"NUnit");
				}

				return applicationDirectory;
			}
		}

		public static string BuildConfiguration
		{
			get
			{
#if DEBUG
				return "Debug";
#else
					return "Release";
#endif
			}
		}

		public static string HelpUrl
		{
			get
			{
				string helpUrl = "http://nunit.org";

				string dir = Path.GetDirectoryName(NUnitBinDirectory);
				string docDir = null;

				while (dir != null)
				{
					docDir = Path.Combine(dir, "doc");
					if (Directory.Exists(docDir))
						break;
					dir = Path.GetDirectoryName(dir);
				}

				if (docDir != null)
				{
					string localPath = Path.Combine(docDir, "index.html");
					if (File.Exists(localPath))
					{
						UriBuilder uri = new UriBuilder();
						uri.Scheme = "file";
						uri.Host = "localhost";
						uri.Path = localPath;
						helpUrl = uri.ToString();
					}
				}

				return helpUrl;
			}
		}

		public static string LogDirectory
		{
			get
			{
				if (logDirectory == null)
					logDirectory = Path.Combine(ApplicationDirectory, "logs");

				return logDirectory;
			}
		}

		//private static string testAgentExePath;
		//private static string TestAgentExePath
		//{
		//    get
		//    {
		//        if (testAgentExePath == null)
		//            testAgentExePath = Path.Combine(NUnitBinDirectory, "nunit-agent.exe");

		//        return testAgentExePath;
		//    }
		//}

		public static string MonoExePath
		{
			get
			{
				if (monoExePath == null)
				{
					string[] searchNames = IsWindows()
						? new string[] { "mono.bat", "mono.cmd", "mono.exe" }
						: new string[] { "mono", "mono.exe" };

					monoExePath = FindOneOnPath(searchNames);

					if (monoExePath == null && IsWindows())
					{
						RegistryKey key = Registry.LocalMachine.OpenSubKey(@"Software\Novell\Mono");
						if (key != null)
						{
							string version = key.GetValue("DefaultCLR") as string;
							if (version != null)
							{
								key = key.OpenSubKey(version);
								if (key != null)
								{
									string installDir = key.GetValue("SdkInstallRoot") as string;
									if (installDir != null)
										monoExePath = Path.Combine(installDir, @"bin\mono.exe");
								}
							}
						}
					}

					if (monoExePath == null)
						return "mono";
				}

				return monoExePath;
			}
		}

		public static string NUnitBinDirectory
		{
			get
			{
				if (nunitBinDirectory == null)
				{
					nunitBinDirectory = NUnitLibDirectory;
					if (Path.GetFileName(nunitBinDirectory).ToLower() == "lib")
						nunitBinDirectory = Path.GetDirectoryName(nunitBinDirectory);
				}

				return nunitBinDirectory;
			}
		}

		private static string NUnitDocDirectory
		{
			get
			{
				if (nunitDocDirectory == null)
				{
					string dir = Path.GetDirectoryName(NUnitBinDirectory);
					nunitDocDirectory = Path.Combine(dir, "doc");
					if (!Directory.Exists(nunitDocDirectory))
					{
						dir = Path.GetDirectoryName(dir);
						nunitDocDirectory = Path.Combine(dir, "doc");
					}
				}

				return nunitDocDirectory;
			}
		}

		/// <summary>
		/// Gets the path to the lib directory for the version and build
		/// of NUnit currently executing.
		/// </summary>
		public static string NUnitLibDirectory
		{
			get
			{
				if (nunitLibDirectory == null)
				{
					nunitLibDirectory =
						AssemblyHelper.GetDirectoryName(Assembly.GetExecutingAssembly());
				}

				return nunitLibDirectory;
			}
		}

		#endregion

		#region Methods/Operators

		private static string FindOneOnPath(string[] names)
		{
			//foreach (string dir in Environment.GetEnvironmentVariable("path").Split(new char[] { Path.PathSeparator }))
			//    foreach (string name in names)
			//    {
			//        string fullPath = Path.Combine(dir, name);
			//        if (File.Exists(fullPath))
			//            return name;
			//    }

			return null;
		}

		private static bool IsWindows()
		{
			return Environment.OSVersion.Platform == PlatformID.Win32NT;
		}

		#endregion
	}
}