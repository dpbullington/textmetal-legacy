/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Middleware.Oxymoron.Legacy.Config;
using TextMetal.Middleware.Oxymoron.Legacy.Config.Strategies;
using TextMetal.Middleware.Solder.Primitives;

namespace TextMetal.Middleware.Oxymoron.Legacy.Strategy
{
	/// <summary>
	/// Returns un-obfuscated, original value.
	/// DATA TYPE: any
	/// </summary>
	public sealed class NoneObfuscationStrategy : ObfuscationStrategy<ObfuscationStrategyConfiguration>
	{
		#region Constructors/Destructors

		public NoneObfuscationStrategy()
		{
		}

		#endregion

		#region Methods/Operators

		protected override object CoreGetObfuscatedValue(IOxymoronEngine oxymoronEngine, ColumnConfiguration<ObfuscationStrategyConfiguration> columnConfiguration, IColumn column, object columnValue)
		{
			if ((object)columnConfiguration == null)
				throw new ArgumentNullException(nameof(columnConfiguration));

			if ((object)column == null)
				throw new ArgumentNullException(nameof(column));

			if ((object)columnConfiguration.ObfuscationStrategySpecificConfiguration == null)
				throw new InvalidOperationException(string.Format("Configuration missing: '{0}'.", nameof(columnConfiguration.ObfuscationStrategySpecificConfiguration)));

			return columnValue;
		}

		#endregion
	}
}