/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using TextMetal.Middleware.Oxymoron.Legacy.Config;
using TextMetal.Middleware.Oxymoron.Legacy.Config.Strategies;
using TextMetal.Middleware.Solder.Primitives;

namespace TextMetal.Middleware.Oxymoron.Legacy.Strategy
{
	public abstract class ObfuscationStrategy<TObfuscationStrategyConfiguration> : IObfuscationStrategy
		where TObfuscationStrategyConfiguration : ObfuscationStrategyConfiguration, new()
	{
		#region Constructors/Destructors

		protected ObfuscationStrategy()
		{
		}

		#endregion

		#region Fields/Constants

		private const long DEFAULT_HASH_BUCKET_SIZE = long.MaxValue;

		#endregion

		#region Methods/Operators

		protected abstract object CoreGetObfuscatedValue(IOxymoronEngine oxymoronEngine, ColumnConfiguration<TObfuscationStrategyConfiguration> columnConfiguration, IColumn column, object columnValue);

		public object GetObfuscatedValue(IOxymoronEngine oxymoronEngine, ColumnConfiguration columnConfiguration, IColumn column, object columnValue)
		{
			ColumnConfiguration<TObfuscationStrategyConfiguration> _columnConfiguration;
			object value;

			if ((object)oxymoronEngine == null)
				throw new ArgumentNullException(nameof(oxymoronEngine));

			if ((object)columnConfiguration == null)
				throw new ArgumentNullException(nameof(columnConfiguration));

			if ((object)column == null)
				throw new ArgumentNullException(nameof(column));

			if ((object)columnValue == DBNull.Value)
				columnValue = null;

			if ((object)columnConfiguration.ObfuscationStrategySpecificConfiguration == null)
				throw new InvalidOperationException(string.Format("Configuration missing: '{0}'.", nameof(columnConfiguration.ObfuscationStrategySpecificConfiguration)));

			_columnConfiguration = new ColumnConfiguration<TObfuscationStrategyConfiguration>(columnConfiguration);

			if ((object)_columnConfiguration.ObfuscationStrategySpecificConfiguration == null)
				throw new InvalidOperationException(string.Format("Configuration missing: '{0}'.", nameof(_columnConfiguration.ObfuscationStrategySpecificConfiguration)));

			value = this.CoreGetObfuscatedValue(oxymoronEngine, _columnConfiguration, column, columnValue);

			return value;
		}

		public Type GetObfuscationStrategySpecificConfigurationType()
		{
			return typeof(TObfuscationStrategyConfiguration);
		}

		protected long GetSignHash(IOxymoronEngine oxymoronEngine, object value)
		{
			long hash;

			if ((object)oxymoronEngine == null)
				throw new ArgumentNullException(nameof(oxymoronEngine));

			hash = oxymoronEngine.GetBoundedHash(DEFAULT_HASH_BUCKET_SIZE, value);

			return hash;
		}

		protected long GetValueHash(IOxymoronEngine oxymoronEngine, long? size, object value)
		{
			long hash;

			if ((object)oxymoronEngine == null)
				throw new ArgumentNullException(nameof(oxymoronEngine));

			hash = oxymoronEngine.GetBoundedHash(size ?? DEFAULT_HASH_BUCKET_SIZE, value);

			return hash;
		}

		public IEnumerable<Message> ValidateObfuscationStrategySpecificConfiguration(ColumnConfiguration columnConfiguration, int? colummIndex)
		{
			ColumnConfiguration<TObfuscationStrategyConfiguration> _columnConfiguration;

			if ((object)columnConfiguration == null)
				throw new ArgumentNullException(nameof(columnConfiguration));

			if ((object)columnConfiguration.ObfuscationStrategySpecificConfiguration == null)
				throw new InvalidOperationException(string.Format("Configuration missing: '{0}'.", nameof(columnConfiguration.ObfuscationStrategySpecificConfiguration)));

			_columnConfiguration = new ColumnConfiguration<TObfuscationStrategyConfiguration>(columnConfiguration);

			if ((object)_columnConfiguration.ObfuscationStrategySpecificConfiguration == null)
				throw new InvalidOperationException(string.Format("Configuration missing: '{0}'.", nameof(_columnConfiguration.ObfuscationStrategySpecificConfiguration)));

			return _columnConfiguration.ObfuscationStrategySpecificConfiguration.Validate(colummIndex);
		}

		#endregion
	}
}