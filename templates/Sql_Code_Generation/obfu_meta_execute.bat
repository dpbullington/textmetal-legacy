@echo off

set TEXTMETAL_EXE=..\..\src\TextMetal.HostImpl.ConsoleTool\bin\Debug\TextMetal.exe

set ADO_NET_CONNECTION_STRING=Server=(local);User ID=obfu_meta_mssql_dev_login;Password=LrJGmP6UfW8TEp7x3wWhECUYULE6zzMcWQ03R6UxeB4xzVmnq5S4Lx0vApegZVH;Database=obfu_meta_ods_dev

set OUTPUT_FILE_PREFIX=Ox_
set OBFUSCATION_CONFIGURATION_DATABASE_NAME=Ox_Config

echo *** obfu_meta_execute ***
"%TEXTMETAL_EXE%" ^
	-templatefile:"obfu_meta_template.xml" ^
	-sourcefile:"obfu_meta_data_source.xml" ^
	-basedir:".\ASU_output\.obfu_meta\config" ^
	-sourcestrategy:"TextMetal.Framework.SourceModel.Primative.SqlDataSourceStrategy, TextMetal.Framework.SourceModel" ^
	-strict:"true" ^
	-debug:"false" ^
	-property:"OutputFilePrefix=%OUTPUT_FILE_PREFIX%" ^
	-property:"ObfuscationConfigurationDatabaseName=%OBFUSCATION_CONFIGURATION_DATABASE_NAME%" ^
	-property:"ConnectionType=System.Data.SqlClient.SqlConnection, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" ^
	-property:"ConnectionString=%ADO_NET_CONNECTION_STRING%"
IF %ERRORLEVEL% NEQ 0 goto pkgError

	
goto pkgSuccess


:pkgError
echo An error occured.
pause > nul
goto :eof

:pkgSuccess
echo Completed successfully.
pause > nul
goto :eof
