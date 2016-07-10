/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Middleware.Oxymoron.Legacy.Config.Adapters;

namespace TextMetal.Middleware.Oxymoron.Legacy.Adapter.Destination
{
	public abstract class AdoNetDestinationAdapter : AdoNetDestinationAdapter<AdoNetAdapterConfiguration>
	{
		#region Constructors/Destructors

		protected AdoNetDestinationAdapter()
		{
		}

		#endregion
	}
}