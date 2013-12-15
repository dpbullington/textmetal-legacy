// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.IO;

namespace NUnit.Util
{
	public class RecentFileEntry
	{
		#region Constructors/Destructors

		public RecentFileEntry(string path)
		{
			this.path = path;
			this.clrVersion = Environment.Version;
		}

		public RecentFileEntry(string path, Version clrVersion)
		{
			this.path = path;
			this.clrVersion = clrVersion;
		}

		#endregion

		#region Fields/Constants

		public static readonly char Separator = ',';
		private Version clrVersion;
		private string path;

		#endregion

		#region Properties/Indexers/Events

		public Version CLRVersion
		{
			get
			{
				return this.clrVersion;
			}
		}

		public bool Exists
		{
			get
			{
				return this.path != null && File.Exists(this.path);
			}
		}

		public bool IsCompatibleCLRVersion
		{
			get
			{
				return this.clrVersion.Major <= Environment.Version.Major;
			}
		}

		public string Path
		{
			get
			{
				return this.path;
			}
		}

		#endregion

		#region Methods/Operators

		public static RecentFileEntry Parse(string text)
		{
			int sepIndex = text.LastIndexOf(Separator);

			if (sepIndex > 0)
			{
				try
				{
					return new RecentFileEntry(text.Substring(0, sepIndex),
						new Version(text.Substring(sepIndex + 1)));
				}
				catch
				{
					//The last part was not a version, so fall through and return the whole text
				}
			}

			return new RecentFileEntry(text);
		}

		public override string ToString()
		{
			return this.Path + Separator + this.CLRVersion.ToString();
		}

		#endregion
	}
}