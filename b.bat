@ECHO off

REM
REM	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
REM	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
REM

CD "."
GOTO setEnv


:setEnv

SET SRC_DIR=.\src
SET BUILD_EXE=C:\Program Files (x86)\MSBuild\12.0\bin\amd64\msbuild.exe
GOTO chooseBuildFlavor


:chooseBuildFlavor

IF "%1" == "" GOTO flavorRelease
IF /i %1 == -release GOTO flavorRelease
IF /i %1 == -debug GOTO flavorDebug
GOTO pkgUsage


:flavorRelease

@ECHO Using [Release] build flavor...
SET BUILD_FLAVOR_DIR=Release
SET BUILD_TOOL_CFG=Release
GOTO writeEnv


:flavorDebug

@ECHO Using [Debug] build flavor...
SET BUILD_FLAVOR_DIR=Debug
SET BUILD_TOOL_CFG=Debug
GOTO writeEnv


:writeEnv

@ECHO SRC_DIR=%SRC_DIR%
@ECHO BUILD_EXE=%BUILD_EXE%
@ECHO BUILD_FLAVOR_DIR=%BUILD_FLAVOR_DIR%
@ECHO BUILD_TOOL_CFG=%BUILD_TOOL_CFG%
GOTO pkgBuild


:pkgBuild

REM ADD SWITCHES FOR SKIPCLEAN, SKIPBUILD, SKIPCOPY

"%BUILD_EXE%" /verbosity:quiet /consoleloggerparameters:ErrorsOnly "%SRC_DIR%\TextMetal.sln" /t:clean /p:Configuration=%BUILD_TOOL_CFG% /p:VisualStudioVersion=12.0
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

"%BUILD_EXE%" /verbosity:quiet /consoleloggerparameters:ErrorsOnly "%SRC_DIR%\TextMetal.sln" /t:build /p:Configuration=%BUILD_TOOL_CFG% /p:VisualStudioVersion=12.0
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

GOTO pkgSuccess


:pkgError
ECHO An error occured during the operation.
GOTO :eof


:pkgSuccess
ECHO The operation completed successfully.
GOTO :eof


:pkgUsage
ECHO Error in script usage. The correct usage is:
ECHO     %0 [flavor]
ECHO where [flavor] is: -release ^| -debug
ECHO:
ECHO For example:
ECHO     %0 -debug
GOTO :eof
