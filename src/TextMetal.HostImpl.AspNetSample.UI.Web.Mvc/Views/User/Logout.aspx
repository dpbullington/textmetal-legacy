<%@ Page Title="Member Logout" MasterPageFile="~/Views/Shared/SiteBase.Master" Language="C#" Inherits="System.Web.Mvc.ViewPage<TextMetal.HostImpl.AspNetSample.UI.Web.Mvc.Models.UserViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MiddleContent" runat="server">

	<p>Would you like to proceed with logout?</p>

	<% using (this.Html.BeginForm())
	   { %>
		<%= this.Html.ValidationSummary(true) %>
		<div class="txtmtl-data-form">
			<div class="txtmtl-data-button">
				<input type="submit" class="txtmtl-input-button" value="Logout" id="Logout" />
			</div>
		</div>
	<% } %>

	<div class="txtmtl-ctxnav-wrapper">
		<%= this.Html.ActionLink("Return to home", "Index", "Welcome") %>
	</div>

</asp:Content>
