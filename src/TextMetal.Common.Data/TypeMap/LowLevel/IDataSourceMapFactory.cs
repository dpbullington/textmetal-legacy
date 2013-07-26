/*
	Copyright ©2002-2013 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Common.Data.TypeMap.LowLevel
{
	public interface IDataSourceMapFactory
	{
		#region Methods/Operators

		DataSourceMap GetMap<TPlainObject>(object obj) where TPlainObject : class, IPlainObject, new();

		DataSourceMap GetMap(Type targetType, object obj);

		#endregion
	}
}