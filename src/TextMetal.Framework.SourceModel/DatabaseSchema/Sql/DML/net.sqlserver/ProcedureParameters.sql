/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

-- parameters[schema, procedure]
-- DECLARE @SchemaName [nvarchar](255); SET @SchemaName = 'testcases'; DECLARE @ProcedureName [nvarchar](255); SET @ProcedureName = 'sproc_simple';

IF CAST(SERVERPROPERTY('ProductVersion') AS [nvarchar](255)) LIKE '8.%' OR
	CAST(SERVERPROPERTY('ProductVersion') AS [nvarchar](255)) LIKE '9.%'
BEGIN

SELECT
	sys_pm.[parameter_id] AS [ParameterOrdinal],
	sys_s.[name] AS [SchemaName],
	sys_p.[name] AS [ProcedureName],
	sys_pm.[name] AS [ParameterName],
	sys_pm.[max_length] as [ParameterSize],
	sys_pm.[precision] as [ParameterPrecision],
	sys_pm.[scale] as [ParameterScale],
	CASE WHEN sys_ty_u.[system_type_id] IS NOT NULL THEN sys_ty_u.[name] ELSE sys_ty.[name] END AS [ParameterSqlType],
	CAST(CASE WHEN sys_ty_u.[system_type_id] IS NOT NULL THEN 1 ELSE 0 END AS [bit]) AS [ParameterIsUserDefinedType],
	sys_pm.[is_output] as [ParameterIsOutput],
	sys_pm.[is_cursor_ref] as [ParameterIsCursorRef],
	sys_pm.[has_default_value] AS [ParameterHasDefault],
	sys_pm.[default_value] AS [ParameterDefaultValue],
	CAST(0 AS [bit]) as [ParameterIsReadOnly],
	--sys_pm.[is_readonly] as [ParameterIsReadOnly], -- SQL 2008+
	NULL AS [ParameterNullable],
	--sys_pm.[is_nullable] AS [ParameterNullable], -- SQL 2014+
	CAST(0 AS [bit]) as [ParameterIsReturnValue],
	CAST(0 AS [bit]) as [ParameterIsResultColumn]
FROM
    [sys].[parameters] sys_pm
	INNER JOIN [sys].[procedures] sys_p ON sys_p.[object_id] = sys_pm.[object_id]
	-- NO NEED TO JOIN ON [sys].[objects] sys_o
	INNER JOIN [sys].[schemas] sys_s ON sys_s.[schema_id] = sys_p.[schema_id]
	INNER JOIN [sys].[types] sys_ty ON sys_ty.[user_type_id] = sys_pm.[user_type_id]
	LEFT OUTER JOIN [sys].[types] sys_ty_u ON
		sys_ty.[is_user_defined] = 1
		AND sys_ty_u.[system_type_id] = sys_ty.[system_type_id]
WHERE
	sys_s.[name] = @SchemaName
	AND sys_p.[name] = @ProcedureName
ORDER BY
	sys_pm.[parameter_id] ASC

END
ELSE
BEGIN

SELECT
	sys_pm.[parameter_id] AS [ParameterOrdinal],
	sys_s.[name] AS [SchemaName],
	sys_p.[name] AS [ProcedureName],
	sys_pm.[name] AS [ParameterName],
	sys_pm.[max_length] as [ParameterSize],
	sys_pm.[precision] as [ParameterPrecision],
	sys_pm.[scale] as [ParameterScale],
	CASE WHEN sys_ty_u.[system_type_id] IS NOT NULL THEN sys_ty_u.[name] ELSE sys_ty.[name] END AS [ParameterSqlType],
	CAST(CASE WHEN sys_ty_u.[system_type_id] IS NOT NULL THEN 1 ELSE 0 END AS [bit]) AS [ParameterIsUserDefinedType],
	sys_pm.[is_output] as [ParameterIsOutput],
	sys_pm.[is_cursor_ref] as [ParameterIsCursorRef],
	sys_pm.[has_default_value] AS [ParameterHasDefault],
	sys_pm.[default_value] AS [ParameterDefaultValue],
	sys_pm.[is_readonly] as [ParameterIsReadOnly], -- SQL 2008+
	NULL AS [ParameterNullable],
	--sys_pm.[is_nullable] AS [ParameterNullable], -- SQL 2014+
	CAST(0 AS [bit]) as [ParameterIsReturnValue],
	CAST(0 AS [bit]) as [ParameterIsResultColumn]
FROM
    [sys].[parameters] sys_pm
	INNER JOIN [sys].[procedures] sys_p ON sys_p.[object_id] = sys_pm.[object_id]
	-- NO NEED TO JOIN ON [sys].[objects] sys_o
	INNER JOIN [sys].[schemas] sys_s ON sys_s.[schema_id] = sys_p.[schema_id]
	INNER JOIN [sys].[types] sys_ty ON sys_ty.[user_type_id] = sys_pm.[user_type_id]
	LEFT OUTER JOIN [sys].[types] sys_ty_u ON
		sys_ty.[is_user_defined] = 1
		AND sys_ty_u.[system_type_id] = sys_ty.[system_type_id]
WHERE
	sys_s.[name] = @SchemaName
	AND sys_p.[name] = @ProcedureName
ORDER BY
	sys_pm.[parameter_id] ASC

END
