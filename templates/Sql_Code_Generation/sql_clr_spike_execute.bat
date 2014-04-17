@echo off

REM
REM	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
REM	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
REM

set PACKAGE_DIR=.\output
set PACKAGE_DIR_EXISTS=%PACKAGE_DIR%\nul

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

copy "..\..\src\TextMetal.Common.SqlServerClr\bin\Debug\TextMetal.Common.SqlServerClr.dll" "%PACKAGE_DIR%\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.Common.SqlServerClr\bin\Debug\TextMetal.Common.SqlServerClr.pdb" "%PACKAGE_DIR%\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

powershell -command "'0x' + [System.BitConverter]::ToString([System.IO.File]::ReadAllBytes('..\..\src\TextMetal.Common.SqlServerClr\bin\Debug\TextMetal.Common.SqlServerClr.dll')).Replace('-', '')" > "%PACKAGE_DIR%\TextMetal.Common.SqlServerClr.dll.txt"
IF %ERRORLEVEL% NEQ 0 goto pkgError

powershell -command "'0x' + [System.BitConverter]::ToString([System.IO.File]::ReadAllBytes('..\..\src\TextMetal.Common.SqlServerClr\bin\Debug\TextMetal.Common.SqlServerClr.pdb')).Replace('-', '')" > "%PACKAGE_DIR%\TextMetal.Common.SqlServerClr.pdb.txt"
IF %ERRORLEVEL% NEQ 0 goto pkgError

echo *** sql_codegen_execute ***
"..\..\src\TextMetal.HostImpl.ConsoleTool\bin\Debug\TextMetal.exe" ^
	-templatefile:"sqlserver_clr_spike_template.xml" ^
	-sourcefile:"_" ^
	-basedir:"%PACKAGE_DIR%" ^
	-sourcestrategy:"TextMetal.Framework.SourceModel.Primative.NullSourceStrategy, TextMetal.Framework.SourceModel" ^
	-strict:"true" ^
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
