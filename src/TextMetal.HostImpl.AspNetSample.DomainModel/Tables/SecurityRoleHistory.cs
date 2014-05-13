/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Linq;

using TextMetal.Common.Core;

namespace TextMetal.HostImpl.AspNetSample.DomainModel.Tables
{
	public partial class SecurityRoleHistory
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
