// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org.
// ****************************************************************

using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace NUnit.Core
{
	/// <summary>
	/// AssemblyReader knows how to find various things in an assembly header
	/// </summary>
	public class AssemblyReader : IDisposable
	{
		#region Constructors/Destructors

		public AssemblyReader(string assemblyPath)
		{
			this.assemblyPath = assemblyPath;
			this.CalcHeaderOffsets();
		}

		public AssemblyReader(Assembly assembly)
		{
			this.assemblyPath = AssemblyHelper.GetAssemblyPath(assembly);
			this.CalcHeaderOffsets();
		}

		#endregion

		#region Fields/Constants

		private string assemblyPath;
		private uint dataDirectory = 0;
		private uint dataSections = 0;

		private UInt16 dos_magic = 0xffff;
		private uint fileHeader = 0;
		private FileStream fs;
		private UInt32 numDataDirectoryEntries;
		private UInt16 numberOfSections;
		private uint optionalHeader = 0;
		private UInt16 optionalHeaderSize;

		private uint peHeader = 0;
		private UInt16 peType;
		private UInt32 pe_signature = 0xffffffff;
		private BinaryReader rdr;

		private DataSection[] sections;

		#endregion

		#region Properties/Indexers/Events

		public string AssemblyPath
		{
			get
			{
				return this.assemblyPath;
			}
		}

		public string ImageRuntimeVersion
		{
			get
			{
				string runtimeVersion = string.Empty;

				if (this.IsDotNetFile)
				{
					uint rva = this.DataDirectoryRva(14);
					if (rva != 0)
					{
						this.fs.Position = this.RvaToLfa(rva) + 8;
						uint metadata = this.rdr.ReadUInt32();
						this.fs.Position = this.RvaToLfa(metadata);
						if (this.rdr.ReadUInt32() == 0x424a5342)
						{
							// Copy string representing runtime version
							this.fs.Position += 12;
							StringBuilder sb = new StringBuilder();
							char c;
							while ((c = this.rdr.ReadChar()) != '\0')
								sb.Append(c);

							if (sb[0] == 'v') // Last sanity check
								runtimeVersion = sb.ToString();

							// Could do fixups here for bad values in older files
							// like 1.x86, 1.build, etc. But we are only using
							// the major version anyway
						}
					}
				}

				return runtimeVersion;
			}
		}

		public bool Is64BitImage
		{
			get
			{
				return this.peType == 0x20b;
			}
		}

		public bool IsDotNetFile
		{
			get
			{
				return this.IsValidPeFile && this.numDataDirectoryEntries > 14 && this.DataDirectoryRva(14) != 0;
			}
		}

		public bool IsValidPeFile
		{
			get
			{
				return this.dos_magic == 0x5a4d && this.pe_signature == 0x00004550;
			}
		}

		#endregion

		#region Methods/Operators

		private void CalcHeaderOffsets()
		{
			this.fs = new FileStream(this.assemblyPath, FileMode.Open, FileAccess.Read);
			this.rdr = new BinaryReader(this.fs);
			this.dos_magic = this.rdr.ReadUInt16();
			if (this.dos_magic == 0x5a4d)
			{
				this.fs.Position = 0x3c;
				this.peHeader = this.rdr.ReadUInt32();
				this.fileHeader = this.peHeader + 4;
				this.optionalHeader = this.fileHeader + 20;

				this.fs.Position = this.optionalHeader;
				this.peType = this.rdr.ReadUInt16();

				this.dataDirectory = this.peType == 0x20b
					? this.optionalHeader + 112
					: this.optionalHeader + 96;

				this.fs.Position = this.dataDirectory - 4;
				this.numDataDirectoryEntries = this.rdr.ReadUInt32();

				this.fs.Position = this.peHeader;
				this.pe_signature = this.rdr.ReadUInt32();
				this.rdr.ReadUInt16(); // machine
				this.numberOfSections = this.rdr.ReadUInt16();
				this.fs.Position += 12;
				this.optionalHeaderSize = this.rdr.ReadUInt16();
				this.dataSections = this.optionalHeader + this.optionalHeaderSize;

				this.sections = new DataSection[this.numberOfSections];
				this.fs.Position = this.dataSections;
				for (int i = 0; i < this.numberOfSections; i++)
				{
					this.fs.Position += 8;
					this.sections[i].virtualSize = this.rdr.ReadUInt32();
					this.sections[i].virtualAddress = this.rdr.ReadUInt32();
					uint rawDataSize = this.rdr.ReadUInt32();
					this.sections[i].fileOffset = this.rdr.ReadUInt32();
					if (this.sections[i].virtualSize == 0)
						this.sections[i].virtualSize = rawDataSize;

					this.fs.Position += 16;
				}
			}
		}

		private uint DataDirectoryRva(int n)
		{
			this.fs.Position = this.dataDirectory + n * 8;
			return this.rdr.ReadUInt32();
		}

		public void Dispose()
		{
			if (this.fs != null)
				this.fs.Close();
			if (this.rdr != null)
				this.rdr.Close();

			this.fs = null;
			this.rdr = null;
		}

		private uint RvaToLfa(uint rva)
		{
			for (int i = 0; i < this.numberOfSections; i++)
			{
				if (rva >= this.sections[i].virtualAddress && rva < this.sections[i].virtualAddress + this.sections[i].virtualSize)
					return rva - this.sections[i].virtualAddress + this.sections[i].fileOffset;
			}

			return 0;
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		private struct DataSection
		{
			#region Fields/Constants

			public uint fileOffset;
			public uint virtualAddress;
			public uint virtualSize;

			#endregion
		};

		#endregion
	}
}