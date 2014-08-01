@echo off

REM
REM	Copyright ?2002-2014 Daniel Bullington (dpbullington@gmail.com)
REM	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
REM

set TEXTMETAL_EXE=..\..\src\TextMetal.HostImpl.ConsoleTool\bin\Debug\TextMetal.exe

echo *** property_path_source_execute ***
"%TEXTMETAL_EXE%" ^
	-templatefile:"property_path_template.xml" ^
	-sourcefile:"property_path_source.xml" ^
	-basedir:".\output" ^
	-sourcestrategy:"TextMetal.Framework.SourceModel.Primative.WellKnownXmlPersistEngineSourceStrategy, TextMetal.Framework.SourceModel" ^
	-strict:"true" ^
	-debug:"false"
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
