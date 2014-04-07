/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using TextMetal.Common.Core;

namespace TextMetal.HostImpl.AspNetSample.ServiceModel
{
	public abstract class ResponseBase
	{
		#region Constructors/Destructors

		protected ResponseBase()
		{
		}

		#endregion

		#region Properties/Indexers/Events

		public IEnumerable<Message> Messages
		{
			get;
			set;
		}

		#endregion
	}
}