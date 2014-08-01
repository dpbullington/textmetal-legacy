/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

-- procedures[schema]
-- DECLARE @SchemaName [nvarchar](255); SET @SchemaName = 'testcases';
SELECT
	sys_p.[object_id] AS [ProcedureId],
	sys_s.[name] AS [SchemaName],
	sys_p.[name] AS [ProcedureName]
FROM
    [sys].[procedures] sys_p
	-- NO NEED TO JOIN ON [sys].[objects] sys_o
	INNER JOIN [sys].[schemas] sys_s ON sys_s.[schema_id] = sys_p.[schema_id]
WHERE
	sys_s.[name] = ?
ORDER BY
	sys_p.[name] ASC
