#
#	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
#	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
#

$root = [System.Environment]::CurrentDirectory

$dnx_bin_dir = "C:\Program Files\dotnet\bin"
$dnx_exe = "$dnx_bin_dir\dotnet.exe"

$src_dir = "$root\src"
$tools_dir = "$root\tools"

$target_assembly = "TextMetal.NopCLI"
$project_dir = "$src_dir\$target_assembly"

$output_base_dir = "$root\output"
$output_file_woext ="$output_base_dir\$target_assembly\unit-test-coverage-report"

echo "The operation is starting..."

pushd
cd "$project_dir"

$target_exe = "$dnx_exe"
$target_args = @("run")
#$target_args = @("--where", "class=TextMetal.Middleware.UnitTests.Solder.Utilities._.ReflectionFascadeTests")
$target_wdir = "$project_dir"

$useDotCover = $true

if ($useDotCover -eq $false)
{
	$cover_exe = "$tools_dir\OpenCover\OpenCover.Console.exe"
	$filter = "+[$target_assembly]*"

	&$cover_exe `
		-register:"user" `
		-target:"$target_exe" `
		-targetargs:"$target_args" `
		-filter:"$filter" `
		-output:"$output_file_woext.xml"
}
else
{
	$cover_exe = "C:\Program Files (x86)\JetBrains\Installations\dotCover04\dotCover.exe"
	$filter = "+:$target_assembly"

	&$cover_exe analyse /Filters="$filter" `
		/TargetExecutable="$target_exe" `
		/TargetArguments="$target_args" `
		/TargetWorkingDir="$target_wdir" `
		/ReportType=HTML `
		/Output="$output_file_woext.html"
}

if (!($LastExitCode -eq $null -or $LastExitCode -eq 0))
{ echo "An error occurred during the operation."; return; }

popd

echo "The operation completed successfully."