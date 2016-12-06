/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

namespace TextMetal.Middleware.Oxymoron.Legacy.Config
{
	public interface IConfigurationObjectCollection<TConfigurationObject> : IConfigurationObjectCollection
		where TConfigurationObject : IConfigurationObject
	{
	}
}