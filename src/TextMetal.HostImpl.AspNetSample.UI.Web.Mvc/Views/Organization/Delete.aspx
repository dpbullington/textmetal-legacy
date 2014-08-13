<%@ Page Title="Delete this Organization" MasterPageFile="~/Views/Shared/SiteBase.Master" Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MiddleContent" runat="server">

	<p>If you choose to delete this organization, it is a permanent operation.</p>

	<% using (this.Html.BeginForm())
	   { %>
		<%= this.Html.ValidationSummary(true) %>
		<div class="txtmtl-data-form">
			<div class="txtmtl-data-button">
				<input type="submit" class="txtmtl-input-button" value="Proceed" id="Proceed" onclick=" javascript: return confirm('Are you sure you want to delete this organization?'); " />
			</div>
		</div>
	<% } %>

	<div class="txtmtl-ctxnav-wrapper">
		<%= this.Html.ActionLink("Return to organization dashboard", "Dashboard", "Organization") %>
	</div>
</asp:Content>
