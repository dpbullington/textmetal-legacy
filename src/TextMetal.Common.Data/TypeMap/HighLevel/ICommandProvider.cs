using System;

using TextMetal.Common.Data.TypeMap.LowLevel;

namespace TextMetal.Common.Data.TypeMap.HighLevel
{
	public interface ICommandProvider
	{
		#region Methods/Operators

		Command GetDelete(TableMappingAttribute tableMappingAttribute, ColumnMappingAttribute[] columnMappingAttributes);

		Command GetInsert(TableMappingAttribute tableMappingAttribute, ColumnMappingAttribute[] columnMappingAttributes);

		Command GetSelectAll(TableMappingAttribute tableMappingAttribute, ColumnMappingAttribute[] columnMappingAttributes);

		For[] GetSelectFors(TableMappingAttribute tableMappingAttribute, ColumnMappingAttribute[] columnMappingAttributes, Query query);

		Command GetSelectNot(TableMappingAttribute tableMappingAttribute, ColumnMappingAttribute[] columnMappingAttributes);

		Command GetSelectOne(TableMappingAttribute tableMappingAttribute, ColumnMappingAttribute[] columnMappingAttributes);

		Command GetUpdate(TableMappingAttribute tableMappingAttribute, ColumnMappingAttribute[] columnMappingAttributes);

		#endregion
	}
}