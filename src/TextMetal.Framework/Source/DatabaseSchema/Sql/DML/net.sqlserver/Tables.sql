/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

-- tables[schema]
-- DECLARE @SchemaName [nvarchar](255); SET @SchemaName = 'application';
SELECT
	sys_t.[object_id] AS [TableId],
	sys_s.[name] AS [SchemaName],
    sys_t.[name] AS [TableName],
	sys_t.[create_date] AS [CreationTimestamp],
	sys_t.[modify_date] AS [ModificationTimestamp],
	sys_t.[is_ms_shipped] AS [IsImplementationDetail],
	sys_kc.[object_id] AS [PrimaryKeyId],
	sys_kc.[name] AS [PrimaryKeyName],
	sys_kc.[is_system_named] AS [PrimaryKeyIsSystemNamed]
FROM
    [sys].[tables] sys_t
	-- NO NEED TO JOIN ON [sys].[objects] sys_o
	INNER JOIN [sys].[schemas] sys_s ON sys_s.[schema_id] = sys_t.[schema_id]
	LEFT OUTER JOIN [sys].[key_constraints] sys_kc ON sys_kc.[parent_object_id] = sys_t.[object_id]
		AND sys_kc.[type] = 'PK'
WHERE
	sys_s.[name] = @SchemaName
