#
#	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
#	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
#

$lib_dir = ".\lib"
$src_dir = ".\src"
$output_base_dir = ".\output"

$cover_exe = "C:\Program Files (x86)\JetBrains\Installations\dotCover04\dotCover.exe"

$target_assembly = "TextMetal.Middleware.UnitTests"
$target_namespaces = @("TextMetal.Framework.UnitTests")
#$target_exe = "$Env:USERPROFILE\.dnx\runtimes\dnx-coreclr-win-x64.1.0.0-rc1-update1\bin\dnx.exe"
#$target_args = @("-p", ".\$src_dir\$target_assembly", "run", "--where", "class=TextMetal.Middleware.UnitTests.Solder.Utilities._.AppConfigFascadeTests")

$target_exe = "C:\Program Files\dotnet\bin\dotnet.exe"

$target_args = @("compile", ".\$src_dir\$target_assembly")

&"$target_exe" $target_args
return $null;

$target_args = @("run", "-p", ".\$src_dir\$target_assembly", "/where", "class=TextMetal.Middleware.UnitTests.Solder.Utilities._.AppConfigFascadeTests")

echo "The operation is starting..."

if ($target_namespaces -ne $null)
{
	foreach ($target_namespace in $target_namespaces)
	{

		&$cover_exe analyse /Filters="$exclude_filter" `
			/TargetExecutable="$target_exe" `
			/TargetArguments="$target_args" `
			/TargetWorkingDir="" /ReportType=HTML `
			/Output="$output_base_dir\$target_namespace\$target_namespace-unit-tests.html"

		if (!($LastExitCode -eq $null -or $LastExitCode -eq 0))
		{ echo "An error occurred during the operation."; return; }

	}
}

echo "The operation completed successfully."