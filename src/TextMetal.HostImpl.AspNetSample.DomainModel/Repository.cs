/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using TextMetal.Common.Cerealization;
using TextMetal.Common.Core;
using TextMetal.Common.Data;
using TextMetal.HostImpl.AspNetSample.Common;
using TextMetal.HostImpl.AspNetSample.DomainModel.Tables;
using TextMetal.HostImpl.AspNetSample.DomainModel.Views;
using TextMetal.HostImpl.Web.Email;

namespace TextMetal.HostImpl.AspNetSample.DomainModel
{
	public partial class Repository
	{
		#region Methods/Operators

		public IEnumerable<IListItem<int?>> GetSecurityRoles()
		{
			List<IListItem<int?>> securityRoles;

			securityRoles = new List<IListItem<int?>>();
			securityRoles.Add(new ListItem<int?>((int?)SecurityRoleEnum.None, SecurityRoleEnum.None.ToString()));
			securityRoles.Add(new ListItem<int?>((int?)SecurityRoleEnum.OrganizationVisitor, SecurityRoleEnum.OrganizationVisitor.ToString()));
			securityRoles.Add(new ListItem<int?>((int?)SecurityRoleEnum.OrganizationContributor, SecurityRoleEnum.OrganizationContributor.ToString()));
			securityRoles.Add(new ListItem<int?>((int?)SecurityRoleEnum.OrganizationDesigner, SecurityRoleEnum.OrganizationDesigner.ToString()));
			securityRoles.Add(new ListItem<int?>((int?)SecurityRoleEnum.OrganizationOwner, SecurityRoleEnum.OrganizationOwner.ToString()));

			return securityRoles;
		}

		partial void OnDiscardConflictEmailAttachment(IUnitOfWork unitOfWork, IEmailAttachment @emailAttachment)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailAttachment == null)
				throw new ArgumentNullException("@emailAttachment");

			// do nothing
		}

		partial void OnDiscardConflictEmailAttachmentHistory(IUnitOfWork unitOfWork, IEmailAttachmentHistory @emailAttachmentHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailAttachmentHistory == null)
				throw new ArgumentNullException("@emailAttachmentHistory");

			// do nothing
		}

		partial void OnDiscardConflictEmailMessage(IUnitOfWork unitOfWork, IEmailMessage @emailMessage)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailMessage == null)
				throw new ArgumentNullException("@emailMessage");

			// do nothing
		}

		partial void OnDiscardConflictEmailMessageHistory(IUnitOfWork unitOfWork, IEmailMessageHistory @emailMessageHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailMessageHistory == null)
				throw new ArgumentNullException("@emailMessageHistory");

			// do nothing
		}

		partial void OnDiscardConflictEventLog(IUnitOfWork unitOfWork, IEventLog @eventLog)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@eventLog == null)
				throw new ArgumentNullException("@eventLog");

			// do nothing
		}

		partial void OnDiscardConflictEventLogExtent(IUnitOfWork unitOfWork, IEventLogExtent @eventLogExtent)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@eventLogExtent == null)
				throw new ArgumentNullException("@eventLogExtent");

			// do nothing
		}

		partial void OnDiscardConflictEventLogHistory(IUnitOfWork unitOfWork, IEventLogHistory @eventLogHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@eventLogHistory == null)
				throw new ArgumentNullException("@eventLogHistory");

			// do nothing
		}

		partial void OnDiscardConflictMember(IUnitOfWork unitOfWork, IMember @member)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@member == null)
				throw new ArgumentNullException("@member");

			// do nothing
		}

		partial void OnDiscardConflictMemberHistory(IUnitOfWork unitOfWork, IMemberHistory @memberHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@memberHistory == null)
				throw new ArgumentNullException("@memberHistory");

			// do nothing
		}

		partial void OnDiscardConflictOrganization(IUnitOfWork unitOfWork, IOrganization @organization)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@organization == null)
				throw new ArgumentNullException("@organization");

			// do nothing
		}

		partial void OnDiscardConflictOrganizationHistory(IUnitOfWork unitOfWork, IOrganizationHistory @organizationHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@organizationHistory == null)
				throw new ArgumentNullException("@organizationHistory");

			// do nothing
		}

		partial void OnDiscardConflictPropertyBag(IUnitOfWork unitOfWork, IPropertyBag @propertyBag)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@propertyBag == null)
				throw new ArgumentNullException("@propertyBag");

			// do nothing
		}

		partial void OnDiscardConflictPropertyBagHistory(IUnitOfWork unitOfWork, IPropertyBagHistory @propertyBagHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@propertyBagHistory == null)
				throw new ArgumentNullException("@propertyBagHistory");

			// do nothing
		}

		partial void OnDiscardConflictSecurityRole(IUnitOfWork unitOfWork, ISecurityRole @securityRole)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@securityRole == null)
				throw new ArgumentNullException("@securityRole");

			// do nothing
		}

		partial void OnDiscardConflictSecurityRoleHistory(IUnitOfWork unitOfWork, ISecurityRoleHistory @securityRoleHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@securityRoleHistory == null)
				throw new ArgumentNullException("@securityRoleHistory");

			// do nothing
		}

		partial void OnDiscardConflictTabNoPrimaryKeyNoIdentity(IUnitOfWork unitOfWork, ITabNoPrimaryKeyNoIdentity @tabNoPrimaryKeyNoIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabNoPrimaryKeyNoIdentity == null)
				throw new ArgumentNullException("@tabNoPrimaryKeyNoIdentity");

			// do nothing
		}

		partial void OnDiscardConflictTabWithCompositePrimaryKeyNoIdentity(IUnitOfWork unitOfWork, ITabWithCompositePrimaryKeyNoIdentity @tabWithCompositePrimaryKeyNoIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithCompositePrimaryKeyNoIdentity == null)
				throw new ArgumentNullException("@tabWithCompositePrimaryKeyNoIdentity");

			// do nothing
		}

		partial void OnDiscardConflictTabWithNoPrimaryKeyWithIdentity(IUnitOfWork unitOfWork, ITabWithNoPrimaryKeyWithIdentity @tabWithNoPrimaryKeyWithIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithNoPrimaryKeyWithIdentity == null)
				throw new ArgumentNullException("@tabWithNoPrimaryKeyWithIdentity");

			// do nothing
		}

		partial void OnDiscardConflictTabWithPrimaryKeyAsDefault(IUnitOfWork unitOfWork, ITabWithPrimaryKeyAsDefault @tabWithPrimaryKeyAsDefault)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithPrimaryKeyAsDefault == null)
				throw new ArgumentNullException("@tabWithPrimaryKeyAsDefault");

			// do nothing
		}

		partial void OnDiscardConflictTabWithPrimaryKeyAsIdentity(IUnitOfWork unitOfWork, ITabWithPrimaryKeyAsIdentity @tabWithPrimaryKeyAsIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithPrimaryKeyAsIdentity == null)
				throw new ArgumentNullException("@tabWithPrimaryKeyAsIdentity");

			// do nothing
		}

		partial void OnDiscardConflictTabWithPrimaryKeyNoIdentity(IUnitOfWork unitOfWork, ITabWithPrimaryKeyNoIdentity @tabWithPrimaryKeyNoIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithPrimaryKeyNoIdentity == null)
				throw new ArgumentNullException("@tabWithPrimaryKeyNoIdentity");

			// do nothing
		}

		partial void OnDiscardConflictTabWithPrimaryKeyWithDifferentIdentity(IUnitOfWork unitOfWork, ITabWithPrimaryKeyWithDifferentIdentity @tabWithPrimaryKeyWithDifferentIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithPrimaryKeyWithDifferentIdentity == null)
				throw new ArgumentNullException("@tabWithPrimaryKeyWithDifferentIdentity");

			// do nothing
		}

		partial void OnDiscardConflictUser(IUnitOfWork unitOfWork, IUser @user)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@user == null)
				throw new ArgumentNullException("@user");

			// do nothing
		}

		partial void OnDiscardConflictUserHistory(IUnitOfWork unitOfWork, IUserHistory @userHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@userHistory == null)
				throw new ArgumentNullException("@userHistory");

			// do nothing
		}

		partial void OnPostDeleteEmailAttachment(IUnitOfWork unitOfWork, IEmailAttachment @emailAttachment)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailAttachment == null)
				throw new ArgumentNullException("@emailAttachment");

			// do nothing
		}

		partial void OnPostDeleteEmailAttachmentHistory(IUnitOfWork unitOfWork, IEmailAttachmentHistory @emailAttachmentHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailAttachmentHistory == null)
				throw new ArgumentNullException("@emailAttachmentHistory");

			// do nothing
		}

		partial void OnPostDeleteEmailMessage(IUnitOfWork unitOfWork, IEmailMessage @emailMessage)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailMessage == null)
				throw new ArgumentNullException("@emailMessage");

			// do nothing
		}

		partial void OnPostDeleteEmailMessageHistory(IUnitOfWork unitOfWork, IEmailMessageHistory @emailMessageHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailMessageHistory == null)
				throw new ArgumentNullException("@emailMessageHistory");

			// do nothing
		}

		partial void OnPostDeleteEventLog(IUnitOfWork unitOfWork, IEventLog @eventLog)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@eventLog == null)
				throw new ArgumentNullException("@eventLog");

			// do nothing
		}

		partial void OnPostDeleteEventLogExtent(IUnitOfWork unitOfWork, IEventLogExtent @eventLogExtent)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@eventLogExtent == null)
				throw new ArgumentNullException("@eventLogExtent");

			// do nothing
		}

		partial void OnPostDeleteEventLogHistory(IUnitOfWork unitOfWork, IEventLogHistory @eventLogHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@eventLogHistory == null)
				throw new ArgumentNullException("@eventLogHistory");

			// do nothing
		}

		partial void OnPostDeleteMember(IUnitOfWork unitOfWork, IMember @member)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@member == null)
				throw new ArgumentNullException("@member");

			// do nothing
		}

		partial void OnPostDeleteMemberHistory(IUnitOfWork unitOfWork, IMemberHistory @memberHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@memberHistory == null)
				throw new ArgumentNullException("@memberHistory");

			// do nothing
		}

		partial void OnPostDeleteOrganization(IUnitOfWork unitOfWork, IOrganization @organization)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@organization == null)
				throw new ArgumentNullException("@organization");

			// do nothing
		}

		partial void OnPostDeleteOrganizationHistory(IUnitOfWork unitOfWork, IOrganizationHistory @organizationHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@organizationHistory == null)
				throw new ArgumentNullException("@organizationHistory");

			// do nothing
		}

		partial void OnPostDeletePropertyBag(IUnitOfWork unitOfWork, IPropertyBag @propertyBag)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@propertyBag == null)
				throw new ArgumentNullException("@propertyBag");

			// do nothing
		}

		partial void OnPostDeletePropertyBagHistory(IUnitOfWork unitOfWork, IPropertyBagHistory @propertyBagHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@propertyBagHistory == null)
				throw new ArgumentNullException("@propertyBagHistory");

			// do nothing
		}

		partial void OnPostDeleteSecurityRole(IUnitOfWork unitOfWork, ISecurityRole @securityRole)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@securityRole == null)
				throw new ArgumentNullException("@securityRole");

			// do nothing
		}

		partial void OnPostDeleteSecurityRoleHistory(IUnitOfWork unitOfWork, ISecurityRoleHistory @securityRoleHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@securityRoleHistory == null)
				throw new ArgumentNullException("@securityRoleHistory");

			// do nothing
		}

		partial void OnPostDeleteTabNoPrimaryKeyNoIdentity(IUnitOfWork unitOfWork, ITabNoPrimaryKeyNoIdentity @tabNoPrimaryKeyNoIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabNoPrimaryKeyNoIdentity == null)
				throw new ArgumentNullException("@tabNoPrimaryKeyNoIdentity");

			// do nothing
		}

		partial void OnPostDeleteTabWithCompositePrimaryKeyNoIdentity(IUnitOfWork unitOfWork, ITabWithCompositePrimaryKeyNoIdentity @tabWithCompositePrimaryKeyNoIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithCompositePrimaryKeyNoIdentity == null)
				throw new ArgumentNullException("@tabWithCompositePrimaryKeyNoIdentity");

			// do nothing
		}

		partial void OnPostDeleteTabWithNoPrimaryKeyWithIdentity(IUnitOfWork unitOfWork, ITabWithNoPrimaryKeyWithIdentity @tabWithNoPrimaryKeyWithIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithNoPrimaryKeyWithIdentity == null)
				throw new ArgumentNullException("@tabWithNoPrimaryKeyWithIdentity");

			// do nothing
		}

		partial void OnPostDeleteTabWithPrimaryKeyAsDefault(IUnitOfWork unitOfWork, ITabWithPrimaryKeyAsDefault @tabWithPrimaryKeyAsDefault)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithPrimaryKeyAsDefault == null)
				throw new ArgumentNullException("@tabWithPrimaryKeyAsDefault");

			// do nothing
		}

		partial void OnPostDeleteTabWithPrimaryKeyAsIdentity(IUnitOfWork unitOfWork, ITabWithPrimaryKeyAsIdentity @tabWithPrimaryKeyAsIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithPrimaryKeyAsIdentity == null)
				throw new ArgumentNullException("@tabWithPrimaryKeyAsIdentity");

			// do nothing
		}

		partial void OnPostDeleteTabWithPrimaryKeyNoIdentity(IUnitOfWork unitOfWork, ITabWithPrimaryKeyNoIdentity @tabWithPrimaryKeyNoIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithPrimaryKeyNoIdentity == null)
				throw new ArgumentNullException("@tabWithPrimaryKeyNoIdentity");

			// do nothing
		}

		partial void OnPostDeleteTabWithPrimaryKeyWithDifferentIdentity(IUnitOfWork unitOfWork, ITabWithPrimaryKeyWithDifferentIdentity @tabWithPrimaryKeyWithDifferentIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithPrimaryKeyWithDifferentIdentity == null)
				throw new ArgumentNullException("@tabWithPrimaryKeyWithDifferentIdentity");

			// do nothing
		}

		partial void OnPostDeleteUser(IUnitOfWork unitOfWork, IUser @user)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@user == null)
				throw new ArgumentNullException("@user");

			// do nothing
		}

		partial void OnPostDeleteUserHistory(IUnitOfWork unitOfWork, IUserHistory @userHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@userHistory == null)
				throw new ArgumentNullException("@userHistory");

			// do nothing
		}

		partial void OnPostInsertEmailAttachment(IUnitOfWork unitOfWork, IEmailAttachment @emailAttachment)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailAttachment == null)
				throw new ArgumentNullException("@emailAttachment");

			// do nothing
		}

		partial void OnPostInsertEmailAttachmentHistory(IUnitOfWork unitOfWork, IEmailAttachmentHistory @emailAttachmentHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailAttachmentHistory == null)
				throw new ArgumentNullException("@emailAttachmentHistory");

			// do nothing
		}

		partial void OnPostInsertEmailMessage(IUnitOfWork unitOfWork, IEmailMessage @emailMessage)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailMessage == null)
				throw new ArgumentNullException("@emailMessage");

			// do nothing
		}

		partial void OnPostInsertEmailMessageHistory(IUnitOfWork unitOfWork, IEmailMessageHistory @emailMessageHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailMessageHistory == null)
				throw new ArgumentNullException("@emailMessageHistory");

			// do nothing
		}

		partial void OnPostInsertEventLog(IUnitOfWork unitOfWork, IEventLog @eventLog)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@eventLog == null)
				throw new ArgumentNullException("@eventLog");

			// do nothing
		}

		partial void OnPostInsertEventLogExtent(IUnitOfWork unitOfWork, IEventLogExtent @eventLogExtent)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@eventLogExtent == null)
				throw new ArgumentNullException("@eventLogExtent");

			// do nothing
		}

		partial void OnPostInsertEventLogHistory(IUnitOfWork unitOfWork, IEventLogHistory @eventLogHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@eventLogHistory == null)
				throw new ArgumentNullException("@eventLogHistory");

			// do nothing
		}

		partial void OnPostInsertMember(IUnitOfWork unitOfWork, IMember @member)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@member == null)
				throw new ArgumentNullException("@member");

			// do nothing
		}

		partial void OnPostInsertMemberHistory(IUnitOfWork unitOfWork, IMemberHistory @memberHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@memberHistory == null)
				throw new ArgumentNullException("@memberHistory");

			// do nothing
		}

		partial void OnPostInsertOrganization(IUnitOfWork unitOfWork, IOrganization @organization)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@organization == null)
				throw new ArgumentNullException("@organization");

			// do nothing
		}

		partial void OnPostInsertOrganizationHistory(IUnitOfWork unitOfWork, IOrganizationHistory @organizationHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@organizationHistory == null)
				throw new ArgumentNullException("@organizationHistory");

			// do nothing
		}

		partial void OnPostInsertPropertyBag(IUnitOfWork unitOfWork, IPropertyBag @propertyBag)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@propertyBag == null)
				throw new ArgumentNullException("@propertyBag");

			// do nothing
		}

		partial void OnPostInsertPropertyBagHistory(IUnitOfWork unitOfWork, IPropertyBagHistory @propertyBagHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@propertyBagHistory == null)
				throw new ArgumentNullException("@propertyBagHistory");

			// do nothing
		}

		partial void OnPostInsertSecurityRole(IUnitOfWork unitOfWork, ISecurityRole @securityRole)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@securityRole == null)
				throw new ArgumentNullException("@securityRole");

			// do nothing
		}

		partial void OnPostInsertSecurityRoleHistory(IUnitOfWork unitOfWork, ISecurityRoleHistory @securityRoleHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@securityRoleHistory == null)
				throw new ArgumentNullException("@securityRoleHistory");

			// do nothing
		}

		partial void OnPostInsertTabNoPrimaryKeyNoIdentity(IUnitOfWork unitOfWork, ITabNoPrimaryKeyNoIdentity @tabNoPrimaryKeyNoIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabNoPrimaryKeyNoIdentity == null)
				throw new ArgumentNullException("@tabNoPrimaryKeyNoIdentity");

			// do nothing
		}

		partial void OnPostInsertTabWithCompositePrimaryKeyNoIdentity(IUnitOfWork unitOfWork, ITabWithCompositePrimaryKeyNoIdentity @tabWithCompositePrimaryKeyNoIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithCompositePrimaryKeyNoIdentity == null)
				throw new ArgumentNullException("@tabWithCompositePrimaryKeyNoIdentity");

			// do nothing
		}

		partial void OnPostInsertTabWithNoPrimaryKeyWithIdentity(IUnitOfWork unitOfWork, ITabWithNoPrimaryKeyWithIdentity @tabWithNoPrimaryKeyWithIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithNoPrimaryKeyWithIdentity == null)
				throw new ArgumentNullException("@tabWithNoPrimaryKeyWithIdentity");

			// do nothing
		}

		partial void OnPostInsertTabWithPrimaryKeyAsDefault(IUnitOfWork unitOfWork, ITabWithPrimaryKeyAsDefault @tabWithPrimaryKeyAsDefault)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithPrimaryKeyAsDefault == null)
				throw new ArgumentNullException("@tabWithPrimaryKeyAsDefault");

			// do nothing
		}

		partial void OnPostInsertTabWithPrimaryKeyAsIdentity(IUnitOfWork unitOfWork, ITabWithPrimaryKeyAsIdentity @tabWithPrimaryKeyAsIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithPrimaryKeyAsIdentity == null)
				throw new ArgumentNullException("@tabWithPrimaryKeyAsIdentity");

			// do nothing
		}

		partial void OnPostInsertTabWithPrimaryKeyNoIdentity(IUnitOfWork unitOfWork, ITabWithPrimaryKeyNoIdentity @tabWithPrimaryKeyNoIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithPrimaryKeyNoIdentity == null)
				throw new ArgumentNullException("@tabWithPrimaryKeyNoIdentity");

			// do nothing
		}

		partial void OnPostInsertTabWithPrimaryKeyWithDifferentIdentity(IUnitOfWork unitOfWork, ITabWithPrimaryKeyWithDifferentIdentity @tabWithPrimaryKeyWithDifferentIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithPrimaryKeyWithDifferentIdentity == null)
				throw new ArgumentNullException("@tabWithPrimaryKeyWithDifferentIdentity");

			// do nothing
		}

		partial void OnPostInsertUser(IUnitOfWork unitOfWork, IUser @user)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@user == null)
				throw new ArgumentNullException("@user");

			// do nothing
		}

		partial void OnPostInsertUserHistory(IUnitOfWork unitOfWork, IUserHistory @userHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@userHistory == null)
				throw new ArgumentNullException("@userHistory");

			// do nothing
		}

		partial void OnPostUpdateEmailAttachment(IUnitOfWork unitOfWork, IEmailAttachment @emailAttachment)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailAttachment == null)
				throw new ArgumentNullException("@emailAttachment");

			// do nothing
		}

		partial void OnPostUpdateEmailAttachmentHistory(IUnitOfWork unitOfWork, IEmailAttachmentHistory @emailAttachmentHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailAttachmentHistory == null)
				throw new ArgumentNullException("@emailAttachmentHistory");

			// do nothing
		}

		partial void OnPostUpdateEmailMessage(IUnitOfWork unitOfWork, IEmailMessage @emailMessage)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailMessage == null)
				throw new ArgumentNullException("@emailMessage");

			// do nothing
		}

		partial void OnPostUpdateEmailMessageHistory(IUnitOfWork unitOfWork, IEmailMessageHistory @emailMessageHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailMessageHistory == null)
				throw new ArgumentNullException("@emailMessageHistory");

			// do nothing
		}

		partial void OnPostUpdateEventLog(IUnitOfWork unitOfWork, IEventLog @eventLog)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@eventLog == null)
				throw new ArgumentNullException("@eventLog");

			// do nothing
		}

		partial void OnPostUpdateEventLogExtent(IUnitOfWork unitOfWork, IEventLogExtent @eventLogExtent)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@eventLogExtent == null)
				throw new ArgumentNullException("@eventLogExtent");

			// do nothing
		}

		partial void OnPostUpdateEventLogHistory(IUnitOfWork unitOfWork, IEventLogHistory @eventLogHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@eventLogHistory == null)
				throw new ArgumentNullException("@eventLogHistory");

			// do nothing
		}

		partial void OnPostUpdateMember(IUnitOfWork unitOfWork, IMember @member)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@member == null)
				throw new ArgumentNullException("@member");

			// do nothing
		}

		partial void OnPostUpdateMemberHistory(IUnitOfWork unitOfWork, IMemberHistory @memberHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@memberHistory == null)
				throw new ArgumentNullException("@memberHistory");

			// do nothing
		}

		partial void OnPostUpdateOrganization(IUnitOfWork unitOfWork, IOrganization @organization)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@organization == null)
				throw new ArgumentNullException("@organization");

			// do nothing
		}

		partial void OnPostUpdateOrganizationHistory(IUnitOfWork unitOfWork, IOrganizationHistory @organizationHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@organizationHistory == null)
				throw new ArgumentNullException("@organizationHistory");

			// do nothing
		}

		partial void OnPostUpdatePropertyBag(IUnitOfWork unitOfWork, IPropertyBag @propertyBag)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@propertyBag == null)
				throw new ArgumentNullException("@propertyBag");

			// do nothing
		}

		partial void OnPostUpdatePropertyBagHistory(IUnitOfWork unitOfWork, IPropertyBagHistory @propertyBagHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@propertyBagHistory == null)
				throw new ArgumentNullException("@propertyBagHistory");

			// do nothing
		}

		partial void OnPostUpdateSecurityRole(IUnitOfWork unitOfWork, ISecurityRole @securityRole)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@securityRole == null)
				throw new ArgumentNullException("@securityRole");

			// do nothing
		}

		partial void OnPostUpdateSecurityRoleHistory(IUnitOfWork unitOfWork, ISecurityRoleHistory @securityRoleHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@securityRoleHistory == null)
				throw new ArgumentNullException("@securityRoleHistory");

			// do nothing
		}

		partial void OnPostUpdateTabNoPrimaryKeyNoIdentity(IUnitOfWork unitOfWork, ITabNoPrimaryKeyNoIdentity @tabNoPrimaryKeyNoIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabNoPrimaryKeyNoIdentity == null)
				throw new ArgumentNullException("@tabNoPrimaryKeyNoIdentity");

			// do nothing
		}

		partial void OnPostUpdateTabWithCompositePrimaryKeyNoIdentity(IUnitOfWork unitOfWork, ITabWithCompositePrimaryKeyNoIdentity @tabWithCompositePrimaryKeyNoIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithCompositePrimaryKeyNoIdentity == null)
				throw new ArgumentNullException("@tabWithCompositePrimaryKeyNoIdentity");

			// do nothing
		}

		partial void OnPostUpdateTabWithNoPrimaryKeyWithIdentity(IUnitOfWork unitOfWork, ITabWithNoPrimaryKeyWithIdentity @tabWithNoPrimaryKeyWithIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithNoPrimaryKeyWithIdentity == null)
				throw new ArgumentNullException("@tabWithNoPrimaryKeyWithIdentity");

			// do nothing
		}

		partial void OnPostUpdateTabWithPrimaryKeyAsDefault(IUnitOfWork unitOfWork, ITabWithPrimaryKeyAsDefault @tabWithPrimaryKeyAsDefault)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithPrimaryKeyAsDefault == null)
				throw new ArgumentNullException("@tabWithPrimaryKeyAsDefault");

			// do nothing
		}

		partial void OnPostUpdateTabWithPrimaryKeyAsIdentity(IUnitOfWork unitOfWork, ITabWithPrimaryKeyAsIdentity @tabWithPrimaryKeyAsIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithPrimaryKeyAsIdentity == null)
				throw new ArgumentNullException("@tabWithPrimaryKeyAsIdentity");

			// do nothing
		}

		partial void OnPostUpdateTabWithPrimaryKeyNoIdentity(IUnitOfWork unitOfWork, ITabWithPrimaryKeyNoIdentity @tabWithPrimaryKeyNoIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithPrimaryKeyNoIdentity == null)
				throw new ArgumentNullException("@tabWithPrimaryKeyNoIdentity");

			// do nothing
		}

		partial void OnPostUpdateTabWithPrimaryKeyWithDifferentIdentity(IUnitOfWork unitOfWork, ITabWithPrimaryKeyWithDifferentIdentity @tabWithPrimaryKeyWithDifferentIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithPrimaryKeyWithDifferentIdentity == null)
				throw new ArgumentNullException("@tabWithPrimaryKeyWithDifferentIdentity");

			// do nothing
		}

		partial void OnPostUpdateUser(IUnitOfWork unitOfWork, IUser @user)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@user == null)
				throw new ArgumentNullException("@user");

			// do nothing
		}

		partial void OnPostUpdateUserHistory(IUnitOfWork unitOfWork, IUserHistory @userHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@userHistory == null)
				throw new ArgumentNullException("@userHistory");

			// do nothing
		}

		partial void OnPreDeleteEmailAttachment(IUnitOfWork unitOfWork, IEmailAttachment @emailAttachment)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailAttachment == null)
				throw new ArgumentNullException("@emailAttachment");

			// do nothing
		}

		partial void OnPreDeleteEmailAttachmentHistory(IUnitOfWork unitOfWork, IEmailAttachmentHistory @emailAttachmentHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailAttachmentHistory == null)
				throw new ArgumentNullException("@emailAttachmentHistory");

			// do nothing
		}

		partial void OnPreDeleteEmailMessage(IUnitOfWork unitOfWork, IEmailMessage @emailMessage)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailMessage == null)
				throw new ArgumentNullException("@emailMessage");

			// do nothing
		}

		partial void OnPreDeleteEmailMessageHistory(IUnitOfWork unitOfWork, IEmailMessageHistory @emailMessageHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailMessageHistory == null)
				throw new ArgumentNullException("@emailMessageHistory");

			// do nothing
		}

		partial void OnPreDeleteEventLog(IUnitOfWork unitOfWork, IEventLog @eventLog)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@eventLog == null)
				throw new ArgumentNullException("@eventLog");

			// do nothing
		}

		partial void OnPreDeleteEventLogExtent(IUnitOfWork unitOfWork, IEventLogExtent @eventLogExtent)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@eventLogExtent == null)
				throw new ArgumentNullException("@eventLogExtent");

			// do nothing
		}

		partial void OnPreDeleteEventLogHistory(IUnitOfWork unitOfWork, IEventLogHistory @eventLogHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@eventLogHistory == null)
				throw new ArgumentNullException("@eventLogHistory");

			// do nothing
		}

		partial void OnPreDeleteMember(IUnitOfWork unitOfWork, IMember @member)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@member == null)
				throw new ArgumentNullException("@member");

			// do nothing
		}

		partial void OnPreDeleteMemberHistory(IUnitOfWork unitOfWork, IMemberHistory @memberHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@memberHistory == null)
				throw new ArgumentNullException("@memberHistory");

			// do nothing
		}

		partial void OnPreDeleteOrganization(IUnitOfWork unitOfWork, IOrganization @organization)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@organization == null)
				throw new ArgumentNullException("@organization");

			// do nothing
		}

		partial void OnPreDeleteOrganizationHistory(IUnitOfWork unitOfWork, IOrganizationHistory @organizationHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@organizationHistory == null)
				throw new ArgumentNullException("@organizationHistory");

			// do nothing
		}

		partial void OnPreDeletePropertyBag(IUnitOfWork unitOfWork, IPropertyBag @propertyBag)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@propertyBag == null)
				throw new ArgumentNullException("@propertyBag");

			// do nothing
		}

		partial void OnPreDeletePropertyBagHistory(IUnitOfWork unitOfWork, IPropertyBagHistory @propertyBagHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@propertyBagHistory == null)
				throw new ArgumentNullException("@propertyBagHistory");

			// do nothing
		}

		partial void OnPreDeleteSecurityRole(IUnitOfWork unitOfWork, ISecurityRole @securityRole)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@securityRole == null)
				throw new ArgumentNullException("@securityRole");

			// do nothing
		}

		partial void OnPreDeleteSecurityRoleHistory(IUnitOfWork unitOfWork, ISecurityRoleHistory @securityRoleHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@securityRoleHistory == null)
				throw new ArgumentNullException("@securityRoleHistory");

			// do nothing
		}

		partial void OnPreDeleteTabNoPrimaryKeyNoIdentity(IUnitOfWork unitOfWork, ITabNoPrimaryKeyNoIdentity @tabNoPrimaryKeyNoIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabNoPrimaryKeyNoIdentity == null)
				throw new ArgumentNullException("@tabNoPrimaryKeyNoIdentity");

			// do nothing
		}

		partial void OnPreDeleteTabWithCompositePrimaryKeyNoIdentity(IUnitOfWork unitOfWork, ITabWithCompositePrimaryKeyNoIdentity @tabWithCompositePrimaryKeyNoIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithCompositePrimaryKeyNoIdentity == null)
				throw new ArgumentNullException("@tabWithCompositePrimaryKeyNoIdentity");

			// do nothing
		}

		partial void OnPreDeleteTabWithNoPrimaryKeyWithIdentity(IUnitOfWork unitOfWork, ITabWithNoPrimaryKeyWithIdentity @tabWithNoPrimaryKeyWithIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithNoPrimaryKeyWithIdentity == null)
				throw new ArgumentNullException("@tabWithNoPrimaryKeyWithIdentity");

			// do nothing
		}

		partial void OnPreDeleteTabWithPrimaryKeyAsDefault(IUnitOfWork unitOfWork, ITabWithPrimaryKeyAsDefault @tabWithPrimaryKeyAsDefault)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithPrimaryKeyAsDefault == null)
				throw new ArgumentNullException("@tabWithPrimaryKeyAsDefault");

			// do nothing
		}

		partial void OnPreDeleteTabWithPrimaryKeyAsIdentity(IUnitOfWork unitOfWork, ITabWithPrimaryKeyAsIdentity @tabWithPrimaryKeyAsIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithPrimaryKeyAsIdentity == null)
				throw new ArgumentNullException("@tabWithPrimaryKeyAsIdentity");

			// do nothing
		}

		partial void OnPreDeleteTabWithPrimaryKeyNoIdentity(IUnitOfWork unitOfWork, ITabWithPrimaryKeyNoIdentity @tabWithPrimaryKeyNoIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithPrimaryKeyNoIdentity == null)
				throw new ArgumentNullException("@tabWithPrimaryKeyNoIdentity");

			// do nothing
		}

		partial void OnPreDeleteTabWithPrimaryKeyWithDifferentIdentity(IUnitOfWork unitOfWork, ITabWithPrimaryKeyWithDifferentIdentity @tabWithPrimaryKeyWithDifferentIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithPrimaryKeyWithDifferentIdentity == null)
				throw new ArgumentNullException("@tabWithPrimaryKeyWithDifferentIdentity");

			// do nothing
		}

		partial void OnPreDeleteUser(IUnitOfWork unitOfWork, IUser @user)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@user == null)
				throw new ArgumentNullException("@user");

			// do nothing
		}

		partial void OnPreDeleteUserHistory(IUnitOfWork unitOfWork, IUserHistory @userHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@userHistory == null)
				throw new ArgumentNullException("@userHistory");

			// do nothing
		}

		partial void OnPreInsertEmailAttachment(IUnitOfWork unitOfWork, IEmailAttachment @emailAttachment)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailAttachment == null)
				throw new ArgumentNullException("@emailAttachment");

			@emailAttachment.Mark();
		}

		partial void OnPreInsertEmailAttachmentHistory(IUnitOfWork unitOfWork, IEmailAttachmentHistory @emailAttachmentHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailAttachmentHistory == null)
				throw new ArgumentNullException("@emailAttachmentHistory");

			@emailAttachmentHistory.Mark();
		}

		partial void OnPreInsertEmailMessage(IUnitOfWork unitOfWork, IEmailMessage @emailMessage)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailMessage == null)
				throw new ArgumentNullException("@emailMessage");

			@emailMessage.Mark();
		}

		partial void OnPreInsertEmailMessageHistory(IUnitOfWork unitOfWork, IEmailMessageHistory @emailMessageHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailMessageHistory == null)
				throw new ArgumentNullException("@emailMessageHistory");

			@emailMessageHistory.Mark();
		}

		partial void OnPreInsertEventLog(IUnitOfWork unitOfWork, IEventLog @eventLog)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@eventLog == null)
				throw new ArgumentNullException("@eventLog");

			@eventLog.Mark();
		}

		partial void OnPreInsertEventLogExtent(IUnitOfWork unitOfWork, IEventLogExtent @eventLogExtent)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@eventLogExtent == null)
				throw new ArgumentNullException("@eventLogExtent");

			@eventLogExtent.Mark();
		}

		partial void OnPreInsertEventLogHistory(IUnitOfWork unitOfWork, IEventLogHistory @eventLogHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@eventLogHistory == null)
				throw new ArgumentNullException("@eventLogHistory");

			@eventLogHistory.Mark();
		}

		partial void OnPreInsertMember(IUnitOfWork unitOfWork, IMember @member)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@member == null)
				throw new ArgumentNullException("@member");

			@member.Mark();
		}

		partial void OnPreInsertMemberHistory(IUnitOfWork unitOfWork, IMemberHistory @memberHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@memberHistory == null)
				throw new ArgumentNullException("@memberHistory");

			@memberHistory.Mark();
		}

		partial void OnPreInsertOrganization(IUnitOfWork unitOfWork, IOrganization @organization)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@organization == null)
				throw new ArgumentNullException("@organization");

			@organization.Mark();
		}

		partial void OnPreInsertOrganizationHistory(IUnitOfWork unitOfWork, IOrganizationHistory @organizationHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@organizationHistory == null)
				throw new ArgumentNullException("@organizationHistory");

			@organizationHistory.Mark();
		}

		partial void OnPreInsertPropertyBag(IUnitOfWork unitOfWork, IPropertyBag @propertyBag)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@propertyBag == null)
				throw new ArgumentNullException("@propertyBag");

			@propertyBag.Mark();
		}

		partial void OnPreInsertPropertyBagHistory(IUnitOfWork unitOfWork, IPropertyBagHistory @propertyBagHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@propertyBagHistory == null)
				throw new ArgumentNullException("@propertyBagHistory");

			@propertyBagHistory.Mark();
		}

		partial void OnPreInsertSecurityRole(IUnitOfWork unitOfWork, ISecurityRole @securityRole)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@securityRole == null)
				throw new ArgumentNullException("@securityRole");

			@securityRole.Mark();
		}

		partial void OnPreInsertSecurityRoleHistory(IUnitOfWork unitOfWork, ISecurityRoleHistory @securityRoleHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@securityRoleHistory == null)
				throw new ArgumentNullException("@securityRoleHistory");

			@securityRoleHistory.Mark();
		}

		partial void OnPreInsertTabNoPrimaryKeyNoIdentity(IUnitOfWork unitOfWork, ITabNoPrimaryKeyNoIdentity @tabNoPrimaryKeyNoIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabNoPrimaryKeyNoIdentity == null)
				throw new ArgumentNullException("@tabNoPrimaryKeyNoIdentity");

			@tabNoPrimaryKeyNoIdentity.Mark();
		}

		partial void OnPreInsertTabWithCompositePrimaryKeyNoIdentity(IUnitOfWork unitOfWork, ITabWithCompositePrimaryKeyNoIdentity @tabWithCompositePrimaryKeyNoIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithCompositePrimaryKeyNoIdentity == null)
				throw new ArgumentNullException("@tabWithCompositePrimaryKeyNoIdentity");

			@tabWithCompositePrimaryKeyNoIdentity.Mark();
		}

		partial void OnPreInsertTabWithNoPrimaryKeyWithIdentity(IUnitOfWork unitOfWork, ITabWithNoPrimaryKeyWithIdentity @tabWithNoPrimaryKeyWithIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithNoPrimaryKeyWithIdentity == null)
				throw new ArgumentNullException("@tabWithNoPrimaryKeyWithIdentity");

			@tabWithNoPrimaryKeyWithIdentity.Mark();
		}

		partial void OnPreInsertTabWithPrimaryKeyAsDefault(IUnitOfWork unitOfWork, ITabWithPrimaryKeyAsDefault @tabWithPrimaryKeyAsDefault)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithPrimaryKeyAsDefault == null)
				throw new ArgumentNullException("@tabWithPrimaryKeyAsDefault");

			@tabWithPrimaryKeyAsDefault.Mark();
		}

		partial void OnPreInsertTabWithPrimaryKeyAsIdentity(IUnitOfWork unitOfWork, ITabWithPrimaryKeyAsIdentity @tabWithPrimaryKeyAsIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithPrimaryKeyAsIdentity == null)
				throw new ArgumentNullException("@tabWithPrimaryKeyAsIdentity");

			@tabWithPrimaryKeyAsIdentity.Mark();
		}

		partial void OnPreInsertTabWithPrimaryKeyNoIdentity(IUnitOfWork unitOfWork, ITabWithPrimaryKeyNoIdentity @tabWithPrimaryKeyNoIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithPrimaryKeyNoIdentity == null)
				throw new ArgumentNullException("@tabWithPrimaryKeyNoIdentity");

			@tabWithPrimaryKeyNoIdentity.Mark();
		}

		partial void OnPreInsertTabWithPrimaryKeyWithDifferentIdentity(IUnitOfWork unitOfWork, ITabWithPrimaryKeyWithDifferentIdentity @tabWithPrimaryKeyWithDifferentIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithPrimaryKeyWithDifferentIdentity == null)
				throw new ArgumentNullException("@tabWithPrimaryKeyWithDifferentIdentity");

			@tabWithPrimaryKeyWithDifferentIdentity.Mark();
		}

		partial void OnPreInsertUser(IUnitOfWork unitOfWork, IUser @user)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@user == null)
				throw new ArgumentNullException("@user");

			@user.Mark();
		}

		partial void OnPreInsertUserHistory(IUnitOfWork unitOfWork, IUserHistory @userHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@userHistory == null)
				throw new ArgumentNullException("@userHistory");

			@userHistory.Mark();
		}

		partial void OnPreUpdateEmailAttachment(IUnitOfWork unitOfWork, IEmailAttachment @emailAttachment)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailAttachment == null)
				throw new ArgumentNullException("@emailAttachment");

			@emailAttachment.Mark();
		}

		partial void OnPreUpdateEmailAttachmentHistory(IUnitOfWork unitOfWork, IEmailAttachmentHistory @emailAttachmentHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailAttachmentHistory == null)
				throw new ArgumentNullException("@emailAttachmentHistory");

			@emailAttachmentHistory.Mark();
		}

		partial void OnPreUpdateEmailMessage(IUnitOfWork unitOfWork, IEmailMessage @emailMessage)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailMessage == null)
				throw new ArgumentNullException("@emailMessage");

			@emailMessage.Mark();
		}

		partial void OnPreUpdateEmailMessageHistory(IUnitOfWork unitOfWork, IEmailMessageHistory @emailMessageHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailMessageHistory == null)
				throw new ArgumentNullException("@emailMessageHistory");

			@emailMessageHistory.Mark();
		}

		partial void OnPreUpdateEventLog(IUnitOfWork unitOfWork, IEventLog @eventLog)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@eventLog == null)
				throw new ArgumentNullException("@eventLog");

			@eventLog.Mark();
		}

		partial void OnPreUpdateEventLogExtent(IUnitOfWork unitOfWork, IEventLogExtent @eventLogExtent)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@eventLogExtent == null)
				throw new ArgumentNullException("@eventLogExtent");

			@eventLogExtent.Mark();
		}

		partial void OnPreUpdateEventLogHistory(IUnitOfWork unitOfWork, IEventLogHistory @eventLogHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@eventLogHistory == null)
				throw new ArgumentNullException("@eventLogHistory");

			@eventLogHistory.Mark();
		}

		partial void OnPreUpdateMember(IUnitOfWork unitOfWork, IMember @member)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@member == null)
				throw new ArgumentNullException("@member");

			@member.Mark();
		}

		partial void OnPreUpdateMemberHistory(IUnitOfWork unitOfWork, IMemberHistory @memberHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@memberHistory == null)
				throw new ArgumentNullException("@memberHistory");

			@memberHistory.Mark();
		}

		partial void OnPreUpdateOrganization(IUnitOfWork unitOfWork, IOrganization @organization)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@organization == null)
				throw new ArgumentNullException("@organization");

			@organization.Mark();
		}

		partial void OnPreUpdateOrganizationHistory(IUnitOfWork unitOfWork, IOrganizationHistory @organizationHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@organizationHistory == null)
				throw new ArgumentNullException("@organizationHistory");

			@organizationHistory.Mark();
		}

		partial void OnPreUpdatePropertyBag(IUnitOfWork unitOfWork, IPropertyBag @propertyBag)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@propertyBag == null)
				throw new ArgumentNullException("@propertyBag");

			@propertyBag.Mark();
		}

		partial void OnPreUpdatePropertyBagHistory(IUnitOfWork unitOfWork, IPropertyBagHistory @propertyBagHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@propertyBagHistory == null)
				throw new ArgumentNullException("@propertyBagHistory");

			@propertyBagHistory.Mark();
		}

		partial void OnPreUpdateSecurityRole(IUnitOfWork unitOfWork, ISecurityRole @securityRole)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@securityRole == null)
				throw new ArgumentNullException("@securityRole");

			@securityRole.Mark();
		}

		partial void OnPreUpdateSecurityRoleHistory(IUnitOfWork unitOfWork, ISecurityRoleHistory @securityRoleHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@securityRoleHistory == null)
				throw new ArgumentNullException("@securityRoleHistory");

			@securityRoleHistory.Mark();
		}

		partial void OnPreUpdateTabNoPrimaryKeyNoIdentity(IUnitOfWork unitOfWork, ITabNoPrimaryKeyNoIdentity @tabNoPrimaryKeyNoIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabNoPrimaryKeyNoIdentity == null)
				throw new ArgumentNullException("@tabNoPrimaryKeyNoIdentity");

			@tabNoPrimaryKeyNoIdentity.Mark();
		}

		partial void OnPreUpdateTabWithCompositePrimaryKeyNoIdentity(IUnitOfWork unitOfWork, ITabWithCompositePrimaryKeyNoIdentity @tabWithCompositePrimaryKeyNoIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithCompositePrimaryKeyNoIdentity == null)
				throw new ArgumentNullException("@tabWithCompositePrimaryKeyNoIdentity");

			@tabWithCompositePrimaryKeyNoIdentity.Mark();
		}

		partial void OnPreUpdateTabWithNoPrimaryKeyWithIdentity(IUnitOfWork unitOfWork, ITabWithNoPrimaryKeyWithIdentity @tabWithNoPrimaryKeyWithIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithNoPrimaryKeyWithIdentity == null)
				throw new ArgumentNullException("@tabWithNoPrimaryKeyWithIdentity");

			@tabWithNoPrimaryKeyWithIdentity.Mark();
		}

		partial void OnPreUpdateTabWithPrimaryKeyAsDefault(IUnitOfWork unitOfWork, ITabWithPrimaryKeyAsDefault @tabWithPrimaryKeyAsDefault)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithPrimaryKeyAsDefault == null)
				throw new ArgumentNullException("@tabWithPrimaryKeyAsDefault");

			@tabWithPrimaryKeyAsDefault.Mark();
		}

		partial void OnPreUpdateTabWithPrimaryKeyAsIdentity(IUnitOfWork unitOfWork, ITabWithPrimaryKeyAsIdentity @tabWithPrimaryKeyAsIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithPrimaryKeyAsIdentity == null)
				throw new ArgumentNullException("@tabWithPrimaryKeyAsIdentity");

			@tabWithPrimaryKeyAsIdentity.Mark();
		}

		partial void OnPreUpdateTabWithPrimaryKeyNoIdentity(IUnitOfWork unitOfWork, ITabWithPrimaryKeyNoIdentity @tabWithPrimaryKeyNoIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithPrimaryKeyNoIdentity == null)
				throw new ArgumentNullException("@tabWithPrimaryKeyNoIdentity");

			@tabWithPrimaryKeyNoIdentity.Mark();
		}

		partial void OnPreUpdateTabWithPrimaryKeyWithDifferentIdentity(IUnitOfWork unitOfWork, ITabWithPrimaryKeyWithDifferentIdentity @tabWithPrimaryKeyWithDifferentIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithPrimaryKeyWithDifferentIdentity == null)
				throw new ArgumentNullException("@tabWithPrimaryKeyWithDifferentIdentity");

			@tabWithPrimaryKeyWithDifferentIdentity.Mark();
		}

		partial void OnPreUpdateUser(IUnitOfWork unitOfWork, IUser @user)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@user == null)
				throw new ArgumentNullException("@user");

			@user.Mark();
		}

		partial void OnPreUpdateUserHistory(IUnitOfWork unitOfWork, IUserHistory @userHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@userHistory == null)
				throw new ArgumentNullException("@userHistory");

			@userHistory.Mark();
		}

		partial void OnSaveConflictEmailAttachment(IUnitOfWork unitOfWork, IEmailAttachment @emailAttachment)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailAttachment == null)
				throw new ArgumentNullException("@emailAttachment");

			// do nothing
		}

		partial void OnSaveConflictEmailAttachmentHistory(IUnitOfWork unitOfWork, IEmailAttachmentHistory @emailAttachmentHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailAttachmentHistory == null)
				throw new ArgumentNullException("@emailAttachmentHistory");

			// do nothing
		}

		partial void OnSaveConflictEmailMessage(IUnitOfWork unitOfWork, IEmailMessage @emailMessage)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailMessage == null)
				throw new ArgumentNullException("@emailMessage");

			// do nothing
		}

		partial void OnSaveConflictEmailMessageHistory(IUnitOfWork unitOfWork, IEmailMessageHistory @emailMessageHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailMessageHistory == null)
				throw new ArgumentNullException("@emailMessageHistory");

			// do nothing
		}

		partial void OnSaveConflictEventLog(IUnitOfWork unitOfWork, IEventLog @eventLog)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@eventLog == null)
				throw new ArgumentNullException("@eventLog");

			// do nothing
		}

		partial void OnSaveConflictEventLogExtent(IUnitOfWork unitOfWork, IEventLogExtent @eventLogExtent)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@eventLogExtent == null)
				throw new ArgumentNullException("@eventLogExtent");

			// do nothing
		}

		partial void OnSaveConflictEventLogHistory(IUnitOfWork unitOfWork, IEventLogHistory @eventLogHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@eventLogHistory == null)
				throw new ArgumentNullException("@eventLogHistory");

			// do nothing
		}

		partial void OnSaveConflictMember(IUnitOfWork unitOfWork, IMember @member)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@member == null)
				throw new ArgumentNullException("@member");

			// do nothing
		}

		partial void OnSaveConflictMemberHistory(IUnitOfWork unitOfWork, IMemberHistory @memberHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@memberHistory == null)
				throw new ArgumentNullException("@memberHistory");

			// do nothing
		}

		partial void OnSaveConflictOrganization(IUnitOfWork unitOfWork, IOrganization @organization)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@organization == null)
				throw new ArgumentNullException("@organization");

			// do nothing
		}

		partial void OnSaveConflictOrganizationHistory(IUnitOfWork unitOfWork, IOrganizationHistory @organizationHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@organizationHistory == null)
				throw new ArgumentNullException("@organizationHistory");

			// do nothing
		}

		partial void OnSaveConflictPropertyBag(IUnitOfWork unitOfWork, IPropertyBag @propertyBag)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@propertyBag == null)
				throw new ArgumentNullException("@propertyBag");

			// do nothing
		}

		partial void OnSaveConflictPropertyBagHistory(IUnitOfWork unitOfWork, IPropertyBagHistory @propertyBagHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@propertyBagHistory == null)
				throw new ArgumentNullException("@propertyBagHistory");

			// do nothing
		}

		partial void OnSaveConflictSecurityRole(IUnitOfWork unitOfWork, ISecurityRole @securityRole)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@securityRole == null)
				throw new ArgumentNullException("@securityRole");

			// do nothing
		}

		partial void OnSaveConflictSecurityRoleHistory(IUnitOfWork unitOfWork, ISecurityRoleHistory @securityRoleHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@securityRoleHistory == null)
				throw new ArgumentNullException("@securityRoleHistory");

			// do nothing
		}

		partial void OnSaveConflictTabNoPrimaryKeyNoIdentity(IUnitOfWork unitOfWork, ITabNoPrimaryKeyNoIdentity @tabNoPrimaryKeyNoIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabNoPrimaryKeyNoIdentity == null)
				throw new ArgumentNullException("@tabNoPrimaryKeyNoIdentity");

			// do nothing
		}

		partial void OnSaveConflictTabWithCompositePrimaryKeyNoIdentity(IUnitOfWork unitOfWork, ITabWithCompositePrimaryKeyNoIdentity @tabWithCompositePrimaryKeyNoIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithCompositePrimaryKeyNoIdentity == null)
				throw new ArgumentNullException("@tabWithCompositePrimaryKeyNoIdentity");

			// do nothing
		}

		partial void OnSaveConflictTabWithNoPrimaryKeyWithIdentity(IUnitOfWork unitOfWork, ITabWithNoPrimaryKeyWithIdentity @tabWithNoPrimaryKeyWithIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithNoPrimaryKeyWithIdentity == null)
				throw new ArgumentNullException("@tabWithNoPrimaryKeyWithIdentity");

			// do nothing
		}

		partial void OnSaveConflictTabWithPrimaryKeyAsDefault(IUnitOfWork unitOfWork, ITabWithPrimaryKeyAsDefault @tabWithPrimaryKeyAsDefault)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithPrimaryKeyAsDefault == null)
				throw new ArgumentNullException("@tabWithPrimaryKeyAsDefault");

			// do nothing
		}

		partial void OnSaveConflictTabWithPrimaryKeyAsIdentity(IUnitOfWork unitOfWork, ITabWithPrimaryKeyAsIdentity @tabWithPrimaryKeyAsIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithPrimaryKeyAsIdentity == null)
				throw new ArgumentNullException("@tabWithPrimaryKeyAsIdentity");

			// do nothing
		}

		partial void OnSaveConflictTabWithPrimaryKeyNoIdentity(IUnitOfWork unitOfWork, ITabWithPrimaryKeyNoIdentity @tabWithPrimaryKeyNoIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithPrimaryKeyNoIdentity == null)
				throw new ArgumentNullException("@tabWithPrimaryKeyNoIdentity");

			// do nothing
		}

		partial void OnSaveConflictTabWithPrimaryKeyWithDifferentIdentity(IUnitOfWork unitOfWork, ITabWithPrimaryKeyWithDifferentIdentity @tabWithPrimaryKeyWithDifferentIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithPrimaryKeyWithDifferentIdentity == null)
				throw new ArgumentNullException("@tabWithPrimaryKeyWithDifferentIdentity");

			// do nothing
		}

		partial void OnSaveConflictUser(IUnitOfWork unitOfWork, IUser @user)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@user == null)
				throw new ArgumentNullException("@user");

			// do nothing
		}

		partial void OnSaveConflictUserHistory(IUnitOfWork unitOfWork, IUserHistory @userHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@userHistory == null)
				throw new ArgumentNullException("@userHistory");

			// do nothing
		}

		partial void OnSelectEmailAttachment(IUnitOfWork unitOfWork, IEmailAttachment @emailAttachment)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailAttachment == null)
				throw new ArgumentNullException("@emailAttachment");

			// do nothing
		}

		partial void OnSelectEmailAttachmentHistory(IUnitOfWork unitOfWork, IEmailAttachmentHistory @emailAttachmentHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailAttachmentHistory == null)
				throw new ArgumentNullException("@emailAttachmentHistory");

			// do nothing
		}

		partial void OnSelectEmailMessage(IUnitOfWork unitOfWork, IEmailMessage @emailMessage)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailMessage == null)
				throw new ArgumentNullException("@emailMessage");

			// do nothing
		}

		partial void OnSelectEmailMessageHistory(IUnitOfWork unitOfWork, IEmailMessageHistory @emailMessageHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailMessageHistory == null)
				throw new ArgumentNullException("@emailMessageHistory");

			// do nothing
		}

		partial void OnSelectEventLog(IUnitOfWork unitOfWork, IEventLog @eventLog)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@eventLog == null)
				throw new ArgumentNullException("@eventLog");

			// do nothing
		}

		partial void OnSelectEventLogExtent(IUnitOfWork unitOfWork, IEventLogExtent @eventLogExtent)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@eventLogExtent == null)
				throw new ArgumentNullException("@eventLogExtent");

			// do nothing
		}

		partial void OnSelectEventLogHistory(IUnitOfWork unitOfWork, IEventLogHistory @eventLogHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@eventLogHistory == null)
				throw new ArgumentNullException("@eventLogHistory");

			// do nothing
		}

		partial void OnSelectMember(IUnitOfWork unitOfWork, IMember @member)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@member == null)
				throw new ArgumentNullException("@member");

			// do nothing
		}

		partial void OnSelectMemberHistory(IUnitOfWork unitOfWork, IMemberHistory @memberHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@memberHistory == null)
				throw new ArgumentNullException("@memberHistory");

			// do nothing
		}

		partial void OnSelectOrganization(IUnitOfWork unitOfWork, IOrganization @organization)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@organization == null)
				throw new ArgumentNullException("@organization");

			// do nothing
		}

		partial void OnSelectOrganizationHistory(IUnitOfWork unitOfWork, IOrganizationHistory @organizationHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@organizationHistory == null)
				throw new ArgumentNullException("@organizationHistory");

			// do nothing
		}

		partial void OnSelectPropertyBag(IUnitOfWork unitOfWork, IPropertyBag @propertyBag)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@propertyBag == null)
				throw new ArgumentNullException("@propertyBag");

			// do nothing
		}

		partial void OnSelectPropertyBagHistory(IUnitOfWork unitOfWork, IPropertyBagHistory @propertyBagHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@propertyBagHistory == null)
				throw new ArgumentNullException("@propertyBagHistory");

			// do nothing
		}

		partial void OnSelectSecurityRole(IUnitOfWork unitOfWork, ISecurityRole @securityRole)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@securityRole == null)
				throw new ArgumentNullException("@securityRole");

			// do nothing
		}

		partial void OnSelectSecurityRoleHistory(IUnitOfWork unitOfWork, ISecurityRoleHistory @securityRoleHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@securityRoleHistory == null)
				throw new ArgumentNullException("@securityRoleHistory");

			// do nothing
		}

		partial void OnSelectTabNoPrimaryKeyNoIdentity(IUnitOfWork unitOfWork, ITabNoPrimaryKeyNoIdentity @tabNoPrimaryKeyNoIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabNoPrimaryKeyNoIdentity == null)
				throw new ArgumentNullException("@tabNoPrimaryKeyNoIdentity");

			// do nothing
		}

		partial void OnSelectTabWithCompositePrimaryKeyNoIdentity(IUnitOfWork unitOfWork, ITabWithCompositePrimaryKeyNoIdentity @tabWithCompositePrimaryKeyNoIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithCompositePrimaryKeyNoIdentity == null)
				throw new ArgumentNullException("@tabWithCompositePrimaryKeyNoIdentity");

			// do nothing
		}

		partial void OnSelectTabWithNoPrimaryKeyWithIdentity(IUnitOfWork unitOfWork, ITabWithNoPrimaryKeyWithIdentity @tabWithNoPrimaryKeyWithIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithNoPrimaryKeyWithIdentity == null)
				throw new ArgumentNullException("@tabWithNoPrimaryKeyWithIdentity");

			// do nothing
		}

		partial void OnSelectTabWithPrimaryKeyAsDefault(IUnitOfWork unitOfWork, ITabWithPrimaryKeyAsDefault @tabWithPrimaryKeyAsDefault)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithPrimaryKeyAsDefault == null)
				throw new ArgumentNullException("@tabWithPrimaryKeyAsDefault");

			// do nothing
		}

		partial void OnSelectTabWithPrimaryKeyAsIdentity(IUnitOfWork unitOfWork, ITabWithPrimaryKeyAsIdentity @tabWithPrimaryKeyAsIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithPrimaryKeyAsIdentity == null)
				throw new ArgumentNullException("@tabWithPrimaryKeyAsIdentity");

			// do nothing
		}

		partial void OnSelectTabWithPrimaryKeyNoIdentity(IUnitOfWork unitOfWork, ITabWithPrimaryKeyNoIdentity @tabWithPrimaryKeyNoIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithPrimaryKeyNoIdentity == null)
				throw new ArgumentNullException("@tabWithPrimaryKeyNoIdentity");

			// do nothing
		}

		partial void OnSelectTabWithPrimaryKeyWithDifferentIdentity(IUnitOfWork unitOfWork, ITabWithPrimaryKeyWithDifferentIdentity @tabWithPrimaryKeyWithDifferentIdentity)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@tabWithPrimaryKeyWithDifferentIdentity == null)
				throw new ArgumentNullException("@tabWithPrimaryKeyWithDifferentIdentity");

			// do nothing
		}

		partial void OnSelectUser(IUnitOfWork unitOfWork, IUser @user)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@user == null)
				throw new ArgumentNullException("@user");

			// do nothing
		}

		partial void OnSelectUserHistory(IUnitOfWork unitOfWork, IUserHistory @userHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@userHistory == null)
				throw new ArgumentNullException("@userHistory");

			// do nothing
		}

		public bool TrySendEmailTemplate(string templateResourceName, object modelObject)
		{
			Type templateResourceType;
			MessageTemplate messageTemplate;
			EmailMessage emailMessage;
			EmailAttachment emailAttachment;

			if ((object)templateResourceName == null)
				throw new ArgumentNullException("templateResourceName");

			if ((object)modelObject == null)
				throw new ArgumentNullException("modelObject");

			try
			{
				templateResourceType = typeof(Repository);
				if (!Cerealization.TryGetFromAssemblyResource<MessageTemplate>(templateResourceType, templateResourceName, out messageTemplate))
					throw new InvalidOperationException(string.Format("Unable to deserialize instance of '{0}' from the manifest resource name '{1}' in the assembly '{2}'.", typeof(MessageTemplate).FullName, templateResourceName, templateResourceType.Assembly.FullName));

				emailMessage = new EmailMessage();
				/*emailAttachment = new EmailAttachment();
				emailAttachment.FileName = "data.txt";
				emailAttachment.MimeType = "text/plain";
				emailAttachment.AttachmentBits = Encoding.UTF8.GetBytes("some sample data");

				emailMessage.EmailAttachments.Add(emailAttachment);*/

				try
				{
					new EmailHost().Host(true, messageTemplate, modelObject, emailMessage);
				}
				finally
				{
					this.SaveEmailMessage(emailMessage);
				}

				return true;
			}
			catch (Exception ex)
			{
				// swallow intentionally
				this.TryWriteEventLogEntry(Reflexion.GetErrors(ex, 0));
				return false;
			}
		}

		public bool TryWriteEventLogEntry(string eventText)
		{
			EventLog eventLog;
			bool result;

			if ((object)eventText == null)
				throw new ArgumentNullException("eventText");

			try
			{
				eventLog = new EventLog();
				eventLog.EventText = eventText;

				result = this.SaveEventLog(eventLog);
				return result;
			}
			catch (Exception ex)
			{
				// swallow intentionally
				return false;
			}
		}

		#endregion
	}
}