/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

-- schemas
SELECT
	sys_s.[schema_id] as [SchemaId],
	sys_s.[principal_id] as [OwnerId],
    sys_s.name as [SchemaName]	
FROM
    [sys].[schemas] sys_s
ORDER BY
	sys_s.[name] ASC