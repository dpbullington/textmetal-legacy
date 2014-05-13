<%@ Page Title="Organization Dashboard" MasterPageFile="~/Views/Shared/SiteBase.Master" Language="C#" Inherits="System.Web.Mvc.ViewPage<TextMetal.HostImpl.AspNetSample.DomainModel.Tables.Organization>" %>
<%@ Import Namespace="TextMetal.Common.Core" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MiddleContent" runat="server">

	<div class="txtmtl-organization-wrapper">
		<div class="txtmtl-organization-logo">
			<img src="<%= this.Url.Content("~/images/organization.png") %>" />
		</div>
		<span class="txtmtl-organization-name"><%= this.Html.Encode(this.Model.OrganizationName.SafeToString(null, "<unknown>", true)) %></span><br />
		Record #<%= this.Html.Encode(this.Model.OrganizationId.SafeToString("00000000", "<unknown>")) %><br />

		<span class="txtmtl-organization-subtext">Created on: <%= this.Html.Encode(this.Model.CreationTimestamp.SafeToString("MM/dd/yyyy hh:mm:ss UTC", "<unknown>")) %></span><br />
		<span class="txtmtl-organization-subtext">Modified on: <%= this.Html.Encode(this.Model.ModificationTimestamp.SafeToString("MM/dd/yyyy hh:mm:ss UTC", "<unknown>")) %></span>
	</div>

	<div class="txtmtl-ctxnav-wrapper">		
		<%= this.Html.ActionLink("Edit this organization", "Edit", "Organization") %><br />
		<%= this.Html.ActionLink("Delete this organization", "Delete", "Organization") %><br />		
		<%= this.Html.ActionLink("Manage members for this organization", "List", "Member") %><br />
		
	</div>
</asp:Content>