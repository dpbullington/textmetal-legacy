/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

-- foreign key columns (refs)[schema, foreign_key]
-- DECLARE @SchemaName [nvarchar](255); SET @SchemaName = 'application'; DECLARE @ForeignKeyName [nvarchar](255); SET @ForeignKeyName = 'fk_Organization_User_creation';
SELECT
	sys_fkc.[constraint_column_id] AS [ForeignKeyColumnOrdinal],
	
	sys_s.[name] AS [SchemaName],
	sys_t.[name] AS [TableName],
	sys_fk.[name] AS [ForeignKeyName],
	sys_fkc.[parent_column_id] AS [ColumnOrdinal],
	sys_c.[name] AS [ColumnName],
		
	sys_s_p.[name] AS [TargetSchemaName],
	sys_t_p.[name] AS [TargetTableName],
	sys_fkc.[referenced_column_id] AS [TargetColumnOrdinal],
	sys_c_p.[name] AS [TargetColumnName]
FROM
	[sys].[foreign_key_columns] sys_fkc
	INNER JOIN [sys].[foreign_keys] sys_fk on sys_fk.[object_id] = sys_fkc.[constraint_object_id]
	INNER JOIN [sys].[tables] sys_t ON sys_t.[object_id] = sys_fk.[parent_object_id]
	-- NO NEED TO JOIN ON [sys].[objects] sys_o
	INNER JOIN [sys].[schemas] sys_s ON sys_s.[schema_id] = sys_t.[schema_id]
	INNER JOIN [sys].[columns] sys_c ON sys_c.[object_id] = sys_t.[object_id]
		AND sys_c.[column_id] = sys_fkc.[parent_column_id]

	INNER JOIN [sys].[tables] sys_t_p ON sys_t_p.[object_id] = sys_fk.[referenced_object_id]
	-- NO NEED TO JOIN ON [sys].[objects] sys_o_p
	INNER JOIN [sys].[schemas] sys_s_p ON sys_s_p.[schema_id] = sys_t_p.[schema_id]
	INNER JOIN [sys].[columns] sys_c_p ON sys_c_p.[object_id] = sys_t_p.[object_id]
		AND sys_c_p.[column_id] = sys_fkc.[referenced_column_id]
WHERE
	sys_s.[name] = @SchemaName
	AND sys_fk.[name] = @ForeignKeyName
ORDER BY
	sys_fkc.[constraint_column_id] ASC