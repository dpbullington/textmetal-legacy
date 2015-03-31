/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/


SET NOCOUNT ON
GO


CREATE SCHEMA [testcases]
GO


CREATE TABLE [testcases].[UpsertCurrent]
(
	[UpsertCurrentId] [int] IDENTITY(1,1) NOT NULL,

	[UpsertCurrentName] [nvarchar](100) NOT NULL,

	CONSTRAINT [pk_UpsertCurrent] PRIMARY KEY
	(
		[UpsertCurrentId]
	)
)
GO


CREATE TABLE [testcases].[UpsertHistory]
(
	[UpsertHistoryId] [int] IDENTITY(100000,1) NOT NULL,
	[UpsertHistoryTs] [datetime2] NOT NULL,
	[UpsertCurrentId] [int] NOT NULL,

	[UpsertHistoryName] [nvarchar](100) NULL,

	CONSTRAINT [pk_UpsertHistory] PRIMARY KEY
	(
		[UpsertHistoryId]
	)
)
GO


CREATE TRIGGER [testcases].[OnUpsertCurrentChange] ON [testcases].[UpsertCurrent] AFTER INSERT, UPDATE, DELETE
AS
	BEGIN
		SET NOCOUNT ON;

		DECLARE @ThisMoment [datetime2]

		SET @ThisMoment = SYSUTCDATETIME()

		INSERT INTO [testcases].[UpsertHistory]
		SELECT
			@ThisMoment AS [UpsertHistoryTs],
			i.[UpsertCurrentId],

			i.[UpsertCurrentName] AS [UpsertHistoryName]
		FROM [inserted] i

		UNION ALL

		SELECT
			@ThisMoment AS [UpsertHistoryTs],
			d.[UpsertCurrentId],

			NULL AS [UpsertHistoryName]
		FROM [deleted] d
		LEFT OUTER JOIN [inserted] i ON i.[UpsertCurrentId] = d.[UpsertCurrentId]
		WHERE i.[UpsertCurrentId] IS NULL
	END
GO


CREATE TABLE [testcases].[tab_with_primary_key_as_identity]
(
	[col_int_id_pk] [int] IDENTITY(1,1) NOT NULL,

	[col_bigint] [bigint] NULL,
	[col_binary] [binary] NULL,
	[col_bit] [bit] NULL,
	[col_char] [char] NULL,
	--[col_cursor] [cursor] NULL,
	[col_date] [date] NULL,
	[col_datetime] [datetime] NULL,
	[col_datetime2] [datetime2] NULL,
	[col_datetimeoffset] [datetimeoffset] NULL,
	[col_decimal] [decimal] NULL,
	[col_float] [float] NULL,
	--[col_geography] [geography] NULL,
	--[col_geometry] [geometry] NULL,
	--[col_hierarchyid] [hierarchyid] NULL,
	[col_image] [image] NULL,
	[col_int] [int] NULL,
	[col_money] [money] NULL,
	[col_nchar] [nchar] NULL,
	[col_ntext] [ntext] NULL,
	[col_numeric] [numeric] NULL,
	[col_nvarchar] [nvarchar] NULL,
	[col_real] [real] NULL,
	[col_rowversion] [rowversion] NULL,
	[col_smalldatetime] [smalldatetime] NULL,
	[col_smallint] [smallint] NULL,
	[col_smallmoney] [smallmoney] NULL,
	[col_sql_variant] [sql_variant] NULL,
	[col_sysname] [sysname] NULL,
	--[col_table] [table] NULL,
	[col_text] [text] NULL,
	[col_time] [time] NULL,
	[col_tinyint] [tinyint] NULL,
	[col_uniqueidentifier] [uniqueidentifier] NULL,
	[col_varbinary] [varbinary] NULL,
	[col_varchar] [varchar] NULL,
	[col_xml] [xml] NULL,

	CONSTRAINT [pk_tab_with_primary_key_as_identity] PRIMARY KEY
	(
		[col_int_id_pk]
	)
)
GO


CREATE TABLE [testcases].[tab_with_primary_key_as_default]
(
	[col_uuid_df_pk] [uniqueidentifier] NOT NULL DEFAULT(newsequentialid()),

	[col_bigint] [bigint] NULL,
	[col_binary] [binary] NULL,
	[col_bit] [bit] NULL,
	[col_char] [char] NULL,
	--[col_cursor] [cursor] NULL,
	[col_date] [date] NULL,
	[col_datetime] [datetime] NULL,
	[col_datetime2] [datetime2] NULL,
	[col_datetimeoffset] [datetimeoffset] NULL,
	[col_decimal] [decimal] NULL,
	[col_float] [float] NULL,
	--[col_geography] [geography] NULL,
	--[col_geometry] [geometry] NULL,
	--[col_hierarchyid] [hierarchyid] NULL,
	[col_image] [image] NULL,
	[col_int] [int] NULL,
	[col_money] [money] NULL,
	[col_nchar] [nchar] NULL,
	[col_ntext] [ntext] NULL,
	[col_numeric] [numeric] NULL,
	[col_nvarchar] [nvarchar] NULL,
	[col_real] [real] NULL,
	[col_rowversion] [rowversion] NULL,
	[col_smalldatetime] [smalldatetime] NULL,
	[col_smallint] [smallint] NULL,
	[col_smallmoney] [smallmoney] NULL,
	[col_sql_variant] [sql_variant] NULL,
	[col_sysname] [sysname] NULL,
	--[col_table] [table] NULL,
	[col_text] [text] NULL,
	[col_time] [time] NULL,
	[col_tinyint] [tinyint] NULL,
	[col_uniqueidentifier] [uniqueidentifier] NULL,
	[col_varbinary] [varbinary] NULL,
	[col_varchar] [varchar] NULL,
	[col_xml] [xml] NULL,

	CONSTRAINT [pk_tab_with_primary_key_as_default] PRIMARY KEY
	(
		[col_uuid_df_pk]
	)
)
GO


CREATE TABLE [testcases].[tab_with_primary_key_with_different_identity]
(
	[col_int_pk] [int] NOT NULL,
	[col_int_id] [int] IDENTITY(1,1) NOT NULL,

	[col_bigint] [bigint] NULL,
	[col_binary] [binary] NULL,
	[col_bit] [bit] NULL,
	[col_char] [char] NULL,
	--[col_cursor] [cursor] NULL,
	[col_date] [date] NULL,
	[col_datetime] [datetime] NULL,
	[col_datetime2] [datetime2] NULL,
	[col_datetimeoffset] [datetimeoffset] NULL,
	[col_decimal] [decimal] NULL,
	[col_float] [float] NULL,
	--[col_geography] [geography] NULL,
	--[col_geometry] [geometry] NULL,
	--[col_hierarchyid] [hierarchyid] NULL,
	[col_image] [image] NULL,
	[col_int] [int] NULL,
	[col_money] [money] NULL,
	[col_nchar] [nchar] NULL,
	[col_ntext] [ntext] NULL,
	[col_numeric] [numeric] NULL,
	[col_nvarchar] [nvarchar] NULL,
	[col_real] [real] NULL,
	[col_rowversion] [rowversion] NULL,
	[col_smalldatetime] [smalldatetime] NULL,
	[col_smallint] [smallint] NULL,
	[col_smallmoney] [smallmoney] NULL,
	[col_sql_variant] [sql_variant] NULL,
	[col_sysname] [sysname] NULL,
	--[col_table] [table] NULL,
	[col_text] [text] NULL,
	[col_time] [time] NULL,
	[col_tinyint] [tinyint] NULL,
	[col_uniqueidentifier] [uniqueidentifier] NULL,
	[col_varbinary] [varbinary] NULL,
	[col_varchar] [varchar] NULL,
	[col_xml] [xml] NULL,

	CONSTRAINT [pk_tab_with_primary_key_with_different_identity] PRIMARY KEY
	(
		[col_int_pk]
	)
)
GO


CREATE TABLE [testcases].[tab_with_no_primary_key_with_identity]
(
	[col_int_id] [int] IDENTITY(1,1) NOT NULL,

	[col_bigint] [bigint] NULL,
	[col_binary] [binary] NULL,
	[col_bit] [bit] NULL,
	[col_char] [char] NULL,
	--[col_cursor] [cursor] NULL,
	[col_date] [date] NULL,
	[col_datetime] [datetime] NULL,
	[col_datetime2] [datetime2] NULL,
	[col_datetimeoffset] [datetimeoffset] NULL,
	[col_decimal] [decimal] NULL,
	[col_float] [float] NULL,
	--[col_geography] [geography] NULL,
	--[col_geometry] [geometry] NULL,
	--[col_hierarchyid] [hierarchyid] NULL,
	[col_image] [image] NULL,
	[col_int] [int] NULL,
	[col_money] [money] NULL,
	[col_nchar] [nchar] NULL,
	[col_ntext] [ntext] NULL,
	[col_numeric] [numeric] NULL,
	[col_nvarchar] [nvarchar] NULL,
	[col_real] [real] NULL,
	[col_rowversion] [rowversion] NULL,
	[col_smalldatetime] [smalldatetime] NULL,
	[col_smallint] [smallint] NULL,
	[col_smallmoney] [smallmoney] NULL,
	[col_sql_variant] [sql_variant] NULL,
	[col_sysname] [sysname] NULL,
	--[col_table] [table] NULL,
	[col_text] [text] NULL,
	[col_time] [time] NULL,
	[col_tinyint] [tinyint] NULL,
	[col_uniqueidentifier] [uniqueidentifier] NULL,
	[col_varbinary] [varbinary] NULL,
	[col_varchar] [varchar] NULL,
	[col_xml] [xml] NULL
)
GO


CREATE TABLE [testcases].[tab_with_primary_key_no_identity]
(
	[col_int_pk] [int] NOT NULL,

	[col_bigint] [bigint] NULL,
	[col_binary] [binary] NULL,
	[col_bit] [bit] NULL,
	[col_char] [char] NULL,
	--[col_cursor] [cursor] NULL,
	[col_date] [date] NULL,
	[col_datetime] [datetime] NULL,
	[col_datetime2] [datetime2] NULL,
	[col_datetimeoffset] [datetimeoffset] NULL,
	[col_decimal] [decimal] NULL,
	[col_float] [float] NULL,
	--[col_geography] [geography] NULL,
	--[col_geometry] [geometry] NULL,
	--[col_hierarchyid] [hierarchyid] NULL,
	[col_image] [image] NULL,
	[col_int] [int] NULL,
	[col_money] [money] NULL,
	[col_nchar] [nchar] NULL,
	[col_ntext] [ntext] NULL,
	[col_numeric] [numeric] NULL,
	[col_nvarchar] [nvarchar] NULL,
	[col_real] [real] NULL,
	[col_rowversion] [rowversion] NULL,
	[col_smalldatetime] [smalldatetime] NULL,
	[col_smallint] [smallint] NULL,
	[col_smallmoney] [smallmoney] NULL,
	[col_sql_variant] [sql_variant] NULL,
	[col_sysname] [sysname] NULL,
	--[col_table] [table] NULL,
	[col_text] [text] NULL,
	[col_time] [time] NULL,
	[col_tinyint] [tinyint] NULL,
	[col_uniqueidentifier] [uniqueidentifier] NULL,
	[col_varbinary] [varbinary] NULL,
	[col_varchar] [varchar] NULL,
	[col_xml] [xml] NULL,

	CONSTRAINT [pk_tab_with_primary_key_no_identity] PRIMARY KEY
	(
		[col_int_pk]
	)
)
GO


CREATE TABLE [testcases].[tab_no_primary_key_no_identity]
(
	[col_bigint] [bigint] NULL,
	[col_binary] [binary] NULL,
	[col_bit] [bit] NULL,
	[col_char] [char] NULL,
	--[col_cursor] [cursor] NULL,
	[col_date] [date] NULL,
	[col_datetime] [datetime] NULL,
	[col_datetime2] [datetime2] NULL,
	[col_datetimeoffset] [datetimeoffset] NULL,
	[col_decimal] [decimal] NULL,
	[col_float] [float] NULL,
	--[col_geography] [geography] NULL,
	--[col_geometry] [geometry] NULL,
	--[col_hierarchyid] [hierarchyid] NULL,
	[col_image] [image] NULL,
	[col_int] [int] NULL,
	[col_money] [money] NULL,
	[col_nchar] [nchar] NULL,
	[col_ntext] [ntext] NULL,
	[col_numeric] [numeric] NULL,
	[col_nvarchar] [nvarchar] NULL,
	[col_real] [real] NULL,
	[col_rowversion] [rowversion] NULL,
	[col_smalldatetime] [smalldatetime] NULL,
	[col_smallint] [smallint] NULL,
	[col_smallmoney] [smallmoney] NULL,
	[col_sql_variant] [sql_variant] NULL,
	[col_sysname] [sysname] NULL,
	--[col_table] [table] NULL,
	[col_text] [text] NULL,
	[col_time] [time] NULL,
	[col_tinyint] [tinyint] NULL,
	[col_uniqueidentifier] [uniqueidentifier] NULL,
	[col_varbinary] [varbinary] NULL,
	[col_varchar] [varchar] NULL,
	[col_xml] [xml] NULL
)
GO


CREATE TABLE [testcases].[tab_with_composite_primary_key_no_identity]
(
	[col_int_pk0] [int] NOT NULL,
	[col_int_pk1] [int] NOT NULL,
	[col_int_pk2] [int] NOT NULL,
	[col_int_pk3] [int] NOT NULL,

	[col_bigint] [bigint] NULL,
	[col_binary] [binary] NULL,
	[col_bit] [bit] NULL,
	[col_char] [char] NULL,
	--[col_cursor] [cursor] NULL,
	[col_date] [date] NULL,
	[col_datetime] [datetime] NULL,
	[col_datetime2] [datetime2] NULL,
	[col_datetimeoffset] [datetimeoffset] NULL,
	[col_decimal] [decimal] NULL,
	[col_float] [float] NULL,
	--[col_geography] [geography] NULL,
	--[col_geometry] [geometry] NULL,
	--[col_hierarchyid] [hierarchyid] NULL,
	[col_image] [image] NULL,
	[col_int] [int] NULL,
	[col_money] [money] NULL,
	[col_nchar] [nchar] NULL,
	[col_ntext] [ntext] NULL,
	[col_numeric] [numeric] NULL,
	[col_nvarchar] [nvarchar] NULL,
	[col_real] [real] NULL,
	[col_rowversion] [rowversion] NULL,
	[col_smalldatetime] [smalldatetime] NULL,
	[col_smallint] [smallint] NULL,
	[col_smallmoney] [smallmoney] NULL,
	[col_sql_variant] [sql_variant] NULL,
	[col_sysname] [sysname] NULL,
	--[col_table] [table] NULL,
	[col_text] [text] NULL,
	[col_time] [time] NULL,
	[col_tinyint] [tinyint] NULL,
	[col_uniqueidentifier] [uniqueidentifier] NULL,
	[col_varbinary] [varbinary] NULL,
	[col_varchar] [varchar] NULL,
	[col_xml] [xml] NULL,

	CONSTRAINT [pk_tab_with_composite_primary_key_no_identity] PRIMARY KEY
	(
		[col_int_pk0],
		[col_int_pk1],
		[col_int_pk2],
		[col_int_pk3]
	)
)
GO


CREATE PROCEDURE [testcases].[sproc_with_inparam_no_outparam_with_rvparam_no_resultset]
(
	@in_numerator [int],
	@in_denominator [int]
)
AS
BEGIN
	RETURN CAST(CRYPT_GEN_RANDOM(1) AS [int])
END
GO


CREATE PROCEDURE [testcases].[sproc_with_inparam_with_outparam_no_rvparam_no_resultset]
(
	@in_numerator [int],
	@in_denominator [int],
	@out_result [float] OUTPUT
)
AS
BEGIN
	SET @out_result = @in_numerator / @in_denominator
END
GO


CREATE PROCEDURE [testcases].[sproc_with_inparam_no_outparam_with_rvparam_with_resultset]
(
	@in_numerator [int],
	@in_denominator [int]
)
AS
BEGIN

	SELECT 1 AS [Id], NEWID() AS [Value], 'aaa' AS [Name]
		UNION ALL
	SELECT 2 AS [Id], NEWID() AS [Value], 'bbb' AS [Name]
		UNION ALL
	SELECT 3 AS [Id], NEWID() AS [Value], 'ccc' AS [Name]
		UNION ALL
	SELECT 4 AS [Id], NEWID() AS [Value], 'ddd' AS [Name]
		UNION ALL
	SELECT 5 AS [Id], NEWID() AS [Value], 'eee' AS [Name]

	RETURN CAST(CRYPT_GEN_RANDOM(1) AS [int])
END
GO


CREATE PROCEDURE [testcases].[sproc_with_inparam_with_outparam_no_rvparam_with_resultset]
(
	@in_numerator [int],
	@in_denominator [int],
	@out_result [float] OUTPUT
)
AS
BEGIN

	SELECT 1 AS [Id], NEWID() AS [Value], 'aaa' AS [Name]
		UNION ALL
	SELECT 2 AS [Id], NEWID() AS [Value], 'bbb' AS [Name]
		UNION ALL
	SELECT 3 AS [Id], NEWID() AS [Value], 'ccc' AS [Name]
		UNION ALL
	SELECT 4 AS [Id], NEWID() AS [Value], 'ddd' AS [Name]
		UNION ALL
	SELECT 5 AS [Id], NEWID() AS [Value], 'eee' AS [Name]

	SET @out_result = @in_numerator / @in_denominator
END
GO


CREATE PROCEDURE [testcases].[sproc_with_inparam_with_outparam_with_rvparam_with_resultset]
(
	@in_numerator [int],
	@in_denominator [int],
	@out_result [float] OUTPUT
)
AS
BEGIN

	SELECT 1 AS [Id], NEWID() AS [Value], 'aaa' AS [Name]
		UNION ALL
	SELECT 2 AS [Id], NEWID() AS [Value], 'bbb' AS [Name]
		UNION ALL
	SELECT 3 AS [Id], NEWID() AS [Value], 'ccc' AS [Name]
		UNION ALL
	SELECT 4 AS [Id], NEWID() AS [Value], 'ddd' AS [Name]
		UNION ALL
	SELECT 5 AS [Id], NEWID() AS [Value], 'eee' AS [Name]

	SET @out_result = @in_numerator / @in_denominator

	RETURN CAST(CRYPT_GEN_RANDOM(1) AS [int])
END
GO


CREATE PROCEDURE [testcases].[sproc_with_inparam_no_outparam_with_rvparam_with_resultset_uses_temp_tbls]
(
	@in_numerator [int],
	@in_denominator [int]
)
AS
BEGIN

	SET NOCOUNT ON

	CREATE TABLE #temp_values ([id] [int], [Value] [uniqueidentifier], [Name] [nvarchar](10));

	INSERT INTO #temp_values
		SELECT 1 AS [Id], NEWID() AS [Value], 'aaa' AS [Name]
			UNION ALL
		SELECT 2 AS [Id], NEWID() AS [Value], 'bbb' AS [Name]
			UNION ALL
		SELECT 3 AS [Id], NEWID() AS [Value], 'ccc' AS [Name]
			UNION ALL
		SELECT 4 AS [Id], NEWID() AS [Value], 'ddd' AS [Name]
			UNION ALL
		SELECT 5 AS [Id], NEWID() AS [Value], 'eee' AS [Name]

	SELECT * FROM #temp_values

	RETURN CAST(CRYPT_GEN_RANDOM(1) AS [int])
END
GO


CREATE TABLE [testcases].[tab_with_primary_key_as_identity_plus_has_fk_uk_ix]
(
	[col_int_id_pk] [int] IDENTITY(1,1) NOT NULL,

	[col_fk_int_id] [int] NULL,

	[col_bigint] [bigint] NULL,
	[col_binary] [binary] NULL,
	[col_bit] [bit] NULL,
	[col_char] [char] NULL,
	--[col_cursor] [cursor] NULL,
	[col_date] [date] NULL,
	[col_datetime] [datetime] NULL,
	[col_datetime2] [datetime2] NULL,
	[col_datetimeoffset] [datetimeoffset] NULL,
	[col_decimal] [decimal] NULL,
	[col_float] [float] NULL,
	--[col_geography] [geography] NULL,
	--[col_geometry] [geometry] NULL,
	--[col_hierarchyid] [hierarchyid] NULL,
	[col_image] [image] NULL,
	[col_int] [int] NULL,
	[col_money] [money] NULL,
	[col_nchar] [nchar] NULL,
	[col_ntext] [ntext] NULL,
	[col_numeric] [numeric] NULL,
	[col_nvarchar] [nvarchar] NULL,
	[col_real] [real] NULL,
	[col_rowversion] [rowversion] NULL,
	[col_smalldatetime] [smalldatetime] NULL,
	[col_smallint] [smallint] NULL,
	[col_smallmoney] [smallmoney] NULL,
	[col_sql_variant] [sql_variant] NULL,
	[col_sysname] [sysname] NULL,
	--[col_table] [table] NULL,
	[col_text] [text] NULL,
	[col_time] [time] NULL,
	[col_tinyint] [tinyint] NULL,
	[col_uniqueidentifier] [uniqueidentifier] NULL,
	[col_varbinary] [varbinary] NULL,
	[col_varchar] [varchar] NULL,
	[col_xml] [xml] NULL,

	CONSTRAINT [pk_tab_with_primary_key_as_identity_plus_has_fk_uk_ix] PRIMARY KEY
	(
		[col_int_id_pk]
	),

	CONSTRAINT [uk__tab_with_primary_key_as_identity_plus_has_fk_uk_ix] UNIQUE
	(
		[col_nvarchar]
	),

	CONSTRAINT [fk_tab_with_primary_key_as_identity_plus_has_fk_uk_ix] FOREIGN KEY
	(
		[col_fk_int_id]
	)
	REFERENCES [testcases].[tab_with_primary_key_as_identity]
	(
		[col_int_id_pk]
	)
)
GO


CREATE NONCLUSTERED INDEX [ix_tab_with_primary_key_as_identity_plus_has_fk_uk_ix] ON [testcases].[tab_with_primary_key_as_identity_plus_has_fk_uk_ix]
(
	[col_bigint] ASC,
	[col_datetime2] ASC
) ON [PRIMARY]
GO