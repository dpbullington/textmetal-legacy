/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

namespace TextMetal.Common.Data.Framework.PoPimp.Strategy
{
	public interface IDataSourceTagStrategyFactory
	{
		#region Methods/Operators

		IDataSourceTagStrategy GetDataSourceTagStrategy(string dataSourceTag);

		#endregion
	}
}