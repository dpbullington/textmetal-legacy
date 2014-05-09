/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/


SET NOCOUNT ON
GO


CREATE SCHEMA [testcases]
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


CREATE PROCEDURE [testcases].[sproc_simple]
(
	@num [int],
	@den [int],
	@res [float] OUTPUT
)
AS
BEGIN
	SET @res = @num / @den

	RETURN @num % @den
END
GO