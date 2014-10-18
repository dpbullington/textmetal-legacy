#
#	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
#	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
#

$src_dir = ".\src"
$output_base_dir = ".\output"
$cover_exe = "C:\Program Files (x86)\JetBrains\dotCover\v2.7\Bin\dotCover.exe"
$exclude_filter = "-:TextMetal.Imports;-:TextMetal.Imports.nunit.console;-:TextMetal.TestFramework;-:TextMetal.Common.UnitTests;"
$nunit_exe = ".\$src_dir\TextMetal.Imports.nunit.console.exe\bin\Debug\TextMetal.Imports.nunit.console.exe"

echo "The operation is starting..."

&$cover_exe analyse /Filters="$exclude_filter" `
	/TargetExecutable="$nunit_exe" `
	/TargetArguments="/run:TextMetal.Common.UnitTests.Core._.ReflexionTests .\$src_dir\TextMetal.Common.UnitTests\bin\Debug\TextMetal.Common.UnitTests.dll" `
	/TargetWorkingDir="" /ReportType=HTML `
	/Output="$output_base_dir\TextMetal.Common.Core\Reflexion\ut_cov_rpt.html"

echo "The operation completed successfully."