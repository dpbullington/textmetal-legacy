/*
	Copyright ©2002-2013 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using TextMetal.Common.Core;

namespace TextMetal.HostImpl.AspNetSample.Objects.Model.Tables
{
	public partial class EventLog
	{
		#region Methods/Operators

		public void Mark()
		{
			DateTime now;

			now = DateTime.Now;

			this.CreationTimestamp = this.CreationTimestamp ?? now;
			this.ModificationTimestamp = !this.IsNew ? now : this.CreationTimestamp;
			this.LogicalDelete = this.LogicalDelete ?? false;
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