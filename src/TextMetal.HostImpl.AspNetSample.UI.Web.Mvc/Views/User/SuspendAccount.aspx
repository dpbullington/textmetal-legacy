<%@ Page Title="Suspend User Account" MasterPageFile="~/Views/Shared/SiteBase.Master" Language="C#" Inherits="System.Web.Mvc.ViewPage<TextMetal.HostImpl.AspNetSample.UI.Web.Mvc.Models.UserViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MiddleContent" runat="server">

	<p>If you choose to suspend this user account, it is a permanent operation.</p>

	<% using (this.Html.BeginForm())
	   { %>
		<%= this.Html.ValidationSummary(true) %>
		<div class="txtmtl-data-form">
			<div class="txtmtl-data-label">
				<%= this.Html.LabelFor(m => m.PasswordClearText, "Password") %>
			</div>
			<div class="txtmtl-data-field">
				<%= this.Html.PasswordFor(m => m.PasswordClearText, new { @class = "txtmtl-input-text" }) %>
				<%= this.Html.ValidationMessageFor(m => m.PasswordClearText) %>
			</div>
			
			<div class="txtmtl-data-label">
				<%= this.Html.LabelFor(m => m.PasswordClearTextConfirm, "Password (confirm)") %>
			</div>
			<div class="txtmtl-data-field">
				<%= this.Html.PasswordFor(m => m.PasswordClearTextConfirm, new { @class = "txtmtl-input-text" }) %>
				<%= this.Html.ValidationMessageFor(m => m.PasswordClearTextConfirm) %>
			</div>

			<div class="txtmtl-data-button">
				<input type="submit" class="txtmtl-input-button" value="Proceed" id="Proceed" />
				
				<span><%= this.Html.CheckBoxFor(m => m.SuspendAccount, new { @class = "txtmtl-input-checkbox" }) %> <%= this.Html.LabelFor(m => m.SuspendAccount, "Suspend account?") %><%= this.Html.ValidationMessageFor(m => m.SuspendAccount) %></span>
			</div>
		</div>
	<% } %>

	<div class="txtmtl-ctxnav-wrapper">
		<%= this.Html.ActionLink("Return to home", "Index", "Welcome") %>
	</div>
</asp:Content>