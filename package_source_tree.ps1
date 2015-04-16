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
	Remove-Item $pkg_dir -recurse -force
}

New-Item -ItemType directory -Path $pkg_dir
New-Item -ItemType directory -Path ($pkg_dir + "\bin")

Copy-Item ".\trunk.bat" "$pkg_dir\."
Copy-Item "$doc_dir\LICENSE.txt" "$pkg_dir\."
Copy-Item "$lib_dir" "$pkg_dir\lib" -recurse
Copy-Item "$templates_dir" "$pkg_dir\templates" -recurse -exclude "*!git*"

Copy-Item "$src_dir\TextMetal.ConsoleTool\bin\$build_flavor_dir\TextMetal.exe" "$pkg_dir\bin\."
Copy-Item "$src_dir\TextMetal.ConsoleTool\bin\$build_flavor_dir\TextMetal.exe.config" "$pkg_dir\bin\."
Copy-Item "$src_dir\TextMetal.ConsoleTool\bin\$build_flavor_dir\TextMetal.xml" "$pkg_dir\bin\."
Copy-Item "$src_dir\TextMetal.ConsoleTool\bin\$build_flavor_dir\TextMetal.pdb" "$pkg_dir\bin\."

Copy-Item "$src_dir\TextMetal.Framework\bin\$build_flavor_dir\TextMetal.Framework.dll" "$pkg_dir\bin\."
Copy-Item "$src_dir\TextMetal.Framework\bin\$build_flavor_dir\TextMetal.Framework.xml" "$pkg_dir\bin\."
Copy-Item "$src_dir\TextMetal.Framework\bin\$build_flavor_dir\TextMetal.Framework.pdb" "$pkg_dir\bin\."

Copy-Item "$src_dir\TextMetal.Middleware.Common\bin\$build_flavor_dir\TextMetal.Middleware.Common.dll" "$pkg_dir\bin\."
Copy-Item "$src_dir\TextMetal.Middleware.Common\bin\$build_flavor_dir\TextMetal.Middleware.Common.xml" "$pkg_dir\bin\."
Copy-Item "$src_dir\TextMetal.Middleware.Common\bin\$build_flavor_dir\TextMetal.Middleware.Common.pdb" "$pkg_dir\bin\."

Copy-Item "$src_dir\TextMetal.Middleware.Data\bin\$build_flavor_dir\TextMetal.Middleware.Data.dll" "$pkg_dir\bin\."
Copy-Item "$src_dir\TextMetal.Middleware.Data\bin\$build_flavor_dir\TextMetal.Middleware.Data.xml" "$pkg_dir\bin\."
Copy-Item "$src_dir\TextMetal.Middleware.Data\bin\$build_flavor_dir\TextMetal.Middleware.Data.pdb" "$pkg_dir\bin\."

Copy-Item "$src_dir\TextMetal.Middleware.Data.Impl.FreakazoidMapper\bin\$build_flavor_dir\TextMetal.Middleware.Data.Impl.FreakazoidMapper.dll" "$pkg_dir\bin\."
Copy-Item "$src_dir\TextMetal.Middleware.Data.Impl.FreakazoidMapper\bin\$build_flavor_dir\TextMetal.Middleware.Data.Impl.FreakazoidMapper.xml" "$pkg_dir\bin\."
Copy-Item "$src_dir\TextMetal.Middleware.Data.Impl.FreakazoidMapper\bin\$build_flavor_dir\TextMetal.Middleware.Data.Impl.FreakazoidMapper.pdb" "$pkg_dir\bin\."

Copy-Item "$src_dir\TextMetal.Middleware.Data.Impl.MicrosoftLinq\bin\$build_flavor_dir\TextMetal.Middleware.Data.Impl.MicrosoftLinq.dll" "$pkg_dir\bin\."
Copy-Item "$src_dir\TextMetal.Middleware.Data.Impl.MicrosoftLinq\bin\$build_flavor_dir\TextMetal.Middleware.Data.Impl.MicrosoftLinq.xml" "$pkg_dir\bin\."
Copy-Item "$src_dir\TextMetal.Middleware.Data.Impl.MicrosoftLinq\bin\$build_flavor_dir\TextMetal.Middleware.Data.Impl.MicrosoftLinq.pdb" "$pkg_dir\bin\."

Copy-Item "$src_dir\TextMetal.Middleware.Solder\bin\$build_flavor_dir\TextMetal.Middleware.Solder.dll" "$pkg_dir\bin\."
Copy-Item "$src_dir\TextMetal.Middleware.Solder\bin\$build_flavor_dir\TextMetal.Middleware.Solder.xml" "$pkg_dir\bin\."
Copy-Item "$src_dir\TextMetal.Middleware.Solder\bin\$build_flavor_dir\TextMetal.Middleware.Solder.pdb" "$pkg_dir\bin\."

Copy-Item "$src_dir\TextMetal.Middleware.Testing\bin\$build_flavor_dir\TextMetal.Middleware.Testing.dll" "$pkg_dir\bin\."
Copy-Item "$src_dir\TextMetal.Middleware.Testing\bin\$build_flavor_dir\TextMetal.Middleware.Testing.xml" "$pkg_dir\bin\."
Copy-Item "$src_dir\TextMetal.Middleware.Testing\bin\$build_flavor_dir\TextMetal.Middleware.Testing.pdb" "$pkg_dir\bin\."

echo "The operation completed successfully."