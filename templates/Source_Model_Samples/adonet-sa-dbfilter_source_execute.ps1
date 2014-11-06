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

$template_file = "$template_dir\empty_template.xml"
$source_file = "Server=(local);User ID=sa;PWD=???"
$base_dir = ".\output"
$source_strategy = "TextMetal.Framework.SourceModel.DatabaseSchema.Sql.SqlSchemaSourceStrategy, TextMetal.Framework.SourceModel"
$strict = $true
$property_connection_type = "System.Data.SqlClient.SqlConnection, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
$property_data_source_tag = "net.sqlserver"
$property_enable_database_filter = $true
$property_database_filter = "???"

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
	"-basedir:$base_dir",
	"-sourcestrategy:$source_strategy",
	"-strict:$strict",
	"-property:ConnectionType=$property_connection_type",
	"-property:DataSourceTag=$property_data_source_tag",
	"-property:EnableDatabaseFilter=$property_enable_database_filter",
	"-property:DatabaseFilter=$property_database_filter")

&"$textmetal_exe" $argz

if (!($LastExitCode -eq $null -or $LastExitCode -eq 0))
{ echo "An error occurred during the operation."; return; }

echo "The operation completed successfully."