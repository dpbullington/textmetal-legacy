/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

namespace TextMetal.Common.Data.Framework.Strategy
{
	public interface IDataSourceTagStrategyFactory
	{
		#region Methods/Operators

		IDataSourceTagStrategy GetDataSourceTagStrategy(string dataSourceTag);

		#endregion
	}
}