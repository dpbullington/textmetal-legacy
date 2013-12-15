// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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
using System.Resources;
using System.Text;

namespace Castle.Core.Resource
{
	using System;

	public class AssemblyBundleResource : AbstractResource
	{
		#region Constructors/Destructors

		public AssemblyBundleResource(CustomUri resource)
		{
			this.resource = resource;
		}

		#endregion

		#region Fields/Constants

		private readonly CustomUri resource;

		#endregion

		#region Methods/Operators

		private static Assembly ObtainAssembly(string assemblyName)
		{
			try
			{
				return Assembly.Load(assemblyName);
			}
			catch (Exception ex)
			{
				var message = String.Format(CultureInfo.InvariantCulture, "The assembly {0} could not be loaded", assemblyName);
				throw new ResourceException(message, ex);
			}
		}

		public override IResource CreateRelative(string relativePath)
		{
			throw new NotImplementedException();
		}

		public override TextReader GetStreamReader()
		{
			var assembly = ObtainAssembly(this.resource.Host);

			var paths = this.resource.Path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
			if (paths.Length != 2)
			{
				throw new ResourceException("AssemblyBundleResource does not support paths with more than 2 levels in depth. See " +
											this.resource.Path);
			}

			var rm = new ResourceManager(paths[0], assembly);

			return new StringReader(rm.GetString(paths[1]));
		}

		public override TextReader GetStreamReader(Encoding encoding)
		{
			return this.GetStreamReader();
		}

		#endregion
	}
}