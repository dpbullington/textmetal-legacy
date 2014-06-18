/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

-- views[schema]
-- DECLARE @SchemaName [nvarchar](255); SET @SchemaName = 'dbo';
SELECT
	sys_v.[object_id] AS [ViewId],
	sys_s.[name] AS [SchemaName],
    sys_v.[name] AS [ViewName],
	sys_v.[create_date] AS [CreationTimestamp],
	sys_v.[modify_date] AS [ModificationTimestamp],
	sys_v.[is_ms_shipped] AS [IsImplementationDetail]
FROM
    [sys].[views] sys_v
	-- NO NEED TO JOIN ON [sys].[objects] sys_o
	INNER JOIN [sys].[schemas] sys_s ON sys_s.[schema_id] = sys_v.[schema_id]
WHERE
	sys_s.[name] = ?
ORDER BY
	sys_v.[name] ASC
