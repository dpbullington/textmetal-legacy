/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Linq;

using TextMetal.HostImpl.AspNetSample.DomainModel.Tables;

using TextMetal.Common.Cerealization;
using TextMetal.Common.Core;
using TextMetal.Common.Data;
using TextMetal.Common.Data.LinqToSql;
using TextMetal.HostImpl.Web.Email;

namespace TextMetal.HostImpl.AspNetSample.DomainModel
{
	public partial class Repository
	{
		#region Methods/Operators

		public IEnumerable<TResultEntity> Find<TDataContext, TResultEntity>(TDataContext dummy, Func<TDataContext, IQueryable<TResultEntity>> callback)
			where TDataContext : class, IDisposable
		{
			IEnumerable<TResultEntity> things;

			if ((object)UnitOfWork.Current == null)
			{
				using (IUnitOfWork unitOfWork = DefaultUnitOfWorkFactory.Instance.GetUnitOfWork())
				{
					things = this.Find<TDataContext, TResultEntity>(dummy, unitOfWork, callback);

					things = things.ToList(); // FORCE EAGER LOAD

					unitOfWork.Complete();
				}
			}
			else
			{
				things = this.Find<TDataContext, TResultEntity>(dummy, UnitOfWork.Current, callback);

				// DO NOT FORCE EAGER LOAD
			}

			return things;
		}

		public IEnumerable<TResultEntity> Find<TDataContext, TResultEntity>(TDataContext dummy, IUnitOfWork unitOfWork, Func<TDataContext, IQueryable<TResultEntity>> callback)
			where TDataContext : class, IDisposable
		{
			IEnumerable<TResultEntity> things;
			IQueryable<TResultEntity> queryable;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)callback == null)
				throw new ArgumentNullException("callback");

			using (AmbientUnitOfWorkAwareDisposableWrapper<TDataContext> wrapper = unitOfWork.GetContext<TDataContext>())
			{
				queryable = callback(wrapper.Disposable);

				if ((object)queryable == null)
					throw new InvalidOperationException(string.Format("The queryable returned was invalid."));

				things = queryable; // DOES NOT FORCE EXECUTION AGAINST STORE

				foreach (TResultEntity thing in things)
				{
					//this.OnSelect(unitOfWork, thing);
					//thing.Mark();
					yield return thing; // LAZY PROCESSING INTENT HERE / DO NOT FORCE EAGER LOAD
				}
			}
		}

		partial void OnDiscardConflictChild(IUnitOfWork unitOfWork, Child @child)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@child == null)
				throw new ArgumentNullException("@child");

			// do nothing
		}

		partial void OnDiscardConflictChildHistory(IUnitOfWork unitOfWork, ChildHistory @childHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@childHistory == null)
				throw new ArgumentNullException("@childHistory");

			// do nothing
		}

		partial void OnDiscardConflictCircle(IUnitOfWork unitOfWork, Circle @circle)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@circle == null)
				throw new ArgumentNullException("@circle");

			// do nothing
		}

		partial void OnDiscardConflictCircleFamily(IUnitOfWork unitOfWork, CircleFamily @circleFamily)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@circleFamily == null)
				throw new ArgumentNullException("@circleFamily");

			// do nothing
		}

		partial void OnDiscardConflictCircleFamilyHistory(IUnitOfWork unitOfWork, CircleFamilyHistory @circleFamilyHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@circleFamilyHistory == null)
				throw new ArgumentNullException("@circleFamilyHistory");

			// do nothing
		}

		partial void OnDiscardConflictCircleHistory(IUnitOfWork unitOfWork, CircleHistory @circleHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@circleHistory == null)
				throw new ArgumentNullException("@circleHistory");

			// do nothing
		}

		partial void OnDiscardConflictEmailAttachment(IUnitOfWork unitOfWork, EmailAttachment @emailAttachment)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailAttachment == null)
				throw new ArgumentNullException("@emailAttachment");

			// do nothing
		}

		partial void OnDiscardConflictEmailAttachmentHistory(IUnitOfWork unitOfWork, EmailAttachmentHistory @emailAttachmentHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailAttachmentHistory == null)
				throw new ArgumentNullException("@emailAttachmentHistory");

			// do nothing
		}

		partial void OnDiscardConflictEmailMessage(IUnitOfWork unitOfWork, EmailMessage @emailMessage)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailMessage == null)
				throw new ArgumentNullException("@emailMessage");

			// do nothing
		}

		partial void OnDiscardConflictEmailMessageHistory(IUnitOfWork unitOfWork, EmailMessageHistory @emailMessageHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailMessageHistory == null)
				throw new ArgumentNullException("@emailMessageHistory");

			// do nothing
		}

		partial void OnDiscardConflictEventLog(IUnitOfWork unitOfWork, EventLog @eventLog)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@eventLog == null)
				throw new ArgumentNullException("@eventLog");

			// do nothing
		}

		partial void OnDiscardConflictEventLogHistory(IUnitOfWork unitOfWork, EventLogHistory @eventLogHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@eventLogHistory == null)
				throw new ArgumentNullException("@eventLogHistory");

			// do nothing
		}

		partial void OnDiscardConflictFamily(IUnitOfWork unitOfWork, Family @family)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@family == null)
				throw new ArgumentNullException("@family");

			// do nothing
		}

		partial void OnDiscardConflictFamilyHistory(IUnitOfWork unitOfWork, FamilyHistory @familyHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@familyHistory == null)
				throw new ArgumentNullException("@familyHistory");

			// do nothing
		}

		partial void OnDiscardConflictNotifyMethodLookup(IUnitOfWork unitOfWork, NotifyMethodLookup @notifyMethodLookup)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@notifyMethodLookup == null)
				throw new ArgumentNullException("@notifyMethodLookup");

			// do nothing
		}

		partial void OnDiscardConflictNotifyMethodLookupHistory(IUnitOfWork unitOfWork, NotifyMethodLookupHistory @notifyMethodLookupHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@notifyMethodLookupHistory == null)
				throw new ArgumentNullException("@notifyMethodLookupHistory");

			// do nothing
		}

		partial void OnDiscardConflictParent(IUnitOfWork unitOfWork, Parent @parent)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@parent == null)
				throw new ArgumentNullException("@parent");

			// do nothing
		}

		partial void OnDiscardConflictParentHistory(IUnitOfWork unitOfWork, ParentHistory @parentHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@parentHistory == null)
				throw new ArgumentNullException("@parentHistory");

			// do nothing
		}

		partial void OnDiscardConflictPropertyBag(IUnitOfWork unitOfWork, PropertyBag @propertyBag)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@propertyBag == null)
				throw new ArgumentNullException("@propertyBag");

			// do nothing
		}

		partial void OnDiscardConflictPropertyBagHistory(IUnitOfWork unitOfWork, PropertyBagHistory @propertyBagHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@propertyBagHistory == null)
				throw new ArgumentNullException("@propertyBagHistory");

			// do nothing
		}

		partial void OnDiscardConflictSexLookup(IUnitOfWork unitOfWork, SexLookup @sexLookup)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@sexLookup == null)
				throw new ArgumentNullException("@sexLookup");

			// do nothing
		}

		partial void OnDiscardConflictSexLookupHistory(IUnitOfWork unitOfWork, SexLookupHistory @sexLookupHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@sexLookupHistory == null)
				throw new ArgumentNullException("@sexLookupHistory");

			// do nothing
		}

		partial void OnDiscardConflictUser(IUnitOfWork unitOfWork, User @user)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@user == null)
				throw new ArgumentNullException("@user");

			// do nothing
		}

		partial void OnDiscardConflictUserHistory(IUnitOfWork unitOfWork, UserHistory @userHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@userHistory == null)
				throw new ArgumentNullException("@userHistory");

			// do nothing
		}

		partial void OnPostDeleteChild(IUnitOfWork unitOfWork, Child @child)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@child == null)
				throw new ArgumentNullException("@child");

			// do nothing
		}

		partial void OnPostDeleteChildHistory(IUnitOfWork unitOfWork, ChildHistory @childHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@childHistory == null)
				throw new ArgumentNullException("@childHistory");

			// do nothing
		}

		partial void OnPostDeleteCircle(IUnitOfWork unitOfWork, Circle @circle)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@circle == null)
				throw new ArgumentNullException("@circle");

			// do nothing
		}

		partial void OnPostDeleteCircleFamily(IUnitOfWork unitOfWork, CircleFamily @circleFamily)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@circleFamily == null)
				throw new ArgumentNullException("@circleFamily");

			// do nothing
		}

		partial void OnPostDeleteCircleFamilyHistory(IUnitOfWork unitOfWork, CircleFamilyHistory @circleFamilyHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@circleFamilyHistory == null)
				throw new ArgumentNullException("@circleFamilyHistory");

			// do nothing
		}

		partial void OnPostDeleteCircleHistory(IUnitOfWork unitOfWork, CircleHistory @circleHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@circleHistory == null)
				throw new ArgumentNullException("@circleHistory");

			// do nothing
		}

		partial void OnPostDeleteEmailAttachment(IUnitOfWork unitOfWork, EmailAttachment @emailAttachment)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailAttachment == null)
				throw new ArgumentNullException("@emailAttachment");

			// do nothing
		}

		partial void OnPostDeleteEmailAttachmentHistory(IUnitOfWork unitOfWork, EmailAttachmentHistory @emailAttachmentHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailAttachmentHistory == null)
				throw new ArgumentNullException("@emailAttachmentHistory");

			// do nothing
		}

		partial void OnPostDeleteEmailMessage(IUnitOfWork unitOfWork, EmailMessage @emailMessage)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailMessage == null)
				throw new ArgumentNullException("@emailMessage");

			// do nothing
		}

		partial void OnPostDeleteEmailMessageHistory(IUnitOfWork unitOfWork, EmailMessageHistory @emailMessageHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailMessageHistory == null)
				throw new ArgumentNullException("@emailMessageHistory");

			// do nothing
		}

		partial void OnPostDeleteEventLog(IUnitOfWork unitOfWork, EventLog @eventLog)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@eventLog == null)
				throw new ArgumentNullException("@eventLog");

			// do nothing
		}

		partial void OnPostDeleteEventLogHistory(IUnitOfWork unitOfWork, EventLogHistory @eventLogHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@eventLogHistory == null)
				throw new ArgumentNullException("@eventLogHistory");

			// do nothing
		}

		partial void OnPostDeleteFamily(IUnitOfWork unitOfWork, Family @family)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@family == null)
				throw new ArgumentNullException("@family");

			// do nothing
		}

		partial void OnPostDeleteFamilyHistory(IUnitOfWork unitOfWork, FamilyHistory @familyHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@familyHistory == null)
				throw new ArgumentNullException("@familyHistory");

			// do nothing
		}

		partial void OnPostDeleteNotifyMethodLookup(IUnitOfWork unitOfWork, NotifyMethodLookup @notifyMethodLookup)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@notifyMethodLookup == null)
				throw new ArgumentNullException("@notifyMethodLookup");

			// do nothing
		}

		partial void OnPostDeleteNotifyMethodLookupHistory(IUnitOfWork unitOfWork, NotifyMethodLookupHistory @notifyMethodLookupHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@notifyMethodLookupHistory == null)
				throw new ArgumentNullException("@notifyMethodLookupHistory");

			// do nothing
		}

		partial void OnPostDeleteParent(IUnitOfWork unitOfWork, Parent @parent)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@parent == null)
				throw new ArgumentNullException("@parent");

			// do nothing
		}

		partial void OnPostDeleteParentHistory(IUnitOfWork unitOfWork, ParentHistory @parentHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@parentHistory == null)
				throw new ArgumentNullException("@parentHistory");

			// do nothing
		}

		partial void OnPostDeletePropertyBag(IUnitOfWork unitOfWork, PropertyBag @propertyBag)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@propertyBag == null)
				throw new ArgumentNullException("@propertyBag");

			// do nothing
		}

		partial void OnPostDeletePropertyBagHistory(IUnitOfWork unitOfWork, PropertyBagHistory @propertyBagHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@propertyBagHistory == null)
				throw new ArgumentNullException("@propertyBagHistory");

			// do nothing
		}

		partial void OnPostDeleteSexLookup(IUnitOfWork unitOfWork, SexLookup @sexLookup)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@sexLookup == null)
				throw new ArgumentNullException("@sexLookup");

			// do nothing
		}

		partial void OnPostDeleteSexLookupHistory(IUnitOfWork unitOfWork, SexLookupHistory @sexLookupHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@sexLookupHistory == null)
				throw new ArgumentNullException("@sexLookupHistory");

			// do nothing
		}

		partial void OnPostDeleteUser(IUnitOfWork unitOfWork, User @user)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@user == null)
				throw new ArgumentNullException("@user");

			// do nothing
		}

		partial void OnPostDeleteUserHistory(IUnitOfWork unitOfWork, UserHistory @userHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@userHistory == null)
				throw new ArgumentNullException("@userHistory");

			// do nothing
		}

		partial void OnPostInsertChild(IUnitOfWork unitOfWork, Child @child)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@child == null)
				throw new ArgumentNullException("@child");

			// do nothing
		}

		partial void OnPostInsertChildHistory(IUnitOfWork unitOfWork, ChildHistory @childHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@childHistory == null)
				throw new ArgumentNullException("@childHistory");

			// do nothing
		}

		partial void OnPostInsertCircle(IUnitOfWork unitOfWork, Circle @circle)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@circle == null)
				throw new ArgumentNullException("@circle");

			// do nothing
		}

		partial void OnPostInsertCircleFamily(IUnitOfWork unitOfWork, CircleFamily @circleFamily)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@circleFamily == null)
				throw new ArgumentNullException("@circleFamily");

			// do nothing
		}

		partial void OnPostInsertCircleFamilyHistory(IUnitOfWork unitOfWork, CircleFamilyHistory @circleFamilyHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@circleFamilyHistory == null)
				throw new ArgumentNullException("@circleFamilyHistory");

			// do nothing
		}

		partial void OnPostInsertCircleHistory(IUnitOfWork unitOfWork, CircleHistory @circleHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@circleHistory == null)
				throw new ArgumentNullException("@circleHistory");

			// do nothing
		}

		partial void OnPostInsertEmailAttachment(IUnitOfWork unitOfWork, EmailAttachment @emailAttachment)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailAttachment == null)
				throw new ArgumentNullException("@emailAttachment");

			// do nothing
		}

		partial void OnPostInsertEmailAttachmentHistory(IUnitOfWork unitOfWork, EmailAttachmentHistory @emailAttachmentHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailAttachmentHistory == null)
				throw new ArgumentNullException("@emailAttachmentHistory");

			// do nothing
		}

		partial void OnPostInsertEmailMessage(IUnitOfWork unitOfWork, EmailMessage @emailMessage)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailMessage == null)
				throw new ArgumentNullException("@emailMessage");

			// do nothing
		}

		partial void OnPostInsertEmailMessageHistory(IUnitOfWork unitOfWork, EmailMessageHistory @emailMessageHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailMessageHistory == null)
				throw new ArgumentNullException("@emailMessageHistory");

			// do nothing
		}

		partial void OnPostInsertEventLog(IUnitOfWork unitOfWork, EventLog @eventLog)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@eventLog == null)
				throw new ArgumentNullException("@eventLog");

			// do nothing
		}

		partial void OnPostInsertEventLogHistory(IUnitOfWork unitOfWork, EventLogHistory @eventLogHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@eventLogHistory == null)
				throw new ArgumentNullException("@eventLogHistory");

			// do nothing
		}

		partial void OnPostInsertFamily(IUnitOfWork unitOfWork, Family @family)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@family == null)
				throw new ArgumentNullException("@family");

			// do nothing
		}

		partial void OnPostInsertFamilyHistory(IUnitOfWork unitOfWork, FamilyHistory @familyHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@familyHistory == null)
				throw new ArgumentNullException("@familyHistory");

			// do nothing
		}

		partial void OnPostInsertNotifyMethodLookup(IUnitOfWork unitOfWork, NotifyMethodLookup @notifyMethodLookup)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@notifyMethodLookup == null)
				throw new ArgumentNullException("@notifyMethodLookup");

			// do nothing
		}

		partial void OnPostInsertNotifyMethodLookupHistory(IUnitOfWork unitOfWork, NotifyMethodLookupHistory @notifyMethodLookupHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@notifyMethodLookupHistory == null)
				throw new ArgumentNullException("@notifyMethodLookupHistory");

			// do nothing
		}

		partial void OnPostInsertParent(IUnitOfWork unitOfWork, Parent @parent)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@parent == null)
				throw new ArgumentNullException("@parent");

			// do nothing
		}

		partial void OnPostInsertParentHistory(IUnitOfWork unitOfWork, ParentHistory @parentHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@parentHistory == null)
				throw new ArgumentNullException("@parentHistory");

			// do nothing
		}

		partial void OnPostInsertPropertyBag(IUnitOfWork unitOfWork, PropertyBag @propertyBag)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@propertyBag == null)
				throw new ArgumentNullException("@propertyBag");

			// do nothing
		}

		partial void OnPostInsertPropertyBagHistory(IUnitOfWork unitOfWork, PropertyBagHistory @propertyBagHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@propertyBagHistory == null)
				throw new ArgumentNullException("@propertyBagHistory");

			// do nothing
		}

		partial void OnPostInsertSexLookup(IUnitOfWork unitOfWork, SexLookup @sexLookup)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@sexLookup == null)
				throw new ArgumentNullException("@sexLookup");

			// do nothing
		}

		partial void OnPostInsertSexLookupHistory(IUnitOfWork unitOfWork, SexLookupHistory @sexLookupHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@sexLookupHistory == null)
				throw new ArgumentNullException("@sexLookupHistory");

			// do nothing
		}

		partial void OnPostInsertUser(IUnitOfWork unitOfWork, User @user)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@user == null)
				throw new ArgumentNullException("@user");

			// do nothing
		}

		partial void OnPostInsertUserHistory(IUnitOfWork unitOfWork, UserHistory @userHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@userHistory == null)
				throw new ArgumentNullException("@userHistory");

			// do nothing
		}

		partial void OnPostUpdateChild(IUnitOfWork unitOfWork, Child @child)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@child == null)
				throw new ArgumentNullException("@child");

			// do nothing
		}

		partial void OnPostUpdateChildHistory(IUnitOfWork unitOfWork, ChildHistory @childHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@childHistory == null)
				throw new ArgumentNullException("@childHistory");

			// do nothing
		}

		partial void OnPostUpdateCircle(IUnitOfWork unitOfWork, Circle @circle)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@circle == null)
				throw new ArgumentNullException("@circle");

			// do nothing
		}

		partial void OnPostUpdateCircleFamily(IUnitOfWork unitOfWork, CircleFamily @circleFamily)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@circleFamily == null)
				throw new ArgumentNullException("@circleFamily");

			// do nothing
		}

		partial void OnPostUpdateCircleFamilyHistory(IUnitOfWork unitOfWork, CircleFamilyHistory @circleFamilyHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@circleFamilyHistory == null)
				throw new ArgumentNullException("@circleFamilyHistory");

			// do nothing
		}

		partial void OnPostUpdateCircleHistory(IUnitOfWork unitOfWork, CircleHistory @circleHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@circleHistory == null)
				throw new ArgumentNullException("@circleHistory");

			// do nothing
		}

		partial void OnPostUpdateEmailAttachment(IUnitOfWork unitOfWork, EmailAttachment @emailAttachment)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailAttachment == null)
				throw new ArgumentNullException("@emailAttachment");

			// do nothing
		}

		partial void OnPostUpdateEmailAttachmentHistory(IUnitOfWork unitOfWork, EmailAttachmentHistory @emailAttachmentHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailAttachmentHistory == null)
				throw new ArgumentNullException("@emailAttachmentHistory");

			// do nothing
		}

		partial void OnPostUpdateEmailMessage(IUnitOfWork unitOfWork, EmailMessage @emailMessage)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailMessage == null)
				throw new ArgumentNullException("@emailMessage");

			// do nothing
		}

		partial void OnPostUpdateEmailMessageHistory(IUnitOfWork unitOfWork, EmailMessageHistory @emailMessageHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailMessageHistory == null)
				throw new ArgumentNullException("@emailMessageHistory");

			// do nothing
		}

		partial void OnPostUpdateEventLog(IUnitOfWork unitOfWork, EventLog @eventLog)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@eventLog == null)
				throw new ArgumentNullException("@eventLog");

			// do nothing
		}

		partial void OnPostUpdateEventLogHistory(IUnitOfWork unitOfWork, EventLogHistory @eventLogHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@eventLogHistory == null)
				throw new ArgumentNullException("@eventLogHistory");

			// do nothing
		}

		partial void OnPostUpdateFamily(IUnitOfWork unitOfWork, Family @family)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@family == null)
				throw new ArgumentNullException("@family");

			// do nothing
		}

		partial void OnPostUpdateFamilyHistory(IUnitOfWork unitOfWork, FamilyHistory @familyHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@familyHistory == null)
				throw new ArgumentNullException("@familyHistory");

			// do nothing
		}

		partial void OnPostUpdateNotifyMethodLookup(IUnitOfWork unitOfWork, NotifyMethodLookup @notifyMethodLookup)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@notifyMethodLookup == null)
				throw new ArgumentNullException("@notifyMethodLookup");

			// do nothing
		}

		partial void OnPostUpdateNotifyMethodLookupHistory(IUnitOfWork unitOfWork, NotifyMethodLookupHistory @notifyMethodLookupHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@notifyMethodLookupHistory == null)
				throw new ArgumentNullException("@notifyMethodLookupHistory");

			// do nothing
		}

		partial void OnPostUpdateParent(IUnitOfWork unitOfWork, Parent @parent)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@parent == null)
				throw new ArgumentNullException("@parent");

			// do nothing
		}

		partial void OnPostUpdateParentHistory(IUnitOfWork unitOfWork, ParentHistory @parentHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@parentHistory == null)
				throw new ArgumentNullException("@parentHistory");

			// do nothing
		}

		partial void OnPostUpdatePropertyBag(IUnitOfWork unitOfWork, PropertyBag @propertyBag)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@propertyBag == null)
				throw new ArgumentNullException("@propertyBag");

			// do nothing
		}

		partial void OnPostUpdatePropertyBagHistory(IUnitOfWork unitOfWork, PropertyBagHistory @propertyBagHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@propertyBagHistory == null)
				throw new ArgumentNullException("@propertyBagHistory");

			// do nothing
		}

		partial void OnPostUpdateSexLookup(IUnitOfWork unitOfWork, SexLookup @sexLookup)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@sexLookup == null)
				throw new ArgumentNullException("@sexLookup");

			// do nothing
		}

		partial void OnPostUpdateSexLookupHistory(IUnitOfWork unitOfWork, SexLookupHistory @sexLookupHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@sexLookupHistory == null)
				throw new ArgumentNullException("@sexLookupHistory");

			// do nothing
		}

		partial void OnPostUpdateUser(IUnitOfWork unitOfWork, User @user)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@user == null)
				throw new ArgumentNullException("@user");

			// do nothing
		}

		partial void OnPostUpdateUserHistory(IUnitOfWork unitOfWork, UserHistory @userHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@userHistory == null)
				throw new ArgumentNullException("@userHistory");

			// do nothing
		}

		partial void OnPreDeleteChild(IUnitOfWork unitOfWork, Child @child)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@child == null)
				throw new ArgumentNullException("@child");

			// do nothing
		}

		partial void OnPreDeleteChildHistory(IUnitOfWork unitOfWork, ChildHistory @childHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@childHistory == null)
				throw new ArgumentNullException("@childHistory");

			// do nothing
		}

		partial void OnPreDeleteCircle(IUnitOfWork unitOfWork, Circle @circle)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@circle == null)
				throw new ArgumentNullException("@circle");

			// do nothing
		}

		partial void OnPreDeleteCircleFamily(IUnitOfWork unitOfWork, CircleFamily @circleFamily)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@circleFamily == null)
				throw new ArgumentNullException("@circleFamily");

			// do nothing
		}

		partial void OnPreDeleteCircleFamilyHistory(IUnitOfWork unitOfWork, CircleFamilyHistory @circleFamilyHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@circleFamilyHistory == null)
				throw new ArgumentNullException("@circleFamilyHistory");

			// do nothing
		}

		partial void OnPreDeleteCircleHistory(IUnitOfWork unitOfWork, CircleHistory @circleHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@circleHistory == null)
				throw new ArgumentNullException("@circleHistory");

			// do nothing
		}

		partial void OnPreDeleteEmailAttachment(IUnitOfWork unitOfWork, EmailAttachment @emailAttachment)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailAttachment == null)
				throw new ArgumentNullException("@emailAttachment");

			// do nothing
		}

		partial void OnPreDeleteEmailAttachmentHistory(IUnitOfWork unitOfWork, EmailAttachmentHistory @emailAttachmentHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailAttachmentHistory == null)
				throw new ArgumentNullException("@emailAttachmentHistory");

			// do nothing
		}

		partial void OnPreDeleteEmailMessage(IUnitOfWork unitOfWork, EmailMessage @emailMessage)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailMessage == null)
				throw new ArgumentNullException("@emailMessage");

			// do nothing
		}

		partial void OnPreDeleteEmailMessageHistory(IUnitOfWork unitOfWork, EmailMessageHistory @emailMessageHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailMessageHistory == null)
				throw new ArgumentNullException("@emailMessageHistory");

			// do nothing
		}

		partial void OnPreDeleteEventLog(IUnitOfWork unitOfWork, EventLog @eventLog)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@eventLog == null)
				throw new ArgumentNullException("@eventLog");

			// do nothing
		}

		partial void OnPreDeleteEventLogHistory(IUnitOfWork unitOfWork, EventLogHistory @eventLogHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@eventLogHistory == null)
				throw new ArgumentNullException("@eventLogHistory");

			// do nothing
		}

		partial void OnPreDeleteFamily(IUnitOfWork unitOfWork, Family @family)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@family == null)
				throw new ArgumentNullException("@family");

			// do nothing
		}

		partial void OnPreDeleteFamilyHistory(IUnitOfWork unitOfWork, FamilyHistory @familyHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@familyHistory == null)
				throw new ArgumentNullException("@familyHistory");

			// do nothing
		}

		partial void OnPreDeleteNotifyMethodLookup(IUnitOfWork unitOfWork, NotifyMethodLookup @notifyMethodLookup)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@notifyMethodLookup == null)
				throw new ArgumentNullException("@notifyMethodLookup");

			// do nothing
		}

		partial void OnPreDeleteNotifyMethodLookupHistory(IUnitOfWork unitOfWork, NotifyMethodLookupHistory @notifyMethodLookupHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@notifyMethodLookupHistory == null)
				throw new ArgumentNullException("@notifyMethodLookupHistory");

			// do nothing
		}

		partial void OnPreDeleteParent(IUnitOfWork unitOfWork, Parent @parent)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@parent == null)
				throw new ArgumentNullException("@parent");

			// do nothing
		}

		partial void OnPreDeleteParentHistory(IUnitOfWork unitOfWork, ParentHistory @parentHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@parentHistory == null)
				throw new ArgumentNullException("@parentHistory");

			// do nothing
		}

		partial void OnPreDeletePropertyBag(IUnitOfWork unitOfWork, PropertyBag @propertyBag)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@propertyBag == null)
				throw new ArgumentNullException("@propertyBag");

			// do nothing
		}

		partial void OnPreDeletePropertyBagHistory(IUnitOfWork unitOfWork, PropertyBagHistory @propertyBagHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@propertyBagHistory == null)
				throw new ArgumentNullException("@propertyBagHistory");

			// do nothing
		}

		partial void OnPreDeleteSexLookup(IUnitOfWork unitOfWork, SexLookup @sexLookup)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@sexLookup == null)
				throw new ArgumentNullException("@sexLookup");

			// do nothing
		}

		partial void OnPreDeleteSexLookupHistory(IUnitOfWork unitOfWork, SexLookupHistory @sexLookupHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@sexLookupHistory == null)
				throw new ArgumentNullException("@sexLookupHistory");

			// do nothing
		}

		partial void OnPreDeleteUser(IUnitOfWork unitOfWork, User @user)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@user == null)
				throw new ArgumentNullException("@user");

			// do nothing
		}

		partial void OnPreDeleteUserHistory(IUnitOfWork unitOfWork, UserHistory @userHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@userHistory == null)
				throw new ArgumentNullException("@userHistory");

			// do nothing
		}

		partial void OnPreInsertChild(IUnitOfWork unitOfWork, Child @child)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@child == null)
				throw new ArgumentNullException("@child");

			@child.Mark();
		}

		partial void OnPreInsertChildHistory(IUnitOfWork unitOfWork, ChildHistory @childHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@childHistory == null)
				throw new ArgumentNullException("@childHistory");

			@childHistory.Mark();
		}

		partial void OnPreInsertCircle(IUnitOfWork unitOfWork, Circle @circle)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@circle == null)
				throw new ArgumentNullException("@circle");

			@circle.Mark();
		}

		partial void OnPreInsertCircleFamily(IUnitOfWork unitOfWork, CircleFamily @circleFamily)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@circleFamily == null)
				throw new ArgumentNullException("@circleFamily");

			@circleFamily.Mark();
		}

		partial void OnPreInsertCircleFamilyHistory(IUnitOfWork unitOfWork, CircleFamilyHistory @circleFamilyHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@circleFamilyHistory == null)
				throw new ArgumentNullException("@circleFamilyHistory");

			@circleFamilyHistory.Mark();
		}

		partial void OnPreInsertCircleHistory(IUnitOfWork unitOfWork, CircleHistory @circleHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@circleHistory == null)
				throw new ArgumentNullException("@circleHistory");

			@circleHistory.Mark();
		}

		partial void OnPreInsertEmailAttachment(IUnitOfWork unitOfWork, EmailAttachment @emailAttachment)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailAttachment == null)
				throw new ArgumentNullException("@emailAttachment");

			@emailAttachment.Mark();
		}

		partial void OnPreInsertEmailAttachmentHistory(IUnitOfWork unitOfWork, EmailAttachmentHistory @emailAttachmentHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailAttachmentHistory == null)
				throw new ArgumentNullException("@emailAttachmentHistory");

			@emailAttachmentHistory.Mark();
		}

		partial void OnPreInsertEmailMessage(IUnitOfWork unitOfWork, EmailMessage @emailMessage)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailMessage == null)
				throw new ArgumentNullException("@emailMessage");

			@emailMessage.Mark();
		}

		partial void OnPreInsertEmailMessageHistory(IUnitOfWork unitOfWork, EmailMessageHistory @emailMessageHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailMessageHistory == null)
				throw new ArgumentNullException("@emailMessageHistory");

			@emailMessageHistory.Mark();
		}

		partial void OnPreInsertEventLog(IUnitOfWork unitOfWork, EventLog @eventLog)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@eventLog == null)
				throw new ArgumentNullException("@eventLog");

			@eventLog.Mark();
		}

		partial void OnPreInsertEventLogHistory(IUnitOfWork unitOfWork, EventLogHistory @eventLogHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@eventLogHistory == null)
				throw new ArgumentNullException("@eventLogHistory");

			@eventLogHistory.Mark();
		}

		partial void OnPreInsertFamily(IUnitOfWork unitOfWork, Family @family)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@family == null)
				throw new ArgumentNullException("@family");

			@family.Mark();
		}

		partial void OnPreInsertFamilyHistory(IUnitOfWork unitOfWork, FamilyHistory @familyHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@familyHistory == null)
				throw new ArgumentNullException("@familyHistory");

			@familyHistory.Mark();
		}

		partial void OnPreInsertNotifyMethodLookup(IUnitOfWork unitOfWork, NotifyMethodLookup @notifyMethodLookup)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@notifyMethodLookup == null)
				throw new ArgumentNullException("@notifyMethodLookup");

			@notifyMethodLookup.Mark();
		}

		partial void OnPreInsertNotifyMethodLookupHistory(IUnitOfWork unitOfWork, NotifyMethodLookupHistory @notifyMethodLookupHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@notifyMethodLookupHistory == null)
				throw new ArgumentNullException("@notifyMethodLookupHistory");

			@notifyMethodLookupHistory.Mark();
		}

		partial void OnPreInsertParent(IUnitOfWork unitOfWork, Parent @parent)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@parent == null)
				throw new ArgumentNullException("@parent");

			@parent.Mark();
		}

		partial void OnPreInsertParentHistory(IUnitOfWork unitOfWork, ParentHistory @parentHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@parentHistory == null)
				throw new ArgumentNullException("@parentHistory");

			@parentHistory.Mark();
		}

		partial void OnPreInsertPropertyBag(IUnitOfWork unitOfWork, PropertyBag @propertyBag)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@propertyBag == null)
				throw new ArgumentNullException("@propertyBag");

			@propertyBag.Mark();
		}

		partial void OnPreInsertPropertyBagHistory(IUnitOfWork unitOfWork, PropertyBagHistory @propertyBagHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@propertyBagHistory == null)
				throw new ArgumentNullException("@propertyBagHistory");

			@propertyBagHistory.Mark();
		}

		partial void OnPreInsertSexLookup(IUnitOfWork unitOfWork, SexLookup @sexLookup)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@sexLookup == null)
				throw new ArgumentNullException("@sexLookup");

			@sexLookup.Mark();
		}

		partial void OnPreInsertSexLookupHistory(IUnitOfWork unitOfWork, SexLookupHistory @sexLookupHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@sexLookupHistory == null)
				throw new ArgumentNullException("@sexLookupHistory");

			@sexLookupHistory.Mark();
		}

		partial void OnPreInsertUser(IUnitOfWork unitOfWork, User @user)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@user == null)
				throw new ArgumentNullException("@user");

			@user.Mark();
		}

		partial void OnPreInsertUserHistory(IUnitOfWork unitOfWork, UserHistory @userHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@userHistory == null)
				throw new ArgumentNullException("@userHistory");

			@userHistory.Mark();
		}

		partial void OnPreUpdateChild(IUnitOfWork unitOfWork, Child @child)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@child == null)
				throw new ArgumentNullException("@child");

			@child.Mark();
		}

		partial void OnPreUpdateChildHistory(IUnitOfWork unitOfWork, ChildHistory @childHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@childHistory == null)
				throw new ArgumentNullException("@childHistory");

			@childHistory.Mark();
		}

		partial void OnPreUpdateCircle(IUnitOfWork unitOfWork, Circle @circle)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@circle == null)
				throw new ArgumentNullException("@circle");

			@circle.Mark();
		}

		partial void OnPreUpdateCircleFamily(IUnitOfWork unitOfWork, CircleFamily @circleFamily)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@circleFamily == null)
				throw new ArgumentNullException("@circleFamily");

			@circleFamily.Mark();
		}

		partial void OnPreUpdateCircleFamilyHistory(IUnitOfWork unitOfWork, CircleFamilyHistory @circleFamilyHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@circleFamilyHistory == null)
				throw new ArgumentNullException("@circleFamilyHistory");

			@circleFamilyHistory.Mark();
		}

		partial void OnPreUpdateCircleHistory(IUnitOfWork unitOfWork, CircleHistory @circleHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@circleHistory == null)
				throw new ArgumentNullException("@circleHistory");

			@circleHistory.Mark();
		}

		partial void OnPreUpdateEmailAttachment(IUnitOfWork unitOfWork, EmailAttachment @emailAttachment)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailAttachment == null)
				throw new ArgumentNullException("@emailAttachment");

			@emailAttachment.Mark();
		}

		partial void OnPreUpdateEmailAttachmentHistory(IUnitOfWork unitOfWork, EmailAttachmentHistory @emailAttachmentHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailAttachmentHistory == null)
				throw new ArgumentNullException("@emailAttachmentHistory");

			@emailAttachmentHistory.Mark();
		}

		partial void OnPreUpdateEmailMessage(IUnitOfWork unitOfWork, EmailMessage @emailMessage)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailMessage == null)
				throw new ArgumentNullException("@emailMessage");

			@emailMessage.Mark();
		}

		partial void OnPreUpdateEmailMessageHistory(IUnitOfWork unitOfWork, EmailMessageHistory @emailMessageHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailMessageHistory == null)
				throw new ArgumentNullException("@emailMessageHistory");

			@emailMessageHistory.Mark();
		}

		partial void OnPreUpdateEventLog(IUnitOfWork unitOfWork, EventLog @eventLog)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@eventLog == null)
				throw new ArgumentNullException("@eventLog");

			@eventLog.Mark();
		}

		partial void OnPreUpdateEventLogHistory(IUnitOfWork unitOfWork, EventLogHistory @eventLogHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@eventLogHistory == null)
				throw new ArgumentNullException("@eventLogHistory");

			@eventLogHistory.Mark();
		}

		partial void OnPreUpdateFamily(IUnitOfWork unitOfWork, Family @family)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@family == null)
				throw new ArgumentNullException("@family");

			@family.Mark();
		}

		partial void OnPreUpdateFamilyHistory(IUnitOfWork unitOfWork, FamilyHistory @familyHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@familyHistory == null)
				throw new ArgumentNullException("@familyHistory");

			@familyHistory.Mark();
		}

		partial void OnPreUpdateNotifyMethodLookup(IUnitOfWork unitOfWork, NotifyMethodLookup @notifyMethodLookup)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@notifyMethodLookup == null)
				throw new ArgumentNullException("@notifyMethodLookup");

			@notifyMethodLookup.Mark();
		}

		partial void OnPreUpdateNotifyMethodLookupHistory(IUnitOfWork unitOfWork, NotifyMethodLookupHistory @notifyMethodLookupHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@notifyMethodLookupHistory == null)
				throw new ArgumentNullException("@notifyMethodLookupHistory");

			@notifyMethodLookupHistory.Mark();
		}

		partial void OnPreUpdateParent(IUnitOfWork unitOfWork, Parent @parent)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@parent == null)
				throw new ArgumentNullException("@parent");

			@parent.Mark();
		}

		partial void OnPreUpdateParentHistory(IUnitOfWork unitOfWork, ParentHistory @parentHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@parentHistory == null)
				throw new ArgumentNullException("@parentHistory");

			@parentHistory.Mark();
		}

		partial void OnPreUpdatePropertyBag(IUnitOfWork unitOfWork, PropertyBag @propertyBag)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@propertyBag == null)
				throw new ArgumentNullException("@propertyBag");

			@propertyBag.Mark();
		}

		partial void OnPreUpdatePropertyBagHistory(IUnitOfWork unitOfWork, PropertyBagHistory @propertyBagHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@propertyBagHistory == null)
				throw new ArgumentNullException("@propertyBagHistory");

			@propertyBagHistory.Mark();
		}

		partial void OnPreUpdateSexLookup(IUnitOfWork unitOfWork, SexLookup @sexLookup)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@sexLookup == null)
				throw new ArgumentNullException("@sexLookup");

			@sexLookup.Mark();
		}

		partial void OnPreUpdateSexLookupHistory(IUnitOfWork unitOfWork, SexLookupHistory @sexLookupHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@sexLookupHistory == null)
				throw new ArgumentNullException("@sexLookupHistory");

			@sexLookupHistory.Mark();
		}

		partial void OnPreUpdateUser(IUnitOfWork unitOfWork, User @user)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@user == null)
				throw new ArgumentNullException("@user");

			@user.Mark();
		}

		partial void OnPreUpdateUserHistory(IUnitOfWork unitOfWork, UserHistory @userHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@userHistory == null)
				throw new ArgumentNullException("@userHistory");

			@userHistory.Mark();
		}

		partial void OnSaveConflictChild(IUnitOfWork unitOfWork, Child @child)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@child == null)
				throw new ArgumentNullException("@child");

			// do nothing
		}

		partial void OnSaveConflictChildHistory(IUnitOfWork unitOfWork, ChildHistory @childHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@childHistory == null)
				throw new ArgumentNullException("@childHistory");

			// do nothing
		}

		partial void OnSaveConflictCircle(IUnitOfWork unitOfWork, Circle @circle)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@circle == null)
				throw new ArgumentNullException("@circle");

			// do nothing
		}

		partial void OnSaveConflictCircleFamily(IUnitOfWork unitOfWork, CircleFamily @circleFamily)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@circleFamily == null)
				throw new ArgumentNullException("@circleFamily");

			// do nothing
		}

		partial void OnSaveConflictCircleFamilyHistory(IUnitOfWork unitOfWork, CircleFamilyHistory @circleFamilyHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@circleFamilyHistory == null)
				throw new ArgumentNullException("@circleFamilyHistory");

			// do nothing
		}

		partial void OnSaveConflictCircleHistory(IUnitOfWork unitOfWork, CircleHistory @circleHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@circleHistory == null)
				throw new ArgumentNullException("@circleHistory");

			// do nothing
		}

		partial void OnSaveConflictEmailAttachment(IUnitOfWork unitOfWork, EmailAttachment @emailAttachment)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailAttachment == null)
				throw new ArgumentNullException("@emailAttachment");

			// do nothing
		}

		partial void OnSaveConflictEmailAttachmentHistory(IUnitOfWork unitOfWork, EmailAttachmentHistory @emailAttachmentHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailAttachmentHistory == null)
				throw new ArgumentNullException("@emailAttachmentHistory");

			// do nothing
		}

		partial void OnSaveConflictEmailMessage(IUnitOfWork unitOfWork, EmailMessage @emailMessage)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailMessage == null)
				throw new ArgumentNullException("@emailMessage");

			// do nothing
		}

		partial void OnSaveConflictEmailMessageHistory(IUnitOfWork unitOfWork, EmailMessageHistory @emailMessageHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailMessageHistory == null)
				throw new ArgumentNullException("@emailMessageHistory");

			// do nothing
		}

		partial void OnSaveConflictEventLog(IUnitOfWork unitOfWork, EventLog @eventLog)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@eventLog == null)
				throw new ArgumentNullException("@eventLog");

			// do nothing
		}

		partial void OnSaveConflictEventLogHistory(IUnitOfWork unitOfWork, EventLogHistory @eventLogHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@eventLogHistory == null)
				throw new ArgumentNullException("@eventLogHistory");

			// do nothing
		}

		partial void OnSaveConflictFamily(IUnitOfWork unitOfWork, Family @family)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@family == null)
				throw new ArgumentNullException("@family");

			// do nothing
		}

		partial void OnSaveConflictFamilyHistory(IUnitOfWork unitOfWork, FamilyHistory @familyHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@familyHistory == null)
				throw new ArgumentNullException("@familyHistory");

			// do nothing
		}

		partial void OnSaveConflictNotifyMethodLookup(IUnitOfWork unitOfWork, NotifyMethodLookup @notifyMethodLookup)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@notifyMethodLookup == null)
				throw new ArgumentNullException("@notifyMethodLookup");

			// do nothing
		}

		partial void OnSaveConflictNotifyMethodLookupHistory(IUnitOfWork unitOfWork, NotifyMethodLookupHistory @notifyMethodLookupHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@notifyMethodLookupHistory == null)
				throw new ArgumentNullException("@notifyMethodLookupHistory");

			// do nothing
		}

		partial void OnSaveConflictParent(IUnitOfWork unitOfWork, Parent @parent)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@parent == null)
				throw new ArgumentNullException("@parent");

			// do nothing
		}

		partial void OnSaveConflictParentHistory(IUnitOfWork unitOfWork, ParentHistory @parentHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@parentHistory == null)
				throw new ArgumentNullException("@parentHistory");

			// do nothing
		}

		partial void OnSaveConflictPropertyBag(IUnitOfWork unitOfWork, PropertyBag @propertyBag)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@propertyBag == null)
				throw new ArgumentNullException("@propertyBag");

			// do nothing
		}

		partial void OnSaveConflictPropertyBagHistory(IUnitOfWork unitOfWork, PropertyBagHistory @propertyBagHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@propertyBagHistory == null)
				throw new ArgumentNullException("@propertyBagHistory");

			// do nothing
		}

		partial void OnSaveConflictSexLookup(IUnitOfWork unitOfWork, SexLookup @sexLookup)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@sexLookup == null)
				throw new ArgumentNullException("@sexLookup");

			// do nothing
		}

		partial void OnSaveConflictSexLookupHistory(IUnitOfWork unitOfWork, SexLookupHistory @sexLookupHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@sexLookupHistory == null)
				throw new ArgumentNullException("@sexLookupHistory");

			// do nothing
		}

		partial void OnSaveConflictUser(IUnitOfWork unitOfWork, User @user)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@user == null)
				throw new ArgumentNullException("@user");

			// do nothing
		}

		partial void OnSaveConflictUserHistory(IUnitOfWork unitOfWork, UserHistory @userHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@userHistory == null)
				throw new ArgumentNullException("@userHistory");

			// do nothing
		}

		partial void OnSelectChild(IUnitOfWork unitOfWork, Child @child)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@child == null)
				throw new ArgumentNullException("@child");

			// do nothing
		}

		partial void OnSelectChildHistory(IUnitOfWork unitOfWork, ChildHistory @childHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@childHistory == null)
				throw new ArgumentNullException("@childHistory");

			// do nothing
		}

		partial void OnSelectCircle(IUnitOfWork unitOfWork, Circle @circle)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@circle == null)
				throw new ArgumentNullException("@circle");

			// do nothing
		}

		partial void OnSelectCircleFamily(IUnitOfWork unitOfWork, CircleFamily @circleFamily)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@circleFamily == null)
				throw new ArgumentNullException("@circleFamily");

			// do nothing
		}

		partial void OnSelectCircleFamilyHistory(IUnitOfWork unitOfWork, CircleFamilyHistory @circleFamilyHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@circleFamilyHistory == null)
				throw new ArgumentNullException("@circleFamilyHistory");

			// do nothing
		}

		partial void OnSelectCircleHistory(IUnitOfWork unitOfWork, CircleHistory @circleHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@circleHistory == null)
				throw new ArgumentNullException("@circleHistory");

			// do nothing
		}

		partial void OnSelectEmailAttachment(IUnitOfWork unitOfWork, EmailAttachment @emailAttachment)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailAttachment == null)
				throw new ArgumentNullException("@emailAttachment");

			// do nothing
		}

		partial void OnSelectEmailAttachmentHistory(IUnitOfWork unitOfWork, EmailAttachmentHistory @emailAttachmentHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailAttachmentHistory == null)
				throw new ArgumentNullException("@emailAttachmentHistory");

			// do nothing
		}

		partial void OnSelectEmailMessage(IUnitOfWork unitOfWork, EmailMessage @emailMessage)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailMessage == null)
				throw new ArgumentNullException("@emailMessage");

			// do nothing
		}

		partial void OnSelectEmailMessageHistory(IUnitOfWork unitOfWork, EmailMessageHistory @emailMessageHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@emailMessageHistory == null)
				throw new ArgumentNullException("@emailMessageHistory");

			// do nothing
		}

		partial void OnSelectEventLog(IUnitOfWork unitOfWork, EventLog @eventLog)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@eventLog == null)
				throw new ArgumentNullException("@eventLog");

			// do nothing
		}

		partial void OnSelectEventLogHistory(IUnitOfWork unitOfWork, EventLogHistory @eventLogHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@eventLogHistory == null)
				throw new ArgumentNullException("@eventLogHistory");

			// do nothing
		}

		partial void OnSelectFamily(IUnitOfWork unitOfWork, Family @family)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@family == null)
				throw new ArgumentNullException("@family");

			// do nothing
		}

		partial void OnSelectFamilyHistory(IUnitOfWork unitOfWork, FamilyHistory @familyHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@familyHistory == null)
				throw new ArgumentNullException("@familyHistory");

			// do nothing
		}

		partial void OnSelectNotifyMethodLookup(IUnitOfWork unitOfWork, NotifyMethodLookup @notifyMethodLookup)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@notifyMethodLookup == null)
				throw new ArgumentNullException("@notifyMethodLookup");

			// do nothing
		}

		partial void OnSelectNotifyMethodLookupHistory(IUnitOfWork unitOfWork, NotifyMethodLookupHistory @notifyMethodLookupHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@notifyMethodLookupHistory == null)
				throw new ArgumentNullException("@notifyMethodLookupHistory");

			// do nothing
		}

		partial void OnSelectParent(IUnitOfWork unitOfWork, Parent @parent)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@parent == null)
				throw new ArgumentNullException("@parent");

			// do nothing
		}

		partial void OnSelectParentHistory(IUnitOfWork unitOfWork, ParentHistory @parentHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@parentHistory == null)
				throw new ArgumentNullException("@parentHistory");

			// do nothing
		}

		partial void OnSelectPropertyBag(IUnitOfWork unitOfWork, PropertyBag @propertyBag)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@propertyBag == null)
				throw new ArgumentNullException("@propertyBag");

			// do nothing
		}

		partial void OnSelectPropertyBagHistory(IUnitOfWork unitOfWork, PropertyBagHistory @propertyBagHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@propertyBagHistory == null)
				throw new ArgumentNullException("@propertyBagHistory");

			// do nothing
		}

		partial void OnSelectSexLookup(IUnitOfWork unitOfWork, SexLookup @sexLookup)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@sexLookup == null)
				throw new ArgumentNullException("@sexLookup");

			// do nothing
		}

		partial void OnSelectSexLookupHistory(IUnitOfWork unitOfWork, SexLookupHistory @sexLookupHistory)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@sexLookupHistory == null)
				throw new ArgumentNullException("@sexLookupHistory");

			// do nothing
		}

		partial void OnSelectUser(IUnitOfWork unitOfWork, User @user)
		{
			if ((object)unitOfWork == null)
				throw new ArgumentNullException("unitOfWork");

			if ((object)@user == null)
				throw new ArgumentNullException("@user");

			// do nothing
		}

		partial void OnSelectUserHistory(IUnitOfWork unitOfWork, UserHistory @userHistory)
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