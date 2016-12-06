/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

-- server
SELECT
	SERVERPROPERTY('ServerName') AS [ServerName],
	SERVERPROPERTY('MachineName') AS [MachineName],
	SERVERPROPERTY('InstanceName') AS [InstanceName],
	SERVERPROPERTY('ProductVersion') AS [ServerVersion],
	SERVERPROPERTY ('ProductLevel') AS [ServerLevel],
	SERVERPROPERTY ('Edition') AS [ServerEdition],
	DB_NAME() AS [DefaultDatabaseName]
