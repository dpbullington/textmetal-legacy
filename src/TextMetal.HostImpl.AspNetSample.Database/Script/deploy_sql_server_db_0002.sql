/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/


SET NOCOUNT ON
GO


CREATE SCHEMA [history]
GO


CREATE TABLE [history].[OrganizationHistory]
(
	[OrganizationHistoryId] [bigint] IDENTITY(1,1) NOT NULL,
	[OrganizationHistoryTs] [datetime2] NOT NULL,
	[OrganizationId] [int] NOT NULL,

	[ParentOrganizationId] [int] NULL,
	[OrganizationName] [nvarchar](255) NULL,
	[TimeZoneId] [nvarchar](255) NULL,
	[SortOrder] [tinyint] NULL,
	[CreationTimestamp] [datetime] NULL,
	[ModificationTimestamp] [datetime] NULL,
	[CreationUserId] [int] NULL,
	[ModificationUserId] [int] NULL,
	[LogicalDelete] [bit] NULL,

	CONSTRAINT [pk_OrganizationHistory] PRIMARY KEY
	(
		[OrganizationHistoryId]
	)
)
GO


CREATE NONCLUSTERED INDEX [ix_OrganizationHistory] ON [history].[OrganizationHistory]
(
	[OrganizationHistoryTs] ASC,
	[OrganizationId] ASC
) ON [PRIMARY]
GO


CREATE TRIGGER [application].[OnOrganizationChange] ON [application].[Organization] AFTER INSERT, UPDATE, DELETE
AS
	BEGIN
		SET NOCOUNT ON;
		
		DECLARE @ThisMoment [datetime2]
		
		SET @ThisMoment = SYSUTCDATETIME()

		INSERT INTO [history].[OrganizationHistory]
		SELECT
			@ThisMoment AS [OrganizationHistoryTs],
			i.[OrganizationId],

			i.[ParentOrganizationId],
			i.[OrganizationName],
			i.[TimeZoneId],
			i.[SortOrder],
			i.[CreationTimestamp],
			i.[ModificationTimestamp],
			i.[CreationUserId],
			i.[ModificationUserId],
			i.[LogicalDelete]
		FROM [inserted] i

		UNION ALL

		SELECT
			@ThisMoment AS [OrganizationHistoryTs],
			d.[OrganizationId],

			NULL AS [ParentOrganizationId],
			NULL AS [OrganizationName],
			NULL AS [TimeZoneId],
			NULL AS [SortOrder],
			NULL AS [CreationTimestamp],
			NULL AS [ModificationTimestamp],
			NULL AS [CreationUserId],
			NULL AS [ModificationUserId],
			NULL AS [LogicalDelete]
		FROM [deleted] d
		LEFT OUTER JOIN [inserted] i ON i.[OrganizationId] = d.[OrganizationId]
		WHERE i.[OrganizationId] IS NULL
	END
GO


CREATE TABLE [history].[MemberHistory]
(
	[MemberHistoryId] [bigint] IDENTITY(1,1) NOT NULL,
	[MemberHistoryTs] [datetime2] NOT NULL,
	[MemberId] [int] NOT NULL,

	[OrganizationId] [int] NULL,
	[ParentMemberId] [int] NULL,
	[MemberName] [nvarchar](255) NULL,
	[MemberTitle] [nvarchar](255) NULL,
	[SecurityRoleId] [int] NULL,
	[SortOrder] [tinyint] NULL,
	[CreationTimestamp] [datetime] NULL,
	[ModificationTimestamp] [datetime] NULL,
	[CreationUserId] [int] NULL,
	[ModificationUserId] [int] NULL,
	[LogicalDelete] [bit] NULL,

	CONSTRAINT [pk_MemberHistory] PRIMARY KEY
	(
		[MemberHistoryId]
	)
)
GO


CREATE NONCLUSTERED INDEX [ix_MemberHistory] ON [history].[MemberHistory]
(
	[MemberHistoryTs] ASC,
	[MemberId] ASC
) ON [PRIMARY]
GO


CREATE TRIGGER [application].[OnMemberChange] ON [application].[Member] AFTER INSERT, UPDATE, DELETE
AS
	BEGIN
		SET NOCOUNT ON;
		
		DECLARE @ThisMoment [datetime2]
		
		SET @ThisMoment = SYSUTCDATETIME()

		INSERT INTO [history].[MemberHistory]
		SELECT
			@ThisMoment AS [MemberHistoryTs],
			i.[MemberId],

			i.[OrganizationId],
			i.[ParentMemberId],
			i.[MemberName],
			i.[MemberTitle],
			i.[SecurityRoleId],
			i.[SortOrder],
			i.[CreationTimestamp],
			i.[ModificationTimestamp],
			i.[CreationUserId],
			i.[ModificationUserId],
			i.[LogicalDelete]
		FROM [inserted] i

		UNION ALL

		SELECT
			@ThisMoment AS [MemberHistoryTs],
			d.[MemberId],

			NULL AS [OrganizationId],
			NULL AS [ParentMemberId],
			NULL AS [MemberName],
			NULL AS [MemberTitle],
			NULL AS [SecurityRoleId],
			NULL AS [SortOrder],
			NULL AS [CreationTimestamp],
			NULL AS [ModificationTimestamp],
			NULL AS [CreationUserId],
			NULL AS [ModificationUserId],
			NULL AS [LogicalDelete]
		FROM [deleted] d
		LEFT OUTER JOIN [inserted] i ON i.[MemberId] = d.[MemberId]
		WHERE i.[MemberId] IS NULL
	END
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


CREATE TRIGGER [global].[OnUserChange] ON [global].[User] AFTER INSERT, UPDATE, DELETE
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


CREATE TABLE [history].[SecurityRoleHistory]
(
	[SecurityRoleHistoryId] [bigint] IDENTITY(1,1) NOT NULL,
	[SecurityRoleHistoryTs] [datetime2] NOT NULL,
	[SecurityRoleId] [int] NOT NULL,

	[SecurityRoleName] [nvarchar](255) NULL,
	[SortOrder] [tinyint] NULL,
	[CreationTimestamp] [datetime] NULL,
	[ModificationTimestamp] [datetime] NULL,
	[CreationUserId] [int] NULL,
	[ModificationUserId] [int] NULL,
	[LogicalDelete] [bit] NULL,

	CONSTRAINT [pk_SecurityRoleHistory] PRIMARY KEY
	(
		[SecurityRoleHistoryId]
	)
)
GO


CREATE NONCLUSTERED INDEX [ix_SecurityRoleHistory] ON [history].[SecurityRoleHistory]
(
	[SecurityRoleHistoryTs] ASC,
	[SecurityRoleId] ASC
) ON [PRIMARY]
GO


CREATE TRIGGER [global].[OnSecurityRoleChange] ON [global].[SecurityRole] AFTER INSERT, UPDATE, DELETE
AS
	BEGIN
		SET NOCOUNT ON;
		
		DECLARE @ThisMoment [datetime2]
		
		SET @ThisMoment = SYSUTCDATETIME()

		INSERT INTO [history].[SecurityRoleHistory]
		SELECT
			@ThisMoment AS [SecurityRoleHistoryTs],
			i.[SecurityRoleId],

			i.[SecurityRoleName],
			i.[SortOrder],
			i.[CreationTimestamp],
			i.[ModificationTimestamp],
			i.[CreationUserId],
			i.[ModificationUserId],
			i.[LogicalDelete]
		FROM [inserted] i

		UNION ALL

		SELECT
			@ThisMoment AS [SecurityRoleHistoryTs],
			d.[SecurityRoleId],

			NULL AS [SecurityRoleName],
			NULL AS [SortOrder],
			NULL AS [CreationTimestamp],
			NULL AS [ModificationTimestamp],
			NULL AS [CreationUserId],
			NULL AS [ModificationUserId],
			NULL AS [LogicalDelete]
		FROM [deleted] d
		LEFT OUTER JOIN [inserted] i ON i.[SecurityRoleId] = d.[SecurityRoleId]
		WHERE i.[SecurityRoleId] IS NULL
	END
GO


CREATE TABLE [history].[PropertyBagHistory]
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


CREATE NONCLUSTERED INDEX [ix_PropertyBagHistory] ON [history].[PropertyBagHistory]
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

		INSERT INTO [history].[PropertyBagHistory]
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


CREATE TRIGGER [global].[OnEventLogChange] ON [global].[EventLog] AFTER INSERT, UPDATE, DELETE
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


CREATE TRIGGER [global].[OnEmailMessageChange] ON [global].[EmailMessage] AFTER INSERT, UPDATE, DELETE
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


CREATE TRIGGER [global].[OnEmailAttachmentChange] ON [global].[EmailAttachment] AFTER INSERT, UPDATE, DELETE
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