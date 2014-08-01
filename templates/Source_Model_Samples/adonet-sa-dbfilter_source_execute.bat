@echo off

REM
REM	Copyright ?2002-2014 Daniel Bullington (dpbullington@gmail.com)
REM	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
REM

set TEXTMETAL_EXE=..\..\src\TextMetal.HostImpl.ConsoleTool\bin\Debug\TextMetal.exe
set ADO_NET_CONNECTION_STRING=Server=(local);User ID=sa;Password=???


echo *** adonet_source_execute ***
"%TEXTMETAL_EXE%" ^
	-templatefile:"empty_template.xml" ^
	-sourcefile:"%ADO_NET_CONNECTION_STRING%" ^
	-basedir:".\output" ^
	-sourcestrategy:"TextMetal.Framework.SourceModel.DatabaseSchema.Sql.SqlSchemaSourceStrategy, TextMetal.Framework.SourceModel" ^
	-strict:"true" ^
	-property:"EnableDatabaseFilter=true" ^
	-property:"DatabaseFilter=???" ^
	-property:"DatabaseFilter=???" ^
	-property:"DatabaseFilter=???" ^
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
