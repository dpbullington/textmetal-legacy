<%@ Page Title="Edit an Organization" MasterPageFile="~/Views/Shared/SiteBase.Master" Language="C#" Inherits="System.Web.Mvc.ViewPage<TextMetal.HostImpl.AspNetSample.DomainModel.Tables.Organization>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MiddleContent" runat="server">
	
	<p>Editing an existing organization requires the following information.</p>

	<% using (this.Html.BeginForm())
	   { %>
		<%= this.Html.ValidationSummary(true) %>
		<div class="txtmtl-data-form">
			
			<div class="txtmtl-data-label">
				Record #
			</div>
			<div class="txtmtl-data-field">
				<%= this.Html.TextBox("OrganizationId", String.Format("{0:00000000}", this.Model.OrganizationId), new { @readonly = "readonly", @class = "txtmtl-input-text" }) %>
				<%= this.Html.ValidationMessage("OrganizationId") %>
			</div>

			<div class="txtmtl-data-label">
				Organization Name
			</div>
			<div class="txtmtl-data-field">
				<%= this.Html.TextBox("OrganizationName", null, new { @class = "txtmtl-input-text" }) %>
				<%= this.Html.ValidationMessage("OrganizationName") %>
			</div>
			
			<div class="txtmtl-data-button">
				<input type="submit" class="txtmtl-input-button" value="Save" />
			</div>
		</div>
	<% } %>
	
	<div class="txtmtl-ctxnav-wrapper">
		<%= this.Html.ActionLink("Return to organization dashboard", "Dashboard", "Organization") %>
	</div>
</asp:Content>