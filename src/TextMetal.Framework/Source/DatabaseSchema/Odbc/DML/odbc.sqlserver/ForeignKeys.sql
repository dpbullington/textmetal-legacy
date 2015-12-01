/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

-- foreign keys[schema, table]
-- DECLARE @SchemaName [nvarchar](255); SET @SchemaName = 'application'; DECLARE @TableName [nvarchar](255); SET @TableName = 'Organization';
SELECT
	sys_fk.[object_id] AS [ForeignKeyId],
	sys_s.[name] AS [SchemaName],
	sys_t.[name] AS [TableName],
	sys_fk.[name] AS [ForeignKeyName],
	sys_s_p.[name] AS [TargetSchemaName],
	sys_t_p.[name] AS [TargetTableName],
	sys_fk.[is_system_named] AS [ForeignKeyIsSystemNamed],
	sys_fk.[is_disabled] AS [ForeignKeyIsDisabled],
	sys_fk.[is_not_for_replication] AS [ForeignKeyIsForReplication],
	sys_fk.[delete_referential_action] AS [ForeignKeyOnDeleteRefIntAction],
	sys_fk.[delete_referential_action_desc] AS [ForeignKeyOnDeleteRefIntActionSqlName],
	sys_fk.[update_referential_action] AS [ForeignKeyOnUpdateRefIntAction],
	sys_fk.[update_referential_action_desc] AS [ForeignKeyOnUpdateRefIntActionSqlName]
FROM
    [sys].[foreign_keys] sys_fk
	INNER JOIN [sys].[tables] sys_t ON sys_t.[object_id] = sys_fk.[parent_object_id]
	-- NO NEED TO JOIN ON [sys].[objects] sys_o
	INNER JOIN [sys].[schemas] sys_s ON sys_s.[schema_id] = sys_t.[schema_id]

	INNER JOIN [sys].[tables] sys_t_p ON sys_t_p.[object_id] = sys_fk.[referenced_object_id]
	-- NO NEED TO JOIN ON [sys].[objects] sys_o_p
	INNER JOIN [sys].[schemas] sys_s_p ON sys_s_p.[schema_id] = sys_t_p.[schema_id]
WHERE
	sys_s.[name] = ?
	AND sys_t.[name] = ?
ORDER BY
	sys_fk.[name] ASC
