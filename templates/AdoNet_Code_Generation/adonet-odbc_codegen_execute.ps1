#
#	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
#	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
#

cls
$root = [System.Environment]::CurrentDirectory

$src_dir = "..\..\src"
$build_flavor_dir = "Debug"
$template_dir = "."

$textmetal_exe = "$src_dir\TextMetal.HostImpl.ConsoleTool\bin\$build_flavor_dir\TextMetal.exe"

$template_file = "$template_dir\master_template.xml"
$source_file = "Driver={SQL Server Native Client 11.0};Server=(local);UID=textmetal_sample_mssql_lcl_login;PWD=LrJGmP6UfW8TEp7x3wWhECUYULE6zzMcWQ03R6UxeB4xzVmnq5S4Lx0vApegZVH;Database=textmetal_sample"
$base_dir = ".\output"
$source_strategy = "TextMetal.Framework.SourceModel.DatabaseSchema.Odbc.OdbcSchemaSourceStrategy, TextMetal.Framework.SourceModel"
$strict = $true
$property_connection_type = "System.Data.Odbc.OdbcConnection, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
$property_data_source_tag = "odbc.sqlserver"
$property_module_name = "TextMetal.Sample.DataModel"
$property_clr_namespace = "TextMetal.Sample.DataModel"
$property_model_clr_super_type = "ModelObject"
$property_request_model_clr_super_type = "RequestModelObject"
$property_result_model_clr_super_type = "ResultModelObject"
$property_response_model_clr_super_type = "ResponseModelObject"

$lib_dir = "..\..\lib"

$base_src_dir = "$base_dir\src"
$base_lib_dir = "$base_dir\lib"

$sn_exe = "C:\Program Files (x86)\Microsoft SDKs\Windows\v8.0A\bin\NETFX 4.0 Tools\sn.exe"
$snk_file = "$base_src_dir\$property_module_name.snk"

echo "The operation is starting..."

if ((Test-Path -Path $base_dir))
{
	Remove-Item $base_dir -recurse

	if (!($LastExitCode -eq $null -or $LastExitCode -eq 0))
	{ echo "An error occurred during the operation."; return; }
}

New-Item -ItemType directory -Path $base_dir

if (!($LastExitCode -eq $null -or $LastExitCode -eq 0))
{ echo "An error occurred during the operation."; return; }

$argz = @("-templatefile:$template_file",
	"-sourcefile:$source_file",
	"-basedir:$base_src_dir",
	"-sourcestrategy:$source_strategy",
	"-strict:$strict",
	"-property:ConnectionType=$property_connection_type",
	"-property:DataSourceTag=$property_data_source_tag",
	"-property:ModuleName=$property_module_name",
	"-property:ClrNamespace=$property_clr_namespace",
	"-property:ModelClrSuperType=$property_model_clr_super_type",
	"-property:RequestModelClrSuperType=$property_request_model_clr_super_type",
	"-property:ResultModelClrSuperType=$property_result_model_clr_super_type",
	"-property:ResponseModelClrSuperType=$property_response_model_clr_super_type")

&"$textmetal_exe" $argz

if (!($LastExitCode -eq $null -or $LastExitCode -eq 0))
{ echo "An error occurred during the operation."; return; }

$argz = @("-k",
	"$snk_file")

&"$sn_exe" $argz

if (!($LastExitCode -eq $null -or $LastExitCode -eq 0))
{ echo "An error occurred during the operation."; return; }


New-Item -ItemType directory -Path $base_lib_dir
New-Item -ItemType directory -Path "$base_lib_dir\TextMetal"
New-Item -ItemType directory -Path "$base_lib_dir\IronRuby"
New-Item -ItemType directory -Path "$base_lib_dir\Sqlite"
New-Item -ItemType directory -Path "$base_lib_dir\Sqlite\x64"
New-Item -ItemType directory -Path "$base_lib_dir\Sqlite\x86"

Copy-Item "$lib_dir\IronRuby\*.*" "$base_lib_dir\IronRuby\."
Copy-Item "$lib_dir\SQLite\x64\*.*" "$base_lib_dir\SQLite\x64\."
Copy-Item "$lib_dir\SQLite\x86\*.*" "$base_lib_dir\SQLite\x86\."

Copy-Item "$src_dir\TextMetal.Common.Core\bin\$build_flavor_dir\TextMetal.Common.Core.dll" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Common.Core\bin\$build_flavor_dir\TextMetal.Common.Core.xml" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Common.Core\bin\$build_flavor_dir\TextMetal.Common.Core.pdb" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Common.Data\bin\$build_flavor_dir\TextMetal.Common.Data.dll" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Common.Data\bin\$build_flavor_dir\TextMetal.Common.Data.xml" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Common.Data\bin\$build_flavor_dir\TextMetal.Common.Data.pdb" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Common.Solder\bin\$build_flavor_dir\TextMetal.Common.Solder.dll" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Common.Solder\bin\$build_flavor_dir\TextMetal.Common.Solder.xml" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Common.Solder\bin\$build_flavor_dir\TextMetal.Common.Solder.pdb" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Common.Syntax\bin\$build_flavor_dir\TextMetal.Common.Syntax.dll" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Common.Syntax\bin\$build_flavor_dir\TextMetal.Common.Syntax.xml" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Common.Syntax\bin\$build_flavor_dir\TextMetal.Common.Syntax.pdb" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Common.UnitTests\bin\$build_flavor_dir\TextMetal.Common.UnitTests.dll" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Common.UnitTests\bin\$build_flavor_dir\TextMetal.Common.UnitTests.xml" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Common.UnitTests\bin\$build_flavor_dir\TextMetal.Common.UnitTests.pdb" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Common.WinForms\bin\$build_flavor_dir\TextMetal.Common.WinForms.dll" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Common.WinForms\bin\$build_flavor_dir\TextMetal.Common.WinForms.xml" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Common.WinForms\bin\$build_flavor_dir\TextMetal.Common.WinForms.pdb" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Common.Xml\bin\$build_flavor_dir\TextMetal.Common.Xml.dll" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Common.Xml\bin\$build_flavor_dir\TextMetal.Common.Xml.xml" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Common.Xml\bin\$build_flavor_dir\TextMetal.Common.Xml.pdb" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Framework.AssociativeModel\bin\$build_flavor_dir\TextMetal.Framework.AssociativeModel.dll" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Framework.AssociativeModel\bin\$build_flavor_dir\TextMetal.Framework.AssociativeModel.xml" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Framework.AssociativeModel\bin\$build_flavor_dir\TextMetal.Framework.AssociativeModel.pdb" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Framework.Core\bin\$build_flavor_dir\TextMetal.Framework.Core.dll" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Framework.Core\bin\$build_flavor_dir\TextMetal.Framework.Core.xml" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Framework.Core\bin\$build_flavor_dir\TextMetal.Framework.Core.pdb" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Framework.DebuggerProfilerModel\bin\$build_flavor_dir\TextMetal.Framework.DebuggerProfilerModel.dll" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Framework.DebuggerProfilerModel\bin\$build_flavor_dir\TextMetal.Framework.DebuggerProfilerModel.xml" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Framework.DebuggerProfilerModel\bin\$build_flavor_dir\TextMetal.Framework.DebuggerProfilerModel.pdb" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Framework.ExpressionModel\bin\$build_flavor_dir\TextMetal.Framework.ExpressionModel.dll" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Framework.ExpressionModel\bin\$build_flavor_dir\TextMetal.Framework.ExpressionModel.xml" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Framework.ExpressionModel\bin\$build_flavor_dir\TextMetal.Framework.ExpressionModel.pdb" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Framework.HostingModel\bin\$build_flavor_dir\TextMetal.Framework.HostingModel.dll" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Framework.HostingModel\bin\$build_flavor_dir\TextMetal.Framework.HostingModel.xml" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Framework.HostingModel\bin\$build_flavor_dir\TextMetal.Framework.HostingModel.pdb" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Framework.InputOutputModel\bin\$build_flavor_dir\TextMetal.Framework.InputOutputModel.dll" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Framework.InputOutputModel\bin\$build_flavor_dir\TextMetal.Framework.InputOutputModel.xml" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Framework.InputOutputModel\bin\$build_flavor_dir\TextMetal.Framework.InputOutputModel.pdb" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Framework.IntegrationTests\bin\$build_flavor_dir\TextMetal.Framework.IntegrationTests.dll" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Framework.IntegrationTests\bin\$build_flavor_dir\TextMetal.Framework.IntegrationTests.xml" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Framework.IntegrationTests\bin\$build_flavor_dir\TextMetal.Framework.IntegrationTests.pdb" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Framework.SortModel\bin\$build_flavor_dir\TextMetal.Framework.SortModel.dll" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Framework.SortModel\bin\$build_flavor_dir\TextMetal.Framework.SortModel.xml" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Framework.SortModel\bin\$build_flavor_dir\TextMetal.Framework.SortModel.pdb" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Framework.SourceModel\bin\$build_flavor_dir\TextMetal.Framework.SourceModel.dll" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Framework.SourceModel\bin\$build_flavor_dir\TextMetal.Framework.SourceModel.xml" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Framework.SourceModel\bin\$build_flavor_dir\TextMetal.Framework.SourceModel.pdb" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Framework.TemplateModel\bin\$build_flavor_dir\TextMetal.Framework.TemplateModel.dll" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Framework.TemplateModel\bin\$build_flavor_dir\TextMetal.Framework.TemplateModel.xml" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Framework.TemplateModel\bin\$build_flavor_dir\TextMetal.Framework.TemplateModel.pdb" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Framework.UnitTests\bin\$build_flavor_dir\TextMetal.Framework.UnitTests.dll" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Framework.UnitTests\bin\$build_flavor_dir\TextMetal.Framework.UnitTests.xml" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Framework.UnitTests\bin\$build_flavor_dir\TextMetal.Framework.UnitTests.pdb" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.HostImpl.ConsoleTool\bin\$build_flavor_dir\TextMetal.exe" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.HostImpl.ConsoleTool\bin\$build_flavor_dir\TextMetal.xml" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.HostImpl.ConsoleTool\bin\$build_flavor_dir\TextMetal.pdb" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.HostImpl.WindowsTool\bin\$build_flavor_dir\TextMetalStudio.exe" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.HostImpl.WindowsTool\bin\$build_flavor_dir\TextMetalStudio.xml" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.HostImpl.WindowsTool\bin\$build_flavor_dir\TextMetalStudio.pdb" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.HostImpl.Tool\bin\$build_flavor_dir\TextMetal.HostImpl.Tool.dll" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.HostImpl.Tool\bin\$build_flavor_dir\TextMetal.HostImpl.Tool.xml" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.HostImpl.Tool\bin\$build_flavor_dir\TextMetal.HostImpl.Tool.pdb" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Imports\bin\$build_flavor_dir\TextMetal.Imports.dll" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Imports\bin\$build_flavor_dir\TextMetal.Imports.xml" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Imports\bin\$build_flavor_dir\TextMetal.Imports.pdb" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Imports.nunit.console.exe\bin\$build_flavor_dir\TextMetal.Imports.nunit.console.exe" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Imports.nunit.console.exe\bin\$build_flavor_dir\TextMetal.Imports.nunit.console.xml" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Imports.nunit.console.exe\bin\$build_flavor_dir\TextMetal.Imports.nunit.console.pdb" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Imports.nunit.gui.exe\bin\$build_flavor_dir\TextMetal.Imports.nunit.gui.exe" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Imports.nunit.gui.exe\bin\$build_flavor_dir\TextMetal.Imports.nunit.gui.xml" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.Imports.nunit.gui.exe\bin\$build_flavor_dir\TextMetal.Imports.nunit.gui.pdb" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.TestFramework\bin\$build_flavor_dir\TextMetal.TestFramework.dll" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.TestFramework\bin\$build_flavor_dir\TextMetal.TestFramework.xml" "$base_lib_dir\TextMetal\."
Copy-Item "$src_dir\TextMetal.TestFramework\bin\$build_flavor_dir\TextMetal.TestFramework.pdb" "$base_lib_dir\TextMetal\."


echo "The operation completed successfully."