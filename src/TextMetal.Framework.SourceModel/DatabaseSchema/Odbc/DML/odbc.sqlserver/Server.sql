/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

-- server
SELECT
	CAST(NULL AS [int]) as [ObjectId],
	SERVERPROPERTY('MachineName') AS [MachineName],
	SERVERPROPERTY('InstanceName') AS [InstanceName],
	SERVERPROPERTY('productversion') AS [ServerVersion],
	SERVERPROPERTY ('productlevel') AS [ServerLevel],
	SERVERPROPERTY ('edition') AS [ServerEdition],
	DB_NAME() AS [DefaultDatabaseName]
