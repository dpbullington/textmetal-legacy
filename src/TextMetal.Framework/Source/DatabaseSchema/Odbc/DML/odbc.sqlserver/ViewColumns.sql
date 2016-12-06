/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

-- columns[schema, view]
-- DECLARE @SchemaName [nvarchar](255); SET @SchemaName = 'dbo'; DECLARE @ViewName [nvarchar](255); SET @ViewName = 'EventLogExtent';
SELECT
	sys_c.[column_id] AS [ColumnOrdinal],
	CASE WHEN LEN(ISNULL(sys_c.[name], '')) = 0 THEN 1 ELSE 0 END AS [ColumnIsAnonymous],
	sys_s.[name] AS [SchemaName],
	sys_v.[name] AS [ViewName],
	CASE WHEN LEN(ISNULL(sys_c.[name], '')) = 0 THEN ('Column_' + REPLICATE('0', 4 - LEN(CAST(sys_c.[column_id] AS NVARCHAR))) + CAST(sys_c.[column_id] AS NVARCHAR)) ELSE sys_c.[name] END AS [ColumnName],
	sys_c.[is_nullable] AS [ColumnNullable],
	sys_c.[max_length] AS [ColumnSize],
	sys_c.[precision] AS [ColumnPrecision],
	sys_c.[scale] AS [ColumnScale],
	CASE WHEN sys_ty_u.[system_type_id] IS NOT NULL THEN sys_ty_u.[name] ELSE sys_ty.[name] END AS [ColumnSqlType],
	CAST(CASE WHEN sys_ty_u.[system_type_id] IS NOT NULL THEN 1 ELSE 0 END AS [bit]) AS [ColumnIsUserDefinedType]
FROM
	[sys].[columns] sys_c
	INNER JOIN [sys].[views] sys_v ON sys_v.[object_id] = sys_c.[object_id]
	-- NO NEED TO JOIN ON [sys].[objects] sys_o
	INNER JOIN [sys].[schemas] sys_s ON sys_s.[schema_id] = sys_v.[schema_id]
	INNER JOIN [sys].[types] sys_ty ON sys_ty.[user_type_id] = sys_c.[user_type_id]
	LEFT OUTER JOIN [sys].[types] sys_ty_u ON
		sys_ty.[is_user_defined] = 1
		AND sys_ty_u.[system_type_id] = sys_ty.[system_type_id]
WHERE
	sys_s.[name] = ?
	and sys_v.[name] = ?
ORDER BY
	sys_c.[column_id] ASC
