/*
	Copyright ©2002-2013 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/


PRINT 'TextMetalWebHostSample database setup...'	
GO


USE master
GO


IF EXISTS (SELECT * FROM sysdatabases WHERE name = 'TextMetalWebHostSample')
BEGIN

	PRINT 'Database [TextMetalWebHostSample] exists...'	
	
	PRINT 'Setting single user access on database [TextMetalWebHostSample]...'

	ALTER DATABASE [TextMetalWebHostSample] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
	
	PRINT 'Dropping database [TextMetalWebHostSample]...'
	
	DROP DATABASE [TextMetalWebHostSample]
	
END
GO


IF EXISTS (SELECT * FROM master.dbo.syslogins WHERE loginname = N'TextMetalWebHostSampleLogin')
BEGIN

	PRINT 'Login [TextMetalWebHostSampleLogin] exists...'	
	
	PRINT 'Dropping login [TextMetalWebHostSampleLogin]...'
	
	DROP LOGIN [TextMetalWebHostSampleLogin]
	
END
GO


PRINT 'Creating database [TextMetalWebHostSample]...'
GO


CREATE DATABASE [TextMetalWebHostSample]
GO


PRINT N'Creating login [TextMetalWebHostSampleLogin]...'
GO

	
CREATE LOGIN [TextMetalWebHostSampleLogin] WITH PASSWORD = 'LrJGmP6UfW8TEp7x3wWhECUYULE6zzMcWQ03R6UxeB4xzVmnq5S4Lx0vApegZVH', CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF, DEFAULT_LANGUAGE=[us_english]
GO


PRINT N'Granting connect to login [TextMetalWebHostSampleLogin]...'
GO
	

GRANT CONNECT SQL TO [TextMetalWebHostSampleLogin]
GO


USE [TextMetalWebHostSample]
GO


PRINT N'Creating user in database...'
GO


CREATE USER [TextMetalWebHostSampleUser] FOR LOGIN [TextMetalWebHostSampleLogin] WITH DEFAULT_SCHEMA=[dbo]
GO


EXEC sp_addrolemember 'db_owner', 'TextMetalWebHostSampleUser'
GO


PRINT N'Altering login [TextMetalWebHostSampleLogin]...'
GO	


ALTER LOGIN [TextMetalWebHostSampleLogin] WITH DEFAULT_DATABASE=[TextMetalWebHostSample]
GO


PRINT 'TextMetalWebHostSample database complete.'	
GO