@echo off

REM
REM	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
REM	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
REM

set TEXTMETAL_EXE=..\..\src\TextMetal.HostImpl.ConsoleTool\bin\Debug\TextMetal.exe

SET SRC_DIR=..\..\src
SET BUILD_FLAVOR_DIR=Debug
SET BUILD_TOOL_CFG=Debug

set SNK_EXE=C:\Program Files (x86)\Microsoft SDKs\Windows\v8.0A\bin\NETFX 4.0 Tools\sn.exe

set BUILD_FLAVOR_DIR=Debug
set BUILD_TOOL_CFG=Debug

set PACKAGE_DIR=.\output
set SRC_DIR=%PACKAGE_DIR%\src
set LIB_DIR=%PACKAGE_DIR%\lib
set PACKAGE_DIR_EXISTS=%PACKAGE_DIR%\nul

set CLR_NAMESPACE=TextMetal.HostImpl.AspNetSample.ModelDomain
set ADO_NET_CONNECTION_STRING=Server=(local);User ID=textmetal_mssql_dev_login;Password=LrJGmP6UfW8TEp7x3wWhECUYULE6zzMcWQ03R6UxeB4xzVmnq5S4Lx0vApegZVH;Database=textmetal_ods_dev

:pkgDir

IF NOT EXIST %PACKAGE_DIR_EXISTS% GOTO noPkgDir
goto delPkgDir

:noPkgDir
@echo Creating output directory...
mkdir "%LIB_DIR%"
IF %ERRORLEVEL% NEQ 0 goto pkgError

mkdir "%LIB_DIR%\IronRuby"
IF %ERRORLEVEL% NEQ 0 goto pkgError

mkdir "%LIB_DIR%\SQLite"
IF %ERRORLEVEL% NEQ 0 goto pkgError

mkdir "%LIB_DIR%\SQLite\x86"
IF %ERRORLEVEL% NEQ 0 goto pkgError

mkdir "%LIB_DIR%\SQLite\x64"
IF %ERRORLEVEL% NEQ 0 goto pkgError

mkdir "%LIB_DIR%\TextMetal"
IF %ERRORLEVEL% NEQ 0 goto pkgError

:delPkgDir
@echo Cleaning output directory...
del "%PACKAGE_DIR%\*.*" /Q /S
rem IF %ERRORLEVEL% NEQ 0 goto pkgError
goto pkgBuild


:pkgBuild

copy "..\..\lib\IronRuby\*.*" "%LIB_DIR%\IronRuby\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\lib\SQLite\x86\*.*" "%LIB_DIR%\SQLite\x86\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\lib\SQLite\x64\*.*" "%LIB_DIR%\SQLite\x64\."
IF %ERRORLEVEL% NEQ 0 goto pkgError




copy "..\..\src\TextMetal.HostImpl.AspNetSample.Database\Script\*.*" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError



copy "..\..\src\TextMetal.Common.Cerealization\bin\%BUILD_FLAVOR_DIR%\TextMetal.Common.Cerealization.dll" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.Common.Cerealization\bin\%BUILD_FLAVOR_DIR%\TextMetal.Common.Cerealization.xml" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.Common.Cerealization\bin\%BUILD_FLAVOR_DIR%\TextMetal.Common.Cerealization.pdb" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError



copy "..\..\src\TextMetal.Common.Core\bin\%BUILD_FLAVOR_DIR%\TextMetal.Common.Core.dll" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.Common.Core\bin\%BUILD_FLAVOR_DIR%\TextMetal.Common.Core.xml" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.Common.Core\bin\%BUILD_FLAVOR_DIR%\TextMetal.Common.Core.pdb" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError



copy "..\..\src\TextMetal.Common.Data\bin\%BUILD_FLAVOR_DIR%\TextMetal.Common.Data.dll" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.Common.Data\bin\%BUILD_FLAVOR_DIR%\TextMetal.Common.Data.xml" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.Common.Data\bin\%BUILD_FLAVOR_DIR%\TextMetal.Common.Data.pdb" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError



copy "..\..\src\TextMetal.Common.Solder\bin\%BUILD_FLAVOR_DIR%\TextMetal.Common.Solder.dll" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.Common.Solder\bin\%BUILD_FLAVOR_DIR%\TextMetal.Common.Solder.xml" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.Common.Solder\bin\%BUILD_FLAVOR_DIR%\TextMetal.Common.Solder.pdb" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError



copy "..\..\src\TextMetal.Common.Syntax\bin\%BUILD_FLAVOR_DIR%\TextMetal.Common.Syntax.dll" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.Common.Syntax\bin\%BUILD_FLAVOR_DIR%\TextMetal.Common.Syntax.xml" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.Common.Syntax\bin\%BUILD_FLAVOR_DIR%\TextMetal.Common.Syntax.pdb" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError



copy "..\..\src\TextMetal.Common.UnitTests\bin\%BUILD_FLAVOR_DIR%\TextMetal.Common.UnitTests.dll" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.Common.UnitTests\bin\%BUILD_FLAVOR_DIR%\TextMetal.Common.UnitTests.xml" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.Common.UnitTests\bin\%BUILD_FLAVOR_DIR%\TextMetal.Common.UnitTests.pdb" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError



copy "..\..\src\TextMetal.Common.WinForms\bin\%BUILD_FLAVOR_DIR%\TextMetal.Common.WinForms.dll" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.Common.WinForms\bin\%BUILD_FLAVOR_DIR%\TextMetal.Common.WinForms.xml" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.Common.WinForms\bin\%BUILD_FLAVOR_DIR%\TextMetal.Common.WinForms.pdb" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError



copy "..\..\src\TextMetal.Common.Xml\bin\%BUILD_FLAVOR_DIR%\TextMetal.Common.Xml.dll" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.Common.Xml\bin\%BUILD_FLAVOR_DIR%\TextMetal.Common.Xml.xml" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.Common.Xml\bin\%BUILD_FLAVOR_DIR%\TextMetal.Common.Xml.pdb" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError



copy "..\..\src\TextMetal.Framework.AssociativeModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.AssociativeModel.dll" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.Framework.AssociativeModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.AssociativeModel.xml" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.Framework.AssociativeModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.AssociativeModel.pdb" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError



copy "..\..\src\TextMetal.Framework.Core\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.Core.dll" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.Framework.Core\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.Core.xml" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.Framework.Core\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.Core.pdb" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError



copy "..\..\src\TextMetal.Framework.DebuggerProfilerModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.DebuggerProfilerModel.dll" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.Framework.DebuggerProfilerModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.DebuggerProfilerModel.xml" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.Framework.DebuggerProfilerModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.DebuggerProfilerModel.pdb" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError



copy "..\..\src\TextMetal.Framework.ExpressionModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.ExpressionModel.dll" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.Framework.ExpressionModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.ExpressionModel.xml" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.Framework.ExpressionModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.ExpressionModel.pdb" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError



copy "..\..\src\TextMetal.Framework.HostingModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.HostingModel.dll" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.Framework.HostingModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.HostingModel.xml" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.Framework.HostingModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.HostingModel.pdb" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError



copy "..\..\src\TextMetal.Framework.InputOutputModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.InputOutputModel.dll" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.Framework.InputOutputModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.InputOutputModel.xml" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.Framework.InputOutputModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.InputOutputModel.pdb" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError



copy "..\..\src\TextMetal.Framework.IntegrationTests\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.IntegrationTests.dll" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.Framework.IntegrationTests\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.IntegrationTests.xml" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.Framework.IntegrationTests\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.IntegrationTests.pdb" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError



copy "..\..\src\TextMetal.Framework.SortModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.SortModel.dll" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.Framework.SortModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.SortModel.xml" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.Framework.SortModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.SortModel.pdb" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError



copy "..\..\src\TextMetal.Framework.SourceModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.SourceModel.dll" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.Framework.SourceModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.SourceModel.xml" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.Framework.SourceModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.SourceModel.pdb" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError



copy "..\..\src\TextMetal.Framework.TemplateModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.TemplateModel.dll" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.Framework.TemplateModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.TemplateModel.xml" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.Framework.TemplateModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.TemplateModel.pdb" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError



copy "..\..\src\TextMetal.Framework.UnitTests\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.UnitTests.dll" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.Framework.UnitTests\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.UnitTests.xml" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.Framework.UnitTests\bin\%BUILD_FLAVOR_DIR%\TextMetal.Framework.UnitTests.pdb" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError



copy "..\..\src\TextMetal.HostImpl.AspNetSample.Common\bin\%BUILD_FLAVOR_DIR%\TextMetal.HostImpl.AspNetSample.Common.dll" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.HostImpl.AspNetSample.Common\bin\%BUILD_FLAVOR_DIR%\TextMetal.HostImpl.AspNetSample.Common.xml" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.HostImpl.AspNetSample.Common\bin\%BUILD_FLAVOR_DIR%\TextMetal.HostImpl.AspNetSample.Common.pdb" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError



copy "..\..\src\TextMetal.HostImpl.AspNetSample.DomainModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.HostImpl.AspNetSample.DomainModel.dll" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.HostImpl.AspNetSample.DomainModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.HostImpl.AspNetSample.DomainModel.xml" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.HostImpl.AspNetSample.DomainModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.HostImpl.AspNetSample.DomainModel.pdb" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError



copy "..\..\src\TextMetal.HostImpl.AspNetSample.ServiceModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.HostImpl.AspNetSample.ServiceModel.dll" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.HostImpl.AspNetSample.ServiceModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.HostImpl.AspNetSample.ServiceModel.xml" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.HostImpl.AspNetSample.ServiceModel\bin\%BUILD_FLAVOR_DIR%\TextMetal.HostImpl.AspNetSample.ServiceModel.pdb" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError



copy "..\..\src\TextMetal.HostImpl.AspNetSample.UI.Web.Mvc\bin\TextMetal.HostImpl.AspNetSample.UI.Web.Mvc.dll" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.HostImpl.AspNetSample.UI.Web.Mvc\bin\TextMetal.HostImpl.AspNetSample.UI.Web.Mvc.xml" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.HostImpl.AspNetSample.UI.Web.Mvc\bin\TextMetal.HostImpl.AspNetSample.UI.Web.Mvc.pdb" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError



copy "..\..\src\TextMetal.HostImpl.AspNetSample.UI.Web.Shared\bin\%BUILD_FLAVOR_DIR%\TextMetal.HostImpl.AspNetSample.UI.Web.Shared.dll" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.HostImpl.AspNetSample.UI.Web.Shared\bin\%BUILD_FLAVOR_DIR%\TextMetal.HostImpl.AspNetSample.UI.Web.Shared.xml" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.HostImpl.AspNetSample.UI.Web.Shared\bin\%BUILD_FLAVOR_DIR%\TextMetal.HostImpl.AspNetSample.UI.Web.Shared.pdb" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError



copy "..\..\src\TextMetal.HostImpl.ConsoleTool\bin\%BUILD_FLAVOR_DIR%\TextMetal.exe" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.HostImpl.ConsoleTool\bin\%BUILD_FLAVOR_DIR%\TextMetal.xml" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.HostImpl.ConsoleTool\bin\%BUILD_FLAVOR_DIR%\TextMetal.pdb" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError



copy "..\..\src\TextMetal.HostImpl.Tool\bin\%BUILD_FLAVOR_DIR%\TextMetal.HostImpl.Tool.dll" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.HostImpl.Tool\bin\%BUILD_FLAVOR_DIR%\TextMetal.HostImpl.Tool.xml" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.HostImpl.Tool\bin\%BUILD_FLAVOR_DIR%\TextMetal.HostImpl.Tool.pdb" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError



copy "..\..\src\TextMetal.HostImpl.Web\bin\%BUILD_FLAVOR_DIR%\TextMetal.HostImpl.Web.dll" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.HostImpl.Web\bin\%BUILD_FLAVOR_DIR%\TextMetal.HostImpl.Web.xml" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.HostImpl.Web\bin\%BUILD_FLAVOR_DIR%\TextMetal.HostImpl.Web.pdb" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError



copy "..\..\src\TextMetal.HostImpl.WindowsTool\bin\%BUILD_FLAVOR_DIR%\TextMetalStudio.exe" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.HostImpl.WindowsTool\bin\%BUILD_FLAVOR_DIR%\TextMetalStudio.xml" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.HostImpl.WindowsTool\bin\%BUILD_FLAVOR_DIR%\TextMetalStudio.pdb" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError



copy "..\..\src\TextMetal.Imports\bin\%BUILD_FLAVOR_DIR%\TextMetal.Imports.dll" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.Imports\bin\%BUILD_FLAVOR_DIR%\TextMetal.Imports.xml" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.Imports\bin\%BUILD_FLAVOR_DIR%\TextMetal.Imports.pdb" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError



copy "..\..\src\TextMetal.Imports.nunit.console.exe\bin\%BUILD_FLAVOR_DIR%\TextMetal.Imports.nunit.console.exe" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.Imports.nunit.console.exe\bin\%BUILD_FLAVOR_DIR%\TextMetal.Imports.nunit.console.xml" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.Imports.nunit.console.exe\bin\%BUILD_FLAVOR_DIR%\TextMetal.Imports.nunit.console.pdb" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError



copy "..\..\src\TextMetal.Imports.nunit.gui.exe\bin\%BUILD_FLAVOR_DIR%\TextMetal.Imports.nunit.gui.exe" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.Imports.nunit.gui.exe\bin\%BUILD_FLAVOR_DIR%\TextMetal.Imports.nunit.gui.xml" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.Imports.nunit.gui.exe\bin\%BUILD_FLAVOR_DIR%\TextMetal.Imports.nunit.gui.pdb" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError



copy "..\..\src\TextMetal.TestFramework\bin\%BUILD_FLAVOR_DIR%\TextMetal.TestFramework.dll" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.TestFramework\bin\%BUILD_FLAVOR_DIR%\TextMetal.TestFramework.xml" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError

copy "..\..\src\TextMetal.TestFramework\bin\%BUILD_FLAVOR_DIR%\TextMetal.TestFramework.pdb" "%PACKAGE_DIR%\lib\TextMetal\."
IF %ERRORLEVEL% NEQ 0 goto pkgError




echo *** adonet_codegen_execute ***
"%TEXTMETAL_EXE%" ^
	-templatefile:"master_template.xml" ^
	-sourcefile:"%ADO_NET_CONNECTION_STRING%" ^
	-basedir:"%SRC_DIR%" ^
	-sourcestrategy:"TextMetal.Framework.SourceModel.DatabaseSchema.Sql.SqlSchemaSourceStrategy, TextMetal.Framework.SourceModel" ^
	-strict:"true" ^
	-property:"ClrNamespace=%CLR_NAMESPACE%" ^
	-property:"ClrSuperType=Object" ^
	-property:"ConnectionType=System.Data.SqlClient.SqlConnection, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" ^
	-property:"DataSourceTag=net.sqlserver"
IF %ERRORLEVEL% NEQ 0 goto pkgError


echo *** strong_name_key_generate ***
"%SNK_EXE%" -k "%SRC_DIR%\%CLR_NAMESPACE%.snk"
IF %ERRORLEVEL% NEQ 0 goto pkgError


goto pkgSuccess


:pkgError
echo An error occured.
pause > nul
goto :eof

:pkgSuccess
echo Completed successfully.
goto :eof
