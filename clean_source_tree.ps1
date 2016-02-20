#
#	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
#	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
#

cls
$this_dir_path = [System.Environment]::CurrentDirectory

$removeItemSpecs = @("artifacts", "obj", "bin", "debug", "release",
	"clientbin", "output", "pkg", "_ReSharper.*", ".partial", "*.suo", "*.user", "*.cache", "*.resharper",
	"*.visualstate.xml", "*.pidb", "*.userprefs", "*.DotSettings", "thumbs.db", "desktop.ini", ".DS_Store", "TestResult.xml")

$removeItems = Get-ChildItem $this_dir_path -Recurse -Include $removeItemSpecs -Force

foreach ($removeItem in $removeItems)
{
	"Deleting item: " + $removeItem.FullName

	try { Remove-Item $removeItem.FullName -Recurse -Force } catch { }
}
