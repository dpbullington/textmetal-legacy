/*
	Copyright ©2002-2014 Daniel Bullington
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using TextMetal.Common.Data.TypeMap.LowLevel;

namespace TextMetal.Common.Data.TypeMap.HighLevel
{
	public interface IExpressionVisitor
	{
		#region Properties/Indexers/Events

		string SqlPredicate
		{
			get;
		}

		Parameter[] SqlPredicateParameters
		{
			get;
		}

		string SqlRestriction
		{
			get;
		}

		string SqlSort
		{
			get;
		}

		#endregion

		#region Methods/Operators

		void GoVisit(Query query, TableMappingAttribute tableMappingAttribute, ColumnMappingAttribute[] columnMappingAttributes, int tableIndex, int parameterIndex);

		#endregion
	}
}