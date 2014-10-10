/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/


SET NOCOUNT ON
GO


INSERT INTO [global].[User]
	(
		[EmailAddress], [UserName], [SaltValue], [PasswordHash], [Question], [AnswerHash],
		[LastLoginSuccessTimestamp], [LastLoginFailureTimestamp], [FailedLoginCount], [MustChangePassword],
		[SortOrder], [CreationTimestamp], [ModificationTimestamp], [CreationUserId], [ModificationUserId], [LogicalDelete]
	)
	VALUES
	(
		'test@textmetal.com', 'test', 'XXX', 'F4E55F3DCED575D62AD785F569EC9B4D5422002FAF3142EB1D4C91BE1B389F699B4C49AE605E61470FD9BBAFEE5D2131BB94984AE924087082FAE61882D3F06D', 'XXX', 'XXX',
		null, null, 0, 0,
		0, GETUTCDATE(), GETUTCDATE(), 0, 0, 0
	);


DECLARE @UserId [int];
SET @UserId = SCOPE_IDENTITY();


INSERT INTO [application].[Organization]
	(
		[OrganizationName],
		[SortOrder],
		[CreationTimestamp],
		[ModificationTimestamp],
		[CreationUserId],
		[ModificationUserId],
		[LogicalDelete]
	)
	VALUES
	(
		'Test Organization',
		0,
		GETUTCDATE(),
		GETUTCDATE(),
		@UserId,
		@UserId,
		0
	);


DECLARE @OrganizationId [int];
SET @OrganizationId = SCOPE_IDENTITY();


INSERT INTO [application].[Member]
	(
		[MemberId],
		[OrganizationId],
		[MemberName],
		[MemberTitle],
		[SecurityRoleId],
		[SortOrder],
		[CreationTimestamp],
		[ModificationTimestamp],
		[CreationUserId],
		[ModificationUserId],
		[LogicalDelete]
	)
	VALUES
	(
		@UserId,
		@OrganizationId,
		'Test Member Name',
		'Test Member Title',
		1,
		0,
		GETUTCDATE(),
		GETUTCDATE(),
		@UserId,
		@UserId,
		0
	);

DECLARE @MemberId [int];
SET @MemberId = SCOPE_IDENTITY();


GO
