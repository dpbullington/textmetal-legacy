/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System.Collections.Generic;

using TextMetal.Middleware.Solder.Primitives;

namespace TextMetal.Middleware.Oxymoron.Legacy.Config
{
	public interface IConfigurationObject
	{
		#region Properties/Indexers/Events

		IConfigurationObject Parent
		{
			get;
			set;
		}

		IConfigurationObjectCollection Surround
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