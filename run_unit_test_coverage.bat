@echo off

REM
REM	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
REM	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
REM

SET COMPLUS_ReadyToRun=0
powershell -command .\run_unit_test_coverage
pause > nul