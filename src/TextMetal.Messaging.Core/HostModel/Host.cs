/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Messaging.Core.HostModel
{
	public abstract class Host : IntegrationComponent, IHost
	{
		#region Constructors/Destructors

		protected Host()
		{
		}

		#endregion
	}
}