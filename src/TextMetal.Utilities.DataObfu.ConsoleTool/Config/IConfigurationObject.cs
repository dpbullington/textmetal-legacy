/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System.Collections.Generic;

using TextMetal.Common.Core;

namespace TextMetal.Utilities.DataObfu.ConsoleTool.Config
{
	public interface IConfigurationObject
	{
		#region Properties/Indexers/Events

		IConfigurationObject Parent
		{
			get;
			set;
		}

		#endregion

		#region Methods/Operators

		IEnumerable<Message> Validate();

		#endregion
	}
}