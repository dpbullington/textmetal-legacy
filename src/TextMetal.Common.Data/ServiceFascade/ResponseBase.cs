/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using TextMetal.Common.Core;

namespace TextMetal.Common.Data.ServiceFascade
{
	public abstract class ResponseBase
	{
		#region Constructors/Destructors

		protected ResponseBase()
		{
		}

		#endregion

		#region Fields/Constants

		private IEnumerable<Message> messages;

		#endregion

		#region Properties/Indexers/Events

		public IEnumerable<Message> Messages
		{
			get
			{
				return this.messages;
			}
			set
			{
				this.messages = value;
			}
		}

		#endregion
	}
}