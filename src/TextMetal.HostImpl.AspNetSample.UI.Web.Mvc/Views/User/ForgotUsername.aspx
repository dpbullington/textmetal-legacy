<%@ Page Title="Forgot Username" MasterPageFile="~/Views/Shared/SiteBase.Master" Language="C#" Inherits="System.Web.Mvc.ViewPage<TextMetal.HostImpl.AspNetSample.UI.Web.Mvc.Models.UserViewModel>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MiddleContent" runat="server">

	<script type="text/javascript">
		$(document).ready(function() {
			$("#EmailAddress").focus();
		});
	</script>

	<p>Please enter the following information in order to have your username sent to your registered email address.</p>

	<% using (this.Html.BeginForm())
	   { %>
		<%= this.Html.ValidationSummary(true) %>
		<div class="txtmtl-data-form">

			<div class="txtmtl-data-label">
				<%= this.Html.LabelFor(m => m.EmailAddress, "Email Address") %>
			</div>
			<div class="txtmtl-data-field">
				<%= this.Html.TextBoxFor(m => m.EmailAddress, new { @class = "txtmtl-input-text" }) %>
				<%= this.Html.ValidationMessageFor(m => m.EmailAddress) %>
			</div>

			<div class="txtmtl-data-label">
				<%= this.Html.LabelFor(m => m.EmailAddressConfirm, "Email Address (confirm)") %>
			</div>
			<div class="txtmtl-data-field">
				<%= this.Html.TextBoxFor(m => m.EmailAddressConfirm, new { @class = "txtmtl-input-text" }) %>
				<%= this.Html.ValidationMessageFor(m => m.EmailAddressConfirm) %>
			</div>

			<div class="txtmtl-data-button">
				<input type="submit" class="txtmtl-input-button" value="Submit" id="Submit" />
			</div>
		</div>
	<% } %>

	<div class="txtmtl-ctxnav-wrapper">
		<%= this.Html.ActionLink("Forgot password", "ForgotPassword", "User") %>
	</div>
</asp:Content>
