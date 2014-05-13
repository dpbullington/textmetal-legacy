/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

namespace TextMetal.HostImpl.AspNetSample.ServiceModel.Organization
{
	public interface IOrganizationService
	{
		#region Methods/Operators

		CreateOrganizationResponse CreateOrganization(CreateOrganizationRequest request);

		EditOrganizationResponse EditOrganization(EditOrganizationRequest request);

		ListOrganizationResponse ListOrganization(ListOrganizationRequest request);

		LoadOrganizationResponse LoadOrganization(LoadOrganizationRequest request);

		RemoveOrganizationResponse RemoveOrganization(RemoveOrganizationRequest request);

		#endregion
	}
}