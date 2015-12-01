/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Middleware.Solder.Utilities
{
	public interface IExecutableApplicationFascade : IDisposable
	{
		#region Properties/Indexers/Events

		AssemblyInformationFascade AssemblyInformationFascade
		{
			get;
		}

		bool HookUnhandledExceptionEvents
		{
			get;
		}

		#endregion

		#region Methods/Operators

		int EntryPoint(string[] args);

		void ShowNestedExceptionsAndThrowBrickAtProcess(Exception e);

		#endregion
	}
}