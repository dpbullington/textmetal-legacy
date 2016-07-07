/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using TextMetal.Middleware.Oxymoron.Legacy.Config;
using TextMetal.Middleware.Solder.Primitives;

namespace TextMetal.Middleware.Oxymoron.Legacy.Hosting.Tool
{
	public interface IToolHost : IOxymoronHost
	{
		#region Methods/Operators

		void Host(string sourceFilePath);

		void Host(ObfuscationConfiguration obfuscationConfiguration, Action<string, long, bool, double> statusCallback);

		bool TryGetUpstreamMetadata(ObfuscationConfiguration obfuscationConfiguration, out IEnumerable<IColumn> metaColumns);

		#endregion
	}
}