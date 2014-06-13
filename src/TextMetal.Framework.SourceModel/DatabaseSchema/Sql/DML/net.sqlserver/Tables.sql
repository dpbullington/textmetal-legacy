/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

-- tables[schema]
-- DECLARE @SchemaName [nvarchar](255); SET @SchemaName = 'dbo';
SELECT
	sys_o.[object_id] AS [TableId],
	sys_s.[name] AS [SchemaName],
    sys_o.[name] AS [TableName],
	sys_o.[create_date] AS [CreationTimestamp],
	sys_o.[modify_date] AS [ModificationTimestamp],
	sys_o.[is_ms_shipped] AS [IsImplementationDetail],
	sys_kc.[object_id] AS [PrimaryKeyId],
	sys_kc.[name] AS [PrimaryKeyName]
FROM
    [sys].[tables] sys_t
	INNER JOIN [sys].[objects] sys_o ON sys_o.[object_id] = sys_t.[object_id]
	INNER JOIN [sys].[schemas] sys_s ON sys_s.[schema_id] = sys_t.[schema_id]
	LEFT OUTER JOIN [sys].[key_constraints] sys_kc ON -- ZERO OR ONE
		sys_kc.[parent_object_id] = sys_o.[object_id]
		AND sys_kc.[type] = 'PK'
WHERE
	sys_s.[name] = @SchemaName