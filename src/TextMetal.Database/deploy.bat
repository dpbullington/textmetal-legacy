@echo off

REM
REM	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
REM	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
REM


"%SQLCMD_EXE%" ^
	-S "%DB_SERVER%" ^
	-U "%DB_SA_USERNAME%" ^
	-P "%DB_SA_PASSWORD%" ^
	-d "%DB_DATABASE_ODS%" ^
	-i ".\deploy_sql_server_db_0001.sql"
IF %ERRORLEVEL% NEQ 0 goto pkgError

GOTO :eof

:pkgError
SET ERRORLEVEL=9999