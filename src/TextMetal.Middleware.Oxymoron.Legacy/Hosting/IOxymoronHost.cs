/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Middleware.Oxymoron.Legacy.Config;
using TextMetal.Middleware.Solder.Primitives;

namespace TextMetal.Middleware.Oxymoron.Legacy.Hosting
{
	public interface IOxymoronHost : IDisposable
	{
		#region Methods/Operators

		object GetValueForIdViaDictionaryResolution(DictionaryConfiguration dictionaryConfiguration, IColumn column, object surrogateId);

		#endregion
	}
}