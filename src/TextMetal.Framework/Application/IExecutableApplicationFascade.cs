/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Middleware.Common.Utilities;

namespace TextMetal.Framework.Application
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