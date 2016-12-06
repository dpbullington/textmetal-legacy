#
#	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
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

$sln_file_name = "TextMetal.sln"
$sln_file_path = "$src_dir_path\$sln_file_name"

$visual_studio_internal_version = "14.0"
$msbuild_dir_path = "C:\Program Files (x86)\MSBuild\$visual_studio_internal_version\bin\amd64"
$msbuild_file_name = "msbuild.exe"
$msbuild_exe = "$msbuild_dir_path\$msbuild_file_name"

$build_flavor = "debug"
$build_verbosity = "quiet"

echo "The operation is starting..."

&"$msbuild_exe" /verbosity:$build_verbosity /consoleloggerparameters:ErrorsOnly "$sln_file_path" /t:clean /p:Configuration="$build_flavor" /p:VisualStudioVersion=$visual_studio_internal_version

if (!($LastExitCode -eq $null -or $LastExitCode -eq 0))
{ echo "An error occurred during the operation."; return; }

&"$msbuild_exe" /verbosity:$build_verbosity /consoleloggerparameters:ErrorsOnly "$sln_file_path" /t:build /p:Configuration="$build_flavor" /p:VisualStudioVersion=$visual_studio_internal_version

if (!($LastExitCode -eq $null -or $LastExitCode -eq 0))
{ echo "An error occurred during the operation."; return; }

echo "The operation completed successfully."