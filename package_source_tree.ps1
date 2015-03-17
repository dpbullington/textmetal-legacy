#
#	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
#	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
#

cls
$root = [System.Environment]::CurrentDirectory

$build_flavor_dir = "Debug"
$build_tool_cfg = "Debug"
$doc_dir = ".\doc"
$src_dir = ".\src"
$lib_dir = ".\lib"
$templates_dir = ".\templates"
$pkg_dir = ".\pkg"

$msbuild_exe = "C:\Program Files (x86)\MSBuild\12.0\bin\amd64\msbuild.exe"

echo "The operation is starting..."

if ((Test-Path -Path $pkg_dir))
{
	Remove-Item $pkg_dir -recurse

	if (!($LastExitCode -eq $null -or $LastExitCode -eq 0))
	{ echo "An error occurred during the operation."; return; }
}

New-Item -ItemType directory -Path $pkg_dir

if (!($LastExitCode -eq $null -or $LastExitCode -eq 0))
{ echo "An error occurred during the operation."; return; }


New-Item -ItemType directory -Path ($pkg_dir + "\bin")

if (!($LastExitCode -eq $null -or $LastExitCode -eq 0))
{ echo "An error occurred during the operation."; return; }


Copy-Item "$lib_dir" "$pkg_dir\lib" -recurse

if (!($LastExitCode -eq $null -or $LastExitCode -eq 0))
{ echo "An error occurred during the operation."; return; }


Copy-Item "$templates_dir" "$pkg_dir\templates" -recurse -exclude "*!git*"
if (!($LastExitCode -eq $null -or $LastExitCode -eq 0))
{ echo "An error occurred during the operation."; return; }


Copy-Item "$lib_dir\IronRuby\*.*" "$pkg_dir\bin\." -recurse

if (!($LastExitCode -eq $null -or $LastExitCode -eq 0))
{ echo "An error occurred during the operation."; return; }


Copy-Item "$lib_dir\LeastViable\*.*" "$pkg_dir\bin\." -recurse

if (!($LastExitCode -eq $null -or $LastExitCode -eq 0))
{ echo "An error occurred during the operation."; return; }


Copy-Item ".\trunk.bat" "$pkg_dir\."

if (!($LastExitCode -eq $null -or $LastExitCode -eq 0))
{ echo "An error occurred during the operation."; return; }


Copy-Item "$doc_dir\IMPORTS.txt" "$pkg_dir\."

if (!($LastExitCode -eq $null -or $LastExitCode -eq 0))
{ echo "An error occurred during the operation."; return; }


Copy-Item "$doc_dir\LICENSE.txt" "$pkg_dir\."

if (!($LastExitCode -eq $null -or $LastExitCode -eq 0))
{ echo "An error occurred during the operation."; return; }




Copy-Item "$src_dir\TextMetal.Framework\bin\$build_flavor_dir\TextMetal.Framework.dll" "$pkg_dir\bin\."

if (!($LastExitCode -eq $null -or $LastExitCode -eq 0))
{ echo "An error occurred during the operation."; return; }

Copy-Item "$src_dir\TextMetal.Framework\bin\$build_flavor_dir\TextMetal.Framework.xml" "$pkg_dir\bin\."

if (!($LastExitCode -eq $null -or $LastExitCode -eq 0))
{ echo "An error occurred during the operation."; return; }

Copy-Item "$src_dir\TextMetal.Framework\bin\$build_flavor_dir\TextMetal.Framework.pdb" "$pkg_dir\bin\."

if (!($LastExitCode -eq $null -or $LastExitCode -eq 0))
{ echo "An error occurred during the operation."; return; }



Copy-Item "$src_dir\TextMetal.Framework.IntegrationTests\bin\$build_flavor_dir\TextMetal.Framework.IntegrationTests.dll" "$pkg_dir\bin\."

if (!($LastExitCode -eq $null -or $LastExitCode -eq 0))
{ echo "An error occurred during the operation."; return; }

Copy-Item "$src_dir\TextMetal.Framework.IntegrationTests\bin\$build_flavor_dir\TextMetal.Framework.IntegrationTests.xml" "$pkg_dir\bin\."

if (!($LastExitCode -eq $null -or $LastExitCode -eq 0))
{ echo "An error occurred during the operation."; return; }

Copy-Item "$src_dir\TextMetal.Framework.IntegrationTests\bin\$build_flavor_dir\TextMetal.Framework.IntegrationTests.pdb" "$pkg_dir\bin\."

if (!($LastExitCode -eq $null -or $LastExitCode -eq 0))
{ echo "An error occurred during the operation."; return; }



Copy-Item "$src_dir\TextMetal.Framework.UnitTests\bin\$build_flavor_dir\TextMetal.Framework.UnitTests.dll" "$pkg_dir\bin\."

if (!($LastExitCode -eq $null -or $LastExitCode -eq 0))
{ echo "An error occurred during the operation."; return; }

Copy-Item "$src_dir\TextMetal.Framework.UnitTests\bin\$build_flavor_dir\TextMetal.Framework.UnitTests.xml" "$pkg_dir\bin\."

if (!($LastExitCode -eq $null -or $LastExitCode -eq 0))
{ echo "An error occurred during the operation."; return; }

Copy-Item "$src_dir\TextMetal.Framework.UnitTests\bin\$build_flavor_dir\TextMetal.Framework.UnitTests.pdb" "$pkg_dir\bin\."

if (!($LastExitCode -eq $null -or $LastExitCode -eq 0))
{ echo "An error occurred during the operation."; return; }



Copy-Item "$src_dir\TextMetal.ConsoleTool\bin\$build_flavor_dir\TextMetal.exe" "$pkg_dir\bin\."

if (!($LastExitCode -eq $null -or $LastExitCode -eq 0))
{ echo "An error occurred during the operation."; return; }

Copy-Item "$src_dir\TextMetal.ConsoleTool\bin\$build_flavor_dir\TextMetal.exe.config" "$pkg_dir\bin\."

if (!($LastExitCode -eq $null -or $LastExitCode -eq 0))
{ echo "An error occurred during the operation."; return; }

Copy-Item "$src_dir\TextMetal.ConsoleTool\bin\$build_flavor_dir\TextMetal.xml" "$pkg_dir\bin\."

if (!($LastExitCode -eq $null -or $LastExitCode -eq 0))
{ echo "An error occurred during the operation."; return; }

Copy-Item "$src_dir\TextMetal.ConsoleTool\bin\$build_flavor_dir\TextMetal.pdb" "$pkg_dir\bin\."

if (!($LastExitCode -eq $null -or $LastExitCode -eq 0))
{ echo "An error occurred during the operation."; return; }



echo "The operation completed successfully."