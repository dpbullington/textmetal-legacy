/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Common.Core;

namespace TextMetal.Common.Data.Framework.PoPimp.Strategy
{
	public sealed class DataSourceTagStrategyFactory : IDataSourceTagStrategyFactory
	{
		#region Constructors/Destructors

		private DataSourceTagStrategyFactory()
		{
		}

		#endregion

		#region Fields/Constants

		private static DataSourceTagStrategyFactory instance;

		#endregion

		#region Properties/Indexers/Events

		public static DataSourceTagStrategyFactory Instance
		{
			get
			{
				return instance;
			}
		}

		#endregion

		#region Methods/Operators

		public IDataSourceTagStrategy GetDataSourceTagStrategy(string dataSourceTag)
		{
			if (dataSourceTag.SafeToString().ToLower() == NetSqlServerDataSourceTagStrategy.Instance.DataSourceTag.SafeToString().ToLower())
				return NetSqlServerDataSourceTagStrategy.Instance;
			else if (dataSourceTag.SafeToString().ToLower() == NetSqliteDataSourceTagStrategy.Instance.DataSourceTag.SafeToString().ToLower())
				return NetSqliteDataSourceTagStrategy.Instance;
			else if (dataSourceTag.SafeToString().ToLower() == OdbcSqlServerDataSourceTagStrategy.Instance.DataSourceTag.SafeToString().ToLower())
				return OdbcSqlServerDataSourceTagStrategy.Instance;
			else
				return null;
		}

		#endregion
	}
}