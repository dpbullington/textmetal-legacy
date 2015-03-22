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
$property_module_name = "TextMetal.Sample.DomainModel"
$property_clr_namespace = "TextMetal.Sample.DomainModel"
$property_model_clr_super_type = "TableModelObject"

$lib_dir = "..\..\lib"

$base_src_dir = "$base_dir\src"
$base_lib_dir = "$base_dir\lib"

$sn_exe = "C:\Program Files (x86)\Microsoft SDKs\Windows\v8.0A\bin\NETFX 4.0 Tools\sn.exe"
$snk_file = "$base_src_dir\$property_module_name.snk"

$sql_metal_exe = "C:\Program Files (x86)\Microsoft SDKs\Windows\v8.0A\bin\NETFX 4.0 Tools\SqlMetal.exe"

$l2s_dir = "$base_src_dir\$property_module_name"
$l2s_clr_namespace = "$property_clr_namespace"
$l2s_data_context_name = "TextMetalDataContext"
$l2s_dbml_file = "$l2s_dir\$l2s_data_context_name.dbml"
$l2s_designer_cs_file = "$l2s_dir\$l2s_data_context_name.designer.cs"

echo "The operation is starting..."

if ((Test-Path -Path $base_dir))
{
	Remove-Item $base_dir -recurse -force

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
	"-property:LinqToSqlTargetDataContextName=$l2s_data_context_name")

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


Copy-Item "$lib_dir\LeastViable\*.*" "$base_lib_dir\LeastViable\."

if (!($LastExitCode -eq $null -or $LastExitCode -eq 0))
{ echo "An error occurred during the operation."; return; }


if ((Test-Path -Path $base_dir) -eq $false)
{
	New-Item -ItemType directory -Path "$l2s_dir"
}

$argz = @("/views",
	"/sprocs",
	"/language:C#",
	"/pluralize",
	"/namespace:$l2s_clr_namespace",
	"/context:$l2s_data_context_name",
	"/dbml:$l2s_dbml_file",
	"/conn:$source_file")

&"$sql_metal_exe" $argz > "$base_dir\SqlMetal_LOG.txt"

if (!($LastExitCode -eq $null -or $LastExitCode -eq 0))
{ echo "An error occurred during the operation."; return; }




$xd = New-Object System.Xml.XmlDocument
$xd.Load($l2s_dbml_file)
$xa_list = $xd.SelectNodes("//@CanBeNull")

if ($xa_list -ne $null)
{
	$xa_list.Count

	foreach ($xa_item in $xa_list)
	{
		$xa_item.Value = "true"
	}
}
else
{
	echo "Warning: no XPATH results."
}

$xd.Save($l2s_dbml_file)




$argz = @("/language:C#",
	"/code:$l2s_designer_cs_file",
	"$l2s_dbml_file")

&"$sql_metal_exe" $argz >> "$base_dir\SqlMetal_LOG.txt"

if (!($LastExitCode -eq $null -or $LastExitCode -eq 0))
{ echo "An error occurred during the operation."; return; }

echo "The operation completed successfully."