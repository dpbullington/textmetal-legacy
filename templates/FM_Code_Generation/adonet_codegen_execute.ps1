#
#	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
#	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
#

cls
$root = [System.Environment]::CurrentDirectory

$src_dir = "..\..\src"
$build_flavor_dir = "Debug"
$template_dir = "."

$textmetal_exe = "$src_dir\TextMetal.ConsoleTool\bin\$build_flavor_dir\TextMetal.exe"

$template_file = "$template_dir\master_template.xml"
$source_file = "Server=(local);User ID=textmetal_sample_mssql_lcl_login;PWD=LrJGmP6UfW8TEp7x3wWhECUYULE6zzMcWQ03R6UxeB4xzVmnq5S4Lx0vApegZVH;Database=textmetal_sample"
$base_dir = ".\output"
$source_strategy = "TextMetal.Framework.Source.DatabaseSchema.Sql.SqlSchemaSourceStrategy, TextMetal.Framework"
$strict = $true
$property_connection_type = "System.Data.SqlClient.SqlConnection, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
$property_data_source_tag = "net.sqlserver"
$property_module_name = "TextMetal.Sample.DataModel"
$property_clr_namespace = "TextMetal.Sample.DataModel"
$property_table_model_clr_super_type = "TableModelObject"
$property_call_procedure_model_clr_super_type = "CallProcedureModelObject"
$property_result_model_clr_super_type = "ResultModelObject"
$property_return_procedure_model_clr_super_type = "ReturnProcedureModelObject"

$lib_dir = "..\..\lib"

$base_src_dir = "$base_dir\src"
$base_lib_dir = "$base_dir\lib"

$sn_exe = "C:\Program Files (x86)\Microsoft SDKs\Windows\v8.0A\bin\NETFX 4.0 Tools\sn.exe"
$snk_file = "$base_src_dir\$property_module_name.snk"

echo "The operation is starting..."

if ((Test-Path -Path $base_dir))
{
	Remove-Item $base_dir -recurse -force
}

New-Item -ItemType directory -Path $base_dir

$argz = @("-templatefile:$template_file",
	"-sourcefile:$source_file",
	"-basedir:$base_src_dir",
	"-sourcestrategy:$source_strategy",
	"-strict:$strict",
	"-property:ConnectionType=$property_connection_type",
	"-property:DataSourceTag=$property_data_source_tag",
	"-property:ModuleName=$property_module_name",
	"-property:ClrNamespace=$property_clr_namespace",
	"-property:TableModelClrSuperType=$property_table_model_clr_super_type",
	"-property:CallProcedureModelClrSuperType=$property_call_procedure_model_clr_super_type",
	"-property:ResultModelClrSuperType=$property_result_model_clr_super_type",
	"-property:ReturnProcedureClrSuperType=$property_return_procedure_model_clr_super_type")

&"$textmetal_exe" $argz

if (!($LastExitCode -eq $null -or $LastExitCode -eq 0))
{ echo "An error occurred during the operation."; return; }

$argz = @("-k",
	"$snk_file")

&"$sn_exe" $argz

if (!($LastExitCode -eq $null -or $LastExitCode -eq 0))
{ echo "An error occurred during the operation."; return; }

New-Item -ItemType directory -Path $base_lib_dir
New-Item -ItemType directory -Path "$base_lib_dir\LeastViable"
New-Item -ItemType directory -Path "$base_lib_dir\Newtonsoft.Json"
New-Item -ItemType directory -Path "$base_lib_dir\Sqlite"
New-Item -ItemType directory -Path "$base_lib_dir\Sqlite\x64"
New-Item -ItemType directory -Path "$base_lib_dir\Sqlite\x86"

Copy-Item "$lib_dir\SQLite\x64\*.*" "$base_lib_dir\SQLite\x64\."
Copy-Item "$lib_dir\SQLite\x86\*.*" "$base_lib_dir\SQLite\x86\."
Copy-Item "$lib_dir\LeastViable\*.*" "$base_lib_dir\LeastViable\."
Copy-Item "$lib_dir\Newtonsoft.Json\*.*" "$base_lib_dir\Newtonsoft.Json\."

echo "The operation completed successfully."