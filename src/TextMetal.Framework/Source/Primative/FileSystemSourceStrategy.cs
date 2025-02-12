﻿/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.IO;

using TextMetal.Framework.Associative;
using TextMetal.Middleware.Solder.Extensions;

namespace TextMetal.Framework.Source.Primative
{
	public class FileSystemSourceStrategy : SourceStrategy
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the FileSystemSourceStrategy class.
		/// </summary>
		public FileSystemSourceStrategy()
		{
		}

		#endregion

		#region Methods/Operators

		private static void EnumerateFileSystem(string directoryPath, bool recursive, string wildcard, AssociativeXmlObject parent, string sourcePath)
		{
			ArrayConstruct arrayConstruct00;
			PropertyConstruct propertyConstruct00;
			ObjectConstruct objectConstruct00;

			ArrayConstruct arrayConstruct01;
			PropertyConstruct propertyConstruct01;
			ObjectConstruct objectConstruct01;

			string[] directories;
			string[] files;

			if ((object)directoryPath == null)
				throw new ArgumentNullException(nameof(directoryPath));

			if ((object)parent == null)
				throw new ArgumentNullException(nameof(parent));

			arrayConstruct00 = new ArrayConstruct();
			arrayConstruct00.Name = "Files";
			parent.Items.Add(arrayConstruct00);

			arrayConstruct01 = new ArrayConstruct();
			arrayConstruct01.Name = "Directories";
			parent.Items.Add(arrayConstruct01);

			if (File.Exists(directoryPath))
			{
				files = new string[] { Path.GetFullPath(directoryPath) };
				directories = null;
			}
			else
			{
				if (SolderFascadeAccessor.DataTypeFascade.IsNullOrWhiteSpace(wildcard))
					files = Directory.GetFiles(directoryPath);
				else
					files = Directory.GetFiles(directoryPath, wildcard);

				if (SolderFascadeAccessor.DataTypeFascade.IsNullOrWhiteSpace(wildcard))
					directories = Directory.GetDirectories(directoryPath);
				else
					directories = Directory.GetDirectories(directoryPath, wildcard);
			}

			if ((object)files != null)
			{
				foreach (string file in files)
				{
					FileInfo fileInfo;

					fileInfo = new FileInfo(file);

					objectConstruct00 = new ObjectConstruct();
					arrayConstruct00.Items.Add(objectConstruct00);

					propertyConstruct00 = new PropertyConstruct();
					propertyConstruct00.Name = "FileFullName";
					propertyConstruct00.RawValue = fileInfo.FullName;
					objectConstruct00.Items.Add(propertyConstruct00);

					propertyConstruct00 = new PropertyConstruct();
					propertyConstruct00.Name = "FileFullNameRelativeToSource";
					propertyConstruct00.RawValue = EvaluateRelativePath2(sourcePath, fileInfo.FullName);
					objectConstruct00.Items.Add(propertyConstruct00);

					propertyConstruct00 = new PropertyConstruct();
					propertyConstruct00.Name = "FileCreationTime";
					propertyConstruct00.RawValue = fileInfo.CreationTime;
					objectConstruct00.Items.Add(propertyConstruct00);

					propertyConstruct00 = new PropertyConstruct();
					propertyConstruct00.Name = "FileCreationTimeUtc";
					propertyConstruct00.RawValue = fileInfo.CreationTimeUtc;
					objectConstruct00.Items.Add(propertyConstruct00);

					propertyConstruct00 = new PropertyConstruct();
					propertyConstruct00.Name = "FileExtension";
					propertyConstruct00.RawValue = fileInfo.Extension;
					objectConstruct00.Items.Add(propertyConstruct00);

					propertyConstruct00 = new PropertyConstruct();
					propertyConstruct00.Name = "FileIsReadOnly";
					propertyConstruct00.RawValue = fileInfo.IsReadOnly;
					objectConstruct00.Items.Add(propertyConstruct00);

					propertyConstruct00 = new PropertyConstruct();
					propertyConstruct00.Name = "FileLastAccessTime";
					propertyConstruct00.RawValue = fileInfo.LastAccessTime;
					objectConstruct00.Items.Add(propertyConstruct00);

					propertyConstruct00 = new PropertyConstruct();
					propertyConstruct00.Name = "FileLastAccessTimeUtc";
					propertyConstruct00.RawValue = fileInfo.LastAccessTimeUtc;
					objectConstruct00.Items.Add(propertyConstruct00);

					propertyConstruct00 = new PropertyConstruct();
					propertyConstruct00.Name = "FileLastWriteTime";
					propertyConstruct00.RawValue = fileInfo.LastWriteTime;
					objectConstruct00.Items.Add(propertyConstruct00);

					propertyConstruct00 = new PropertyConstruct();
					propertyConstruct00.Name = "FileLastWriteTimeUtc";
					propertyConstruct00.RawValue = fileInfo.LastWriteTimeUtc;
					objectConstruct00.Items.Add(propertyConstruct00);

					propertyConstruct00 = new PropertyConstruct();
					propertyConstruct00.Name = "FileLength";
					propertyConstruct00.RawValue = fileInfo.Length;
					objectConstruct00.Items.Add(propertyConstruct00);

					propertyConstruct00 = new PropertyConstruct();
					propertyConstruct00.Name = "FileName";
					propertyConstruct00.RawValue = fileInfo.Name;
					objectConstruct00.Items.Add(propertyConstruct00);
					
					// DPB 2021-11-24
					propertyConstruct00 = new PropertyConstruct();
					propertyConstruct00.Name = "FileNameWoExt";
					propertyConstruct00.RawValue = Path.GetFileNameWithoutExtension(fileInfo.Name);
					objectConstruct00.Items.Add(propertyConstruct00);
				}
			}

			if ((object)directories != null)
			{
				foreach (string directory in directories)
				{
					DirectoryInfo directoryInfo;

					directoryInfo = new DirectoryInfo(directory);

					objectConstruct01 = new ObjectConstruct();
					arrayConstruct01.Items.Add(objectConstruct01);

					propertyConstruct01 = new PropertyConstruct();
					propertyConstruct01.Name = "DirectoryFullName";
					propertyConstruct01.RawValue = directoryInfo.FullName;
					objectConstruct01.Items.Add(propertyConstruct01);

					propertyConstruct01 = new PropertyConstruct();
					propertyConstruct01.Name = "DirectoryFullNameRelativeToSource";
					propertyConstruct01.RawValue = EvaluateRelativePath2(sourcePath, directoryInfo.FullName);
					objectConstruct01.Items.Add(propertyConstruct01);

					propertyConstruct01 = new PropertyConstruct();
					propertyConstruct01.Name = "DirectoryAttributes";
					propertyConstruct01.RawValue = directoryInfo.Attributes;
					objectConstruct01.Items.Add(propertyConstruct01);

					propertyConstruct01 = new PropertyConstruct();
					propertyConstruct01.Name = "DirectoryCreationTime";
					propertyConstruct01.RawValue = directoryInfo.CreationTime;
					objectConstruct01.Items.Add(propertyConstruct01);

					propertyConstruct01 = new PropertyConstruct();
					propertyConstruct01.Name = "DirectoryCreationTimeUtc";
					propertyConstruct01.RawValue = directoryInfo.CreationTimeUtc;
					objectConstruct01.Items.Add(propertyConstruct01);

					propertyConstruct01 = new PropertyConstruct();
					propertyConstruct01.Name = "DirectoryExtension";
					propertyConstruct01.RawValue = directoryInfo.Extension;
					objectConstruct01.Items.Add(propertyConstruct01);

					propertyConstruct01 = new PropertyConstruct();
					propertyConstruct01.Name = "DirectoryLastAccessTime";
					propertyConstruct01.RawValue = directoryInfo.LastAccessTime;
					objectConstruct01.Items.Add(propertyConstruct01);

					propertyConstruct01 = new PropertyConstruct();
					propertyConstruct01.Name = "DirectoryLastAccessTimeUtc";
					propertyConstruct01.RawValue = directoryInfo.LastAccessTimeUtc;
					objectConstruct01.Items.Add(propertyConstruct01);

					propertyConstruct01 = new PropertyConstruct();
					propertyConstruct01.Name = "DirectoryLastWriteTime";
					propertyConstruct01.RawValue = directoryInfo.LastWriteTime;
					objectConstruct01.Items.Add(propertyConstruct01);

					propertyConstruct01 = new PropertyConstruct();
					propertyConstruct01.Name = "DirectoryLastWriteTimeUtc";
					propertyConstruct01.RawValue = directoryInfo.LastWriteTimeUtc;
					objectConstruct01.Items.Add(propertyConstruct01);

					propertyConstruct01 = new PropertyConstruct();
					propertyConstruct01.Name = "DirectoryName";
					propertyConstruct01.RawValue = directoryInfo.Name;
					objectConstruct01.Items.Add(propertyConstruct01);
					
					// DPB 2021-11-24
					propertyConstruct00 = new PropertyConstruct();
					propertyConstruct00.Name = "DirectoryNameWoExt";
					propertyConstruct00.RawValue = Path.GetFileNameWithoutExtension(directoryInfo.Name);
					objectConstruct01.Items.Add(propertyConstruct00);

					if (recursive)
						EnumerateFileSystem(directory, recursive, wildcard, objectConstruct01, sourcePath);
				}
			}
		}

		private static string EvaluateRelativePath(string mainDirPath, string absoluteFilePath)
		{
			string[] firstPathParts = mainDirPath.Trim(Path.DirectorySeparatorChar).Split(Path.DirectorySeparatorChar);
			string[] secondPathParts = absoluteFilePath.Trim(Path.DirectorySeparatorChar).Split(Path.DirectorySeparatorChar);

			int sameCounter = 0;
			for (int i = 0; i < Math.Min(firstPathParts.Length, secondPathParts.Length); i++)
			{
				if (!firstPathParts[i].ToLower().Equals(secondPathParts[i].ToLower()))
					break;

				sameCounter++;
			}

			if (sameCounter == 0)
				return absoluteFilePath;

			string newPath = string.Empty;
			for (int i = sameCounter; i < firstPathParts.Length; i++)
			{
				if (i > sameCounter)
					newPath += Path.DirectorySeparatorChar;

				newPath += "..";
			}

			if (newPath.Length == 0)
				newPath = ".";

			for (int i = sameCounter; i < secondPathParts.Length; i++)
			{
				newPath += Path.DirectorySeparatorChar;
				newPath += secondPathParts[i];
			}

			return newPath;
		}

		private static string EvaluateRelativePath2(string mainDirPath, string absoluteFilePath)
		{
			string p = EvaluateRelativePath(mainDirPath, absoluteFilePath);

			if (p.StartsWith(".\\"))
				p = p.Substring(2);

			return p;
		}

		protected override object CoreGetSourceObject(string sourceFilePath, IDictionary<string, IList<string>> properties)
		{
			const string PROP_TOKEN_RECURSIVE = "Recursive";
			const string PROP_TOKEN_WILDCARD = "Wildcard";
			ObjectConstruct objectConstruct00;
			PropertyConstruct propertyConstruct00;
			IList<string> values;
			bool recursive = false;
			string recursiveStr;
			string wildcard;

			if ((object)sourceFilePath == null)
				throw new ArgumentNullException(nameof(sourceFilePath));

			if ((object)properties == null)
				throw new ArgumentNullException(nameof(properties));

			if (SolderFascadeAccessor.DataTypeFascade.IsWhiteSpace(sourceFilePath))
				throw new ArgumentOutOfRangeException(nameof(sourceFilePath));

			sourceFilePath = Path.GetFullPath(sourceFilePath);

			recursiveStr = null;
			if (properties.TryGetValue(PROP_TOKEN_RECURSIVE, out values))
			{
				if ((object)values != null && values.Count == 1)
				{
					recursiveStr = values[0];
					if (!SolderFascadeAccessor.DataTypeFascade.TryParse<bool>(recursiveStr, out recursive))
					{
						// do nothing
					}
				}
			}

			wildcard = null;
			if (properties.TryGetValue(PROP_TOKEN_WILDCARD, out values))
			{
				if ((object)values != null && values.Count == 1)
					wildcard = values[0];
			}

			objectConstruct00 = new ObjectConstruct();

			propertyConstruct00 = new PropertyConstruct();
			propertyConstruct00.Name = "SourceFullPath";
			propertyConstruct00.RawValue = sourceFilePath;
			objectConstruct00.Items.Add(propertyConstruct00);

			propertyConstruct00 = new PropertyConstruct();
			propertyConstruct00.Name = "Recursive";
			propertyConstruct00.RawValue = recursive;
			objectConstruct00.Items.Add(propertyConstruct00);

			propertyConstruct00 = new PropertyConstruct();
			propertyConstruct00.Name = "Wildcard";
			propertyConstruct00.RawValue = wildcard;
			objectConstruct00.Items.Add(propertyConstruct00);

			EnumerateFileSystem(sourceFilePath, recursive, wildcard, objectConstruct00, sourceFilePath);

			return objectConstruct00;
		}

		#endregion
	}
}