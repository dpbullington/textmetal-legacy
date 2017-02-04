/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Reflection;

namespace TextMetal.Middleware.Solder.Abstractions
{
	public interface IRuntimeAdapter : IDisposable
	{
		#region Properties/Indexers/Events

		event Action<Assembly> AssemblyLoaded;

		event Action<IRuntimeAdapter> RuntimeTeardown;

		#endregion

		#region Methods/Operators

		IEnumerable<Assembly> GetLoadedAssemblies();

		Assembly LoadAssembly(AssemblyName assemblyName);

		#endregion
	}
}