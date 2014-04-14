/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

namespace TextMetal.HostImpl.AspNetSample.ServiceModel.Member
{
	public interface IMemberService
	{
		#region Methods/Operators

		CreateMemberResponse CreateMember(CreateMemberRequest request);

		EditMemberResponse EditMember(EditMemberRequest request);

		ListMembersResponse ListMembers(ListMembersRequest request);

		LoadMemberResponse LoadMember(LoadMemberRequest request);

		RemoveMemberResponse RemoveMember(RemoveMemberRequest request);

		#endregion
	}
}