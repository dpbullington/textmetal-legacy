/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

-- extended properties for database
SELECT
	sys_ep.[name] AS [PropertyName],
	sys_ep.[value] AS [PropertyValue]
FROM
    [sys].[extended_properties] sys_ep
WHERE
	@ServerName IS NOT NULL
	AND @DatabaseName IS NOT NULL
	AND sys_ep.[class] = 0 -- DATABASE
	AND sys_ep.[major_id] = 0
	AND sys_ep.[minor_id] = 0
ORDER BY
	sys_ep.[name] ASC