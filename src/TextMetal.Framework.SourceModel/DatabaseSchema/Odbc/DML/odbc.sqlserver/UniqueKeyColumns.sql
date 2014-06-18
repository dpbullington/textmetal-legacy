/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

-- unique key columns (refs)[schema, unique_key]
-- DECLARE @SchemaName [nvarchar](255); SET @SchemaName = 'application'; DECLARE @UniqueKeyName [nvarchar](255); SET @UniqueKeyName = 'uk_Organization';
SELECT
	sys_ixc.[index_column_id] AS [UniqueKeyColumnOrdinal],
	sys_s.[name] AS [SchemaName],
	sys_t.[name] AS [TableName],
	sys_kc.[name] AS [UniqueKeyName],
	sys_ixc.[column_id] AS [ColumnOrdinal],
	sys_c.[name] AS [ColumnName],
	sys_ixc.[is_descending_key] AS [UniqueKeyColumnDescendingSort]
FROM
	[sys].[index_columns] AS sys_ixc
	INNER JOIN [sys].[key_constraints] AS sys_kc ON sys_kc.[unique_index_id] = sys_ixc.[index_id]
	INNER JOIN [sys].[tables] sys_t ON sys_t.[object_id] = sys_kc.[parent_object_id]
		AND sys_t.[object_id] = sys_ixc.[object_id]
	-- NO NEED TO JOIN ON [sys].[objects] sys_o
	INNER JOIN [sys].[schemas] sys_s ON sys_s.[schema_id] = sys_t.[schema_id]
	INNER JOIN [sys].[columns] sys_c ON sys_c.[object_id] = sys_t.[object_id]
		AND sys_c.[column_id] = sys_ixc.[column_id]
	INNER JOIN [sys].[indexes] AS sys_ix ON sys_ix.[object_id] = sys_ixc.[object_id]
		AND sys_ix.[index_id] = sys_ixc.[index_id]
WHERE
	sys_s.[name] = ?
	AND sys_kc.[name] = ?
	AND sys_kc.[type] = 'UQ' -- UNIQUE KEY CONSTRAINT
ORDER BY
	sys_ixc.[key_ordinal] ASC