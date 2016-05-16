#
#	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
#	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
#

cls
$this_dir_path = [System.Environment]::CurrentDirectory

$src_dir_path = "$this_dir_path\src"
$doc_dir_path = "$this_dir_path\doc"
$imports_dir_path = "$this_dir_path\imports"
$output_dir_path = "$this_dir_path\output"
$pkg_dir_path = "$this_dir_path\pkg"
$templates_dir_path = "$this_dir_path\templates"
$tools_dir_path = "$this_dir_path\tools"

$build_flavor = "debug"

$dotnet_dir_path = "C:\Program Files\dotnet\bin"
$dotnet_file_name = "dotnet.exe"
$dotnet_exe = "$dotnet_dir_path\$dotnet_file_name"

$opencover_dir_path = "$tools_dir_path\OpenCover"
$opencover_file_name = "OpenCover.Console.exe"
$opencover_exe = "$opencover_dir_path\$opencover_file_name"

$dotcover_dir_path = "C:\Program Files (x86)\JetBrains\Installations\dotCover05"
$dotcover_file_name = "dotCover.exe"
$dotcover_exe = "$dotcover_dir_path\$dotcover_file_name"

echo "The operation is starting..."

$target_assembly_name = "TextMetal.Middleware.Solder"
$testsuite_assembly_name = "TextMetal.Middleware.UnitTests"
$xproject_dir = "$src_dir_path\$testsuite_assembly_name"

$coverage_output_dir_path = "$output_dir_path\$testsuite_assembly_name"
$coverage_output_file_path_woext ="$coverage_output_dir_path\unit-test-coverage-report"

pushd
cd "$xproject_dir"

$target_exe = "$dotnet_exe"
$target_args = @("run", "--", "--where", "class==TextMetal.Middleware.UnitTests.Solder.Injection._.*")
$target_wdir = "$xproject_dir"

$useDotCover = $true

if ((Test-Path -Path $coverage_output_dir_path))
{
	Remove-Item $coverage_output_dir_path -recurse -force
}

New-Item -ItemType directory -Path $coverage_output_dir_path

if ($useDotCover -eq $false)
{
	$filter = "+[$target_assembly_name]*"

	&$opencover_exe `
		-register:"user" `
		-target:"$target_exe" `
		-targetargs:"$target_args" `
		-filter:"$filter" `
		-output:"$coverage_output_file_path_woext.xml"
}
else
{
	$filter = "+:$target_assembly_name*"

	&$dotcover_exe analyse /Filters="$filter" `
		/TargetExecutable="$target_exe" `
		/TargetArguments="$target_args" `
		/TargetWorkingDir="$target_wdir" `
		/ReportType=HTML `
		/Output="$coverage_output_file_path_woext.html" > "$coverage_output_file_path_woext.log"

	(New-Object -Com Shell.Application).Open("$coverage_output_file_path_woext.html")
}

if (!($LastExitCode -eq $null -or $LastExitCode -eq 0))
{ echo "An error occurred during the operation."; return; }

popd

echo "The operation completed successfully."