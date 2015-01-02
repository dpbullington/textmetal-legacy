/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

-- unique keys[schema, table]
-- DECLARE @SchemaName [nvarchar](255); SET @SchemaName = 'application'; DECLARE @TableName [nvarchar](255); SET @TableName = 'Organization';
SELECT
	sys_kc.[object_id] AS [UniqueKeyId],
	sys_s.[name] AS [SchemaName],
	sys_t.[name] AS [TableName],
	sys_kc.[name] AS [UniqueKeyName],
	sys_kc.[is_system_named] AS [UniqueKeyIsSystemNamed]
FROM
	[sys].[key_constraints] AS sys_kc -- UKs
	INNER JOIN [sys].[tables] sys_t ON sys_t.[object_id] = sys_kc.[parent_object_id]
	-- NO NEED TO JOIN ON [sys].[objects] sys_o
	INNER JOIN [sys].[schemas] sys_s ON sys_s.[schema_id] = sys_t.[schema_id]
WHERE
	sys_s.[name] = ?
	AND sys_t.[name] = ?
	AND sys_kc.[type] = 'UQ' -- UNIQUE KEY CONSTRAINT
ORDER BY
	sys_kc.[name] ASC
