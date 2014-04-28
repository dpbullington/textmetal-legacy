/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

-- database
SELECT
	sys_d.[database_id] as [DatabaseId],
	sys_d.[name] as [DatabaseName],
	*
FROM
	[sys].[databases] sys_d