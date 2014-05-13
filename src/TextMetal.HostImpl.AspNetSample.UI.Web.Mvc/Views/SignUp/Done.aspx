<%@ Page Title="Signing Up: Done" MasterPageFile="~/Views/Shared/SiteBase.Master" Language="C#" Inherits="System.Web.Mvc.ViewPage<TextMetal.HostImpl.AspNetSample.UI.Web.Mvc.Models.SignUpViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MiddleContent" runat="server">

	<p style="margin-top: 32px" class="txtmtl-callout-text">TextMetal sign-up complete!</p>

	<div class="txtmtl-ctxnav-wrapper">
		<%= this.Html.ActionLink("Go to organization", "Dashboard", "Organization") %>
	</div>

</asp:Content>