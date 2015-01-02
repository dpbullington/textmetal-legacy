#
#	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
#	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
#

$src_dir = ".\src"
$output_base_dir = ".\output"
$cover_exe = "C:\Program Files (x86)\JetBrains\dotCover\v2.7\Bin\dotCover.exe"
$exclude_filter = "-:TextMetal.Common.Solder;-:TextMetal.Imports;-:TextMetal.Imports.nunit.console;-:TextMetal.TestFramework;-:TextMetal.Common.UnitTests;"
$nunit_exe = ".\$src_dir\TextMetal.Imports.nunit.console.exe\bin\Debug\TextMetal.Imports.nunit.console.exe"

echo "The operation is starting..."

if ($false)
{
	# --------------------------------

&$cover_exe analyse /Filters="$exclude_filter" `
	/TargetExecutable="$nunit_exe" `
	/TargetArguments="/run:TextMetal.Common.UnitTests.Core._.AppConfigTests .\$src_dir\TextMetal.Common.UnitTests\bin\Debug\TextMetal.Common.UnitTests.dll" `
	/TargetWorkingDir="" /ReportType=HTML `
	/Output="$output_base_dir\TextMetal.Common.Core\AppConfig\ut_cov_rpt.html"

if (!($LastExitCode -eq $null -or $LastExitCode -eq 0))
{ echo "An error occurred during the operation."; return; }

	# --------------------------------
}

if ($true)
{
	# --------------------------------

&$cover_exe analyse /Filters="$exclude_filter" `
	/TargetExecutable="$nunit_exe" `
	/TargetArguments="/run:TextMetal.Common.UnitTests.Core._.ReflexionTests .\$src_dir\TextMetal.Common.UnitTests\bin\Debug\TextMetal.Common.UnitTests.dll" `
	/TargetWorkingDir="" /ReportType=HTML `
	/Output="$output_base_dir\TextMetal.Common.Core\Reflexion\ut_cov_rpt.html"

if (!($LastExitCode -eq $null -or $LastExitCode -eq 0))
{ echo "An error occurred during the operation."; return; }

	# --------------------------------
}

echo "The operation completed successfully."