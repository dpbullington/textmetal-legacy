#
#	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
#	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
#

cls

$this_dir_path = [System.Environment]::CurrentDirectory
$src_dir_path = "$this_dir_path\src"
$output_dir_path = "$this_dir_path\output"

$build_flavor = "debug"
$build_tfm = "netcoreapp1.1"

$dotnet_dir_path = "C:\Program Files\dotnet"
$dotnet_file_name = "dotnet.exe"
$dotnet_exe = "$dotnet_dir_path\$dotnet_file_name"

$dotcover_dir_path = "C:\Program Files (x86)\JetBrains\Installations\dotCover07"
$dotcover_file_name = "dotCover.exe"
$dotcover_exe = "$dotcover_dir_path\$dotcover_file_name"

echo "The operation is starting..."


### -- for assembly under test (testsuite_for_assembly_name),
### -- run the entry point in the test suite assembly (testsuite_assembly_name),
### -- filter and execute tests cases in sub namespace (testsuite_filter_sub_ns_frag)

#$testsuite_for_assembly_name = "TextMetal.Middleware.Solder.Primitives"
#$testsuite_assembly_name = "TextMetal.Middleware.UnitTests"
#$testsuite_filter_sub_ns_frag = "Solder.Primitives._"

#$testsuite_for_assembly_name = "TextMetal.Middleware.Solder.Context"
#$testsuite_assembly_name = "TextMetal.Middleware.UnitTests"
#$testsuite_filter_sub_ns_frag = "Solder.Context._"

#$testsuite_for_assembly_name = "TextMetal.Middleware.Solder.Utilities"
#$testsuite_assembly_name = "TextMetal.Middleware.UnitTests"
#$testsuite_filter_sub_ns_frag = "Solder.Utilities._"

$testsuite_for_assembly_name = "TextMetal.Middleware.Solder.Utilities"
$testsuite_assembly_name = "TextMetal.Middleware.UnitTests"
$testsuite_filter_sub_ns_frag = "Solder.Utilities.Vfs._"


$testsuite_filter_namespace = "$testsuite_assembly_name.$testsuite_filter_sub_ns_frag"
$testsuite_assembly_dir = "$src_dir_path\$testsuite_assembly_name\bin\$build_flavor\$build_tfm"
$testsuite_assembly_path = "$testsuite_assembly_dir\$testsuite_assembly_name.dll"

$testsuite_filter = "--where class=~$testsuite_filter_namespace.*" # requires the wildcard suffix for class name (use of underscore in unit tests keeps out sub namespaces)
$coverage_filter = "+:$testsuite_for_assembly_name" # removed the wildcard suffix

$coverage_output_dir_path = "$output_dir_path\$testsuite_assembly_name\$testsuite_filter_sub_ns_frag"
$coverage_output_file_path_woext ="$coverage_output_dir_path\unit-test-coverage-report"

$target_exe = "$dotnet_exe"
$target_args = @("$testsuite_assembly_path", "$testsuite_filter")
$target_wdir = "."

if ((Test-Path -Path $coverage_output_dir_path))
{
	Remove-Item $coverage_output_dir_path -recurse -force
}

New-Item -ItemType directory -Path $coverage_output_dir_path

&$dotcover_exe analyse /Filters="$coverage_filter" `
	/TargetExecutable="$target_exe" `
	/TargetArguments="$target_args" `
	/TargetWorkingDir="$target_wdir" `
	/ReportType=HTML `
	/Output="$coverage_output_file_path_woext.html" > "$coverage_output_file_path_woext.log"

(New-Object -Com Shell.Application).Open("$coverage_output_file_path_woext.html")

if (!($LastExitCode -eq $null -or $LastExitCode -eq 0))
{ echo "An error occurred during the operation."; return; }

echo "The operation completed successfully."