/*
	Copyright ©2002-2014 Daniel Bullington
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using TextMetal.Common.Data.TypeMap.LowLevel;
using TextMetal.Common.Data.TypeMap.Objects;

namespace TextMetal.Common.Data.TypeMap.HighLevel
{
	public class AttributeReflectionDataSourceMapFactory : IDataSourceMapFactory
	{
		#region Constructors/Destructors

		public AttributeReflectionDataSourceMapFactory(ICommandProvider commandProvider)
		{
			if ((object)commandProvider == null)
				throw new ArgumentNullException("commandProvider");

			this.commandProvider = commandProvider;
		}

		#endregion

		#region Fields/Constants

		private readonly ICommandProvider commandProvider;

		#endregion

		#region Methods/Operators

		public virtual DataSourceMap GetMap<TPlainObject>(IObject @object) where TPlainObject : class, IPlainObject, new()
		{
			Type targetType;

			targetType = typeof(TPlainObject);

			return this.GetMap(targetType, @object);
		}

		public virtual DataSourceMap GetMap(Type targetType, IObject @object)
		{
			return this.GetMapOverQuery(targetType, @object as Query);
		}

		protected virtual DataSourceMap GetMapOverQuery(Type targetType, Query query)
		{
			DataSourceMap dataSourceMap, assemblyResourceDataSourceMap;

			SuppressMappingAttribute suppressMappingAttribute;
			TableMappingAttribute tableMappingAttribute;
			List<ColumnMappingAttribute> columnMappingAttributes;
			ColumnMappingAttribute[] columnMappingAttributeList;

			PropertyInfo[] propertyInfos;
			ColumnMappingAttribute columnMappingAttribute;

			if ((object)targetType == null)
				throw new ArgumentNullException("targetType");

			propertyInfos = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

			suppressMappingAttribute = Reflexion.GetOneAttribute<SuppressMappingAttribute>(targetType);

			if ((object)suppressMappingAttribute != null)
				throw new InvalidOperationException(string.Format("The type '{0}' specifies the '{1}' attribute.", targetType.FullName, typeof(SuppressMappingAttribute).FullName));

			tableMappingAttribute = Reflexion.GetOneAttribute<TableMappingAttribute>(targetType);

			if ((object)tableMappingAttribute == null)
				throw new InvalidOperationException(string.Format("The type '{0}' does not specify the '{1}' attribute.", targetType.FullName, typeof(TableMappingAttribute).FullName));

			tableMappingAttribute.TargetType = targetType;

			if ((object)propertyInfos == null)
				throw new InvalidOperationException(string.Format("The table mapped type '{0}' does not specify any public properties.", targetType.FullName));

			columnMappingAttributes = new List<ColumnMappingAttribute>();

			foreach (PropertyInfo propertyInfo in propertyInfos)
			{
				if (!propertyInfo.CanRead || !propertyInfo.CanWrite)
					continue;

				suppressMappingAttribute = Reflexion.GetOneAttribute<SuppressMappingAttribute>(propertyInfo);

				if ((object)suppressMappingAttribute != null)
					continue;

				columnMappingAttribute = Reflexion.GetOneAttribute<ColumnMappingAttribute>(propertyInfo);

				if ((object)columnMappingAttribute == null)
					continue;

				columnMappingAttribute.TargetProperty = propertyInfo;
				columnMappingAttributes.Add(columnMappingAttribute);
			}

			// must have a primary key
			if (columnMappingAttributes.Count(x => x.IsPrimaryKey) < 1)
				throw new InvalidOperationException(string.Format("The table mapped type '{0}' does not specify the '{1}' attribute for at least one column mapped property with IsPrimaryKey=true.", targetType.FullName, typeof(ColumnMappingAttribute).FullName));

			columnMappingAttributeList = columnMappingAttributes.ToArray();

			dataSourceMap = new DataSourceMap();
			dataSourceMap.Insert = this.commandProvider.GetInsert(tableMappingAttribute, columnMappingAttributeList);
			dataSourceMap.Update = this.commandProvider.GetUpdate(tableMappingAttribute, columnMappingAttributeList);
			dataSourceMap.Delete = this.commandProvider.GetDelete(tableMappingAttribute, columnMappingAttributeList);
			dataSourceMap.SelectAll = this.commandProvider.GetSelectAll(tableMappingAttribute, columnMappingAttributeList);
			dataSourceMap.SelectOne = this.commandProvider.GetSelectOne(tableMappingAttribute, columnMappingAttributeList);
			dataSourceMap.SelectFors.AddRange(this.commandProvider.GetSelectFors(tableMappingAttribute, columnMappingAttributeList, query));

			// fallaback to assembly manifest resource map...
			if (AssemblyResourceDataSourceMapFactory.TryGetMapFromType(targetType, out assemblyResourceDataSourceMap))
			{
				if ((object)assemblyResourceDataSourceMap != null &&
				    (object)assemblyResourceDataSourceMap.SelectFors != null)
					dataSourceMap.SelectFors.AddRange(assemblyResourceDataSourceMap.SelectFors);
			}

			return dataSourceMap;
		}

		#endregion
	}
}