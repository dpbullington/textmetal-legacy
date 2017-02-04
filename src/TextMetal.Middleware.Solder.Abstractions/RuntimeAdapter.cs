/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Reflection;

namespace TextMetal.Middleware.Solder.Abstractions
{
	public abstract class RuntimeAdapter : IRuntimeAdapter
	{
		#region Constructors/Destructors

		protected RuntimeAdapter()
		{
		}

		#endregion

		#region Properties/Indexers/Events

		public event Action<Assembly> AssemblyLoaded;

		public event Action<IRuntimeAdapter> RuntimeTeardown;

		#endregion

		#region Methods/Operators

		public abstract void Dispose();

		public abstract IEnumerable<Assembly> GetLoadedAssemblies();

		public abstract Assembly LoadAssembly(AssemblyName assemblyName);

		protected void OnAssemblyLoaded(Assembly assembly)
		{
			if ((object)assembly == null)
				throw new ArgumentNullException(nameof(assembly));

			if ((object)this.AssemblyLoaded != null)
				this.AssemblyLoaded(assembly);
		}

		protected void OnRuntimeTeardown()
		{
			if ((object)this.RuntimeTeardown != null)
				this.RuntimeTeardown(this);
		}

		#endregion
	}
}