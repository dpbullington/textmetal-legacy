// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Globalization;
using System.IO;

namespace Castle.Core.Resource
{
	using System;

	/// <summary>
	/// </summary>
	public class FileResource : AbstractStreamResource
	{
		#region Constructors/Destructors

		public FileResource(CustomUri resource)
		{
			this.CreateStream = delegate
								{
									return this.CreateStreamFromUri(resource, DefaultBasePath);
								};
		}

		public FileResource(CustomUri resource, String basePath)
		{
			this.CreateStream = delegate
								{
									return this.CreateStreamFromUri(resource, basePath);
								};
		}

		public FileResource(String resourceName)
		{
			this.CreateStream = delegate
								{
									return this.CreateStreamFromPath(resourceName, DefaultBasePath);
								};
		}

		public FileResource(String resourceName, String basePath)
		{
			this.CreateStream = delegate
								{
									return this.CreateStreamFromPath(resourceName, basePath);
								};
		}

		#endregion

		#region Fields/Constants

		private String basePath;
		private string filePath;

		#endregion

		#region Properties/Indexers/Events

		public override String FileBasePath
		{
			get
			{
				return this.basePath;
			}
		}

		#endregion

		#region Methods/Operators

		private static void CheckFileExists(String path)
		{
			if (!File.Exists(path))
			{
				String message = String.Format(CultureInfo.InvariantCulture, "File {0} could not be found", new FileInfo(path).FullName);
				throw new ResourceException(message);
			}
		}

		public override IResource CreateRelative(String relativePath)
		{
			return new FileResource(relativePath, this.basePath);
		}

		private Stream CreateStreamFromPath(String resourcePath, String rootPath)
		{
			if (resourcePath == null)
				throw new ArgumentNullException("resourcePath");
			if (rootPath == null)
				throw new ArgumentNullException("rootPath");

			if (!Path.IsPathRooted(resourcePath) || !File.Exists(resourcePath))
			{
				// For a relative path, we use the basePath to
				// resolve the full path

				resourcePath = Path.Combine(rootPath, resourcePath);
			}

			CheckFileExists(resourcePath);

			this.filePath = Path.GetFileName(resourcePath);
			this.basePath = Path.GetDirectoryName(resourcePath);

			return File.OpenRead(resourcePath);
		}

		private Stream CreateStreamFromUri(CustomUri resource, String rootPath)
		{
			if (resource == null)
				throw new ArgumentNullException("resource");
			if (rootPath == null)
				throw new ArgumentNullException("rootPath");

			if (!resource.IsFile)
				throw new ArgumentException("The specified resource is not a file", "resource");

			return this.CreateStreamFromPath(resource.Path, rootPath);
		}

		public override string ToString()
		{
			return String.Format(CultureInfo.CurrentCulture, "FileResource: [{0}] [{1}]", this.filePath, this.basePath);
		}

		#endregion
	}
}