#
#	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
#	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
#

$lib_dir = ".\lib"
$src_dir = ".\src"
$output_base_dir = ".\output"
$cover_exe = "$Env:USERPROFILE\AppData\Local\JetBrains\Installations\dotCover01\dotCover.exe"
$nunit_exe = ".\$lib_dir\NUnit\nunit-console.exe"

$exclude_filter = "-:nunit-console;-:TextMetal.Middleware.Testing;-:TextMetal.Framework.UnitTests"
$target_assembly = "TextMetal.Framework.UnitTests"
$target_namespaces = @("TextMetal.Framework.UnitTests")

echo "The operation is starting..."

if ($target_namespaces -ne $null)
{
	foreach ($target_namespace in $target_namespaces)
	{

		&$cover_exe analyse /Filters="$exclude_filter" `
			/TargetExecutable="$nunit_exe" `
			/TargetArguments="/run:$target_namespace .\$src_dir\$target_assembly\bin\Debug\$target_assembly.dll /framework:net-4.5.1" `
			/TargetWorkingDir="" /ReportType=HTML `
			/Output="$output_base_dir\$target_namespace\$target_namespace-unit-tests.html"

		if (!($LastExitCode -eq $null -or $LastExitCode -eq 0))
		{ echo "An error occurred during the operation."; return; }

	}
}


$exclude_filter = "-:nunit-console;-:TextMetal.Middleware.Testing;-:TextMetal.Middleware.UnitTests"
$target_assembly = "TextMetal.Middleware.UnitTests"
$target_namespaces = @("TextMetal.Middleware.UnitTests")

echo "The operation is starting..."

if ($target_namespaces -ne $null)
{
	foreach ($target_namespace in $target_namespaces)
	{

		&$cover_exe analyse /Filters="$exclude_filter" `
			/TargetExecutable="$nunit_exe" `
			/TargetArguments="/run:$target_namespace .\$src_dir\$target_assembly\bin\Debug\$target_assembly.dll /framework:net-4.5.1" `
			/TargetWorkingDir="" /ReportType=HTML `
			/Output="$output_base_dir\$target_namespace\$target_namespace-unit-tests.html"

		if (!($LastExitCode -eq $null -or $LastExitCode -eq 0))
		{ echo "An error occurred during the operation."; return; }

	}
}

echo "The operation completed successfully."