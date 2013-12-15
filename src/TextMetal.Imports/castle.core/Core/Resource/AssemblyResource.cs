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
using System.Reflection;

namespace Castle.Core.Resource
{
	using System;

	public class AssemblyResource : AbstractStreamResource
	{
		#region Constructors/Destructors

		public AssemblyResource(CustomUri resource)
		{
			this.CreateStream = delegate
								{
									return this.CreateResourceFromUri(resource, null);
								};
		}

		public AssemblyResource(CustomUri resource, String basePath)
		{
			this.CreateStream = delegate
								{
									return this.CreateResourceFromUri(resource, basePath);
								};
		}

		public AssemblyResource(String resource)
		{
			this.CreateStream = delegate
								{
									return this.CreateResourceFromPath(resource, this.basePath);
								};
		}

		#endregion

		#region Fields/Constants

		private string assemblyName;
		private String basePath;
		private string resourcePath;

		#endregion

		#region Methods/Operators

		private static Assembly ObtainAssembly(String assemblyName)
		{
			try
			{
				return Assembly.Load(assemblyName);
			}
			catch (Exception ex)
			{
				String message = String.Format(CultureInfo.InvariantCulture, "The assembly {0} could not be loaded", assemblyName);
				throw new ResourceException(message, ex);
			}
		}

		private string ConvertToPath(String resource)
		{
			string path = resource.Replace('.', '/');
			if (path[0] != '/')
				path = string.Format(CultureInfo.CurrentCulture, "/{0}", path);
			return path;
		}

		private string ConvertToResourceName(String assembly, String resource)
		{
			assembly = this.GetSimpleName(assembly);
			// TODO: use path for relative name construction
			return String.Format(CultureInfo.CurrentCulture, "{0}{1}", assembly, resource.Replace('/', '.'));
		}

		public override IResource CreateRelative(String relativePath)
		{
			throw new NotImplementedException();
		}

		private Stream CreateResourceFromPath(String resource, String path)
		{
			if (!resource.StartsWith("assembly" + CustomUri.SchemeDelimiter, StringComparison.CurrentCulture))
				resource = "assembly" + CustomUri.SchemeDelimiter + resource;

			return this.CreateResourceFromUri(new CustomUri(resource), path);
		}

		private Stream CreateResourceFromUri(CustomUri resourcex, String path)
		{
			if (resourcex == null)
				throw new ArgumentNullException("resourcex");

			this.assemblyName = resourcex.Host;
			this.resourcePath = this.ConvertToResourceName(this.assemblyName, resourcex.Path);

			Assembly assembly = ObtainAssembly(this.assemblyName);

			String[] names = assembly.GetManifestResourceNames();

			String nameFound = this.GetNameFound(names);

			if (nameFound == null)
			{
				this.resourcePath = resourcex.Path.Replace('/', '.').Substring(1);
				nameFound = this.GetNameFound(names);
			}

			if (nameFound == null)
			{
				String message = String.Format(CultureInfo.InvariantCulture, "The assembly resource {0} could not be located", this.resourcePath);
				throw new ResourceException(message);
			}

			this.basePath = this.ConvertToPath(this.resourcePath);

			return assembly.GetManifestResourceStream(nameFound);
		}

		private string GetNameFound(string[] names)
		{
			string nameFound = null;
			foreach (String name in names)
			{
				if (String.Compare(this.resourcePath, name, StringComparison.OrdinalIgnoreCase) == 0)
				{
					nameFound = name;
					break;
				}
			}
			return nameFound;
		}

		private string GetSimpleName(string assembly)
		{
			int indexOfComma = assembly.IndexOf(',');
			if (indexOfComma < 0)
				return assembly;
			return assembly.Substring(0, indexOfComma);
		}

		public override string ToString()
		{
			return String.Format(CultureInfo.CurrentCulture, "AssemblyResource: [{0}] [{1}]", this.assemblyName, this.resourcePath);
		}

		#endregion
	}
}