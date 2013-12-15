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

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;

namespace Castle.Core.Resource
{
	using System;

	/// <summary>
	/// Enable access to files on network shares
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Unc")]
	public class UncResource : AbstractStreamResource
	{
		#region Constructors/Destructors

		public UncResource(CustomUri resource)
		{
			this.CreateStream = delegate
								{
									return this.CreateStreamFromUri(resource, DefaultBasePath);
								};
		}

		public UncResource(CustomUri resource, String basePath)
		{
			this.CreateStream = delegate
								{
									return this.CreateStreamFromUri(resource, basePath);
								};
		}

		public UncResource(String resourceName)
			: this(new CustomUri(resourceName))
		{
		}

		public UncResource(String resourceName, String basePath)
			: this(new CustomUri(resourceName), basePath)
		{
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
				String message = String.Format(CultureInfo.InvariantCulture, "File {0} could not be found", path);
				throw new ResourceException(message);
			}
		}

		public override IResource CreateRelative(String relativePath)
		{
			return new UncResource(Path.Combine(this.basePath, relativePath));
		}

		private Stream CreateStreamFromUri(CustomUri resource, String rootPath)
		{
			if (resource == null)
				throw new ArgumentNullException("resource");
			if (!resource.IsUnc)
				throw new ArgumentException("Resource must be an Unc", "resource");
			if (!resource.IsFile)
				throw new ArgumentException("The specified resource is not a file", "resource");

			String resourcePath = resource.Path;

			if (!File.Exists(resourcePath) && rootPath != null)
				resourcePath = Path.Combine(rootPath, resourcePath);

			this.filePath = Path.GetFileName(resourcePath);
			this.basePath = Path.GetDirectoryName(resourcePath);

			CheckFileExists(resourcePath);

			return File.OpenRead(resourcePath);
		}

		public override string ToString()
		{
			return String.Format(CultureInfo.CurrentCulture, "UncResource: [{0}] [{1}]", this.filePath, this.basePath);
		}

		#endregion
	}
}