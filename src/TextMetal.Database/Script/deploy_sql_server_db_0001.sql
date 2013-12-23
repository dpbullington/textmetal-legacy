/*
	Copyright ©2002-2013 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/


USE [TextMetalWebHostSample]
GO


CREATE SCHEMA [TxtMtl]
GO


CREATE TABLE [TxtMtl].[SexualChocolate]
(
	[SexualChocolateId] [int] IDENTITY(1,1) NOT NULL,
	[EM] [nvarchar](MAX) NOT NULL,	
	[Blob] [varbinary](MAX) NOT NULL,
	[CreationTimestamp] [datetime] NOT NULL,
	[ModificationTimestamp] [datetime] NOT NULL,
	[LogicalDelete] [bit] NOT NULL DEFAULT(0),
	
	CONSTRAINT [pk_SexualChocolate] PRIMARY KEY
	(
		[SexualChocolateId]
	)	
)
GO


CREATE TABLE [User]
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
	REFERENCES [User]
	(
		[UserId]
	),

	CONSTRAINT [fk_User_User_modification] FOREIGN KEY
	(
		[ModificationUserId]
	)
	REFERENCES [User]
	(
		[UserId]
	)
)
GO


CREATE TABLE [EventLog]
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
	REFERENCES [User]
	(
		[UserId]
	),
	
	CONSTRAINT [fk_EventLog_User_modification] FOREIGN KEY
	(
		[ModificationUserId]
	)
	REFERENCES [User]
	(
		[UserId]
	)
)
GO


CREATE TABLE [EmailMessage]
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
	REFERENCES [User]
	(
		[UserId]
	),
	
	CONSTRAINT [fk_EmailMessage_User_modification] FOREIGN KEY
	(
		[ModificationUserId]
	)
	REFERENCES [User]
	(
		[UserId]
	)
)
GO


CREATE TABLE [EmailAttachment]
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
		[EmailMessageId]
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
	REFERENCES [EmailMessage]
	(
		[EmailMessageId]
	),
	
	CONSTRAINT [fk_EmailAttachment_User_creation] FOREIGN KEY
	(
		[CreationUserId]
	)
	REFERENCES [User]
	(
		[UserId]
	),
	
	CONSTRAINT [fk_EmailAttachment_User_modification] FOREIGN KEY
	(
		[ModificationUserId]
	)
	REFERENCES [User]
	(
		[UserId]
	)
)
GO


CREATE TABLE [TableWithPrimaryKeyAsIdentity]
(
	[PkId] [int] IDENTITY(1,1) NOT NULL,
	[Data01] [BIT] NULL,
	[Data02] [DATETIME] NULL,
	[Data03] [INT] NULL,
	[Data04] [NVARCHAR](100) NULL,
	
	CONSTRAINT [pk_TableWithPrimaryKeyAsIdentity] PRIMARY KEY
	(
		[PkId]
	)	
)
GO


CREATE TABLE [TableWithPrimaryKeyAsDefault]
(
	[PkDf] [UNIQUEIDENTIFIER] NOT NULL DEFAULT(newsequentialid()),
	[Data01] [BIT] NULL,
	[Data02] [DATETIME] NULL,
	[Data03] [INT] NULL,
	[Data04] [NVARCHAR](100) NULL,
	
	CONSTRAINT [pk_TableWithPrimaryKeyAsDefault] PRIMARY KEY
	(
		[PkDf]
	)	
)
GO


CREATE TABLE [TableWithPrimaryKeyWithDiffIdentity]
(
	[Pk] [int] NOT NULL,
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Data01] [BIT] NULL,
	[Data02] [DATETIME] NULL,
	[Data03] [INT] NULL,
	[Data04] [NVARCHAR](100) NULL,
	
	CONSTRAINT [pk_TableWithPrimaryKeyWithDiffIdentity] PRIMARY KEY
	(
		[Pk]
	)	
)
GO


CREATE TABLE [TableNoPrimaryKeyWithIdentity]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Data01] [BIT] NULL,
	[Data02] [DATETIME] NULL,
	[Data03] [INT] NULL,
	[Data04] [NVARCHAR](100) NULL
)
GO


CREATE TABLE [TableWithPrimaryKeyNoIdentity]
(
	[Pk] [int] NOT NULL,
	[Data01] [BIT] NULL,
	[Data02] [DATETIME] NULL,
	[Data03] [INT] NULL,
	[Data04] [NVARCHAR](100) NULL,
	
	CONSTRAINT [pk_TableWithPrimaryKeyNoIdentity] PRIMARY KEY
	(
		[Pk]
	)	
)
GO


CREATE TABLE [TableWithCompositePrimaryKeyNoIdentity]
(
	[Pk0] [int] NOT NULL,
	[Pk1] [int] NOT NULL,
	[Pk2] [int] NOT NULL,
	[Pk3] [int] NOT NULL,
	[Data01] [BIT] NULL,
	[Data02] [DATETIME] NULL,
	[Data03] [INT] NULL,
	[Data04] [NVARCHAR](100) NULL,
	
	CONSTRAINT [pk_TableWithCompositePrimaryKeyNoIdentity] PRIMARY KEY
	(
		[Pk0],[Pk1],[Pk2],[Pk3]
	)	
)
GO


CREATE TABLE [TableNoPrimaryKeyNoIdentity]
(
	[Value] [int] NOT NULL,
	[Data01] [BIT] NULL,
	[Data02] [DATETIME] NULL,
	[Data03] [INT] NULL,
	[Data04] [NVARCHAR](100) NULL
)
GO


CREATE TABLE [TableTypeTest]
(
	[PkId] [int] IDENTITY(1,1) NOT NULL,
	[Data00] [BIGINT] NULL,
	[Data01] [BINARY] NULL,
	[Data02] [BIT] NULL,
	[Data03] [CHAR] NULL,
	--[Data04] [CURSOR] NULL,
	[Data05] [DATE] NULL,
	[Data06] [DATETIME] NULL,
	[Data07] [DATETIME2] NULL,
	[Data08] [DATETIMEOFFSET] NULL,
	[Data09] [DECIMAL] NULL,
	[Data10] [FLOAT] NULL,
	--[Data11] [HIERARCHYID] NULL,
	[Data12] [IMAGE] NULL,
	[Data13] [INT] NULL,
	[Data14] [MONEY] NULL,
	[Data15] [NCHAR] NULL,
	[Data16] [nvarchar](MAX) NULL,
	[Data17] [NUMERIC] NULL,
	[Data18] [NVARCHAR] NULL,
	[Data19] [REAL] NULL,
	[Data20] [SMALLDATETIME] NULL,
	[Data21] [SMALLINT] NULL,
	[Data22] [SMALLMONEY] NULL,
	--[Data23] [SQL_VARIANT] NULL,
	--[Data24] [SYSNAME] NULL,
	--[Data25] [TABLE] NULL,
	[Data26] [TEXT] NULL,
	[Data27] [TIME] NULL,
	--[Data28] [TIMESTAMP] NULL,
	[Data29] [TINYINT] NULL,
	[Data30] [UNIQUEIDENTIFIER] NULL,
	[Data31] [VARBINARY] NULL,
	[Data32] [VARCHAR] NULL,
	--[Data33] [XML] NULL,
	
	CONSTRAINT [pk_TableTypeTest] PRIMARY KEY
	(
		[PkId]
	)	
)
GO


CREATE VIEW [dbo].[EventLogAggregation] AS
	select
	min([CreationTimestamp]) as [MinCreationTimestamp],
	max([ModificationTimestamp]) as [MaxModificationTimestamp]
	from [dbo].[EventLog]
GO


CREATE PROCEDURE [dbo].[GetBlahBlahBlah]
(
	@pInt as int,
	@pImage as image,
	@pVarchar varchar(100) output
)
AS
BEGIN
	set @pVarchar = 'TEXTMETAL'

	SELECT 1 as a, 2 as b, @pInt as c, @pImage as d
	union all
	SELECT 1 as a, 2 as b, @pInt as c, @pImage as d 

	return 16
END
GO
