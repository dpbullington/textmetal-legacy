@echo off

REM
REM	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
REM	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
REM

CALL set-ps-env.bat

"%POWERSHELL_CORE_EXE_PATH%\powershell.exe" -command .\clean_source_tree > "%temp%\clean_source_tree.log"
