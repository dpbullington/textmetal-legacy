/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TextMetal.Middleware.Solder.Executive
{
	public interface IExecutableApplicationFascade : IDisposable
	{
		#region Methods/Operators

		// dpb 2021-11-25
		//IDictionary<string, IList<string>> ParseCommandLineArguments(string[] args);

		int ShowNestedExceptionsAndThrowBrickAtProcess(Exception e);

		// dpb 2021-11-25
		//bool TryParseCommandLineArgumentProperty(string arg, out string key, out string value);

		#endregion
	}

	public interface IAsyncExecutableApplicationFascade : IExecutableApplicationFascade
	{
		#region Methods/Operators

		Task<int> EntryPointAsync(string[] args);

		#endregion
	}

	public interface ISyncExecutableApplicationFascade : IExecutableApplicationFascade
	{
		#region Methods/Operators

		int EntryPoint(string[] args);

		#endregion
	}
}