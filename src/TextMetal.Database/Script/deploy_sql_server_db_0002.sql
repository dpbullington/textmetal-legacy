/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/


USE [TextMetalWebHostSample]
GO


CREATE SCHEMA [history]
GO


CREATE TABLE [history].[UserHistory]
(
	[UserHistoryId] [bigint] IDENTITY(1,1) NOT NULL,
	[UserHistoryTs] [datetime2] NOT NULL,
	[UserId] [int] NOT NULL,

	[EmailAddress] [nvarchar](255) NULL,
	[UserName] [nvarchar](255) NULL,
	[SaltValue] [nvarchar](255) NULL,
	[PasswordHash] [nvarchar](255) NULL,
	[Question] [nvarchar](255) NULL,
	[AnswerHash] [nvarchar](255) NULL,
	[LastLoginSuccessTimestamp] [datetime] NULL,
	[LastLoginFailureTimestamp] [datetime] NULL,
	[FailedLoginCount] [smallint] NULL,
	[MustChangePassword] [bit] NULL,
	[SortOrder] [tinyint] NULL,
	[CreationTimestamp] [datetime] NULL,
	[ModificationTimestamp] [datetime] NULL,
	[CreationUserId] [int] NULL,
	[ModificationUserId] [int] NULL,
	[LogicalDelete] [bit] NULL,

	CONSTRAINT [pk_UserHistory] PRIMARY KEY
	(
		[UserHistoryId]
	)
)
GO

CREATE NONCLUSTERED INDEX [ix_UserHistory] ON [history].[UserHistory]
(
	[UserHistoryTs] ASC,
	[UserId] ASC
) ON [PRIMARY]
GO


CREATE TRIGGER [dbo].[OnUserChange] ON [dbo].[User] AFTER INSERT, UPDATE, DELETE
AS
	BEGIN
		SET NOCOUNT ON;
		
		DECLARE @ThisMoment [datetime2]
		
		SET @ThisMoment = SYSUTCDATETIME()

		INSERT INTO [history].[UserHistory]
		SELECT
			@ThisMoment AS [UserHistoryTs],
			i.[UserId],

			i.[EmailAddress],
			i.[UserName],
			i.[SaltValue],
			i.[PasswordHash],
			i.[Question],
			i.[AnswerHash],
			i.[LastLoginSuccessTimestamp],
			i.[LastLoginFailureTimestamp],
			i.[FailedLoginCount],
			i.[MustChangePassword],
			i.[SortOrder],
			i.[CreationTimestamp],
			i.[ModificationTimestamp],
			i.[CreationUserId],
			i.[ModificationUserId],
			i.[LogicalDelete]
		FROM [inserted] i

		UNION ALL

		SELECT
			@ThisMoment AS [UserHistoryTs],
			d.[UserId],

			NULL AS [EmailAddress],
			NULL AS [UserName],
			NULL AS [SaltValue],
			NULL AS [PasswordHash],
			NULL AS [Question],
			NULL AS [AnswerHash],
			NULL AS [LastLoginSuccessTimestamp],
			NULL AS [LastLoginFailureTimestamp],
			NULL AS [FailedLoginCount],
			NULL AS [MustChangePassword],
			NULL AS [SortOrder],
			NULL AS [CreationTimestamp],
			NULL AS [ModificationTimestamp],
			NULL AS [CreationUserId],
			NULL AS [ModificationUserId],
			NULL AS [LogicalDelete]
		FROM [deleted] d
		LEFT OUTER JOIN [inserted] i ON i.[UserId] = d.[UserId]
		WHERE i.[UserId] IS NULL
	END
GO


CREATE TABLE [history].[EventLogHistory]
(
	[EventLogHistoryId] [bigint] IDENTITY(1,1) NOT NULL,
	[EventLogHistoryTs] [datetime2] NOT NULL,
	[EventLogId] [int] NOT NULL,

	[EventText] [nvarchar](MAX) NULL,
	[SortOrder] [tinyint] NULL,
	[CreationTimestamp] [datetime] NULL,
	[ModificationTimestamp] [datetime] NULL,
	[CreationUserId] [int] NULL,
	[ModificationUserId] [int] NULL,
	[LogicalDelete] [bit] NULL,

	CONSTRAINT [pk_EventLogHistory] PRIMARY KEY
	(
		[EventLogHistoryId]
	)
)
GO

CREATE NONCLUSTERED INDEX [ix_EventLogHistory] ON [history].[EventLogHistory]
(
	[EventLogHistoryTs] ASC,
	[EventLogId] ASC
) ON [PRIMARY]
GO


CREATE TRIGGER [dbo].[OnEventLogChange] ON [dbo].[EventLog] AFTER INSERT, UPDATE, DELETE
AS
	BEGIN
		SET NOCOUNT ON;
		
		DECLARE @ThisMoment [datetime2]
		
		SET @ThisMoment = SYSUTCDATETIME()

		INSERT INTO [history].[EventLogHistory]
		SELECT
			@ThisMoment AS [EventLogHistoryTs],
			i.[EventLogId],

			i.[EventText],
			i.[SortOrder],
			i.[CreationTimestamp],
			i.[ModificationTimestamp],
			i.[CreationUserId],
			i.[ModificationUserId],
			i.[LogicalDelete]
		FROM [inserted] i

		UNION ALL

		SELECT
			@ThisMoment AS [EventLogHistoryTs],
			d.[EventLogId],

			NULL AS [EventText],
			NULL AS [SortOrder],
			NULL AS [CreationTimestamp],
			NULL AS [ModificationTimestamp],
			NULL AS [CreationUserId],
			NULL AS [ModificationUserId],
			NULL AS [LogicalDelete]
		FROM [deleted] d
		LEFT OUTER JOIN [inserted] i ON i.[EventLogId] = d.[EventLogId]
		WHERE i.[EventLogId] IS NULL
	END
GO


CREATE TABLE [history].[EmailMessageHistory]
(
	[EmailMessageHistoryId] [bigint] IDENTITY(1,1) NOT NULL,
	[EmailMessageHistoryTs] [datetime2] NOT NULL,
	[EmailMessageId] [int] NOT NULL,

	[From] [nvarchar](2047) NULL,
	[Sender] [nvarchar](2047) NULL,
	[ReplyTo] [nvarchar](2047) NULL,
	[To] [nvarchar](2047) NULL,
	[CarbonCopy] [nvarchar](2047) NULL,
	[BlindCarbonCopy] [nvarchar](2047) NULL,
	[Subject] [nvarchar](2047) NULL,
	[IsBodyHtml] [bit] NULL,
	[Body] [nvarchar](MAX) NULL,
	[Processed] [bit] NULL,
	[SortOrder] [tinyint] NULL,
	[CreationTimestamp] [datetime] NULL,
	[ModificationTimestamp] [datetime] NULL,
	[CreationUserId] [int] NULL,
	[ModificationUserId] [int] NULL,
	[LogicalDelete] [bit] NULL,

	CONSTRAINT [pk_EmailMessageHistory] PRIMARY KEY
	(
		[EmailMessageHistoryId]
	)
)
GO

CREATE NONCLUSTERED INDEX [ix_EmailMessageHistory] ON [history].[EmailMessageHistory]
(
	[EmailMessageHistoryTs] ASC,
	[EmailMessageId] ASC
) ON [PRIMARY]
GO


CREATE TRIGGER [dbo].[OnEmailMessageChange] ON [dbo].[EmailMessage] AFTER INSERT, UPDATE, DELETE
AS
	BEGIN
		SET NOCOUNT ON;
		
		DECLARE @ThisMoment [datetime2]
		
		SET @ThisMoment = SYSUTCDATETIME()

		INSERT INTO [history].[EmailMessageHistory]
		SELECT
			@ThisMoment AS [EmailMessageHistoryTs],
			i.[EmailMessageId],

			i.[From],
			i.[Sender],
			i.[ReplyTo],
			i.[To],
			i.[CarbonCopy],
			i.[BlindCarbonCopy],
			i.[Subject],
			i.[IsBodyHtml],
			i.[Body],
			i.[Processed],
			i.[SortOrder],
			i.[CreationTimestamp],
			i.[ModificationTimestamp],
			i.[CreationUserId],
			i.[ModificationUserId],
			i.[LogicalDelete]
		FROM [inserted] i

		UNION ALL

		SELECT
			@ThisMoment AS [EmailMessageHistoryTs],
			d.[EmailMessageId],

			NULL AS [From],
			NULL AS [Sender],
			NULL AS [ReplyTo],
			NULL AS [To],
			NULL AS [CarbonCopy],
			NULL AS [BlindCarbonCopy],
			NULL AS [Subject],
			NULL AS [IsBodyHtml],
			NULL AS [Body],
			NULL AS [Processed],
			NULL AS [SortOrder],
			NULL AS [CreationTimestamp],
			NULL AS [ModificationTimestamp],
			NULL AS [CreationUserId],
			NULL AS [ModificationUserId],
			NULL AS [LogicalDelete]
		FROM [deleted] d
		LEFT OUTER JOIN [inserted] i ON i.[EmailMessageId] = d.[EmailMessageId]
		WHERE i.[EmailMessageId] IS NULL
	END
GO


CREATE TABLE [history].[EmailAttachmentHistory]
(
	[EmailAttachmentHistoryId] [bigint] IDENTITY(1,1) NOT NULL,
	[EmailAttachmentHistoryTs] [datetime2] NOT NULL,
	[EmailMessageId] [int] NOT NULL,

	[EmailAttachmentId] [int] NULL,
	[FileName] [nvarchar](255) NULL,
	[FileSize] [bigint] NULL,
	[MimeType] [nvarchar](255) NULL,
	[AttachmentBits] [varbinary](MAX) NULL,
	[SortOrder] [tinyint] NULL,
	[CreationTimestamp] [datetime] NULL,
	[ModificationTimestamp] [datetime] NULL,
	[CreationUserId] [int] NULL,
	[ModificationUserId] [int] NULL,
	[LogicalDelete] [bit] NULL,

	CONSTRAINT [pk_EmailAttachmentHistory] PRIMARY KEY
	(
		[EmailAttachmentHistoryId]
	)
)
GO

CREATE NONCLUSTERED INDEX [ix_EmailAttachmentHistory] ON [history].[EmailAttachmentHistory]
(
	[EmailAttachmentHistoryTs] ASC,
	[EmailMessageId] ASC
) ON [PRIMARY]
GO


CREATE TRIGGER [dbo].[OnEmailAttachmentChange] ON [dbo].[EmailAttachment] AFTER INSERT, UPDATE, DELETE
AS
	BEGIN
		SET NOCOUNT ON;
		
		DECLARE @ThisMoment [datetime2]
		
		SET @ThisMoment = SYSUTCDATETIME()

		INSERT INTO [history].[EmailAttachmentHistory]
		SELECT
			@ThisMoment AS [EmailAttachmentHistoryTs],
			i.[EmailMessageId],

			i.[EmailAttachmentId],
			i.[FileName],
			i.[FileSize],
			i.[MimeType],
			i.[AttachmentBits],
			i.[SortOrder],
			i.[CreationTimestamp],
			i.[ModificationTimestamp],
			i.[CreationUserId],
			i.[ModificationUserId],
			i.[LogicalDelete]
		FROM [inserted] i

		UNION ALL

		SELECT
			@ThisMoment AS [EmailAttachmentHistoryTs],
			d.[EmailMessageId],

			NULL AS [EmailAttachmentId],
			NULL AS [FileName],
			NULL AS [FileSize],
			NULL AS [MimeType],
			NULL AS [AttachmentBits],
			NULL AS [SortOrder],
			NULL AS [CreationTimestamp],
			NULL AS [ModificationTimestamp],
			NULL AS [CreationUserId],
			NULL AS [ModificationUserId],
			NULL AS [LogicalDelete]
		FROM [deleted] d
		LEFT OUTER JOIN [inserted] i ON i.[EmailMessageId] = d.[EmailMessageId]
		WHERE i.[EmailMessageId] IS NULL
	END
GO


CREATE TABLE [history].[TableWithPrimaryKeyAsIdentityHistory]
(
	[TableWithPrimaryKeyAsIdentityHistoryId] [bigint] IDENTITY(1,1) NOT NULL,
	[TableWithPrimaryKeyAsIdentityHistoryTs] [datetime2] NOT NULL,
	[PkId] [int] NOT NULL,

	[Data01] [bit] NULL,
	[Data02] [datetime] NULL,
	[Data03] [int] NULL,
	[Data04] [nvarchar](100) NULL,

	CONSTRAINT [pk_TableWithPrimaryKeyAsIdentityHistory] PRIMARY KEY
	(
		[TableWithPrimaryKeyAsIdentityHistoryId]
	)
)
GO

CREATE NONCLUSTERED INDEX [ix_TableWithPrimaryKeyAsIdentityHistory] ON [history].[TableWithPrimaryKeyAsIdentityHistory]
(
	[TableWithPrimaryKeyAsIdentityHistoryTs] ASC,
	[PkId] ASC
) ON [PRIMARY]
GO


CREATE TRIGGER [dbo].[OnTableWithPrimaryKeyAsIdentityChange] ON [dbo].[TableWithPrimaryKeyAsIdentity] AFTER INSERT, UPDATE, DELETE
AS
	BEGIN
		SET NOCOUNT ON;
		
		DECLARE @ThisMoment [datetime2]
		
		SET @ThisMoment = SYSUTCDATETIME()

		INSERT INTO [history].[TableWithPrimaryKeyAsIdentityHistory]
		SELECT
			@ThisMoment AS [TableWithPrimaryKeyAsIdentityHistoryTs],
			i.[PkId],

			i.[Data01],
			i.[Data02],
			i.[Data03],
			i.[Data04]
		FROM [inserted] i

		UNION ALL

		SELECT
			@ThisMoment AS [TableWithPrimaryKeyAsIdentityHistoryTs],
			d.[PkId],

			NULL AS [Data01],
			NULL AS [Data02],
			NULL AS [Data03],
			NULL AS [Data04]
		FROM [deleted] d
		LEFT OUTER JOIN [inserted] i ON i.[PkId] = d.[PkId]
		WHERE i.[PkId] IS NULL
	END
GO


CREATE TABLE [history].[TableWithPrimaryKeyAsDefaultHistory]
(
	[TableWithPrimaryKeyAsDefaultHistoryId] [bigint] IDENTITY(1,1) NOT NULL,
	[TableWithPrimaryKeyAsDefaultHistoryTs] [datetime2] NOT NULL,
	[PkDf] [uniqueidentifier] NOT NULL,

	[Data01] [bit] NULL,
	[Data02] [datetime] NULL,
	[Data03] [int] NULL,
	[Data04] [nvarchar](100) NULL,

	CONSTRAINT [pk_TableWithPrimaryKeyAsDefaultHistory] PRIMARY KEY
	(
		[TableWithPrimaryKeyAsDefaultHistoryId]
	)
)
GO

CREATE NONCLUSTERED INDEX [ix_TableWithPrimaryKeyAsDefaultHistory] ON [history].[TableWithPrimaryKeyAsDefaultHistory]
(
	[TableWithPrimaryKeyAsDefaultHistoryTs] ASC,
	[PkDf] ASC
) ON [PRIMARY]
GO


CREATE TRIGGER [dbo].[OnTableWithPrimaryKeyAsDefaultChange] ON [dbo].[TableWithPrimaryKeyAsDefault] AFTER INSERT, UPDATE, DELETE
AS
	BEGIN
		SET NOCOUNT ON;
		
		DECLARE @ThisMoment [datetime2]
		
		SET @ThisMoment = SYSUTCDATETIME()

		INSERT INTO [history].[TableWithPrimaryKeyAsDefaultHistory]
		SELECT
			@ThisMoment AS [TableWithPrimaryKeyAsDefaultHistoryTs],
			i.[PkDf],

			i.[Data01],
			i.[Data02],
			i.[Data03],
			i.[Data04]
		FROM [inserted] i

		UNION ALL

		SELECT
			@ThisMoment AS [TableWithPrimaryKeyAsDefaultHistoryTs],
			d.[PkDf],

			NULL AS [Data01],
			NULL AS [Data02],
			NULL AS [Data03],
			NULL AS [Data04]
		FROM [deleted] d
		LEFT OUTER JOIN [inserted] i ON i.[PkDf] = d.[PkDf]
		WHERE i.[PkDf] IS NULL
	END
GO


CREATE TABLE [history].[TableWithPrimaryKeyWithDiffIdentityHistory]
(
	[TableWithPrimaryKeyWithDiffIdentityHistoryId] [bigint] IDENTITY(1,1) NOT NULL,
	[TableWithPrimaryKeyWithDiffIdentityHistoryTs] [datetime2] NOT NULL,
	[Pk] [int] NOT NULL,

	[Id] [int] NULL,
	[Data01] [bit] NULL,
	[Data02] [datetime] NULL,
	[Data03] [int] NULL,
	[Data04] [nvarchar](100) NULL,

	CONSTRAINT [pk_TableWithPrimaryKeyWithDiffIdentityHistory] PRIMARY KEY
	(
		[TableWithPrimaryKeyWithDiffIdentityHistoryId]
	)
)
GO

CREATE NONCLUSTERED INDEX [ix_TableWithPrimaryKeyWithDiffIdentityHistory] ON [history].[TableWithPrimaryKeyWithDiffIdentityHistory]
(
	[TableWithPrimaryKeyWithDiffIdentityHistoryTs] ASC,
	[Pk] ASC
) ON [PRIMARY]
GO


CREATE TRIGGER [dbo].[OnTableWithPrimaryKeyWithDiffIdentityChange] ON [dbo].[TableWithPrimaryKeyWithDiffIdentity] AFTER INSERT, UPDATE, DELETE
AS
	BEGIN
		SET NOCOUNT ON;
		
		DECLARE @ThisMoment [datetime2]
		
		SET @ThisMoment = SYSUTCDATETIME()

		INSERT INTO [history].[TableWithPrimaryKeyWithDiffIdentityHistory]
		SELECT
			@ThisMoment AS [TableWithPrimaryKeyWithDiffIdentityHistoryTs],
			i.[Pk],

			i.[Id],
			i.[Data01],
			i.[Data02],
			i.[Data03],
			i.[Data04]
		FROM [inserted] i

		UNION ALL

		SELECT
			@ThisMoment AS [TableWithPrimaryKeyWithDiffIdentityHistoryTs],
			d.[Pk],

			NULL AS [Id],
			NULL AS [Data01],
			NULL AS [Data02],
			NULL AS [Data03],
			NULL AS [Data04]
		FROM [deleted] d
		LEFT OUTER JOIN [inserted] i ON i.[Pk] = d.[Pk]
		WHERE i.[Pk] IS NULL
	END
GO


CREATE TABLE [history].[TableNoPrimaryKeyWithIdentityHistory]
(
	[TableNoPrimaryKeyWithIdentityHistoryId] [bigint] IDENTITY(1,1) NOT NULL,
	[TableNoPrimaryKeyWithIdentityHistoryTs] [datetime2] NOT NULL,
	[Id] [int] NOT NULL,
	[Data01] [bit] NOT NULL,
	[Data02] [datetime] NOT NULL,
	[Data03] [int] NOT NULL,
	[Data04] [nvarchar](100) NOT NULL,



	CONSTRAINT [pk_TableNoPrimaryKeyWithIdentityHistory] PRIMARY KEY
	(
		[TableNoPrimaryKeyWithIdentityHistoryId]
	)
)
GO

CREATE NONCLUSTERED INDEX [ix_TableNoPrimaryKeyWithIdentityHistory] ON [history].[TableNoPrimaryKeyWithIdentityHistory]
(
	[TableNoPrimaryKeyWithIdentityHistoryTs] ASC,
	[Id] ASC,
	[Data01] ASC,
	[Data02] ASC,
	[Data03] ASC,
	[Data04] ASC
) ON [PRIMARY]
GO


CREATE TRIGGER [dbo].[OnTableNoPrimaryKeyWithIdentityChange] ON [dbo].[TableNoPrimaryKeyWithIdentity] AFTER INSERT, UPDATE, DELETE
AS
	BEGIN
		SET NOCOUNT ON;
		
		DECLARE @ThisMoment [datetime2]
		
		SET @ThisMoment = SYSUTCDATETIME()

		INSERT INTO [history].[TableNoPrimaryKeyWithIdentityHistory]
		SELECT
			@ThisMoment AS [TableNoPrimaryKeyWithIdentityHistoryTs],
			i.[Id],
			i.[Data01],
			i.[Data02],
			i.[Data03],
			i.[Data04],


		FROM [inserted] i

		UNION ALL

		SELECT
			@ThisMoment AS [TableNoPrimaryKeyWithIdentityHistoryTs],
			d.[Id],
			d.[Data01],
			d.[Data02],
			d.[Data03],
			d.[Data04],


		FROM [deleted] d
		LEFT OUTER JOIN [inserted] i ON i.[Id] = d.[Id] AND i.[Data01] = d.[Data01] AND i.[Data02] = d.[Data02] AND i.[Data03] = d.[Data03] AND i.[Data04] = d.[Data04]
		WHERE i.[Id] IS NULL AND i.[Data01] IS NULL AND i.[Data02] IS NULL AND i.[Data03] IS NULL AND i.[Data04] IS NULL
	END
GO


CREATE TABLE [history].[TableWithPrimaryKeyNoIdentityHistory]
(
	[TableWithPrimaryKeyNoIdentityHistoryId] [bigint] IDENTITY(1,1) NOT NULL,
	[TableWithPrimaryKeyNoIdentityHistoryTs] [datetime2] NOT NULL,
	[Pk] [int] NOT NULL,

	[Data01] [bit] NULL,
	[Data02] [datetime] NULL,
	[Data03] [int] NULL,
	[Data04] [nvarchar](100) NULL,

	CONSTRAINT [pk_TableWithPrimaryKeyNoIdentityHistory] PRIMARY KEY
	(
		[TableWithPrimaryKeyNoIdentityHistoryId]
	)
)
GO

CREATE NONCLUSTERED INDEX [ix_TableWithPrimaryKeyNoIdentityHistory] ON [history].[TableWithPrimaryKeyNoIdentityHistory]
(
	[TableWithPrimaryKeyNoIdentityHistoryTs] ASC,
	[Pk] ASC
) ON [PRIMARY]
GO


CREATE TRIGGER [dbo].[OnTableWithPrimaryKeyNoIdentityChange] ON [dbo].[TableWithPrimaryKeyNoIdentity] AFTER INSERT, UPDATE, DELETE
AS
	BEGIN
		SET NOCOUNT ON;
		
		DECLARE @ThisMoment [datetime2]
		
		SET @ThisMoment = SYSUTCDATETIME()

		INSERT INTO [history].[TableWithPrimaryKeyNoIdentityHistory]
		SELECT
			@ThisMoment AS [TableWithPrimaryKeyNoIdentityHistoryTs],
			i.[Pk],

			i.[Data01],
			i.[Data02],
			i.[Data03],
			i.[Data04]
		FROM [inserted] i

		UNION ALL

		SELECT
			@ThisMoment AS [TableWithPrimaryKeyNoIdentityHistoryTs],
			d.[Pk],

			NULL AS [Data01],
			NULL AS [Data02],
			NULL AS [Data03],
			NULL AS [Data04]
		FROM [deleted] d
		LEFT OUTER JOIN [inserted] i ON i.[Pk] = d.[Pk]
		WHERE i.[Pk] IS NULL
	END
GO


CREATE TABLE [history].[TableWithCompositePrimaryKeyNoIdentityHistory]
(
	[TableWithCompositePrimaryKeyNoIdentityHistoryId] [bigint] IDENTITY(1,1) NOT NULL,
	[TableWithCompositePrimaryKeyNoIdentityHistoryTs] [datetime2] NOT NULL,
	[Pk0] [int] NOT NULL,
	[Pk1] [int] NOT NULL,
	[Pk2] [int] NOT NULL,
	[Pk3] [int] NOT NULL,

	[Data01] [bit] NULL,
	[Data02] [datetime] NULL,
	[Data03] [int] NULL,
	[Data04] [nvarchar](100) NULL,

	CONSTRAINT [pk_TableWithCompositePrimaryKeyNoIdentityHistory] PRIMARY KEY
	(
		[TableWithCompositePrimaryKeyNoIdentityHistoryId]
	)
)
GO

CREATE NONCLUSTERED INDEX [ix_TableWithCompositePrimaryKeyNoIdentityHistory] ON [history].[TableWithCompositePrimaryKeyNoIdentityHistory]
(
	[TableWithCompositePrimaryKeyNoIdentityHistoryTs] ASC,
	[Pk0] ASC,
	[Pk1] ASC,
	[Pk2] ASC,
	[Pk3] ASC
) ON [PRIMARY]
GO


CREATE TRIGGER [dbo].[OnTableWithCompositePrimaryKeyNoIdentityChange] ON [dbo].[TableWithCompositePrimaryKeyNoIdentity] AFTER INSERT, UPDATE, DELETE
AS
	BEGIN
		SET NOCOUNT ON;
		
		DECLARE @ThisMoment [datetime2]
		
		SET @ThisMoment = SYSUTCDATETIME()

		INSERT INTO [history].[TableWithCompositePrimaryKeyNoIdentityHistory]
		SELECT
			@ThisMoment AS [TableWithCompositePrimaryKeyNoIdentityHistoryTs],
			i.[Pk0],
			i.[Pk1],
			i.[Pk2],
			i.[Pk3],

			i.[Data01],
			i.[Data02],
			i.[Data03],
			i.[Data04]
		FROM [inserted] i

		UNION ALL

		SELECT
			@ThisMoment AS [TableWithCompositePrimaryKeyNoIdentityHistoryTs],
			d.[Pk0],
			d.[Pk1],
			d.[Pk2],
			d.[Pk3],

			NULL AS [Data01],
			NULL AS [Data02],
			NULL AS [Data03],
			NULL AS [Data04]
		FROM [deleted] d
		LEFT OUTER JOIN [inserted] i ON i.[Pk0] = d.[Pk0] AND i.[Pk1] = d.[Pk1] AND i.[Pk2] = d.[Pk2] AND i.[Pk3] = d.[Pk3]
		WHERE i.[Pk0] IS NULL AND i.[Pk1] IS NULL AND i.[Pk2] IS NULL AND i.[Pk3] IS NULL
	END
GO


CREATE TABLE [history].[TableNoPrimaryKeyNoIdentityHistory]
(
	[TableNoPrimaryKeyNoIdentityHistoryId] [bigint] IDENTITY(1,1) NOT NULL,
	[TableNoPrimaryKeyNoIdentityHistoryTs] [datetime2] NOT NULL,
	[Value] [int] NOT NULL,
	[Data01] [bit] NOT NULL,
	[Data02] [datetime] NOT NULL,
	[Data03] [int] NOT NULL,
	[Data04] [nvarchar](100) NOT NULL,



	CONSTRAINT [pk_TableNoPrimaryKeyNoIdentityHistory] PRIMARY KEY
	(
		[TableNoPrimaryKeyNoIdentityHistoryId]
	)
)
GO

CREATE NONCLUSTERED INDEX [ix_TableNoPrimaryKeyNoIdentityHistory] ON [history].[TableNoPrimaryKeyNoIdentityHistory]
(
	[TableNoPrimaryKeyNoIdentityHistoryTs] ASC,
	[Value] ASC,
	[Data01] ASC,
	[Data02] ASC,
	[Data03] ASC,
	[Data04] ASC
) ON [PRIMARY]
GO


CREATE TRIGGER [dbo].[OnTableNoPrimaryKeyNoIdentityChange] ON [dbo].[TableNoPrimaryKeyNoIdentity] AFTER INSERT, UPDATE, DELETE
AS
	BEGIN
		SET NOCOUNT ON;
		
		DECLARE @ThisMoment [datetime2]
		
		SET @ThisMoment = SYSUTCDATETIME()

		INSERT INTO [history].[TableNoPrimaryKeyNoIdentityHistory]
		SELECT
			@ThisMoment AS [TableNoPrimaryKeyNoIdentityHistoryTs],
			i.[Value],
			i.[Data01],
			i.[Data02],
			i.[Data03],
			i.[Data04],


		FROM [inserted] i

		UNION ALL

		SELECT
			@ThisMoment AS [TableNoPrimaryKeyNoIdentityHistoryTs],
			d.[Value],
			d.[Data01],
			d.[Data02],
			d.[Data03],
			d.[Data04],


		FROM [deleted] d
		LEFT OUTER JOIN [inserted] i ON i.[Value] = d.[Value] AND i.[Data01] = d.[Data01] AND i.[Data02] = d.[Data02] AND i.[Data03] = d.[Data03] AND i.[Data04] = d.[Data04]
		WHERE i.[Value] IS NULL AND i.[Data01] IS NULL AND i.[Data02] IS NULL AND i.[Data03] IS NULL AND i.[Data04] IS NULL
	END
GO


CREATE TABLE [history].[TableTypeTestHistory]
(
	[TableTypeTestHistoryId] [bigint] IDENTITY(1,1) NOT NULL,
	[TableTypeTestHistoryTs] [datetime2] NOT NULL,
	[PkId] [int] NOT NULL,

	[Data00] [bigint] NULL,
	[Data01] [binary] NULL,
	[Data02] [bit] NULL,
	[Data03] [char] NULL,
	[Data05] [date] NULL,
	[Data06] [datetime] NULL,
	[Data07] [datetime2] NULL,
	[Data08] [datetimeoffset] NULL,
	[Data09] [decimal] NULL,
	[Data10] [float] NULL,
	[Data12] [image] NULL,
	[Data13] [int] NULL,
	[Data14] [money] NULL,
	[Data15] [nchar] NULL,
	[Data16] [nvarchar](MAX) NULL,
	[Data17] [numeric] NULL,
	[Data18] [nvarchar](1) NULL,
	[Data19] [real] NULL,
	[Data20] [smalldatetime] NULL,
	[Data21] [smallint] NULL,
	[Data22] [smallmoney] NULL,
	[Data26] [text] NULL,
	[Data27] [time] NULL,
	[Data29] [tinyint] NULL,
	[Data30] [uniqueidentifier] NULL,
	[Data31] [varbinary](1) NULL,
	[Data32] [varchar](1) NULL,

	CONSTRAINT [pk_TableTypeTestHistory] PRIMARY KEY
	(
		[TableTypeTestHistoryId]
	)
)
GO

CREATE NONCLUSTERED INDEX [ix_TableTypeTestHistory] ON [history].[TableTypeTestHistory]
(
	[TableTypeTestHistoryTs] ASC,
	[PkId] ASC
) ON [PRIMARY]
GO


CREATE TRIGGER [dbo].[OnTableTypeTestChange] ON [dbo].[TableTypeTest] AFTER INSERT, UPDATE, DELETE
AS
	BEGIN
		SET NOCOUNT ON;
		
		DECLARE @ThisMoment [datetime2]
		
		SET @ThisMoment = SYSUTCDATETIME()

		INSERT INTO [history].[TableTypeTestHistory]
		SELECT
			@ThisMoment AS [TableTypeTestHistoryTs],
			i.[PkId],

			i.[Data00],
			i.[Data01],
			i.[Data02],
			i.[Data03],
			i.[Data05],
			i.[Data06],
			i.[Data07],
			i.[Data08],
			i.[Data09],
			i.[Data10],
			i.[Data12],
			i.[Data13],
			i.[Data14],
			i.[Data15],
			i.[Data16],
			i.[Data17],
			i.[Data18],
			i.[Data19],
			i.[Data20],
			i.[Data21],
			i.[Data22],
			i.[Data26],
			i.[Data27],
			i.[Data29],
			i.[Data30],
			i.[Data31],
			i.[Data32]
		FROM [inserted] i

		UNION ALL

		SELECT
			@ThisMoment AS [TableTypeTestHistoryTs],
			d.[PkId],

			NULL AS [Data00],
			NULL AS [Data01],
			NULL AS [Data02],
			NULL AS [Data03],
			NULL AS [Data05],
			NULL AS [Data06],
			NULL AS [Data07],
			NULL AS [Data08],
			NULL AS [Data09],
			NULL AS [Data10],
			NULL AS [Data12],
			NULL AS [Data13],
			NULL AS [Data14],
			NULL AS [Data15],
			NULL AS [Data16],
			NULL AS [Data17],
			NULL AS [Data18],
			NULL AS [Data19],
			NULL AS [Data20],
			NULL AS [Data21],
			NULL AS [Data22],
			NULL AS [Data26],
			NULL AS [Data27],
			NULL AS [Data29],
			NULL AS [Data30],
			NULL AS [Data31],
			NULL AS [Data32]
		FROM [deleted] d
		LEFT OUTER JOIN [inserted] i ON i.[PkId] = d.[PkId]
		WHERE i.[PkId] IS NULL
	END
GO


CREATE TABLE [history].[SexualChocolateHistory]
(
	[SexualChocolateHistoryId] [bigint] IDENTITY(1,1) NOT NULL,
	[SexualChocolateHistoryTs] [datetime2] NOT NULL,
	[SexualChocolateId] [int] NOT NULL,

	[EM] [nvarchar](MAX) NULL,
	[Blob] [varbinary](MAX) NULL,
	[CreationTimestamp] [datetime] NULL,
	[ModificationTimestamp] [datetime] NULL,
	[LogicalDelete] [bit] NULL,

	CONSTRAINT [pk_SexualChocolateHistory] PRIMARY KEY
	(
		[SexualChocolateHistoryId]
	)
)
GO

CREATE NONCLUSTERED INDEX [ix_SexualChocolateHistory] ON [history].[SexualChocolateHistory]
(
	[SexualChocolateHistoryTs] ASC,
	[SexualChocolateId] ASC
) ON [PRIMARY]
GO


CREATE TRIGGER [TxtMtl].[OnSexualChocolateChange] ON [TxtMtl].[SexualChocolate] AFTER INSERT, UPDATE, DELETE
AS
	BEGIN
		SET NOCOUNT ON;
		
		DECLARE @ThisMoment [datetime2]
		
		SET @ThisMoment = SYSUTCDATETIME()

		INSERT INTO [history].[SexualChocolateHistory]
		SELECT
			@ThisMoment AS [SexualChocolateHistoryTs],
			i.[SexualChocolateId],

			i.[EM],
			i.[Blob],
			i.[CreationTimestamp],
			i.[ModificationTimestamp],
			i.[LogicalDelete]
		FROM [inserted] i

		UNION ALL

		SELECT
			@ThisMoment AS [SexualChocolateHistoryTs],
			d.[SexualChocolateId],

			NULL AS [EM],
			NULL AS [Blob],
			NULL AS [CreationTimestamp],
			NULL AS [ModificationTimestamp],
			NULL AS [LogicalDelete]
		FROM [deleted] d
		LEFT OUTER JOIN [inserted] i ON i.[SexualChocolateId] = d.[SexualChocolateId]
		WHERE i.[SexualChocolateId] IS NULL
	END
GO
