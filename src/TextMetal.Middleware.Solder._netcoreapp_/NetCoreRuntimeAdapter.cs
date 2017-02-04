/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

using Microsoft.Extensions.DependencyModel;

using TextMetal.Middleware.Solder.Abstractions;

namespace TextMetal.Middleware.Solder._netcoreapp_
{
	public sealed class NetCoreRuntimeAdapter : RuntimeAdapter
	{
		#region Constructors/Destructors

		public NetCoreRuntimeAdapter()
			:
			this(AssemblyLoadContext.Default, DependencyContext.Default)
		{
		}

		private NetCoreRuntimeAdapter(AssemblyLoadContext assemblyLoadContext, DependencyContext dependencyContext)
		{
			if ((object)assemblyLoadContext == null)
				throw new ArgumentNullException(nameof(assemblyLoadContext));

			if ((object)dependencyContext == null)
				throw new ArgumentNullException(nameof(dependencyContext));

			this.assemblyLoadContext = assemblyLoadContext;
			this.dependencyContext = dependencyContext;

			// hook assembly load context events
			this.AssemblyLoadContext.Unloading += this.AssemblyLoadContext_OnUnloading;
		}

		#endregion

		#region Fields/Constants

		private readonly AssemblyLoadContext assemblyLoadContext;
		private readonly DependencyContext dependencyContext;

		#endregion

		#region Properties/Indexers/Events

		private AssemblyLoadContext AssemblyLoadContext
		{
			get
			{
				return this.assemblyLoadContext;
			}
		}

		private DependencyContext DependencyContext
		{
			get
			{
				return this.dependencyContext;
			}
		}

		#endregion

		#region Methods/Operators

		private void AssemblyLoadContext_OnUnloading(AssemblyLoadContext assemblyLoadContext)
		{
			if ((object)assemblyLoadContext == null)
				throw new ArgumentNullException(nameof(assemblyLoadContext));

			this.OnRuntimeTeardown();
		}

		public override void Dispose()
		{
			// unhook assembly load context events
			this.AssemblyLoadContext.Unloading -= this.AssemblyLoadContext_OnUnloading;
		}

		public override IEnumerable<Assembly> GetLoadedAssemblies()
		{
			return Enumerable.ToArray<Assembly>(this.DependencyContext.RuntimeLibraries
				.SelectMany(library =>
							{
								var _dependencyContext = this.DependencyContext;
								return DependencyContextExtensions.GetDefaultAssemblyNames(library, _dependencyContext);
							})
				.Select(assemblyName => Assembly.Load(assemblyName)));
		}

		public override Assembly LoadAssembly(AssemblyName assemblyName)
		{
			Assembly assembly;

			assembly = this.AssemblyLoadContext.LoadFromAssemblyName(assemblyName);

			return assembly;
		}

		#endregion
	}
}