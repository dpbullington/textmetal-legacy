@echo off

REM
REM	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
REM	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
REM

set SQLCMD_EXE=sqlcmd.exe

set DB_SERVER=(local)
set DB_DATABASE_MASTER=master
set DB_SA_USERNAME=sa
set DB_SA_PASSWORD=???

set DB_DATABASE_ODS=textmetal_ods_dev
set DB_LOGIN_ODS=textmetal_mssql_dev_login
set DB_PASSWORD_ODS=LrJGmP6UfW8TEp7x3wWhECUYULE6zzMcWQ03R6UxeB4xzVmnq5S4Lx0vApegZVH
set DB_USER_ODS=textmetal_mssql_dev_user


REM MUST BE ONLY FOR DEV
"%SQLCMD_EXE%" ^
	-S "%DB_SERVER%" ^
	-U "%DB_SA_USERNAME%" ^
	-P "%DB_SA_PASSWORD%" ^
	-d "%DB_DATABASE_MASTER%" ^
	-v VAR_DB_DATABASE_ODS="%DB_DATABASE_ODS%" ^
	-v VAR_DB_LOGIN_ODS="%DB_LOGIN_ODS%" ^
	-v VAR_DB_PASSWORD_ODS="%DB_PASSWORD_ODS%" ^
	-v VAR_DB_USER_ODS="%DB_USER_ODS%" ^
	-i ".\deploy_sql_server_db_0000.sql"
IF %ERRORLEVEL% NEQ 0 goto pkgError


CALL deploy.bat
IF %ERRORLEVEL% NEQ 0 goto pkgError


GOTO pkgSuccess


:pkgError
echo An error occured.
pause > nul
goto :eof

:pkgSuccess
echo Completed successfully.
pause > nul
goto :eof
