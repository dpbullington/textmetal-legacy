/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.HostImpl.Web.Email
{
	public interface IEmailHost
	{
		#region Methods/Operators

		void Host(bool strictMatching, MessageTemplate messageTemplate, object modelObject, IHostEmailMessage hostEmailMessage);

		#endregion
	}
}