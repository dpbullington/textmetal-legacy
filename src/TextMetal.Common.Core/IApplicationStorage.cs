/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Common.Solder.AmbientExecutionContext;

namespace TextMetal.Common.Core
{
	public interface IApplicationStorage : IExecutionPathStorage
	{
		#region Properties/Indexers/Events

		bool IsSafeCrossPrincipal
		{
			get;
		}

		#endregion
	}
}