/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace TextMetal.Common.Data.TypeMap.LowLevel
{
	public class AssemblyResourceDataSourceMapFactory : IDataSourceMapFactory
	{
		#region Constructors/Destructors

		public AssemblyResourceDataSourceMapFactory()
		{
		}

		#endregion

		#region Methods/Operators

		public virtual DataSourceMap GetMap<TPlainObject>(object obj) where TPlainObject : class, IPlainObject, new()
		{
			Type targetType;
			DataSourceMap dataSourceMap;

			targetType = typeof(TPlainObject);

			dataSourceMap = this.GetMap(targetType, obj);

			return dataSourceMap;
		}

		public virtual DataSourceMap GetMap(Type targetType, object obj)
		{
			DataSourceMap dataSourceMap;
			string resourceName;

			if ((object)targetType == null)
				throw new ArgumentNullException("targetType");

			resourceName = string.Format("{0}.xml", targetType.FullName);

			if (!Cerealization.Cerealization.TryGetFromAssemblyResource<DataSourceMap>(targetType, resourceName, out dataSourceMap))
				throw new InvalidOperationException(string.Format("Unable to deserialize instance of '{0}' from the manifest resource name '{1}' in the assembly '{2}'.", typeof(DataSourceMap).FullName, resourceName, targetType.Assembly.FullName));

			return dataSourceMap;
		}

		#endregion
	}
}