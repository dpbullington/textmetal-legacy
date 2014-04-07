/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/


SET NOCOUNT ON
GO


CREATE SCHEMA [cdc]
GO


CREATE TABLE [cdc].[UserHistory]
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

CREATE NONCLUSTERED INDEX [ix_UserHistory] ON [cdc].[UserHistory]
(
	[UserHistoryTs] ASC,
	[UserId] ASC
) ON [PRIMARY]
GO


CREATE TRIGGER [global].[OnUserChange] ON [global].[User] AFTER INSERT, UPDATE, DELETE
AS
	BEGIN
		SET NOCOUNT ON;
		
		DECLARE @ThisMoment [datetime2]
		
		SET @ThisMoment = SYSUTCDATETIME()

		INSERT INTO [cdc].[UserHistory]
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


CREATE TABLE [cdc].[PropertyBagHistory]
(
	[PropertyBagHistoryId] [bigint] IDENTITY(1,1) NOT NULL,
	[PropertyBagHistoryTs] [datetime2] NOT NULL,
	[PropertyBagId] [int] NOT NULL,

	[PropertyKey] [nvarchar](255) NULL,
	[PropertyType] [nvarchar](255) NULL,
	[PropertyValue] [nvarchar](2047) NULL,
	[SortOrder] [tinyint] NULL,
	[CreationTimestamp] [datetime] NULL,
	[ModificationTimestamp] [datetime] NULL,
	[CreationUserId] [int] NULL,
	[ModificationUserId] [int] NULL,
	[LogicalDelete] [bit] NULL,

	CONSTRAINT [pk_PropertyBagHistory] PRIMARY KEY
	(
		[PropertyBagHistoryId]
	)
)
GO

CREATE NONCLUSTERED INDEX [ix_PropertyBagHistory] ON [cdc].[PropertyBagHistory]
(
	[PropertyBagHistoryTs] ASC,
	[PropertyBagId] ASC
) ON [PRIMARY]
GO


CREATE TRIGGER [global].[OnPropertyBagChange] ON [global].[PropertyBag] AFTER INSERT, UPDATE, DELETE
AS
	BEGIN
		SET NOCOUNT ON;
		
		DECLARE @ThisMoment [datetime2]
		
		SET @ThisMoment = SYSUTCDATETIME()

		INSERT INTO [cdc].[PropertyBagHistory]
		SELECT
			@ThisMoment AS [PropertyBagHistoryTs],
			i.[PropertyBagId],

			i.[PropertyKey],
			i.[PropertyType],
			i.[PropertyValue],
			i.[SortOrder],
			i.[CreationTimestamp],
			i.[ModificationTimestamp],
			i.[CreationUserId],
			i.[ModificationUserId],
			i.[LogicalDelete]
		FROM [inserted] i

		UNION ALL

		SELECT
			@ThisMoment AS [PropertyBagHistoryTs],
			d.[PropertyBagId],

			NULL AS [PropertyKey],
			NULL AS [PropertyType],
			NULL AS [PropertyValue],
			NULL AS [SortOrder],
			NULL AS [CreationTimestamp],
			NULL AS [ModificationTimestamp],
			NULL AS [CreationUserId],
			NULL AS [ModificationUserId],
			NULL AS [LogicalDelete]
		FROM [deleted] d
		LEFT OUTER JOIN [inserted] i ON i.[PropertyBagId] = d.[PropertyBagId]
		WHERE i.[PropertyBagId] IS NULL
	END
GO


CREATE TABLE [cdc].[EventLogHistory]
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

CREATE NONCLUSTERED INDEX [ix_EventLogHistory] ON [cdc].[EventLogHistory]
(
	[EventLogHistoryTs] ASC,
	[EventLogId] ASC
) ON [PRIMARY]
GO


CREATE TRIGGER [global].[OnEventLogChange] ON [global].[EventLog] AFTER INSERT, UPDATE, DELETE
AS
	BEGIN
		SET NOCOUNT ON;
		
		DECLARE @ThisMoment [datetime2]
		
		SET @ThisMoment = SYSUTCDATETIME()

		INSERT INTO [cdc].[EventLogHistory]
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


CREATE TABLE [cdc].[EmailMessageHistory]
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

CREATE NONCLUSTERED INDEX [ix_EmailMessageHistory] ON [cdc].[EmailMessageHistory]
(
	[EmailMessageHistoryTs] ASC,
	[EmailMessageId] ASC
) ON [PRIMARY]
GO


CREATE TRIGGER [global].[OnEmailMessageChange] ON [global].[EmailMessage] AFTER INSERT, UPDATE, DELETE
AS
	BEGIN
		SET NOCOUNT ON;
		
		DECLARE @ThisMoment [datetime2]
		
		SET @ThisMoment = SYSUTCDATETIME()

		INSERT INTO [cdc].[EmailMessageHistory]
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


CREATE TABLE [cdc].[EmailAttachmentHistory]
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

CREATE NONCLUSTERED INDEX [ix_EmailAttachmentHistory] ON [cdc].[EmailAttachmentHistory]
(
	[EmailAttachmentHistoryTs] ASC,
	[EmailMessageId] ASC
) ON [PRIMARY]
GO


CREATE TRIGGER [global].[OnEmailAttachmentChange] ON [global].[EmailAttachment] AFTER INSERT, UPDATE, DELETE
AS
	BEGIN
		SET NOCOUNT ON;
		
		DECLARE @ThisMoment [datetime2]
		
		SET @ThisMoment = SYSUTCDATETIME()

		INSERT INTO [cdc].[EmailAttachmentHistory]
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

