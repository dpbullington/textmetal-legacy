// ****************************************************************
// This is free software licensed under the NUnit license. You
// may obtain a copy of the license as well as information regarding
// copyright ownership at http://nunit.org.
// ****************************************************************

using System.Collections;
using System.IO;
using System.Reflection;

namespace NUnit.Core
{
	using System;

	/// <summary>
	/// Class adapted from NUnitAddin for use in handling assemblies that are not
	/// found in the test AppDomain.
	/// </summary>
	public class AssemblyResolver : MarshalByRefObject, IDisposable
	{
		#region Constructors/Destructors

		public AssemblyResolver()
		{
			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(this.CurrentDomain_AssemblyResolve);
		}

		#endregion

		#region Fields/Constants

		private static Logger log = InternalTrace.GetLogger(typeof(AssemblyResolver));

		private AssemblyCache _cache = new AssemblyCache();

		private ArrayList _dirs = new ArrayList();

		#endregion

		#region Methods/Operators

		public void AddDirectory(string directory)
		{
			if (Directory.Exists(directory))
				this._dirs.Add(directory);
		}

		public void AddFile(string file)
		{
			Assembly assembly = Assembly.LoadFrom(file);
			this._cache.Add(assembly.GetName().FullName, assembly);
		}

		public void AddFiles(string directory, string pattern)
		{
			if (Directory.Exists(directory))
			{
				foreach (string file in Directory.GetFiles(directory, pattern))
					this.AddFile(file);
			}
		}

		private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			string fullName = args.Name;
			int index = fullName.IndexOf(',');
			if (index == -1) // Only resolve using full name.
			{
				log.Debug(string.Format("Not a strong name: {0}", fullName));
				return null;
			}

			if (this._cache.Contains(fullName))
			{
				log.Info(string.Format("Resolved from Cache: {0}", fullName));
				return this._cache.Resolve(fullName);
			}

			foreach (string dir in this._dirs)
			{
				foreach (string file in Directory.GetFiles(dir, "*.dll"))
				{
					string fullFile = Path.Combine(dir, file);
					using (AssemblyReader rdr = new AssemblyReader(fullFile))
					{
						try
						{
							if (rdr.IsDotNetFile)
							{
								if (AssemblyName.GetAssemblyName(fullFile).FullName == fullName)
								{
									log.Info(string.Format("Added to Cache: {0}", fullFile));
									this.AddFile(fullFile);
									return this._cache.Resolve(fullName);
								}
							}
						}
						catch (Exception ex)
						{
							log.Error("Unable to load addin assembly", ex);
							throw;
						}
					}
				}
			}

			log.Debug(string.Format("Not in Cache: {0}", fullName));
			return null;
		}

		public void Dispose()
		{
			AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(this.CurrentDomain_AssemblyResolve);
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		private class AssemblyCache
		{
			#region Fields/Constants

			private Hashtable _resolved = new Hashtable();

			#endregion

			#region Methods/Operators

			public void Add(string name, Assembly assembly)
			{
				this._resolved[name] = assembly;
			}

			public bool Contains(string name)
			{
				return this._resolved.ContainsKey(name);
			}

			public Assembly Resolve(string name)
			{
				if (this._resolved.ContainsKey(name))
					return (Assembly)this._resolved[name];

				return null;
			}

			#endregion
		}

		#endregion
	}
}