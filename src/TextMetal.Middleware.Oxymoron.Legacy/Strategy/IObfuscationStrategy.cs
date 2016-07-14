/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using TextMetal.Middleware.Oxymoron.Legacy.Config;
using TextMetal.Middleware.Solder.Primitives;

namespace TextMetal.Middleware.Oxymoron.Legacy.Strategy
{
	public interface IObfuscationStrategy
	{
		#region Methods/Operators

		object GetObfuscatedValue(IOxymoronEngine oxymoronEngine, ColumnConfiguration columnConfiguration, IColumn column, object columnValue);

		Type GetObfuscationStrategySpecificConfigurationType();

		IEnumerable<Message> ValidateObfuscationStrategySpecificConfiguration(ColumnConfiguration columnConfiguration, int? columnIndex);

		#endregion
	}
}