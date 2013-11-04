/*
	Copyright ©2002-2013 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using TextMetal.Common.Core;

namespace TextMetal.HostImpl.AspNetSample.Objects.Model.Tables
{
	public partial class TableTypeTestHistory
	{		
		#region Methods/Operators

		public void Mark()
		{
		}

		public virtual Message[] Validate()
		{
			List<Message> messages;
			
			messages = new List<Message>();

			return messages.ToArray();
		}

		#endregion
	}
}
