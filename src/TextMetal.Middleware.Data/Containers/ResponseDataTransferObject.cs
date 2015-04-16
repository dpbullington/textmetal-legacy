/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using TextMetal.Middleware.Common.ValueObjects;

namespace TextMetal.Middleware.Data.Containers
{
	public abstract class ResponseDataTransferObject : IResponseDataTransferObject
	{
		#region Constructors/Destructors

		protected ResponseDataTransferObject()
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