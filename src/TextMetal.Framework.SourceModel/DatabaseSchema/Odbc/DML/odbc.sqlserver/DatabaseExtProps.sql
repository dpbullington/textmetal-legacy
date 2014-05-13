/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

-- extended properties for server
SELECT 'BuildClrVersion' AS [PropertyName], SERVERPROPERTY('BuildClrVersion') AS [PropertyValue] UNION ALL
SELECT 'Collation' AS [PropertyName], SERVERPROPERTY('Collation') AS [PropertyValue] UNION ALL
SELECT 'CollationID' AS [PropertyName], SERVERPROPERTY('CollationID') AS [PropertyValue] UNION ALL
SELECT 'ComparisonStyle' AS [PropertyName], SERVERPROPERTY('ComparisonStyle') AS [PropertyValue] UNION ALL
SELECT 'ComputerNamePhysicalNetBIOS' AS [PropertyName], SERVERPROPERTY('ComputerNamePhysicalNetBIOS') AS [PropertyValue] UNION ALL
SELECT 'Edition' AS [PropertyName], SERVERPROPERTY('Edition') AS [PropertyValue] UNION ALL
SELECT 'EditionID' AS [PropertyName], SERVERPROPERTY('EditionID') AS [PropertyValue] UNION ALL
SELECT 'EngineEdition' AS [PropertyName], SERVERPROPERTY('EngineEdition') AS [PropertyValue] UNION ALL
SELECT 'FilestreamConfiguredLevel' AS [PropertyName], SERVERPROPERTY('FilestreamConfiguredLevel') AS [PropertyValue] UNION ALL
SELECT 'FilestreamEffectiveLevel' AS [PropertyName], SERVERPROPERTY('FilestreamEffectiveLevel') AS [PropertyValue] UNION ALL
SELECT 'FilestreamShareName' AS [PropertyName], SERVERPROPERTY('FilestreamShareName') AS [PropertyValue] UNION ALL
SELECT 'HadrManagerStatus' AS [PropertyName], SERVERPROPERTY('HadrManagerStatus') AS [PropertyValue] UNION ALL
SELECT 'InstanceName' AS [PropertyName], SERVERPROPERTY('InstanceName') AS [PropertyValue] UNION ALL
SELECT 'IsClustered' AS [PropertyName], SERVERPROPERTY('IsClustered') AS [PropertyValue] UNION ALL
SELECT 'IsFullTextInstalled' AS [PropertyName], SERVERPROPERTY('IsFullTextInstalled') AS [PropertyValue] UNION ALL
SELECT 'IsHadrEnabled' AS [PropertyName], SERVERPROPERTY('IsHadrEnabled') AS [PropertyValue] UNION ALL
SELECT 'IsIntegratedSecurityOnly' AS [PropertyName], SERVERPROPERTY('IsIntegratedSecurityOnly') AS [PropertyValue] UNION ALL
SELECT 'IsLocalDB' AS [PropertyName], SERVERPROPERTY('IsLocalDB') AS [PropertyValue] UNION ALL
SELECT 'IsSingleUser' AS [PropertyName], SERVERPROPERTY('IsSingleUser') AS [PropertyValue] UNION ALL
SELECT 'LCID' AS [PropertyName], SERVERPROPERTY('LCID') AS [PropertyValue] UNION ALL
SELECT 'LicenseType' AS [PropertyName], SERVERPROPERTY('LicenseType') AS [PropertyValue] UNION ALL
SELECT 'MachineName' AS [PropertyName], SERVERPROPERTY('MachineName') AS [PropertyValue] UNION ALL
SELECT 'NumLicenses' AS [PropertyName], SERVERPROPERTY('NumLicenses') AS [PropertyValue] UNION ALL
SELECT 'ProcessID' AS [PropertyName], SERVERPROPERTY('ProcessID') AS [PropertyValue] UNION ALL
SELECT 'ProductLevel' AS [PropertyName], SERVERPROPERTY('ProductLevel') AS [PropertyValue] UNION ALL
SELECT 'ProductVersion' AS [PropertyName], SERVERPROPERTY('ProductVersion') AS [PropertyValue] UNION ALL
SELECT 'ResourceLastUpdateDateTime' AS [PropertyName], SERVERPROPERTY('ResourceLastUpdateDateTime') AS [PropertyValue] UNION ALL
SELECT 'ResourceVersion' AS [PropertyName], SERVERPROPERTY('ResourceVersion') AS [PropertyValue] UNION ALL
SELECT 'ServerName' AS [PropertyName], SERVERPROPERTY('ServerName') AS [PropertyValue] UNION ALL
SELECT 'SqlCharSet' AS [PropertyName], SERVERPROPERTY('SqlCharSet') AS [PropertyValue] UNION ALL
SELECT 'SqlCharSetName' AS [PropertyName], SERVERPROPERTY('SqlCharSetName') AS [PropertyValue] UNION ALL
SELECT 'SqlSortOrder' AS [PropertyName], SERVERPROPERTY('SqlSortOrder') AS [PropertyValue] UNION ALL
SELECT 'SqlSortOrderName' AS [PropertyName], SERVERPROPERTY('SqlSortOrderName') AS [PropertyValue];