/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Middleware.Oxymoron.Legacy.Config;
using TextMetal.Middleware.Oxymoron.Legacy.Config.Strategies;
using TextMetal.Middleware.Solder.Extensions;
using TextMetal.Middleware.Solder.Primitives;

namespace TextMetal.Middleware.Oxymoron.Legacy.Strategy
{
	/// <summary>
	/// Returns an alternate value that is always null if NULL or the default value if NOT NULL.
	/// DATA TYPE: any
	/// </summary>
	public sealed class DefaultingObfuscationStrategy : ObfuscationStrategy<DefaultingObfuscationStrategyConfiguration>
	{
		#region Constructors/Destructors

		public DefaultingObfuscationStrategy()
		{
		}

		#endregion

		#region Methods/Operators

		private static object GetDefault(bool isNullable, Type valueType)
		{
			if ((object)valueType == null)
				throw new ArgumentNullException(nameof(valueType));

			if (valueType == typeof(String))
				return isNullable ? null : string.Empty;

			if (isNullable)
				valueType = SolderLegacyInstanceAccessor.ReflectionFascadeLegacyInstance.MakeNullableType(valueType);

			return SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.DefaultValue(valueType);
		}

		protected override object CoreGetObfuscatedValue(IOxymoronEngine oxymoronEngine, ColumnConfiguration<DefaultingObfuscationStrategyConfiguration> columnConfiguration, IColumn metaColumn, object columnValue)
		{
			object value;

			if ((object)columnConfiguration == null)
				throw new ArgumentNullException(nameof(columnConfiguration));

			if ((object)metaColumn == null)
				throw new ArgumentNullException(nameof(metaColumn));

			if ((object)columnConfiguration.ObfuscationStrategySpecificConfiguration == null)
				throw new InvalidOperationException(string.Format("Configuration missing: '{0}'.", "ObfuscationStrategyConfiguration"));

			value = GetDefault(metaColumn.ColumnIsNullable ?? columnConfiguration.ObfuscationStrategySpecificConfiguration.DefaultingCanBeNull ?? false, metaColumn.ColumnType);

			return value;
		}

		#endregion
	}
}