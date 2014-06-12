/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

-- DML triggers[schema, table]
-- DECLARE @SchemaName [nvarchar](255); SET @SchemaName = 'global'; DECLARE @TableName [nvarchar](255); SET @TableName = 'User';
SELECT
	sys_o.[object_id] AS [TriggerId],
	sys_s.[name] as [SchemaName],
	sys_o_p.[name] AS [TableName],
	sys_s_p.[name] as [_SchemaName],
	sys_o.[name] AS [TriggerName],	
	CAST(CASE WHEN sys_tr.[type] = 'TA' THEN 1
		ELSE 0 END AS [bit]) AS [IsClrTrigger],
	sys_tr.[is_disabled] AS [IsTriggerDisabled],
	sys_tr.[is_not_for_replication] AS [IsTriggerNotForReplication],
	sys_tr.[is_instead_of_trigger] AS [IsInsteadOfTrigger]
FROM
	[sys].[triggers] sys_tr
	INNER JOIN [sys].[objects] sys_o ON sys_o.[object_id] = sys_tr.[object_id]
	INNER JOIN [sys].[schemas] sys_s ON sys_s.[schema_id] = sys_o.[schema_id]
	-- PARENT JOINS
	INNER JOIN [sys].[tables] sys_t_p ON sys_t_p.[object_id] = sys_tr.[parent_id]
	INNER JOIN [sys].[objects] sys_o_p ON sys_o_p.[object_id] = sys_tr.[parent_id]
	INNER JOIN [sys].[schemas] sys_s_p ON sys_s_p.[schema_id] = sys_o_p.[schema_id]
WHERE
	sys_tr.[parent_class] = 1
	AND sys_s_p.[name] = @SchemaName
	AND sys_o_p.[name] = @TableName
ORDER BY
	sys_tr.[name] ASC