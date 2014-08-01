@ECHO off

REM
REM	Copyright ?2002-2014 Daniel Bullington (dpbullington@gmail.com)
REM	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
REM

CD "."
GOTO setEnv


:setEnv

SET SRC_DIR=.\src
SET LIB_DIR=.\lib
SET TEMPLATES_DIR=.\templates
SET PACKAGE_DIR=.\pkg
SET PACKAGE_DIR_EXISTS=%PACKAGE_DIR%\nul
SET BUILD_EXE=C:\Program Files (x86)\MSBuild\12.0\bin\amd64\msbuild.exe
SET ROBOCOPY_EXE=robocopy.exe
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
@ECHO LIB_DIR=%LIB_DIR%
@ECHO TEMPLATES_DIR=%TEMPLATES_DIR%
@ECHO PACKAGE_DIR=%PACKAGE_DIR%
@ECHO PACKAGE_DIR_EXISTS=%PACKAGE_DIR_EXISTS%
@ECHO BUILD_EXE=%BUILD_EXE%
@ECHO BUILD_FLAVOR_DIR=%BUILD_FLAVOR_DIR%
@ECHO BUILD_TOOL_CFG=%BUILD_TOOL_CFG%
@ECHO ROBOCOPY_EXE=%ROBOCOPY_EXE%
GOTO pkgDir


:pkgDir

IF NOT EXIST %PACKAGE_DIR_EXISTS% GOTO noPkgDir
@ECHO Cleaning %PACKAGE_DIR% directory...
RMDIR "%PACKAGE_DIR%" /Q /S
IF %ERRORLEVEL% NEQ 0 GOTO pkgError
GOTO noPkgDir


:noPkgDir

@ECHO Creating %PACKAGE_DIR% directory...
MKDIR "%PACKAGE_DIR%"
IF %ERRORLEVEL% NEQ 0 GOTO pkgError
GOTO pkgBuild


:pkgBuild

REM ADD SWITCHES FOR SKIPCLEAN, SKIPBUILD, SKIPCOPY

"%BUILD_EXE%" "%SRC_DIR%\TextMetal.sln" /t:clean /p:Configuration=%BUILD_TOOL_CFG% /p:VisualStudioVersion=12.0
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

"%BUILD_EXE%" "%SRC_DIR%\TextMetal.sln" /t:build /p:Configuration=%BUILD_TOOL_CFG% /p:VisualStudioVersion=12.0
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

GOTO pkgCopy


:pkgCopy

"%ROBOCOPY_EXE%" "%LIB_DIR%" "%PACKAGE_DIR%\lib" /E /NS /NC /NFL /NDL /NP
IF %ERRORLEVEL% NEQ 1 GOTO pkgError

"%ROBOCOPY_EXE%" "%TEMPLATES_DIR%" "%PACKAGE_DIR%\templates" /E /NS /NC /NFL /NDL /NP /XF "*!git*" /XD "output" "*!git*"
IF %ERRORLEVEL% NEQ 1 GOTO pkgError

"%ROBOCOPY_EXE%" "." "%PACKAGE_DIR%\bin" "trunk.bat"
IF %ERRORLEVEL% NEQ 1 GOTO pkgError


:pkgCopyFragBegin

copy "%SRC_DIR%\TextMetal.Common.Cerealization\bin\%BUILD_FLAVOR_DIR%\TextMetal.Common.Cerealization.dll" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

copy "%SRC_DIR%\TextMetal.Common.Cerealization\bin\%BUILD_FLAVOR_DIR%\TextMetal.Common.Cerealization.xml" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

copy "%SRC_DIR%\TextMetal.Common.Cerealization\bin\%BUILD_FLAVOR_DIR%\TextMetal.Common.Cerealization.pdb" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError



copy "%SRC_DIR%\TextMetal.Common.Core\bin\%BUILD_FLAVOR_DIR%\TextMetal.Common.Core.dll" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

copy "%SRC_DIR%\TextMetal.Common.Core\bin\%BUILD_FLAVOR_DIR%\TextMetal.Common.Core.xml" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

copy "%SRC_DIR%\TextMetal.Common.Core\bin\%BUILD_FLAVOR_DIR%\TextMetal.Common.Core.pdb" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError



copy "%SRC_DIR%\TextMetal.Common.Data\bin\%BUILD_FLAVOR_DIR%\TextMetal.Common.Data.dll" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

copy "%SRC_DIR%\TextMetal.Common.Data\bin\%BUILD_FLAVOR_DIR%\TextMetal.Common.Data.xml" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

copy "%SRC_DIR%\TextMetal.Common.Data\bin\%BUILD_FLAVOR_DIR%\TextMetal.Common.Data.pdb" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError



copy "%SRC_DIR%\TextMetal.Common.Solder\bin\%BUILD_FLAVOR_DIR%\TextMetal.Common.Solder.dll" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

copy "%SRC_DIR%\TextMetal.Common.Solder\bin\%BUILD_FLAVOR_DIR%\TextMetal.Common.Solder.xml" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

copy "%SRC_DIR%\TextMetal.Common.Solder\bin\%BUILD_FLAVOR_DIR%\TextMetal.Common.Solder.pdb" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError



copy "%SRC_DIR%\TextMetal.Common.Syntax\bin\%BUILD_FLAVOR_DIR%\TextMetal.Common.Syntax.dll" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

copy "%SRC_DIR%\TextMetal.Common.Syntax\bin\%BUILD_FLAVOR_DIR%\TextMetal.Common.Syntax.xml" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

copy "%SRC_DIR%\TextMetal.Common.Syntax\bin\%BUILD_FLAVOR_DIR%\TextMetal.Common.Syntax.pdb" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError



copy "%SRC_DIR%\TextMetal.Common.UnitTests\bin\%BUILD_FLAVOR_DIR%\TextMetal.Common.UnitTests.dll" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

copy "%SRC_DIR%\TextMetal.Common.UnitTests\bin\%BUILD_FLAVOR_DIR%\TextMetal.Common.UnitTests.xml" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

copy "%SRC_DIR%\TextMetal.Common.UnitTests\bin\%BUILD_FLAVOR_DIR%\TextMetal.Common.UnitTests.pdb" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError



copy "%SRC_DIR%\TextMetal.Common.Xml\bin\%BUILD_FLAVOR_DIR%\TextMetal.Common.Xml.dll" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

copy "%SRC_DIR%\TextMetal.Common.Xml\bin\%BUILD_FLAVOR_DIR%\TextMetal.Common.Xml.xml" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

copy "%SRC_DIR%\TextMetal.Common.Xml\bin\%BUILD_FLAVOR_DIR%\TextMetal.Common.Xml.pdb" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError



copy "%SRC_DIR%\TextMetal.Framework.AssociativeModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.AssociativeModel.dll" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

copy "%SRC_DIR%\TextMetal.Framework.AssociativeModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.AssociativeModel.xml" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

copy "%SRC_DIR%\TextMetal.Framework.AssociativeModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.AssociativeModel.pdb" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError



copy "%SRC_DIR%\TextMetal.Framework.Core\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.Core.dll" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

copy "%SRC_DIR%\TextMetal.Framework.Core\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.Core.xml" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

copy "%SRC_DIR%\TextMetal.Framework.Core\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.Core.pdb" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError



copy "%SRC_DIR%\TextMetal.Framework.DebuggerProfilerModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.DebuggerProfilerModel.dll" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

copy "%SRC_DIR%\TextMetal.Framework.DebuggerProfilerModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.DebuggerProfilerModel.xml" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

copy "%SRC_DIR%\TextMetal.Framework.DebuggerProfilerModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.DebuggerProfilerModel.pdb" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError



copy "%SRC_DIR%\TextMetal.Framework.ExpressionModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.ExpressionModel.dll" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

copy "%SRC_DIR%\TextMetal.Framework.ExpressionModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.ExpressionModel.xml" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

copy "%SRC_DIR%\TextMetal.Framework.ExpressionModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.ExpressionModel.pdb" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError



copy "%SRC_DIR%\TextMetal.Framework.HostingModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.HostingModel.dll" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

copy "%SRC_DIR%\TextMetal.Framework.HostingModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.HostingModel.xml" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

copy "%SRC_DIR%\TextMetal.Framework.HostingModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.HostingModel.pdb" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError



copy "%SRC_DIR%\TextMetal.Framework.InputOutputModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.InputOutputModel.dll" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

copy "%SRC_DIR%\TextMetal.Framework.InputOutputModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.InputOutputModel.xml" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

copy "%SRC_DIR%\TextMetal.Framework.InputOutputModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.InputOutputModel.pdb" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError



copy "%SRC_DIR%\TextMetal.Framework.IntegrationTests\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.IntegrationTests.dll" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

copy "%SRC_DIR%\TextMetal.Framework.IntegrationTests\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.IntegrationTests.xml" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

copy "%SRC_DIR%\TextMetal.Framework.IntegrationTests\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.IntegrationTests.pdb" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError



copy "%SRC_DIR%\TextMetal.Framework.SortModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.SortModel.dll" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

copy "%SRC_DIR%\TextMetal.Framework.SortModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.SortModel.xml" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

copy "%SRC_DIR%\TextMetal.Framework.SortModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.SortModel.pdb" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError



copy "%SRC_DIR%\TextMetal.Framework.SourceModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.SourceModel.dll" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

copy "%SRC_DIR%\TextMetal.Framework.SourceModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.SourceModel.xml" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

copy "%SRC_DIR%\TextMetal.Framework.SourceModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.SourceModel.pdb" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError



copy "%SRC_DIR%\TextMetal.Framework.TemplateModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.TemplateModel.dll" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

copy "%SRC_DIR%\TextMetal.Framework.TemplateModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.TemplateModel.xml" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

copy "%SRC_DIR%\TextMetal.Framework.TemplateModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.TemplateModel.pdb" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError



copy "%SRC_DIR%\TextMetal.Framework.UnitTests\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.UnitTests.dll" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

copy "%SRC_DIR%\TextMetal.Framework.UnitTests\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.UnitTests.xml" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

copy "%SRC_DIR%\TextMetal.Framework.UnitTests\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.UnitTests.pdb" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError



copy "%SRC_DIR%\TextMetal.HostImpl.ConsoleTool\bin\%BUILD_FLAVOR_DIR%\TextMetal.exe" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

copy "%SRC_DIR%\TextMetal.HostImpl.ConsoleTool\bin\%BUILD_FLAVOR_DIR%\TextMetal.xml" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

copy "%SRC_DIR%\TextMetal.HostImpl.ConsoleTool\bin\%BUILD_FLAVOR_DIR%\TextMetal.pdb" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError



copy "%SRC_DIR%\TextMetal.HostImpl.Tool\bin\%BUILD_FLAVOR_DIR%\TextMetal.HostImpl.Tool.dll" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

copy "%SRC_DIR%\TextMetal.HostImpl.Tool\bin\%BUILD_FLAVOR_DIR%\TextMetal.HostImpl.Tool.xml" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

copy "%SRC_DIR%\TextMetal.HostImpl.Tool\bin\%BUILD_FLAVOR_DIR%\TextMetal.HostImpl.Tool.pdb" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError



copy "%SRC_DIR%\TextMetal.HostImpl.WindowsTool\bin\%BUILD_FLAVOR_DIR%\TextMetalStudio.exe" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

copy "%SRC_DIR%\TextMetal.HostImpl.WindowsTool\bin\%BUILD_FLAVOR_DIR%\TextMetalStudio.xml" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

copy "%SRC_DIR%\TextMetal.HostImpl.WindowsTool\bin\%BUILD_FLAVOR_DIR%\TextMetalStudio.pdb" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError



copy "%SRC_DIR%\TextMetal.Imports\bin\%BUILD_FLAVOR_DIR%\TextMetal.Imports.dll" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

copy "%SRC_DIR%\TextMetal.Imports\bin\%BUILD_FLAVOR_DIR%\TextMetal.Imports.xml" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

copy "%SRC_DIR%\TextMetal.Imports\bin\%BUILD_FLAVOR_DIR%\TextMetal.Imports.pdb" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError



copy "%SRC_DIR%\TextMetal.Imports.nunit.console.exe\bin\%BUILD_FLAVOR_DIR%\TextMetal.Imports.nunit.console.exe" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

copy "%SRC_DIR%\TextMetal.Imports.nunit.console.exe\bin\%BUILD_FLAVOR_DIR%\TextMetal.Imports.nunit.console.xml" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

copy "%SRC_DIR%\TextMetal.Imports.nunit.console.exe\bin\%BUILD_FLAVOR_DIR%\TextMetal.Imports.nunit.console.pdb" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError



copy "%SRC_DIR%\TextMetal.Imports.nunit.gui.exe\bin\%BUILD_FLAVOR_DIR%\TextMetal.Imports.nunit.gui.exe" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

copy "%SRC_DIR%\TextMetal.Imports.nunit.gui.exe\bin\%BUILD_FLAVOR_DIR%\TextMetal.Imports.nunit.gui.xml" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

copy "%SRC_DIR%\TextMetal.Imports.nunit.gui.exe\bin\%BUILD_FLAVOR_DIR%\TextMetal.Imports.nunit.gui.pdb" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError



copy "%SRC_DIR%\TextMetal.TestFramework\bin\%BUILD_FLAVOR_DIR%\TextMetal.TestFramework.dll" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

copy "%SRC_DIR%\TextMetal.TestFramework\bin\%BUILD_FLAVOR_DIR%\TextMetal.TestFramework.xml" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

copy "%SRC_DIR%\TextMetal.TestFramework\bin\%BUILD_FLAVOR_DIR%\TextMetal.TestFramework.pdb" "%PACKAGE_DIR%\bin\."
IF %ERRORLEVEL% NEQ 0 GOTO pkgError

:pkgCopyFragEnd


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
