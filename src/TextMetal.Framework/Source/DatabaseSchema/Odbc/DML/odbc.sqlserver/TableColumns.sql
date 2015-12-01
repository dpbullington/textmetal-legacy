/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

-- columns[schema, table]
-- DECLARE @SchemaName [nvarchar](255); SET @SchemaName = 'testcases'; DECLARE @TableName [nvarchar](255); SET @TableName = 'tab_with_composite_primary_key_no_identity';
SELECT
		sys_c.[column_id] AS [ColumnOrdinal],
	CASE WHEN LEN(ISNULL(sys_c.[name], '')) = 0 THEN 1 ELSE 0 END AS [ColumnIsAnonymous],
	sys_s.[name] AS [SchemaName],
	sys_t.[name] AS [TableName],
	CASE WHEN LEN(ISNULL(sys_c.[name], '')) = 0 THEN ('Column_' + REPLICATE('0', 4 - LEN(CAST(sys_c.[column_id] AS NVARCHAR))) + CAST(sys_c.[column_id] AS NVARCHAR)) ELSE sys_c.[name] END AS [ColumnName],
	sys_c.[is_nullable] AS [ColumnNullable],
	sys_c.[max_length] AS [ColumnSize],
	sys_c.[precision] AS [ColumnPrecision],
	sys_c.[scale] AS [ColumnScale],
	CASE WHEN sys_ty_u.[system_type_id] IS NOT NULL THEN sys_ty_u.[name] ELSE sys_ty.[name] END AS [ColumnSqlType],
	CAST(CASE WHEN sys_ty_u.[system_type_id] IS NOT NULL THEN 1 ELSE 0 END AS [bit]) AS [ColumnIsUserDefinedType],

	sys_c.[is_identity] AS [ColumnIsIdentity],
	sys_c.[is_computed] AS [ColumnIsComputed],

	CAST(CASE
		WHEN sys_c.[default_object_id] = 0 THEN 0
		ELSE 1
	END AS [bit]) AS [ColumnHasDefault],
	CAST(CASE
		WHEN sys_c.[rule_object_id] = 0 THEN 0
		ELSE 1
	END AS [bit]) AS [ColumnHasCheck],

	(SELECT
		CAST(CASE COUNT(_sys_kc.[name])
			WHEN 1 THEN 1
			WHEN 0 THEN 0
			ELSE NULL
		END AS [bit])
	FROM
		[sys].[key_constraints] _sys_kc
		INNER JOIN [sys].[indexes] _sys_ix ON
			_sys_ix.[object_id] = _sys_kc.[parent_object_id]
			AND _sys_kc.[unique_index_id] = _sys_ix.[index_id]
		INNER JOIN [sys].[index_columns] _sys_ixc ON
			_sys_ixc.[object_id] = _sys_ix.[object_id]
			AND _sys_ixc.[index_id] = _sys_ix.[index_id]
	WHERE
		_sys_kc.[type] = 'PK'
		AND _sys_ix.[object_id] = sys_c.[object_id]
		AND _sys_ixc.[column_id] = sys_c.[column_id]
		AND _sys_ixc.[is_included_column] = 0
		AND _sys_ix.[object_id] = sys_t.[object_id]
	) AS [ColumnIsPrimaryKey],

	(SELECT
		_sys_ixc.[index_column_id]
	FROM
		[sys].[key_constraints] _sys_kc
		INNER JOIN [sys].[indexes] _sys_ix ON
			_sys_ix.[object_id] = _sys_kc.[parent_object_id]
			AND _sys_kc.[unique_index_id] = _sys_ix.[index_id]
		INNER JOIN [sys].[index_columns] _sys_ixc ON
			_sys_ixc.[object_id] = _sys_ix.[object_id]
			AND _sys_ixc.[index_id] = _sys_ix.[index_id]
	WHERE
		_sys_kc.[type] = 'PK'
		AND _sys_ix.[object_id] = sys_c.[object_id]
		AND _sys_ixc.[column_id] = sys_c.[column_id]
		AND _sys_ixc.[is_included_column] = 0
		AND _sys_ix.[object_id] = sys_t.[object_id]
	) AS [ColumnPrimaryKeyOrdinal]
FROM
	[sys].[columns] sys_c
	INNER JOIN [sys].[tables] sys_t ON sys_t.[object_id] = sys_c.[object_id]
	-- NO NEED TO JOIN ON [sys].[objects] sys_o
	INNER JOIN [sys].[schemas] sys_s ON sys_s.[schema_id] = sys_t.[schema_id]
	INNER JOIN [sys].[types] sys_ty ON sys_ty.[user_type_id] = sys_c.[user_type_id]
	LEFT OUTER JOIN [sys].[types] sys_ty_u ON
		sys_ty.[is_user_defined] = 1
		AND sys_ty_u.[system_type_id] = sys_ty.[system_type_id]
WHERE
	sys_s.[name] = ?
	AND sys_t.[name] = ?
ORDER BY
	sys_c.[column_id] ASC
