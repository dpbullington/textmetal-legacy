/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using TextMetal.Middleware.Oxymoron.Legacy.Config;
using TextMetal.Middleware.Solder.Primitives;

namespace TextMetal.Middleware.Oxymoron.Legacy.Adapter
{
	public interface IAdapter : IDisposable
	{
		#region Methods/Operators

		Type GetAdapterSpecificConfigurationType();

		void Initialize(AdapterConfiguration adapterConfiguration);

		void Terminate();

		IEnumerable<Message> ValidateAdapterSpecificConfiguration(AdapterConfiguration adapterConfiguration, string adapterContext);

		#endregion
	}
}