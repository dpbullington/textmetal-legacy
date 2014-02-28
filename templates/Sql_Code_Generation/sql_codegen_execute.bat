@echo off

REM
REM	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
REM	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
REM

set PACKAGE_DIR=.\output
set PACKAGE_DIR_EXISTS=%PACKAGE_DIR%\nul

set HISTORY_SCHEMA_NAME=history
set ADO_NET_CONNECTION_STRING=Data Source=(local);User ID=TextMetalWebHostSampleLogin;Password=LrJGmP6UfW8TEp7x3wWhECUYULE6zzMcWQ03R6UxeB4xzVmnq5S4Lx0vApegZVH;Initial Catalog=TextMetalWebHostSample
set DATA_OBFUSCATION_PROXY_CATALOG_NAME=TextMetalWebHostSample_Proxy
set DATA_OBFUSCATION_TARGET_SERVER_NAME=FRAMEWORK

:pkgDir

IF NOT EXIST %PACKAGE_DIR_EXISTS% GOTO noPkgDir
goto delPkgDir

:noPkgDir
@echo Creating output directory...
mkdir "%PACKAGE_DIR%"
IF %ERRORLEVEL% NEQ 0 goto pkgError
goto pkgBuild

:delPkgDir
@echo Cleaning output directory...
del "%PACKAGE_DIR%\*.*" /Q /S
rem IF %ERRORLEVEL% NEQ 0 goto pkgError
goto pkgBuild


:pkgBuild

echo *** sql_codegen_execute ***
"..\..\src\TextMetal.HostImpl.ConsoleTool\bin\Debug\TextMetal.exe" ^
	-templatefile:"master_template.xml" ^
	-sourcefile:"%ADO_NET_CONNECTION_STRING%" ^
	-basedir:"%PACKAGE_DIR%" ^
	-sourcestrategy:"TextMetal.Framework.SourceModel.DatabaseSchema.Sql.SqlSchemaSourceStrategy, TextMetal.Framework.SourceModel" ^
	-strict:"true" ^
	-property:"HistorySchemaName=%HISTORY_SCHEMA_NAME%" ^
	-property:"DataObfuscationProxyCatalogName=%DATA_OBFUSCATION_PROXY_CATALOG_NAME%" ^
	-property:"DataObfuscationTargetServerName=%DATA_OBFUSCATION_TARGET_SERVER_NAME%" ^
	-property:"ConnectionType=System.Data.SqlClient.SqlConnection, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" ^
	-property:"DataSourceTag=net.sqlserver"
IF %ERRORLEVEL% NEQ 0 goto pkgError


goto pkgSuccess


:pkgError
echo An error occured.
pause > nul
goto :eof

:pkgSuccess
echo Completed successfully.
goto :eof
