/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

namespace TextMetal.Middleware.Datazoid.Repositories.Impl.Strategies
{
	public interface IDataSourceTagStrategyFactory
	{
		#region Methods/Operators

		IDataSourceTagStrategy GetDataSourceTagStrategy(string dataSourceTag);

		#endregion
	}
}