/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

-- views[schema]
-- DECLARE @SchemaName [nvarchar](255); SET @SchemaName = 'dbo';
SELECT
	sys_o.[object_id] AS [ViewId],
	sys_s.[name] AS [SchemaName],
    sys_o.[name] AS [ViewName],
	sys_o.[create_date] AS [CreationTimestamp],
	sys_o.[modify_date] AS [ModificationTimestamp],
	sys_o.[is_ms_shipped] AS [IsImplementationDetail]
FROM
    [sys].[views] sys_v
	INNER JOIN [sys].[objects] sys_o ON sys_o.[object_id] = sys_v.[object_id]
	INNER JOIN [sys].[schemas] sys_s ON sys_s.[schema_id] = sys_v.[schema_id]
WHERE
	sys_s.[name] = @SchemaName
ORDER BY
	sys_o.[name] ASC
