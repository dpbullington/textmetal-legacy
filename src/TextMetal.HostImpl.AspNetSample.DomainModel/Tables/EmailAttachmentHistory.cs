/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using TextMetal.Common.Core;

namespace TextMetal.HostImpl.AspNetSample.DomainModel.Tables
{
	public partial class EmailAttachmentHistory
	{
		#region Methods/Operators

		partial void OnMark()
		{
		}

		partial void OnValidate(ref IEnumerable<Message> messages)
		{
			messages = null;
		}

		#endregion
	}
}