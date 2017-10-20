@echo off

REM
REM	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
REM	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
REM

REM required for successful test coverage execution under .NET Core
SET COMPLUS_ReadyToRun=0
SET COREHOST_TRACE=0
SET DOTNET_CLI_CAPTURE_TIMING=0

CALL set-ps-env.bat

"%POWERSHELL_CORE_EXE_PATH%\powershell.exe" -command .\run_unit_test_coverage
pause > nul