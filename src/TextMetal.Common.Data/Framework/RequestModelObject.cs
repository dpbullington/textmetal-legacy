/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using TextMetal.Common.Core;

namespace TextMetal.Common.Data.Framework
{
	public abstract class RequestModelObject : BasicObject, IRequestModelObject
	{
		#region Constructors/Destructors

		protected RequestModelObject()
		{
		}

		#endregion

		#region Methods/Operators

		public virtual IEnumerable<Message> Validate()
		{
			List<Message> messages;

			messages = new List<Message>();

			return messages.ToArray();
		}

		#endregion
	}
}