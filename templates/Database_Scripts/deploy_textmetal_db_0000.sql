/*
	Copyright �2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/


SET NOCOUNT ON
GO


PRINT '$(VAR_DB_DATABASE_ODS) database setup...'
GO


--USE master
--GO

IF EXISTS (SELECT sys_d.[database_id] FROM [sys].[databases] sys_d WHERE sys_d.[name] = '$(VAR_DB_DATABASE_ODS)')
BEGIN

	PRINT 'Database [$(VAR_DB_DATABASE_ODS)] exists...'

	PRINT 'Setting single user access on database [$(VAR_DB_DATABASE_ODS)]...'

	ALTER DATABASE [$(VAR_DB_DATABASE_ODS)] SET SINGLE_USER WITH ROLLBACK IMMEDIATE

	PRINT 'Dropping database [$(VAR_DB_DATABASE_ODS)]...'

	DROP DATABASE [$(VAR_DB_DATABASE_ODS)]

END
GO


IF EXISTS (SELECT sys_spl.[principal_id] FROM [sys].[server_principals] sys_spl WHERE sys_spl.[name] = N'$(VAR_DB_LOGIN_ODS)')
BEGIN

	PRINT 'Login [$(VAR_DB_LOGIN_ODS)] exists...'

	PRINT 'Dropping login [$(VAR_DB_LOGIN_ODS)]...'

	DROP LOGIN [$(VAR_DB_LOGIN_ODS)]

END
GO


PRINT 'Creating database [$(VAR_DB_DATABASE_ODS)]...'
GO


CREATE DATABASE [$(VAR_DB_DATABASE_ODS)]
GO


PRINT N'Creating login [$(VAR_DB_LOGIN_ODS)]...'
GO


CREATE LOGIN [$(VAR_DB_LOGIN_ODS)] WITH PASSWORD = '$(VAR_DB_PASSWORD_ODS)', CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF, DEFAULT_LANGUAGE=[us_english]
GO


PRINT N'Granting connect to login [$(VAR_DB_LOGIN_ODS)]...'
GO


GRANT CONNECT SQL TO [$(VAR_DB_LOGIN_ODS)]
GO


USE [$(VAR_DB_DATABASE_ODS)]
GO


PRINT N'Creating user [$(VAR_DB_USER_ODS)] in database...'
GO


CREATE USER [$(VAR_DB_USER_ODS)] FOR LOGIN [$(VAR_DB_LOGIN_ODS)] WITH DEFAULT_SCHEMA=[dbo]
GO


EXEC sp_addrolemember 'db_owner', '$(VAR_DB_USER_ODS)'
GO


PRINT N'Altering login [$(VAR_DB_LOGIN_ODS)]...'
GO


ALTER LOGIN [$(VAR_DB_LOGIN_ODS)] WITH DEFAULT_DATABASE=[$(VAR_DB_DATABASE_ODS)]
GO


PRINT '$(VAR_DB_DATABASE_ODS) database complete.'
GO
