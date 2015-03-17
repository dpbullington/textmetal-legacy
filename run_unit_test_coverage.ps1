#
#	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
#	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
#

$lib_dir = ".\lib"
$src_dir = ".\src"
$output_base_dir = ".\output"
$cover_exe = "$Env:USERPROFILE\AppData\Local\JetBrains\Installations\dotCover01\dotCover.exe"
$exclude_filter = "-:TestingConsoleTool;-:LeastViable.Testing;-:TextMetal.Framework.UnitTests;"
$nunit_exe = ".\$lib_dir\LeastViable\TestingConsoleTool.exe"

echo "The operation is starting..."

&$cover_exe analyse /Filters="$exclude_filter" `
	/TargetExecutable="$nunit_exe" `
	/TargetArguments="/run:TextMetal.Framework.UnitTests .\$src_dir\TextMetal.Framework.UnitTests\bin\Debug\TextMetal.Framework.UnitTests.dll" `
	/TargetWorkingDir="" /ReportType=HTML `
	/Output="$output_base_dir\LeastViable\ut_cov_rpt.html"

if (!($LastExitCode -eq $null -or $LastExitCode -eq 0))
{ echo "An error occurred during the operation."; return; }

echo "The operation completed successfully."