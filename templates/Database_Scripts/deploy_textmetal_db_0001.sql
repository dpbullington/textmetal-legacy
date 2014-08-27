/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/


SET NOCOUNT ON
GO


CREATE SCHEMA [global]
GO


CREATE TABLE [global].[User]
(
	[UserId] [int] IDENTITY(0,1) NOT NULL,

	[EmailAddress] [nvarchar](255) NOT NULL,
	[UserName] [nvarchar](255) NOT NULL,
	[SaltValue] [nvarchar](255) NOT NULL,
	[PasswordHash] [nvarchar](255) NOT NULL,
	[Question] [nvarchar](255) NOT NULL,
	[AnswerHash] [nvarchar](255) NOT NULL,
	[LastLoginSuccessTimestamp] [datetime] NULL,
	[LastLoginFailureTimestamp] [datetime] NULL,
	[FailedLoginCount] [smallint] NOT NULL,
	[MustChangePassword] [bit] NOT NULL,

	[SortOrder] [tinyint] NULL,
	[CreationTimestamp] [datetime] NOT NULL,
	[ModificationTimestamp] [datetime] NOT NULL,
	[CreationUserId] [int] NULL,
	[ModificationUserId] [int] NULL,
	[LogicalDelete] [bit] NOT NULL DEFAULT(0),

	CONSTRAINT [pk_User] PRIMARY KEY
	(
		[UserId]
	),

	CONSTRAINT [uk_User_username] UNIQUE
	(
		[UserName]
	),

	CONSTRAINT [uk_User_emailaddress] UNIQUE
	(
		[EmailAddress]
	),

	CONSTRAINT [fk_User_User_creation] FOREIGN KEY
	(
		[CreationUserId]
	)
	REFERENCES [global].[User]
	(
		[UserId]
	),

	CONSTRAINT [fk_User_User_modification] FOREIGN KEY
	(
		[ModificationUserId]
	)
	REFERENCES [global].[User]
	(
		[UserId]
	)
)
GO


INSERT INTO [global].[User]
(
	[EmailAddress], [UserName], [SaltValue], [PasswordHash], [Question], [AnswerHash],
	[LastLoginSuccessTimestamp], [LastLoginFailureTimestamp], [FailedLoginCount], [MustChangePassword],
	[SortOrder], [CreationTimestamp], [ModificationTimestamp], [CreationUserId], [ModificationUserId], [LogicalDelete]
)
VALUES
(
	'system@textmetal.com', 'system', '', '', '', '',
	null, null, 0, 0,
	0, GETUTCDATE(), GETUTCDATE(), null, null, 0
);


UPDATE [global].[User] SET
[CreationUserId] = 0, [ModificationUserId] = 0
WHERE [UserId] = 0
GO


ALTER TABLE [global].[User] ALTER COLUMN [CreationUserId] [int] NOT NULL
ALTER TABLE [global].[User] ALTER COLUMN [ModificationUserId] [int] NOT NULL
GO


CREATE TABLE [global].[SecurityRole]
(
	[SecurityRoleId] [int] NOT NULL,

	[SecurityRoleName] [nvarchar](255) NOT NULL,

	[SortOrder] [tinyint] NULL,
	[CreationTimestamp] [datetime] NOT NULL,
	[ModificationTimestamp] [datetime] NOT NULL,
	[CreationUserId] [int] NOT NULL,
	[ModificationUserId] [int] NOT NULL,
	[LogicalDelete] [bit] NOT NULL DEFAULT(0),

	CONSTRAINT [pk_SecurityRole] PRIMARY KEY
	(
		[SecurityRoleId]
	),

	CONSTRAINT [uk_SecurityRole] UNIQUE
	(
		[SecurityRoleName]
	),

	CONSTRAINT [fk_SecurityRole_User_creation] FOREIGN KEY
	(
		[CreationUserId]
	)
	REFERENCES [global].[User]
	(
		[UserId]
	),

	CONSTRAINT [fk_SecurityRole_User_modification] FOREIGN KEY
	(
		[ModificationUserId]
	)
	REFERENCES [global].[User]
	(
		[UserId]
	)
)
GO


INSERT INTO [global].[SecurityRole] ([SecurityRoleId], [SecurityRoleName], [CreationTimestamp], [ModificationTimestamp], [CreationUserId], [ModificationUserId]) VALUES (0, 'None', GetUtcDate(), GetUtcDate(), 0, 0);
INSERT INTO [global].[SecurityRole] ([SecurityRoleId], [SecurityRoleName], [CreationTimestamp], [ModificationTimestamp], [CreationUserId], [ModificationUserId]) VALUES (1, 'OrganizationOwner', GetUtcDate(), GetUtcDate(), 0, 0);
INSERT INTO [global].[SecurityRole] ([SecurityRoleId], [SecurityRoleName], [CreationTimestamp], [ModificationTimestamp], [CreationUserId], [ModificationUserId]) VALUES (2, 'OrganizationDesigner', GetUtcDate(), GetUtcDate(), 0, 0);
INSERT INTO [global].[SecurityRole] ([SecurityRoleId], [SecurityRoleName], [CreationTimestamp], [ModificationTimestamp], [CreationUserId], [ModificationUserId]) VALUES (3, 'OrganizationContributor', GetUtcDate(), GetUtcDate(), 0, 0);
INSERT INTO [global].[SecurityRole] ([SecurityRoleId], [SecurityRoleName], [CreationTimestamp], [ModificationTimestamp], [CreationUserId], [ModificationUserId]) VALUES (4, 'OrganizationVisitor', GetUtcDate(), GetUtcDate(), 0, 0);
GO


CREATE TABLE [global].[PropertyBag]
(
	[PropertyBagId] [int] IDENTITY(1,1) NOT NULL,

	[PropertyKey] [nvarchar](255) NOT NULL,
	[PropertyType] [nvarchar](255) NULL,
	[PropertyValue] [nvarchar](2047) NULL,

	[SortOrder] [tinyint] NULL,
	[CreationTimestamp] [datetime] NOT NULL,
	[ModificationTimestamp] [datetime] NOT NULL,
	[CreationUserId] [int] NOT NULL,
	[ModificationUserId] [int] NOT NULL,
	[LogicalDelete] [bit] NOT NULL DEFAULT(0),

	CONSTRAINT [pk_PropertyBag] PRIMARY KEY
	(
		[PropertyBagId]
	),

	CONSTRAINT [fk_PropertyBag_User_creation] FOREIGN KEY
	(
		[CreationUserId]
	)
	REFERENCES [global].[User]
	(
		[UserId]
	),

	CONSTRAINT [fk_PropertyBag_User_modification] FOREIGN KEY
	(
		[ModificationUserId]
	)
	REFERENCES [global].[User]
	(
		[UserId]
	)
)
GO


INSERT INTO [global].[PropertyBag] ([PropertyKey], [PropertyValue], [CreationTimestamp], [ModificationTimestamp], [CreationUserId], [ModificationUserId]) VALUES ('RepositoryGuid', NEWID(), GetUtcDate(), GetUtcDate(), 0, 0);
INSERT INTO [global].[PropertyBag] ([PropertyKey], [PropertyValue], [CreationTimestamp], [ModificationTimestamp], [CreationUserId], [ModificationUserId]) VALUES ('SchemaRevision', '1', GetUtcDate(), GetUtcDate(), 0, 0);
GO


CREATE TABLE [global].[EventLog]
(
	[EventLogId] [int] IDENTITY(1,1) NOT NULL,
	[EventText] [nvarchar](MAX) NOT NULL,

	[SortOrder] [tinyint] NULL,
	[CreationTimestamp] [datetime] NOT NULL,
	[ModificationTimestamp] [datetime] NOT NULL,
	[CreationUserId] [int] NULL,
	[ModificationUserId] [int] NULL,
	[LogicalDelete] [bit] NOT NULL DEFAULT(0),

	CONSTRAINT [pk_EventLog] PRIMARY KEY
	(
		[EventLogId]
	),

	CONSTRAINT [fk_EventLog_User_creation] FOREIGN KEY
	(
		[CreationUserId]
	)
	REFERENCES [global].[User]
	(
		[UserId]
	),

	CONSTRAINT [fk_EventLog_User_modification] FOREIGN KEY
	(
		[ModificationUserId]
	)
	REFERENCES [global].[User]
	(
		[UserId]
	)
)
GO


CREATE TABLE [global].[EmailMessage]
(
	[EmailMessageId] [int] IDENTITY(1,1) NOT NULL,

	[From] [nvarchar](2047) NOT NULL,
	[Sender] [nvarchar](2047) NULL,
	[ReplyTo] [nvarchar](2047) NULL,
	[To] [nvarchar](2047) NOT NULL,
	[CarbonCopy] [nvarchar](2047) NULL,
	[BlindCarbonCopy] [nvarchar](2047) NULL,
	[Subject] [nvarchar](2047) NOT NULL,
	[IsBodyHtml] [bit] NOT NULL DEFAULT(0),
	[Body] [nvarchar](MAX) NOT NULL,
	[Processed] [bit] NOT NULL,

	[SortOrder] [tinyint] NULL,
	[CreationTimestamp] [datetime] NOT NULL,
	[ModificationTimestamp] [datetime] NOT NULL,
	[CreationUserId] [int] NULL,
	[ModificationUserId] [int] NULL,
	[LogicalDelete] [bit] NOT NULL DEFAULT(0),

	CONSTRAINT [pk_EmailMessage] PRIMARY KEY
	(
		[EmailMessageId]
	),

	CONSTRAINT [fk_EmailMessage_User_creation] FOREIGN KEY
	(
		[CreationUserId]
	)
	REFERENCES [global].[User]
	(
		[UserId]
	),

	CONSTRAINT [fk_EmailMessage_User_modification] FOREIGN KEY
	(
		[ModificationUserId]
	)
	REFERENCES [global].[User]
	(
		[UserId]
	)
)
GO


CREATE TABLE [global].[EmailAttachment]
(
	[EmailAttachmentId] [int] IDENTITY(1,1) NOT NULL,
	[EmailMessageId] [int] NOT NULL,

	[FileName] [nvarchar](255) NOT NULL,
	[FileSize] [bigint] NOT NULL DEFAULT(0),
	[MimeType] [nvarchar](255) NOT NULL,
	[AttachmentBits] [varbinary](MAX) NULL,

	[SortOrder] [tinyint] NULL,
	[CreationTimestamp] [datetime] NOT NULL,
	[ModificationTimestamp] [datetime] NOT NULL,
	[CreationUserId] [int] NULL,
	[ModificationUserId] [int] NULL,
	[LogicalDelete] [bit] NOT NULL DEFAULT(0),

	CONSTRAINT [pk_EmailAttachment] PRIMARY KEY
	(
		[EmailAttachmentId]
	),

	CONSTRAINT [uk_EmailAttachment] UNIQUE
	(
		[EmailMessageId],
		[FileName]
	),

	CONSTRAINT [fk_EmailAttachment_EmailMessage] FOREIGN KEY
	(
		[EmailMessageId]
	)
	REFERENCES [global].[EmailMessage]
	(
		[EmailMessageId]
	),

	CONSTRAINT [fk_EmailAttachment_User_creation] FOREIGN KEY
	(
		[CreationUserId]
	)
	REFERENCES [global].[User]
	(
		[UserId]
	),

	CONSTRAINT [fk_EmailAttachment_User_modification] FOREIGN KEY
	(
		[ModificationUserId]
	)
	REFERENCES [global].[User]
	(
		[UserId]
	)
)
GO


CREATE SCHEMA [application]
GO


CREATE TABLE [application].[Organization]
(
	[OrganizationId] [int] IDENTITY(1,1) NOT NULL,
	[ParentOrganizationId] [int] NULL,

	[OrganizationName] [nvarchar](255) NOT NULL,
	[TimeZoneId] [nvarchar](255) NULL,

	[SortOrder] [tinyint] NULL,
	[CreationTimestamp] [datetime] NOT NULL,
	[ModificationTimestamp] [datetime] NOT NULL,
	[CreationUserId] [int] NOT NULL,
	[ModificationUserId] [int] NOT NULL,
	[LogicalDelete] [bit] NOT NULL DEFAULT(0),

	CONSTRAINT [pk_Organization] PRIMARY KEY
	(
		[OrganizationId]
	),

	CONSTRAINT [uk_Organization] UNIQUE
	(
		[OrganizationName]
	),

	CONSTRAINT [fk_Organization_Organization_parent] FOREIGN KEY
	(
		[ParentOrganizationId]
	)
	REFERENCES [application].[Organization]
	(
		[OrganizationId]
	),

	CONSTRAINT [fk_Organization_User_creation] FOREIGN KEY
	(
		[CreationUserId]
	)
	REFERENCES [global].[User]
	(
		[UserId]
	),

	CONSTRAINT [fk_Organization_User_modification] FOREIGN KEY
	(
		[ModificationUserId]
	)
	REFERENCES [global].[User]
	(
		[UserId]
	)
)
GO


CREATE TABLE [application].[Member]
(
	[MemberId] [int] NOT NULL, -- same as [User].[UserId]
	[OrganizationId] [int] NOT NULL,
	[ParentMemberId] [int] NULL,

	[MemberName] [nvarchar](255) NULL,
	[MemberTitle] [nvarchar](255) NULL,

	[SecurityRoleId] [int] NOT NULL,

	[SortOrder] [tinyint] NULL,
	[CreationTimestamp] [datetime] NOT NULL,
	[ModificationTimestamp] [datetime] NOT NULL,
	[CreationUserId] [int] NOT NULL,
	[ModificationUserId] [int] NOT NULL,
	[LogicalDelete] [bit] NOT NULL DEFAULT(0),

	CONSTRAINT [pk_Member] PRIMARY KEY
	(
		[MemberId]
	),

	CONSTRAINT [uk_Member] UNIQUE
	(
		[MemberName]
	),

	CONSTRAINT [fk_Member_User] FOREIGN KEY
	(
		[MemberId]
	)
	REFERENCES [global].[User]
	(
		[UserId]
	),

	CONSTRAINT [fk_Member_Member_parent] FOREIGN KEY
	(
		[ParentMemberId]
	)
	REFERENCES [application].[Member]
	(
		[MemberId]
	),

	CONSTRAINT [fk_Member_Organization] FOREIGN KEY
	(
		[OrganizationId]
	)
	REFERENCES [application].[Organization]
	(
		[OrganizationId]
	),

	CONSTRAINT [fk_Member_SecurityRole] FOREIGN KEY
	(
		[SecurityRoleId]
	)
	REFERENCES [global].[SecurityRole]
	(
		[SecurityRoleId]
	),

	CONSTRAINT [fk_Member_User_creation] FOREIGN KEY
	(
		[CreationUserId]
	)
	REFERENCES [global].[User]
	(
		[UserId]
	),

	CONSTRAINT [fk_Member_User_modification] FOREIGN KEY
	(
		[ModificationUserId]
	)
	REFERENCES [global].[User]
	(
		[UserId]
	)
)
GO


CREATE VIEW [dbo].[EventLogExtent] AS
	SELECT
	MIN(t.[CreationTimestamp]) AS [MinCreationTimestamp],
	AVG(DAY(t.[ModificationTimestamp] - t.[CreationTimestamp])) AS [AvgDifferenceTimestamps],
	MAX(t.[ModificationTimestamp]) AS [MaxModificationTimestamp]
	FROM [global].[EventLog] t
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


CREATE PROCEDURE [testcases].[sproc_resultset]
AS
BEGIN
	SELECT 1 AS [Id], 227 AS [Value], 'XXX' AS [Name]
UNION ALL
	SELECT 2 AS [Id], 112 AS [Value], 'XXX' AS [Name]

	RETURN RAND()
END
GO
